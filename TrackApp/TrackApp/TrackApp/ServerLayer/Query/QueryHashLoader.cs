using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Runtime;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TrackApp.ServerLayer.Query
{
    public class QueryHashLoader
    {
        public static async Task<T> LoadData<T>(string hashKey) 
        {
            try
            {
                var context = AwsUtils.GetContext();
                var returnedValue = await context.LoadAsync<T>(hashKey, new DynamoDBContextConfig
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
    }
}
