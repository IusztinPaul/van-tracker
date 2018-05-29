using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Runtime;
using TrackApp.ServerLayer;

namespace TrackApp.ServerLayer.Save
{
    public abstract class ISaveData
    {
        public abstract Task SaveData();
        public static async Task DeleteOnlyHashKeyData<T>(string id)
        {
            try
            {
                var context = AwsUtils.GetContext();
                await context.DeleteAsync<T>(id, new DynamoDBContextConfig
                {
                    ConsistentRead = true
                });
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
