using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace TrackerApp.DataFormat
{
    public class LocationData : JsonIdObject
    {
        
        public string IdUser { get; set; }
        public float Longitude { get; set; }
        public float Latitude { get; set; }
        public TrackCalendar Date { get; set; }

    }
}
