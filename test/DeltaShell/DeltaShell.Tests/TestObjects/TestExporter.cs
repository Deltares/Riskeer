using System;
using System.Collections.Generic;
using System.Drawing;
using DelftTools.Shell.Core;

namespace DeltaShell.Tests.TestObjects
{
    public class TestExporter : IFileExporter
    {
        public string Name
        {
            get
            {
                return "test exporter";
            }
        }

        public string Category
        {
            get
            {
                return "General";
            }
        }

        public string FileFilter
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public Bitmap Icon { get; private set; }

        public bool Export(object item, string path)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Type> SourceTypes()
        {
            throw new NotImplementedException();
        }

        public bool CanExportFor(object item)
        {
            return true;
        }
    }
}