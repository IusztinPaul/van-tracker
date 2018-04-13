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
                    var context = AwsUtils.GetContext();
                    await context.SaveAsync(TrackUser);
                }
                catch (AmazonDynamoDBException e) { Console.WriteLine(e.Message); }
                catch (AmazonServiceException e) { Console.WriteLine(e.Message); }
                catch (Exception e) { Console.WriteLine(e.Message); }
            }
            else
                throw new Exception("TrackUser has no Id/Username!!!");
        }

        public override async Task DeleteData(String id)
        {
            try
            {
                var context = AwsUtils.GetContext();
                await context.DeleteAsync<TrackUser>(id, new DynamoDBContextConfig
                {
                    ConsistentRead = true
                });
            }
            catch (AmazonDynamoDBException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (AmazonServiceException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
