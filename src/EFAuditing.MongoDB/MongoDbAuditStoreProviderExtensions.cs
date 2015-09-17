using System;
using System.Linq;
using Microsoft.Framework.Configuration;
using MongoDB.Driver;

namespace EFAuditing.MongoDB
{
    public static class MongoDbAuditStoreProviderExtensions
    {
        public static MongoDbAuditStoreProvider WithConfiguration(
            this MongoDbAuditStoreProvider @mongoDbAuditStoreProvider, IConfiguration configuration)
        {
            var configSection = configuration.GetSection($"{nameof(MongoDbAuditStoreProvider)}");
            if (configSection == null)
                throw new ArgumentException(
                    $"Not configuration section named {nameof(MongoDbAuditStoreProvider)} found.");
            if (!configSection.GetChildren().Any())
                throw new ArgumentException(
                    $"Configuration section {nameof(MongoDbAuditStoreProvider)} did not contain any settings.");

            var serverName = configSection["Server"];
            var timeoutMilliseconds = configSection["TimeoutMilliseconds"];
            var databaseName = configSection["DatabaseName"];
            var collectionName = configSection["CollectionName"];

            @mongoDbAuditStoreProvider.ServerName = string.IsNullOrEmpty(serverName)
                ? @mongoDbAuditStoreProvider.ServerName
                : serverName;
            @mongoDbAuditStoreProvider.TimeoutMilliseconds = string.IsNullOrEmpty(timeoutMilliseconds)
                ? @mongoDbAuditStoreProvider.TimeoutMilliseconds
                : int.Parse(timeoutMilliseconds);
            @mongoDbAuditStoreProvider.DatabaseName = string.IsNullOrEmpty(databaseName)
                ? @mongoDbAuditStoreProvider.DatabaseName
                : databaseName;
            @mongoDbAuditStoreProvider.CollectionName = string.IsNullOrEmpty(collectionName)
                ? @mongoDbAuditStoreProvider.CollectionName
                : collectionName;

            return @mongoDbAuditStoreProvider;
        }

        public static MongoDbAuditStoreProvider WithServer(this MongoDbAuditStoreProvider @mongoDbAuditStoreProvider,
            string serverName)
        {
            if (serverName.Contains("mongodb://"))
                throw new ArgumentException("Please specify servername only");
            @mongoDbAuditStoreProvider.ServerName = serverName;
            return @mongoDbAuditStoreProvider;
        }

        public static MongoDbAuditStoreProvider WithTimeOut(this MongoDbAuditStoreProvider @mongoDbAuditStoreProvider,
            int timeoutMilliseconds)
        {
            @mongoDbAuditStoreProvider.TimeoutMilliseconds = timeoutMilliseconds;
            return @mongoDbAuditStoreProvider;
        }

        public static MongoDbAuditStoreProvider WithDatabase(this MongoDbAuditStoreProvider @mongoDbAuditStoreProvider,
            string databaseName)
        {
            @mongoDbAuditStoreProvider.DatabaseName = databaseName;
            return @mongoDbAuditStoreProvider;
        }

        public static MongoDbAuditStoreProvider WithCollection(
            this MongoDbAuditStoreProvider @mongoDbAuditStoreProvider, string collectionName)
        {
            @mongoDbAuditStoreProvider.CollectionName = collectionName;
            return @mongoDbAuditStoreProvider;
        }

        public static MongoDbAuditStoreProvider Start(this MongoDbAuditStoreProvider @mongoDbAuditStoreProvider)
        {
            var settings = new MongoClientSettings
            {
                ClusterConfigurator = cb => cb.ConfigureCluster(c => c.With(serverSelectionTimeout:
                    TimeSpan.FromMilliseconds(@mongoDbAuditStoreProvider.TimeoutMilliseconds))),
                Server = new MongoServerAddress(@mongoDbAuditStoreProvider.ServerName)
            };
            @mongoDbAuditStoreProvider.Client = new MongoClient(settings);
            return @mongoDbAuditStoreProvider;
        }
    }
}
