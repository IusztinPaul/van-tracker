using System;

namespace TrackerApp.DataFormat.ServerSaveData
{
    public class SaveLocationData : ISaveData
    {
        public LocationData LocationData { get; set; }

        public override void SaveData()
        {
            throw new NotImplementedException();
        }
    }
}
