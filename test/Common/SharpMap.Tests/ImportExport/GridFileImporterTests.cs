using System.Collections.Generic;
using DelftShell.Plugins.SharpMapGis.ImportExport;
using DelftTools.Core;
using GeoAPI.Extensions.Coverage;
using NUnit.Framework;

namespace SharpMap.Tests.ImportExport
{
    [TestFixture]
    public class GridFileImporterTests
    {
        [Test]
        [Category("DataAccess")]
        public void ImportTwoBilFiles()
        {
            IMultipleFileImporter importer = new GridFileImporter();
            var fileNames = new[]
                                {
                                    @"../../../../data/rasterdata/timedependent/schematisatie.bil",
                                    @"../../../../data/rasterdata/timedependent/schematisatie1.bil"
                                };
            var coverages = (IList<IRegularGridCoverage>) importer.Import(null, fileNames);

            Assert.AreEqual(3, coverages[0].SizeX);
            Assert.AreEqual(3, coverages[0].SizeY);
        }
    }
}