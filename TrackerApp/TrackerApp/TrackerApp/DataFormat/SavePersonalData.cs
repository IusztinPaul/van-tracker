using System;
using System.Collections.Generic;
using System.Text;

namespace TrackerApp.DataFormat
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
