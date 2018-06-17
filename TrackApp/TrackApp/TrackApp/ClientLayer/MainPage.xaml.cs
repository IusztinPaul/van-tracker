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
        private readonly TrackUser currentUser;
        public MainPage(TrackUser currentUser)
        {
            Title = ClientConsts.MAIN_PAGE_TITLE;
          
            this.currentUser = currentUser;
            Master = new NavigationMasterPage(currentUser, this);
            Detail = new NavigationPage(new Maper.MapTabbedPage(currentUser));
        }

        public void OnItemSelected(object sender, SelectedItemChangedEventArgs e) 
            // function that is called within the NavigationMasterPage
        {
            var item = e.SelectedItem as MenuItem;
            if (item != null)
            {
                Detail = new NavigationPage((Page)Activator.CreateInstance(item.TargeType, currentUser));
                IsPresented = false;
            }
        }

        protected override bool OnBackButtonPressed()
        {
            bool returnedItem = true;
            Device.BeginInvokeOnMainThread( async () => returnedItem = await DisplayAlert("Iesit", "Vreti sa iesiti din aplicatie?", "Da", "Nu"));
            return returnedItem;
        }
    }
}
