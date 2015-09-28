// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IGui.cs" company="Deltares">Deltares. All rights reserved.</copyright>
// <summary>
//   Provides graphical user interface logic required to work with an application.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using DelftTools.Controls;
using DelftTools.Shell.Core;
using DelftTools.Shell.Gui.Forms;

namespace DelftTools.Shell.Gui
{
    /// <summary>
    /// Provides graphical user interface logic required to work with an application.
    /// </summary>
    public interface IGui : ISelectionContainer
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
        /// Get a string that describes all loaded plugins and their versions. 
        /// </summary>
        string PluginVersions { get; }

        /// <summary>
        /// Suspends view removal on project item delete. Useful to avoid unnecessary checks (faster item removal).
        /// </summary>
        bool IsViewRemoveOnItemDeleteSuspended { get; set; }

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

        /// <summary>
        /// Fired after application has been started.
        /// </summary>
        event Action AfterRun;
        #endregion
    }
}