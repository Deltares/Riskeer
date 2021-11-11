// Copyright (C) Stichting Deltares 2021. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using Riskeer.Common.Data.TestUtil;
using Riskeer.DuneErosion.Data;
using Riskeer.DuneErosion.Data.TestUtil;
using Riskeer.Storage.Core.Create;
using Riskeer.Storage.Core.Create.DuneErosion;
using Riskeer.Storage.Core.DbContext;

namespace Riskeer.Storage.Core.Test.Create.DuneErosion
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
            void Call() => failureMechanism.Create(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(Call).ParamName;
            Assert.AreEqual("registry", paramName);
        }

        [Test]
        public void Create_WithCollectorAndPropertiesSet_ReturnsFailureMechanismEntityWithPropertiesSet()
        {
            // Setup
            var random = new Random(21);
            var failureMechanism = new DuneErosionFailureMechanism
            {
                InAssembly = random.NextBoolean(),
                InAssemblyInputComments =
                {
                    Body = "Some input text"
                },
                InAssemblyOutputComments =
                {
                    Body = "Some output text"
                },
                NotInAssemblyComments =
                {
                    Body = "Really not in assembly"
                },
                CalculationsInputComments =
                {
                    Body = "Some calculation comments"
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
            Assert.AreEqual(Convert.ToByte(failureMechanism.InAssembly), entity.InAssembly);
            Assert.AreEqual(failureMechanism.InAssemblyInputComments.Body, entity.InAssemblyInputComments);
            Assert.AreEqual(failureMechanism.InAssemblyOutputComments.Body, entity.InAssemblyOutputComments);
            Assert.AreEqual(failureMechanism.NotInAssemblyComments.Body, entity.NotInAssemblyComments);
            Assert.AreEqual(failureMechanism.CalculationsInputComments.Body, entity.CalculationsInputComments);

            DuneErosionFailureMechanismMetaEntity metaEntity = entity.DuneErosionFailureMechanismMetaEntities.Single();
            Assert.AreEqual(failureMechanism.GeneralInput.N, metaEntity.N, failureMechanism.GeneralInput.N.GetAccuracy());
        }

        [Test]
        public void Create_StringPropertiesDoNotShareReference()
        {
            // Setup
            const string originalInAssemblyInputComments = "Some input text";
            const string originalInAssemblyOutputComments = "Some output text";
            const string originalNotInAssemblyComments = "Really not in assembly";
            const string originalCalculationsInputComments = "Some calculation text";
            var failureMechanism = new DuneErosionFailureMechanism
            {
                InAssemblyInputComments =
                {
                    Body = originalInAssemblyInputComments
                },
                InAssemblyOutputComments =
                {
                    Body = originalInAssemblyOutputComments
                },
                NotInAssemblyComments =
                {
                    Body = originalNotInAssemblyComments
                },
                CalculationsInputComments =
                {
                    Body = originalCalculationsInputComments
                }
            };
            var registry = new PersistenceRegistry();

            // Call
            FailureMechanismEntity entity = failureMechanism.Create(registry);

            // Assert
            TestHelper.AssertAreEqualButNotSame(failureMechanism.InAssemblyInputComments.Body, entity.InAssemblyInputComments);
            TestHelper.AssertAreEqualButNotSame(failureMechanism.InAssemblyOutputComments.Body, entity.InAssemblyOutputComments);
            TestHelper.AssertAreEqualButNotSame(failureMechanism.NotInAssemblyComments.Body, entity.NotInAssemblyComments);
            TestHelper.AssertAreEqualButNotSame(failureMechanism.CalculationsInputComments.Body, entity.CalculationsInputComments);
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
            Assert.IsNull(entity.FailureMechanismSectionCollectionSourcePath);
        }

        [Test]
        public void Create_WithSections_FailureMechanismSectionEntitiesCreated()
        {
            // Setup
            const string filePath = "failureMechanismSections/file/path";
            var failureMechanism = new DuneErosionFailureMechanism();
            failureMechanism.SetSections(new[]
            {
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection()
            }, filePath);

            // Call
            FailureMechanismEntity entity = failureMechanism.Create(new PersistenceRegistry());

            // Assert
            Assert.AreEqual(1, entity.FailureMechanismSectionEntities.Count);
            Assert.AreEqual(1, entity.FailureMechanismSectionEntities.SelectMany(fms => fms.DuneErosionSectionResultEntities).Count());
            TestHelper.AssertAreEqualButNotSame(filePath, entity.FailureMechanismSectionCollectionSourcePath);
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
            AssertDuneLocationCalculationCollectionEntities(failureMechanism, metaEntity);
        }

        [Test]
        public void Create_WithDuneLocations_ReturnsEntityWithDuneLocations()
        {
            // Setup
            var duneLocation = new TestDuneLocation();
            var failureMechanism = new DuneErosionFailureMechanism
            {
                DuneLocationCalculationsForUserDefinedTargetProbabilities =
                {
                    new DuneLocationCalculationsForTargetProbability(0.1),
                    new DuneLocationCalculationsForTargetProbability(0.01)
                }
            };
            failureMechanism.SetDuneLocations(new[]
            {
                duneLocation
            });

            var duneLocationEntity = new DuneLocationEntity();
            var registry = new PersistenceRegistry();
            registry.Register(duneLocationEntity, duneLocation);

            // Call
            FailureMechanismEntity entity = failureMechanism.Create(registry);

            // Assert
            DuneLocationEntity actualDuneLocationEntity = entity.DuneLocationEntities.Single();
            Assert.AreSame(duneLocationEntity, actualDuneLocationEntity);

            DuneErosionFailureMechanismMetaEntity metaEntity = entity.DuneErosionFailureMechanismMetaEntities.Single();
            AssertDuneLocationCalculationCollectionEntities(failureMechanism, metaEntity);
        }

        private static void AssertDuneLocationCalculationCollectionEntities(DuneErosionFailureMechanism failureMechanism,
                                                                            DuneErosionFailureMechanismMetaEntity metaEntity)
        {
            AssertDuneLocationCalculationCollectionEntity(failureMechanism.DuneLocationCalculationsForUserDefinedTargetProbabilities,
                                                          metaEntity.DuneLocationCalculationForTargetProbabilityCollectionEntities);
        }

        private static void AssertDuneLocationCalculationCollectionEntity(
            IEnumerable<DuneLocationCalculationsForTargetProbability> expectedCalculationCollections,
            IEnumerable<DuneLocationCalculationForTargetProbabilityCollectionEntity> actualCalculationCollectionEntities)
        {
            Assert.AreEqual(expectedCalculationCollections.Count(), actualCalculationCollectionEntities.Count());

            for (var i = 0; i < expectedCalculationCollections.Count(); i++)
            {
                DuneLocationCalculationForTargetProbabilityCollectionEntity actualCalculationCollectionEntity = actualCalculationCollectionEntities.ElementAt(i);
                AssertDuneLocationCalculationCollectionEntity(expectedCalculationCollections.ElementAt(i), actualCalculationCollectionEntity);
            }
        }

        private static void AssertDuneLocationCalculationCollectionEntity(
            DuneLocationCalculationsForTargetProbability expectedCalculations,
            DuneLocationCalculationForTargetProbabilityCollectionEntity entity)
        {
            Assert.AreEqual(expectedCalculations.TargetProbability, entity.TargetProbability);
            Assert.AreEqual(expectedCalculations.DuneLocationCalculations.Count, entity.DuneLocationCalculationEntities.Count);
        }
    }
}