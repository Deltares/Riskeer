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
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.HeightStructures.Data;
using Ringtoets.HeightStructures.Data.TestUtil;
using Ringtoets.Integration.Plugin.FileImporters;
using Ringtoets.Integration.TestUtils;
using Ringtoets.Revetment.TestUtil;
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
        public void UpdateForeshoreProfilesWithImportedData_FullyConfiguredWaveImpactAsphaltCoverFailureMechanism_UpdatesAffectedCalculation()
        {
            // Setup
            WaveImpactAsphaltCoverFailureMechanism failureMechanism =
                TestDataGenerator.GetWaveImpactAsphaltCoverFailureMechanismWithAllCalculationConfigurations();

            ForeshoreProfile profileToBeUpdated = failureMechanism.ForeshoreProfiles[0];
            ForeshoreProfile profileToUpdateFrom = DeepCloneAndModify(profileToBeUpdated);
            ForeshoreProfile profileToBeRemoved = failureMechanism.ForeshoreProfiles[1];

            const string unaffectedProfileName = "Custom Profile";
            const string unaffectedProfileId = "Custom ID";
            var unaffectedProfile = new TestForeshoreProfile(unaffectedProfileName, unaffectedProfileId);
            failureMechanism.ForeshoreProfiles.AddRange(new[]
            {
                unaffectedProfile
            }, sourceFilePath);
            var unaffectedCalculation = new WaveImpactAsphaltCoverWaveConditionsCalculation
            {
                InputParameters =
                {
                    ForeshoreProfile = unaffectedProfile
                },
                Output = new WaveImpactAsphaltCoverWaveConditionsOutput(new[]
                {
                    new TestWaveConditionsOutput()
                })
            };
            failureMechanism.WaveConditionsCalculationGroup.Children.Add(unaffectedCalculation);

            var strategy = new ForeshoreProfileUpdateDataStrategy(failureMechanism);

            ICalculation<ICalculationInput>[] calculationsWithUpdatedForeshoreProfile =
                failureMechanism.Calculations
                                .Cast<ICalculation<ICalculationInput>>()
                                .Where(calc => ReferenceEquals(((IHasForeshoreProfile) calc.InputParameters).ForeshoreProfile,
                                                               profileToBeUpdated))
                                .ToArray();

            ICalculation<ICalculationInput>[] calculationsWithUpdatedForeshoreProfileWithOutputs =
                calculationsWithUpdatedForeshoreProfile.Where(calc => calc.HasOutput)
                                                       .ToArray();

            ICalculation<ICalculationInput>[] calculationsWithRemovedForeshoreProfile =
                failureMechanism.Calculations
                                .Cast<ICalculation<ICalculationInput>>()
                                .Where(calc => ReferenceEquals(((IHasForeshoreProfile) calc.InputParameters).ForeshoreProfile,
                                                               profileToBeRemoved))
                                .ToArray();

            ICalculation<ICalculationInput>[] calculationsWithRemovedForeshoreProfileWithOutputs =
                calculationsWithRemovedForeshoreProfile.Where(calc => calc.HasOutput)
                                                       .ToArray();

            // Call
            IEnumerable<IObservable> affectedObjects =
                strategy.UpdateForeshoreProfilesWithImportedData(failureMechanism.ForeshoreProfiles,
                                                                 new[]
                                                                 {
                                                                     profileToUpdateFrom,
                                                                     new TestForeshoreProfile(unaffectedProfileName,
                                                                                              unaffectedProfileId)
                                                                 },
                                                                 sourceFilePath);

            // Assert
            Assert.IsTrue(unaffectedCalculation.HasOutput);
            Assert.AreSame(unaffectedProfile, unaffectedCalculation.InputParameters.ForeshoreProfile);

            Assert.IsTrue(calculationsWithUpdatedForeshoreProfile.All(calc => !calc.HasOutput));
            Assert.IsTrue(calculationsWithUpdatedForeshoreProfile.All(calc => ReferenceEquals(GetForeshoreProfile(calc),
                                                                                              profileToBeUpdated)));

            Assert.IsTrue(calculationsWithRemovedForeshoreProfile.All(calc => !calc.HasOutput));
            Assert.IsTrue(calculationsWithRemovedForeshoreProfile.All(calc => GetForeshoreProfile(calc) == null));

            var expectedAffectedObjects = new List<IObservable>
            {
                failureMechanism.ForeshoreProfiles,
                profileToBeUpdated
            };
            expectedAffectedObjects.AddRange(calculationsWithUpdatedForeshoreProfileWithOutputs);
            expectedAffectedObjects.AddRange(calculationsWithRemovedForeshoreProfileWithOutputs);
            expectedAffectedObjects.AddRange(calculationsWithUpdatedForeshoreProfile.Select(calc => calc.InputParameters));
            expectedAffectedObjects.AddRange(calculationsWithRemovedForeshoreProfile.Select(calc => calc.InputParameters));

            CollectionAssert.AreEquivalent(expectedAffectedObjects, affectedObjects);
        }

        [Test]
        public void UpdateForeshoreProfilesWithImportedData_FullyConfiguredGrassCoverOutwardsFailureMechanism_UpdatesAffectedCalculation()
        {
            // Setup
            GrassCoverErosionOutwardsFailureMechanism failureMechanism =
                TestDataGenerator.GetGrassCoverErosionOutwardsFailureMechanismWithAllCalculationConfigurations();

            ForeshoreProfile profileToBeUpdated = failureMechanism.ForeshoreProfiles[0];
            ForeshoreProfile profileToUpdateFrom = DeepCloneAndModify(profileToBeUpdated);
            ForeshoreProfile profileToBeRemoved = failureMechanism.ForeshoreProfiles[1];

            const string unaffectedProfileName = "Custom Profile";
            const string unaffectedProfileId = "Custom ID";
            var unaffectedProfile = new TestForeshoreProfile(unaffectedProfileName, unaffectedProfileId);
            failureMechanism.ForeshoreProfiles.AddRange(new[]
            {
                unaffectedProfile
            }, sourceFilePath);
            var unaffectedCalculation = new GrassCoverErosionOutwardsWaveConditionsCalculation
            {
                InputParameters =
                {
                    ForeshoreProfile = unaffectedProfile
                },
                Output = new GrassCoverErosionOutwardsWaveConditionsOutput(new[]
                {
                    new TestWaveConditionsOutput()
                })
            };
            failureMechanism.WaveConditionsCalculationGroup.Children.Add(unaffectedCalculation);

            var strategy = new ForeshoreProfileUpdateDataStrategy(failureMechanism);

            ICalculation<ICalculationInput>[] calculationsWithUpdatedForeshoreProfile =
                failureMechanism.Calculations
                                .Cast<ICalculation<ICalculationInput>>()
                                .Where(calc => ReferenceEquals(((IHasForeshoreProfile) calc.InputParameters).ForeshoreProfile,
                                                               profileToBeUpdated))
                                .ToArray();

            ICalculation<ICalculationInput>[] calculationsWithUpdatedForeshoreProfileWithOutputs =
                calculationsWithUpdatedForeshoreProfile.Where(calc => calc.HasOutput)
                                                       .ToArray();

            ICalculation<ICalculationInput>[] calculationsWithRemovedForeshoreProfile =
                failureMechanism.Calculations
                                .Cast<ICalculation<ICalculationInput>>()
                                .Where(calc => ReferenceEquals(((IHasForeshoreProfile) calc.InputParameters).ForeshoreProfile,
                                                               profileToBeRemoved))
                                .ToArray();

            ICalculation<ICalculationInput>[] calculationsWithRemovedForeshoreProfileWithOutputs =
                calculationsWithRemovedForeshoreProfile.Where(calc => calc.HasOutput)
                                                       .ToArray();

            // Call
            IEnumerable<IObservable> affectedObjects =
                strategy.UpdateForeshoreProfilesWithImportedData(failureMechanism.ForeshoreProfiles,
                                                                 new[]
                                                                 {
                                                                     profileToUpdateFrom,
                                                                     new TestForeshoreProfile(unaffectedProfileName,
                                                                                              unaffectedProfileId)
                                                                 },
                                                                 sourceFilePath);

            // Assert
            Assert.IsTrue(unaffectedCalculation.HasOutput);
            Assert.AreSame(unaffectedProfile, unaffectedCalculation.InputParameters.ForeshoreProfile);

            Assert.IsTrue(calculationsWithUpdatedForeshoreProfile.All(calc => !calc.HasOutput));
            Assert.IsTrue(calculationsWithUpdatedForeshoreProfile.All(calc => ReferenceEquals(GetForeshoreProfile(calc),
                                                                                              profileToBeUpdated)));

            Assert.IsTrue(calculationsWithRemovedForeshoreProfile.All(calc => !calc.HasOutput));
            Assert.IsTrue(calculationsWithRemovedForeshoreProfile.All(calc => GetForeshoreProfile(calc) == null));

            var expectedAffectedObjects = new List<IObservable>
            {
                failureMechanism.ForeshoreProfiles,
                profileToBeUpdated
            };
            expectedAffectedObjects.AddRange(calculationsWithUpdatedForeshoreProfileWithOutputs);
            expectedAffectedObjects.AddRange(calculationsWithRemovedForeshoreProfileWithOutputs);
            expectedAffectedObjects.AddRange(calculationsWithUpdatedForeshoreProfile.Select(calc => calc.InputParameters));
            expectedAffectedObjects.AddRange(calculationsWithRemovedForeshoreProfile.Select(calc => calc.InputParameters));

            CollectionAssert.AreEquivalent(expectedAffectedObjects, affectedObjects);
        }

        [Test]
        public void UpdateForeshoreProfilesWithImportedData_FullyConfiguredStabilityStoneCoverFailureMechanism_UpdatesAffectedCalculation()
        {
            // Setup
            StabilityStoneCoverFailureMechanism failureMechanism =
                TestDataGenerator.GetStabilityStoneCoverFailureMechanismWithAllCalculationConfigurations();

            ForeshoreProfile profileToBeUpdated = failureMechanism.ForeshoreProfiles[0];
            ForeshoreProfile profileToUpdateFrom = DeepCloneAndModify(profileToBeUpdated);
            ForeshoreProfile profileToBeRemoved = failureMechanism.ForeshoreProfiles[1];

            const string unaffectedProfileName = "Custom Profile";
            const string unaffectedProfileId = "Custom ID";
            var unaffectedProfile = new TestForeshoreProfile(unaffectedProfileName, unaffectedProfileId);
            failureMechanism.ForeshoreProfiles.AddRange(new[]
            {
                unaffectedProfile
            }, sourceFilePath);
            var unaffectedCalculation = new StabilityStoneCoverWaveConditionsCalculation
            {
                InputParameters =
                {
                    ForeshoreProfile = unaffectedProfile
                },
                Output = new StabilityStoneCoverWaveConditionsOutput(new[]
                {
                    new TestWaveConditionsOutput()
                }, new[]
                {
                    new TestWaveConditionsOutput()
                })
            };
            failureMechanism.WaveConditionsCalculationGroup.Children.Add(unaffectedCalculation);

            var strategy = new ForeshoreProfileUpdateDataStrategy(failureMechanism);

            ICalculation<ICalculationInput>[] calculationsWithUpdatedForeshoreProfile =
                failureMechanism.Calculations
                                .Cast<ICalculation<ICalculationInput>>()
                                .Where(calc => ReferenceEquals(((IHasForeshoreProfile) calc.InputParameters).ForeshoreProfile,
                                                               profileToBeUpdated))
                                .ToArray();

            ICalculation<ICalculationInput>[] calculationsWithUpdatedForeshoreProfileWithOutputs =
                calculationsWithUpdatedForeshoreProfile.Where(calc => calc.HasOutput)
                                                       .ToArray();

            ICalculation<ICalculationInput>[] calculationsWithRemovedForeshoreProfile =
                failureMechanism.Calculations
                                .Cast<ICalculation<ICalculationInput>>()
                                .Where(calc => ReferenceEquals(((IHasForeshoreProfile) calc.InputParameters).ForeshoreProfile,
                                                               profileToBeRemoved))
                                .ToArray();

            ICalculation<ICalculationInput>[] calculationsWithRemovedForeshoreProfileWithOutputs =
                calculationsWithRemovedForeshoreProfile.Where(calc => calc.HasOutput)
                                                       .ToArray();

            // Call
            IEnumerable<IObservable> affectedObjects =
                strategy.UpdateForeshoreProfilesWithImportedData(failureMechanism.ForeshoreProfiles,
                                                                 new[]
                                                                 {
                                                                     profileToUpdateFrom,
                                                                     new TestForeshoreProfile(unaffectedProfileName,
                                                                                              unaffectedProfileId)
                                                                 },
                                                                 sourceFilePath);

            // Assert
            Assert.IsTrue(unaffectedCalculation.HasOutput);
            Assert.AreSame(unaffectedProfile, unaffectedCalculation.InputParameters.ForeshoreProfile);

            Assert.IsTrue(calculationsWithUpdatedForeshoreProfile.All(calc => !calc.HasOutput));
            Assert.IsTrue(calculationsWithUpdatedForeshoreProfile.All(calc => ReferenceEquals(GetForeshoreProfile(calc),
                                                                                              profileToBeUpdated)));

            Assert.IsTrue(calculationsWithRemovedForeshoreProfile.All(calc => !calc.HasOutput));
            Assert.IsTrue(calculationsWithRemovedForeshoreProfile.All(calc => GetForeshoreProfile(calc) == null));

            var expectedAffectedObjects = new List<IObservable>
            {
                failureMechanism.ForeshoreProfiles,
                profileToBeUpdated
            };
            expectedAffectedObjects.AddRange(calculationsWithUpdatedForeshoreProfileWithOutputs);
            expectedAffectedObjects.AddRange(calculationsWithRemovedForeshoreProfileWithOutputs);
            expectedAffectedObjects.AddRange(calculationsWithUpdatedForeshoreProfile.Select(calc => calc.InputParameters));
            expectedAffectedObjects.AddRange(calculationsWithRemovedForeshoreProfile.Select(calc => calc.InputParameters));

            CollectionAssert.AreEquivalent(expectedAffectedObjects, affectedObjects);
        }

        [Test]
        public void UpdateForeshoreProfilesWithImportedData_FullyConfiguredHeightStructuresFailureMechanism_UpdatesAffectedCalculation()
        {
            // Setup
            HeightStructuresFailureMechanism failureMechanism =
                TestDataGenerator.GetHeightStructuresFailureMechanismWithAlLCalculationConfigurations();

            ForeshoreProfile profileToBeUpdated = failureMechanism.ForeshoreProfiles[0];
            ForeshoreProfile profileToUpdateFrom = DeepCloneAndModify(profileToBeUpdated);
            ForeshoreProfile profileToBeRemoved = failureMechanism.ForeshoreProfiles[1];

            const string unaffectedProfileName = "Custom Profile";
            const string unaffectedProfileId = "Custom ID";
            var unaffectedProfile = new TestForeshoreProfile(unaffectedProfileName, unaffectedProfileId);
            failureMechanism.ForeshoreProfiles.AddRange(new[]
            {
                unaffectedProfile
            }, sourceFilePath);
            var unaffectedCalculation = new TestHeightStructuresCalculation
            {
                InputParameters =
                {
                    ForeshoreProfile = unaffectedProfile
                },
                Output = new TestStructuresOutput()
            };
            failureMechanism.CalculationsGroup.Children.Add(unaffectedCalculation);

            var strategy = new ForeshoreProfileUpdateDataStrategy(failureMechanism);

            ICalculation<ICalculationInput>[] calculationsWithUpdatedForeshoreProfile =
                failureMechanism.Calculations
                                .Cast<ICalculation<ICalculationInput>>()
                                .Where(calc => ReferenceEquals(((IHasForeshoreProfile) calc.InputParameters).ForeshoreProfile,
                                                               profileToBeUpdated))
                                .ToArray();

            ICalculation<ICalculationInput>[] calculationsWithUpdatedForeshoreProfileWithOutputs =
                calculationsWithUpdatedForeshoreProfile.Where(calc => calc.HasOutput)
                                                       .ToArray();

            ICalculation<ICalculationInput>[] calculationsWithRemovedForeshoreProfile =
                failureMechanism.Calculations
                                .Cast<ICalculation<ICalculationInput>>()
                                .Where(calc => ReferenceEquals(((IHasForeshoreProfile) calc.InputParameters).ForeshoreProfile,
                                                               profileToBeRemoved))
                                .ToArray();

            ICalculation<ICalculationInput>[] calculationsWithRemovedForeshoreProfileWithOutputs =
                calculationsWithRemovedForeshoreProfile.Where(calc => calc.HasOutput)
                                                       .ToArray();

            // Call
            IEnumerable<IObservable> affectedObjects =
                strategy.UpdateForeshoreProfilesWithImportedData(failureMechanism.ForeshoreProfiles,
                                                                 new[]
                                                                 {
                                                                     profileToUpdateFrom,
                                                                     new TestForeshoreProfile(unaffectedProfileName,
                                                                                              unaffectedProfileId)
                                                                 },
                                                                 sourceFilePath);

            // Assert
            Assert.IsTrue(unaffectedCalculation.HasOutput);
            Assert.AreSame(unaffectedProfile, unaffectedCalculation.InputParameters.ForeshoreProfile);

            Assert.IsTrue(calculationsWithUpdatedForeshoreProfile.All(calc => !calc.HasOutput));
            Assert.IsTrue(calculationsWithUpdatedForeshoreProfile.All(calc => ReferenceEquals(GetForeshoreProfile(calc),
                                                                                              profileToBeUpdated)));

            Assert.IsTrue(calculationsWithRemovedForeshoreProfile.All(calc => !calc.HasOutput));
            Assert.IsTrue(calculationsWithRemovedForeshoreProfile.All(calc => GetForeshoreProfile(calc) == null));

            var expectedAffectedObjects = new List<IObservable>
            {
                failureMechanism.ForeshoreProfiles,
                profileToBeUpdated
            };
            expectedAffectedObjects.AddRange(calculationsWithUpdatedForeshoreProfileWithOutputs);
            expectedAffectedObjects.AddRange(calculationsWithRemovedForeshoreProfileWithOutputs);
            expectedAffectedObjects.AddRange(calculationsWithUpdatedForeshoreProfile.Select(calc => calc.InputParameters));
            expectedAffectedObjects.AddRange(calculationsWithRemovedForeshoreProfile.Select(calc => calc.InputParameters));

            CollectionAssert.AreEquivalent(expectedAffectedObjects, affectedObjects);
        }

        [Test]
        public void UpdateForeshoreProfilesWithImportedData_FullyConfiguredStabilityPointStructuresFailureMechanism_UpdatesAffectedCalculation()
        {
            // Setup
            StabilityPointStructuresFailureMechanism failureMechanism =
                TestDataGenerator.GetStabilityPointStructuresFailureMechanismWithAllCalculationConfigurations();

            ForeshoreProfile profileToBeUpdated = failureMechanism.ForeshoreProfiles[0];
            ForeshoreProfile profileToUpdateFrom = DeepCloneAndModify(profileToBeUpdated);
            ForeshoreProfile profileToBeRemoved = failureMechanism.ForeshoreProfiles[1];

            const string unaffectedProfileName = "Custom Profile";
            const string unaffectedProfileId = "Custom ID";
            var unaffectedProfile = new TestForeshoreProfile(unaffectedProfileName, unaffectedProfileId);
            failureMechanism.ForeshoreProfiles.AddRange(new[]
            {
                unaffectedProfile
            }, sourceFilePath);
            var unaffectedCalculation = new TestStabilityPointStructuresCalculation
            {
                InputParameters =
                {
                    ForeshoreProfile = unaffectedProfile
                },
                Output = new TestStructuresOutput()
            };
            failureMechanism.CalculationsGroup.Children.Add(unaffectedCalculation);

            var strategy = new ForeshoreProfileUpdateDataStrategy(failureMechanism);

            ICalculation<ICalculationInput>[] calculationsWithUpdatedForeshoreProfile =
                failureMechanism.Calculations
                                .Cast<ICalculation<ICalculationInput>>()
                                .Where(calc => ReferenceEquals(((IHasForeshoreProfile) calc.InputParameters).ForeshoreProfile,
                                                               profileToBeUpdated))
                                .ToArray();

            ICalculation<ICalculationInput>[] calculationsWithUpdatedForeshoreProfileWithOutputs =
                calculationsWithUpdatedForeshoreProfile.Where(calc => calc.HasOutput)
                                                       .ToArray();

            ICalculation<ICalculationInput>[] calculationsWithRemovedForeshoreProfile =
                failureMechanism.Calculations
                                .Cast<ICalculation<ICalculationInput>>()
                                .Where(calc => ReferenceEquals(((IHasForeshoreProfile) calc.InputParameters).ForeshoreProfile,
                                                               profileToBeRemoved))
                                .ToArray();

            ICalculation<ICalculationInput>[] calculationsWithRemovedForeshoreProfileWithOutputs =
                calculationsWithRemovedForeshoreProfile.Where(calc => calc.HasOutput)
                                                       .ToArray();

            // Call
            IEnumerable<IObservable> affectedObjects =
                strategy.UpdateForeshoreProfilesWithImportedData(failureMechanism.ForeshoreProfiles,
                                                                 new[]
                                                                 {
                                                                     profileToUpdateFrom,
                                                                     new TestForeshoreProfile(unaffectedProfileName,
                                                                                              unaffectedProfileId)
                                                                 },
                                                                 sourceFilePath);

            // Assert
            Assert.IsTrue(unaffectedCalculation.HasOutput);
            Assert.AreSame(unaffectedProfile, unaffectedCalculation.InputParameters.ForeshoreProfile);

            Assert.IsTrue(calculationsWithUpdatedForeshoreProfile.All(calc => !calc.HasOutput));
            Assert.IsTrue(calculationsWithUpdatedForeshoreProfile.All(calc => ReferenceEquals(GetForeshoreProfile(calc),
                                                                                              profileToBeUpdated)));

            Assert.IsTrue(calculationsWithRemovedForeshoreProfile.All(calc => !calc.HasOutput));
            Assert.IsTrue(calculationsWithRemovedForeshoreProfile.All(calc => GetForeshoreProfile(calc) == null));

            var expectedAffectedObjects = new List<IObservable>
            {
                failureMechanism.ForeshoreProfiles,
                profileToBeUpdated
            };
            expectedAffectedObjects.AddRange(calculationsWithUpdatedForeshoreProfileWithOutputs);
            expectedAffectedObjects.AddRange(calculationsWithRemovedForeshoreProfileWithOutputs);
            expectedAffectedObjects.AddRange(calculationsWithUpdatedForeshoreProfile.Select(calc => calc.InputParameters));
            expectedAffectedObjects.AddRange(calculationsWithRemovedForeshoreProfile.Select(calc => calc.InputParameters));

            CollectionAssert.AreEquivalent(expectedAffectedObjects, affectedObjects);
        }

        [Test]
        public void UpdateForeshoreProfilesWithImportedData_FullyConfiguredClosingStructuresFailureMechanism_UpdatesAffectedCalculation()
        {
            // Setup
            ClosingStructuresFailureMechanism failureMechanism =
                TestDataGenerator.GetClosingStructuresFailureMechanismWithAllCalculationConfigurations();

            ForeshoreProfile profileToBeUpdated = failureMechanism.ForeshoreProfiles[0];
            ForeshoreProfile profileToUpdateFrom = DeepCloneAndModify(profileToBeUpdated);
            ForeshoreProfile profileToBeRemoved = failureMechanism.ForeshoreProfiles[1];

            const string unaffectedProfileName = "Custom Profile";
            const string unaffectedProfileId = "Custom ID";
            var unaffectedProfile = new TestForeshoreProfile(unaffectedProfileName, unaffectedProfileId);
            failureMechanism.ForeshoreProfiles.AddRange(new[]
            {
                unaffectedProfile
            }, sourceFilePath);
            var unaffectedCalculation = new TestClosingStructuresCalculation
            {
                InputParameters =
                {
                    ForeshoreProfile = unaffectedProfile
                },
                Output = new TestStructuresOutput()
            };
            failureMechanism.CalculationsGroup.Children.Add(unaffectedCalculation);

            var strategy = new ForeshoreProfileUpdateDataStrategy(failureMechanism);

            ICalculation<ICalculationInput>[] calculationsWithUpdatedForeshoreProfile =
                failureMechanism.Calculations
                                .Cast<ICalculation<ICalculationInput>>()
                                .Where(calc => ReferenceEquals(((IHasForeshoreProfile) calc.InputParameters).ForeshoreProfile,
                                                               profileToBeUpdated))
                                .ToArray();

            ICalculation<ICalculationInput>[] calculationsWithUpdatedForeshoreProfileWithOutputs =
                calculationsWithUpdatedForeshoreProfile.Where(calc => calc.HasOutput)
                                                       .ToArray();

            ICalculation<ICalculationInput>[] calculationsWithRemovedForeshoreProfile =
                failureMechanism.Calculations
                                .Cast<ICalculation<ICalculationInput>>()
                                .Where(calc => ReferenceEquals(((IHasForeshoreProfile) calc.InputParameters).ForeshoreProfile,
                                                               profileToBeRemoved))
                                .ToArray();

            ICalculation<ICalculationInput>[] calculationsWithRemovedForeshoreProfileWithOutputs =
                calculationsWithRemovedForeshoreProfile.Where(calc => calc.HasOutput)
                                                       .ToArray();

            // Call
            IEnumerable<IObservable> affectedObjects =
                strategy.UpdateForeshoreProfilesWithImportedData(failureMechanism.ForeshoreProfiles,
                                                                 new[]
                                                                 {
                                                                     profileToUpdateFrom,
                                                                     new TestForeshoreProfile(unaffectedProfileName,
                                                                                              unaffectedProfileId)
                                                                 },
                                                                 sourceFilePath);

            // Assert
            Assert.IsTrue(unaffectedCalculation.HasOutput);
            Assert.AreSame(unaffectedProfile, unaffectedCalculation.InputParameters.ForeshoreProfile);

            Assert.IsTrue(calculationsWithUpdatedForeshoreProfile.All(calc => !calc.HasOutput));
            Assert.IsTrue(calculationsWithUpdatedForeshoreProfile.All(calc => ReferenceEquals(GetForeshoreProfile(calc),
                                                                                              profileToBeUpdated)));

            Assert.IsTrue(calculationsWithRemovedForeshoreProfile.All(calc => !calc.HasOutput));
            Assert.IsTrue(calculationsWithRemovedForeshoreProfile.All(calc => GetForeshoreProfile(calc) == null));

            var expectedAffectedObjects = new List<IObservable>
            {
                failureMechanism.ForeshoreProfiles,
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
    }
}