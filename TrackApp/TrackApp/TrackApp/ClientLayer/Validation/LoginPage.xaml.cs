using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
		public LoginPage ()
		{
			InitializeComponent ();

            // logic button listener
		    BtnLogin.Clicked += (source, args) =>
		    {
		        BtnLoginClickListener();
		    };

		    // click listener for the bottom label
		    LabelGoToSignUp.GestureRecognizers.Add(
		        new TapGestureRecognizer()
		        {
		            Command = new Command(() => OnLabelGoToSignUpClicked())
		        });
        }

	    private async Task OnLabelGoToSignUpClicked()
	    {
	        await Navigation.PushAsync(new NavigationPage(new SignUpPage()));
	    }

	    private async Task<TrackUser> GetUserFromEntry()
	    {
	        string username = EntryUsername.Text.Trim();

	        if (String.IsNullOrEmpty(username))
	            throw new ValidationException("Introduce cont!");

	        QueryUser query = new QueryUser();
	        var user = await query.LoadData(username);

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
	        try
	        {
	            var user = await GetUserFromEntry();
                CheckPasswordForUser(user);

                Application.Current.MainPage = new MainPage(user);
	        }
	        catch (ValidationException e) // display error message to user
	        {
                DependencyService.Get<IMessage>().ShortAlert(e.Message);
	        }

	    }
    }
}