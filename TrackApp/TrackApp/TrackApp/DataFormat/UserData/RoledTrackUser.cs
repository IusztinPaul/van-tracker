using System;
using System.Collections.Generic;
using System.Text;
using TrackApp.ClientLayer.Exceptions;

namespace TrackApp.DataFormat.UserData
{
    public class RoledTrackUser
    {
        public const string TYPE_ADMINISTRATOR = "administrator";
        public const string TYPE_DRIVER = "driver";

        public TrackUser TrackUser { get; set; }
        private string _role;
        public string Role
        {
            get => _role;
            set
            {
                if (value.Equals(TYPE_ADMINISTRATOR) || value.Equals(TYPE_DRIVER))
                    _role = value;
                else
                    throw new TypeException("Wrong role given to the user");
            }
        }
    }
}
