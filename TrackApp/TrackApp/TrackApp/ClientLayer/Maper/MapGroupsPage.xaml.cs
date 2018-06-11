using System.Threading.Tasks;
using TrackApp.ClientLayer.CustomUI;
using TrackApp.ClientLayer.Maper.Group;
using TrackApp.DataFormat.UserData;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Plugin.Permissions.Abstractions;

namespace TrackApp.ClientLayer.Maper
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MapGroupsPage : ContentPage
    {
        private TrackUser currentUser;

        public MapGroupsPage(TrackUser currentUser)
        {
            InitializeComponent();

            BindingContext = new MapGroupsViewModel(currentUser);
            this.currentUser = currentUser;

            GroupsList.ItemSelected += async (source, args) => await OnItemSelected(source, args);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            //populate
            var bindCont = BindingContext as MapGroupsViewModel;
            if (bindCont != null)
                Device.BeginInvokeOnMainThread(async () =>
               {
                   await bindCont.PopulateAsync();
               });

            SrcBarNames.Text = "";
        }

        public void OnTextChanged(object source, TextChangedEventArgs e)
        {
            var viewmodel = BindingContext as MapGroupsViewModel;
            viewmodel?.SearchBarListener(e.NewTextValue);
        }


        public async Task OnItemSelected(object sender, SelectedItemChangedEventArgs args)
        {
            if (args.SelectedItem != null)
            {
                var item = args.SelectedItem as GroupListViewWrapper;
                if (item != null)
                {
                    var roledUser = new RoledTrackUser(currentUser)
                    {
                        Role = item.Type == MapGroupsViewModel.ADMINISTRATOR_DETAIL ? RoledTrackUser.TYPE_ADMINISTRATOR :
                                    item.Type == MapGroupsViewModel.DRIVER_DETAIL ? RoledTrackUser.TYPE_DRIVER : RoledTrackUser.TYPE_NONE
                    };

                    DependencyService.Get<IMessage>().ShortAlert(ClientConsts.START_PROCESS_SIGNAL);

                    //ask for permissions
                    var result = await PermissionsCaller.PermissionLocationCaller(this);

                    if (result.Equals(PermissionStatus.Granted))
                    {
                        if (App.locationServiceController.IsLoopRunning()) // if it was started before stop it
                            App.locationServiceController.StopLocationLooper();

                        if(!App.locationServiceController.IsLoopRunning() && roledUser.Role.Equals(RoledTrackUser.TYPE_DRIVER))
                            App.locationServiceController.StartLocationLoop(roledUser.Username); //start if fresh always with the current user

                        //navigate to the group page
                        await Navigation.PushAsync(new GroupTabbedPage(roledUser, item.Name));
                    }
                    else
                        await DisplayAlert("Atentie", "Nu va putem lasa sa accesati grupurile fara acces la locatia dumneavoastra!", "Ok");
                }
                
                    (sender as ListView).SelectedItem = null;
            }
        }
    }
}