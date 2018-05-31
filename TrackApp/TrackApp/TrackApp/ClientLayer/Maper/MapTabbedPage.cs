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
            Title = ClientConsts.MAPER_PAGE_TITLE;

            //creating pages 
            var groupsPage = new MapGroupsPage(currentUser);
            groupsPage.Title = ClientConsts.MAPER_PAGE_TITLE;

            var requestsPage = new RequestsPage(currentUser);
            requestsPage.Title = ClientConsts.REQUEST_PAGE_TITLE;

            //adding pages to the tabbed page
            Children.Add(groupsPage);
            Children.Add(requestsPage);

            //toolbar items
            ToolbarItem addGroup = new ToolbarItem();
            addGroup.Text = ClientConsts.TOOL_BAR_ADD_GROUP;
            addGroup.Priority = 0;
            addGroup.Order = ToolbarItemOrder.Primary;
            addGroup.Icon = ClientConsts.ADD_ITEM_ICON;
            addGroup.Command = new Command(() => Navigation.PushAsync(new CreateGroupPage(currentUser)));

            this.ToolbarItems.Add(addGroup);
        }
    }
}
