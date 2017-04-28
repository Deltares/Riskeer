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
using System.Linq;
using Application.Ringtoets.Storage.Create;
using Application.Ringtoets.Storage.Create.HeightStructures;
using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.HeightStructures.Data;
using Ringtoets.HeightStructures.Data.TestUtil;

namespace Application.Ringtoets.Storage.Test.Create.HeightStructures
{
    [TestFixture]
    public class HeightStructuresFailureMechanismCreateExtensionsTest
    {
        [Test]
        public void Create_WithoutPersistenceRegistry_ThrowsArgumentNullException()
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
                    N = 7
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
        }

        [Test]
        public void Create_StringPropertiesDoNotShareReference()
        {
            // Setup
            const string originalInput = "Some input text";
            const string originalOutput = "Some output text";
            const string originalNotRelevantText = "Really not relevant";
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
            var failureMechanism = new HeightStructuresFailureMechanism();

            // Call
            FailureMechanismEntity entity = failureMechanism.Create(new PersistenceRegistry());

            // Assert
            Assert.IsEmpty(entity.FailureMechanismSectionEntities);
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
        public void Create_WithForeshoreProfiles_ForeshoreProfileEntitiesCreated()
        {
            // Setup
            var profile = new TestForeshoreProfile();

            var failureMechanism = new HeightStructuresFailureMechanism();
            failureMechanism.ForeshoreProfiles.Add(profile);

            var persistenceRegistry = new PersistenceRegistry();

            // Call
            FailureMechanismEntity entity = failureMechanism.Create(persistenceRegistry);

            // Assert
            Assert.AreEqual(1, entity.ForeshoreProfileEntities.Count);
            Assert.IsTrue(persistenceRegistry.Contains(profile));
        }

        [Test]
        public void Create_WithHeightStructures_HeightStructureEntitiesCreated()
        {
            // Setup
            HeightStructure structure = new TestHeightStructure();

            var failureMechanism = new HeightStructuresFailureMechanism();
            failureMechanism.HeightStructures.Add(structure);

            var persistenceRegistry = new PersistenceRegistry();

            // Call
            FailureMechanismEntity entity = failureMechanism.Create(persistenceRegistry);

            // Assert
            Assert.AreEqual(1, entity.HeightStructureEntities.Count);
            Assert.IsTrue(persistenceRegistry.Contains(structure));
        }

        [Test]
        public void Create_WithCalculationGroup_ReturnFailureMechanismEntityWithCalculationGroupEntities()
        {
            // Setup
            var failureMechanism = new HeightStructuresFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(new CalculationGroup("A", true));
            failureMechanism.CalculationsGroup.Children.Add(new CalculationGroup("B", true));

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