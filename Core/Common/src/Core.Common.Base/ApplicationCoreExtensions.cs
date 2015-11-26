using System.Collections.Generic;
using System.Linq;
using Core.Common.Utils.Reflection;

namespace Core.Common.Base
{
    public static class ApplicationCoreExtensions
    {
        public static IList<IFileImporter> GetImporters(this ApplicationCore applicationCore, object target)
        {
            var targetType = target == null ? null : target.GetType();

            IList<IFileImporter> importers = new List<IFileImporter>();

            foreach (IFileImporter importer in applicationCore.Plugins.SelectMany(plugin => plugin.GetFileImporters()))
            {
                if (targetType == null && !importer.CanImportOnRootLevel)
                {
                    //this importer requires something to import into, but we're importing globally (into project or folder), so skip it
                    continue;
                }

                // filter importers only to those which can import into targetType
                if ((targetType == null) || (importer.SupportedItemTypes.Any(t => (t == targetType) || targetType.Implements(t)) && importer.CanImportOn(target)))
                {
                    importers.Add(importer);
                }
            }

            return importers;
        }

        public static IEnumerable<IFileExporter> GetSupportedExportersForItem(this ApplicationCore applicationCore, object itemToExport)
        {
            var sourceType = itemToExport.GetType();

            return applicationCore.Plugins.SelectMany(plugin => plugin.GetFileExporters())
                                  .Where(e => e.SourceTypes().Any(type => type == sourceType || type.IsAssignableFrom(sourceType)) &&
                                              e.CanExportFor(itemToExport));
        }

        public static IEnumerable<DataItemInfo> GetSupportedDataItemInfos(this ApplicationCore applicationCore, object parent)
        {
            return applicationCore.Plugins
                                  .SelectMany(p => p.GetDataItemInfos())
                                  .Where(dataItemInfo => dataItemInfo.AdditionalOwnerCheck == null || dataItemInfo.AdditionalOwnerCheck(parent));
        }
    }
}