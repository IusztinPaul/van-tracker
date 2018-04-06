using System;
using System.Collections.Generic;
using System.Text;

namespace TrackerApp.DataFormat
{
    public class Admin : User
    {
        public new static string GetAccessType()
        {
            return DataConstants.ADMIN_ACCESS_STRING;
        }
    }
}
