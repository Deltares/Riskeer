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
using Riskeer.Common.Data.TestUtil;
using Riskeer.Integration.Data.StandAlone;
using Riskeer.Storage.Core.Create;
using Riskeer.Storage.Core.Create.MacroStabilityOutwards;
using Riskeer.Storage.Core.DbContext;

namespace Riskeer.Storage.Core.Test.Create.MacroStabilityOutwards
{
    [TestFixture]
    public class MacroStabilityOutwardsFailureMechanismCreateExtensionsTest
    {
        [Test]
        public void Create_PersistenceRegistryNull_ThrowsArgumentNullException()
        {
            // Setup
            var failureMechanism = new MacroStabilityOutwardsFailureMechanism();

            // Call
            TestDelegate test = () => failureMechanism.Create(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("registry", paramName);
        }

        [Test]
        public void Create_WithoutAllPropertiesSet_ReturnsFailureMechanismEntityWithPropertiesSet()
        {
            // Setup
            var failureMechanism = new MacroStabilityOutwardsFailureMechanism();
            var registry = new PersistenceRegistry();

            // Call
            FailureMechanismEntity entity = failureMechanism.Create(registry);

            // Assert
            Assert.IsNotNull(entity);
            Assert.AreEqual((short) FailureMechanismType.MacroStabilityOutwards, entity.FailureMechanismType);
            Assert.AreEqual(Convert.ToByte(failureMechanism.InAssembly), entity.InAssembly);
            Assert.AreEqual(failureMechanism.InAssemblyInputComments.Body, entity.InAssemblyInputComments);
            Assert.AreEqual(failureMechanism.InAssemblyOutputComments.Body, entity.InAssemblyOutputComments);
            Assert.AreEqual(failureMechanism.NotInAssemblyComments.Body, entity.NotInAssemblyComments);
            Assert.AreEqual(failureMechanism.CalculationsComments.Body, entity.CalculationsComments);

            CollectionAssert.IsEmpty(entity.StochasticSoilModelEntities);
            MacroStabilityOutwardsFailureMechanismMetaEntity failureMechanismMetaEntity = entity.MacroStabilityOutwardsFailureMechanismMetaEntities.Single();
            Assert.AreEqual(failureMechanism.MacroStabilityOutwardsProbabilityAssessmentInput.A, failureMechanismMetaEntity.A);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Create_WithCollectorAndPropertiesSet_ReturnsFailureMechanismEntityWithPropertiesSet(bool inAssembly)
        {
            // Setup
            var random = new Random(31);
            var failureMechanism = new MacroStabilityOutwardsFailureMechanism
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
                MacroStabilityOutwardsProbabilityAssessmentInput =
                {
                    A = random.NextDouble()
                }
            };
            var registry = new PersistenceRegistry();

            // Call
            FailureMechanismEntity entity = failureMechanism.Create(registry);

            // Assert
            Assert.IsNotNull(entity);
            Assert.AreEqual((short) FailureMechanismType.MacroStabilityOutwards, entity.FailureMechanismType);
            Assert.AreEqual(Convert.ToByte(inAssembly), entity.InAssembly);
            Assert.AreEqual(failureMechanism.InAssemblyInputComments.Body, entity.InAssemblyInputComments);
            Assert.AreEqual(failureMechanism.InAssemblyOutputComments.Body, entity.InAssemblyOutputComments);
            Assert.AreEqual(failureMechanism.NotInAssemblyComments.Body, entity.NotInAssemblyComments);
            Assert.AreEqual(failureMechanism.CalculationsComments.Body, entity.CalculationsComments);
            MacroStabilityOutwardsFailureMechanismMetaEntity failureMechanismMetaEntity = entity.MacroStabilityOutwardsFailureMechanismMetaEntities.Single();
            Assert.AreEqual(failureMechanism.MacroStabilityOutwardsProbabilityAssessmentInput.A, failureMechanismMetaEntity.A);
        }

        [Test]
        public void Create_StringPropertiesDoNotShareReference()
        {
            // Setup
            const string originalInput = "Some input text";
            const string originalOutput = "Some output text";
            const string originalNotInAssemblyText = "Really not in assembly";
            const string originalCalculationsText = "Some calculation text";
            var failureMechanism = new MacroStabilityOutwardsFailureMechanism
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
            TestHelper.AssertAreEqualButNotSame(originalInput, entity.InAssemblyInputComments);
            TestHelper.AssertAreEqualButNotSame(originalOutput, entity.InAssemblyOutputComments);
            TestHelper.AssertAreEqualButNotSame(originalNotInAssemblyText, entity.NotInAssemblyComments);
            TestHelper.AssertAreEqualButNotSame(originalCalculationsText, entity.CalculationsComments);
        }

        [Test]
        public void Create_WithoutSections_EmptyFailureMechanismSectionEntities()
        {
            // Setup
            var failureMechanism = new MacroStabilityOutwardsFailureMechanism();

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
            const string filePath = "failureMechanismSection/File/Path";
            var failureMechanism = new MacroStabilityOutwardsFailureMechanism();
            failureMechanism.SetSections(new[]
            {
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection()
            }, filePath);

            // Call
            FailureMechanismEntity entity = failureMechanism.Create(new PersistenceRegistry());

            // Assert
            Assert.AreEqual(1, entity.FailureMechanismSectionEntities.Count);
            Assert.AreEqual(1, entity.FailureMechanismSectionEntities.SelectMany(fms => fms.MacroStabilityOutwardsSectionResultEntities).Count());
            TestHelper.AssertAreEqualButNotSame(filePath, entity.FailureMechanismSectionCollectionSourcePath);
        }
    }
}