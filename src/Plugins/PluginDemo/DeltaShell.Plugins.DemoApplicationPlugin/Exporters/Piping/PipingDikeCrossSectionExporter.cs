using System;
using System.Collections.Generic;
using System.Drawing;
using DelftTools.Shell.Core;
using DelftTools.Utils.Collections.Generic;
using DeltaShell.Plugins.DemoApplicationPlugin.DataObjects.Schematization;

namespace DeltaShell.Plugins.DemoApplicationPlugin.Exporters.Piping
{
    public class PipingDikeCrossSectionExporter : IFileExporter
    {
        public string Name
        {
            get
            {
                return "Piping dike cross section exporter";
            }
        }

        public string Category
        {
            get
            {
                return "Piping";
            }
        }

        public bool Export(object item, string path)
        {
            // TODO: Implement export routine

            return true;
        }

        public IEnumerable<Type> SourceTypes()
        {
            yield return typeof(IEventedList<DikeCrossSection>);
        }

        public string FileFilter
        {
            get
            {
                return "Shape files (*.shp)|*.shp";
            }
        }

        public Bitmap Icon { get; private set; }

        public bool CanExportFor(object item)
        {
            return true;
        }
    }
}