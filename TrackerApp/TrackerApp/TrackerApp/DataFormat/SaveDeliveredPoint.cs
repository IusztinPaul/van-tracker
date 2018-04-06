using System;
using System.Collections.Generic;
using System.Text;

namespace TrackerApp.DataFormat
{
    public class SaveDeliveredPoint : ISaveData
    {
        private string _idAddress; //idAdress for RouteData
        private bool _isDelivered;
        private TrackCalendar _date;

        public SaveDeliveredPoint(string idAddress, bool isDelivered, TrackCalendar date)
        {
            this._idAddress = idAddress;
            this._isDelivered = isDelivered;
            this._date = date;
        }

        public override void SaveData()
        {
            throw new NotImplementedException();
        }
    }
}
