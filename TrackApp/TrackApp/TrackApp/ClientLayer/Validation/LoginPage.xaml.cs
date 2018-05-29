using System;
using System.Net;
using System.Threading.Tasks;
using Amazon.Runtime;
using TrackApp.ClientLayer.CustomUI;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using TrackApp.ClientLayer.Exceptions;
using TrackApp.DataFormat.UserData;
using TrackApp.ServerLayer.Query;

namespace TrackApp.ClientLayer.Validation
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class LoginPage : ContentPage
	{
        //TODO add custom renderer for the progress bar 
        //TODO change exception message to public const string INTERNET_EXCEPTION_MESSAGE = "Nu este internet!"; from ClientConsts
        //TODO if no internet show message instead of login page
        //TODO save login state into a local db and send the currentUser to the main page if it already logged in from another time

        public LoginPage (TrackUser currentUser)
		{
			InitializeComponent ();

            // logic button listener
		    BtnLogin.Clicked += async (source, args) =>
		    {
                //reset the progess bar 
		        ProgBarLogBtn.Progress = 0d;
		        ProgBarLogBtn.IsVisible = true;

                await BtnLoginClickListener();

                // make the prog bar invisible
		        ProgBarLogBtn.IsVisible = false;
            };

		    // click listener for the bottom label
		    LabelGoToSignUp.GestureRecognizers.Add(
		        new TapGestureRecognizer()
		        {
		            Command = new Command( async () => await OnLabelGoToSignUpClicked())
		        });

            //logout logic 
		    if (currentUser != null)
		    {
		        EntryUsername.Text = currentUser.Username;
		        EntryPassword.Text = currentUser.Password;
            }
		    else
		    {
		        //escape some internal null exception
		        EntryUsername.Text = "";
		        EntryPassword.Text = "";
            }
           

		}

        private async Task OnLabelGoToSignUpClicked()
	    {
            //first ask for permissions
            await PermissionsCaller.PermissionLocationCaller(this);
            await PermissionsCaller.PermissionStorageCaller(this);

            await Navigation.PushAsync(new SignUpPage());
	    }

	    private async Task<TrackUser> GetUserFromEntry()
	    {
	        string username = EntryUsername.Text.Trim();

	        if (String.IsNullOrEmpty(username))
	            throw new ValidationException("Introduce cont!");

	        QueryUser query = new QueryUser();
	        var user = await query.LoadData<TrackUser>(username);

	        if (user == null) // username is unique 
	            throw new ValidationException("Contul nu exista!");

	        return user;
	    }

	    private void CheckPasswordForUser(TrackUser user)
	    {
	        string password = EntryPassword.Text.Trim();
	        if (String.IsNullOrEmpty(password))
	            throw new ValidationException("Introduce parola!");

	        if (!password.Equals(user.Password))
	            throw new ValidationException("Parola incorecta!");
	    }

	    private async Task BtnLoginClickListener()
	    {
            BtnLogin.IsEnabled = false;

            double progBarIncrementRate = 1d / 2d;
	        double progBarCurentValue = 0d;

            //first ask for permissions
            await PermissionsCaller.PermissionLocationCaller(this);
            await PermissionsCaller.PermissionStorageCaller(this);

            try
	        {
	            // validate the data and show progress bar animation
	            var user = await GetUserFromEntry();
	            progBarCurentValue += progBarIncrementRate;
	            await InscreaseProgBar(progBarCurentValue);

	            CheckPasswordForUser(user);
	            progBarCurentValue += progBarIncrementRate;
	            await InscreaseProgBar(progBarCurentValue);

	            Application.Current.MainPage = new MainPage(user);
	        }
	        catch (AmazonServiceException e) // if there are problems with the service or with the internet
	        {
	            DependencyService.Get<IMessage>().ShortAlert(ClientConsts.DYNAMODB_EXCEPTION_MESSAGE2);
	        }
	        catch (ValidationException e) // display error message to currentUser
	        {
	            DependencyService.Get<IMessage>().ShortAlert(e.Message);
	        }
	        catch (WebException e)
	        {
	            DependencyService.Get<IMessage>().LongAlert(ClientConsts.DYNAMODB_EXCEPTION_MESSAGE1);
            }
	        catch (Exception e) // in case of unexpected error 
            {
                Console.WriteLine("EXCEPTION COUGHT: " + e.Message);
                Console.WriteLine("TYPE: " + e.GetType());
                DependencyService.Get<IMessage>().LongAlert(e.Message);
            }
            finally
            {
                BtnLogin.IsEnabled = true;
            }

        }

	    private async Task InscreaseProgBar(double currentValue)
	    {
	        await ProgBarLogBtn.ProgressTo(currentValue, 250, Easing.Linear);
	    }
    }
}