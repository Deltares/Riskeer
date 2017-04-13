// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

using System.Collections.Generic;
using System.Linq;
using Application.Ringtoets.Migration.Core;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.ClosingStructures.Data;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.Structures;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.HeightStructures.Data;
using Ringtoets.Integration.Data;
using Ringtoets.Piping.Data;
using Ringtoets.StabilityPointStructures.Data;
using Ringtoets.StabilityStoneCover.Data;
using Ringtoets.WaveImpactAsphaltCover.Data;

namespace Application.Ringtoets.Storage.Test.IntegrationTests
{
    [TestFixture]
    public class MigrationTo171IntegrationTest
    {
        private const string newVersion = "17.1";
        private readonly TestDataPath testPath = TestDataPath.Application.Ringtoets.Migration.Core;

        [Test]
        public void Given164Project_WhenUpgradedTo171_ThenProjectAsExpected()
        {
            // Given
            string sourceFilePath = TestHelper.GetTestDataPath(testPath, "FullTestProject164.rtd");
            var fromVersionedFile = new RingtoetsVersionedFile(sourceFilePath);

            string targetFilePath = TestHelper.GetScratchPadPath(nameof(Given164Project_WhenUpgradedTo171_ThenProjectAsExpected));
            var migrator = new RingtoetsSqLiteDatabaseFileMigrator();

            using (new FileDisposeHelper(targetFilePath))
            {
                // When
                migrator.Migrate(fromVersionedFile, newVersion, targetFilePath);

                // Then
                var storageSqLite = new StorageSqLite();
                var project = (RingtoetsProject) storageSqLite.LoadProject(targetFilePath);

                AssertClosingStructuresFailureMechanism(project.AssessmentSections);
                AssertGrassCoverErosionInwardsFailureMechanism(project.AssessmentSections);
                AssertGrassCoverErosionOutwardsFailureMechanism(project.AssessmentSections);
                AssertHeightStructuresFailureMechanism(project.AssessmentSections);
                AssertHydraulicBoundaryDatabase(project.AssessmentSections);
                AssertPipingFailureMechanism(project.AssessmentSections);
                AssertStabilityPointStructuresFailureMechanism(project.AssessmentSections);
                AssertStabilityStoneCoverFailureMechanism(project.AssessmentSections);
                AssertWaveImpactAsphaltCoverFailureMechanism(project.AssessmentSections);
            }
        }

        private static void AssertClosingStructuresFailureMechanism(IList<AssessmentSection> assessmentSections)
        {
            Assert.IsFalse(assessmentSections.All(a => a.ClosingStructures.Calculations.OfType<StructuresCalculation<ClosingStructuresInput>>().Any(pcs => pcs.HasOutput)));
        }

        private static void AssertGrassCoverErosionInwardsFailureMechanism(IList<AssessmentSection> assessmentSections)
        {
            Assert.IsFalse(assessmentSections.All(a => a.GrassCoverErosionInwards.Calculations.OfType<GrassCoverErosionInwardsCalculation>().Any(pcs => pcs.HasOutput)));
        }

        private static void AssertGrassCoverErosionOutwardsFailureMechanism(IList<AssessmentSection> assessmentSections)
        {
            Assert.IsFalse(assessmentSections.All(a => a.GrassCoverErosionOutwards.Calculations.OfType<GrassCoverErosionOutwardsWaveConditionsCalculation>().Any(pcs => pcs.HasOutput)));

            foreach (AssessmentSection assessmentSection in assessmentSections)
            {
                AssertHydraulicBoundaryLocations(assessmentSection.GrassCoverErosionOutwards.HydraulicBoundaryLocations);
            }
        }

        private static void AssertHeightStructuresFailureMechanism(IList<AssessmentSection> assessmentSections)
        {
            Assert.IsFalse(assessmentSections.All(a => a.ClosingStructures.Calculations.OfType<StructuresCalculation<HeightStructuresInput>>().Any(pcs => pcs.HasOutput)));
        }

        private static void AssertHydraulicBoundaryDatabase(IList<AssessmentSection> assessmentSections)
        {
            foreach (AssessmentSection assessmentSection in assessmentSections)
            {
                AssertHydraulicBoundaryLocations(assessmentSection.HydraulicBoundaryDatabase.Locations);
            }
        }

        private static void AssertHydraulicBoundaryLocations(IList<HydraulicBoundaryLocation> hydraulicBoundaryLocations)
        {
            Assert.IsFalse(hydraulicBoundaryLocations.Any(l => l.DesignWaterLevelOutput != null || l.WaveHeightOutput != null));
        }

        private static void AssertPipingFailureMechanism(IList<AssessmentSection> assessmentSections)
        {
            Assert.IsFalse(assessmentSections.All(a => a.PipingFailureMechanism.Calculations.OfType<PipingCalculationScenario>().Any(pcs => pcs.HasOutput)));
        }

        private static void AssertStabilityPointStructuresFailureMechanism(IList<AssessmentSection> assessmentSections)
        {
            Assert.IsFalse(assessmentSections.All(a => a.StabilityPointStructures.Calculations.OfType<StructuresCalculation<StabilityPointStructuresInput>>().Any(pcs => pcs.HasOutput)));
        }

        private static void AssertStabilityStoneCoverFailureMechanism(IList<AssessmentSection> assessmentSections)
        {
            Assert.IsFalse(assessmentSections.All(a => a.StabilityStoneCover.Calculations.OfType<StabilityStoneCoverWaveConditionsCalculation>().Any(pcs => pcs.HasOutput)));
        }

        private static void AssertWaveImpactAsphaltCoverFailureMechanism(IList<AssessmentSection> assessmentSections)
        {
            Assert.IsFalse(assessmentSections.All(a => a.WaveImpactAsphaltCover.Calculations.OfType<WaveImpactAsphaltCoverWaveConditionsCalculation>().Any(pcs => pcs.HasOutput)));
        }
    }
}