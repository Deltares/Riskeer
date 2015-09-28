﻿using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Resources;

namespace DelftTools.Shell.Core
{
    /// <summary>
    /// Provides default functionality making it easier to implement IPlugin.
    /// Handles Active status in activate/deactivate.
    /// </summary>
    public abstract class ApplicationPlugin : IPlugin
    {
        /// <summary>
        ///  Gets the name of the plugin.
        ///  <value>The name.</value></summary>
        public abstract string Name { get; }

        /// <summary>
        ///  Gets the name of the plugin as displayed in the Gui.
        ///  <value>The name.</value></summary>
        public abstract string DisplayName { get; }

        /// <summary>
        ///  Gets the description.
        ///  <value>The description.</value></summary>
        public abstract string Description { get; }

        /// <summary>
        ///  Gets the version of the plugin.
        ///  <value>The version.</value></summary>
        public abstract string Version { get; }

        /// <summary>
        ///  Activates the plugin.
        ///  </summary>
        public virtual void Activate()
        {
            IsActive = true;
        }

        /// <summary>
        ///  Deactivates the plugin.
        ///  </summary>
        public virtual void Deactivate()
        {
            IsActive = false;
        }

        /// <summary>
        ///  Gets a value indicating whether the plugin is active.
        ///  <value><c>true</c> if this instance is active; otherwise, <c>false</c>.</value></summary>
        public virtual bool IsActive { get; protected set; }

        /// <summary>
        ///  Image for displaying in gui. Default format 32x32 bitmap or scalable. 
        ///  </summary>
        public virtual Image Image { get { return null; } }

        /// <summary>
        ///  ResourceManger of plugin. Default Properties.Resources.
        ///  </summary>
        public virtual ResourceManager Resources { get; set; }

        /// <summary>
        ///  Gets or sets the application.
        ///  <value>The application.</value></summary>
        public virtual IApplication Application { get; set; }

        /// <summary>
        /// Provides information about data that can be created
        /// </summary>
        public virtual IEnumerable<DataItemInfo> GetDataItemInfos()
        {
            yield break;
        }

        /// <summary>
        /// File importers of this plugin
        /// </summary>
        public virtual IEnumerable<IFileImporter> GetFileImporters()
        {
            yield break;
        }

        /// <summary>
        /// File exporters of this plugin
        /// </summary>
        public virtual IEnumerable<IFileExporter> GetFileExporters()
        {
            yield break;
        }

        // TODO: check if it is used, otherwise remove
        public virtual string[] DependentPluginNames { get { return new string[] { }; } }

        // TODO: check if we can remove it
        public virtual IEnumerable<Assembly> GetPersistentAssemblies() { yield break; }
    }
}