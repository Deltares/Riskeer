using System.Linq;

using Core.Common.Base;
using Core.Common.TestUtils;

using NUnit.Framework;

using Ringtoets.Integration.Data;
using RingtoetsFormsResources = Ringtoets.Integration.Forms.Properties.Resources;

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
            Assert.AreEqual(2, dataItemDefinitions.Length);

            DataItemInfo dikeAssessmentSectionDataItemDefinition = dataItemDefinitions.Single(did => did.ValueType == typeof(DikeAssessmentSection));
            Assert.AreEqual("Dijktraject", dikeAssessmentSectionDataItemDefinition.Name);
            Assert.AreEqual("Algemeen", dikeAssessmentSectionDataItemDefinition.Category);
            TestHelper.AssertImagesAreEqual(RingtoetsFormsResources.AssessmentSectionFolderIcon, dikeAssessmentSectionDataItemDefinition.Image);
            Assert.IsNull(dikeAssessmentSectionDataItemDefinition.AdditionalOwnerCheck);
            Assert.IsInstanceOf<DikeAssessmentSection>(dikeAssessmentSectionDataItemDefinition.CreateData(null));
            Assert.IsNull(dikeAssessmentSectionDataItemDefinition.AddExampleData);

            DataItemInfo duneAssessmentDataItemDefinition = dataItemDefinitions.Single(did => did.ValueType == typeof(DuneAssessmentSection));
            Assert.AreEqual("Duintraject", duneAssessmentDataItemDefinition.Name);
            Assert.AreEqual("Algemeen", duneAssessmentDataItemDefinition.Category);
            TestHelper.AssertImagesAreEqual(RingtoetsFormsResources.AssessmentSectionFolderIcon, duneAssessmentDataItemDefinition.Image);
            Assert.IsNull(duneAssessmentDataItemDefinition.AdditionalOwnerCheck);
            Assert.IsInstanceOf<DuneAssessmentSection>(duneAssessmentDataItemDefinition.CreateData(null));
            Assert.IsNull(duneAssessmentDataItemDefinition.AddExampleData);
        }
    }
}