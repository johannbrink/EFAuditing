using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace EFAuditing.MongoDB
{
    public class AuditLogExtended : AuditLog
    {
        [BsonId]
        public ObjectId ObjectId { get; set; }
    }

    public class MongoDbAuditStoreProvider : IExternalAuditStoreProvider
    {
        internal MongoClient Client { private get; set; }
        internal string ServerName { get; set; }
        internal int TimeoutMilliseconds { get; set; }
        internal string DatabaseName { get; set; }
        internal string CollectionName { get; set; }

        public MongoDbAuditStoreProvider()
        {
            ServerName = "localhost";
            DatabaseName = "local";
            CollectionName = "AuditLogs";
            TimeoutMilliseconds = 1000;
        }

        public IEnumerable<AuditLog> ReadAuditLogs()
        {
            if (Client == null)
                throw new Exception("The Start extension method was not called");
            var database = Client.GetDatabase(DatabaseName);
            var collection = database.GetCollection<AuditLogExtended>(CollectionName);
            return collection.Find(x=>x.AuditLogId >= 0).ToListAsync().Result.Select(x=>(AuditLog)x);
        }

        public async Task WriteAuditLogs(IEnumerable<AuditLog> auditLogs)
        {
            if (Client == null)
                throw new Exception("The Start extension method was not called");
            var database = Client.GetDatabase(DatabaseName);
            var collection = database.GetCollection<AuditLog>(CollectionName);
            await collection.InsertManyAsync(auditLogs);
        }
    }
}