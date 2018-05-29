using Amazon.DynamoDBv2.DataModel;
using TrackApp.ServerLayer;

namespace TrackApp.DataFormat
{
    [DynamoDBTable(ServerConsts.ROUTES_INFO_DB_NAME)]
    public class RouteInfo
    {
        [DynamoDBHashKey]
        public string RouteId { get; set; } // groupname#username
        [DynamoDBRangeKey]
        public int Count { get; set; } // index (index = count of routes in group)
        [DynamoDBProperty]
        public int CountRouteAddresses { get; set; } // max number of addressess in a route (will help use in querying the right range key in the Routes table)
        [DynamoDBProperty]
        public string RouteName { get; set; }
    }
}
