using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TrackApp.DataFormat;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2;
using Amazon.Runtime;

namespace TrackApp.ServerLayer.Query
{
    public class QueryRoute
    {

        public static async Task<IEnumerable<RouteInfo>> QueryRouteInfo(string username, string groupname, int maxCount)
        {
            try
            {
                var routeId = groupname + ServerConsts.CONCAT_SPECIAL_CHARACTER + username;
                var context = AwsUtils.GetContext();
                return await context.QueryAsync<RouteInfo>(
                    routeId,
                    QueryOperator.LessThanOrEqual,
                    new List<object> { maxCount }).GetRemainingAsync();
            }
            catch (AmazonDynamoDBException e)
            {
                Console.WriteLine("AmazonDynamoDBException CAUGHT: " + e.Message);
                throw new AmazonServiceException("AmazonDynamoDBException CAUGHT: " + e.Message);
            }
            catch (AmazonServiceException e)
            {
                Console.WriteLine("AmazonServiceException CAUGHT: " + e.Message);
                throw new AmazonServiceException("AmazonServiceException CAUGHT: " + e.Message);

            }
            catch (Exception e)
            {
                Console.WriteLine("Exception CAUGHT: " + e.Message);
                throw new Exception("Exception CAUGHT: " + e.Message);

            }
        }

        public static async Task<IEnumerable<Route>> QueryRoutes(string username, string groupname, string count, int addressesRange)
        {
            return await QueryRoutes(groupname + ServerConsts.CONCAT_SPECIAL_CHARACTER +
                                    username + ServerConsts.CONCAT_SPECIAL_CHARACTER +
                                    count, addressesRange);
        }

        public static async Task<IEnumerable<Route>> QueryRoutes(string routeId, int addressRange)
        {
            try
            {
                var context = AwsUtils.GetContext();
                return await context.QueryAsync<Route>(
                   routeId,
                   QueryOperator.LessThanOrEqual,
                   new List<object> { addressRange }).GetRemainingAsync();
            }
            catch (AmazonDynamoDBException e)
            {
                Console.WriteLine("AmazonDynamoDBException CAUGHT: " + e.Message);
                throw new AmazonServiceException("AmazonDynamoDBException CAUGHT: " + e.Message);
            }
            catch (AmazonServiceException e)
            {
                Console.WriteLine("AmazonServiceException CAUGHT: " + e.Message);
                throw new AmazonServiceException("AmazonServiceException CAUGHT: " + e.Message);

            }
            catch (Exception e)
            {
                Console.WriteLine("Exception CAUGHT: " + e.Message);
                throw new Exception("Exception CAUGHT: " + e.Message);

            }
        }
    }
}
