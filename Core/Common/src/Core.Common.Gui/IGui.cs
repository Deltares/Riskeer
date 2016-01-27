using System;

using Core.Common.Base.Plugin;
using Core.Common.Base.Storage;
using Core.Common.Gui.Forms.PropertyGridView;
using Core.Common.Gui.PropertyBag;

namespace Core.Common.Gui
{
    /// <summary>
    /// Provides graphical user interface logic required to work with an application.
    /// </summary>
    public interface IGui : ICommandsOwner, ISettingsOwner, IToolViewController, IProjectOwner,
                            IApplicationSelection, IDocumentViewController, IContextMenuBuilderProvider,
                            IMainWindowController, IGuiPluginsHost, IDisposable
    {
        /// <summary>
        /// Object responsible for retrieving the <see cref="ObjectProperties{T}"/> instance 
        /// for a given data object and wrapping that in a <see cref="DynamicPropertyBag"/> 
        /// for the application to be used.
        /// </summary>
        IPropertyResolver PropertyResolver { get; }

        /// <summary>
        /// Gets the <see cref="ApplicationCore"/> of the <see cref="IGui"/>.
        /// </summary>
        ApplicationCore ApplicationCore { get; }

        /// <summary>
        /// Gets or sets the current storage.
        /// </summary>
        IStoreProject Storage { get; }

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
        /// Exits gui by user request.
        /// </summary>
        void Exit();
    }
}