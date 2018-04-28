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

        public async Task<T> LoadData<T>(string username) //used to get values from the following tables: USERS_DB_NAME, USERS_FRIENDS_DB_NAME
        // with the types TrackUser and UserFriends from the DataFormat.UserData types
        {
            try
            {
                var context = AwsUtils.GetContext();
                var returnedValue = await context.LoadAsync<T>(username, new DynamoDBContextConfig
                {
                    ConsistentRead = true
                });
                return returnedValue;
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

        public static async Task<List<TrackUser>> ScanAllTrackUsers()
        {
            
            DynamoDBContext context = AwsUtils.GetContext();

            var search = context.FromScanAsync<TrackUser>(new Amazon.DynamoDBv2.DocumentModel.ScanOperationConfig()
            {
                ConsistentRead = true
            });

             
             return await search.GetRemainingAsync();
        }
    }
}
