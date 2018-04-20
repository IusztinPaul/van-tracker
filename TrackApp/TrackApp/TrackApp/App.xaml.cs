using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrackApp.ClientLayer;
using TrackApp.ClientLayer.Validation;
using Xamarin.Forms;

namespace TrackApp
{
	public partial class App : Application
	{
	    public static MasterDetailPage MasterDetailPage;

	    public App ()
		{
			InitializeComponent();

            // MainPage = new NavigationPage(new ClientLayer.Validation.LoginPage());

            /*
            MasterDetailPage = new MasterDetailPage
		    {
		        Master = new NavigationMasterPage(),
		        Detail = new NavigationPage(new MapGroupsPage())

		    }; */

		    MainPage = new NavigationPage(new LoginPage());
		}

		protected override void OnStart ()
		{
			// Handle when your app starts
		}

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}
	}
}
