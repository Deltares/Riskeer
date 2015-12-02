using System.Collections.Generic;
using Core.Common.Base.IO;

namespace Core.Common.Base.Plugin
{
    /// <summary>
    /// Class that provides application plugin objects (file importers, file exporters and data items).
    /// </summary>
    public abstract class ApplicationPlugin
    {
        /// <summary>
        /// This method activates the <see cref="ApplicationPlugin"/>.
        /// </summary>
        public virtual void Activate()
        {

        }

        /// <summary>
        /// This method deactivates the <see cref="ApplicationPlugin"/>.
        /// </summary>
        public virtual void Deactivate()
        {

        }

        /// <summary>
        /// This method returns a collection of <see cref="IFileImporter"/>.
        /// </summary>
        /// <returns>The collection of <see cref="IFileImporter"/> provided by the <see cref="ApplicationPlugin"/>.</returns>
        public virtual IEnumerable<IFileImporter> GetFileImporters()
        {
            yield break;
        }

        /// <summary>
        /// This method returns a collection of <see cref="IFileExporter"/>.
        /// </summary>
        /// <returns>The collection of <see cref="IFileExporter"/> provided by the <see cref="ApplicationPlugin"/>.</returns>
        public virtual IEnumerable<IFileExporter> GetFileExporters()
        {
            yield break;
        }

        /// <summary>
        /// This method returns a collection of <see cref="DataItemInfo"/>.
        /// </summary>
        /// <returns>The collection of <see cref="DataItemInfo"/> provided by the <see cref="ApplicationPlugin"/>.</returns>
        public virtual IEnumerable<DataItemInfo> GetDataItemInfos()
        {
            yield break;
        }
    }
}