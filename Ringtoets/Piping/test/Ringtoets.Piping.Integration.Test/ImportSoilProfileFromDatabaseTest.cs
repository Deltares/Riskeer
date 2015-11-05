using System.IO;
using System.Linq;
using Core.Common.TestUtils;
using Deltares.WTIPiping;
using NUnit.Framework;
using Ringtoets.Piping.Calculation.Piping;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Plugin.FileImporter;

namespace Ringtoets.Piping.Integration.Test
{
    public class ImportSoilProfileFromDatabaseTest
    {
        [Test]
        public void GivenDatabaseWithSimple1DProfile_WhenImportingPipingProfile_ThenPipingProfileHasValuesCorrectlySet()
        {
            // Given
            string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Piping.IO, "PipingSoilProfilesReader");
            var databasePath = Path.Combine(testDataPath, "1dprofile.soil");
            var pipingFailureMechanism = new PipingFailureMechanism();

            // When
            var importer = new PipingSoilProfilesImporter();
            importer.ImportItem(databasePath, pipingFailureMechanism.SoilProfiles);

            // Then
            Assert.AreEqual(1, pipingFailureMechanism.SoilProfiles.Count());
            PipingSoilProfile profile = pipingFailureMechanism.SoilProfiles.ElementAt(0);

            var pipingProfile = PipingProfileCreator.Create(profile);

            var defaultPipingLayer = new PipingLayer();

            Assert.AreEqual(-2.1, pipingProfile.BottomLevel);
            Assert.AreEqual(3, pipingProfile.Layers.Count);
            CollectionAssert.AreEqual(new[] { 0.001, 0.001, 0.001 }, pipingProfile.Layers.Select(l => l.AbovePhreaticLevel));
            CollectionAssert.AreEqual(new[] { 0.001, 0.001, 0.001 }, pipingProfile.Layers.Select(l => l.BelowPhreaticLevel));
            CollectionAssert.AreEqual(Enumerable.Repeat(defaultPipingLayer.DryUnitWeight, 3), pipingProfile.Layers.Select(l => l.DryUnitWeight));
            CollectionAssert.AreEqual(new[] { false, false, true }, pipingProfile.Layers.Select(l => l.IsAquifer));
            CollectionAssert.AreEqual(new[] { 3.3, 2.2, 1.1 }, pipingProfile.Layers.Select(l => l.TopLevel));
        }

        [Test]
        public void GivenDatabaseWithValid2DProfileAnd2dProfileWithInvalidLayerGeometry_WhenImportingPipingProfile_ThenValidPipingProfileHasValuesCorrectlySet()
        {
            // Given
            string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Piping.IO, "PipingSoilProfilesReader");
            var databasePath = Path.Combine(testDataPath, "invalid2dGeometry.soil");
            var pipingFailureMechanism = new PipingFailureMechanism();

            // When
            var importer = new PipingSoilProfilesImporter();
            importer.ImportItem(databasePath, pipingFailureMechanism.SoilProfiles);

            // Then
            Assert.AreEqual(1, pipingFailureMechanism.SoilProfiles.Count());
            PipingSoilProfile profile = pipingFailureMechanism.SoilProfiles.ElementAt(0);

            var pipingProfile = PipingProfileCreator.Create(profile);

            Assert.AreEqual(1.25, pipingProfile.BottomLevel);
            Assert.AreEqual(3, pipingProfile.Layers.Count);
            CollectionAssert.AreEqual(new[] { 0.02, 0.002, 0.3 }, pipingProfile.Layers.Select(l => l.AbovePhreaticLevel));
            CollectionAssert.AreEqual(new[] { 0.0, 0.001, 0.009 }, pipingProfile.Layers.Select(l => l.BelowPhreaticLevel));
            CollectionAssert.AreEqual(new[] { 0.15, 0.25, 0.35}, pipingProfile.Layers.Select(l => l.DryUnitWeight));
            CollectionAssert.AreEqual(new[] { false, false, false }, pipingProfile.Layers.Select(l => l.IsAquifer));
            CollectionAssert.AreEqual(new[] { 4,3.75,2.75 }, pipingProfile.Layers.Select(l => l.TopLevel));
        }

        [Test]
        public void GivenDatabaseWithNoLayerValues_WhenImportingPipingProfile_ThenValidPipingProfileWithDefaultValuesCreated()
        {
            // Given
            string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Piping.IO, "PipingSoilProfilesReader");
            var databasePath = Path.Combine(testDataPath, "1dprofileNoValues.soil");
            var pipingFailureMechanism = new PipingFailureMechanism();

            // When
            var importer = new PipingSoilProfilesImporter();
            importer.ImportItem(databasePath, pipingFailureMechanism.SoilProfiles);

            // Then
            Assert.AreEqual(1, pipingFailureMechanism.SoilProfiles.Count());
            PipingSoilProfile profile = pipingFailureMechanism.SoilProfiles.ElementAt(0);

            var pipingProfile = PipingProfileCreator.Create(profile);
            var defaultPipingLayer = new PipingLayer();

            Assert.AreEqual(-2.1, pipingProfile.BottomLevel);
            Assert.AreEqual(3, pipingProfile.Layers.Count);
            CollectionAssert.AreEqual(Enumerable.Repeat(defaultPipingLayer.AbovePhreaticLevel, 3), pipingProfile.Layers.Select(l => l.AbovePhreaticLevel));
            CollectionAssert.AreEqual(Enumerable.Repeat(defaultPipingLayer.BelowPhreaticLevel, 3), pipingProfile.Layers.Select(l => l.BelowPhreaticLevel));
            CollectionAssert.AreEqual(Enumerable.Repeat(defaultPipingLayer.DryUnitWeight, 3), pipingProfile.Layers.Select(l => l.DryUnitWeight));
            CollectionAssert.AreEqual(Enumerable.Repeat(defaultPipingLayer.IsAquifer, 3), pipingProfile.Layers.Select(l => l.IsAquifer));
            CollectionAssert.AreEqual(new[] { 3.3, 2.2, 1.1 }, pipingProfile.Layers.Select(l => l.TopLevel));
        } 
    }
}