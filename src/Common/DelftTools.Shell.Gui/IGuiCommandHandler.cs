using System;
using System.Collections.Generic;
using DelftTools.Shell.Core;

namespace DelftTools.Shell.Gui
{
    /// <summary>
    /// Handles all common high-level commands in the graphical user interface invoked via menu / toolbar.
    /// </summary>
    public interface IGuiCommandHandler : IDisposable
    {
        void CreateNewProject();

        /// <summary>
        /// Tries to open an existing WTI project.
        /// </summary>
        /// <remarks>
        /// The opening action might be cancelled (due to user interaction).
        /// </remarks>
        /// <returns>Whether or not an existing WTI project was correctly opened.</returns>
        bool TryOpenExistingWTIProject();

        /// <summary>
        /// Tries to open an existing WTI project from file.
        /// </summary>
        /// <param name="filePath">The path to the existing WTI project file.</param>
        /// <remarks>
        /// The opening action might be cancelled (due to user interaction).
        /// </remarks>
        /// <returns>Whether or not an existing WTI project was correctly opened.</returns>
        bool TryOpenExistingWTIProject(string filePath);

        bool TryCloseWTIProject();

        bool SaveProject();
        bool SaveProjectAs();

        /// <summary>
        /// Presents the user with a dialog to choose an editor for the selected dataitem
        /// </summary>
        void OpenSelectViewDialog();

        /// <summary>
        /// Opens the default view for the current selection
        /// </summary>
        void OpenDefaultViewForSelection();

        void OpenViewForSelection(Type viewType = null);

        void OpenView(object dataObject, Type viewType = null);

        void RemoveAllViewsForItem(object dataObject);
        
        /// <summary>
        /// Presents the user with a dialog from which items can be selected and then created. The items are retrieved 
        /// using the DataItemInfo objects of plugins. The item is NOT added to the project or wrapped in a DataItem.
        /// </summary>
        /// <param name="itemConstraints">The predicate which must evaluate to true for an item type to be included in the list</param>
        /// <returns></returns>
        object AddNewChildItem(object parent, IEnumerable<Type> childItemTypes);

        object AddNewProjectItem(object parent);

        ///<summary>
        ///</summary>
        ///<returns>true if there is a default vioew for the current selection</returns>
        bool CanOpenDefaultViewForSelection();

        ///<summary>
        ///</summary>
        ///<returns>true if there are more supported views for the current selection</returns>
        bool CanOpenSelectViewDialog();

        void AddItemToProject(object item);

        void ExportSelectedItem();

        /// <summary>
        /// Activates the propertyGrid toolbox
        /// </summary>
        void ShowProperties();

        // TODO: move to import plugin
        void ImportToGuiSelection();

        /// <summary>
        /// Indicates if there are importers for the current Gui.Selection
        /// </summary>
        bool CanImportToGuiSelection();

        IProjectItem GetProjectItemForActiveView();

        void OpenLogFileExternal();

        void ExportFrom(object data, IFileExporter exporter = null);

        void ImportOn(object target, IFileImporter importer = null);
    }
}