using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Runtime;
using TrackApp.DataFormat.UserData;


namespace TrackApp.ServerLayer.Query
{
    public class QueryUser
    {
        public TrackUser TrackUser { get; set; }

        public async Task<TrackUser> LoadData(string username)
        {
            try
            {
                var context = AwsUtils.GetContext();
                TrackUser = await context.LoadAsync<TrackUser>(username, new DynamoDBContextConfig
                {
                    ConsistentRead = true
                });
                return TrackUser;
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
