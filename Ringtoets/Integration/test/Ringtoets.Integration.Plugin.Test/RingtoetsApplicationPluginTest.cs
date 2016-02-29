using System.Linq;
using Core.Common.Base.Data;
using Core.Common.Base.IO;
using Core.Common.Base.Plugin;
using Core.Common.TestUtil;
using NUnit.Framework;

using Ringtoets.Common.Data;
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
            Assert.AreEqual(2, dataItemDefinitions.Length);

            DataItemInfo dikeAssessmentSectionDataItemDefinition = dataItemDefinitions.Single(did => did.ValueType == typeof(DikeAssessmentSection));
            Assert.AreEqual("Dijktraject", dikeAssessmentSectionDataItemDefinition.Name);
            Assert.AreEqual("Algemeen", dikeAssessmentSectionDataItemDefinition.Category);
            TestHelper.AssertImagesAreEqual(RingtoetsFormsResources.AssessmentSectionFolderIcon, dikeAssessmentSectionDataItemDefinition.Image);
            Assert.IsNull(dikeAssessmentSectionDataItemDefinition.AdditionalOwnerCheck);
            Assert.IsInstanceOf<DikeAssessmentSection>(dikeAssessmentSectionDataItemDefinition.CreateData(new Project()));

            DataItemInfo duneAssessmentDataItemDefinition = dataItemDefinitions.Single(did => did.ValueType == typeof(DuneAssessmentSection));
            Assert.AreEqual("Duintraject", duneAssessmentDataItemDefinition.Name);
            Assert.AreEqual("Algemeen", duneAssessmentDataItemDefinition.Category);
            TestHelper.AssertImagesAreEqual(RingtoetsFormsResources.AssessmentSectionFolderIcon, duneAssessmentDataItemDefinition.Image);
            Assert.IsNull(duneAssessmentDataItemDefinition.AdditionalOwnerCheck);
            Assert.IsInstanceOf<DuneAssessmentSection>(duneAssessmentDataItemDefinition.CreateData(new Project()));
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
        [TestCase(AssessmentSectionType.Dike)]
        [TestCase(AssessmentSectionType.Dune)]
        public void WhenAddingAssessmentSection_GivenProjectHasAssessmentSection_ThenAddedAssessmentSectionHasUniqueName(AssessmentSectionType type)
        {
            // Setup
            var project = new Project();

            var plugin = new RingtoetsApplicationPlugin();
            AddAssessmentSectionToProject(project, plugin, type);

            // Call
            AddAssessmentSectionToProject(project, plugin, type);

            // Assert
            CollectionAssert.AllItemsAreUnique(project.Items.Cast<AssessmentSectionBase>().Select(section => section.Name));
        }

        private void AddAssessmentSectionToProject(Project project, RingtoetsApplicationPlugin plugin, AssessmentSectionType type)
        {
            object itemToAdd = null;
            switch (type)
            {
                case AssessmentSectionType.Dike:
                    itemToAdd = plugin.GetDataItemInfos().First(di => di.ValueType == typeof(DikeAssessmentSection)).CreateData(project);
                    break;
                case AssessmentSectionType.Dune:
                    itemToAdd = plugin.GetDataItemInfos().First(di => di.ValueType == typeof(DuneAssessmentSection)).CreateData(project);
                    break;
            }

            project.Items.Add(itemToAdd);
        }

        public enum AssessmentSectionType
        {
            /// <summary>
            /// Type value representing <see cref="DikeAssessmentSection"/> instances.
            /// </summary>
            Dike,

            /// <summary>
            /// Type value representing <see cref="DuneAssessmentSection"/> instances.
            /// </summary>
            Dune
        }
    }
}