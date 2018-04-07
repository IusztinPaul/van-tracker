using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json;
using TrackerApp.DataFormat;

namespace TrackerApp.ServerLayer
{
    class ServerS
    {
        //TODO add if no internet logic to all methods

        private static ServerS instance;

        private readonly DocumentClient client;

        private ServerS()
        {
            try
            {
                client = new DocumentClient(ServerConst.endPointUri, ServerConst.primaryKey);
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
            if (instance == null)
                instance = new ServerS();

            return instance;
        }

        public async Task<ResourceResponse<Database>> CreateDatabaseIfNotExist(string name)
        {
            try
            {
                if (client != null)
                    return await client.CreateDatabaseIfNotExistsAsync(new Database { Id = name });
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

        public async Task<ResourceResponse<DocumentCollection>> CreateCollectionIfNotExist(string databaseName, string collectionName)
        {
            if (client != null)
                try
                {
                    return await client.CreateDocumentCollectionIfNotExistsAsync(
                        UriFactory.CreateDatabaseUri(databaseName), new DocumentCollection { Id = collectionName });
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

        public async Task CreateDocumentIfNotExists(string databaseName, string collectionName, JsonIdObject obj)
        {
            
            try
            {
                await this.client.ReadDocumentAsync(UriFactory.CreateDocumentUri(databaseName, collectionName, obj.Id));
            }
            catch (DocumentClientException de)
            {
                if (de.StatusCode == HttpStatusCode.NotFound)
                {
                    await this.client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(databaseName, collectionName), obj);
                }
                else
                {
                    throw;
                }
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine("NO DOCUMENT LINK");
                //TODO GUI output that the document does not exist
            }

        }

        public async Task ReplaceDocument(string databaseName, string collectionName, string id, JsonIdObject updatedObject)
        {
            try
            {
                await this.client.ReplaceDocumentAsync(UriFactory.CreateDocumentUri(databaseName, collectionName, id), updatedObject);
            }
            catch (DocumentClientException de)
            {
                if (de.StatusCode == HttpStatusCode.NotFound)
                {
                    Console.WriteLine("DOCUMENT DOES NOT EXIST. CREATING NEW ONE");
                    await this.client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(databaseName, collectionName), updatedObject);
                }
                else
                {
                    throw;
                }
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine("NO DOCUMENT LINK");
                //TODO GUI output that the document does not exist
            }

        }

        private async Task DeleteDocument(string databaseName, string collectionName, string documentId)
        {
            try
            {
                await this.client.DeleteDocumentAsync(UriFactory.CreateDocumentUri(databaseName, collectionName,
                    documentId));
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == HttpStatusCode.NotFound)
                {
                    Console.WriteLine("DOCUMENT DOES NOT EXIST");
                    //TODO GUI output that the document does not exist
                }
                else
                {
                    throw;
                }

            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine("NO DOCUMENT LINK");
                //TODO GUI output that the document does not exist
            }
        }

    }
}
