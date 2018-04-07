using System;
using TrackerApp.DataFormat;

namespace TrackerApp.ServerLayer.ServerSaveData
{
    public class SaveDeliveredPoint : ISaveData
    {
        public DeliveredPoint DeliveredPoint { get; set; }

        public override void SaveData()
        {
            throw new NotImplementedException();
        }
    }
}
