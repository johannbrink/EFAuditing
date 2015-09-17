using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using EFAuditing.MongoDB;
using EFAuditing.TestHarness.Shared;

namespace EFAuditing.TestHarness.MongoDB
{
    public class Program
    {
        public void Main(string[] args)
        {
            var mongoDBProvider = new MongoDBAuditStoreProvider("mongodb://localhost", "TestLogging","AuditLogs");
            using (var myDbContext = new MyDBContext())
            {
                myDbContext.Database.EnsureDeleted();
                myDbContext.Database.EnsureCreated();

                var customer = new Customer()
                {
                    FirstName = "TestFirstName",
                    LastName = "TEstLAstNAme"
                };
                myDbContext.Customers.Add(customer);

                var auditablePropCount =
                    customer.GetType()
                        .GetProperties()
                        .Count(p => !p.GetCustomAttributes(typeof(DoNotAudit), true).Any());

                var nonAuditablePropCount =
                    customer.GetType()
                        .GetProperties()
                        .Count(p => p.GetCustomAttributes(typeof(DoNotAudit), true).Any());
                myDbContext.SaveChanges("Test User");

                Console.WriteLine($"Added object with {auditablePropCount} auditable properties and {nonAuditablePropCount} non-auditable properties.");

                var auditLogs = myDbContext.GetAuditLogs().ToList();
                Console.WriteLine($"Audit log contains {auditLogs.Count()} entries.");
                foreach (var auditLog in myDbContext.GetAuditLogs())
                {
                    Console.WriteLine($"AuditLogId:{auditLog.AuditLogId} TableName:{auditLog.TableName} ColumnName:{auditLog.ColumnName} OriginalValue:{auditLog.OriginalValue} NewValue:{auditLog.NewValue} EventDateTime:{auditLog.EventDateTime}");
                }

                if (auditLogs.Count() == auditablePropCount)
                    Console.WriteLine("Test succeeded.");
                else
                    throw new Exception("Something is wrong.");

                Console.Read();
                myDbContext.Database.EnsureDeleted();
            }
        }
    }
}
