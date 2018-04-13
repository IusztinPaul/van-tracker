using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.DataModel;
using TrackApp.ServerLayer;

namespace TrackApp.ServerLayer.Save
{
    public abstract class ISaveData
    {
        public abstract Task SaveData();
        public abstract Task DeleteData(String id);
    }
}
