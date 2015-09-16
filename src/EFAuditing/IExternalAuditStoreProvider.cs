using System.Collections.Generic;
using System.Threading.Tasks;

namespace EFAuditing
{
    public interface IExternalAuditStoreProvider
    {
        /// <summary>
        /// Writes the audit logs. to the Provider's underlying data store.
        /// </summary>
        /// <param name="auditLogs">The audit logs.</param>
        Task WriteAuditLogs(IEnumerable<AuditLog> auditLogs);

        /// <summary>
        /// Reads the audit logs to the Provider's underlying data store. Simple method that should be removed once parameterized Read Methods are defined.
        /// </summary>
        /// <returns></returns>
        IEnumerable<AuditLog> ReadAuditLogs();
    }
}
