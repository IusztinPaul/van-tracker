using Amazon.DynamoDBv2.DataModel;
using System;
using TrackApp.ClientLayer;
using TrackApp.ServerLayer;

namespace TrackApp.DataFormat
{

    [DynamoDBTable(ServerConsts.ROUTES_DB_NAME)]
    public class Route
    {
        [DynamoDBHashKey]
        public string RouteId { get; set; } // groupname#username#count --> count = the number of route in a specific group (same as Count property from RouteInfo)
        [DynamoDBRangeKey]
        public int AddressId { get; set; }
        [DynamoDBProperty(Converter = typeof(LocationTypeConverter))]
        public Location Location { get; set; }
        [DynamoDBProperty]
        public bool Delivered { get; set; } = false;
        [DynamoDBProperty]
        public DateTime DateTime { get; set; } //datetime for delivery

        [DynamoDBIgnore]
        public string BottomRouteText { get
            {
                return " nr. " + Location.Nr + " " + Location.Block;
            }
        }
        [DynamoDBIgnore]
        public string UpperRouteText
        {
            get
            {
                return Location.Street;
            }
        }

        [DynamoDBIgnore]
        public string Icon
        { get
            {
                return Delivered == true ? ClientConsts.DELIVERED_ICON : ClientConsts.NOT_DELIVERED_ICON;
            }
        }

        public override bool Equals(object obj)
        {
            var item = obj as Route;
            if (item != null)
                return RouteId.Equals(item.RouteId) && Location.Equals(item.Location);

            return false;
        }

        public override int GetHashCode()
        {
            return RouteId.GetHashCode();
        }

        public override string ToString()
        {
            return RouteId + " " + AddressId + " " + Location;
        }
    }
}
