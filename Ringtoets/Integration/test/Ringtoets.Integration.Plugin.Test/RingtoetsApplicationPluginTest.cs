using System.Linq;
using Core.Common.Base.Data;
using Core.Common.Base.IO;
using Core.Common.Base.Plugin;
using Core.Common.TestUtil;
using NUnit.Framework;

using Ringtoets.Common.Data;
using Ringtoets.Common.IO;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.Plugin.FileImporters;

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
            Assert.AreEqual(1, dataItemDefinitions.Length);

            DataItemInfo dikeAssessmentSectionDataItemDefinition = dataItemDefinitions.Single(did => did.ValueType == typeof(DikeAssessmentSection));
            Assert.AreEqual("Dijktraject", dikeAssessmentSectionDataItemDefinition.Name);
            Assert.AreEqual("Algemeen", dikeAssessmentSectionDataItemDefinition.Category);
            TestHelper.AssertImagesAreEqual(RingtoetsFormsResources.AssessmentSectionFolderIcon, dikeAssessmentSectionDataItemDefinition.Image);
            Assert.IsNull(dikeAssessmentSectionDataItemDefinition.AdditionalOwnerCheck);
            Assert.IsInstanceOf<DikeAssessmentSection>(dikeAssessmentSectionDataItemDefinition.CreateData(new Project()));
        }

        [Test]
        public void GetFileImporters_ReturnsExpectedFileImporters()
        {
            // Setup
            var plugin = new RingtoetsApplicationPlugin();

            // Call
            IFileImporter[] importers = plugin.GetFileImporters().ToArray();

            // Assert
            Assert.AreEqual(2, importers.Length);
            Assert.AreEqual(1, importers.Count(i => i is ReferenceLineImporter));
            Assert.AreEqual(1, importers.Count(i => i is FailureMechanismSectionsImporter));
        }

        [Test]
        public void WhenAddingAssessmentSection_GivenProjectHasAssessmentSection_ThenAddedAssessmentSectionHasUniqueName()
        {
            // Setup
            var project = new Project();

            var plugin = new RingtoetsApplicationPlugin();
            AddAssessmentSectionToProject(project, plugin);

            // Call
            AddAssessmentSectionToProject(project, plugin);

            // Assert
            CollectionAssert.AllItemsAreUnique(project.Items.Cast<AssessmentSectionBase>().Select(section => section.Name));
        }

        private void AddAssessmentSectionToProject(Project project, RingtoetsApplicationPlugin plugin)
        {
            var itemToAdd = plugin.GetDataItemInfos()
                                  .First(di => di.ValueType == typeof(DikeAssessmentSection))
                                  .CreateData(project);

            project.Items.Add(itemToAdd);
        }
    }
}