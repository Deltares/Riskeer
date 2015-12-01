using System.Collections.Generic;
using Core.Common.Base.IO;

namespace Core.Common.Base.Plugin
{
    /// <summary>
    /// Template class for application plugin definitions.
    /// </summary>
    public abstract class ApplicationPlugin
    {
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

        /// <summary>
        /// Data items which can be provided by the application plugin.
        /// </summary>
        public virtual IEnumerable<DataItemInfo> GetDataItemInfos()
        {
            yield break;
        }
    }
}