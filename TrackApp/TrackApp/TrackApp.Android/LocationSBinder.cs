using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace TrackApp.Droid
{
    public class LocationSBinder : Binder
    {
        public LocationService Service { get; private set; }

        public LocationSBinder(LocationService service)
        {
            this.Service = service;
        }
    }
}