using System.Data.Entity;
using System.Data.Entity.Core;
using System.Data.Entity.Core.Objects;

namespace Application.Ringtoets.Storage.DbContext
{
    /// <summary>
    /// Interface that describes the properties and methods that must be implemented on classes that extend from a database context.
    /// </summary>
    public interface IRingtoetsEntities
    {
        /// <summary>
        /// <see cref="IDbSet{TEntity}"/> of <see cref="ProjectEntity"/>
        /// </summary>
        IDbSet<ProjectEntity> ProjectEntities { get; }

        /// <summary> 
        /// Persists all updates to the database and resets change tracking in the object context, see <see cref="ObjectContext.SaveChanges()"/>.
        /// </summary>
        /// <returns>The number of state entries written to the underlying database. This can include state entries for entities and/or relationships. 
        /// Relationship state entries are created for many-to-many relationships and relationships where there is no foreign key property included in the entity class 
        /// (often referred to as independent associations).</returns>
        /// <exception cref="OptimisticConcurrencyException">An optimistic concurrency violation has occurred while saving changes.</exception>
        int SaveChanges();
    }
}