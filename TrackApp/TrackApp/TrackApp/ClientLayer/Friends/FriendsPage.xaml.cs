using System;
using System.Linq;
using TrackApp.DataFormat.UserData;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Collections.ObjectModel;
using TrackApp.ClientLayer.Profile;
using System.Collections.Generic;

namespace TrackApp.ClientLayer.Friends
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class FriendsPage : ContentPage
	{
        //TODO add activity indicator when a user clicks to go to a specific profile

        private TrackUser currentUser;

		public FriendsPage (TrackUser trackUser)
		{
			InitializeComponent ();
            this.currentUser = trackUser;
            BindingContext = new CurrentFriendsListViewModel(trackUser);

            FriendsList.ItemSelected += async (sender, args) =>
            {
                if (args.SelectedItem != null)
                {
                    ((ListView)sender).SelectedItem = null;
                    var tappedUser = args.SelectedItem as TrackUser;
                    if (tappedUser != null)
                    {
                        await Navigation.PushAsync(new NavigationPage(new ProfilePageNoToolbar(tappedUser)));
                    }
                }
            };
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            
            //populate view
            var bindcont = BindingContext as CurrentFriendsListViewModel;
            Device.BeginInvokeOnMainThread( async () =>
            {
                await bindcont.PopulateAsync();
            });

            SrcBarNames.Text = "";

        } 


        public void OnTextChanged(object source, TextChangedEventArgs e)
	    {
            var viewmodel = BindingContext as CurrentFriendsListViewModel; // get viewmodel from the bindingcontext

            if (viewmodel != null)
            {

                if (viewmodel.AllCurrentFriends == null)
                    viewmodel.AllCurrentFriends = new List<TrackUser>();

                //filter the data 
                if (String.IsNullOrEmpty(e.NewTextValue))
                {
                    viewmodel.CurrentUserFriends = new ObservableCollection<TrackUser>(viewmodel.AllCurrentFriends);
                }
                else
                {

                    viewmodel.CurrentUserFriends = new ObservableCollection<TrackUser>(viewmodel.AllCurrentFriends.Where(x => x.Username.ToUpper().StartsWith(e.NewTextValue.Trim().ToUpper())));
                }
            }
        }
        
	}
}