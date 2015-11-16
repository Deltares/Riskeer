using System.Linq;

using Core.Common.Base;

using NUnit.Framework;

using Ringtoets.Piping.Plugin.FileImporter;

namespace Ringtoets.Piping.Plugin.Test
{
    [TestFixture]
    public class PipingFileImporterProviderTest
    {
        [Test]
        public void GetFileImporters_ReturnPipingImporters()
        {
            // Call
            IFileImporter[] importers = PipingFileImporterProvider.GetFileImporters().ToArray();

            // Assert
            Assert.AreEqual(2, importers.Length);
            Assert.AreEqual(1, importers.Count(i => i is PipingSurfaceLinesCsvImporter));
            Assert.AreEqual(1, importers.Count(i => i is PipingSoilProfilesImporter));
        }
    }
}