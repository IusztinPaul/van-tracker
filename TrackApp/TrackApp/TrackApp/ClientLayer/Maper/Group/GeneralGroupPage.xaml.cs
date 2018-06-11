using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackApp.DataFormat.UserData;
using TrackApp.ServerLayer.Query;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using TrackApp.DataFormat;
using TrackApp.ClientLayer.Exceptions;
using System.Net;
using TrackApp.ClientLayer.CustomUI;
using Amazon.Runtime;
using TrackApp.ClientLayer.Maper.Group.MapN;

namespace TrackApp.ClientLayer.Maper.Group
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GeneralGroupPage : ContentPage
    {
        //driver const text
        public const string DRIVER_TITLE_TEXT = "Ruta activa";
        public const string ADDRESSES_NUMBER_TEXT = "Ruta are {0} adrese";
        public const string NO_DATA_TEXT = "Nu exista rute curente";
        public const string NO_ROUTES_TITLE_ALERT = "Atentie";
        public const string NO_ROUTES_CONTENT_ALERT = "Nu va putem lasa sa accesati harta, deoarece nu exista nici o ruta activa";

        protected ActivityIndicator actInd;
        protected Label labelNoData;

        private RoledTrackUser currentUser;
        private string groupName;

        public GeneralGroupPage(RoledTrackUser currentUser, string groupName)
        {
            InitializeComponent();
            this.currentUser = currentUser;
            this.groupName = groupName;

            actInd = new ActivityIndicator
            {
                IsEnabled = true,
                IsRunning = true
            };
            contentStackLayout.Children.Add(actInd);

            labelNoData = new Label
            {
                Text = NO_DATA_TEXT,
                FontSize = 22d,
                TextColor = Color.Black,
                FontAttributes = FontAttributes.Bold,
                HorizontalOptions = LayoutOptions.StartAndExpand,
                Margin = new Thickness(8, 1)
            };

            BtnGoToMap.Clicked += (source, args) =>
            {
                Device.BeginInvokeOnMainThread(async () => await BtnGoToMapListener());
            };
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            await CreateView();

            if (currentUser.Role.Equals(RoledTrackUser.TYPE_ADMINISTRATOR))
                ActiveRoutesList.ItemSelected += AdiminstratorListItemSelectedListener; //add listener
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            if (contentStackLayout.Children.Contains(labelNoData))
                contentStackLayout.Children.Remove(labelNoData);

            if(currentUser.Role.Equals(RoledTrackUser.TYPE_ADMINISTRATOR))
                ActiveRoutesList.ItemSelected -= AdiminstratorListItemSelectedListener; //remove listener
        }

        private async Task BtnGoToMapListener()
        {
            try
            {
                BtnGoToMap.IsEnabled = false;

                var updatedUser = await QueryHashLoader.LoadData<TrackUser>(currentUser.Username);
                updatedUser = new RoledTrackUser(updatedUser)
                {
                    Role = currentUser.Role
                };
                var mapUsers = await GetUserForModelView();
                if (mapUsers.Length > 0)
                    await Navigation.PushAsync(new ActualMapTabbedPage(updatedUser as RoledTrackUser, groupName, mapUsers));
                else
                    await DisplayAlert(NO_ROUTES_TITLE_ALERT, NO_ROUTES_CONTENT_ALERT, "Ok");
            }
            catch (AmazonServiceException e) // if there are problems with the service or with the internet
            {
                DependencyService.Get<IMessage>().ShortAlert(ClientConsts.DYNAMODB_EXCEPTION_MESSAGE2);
            }
            catch (ValidationException e) // display error message to currentUser
            {
                DependencyService.Get<IMessage>().ShortAlert(e.Message);
            }
            catch (WebException e)
            {
                DependencyService.Get<IMessage>().LongAlert(ClientConsts.DYNAMODB_EXCEPTION_MESSAGE1);
            }
            catch (Exception e) // in case of unexpected error 
            {
                Console.WriteLine("EXCEPTION COUGHT: " + e.Message);
                Console.WriteLine("TYPE: " + e.GetType());
                DependencyService.Get<IMessage>().LongAlert(e.Message);
            }
            finally
            {
                BtnGoToMap.IsEnabled = true;
            }
        }

        private async Task CreateView()
        {
            try
            {
                var group = await QueryHashLoader.LoadData<DataFormat.Group>(groupName);

                if (group != null)
                {
                    if (currentUser.Role.Equals(RoledTrackUser.TYPE_DRIVER))
                        await MakeDriverView(group);
                    else if (currentUser.Role.Equals(RoledTrackUser.TYPE_ADMINISTRATOR))
                        await MakeAdministratorView(group);
                    else
                        throw new Exception("There are no other roles. Problems in your logic!!!");
                }
            }
            catch (AmazonServiceException e) // if there are problems with the service or with the internet
            {
                DependencyService.Get<IMessage>().ShortAlert(ClientConsts.DYNAMODB_EXCEPTION_MESSAGE2);
            }
            catch (ValidationException e) // display error message to currentUser
            {
                DependencyService.Get<IMessage>().ShortAlert(e.Message);
            }
            catch (WebException e)
            {
                DependencyService.Get<IMessage>().LongAlert(ClientConsts.DYNAMODB_EXCEPTION_MESSAGE1);
            }
            catch (Exception e) // in case of unexpected error 
            {
                Console.WriteLine("EXCEPTION COUGHT: " + e.Message);
                Console.WriteLine("TYPE: " + e.GetType());
                DependencyService.Get<IMessage>().LongAlert(e.Message);
            }
            finally
            {
                contentStackLayout.Children.Remove(actInd);
            }
        }


        private async Task MakeDriverView(DataFormat.Group group)
        {
            //switch views
            StackLayoutDriver.IsVisible = true;

            ActiveRoutesList.HeightRequest = 0;
            ActiveRoutesList.IsVisible = false;

            //get route index
            int countOfRoute = -1;
            if (group.ActiveDriverRoutes != null)
                foreach (var activeRoute in group.ActiveDriverRoutes)
                    if ((activeRoute.Split(ClientConsts.CONCAT_SPECIAL_CHARACTER[0])[0]).Trim().Equals(currentUser.Username.Trim()))
                    {
                        countOfRoute = Int32.Parse(activeRoute.Trim().Split(ClientConsts.CONCAT_SPECIAL_CHARACTER[0])[1]);
                        break;
                    }

            // fill with data
            if (countOfRoute != -1)
            {

                var routeInfo = await QueryRoute.QuerySingleRouteInfo(currentUser.Username, groupName, countOfRoute);
                if (routeInfo != null)
                {
                    //maybe before the route Info was null so there was no data to display
                    LabelRouteName.IsVisible = true;
                    LabelAddressesNumber.IsVisible = true;

                    LabelNoData.IsVisible = false;

                    //populate view
                    LabelRouteName.Text = routeInfo.RouteName;
                    LabelRouteName.GestureRecognizers.Clear(); // first clear so the gesture recognizers wont be added multiple times
                    LabelRouteName.GestureRecognizers.Add(new TapGestureRecognizer()
                    {
                        Command = new Command(() => Device.BeginInvokeOnMainThread(async () => await Navigation.PushAsync(new SingleRoutePage(routeInfo, currentUser, groupName, currentUser.Username))))
                    });

                    LabelAddressesNumber.Text = String.Format(ADDRESSES_NUMBER_TEXT, routeInfo.CountRouteAddresses);
                }
            }
            else
            {
                //show no data message
                LabelRouteName.IsVisible = false;
                LabelAddressesNumber.IsVisible = false;

                LabelNoData.IsVisible = true;
            }
        }

        private async Task MakeAdministratorView(DataFormat.Group group)
        {


            //get data
            if (group.ActiveDriverRoutes != null && group.ActiveDriverRoutes.Count > 0)
            {
                var listOfRouteInfos = new List<RouteInfo>();
                foreach (var activeRoute in group.ActiveDriverRoutes)
                {
                    var items = activeRoute.Split(ClientConsts.CONCAT_SPECIAL_CHARACTER[0]);
                    var routeInfo = new RouteInfo
                    {
                        RouteId = groupName + ClientConsts.CONCAT_SPECIAL_CHARACTER + items[0],
                        Count = Int32.Parse(items[1])
                    };
                    listOfRouteInfos.Add(routeInfo);
                }

                //get list of routes info with full data and sort it
                listOfRouteInfos = await QueryRoute.QueryMultipleRoutesInfo(listOfRouteInfos);
                listOfRouteInfos.Sort((a, b) => a.OwnerUsername.CompareTo(b.OwnerUsername));
                //populate list
                ActiveRoutesList.ItemsSource = listOfRouteInfos;

                //switch view
                ActiveRoutesList.HeightRequest = -1;
                ActiveRoutesList.IsVisible = true;
                StackLayoutDriver.IsVisible = false;
                
            }
            else
            {
                contentStackLayout.Children.Insert(0, labelNoData);

                ActiveRoutesList.HeightRequest = 0;
                ActiveRoutesList.IsVisible = false;

                StackLayoutDriver.IsVisible = false;
            }
        }

        private void AdiminstratorListItemSelectedListener(object source, SelectedItemChangedEventArgs args)
        {
            if (args.SelectedItem != null)
            {
                var tappedRouteInfo = args.SelectedItem as RouteInfo;
                if (tappedRouteInfo != null)
                    Device.BeginInvokeOnMainThread(async () => await Navigation.PushAsync(new SingleRoutePage(tappedRouteInfo, currentUser, groupName, tappedRouteInfo.OwnerUsername)));
                (source as ListView).SelectedItem = null;
            }
        }

        private async Task<RoledTrackUser[]> GetUserForModelView()
        {
            try
            {
                var group = await QueryHashLoader.LoadData<DataFormat.Group>(groupName);
                if (group != null && group.ActiveDriverRoutes != null)
                {
                    if (currentUser.Role.Equals(RoledTrackUser.TYPE_DRIVER))
                    {
                        foreach(var driver in group.ActiveDriverRoutes) // check to see if the driver has a active route
                            if((driver.Split(ClientConsts.CONCAT_SPECIAL_CHARACTER[0])[0]).Equals(currentUser.Username))
                                return new RoledTrackUser[] { currentUser };

                        return new RoledTrackUser[0];
                    }
                    else if (currentUser.Role.Equals(RoledTrackUser.TYPE_ADMINISTRATOR))
                    {


                        var drivers = new RoledTrackUser[group.ActiveDriverRoutes.Count];
                        int i = 0;
                        foreach (var driver in group.ActiveDriverRoutes)
                        {
                            drivers[i] = new RoledTrackUser(new TrackUser
                            {
                                Username = driver.Split(ClientConsts.CONCAT_SPECIAL_CHARACTER[0])[0].Trim()
                            })
                            { Role = RoledTrackUser.TYPE_DRIVER,
                              };
                            i++;
                        }

                        return drivers;
                    }
                    else
                        return new RoledTrackUser[0];
                }
                return new RoledTrackUser[0];
            }
            catch (Exception e)
            {
                Console.WriteLine("EXCEPTION COUGHT {0} ", e.Message);
                return new RoledTrackUser[0];
            }
        }
    }
}