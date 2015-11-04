using System.Collections.Generic;
using System.Resources;

namespace Core.Common.Base
{
    /// <summary>
    /// Provides default functionality making it easier to implement <see cref="IPlugin"/>.
    /// Handles Active status in activate/deactivate.
    /// </summary>
    public abstract class ApplicationPlugin : IPlugin
    {
        /// <summary>
        ///  Gets or sets the application.
        ///  <value>The application.</value></summary>
        public virtual IApplication Application { get; set; }

        /// <summary>
        ///  ResourceManger of plugin. Default Properties.Resources.
        ///  </summary>
        public virtual ResourceManager Resources { get; set; }

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

        /// <summary>
        ///  Activates the plugin.
        ///  </summary>
        public virtual void Activate()
        {

        }

        /// <summary>
        ///  Deactivates the plugin.
        ///  </summary>
        public virtual void Deactivate()
        {

        }
    }
}