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
using NUnit.Framework;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Storage.Core.Create;
using Ringtoets.Storage.Core.DbContext;

namespace Ringtoets.Storage.Core.Test.Create
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
            failureMechanism.AddSections(new[]
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
            const string originalNotRelevantText = "Really not relevant";
            IFailureMechanism failureMechanism = new TestFailureMechanism("a", "cool");
            failureMechanism.InputComments.Body = originalInput;
            failureMechanism.OutputComments.Body = originalOutput;
            failureMechanism.NotRelevantComments.Body = originalNotRelevantText;

            var registry = new PersistenceRegistry();

            // Call
            FailureMechanismEntity entity = failureMechanism.Create(FailureMechanismType.DuneErosion, registry);

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
    }
}