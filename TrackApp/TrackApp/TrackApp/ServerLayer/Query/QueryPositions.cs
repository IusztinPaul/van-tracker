using Amazon.DynamoDBv2.DocumentModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TrackApp.DataFormat;

namespace TrackApp.ServerLayer.Query
{
    public class QueryPositions
    {
        public async static Task<IEnumerable<PositionDB>> QueryPositionsInLastMiliseconds(string username, long miliseconds)
        {
            DateTime milisecondsAgo = DateTime.Now - TimeSpan.FromMilliseconds(miliseconds);
            var context = AwsUtils.GetContext();
            return await context.QueryAsync<PositionDB>(username, 
                QueryOperator.GreaterThanOrEqual, 
                new List<object> { milisecondsAgo }).GetRemainingAsync();
        }

        public async static Task<IEnumerable<PositionDB>> QueryPositionsInLastHours(string username, long hours)
        {
            DateTime hoursAgo = DateTime.Now - TimeSpan.FromHours(hours);
            var context = AwsUtils.GetContext();
            return await context.QueryAsync<PositionDB>(username,
                QueryOperator.GreaterThanOrEqual,
                new List<object> { hoursAgo }).GetRemainingAsync();
        }

        public async static Task<IEnumerable<PositionDB>> QueryPositionsInLastDays(string username, long days)
        {
            DateTime daysAgo = DateTime.Now - TimeSpan.FromDays(days);
            var context = AwsUtils.GetContext();
            return await context.QueryAsync<PositionDB>(username,
                QueryOperator.GreaterThanOrEqual,
                new List<object> { daysAgo }).GetRemainingAsync();
        }
    }
}
