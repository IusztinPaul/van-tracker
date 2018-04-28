using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Runtime;
using TrackApp.DataFormat.UserData;

namespace TrackApp.ServerLayer.Save
{
    public class SaveUserFriends : ISaveData
    {

        public UserFriends UserFriends { get; set; }

        public override async Task SaveData()
        {
            if (UserFriends.Username != null)
            {
                try
                {
                    var context = AwsUtils.GetContextV2();
                    await context.SaveAsync(UserFriends);
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
