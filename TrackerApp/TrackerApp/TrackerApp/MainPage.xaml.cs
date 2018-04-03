using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

using TrackerApp.ServerLayer;

namespace TrackerApp
{
	public partial class MainPage : ContentPage
	{
		public MainPage()
		{
			InitializeComponent();
		    btnTest.Clicked += (a, b) => { ServerS.GetInstance(); };
		}
	}
}
