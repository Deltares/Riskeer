﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using System.Linq;
using Application.Ringtoets.Storage.TestUtil;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.HeightStructures.Data;
using Ringtoets.HeightStructures.Data.TestUtil;

namespace Ringtoets.Storage.Core.Test.Create.HeightStructures
{
    [TestFixture]
    public class HeightStructuresFailureMechanismCreateExtensionsTest
    {
        [Test]
        public void Create_PersistenceRegistryNull_ThrowsArgumentNullException()
        {
            // Setup
            var failureMechanism = new HeightStructuresFailureMechanism();

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
            var failureMechanism = new HeightStructuresFailureMechanism
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
                    N = new Random().NextRoundedDouble(1, 20)
                }
            };
            var registry = new PersistenceRegistry();

            // Call
            FailureMechanismEntity entity = failureMechanism.Create(registry);

            // Assert
            Assert.IsNotNull(entity);
            Assert.AreEqual((short) FailureMechanismType.StructureHeight, entity.FailureMechanismType);
            Assert.AreEqual(Convert.ToByte(isRelevant), entity.IsRelevant);
            Assert.AreEqual(failureMechanism.InputComments.Body, entity.InputComments);
            Assert.AreEqual(failureMechanism.OutputComments.Body, entity.OutputComments);
            Assert.AreEqual(failureMechanism.NotRelevantComments.Body, entity.NotRelevantComments);

