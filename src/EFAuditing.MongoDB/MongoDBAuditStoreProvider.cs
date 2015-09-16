using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;

namespace EFAuditing.MongoDB
{
    public class MongoDBAuditStoreProvider : IExternalAuditStoreProvider
    {
        private readonly string _connectionString;

        public MongoDBAuditStoreProvider(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<AuditLog> ReadAuditLogs()
        {
            throw new NotImplementedException();
        }

        public async void WriteAuditLogsAsync(IEnumerable<AuditLog> auditLogs)
        {
            var client = new MongoClient("mongodb://localhost");
            var database = client.GetDatabase("foo");
            var collection = database.GetCollection<BsonDocument>("bar");
            throw new NotImplementedException();
        }
    }
}
