using System;
using System.Configuration;

namespace DocumentDbDemo.Service.Configuration
{
    public class ConfigFactory
    {
        public virtual Config GetConfig()
        {
            return new Config
            {                
                DocumentDbAuthKey = ConfigurationManager.AppSettings["DocumentDbAuthKey"],
                DocumentDbCollectionId = ConfigurationManager.AppSettings["DocumentDbCollectionId"],
                DocumentDbDatabaseId = ConfigurationManager.AppSettings["DocumentDbDatabaseId"],
                DocumentDbServiceEndpoint = new Uri(ConfigurationManager.AppSettings["DocumentDbServiceEndpoint"]),
            };            
        }
     
    }
}