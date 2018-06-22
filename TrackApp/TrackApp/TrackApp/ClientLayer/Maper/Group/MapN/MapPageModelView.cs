using Amazon.Runtime;
using Plugin.Geolocator;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using TK.CustomMap;
using TK.CustomMap.Overlays;
using TrackApp.ClientLayer.CustomUI;
using TrackApp.ClientLayer.Maper.MapThread;
using TrackApp.DataFormat.UserData;
using TrackApp.ServerLayer.Query;
using Xamarin.Forms;

namespace TrackApp.ClientLayer.Maper.Group.MapN
{
    public class MapPageModelView : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public const double METERS_ERROR = 1000;
        public static readonly double LATITUDE_ACCEPTED_ERROR = METERS_ERROR / 111320;
        public static readonly double LONGITUDE_ACCEPTED_ERROR = 40075000 * Math.Cos(LATITUDE_ACCEPTED_ERROR) / 360;

        public const int LINE_DISPLAY_LOOPER = 6;

        public static readonly string NO_POS_ALERT_TEXT = "Nu a fost gasita nici o locatie in ultimele {0} ore pentru {1}";

        private ObservableCollection<TKCircle> _userCurrentPositions;
        public ObservableCollection<TKCircle> UserCurrentPositions
        {
            get => _userCurrentPositions;
            set
            {
                _userCurrentPositions = value;
                OnPropertyChanged("UserCurrentPositions");
            }
        }

        private ObservableCollection<TKCustomMapPin> _pins;
        public ObservableCollection<TKCustomMapPin> Pins
        {
            get => _pins;
            set
            {
                _pins = value;
                OnPropertyChanged("Pins");
            }
        }

        private MapSpan _mapRegion;
        public MapSpan MapRegion
        {
            get => _mapRegion;
            set
            {
                _mapRegion = value;
                OnPropertyChanged("MapRegion");
            }
        }

        private MapUserGeneratorThread[] generators;
        private Task[] generatorTasks;
        private RoledTrackUser[] users;
        private TKCustomMap map;
        private string groupName;
        private RoledTrackUser currentUser;

        public MapPageModelView(RoledTrackUser[] users, TKCustomMap map, string groupName, RoledTrackUser currentUser)
        {
           // prepare memory for the circles so every circle will have it's index(part of memory) when the tasks will start
            TKCircle[] circles = users != null ? new TKCircle[users.Length] : new TKCircle[0];
            for (int i = 0; i < circles.Length; i++)
                circles[i] = new TKCircle
                {
                    Center = MapPage.DEFAULT_MAP_POSITION,
                    Color = Color.Transparent,
                    StrokeColor = Color.White,
                    StrokeWidth = ClientConsts.CIRCLE_STROKE_WIDTH,
                    Radius = 1
                };

            // Bindings have to be initialized 
            UserCurrentPositions = new ObservableCollection<TKCircle>(circles);
            MapRegion = MapSpan.FromCenterAndRadius(MapPage.DEFAULT_MAP_POSITION, Distance.FromMiles(ClientConsts.FROM_KM_MAP_DISTANCE));

            this.users = users;
            this.map = map;
            this.groupName = groupName;
            this.generators = new MapUserGeneratorThread[users.Length];
            this.generatorTasks = new Task[users.Length];
            this.currentUser = currentUser;

            Array.Sort(users, (a, b) => a.Username.CompareTo(b.Username)); // map colours to users by sorting them by username
        }

        public void StartGenerators(bool singleDriver)
        {
            Array.Sort(users, (a, b) => a.Username.CompareTo(b.Username)); // map colours to users by sorting them by username

            for (int i = 0; i < users.Length; i++)
            {
                var generator = new MapUserGeneratorThread(users[i].Username, singleDriver, this, i, ClientConsts.Colours[i]);
                generators[i] = generator;

                var task = generators[i].RunTask();
                generatorTasks[i] = task;
            }
        }

        public void StopGenerators()
        {
            for (int i = 0; i < generators.Length; i++)
                if (generators[i] != null)
                    generators[i].StopLoop();

            UserCurrentPositions = new ObservableCollection<TKCircle>();
        }

        public async Task PopulateWithPinsAndLinesForAllUsers()
        {
            try
            {
                Pins = new ObservableCollection<TKCustomMapPin>();
                Array.Sort(users, (a, b) => a.Username.CompareTo(b.Username)); // map colours to users by sorting them by username

                //firsly calibrate the map so the user does not think that the map freezed cuz the pins take a lot of time to be created
                if (users.Length > 0 && MapPage.LastKnownLocation.Center.Equals(MapPage.DEFAULT_MAP_POSITION))
                    await CalibrateMapRegion(users[0].Username);
                else if (!MapPage.LastKnownLocation.Center.Equals(MapPage.DEFAULT_MAP_POSITION))
                    MapRegion = MapPage.LastKnownLocation;
                else
                    MapRegion = MapSpan.FromCenterAndRadius(
                  MapPage.DEFAULT_MAP_POSITION, Distance.FromMiles(ClientConsts.FROM_KM_MAP_DISTANCE));

                var tasks = users.Select(async (user) => await PopulateWithPinsAndLinesForSingleUser(user.Username));
                await Task.WhenAll(tasks);
            }
            catch (AmazonServiceException e) // if there are problems with the service or with the internet
            {
                DependencyService.Get<IMessage>().ShortAlert(ClientConsts.DYNAMODB_EXCEPTION_MESSAGE2);
                Console.WriteLine(e.Message);
            }
            catch (WebException e)
            {
                Console.WriteLine(e.Message);
                DependencyService.Get<IMessage>().LongAlert(ClientConsts.DYNAMODB_EXCEPTION_MESSAGE1);
            }
            catch (Exception e) // in case of unexpected error like Error: NameResolutionFailure
            {
                Console.WriteLine("EXCEPTION COUGHT:\n {0} ", e.StackTrace);
                DependencyService.Get<IMessage>().ShortAlert(ClientConsts.INTERNET_EXCEPTION_MESSAGE);
            }
        }

