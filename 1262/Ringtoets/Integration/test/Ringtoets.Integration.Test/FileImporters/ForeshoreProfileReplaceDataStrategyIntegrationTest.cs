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

using System.Collections.Generic;
using System.Linq;
using Core.Common.Base;
using NUnit.Framework;
using Ringtoets.ClosingStructures.Data;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.Structures;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.HeightStructures.Data;
using Ringtoets.Integration.Plugin.FileImporters;
using Ringtoets.Integration.TestUtils;
using Ringtoets.StabilityPointStructures.Data;
using Ringtoets.StabilityStoneCover.Data;
using Ringtoets.WaveImpactAsphaltCover.Data;

namespace Ringtoets.Integration.Test.FileImporters
{
    [TestFixture]
    public class ForeshoreProfileReplaceDataStrategyIntegrationTest
    {
        private const string sourceFilePath = "path/to/foreshoreProfiles";

        [Test]
        public void UpdateForeshoreProfilesWithImportedData_WaveImpactAsphaltCoverCalculationWithForeshoreProfile_CalculationUpdatedAndReturnsAffectedData()
        {
            // Setup
            WaveImpactAsphaltCoverFailureMechanism failureMechanism =
                TestDataGenerator.GetWaveImpactAsphaltCoverFailureMechanismWithAllCalculationConfigurations();

            WaveImpactAsphaltCoverWaveConditionsCalculation[] calculationsWithForeshoreProfiles =
                failureMechanism.Calculations
                                .Cast<WaveImpactAsphaltCoverWaveConditionsCalculation>()
                                .Where(calc => calc.InputParameters.ForeshoreProfile != null)
                                .ToArray();

            WaveImpactAsphaltCoverWaveConditionsCalculation[] calculationsWithOutput =
                calculationsWithForeshoreProfiles.Where(calc => calc.HasOutput)
                                                 .ToArray();

            // Precondition
            CollectionAssert.IsNotEmpty(calculationsWithForeshoreProfiles);

            var strategy = new ForeshoreProfileReplaceDataStrategy(failureMechanism, failureMechanism.ForeshoreProfiles);

            // Call 
            IEnumerable<IObservable> affectedObjects =
                strategy.UpdateForeshoreProfilesWithImportedData(failureMechanism.ForeshoreProfiles,
                                                                 Enumerable.Empty<ForeshoreProfile>(),
                                                                 sourceFilePath);

            // Assert
            Assert.IsFalse(calculationsWithOutput.All(calc => calc.HasOutput));
            Assert.IsTrue(calculationsWithForeshoreProfiles.All(calc => calc.InputParameters
                                                                            .ForeshoreProfile == null));
            CollectionAssert.IsEmpty(failureMechanism.ForeshoreProfiles);

            IEnumerable<IObservable> expectedAffectedObjects =
                calculationsWithForeshoreProfiles.Select(calc => calc.InputParameters)
                                                 .Concat(new IObservable[]
                                                             {
                                                                 failureMechanism.ForeshoreProfiles
                                                             }
                                                             .Concat(calculationsWithOutput));
            CollectionAssert.AreEquivalent(expectedAffectedObjects, affectedObjects);
        }

        [Test]
        public void UpdateForeshoreProfilesWithImportedData_GrassCoverErosionOutwardsCalculationWithForeshoreProfile_CalculationUpdatedAndReturnsAffectedData()
        {
            // Setup
            GrassCoverErosionOutwardsFailureMechanism failureMechanism =
                TestDataGenerator.GetGrassCoverErosionOutwardsFailureMechanismWithAllCalculationConfigurations();

            GrassCoverErosionOutwardsWaveConditionsCalculation[] calculationsWithForeshoreProfiles =
                failureMechanism.Calculations
                                .Cast<GrassCoverErosionOutwardsWaveConditionsCalculation>()
                                .Where(calc => calc.InputParameters.ForeshoreProfile != null)
                                .ToArray();

            GrassCoverErosionOutwardsWaveConditionsCalculation[] calculationsWithOutput =
                calculationsWithForeshoreProfiles.Where(calc => calc.HasOutput)
                                                 .ToArray();

            // Precondition
            CollectionAssert.IsNotEmpty(calculationsWithForeshoreProfiles);

            var strategy = new ForeshoreProfileReplaceDataStrategy(failureMechanism, failureMechanism.ForeshoreProfiles);

            // Call 
            IEnumerable<IObservable> affectedObjects =
                strategy.UpdateForeshoreProfilesWithImportedData(failureMechanism.ForeshoreProfiles,
                                                                 Enumerable.Empty<ForeshoreProfile>(),
                                                                 sourceFilePath);

            // Assert
            Assert.IsFalse(calculationsWithOutput.All(calc => calc.HasOutput));
            Assert.IsTrue(calculationsWithForeshoreProfiles.All(calc => calc.InputParameters
                                                                            .ForeshoreProfile == null));
            CollectionAssert.IsEmpty(failureMechanism.ForeshoreProfiles);

            IEnumerable<IObservable> expectedAffectedObjects =
                calculationsWithForeshoreProfiles.Select(calc => calc.InputParameters)
                                                 .Concat(new IObservable[]
                                                             {
                                                                 failureMechanism.ForeshoreProfiles
                                                             }
                                                             .Concat(calculationsWithOutput));
            CollectionAssert.AreEquivalent(expectedAffectedObjects, affectedObjects);
        }

