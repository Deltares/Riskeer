using System;
using System.Drawing;

namespace Core.Common.Base.IO
{
    /// <summary>
    /// Interface for data export to external formats.
    /// </summary>
    public interface IFileExporter
    {
        /// <summary>
        /// Gets the name of the <see cref="IFileExporter"/>.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the category of the <see cref="IFileExporter"/>.
        /// </summary>
        string Category { get; }

        /// <summary>
        /// Gets the image of the <see cref="IFileExporter"/>.
        /// </summary>
        /// <remarks>This image can be used in selection and/or progress dialogs.</remarks>
        Bitmap Image { get; }

        /// <summary>
        /// Gets the <see cref="Type"/> of the items supported by the <see cref="IFileExporter"/>.
        /// </summary>
        Type SupportedItemType { get; }

        /// <summary>
        /// Gets the file filter of the <see cref="IFileExporter"/>.
        /// </summary>
        /// <example>
        /// "My file format1 (*.ext1)|*.ext1|My file format2 (*.ext2)|*.ext2".
        /// </example>
        string FileFilter { get; }

        /// <summary>
        /// Performs the export of <paramref name="sourceItem"/> to a file with path <paramref name="filePath"/>
        /// and returns a <see cref="bool"/> which reflects whether or not the export was successful.
        /// </summary>
        /// <param name="sourceItem">The item to export the data from.</param>
        /// <param name="filePath">The path of the file to export the data to.</param>
        /// <returns><c>true</c> if the export was successful. <c>false</c> otherwise.</returns>
        bool Export(object sourceItem, string filePath);

        /// <summary>
        /// This method checks if <paramref name="sourceItem"/> can be exported.
        /// </summary>
        /// <param name="sourceItem">The source item to check.</param>
        /// <returns><c>true</c> if the <paramref name="sourceItem"/> can be exported. <c>false</c> otherwise.</returns>
        bool CanExportFor(object sourceItem);
    }
}