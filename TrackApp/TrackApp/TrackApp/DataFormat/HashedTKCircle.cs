using System;
using System.Collections.Generic;
using System.Text;
using TK.CustomMap.Overlays;

namespace TrackApp.DataFormat
{
    public class HashedTKCircle : TKCircle
    {
        public override bool Equals(object obj)
        {
            var item = obj as HashedTKCircle;
            if (item != null)
                return item.Center.Latitude == this.Center.Latitude && item.Center.Longitude == this.Center.Longitude;

            return false;
        }

        public override int GetHashCode()
        {
            return this.Center.Latitude.GetHashCode() + this.Center.Longitude.GetHashCode();
        }
    }
}
