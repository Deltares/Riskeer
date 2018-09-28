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

using System;
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.ClosingStructures.Data;
using Ringtoets.ClosingStructures.Data.TestUtil;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.HeightStructures.Data;
using Ringtoets.HeightStructures.Data.TestUtil;
using Ringtoets.Integration.Plugin.FileImporters;
using Ringtoets.Integration.TestUtil;
using Ringtoets.Revetment.Data.TestUtil;
using Ringtoets.StabilityPointStructures.Data;
using Ringtoets.StabilityPointStructures.Data.TestUtil;
using Ringtoets.StabilityStoneCover.Data;
using Ringtoets.WaveImpactAsphaltCover.Data;

namespace Ringtoets.Integration.Test.FileImporters
{
    [TestFixture]
    public class ForeshoreProfileUpdateDataStrategyIntegrationTest
    {
        private const string sourceFilePath = "path/to/foreshoreProfiles";

        [Test]
        [TestCaseSource(nameof(GetSupportedFailureMechanisms))]
        public void UpdateForeshoreProfilesWithImportedData_SupportedFailureMechanisms_UpdatesAffectedCalculationAndReturnsAffectedData(
            IFailureMechanism failureMechanism,
            ForeshoreProfileCollection foreshoreProfiles,
            ForeshoreProfile unaffectedForeshoreProfile)
        {
            // Setup
            ForeshoreProfile profileToBeUpdated = foreshoreProfiles[0];
            ForeshoreProfile profileToUpdateFrom = DeepCloneAndModify(profileToBeUpdated);
            ForeshoreProfile profileToBeRemoved = foreshoreProfiles[1];

            var strategy = new ForeshoreProfileUpdateDataStrategy(failureMechanism, foreshoreProfiles);

            ICalculation<ICalculationInput>[] calculationsWithUpdatedForeshoreProfile =
                failureMechanism.Calculations
                                .Cast<ICalculation<ICalculationInput>>()
                                .Where(calc => ReferenceEquals(GetForeshoreProfile(calc),
                                                               profileToBeUpdated))
                                .ToArray();

            ICalculation<ICalculationInput>[] calculationsWithUpdatedForeshoreProfileWithOutputs =
                calculationsWithUpdatedForeshoreProfile.Where(calc => calc.HasOutput)
                                                       .ToArray();

            ICalculation<ICalculationInput>[] calculationsWithRemovedForeshoreProfile =
                failureMechanism.Calculations
                                .Cast<ICalculation<ICalculationInput>>()
                                .Where(calc => ReferenceEquals(GetForeshoreProfile(calc),
                                                               profileToBeRemoved))
                                .ToArray();

            ICalculation<ICalculationInput>[] calculationsWithRemovedForeshoreProfileWithOutputs =
                calculationsWithRemovedForeshoreProfile.Where(calc => calc.HasOutput)
                                                       .ToArray();

            ICalculation<ICalculationInput>[] calculationsWithUnaffectedForeshoreProfile =
                failureMechanism.Calculations
                                .Cast<ICalculation<ICalculationInput>>()
                                .Where(calc => ReferenceEquals(GetForeshoreProfile(calc),
                                                               unaffectedForeshoreProfile))
                                .ToArray();

            ICalculation<ICalculationInput>[] calculationsWithUnaffectedForeshoreProfileAndOutput =
                failureMechanism.Calculations
                                .Cast<ICalculation<ICalculationInput>>()
                                .Where(calc => ReferenceEquals(GetForeshoreProfile(calc),
                                                               unaffectedForeshoreProfile))
                                .ToArray();

            // Call
            IEnumerable<IObservable> affectedObjects =
                strategy.UpdateForeshoreProfilesWithImportedData(new[]
                                                                 {
                                                                     profileToUpdateFrom,
                                                                     new TestForeshoreProfile(unaffectedForeshoreProfile.Name,
                                                                                              unaffectedForeshoreProfile.Id)
                                                                 },
                                                                 sourceFilePath);

            // Assert
            Assert.IsTrue(calculationsWithUnaffectedForeshoreProfileAndOutput.All(calc => calc.HasOutput));
            Assert.IsTrue(calculationsWithUnaffectedForeshoreProfile.All(calc => ReferenceEquals(GetForeshoreProfile(calc),
                                                                                                 unaffectedForeshoreProfile)));

            Assert.IsTrue(calculationsWithUpdatedForeshoreProfile.All(calc => !calc.HasOutput));
            Assert.IsTrue(calculationsWithUpdatedForeshoreProfile.All(calc => ReferenceEquals(GetForeshoreProfile(calc),
                                                                                              profileToBeUpdated)));

            Assert.IsTrue(calculationsWithRemovedForeshoreProfile.All(calc => !calc.HasOutput));
            Assert.IsTrue(calculationsWithRemovedForeshoreProfile.All(calc => GetForeshoreProfile(calc) == null));

            var expectedAffectedObjects = new List<IObservable>
            {
                foreshoreProfiles,
                profileToBeUpdated
            };
            expectedAffectedObjects.AddRange(calculationsWithUpdatedForeshoreProfileWithOutputs);
            expectedAffectedObjects.AddRange(calculationsWithRemovedForeshoreProfileWithOutputs);
            expectedAffectedObjects.AddRange(calculationsWithUpdatedForeshoreProfile.Select(calc => calc.InputParameters));
            expectedAffectedObjects.AddRange(calculationsWithRemovedForeshoreProfile.Select(calc => calc.InputParameters));

            CollectionAssert.AreEquivalent(expectedAffectedObjects, affectedObjects);
        }

