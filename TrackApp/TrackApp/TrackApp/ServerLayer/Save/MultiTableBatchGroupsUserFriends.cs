using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Runtime;
using Amazon.SecurityToken;
using TrackApp.DataFormat;
using TrackApp.DataFormat.UserData;

namespace TrackApp.ServerLayer.Save
{
    public class MultiTableBatchGroupsUserFriends : ISaveData
    {
        public Group Group { get; set; }
        public UserFriends UserFriends { get; set; }
       
        public async override Task SaveData()
        {
            if (Group != null && UserFriends != null) {
                try
                {
                    var context = AwsUtils.GetContextSkipVersionCheck();

                    var groupBatch = context.CreateBatchWrite<Group>();
                    var userFriendsBatch = context.CreateBatchWrite<UserFriends>();

                    groupBatch.AddPutItem(Group);
                    userFriendsBatch.AddPutItem(UserFriends);

                    var superBatch = new MultiTableBatchWrite(groupBatch, userFriendsBatch);

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
                throw new Exception("Group or UserFriends is null");

            }
        }
    }
}
