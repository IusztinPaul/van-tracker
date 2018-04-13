
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;

namespace TrackApp.DataFormat
{
    public class LocationTypeConverter : IPropertyConverter
    {
        public DynamoDBEntry ToEntry(object value)
        {
            Location location = value as Location;

            if (location == null)
                return null;

            DynamoDBEntry entry = new Document(location.toDict());

            return entry;
        }

        public object FromEntry(DynamoDBEntry entry)
        {
            var doc = entry as Document;

            if (doc == null)
                return null;
        
            return Location.ToLocation(doc);
        }
    }
}
