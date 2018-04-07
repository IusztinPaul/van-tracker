using System;
using TrackerApp.DataFormat;

namespace TrackerApp.ServerLayer.ServerSaveData
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
