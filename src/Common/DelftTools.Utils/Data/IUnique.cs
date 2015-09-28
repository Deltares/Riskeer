using System;

namespace DelftTools.Utils.Data
{
    /// <summary>
    /// Defines a property which needs to be implemented to identify unique entity.
    /// 
    /// TODO: rename to IEntity
    /// </summary>
    /// <typeparam name="T">The type used to identify an entity.</typeparam>
    public interface IUnique<T>
    {
        /// <summary>
        /// Gets or sets id of the object.
        /// </summary>
        T Id { get; set; }

        /// <summary>
        /// Returns type of the entity.
        /// </summary>
        /// <returns></returns>
        Type GetEntityType();
    }
}