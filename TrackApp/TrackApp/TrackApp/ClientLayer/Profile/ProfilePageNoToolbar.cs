using TrackApp.DataFormat.UserData;

namespace TrackApp.ClientLayer.Profile
{
    public class ProfilePageNoToolbar : ProfilePage
    {
        public ProfilePageNoToolbar(TrackUser trackUser) : base(trackUser) {
            ToolbarItems.Clear();
        }

    }
}
