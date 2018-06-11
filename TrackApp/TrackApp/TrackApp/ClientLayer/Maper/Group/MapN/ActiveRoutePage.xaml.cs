using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackApp.ClientLayer.CustomUI;
using TrackApp.DataFormat;
using TrackApp.DataFormat.UserData;
using TrackApp.ServerLayer.Save;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TrackApp.ClientLayer.Maper.Group.MapN
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ActiveRoutePage : ContentPage
    {
        public const string NOT_AT_ADDRESS_POSITION_MESSAGE = "Nu sunteti la adresa respectiva!";

        public const double METERS_ERROR = 19;
        public static readonly double LATITUDE_ACCEPTED_ERROR = METERS_ERROR / 111320;
        public static readonly double LONGITUDE_ACCEPTED_ERROR = 40075000 * Math.Cos(LATITUDE_ACCEPTED_ERROR) / 360;

        private static DateTime lastCheckedAddress = DateTime.Now;
        private static TimeSpan beetweenCheckTimeInterval = TimeSpan.FromSeconds(10);
        private static bool firstTime = true;

        private RoledTrackUser currentUser;
        private string groupName;

        public ActiveRoutePage(RoledTrackUser currentUser, string groupName)
        {
            InitializeComponent();
            BindingContext = new ActiveRouteViewModel(currentUser, groupName);

            this.currentUser = currentUser;
            this.groupName = groupName;

            if (currentUser.Role.Equals(RoledTrackUser.TYPE_ADMINISTRATOR))
            {
                ActiveRoutesList.ItemSelected += (s, a) => OnItemSelectedAdministrator(s, a);
            }
            else if (currentUser.Role.Equals(RoledTrackUser.TYPE_DRIVER))
            {
                AddressesListView.ItemSelected += async (s, a) => await OnItemSelectedDriver(s, a);
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            var bindCont = BindingContext as ActiveRouteViewModel;
            if (bindCont != null)
                Device.BeginInvokeOnMainThread(async () => await bindCont.PopulateAsync());

            SrcBarData.Text = "";
        }

        public void OnTextChanged(object source, TextChangedEventArgs e)
        {
            var viewmodel = BindingContext as ActiveRouteViewModel;
            viewmodel?.SearchBarListener(e.NewTextValue);
        }

        private async Task OnItemSelectedDriver(object source, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem != null)
            {
                var item = e.SelectedItem as Route;
                if (item != null && item.Delivered == false)
                {

                    if (firstTime || (DateTime.Now - lastCheckedAddress >= beetweenCheckTimeInterval))
                    {
                        if (firstTime) // so the user does not have to wait on the first click
                            firstTime = false;
                        lastCheckedAddress = DateTime.Now;

                        var locator = CrossGeolocator.Current;
                        var position = await locator.GetPositionAsync(TimeSpan.FromSeconds(1), null, true);

                        var addressPositions = await locator.GetPositionsForAddressAsync(item.Location.ToString());
                        if (addressPositions != null)
                            foreach (var pos in addressPositions)
                                if (TestPositions(position, pos))
                                {
                                    item.Delivered = true;
                                    item.DateTime = DateTime.Now;

                                    //change view
                                    var bindCont = BindingContext as ActiveRouteViewModel;
                                    if (bindCont != null)
                                        Device.BeginInvokeOnMainThread(() => bindCont.ChangeAddressToTrue(item));
                                    break;
                                }

                        if (item.Delivered == true)
                        {
                            await RouteSaver.SaveSingleRoute(item);
                        }
                        else
                        {
                            DependencyService.Get<IMessage>().LongAlert(NOT_AT_ADDRESS_POSITION_MESSAGE);
                        }
                    }
                }
                (source as ListView).SelectedItem = null;
            }
        }

        private void OnItemSelectedAdministrator(object source, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem != null)
            {
                var item = e.SelectedItem as RouteInfo;
                if (item != null)
                {
                    Device.BeginInvokeOnMainThread(async () => await Navigation.PushAsync(new SingleRoutePage(item, currentUser, groupName, item.OwnerUsername)));
                }
                (source as ListView).SelectedItem = null;
            }
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