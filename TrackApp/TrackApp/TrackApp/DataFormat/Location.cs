using System;
using System.Collections.Generic;
using System.Text;
using Amazon.DynamoDBv2.DocumentModel;

namespace TrackApp.DataFormat
{
    public class Location
    {
        public string Country { get; set; } = "";
        public string Region { get; set; } = "";
        public string City { get; set; } = "";
        public string Street { get; set; } = "";
        public string Nr { get; set; } = "";
        public string Block { get; set; } = "";
       

        public Dictionary<string, DynamoDBEntry> toDict()
        {
            var dict = new Dictionary<string, DynamoDBEntry>();
            dict.Add("Country", new Primitive{ Value = Country });
            dict.Add("Region", new Primitive { Value = Region });
            dict.Add("City", new Primitive { Value = City });
            dict.Add("Street", new Primitive { Value = Street });
            dict.Add("Nr", new Primitive { Value = Nr });
            dict.Add("Block", new Primitive { Value = Block });
            return dict;
        }

        
        public static Location ToLocation(Document dict)
        {
            var loc = new Location();

            loc.Country = dict["Country"];
            loc.Region = dict["Region"];
            loc.City = dict["City"];
            loc.Street = dict["Street"];
            loc.Nr = dict["Nr"];

            if (dict.ContainsKey("Block"))
                loc.Block = dict["Block"];
            else
                loc.Block = null;

            return loc;
        }

        public override string ToString()
        {
            var s = Country + " " + Region + " " + City + ", " + Street + " " + Nr;

            if (Block != null && !Block.Trim().Equals("-"))
                s = s+ " " + Block;

            return s;
        }

        public override bool Equals(object obj)
        {
            var item = obj as Location;
            if (item != null)
            {
                if (item.Block == null)
                    item.Block = "";

                if (Block == null)
                    Block = "";

                return item.Country.Equals(Country) && item.Region.Equals(Region) && item.City.Equals(City) &&
                    item.Street.Equals(Street) && item.Nr.Equals(Nr) && item.Block.Equals(Block);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return Country.GetHashCode() + Region.GetHashCode() + City.GetHashCode();
        }
    }
}
