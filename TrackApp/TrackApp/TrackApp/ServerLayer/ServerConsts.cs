using Amazon;




namespace TrackApp.ServerLayer
{
    public static class ServerConsts
    {
        //aws connection constants
        public const string POOL_IDENTITY_ID = "eu-central-1:e4d9ae6e-2826-4137-8c7e-137d4c920d55";
        public static RegionEndpoint COGNITO_REGION = RegionEndpoint.EUCentral1;
        public static RegionEndpoint DYNAMODB_REGION = RegionEndpoint.EUCentral1;

        //aws database names
        public const string USERS_DB_NAME = "UserData";
        public const string USERS_FRIENDS_DB_NAME = "UserFriends";

        //aws databases hashkeys
        public const string USER_DATA_HASHKEY = "Username";

    }
}
