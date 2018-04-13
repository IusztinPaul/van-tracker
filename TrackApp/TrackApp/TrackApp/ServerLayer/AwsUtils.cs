using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using Amazon.CognitoIdentity;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using TrackApp.DataFormat;
using TrackApp.DataFormat.UserData;


namespace TrackApp.ServerLayer
{

    
    public class AwsUtils
    {
        //TODO add if no internet logic to all methods

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

        public static DynamoDBContext GetContext()
        {
            return AwsUtils.DDBContext;
        }
    }
}
