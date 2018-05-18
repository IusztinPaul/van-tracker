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
	public partial class GroupMembersPage : ContentPage
	{
		public GroupMembersPage (RoledTrackUser currentUser, string groupName)
		{
			InitializeComponent ();
            BindingContext = new GroupPageViewModel(currentUser, groupName);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            //populate
            var bindCont = BindingContext as GroupPageViewModel;
            if (bindCont != null)
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await bindCont.PopulateAsync();
                });

            SrcBarNames.Text = "";
        }

        public void OnTextChanged(object source, TextChangedEventArgs e)
        {
            var viewmodel = BindingContext as GroupPageViewModel;
            viewmodel?.SearchBarListener(e.NewTextValue);
        }
    }
}