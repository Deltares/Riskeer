using System.Linq;

using Core.Common.Base;

using NUnit.Framework;

using Ringtoets.Integration.Data;
using Ringtoets.Piping.Plugin.FileImporter;

namespace Ringtoets.Integration.Plugin.Test
{
    [TestFixture]
    public class RingtoetsApplicationPluginTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // call
            var ringtoetsApplicationPlugin = new RingtoetsApplicationPlugin();

            // assert
            Assert.IsInstanceOf<ApplicationPlugin>(ringtoetsApplicationPlugin);
        }

        [Test]
        public void GetDataItemInfos_ReturnsExpectedDataItemDefinitions()
        {
            // setup
            var plugin = new RingtoetsApplicationPlugin();

            // call
            var dataItemDefinitions = plugin.GetDataItemInfos().ToArray();

            // assert
            Assert.AreEqual(1, dataItemDefinitions.Length);

            DataItemInfo AssessmentSectionDataItemDefinition = dataItemDefinitions.Single(did => did.ValueType == typeof(DikeAssessmentSection));
            Assert.AreEqual("Toetstraject", AssessmentSectionDataItemDefinition.Name);
            Assert.AreEqual("Algemeen", AssessmentSectionDataItemDefinition.Category);
            Assert.AreEqual(16, AssessmentSectionDataItemDefinition.Image.Width);
            Assert.AreEqual(16, AssessmentSectionDataItemDefinition.Image.Height);
            Assert.IsNull(AssessmentSectionDataItemDefinition.AdditionalOwnerCheck);
            Assert.IsInstanceOf<DikeAssessmentSection>(AssessmentSectionDataItemDefinition.CreateData(null));
            Assert.IsNull(AssessmentSectionDataItemDefinition.AddExampleData);
        }

        [Test]
        public void GetFileImporters_Always_ReturnExpectedFileImporters()
        {
            // Setup
            var plugin = new RingtoetsApplicationPlugin();

            // Call
            var importers = plugin.GetFileImporters().ToArray();

            // Assert
            Assert.AreEqual(2, importers.Length);
            Assert.IsInstanceOf<PipingSurfaceLinesCsvImporter>(importers[0]);
            Assert.IsInstanceOf<PipingSoilProfilesImporter>(importers[1]);
        }
    }
}