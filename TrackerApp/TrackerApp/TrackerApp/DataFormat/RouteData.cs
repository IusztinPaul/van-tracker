
namespace TrackerApp.DataFormat
{
    public class RouteData : JsonIdObject
    {
        public string IdRoute { get; set; }
        public string IdUser { get; set; }
        public Location Location { get; set; }

    }
}
