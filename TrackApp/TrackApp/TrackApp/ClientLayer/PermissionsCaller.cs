using System.Threading.Tasks;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Xamarin.Forms;

namespace TrackApp.ClientLayer
{
    public static class PermissionsCaller
    {
        private static async Task<PermissionStatus> GenericPermissionCaller(Page hostPage, Permission permission, string title, string content, string buttonText)
        {
            var status = await CrossPermissions.Current.CheckPermissionStatusAsync(permission);
            if (status != PermissionStatus.Granted)
            {
                if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(permission))
                {
                    await hostPage.DisplayAlert(title, content, buttonText);
                }

                var results = await CrossPermissions.Current.RequestPermissionsAsync(permission);

                //Best practice to always check that the key exists
                if (results.ContainsKey(Permission.Location))
                    status = results[Permission.Location];
            }

            return status;
        }
        
        public static async Task<PermissionStatus> PermissionLocationCaller(Page hostPage)
        {
            return await GenericPermissionCaller(hostPage, Permission.Location, "Locatie", "Aplicatia aceasta are nevoie de locatia dumneavoastra ca sa functioneze corect", "Ok");
        }

        public static async Task<PermissionStatus> PermissionStorageCaller(Page hostPage)
        {
            return await GenericPermissionCaller(hostPage, Permission.Storage, "Spatiu de stocare", "Aplicatia aceasta are nevoie de spatiul de stocare al telefonului dumneavoastra ca sa functioneze corect", "Ok");
        }
    }
}
