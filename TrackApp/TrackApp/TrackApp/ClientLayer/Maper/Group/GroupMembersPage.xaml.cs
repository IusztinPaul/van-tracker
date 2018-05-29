using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackApp.ClientLayer.CustomUI;
using TrackApp.DataFormat.UserData;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TrackApp.ClientLayer.Maper.Group
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class GroupMembersPage : ContentPage
	{
        public const string MESSAGE_ADMINISTRATOR_TAPPED = "Administratorul nu are si nu poate primi rute!";

        private string groupName;
        private RoledTrackUser currentUser;

        public GroupMembersPage (RoledTrackUser currentUser, string groupName)
		{
			InitializeComponent ();
            BindingContext = new GroupPageViewModel(currentUser, groupName);
            this.groupName = groupName;
            this.currentUser = currentUser;

            MembersList.ItemSelected += async (source, args) => await OnListItemSelected(source, args);
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

        private async Task OnListItemSelected(object sender, SelectedItemChangedEventArgs args)
        {
            if (args.SelectedItem != null)
            {
                var item = args.SelectedItem as RoledUsername;
                if (item != null)
                {
                    if (item.Role.Equals(RoledTrackUser.TYPE_ADMINISTRATOR))
                    {
                        DependencyService.Get<IMessage>().LongAlert(MESSAGE_ADMINISTRATOR_TAPPED);
                    }
                    else if(RoledTrackUser.TYPE_DRIVER.Equals(item.Role))
                    {
                        await Navigation.PushAsync(new UserRoutesPage(item, groupName, currentUser));
                    }
                    else
                    {
                        throw new Exception("There is no such options. Problems in your logic. NO more than driver or administrator roles!");
                    }
                }
                (sender as ListView).SelectedItem = null;
            }
        }
    }
}