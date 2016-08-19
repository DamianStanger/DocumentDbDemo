using System;
using System.Threading.Tasks;
using DocumentDbDemo.Service;
using DocumentDbDemo.Service.Configuration;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;

namespace DocumentDbDemo.Data
{
    public class AggregateRepository
    {
        private readonly Config _config;
        private readonly DocumentClient _client;

        public AggregateRepository(ConfigFactory configFactory)
        {
            _config = configFactory.GetConfig();
            _client = new DocumentClient(_config.DocumentDbServiceEndpoint, _config.DocumentDbAuthKey);
            CreateCollectionIfNotExists();
        }

        private void CreateCollectionIfNotExists()
        {
            try
            {
                _client.ReadDocumentCollectionAsync(UriFactory.CreateDocumentCollectionUri(_config.DocumentDbDatabaseId, _config.DocumentDbCollectionId)).Wait();
            }
            catch (AggregateException e)
            {
                if (((DocumentClientException)e.InnerException).StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    _client.CreateDocumentCollectionAsync(
                        UriFactory.CreateDatabaseUri(_config.DocumentDbDatabaseId),
                        new DocumentCollection { Id = _config.DocumentDbCollectionId },
                        new RequestOptions { OfferThroughput = 1000 }).Wait();
                }
                else
                {
                    throw;
                }
            }
        }

        public async Task SaveAsync(Aggregate aggregate)
        {
            await _client.UpsertDocumentAsync(UriFactory.CreateDocumentCollectionUri(_config.DocumentDbDatabaseId, _config.DocumentDbCollectionId), aggregate);
        }

        public Aggregate Read(string policyNumber)
        {
            try
            {
                var resourceResponse = _client.ReadDocumentAsync(UriFactory.CreateDocumentUri(_config.DocumentDbDatabaseId, _config.DocumentDbCollectionId, policyNumber)).Result;
                return (Aggregate) (dynamic) resourceResponse.Resource;
            }
            catch (Exception ex) when (ex.InnerException.Message.Contains("Resource Not Found"))
            {
                return null;
            }
        }
    }
}
