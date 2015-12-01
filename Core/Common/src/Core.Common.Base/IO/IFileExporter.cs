using System;
using System.Collections.Generic;
using System.Drawing;

namespace Core.Common.Base.IO
{
    /// <summary>
    /// Interface for data export to external formats.
    /// </summary>
    public interface IFileExporter
    {
        /// <summary>
        /// User readable name of the exporter.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The category of the exporter
        /// </summary>
        string Category { get; }

        /// <summary>
        /// Must be implemented if exporter exports to the file, otherwise null.
        /// <example>
        /// "My file format1 (*.ext1)|*.ext1|My file format2 (*.ext2)|*.ext2"
        /// </example>
        /// </summary>
        string FileFilter { get; }

        /// <summary>
        /// The icon shown in dialogs for the exporter
        /// </summary>
        Bitmap Icon { get; }

        /// <summary>
        /// Exports given item to the data source provided by path.
        /// </summary>
        /// <returns>True if the export was successful, false otherwise.</returns>
        bool Export(object item, string path);

        /// <summary>
        /// Exporter supports export of the following data types
        /// </summary>
        /// <returns></returns>
        IEnumerable<Type> SourceTypes();

        /// <summary>
        /// Checks if the item can be exported
        /// </summary>
        /// <param name="item">object to export</param>
        bool CanExportFor(object item);
    }
}