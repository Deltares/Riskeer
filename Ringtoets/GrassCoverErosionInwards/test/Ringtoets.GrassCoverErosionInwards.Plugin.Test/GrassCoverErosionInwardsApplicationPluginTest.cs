using System.Linq;
using Core.Common.Base.Plugin;
using NUnit.Framework;
using Ringtoets.GrassCoverErosionInwards.Plugin.FileImporter;

namespace Ringtoets.GrassCoverErosionInwards.Plugin.Test
{
    [TestFixture]
    public class GrassCoverErosionInwardsApplicationPluginTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // call
            var applicationPlugin = new GrassCoverErosionInwardsApplicationPlugin();

            // assert
            Assert.IsInstanceOf<ApplicationPlugin>(applicationPlugin);
        }

        [Test]
        public void GetFileImporters_Always_ReturnExpectedFileImporter()
        {
            // Setup
            var plugin = new GrassCoverErosionInwardsApplicationPlugin();

            // Call
            var importers = plugin.GetFileImporters().ToArray();

            // Assert
            Assert.AreEqual(1, importers.Length);
            Assert.IsInstanceOf<DikeProfilesImporter>(importers[0]);
        }
    }
}