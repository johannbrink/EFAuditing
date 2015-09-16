using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Infrastructure;
using System.Linq;

namespace EFAuditing
{
    /// <summary>
    /// Summary:
    ///     A AuditingDbContext instane extends DbContext that represents a session with 
    ///     the database and can be used to query and save instances of your entities. 
    ///     DbContext is a combination of the Unit Of Work and Repository patterns.
    /// </summary>
    public abstract class AuditingDbContext : DbContext
    {
        private const string DefaultAuditTableName = "AuditLogs";
        private const string DefaultAuditSchemaName = "audit";

        private readonly string _auditTableName;
        private readonly string _auditSchemaName;
        private readonly IExternalAuditStoreProvider _externalAuditStoreProvider;

        private bool ExternalProviderSpecified => _externalAuditStoreProvider != null;

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the AuditingDbContext class (Extends Microsoft.Data.Entity.DbContext). 
        ///     This class writes Audit Logs to the current database using Entity Framework to the default table: [audit].[AuditLogs]
        ///     The Microsoft.Data.Entity.DbContext.OnConfiguring(Microsoft.Data.Entity.DbContextOptionsBuilder)
        ///     method will be called to configure the database (and other options) to be used
        ///     for this context.
        /// </summary>
        protected AuditingDbContext()
        {
            _auditTableName = DefaultAuditTableName;
            _auditSchemaName = DefaultAuditSchemaName;
        }

        /// <summary>
        ///     Initializes a new instance of the AuditingDbContext class (Extends Microsoft.Data.Entity.DbContext). 
        ///     This class writes Audit Logs to the current database using Entity Framework to the default table: [audit].[AuditLogs]
        ///     The Microsoft.Data.Entity.DbContext.OnConfiguring(Microsoft.Data.Entity.DbContextOptionsBuilder)
        ///     method will be called to configure the database (and other options) to be used
        ///     for this context.
        /// </summary>
        protected AuditingDbContext(IExternalAuditStoreProvider externalAuditStoreProvider)
        {
            _externalAuditStoreProvider = externalAuditStoreProvider;
        }

        /// <summary>
        ///     Initializes a new instance of the AuditingDbContext class (Extends Microsoft.Data.Entity.DbContext). 
        ///     This class writes Audit Logs to the current database using Entity Framework using the specified table and schema.
        ///     The Microsoft.Data.Entity.DbContext.OnConfiguring(Microsoft.Data.Entity.DbContextOptionsBuilder)
        ///     method will be called to configure the database (and other options) to be used
        ///     for this context.
        /// </summary>
        /// <param name="auditTableName">SQL Table Name</param>
        /// <param name="auditSchemaName">SQL Schema Name</param>
        protected AuditingDbContext(string auditTableName, string auditSchemaName)
        {
            _auditTableName = auditTableName;
            _auditSchemaName = auditSchemaName;
        }

