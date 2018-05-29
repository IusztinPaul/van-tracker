using Amazon;




namespace TrackApp.ServerLayer
{
    public static class ServerConsts
    {
        //concat special caracter
        public const string CONCAT_SPECIAL_CHARACTER = ClientLayer.ClientConsts.CONCAT_SPECIAL_CHARACTER;

        //aws connection constants
        public const string POOL_IDENTITY_ID = "eu-central-1:e4d9ae6e-2826-4137-8c7e-137d4c920d55";
        public static RegionEndpoint COGNITO_REGION = RegionEndpoint.EUCentral1;
        public static RegionEndpoint DYNAMODB_REGION = RegionEndpoint.EUCentral1;

        //aws database names
        public const string USERS_DB_NAME = "UserData";
        public const string USERS_FRIENDS_DB_NAME = "UserFriends";
        public const string GROUPS_DB_NAME = "Groups";
        public const string ROUTES_DB_NAME = "RoutesDb";
        public const string ROUTES_INFO_DB_NAME = "RoutesInfo";

        //aws databases hashkeys
        public const string USER_DATA_HASHKEY = "Username";

    }
}
