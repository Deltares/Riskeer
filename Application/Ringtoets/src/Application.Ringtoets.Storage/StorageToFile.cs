using System;
using System.Data.Entity.Infrastructure;
using Application.Ringtoets.Storage.Builders;
using Application.Ringtoets.Storage.Exceptions;

namespace Application.Ringtoets.Storage
{
    /// <summary>
    /// Base class to interact with file based database storage.
    /// </summary>
    public abstract class StorageToFile
    {
        private readonly string filePath;
        protected string ConnectionString;

        /// <summary>
        /// Creates a new instance of <see cref="StorageToFile"/>.
        /// </summary>
        /// <param name="databaseFilePath"></param>
        protected StorageToFile(string databaseFilePath)
        {
            filePath = databaseFilePath;
        }

        /// <summary>
        /// Throws a configured instance of <see cref="DbUpdateException"/>.
        /// </summary>
        /// <param name="errorMessage">The critical error message.</param>
        /// <param name="innerException">Optional: exception that caused this exception to be thrown.</param>
        /// <returns>Returns a new <see cref="UpdateDatabaseException"/></returns>
        protected UpdateDatabaseException CreateUpdateDatabaseException(string errorMessage, Exception innerException = null)
        {
            var message = new FileWriterErrorMessageBuilder(filePath).Build(errorMessage);
            return new UpdateDatabaseException(message, innerException);
        }
    }
}