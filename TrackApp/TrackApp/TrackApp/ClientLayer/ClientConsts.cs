using System;
using System.Collections.Generic;
using System.Text;

namespace TrackApp.ClientLayer
{
    public static class ClientConsts
    {

        //concat special caracter
        public const string CONCAT_SPECIAL_CHARACTER = "#";

        //special notification  signal
        public const string ADD_SIGNAL = "add";
        public const string REMOVE_SIGNAL = "remove";

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

        // login and sign up constants 
        public const int PASSWORD_LENGTH = 6;
        public static readonly string PASSWORD_LENGTH_STRING = "Parola de minim " + PASSWORD_LENGTH + " caractere";
        public const string ACCOUNT_CREATED_MESSAGE = "Contul a fost creat!";
        public const string INTERNET_EXCEPTION_MESSAGE = "Nu este internet!";
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

    }
}
