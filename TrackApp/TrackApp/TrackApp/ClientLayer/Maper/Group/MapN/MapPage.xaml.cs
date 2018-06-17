using System;
using System.Threading.Tasks;
using TrackApp.DataFormat.UserData;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using TK.CustomMap;
using TrackApp.ServerLayer.Query;
using Plugin.Geolocator;
using TrackApp.ClientLayer.CustomUI;

namespace TrackApp.ClientLayer.Maper.Group.MapN
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MapPage : ContentPage
    {
        public static readonly Position DEFAULT_MAP_POSITION = new Position(45.7489, 21.2087); //Timisoara Romania
        public const string DISPLAY_ACTION_SHEET_TITLE = "Centraza";
        public const string DISPLAY_ACTION_SHEET_CANCEL_BUTTON_TEXT = "Renunta";
        public const string NO_ADDRESS_FOUND_TEXT = "Adresa nu exista in baza noastra de date!";

        public static Position LastKnownLocation = DEFAULT_MAP_POSITION;

        private TKCustomMap _map;

        private RoledTrackUser currentUser;
        private string groupName;
        private RoledTrackUser[] mapUsers;

        private bool IsSearchBarShowing = false;
        private SearchBar locationSearchBar = new SearchBar
        {
            Placeholder = "Cauta locatie",
            IsVisible = false,
            Margin = new Thickness(2),
            HeightRequest = 30,
            WidthRequest = 300,
            Scale = 0.9f
        };

        public MapPage(RoledTrackUser currentUser, string groupName, RoledTrackUser[] mapUsers)
        {
            InitializeComponent();

            this.currentUser = currentUser;
            this.groupName = groupName;
            this.mapUsers = mapUsers;

            this._map = new TKCustomMap(
          MapSpan.FromCenterAndRadius(
                  DEFAULT_MAP_POSITION, Distance.FromMiles(ClientConsts.FROM_KM_MAP_DISTANCE)))
            {
                IsShowingUser = false,
                HeightRequest = 100,
                WidthRequest = 960,
                IsRegionChangeAnimated = true,
                VerticalOptions = LayoutOptions.FillAndExpand
            };
            this._map.SetBinding(TKCustomMap.CirclesProperty, "UserCurrentPositions");
            this._map.SetBinding(TKCustomMap.MapRegionProperty, "MapRegion");
            this._map.SetBinding(TKCustomMap.PinsProperty, "Pins");

            var stack = new StackLayout { Spacing = 0 };
            stack.Children.Add(locationSearchBar);
            stack.Children.Add(_map);
            Content = new ScrollView { Content = stack };

            BindingContext = new MapPageModelView(mapUsers, _map, groupName, currentUser);

            ToolbarItem calibrateMapTBItem = new ToolbarItem();
            calibrateMapTBItem.Text = ClientConsts.MAP_CALIBRATE_TOOL_BAR_ITEM_TITLE;
            calibrateMapTBItem.Priority = 1;
            calibrateMapTBItem.Order = ToolbarItemOrder.Secondary;
            calibrateMapTBItem.Command = new Command(async () => await CalibrateMapRegion());
            this.ToolbarItems.Add(calibrateMapTBItem);

            ToolbarItem searchBarTBItem = new ToolbarItem();
            searchBarTBItem.Text = ClientConsts.SEARCH_BAR_TOOL_BAR_ITEM_TITLE;
            searchBarTBItem.Priority = 0;
            searchBarTBItem.Order = ToolbarItemOrder.Secondary;
            searchBarTBItem.Command = new Command(() => {
                IsSearchBarShowing = !IsSearchBarShowing;
                locationSearchBar.IsVisible = IsSearchBarShowing;
                });
            this.ToolbarItems.Add(searchBarTBItem);
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            var bindCont = BindingContext as MapPageModelView;
            if (bindCont != null)
            {
                //firstly create the user circles
                if (currentUser.Role.Equals(RoledTrackUser.TYPE_ADMINISTRATOR))
                    bindCont.StartGenerators(false);
                else if (currentUser.Role.Equals(RoledTrackUser.TYPE_DRIVER))
                    bindCont.StartGenerators(true);

                //after create the pins and calibrate the map
                await bindCont.PopulateWithPinsAndLinesForAllUsers(); // this also calibrates the map
            }

            locationSearchBar.SearchButtonPressed += SearchButtonPressed;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            var bindCont = BindingContext as MapPageModelView;
            if (bindCont != null)
                bindCont.StopGenerators();

            locationSearchBar.SearchButtonPressed -= SearchButtonPressed;

            //update last know location
            LastKnownLocation = _map.MapRegion.Center;
        }

        private async Task CalibrateMapRegion()
        {
            var bindCont = BindingContext as MapPageModelView;
            if (bindCont != null)
            {

                string username = null;

                if (currentUser.Role.Equals(RoledTrackUser.TYPE_ADMINISTRATOR)) // pop a list of driver choices
                {
                    var group = await QueryHashLoader.LoadData<DataFormat.Group>(groupName);
                    if (group != null && group.ActiveDriverRoutes != null)
                    {
                        string[] names = new string[group.ActiveDriverRoutes.Count];
                        for (int i = 0; i < group.ActiveDriverRoutes.Count; i++)
                            names[i] = group.ActiveDriverRoutes[i].Split(ClientConsts.CONCAT_SPECIAL_CHARACTER[0])[0]; //take the driver names

                        username = await DisplayActionSheet(DISPLAY_ACTION_SHEET_TITLE, DISPLAY_ACTION_SHEET_CANCEL_BUTTON_TEXT, null, names);
                    }
                }

                if (username == null || !username.Equals(DISPLAY_ACTION_SHEET_CANCEL_BUTTON_TEXT)) // calibrate works with null parameter
                    Device.BeginInvokeOnMainThread(async () =>
                   {
                       await bindCont.CalibrateMapRegion(username); // if the current user is a driver it does not matter because it will take the username from the currentUser object of the model
                   });
            }
        }

        private async void SearchButtonPressed(object sender, EventArgs args)
        {
            var text = locationSearchBar.Text?.Trim();
            if(!String.IsNullOrEmpty(text))
            {
                var locator = CrossGeolocator.Current;
                var addressPositions = await locator.GetPositionsForAddressAsync(text);
                if (addressPositions != null) //if found calibrate to the first position found
                    foreach (var pos in addressPositions)
                    {
                        (BindingContext as MapPageModelView)?.SetMapRegion(new TK.CustomMap.Position(pos.Latitude, pos.Longitude));
                        break;
                    }
                else
                    Device.BeginInvokeOnMainThread(() => DependencyService.Get<IMessage>().LongAlert(NO_ADDRESS_FOUND_TEXT));
            }
           
        }
    }
}