using System;
using TrackApp.DataFormat.UserData;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using TrackApp.DataFormat;
using TrackApp.ClientLayer.Profile;
using TrackApp.ServerLayer.Query;
using System.Threading.Tasks;
using TrackApp.ClientLayer.CustomUI;
using TrackApp.ServerLayer.Save;

namespace TrackApp.ClientLayer.Maper.Group
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UserRoutesPage : ContentPage
    {
        public UserRoutesPage(RoledUsername tappedUser, string groupName, RoledTrackUser currentUser)
        {
            InitializeComponent();

            //add create route logic only for the administators
            if (currentUser.Role.Equals(RoledTrackUser.TYPE_ADMINISTRATOR))
            {
                //toolbar items
                ToolbarItem addRoute = new ToolbarItem();
                addRoute.Text = ClientConsts.ADD_ROUTE_TOOL_BAR_TITLE;
                addRoute.Priority = 0;
                addRoute.Order = ToolbarItemOrder.Primary;
                addRoute.Icon = ClientConsts.ADD_ITEM_ICON;
                addRoute.Command = new Command(() =>
                {
                    Device.BeginInvokeOnMainThread(async () => await Navigation.PushAsync(new CreateRouteGenericPage(groupName, tappedUser.Username)));
                });

                //see driver location toolbar item 
                ToolbarItem seeDriverTbItem = new ToolbarItem();
                seeDriverTbItem.Text = ClientConsts.SEE_USER_POSITON_TOOL_BAR_TITLE;
                seeDriverTbItem.Priority = 0;
                seeDriverTbItem.Order = ToolbarItemOrder.Secondary;
                seeDriverTbItem.Command = new Command(async () =>
                {
                    await Navigation.PushAsync(new SeeUserPage(currentUser, tappedUser.Username));
                });

                //delete driver from group toolbar item 
                ToolbarItem deleteDriverTbItem = new ToolbarItem();
                deleteDriverTbItem.Text = ClientConsts.DELETE_DRIVER_FROM_GROUP_TOOL_BAR_TITLE;
                deleteDriverTbItem.Priority = 2;
                deleteDriverTbItem.Order = ToolbarItemOrder.Secondary;
                deleteDriverTbItem.Command = new Command(async () =>
                {
                    await DeleteDriverCommand(tappedUser.Username, groupName);
                });

                this.ToolbarItems.Add(addRoute);
                this.ToolbarItems.Add(seeDriverTbItem);
                this.ToolbarItems.Add(deleteDriverTbItem);

            }
            else if (currentUser.Role.Equals(RoledTrackUser.TYPE_NONE))
            {
                throw new Exception("RoledTrackUser.TYPE_NONE occured");
            }

            if (tappedUser.Role.Equals(RoledTrackUser.TYPE_ADMINISTRATOR))
                throw new Exception("Administrator has no routes!!!");

            //see profile tb item 
            ToolbarItem seeProfileTbItem = new ToolbarItem();
            seeProfileTbItem.Text = ClientConsts.SEE_PROFILE_ACTIVE_TOOL_BAR_TITLE;
            seeProfileTbItem.Priority = 1;
            seeProfileTbItem.Order = ToolbarItemOrder.Secondary;
            seeProfileTbItem.Command = new Command(async () =>
            {
                try
                {
                    var user = await QueryHashLoader.LoadData<TrackUser>(tappedUser.Username);
                    await Navigation.PushAsync(new ProfilePage(user));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.StackTrace);
                }
            });
            ToolbarItems.Add(seeProfileTbItem);

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

        private async Task DeleteDriverCommand(string username, string groupName)
        {
            var actInd = new ActivityIndicator
            {
                IsRunning = true,
                IsEnabled = true
            };
            contentStackLayout.Children.Insert(0, actInd);
            try
            {
                var group = await QueryHashLoader.LoadData<DataFormat.Group>(groupName);
                var userFriends = await QueryHashLoader.LoadData<UserFriends>(username);

                //delete user from group 
                if (group != null && group.ActiveDriverRoutes != null)
                    for (int i = 0; i < group.ActiveDriverRoutes.Count; i++)
                        if ((group.ActiveDriverRoutes[i].Split(ClientConsts.CONCAT_SPECIAL_CHARACTER[0])[0]).Trim().Equals(username))
                        {
                            group.ActiveDriverRoutes.RemoveAt(i);
                            break;
                        }

                if (group != null && group.Drivers != null)
                    for (int i = 0; i < group.Drivers.Count; i++)
                        if ((group.Drivers[i].Split(ClientConsts.CONCAT_SPECIAL_CHARACTER[0])[0]).Trim().Equals(username))
                        {
                            group.Drivers.RemoveAt(i);
                            break;
                        }

                //delete group reference from user friends 
                if (userFriends != null && userFriends.Groups != null)
                    for (int i = 0; i < userFriends.Groups.Count; i++)
                        if ((userFriends.Groups[i].Split(ClientConsts.CONCAT_SPECIAL_CHARACTER[0])[0]).Trim().Equals(groupName))
                        {
                            userFriends.Groups.RemoveAt(i);
                            break;
                        }

                //save updated data
                var saver = new MultiTableBatchGroupsUserFriends
                {
                    Group = group,
                    UserFriends = userFriends
                };
                await saver.SaveData();

                Device.BeginInvokeOnMainThread(async () => await Navigation.PopAsync());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
                DependencyService.Get<IMessage>().LongAlert("Nu s-a putut procesa cererea.");
            }
            finally
            {
                contentStackLayout.Children.Remove(actInd);
            }
        }
    }
}