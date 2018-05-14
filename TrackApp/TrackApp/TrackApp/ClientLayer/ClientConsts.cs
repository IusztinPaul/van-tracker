using System;
using System.Collections.Generic;
using System.Text;

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
        public const string ADMINISTRATOR_SIGNAL = "a";
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

        // login and sign up constants 
        public const int PASSWORD_LENGTH_MINIMUM = 6;
        public static readonly string PASSWORD_LENGTH_STRING = "Parola de minim " + PASSWORD_LENGTH_MINIMUM + " caractere";
        public static readonly string PASSWORD_REPEAT = "Repeta parola";
        public const string ACCOUNT_CREATED_MESSAGE = "Contul a fost creat!";
        public const string INTERNET_EXCEPTION_MESSAGE = "Nu este internet sau exista o problema!";
        public const string DYNAMODB_EXCEPTION_MESSAGE1 = "Probleme cu internet-ul!";
        public const string DYNAMODB_EXCEPTION_MESSAGE2 = "Probleme cu server-ul!";

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
        public const string MAP_PAGE_TITLE = "Grupuri";
        public const string REQUEST_PAGE_TITLE = "Cereri";
        public const string TOOL_BAR_ADD_GROUP = "Creaza grup";

        //group tabbed page
        public const string MEMBERS_PAGE_TITLE = "Membrii";
        public const string NOTIFICATIONS_PAGE_TITLE = "Istoric";
        public const string START_PROCESS_SIGNAL = "Asteptati sa se incarce datele!";
        public const string GROUP_TITLE = "Grup";

    }
}
