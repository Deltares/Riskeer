using System.Data.Entity;

namespace Application.Ringtoets.Storage.Persistors
{
    /// <summary>
    /// Interface for entity persistors.
    /// </summary>
    public interface IEntityPersistor<T1, T2> where T1 : class where T2 : class
    {
        /// <summary>
        /// Insert the <see cref="T2"/>, based upon the <paramref name="model"/>, in the sequence.
        /// </summary>
        /// <remarks>Execute <see cref="IDbSet{TEntity}"/>.SaveChanges() afterwards to update the storage.</remarks>
        /// <param name="model"><see cref="T1"/> to be inserted in the storage.</param>
        /// <returns>New instance of <see cref="T2"/>.</returns>
        T2 AddEntity(T1 model);

        /// <summary>
        /// Updates the <see cref="T2"/>, based upon the <paramref name="model"/>, in the sequence.
        /// </summary>
        /// <remarks>Execute <see cref="IDbSet{TEntity}"/>.SaveChanges() afterwards to update the storage.</remarks>
        /// <param name="model"><see cref="T1"/> to be saved in the storage.</param>
        /// <returns>The <see cref="T2"/>.</returns>
        T2 UpdateEntity(T1 model);
    }
}