using System;
using System.Collections.Generic;
using System.Text;
using TrackApp.DataFormat.UserData;
using Xamarin.Forms;

namespace TrackApp.ClientLayer.Maper
{
    public class MapTabbedPage : TabbedPage
    {

        public MapTabbedPage(TrackUser currentUser)
        {
            Title = ClientConsts.MAP_PAGE_TITLE;

            //creating pages 
            var groupsPage = new MapGroupsPage(currentUser);
            groupsPage.Title = ClientConsts.MAP_PAGE_TITLE;

            var requestsPage = new RequestsPage();
            requestsPage.Title = ClientConsts.REQUEST_PAGE_TITLE;

            //adding pages to the tabbed page
            Children.Add(groupsPage);
            Children.Add(requestsPage);

            //toolbar items
            ToolbarItem addFriend = new ToolbarItem();
            addFriend.Text = ClientConsts.TOOL_BAR_ADD_GROUP;
            addFriend.Priority = 0;
            addFriend.Order = ToolbarItemOrder.Primary;
            addFriend.Icon = ClientConsts.ADD_ITEM_ICON;
            addFriend.Command = new Command(() => Navigation.PushAsync(new CreateGroupPage(currentUser)));

            this.ToolbarItems.Add(addFriend);
        }
    }
}
