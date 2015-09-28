using System.Collections.Generic;

namespace DelftTools.Utils.Data
{
    /// <summary>
    /// Defines functionality to be implemented to have generic data access.
    /// <see href="http://www.hibernate.org/328.html">Generic Data Access Object using Hibernate (Java)</see>.
    /// </summary>
    /// <typeparam name="T">The type of objects to be data accessed.</typeparam>
    /// <typeparam name="ID">The type of unique object identifier.</typeparam>
    public interface IObjectRepository<T, ID>
    {
        /// <summary>
        /// Returns all entities from the storage.
        /// </summary>
        /// <returns></returns>
        IList<T> GetAll();

        /// <summary>
        /// Returns an entity identified by its unique id.
        /// </summary>
        /// <param name="id">Unique id of an entity.</param>
        /// <returns></returns>
        T GetById(ID id);

        /// <summary>
        /// Adds an object to the repository or updates it fields which were changed.
        /// </summary>
        /// <param name="o"></param>
        void SaveOrUpdate(T o);
    }
}