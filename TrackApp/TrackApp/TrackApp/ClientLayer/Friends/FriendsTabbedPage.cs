using System;
using System.Collections.Generic;
using System.Text;
using TrackApp.ClientLayer.Maper;
using TrackApp.DataFormat.UserData;
using Xamarin.Forms;

namespace TrackApp.ClientLayer.Friends
{
    public class FriendsTabbedPage : TabbedPage
    {

        public FriendsTabbedPage(TrackUser currentUser)
        {
            Title = ClientConsts.FRIENDS_PAGE_TITLE;

            //creating pages 
            var friendsPage = new FriendsPage(currentUser);
            friendsPage.Title = ClientConsts.TAB_FRIENDS_PAGE;

            var notificationPage = new NotificationPage(currentUser);
            notificationPage.Title = ClientConsts.TAB_NOTIFICATION_PAGE;

            //adding pages to the tabbed page
            Children.Add(friendsPage);
            Children.Add(notificationPage);

            //toolbar items
            ToolbarItem addFriend = new ToolbarItem();
            addFriend.Text = ClientConsts.TOOL_BAR_ADD_FRIEND;
            addFriend.Priority = 0;
            addFriend.Order = ToolbarItemOrder.Primary;
            addFriend.Icon = ClientConsts.ADD_ITEM_ICON;
            addFriend.Command = new Command(() => Navigation.PushAsync(new AddFriendPage(currentUser)));
            
            this.ToolbarItems.Add(addFriend);
        }
    }
}