        /// <summary>
        ///      Initializes a new instance of the AuditingDbContext class (Extends Microsoft.Data.Entity.DbContext) with the specified
        ///     options. The Microsoft.Data.Entity.DbContext.OnConfiguring(Microsoft.Data.Entity.DbContextOptionsBuilder)
        ///     method will still be called to allow further configuration of the options.
        /// </summary>
        /// <param name="options">The options for this context.</param>
        protected AuditingDbContext(DbContextOptions options) : base(options)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the AuditingDbContext class (Extends Microsoft.Data.Entity.DbContext) using an System.IServiceProvider.
        ///     The service provider must contain all the services required by Entity Framework
        ///     (and the database being used). The Entity Framework services can be registered
        ///     using the Microsoft.Framework.DependencyInjection.EntityFrameworkServiceCollectionExtensions.AddEntityFramework(Microsoft.Framework.DependencyInjection.IServiceCollection)
        ///     method. Most databases also provide an extension method on Microsoft.Framework.DependencyInjection.IServiceCollection
        ///     to register the services required.
        ///     If the System.IServiceProvider has a Microsoft.Data.Entity.Infrastructure.DbContextOptions
        ///     or Microsoft.Data.Entity.Infrastructure.DbContextOptions`1 registered, then this
        ///     will be used as the options for this context instance. The Microsoft.Data.Entity.DbContext.OnConfiguring(Microsoft.Data.Entity.DbContextOptionsBuilder)
        ///     method will still be called to allow further configuration of the options.
        /// </summary>
        /// <param name="serviceProvider">The service provider to be used.</param>
        protected AuditingDbContext(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the AuditingDbContext class (Extends Microsoft.Data.Entity.DbContext) using
        ///     an System.IServiceProvider and the specified options.
        ///     The Microsoft.Data.Entity.DbContext.OnConfiguring(Microsoft.Data.Entity.DbContextOptionsBuilder)
        ///     method will still be called to allow further configuration of the options.
        ///     The service provider must contain all the services required by Entity Framework
        ///     (and the databases being used). The Entity Framework services can be registered
        ///     using the Microsoft.Framework.DependencyInjection.EntityFrameworkServiceCollectionExtensions.AddEntityFramework(Microsoft.Framework.DependencyInjection.IServiceCollection)
        ///     method. Most databases also provide an extension method on Microsoft.Framework.DependencyInjection.IServiceCollection
        ///     to register the services required.
        ///     If the System.IServiceProvider has a Microsoft.Data.Entity.Infrastructure.DbContextOptions
        ///     or Microsoft.Data.Entity.Infrastructure.DbContextOptions`1 registered, then this
        ///     will be used as the options for this context instance. The Microsoft.Data.Entity.DbContext.OnConfiguring(Microsoft.Data.Entity.DbContextOptionsBuilder)
        ///     method will still be called to allow further configuration of the options.
        /// </summary>
        /// <param name="serviceProvider">The service provider to be used.</param>
        /// <param name="options">The options for this context.</param>
        protected AuditingDbContext(IServiceProvider serviceProvider, DbContextOptions options)
            : base(serviceProvider, options)
        {
        }

        #endregion

        #region Obsolete Base Members

        [Obsolete("A UserName is required. Use SaveChanges(string userName) instead.")]
        public new int SaveChanges()
        {
            throw new InvalidOperationException("A UserName is required. Use SaveChanges(string userName) instead.");
        }

        [Obsolete("A UserName is required. Use SaveChanges(string userName) instead.")]
        public new int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            throw new InvalidOperationException("A UserName is required. Use SaveChanges(string userName) instead.");
        }


        [Obsolete("A UserName is required. Use SaveChangesAsync(userName, cancellationToken) instead.")]
        public new Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new InvalidOperationException(
                "A UserName is required. Use SaveChangesAsync(userName, cancellationToken) instead.");
        }

