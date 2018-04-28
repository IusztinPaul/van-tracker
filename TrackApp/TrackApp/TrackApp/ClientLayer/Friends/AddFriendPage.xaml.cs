using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackApp.DataFormat.UserData;
using TrackApp.ServerLayer.Query;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TrackApp.ClientLayer.Friends
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class AddFriendPage : ContentPage
	{
	   
	   

        public AddFriendPage (TrackUser currentUser)
		{
			InitializeComponent ();
		    BindingContext = new FriendsListViewModel(currentUser);

             UsersList.ItemSelected += (source, args) => UsersList.SelectedItem = null; // so the listview can't be selected
 
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