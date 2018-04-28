using System;
using System.Collections.Generic;
using System.Text;
using Amazon.DynamoDBv2.DataModel;
using TrackApp.ClientLayer;
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
        [DynamoDBIgnore]
        public string FullName {
            get => FirstName + " " + LastName; 
        }
        [DynamoDBProperty]
        public string Password { get; set; }
        [DynamoDBProperty]
        public string Phone { get; set; }
        [DynamoDBProperty]
        public string Email { get; set; }
        [DynamoDBProperty]
        public string Icon { get; set; } //member with which the Icon it is stored in the DB

        [DynamoDBIgnore]
        public string IconSource //member which it is called from the code
        {
            get
            {
                return Icon != null ? Icon : ClientConsts.USER_PLACEHOLDER;
            }
            set { Icon = value; }
        }

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

        public override bool Equals(object obj)
        {
            var user = obj as TrackUser;
            return user != null &&
                   Username == user.Username;
        }
    }
}
