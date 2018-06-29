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
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.Storage.Core.Create;
using Ringtoets.Storage.Core.Create.GrassCoverErosionOutwards;
using Ringtoets.Storage.Core.DbContext;
using Ringtoets.Storage.Core.TestUtil;

namespace Ringtoets.Storage.Core.Test.Create.GrassCoverErosionOutwards
{
    [TestFixture]
    public class GrassCoverErosionOutwardsFailureMechanismCreateExtensionsTest
    {
        [Test]
        public void Create_PersistenceRegistryNull_ThrowsArgumentNullException()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();

            // Call
            TestDelegate test = () => failureMechanism.Create(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("registry", paramName);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Create_WithCollectorAndPropertiesSet_ReturnsFailureMechanismEntityWithPropertiesSet(bool isRelevant)
        {
            // Setup
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism
            {
                IsRelevant = isRelevant,
                InputComments =
                {
                    Body = "Some input text"
                },
                OutputComments =
                {
                    Body = "Some output text"
                },
                NotRelevantComments =
                {
                    Body = "Really not relevant"
                },
                GeneralInput =
                {
                    N = new Random(39).NextRoundedDouble(1, 20)
                }
            };
            var registry = new PersistenceRegistry();

            // Call
            FailureMechanismEntity entity = failureMechanism.Create(registry);

            // Assert
            Assert.IsNotNull(entity);
            Assert.AreEqual((short) FailureMechanismType.GrassRevetmentErosionOutwards, entity.FailureMechanismType);
            Assert.AreEqual(Convert.ToByte(isRelevant), entity.IsRelevant);
            Assert.AreEqual(failureMechanism.InputComments.Body, entity.InputComments);
            Assert.AreEqual(failureMechanism.OutputComments.Body, entity.OutputComments);
            Assert.AreEqual(failureMechanism.NotRelevantComments.Body, entity.NotRelevantComments);

            Assert.AreEqual(1, entity.GrassCoverErosionOutwardsFailureMechanismMetaEntities.Count);
            GrassCoverErosionOutwardsFailureMechanismMetaEntity generalInputEntity = entity.GrassCoverErosionOutwardsFailureMechanismMetaEntities.First();
            Assert.AreEqual(failureMechanism.GeneralInput.N, generalInputEntity.N);
        }

        [Test]
        public void Create_StringPropertiesDoNotShareReference()
        {
            // Setup
            const string originalInput = "Some input text";
            const string originalOutput = "Some output text";
            const string originalNotRelevantText = "Really not relevant";
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism
            {
                InputComments =
                {
                    Body = originalInput
                },
                OutputComments =
                {
                    Body = originalOutput
                },
                NotRelevantComments =
                {
                    Body = originalNotRelevantText
                }
            };
            var registry = new PersistenceRegistry();

            // Call
            FailureMechanismEntity entity = failureMechanism.Create(registry);

            // Assert
            Assert.AreNotSame(originalInput, entity.InputComments,
                              "To create stable binary representations/fingerprints, it's really important that strings are not shared.");
            Assert.AreEqual(failureMechanism.InputComments.Body, entity.InputComments);
            Assert.AreNotSame(originalOutput, entity.OutputComments,
                              "To create stable binary representations/fingerprints, it's really important that strings are not shared.");
            Assert.AreEqual(failureMechanism.OutputComments.Body, entity.OutputComments);
            Assert.AreNotSame(originalNotRelevantText, entity.NotRelevantComments,
                              "To create stable binary representations/fingerprints, it's really important that strings are not shared.");
            Assert.AreEqual(failureMechanism.NotRelevantComments.Body, entity.NotRelevantComments);
        }

        [Test]
        public void Create_WithoutSections_EmptyFailureMechanismSectionEntities()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();

            // Call
            FailureMechanismEntity entity = failureMechanism.Create(new PersistenceRegistry());

            // Assert
            CollectionAssert.IsEmpty(entity.FailureMechanismSectionEntities);
        }

        [Test]
        public void Create_WithSections_FailureMechanismSectionEntitiesCreated()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            failureMechanism.AddSection(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());

            // Call
            FailureMechanismEntity entity = failureMechanism.Create(new PersistenceRegistry());

            // Assert
            Assert.AreEqual(1, entity.FailureMechanismSectionEntities.Count);
            Assert.AreEqual(1, entity.FailureMechanismSectionEntities.SelectMany(fms => fms.GrassCoverErosionOutwardsSectionResultEntities).Count());
        }

        [Test]
        public void Create_WithoutForeshoreProfiles_EmptyForeshoreProfilesEntities()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();

            // Call
            FailureMechanismEntity entity = failureMechanism.Create(new PersistenceRegistry());

