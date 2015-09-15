using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
// ReSharper disable once RedundantUsingDirective Since GetProperties Need this for dnxcore50
using System.Reflection;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.ChangeTracking;
using Microsoft.Data.Entity.Metadata;


namespace EFAuditing
{
    /// <summary>
    /// The AuditLogBuilder class is used internally by AuditingDbContext.
    /// </summary>
    internal static class AuditLogBuilder
    {
        private const string KeySeperator = ";";

        internal static IEnumerable<AuditLog> GetAuditLogs(string userName,
            IEnumerable<EntityEntry> addedEntityEntries,
            IEnumerable<EntityEntry> modifiedEntityEntries,
            IEnumerable<EntityEntry> deletedEntityEntries)
        {
            var auditLogs = new List<AuditLog>();
            foreach (
                var auditRecordsForEntityEntry in
                    addedEntityEntries.Select(
                        changedEntity => GetAuditLogs(changedEntity, userName, EntityState.Added)))
                auditLogs.AddRange(auditRecordsForEntityEntry);
            foreach (
                var auditRecordsForEntityEntry in
                    modifiedEntityEntries.Select(
                        changedEntity => GetAuditLogs(changedEntity, userName, EntityState.Modified)))
                auditLogs.AddRange(auditRecordsForEntityEntry);
            foreach (
                var auditRecordsForEntityEntry in
                    deletedEntityEntries.Select(
                        changedEntity => GetAuditLogs(changedEntity, userName, EntityState.Deleted)))
                auditLogs.AddRange(auditRecordsForEntityEntry);
            return auditLogs;
        }

        private static IEnumerable<AuditLog> GetAuditLogs(EntityEntry entityEntry, string userName, EntityState entityState)
        {
            var returnValue = new List<AuditLog>();
            var keyRepresentation = BuildKeyRepresentation(entityEntry, KeySeperator);

            var auditedPropertyNames =
                entityEntry.Entity.GetType()
                    .GetProperties().Where(p => !p.GetCustomAttributes(typeof (DoNotAudit), true).Any())
                    .Select(info => info.Name);
            foreach (var propertyEntry in entityEntry.Metadata.GetProperties()
                .Where(x => auditedPropertyNames.Contains(x.Name))
                .Select(property => entityEntry.Property(property.Name)))
            {
                returnValue.Add(new AuditLog
                {
                    KeyNames = keyRepresentation.Key,
                    KeyValues = keyRepresentation.Value,
                    OriginalValue = entityState != EntityState.Added
                        ? Convert.ToString(propertyEntry.OriginalValue)
                        : null,
                    NewValue = entityState == EntityState.Modified || entityState == EntityState.Added
                        ? Convert.ToString(propertyEntry.CurrentValue)
                        : null,
                    ColumnName = propertyEntry.Metadata.Name,
                    EventDateTime = DateTime.Now,
                    EventType = entityState.ToString(),
                    UserName = userName,
                    TableName = entityEntry.Entity.GetType().Name
                });
            }
            return returnValue;
        }


        private static KeyValuePair<string, string> BuildKeyRepresentation(EntityEntry entityEntry, string seperator)
        {
            var keyProperties = entityEntry.Metadata.GetProperties().Where(x => x.IsPrimaryKey()).ToList();
            if (keyProperties == null)
                throw new ArgumentException("No key found in");
            var keyPropertyEntries =
                keyProperties.Select(keyProperty => entityEntry.Property(keyProperty.Name)).ToList();
            var keyNameString = new StringBuilder();
            foreach (var keyProperty in keyProperties)
            {
                keyNameString.Append(keyProperty.Name);
                keyNameString.Append(seperator);
            }
            keyNameString.Remove(keyNameString.Length - 1, 1);
            var keyValueString = new StringBuilder();
            foreach (var keyPropertyEntry in keyPropertyEntries)
            {
                keyValueString.Append(keyPropertyEntry.CurrentValue);
                keyValueString.Append(seperator);
            }
            keyValueString.Remove(keyValueString.Length - 1, 1);
            var key = keyNameString.ToString();
            var value = keyValueString.ToString();
            return new KeyValuePair<string, string>(key, value);
        }
    }
}
