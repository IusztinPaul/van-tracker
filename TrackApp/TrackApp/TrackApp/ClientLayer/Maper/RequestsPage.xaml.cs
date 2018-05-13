using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TrackApp.ClientLayer.Maper
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class RequestsPage : ContentPage
	{
		public RequestsPage ()
		{
			InitializeComponent ();
            BindingContext = new RequestsViewModel();
		}
	}
}