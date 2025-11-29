using SmartServices.Framework.Core;

namespace SmartServices.Framework.Domain;

/// <summary>
/// Base class for all domain entities.
/// Provides common properties required by all entities.
/// </summary>
public abstract class EntityBase : IEntity
{
    /// <summary>
    /// Gets or sets the unique identifier for the entity.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the creation timestamp in UTC.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the last modification timestamp in UTC, if applicable.
    /// </summary>
    public DateTime? ModifiedAt { get; set; }

    /// <summary>
    /// Gets a value indicating whether this entity is transient (not yet persisted).
    /// </summary>
    public bool IsTransient => Id == Guid.Empty;

    /// <summary>
    /// Initializes a new instance of the EntityBase class.
    /// </summary>
    protected EntityBase()
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Initializes a new instance of the EntityBase class with a specific identifier.
    /// </summary>
    /// <param name="id">The entity identifier.</param>
    protected EntityBase(Guid id)
    {
        Id = id;
        CreatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Marks the entity as modified.
    /// </summary>
    public virtual void MarkAsModified()
    {
        ModifiedAt = DateTime.UtcNow;
    }
}

/// <summary>
/// Base class for auditable entities that track creation and modification information.
/// </summary>
public abstract class AuditableEntityBase : EntityBase
{
    /// <summary>
    /// Gets or sets the identifier of the user who created the entity.
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the user who last modified the entity.
    /// </summary>
    public string? ModifiedBy { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the entity has been deleted (soft delete).
    /// </summary>
    public bool IsDeleted { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when the entity was deleted (soft delete).
    /// </summary>
    public DateTime? DeletedAt { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the user who deleted the entity.
    /// </summary>
    public string? DeletedBy { get; set; }

    /// <summary>
    /// Initializes a new instance of the AuditableEntityBase class.
    /// </summary>
    protected AuditableEntityBase() : base()
    {
    }

    /// <summary>
    /// Initializes a new instance of the AuditableEntityBase class with a specific identifier.
    /// </summary>
    /// <param name="id">The entity identifier.</param>
    protected AuditableEntityBase(Guid id) : base(id)
    {
    }

    /// <summary>
    /// Marks the entity as deleted (soft delete).
    /// </summary>
    /// <param name="deletedBy">The identifier of the user performing the deletion.</param>
    public virtual void MarkAsDeleted(string deletedBy)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedBy;
        MarkAsModified();
    }

    /// <summary>
    /// Restores the entity from a deleted state (soft delete restoration).
    /// </summary>
    public virtual void Restore()
    {
        IsDeleted = false;
        DeletedAt = null;
        DeletedBy = null;
        MarkAsModified();
    }
}
