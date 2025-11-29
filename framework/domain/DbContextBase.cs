using Microsoft.EntityFrameworkCore;

namespace SmartServices.Framework.Domain;

/// <summary>
/// Base class for all DbContext implementations in the application.
/// Provides common Entity Framework configuration and utilities.
/// </summary>
public abstract class DbContextBase : DbContext
{
    /// <summary>
    /// Initializes a new instance of the DbContextBase class.
    /// </summary>
    /// <param name="options">The DbContext options.</param>
    protected DbContextBase(DbContextOptions options) : base(options)
    {
    }

    /// <summary>
    /// Configures the model conventions and entity configurations.
    /// </summary>
    /// <param name="modelBuilder">The model builder instance.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply UTC kind to all DateTime properties
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(DateTime) || property.ClrType == typeof(DateTime?))
                {
                    property.SetValueConverter(
                        new Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<DateTime, DateTime>(
                            v => v.Kind == DateTimeKind.Utc ? v : v.ToUniversalTime(),
                            v => DateTime.SpecifyKind(v, DateTimeKind.Utc)
                        )
                    );
                }
            }
        }

        // Configure soft delete query filters for AuditableEntityBase
        var auditableEntityType = typeof(AuditableEntityBase);
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (auditableEntityType.IsAssignableFrom(entityType.ClrType))
            {
                entityType.AddSoftDeleteQueryFilter();
            }
        }
    }

    /// <summary>
    /// Saves all changes asynchronously with automatic audit information.
    /// </summary>
    /// <param name="userId">The identifier of the current user.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The number of entities written to the database.</returns>
    public async Task<int> SaveChangesAsync(string? userId = null, CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker
            .Entries()
            .Where(e => e.Entity is AuditableEntityBase && (
                e.State == EntityState.Added || e.State == EntityState.Modified))
            .ToList();

        var now = DateTime.UtcNow;

        foreach (var entry in entries)
        {
            if (entry.Entity is AuditableEntityBase auditable)
            {
                if (entry.State == EntityState.Added)
                {
                    auditable.CreatedBy = userId;
                    auditable.CreatedAt = now;
                }
                else if (entry.State == EntityState.Modified)
                {
                    auditable.ModifiedBy = userId;
                    auditable.ModifiedAt = now;
                }
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Saves all changes synchronously with automatic audit information.
    /// </summary>
    /// <param name="userId">The identifier of the current user.</param>
    /// <returns>The number of entities written to the database.</returns>
    public int SaveChanges(string? userId = null)
    {
        var entries = ChangeTracker
            .Entries()
            .Where(e => e.Entity is AuditableEntityBase && (
                e.State == EntityState.Added || e.State == EntityState.Modified))
            .ToList();

        var now = DateTime.UtcNow;

        foreach (var entry in entries)
        {
            if (entry.Entity is AuditableEntityBase auditable)
            {
                if (entry.State == EntityState.Added)
                {
                    auditable.CreatedBy = userId;
                    auditable.CreatedAt = now;
                }
                else if (entry.State == EntityState.Modified)
                {
                    auditable.ModifiedBy = userId;
                    auditable.ModifiedAt = now;
                }
            }
        }

        return base.SaveChanges();
    }
}

/// <summary>
/// Extension methods for ModelBuilder to configure soft delete behavior.
/// </summary>
internal static class ModelBuilderExtensions
{
    /// <summary>
    /// Adds a global query filter for soft-deleted entities.
    /// </summary>
    /// <param name="entityType">The entity type to configure.</param>
    internal static void AddSoftDeleteQueryFilter(this Microsoft.EntityFrameworkCore.Metadata.IMutableEntityType entityType)
    {
        // This would be implemented with LINQ expression in actual usage
        // Placeholder for soft delete filter configuration
    }
}
