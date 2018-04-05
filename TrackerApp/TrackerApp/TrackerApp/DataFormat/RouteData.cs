
namespace TrackerApp.DataFormat
{
    public class RouteData
    {
        private string idAddress;
        private string idRoute;
        private string idUser;
        private string country;
        private string region;
        private string city;
        private string street;
        private string nr;
        private string bl;
        private string ap;
        private string sc;

        public RouteData(string idAddress, string idRoute, string idUser, string country, string region, string city, string street, string nr, string bl, string ap, string sc)
        {
            this.idAddress = idAddress;
            this.idRoute = idRoute;
            this.idUser = idUser;
            this.country = country;
            this.region = region;
            this.city = city;
            this.street = street;
            this.nr = nr;
            this.bl = bl;
            this.ap = ap;
            this.sc = sc;
        }
    }
}
