﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Storage.Core.Create;
using Riskeer.Storage.Core.DbContext;

namespace Riskeer.Storage.Core.Test.Create
{
    [TestFixture]
    public class FailureMechanismCreateExtensionsTest
    {
        [Test]
        public void AddEntitiesForFailureMechanismSections_WithoutCollector_ThrowsArgumentNullException()
        {
            // Setup
            var failureMechanism = new TestFailureMechanism();

            // Call
            TestDelegate test = () => failureMechanism.AddEntitiesForFailureMechanismSections(null, new FailureMechanismEntity());

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("registry", paramName);
        }

        [Test]
        public void AddEntitiesForFailureMechanismSections_WithoutEntity_ThrowsArgumentNullException()
        {
            // Setup
            var failureMechanism = new TestFailureMechanism();

            // Call
            TestDelegate test = () => failureMechanism.AddEntitiesForFailureMechanismSections(new PersistenceRegistry(), null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("entity", paramName);
        }

        [Test]
        public void AddEntitiesForFailureMechanismSections_WithoutSections_EmptyFailureMechanismSectionEntities()
        {
            // Setup
            var failureMechanism = new TestFailureMechanism();
            var failureMechanismEntity = new FailureMechanismEntity();

            // Call
            failureMechanism.AddEntitiesForFailureMechanismSections(new PersistenceRegistry(), failureMechanismEntity);

            // Assert
            CollectionAssert.IsEmpty(failureMechanismEntity.FailureMechanismSectionEntities);
        }

        [Test]
        public void AddEntitiesForFailureMechanismSections_WithSections_FailureMechanismSectionEntitiesCreated()
        {
            // Setup
            var failureMechanism = new TestFailureMechanism();
            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection()
            });
            var failureMechanismEntity = new FailureMechanismEntity();

            // Call
            failureMechanism.AddEntitiesForFailureMechanismSections(new PersistenceRegistry(), failureMechanismEntity);

            // Assert
            Assert.AreEqual(1, failureMechanismEntity.FailureMechanismSectionEntities.Count);
        }

        [Test]
        public void Create_StringPropertiesDoNotShareReference()
        {
            // Setup
            const string originalInput = "Some input text";
            const string originalOutput = "Some output text";
            const string originalNotInAssemblyText = "Really not in assembly";
            const string originalCalculationsComments = "Some calculations text";
            const string failureMechanismSectionsSourcePath = "File\\Path";
            IFailureMechanism failureMechanism = new TestFailureMechanism("a", "cool");
            failureMechanism.InAssemblyInputComments.Body = originalInput;
            failureMechanism.InAssemblyOutputComments.Body = originalOutput;
            failureMechanism.NotInAssemblyComments.Body = originalNotInAssemblyText;
            failureMechanism.CalculationsComments.Body = originalCalculationsComments;
            failureMechanism.SetSections(new[]
            {
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection()
            }, failureMechanismSectionsSourcePath);

            var registry = new PersistenceRegistry();

            // Call
            FailureMechanismEntity entity = failureMechanism.Create(FailureMechanismType.DuneErosion, registry);

            // Assert
            TestHelper.AssertAreEqualButNotSame(failureMechanism.InAssemblyInputComments.Body, entity.InAssemblyInputComments);
            TestHelper.AssertAreEqualButNotSame(failureMechanism.InAssemblyOutputComments.Body, entity.InAssemblyOutputComments);
            TestHelper.AssertAreEqualButNotSame(failureMechanism.NotInAssemblyComments.Body, entity.NotInAssemblyComments);
            TestHelper.AssertAreEqualButNotSame(failureMechanism.CalculationsComments.Body, entity.CalculationsComments);
            TestHelper.AssertAreEqualButNotSame(failureMechanism.FailureMechanismSectionSourcePath, entity.FailureMechanismSectionCollectionSourcePath);
        }
    }
}