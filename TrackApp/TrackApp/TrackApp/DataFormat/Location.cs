using System;
using System.Collections.Generic;
using System.Text;
using Amazon.DynamoDBv2.DocumentModel;

namespace TrackApp.DataFormat
{
    public class Location
    {
        public string Country { get ; set; }
        public string Region { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string Nr { get; set; }
        public string Block { get; set; }
       

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
            loc.Block = dict["Block"];

            return loc;
        }

        public override string ToString()
        {
            return Country + " " + Region + " " + City + " " + Street + " " + Block;
        }
    }
}
