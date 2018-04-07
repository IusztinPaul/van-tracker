using System;
using TrackerApp.DataFormat;

namespace TrackerApp.ServerLayer.ServerSaveData
{
    public class SaveDeliveryData : ISaveData
    {
        public DeliveryData DeliveryData { get; set; }

        public override void SaveData()
        {
            throw new NotImplementedException();
        }
    }
}
