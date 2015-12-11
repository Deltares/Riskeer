using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Core.Common.Base.IO;
using Core.Common.Utils.Reflection;

namespace Core.Common.Base.Plugin
{
    /// <summary>
    /// Class that manages <see cref="ApplicationPlugin"/> plugins and exposes their contents (file importers, file exporters and data items).
    /// </summary>
    public class ApplicationCore : IDisposable
    {
        private readonly ICollection<ApplicationPlugin> plugins;

        /// <summary>
        /// Constructs a new <see cref="ApplicationCore"/>.
        /// </summary>
        public ApplicationCore()
        {
            plugins = new Collection<ApplicationPlugin>();
        }

        /// <summary>
        /// This method adds an <see cref="ApplicationPlugin"/> to the <see cref="ApplicationCore"/>.
        /// Additionally, the provided <see cref="ApplicationPlugin"/> is activated.
        /// </summary>
        /// <param name="applicationPlugin">The <see cref="ApplicationPlugin"/> to add and activate.</param>
        public void AddPlugin(ApplicationPlugin applicationPlugin)
        {
            plugins.Add(applicationPlugin);

            applicationPlugin.Activate();
        }

        /// <summary>
        /// This method removes an <see cref="ApplicationPlugin"/> from the <see cref="ApplicationCore"/>.
        /// Additionally, the provided <see cref="ApplicationPlugin"/> is deactivated.
        /// </summary>
        /// <param name="applicationPlugin">The <see cref="ApplicationPlugin"/> to remove and deactivate.</param>
        public void RemovePlugin(ApplicationPlugin applicationPlugin)
        {
            plugins.Remove(applicationPlugin);

            applicationPlugin.Deactivate();
        }

        /// <summary>
        /// This method returns an enumeration of <see cref="IFileImporter"/> that support the <paramref name="target"/>.
        /// </summary>
        /// <param name="target">The target to get the enumeration of supported <see cref="IFileImporter"/> for.</param>
        /// <returns>The enumeration of supported <see cref="IFileImporter"/>.</returns>
        public IEnumerable<IFileImporter> GetSupportedFileImporters(object target)
        {
            if (target == null)
            {
                return Enumerable.Empty<IFileImporter>();
            }

            var targetType = target.GetType();

            return plugins.SelectMany(plugin => plugin.GetFileImporters())
                          .Where(fileImporter => (fileImporter.SupportedItemType == targetType || targetType.Implements(fileImporter.SupportedItemType)));
        }

        /// <summary>
        /// This method returns an enumeration of <see cref="IFileExporter"/> that support the <paramref name="source"/>.
        /// </summary>
        /// <param name="source">The source to get the enumeration of supported <see cref="IFileExporter"/> for.</param>
        /// <returns>The enumeration of supported <see cref="IFileExporter"/>.</returns>
        public IEnumerable<IFileExporter> GetSupportedFileExporters(object source)
        {
            if (source == null)
            {
                return Enumerable.Empty<IFileExporter>();
            }

            var sourceType = source.GetType();

            return plugins.SelectMany(plugin => plugin.GetFileExporters())
                          .Where(fileExporter => (fileExporter.SupportedItemType == sourceType || sourceType.Implements(fileExporter.SupportedItemType)));
        }

        /// <summary>
        /// This method returns an enumeration of <see cref="DataItemInfo"/> that are supported for <paramref name="owner"/>.
        /// </summary>
        /// <param name="owner">The owner to get the enumeration of supported <see cref="DataItemInfo"/> for.</param>
        /// <returns>The enumeration of supported <see cref="DataItemInfo"/>.</returns>
        public IEnumerable<DataItemInfo> GetSupportedDataItemInfos(object owner)
        {
            if (owner == null)
            {
                return Enumerable.Empty<DataItemInfo>();
            }

            return plugins.SelectMany(p => p.GetDataItemInfos())
                          .Where(dataItemInfo => dataItemInfo.AdditionalOwnerCheck == null || dataItemInfo.AdditionalOwnerCheck(owner));
        }

        public virtual void Dispose()
        {
            foreach (var plugin in plugins.ToArray())
            {
                RemovePlugin(plugin);
            }
        }
    }
}