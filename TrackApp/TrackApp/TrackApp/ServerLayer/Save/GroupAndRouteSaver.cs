using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Runtime;
using TrackApp.DataFormat;

namespace TrackApp.ServerLayer.Save
{
    public class GroupAndRouteSaver : ISaveData
    {
        public DataFormat.Group Group { get; set; }
        public List<Route> Routes { get; set; }
        public RouteInfo RouteInfo { get; set; }

        public async override Task SaveData()
        {
            if (Routes != null && RouteInfo != null && Group != null)
            {
                try
                {
                    var context = AwsUtils.GetContext();

                    //make routes batch
                    var routesBatch = context.CreateBatchWrite<Route>();
                    routesBatch.AddPutItems(Routes);

                    //make route info batch
                    var routesInfoBatch = context.CreateBatchWrite<RouteInfo>();
                    routesInfoBatch.AddPutItem(RouteInfo);

                    //make Group batch
                    var groupBatch = context.CreateBatchWrite<Group>();
                    groupBatch.AddPutItem(Group);

                    //save with super batch
                    var superBatch = new MultiTableBatchWrite(routesBatch, routesInfoBatch, groupBatch);
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
                throw new Exception("Routes or RouteInfo or Group are null!");
            }
        }
    }
}
