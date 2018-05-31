using System;
using System.Collections.Generic;
using System.Text;
using TrackApp.ClientLayer.Exceptions;
using TrackApp.DataFormat.UserData;
using Xamarin.Forms;

namespace TrackApp.ClientLayer.Maper.Group
{
    public class GroupTabbedPage : TabbedPage
    {

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
                addFriend.Command = new Command(() => Navigation.PushAsync(new AddMemberPage(currentUser, groupName)));

                this.ToolbarItems.Add(addFriend);
            }
            else if (currentUser.Role == RoledTrackUser.TYPE_NONE)
            {
                throw new TypeException("RoledTrackUser.TYPE_NONE occured");
            }
        }
    }
}
