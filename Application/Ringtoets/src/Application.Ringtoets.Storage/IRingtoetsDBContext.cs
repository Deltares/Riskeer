using System.Data.Entity;
using System.Data.Entity.Core.Objects;

namespace Application.Ringtoets.Storage
{
    /// <summary>
    /// Interface that describes the properties and methods that must be implemented on classes that use a database context.
    /// </summary>
    public interface IRingtoetsDbContext
    {
        /// <summary>
        /// <see cref="IDbSet{TEntity}"/> of <see cref="ProjectEntity"/>
        /// </summary>
        IDbSet<ProjectEntity> ProjectEntities { get; set; }

        /// <summary> 
        /// <see cref="ObjectContext.SaveChanges()"/>
        /// </summary>
        /// <returns>The number of objects in an Added, Modified, or Deleted state when SaveChanges was called.</returns>
        int SaveChanges();
    }
}