using System;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Runtime;
using TrackApp.DataFormat.UserData;

namespace TrackApp.ServerLayer.Save
{
    public class SaveUser : ISaveData
    {
        public TrackUser TrackUser { get; set; }
        public override async Task SaveData()
        {
            if (TrackUser.Username != null)
            {
                try
                {
                    var context = AwsUtils.GetContextSkipVersionCheck();
                    await context.SaveAsync(TrackUser);
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
                throw new Exception("TrackUser has no Id/Username!!!");
        }

        public async Task SaveUserAndUserFriends(UserFriends userFriends)
        {
            if (TrackUser.Username != null)
            {
                try
                {
                    var context = AwsUtils.GetContextSkipVersionCheck();

                    var userFriendsBatch = context.CreateBatchWrite<UserFriends>();
                    var userBatch = context.CreateBatchWrite<TrackUser>();

                    userBatch.AddPutItem(TrackUser);

                    if (userFriends == null)
                        userFriendsBatch.AddPutItem(new UserFriends { Username = TrackUser.Username });
                    else
                        userFriendsBatch.AddPutItem(userFriends);

                    MultiTableBatchWrite multiBatch = new MultiTableBatchWrite(userFriendsBatch, userBatch);
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
            else
                throw new Exception("TrackUser has no Id/Username!!!");
        }
    }
}
