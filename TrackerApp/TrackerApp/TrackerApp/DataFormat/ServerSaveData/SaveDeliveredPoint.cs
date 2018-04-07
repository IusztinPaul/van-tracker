using System;

namespace TrackerApp.DataFormat.ServerSaveData
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
