using Amazon.DynamoDBv2.DataModel;
using System;
using TrackApp.ClientLayer;
using TrackApp.ServerLayer;

namespace TrackApp.DataFormat
{
    [DynamoDBTable(ServerConsts.POSITION_DB_NAME)]
    public class PositionDB
    {
        [DynamoDBHashKey]
        public string Username { get; set; }
        [DynamoDBRangeKey]
        public DateTime DateTime { get; set; }
        [DynamoDBProperty]
        public double Latitude { get; set; }
        [DynamoDBProperty]
        public double Longitude { get; set; }
        [DynamoDBVersion]
        public int? VersionNumber { get; set; } //TODO delete this , redundant, after clearing database
    }
}
