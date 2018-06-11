using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using TrackApp.DataFormat.UserData;
using TrackApp.DataFormat;
using TrackApp.ServerLayer.Query;
using Amazon.Runtime;
using TrackApp.ClientLayer.CustomUI;
using Xamarin.Forms;
using TrackApp.ClientLayer.Exceptions;
using System.Net;
using TrackApp.ClientLayer.Extensions;

namespace TrackApp.ClientLayer.Maper.Group
{
    public class UserRoutesViewModel : BasicRefreshingModelView
    {

        private ObservableCollection<RouteInfo> _routesList;
        public ObservableCollection<RouteInfo> RoutesList
        {
            get => _routesList;
            set
            {
                _routesList = value;
                OnPropertyChanged("RoutesList");
            }
        }


        private RoledUsername tappedUsername;
        private string groupName;

        public UserRoutesViewModel(RoledTrackUser currentUser, RoledUsername tappedUsername, string groupName) : base(currentUser)
        {
            this.tappedUsername = tappedUsername;
            this.groupName = groupName;
        }

        public async override Task PopulateAsync()
        {
            // if already refreshing don't populate
            if (IsBusy)
                return;

            //set the refresh state so another refresh wont be possible
            IsBusy = true;
            (OnRefreshCommand as Command)?.ChangeCanExecute();

            try
            {

                //find tapped user driver in group
                var group = await QueryHashLoader.LoadData<DataFormat.Group>(groupName);
                int numberOfRoutesInGroup = -1;

                foreach (string drivers in group.Drivers)
                {
                    var items = drivers.Split(ClientConsts.CONCAT_SPECIAL_CHARACTER[0]);
                    var name = items[0];

                    if (name.Equals(tappedUsername.Username))
                    {
                        numberOfRoutesInGroup = Int32.Parse(items[1]);
                        break;
                    }
                }

                //find active route
                int activeRouteCount = -1;
                if(group.ActiveDriverRoutes != null)
                    foreach (string activeRoutes in group.ActiveDriverRoutes)
                        if ((activeRoutes.Split(ClientConsts.CONCAT_SPECIAL_CHARACTER[0])[0]).Trim().StartsWith(tappedUsername.Username))
                        {
                            activeRouteCount = Int32.Parse(activeRoutes.Split(ClientConsts.CONCAT_SPECIAL_CHARACTER[0])[1]);
                            break;
                        }

                if (numberOfRoutesInGroup == -1)
                    throw new Exception("The driver should be 100% in the group. There are problems in your logic!!!!");

                //query routes
                var routes = await QueryRoute.QueryRouteInfo(tappedUsername.Username, groupName, numberOfRoutesInGroup);

                //bind view
                if (routes == null)
                    RoutesList = new ObservableCollection<RouteInfo>();
                else
                {
                    RoutesList = new ObservableCollection<RouteInfo>(routes);

                    //set label name color
                    foreach (var routeinfo in RoutesList)
                        if (routeinfo.Count == activeRouteCount)
                            routeinfo.LabelColor = Color.Red;
                        else
                            routeinfo.LabelColor = Color.Black;
                    //sort
                    RoutesList.Sort<RouteInfo>((a, b) => a.RouteName.CompareTo(b.RouteName));
                }

            }
            catch (AmazonServiceException e) // if there are problems with the service or with the internet
            {
                DependencyService.Get<IMessage>().ShortAlert(ClientConsts.DYNAMODB_EXCEPTION_MESSAGE2);
                Console.WriteLine(e.Message);
            }
            catch (ValidationException e) // show error message to the user
            {
                DependencyService.Get<IMessage>().ShortAlert(e.Message);
                Console.WriteLine(e.Message);
            }
            catch (WebException e)
            {
                Console.WriteLine(e.Message);
                DependencyService.Get<IMessage>().LongAlert(ClientConsts.DYNAMODB_EXCEPTION_MESSAGE1);
            }
            catch (Exception e) // in case of unexpected error like Error: NameResolutionFailure
            {
                Console.WriteLine("EXCEPTION COUGHT {0} ", e.Message);
                DependencyService.Get<IMessage>().ShortAlert(ClientConsts.INTERNET_EXCEPTION_MESSAGE);
            }
            finally
            {
                //allow another refreshes
                IsBusy = false;
                (OnRefreshCommand as Command)?.ChangeCanExecute();
            }

        }
    }


}
