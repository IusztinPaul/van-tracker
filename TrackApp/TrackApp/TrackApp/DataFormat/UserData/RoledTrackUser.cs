using System;
using System.Collections.Generic;
using System.Text;
using TrackApp.ClientLayer.Exceptions;

namespace TrackApp.DataFormat.UserData
{
    public class RoledTrackUser : TrackUser
    {
        public const string TYPE_ADMINISTRATOR = "Administrator";
        public const string TYPE_DRIVER = "Driver";
        public const string TYPE_NONE = "none"; // if this type exists it means there is a error in logic

        public RoledTrackUser(TrackUser trackUser) : base(trackUser) { }

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
