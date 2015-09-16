using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;

namespace EFAuditing.MongoDB
{
    public class MongoDBAuditStoreProvider : IExternalAuditStoreProvider
    {
        private readonly string _connectionString;
        private readonly string _databaseName;
        private readonly string _collectionName;

        public MongoDBAuditStoreProvider(string connectionString, string databaseName, string collectionName)
        {
            _connectionString = connectionString;
            _databaseName = databaseName;
            _collectionName = collectionName;
        }

        public List<AuditLog> ReadAuditLogs()
        {
            throw new NotImplementedException();
        }

        public async void WriteAuditLogsAsync(IEnumerable<AuditLog> auditLogs)
        {
            var client = new MongoClient(_connectionString);
            var database = client.GetDatabase(_databaseName);
            var collection = database.GetCollection<BsonDocument>(_collectionName);
            foreach (var auditLog in auditLogs)
            {
                auditLog.ToBsonDocument(typeof(AuditLog), )
            }
            collection.InsertManyAsync(auditLogs.ToBsonDocument<AuditLog>());
            throw new NotImplementedException();
        }
    }
}
