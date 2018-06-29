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
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.DuneErosion.Data;
using Ringtoets.DuneErosion.Data.TestUtil;
using Ringtoets.Storage.Core.Create;
using Ringtoets.Storage.Core.Create.DuneErosion;
using Ringtoets.Storage.Core.DbContext;
using Ringtoets.Storage.Core.TestUtil;

namespace Ringtoets.Storage.Core.Test.Create.DuneErosion
{
    [TestFixture]
    public class DuneErosionFailureMechanismCreateExtensionsTest
    {
        [Test]
        public void Create_PersistenceRegistryNull_ThrowsArgumentNullException()
        {
            // Setup
            var failureMechanism = new DuneErosionFailureMechanism();

            // Call
            TestDelegate test = () => failureMechanism.Create(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("registry", paramName);
        }

        [Test]
        public void Create_WithCollectorAndPropertiesSet_ReturnsFailureMechanismEntityWithPropertiesSet()
        {
            // Setup
            var random = new Random(21);
            var failureMechanism = new DuneErosionFailureMechanism
            {
                IsRelevant = random.NextBoolean(),
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
                    N = random.NextRoundedDouble(1, 20)
                }
            };

            var registry = new PersistenceRegistry();

            // Call
            FailureMechanismEntity entity = failureMechanism.Create(registry);

            // Assert
            Assert.IsNotNull(entity);
            Assert.AreEqual((short) FailureMechanismType.DuneErosion, entity.FailureMechanismType);
            Assert.AreEqual(Convert.ToByte(failureMechanism.IsRelevant), entity.IsRelevant);
            Assert.AreEqual(failureMechanism.InputComments.Body, entity.InputComments);
            Assert.AreEqual(failureMechanism.OutputComments.Body, entity.OutputComments);
            Assert.AreEqual(failureMechanism.NotRelevantComments.Body, entity.NotRelevantComments);

            DuneErosionFailureMechanismMetaEntity metaEntity = entity.DuneErosionFailureMechanismMetaEntities.First();
            Assert.AreEqual(failureMechanism.GeneralInput.N, metaEntity.N, failureMechanism.GeneralInput.N.GetAccuracy());
        }

        [Test]
        public void Create_StringPropertiesDoNotShareReference()
        {
            // Setup
            const string originalInput = "Some input text";
            const string originalOutput = "Some output text";
            const string originalNotRelevantText = "Really not relevant";
            var failureMechanism = new DuneErosionFailureMechanism
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
            var failureMechanism = new DuneErosionFailureMechanism();

            // Call
            FailureMechanismEntity entity = failureMechanism.Create(new PersistenceRegistry());

            // Assert
            CollectionAssert.IsEmpty(entity.FailureMechanismSectionEntities);
        }

        [Test]
        public void Create_WithSections_FailureMechanismSectionEntitiesCreated()
        {
            // Setup
            var failureMechanism = new DuneErosionFailureMechanism();
            failureMechanism.AddSection(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());

            // Call
            FailureMechanismEntity entity = failureMechanism.Create(new PersistenceRegistry());

            // Assert
            Assert.AreEqual(1, entity.FailureMechanismSectionEntities.Count);
            Assert.AreEqual(1, entity.FailureMechanismSectionEntities.SelectMany(fms => fms.DuneErosionSectionResultEntities).Count());
        }

        [Test]
        public void Create_WithoutDuneLocations_ReturnsEntityWithoutDuneLocations()
        {
            // Setup
            var failureMechanism = new DuneErosionFailureMechanism();
            var registry = new PersistenceRegistry();

            // Call
            FailureMechanismEntity entity = failureMechanism.Create(registry);

            // Assert
            CollectionAssert.IsEmpty(entity.DuneLocationEntities);

            DuneErosionFailureMechanismMetaEntity metaEntity = entity.DuneErosionFailureMechanismMetaEntities.Single();
            AssertDuneLocationCalculationCollectionEntities(failureMechanism, registry, metaEntity);
        }

