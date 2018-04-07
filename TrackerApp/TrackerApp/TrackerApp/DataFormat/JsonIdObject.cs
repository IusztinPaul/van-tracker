using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace TrackerApp.DataFormat
{
    public class JsonIdObject
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
    }
}
