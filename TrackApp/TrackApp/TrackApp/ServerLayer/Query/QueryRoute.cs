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
            return await QueryRouteInfoGeneral(username, groupname, maxCount, QueryOperator.LessThanOrEqual);
        }

        public static async Task<RouteInfo> QuerySingleRouteInfo(string username, string groupName, int count)
        {
            var list = await QueryRouteInfoGeneral(username, groupName, count, QueryOperator.Equal);

            if (list != null)
            {
                return new List<RouteInfo>(list)[0];
            }
            else
                return null;
        }

        private static async Task<IEnumerable<RouteInfo>> QueryRouteInfoGeneral(string username, string groupname, int maxCount, QueryOperator queryOperator)
        {
            try
            {
                var routeId = groupname + ServerConsts.CONCAT_SPECIAL_CHARACTER + username;
                var context = AwsUtils.GetContext();
                return await context.QueryAsync<RouteInfo>(
                    routeId,
                    queryOperator,
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

        public static async Task<List<RouteInfo>> QueryMultipleRoutesInfo(IList<RouteInfo> data)
        {
            try
            {
                var context = AwsUtils.GetContext();
                var routeInfoBatch = context.CreateBatchGet<RouteInfo>();

                if (data != null)
                    foreach (var routeinfo in data)
                    {
                        routeInfoBatch.AddKey(routeinfo.RouteId, routeinfo.Count);
                    }

                await routeInfoBatch.ExecuteAsync();
                return routeInfoBatch.Results;
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

        public static async Task<IEnumerable<Route>> QueryRoutes(string routeHashKey, int addressRange)
        {
            try
            {
                var context = AwsUtils.GetContext();
                return await context.QueryAsync<Route>(
                   routeHashKey,
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
