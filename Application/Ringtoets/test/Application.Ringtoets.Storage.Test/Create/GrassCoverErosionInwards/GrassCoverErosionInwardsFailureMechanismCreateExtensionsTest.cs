// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Application.Ringtoets.Storage.Create.GrassCoverErosionInwards;
using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.GrassCoverErosionInwards.Data;

namespace Application.Ringtoets.Storage.Test.Create.GrassCoverErosionInwards
{
    [TestFixture]
    public class GrassCoverErosionInwardsFailureMechanismCreateExtensionsTest
    {
        [Test]
        public void Create_WithoutPersistenceRegistry_ThrowsArgumentNullException()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

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
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism
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
                    N = 12
                }
            };
            var registry = new PersistenceRegistry();

            // Call
            var entity = failureMechanism.Create(registry);

            // Assert
            Assert.IsNotNull(entity);
            Assert.AreEqual((short) FailureMechanismType.GrassRevetmentTopErosionAndInwards, entity.FailureMechanismType);
            Assert.AreEqual(Convert.ToByte(isRelevant), entity.IsRelevant);
            Assert.AreEqual(failureMechanism.InputComments.Body, entity.InputComments);
            Assert.AreEqual(failureMechanism.OutputComments.Body, entity.OutputComments);
            Assert.AreEqual(failureMechanism.NotRelevantComments.Body, entity.NotRelevantComments);

            Assert.AreEqual(1, entity.GrassCoverErosionInwardsFailureMechanismMetaEntities.Count);
            GrassCoverErosionInwardsFailureMechanismMetaEntity generalInputEntity = entity.GrassCoverErosionInwardsFailureMechanismMetaEntities.First();
            Assert.AreEqual(failureMechanism.GeneralInput.N, generalInputEntity.N);
        }

        [Test]
        public void Create_StringPropertiesDoNotShareReference()
        {
            // Setup
            const string originalInput = "Some input text";
            const string originalOutput = "Some output text";
            const string originalNotRelevantText = "Really not relevant";
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism
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
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            // Call
            var entity = failureMechanism.Create(new PersistenceRegistry());

            // Assert
            Assert.IsEmpty(entity.FailureMechanismSectionEntities);
        }

        [Test]
        public void Create_WithSections_FailureMechanismSectionEntitiesCreated()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            failureMechanism.AddSection(new TestFailureMechanismSection());

            // Call
            var entity = failureMechanism.Create(new PersistenceRegistry());

            // Assert
            Assert.AreEqual(1, entity.FailureMechanismSectionEntities.Count);
            Assert.AreEqual(1, entity.FailureMechanismSectionEntities.SelectMany(fms => fms.GrassCoverErosionInwardsSectionResultEntities).Count());
        }

        [Test]
        public void Create_WithoutDikeProfiles_EmptyDikeProfileEntities()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var registry = new PersistenceRegistry();

            // Call
            var entity = failureMechanism.Create(registry);

            // Assert
            CollectionAssert.IsEmpty(entity.DikeProfileEntities);
        }

        [Test]
        public void Create_WithDikeProfiles_AddDikeProfileEntities()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            failureMechanism.DikeProfiles.Add(new TestDikeProfile());
            failureMechanism.DikeProfiles.Add(new TestDikeProfile());
            var registry = new PersistenceRegistry();

            // Call
            var entity = failureMechanism.Create(registry);

            // Assert
            Assert.AreEqual(2, entity.DikeProfileEntities.Count);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Create_WithCalculationGroup_ReturnFailureMechanismEntityWithCalculationGroupEntities(bool isRelevant)
        {
            // Setup
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
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
            Assert.AreEqual("A", childGroupEntities[0].Name);
            Assert.AreEqual(0, childGroupEntities[0].Order);
            Assert.AreEqual("B", childGroupEntities[1].Name);
            Assert.AreEqual(1, childGroupEntities[1].Order);
        }
    }
}