        [Test]
        public void UpdateForeshoreProfilesWithImportedData_StabilityStoneCoverCalculationWithForeshoreProfile_CalculationUpdatedAndReturnsAffectedData()
        {
            // Setup
            StabilityStoneCoverFailureMechanism failureMechanism =
                TestDataGenerator.GetStabilityStoneCoverFailureMechanismWithAllCalculationConfigurations();

            StabilityStoneCoverWaveConditionsCalculation[] calculationsWithForeshoreProfiles =
                failureMechanism.Calculations
                                .Cast<StabilityStoneCoverWaveConditionsCalculation>()
                                .Where(calc => calc.InputParameters.ForeshoreProfile != null)
                                .ToArray();

            StabilityStoneCoverWaveConditionsCalculation[] calculationsWithOutput =
                calculationsWithForeshoreProfiles.Where(calc => calc.HasOutput)
                                                 .ToArray();

            // Precondition
            CollectionAssert.IsNotEmpty(calculationsWithForeshoreProfiles);

            var strategy = new ForeshoreProfileReplaceDataStrategy(failureMechanism, failureMechanism.ForeshoreProfiles);

            // Call 
            IEnumerable<IObservable> affectedObjects =
                strategy.UpdateForeshoreProfilesWithImportedData(failureMechanism.ForeshoreProfiles,
                                                                 Enumerable.Empty<ForeshoreProfile>(),
                                                                 sourceFilePath);

            // Assert
            Assert.IsFalse(calculationsWithOutput.All(calc => calc.HasOutput));
            Assert.IsTrue(calculationsWithForeshoreProfiles.All(calc => calc.InputParameters
                                                                            .ForeshoreProfile == null));
            CollectionAssert.IsEmpty(failureMechanism.ForeshoreProfiles);

            IEnumerable<IObservable> expectedAffectedObjects =
                calculationsWithForeshoreProfiles.Select(calc => calc.InputParameters)
                                                 .Concat(new IObservable[]
                                                             {
                                                                 failureMechanism.ForeshoreProfiles
                                                             }
                                                             .Concat(calculationsWithOutput));
            CollectionAssert.AreEquivalent(expectedAffectedObjects, affectedObjects);
        }

        [Test]
        public void UpdateForeshoreProfilesWithImportedData_HeightStructuresCalculationWithForeshoreProfile_CalculationUpdatedAndReturnsAffectedData()
        {
            // Setup
            HeightStructuresFailureMechanism failureMechanism =
                TestDataGenerator.GetHeightStructuresFailureMechanismWithAlLCalculationConfigurations();

            StructuresCalculation<HeightStructuresInput>[] calculationsWithForeshoreProfiles =
                failureMechanism.Calculations
                                .Cast<StructuresCalculation<HeightStructuresInput>>()
                                .Where(calc => calc.InputParameters.ForeshoreProfile != null)
                                .ToArray();

            StructuresCalculation<HeightStructuresInput>[] calculationsWithOutput =
                calculationsWithForeshoreProfiles.Where(calc => calc.HasOutput)
                                                 .ToArray();

            // Precondition
            CollectionAssert.IsNotEmpty(calculationsWithForeshoreProfiles);

            var strategy = new ForeshoreProfileReplaceDataStrategy(failureMechanism, failureMechanism.ForeshoreProfiles);

            // Call 
            IEnumerable<IObservable> affectedObjects =
                strategy.UpdateForeshoreProfilesWithImportedData(failureMechanism.ForeshoreProfiles,
                                                                 Enumerable.Empty<ForeshoreProfile>(),
                                                                 sourceFilePath);

            // Assert
            Assert.IsFalse(calculationsWithOutput.All(calc => calc.HasOutput));
            Assert.IsTrue(calculationsWithForeshoreProfiles.All(calc => calc.InputParameters
                                                                            .ForeshoreProfile == null));
            CollectionAssert.IsEmpty(failureMechanism.ForeshoreProfiles);

            IEnumerable<IObservable> expectedAffectedObjects =
                calculationsWithForeshoreProfiles.Select(calc => calc.InputParameters)
                                                 .Concat(new IObservable[]
                                                             {
                                                                 failureMechanism.ForeshoreProfiles
                                                             }
                                                             .Concat(calculationsWithOutput));
            CollectionAssert.AreEquivalent(expectedAffectedObjects, affectedObjects);
        }

