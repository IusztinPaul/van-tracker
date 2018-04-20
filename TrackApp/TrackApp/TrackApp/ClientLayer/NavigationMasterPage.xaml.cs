using System;
using System.Collections.Generic;
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

        public NavigationMasterPage (TrackUser user, MainPage mainPage)
		{

			InitializeComponent ();

		    listView.ItemSelected += (source, args) =>
		    {
		        mainPage.OnItemSelected(source, args);
		        listView.SelectedItem = null; // to not be clicked at the next look in the listview
		    };

            // set custom labels
		    UsernameLabel.Text = " " + user.Username;
		    FullNameLabel.Text = String.Format(" {0} {1}",user.FirstName, user.LastName);
		}


	}
}