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

namespace TrackApp
{



    public partial class MainPage : ContentPage
	{
        public MainPage()
        {
            InitializeComponent();

        }
    }
}
