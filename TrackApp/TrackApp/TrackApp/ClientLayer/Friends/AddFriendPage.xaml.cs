using System;
using System.Collections.ObjectModel;
using System.Linq;
using TrackApp.ClientLayer.Profile;
using TrackApp.DataFormat.UserData;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TrackApp.ClientLayer.Friends
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class AddFriendPage : ContentPage
	{



        public AddFriendPage(TrackUser currentUser)
        {
            InitializeComponent();
            BindingContext = new FriendsListViewModel(currentUser);

            UsersList.ItemSelected += async (sender, args) =>
            {
                if (args.SelectedItem != null)
                {
                    ((ListView)sender).SelectedItem = null;

                    var tappedUser = args.SelectedItem as TrackUser;
                    if (tappedUser != null)
                    {
                        await Navigation.PushAsync(new NavigationPage(new ProfilePageNoToolbar(tappedUser)));
                    }
                      
                };

            };
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            //populate view
            var bindCont = BindingContext as FriendsListViewModel;
           Device.BeginInvokeOnMainThread(async () =>
           {
               await bindCont.PopulateAsync();
           });
        }

        public void OnTextChanged(object source, TextChangedEventArgs e)
        {
            var viewmodel = BindingContext as FriendsListViewModel; // get viewmodel from the bindingcontext

            if (viewmodel != null)
            {
                //filter the data 
                if (String.IsNullOrEmpty(e.NewTextValue))
                {
                    viewmodel.TrackUsers = new ObservableCollection<TrackUser>(viewmodel.GetAllUsers());
                }
                else
                {

                    viewmodel.TrackUsers = new ObservableCollection<TrackUser>(viewmodel.GetAllUsers().Where(x => x.Username.ToUpper().StartsWith(e.NewTextValue.ToUpper())));
                }
            }

        }

    }
}