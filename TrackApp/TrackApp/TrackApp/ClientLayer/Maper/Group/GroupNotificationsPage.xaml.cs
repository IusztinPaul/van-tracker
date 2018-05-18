using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackApp.DataFormat.UserData;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TrackApp.ClientLayer.Maper.Group
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class GroupNotificationsPage : ContentPage
	{
		public GroupNotificationsPage (TrackUser currentUser, string groupName)
		{
			InitializeComponent ();
            BindingContext = new GroupNotificationPageViewModel(currentUser, groupName);
		}

        protected override void OnAppearing()
        {
            base.OnAppearing();

            //populate view
            var bindcont = BindingContext as GroupNotificationPageViewModel;
            Device.BeginInvokeOnMainThread(async () =>
            {
                await bindcont.PopulateAsync();
            });
            
        }
    }
}