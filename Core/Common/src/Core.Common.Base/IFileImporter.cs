using System;
using System.Collections.Generic;
using System.Drawing;

namespace Core.Common.Base
{
    public delegate void ImportProgressChangedDelegate(string currentStepName, int currentStep, int totalSteps);

    /// <summary>
    /// Interface for data import from external formats
    /// </summary>
    public interface IFileImporter
    {
        /// <summary>
        /// The name of the importer
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The category of the importer
        /// </summary>
        string Category { get; }

        /// <summary>
        /// The image of the importer
        /// </summary>
        Bitmap Image { get; }

        /// <summary>
        /// The data types supported by the importer
        /// </summary>
        IEnumerable<Type> SupportedItemTypes { get; }

        /// <summary>
        /// The file filter of the importer
        /// </summary>
        /// <example>"My file format1 (*.ext1)|*.ext1|My file format2 (*.ext2)|*.ext2"</example>
        string FileFilter { get; }

        /// <summary>
        /// Whether or not an import task should be cancelled
        /// </summary>
        /// <remarks>This property must be observed by the importer (thread-safe); when it is true the importer must stop current import task</remarks>
        bool ShouldCancel { get; set; }

        /// <summary>
        /// Fired when progress has been changed
        /// </summary>
        ImportProgressChangedDelegate ProgressChanged { get; set; }

        /// <summary>
        /// Indicates if this importer can import on the <paramref name="targetObject"></paramref>
        /// </summary>
        /// <param name="targetObject">Target object to check</param>
        bool CanImportOn(object targetObject);

        /// <summary>
        /// Imports data from the file with path <paramref name="path"/>
        /// </summary>
        object ImportItem(string path, object target = null);
    }
}