using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TrackApp.DataFormat.UserData;
using TrackApp.ServerLayer.Query;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using TrackApp.ClientLayer.Extensions;
using System.Collections.ObjectModel;

namespace TrackApp.ClientLayer.Friends
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class FriendsPage : ContentPage
	{
        private TrackUser currentUser;

		public FriendsPage (TrackUser trackUser)
		{
			InitializeComponent ();
            this.currentUser = trackUser;
            BindingContext = new CurrentFriendsListViewModel(trackUser);

            FriendsList.ItemSelected += (source, args) => FriendsList.SelectedItem = null; // so the listview can't be selected
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
            
        } 


        public void OnTextChanged(object source, TextChangedEventArgs e)
	    {
            var viewmodel = BindingContext as CurrentFriendsListViewModel; // get viewmodel from the bindingcontext

            if (viewmodel != null)
            {
                //filter the data 
                if (String.IsNullOrEmpty(e.NewTextValue))
                {
                    viewmodel.CurrentUserFriends = new ObservableCollection<TrackUser>(viewmodel.AllCurrentFriends);
                }
                else
                {

                    viewmodel.CurrentUserFriends = new ObservableCollection<TrackUser>(viewmodel.AllCurrentFriends.Where(x => x.Username.ToUpper().StartsWith(e.NewTextValue.ToUpper())));
                }
            }
        }
	}
}