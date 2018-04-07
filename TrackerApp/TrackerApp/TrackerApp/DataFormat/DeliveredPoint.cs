using System;
using System.Collections.Generic;
using System.Text;

namespace TrackerApp.DataFormat
{
    public class DeliveredPoint : JsonIdObject
    {
        public string IdAddress { get; set; } //idAdress for RouteData
        public bool IsDelivered { get; set; }
        public TrackCalendar Date { get; set; }

    }
}
