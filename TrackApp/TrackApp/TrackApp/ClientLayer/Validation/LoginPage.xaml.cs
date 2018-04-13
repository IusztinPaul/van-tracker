using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TrackApp.ClientLayer.Validation
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class LoginPage : ContentPage
	{
		public LoginPage ()
		{
			InitializeComponent ();

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
    }
}