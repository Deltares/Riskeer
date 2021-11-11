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
using Riskeer.Storage.Core.Create.WaterPressureAsphaltCover;
using Riskeer.Storage.Core.DbContext;

namespace Riskeer.Storage.Core.Test.Create.WaterPressureAsphaltCover
{
    [TestFixture]
    public class WaterPressureAsphaltCoverFailureMechanismCreateExtensionsTest
    {
        [Test]
        public void Create_PersistenceRegistryNull_ThrowsArgumentNullException()
        {
            // Setup
            var failureMechanism = new WaterPressureAsphaltCoverFailureMechanism();

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
            var failureMechanism = new WaterPressureAsphaltCoverFailureMechanism
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
                }
            };
            var registry = new PersistenceRegistry();

            // Call
            FailureMechanismEntity entity = failureMechanism.Create(registry);

            // Assert
            Assert.IsNotNull(entity);
            Assert.AreEqual((short) FailureMechanismType.WaterOverpressureAsphaltRevetment, entity.FailureMechanismType);
            Assert.AreEqual(Convert.ToByte(inAssembly), entity.InAssembly);
            Assert.AreEqual(failureMechanism.InAssemblyInputComments.Body, entity.InAssemblyInputComments);
            Assert.AreEqual(failureMechanism.InAssemblyOutputComments.Body, entity.InAssemblyOutputComments);
            Assert.AreEqual(failureMechanism.NotInAssemblyComments.Body, entity.NotInAssemblyComments);
            Assert.AreEqual(failureMechanism.CalculationsInputComments.Body, entity.CalculationsInputComments);
        }

        [Test]
        public void Create_StringPropertiesDoNotShareReference()
        {
            // Setup
            var failureMechanism = new WaterPressureAsphaltCoverFailureMechanism
            {
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
                    Body = "Some calculations text"
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
            var failureMechanism = new WaterPressureAsphaltCoverFailureMechanism();

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
            const string filePath = "failureMechanismSections/File/Path";
            var failureMechanism = new WaterPressureAsphaltCoverFailureMechanism();
            failureMechanism.SetSections(new[]
            {
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection()
            }, filePath);

            // Call
            FailureMechanismEntity entity = failureMechanism.Create(new PersistenceRegistry());

            // Assert
            Assert.AreEqual(1, entity.FailureMechanismSectionEntities.Count);
            Assert.AreEqual(1, entity.FailureMechanismSectionEntities.SelectMany(fms => fms.WaterPressureAsphaltCoverSectionResultEntities).Count());
            TestHelper.AssertAreEqualButNotSame(filePath, entity.FailureMechanismSectionCollectionSourcePath);
        }
    }
}