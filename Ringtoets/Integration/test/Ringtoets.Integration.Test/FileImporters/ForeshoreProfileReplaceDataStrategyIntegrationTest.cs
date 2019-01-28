// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Core.Common.Base;
using NUnit.Framework;
using Ringtoets.ClosingStructures.Data;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.HeightStructures.Data;
using Ringtoets.Integration.TestUtil;
using Ringtoets.StabilityPointStructures.Data;
using Ringtoets.StabilityStoneCover.Data;
using Ringtoets.WaveImpactAsphaltCover.Data;
using Riskeer.Integration.Plugin.FileImporters;

namespace Ringtoets.Integration.Test.FileImporters
{
    [TestFixture]
    public class ForeshoreProfileReplaceDataStrategyIntegrationTest
    {
        private const string sourceFilePath = "path/to/foreshoreProfiles";

        [Test]
        [TestCaseSource(nameof(GetSupportedFailureMechanisms))]
        public void UpdateForeshoreProfilesWithImportedData_SupportedFailureMechanism_CalculationUpdatedAndReturnsAffectedData(
            IFailureMechanism failureMechanism,
            ForeshoreProfileCollection foreshoreProfiles)
        {
            // Setup
            ICalculation<ICalculationInput>[] calculationsWithForeshoreProfiles =
                failureMechanism.Calculations
                                .Cast<ICalculation<ICalculationInput>>()
                                .Where(calc => ((IHasForeshoreProfile) calc.InputParameters).ForeshoreProfile != null)
                                .ToArray();

            ICalculation<ICalculationInput>[] calculationsWithOutput =
                calculationsWithForeshoreProfiles.Where(calc => calc.HasOutput)
                                                 .ToArray();

            // Precondition
            CollectionAssert.IsNotEmpty(calculationsWithForeshoreProfiles);
            CollectionAssert.IsNotEmpty(calculationsWithOutput);

            var strategy = new ForeshoreProfileReplaceDataStrategy(failureMechanism, foreshoreProfiles);

            // Call 
            IEnumerable<IObservable> affectedObjects =
                strategy.UpdateForeshoreProfilesWithImportedData(Enumerable.Empty<ForeshoreProfile>(),
                                                                 sourceFilePath);

            // Assert
            Assert.IsFalse(calculationsWithOutput.All(calc => calc.HasOutput));
            Assert.IsTrue(calculationsWithForeshoreProfiles.All(calc => ((IHasForeshoreProfile) calc.InputParameters)
                                                                        .ForeshoreProfile == null));
            CollectionAssert.IsEmpty(foreshoreProfiles);

            IEnumerable<IObservable> expectedAffectedObjects =
                calculationsWithForeshoreProfiles.Select(calc => calc.InputParameters)
                                                 .Concat(new IObservable[]
                                                             {
                                                                 foreshoreProfiles
                                                             }
                                                             .Concat(calculationsWithOutput));
            CollectionAssert.AreEquivalent(expectedAffectedObjects, affectedObjects);
        }

        private static IEnumerable<TestCaseData> GetSupportedFailureMechanisms()
        {
            WaveImpactAsphaltCoverFailureMechanism waveImpactAsphaltCoverFailureMechanism =
                TestDataGenerator.GetWaveImpactAsphaltCoverFailureMechanismWithAllCalculationConfigurations();
            yield return new TestCaseData(
                    waveImpactAsphaltCoverFailureMechanism, waveImpactAsphaltCoverFailureMechanism.ForeshoreProfiles)
                .SetName("WaveImpactAsphaltCoverFailureMechanism");

            GrassCoverErosionOutwardsFailureMechanism grassCoverErosionOutwardsFailureMechanism =
                TestDataGenerator.GetGrassCoverErosionOutwardsFailureMechanismWithAllCalculationConfigurations();
            yield return new TestCaseData(
                    grassCoverErosionOutwardsFailureMechanism, grassCoverErosionOutwardsFailureMechanism.ForeshoreProfiles)
                .SetName("GrassCoverErosionOutwardsFailureMechanism");

            StabilityStoneCoverFailureMechanism stabilityStoneCoverFailureMechanism =
                TestDataGenerator.GetStabilityStoneCoverFailureMechanismWithAllCalculationConfigurations();
            yield return new TestCaseData(
                    stabilityStoneCoverFailureMechanism, stabilityStoneCoverFailureMechanism.ForeshoreProfiles)
                .SetName("StabilityStoneCoverFailureMechanism");

            StabilityPointStructuresFailureMechanism stabilityPointStructuresFailureMechanism =
                TestDataGenerator.GetStabilityPointStructuresFailureMechanismWithAllCalculationConfigurations();
            yield return new TestCaseData(
                    stabilityPointStructuresFailureMechanism, stabilityPointStructuresFailureMechanism.ForeshoreProfiles)
                .SetName("StabilityPointStructuresFailureMechanism");

            ClosingStructuresFailureMechanism closingStructuresFailureMechanism =
                TestDataGenerator.GetClosingStructuresFailureMechanismWithAllCalculationConfigurations();
            yield return new TestCaseData(
                    closingStructuresFailureMechanism, closingStructuresFailureMechanism.ForeshoreProfiles)
                .SetName("ClosingStructuresFailureMechanism");

            HeightStructuresFailureMechanism heightStructuresFailureMechanism =
                TestDataGenerator.GetHeightStructuresFailureMechanismWithAlLCalculationConfigurations();
            yield return new TestCaseData(
                    heightStructuresFailureMechanism, heightStructuresFailureMechanism.ForeshoreProfiles)
                .SetName("HeightStructuresFailureMechanism");
        }
    }
}