            HeightStructuresFailureMechanismMetaEntity metaEntity = entity.HeightStructuresFailureMechanismMetaEntities.First();
            Assert.AreEqual(failureMechanism.GeneralInput.N, metaEntity.N);
            Assert.IsNull(metaEntity.HeightStructureCollectionSourcePath);
        }

        [Test]
        public void Create_StringPropertiesDoNotShareReference()
        {
            // Setup
            const string originalInput = "Some input text";
            const string originalOutput = "Some output text";
            const string originalNotRelevantText = "Really not relevant";
            const string originalSourcePath = "some output path";
            var failureMechanism = new HeightStructuresFailureMechanism
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
            failureMechanism.HeightStructures.AddRange(Enumerable.Empty<HeightStructure>(), originalSourcePath);
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

            HeightStructuresFailureMechanismMetaEntity metaEntity = entity.HeightStructuresFailureMechanismMetaEntities.First();
            Assert.AreEqual(originalSourcePath, metaEntity.HeightStructureCollectionSourcePath);
            Assert.AreNotSame(originalSourcePath, metaEntity.HeightStructureCollectionSourcePath,
                              "To create stable binary representations/fingerprints, it's really important that strings are not shared.");
        }

        [Test]
        public void Create_WithoutSections_EmptyFailureMechanismSectionEntities()
        {
            // Setup
            var failureMechanism = new HeightStructuresFailureMechanism();

            // Call
            FailureMechanismEntity entity = failureMechanism.Create(new PersistenceRegistry());

            // Assert
            CollectionAssert.IsEmpty(entity.FailureMechanismSectionEntities);
        }

        [Test]
        public void Create_WithSections_FailureMechanismSectionEntitiesCreated()
        {
            // Setup
            var failureMechanism = new HeightStructuresFailureMechanism();
            failureMechanism.AddSection(new TestFailureMechanismSection());

            // Call
            FailureMechanismEntity entity = failureMechanism.Create(new PersistenceRegistry());

            // Assert
            Assert.AreEqual(1, entity.FailureMechanismSectionEntities.Count);
            Assert.AreEqual(1, entity.FailureMechanismSectionEntities.SelectMany(fms => fms.HeightStructuresSectionResultEntities).Count());
        }

        [Test]
        public void Create_WithoutForeshoreProfiles_EmptyForeshoreProfilesEntities()
        {
            // Setup
            var failureMechanism = new HeightStructuresFailureMechanism();

            // Call
            FailureMechanismEntity entity = failureMechanism.Create(new PersistenceRegistry());

            // Assert
            CollectionAssert.IsEmpty(entity.ForeshoreProfileEntities);

            HeightStructuresFailureMechanismMetaEntity metaEntity =
                entity.HeightStructuresFailureMechanismMetaEntities.Single();
            Assert.IsNull(metaEntity.ForeshoreProfileCollectionSourcePath);
        }

        [Test]
        public void Create_WithForeshoreProfiles_ForeshoreProfileEntitiesCreated()
        {
            // Setup
            var profile = new TestForeshoreProfile();

            var failureMechanism = new HeightStructuresFailureMechanism();
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

            HeightStructuresFailureMechanismMetaEntity metaEntity =
                entity.HeightStructuresFailureMechanismMetaEntities.Single();
            string metaEntityForeshoreProfileCollectionSourcePath = metaEntity.ForeshoreProfileCollectionSourcePath;
            Assert.AreNotSame(filePath, metaEntityForeshoreProfileCollectionSourcePath);
            Assert.AreEqual(filePath, metaEntityForeshoreProfileCollectionSourcePath);
        }

        [Test]
        public void Create_WithoutHeightStructures_EmptyHeightStructureEntities()
        {
            // Setup
            var failureMechanism = new HeightStructuresFailureMechanism();

            var persistenceRegistry = new PersistenceRegistry();

            // Call
            FailureMechanismEntity entity = failureMechanism.Create(persistenceRegistry);

            // Assert
            Assert.AreEqual(0, entity.HeightStructureEntities.Count);

            HeightStructuresFailureMechanismMetaEntity metaEntity =
                entity.HeightStructuresFailureMechanismMetaEntities.Single();
            Assert.IsNull(metaEntity.HeightStructureCollectionSourcePath);
        }

        [Test]
        public void Create_WithoutHeightStructuresWithSourcePath_EmptyHeightStructureEntitiesWithSourcePath()
        {
            // Setup
            const string originalSourcePath = "some output path";
            var failureMechanism = new HeightStructuresFailureMechanism();
            failureMechanism.HeightStructures.AddRange(Enumerable.Empty<HeightStructure>(), originalSourcePath);

            var persistenceRegistry = new PersistenceRegistry();

            // Call
            FailureMechanismEntity entity = failureMechanism.Create(persistenceRegistry);

            // Assert
            Assert.AreEqual(0, entity.HeightStructureEntities.Count);

            HeightStructuresFailureMechanismMetaEntity metaEntity =
                entity.HeightStructuresFailureMechanismMetaEntities.Single();
            Assert.AreEqual(originalSourcePath, metaEntity.HeightStructureCollectionSourcePath);
        }

        [Test]
        public void Create_WithHeightStructures_HeightStructureEntitiesCreated()
        {
            // Setup
            HeightStructure structure = new TestHeightStructure();

            var failureMechanism = new HeightStructuresFailureMechanism();
            const string filePath = "some/path/to/structures";
            failureMechanism.HeightStructures.AddRange(new[]
            {
                structure
            }, filePath);

            var persistenceRegistry = new PersistenceRegistry();

            // Call
            FailureMechanismEntity entity = failureMechanism.Create(persistenceRegistry);

            // Assert
            Assert.AreEqual(1, entity.HeightStructureEntities.Count);
            Assert.IsTrue(persistenceRegistry.Contains(structure));

            HeightStructuresFailureMechanismMetaEntity metaEntity =
                entity.HeightStructuresFailureMechanismMetaEntities.Single();
            string metaEntityHeightStructureCollectionSourcePath = metaEntity.HeightStructureCollectionSourcePath;
            Assert.AreNotSame(filePath, metaEntityHeightStructureCollectionSourcePath);
            Assert.AreEqual(filePath, metaEntityHeightStructureCollectionSourcePath);
        }

        [Test]
        public void Create_WithCalculationGroup_ReturnFailureMechanismEntityWithCalculationGroupEntities()
        {
            // Setup
            var failureMechanism = new HeightStructuresFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(new CalculationGroup
            {
                Name = "A"
            });
            failureMechanism.CalculationsGroup.Children.Add(new CalculationGroup
            {
                Name = "B"
            });

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
            Assert.AreEqual(2, childGroupEntities.Length);
            CalculationGroupEntity childGroupEntity1 = childGroupEntities[0];
            Assert.AreEqual("A", childGroupEntity1.Name);
            Assert.AreEqual(0, childGroupEntity1.Order);
            CalculationGroupEntity childGroupEntity2 = childGroupEntities[1];
            Assert.AreEqual("B", childGroupEntity2.Name);
            Assert.AreEqual(1, childGroupEntity2.Order);
        }
    }
}