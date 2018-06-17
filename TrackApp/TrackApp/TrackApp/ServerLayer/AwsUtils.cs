using Amazon.CognitoIdentity;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;

namespace TrackApp.ServerLayer
{

    
    public static class AwsUtils
    {
        private static CognitoAWSCredentials _credentials;

        public static CognitoAWSCredentials Credentials
        {
            get
            {
                
                if (_credentials == null)
                    _credentials = new CognitoAWSCredentials(ServerConsts.POOL_IDENTITY_ID, ServerConsts.COGNITO_REGION);
                return _credentials;
            }
        }

        private static AmazonDynamoDBClient _client;

        public static AmazonDynamoDBClient DynamoDBClient
        {
            get
            {
                if (_credentials == null)
                    _credentials = new CognitoAWSCredentials(ServerConsts.POOL_IDENTITY_ID, ServerConsts.COGNITO_REGION);

                if (_client == null)
                    _client = new AmazonDynamoDBClient(Credentials, ServerConsts.DYNAMODB_REGION);
                return _client;
            }
        }


        private static DynamoDBContext _context;

        public static DynamoDBContext DDBContext
        {
            get
            {
                if (_credentials == null)
                    _credentials = new CognitoAWSCredentials(ServerConsts.POOL_IDENTITY_ID, ServerConsts.COGNITO_REGION);

                if (_client == null)
                    _client = new AmazonDynamoDBClient(Credentials, ServerConsts.DYNAMODB_REGION);

                if (_context == null)
                    _context = new DynamoDBContext(DynamoDBClient);

                return _context;
            }
        }

        private static DynamoDBContext _contextSkipVersioning;
      
        public static DynamoDBContext DDBContextSkipVersioning 
        {
            get
            {
                if (_credentials == null)
                    _credentials = new CognitoAWSCredentials(ServerConsts.POOL_IDENTITY_ID, ServerConsts.COGNITO_REGION);

                if (_client == null)
                    _client = new AmazonDynamoDBClient(Credentials, ServerConsts.DYNAMODB_REGION);

                if (_contextSkipVersioning == null)
                    _contextSkipVersioning = new DynamoDBContext(DynamoDBClient, new DynamoDBContextConfig
                    {
                        ConsistentRead = true,
                        SkipVersionCheck = true //here is the setup
                    });

                return _contextSkipVersioning;
            }
        }

        public static DynamoDBContext GetContext()
        {
            return AwsUtils.DDBContext;
        }

        public static DynamoDBContext GetContextSkipVersionCheck()
        {
            return DDBContextSkipVersioning;
        }
    }
}
