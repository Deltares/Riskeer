using System.Collections.Generic;
using DelftTools.Shell.Core;
using DeltaShell.Plugins.DemoApplicationPlugin.DataObjects;
using DeltaShell.Plugins.DemoApplicationPlugin.Exporters.Piping;
using DeltaShell.Plugins.DemoApplicationPlugin.Factories;
using DeltaShell.Plugins.DemoApplicationPlugin.Importers.Piping;
using Mono.Addins;

namespace DeltaShell.Plugins.DemoApplicationPlugin
{
    [Extension(typeof(IPlugin))]
    public class WTIApplicationPlugin : ApplicationPlugin
    {
        public override string Name
        {
            get { return "WTI application plugin"; }
        }

        public override string DisplayName
        {
            get { return "WTI application plugin"; }
        }

        public override string Description
        {
            get { return "WTI application plugin"; }
        }

        public override string Version
        {
            get { return "1.0.0.0"; }
        }

        public override IEnumerable<DataItemInfo> GetDataItemInfos()
        {
            yield return new DataItemInfo<WTIProject>
                {
                    Name = "WTI project",
                    Category = "WTI",
                    Image = Properties.Resources.projection_screen,
                    CreateData = owner => WTIProjectFactory.CreateWTIProject()
                };
        }

        public override IEnumerable<IFileImporter> GetFileImporters()
        {
            yield return new PipingDikeCrossSectionImporter();
        }

        public override IEnumerable<IFileExporter> GetFileExporters()
        {
            yield return new PipingDikeCrossSectionExporter();
        }
    }
}
