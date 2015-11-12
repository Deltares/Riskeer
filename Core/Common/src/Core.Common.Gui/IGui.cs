// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IGui.cs" company="Deltares">Deltares. All rights reserved.</copyright>
// <summary>
//   Provides graphical user interface logic required to work with an application.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Core.Common.Base;
using Core.Common.Gui.Forms.MainWindow;

namespace Core.Common.Gui
{
    /// <summary>
    /// Provides graphical user interface logic required to work with an application.
    /// </summary>
    public interface IGui
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets Application wrapped by the current Gui.
        /// </summary>
        IApplication Application { get; set; }

        /// <summary>
        /// Gets or sets CommandHandler.
        /// </summary>
        IGuiCommandHandler CommandHandler { get; set; }

        /// <summary>
        /// Gets commands.
        /// </summary>
        IList<IGuiCommand> Commands { get; }

        /// <summary>
        ///  Gets all document views currently opened in the gui.
        /// </summary>
        IViewList DocumentViews { get; }

        /// <summary>
        /// Resolves document views
        /// </summary>
        IViewResolver DocumentViewsResolver { get; }

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
        /// Suspends view removal on item delete. Useful to avoid unnecessary checks (faster item removal).
        /// </summary>
        bool IsViewRemoveOnItemDeleteSuspended { get; set; }

        /// <summary>
        /// Gets or sets current selected object(s). 
        /// Visibility of the menus, toolbars and other controls should be updated when selected object is changed.
        /// Default implementation will also show it in the PropertyGrid.
        /// </summary>
        object Selection { get; set; }

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
        /// Runs gui. Internally it runs <see cref="Application"/>, initializes all user interface components, including 
        /// those loaded from plugins. After that it creates and shows main window.
        /// </summary>
        void Run();

        /// <summary>
        /// Runs gui and opens a given project in gui.Application.
        /// </summary>
        /// <param name="projectPath">Path to the project to be opened.</param>
        void Run(string projectPath);

        #endregion

        #region Events

        /// <summary>
        /// Fired when user changes selection by clicking on it or by setting it using Selection property.
        /// </summary>
        event EventHandler<SelectedItemChangedEventArgs> SelectionChanged;

        #endregion
    }
}