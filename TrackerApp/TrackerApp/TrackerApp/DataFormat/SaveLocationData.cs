using System;
using System.Collections.Generic;
using System.Text;

namespace TrackerApp.DataFormat
{
    public class SaveLocationData : ISaveData
    {
        private string idUser;
        private float longitude;
        private float latitude;
        private TrackCalendar date;

        public SaveLocationData(string idUser, float longitude, float latitude, int year, int month, int day, int hour, int minute, int second, string tzone)
        {
            this.idUser = idUser;
            this.longitude = longitude;
            this.latitude = latitude;
            this.date = new TrackCalendar()
            {
                Year = year,
                Month = month,
                Day = day,
                Hour = hour,
                Minute = minute,
                Second = second,
                Tzone = tzone
            };
        }

        public SaveLocationData(string idUser, float longitude, float latitude, TrackCalendar date)
        {
            this.idUser = idUser;
            this.longitude = longitude;
            this.latitude = latitude;
            this.date = date;
        }

        public override void SaveData()
        {
            throw new NotImplementedException();
        }
    }
}
