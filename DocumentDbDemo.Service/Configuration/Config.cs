using System;

namespace DocumentDbDemo.Service.Configuration
{
    public class Config
    {
        public Uri DocumentDbServiceEndpoint { get; set; }
        public string DocumentDbAuthKey { get; set; }
        public string DocumentDbCollectionId { get; set; }
        public string DocumentDbDatabaseId { get; set; }
    }
}