        [Obsolete("A UserName is required. Use SaveChangesAsync(userName, cancellationToken) instead.")]
        public new Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new InvalidOperationException(
                "A UserName is required. Use SaveChangesAsync(userName, cancellationToken) instead.");
        }

        #endregion

        /// <summary>
        /// Override this method to further configure the model that was discovered by convention from the entity types
        ///                 exposed in <see cref="T:Microsoft.Data.Entity.DbSet`1"/> properties on your derived context. The resulting model may be cached
        ///                 and re-used for subsequent instances of your derived context.
        /// </summary>
        /// <param name="modelBuilder">The builder being used to construct the model for this context. Databases (and other extensions) typically
        ///                 define extension methods on this object that allow you to configure aspects of the model that are specific
        ///                 to a given database.
        ///             </param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            if (!ExternalProviderSpecified)
                ConfigureModelBuilder(modelBuilder, _auditTableName, _auditSchemaName);
        }

        /// <summary>
        /// Configuration that sets up the ModelBuilder. 
        /// This method can be used in scenarios where OnModelCreating will not be called like 
        /// when an external DbContext is used to centrally manage code first migrations when multiple
        /// DbContexts is used in the same solution on the same database. This typically occurs when following using Bounded Contexts
        /// as per DDD (Domain Driven Design)
        /// </summary>
        /// <param name="modelBuilder">The builder being used to construct the model for this context. Databases (and other extensions) typically define extension methods on this object that allow you to configure aspects of the model that are specific to a given database.</param>
        /// <param name="auditTableName">SQL Table Name</param>
        /// <param name="auditSchemaName">SQL Schema Name</param>
        public static void ConfigureModelBuilder(ModelBuilder modelBuilder, string auditTableName,
            string auditSchemaName)
        {
            modelBuilder.Entity<AuditLog>().ToTable(auditTableName, schema: auditSchemaName);
        }

        /// <summary>
        /// Saves all changes made in this context to the underlying database.
        /// </summary>
        /// <remarks>
        /// This method will automatically call <see cref="M:Microsoft.Data.Entity.ChangeTracking.ChangeTracker.DetectChanges"/> to discover any changes
        ///                 to entity instances before saving to the underlying database. This can be disabled via
        ///                 <see cref="P:Microsoft.Data.Entity.ChangeTracking.ChangeTracker.AutoDetectChangesEnabled"/>.
        /// </remarks>
        /// <param name="userName">Username that will be used in the audit entry.</param>
        /// <returns>
        /// The number of state entries written to the underlying database.
        /// </returns>
        public virtual int SaveChanges(string userName)
        {
            var addedEntityEntries = ChangeTracker.Entries().Where(p => p.State == EntityState.Added).ToList();
            var modifiedEntityEntries = ChangeTracker.Entries().Where(p => p.State == EntityState.Modified).ToList();
            var deletedEntityEntries = ChangeTracker.Entries().Where(p => p.State == EntityState.Deleted).ToList();

            var result = base.SaveChanges();

            var auditLogs = AuditLogBuilder.GetAuditLogs(userName, addedEntityEntries, modifiedEntityEntries,
                deletedEntityEntries);

            if (ExternalProviderSpecified)
            {
                var task = _externalAuditStoreProvider.WriteAuditLogs(auditLogs);
                while (!task.IsCompleted)
                {
                    task.Wait(10);
                }
                if (!task.IsFaulted) return result;
                if (task.Exception != null) throw task.Exception.InnerException;
            }
            else
            {
                Set<AuditLog>().AddRange(auditLogs);
                base.SaveChanges();
            }
            return result;
        }

        /// <summary>
        /// Asynchronously saves all changes made in this context to the underlying database.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method will automatically call <see cref="M:Microsoft.Data.Entity.ChangeTracking.ChangeTracker.DetectChanges"/> to discover any changes
        ///                     to entity instances before saving to the underlying database. This can be disabled via
        ///                     <see cref="P:Microsoft.Data.Entity.ChangeTracking.ChangeTracker.AutoDetectChangesEnabled"/>.
        /// </para>
        /// <para>
        /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
        ///                     that any asynchronous operations have completed before calling another method on this context.
        /// </para>
        /// </remarks>
        /// <param name="userName">Username that will be used in the audit entry.</param>
        /// <param name="cancellationToken">A <see cref="T:System.Threading.CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous save operation. The task result contains the
        ///                 number of state entries written to the underlying database.
        /// </returns>
        public virtual Task<int> SaveChangesAsync(string userName, CancellationToken cancellationToken)
        {
            //TODO: Implement this
            throw new NotImplementedException(
                $"Audit logic not implemented for SaveChangesAsync(string userName, CancellationToken cancellationToken) yet. Use SaveChanges(string userName) instead.");
            //return base.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Reads the audit logs. Simple method that should be refined when using external AuditStoreProviders.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<AuditLog> GetAuditLogs()
        {
            return ExternalProviderSpecified ? _externalAuditStoreProvider.ReadAuditLogs() : Set<AuditLog>();
            //TODO: This should filter when using external providers as _externalAuditStoreProvider.ReadAuditLogs() does not chain LINQ to subsystem queries.
        }
    }
}