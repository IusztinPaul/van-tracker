using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using TrackApp.DataFormat;
using TrackApp.DataFormat.UserData;
using TrackApp.ClientLayer.CustomUI;

namespace TrackApp.ClientLayer.Maper.Group
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SingleRoutePage : ContentPage
	{
        public const string NO_PERMISSION_TEXT = "Nu ai permisii de a edita ruta!";

		public SingleRoutePage (RouteInfo routeInfo, RoledTrackUser currentUser)
		{
			InitializeComponent ();
            BindingContext = new SingleRouteViewModel(routeInfo);

            AddressesListView.ItemSelected += (source, args) =>
            {
                if (args.SelectedItem != null)
                {
                    var item = args.SelectedItem as Route;
                    if (item != null)
                    {
                        if (currentUser.Role.Equals(RoledTrackUser.TYPE_ADMINISTRATOR))
                            Device.BeginInvokeOnMainThread(async () => await Navigation.PushAsync(new EditAddressPage(item)));
                        else
                            DependencyService.Get<IMessage>().LongAlert(NO_PERMISSION_TEXT);
                    }
                    (source as ListView).SelectedItem = null;
                }
            };

        }



        protected override void OnAppearing()
        {
            base.OnAppearing();

            //populate
            var bindCont = BindingContext as SingleRouteViewModel;
            if (bindCont != null)
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await bindCont.PopulateAsync();
                });
        }
    }
}