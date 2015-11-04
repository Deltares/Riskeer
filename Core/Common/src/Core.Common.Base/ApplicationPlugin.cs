using System.Collections.Generic;

namespace Core.Common.Base
{
    public abstract class ApplicationPlugin
    {
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