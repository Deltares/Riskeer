using System;
using System.Drawing;

namespace Core.Common.Base.IO
{
    /// <summary>
    /// Interface for data import from external formats.
    /// </summary>
    public interface IFileImporter
    {
        /// <summary>
        /// Gets the name of the <see cref="IFileImporter"/>.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the category of the <see cref="IFileImporter"/>.
        /// </summary>
        string Category { get; }

        /// <summary>
        /// Gets the image of the <see cref="IFileImporter"/>.
        /// </summary>
        /// <remarks>This image can be used in selection and/or progress dialogs.</remarks>
        Bitmap Image { get; }

        /// <summary>
        /// Gets the <see cref="Type"/> of the item supported by the <see cref="IFileImporter"/>.
        /// </summary>
        Type SupportedItemType { get; }

        /// <summary>
        /// Gets the file filter of the <see cref="IFileImporter"/>.
        /// </summary>
        /// <example>
        /// "My file format1 (*.ext1)|*.ext1|My file format2 (*.ext2)|*.ext2".
        /// </example>
        string FileFilter { get; }

        /// <summary>
        /// Sets the action to perform when progress has changed.
        /// </summary>
        ProgressChangedDelegate ProgressChanged { set; }

        /// <summary>
        /// This method imports the data to an item from a file at the given location.
        /// </summary>
        /// <param name="targetItem">The item to perform the import on.</param>
        /// <param name="filePath">The path of the file to import the data from.</param>
        /// <returns><c>true</c> if the import was successful. <c>false</c> otherwise.</returns>
        /// <remarks>Implementations of this import method are allowed to throw exceptions of any kind.</remarks>
        bool Import(object targetItem, string filePath);

        /// <summary>
        /// This method cancels an import.
        /// </summary>
        void Cancel();
    }
}