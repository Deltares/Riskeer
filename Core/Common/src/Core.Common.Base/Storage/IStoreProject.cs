using Core.Common.Base.Data;

namespace Core.Common.Base.Storage
{
    /// <summary>
    /// Interface that describes the methods that need to be implemented on classes that provide a storage for Ringtoets projects.
    /// </summary>
    public interface IStoreProject
    {
        /// <summary>
        /// Converts <paramref name="project"/> to a new storage entry.
        /// </summary>
        /// <param name="project"><see cref="Project"/> to save.</param>
        /// <param name="connectionArguments">Arguments required to connect to the storage.</param>
        /// <returns>Returns the number of changes that were saved.</returns>
        /// <exception cref="System.ArgumentException"><paramref name="connectionArguments"/> is invalid.</exception>
        /// <exception cref="CouldNotConnectException">Thrown when no new storage was created.</exception>
        /// <exception cref="StorageException">Thrown when
        /// <list type="bullet">
        /// <item>No new storage was created.</item>
        /// <item>The storage is no valid Ringtoets project.</item>
        /// <item>Saving the <paramref name="project"/> to the storage failed.</item>
        /// <item>The connection to the storage failed.</item>
        /// </list>
        /// </exception>
        int SaveProjectAs(string connectionArguments, Project project);

        /// <summary>
        /// Converts <paramref name="project"/> to an existing entity in the storage.
        /// </summary>
        /// <param name="connectionArguments"></param>
        /// <param name="project">The <see cref="Project"/> to save.</param>
        /// <returns>Returns the number of changes that were saved.</returns>
        /// <exception cref="StorageException">Thrown when
        /// <list type="bullet">
        /// <item><paramref name="connectionArguments"/> does not exist.</item>
        /// <item>The storage is no valid Ringtoets project.</item>
        /// <item>Saving the <paramref name="project"/> to the storage failed.</item>
        /// <item>The connection to the storage failed.</item>
        /// <item>The related entity was not found in the storage. Therefore, no update was possible.</item>
        /// </list>
        /// </exception>
        int SaveProject(string connectionArguments, Project project);

        /// <summary>
        /// Attempts to load the <see cref="Project"/> from the storage.
        /// </summary>
        /// <param name="connectionArguments">Arguments required to connect to the storage.</param>
        /// <returns>Returns a new instance of <see cref="Project"/> with the data from the storage or <c>null</c> when not found.</returns>
        /// <exception cref="System.ArgumentException"><paramref name="connectionArguments"/> is invalid.</exception>
        /// <exception cref="CouldNotConnectException">Thrown when no new storage was created.</exception>
        /// <exception cref="UpdateStorageException">Thrown when updating the storage fails.</exception>
        /// <exception cref="StorageValidationException">Thrown when the storage is no valid Ringtoets project.</exception>
        Project LoadProject(string connectionArguments);
    }
}