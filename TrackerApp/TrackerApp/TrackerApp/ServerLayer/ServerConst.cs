using System;
using System.Collections.Generic;
using System.Text;

namespace TrackerApp.ServerLayer
{
    static class ServerConst
    {
        public const string endPointString = "https://trackerapp.documents.azure.com:443/";
        public static readonly Uri endPointUri = new Uri(endPointString);
        public const string primaryKey = "vLJSWdeixErIZiN9tQOhy1DM5ixyIyS9UOXzF5rZhCZfWQ4fBfDpC7OkCw1T23WINKoBWBXN7q0P98UoNHAF8g==";
    }
}
