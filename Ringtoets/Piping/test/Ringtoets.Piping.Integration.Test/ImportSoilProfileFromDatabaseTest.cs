using System.IO;
using System.Linq;
using Core.Common.Base;
using Core.Common.TestUtil;
using Deltares.WTIPiping;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.PresentationObjects;
using Ringtoets.Piping.KernelWrapper;
using Ringtoets.Piping.Plugin.FileImporter;
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.Integration.Test
{
    public class ImportSoilProfileFromDatabaseTest
    {
        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Piping.IO, "PipingSoilProfilesReader");

        [Test]
        public void GivenDatabaseWithSimple1DProfile_WhenImportingPipingProfile_ThenPipingProfileHasValuesCorrectlySet()
        {
            // Given
            var databasePath = Path.Combine(testDataPath, "1dprofile.soil");
            var pipingFailureMechanism = new PipingFailureMechanism();

            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.ReferenceLine = new ReferenceLine();
            mocks.ReplayAll();

            var context = new StochasticSoilModelContext(pipingFailureMechanism, assessmentSection);
            context.Attach(observer);

            // When
            var importer = new PipingSoilProfilesImporter();
            importer.Import(context, databasePath);

            // Then
            Assert.AreEqual(1, pipingFailureMechanism.StochasticSoilModels.Count);
            PipingSoilProfile profile = pipingFailureMechanism.StochasticSoilModels[0].StochasticSoilProfiles[0].SoilProfile;

            PipingProfile pipingProfile = PipingProfileCreator.Create(profile);

            var defaultPipingLayer = new PipingLayer();

            Assert.AreEqual(-2.1, pipingProfile.BottomLevel);
            Assert.AreEqual(3, pipingProfile.Layers.Count);
            CollectionAssert.AreEqual(new[]
            {
                0.001,
                0.001,
                0.001
            }, pipingProfile.Layers.Select(l => l.AbovePhreaticLevel));
            CollectionAssert.AreEqual(new[]
            {
                0.001,
                0.001,
                0.001
            }, pipingProfile.Layers.Select(l => l.BelowPhreaticLevel));
            CollectionAssert.AreEqual(Enumerable.Repeat(defaultPipingLayer.DryUnitWeight, 3), pipingProfile.Layers.Select(l => l.DryUnitWeight));
            CollectionAssert.AreEqual(new[]
            {
                false,
                false,
                true
            }, pipingProfile.Layers.Select(l => l.IsAquifer));
            CollectionAssert.AreEqual(new[]
            {
                3.3,
                2.2,
                1.1
            }, pipingProfile.Layers.Select(l => l.TopLevel));

            mocks.VerifyAll(); // Ensure there are no calls to UpdateObserver
        }

        /// <summary>
        /// This database contains 2 profiles. One of the profiles has an invalid layer geometry and should be skipped by the importer.
        /// The other profile has valid geometries for its layers and should have the values correctly set.
        /// </summary>
        [Test]
        public void GivenDatabaseWithValid2DProfileAnd2dProfileWithInvalidLayerGeometry_WhenImportingPipingProfile_ThenValidPipingProfileHasValuesCorrectlySet()
        {
            // Given
            var databasePath = Path.Combine(testDataPath, "invalid2dGeometry.soil");
            var pipingFailureMechanism = new PipingFailureMechanism();

            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.ReferenceLine = new ReferenceLine();
            mocks.ReplayAll();

            var context = new StochasticSoilModelContext(pipingFailureMechanism, assessmentSection);
            context.Attach(observer);

            // When
            var importer = new PipingSoilProfilesImporter();
            importer.Import(context, databasePath);

            // Then
            Assert.AreEqual(1, pipingFailureMechanism.StochasticSoilModels.Count);
            PipingSoilProfile profile = pipingFailureMechanism.StochasticSoilModels[0].StochasticSoilProfiles[0].SoilProfile;

            PipingProfile pipingProfile = PipingProfileCreator.Create(profile);

            Assert.AreEqual(1.25, pipingProfile.BottomLevel);
            Assert.AreEqual(3, pipingProfile.Layers.Count);
            CollectionAssert.AreEqual(new[]
            {
                0.02,
                0.002,
                0.3
            }, pipingProfile.Layers.Select(l => l.AbovePhreaticLevel));
            CollectionAssert.AreEqual(new[]
            {
                0.0,
                0.001,
                0.009
            }, pipingProfile.Layers.Select(l => l.BelowPhreaticLevel));
            CollectionAssert.AreEqual(new[]
            {
                0.15,
                0.25,
                0.35
            }, pipingProfile.Layers.Select(l => l.DryUnitWeight));
            CollectionAssert.AreEqual(new[]
            {
                false,
                false,
                false
            }, pipingProfile.Layers.Select(l => l.IsAquifer));
            CollectionAssert.AreEqual(new[]
            {
                4,
                3.75,
                2.75
            }, pipingProfile.Layers.Select(l => l.TopLevel));

            mocks.VerifyAll(); // Ensure there are no calls to UpdateObserver
        }

        [Test]
        public void GivenDatabaseWithNoLayerValues_WhenImportingPipingProfile_ThenValidPipingProfileWithDefaultValuesCreated()
        {
            // Given
            var databasePath = Path.Combine(testDataPath, "1dprofileNoValues.soil");
            var pipingFailureMechanism = new PipingFailureMechanism();

            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.ReferenceLine = new ReferenceLine();
            mocks.ReplayAll();

            var context = new StochasticSoilModelContext(pipingFailureMechanism, assessmentSection);
            context.Attach(observer);

            // When
            var importer = new PipingSoilProfilesImporter();
            importer.Import(context, databasePath);

            // Then
            Assert.AreEqual(1, pipingFailureMechanism.StochasticSoilModels.Count);
            PipingSoilProfile profile = pipingFailureMechanism.StochasticSoilModels[0].StochasticSoilProfiles[0].SoilProfile;

            PipingProfile pipingProfile = PipingProfileCreator.Create(profile);
            var defaultPipingLayer = new PipingLayer();

            Assert.AreEqual(-2.1, pipingProfile.BottomLevel);
            Assert.AreEqual(3, pipingProfile.Layers.Count);
            CollectionAssert.AreEqual(Enumerable.Repeat(defaultPipingLayer.AbovePhreaticLevel, 3), pipingProfile.Layers.Select(l => l.AbovePhreaticLevel));
            CollectionAssert.AreEqual(Enumerable.Repeat(defaultPipingLayer.BelowPhreaticLevel, 3), pipingProfile.Layers.Select(l => l.BelowPhreaticLevel));
            CollectionAssert.AreEqual(Enumerable.Repeat(defaultPipingLayer.DryUnitWeight, 3), pipingProfile.Layers.Select(l => l.DryUnitWeight));
            CollectionAssert.AreEqual(Enumerable.Repeat(defaultPipingLayer.IsAquifer, 3), pipingProfile.Layers.Select(l => l.IsAquifer));
            CollectionAssert.AreEqual(new[]
            {
                3.3,
                2.2,
                1.1
            }, pipingProfile.Layers.Select(l => l.TopLevel));
            mocks.VerifyAll(); // Ensure there are no calls to UpdateObserver
        }
    }
}