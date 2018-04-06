using System;
using System.Collections.Generic;
using System.Text;

namespace TrackerApp.DataFormat
{
    public class User
    {
        public string IdUser { get; set; }
        public string Type { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public Location Location { get; set; }

        public static string GetAccessType()
        {
            return DataConstants.USER_ACCESS_STRING;
        }
    }
}
