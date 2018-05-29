using System;
using System.Text;
using System.IO;
using Amazon.CognitoIdentity;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Runtime;
using TrackApp.ClientLayer;
using TrackApp.ClientLayer.CustomUI;
using TrackApp.ClientLayer.Friends;
using TrackApp.ClientLayer.Profile;
using TrackApp.ClientLayer.Validation;
using TrackApp.DataFormat.UserData;
using TrackApp.ServerLayer;
using Xamarin.Forms;
using TrackApp.ClientLayer.Maper;
using TrackApp.ServerLayer.Save;
using System.Threading.Tasks;
using TrackApp.ClientLayer.Maper.Group;

namespace TrackApp
{
	public partial class App : Application
	{

	    public App ()
		{
			InitializeComponent();

             var user = new TrackUser()
            {
                Username = "PaulakaPaul",
                FirstName = "Paul",
                LastName = "Iusztin",
                Phone = "0732509516",
                Email = "p.e.iusztin@gmail.com",
                Location = new DataFormat.Location
                {
                    Country = "Romania",
                    Region = "Timis",
                    City = "Timisoara",
                    Street = "Horia",
                    Nr = "98",
                    Block = "-"
                }
            };

            var roledUser = new RoledTrackUser(user)
            {
                Role = "Administrator"
            };

          //    MainPage = new NavigationPage(new GroupTabbedPage(roledUser, "TestGroup2")); 
             MainPage = new NavigationPage(new LoginPage(user));
           // MainPage = new NavigationPage(new CreateRoutePage("TestGroup2", "Leppar"));
            // MainPage = new DemoPickPage();
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
            }
            catch (AmazonServiceException e) // if there are problems with the service or with the internet
            {
                Console.WriteLine("AmazonDynamoDBException CAUGHT: " + e.Message);
            }
            catch (Exception e) // in case of unexpected error 
            {
                Console.WriteLine("EXCEPTION COUGHT: " + e.Message);
                Console.WriteLine("TYPE: " + e.GetType());
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
