namespace SmartServices.Framework.Core;

/// <summary>
/// Base class for all models in the application.
/// Provides common properties for data transfer objects and view models.
/// </summary>
public abstract class ModelBase
{
    /// <summary>
    /// Gets or sets the unique identifier.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the creation timestamp.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the last modification timestamp.
    /// </summary>
    public DateTime? ModifiedAt { get; set; }

    /// <summary>
    /// Initializes a new instance of the ModelBase class.
    /// </summary>
    protected ModelBase()
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Marks the model as modified.
    /// </summary>
    public void MarkAsModified()
    {
        ModifiedAt = DateTime.UtcNow;
    }
}

/// <summary>
/// Generic interface for entities with identity.
/// </summary>
/// <typeparam name="TKey">The type of the entity key.</typeparam>
public interface IEntity<out TKey> where TKey : notnull
{
    /// <summary>
    /// Gets the entity's unique identifier.
    /// </summary>
    TKey Id { get; }
}

/// <summary>
/// Marker interface for entities with GUID identity.
/// </summary>
public interface IEntity : IEntity<Guid>
{
}
