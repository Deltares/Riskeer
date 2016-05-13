using System.Linq;
using Core.Common.Base.Data;
using Core.Common.Base.IO;
using Core.Common.Base.Plugin;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.AssessmentSection;
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

            DataItemInfo assessmentSectionDataItemDefinition = dataItemDefinitions.Single(did => did.ValueType == typeof(AssessmentSection));
            Assert.AreEqual("Traject", assessmentSectionDataItemDefinition.Name);
            Assert.AreEqual("Algemeen", assessmentSectionDataItemDefinition.Category);
            TestHelper.AssertImagesAreEqual(RingtoetsFormsResources.AssessmentSectionFolderIcon, assessmentSectionDataItemDefinition.Image);
            Assert.IsNull(assessmentSectionDataItemDefinition.AdditionalOwnerCheck);
            Assert.IsInstanceOf<AssessmentSection>(assessmentSectionDataItemDefinition.CreateData(new Project()));
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
            CollectionAssert.AllItemsAreUnique(project.Items.Cast<IAssessmentSection>().Select(section => section.Name));
        }

        private void AddAssessmentSectionToProject(Project project, RingtoetsApplicationPlugin plugin)
        {
            var itemToAdd = plugin.GetDataItemInfos()
                                  .First(di => di.ValueType == typeof(AssessmentSection))
                                  .CreateData(project);

            project.Items.Add(itemToAdd);
        }
    }
}