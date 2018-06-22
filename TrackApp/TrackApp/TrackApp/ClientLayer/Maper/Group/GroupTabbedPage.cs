using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TK.CustomMap;
using TrackApp.ClientLayer.CustomUI;
using TrackApp.ClientLayer.Exceptions;
using TrackApp.ClientLayer.Maper.Group.MapN;
using TrackApp.DataFormat.UserData;
using TrackApp.ServerLayer.Query;
using Xamarin.Forms;

namespace TrackApp.ClientLayer.Maper.Group
{
    public class GroupTabbedPage : TabbedPage
    {
        public static readonly string NO_MORE_DRIVERS_ALERT_TEXT = $"Numarul maxim de {ClientConsts.MAX_DRIVERS_IN_GROUP} soferi a fost atins!";

        public GroupTabbedPage(RoledTrackUser currentUser, string groupName)
        {
            Title = ClientConsts.GROUP_TITLE + " " + groupName;

            //creating pages 
            var membersPage = new GroupMembersPage(currentUser, groupName);
            membersPage.Title = ClientConsts.MEMBERS_PAGE_TITLE;

            var notificationsPage = new GroupNotificationsPage(currentUser, groupName);
            notificationsPage.Title = ClientConsts.NOTIFICATIONS_PAGE_TITLE;

            var generalGroupPage = new GeneralGroupPage(currentUser, groupName);
            generalGroupPage.Title = ClientConsts.GENERAL_GROUP_PAGE_TITLE;

            //adding pages to the tabbed page
            Children.Add(generalGroupPage);
            Children.Add(membersPage);
            Children.Add(notificationsPage);

            if (currentUser.Role == RoledTrackUser.TYPE_ADMINISTRATOR)
            {
                //toolbar items
                ToolbarItem addFriend = new ToolbarItem();
                addFriend.Text = ClientConsts.TOOL_BAR_ADD_GROUP;
                addFriend.Priority = 0;
                addFriend.Order = ToolbarItemOrder.Primary;
                addFriend.Icon = ClientConsts.ADD_ITEM_ICON;
                addFriend.Command = new Command(async () => {
                    var numberOfDrivers = await GetNumberOfDrivers(groupName);
                    if (numberOfDrivers != null && numberOfDrivers < ClientConsts.MAX_DRIVERS_IN_GROUP)
                        await Navigation.PushAsync(new AddMemberPage(currentUser, groupName));
                    else
                        Device.BeginInvokeOnMainThread(() => DependencyService.Get<IMessage>().LongAlert(NO_MORE_DRIVERS_ALERT_TEXT));
                    });

                this.ToolbarItems.Add(addFriend);
            }
            else if (currentUser.Role == RoledTrackUser.TYPE_NONE)
            {
                throw new TypeException("RoledTrackUser.TYPE_NONE occured");
            }
        }

        private async Task<int?> GetNumberOfDrivers(string groupName)
        {
            try
            {
                var group = await QueryHashLoader.LoadData<DataFormat.Group>(groupName);

                if (group != null && group.Drivers != null)
                    return group.Drivers.Count;

                if (group.Drivers == null)
                    return 0;

            }catch(Exception e)
            {
                Console.WriteLine(e.StackTrace); 
            }

            return null;
        }
    }
}
