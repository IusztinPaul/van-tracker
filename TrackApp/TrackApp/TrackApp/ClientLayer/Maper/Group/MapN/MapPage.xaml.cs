using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackApp.ClientLayer.Maper.MapThread;
using TrackApp.DataFormat.UserData;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using TK.CustomMap;
using TK.CustomMap.Overlays;
using TrackApp.ServerLayer.Query;

namespace TrackApp.ClientLayer.Maper.Group.MapN
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MapPage : ContentPage
    {
        public static readonly Position DEFAULT_MAP_POSITION = new Position(45.7489, 21.2087); //Timisoara Romania
        public const string DISPLAY_ACTION_SHEET_TITLE = "Centraza";
        public const string DISPLAY_ACTION_SHEET_CANCEL_BUTTON_TEXT = "Renunta";

        private TKCustomMap _map;

        private RoledTrackUser currentUser;
        private string groupName;
        private RoledTrackUser[] mapUsers;

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
                VerticalOptions = LayoutOptions.FillAndExpand
            };
            this._map.SetBinding(TKCustomMap.CirclesProperty, "UserCurrentPositions");
            this._map.SetBinding(TKCustomMap.MapRegionProperty, "MapRegion");
            this._map.SetBinding(TKCustomMap.PinsProperty, "Pins");

            var stack = new StackLayout { Spacing = 0 };
            stack.Children.Add(_map);
            Content = stack;

            BindingContext = new MapPageModelView(mapUsers, _map, groupName, currentUser);

            ToolbarItem calibrateMapTBItem = new ToolbarItem();
            calibrateMapTBItem.Text = ClientConsts.MAP_CALIBRATE_TOOL_BAR_ITEM_TITLE;
            calibrateMapTBItem.Priority = 0;
            calibrateMapTBItem.Order = ToolbarItemOrder.Secondary;
            calibrateMapTBItem.Command = new Command(async () => await CalibrateMapRegion());
            this.ToolbarItems.Add(calibrateMapTBItem);
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            var bindCont = BindingContext as MapPageModelView;
            if (bindCont != null)
            {
                await bindCont.PopulateWithPinsAndLinesForAllUsers(); // this also calibrates the map

                if (currentUser.Role.Equals(RoledTrackUser.TYPE_ADMINISTRATOR))
                    bindCont.StartGenerators(false);
                else if (currentUser.Role.Equals(RoledTrackUser.TYPE_DRIVER))
                    bindCont.StartGenerators(true);
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            var bindCont = BindingContext as MapPageModelView;
            if (bindCont != null)
                bindCont.StopGenerators();

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
    }
}