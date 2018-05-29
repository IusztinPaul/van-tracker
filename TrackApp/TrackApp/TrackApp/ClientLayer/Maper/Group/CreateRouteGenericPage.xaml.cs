using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackApp.ClientLayer.CustomUI;
using TrackApp.ClientLayer.Exceptions;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TrackApp.ClientLayer.Maper.Group
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CreateRouteGenericPage : ContentPage
    {
        // first step of creating a route

        public const string MESSAGE_INVALID_GROUPNAME = "Nume grup invalid!";
        public const string MESSAGE_INVALID_COUNTRY = "Tara invalida!";
        public const string MESSAGE_INVALID_REGION = "Regiune invalida!";
        public const string MESSAGE_INVALID_CITY = "Oras invalid!";

        private string groupName;
        private string driverUsername;

        public CreateRouteGenericPage(string groupName, string driverUsername)
        {
            InitializeComponent();
            this.groupName = groupName;
            this.driverUsername = driverUsername;

            BtnProceed.Clicked += (s, a) =>
            {
                Device.BeginInvokeOnMainThread(async () =>
               {
                   ActivityIndicator actInd = new ActivityIndicator
                   {
                       IsRunning = true,
                       IsEnabled = true
                   };
                   StackLayContent.Children.Add(actInd);
                   BtnProceed.IsEnabled = false;

                   await BtnProceedListener(s, a);


                   StackLayContent.Children.Remove(actInd);
                   BtnProceed.IsEnabled = true;
               });
            };
        }


        private async Task BtnProceedListener(object source, EventArgs args)
        {

            try
            {
                var name = CheckForNull(EntryGroupName.Text?.Trim(), MESSAGE_INVALID_GROUPNAME);
                var country = CheckForNull(EntryCountry.Text?.Trim(), MESSAGE_INVALID_COUNTRY);
                var region = CheckForNull(EntryRegion.Text?.Trim(), MESSAGE_INVALID_REGION);
                var city = CheckForNull(EntryCity.Text?.Trim(), MESSAGE_INVALID_CITY);

                await Navigation.PushAsync(new CreateRoutePage(groupName, driverUsername, name, country, region, city));

            }
            catch (ValidationException e)
            {
                DependencyService.Get<IMessage>().ShortAlert(e.Message);
            }
        }

        private string CheckForNull(string data, string message)
        {
            if (String.IsNullOrEmpty(data))
                throw new ValidationException(message);

            return data;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            EntryGroupName.Text = "";
            EntryCountry.Text = "";
            EntryRegion.Text = "";
            EntryCity.Text = "";
        }
    }
}