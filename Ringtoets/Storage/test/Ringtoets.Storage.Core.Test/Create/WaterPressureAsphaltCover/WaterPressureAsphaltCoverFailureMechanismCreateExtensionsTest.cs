﻿// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Integration.Data.StandAlone;
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
        public void Create_WithCollectorAndPropertiesSet_ReturnsFailureMechanismEntityWithPropertiesSet(bool isRelevant)
        {
            // Setup
            var failureMechanism = new WaterPressureAsphaltCoverFailureMechanism
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
                }
            };
            var registry = new PersistenceRegistry();

            // Call
            FailureMechanismEntity entity = failureMechanism.Create(registry);

            // Assert
            Assert.IsNotNull(entity);
            Assert.AreEqual((short) FailureMechanismType.WaterOverpressureAsphaltRevetment, entity.FailureMechanismType);
            Assert.AreEqual(Convert.ToByte(isRelevant), entity.IsRelevant);
            Assert.AreEqual(failureMechanism.InputComments.Body, entity.InputComments);
            Assert.AreEqual(failureMechanism.OutputComments.Body, entity.OutputComments);
            Assert.AreEqual(failureMechanism.NotRelevantComments.Body, entity.NotRelevantComments);
        }

        [Test]
        public void Create_StringPropertiesDoNotShareReference()
        {
            // Setup
            const string originalInput = "Some input text";
            const string originalOutput = "Some output text";
            const string originalNotRelevantText = "Really not relevant";
            var failureMechanism = new WaterPressureAsphaltCoverFailureMechanism
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
            TestHelper.AssertAreEqualButNotSame(failureMechanism.InputComments.Body, entity.InputComments);
            TestHelper.AssertAreEqualButNotSame(failureMechanism.OutputComments.Body, entity.OutputComments);
            TestHelper.AssertAreEqualButNotSame(failureMechanism.NotRelevantComments.Body, entity.NotRelevantComments);
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