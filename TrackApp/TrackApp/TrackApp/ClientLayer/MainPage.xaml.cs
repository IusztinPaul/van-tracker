using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

using TrackApp.DataFormat;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2;
using Amazon.CognitoIdentity;
using Amazon;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.Runtime;
using TrackApp.ServerLayer.Save;
using TrackApp.DataFormat.UserData;
using TrackApp.ServerLayer;
using TrackApp.ServerLayer.Query;

namespace TrackApp.ClientLayer
{
    
    public class MainPage : MasterDetailPage
    {
        private readonly TrackUser user;
        public MainPage(TrackUser user)
        {
          
            this.user = user;
            Master = new NavigationMasterPage(user, this);
            Detail = new NavigationPage(new Maper.MapGroupsPage());

            
        }

        public void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var item = e.SelectedItem as MenuItem;
            if (item != null)
            {
                Detail = new NavigationPage((Page)Activator.CreateInstance(item.TargeType));
                IsPresented = false;
            }
        }
    }
}