        private static ForeshoreProfile GetForeshoreProfile(ICalculation<ICalculationInput> calc)
        {
            return ((IHasForeshoreProfile) calc.InputParameters).ForeshoreProfile;
        }

        /// <summary>
        /// Makes a deep clone of the foreshore profile and modifies all the properties,
        /// except the <see cref="ForeshoreProfile.Id"/>.
        /// </summary>
        /// <param name="foreshoreProfile">The foreshore profile to deep clone.</param>
        /// <returns>A deep clone of the <paramref name="foreshoreProfile"/>.</returns>
        private static ForeshoreProfile DeepCloneAndModify(ForeshoreProfile foreshoreProfile)
        {
            var random = new Random(21);

            Point2D originalWorldCoordinate = foreshoreProfile.WorldReferencePoint;
            var modifiedWorldCoordinate = new Point2D(originalWorldCoordinate.X + random.NextDouble(),
                                                      originalWorldCoordinate.Y + random.NextDouble());

            List<Point2D> modifiedForeshoreGeometry = foreshoreProfile.Geometry.ToList();
            modifiedForeshoreGeometry.Add(new Point2D(1, 2));

            RoundedDouble originalBreakWaterHeight = foreshoreProfile.BreakWater?.Height ?? (RoundedDouble) 0.0;
            var modifiedBreakWater = new BreakWater(random.NextEnumValue<BreakWaterType>(),
                                                    originalBreakWaterHeight + random.NextDouble());

            string modifiedName = $"new_name_{foreshoreProfile.Name}";
            double modifiedOrientation = foreshoreProfile.Orientation + random.NextDouble();
            double modifiedX0 = foreshoreProfile.X0 + random.NextDouble();

            return new ForeshoreProfile(modifiedWorldCoordinate, modifiedForeshoreGeometry,
                                        modifiedBreakWater,
                                        new ForeshoreProfile.ConstructionProperties
                                        {
                                            Name = modifiedName,
                                            Id = foreshoreProfile.Id,
                                            Orientation = modifiedOrientation,
                                            X0 = modifiedX0
                                        });
        }

        #region TestData

