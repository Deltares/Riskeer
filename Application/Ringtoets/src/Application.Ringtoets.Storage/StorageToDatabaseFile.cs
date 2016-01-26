using System;
using Core.Common.Base.Storage;
using Core.Common.Utils;
using Core.Common.Utils.Builders;

namespace Application.Ringtoets.Storage
{
    /// <summary>
    /// Base class to interact with file based database storage.
    /// </summary>
    public abstract class StorageToDatabaseFile
    {
        protected string FilePath;
        protected string ConnectionString;

        /// <summary>
        /// Validates the <paramref name="filePath"/>.
        /// </summary>
        /// <param name="filePath">Path to storage database file.</param>
        /// <exception cref="System.ArgumentException"><paramref name="filePath"/> is invalid.</exception>
        protected virtual void Connect(string filePath)
        {
            FileUtils.ValidateFilePath(filePath);
            FilePath = filePath;
        }

        /// <summary>
        /// Throws a configured instance of <see cref="UpdateStorageException"/>.
        /// </summary>
        /// <param name="errorMessage">The critical error message.</param>
        /// <param name="innerException">Optional: exception that caused this exception to be thrown.</param>
        /// <returns>Returns a new <see cref="UpdateStorageException"/></returns>
        protected UpdateStorageException CreateUpdateStorageException(string errorMessage, Exception innerException = null)
        {
            var message = new FileWriterErrorMessageBuilder(FilePath).Build(errorMessage);
            return new UpdateStorageException(message, innerException);
        }
    }
}