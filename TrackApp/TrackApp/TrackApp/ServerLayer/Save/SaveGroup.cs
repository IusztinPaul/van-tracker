using Amazon.DynamoDBv2;
using Amazon.Runtime;
using System;
using System.Threading.Tasks;
using TrackApp.DataFormat;

namespace TrackApp.ServerLayer.Save
{
    public class SaveGroup : ISaveData
    {
        public Group Group { get; set; }

        public override async Task SaveData()
        {
            if (Group.Name != null)
            {
                try
                {
                    var context = AwsUtils.GetContext();
                    await context.SaveAsync(Group);
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
                throw new Exception("Group has no Name!!!");
        }
    }
}