        [Test]
        public void UpdateForeshoreProfilesWithImportedData_StabilityPointStructuresCalculationWithForeshoreProfile_CalculationUpdatedAndReturnsAffectedData()
        {
            // Setup
            StabilityPointStructuresFailureMechanism failureMechanism =
                TestDataGenerator.GetStabilityPointStructuresFailureMechanismWithAllCalculationConfigurations();

            StructuresCalculation<StabilityPointStructuresInput>[] calculationsWithForeshoreProfiles =
                failureMechanism.Calculations
                                .Cast<StructuresCalculation<StabilityPointStructuresInput>>()
                                .Where(calc => calc.InputParameters.ForeshoreProfile != null)
                                .ToArray();

            StructuresCalculation<StabilityPointStructuresInput>[] calculationsWithOutput =
                calculationsWithForeshoreProfiles.Where(calc => calc.HasOutput)
                                                 .ToArray();

            // Precondition
            CollectionAssert.IsNotEmpty(calculationsWithForeshoreProfiles);

            var strategy = new ForeshoreProfileReplaceDataStrategy(failureMechanism, failureMechanism.ForeshoreProfiles);

            // Call 
            IEnumerable<IObservable> affectedObjects =
                strategy.UpdateForeshoreProfilesWithImportedData(failureMechanism.ForeshoreProfiles,
                                                                 Enumerable.Empty<ForeshoreProfile>(),
                                                                 sourceFilePath);

            // Assert
            Assert.IsFalse(calculationsWithOutput.All(calc => calc.HasOutput));
            Assert.IsTrue(calculationsWithForeshoreProfiles.All(calc => calc.InputParameters
                                                                            .ForeshoreProfile == null));
            CollectionAssert.IsEmpty(failureMechanism.ForeshoreProfiles);

            IEnumerable<IObservable> expectedAffectedObjects =
                calculationsWithForeshoreProfiles.Select(calc => calc.InputParameters)
                                                 .Concat(new IObservable[]
                                                             {
                                                                 failureMechanism.ForeshoreProfiles
                                                             }
                                                             .Concat(calculationsWithOutput));
            CollectionAssert.AreEquivalent(expectedAffectedObjects, affectedObjects);
        }

        [Test]
        public void UpdateForeshoreProfilesWithImportedData_ClosingStructuresCalculationWithForeshoreProfile_CalculationUpdatedAndReturnsAffectedData()
        {
            // Setup
            ClosingStructuresFailureMechanism failureMechanism =
                TestDataGenerator.GetClosingStructuresFailureMechanismWithAllCalculationConfigurations();

            StructuresCalculation<ClosingStructuresInput>[] calculationsWithForeshoreProfiles =
                failureMechanism.Calculations
                                .Cast<StructuresCalculation<ClosingStructuresInput>>()
                                .Where(calc => calc.InputParameters.ForeshoreProfile != null)
                                .ToArray();

            StructuresCalculation<ClosingStructuresInput>[] calculationsWithOutput =
                calculationsWithForeshoreProfiles.Where(calc => calc.HasOutput)
                                                 .ToArray();

            // Precondition
            CollectionAssert.IsNotEmpty(calculationsWithForeshoreProfiles);

            var strategy = new ForeshoreProfileReplaceDataStrategy(failureMechanism, failureMechanism.ForeshoreProfiles);

            // Call 
            IEnumerable<IObservable> affectedObjects =
                strategy.UpdateForeshoreProfilesWithImportedData(failureMechanism.ForeshoreProfiles,
                                                                 Enumerable.Empty<ForeshoreProfile>(),
                                                                 sourceFilePath);

            // Assert
            Assert.IsTrue(calculationsWithOutput.All(calc => calc.HasOutput == false));
            Assert.IsTrue(calculationsWithForeshoreProfiles.All(calc => calc.InputParameters
                                                                            .ForeshoreProfile == null));
            CollectionAssert.IsEmpty(failureMechanism.ForeshoreProfiles);

            IEnumerable<IObservable> expectedAffectedObjects =
                calculationsWithForeshoreProfiles.Select(calc => calc.InputParameters)
                                                 .Concat(new IObservable[]
                                                 {
                                                     failureMechanism.ForeshoreProfiles
                                                 }).Concat(calculationsWithOutput);
            CollectionAssert.AreEquivalent(expectedAffectedObjects, affectedObjects);
        }
    }
}