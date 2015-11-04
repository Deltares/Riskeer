using System.Collections.Generic;

namespace Ringtoets.Piping.Plugin.FileImporter
{
    /// <summary>
    /// This class can be used in importers to return a result from a method where some critical error
    /// may have occurred. The type of items which are collected is supplied by <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of the items which are returned in this result as <see cref="ICollection{T}"/>.</typeparam>
    public class PipingReadResult<T>
    {
        /// <summary>
        /// Creates a new instance of <see cref="PipingReadResult{T}"/>, for which the <see cref="CriticalErrorOccurred"/>
        /// is set to <paramref name="errorOccurred"/>.
        /// </summary>
        /// <param name="errorOccurred"><see cref="bool"/> value indicating whether an error has occurred while collecting
        /// the import items for this <see cref="PipingReadResult{T}"/>.</param>
        public PipingReadResult(bool errorOccurred)
        {
            CriticalErrorOccurred = errorOccurred;
            ImportedItems = new T[0];
        }

        /// <summary>
        /// Gets or sets the <see cref="ICollection{T}"/> of items that were imported. 
        /// </summary>
        public ICollection<T> ImportedItems { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="bool"/> representing whether an critical error has occurred during
        /// import.
        /// </summary>
        public bool CriticalErrorOccurred { get; private set; }
    }
}