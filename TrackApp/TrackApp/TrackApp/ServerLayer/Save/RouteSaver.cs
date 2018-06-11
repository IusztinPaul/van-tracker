using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Runtime;
using TrackApp.DataFormat;

namespace TrackApp.ServerLayer.Save
{
    public class RouteSaver : ISaveData // this class will save both the given RouteInfo and array of routes
    {
        public List<Route> Routes { get; set; }
        public RouteInfo RouteInfo { get; set; }

        public async override Task SaveData()
        {
            if(Routes != null && RouteInfo != null)
            {
                try
                {
                    var context = AwsUtils.GetContextSkipVersionCheck();

                //make routes batch
                var routesBatch = context.CreateBatchWrite<Route>();
                routesBatch.AddPutItems(Routes);

                //make route info batch
                var routesInfoBatch = context.CreateBatchWrite<RouteInfo>();
                routesInfoBatch.AddPutItem(RouteInfo);

                //save with super batch
                var superBatch = new MultiTableBatchWrite(routesBatch, routesInfoBatch);
                await superBatch.ExecuteAsync();
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

            } else
            {
                throw new Exception("Routes or RouteInfo are null!");
            }
        }

        public static async Task SaveSingleRoute(Route route)
        {
            if(route != null)
            {
                try { 
                var context = AwsUtils.GetContext();
                await context.SaveAsync<Route>(route);
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

        public static async Task DeleteSingleRoute(Route route)
        {
            if (route != null)
            {
                try
                {
                    var context = AwsUtils.GetContext();
                    await context.DeleteAsync<Route>(route);
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

        public async Task DeleteRoute()
        {

            if (Routes != null && RouteInfo != null)
            {
                try
                {
                    var context = AwsUtils.GetContext();

                    //make routes batch
                    var routesBatch = context.CreateBatchWrite<Route>();
                    routesBatch.AddDeleteItems(Routes);

                    //make route info batch
                    var routesInfoBatch = context.CreateBatchWrite<RouteInfo>();
                    routesInfoBatch.AddDeleteItem(RouteInfo);

                    //save with super batch
                    var superBatch = new MultiTableBatchWrite(routesBatch, routesInfoBatch);
                    await superBatch.ExecuteAsync();
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
            else
            {
                throw new Exception("Routes or RouteInfo are null!");
            }
        }
    }
}
