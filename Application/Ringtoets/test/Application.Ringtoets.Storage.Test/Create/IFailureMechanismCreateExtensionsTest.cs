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
using Application.Ringtoets.Storage.Create;
using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.TestUtil;

namespace Application.Ringtoets.Storage.Test.Create
{
    [TestFixture]
    public class IFailureMechanismCreateExtensionsTest
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
            Assert.IsEmpty(failureMechanismEntity.FailureMechanismSectionEntities);
        }

        [Test]
        public void AddEntitiesForFailureMechanismSections_WithSections_FailureMechanismSectionEntitiesCreated()
        {
            // Setup
            var failureMechanism = new TestFailureMechanism();
            failureMechanism.AddSection(new TestFailureMechanismSection());
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
            const string original = "ladida";
            IFailureMechanism failureMechanism = new TestFailureMechanism("a", "cool");
            failureMechanism.Comments = original;

            var registry = new PersistenceRegistry();

            // Call
            FailureMechanismEntity entity = failureMechanism.Create(FailureMechanismType.DuneErosion, registry);

            // Assert
            Assert.AreNotSame(original, entity.Comments,
                              "To create stable binary representations/fingerprints, it's really important that strings are not shared.");
            Assert.AreEqual(original, entity.Comments);
        }
    }
}