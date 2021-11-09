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
using Riskeer.Common.Data.TestUtil;
using Riskeer.GrassCoverErosionOutwards.Data;
using Riskeer.Storage.Core.Create;
using Riskeer.Storage.Core.Create.GrassCoverErosionOutwards;
using Riskeer.Storage.Core.DbContext;

namespace Riskeer.Storage.Core.Test.Create.GrassCoverErosionOutwards
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
        public void Create_WithCollectorAndPropertiesSet_ReturnsFailureMechanismEntityWithPropertiesSet(bool inAssembly)
        {
            // Setup
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism
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
                CalculationsComments =
                {
                    Body = "Some calculation text"
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
            Assert.AreEqual(Convert.ToByte(inAssembly), entity.InAssembly);
            Assert.AreEqual(failureMechanism.InAssemblyInputComments.Body, entity.InAssemblyInputComments);
            Assert.AreEqual(failureMechanism.InAssemblyOutputComments.Body, entity.InAssemblyOutputComments);
            Assert.AreEqual(failureMechanism.NotInAssemblyComments.Body, entity.NotInAssemblyComments);
            Assert.AreEqual(failureMechanism.CalculationsComments.Body, entity.CalculationsComments);

            Assert.AreEqual(1, entity.GrassCoverErosionOutwardsFailureMechanismMetaEntities.Count);
            GrassCoverErosionOutwardsFailureMechanismMetaEntity generalInputEntity = entity.GrassCoverErosionOutwardsFailureMechanismMetaEntities.Single();
            Assert.AreEqual(failureMechanism.GeneralInput.N, generalInputEntity.N);
        }

        [Test]
        public void Create_StringPropertiesDoNotShareReference()
        {
            // Setup
            const string originalInput = "Some input text";
            const string originalOutput = "Some output text";
            const string originalNotInAssemblyText = "Really not in assembly";
            const string originalCalculationsText = "Some calculation text";
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism
            {
                InAssemblyInputComments =
                {
                    Body = originalInput
                },
                InAssemblyOutputComments =
                {
                    Body = originalOutput
                },
                NotInAssemblyComments =
                {
                    Body = originalNotInAssemblyText
                },
                CalculationsComments =
                {
                    Body = originalCalculationsText
                }
            };
            var registry = new PersistenceRegistry();

            // Call
            FailureMechanismEntity entity = failureMechanism.Create(registry);

            // Assert
            TestHelper.AssertAreEqualButNotSame(failureMechanism.InAssemblyInputComments.Body, entity.InAssemblyInputComments);
            TestHelper.AssertAreEqualButNotSame(failureMechanism.InAssemblyOutputComments.Body, entity.InAssemblyOutputComments);
            TestHelper.AssertAreEqualButNotSame(failureMechanism.NotInAssemblyComments.Body, entity.NotInAssemblyComments);
            TestHelper.AssertAreEqualButNotSame(failureMechanism.CalculationsComments.Body, entity.CalculationsComments);
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
            Assert.IsNull(entity.FailureMechanismSectionCollectionSourcePath);
        }

        [Test]
        public void Create_WithSections_FailureMechanismSectionEntitiesCreated()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            const string filePath = "failureMechanismSections/file/path";
            failureMechanism.SetSections(new[]
            {
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection()
            }, filePath);

            // Call
            FailureMechanismEntity entity = failureMechanism.Create(new PersistenceRegistry());

            // Assert
            Assert.AreEqual(1, entity.FailureMechanismSectionEntities.Count);
            Assert.AreEqual(1, entity.FailureMechanismSectionEntities.SelectMany(fms => fms.GrassCoverErosionOutwardsSectionResultEntities).Count());
            TestHelper.AssertAreEqualButNotSame(failureMechanism.FailureMechanismSectionSourcePath, entity.FailureMechanismSectionCollectionSourcePath);
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
        public void Create_WithCalculationGroup_ReturnFailureMechanismEntityWithCalculationGroupEntities(bool inAssembly)
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
    }
}