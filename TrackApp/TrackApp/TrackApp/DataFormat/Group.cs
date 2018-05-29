using Amazon.DynamoDBv2.DataModel;
using System;
using System.Collections.Generic;
using System.Text;
using TrackApp.ServerLayer;

namespace TrackApp.DataFormat
{
    [DynamoDBTable(ServerConsts.GROUPS_DB_NAME)]
    public class Group
    {
        public const string ACCEPTED_REQUEST_STATE = "a";
        public const string DENIED_REQUEST_STATE = "d";

        [DynamoDBHashKey]
        public string Name { get; set; }

        [DynamoDBProperty("Admins")]
        public List<string> Admins { get; set; } // username

        [DynamoDBProperty("Drivers")]
        public List<string> Drivers { get; set; } //username#numberOfRoutesInGroup

        [DynamoDBProperty("Notifications")]
        public List<string> Notifications { get; set; } //username#add/remove#index

        public override String ToString()
        {
            return "Name: "+ Name + "\nAdmins: " + Admins + "\nDrivers: " + Drivers + "\nNotifications: " + Notifications;
        }
    }
}
