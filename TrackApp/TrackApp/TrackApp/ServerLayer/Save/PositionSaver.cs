using Amazon.DynamoDBv2;
using Amazon.Runtime;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TrackApp.DataFormat;

namespace TrackApp.ServerLayer.Save
{
    public class PositionSaver : ISaveData
    {
        public PositionDB position { get; private set; }
        public PositionSaver(PositionDB position)
        {
            this.position = position;
        }

        public async override Task SaveData()
        {
            try
            {
                var context = AwsUtils.GetContext();
                await context.SaveAsync(position);
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
