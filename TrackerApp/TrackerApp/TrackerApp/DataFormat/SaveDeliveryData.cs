using System;
using System.Collections.Generic;
using System.Text;

namespace TrackerApp.DataFormat
{
    public class SaveDeliveryData : ISaveData
    {
        public List<RouteData> dataToGenerate { get; set; }

        public override void SaveData()
        {
            throw new NotImplementedException();
        }
    }
}
