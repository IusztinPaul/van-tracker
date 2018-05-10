using Amazon.DynamoDBv2.DataModel;
using System;
using System.Collections.Generic;
using System.IO;
using TrackApp.ClientLayer;
using TrackApp.ServerLayer;
using Xamarin.Forms;

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
        public string Icon { get; set; } //member with which the Icon it is stored in the DB serialized as a string

        [DynamoDBIgnore]
        public ImageSource IconSource //member which it is called from the code
        {
            get
            {
                if(!String.IsNullOrEmpty(Icon)) //convert Icon to ImageSource
                {
                    byte[] Base64Stream = Convert.FromBase64String(Icon);
                    return ImageSource.FromStream(() => new MemoryStream(Base64Stream));

                } else //get default embedded data
                {
                    return ImageSource.FromFile(ClientConsts.USER_PLACEHOLDER);
                }
            }
        }

        [DynamoDBProperty(Converter = typeof(LocationTypeConverter))]
        public Location Location { get; set; }

        public TrackUser() { }

        public TrackUser(TrackUser trackUser)
        {
            Username = trackUser.Username;
            FirstName = trackUser.FirstName;
            LastName = trackUser.LastName;
            Password = trackUser.Password;
            Phone = trackUser.Phone;
            Email = trackUser.Email;
            Icon = trackUser.Icon;
            Location = trackUser.Location;
        }

        public override string ToString()
        {
            return "Username: " + Username + " ****** Name: " + FirstName + " " + LastName + "\nLocation: " + Location;
        }


        public override bool Equals(object obj)
        {
            var user = obj as TrackUser;
            return user != null &&
                   Username == user.Username;
        }

        public override int GetHashCode()
        {
            return -182246463 + EqualityComparer<string>.Default.GetHashCode(Username);
        }
    }
}
