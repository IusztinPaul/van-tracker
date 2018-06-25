using Xamarin.Forms;

namespace TrackApp.ClientLayer
{
    public static class ClientConsts
    {
        //concat special caracter
        public const string CONCAT_SPECIAL_CHARACTER = "#";

        //special notification signals
        public const string ADD_SIGNAL = "add";
        public const string REMOVE_SIGNAL = "remove";

        //special type signals 
        public const string ADMINISTRATOR_SIGNAL = "a"; // used as group and userfriends signals
        public const string DRIVER_SIGNAL = "d";

        //main page consts
        public const string MAIN_PAGE_TITLE = "TrackApp";

        //image names
        public const string USER_PLACEHOLDER = "avatar_placeholder.png";
        public const string FRIENDS_ICON = "friends_icon.jpg";
        public const string GROUP_ICON = "group_icon.png";
        public const string LOGOUT_ICON = "logout_icon.png";
        public const string USER_ICON = "user_icon.png";
        public const string APP_LOGO = "vantrackerlogo.png";
        public const string ADD_ITEM_ICON = "add_item_icon.jpg";
        public const string FOLLOWED_ICON = "follow_icon.png";
        public const string UNFOLLWED_ICON = "unfollow_icon.png";
        public const string USER_BLACK_ICON = "profile_icon_black.png";
        public const string CONTACT_BLACK_ICON = "contact_icon_black.png";
        public const string LOCATION_BLACK_ICON = "location_icon_black.png";
        public const string ADMINISTRATOR_ICON = "administrator_icon.png";
        public const string DRIVER_ICON = "driver_icon.jpg";
        public const string THICK_ICON = "thick_icon.png";
        public const string X_ICON = "x_icon.png";
        public const string DELIVERED_ICON = "thick_icon_black.png";
        public const string NOT_DELIVERED_ICON = "red_cross_icon.png";

        // login and sign up constants 
        public const string LOGIN_KEY_FLAG = "login"; // key for App.Current.Properties for login ( value = username/LOGIN_NO_USER_FLAG)
        public const string LOGIN_NO_USER_FLAG = "$$$";
        public const int PASSWORD_LENGTH_MINIMUM = 6;
        public static readonly string PASSWORD_LENGTH_STRING = "Parola de minim " + PASSWORD_LENGTH_MINIMUM + " caractere";
        public static readonly string PASSWORD_REPEAT = "Repeta parola";
        public const string ACCOUNT_CREATED_MESSAGE = "Contul a fost creat!";
        public const string INTERNET_EXCEPTION_MESSAGE = "Exista o problema! Reincercati mai tarziu!";
        public const string DYNAMODB_EXCEPTION_MESSAGE1 = "Probleme cu internet-ul! Reincercati mai tarziu!";
        public const string DYNAMODB_EXCEPTION_MESSAGE2 = "Probleme cu server-ul! Reincercati mai tarziu!";

        // friends constants
        public const string FRIENDS_PAGE_TITLE = "Relatii";
        public const string TAB_FRIENDS_PAGE = "Prieteni";
        public const string TAB_NOTIFICATION_PAGE = "Notificari";
        public const string TOOL_BAR_ADD_FRIEND = "Adauga prieten";

        // to user messages 
        public const string REQUEST_MESSAGE = "Cererea s-a trimis";
        public const string ACHIEVED_MESSAGE = "Cererea s-a inregistrat";
        public const string FAILURE_MESSAGE = "Cererea a esuat";

        //user page
        public const string NR = "nr. ";
        public const string EDIT_FINALIZE_MESSAGE = "Datele au fost salvate!";

        // map tabbed page
        public const string MAPER_PAGE_TITLE = "Grupuri";
        public const string REQUEST_PAGE_TITLE = "Cereri";
        public const string TOOL_BAR_ADD_GROUP = "Creaza grup";

        //group tabbed page
        public const string MEMBERS_PAGE_TITLE = "Membri";
        public const string NOTIFICATIONS_PAGE_TITLE = "Istoric";
        public const string GENERAL_GROUP_PAGE_TITLE = "Harta";
        public const string START_PROCESS_SIGNAL = "Asteptati sa se incarce datele!";
        public const string GROUP_TITLE = "Grup";

        //group members and routes pages
        public const string ADD_ROUTE_TOOL_BAR_TITLE = "Creaza ruta";
        public const string REMOVE_ROUTE_TOOL_BAR_TITLE = "Sterge ruta";
        public const string SEE_USER_POSITON_TOOL_BAR_TITLE = "Vezi pozitie";
        public const string MAKE_ROUTE_ACTIVE_TOOL_BAR_TITLE = "Ruta activa";
        public const string SEE_PROFILE_ACTIVE_TOOL_BAR_TITLE = "Vezi profil";
        public const string DELETE_DRIVER_FROM_GROUP_TOOL_BAR_TITLE = "Sterge";

        //map namespace
        public const string MAP_TABBED_PAGE_TITLE = "Harta";
        public const string MAP_PAGE_TITLE = "Harta";
        public const string MAP_ACTIVE_ROUTE_TITLE = "Ruta";
        public const string MAP_CALIBRATE_TOOL_BAR_ITEM_TITLE = "Centreaza";
        public const string SEARCH_BAR_TOOL_BAR_ITEM_TITLE = "Cauta locatie";

        public const double CIRCLE_RADIUS = 8;
        public const float CIRCLE_STROKE_WIDTH = 4f;
        public const float LINE_WIDTH = 35f;
        public const double FROM_KM_MAP_DISTANCE = 0.15f;

        public static readonly Color[] Colours = new Color[] { Color.DarkRed, Color.DarkBlue, Color.DarkGreen, Color.DarkOrange, Color.DarkViolet, Color.Pink, Color.Silver, Color.Turquoise };
        public static readonly int MAX_DRIVERS_IN_GROUP = Colours.Length;

        //service 
        public const int LOCATION_LOOP_WAIT_TIME_MILISECONDS = 100;
        public const int UPDATE_CIRCLE_LOCATION_LOOP_TIME_MILISECONDS = 100;
    }
}
