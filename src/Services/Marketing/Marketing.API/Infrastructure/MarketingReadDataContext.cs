namespace Microsoft.eShopOnContainers.Services.Marketing.API.Infrastructure
{
    using System;
    using Microsoft.eShopOnContainers.Services.Marketing.API.Model;
    using Microsoft.Extensions.Options;
    using MongoDB.Driver;

    public class MarketingReadDataContext
    {
        private readonly IMongoDatabase _database = null;

        public MarketingReadDataContext(IOptions<MarketingSettings> settings, IMongoClient mongoClient, MongoUrl mongoUrl)
        {
            if (mongoClient != null)
            {
                Console.WriteLine($"Connecting to: {mongoClient.Settings.Server.Host}:{mongoClient.Settings.Server.Port}/{mongoUrl.DatabaseName}");
                _database = mongoClient.GetDatabase(mongoUrl.DatabaseName);
            }
        }

        public IMongoCollection<MarketingData> MarketingData
        {
            get
            {
                return _database.GetCollection<MarketingData>("MarketingReadDataModel");
            }
        }        
    }
}
