using System;
using Amazon.DynamoDBv2;
using Amazon.Runtime;
using TrackApp.ClientLayer;
using TrackApp.ClientLayer.CustomUI;
using TrackApp.ClientLayer.Friends;
using TrackApp.ClientLayer.Profile;
using TrackApp.ClientLayer.Validation;
using TrackApp.DataFormat.UserData;
using TrackApp.ServerLayer;
using Xamarin.Forms;

namespace TrackApp
{
	public partial class App : Application
	{
	    public static MasterDetailPage MasterDetailPage;

	    public App ()
		{
			InitializeComponent();

            
             // MainPage = new ProfilePage(new TrackUser() { Username = "PaulCelMare" });
              MainPage = new NavigationPage(new LoginPage(new TrackUser(){Username = "PaulCelMare"}));
		}

		protected override void OnStart ()
		{
            // Handle when your app starts

            //make connection with the dynamo DB
            try
            {
                AwsUtils.GetContext();

            }
            catch (AmazonDynamoDBException e) // problems with the service
            {
                Console.WriteLine("AmazonDynamoDBException CAUGHT: " + e.Message);
                DependencyService.Get<IMessage>().ShortAlert(ClientConsts.DYNAMODB_EXCEPTION_MESSAGE2);
            }
            catch (AmazonServiceException e) // if there are problems with the service or with the internet
            {
                DependencyService.Get<IMessage>().ShortAlert(ClientConsts.DYNAMODB_EXCEPTION_MESSAGE2);
            }
            catch (Exception e) // in case of unexpected error 
            {
                Console.WriteLine("EXCEPTION COUGHT: " + e.Message);
                Console.WriteLine("TYPE: " + e.GetType());
                DependencyService.Get<IMessage>().LongAlert(e.Message);
            }


            
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
