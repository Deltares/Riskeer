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
using System.Linq;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.Structures;
using Riskeer.Common.Data.TestUtil;
using Riskeer.StabilityPointStructures.Data;
using Riskeer.StabilityPointStructures.Data.TestUtil;
using Riskeer.Storage.Core.Create;
using Riskeer.Storage.Core.Create.StabilityPointStructures;
using Riskeer.Storage.Core.DbContext;

namespace Riskeer.Storage.Core.Test.Create.StabilityPointStructures
{
    [TestFixture]
    public class StabilityPointStructuresFailureMechanismCreateExtensionsTest
    {
        [Test]
        public void Create_PersistenceRegistryNull_ThrowsArgumentNullException()
        {
            // Setup
            var failureMechanism = new StabilityPointStructuresFailureMechanism();

            // Call
            TestDelegate test = () => failureMechanism.Create(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("registry", paramName);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Create_WithCollectorAndPropertiesSet_ReturnsExpectedEntity(bool inAssembly)
        {
            // Setup
            var failureMechanism = new StabilityPointStructuresFailureMechanism
            {
                InAssembly = inAssembly,
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
                    Body = "Some calculation text"
                },
                GeneralInput =
                {
                    N = new Random().NextRoundedDouble(1, 20)
                }
            };
            var registry = new PersistenceRegistry();

            // Call
            FailureMechanismEntity entity = failureMechanism.Create(registry);

            // Assert
            Assert.IsNotNull(entity);
            Assert.AreEqual((short) FailureMechanismType.StabilityPointStructures, entity.FailureMechanismType);
            Assert.AreEqual(Convert.ToByte(inAssembly), entity.InAssembly);
            Assert.AreEqual(failureMechanism.InAssemblyInputComments.Body, entity.InAssemblyInputComments);
            Assert.AreEqual(failureMechanism.InAssemblyOutputComments.Body, entity.InAssemblyOutputComments);
            Assert.AreEqual(failureMechanism.NotInAssemblyComments.Body, entity.NotInAssemblyComments);
            Assert.AreEqual(failureMechanism.CalculationsInputComments.Body, entity.CalculationsInputComments);

            StabilityPointStructuresFailureMechanismMetaEntity metaEntity = entity.StabilityPointStructuresFailureMechanismMetaEntities.Single();
            Assert.AreEqual(failureMechanism.GeneralInput.N, metaEntity.N);
        }

        [Test]
        public void Create_StringPropertiesDoNotShareReference()
        {
            // Setup
            const string originalInAssemblyInputComments = "Some input text";
            const string originalInAssemblyOutputComments = "Some output text";
            const string originalNotInAssemblyComments = "Really not in assembly";
            const string originalCalculationsInputComments = "Some calculation text";
            var failureMechanism = new StabilityPointStructuresFailureMechanism
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
            var failureMechanism = new StabilityPointStructuresFailureMechanism();

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
            const string sourcePath = "failureMechanismSections/File/Path";
            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            failureMechanism.SetSections(new[]
            {
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection()
            }, sourcePath);

            // Call
            FailureMechanismEntity entity = failureMechanism.Create(new PersistenceRegistry());

            // Assert
            Assert.AreEqual(1, entity.FailureMechanismSectionEntities.Count);
            Assert.AreEqual(1, entity.FailureMechanismSectionEntities.SelectMany(fms => fms.StabilityPointStructuresSectionResultEntities).Count());
            TestHelper.AssertAreEqualButNotSame(sourcePath, entity.FailureMechanismSectionCollectionSourcePath);
        }

        [Test]
        public void Create_WithoutForeshoreProfiles_EmptyForeshoreProfilesEntities()
        {
            // Setup
            var failureMechanism = new StabilityPointStructuresFailureMechanism();

            // Call
            FailureMechanismEntity entity = failureMechanism.Create(new PersistenceRegistry());

            // Assert
            CollectionAssert.IsEmpty(entity.ForeshoreProfileEntities);

            StabilityPointStructuresFailureMechanismMetaEntity metaEntity =
                entity.StabilityPointStructuresFailureMechanismMetaEntities.Single();
            Assert.IsNull(metaEntity.ForeshoreProfileCollectionSourcePath);
        }

        [Test]
        public void Create_WithForeshoreProfiles_ForeshoreProfileEntitiesCreated()
        {
            // Setup
            var profile = new TestForeshoreProfile();

            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            const string filePath = "some/path/to/foreshoreProfiles";
            failureMechanism.ForeshoreProfiles.AddRange(new[]
            {
                profile
            }, filePath);

            var persistenceRegistry = new PersistenceRegistry();

            // Call
            FailureMechanismEntity entity = failureMechanism.Create(persistenceRegistry);

            // Assert
            Assert.AreEqual(1, entity.ForeshoreProfileEntities.Count);
            Assert.IsTrue(persistenceRegistry.Contains(profile));

            StabilityPointStructuresFailureMechanismMetaEntity metaEntity =
                entity.StabilityPointStructuresFailureMechanismMetaEntities.Single();
            string metaEntityForeshoreProfileCollectionSourcePath = metaEntity.ForeshoreProfileCollectionSourcePath;
            TestHelper.AssertAreEqualButNotSame(filePath, metaEntityForeshoreProfileCollectionSourcePath);
        }

        [Test]
        public void Create_WithoutStabilityPointStructures_EmptyStabilityPointStructuresEntities()
        {
            // Setup
            var failureMechanism = new StabilityPointStructuresFailureMechanism();

            // Call
            FailureMechanismEntity entity = failureMechanism.Create(new PersistenceRegistry());

            // Assert
            CollectionAssert.IsEmpty(entity.StabilityPointStructureEntities);

            StabilityPointStructuresFailureMechanismMetaEntity metaEntity =
                entity.StabilityPointStructuresFailureMechanismMetaEntities.Single();
            Assert.IsNull(metaEntity.StabilityPointStructureCollectionSourcePath);
        }

        [Test]
        public void Create_WithStabilityPointStructures_StabilityPointStructureEntitiesCreated()
        {
            // Setup
            StabilityPointStructure structure = new TestStabilityPointStructure();

            const string filePath = "path/to/structures";
            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            failureMechanism.StabilityPointStructures.AddRange(new[]
            {
                structure
            }, filePath);

            var persistenceRegistry = new PersistenceRegistry();

            // Call
            FailureMechanismEntity entity = failureMechanism.Create(persistenceRegistry);

            // Assert
            Assert.AreEqual(1, entity.StabilityPointStructureEntities.Count);
            Assert.IsTrue(persistenceRegistry.Contains(structure));

            StabilityPointStructuresFailureMechanismMetaEntity metaEntity =
                entity.StabilityPointStructuresFailureMechanismMetaEntities.Single();
            TestHelper.AssertAreEqualButNotSame(filePath, metaEntity.StabilityPointStructureCollectionSourcePath);
        }

        [Test]
        public void Create_WithCalculationGroup_ReturnFailureMechanismEntityWithCalculationGroupEntities()
        {
            // Setup
            StructuresCalculationScenario<StabilityPointStructuresInput> calculation = new TestStabilityPointStructuresCalculationScenario();
            calculation.InputParameters.Structure = null;
            calculation.InputParameters.HydraulicBoundaryLocation = null;

            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(new CalculationGroup
            {
                Name = "A"
            });
            failureMechanism.CalculationsGroup.Children.Add(calculation);

            var registry = new PersistenceRegistry();

            // Call
            FailureMechanismEntity entity = failureMechanism.Create(registry);

            // Assert
            Assert.IsNotNull(entity);
            Assert.AreEqual(failureMechanism.CalculationsGroup.Name, entity.CalculationGroupEntity.Name);
            Assert.AreEqual(0, entity.CalculationGroupEntity.Order);

            CalculationGroupEntity[] childGroupEntities = entity.CalculationGroupEntity.CalculationGroupEntity1
                                                                .OrderBy(cge => cge.Order)
                                                                .ToArray();
            Assert.AreEqual(1, childGroupEntities.Length);
            CalculationGroupEntity childGroupEntity = childGroupEntities[0];
            Assert.AreEqual("A", childGroupEntity.Name);
            Assert.AreEqual(0, childGroupEntity.Order);

            StabilityPointStructuresCalculationEntity[] calculationEntities = entity.CalculationGroupEntity.StabilityPointStructuresCalculationEntities
                                                                                    .OrderBy(ce => ce.Order)
                                                                                    .ToArray();
            StabilityPointStructuresCalculationEntity calculationEntity = calculationEntities[0];
            Assert.AreEqual("Nieuwe berekening", calculationEntity.Name);
            Assert.AreEqual(1, calculationEntity.Order);
        }
    }
}