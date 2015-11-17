using System.Linq;
using Core.Common.Base;
using NUnit.Framework;
using Ringtoets.Piping.Plugin.FileImporter;

namespace Ringtoets.Piping.Plugin.Test
{
    [TestFixture]
    public class PipingApplicationPluginTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // call
            var ringtoetsApplicationPlugin = new PipingApplicationPlugin();

            // assert
            Assert.IsInstanceOf<ApplicationPlugin>(ringtoetsApplicationPlugin);
        }

        [Test]
        public void GetFileImporters_Always_ReturnExpectedFileImporters()
        {
            // Setup
            var plugin = new PipingApplicationPlugin();

            // Call
            var importers = plugin.GetFileImporters().ToArray();

            // Assert
            Assert.AreEqual(2, importers.Length);
            Assert.IsInstanceOf<PipingSurfaceLinesCsvImporter>(importers[0]);
            Assert.IsInstanceOf<PipingSoilProfilesImporter>(importers[1]);
        }
    }
}