using System;
using System.Collections.Generic;
using System.Drawing;
using DelftTools.Shell.Core;
using DelftTools.Utils.Collections.Generic;
using DeltaShell.Plugins.DemoApplicationPlugin.DataObjects.Schematization;

namespace DeltaShell.Plugins.DemoApplicationPlugin.Importers.Piping
{
    public class PipingDikeCrossSectionImporter : IFileImporter
    {
        public string Name
        {
            get { return "Piping dike cross section importer"; }
        }

        public string Category
        {
            get { return "Piping"; }
        }

        public Bitmap Image
        {
            get { return null; }
        }

        public IEnumerable<Type> SupportedItemTypes
        {
            get { yield return typeof(IEventedList<DikeCrossSection>); }
        }

        public bool OpenViewAfterImport { get { return false; } }

        public bool CanImportOn(object targetObject)
        {
            return true;
        }

        public bool CanImportOnRootLevel
        {
            get { return false; }
        }

        public string FileFilter
        {
            get { return "Shape files (*.shp)|*.shp"; }
        }

        public string TargetDataDirectory
        {
            get; set;
        }
        
        public bool ShouldCancel
        {
            get; set;
        }

        public ImportProgressChangedDelegate ProgressChanged
        {
            get; set;
        }

        public object ImportItem(string path, object target = null)
        {
            // TODO: Implement import routine

            return true;
        }
    }
}