using System;
using System.Collections.Generic;

namespace TrackerApp.DataFormat.ServerSaveData
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
