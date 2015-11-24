using System.Collections.Generic;

namespace Core.Common.Base
{
    /// <summary>
    /// Template class for application plugin definitions.
    /// </summary>
    public abstract class ApplicationPlugin
    {
        /// <summary>
        /// Gets or sets the application.
        /// </summary>
        public virtual RingtoetsApplication Application { get; set; }

        /// <summary>
        /// Activates the application plugin.
        /// </summary>
        public virtual void Activate()
        {

        }

        /// <summary>
        /// Deactivates the application plugin.
        /// </summary>
        public virtual void Deactivate()
        {

        }

        /// <summary>
        /// Data items which can be provided by the application plugin.
        /// </summary>
        public virtual IEnumerable<DataItemInfo> GetDataItemInfos()
        {
            yield break;
        }

        /// <summary>
        /// File importers which can be provided by the application plugin.
        /// </summary>
        public virtual IEnumerable<IFileImporter> GetFileImporters()
        {
            yield break;
        }

        /// <summary>
        /// File exporters which can be provided by the application plugin.
        /// </summary>
        public virtual IEnumerable<IFileExporter> GetFileExporters()
        {
            yield break;
        }
    }
}