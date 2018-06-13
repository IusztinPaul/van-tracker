using System;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Runtime;
using System.Threading.Tasks;
using TrackApp.DataFormat.UserData;

namespace TrackApp.ServerLayer.Save
{
    public class RequestsSavercs
    {
        public static async Task SaveGroupAndUserFriends(DataFormat.Group group, UserFriends userFriends)
        {
            try
            {
                var context = AwsUtils.GetContextSkipVersionCheck();

                var userFriendsBatch = context.CreateBatchWrite<UserFriends>();
                var groupBatch = context.CreateBatchWrite<DataFormat.Group>();

                groupBatch.AddPutItem(group);
                userFriendsBatch.AddPutItem(userFriends);

                MultiTableBatchWrite multiBatch = new MultiTableBatchWrite(userFriendsBatch, groupBatch);
                await multiBatch.ExecuteAsync();
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
