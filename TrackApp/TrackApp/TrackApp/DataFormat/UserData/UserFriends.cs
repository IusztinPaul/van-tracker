using System;
using System.Collections.Generic;
using System.Text;
using Amazon.DynamoDBv2.DataModel;
using TrackApp.ServerLayer;

namespace TrackApp.DataFormat.UserData
{
    [DynamoDBTable(ServerConsts.USERS_FRIENDS_DB_NAME)]
    public class UserFriends
    {
        [DynamoDBHashKey]
        public string Username { get; set; }
        [DynamoDBProperty("Friends")]
        public List<string> Friends { get; set; } //actual friends (list will hold currentUser ids/usernames)
        [DynamoDBProperty("Notifications")]
        public List<string> Notifications { get; set; } // add or delete notifications (list item blueprint: username#add or username#delete)

    }
}
