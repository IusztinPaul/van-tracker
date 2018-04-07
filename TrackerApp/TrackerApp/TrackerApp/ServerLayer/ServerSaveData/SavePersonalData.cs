using System;
using TrackerApp.DataFormat.UserData;

namespace TrackerApp.ServerLayer.ServerSaveData
{
    public class SavePersonalData : ISaveData
    {
        public User User { get; set; }

        public override void SaveData()
        {
            throw new NotImplementedException();
        }
    }
}
