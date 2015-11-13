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

            DataItemInfo projectDataItemDefinition = dataItemDefinitions.Single(did => did.ValueType == typeof(AssessmentSection));
            Assert.AreEqual(RingtoetsFormsResources.AssessmentSectionProperties_DisplayName, projectDataItemDefinition.Name);
            Assert.AreEqual(RingtoetsFormsResources.AssessmentSectionProperties_Category, projectDataItemDefinition.Category);
            Assert.AreEqual(16, projectDataItemDefinition.Image.Width);
            Assert.AreEqual(16, projectDataItemDefinition.Image.Height);
            Assert.IsNull(projectDataItemDefinition.AdditionalOwnerCheck);
            Assert.IsInstanceOf<AssessmentSection>(projectDataItemDefinition.CreateData(null));
            Assert.IsNull(projectDataItemDefinition.AddExampleData);
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