        [Test]
        public void Create_WithDuneLocations_ReturnsEntityWithDuneLocations()
        {
            // Setup
            var duneLocation = new TestDuneLocation();
            var failureMechanism = new DuneErosionFailureMechanism();
            failureMechanism.SetDuneLocations(new[]
            {
                duneLocation
            });
            SetDuneLocationCalculationOutput(failureMechanism);

            var duneLocationEntity = new DuneLocationEntity();
            var registry = new PersistenceRegistry();
            registry.Register(duneLocationEntity, duneLocation);

            // Call
            FailureMechanismEntity entity = failureMechanism.Create(registry);

            // Assert
            DuneLocationEntity actualDuneLocationEntity = entity.DuneLocationEntities.Single();
            Assert.AreSame(duneLocationEntity, actualDuneLocationEntity);

            DuneErosionFailureMechanismMetaEntity metaEntity = entity.DuneErosionFailureMechanismMetaEntities.Single();
            AssertDuneLocationCalculationCollectionEntities(failureMechanism, registry, metaEntity);
        }

        private static void AssertDuneLocationCalculationCollectionEntities(DuneErosionFailureMechanism failureMechanism,
                                                                            PersistenceRegistry registry,
                                                                            DuneErosionFailureMechanismMetaEntity metaEntity)
        {
            AssertDuneLocationCalculationCollectionEntity(failureMechanism.CalculationsForFactorizedLowerLimitNorm,
                                                          registry,
                                                          metaEntity.DuneLocationCalculationCollectionEntity);
            AssertDuneLocationCalculationCollectionEntity(failureMechanism.CalculationsForLowerLimitNorm,
                                                          registry,
                                                          metaEntity.DuneLocationCalculationCollectionEntity1);
            AssertDuneLocationCalculationCollectionEntity(failureMechanism.CalculationsForMechanismSpecificLowerLimitNorm,
                                                          registry,
                                                          metaEntity.DuneLocationCalculationCollectionEntity2);
            AssertDuneLocationCalculationCollectionEntity(failureMechanism.CalculationsForMechanismSpecificSignalingNorm,
                                                          registry,
                                                          metaEntity.DuneLocationCalculationCollectionEntity3);
            AssertDuneLocationCalculationCollectionEntity(failureMechanism.CalculationsForMechanismSpecificFactorizedSignalingNorm,
                                                          registry,
                                                          metaEntity.DuneLocationCalculationCollectionEntity4);
        }

        private static void SetDuneLocationCalculationOutput(DuneErosionFailureMechanism failureMechanism)
        {
            SetDuneLocationCalculationOutput(failureMechanism.CalculationsForMechanismSpecificFactorizedSignalingNorm, 1);
            SetDuneLocationCalculationOutput(failureMechanism.CalculationsForMechanismSpecificSignalingNorm, 2);
            SetDuneLocationCalculationOutput(failureMechanism.CalculationsForMechanismSpecificLowerLimitNorm, 3);
            SetDuneLocationCalculationOutput(failureMechanism.CalculationsForLowerLimitNorm, 4);
            SetDuneLocationCalculationOutput(failureMechanism.CalculationsForFactorizedLowerLimitNorm, 5);
        }

        private static void SetDuneLocationCalculationOutput(IEnumerable<DuneLocationCalculation> calculations,
                                                             int seed)
        {
            var random = new Random(seed);
            foreach (DuneLocationCalculation calculation in calculations)
            {
                calculation.Output = random.NextBoolean()
                                         ? new TestDuneLocationCalculationOutput()
                                         : null;
            }
        }

        private static void AssertDuneLocationCalculationCollectionEntity(IEnumerable<DuneLocationCalculation> expectedCalculations,
                                                                          PersistenceRegistry registry,
                                                                          DuneLocationCalculationCollectionEntity actualCollectionEntity)
        {
            Assert.IsNotNull(actualCollectionEntity);
            ICollection<DuneLocationCalculationEntity> duneLocationCalculationEntities = actualCollectionEntity.DuneLocationCalculationEntities;
            Assert.AreEqual(expectedCalculations.Count(), duneLocationCalculationEntities.Count);

            var i = 0;
            foreach (DuneLocationCalculationEntity duneLocationCalculationEntity in duneLocationCalculationEntities)
            {
                DuneLocationCalculation expectedDuneLocationCalculation = expectedCalculations.ElementAt(i);
                DuneLocation expectedDuneLocation = expectedDuneLocationCalculation.DuneLocation;
                Assert.AreSame(registry.Get(expectedDuneLocation), duneLocationCalculationEntity.DuneLocationEntity);

                DuneLocationCalculationOutputEntity actualOutput = duneLocationCalculationEntity.DuneLocationCalculationOutputEntities.SingleOrDefault();
                if (expectedDuneLocationCalculation.Output != null)
                {
                    Assert.IsNotNull(actualOutput);
                }
                else
                {
                    Assert.IsNull(actualOutput);
                }

                i++;
            }
        }
    }
}