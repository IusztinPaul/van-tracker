using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackApp.DataFormat.UserData;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TrackApp.ClientLayer
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class NavigationMasterPage : ContentPage
	{
	    public const string USERNAME = "Nume cont: ";
	    public const string FULL_NAME = "Nume:";

        private TrackUser currentUser;
        public static NavigationMasterPage Instance = null;

        public NavigationMasterPage (TrackUser currentUser, MainPage mainPage)
		{

			InitializeComponent ();
            this.currentUser = currentUser;

		    listView.ItemSelected += (source, args) =>
		    {
		        mainPage.OnItemSelected(source, args);
		        listView.SelectedItem = null; // to not be clicked at the next look in the listview
		    };

            // set custom labels
		    if (currentUser != null)
		    {
		        UsernameLabel.Text = " " + currentUser.Username;
		        FullNameLabel.Text = String.Format(" {0} {1}", currentUser.FirstName, currentUser.LastName);
		    }

            if (Instance == null)
                Instance = this;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            //setup image source
            ChangeProfilePhotoWithImgSource(currentUser.IconSource);
        }

        public void ChangeProfilePhotoWithImgSource(ImageSource img)
        {
            ImgProfile.Source = img;
        }
    }
}