        private async Task PopulateWithPinsAndLinesForSingleUser(string username)
        {
            int routeCount = -1;

            //find RouteCount for user
            var group = await QueryHashLoader.LoadData<DataFormat.Group>(groupName);
            if (group != null && group.ActiveDriverRoutes != null)
                foreach (var activeRoute in group.ActiveDriverRoutes)
                    if ((activeRoute.Split(ClientConsts.CONCAT_SPECIAL_CHARACTER[0])[0]).Equals(username))
                    {
                        routeCount = Int32.Parse(activeRoute.Split(ClientConsts.CONCAT_SPECIAL_CHARACTER[0])[1]);
                        break;
                    }

            if (routeCount != -1)
            {

                //get current user color
                int indexOfUser = -1;
                for (int i = 0; i < users.Length; i++)
                    if (users[i].Username.Equals(username))
                    {
                        indexOfUser = i;
                        break;
                    }
                Color color;
                if (indexOfUser == -1)
                    color = Color.Red;
                else
                    color = ClientConsts.Colours[indexOfUser];


                //query routes
                var routeInfo = await QueryRoute.QuerySingleRouteInfo(username, groupName, routeCount);
                var routes = await QueryRoute.QueryRoutes(username, groupName, routeCount.ToString(), routeInfo.CountRouteAddresses);

                foreach (var route in routes)
                {
                    if (route.Delivered == false) // pass this data
                        continue;

                    //map routes to lat and lng and add them to the pins list
                    var locator = CrossGeolocator.Current;
                    var addressPositions = await locator.GetPositionsForAddressAsync(route.Location.ToString());

                    if (addressPositions != null)
                    {

                        if (route.Delivered == true) // add pins only for the delivered routes
                        {
                            foreach (var pos in addressPositions)
                            {
                                var pin = new TKCustomMapPin
                                {
                                    Position = new Position(pos.Latitude, pos.Longitude),
                                    Title = route.Location.ToString(),
                                    Subtitle = username,
                                    ShowCallout = true,
                                    DefaultPinColor = color
                                };

                                Device.BeginInvokeOnMainThread(() =>
                                {
                                    Pins.Add(pin);
                                });
                            }
                        }
                    }
                }
            }
        }

        public async Task CalibrateMapRegion(string username)
        {
            Plugin.Geolocator.Abstractions.Position pos = null;
            string showUsername = "";

            if (currentUser.Role.Equals(RoledTrackUser.TYPE_DRIVER)) // then just get his current position
            {
                pos = await MapUserGeneratorThread.GetPositionForCurrentType(true, currentUser.Username);
                showUsername = currentUser.Username;
            }
            else if (currentUser.Role.Equals(RoledTrackUser.TYPE_ADMINISTRATOR))
            {
                pos = await MapUserGeneratorThread.GetPositionForCurrentType(false, username);
                showUsername = username;
            }

            if (pos != null)
            {
                SetMapRegion(new Position(pos.Latitude, pos.Longitude));
            }
            else
            {
                SetMapRegion(MapPage.DEFAULT_MAP_POSITION); // calibrate the map to the default pos
                Device.BeginInvokeOnMainThread(() => DependencyService.Get<IMessage>().LongAlert(String.Format(NO_POS_ALERT_TEXT, MapUserGeneratorThread.END_ADMINISTRATOR_LOOK_UP_POSITION_TIME_HOURS, showUsername)));
            }
        }



        public void AddCircle(int index, TKCircle circle)
        {
            if (UserCurrentPositions != null && index <= UserCurrentPositions.Count)
                UserCurrentPositions.Insert(index, circle);
        }

        public void RemoveCircle(int index)
        {
            if (UserCurrentPositions != null && index < UserCurrentPositions.Count)
                UserCurrentPositions.RemoveAt(index);
        }

        public TKCircle GetCircleAtIndex(int index)
        {
            if (index < UserCurrentPositions.Count)
                return UserCurrentPositions[index];

            return null;
        }

        public void SetMapRegion(Position position)
        {
            MapRegion = MapSpan.FromCenterAndRadius(position, Distance.FromKilometers(
                ClientConsts.FROM_KM_MAP_DISTANCE));
        }
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private bool TestPositions(Position currentPos, Position givenPos)
        {
            if ((givenPos.Latitude - LATITUDE_ACCEPTED_ERROR <= currentPos.Latitude) && (currentPos.Latitude <= givenPos.Latitude + LATITUDE_ACCEPTED_ERROR) &&
                (givenPos.Longitude - LONGITUDE_ACCEPTED_ERROR <= currentPos.Longitude) && (currentPos.Longitude <= givenPos.Longitude + LONGITUDE_ACCEPTED_ERROR))
                return true;

            return false;
        }


    }
}
