using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json;

namespace TrackerApp.ServerLayer
{
    class ServerS
    {
        private static ServerS instance;

        private DocumentClient dcServer;

        private ServerS()
        {
            try
            {
                dcServer = new DocumentClient(ServerConst.endPointUri, ServerConst.primaryKey);
            }
            catch (DocumentClientException de)
            {
                //TODO handle exception
                Exception baseException = de.GetBaseException();
                Console.WriteLine("{0} error occurred: {1}, Message: {2}", de.StatusCode, de.Message, baseException.Message);
            }
            catch (Exception e)
            {
                //TODO handle exception
                Exception baseException = e.GetBaseException();
                Console.WriteLine("Error: {0}, Message: {1}", e.Message, baseException.Message);
            }

        }

        public static ServerS GetInstance()
        {
            if(instance == null)
                instance = new ServerS();

            return instance;
        }

        public async Task<ResourceResponse<Database>> CreateDatabase(string name)
        {
            try
            {
                if (dcServer != null)
                    return await this.dcServer.CreateDatabaseIfNotExistsAsync(new Database {Id = name});
            }
            catch (ArgumentNullException e)
            {
                //TODO handle exception
                Exception baseException = e.GetBaseException();
                Console.WriteLine("{0}, Message: {1", e.Message, baseException.Message);
                return null;
            }
            catch (AggregateException e)
            {
                //TODO handle exception
                Exception baseException = e.GetBaseException();
                Console.WriteLine("{0}, Message: {1", e.Message, baseException.Message);
                return null;

            }
            catch (DocumentClientException e)
            {
                //TODO handle exception
                Exception baseException = e.GetBaseException();
                Console.WriteLine("{0}, Message: {1", e.Message, baseException.Message);
                return null;

            }

            return null;
        }

        public async Task<ResourceResponse<DocumentCollection>> CreateCollection(string databaseName, string collectionName)
        {
            if(dcServer != null)
                try
                {
                    return await dcServer.CreateDocumentCollectionIfNotExistsAsync(
                        UriFactory.CreateDatabaseUri(databaseName), new DocumentCollection {Id = collectionName});
                }
                catch (ArgumentNullException e)
                {
                    //TODO handle exception
                    Exception baseException = e.GetBaseException();
                    Console.WriteLine("{0}, Message: {1", e.Message, baseException.Message);
                    return null;
                }
                catch (AggregateException e)
                {
                    //TODO handle exception
                    Exception baseException = e.GetBaseException();
                    Console.WriteLine("{0}, Message: {1", e.Message, baseException.Message);
                    return null;

                }
                catch (DocumentClientException e)
                {
                    //TODO handle exception
                    Exception baseException = e.GetBaseException();
                    Console.WriteLine("{0}, Message: {1", e.Message, baseException.Message);
                    return null;

                }


            return null;
        }



    }
}
