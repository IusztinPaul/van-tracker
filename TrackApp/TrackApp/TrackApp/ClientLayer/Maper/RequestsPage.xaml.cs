using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackApp.ClientLayer.CustomUI;
using TrackApp.DataFormat.UserData;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TrackApp.ClientLayer.Maper
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class RequestsPage : ContentPage
	{
        public const int MAX_SIZE_REQUEST_NOTIFICATIONS = 40;
        public const string SELECTED_ITEM_TEXT = "accepta/refuza";

		public RequestsPage (TrackUser currentUser)
		{
			InitializeComponent ();
            BindingContext = new RequestsViewModel(currentUser, this);

            UserGroupRequests.ItemSelected += (s, a) =>
            {
                if(a.SelectedItem != null)
                {
                    Device.BeginInvokeOnMainThread(() => DependencyService.Get<IMessage>().ShortAlert(SELECTED_ITEM_TEXT));
                    (s as ListView).SelectedItem = null;
                }
            };

        }


        protected override void OnAppearing()
        {
            base.OnAppearing();

            //populate view
            var bindcont = BindingContext as RequestsViewModel;
            Device.BeginInvokeOnMainThread(async () =>
            {
                await bindcont.PopulateAsync();
            });

        }

        public void StartActivityIndicator()
        {
            AcIndicatorOnAdd.IsRunning = true;
            AcIndicatorOnAdd.IsVisible = true;

            UserGroupRequests.IsVisible = false;
            UserGroupRequests.HeightRequest = 0;
        }

        public void PauseActivityIndicator()
        {
            AcIndicatorOnAdd.IsRunning = false;
            AcIndicatorOnAdd.IsVisible = false;

            UserGroupRequests.IsVisible = true;
            UserGroupRequests.HeightRequest = -1;
        }
    }
}