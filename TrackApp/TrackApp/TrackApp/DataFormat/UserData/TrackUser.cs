using System;
using System.Collections.Generic;
using System.Text;
using Amazon.DynamoDBv2.DataModel;
using TrackApp.ServerLayer;

namespace TrackApp.DataFormat.UserData
{
    [DynamoDBTable(ServerConsts.USERS_DB_NAME)]
    public class TrackUser 
    {
        [DynamoDBHashKey]
        public string Username { get; set; } // also User Id (every Username will be unique)
        [DynamoDBProperty]
        public string FirstName { get; set; }
        [DynamoDBProperty]
        public string LastName { get; set; }
        [DynamoDBProperty]
        public string Password { get; set; }
        [DynamoDBProperty]
        public string Phone { get; set; }
        [DynamoDBProperty]
        public string Email { get; set; }
        [DynamoDBProperty(Converter = typeof(LocationTypeConverter))]
        public Location Location { get; set; }


        public override string ToString()
        {
            return "Username: " + Username + " ****** Name: " + FirstName + " " + LastName + "\nLocation: " + Location;
        }

        public static string GetAccessType()
        {
            return DataConsts.USER_ACCESS_STRING;
        }
    }
}
