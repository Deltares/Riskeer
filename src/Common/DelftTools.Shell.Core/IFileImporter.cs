using System;
using System.Collections.Generic;
using System.Drawing;

namespace DelftTools.Shell.Core
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
        /// Indicates if this importer can import on the <paramref name="targetObject"></paramref>
        /// </summary>
        /// <param name="targetObject">Target object to check</param>
        bool CanImportOn(object targetObject);

        /// <summary>
        /// Indicates whether or not the importer can import at root level (folder/project). If true, the
        /// importer will always show up in the project->import list. If false this importer can only be
        /// retrieved by supported type, eg, in code. Use false for partial/composite importers and importers
        /// called from map tools etc. If true for TargetFileImporter, the importer is assumed to support
        /// both *new* and *into* modes.
        /// </summary>
        /// <remarks>HACK: REMOVE IT, KEEP DESIGN SIMPLE!</remarks>
        bool CanImportOnRootLevel { get; }

        /// <summary>
        /// The file filter of the importer
        /// </summary>
        /// <example>"My file format1 (*.ext1)|*.ext1|My file format2 (*.ext2)|*.ext2"</example>
        string FileFilter { get; }

        /// <summary>
        /// Path where external data files can be copied into
        /// </summary>
        /// <remarks>Optional, used only when external files need to be copied into the project "as is"</remarks> 
        string TargetDataDirectory { get; set; }

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
        /// Should the view for the imported item (if any) be automatically opened after import?
        /// </summary>
        bool OpenViewAfterImport { get; }

        /// <summary>
        /// Imports data from the file with path <paramref name="path"/>
        /// </summary>
        object ImportItem(string path, object target = null);
    }
}