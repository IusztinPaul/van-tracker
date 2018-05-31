using System;
using TrackApp.DataFormat.UserData;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using TrackApp.DataFormat;

namespace TrackApp.ClientLayer.Maper.Group
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class UserRoutesPage : ContentPage
	{
		public UserRoutesPage(RoledUsername tappedUser, string groupName, RoledTrackUser currentUser)
		{
			InitializeComponent ();

            //add create route logic only for the administators
            if (currentUser.Role.Equals(RoledTrackUser.TYPE_ADMINISTRATOR))
            {
                //toolbar items
                ToolbarItem addRoute = new ToolbarItem();
                addRoute.Text = ClientConsts.ADD_ROUTE_TOOL_BAR_TITLE;
                addRoute.Priority = 0;
                addRoute.Order = ToolbarItemOrder.Primary;
                addRoute.Icon = ClientConsts.ADD_ITEM_ICON;
                addRoute.Command = new Command( () => {
                    Device.BeginInvokeOnMainThread(async () => await Navigation.PushAsync(new CreateRouteGenericPage(groupName, tappedUser.Username)));
                    });

                this.ToolbarItems.Add(addRoute);
            }
            else if (currentUser.Role.Equals(RoledTrackUser.TYPE_NONE))
            {
                throw new Exception("RoledTrackUser.TYPE_NONE occured");
            }

            if (tappedUser.Role.Equals(RoledTrackUser.TYPE_ADMINISTRATOR))
                throw new Exception("Administrator has no routes!!!");

            BindingContext = new UserRoutesViewModel(currentUser, tappedUser, groupName);

            RoutesList.ItemSelected += async (source, args) =>
            {
                if (args.SelectedItem != null)
                {
                    var item = args.SelectedItem as RouteInfo;
                    if (item != null)
                        await Navigation.PushAsync(new SingleRoutePage(item, currentUser, groupName, tappedUser.Username));

                    (source as ListView).SelectedItem = null;
                }
            };
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            //populate
            var bindCont = BindingContext as UserRoutesViewModel;
            if (bindCont != null)
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await bindCont.PopulateAsync();
                });
        }
    }
}