            // Assert
            CollectionAssert.IsEmpty(entity.ForeshoreProfileEntities);

            GrassCoverErosionOutwardsFailureMechanismMetaEntity metaEntity =
                entity.GrassCoverErosionOutwardsFailureMechanismMetaEntities.Single();
            Assert.IsNull(metaEntity.ForeshoreProfileCollectionSourcePath);
        }

        [Test]
        public void Create_WithForeshoreProfiles_ForeshoreProfilesEntitiesCreated()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            const string filePath = "some/path/to/foreshoreProfiles";
            failureMechanism.ForeshoreProfiles.AddRange(new[]
            {
                new TestForeshoreProfile()
            }, filePath);

            // Call
            FailureMechanismEntity entity = failureMechanism.Create(new PersistenceRegistry());

            // Assert
            Assert.AreEqual(1, entity.ForeshoreProfileEntities.Count);

            GrassCoverErosionOutwardsFailureMechanismMetaEntity metaEntity =
                entity.GrassCoverErosionOutwardsFailureMechanismMetaEntities.Single();
            string metaEntityForeshoreProfileCollectionSourcePath = metaEntity.ForeshoreProfileCollectionSourcePath;
            TestHelper.AssertAreEqualButNotSame(filePath, metaEntityForeshoreProfileCollectionSourcePath);
        }

        [Test]
        [TestCase(true, TestName = "Create_WithCalculationGroup_ReturnFMEntityWithCalculationGroupEntities(true)")]
        [TestCase(false, TestName = "Create_WithCalculationGroup_ReturnFMEntityWithCalculationGroupEntities(false)")]
        public void Create_WithCalculationGroup_ReturnFailureMechanismEntityWithCalculationGroupEntities(bool isRelevant)
        {
            // Setup
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            failureMechanism.WaveConditionsCalculationGroup.Children.Add(new CalculationGroup
            {
                Name = "A"
            });
            failureMechanism.WaveConditionsCalculationGroup.Children.Add(new CalculationGroup
            {
                Name = "B"
            });

            var registry = new PersistenceRegistry();

            // Call
            FailureMechanismEntity entity = failureMechanism.Create(registry);

            // Assert
            Assert.IsNotNull(entity);
            Assert.AreEqual(failureMechanism.WaveConditionsCalculationGroup.Name, entity.CalculationGroupEntity.Name);
            Assert.AreEqual(0, entity.CalculationGroupEntity.Order);

            CalculationGroupEntity[] childGroupEntities = entity.CalculationGroupEntity.CalculationGroupEntity1
                                                                .OrderBy(cge => cge.Order)
                                                                .ToArray();
            Assert.AreEqual(2, childGroupEntities.Length);
            Assert.AreEqual("A", childGroupEntities[0].Name);
            Assert.AreEqual(0, childGroupEntities[0].Order);
            Assert.AreEqual("B", childGroupEntities[1].Name);
            Assert.AreEqual(1, childGroupEntities[1].Order);
        }

        [Test]
        public void Create_WithoutHydraulicBoundaryLocationCalculations_ReturnsFailureMechanismWithoutHydraulicLocationCalculationEntities()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            var registry = new PersistenceRegistry();

            // Call
            FailureMechanismEntity entity = failureMechanism.Create(registry);

            // Assert
            GrassCoverErosionOutwardsFailureMechanismMetaEntity metaEntity =
                entity.GrassCoverErosionOutwardsFailureMechanismMetaEntities.Single();

            AssertHydraulicLocationCalculationCollectionEntities(failureMechanism, registry, metaEntity);
        }

        [Test]
        public void Create_WithHydraulicBoundaryLocationCalculations_ReturnsFailureMechanismWithHydraulicLocationCalculationEntities()
        {
            // Setup
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(0, "A", 0, 0);
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            failureMechanism.SetHydraulicBoundaryLocationCalculations(new[]
            {
                hydraulicBoundaryLocation
            });
            SetHydraulicBoundaryLocationCalculationInputsOfFailureMechanism(failureMechanism);

            var registry = new PersistenceRegistry();
            registry.Register(new HydraulicLocationEntity(), hydraulicBoundaryLocation);

            // Call
            FailureMechanismEntity entity = failureMechanism.Create(registry);

            // Assert
            GrassCoverErosionOutwardsFailureMechanismMetaEntity metaEntity =
                entity.GrassCoverErosionOutwardsFailureMechanismMetaEntities.Single();

            AssertHydraulicLocationCalculationCollectionEntities(failureMechanism, registry, metaEntity);
        }

        private static void AssertHydraulicLocationCalculationCollectionEntities(GrassCoverErosionOutwardsFailureMechanism expectedFailureMechanism,
                                                                                 PersistenceRegistry registry,
                                                                                 GrassCoverErosionOutwardsFailureMechanismMetaEntity actualEntity)
        {
            AssertHydraulicLocationCalculationCollectionEntity(expectedFailureMechanism.WaveHeightCalculationsForMechanismSpecificLowerLimitNorm,
                                                               registry,
                                                               actualEntity.HydraulicLocationCalculationCollectionEntity);
            AssertHydraulicLocationCalculationCollectionEntity(expectedFailureMechanism.WaveHeightCalculationsForMechanismSpecificSignalingNorm,
                                                               registry,
                                                               actualEntity.HydraulicLocationCalculationCollectionEntity1);
            AssertHydraulicLocationCalculationCollectionEntity(expectedFailureMechanism.WaveHeightCalculationsForMechanismSpecificFactorizedSignalingNorm,
                                                               registry,
                                                               actualEntity.HydraulicLocationCalculationCollectionEntity2);

            AssertHydraulicLocationCalculationCollectionEntity(expectedFailureMechanism.WaterLevelCalculationsForMechanismSpecificLowerLimitNorm,
                                                               registry,
                                                               actualEntity.HydraulicLocationCalculationCollectionEntity3);
            AssertHydraulicLocationCalculationCollectionEntity(expectedFailureMechanism.WaterLevelCalculationsForMechanismSpecificSignalingNorm,
                                                               registry,
                                                               actualEntity.HydraulicLocationCalculationCollectionEntity4);
            AssertHydraulicLocationCalculationCollectionEntity(expectedFailureMechanism.WaterLevelCalculationsForMechanismSpecificFactorizedSignalingNorm,
                                                               registry,
                                                               actualEntity.HydraulicLocationCalculationCollectionEntity5);
        }

        private static void SetHydraulicBoundaryLocationCalculationInputsOfFailureMechanism(GrassCoverErosionOutwardsFailureMechanism failureMechanism)
        {
            SetHydraulicBoundaryLocationCalculationInputs(failureMechanism.WaterLevelCalculationsForMechanismSpecificFactorizedSignalingNorm, 1);
            SetHydraulicBoundaryLocationCalculationInputs(failureMechanism.WaterLevelCalculationsForMechanismSpecificSignalingNorm, 2);
            SetHydraulicBoundaryLocationCalculationInputs(failureMechanism.WaterLevelCalculationsForMechanismSpecificLowerLimitNorm, 3);

            SetHydraulicBoundaryLocationCalculationInputs(failureMechanism.WaveHeightCalculationsForMechanismSpecificFactorizedSignalingNorm, 4);
            SetHydraulicBoundaryLocationCalculationInputs(failureMechanism.WaveHeightCalculationsForMechanismSpecificSignalingNorm, 5);
            SetHydraulicBoundaryLocationCalculationInputs(failureMechanism.WaveHeightCalculationsForMechanismSpecificLowerLimitNorm, 6);
        }

        private static void SetHydraulicBoundaryLocationCalculationInputs(IEnumerable<HydraulicBoundaryLocationCalculation> calculations,
                                                                          int seed)
        {
            var random = new Random(seed);
            foreach (HydraulicBoundaryLocationCalculation calculation in calculations)
            {
                calculation.InputParameters.ShouldIllustrationPointsBeCalculated = random.NextBoolean();
            }
        }

        private static void AssertHydraulicLocationCalculationCollectionEntity(IEnumerable<HydraulicBoundaryLocationCalculation> expectedCalculations,
                                                                               PersistenceRegistry registry,
                                                                               HydraulicLocationCalculationCollectionEntity actualCollectionEntity)
        {
            Assert.IsNotNull(actualCollectionEntity);

            HydraulicBoundaryLocationCalculation[] expectedCalculationsArray = expectedCalculations.ToArray();
            ICollection<HydraulicLocationCalculationEntity> hydraulicLocationCalculationEntities = actualCollectionEntity.HydraulicLocationCalculationEntities;
            Assert.AreEqual(expectedCalculationsArray.Length, hydraulicLocationCalculationEntities.Count);

            var i = 0;
            foreach (HydraulicLocationCalculationEntity actualCalculationEntity in hydraulicLocationCalculationEntities)
            {
                HydraulicBoundaryLocationCalculation expectedCalculation = expectedCalculationsArray[i];
                HydraulicBoundaryLocation expectedLocation = expectedCalculation.HydraulicBoundaryLocation;
                Assert.AreSame(registry.Get(expectedLocation), actualCalculationEntity.HydraulicLocationEntity);

                Assert.AreEqual(Convert.ToByte(expectedCalculation.InputParameters.ShouldIllustrationPointsBeCalculated),
                                actualCalculationEntity.ShouldIllustrationPointsBeCalculated);
                i++;
            }
        }
    }
}