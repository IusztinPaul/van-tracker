namespace TrackerApp.DataFormat.UserData
{
    public class Admin : User
    {
        public new static string GetAccessType()
        {
            return DataConstants.ADMIN_ACCESS_STRING;
        }
    }
}
