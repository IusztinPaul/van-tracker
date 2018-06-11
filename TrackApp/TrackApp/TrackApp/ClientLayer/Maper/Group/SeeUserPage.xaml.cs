using Amazon.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TK.CustomMap;
using TrackApp.ClientLayer.CustomUI;
using TrackApp.ClientLayer.Exceptions;
using TrackApp.ClientLayer.Maper.Group.MapN;
using TrackApp.DataFormat;
using TrackApp.DataFormat.UserData;
using TrackApp.ServerLayer.Query;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TrackApp.ClientLayer.Maper.Group
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SeeUserPage : ContentPage
    {
        public const string EXCEPTION_MESSAGE = "Numar ore introduse gresit!";
        public const string NO_DATA_TEXT = "Nu a fost gasita nici o pozitie in timpul dat";

        private RoledTrackUser currentUser;
        private string tappedUsername;

        public SeeUserPage(RoledTrackUser currentUser, string tappedUsername)
        {
            InitializeComponent();

            this.currentUser = currentUser;
            this.tappedUsername = tappedUsername;

            BtnContinue.Clicked += async (s, a) => await BtnContinueListener(); 
        }

        public TKCustomMap _map { get; private set; }

        private async Task BtnContinueListener()
        {
            BtnContinue.IsEnabled = false;

            var activityIndicator = new ActivityIndicator
            {
                IsEnabled = true,
                IsRunning = true
            };
            Device.BeginInvokeOnMainThread(() => stackData.Children.Add(activityIndicator));

            try
            {
                var hours = Int32.Parse(ValidateNumberOfHours());
                var positions = await QueryPositions.QueryPositionsInLastHours(tappedUsername, hours);
                List<PositionDB> positionsList = positions?.ToList();

                if (positionsList == null || positionsList.Count == 0)
                {
                    //show message that there is no data
                    Device.BeginInvokeOnMainThread(() => stackData.Children.Remove(activityIndicator));
                    Device.BeginInvokeOnMainThread(() => DependencyService.Get<IMessage>().ShortAlert(NO_DATA_TEXT));
                }
                else
                {
                    //take the closest item from the given input
                    positionsList.Sort((a, b) => a.DateTime.CompareTo(b.DateTime));
                    Device.BeginInvokeOnMainThread(() => stackData.Children.Remove(activityIndicator));

                    //set binding context
                    var bindCont = new SingleUserModelView();
                    BindingContext = bindCont;

                    Device.BeginInvokeOnMainThread(() =>
                   {

                       //change view with map
                       this._map = new TKCustomMap(
               MapSpan.FromCenterAndRadius(
                  MapPage.DEFAULT_MAP_POSITION, Distance.FromMiles(ClientConsts.FROM_KM_MAP_DISTANCE)))
                       {
                           IsShowingUser = false,
                           HeightRequest = 100,
                           WidthRequest = 960,
                           VerticalOptions = LayoutOptions.FillAndExpand
                       };
                       var stack = new StackLayout { Spacing = 0 };
                       stack.Children.Add(_map);
                       Content = stack;

                       this._map.SetBinding(TKCustomMap.CirclesProperty, "UserCurrentPositions");
                       this._map.SetBinding(TKCustomMap.MapRegionProperty, "MapRegion");

                       // set position and circle in the binding context
                       Position pos = new Position(positionsList[0].Latitude, positionsList[0].Longitude);
                       bindCont.AddCircleAtPosition(pos);
                       bindCont.SetMapRegion(pos);

                   });
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
                DependencyService.Get<IMessage>().ShortAlert(ClientConsts.INTERNET_EXCEPTION_MESSAGE);
                Console.WriteLine(e.StackTrace);
            }
            finally
            {
                BtnContinue.IsEnabled = true;
                Device.BeginInvokeOnMainThread(() => stackData.Children.Remove(activityIndicator));
            }
        }

        private string ValidateNumberOfHours()
        {
            var text = EntryNumberOfHours.Text?.Trim();

            if (String.IsNullOrEmpty(text))
                throw new ValidationException(EXCEPTION_MESSAGE);

            var match = Regex.Match(text, @"^[0-9]+$");
            if (!match.Success)
                throw new ValidationException(EXCEPTION_MESSAGE);

            return text;
        }
    }
}