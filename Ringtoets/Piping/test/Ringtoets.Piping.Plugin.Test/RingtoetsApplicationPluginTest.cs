using System.Linq;
using Core.Common.Base;
using NUnit.Framework;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Plugin.FileImporter;
using ApplicationResources = Ringtoets.Piping.Plugin.Properties.Resources;
using RingtoetsFormsResources = Ringtoets.Piping.Forms.Properties.Resources;

namespace Ringtoets.Piping.Plugin.Test
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

            DataItemInfo assessmentSectionDataItemDefinition = dataItemDefinitions.Single(did => did.ValueType == typeof(AssessmentSection));
            Assert.AreEqual(RingtoetsFormsResources.AssessmentSectionProperties_DisplayName, assessmentSectionDataItemDefinition.Name);
            Assert.AreEqual(RingtoetsFormsResources.AssessmentSectionProperties_Category, assessmentSectionDataItemDefinition.Category);
            Assert.AreEqual(16, assessmentSectionDataItemDefinition.Image.Width);
            Assert.AreEqual(16, assessmentSectionDataItemDefinition.Image.Height);
            Assert.IsNull(assessmentSectionDataItemDefinition.AdditionalOwnerCheck);
            Assert.IsInstanceOf<AssessmentSection>(assessmentSectionDataItemDefinition.CreateData(null));
            Assert.IsNull(assessmentSectionDataItemDefinition.AddExampleData);
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