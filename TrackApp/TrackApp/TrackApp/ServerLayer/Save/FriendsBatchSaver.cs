using Amazon.DynamoDBv2;
using Amazon.Runtime;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TrackApp.DataFormat.UserData;

namespace TrackApp.ServerLayer.Save
{
    public class FriendsBatchSaver 
    {
        public static async Task SaveUserFriends(UserFriends currentUserFriends, UserFriends friendUserFriends)
        {
            try
            {
                if (currentUserFriends != null && friendUserFriends != null)
                {
                    var context = AwsUtils.GetContext();
                    var batch = context.CreateBatchWrite<UserFriends>();

                    batch.AddPutItem(currentUserFriends);
                    batch.AddPutItem(friendUserFriends);

                    await batch.ExecuteAsync();
                }
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