        private static IEnumerable<TestCaseData> GetSupportedFailureMechanisms()
        {
            const string unaffectedProfileName = "Custom Profile";
            const string unaffectedProfileId = "Custom ID";

            var unaffectedForeshoreProfile = new TestForeshoreProfile(unaffectedProfileName, unaffectedProfileId);

            WaveImpactAsphaltCoverFailureMechanism waveImpactAsphaltCoverFailureMechanism =
                CreateWaveImpactAsphaltCoverFailureMechanismWithAllUpdateForeshoreProfileScenarios(unaffectedForeshoreProfile);
            yield return new TestCaseData(
                    waveImpactAsphaltCoverFailureMechanism,
                    waveImpactAsphaltCoverFailureMechanism.ForeshoreProfiles,
                    unaffectedForeshoreProfile)
                .SetName("WaveImpactAsphaltCoverFailureMechanism");

            GrassCoverErosionOutwardsFailureMechanism grassCoverErosionOutwardsFailureMechanism =
                CreateGrassCoverErosionOutwardsFailureMechanismWithAllUpdateForeshoreProfileScenarios(unaffectedForeshoreProfile);
            yield return new TestCaseData(
                    grassCoverErosionOutwardsFailureMechanism,
                    grassCoverErosionOutwardsFailureMechanism.ForeshoreProfiles,
                    unaffectedForeshoreProfile)
                .SetName("GrassCoverErosionOutwardsFailureMechanism");

            StabilityStoneCoverFailureMechanism stabilityStoneCoverFailureMechanism =
                CreateStabilityStoneCoverFailureMechanismWithAllUpdateForeshoreProfileScenarios(unaffectedForeshoreProfile);
            yield return new TestCaseData(
                    stabilityStoneCoverFailureMechanism,
                    stabilityStoneCoverFailureMechanism.ForeshoreProfiles,
                    unaffectedForeshoreProfile)
                .SetName("StabilityStoneCoverFailureMechanism");

            StabilityPointStructuresFailureMechanism stabilityPointStructuresFailureMechanism =
                CreateStabilityPointStructuresFailureMechanismWithAllUpdateForeshoreProfileScenarios(unaffectedForeshoreProfile);
            yield return new TestCaseData(
                    stabilityPointStructuresFailureMechanism,
                    stabilityPointStructuresFailureMechanism.ForeshoreProfiles,
                    unaffectedForeshoreProfile)
                .SetName("StabilityPointStructuresFailureMechanism");

            ClosingStructuresFailureMechanism closingStructuresFailureMechanism =
                CreateClosingStructuresFailureMechanismWithAllUpdateForeshoreProfileScenarios(unaffectedForeshoreProfile);
            yield return new TestCaseData(
                    closingStructuresFailureMechanism,
                    closingStructuresFailureMechanism.ForeshoreProfiles,
                    unaffectedForeshoreProfile)
                .SetName("ClosingStructuresFailureMechanism");

            HeightStructuresFailureMechanism heightStructuresFailureMechanism =
                CreateHeightStructuresFailureMechanismWithAllUpdateForeshoreProfileScenarios(unaffectedForeshoreProfile);
            yield return new TestCaseData(
                    heightStructuresFailureMechanism,
                    heightStructuresFailureMechanism.ForeshoreProfiles,
                    unaffectedForeshoreProfile)
                .SetName("HeightStructuresFailureMechanism");
        }

        private static WaveImpactAsphaltCoverFailureMechanism CreateWaveImpactAsphaltCoverFailureMechanismWithAllUpdateForeshoreProfileScenarios(
            ForeshoreProfile unaffectedForeshoreProfile)
        {
            WaveImpactAsphaltCoverFailureMechanism waveImpactAsphaltCoverFailureMechanism =
                TestDataGenerator.GetWaveImpactAsphaltCoverFailureMechanismWithAllCalculationConfigurations();
            waveImpactAsphaltCoverFailureMechanism.ForeshoreProfiles.AddRange(new[]
            {
                unaffectedForeshoreProfile
            }, sourceFilePath);
            var unaffectedCalculation = new WaveImpactAsphaltCoverWaveConditionsCalculation
            {
                InputParameters =
                {
                    ForeshoreProfile = unaffectedForeshoreProfile
                },
                Output = new WaveImpactAsphaltCoverWaveConditionsOutput(new[]
                {
                    new TestWaveConditionsOutput()
                })
            };
            waveImpactAsphaltCoverFailureMechanism.WaveConditionsCalculationGroup.Children.Add(unaffectedCalculation);
            return waveImpactAsphaltCoverFailureMechanism;
        }

        private static GrassCoverErosionOutwardsFailureMechanism CreateGrassCoverErosionOutwardsFailureMechanismWithAllUpdateForeshoreProfileScenarios(
            ForeshoreProfile unaffectedForeshoreProfile)
        {
            GrassCoverErosionOutwardsFailureMechanism grassCoverErosionOutwardsFailureMechanism =
                TestDataGenerator.GetGrassCoverErosionOutwardsFailureMechanismWithAllCalculationConfigurations();
            grassCoverErosionOutwardsFailureMechanism.ForeshoreProfiles.AddRange(new[]
            {
                unaffectedForeshoreProfile
            }, sourceFilePath);
            var unaffectedCalculation = new GrassCoverErosionOutwardsWaveConditionsCalculation
            {
                InputParameters =
                {
                    ForeshoreProfile = unaffectedForeshoreProfile
                },
                Output = new GrassCoverErosionOutwardsWaveConditionsOutput(new[]
                {
                    new TestWaveConditionsOutput()
                })
            };
            grassCoverErosionOutwardsFailureMechanism.WaveConditionsCalculationGroup.Children.Add(unaffectedCalculation);
            return grassCoverErosionOutwardsFailureMechanism;
        }

