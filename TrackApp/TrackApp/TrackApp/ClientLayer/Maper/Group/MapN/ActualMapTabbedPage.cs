using System;
using System.Collections.Generic;
using System.Text;
using TrackApp.DataFormat.UserData;
using Xamarin.Forms;

namespace TrackApp.ClientLayer.Maper.Group.MapN
{
    public class ActualMapTabbedPage : TabbedPage
    {
        private RoledTrackUser currentUser;

        public ActualMapTabbedPage(RoledTrackUser currentUser, string groupName, RoledTrackUser[] mapUsers)
        {
            this.currentUser = currentUser;

            Title = ClientConsts.MAP_TABBED_PAGE_TITLE;

            var mapPage = new MapPage(currentUser, groupName, mapUsers);
            mapPage.Title = ClientConsts.MAP_PAGE_TITLE;

            var activeRoutePage = new ActiveRoutePage(currentUser, groupName);
            activeRoutePage.Title = ClientConsts.MAP_ACTIVE_ROUTE_TITLE;

            Children.Add(mapPage);
            Children.Add(activeRoutePage);
        }
    }
}
