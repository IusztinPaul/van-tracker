using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TrackerApp.ClientLayer.Validation
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class LoginPage : ContentPage
	{
		public LoginPage ()
		{
			InitializeComponent ();
		  
		  
        }

	    void OnSliderValueChanged(object sender, ValueChangedEventArgs args)
	    {
	      //  ValueLabel.Text = args.NewValue.ToString("F5");
	    }

	    async void OnButtonClicked(object sender, EventArgs args)
	    {
	        Button button = (Button)sender;
	        await DisplayAlert("Clicked!",
	            "The button labeled '" + button.Text + "' has been clicked",
	            "OK");
	    }
    }
}