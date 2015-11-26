using System;
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.Workflow;
using Core.Common.Utils.Reflection;

namespace Core.Common.Base
{
    public class ApplicationCore : IDisposable
    {
        private readonly ActivityRunner activityRunner;
        private readonly List<ApplicationPlugin> plugins;

        public ApplicationCore()
        {
            plugins = new List<ApplicationPlugin>();
            activityRunner = new ActivityRunner();

            if (RunningActivityLogAppender.Instance != null)
            {
                RunningActivityLogAppender.Instance.ActivityRunner = ActivityRunner;
            }
        }

        public IActivityRunner ActivityRunner
        {
            get
            {
                return activityRunner;
            }
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

            if (RunningActivityLogAppender.Instance != null)
            {
                RunningActivityLogAppender.Instance.ActivityRunner = null;
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
                          .Where(fileImporter => fileImporter.SupportedItemTypes.Any(t => t == targetType || targetType.Implements(t))
                                                 && fileImporter.CanImportOn(target));
        }

        public IEnumerable<IFileExporter> GetSupportedFileExporters(object source)
        {
            if (source == null)
            {
                return Enumerable.Empty<IFileExporter>();
            }

            var sourceType = source.GetType();

            return plugins.SelectMany(plugin => plugin.GetFileExporters())
                          .Where(fileExporter => fileExporter.SourceTypes().Any(t => t == sourceType || sourceType.Implements(t))
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