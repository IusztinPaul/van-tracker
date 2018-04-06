
namespace TrackerApp.DataFormat
{
    public class RouteData
    {
        private string idAddress;
        private string idRoute;
        private string idUser;
        private Location location;

        public RouteData(string idAddress, string idRoute, string idUser, string country, string region, string city, string street, string nr, string bl, string ap, string sc)
        {
            this.idAddress = idAddress;
            this.idRoute = idRoute;
            this.idUser = idUser;
            this.location = new Location() { Country = country, Region = region, City = city, Street = street, Nr = nr, Bl = bl, Ap = ap, Sc = sc };
        }

        public RouteData(string idAddress, string idRoute, string idUser, Location location)
        {
            this.idAddress = idAddress;
            this.idRoute = idRoute;
            this.idUser = idUser;
            this.location = location;
        }

        
    }
}
