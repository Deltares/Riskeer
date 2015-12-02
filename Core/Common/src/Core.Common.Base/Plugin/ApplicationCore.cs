using System;
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.IO;
using Core.Common.Utils.Reflection;

namespace Core.Common.Base.Plugin
{
    public class ApplicationCore : IDisposable
    {
        private readonly List<ApplicationPlugin> plugins;

        public ApplicationCore()
        {
            plugins = new List<ApplicationPlugin>();
        }

        public void AddPlugin(ApplicationPlugin applicationPlugin)
        {
            plugins.Add(applicationPlugin);

            applicationPlugin.Activate();
        }

        public void RemovePlugin(ApplicationPlugin applicationPlugin)
        {
            plugins.Remove(applicationPlugin);

            applicationPlugin.Deactivate();
        }

        public virtual void Dispose()
        {
            foreach (var plugin in plugins.ToList())
            {
                RemovePlugin(plugin);
            }
        }

        public IEnumerable<IFileImporter> GetSupportedFileImporters(object target)
        {
            if (target == null)
            {
                return Enumerable.Empty<IFileImporter>();
            }

            var targetType = target.GetType();

            return plugins.SelectMany(plugin => plugin.GetFileImporters())
                          .Where(fileImporter => (fileImporter.SupportedItemType == targetType || targetType.Implements(fileImporter.SupportedItemType))
                                                 && fileImporter.CanImportFor(target));
        }

        public IEnumerable<IFileExporter> GetSupportedFileExporters(object source)
        {
            if (source == null)
            {
                return Enumerable.Empty<IFileExporter>();
            }

            var sourceType = source.GetType();

            return plugins.SelectMany(plugin => plugin.GetFileExporters())
                          .Where(fileExporter => (fileExporter.SupportedItemType == sourceType || sourceType.Implements(fileExporter.SupportedItemType))
                                                 && fileExporter.CanExportFor(source));
        }

        public IEnumerable<DataItemInfo> GetSupportedDataItemInfos(object target)
        {
            if (target == null)
            {
                return Enumerable.Empty<DataItemInfo>();
            }

            return plugins.SelectMany(p => p.GetDataItemInfos())
                          .Where(dataItemInfo => dataItemInfo.AdditionalOwnerCheck == null || dataItemInfo.AdditionalOwnerCheck(target));
        }
    }
}