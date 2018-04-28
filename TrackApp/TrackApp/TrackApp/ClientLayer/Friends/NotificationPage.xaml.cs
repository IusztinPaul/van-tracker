using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackApp.DataFormat.UserData;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TrackApp.ClientLayer.Friends
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class NotificationPage : ContentPage
	{
        public const int NOTIFICATION_MAX_NUMBER = 6;
        public const string FOLLOW_TEXT = "te-a adaugat in lista de prieteni";
        public const string UNFOLLOW_TEXT = "te-a sters din lista de prieteni";

		public NotificationPage (TrackUser currentUser)
		{
			InitializeComponent ();
            BindingContext = new NotificationsModelView(currentUser);

            NotificationsList.ItemSelected += (source, args) => { NotificationsList.SelectedItem = null; };

        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            //populate view
            Device.BeginInvokeOnMainThread(async () => {
                await (BindingContext as NotificationsModelView)?.PopulateAsync();
                });
        }
    }
}