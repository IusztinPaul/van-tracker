using System;
using System.Collections.Generic;
using System.Text;
using Amazon.DynamoDBv2.DataModel;

namespace TrackApp.DataFormat
{
    [DynamoDBTable("TestTable")]
    public class DBIdObject
    {
        [DynamoDBHashKey]
        public string Id { get; set; }
      

        public override string ToString()
        {
            return Id;
        }
    }
}
