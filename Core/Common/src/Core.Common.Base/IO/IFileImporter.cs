using System;
using System.Drawing;

namespace Core.Common.Base.IO
{
    public delegate void ImportProgressChangedDelegate(string currentStepName, int currentStep, int totalSteps);

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
        /// <remarks>This image can be used in selection and progress dialogs.</remarks>
        Bitmap Image { get; }

        /// <summary>
        /// Gets the <see cref="Type"/> of the items supported by the <see cref="IFileImporter"/>.
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
        /// Performs an import on <paramref name="targetItem"/> from a file with path <paramref name="filePath"/>
        /// and returns <paramref name="targetItem"/>.
        /// </summary>
        /// <param name="filePath">The path of the file to import the data from.</param>
        /// <param name="targetItem">The item to perform the import on.</param>
        /// <returns></returns>
        object ImportItem(string filePath, object targetItem);

        /// <summary>
        /// This method checks if an import can be performed on <paramref name="targetItem"/>.
        /// </summary>
        /// <param name="targetItem">The target item to check.</param>
        /// <returns><c>true</c> if an import can be performed on <paramref name="targetItem"/>. <c>false</c> otherwise.</returns>
        bool CanImportFor(object targetItem);

        /// <summary>
        /// Fired when progress has been changed.
        /// </summary>
        ImportProgressChangedDelegate ProgressChanged { set; }

        /// <summary>
        /// Whether or not an import task should be cancelled.
        /// </summary>
        /// <remarks>This property must be observed by the importer (thread-safe); when it is true the importer must stop current import task.</remarks>
        bool ShouldCancel { get; set; }
    }
}