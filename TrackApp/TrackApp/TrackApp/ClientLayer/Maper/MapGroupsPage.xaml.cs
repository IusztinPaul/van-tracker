using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackApp.DataFormat.UserData;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TrackApp.ClientLayer.Maper
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MapGroupsPage : ContentPage
	{
		public MapGroupsPage (TrackUser currentUser)
		{
			InitializeComponent ();
		}
	}
}