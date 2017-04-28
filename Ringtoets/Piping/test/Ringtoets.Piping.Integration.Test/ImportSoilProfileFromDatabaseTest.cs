// Copyright (C) Stichting Deltares 2017. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System.IO;
using System.Linq;
using Core.Common.TestUtil;
using Deltares.WTIPiping;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.IO.FileImporters.MessageProviders;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.IO.Importers;
using Ringtoets.Piping.KernelWrapper;
using Ringtoets.Piping.Plugin.FileImporter;
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.Integration.Test
{
    [TestFixture]
    public class ImportSoilProfileFromDatabaseTest
    {
        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Piping.IO, "PipingSoilProfilesReader");

        [Test]
        public void GivenDatabaseWithSimple1DProfile_WhenImportingPipingProfile_ThenPipingProfileHasValuesCorrectlySet()
        {
            // Given
            var mocks = new MockRepository();
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            mocks.ReplayAll();

            string databasePath = Path.Combine(testDataPath, "1dprofile.soil");
            var pipingFailureMechanism = new PipingFailureMechanism();

            // When
            var importer = new StochasticSoilModelImporter(
                pipingFailureMechanism.StochasticSoilModels,
                databasePath,
                messageProvider,
                new StochasticSoilModelReplaceDataStrategy(pipingFailureMechanism));
            importer.Import();

            // Then
            Assert.AreEqual(1, pipingFailureMechanism.StochasticSoilModels.Count);
            PipingSoilProfile profile = pipingFailureMechanism.StochasticSoilModels[0].StochasticSoilProfiles[0].SoilProfile;

            PipingProfile pipingProfile = PipingProfileCreator.Create(profile);

            Assert.AreEqual(-2.1, pipingProfile.BottomLevel);
            Assert.AreEqual(3, pipingProfile.Layers.Count);
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

            mocks.VerifyAll();
        }

        /// <summary>
        /// This database contains 2 profiles. One of the profiles has an invalid layer geometry and should be skipped by the importer.
        /// The other profile has valid geometries for its layers and should have the values correctly set.
        /// </summary>
        [Test]
        public void GivenDatabaseWithValid2DProfileAnd2dProfileWithInvalidLayerGeometry_WhenImportingPipingProfile_ThenValidPipingProfileHasValuesCorrectlySet()
        {
            // Given
            var mocks = new MockRepository();
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            mocks.ReplayAll();

            string databasePath = Path.Combine(testDataPath, "invalid2dGeometry.soil");
            var pipingFailureMechanism = new PipingFailureMechanism();

            // When
            var importer = new StochasticSoilModelImporter(
                pipingFailureMechanism.StochasticSoilModels,
                databasePath,
                messageProvider,
                new StochasticSoilModelReplaceDataStrategy(pipingFailureMechanism));
            importer.Import();

            // Then
            Assert.AreEqual(1, pipingFailureMechanism.StochasticSoilModels.Count);
            PipingSoilProfile profile = pipingFailureMechanism.StochasticSoilModels[0].StochasticSoilProfiles[0].SoilProfile;

            PipingProfile pipingProfile = PipingProfileCreator.Create(profile);

            Assert.AreEqual(1.25, pipingProfile.BottomLevel);
            Assert.AreEqual(3, pipingProfile.Layers.Count);
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

            mocks.VerifyAll();
        }

        [Test]
        public void GivenDatabaseWithNoLayerValues_WhenImportingPipingProfile_ThenValidPipingProfileWithDefaultValuesCreated()
        {
            // Given
            var mocks = new MockRepository();
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            mocks.ReplayAll();

            string databasePath = Path.Combine(testDataPath, "1dprofileNoValues.soil");
            var pipingFailureMechanism = new PipingFailureMechanism();

            // When
            var importer = new StochasticSoilModelImporter(
                pipingFailureMechanism.StochasticSoilModels,
                databasePath,
                messageProvider,
                new StochasticSoilModelReplaceDataStrategy(pipingFailureMechanism));
            importer.Import();

            // Then
            Assert.AreEqual(1, pipingFailureMechanism.StochasticSoilModels.Count);
            PipingSoilProfile profile = pipingFailureMechanism.StochasticSoilModels[0].StochasticSoilProfiles[0].SoilProfile;

            PipingProfile pipingProfile = PipingProfileCreator.Create(profile);

            Assert.AreEqual(-2.1, pipingProfile.BottomLevel);
            Assert.AreEqual(3, pipingProfile.Layers.Count);
            CollectionAssert.AreEqual(Enumerable.Repeat(false, 3), pipingProfile.Layers.Select(l => l.IsAquifer));
            CollectionAssert.AreEqual(new[]
            {
                3.3,
                2.2,
                1.1
            }, pipingProfile.Layers.Select(l => l.TopLevel));

            mocks.VerifyAll();
        }
    }
}