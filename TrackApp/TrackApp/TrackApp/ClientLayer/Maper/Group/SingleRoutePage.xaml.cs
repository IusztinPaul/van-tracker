using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using TrackApp.DataFormat;
using TrackApp.DataFormat.UserData;
using TrackApp.ClientLayer.CustomUI;
using TrackApp.ServerLayer.Save;
using Amazon.Runtime;
using TrackApp.ClientLayer.Exceptions;
using System.Net;
using TrackApp.ServerLayer.Query;

namespace TrackApp.ClientLayer.Maper.Group
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SingleRoutePage : ContentPage
    {
        public const string NO_PERMISSION_TEXT = "Nu ai permisiunea de a edita ruta!";
        public const string DELETE_WAIT_FOR_DATA_MESSAGE = "Asteapta ca datele sa se incarce pentru a putea fi sterse!";
        public const string TEXT_ACTIVE_ROUTE_CHANGED = "Aceasta ruta a devenit activa pentru {0}";

        public const string TEXT_ACTIVE_ROUTE = "Activa";
        public const string TEXT_UNACTIVE_ROUTE = "Inactiva";

        private RouteInfo routeInfo;
        private string groupName;
        private string tappedUsername;

        public SingleRoutePage(RouteInfo routeInfo, RoledTrackUser currentUser, string groupName, string tappedUsername)
        {
            InitializeComponent();
            BindingContext = new SingleRouteViewModel(routeInfo, groupName, tappedUsername);
            this.routeInfo = routeInfo;
            this.groupName = groupName;
            this.tappedUsername = tappedUsername;

            Title = tappedUsername + " " + routeInfo.RouteName;

            //list view click listener
            AddressesListView.ItemSelected += (source, args) =>
            {
                if (args.SelectedItem != null)
                {
                    var item = args.SelectedItem as Route;
                    if (item != null)
                    {
                        if (currentUser.Role.Equals(RoledTrackUser.TYPE_ADMINISTRATOR))
                            Device.BeginInvokeOnMainThread(async () => await Navigation.PushAsync(new EditAddressPage(item)));
                        else
                            DependencyService.Get<IMessage>().LongAlert(NO_PERMISSION_TEXT);
                    }
                    (source as ListView).SelectedItem = null;
                }
            };

            if (currentUser.Role.Equals(RoledTrackUser.TYPE_ADMINISTRATOR))
            {
                //toolbar items
                //remove route
                ToolbarItem removeRouteTbItem = new ToolbarItem();
                removeRouteTbItem.Text = ClientConsts.REMOVE_ROUTE_TOOL_BAR_TITLE;
                removeRouteTbItem.Priority = 0;
                removeRouteTbItem.Order = ToolbarItemOrder.Secondary;
                removeRouteTbItem.Command = new Command(async () =>
                {
                    var actInd = new ActivityIndicator
                    {
                        IsEnabled = true,
                        IsRunning = true
                    };
                    contentStackLayout.Children.Insert(0, actInd);
                    await RemoveCurrentRoute();
                    contentStackLayout.Children.Remove(actInd);
                });

                //make route active 
                ToolbarItem activeRouteActivatorTbItem = new ToolbarItem();
                activeRouteActivatorTbItem.Text = ClientConsts.MAKE_ROUTE_ACTIVE_TOOL_BAR_TITLE;
                activeRouteActivatorTbItem.Priority = 0;
                activeRouteActivatorTbItem.Order = ToolbarItemOrder.Secondary;
                activeRouteActivatorTbItem.Command = new Command(async () =>
                {
                    var actInd = new ActivityIndicator
                    {
                        IsEnabled = true,
                        IsRunning = true
                    };
                    contentStackLayout.Children.Insert(0, actInd);
                    await MakeCurrentRouteActive();
                    contentStackLayout.Children.Remove(actInd);
                });

                this.ToolbarItems.Add(removeRouteTbItem);
                this.ToolbarItems.Add(activeRouteActivatorTbItem);
            }
        }



        protected override void OnAppearing()
        {
            base.OnAppearing();

            //populate
            var bindCont = BindingContext as SingleRouteViewModel;
            if (bindCont != null)
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await bindCont.PopulateAsync();
                });
        }

        private async Task MakeCurrentRouteActive()
        {
            try
            {
                var group = await QueryHashLoader.LoadData<DataFormat.Group>(groupName);
                if(group != null)
                {
                    string activeRouteData = "";
                    int indexOfChangingData = -1;

                    //grab the index and data of the active route that has to be modified
                    if(group.ActiveDriverRoutes != null)
                        foreach(var currentRoute in group.ActiveDriverRoutes)
                        {
                            if(currentRoute.Trim().StartsWith(tappedUsername.Trim()))
                            {
                                activeRouteData = currentRoute;
                                indexOfChangingData = group.ActiveDriverRoutes.IndexOf(currentRoute);
                                break;
                            }
                        }

                    if (indexOfChangingData == -1) // if somehow the user has no active route
                    {
                        if (group.ActiveDriverRoutes == null)
                            group.ActiveDriverRoutes = new List<string>();

                        group.ActiveDriverRoutes.Add(tappedUsername + ClientConsts.CONCAT_SPECIAL_CHARACTER +
                                                        routeInfo.Count);
                    }
                    else
                    {
                        //modify data -> tapped user active route ( structure: username#countOfRouteInGroup)
                        activeRouteData = activeRouteData.Split(ClientConsts.CONCAT_SPECIAL_CHARACTER[0])[0] +
                                            ClientConsts.CONCAT_SPECIAL_CHARACTER + routeInfo.Count;
                        group.ActiveDriverRoutes[indexOfChangingData] = activeRouteData;
                    }

                    //update view
                    (BindingContext as SingleRouteViewModel).ActiveRouteText = TEXT_ACTIVE_ROUTE;

                    //now save data
                    await new SaveGroup
                    {
                        Group = group
                    }.SaveData();

                    DependencyService.Get<IMessage>().LongAlert(String.Format(TEXT_ACTIVE_ROUTE_CHANGED, tappedUsername));
                }
                else
                {
                    throw new Exception("Problems in your logic. The group should exist!!!");
                }
            }
            catch (AmazonServiceException e) // if there are problems with the service or with the internet
            {
                DependencyService.Get<IMessage>().ShortAlert(ClientConsts.DYNAMODB_EXCEPTION_MESSAGE2);
            }
            catch (ValidationException e) // show error message to the user
            {
                DependencyService.Get<IMessage>().ShortAlert(e.Message);
            }
            catch (WebException e)
            {
                DependencyService.Get<IMessage>().LongAlert(ClientConsts.DYNAMODB_EXCEPTION_MESSAGE1);
            }
            catch (Exception e) // in case of unexpected error like Error: NameResolutionFailure
            {
                Console.WriteLine("EXCEPTION {0}", e.Message);
                DependencyService.Get<IMessage>().ShortAlert(ClientConsts.INTERNET_EXCEPTION_MESSAGE);
            }
        }

        private async Task RemoveCurrentRoute()
        {
            try
            {
                //first grab group to see if the current route it's active --> cannot delete an active route
                var group = await QueryHashLoader.LoadData<DataFormat.Group>(groupName);
                if (group.ActiveDriverRoutes != null && !group.ActiveDriverRoutes.Contains(tappedUsername.Trim() + ClientConsts.CONCAT_SPECIAL_CHARACTER + routeInfo.Count))
                {

                    var bindCont = BindingContext as SingleRouteViewModel;
                    if (bindCont.AddressesList != null)
                    {
                        if (await DisplayAlert("Atentie", "Sunteti sigur ca vreti sa stergeti ruta?", "Da", "Nu"))
                        {
                            var deleter = new RouteSaver
                            {
                                RouteInfo = this.routeInfo,
                                Routes = new List<Route>(bindCont.AddressesList)
                            };
                            await deleter.DeleteRoute();
                        }

                        //after deletion go back to the routes page
                        Device.BeginInvokeOnMainThread(async () => await Navigation.PopAsync());
                    }
                    else
                        DependencyService.Get<IMessage>().LongAlert(DELETE_WAIT_FOR_DATA_MESSAGE);
                }
                else await DisplayAlert("Atentie", "Nu puteti sterge o ruta activa!", "Ok");
            }
            catch (AmazonServiceException e) // if there are problems with the service or with the internet
            {
                DependencyService.Get<IMessage>().ShortAlert(ClientConsts.DYNAMODB_EXCEPTION_MESSAGE2);
            }
            catch (ValidationException e) // show error message to the user
            {
                DependencyService.Get<IMessage>().ShortAlert(e.Message);
            }
            catch (WebException e)
            {
                DependencyService.Get<IMessage>().LongAlert(ClientConsts.DYNAMODB_EXCEPTION_MESSAGE1);
            }
            catch (Exception e) // in case of unexpected error like Error: NameResolutionFailure
            {
                Console.WriteLine("EXCEPTION {0}", e.Message);
                DependencyService.Get<IMessage>().ShortAlert(ClientConsts.INTERNET_EXCEPTION_MESSAGE);
            }
        }
    }
}