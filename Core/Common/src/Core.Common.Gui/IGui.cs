// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IGui.cs" company="Deltares">Deltares. All rights reserved.</copyright>
// <summary>
//   Provides graphical user interface logic required to work with an application.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using Core.Common.Base.Data;
using Core.Common.Base.Plugin;
using Core.Common.Controls.Views;
using Core.Common.Gui.Forms.MainWindow;
using Core.Common.Gui.Forms.PropertyGridView;
using Core.Common.Utils.PropertyBag;

namespace Core.Common.Gui
{
    /// <summary>
    /// Provides graphical user interface logic required to work with an application.
    /// </summary>
    public interface IGui : IDisposable
    {
        #region Public properties

        event Action<Project> ProjectOpened;
        event Action<Project> ProjectClosing;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the <see cref="ApplicationCore"/> of the <see cref="IGui"/>.
        /// </summary>
        ApplicationCore ApplicationCore { get; }

        /// <summary>
        /// Gets or sets the project of the <see cref="IGui"/>.
        /// </summary>
        Project Project { get; set; }

        /// <summary>
        /// Gets or sets the project path of the <see cref="IGui"/>.
        /// </summary>
        string ProjectFilePath { get; set; }

        /// <summary>
        /// Gets or sets CommandHandler.
        /// </summary>
        IGuiCommandHandler CommandHandler { get; set; }

        /// <summary>
        /// Gets commands.
        /// </summary>
        IList<IGuiCommand> Commands { get; }

        /// <summary>
        /// Gets the currently active document <see cref="IView"/>.
        /// </summary>
        IView ActiveView { get; }

        /// <summary>
        ///  Gets all document views currently opened in the gui.
        /// </summary>
        IViewList DocumentViews { get; }

        /// <summary>
        /// Resolves document views
        /// </summary>
        IViewResolver DocumentViewsResolver { get; }

        /// <summary>
        /// Object responsible for retrieving the <see cref="ObjectProperties{T}"/> instance 
        /// for a given data object and wrapping that in a <see cref="DynamicPropertyBag"/> 
        /// for the application to be used.
        /// </summary>
        IPropertyResolver PropertyResolver { get; }

        /// <summary>
        /// Gets main window of the graphical user interface.
        /// </summary>
        IMainWindow MainWindow { get; }

        /// <summary>
        /// Gets view manager used to handle tool windows.
        /// </summary>
        IViewList ToolWindowViews { get; }

        /// <summary>
        /// List of plugins.
        /// </summary>
        IList<GuiPlugin> Plugins { get; }

        /// <summary>
        /// Gets the fixed settings of the <see cref="IGui"/>.
        /// </summary>
        GuiCoreSettings FixedSettings { get; }

        /// <summary>
        /// Gets the user specific settings of the <see cref="IGui"/>.
        /// </summary>
        ApplicationSettingsBase UserSettings { get; }

        /// <summary>
        /// Suspends view removal on item delete. Useful to avoid unnecessary checks (faster item removal).
        /// </summary>
        bool IsViewRemoveOnItemDeleteSuspended { get; set; }

        /// <summary>
        /// Gets or sets current selected object(s). 
        /// Visibility of the menus, toolbars and other controls should be updated when selected object is changed.
        /// Default implementation will also show it in the PropertyGrid.
        /// </summary>
        object Selection { get; set; }

        /// <summary>
        /// Gets the <see cref="IContextMenuBuilderProvider"/> of the <see cref="IGui"/>
        /// </summary>
        IContextMenuBuilderProvider ContextMenuProvider { get; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Exits gui by user request.
        /// </summary>
        void Exit();

        /// <summary>
        /// Returns GuiPlugin for a given type.
        /// TODO: a bit too implicit method, to be removed.
        /// </summary>
        /// <param name="type">Any type loaded from plugin.</param>
        /// <returns>Plugin gui associated with a given type</returns>
        GuiPlugin GetPluginGuiForType(Type type);

        /// <summary>
        /// Queries the plugins to get all data with view definitions recursively given a
        /// piece of hierarchical data.
        /// </summary>
        /// <param name="rootDataObject">The root data object.</param>
        /// <returns>An enumeration of all (child)data that have view definitions declared.</returns>
        IEnumerable GetAllDataWithViewDefinitionsRecursively(object rootDataObject);

        /// <summary>
        /// Runs gui. Internally it runs <see cref="ApplicationCore"/>, initializes all user interface components, including 
        /// those loaded from plugins. After that it creates and shows main window.
        /// </summary>
        void Run();

        /// <summary>
        /// Runs gui and opens a given project in gui.ApplicationCore.
        /// </summary>
        /// <param name="projectPath">Path to the project to be opened.</param>
        void Run(string projectPath);

        /// <summary>
        /// Updates the title of the main window.
        /// </summary>
        void UpdateTitle();

        /// <summary>
        /// Update the tool tip for every view currently open. Reasons for doing so 
        /// include the modification of the tree structure which is reflected in a tool tip.
        /// </summary>
        void UpdateToolTips();

        /// <summary>
        /// Open the tool view and make it visible in the interface.
        /// </summary>
        /// <param name="toolView">The tool view to open.</param>
        void OpenToolView(IView toolView);

        /// <summary>
        /// Close the tool view removing it from the interface.
        /// </summary>
        /// <param name="toolView">The tool view to close.</param>
        void CloseToolView(IView toolView);

        #endregion

        #region Events

        /// <summary>
        /// Fired when user changes selection by clicking on it or by setting it using Selection property.
        /// </summary>
        event EventHandler<SelectedItemChangedEventArgs> SelectionChanged;

        /// <summary>
        /// Fired when the active view in the document pane changes.
        /// </summary>
        event EventHandler<ActiveViewChangeEventArgs> ActiveViewChanged;

        #endregion
    }
}