        private static StabilityStoneCoverFailureMechanism CreateStabilityStoneCoverFailureMechanismWithAllUpdateForeshoreProfileScenarios(
            ForeshoreProfile unaffectedForeshoreProfile)
        {
            StabilityStoneCoverFailureMechanism stabilityStoneCoverFailureMechanism =
                TestDataGenerator.GetStabilityStoneCoverFailureMechanismWithAllCalculationConfigurations();
            stabilityStoneCoverFailureMechanism.ForeshoreProfiles.AddRange(new[]
            {
                unaffectedForeshoreProfile
            }, sourceFilePath);
            var unaffectedCalculation = new StabilityStoneCoverWaveConditionsCalculation
            {
                InputParameters =
                {
                    ForeshoreProfile = unaffectedForeshoreProfile
                },
                Output = new StabilityStoneCoverWaveConditionsOutput(new[]
                                                                     {
                                                                         new TestWaveConditionsOutput()
                                                                     },
                                                                     new[]
                                                                     {
                                                                         new TestWaveConditionsOutput()
                                                                     })
            };
            stabilityStoneCoverFailureMechanism.WaveConditionsCalculationGroup.Children.Add(unaffectedCalculation);
            return stabilityStoneCoverFailureMechanism;
        }

        private static HeightStructuresFailureMechanism CreateHeightStructuresFailureMechanismWithAllUpdateForeshoreProfileScenarios(
            ForeshoreProfile unaffectedForeshoreProfile)
        {
            HeightStructuresFailureMechanism heightStructuresFailureMechanism =
                TestDataGenerator.GetHeightStructuresFailureMechanismWithAlLCalculationConfigurations();
            heightStructuresFailureMechanism.ForeshoreProfiles.AddRange(new[]
            {
                unaffectedForeshoreProfile
            }, sourceFilePath);
            var unaffectedCalculation = new TestHeightStructuresCalculation
            {
                InputParameters =
                {
                    ForeshoreProfile = unaffectedForeshoreProfile
                },
                Output = new TestStructuresOutput()
            };
            heightStructuresFailureMechanism.CalculationsGroup.Children.Add(unaffectedCalculation);
            return heightStructuresFailureMechanism;
        }

        private static ClosingStructuresFailureMechanism CreateClosingStructuresFailureMechanismWithAllUpdateForeshoreProfileScenarios(
            ForeshoreProfile unaffectedForeshoreProfile)
        {
            ClosingStructuresFailureMechanism closingStructuresFailureMechanism =
                TestDataGenerator.GetClosingStructuresFailureMechanismWithAllCalculationConfigurations();
            closingStructuresFailureMechanism.ForeshoreProfiles.AddRange(new[]
            {
                unaffectedForeshoreProfile
            }, sourceFilePath);
            var unaffectedCalculation = new TestClosingStructuresCalculation
            {
                InputParameters =
                {
                    ForeshoreProfile = unaffectedForeshoreProfile
                },
                Output = new TestStructuresOutput()
            };
            closingStructuresFailureMechanism.CalculationsGroup.Children.Add(unaffectedCalculation);
            return closingStructuresFailureMechanism;
        }

        private static StabilityPointStructuresFailureMechanism CreateStabilityPointStructuresFailureMechanismWithAllUpdateForeshoreProfileScenarios(
            ForeshoreProfile unaffectedForeshoreProfile)
        {
            StabilityPointStructuresFailureMechanism stabilityPointStructuresFailureMechanism =
                TestDataGenerator.GetStabilityPointStructuresFailureMechanismWithAllCalculationConfigurations();
            stabilityPointStructuresFailureMechanism.ForeshoreProfiles.AddRange(new[]
            {
                unaffectedForeshoreProfile
            }, sourceFilePath);
            var unaffectedCalculation = new TestStabilityPointStructuresCalculation
            {
                InputParameters =
                {
                    ForeshoreProfile = unaffectedForeshoreProfile
                },
                Output = new TestStructuresOutput()
            };
            stabilityPointStructuresFailureMechanism.CalculationsGroup.Children.Add(unaffectedCalculation);
            return stabilityPointStructuresFailureMechanism;
        }

        #endregion
    }
}