namespace Microsoft.eShopOnContainers.Services.Locations.API.Infrastructure
{
    using Microsoft.eShopOnContainers.Services.Locations.API.Model;
    using Microsoft.Extensions.Options;
    using MongoDB.Driver;
    using System;

    public class LocationsContext
    {
        private readonly IMongoDatabase _database = null;

        public LocationsContext(IOptions<LocationSettings> settings, IMongoClient mongoClient, MongoUrl mongoUrl)
        {
            if (mongoClient != null)
            {
                Console.WriteLine($"Connecting to: {mongoClient.Settings.Server.Host}:{mongoClient.Settings.Server.Port}/{mongoUrl.DatabaseName}");
                _database = mongoClient.GetDatabase(mongoUrl.DatabaseName);
            }
        }

        public IMongoCollection<UserLocation> UserLocation
        {
            get
            {
                return _database.GetCollection<UserLocation>("UserLocation");
            }
        }

        public IMongoCollection<Locations> Locations
        {
            get
            {
                return _database.GetCollection<Locations>("Locations");
            }
        }       
    }
}
