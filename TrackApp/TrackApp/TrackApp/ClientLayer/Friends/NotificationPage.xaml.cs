using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackApp.ClientLayer.CustomUI;
using TrackApp.ClientLayer.Profile;
using TrackApp.DataFormat.UserData;
using TrackApp.ServerLayer.Query;
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

            NotificationsList.ItemSelected += async (sender, args) =>
            {
                if (args.SelectedItem != null)
                {
                    var tappedUserNotif = args.SelectedItem as Notification;
                    if (tappedUserNotif != null)
                    {
                        ((ListView)sender).SelectedItem = null;

                        try
                        {
                            var tappedUser = await new QueryUser().LoadData<TrackUser>(tappedUserNotif.Username);
                            await Navigation.PushAsync(new NavigationPage(new ProfilePageNoToolbar(tappedUser)));
                        }
                        catch (Exception e)
                        {
                            DependencyService.Get<IMessage>().LongAlert("Cerere anulata. Incearca din nou in cateva momente!");
                        }

                    }
                   
                }
            };

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