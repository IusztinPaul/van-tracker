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

        public async Task<TrackUser> LoadData(string Username)
        {
            try
            {
                var context = AwsUtils.GetContext();
                TrackUser = await context.LoadAsync<TrackUser>(Username, new DynamoDBContextConfig
                {
                    ConsistentRead = true
                });
                return TrackUser;
            }
            catch (AmazonDynamoDBException e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
            catch (AmazonServiceException e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }
    }
}
