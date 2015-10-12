using System;
using System.Collections.Generic;
using System.Drawing;
using DelftTools.Shell.Core;

namespace DeltaShell.Tests.TestObjects
{
    public class TestImporter : IFileImporter
    {
        public string Name
        {
            get
            {
                return "Test Importer";
            }
        }

        public string Category { get; private set; }

        public Bitmap Image { get; private set; }

        public bool CanImportOnRootLevel
        {
            get
            {
                return true;
            }
        }

        public IEnumerable<Type> SupportedItemTypes
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string FileFilter
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool OpenViewAfterImport
        {
            get
            {
                return false;
            }
        }

        public string TargetDataDirectory
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public bool ShouldCancel { get; set; }

        public ImportProgressChangedDelegate ProgressChanged { get; set; }

        public bool CanImportOn(object targetObject)
        {
            throw new NotImplementedException();
        }

        public object ImportItem(string path, object target = null)
        {
            throw new NotImplementedException();
        }
    }
}