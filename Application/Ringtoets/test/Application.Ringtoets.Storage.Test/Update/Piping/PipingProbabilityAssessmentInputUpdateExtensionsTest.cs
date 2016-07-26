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
using Application.Ringtoets.Storage.Exceptions;
using Application.Ringtoets.Storage.TestUtil;
using Application.Ringtoets.Storage.Update.Piping;

using NUnit.Framework;

using Rhino.Mocks;

using Ringtoets.Piping.Data;

namespace Application.Ringtoets.Storage.Test.Update.Piping
{
    [TestFixture]
    public class PipingProbabilityAssessmentInputUpdateExtensionsTest
    {
        [Test]
        public void Update_WithoutContext_ArgumentNullException()
        {
            // Setup
            var probabilityAssessmentInput = new PipingProbabilityAssessmentInput();

            // Call
            TestDelegate test = () => probabilityAssessmentInput.Update(new PersistenceRegistry(), null);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("context", paramName);
        }

        [Test]
        public void Update_WithoutPersistenceRegistry_ArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var ringtoetsEntities = mocks.Stub<IRingtoetsEntities>();
            mocks.ReplayAll();

            var probabilityAssessmentInput = new PipingProbabilityAssessmentInput();

            // Call
            TestDelegate test = () => probabilityAssessmentInput.Update(null, ringtoetsEntities);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("registry", paramName);
            mocks.VerifyAll();
        }

        [Test]
        public void Update_ContextWithNoPipingProbabilityAssessmentInput_EntityNotFoundException()
        {
            // Setup
            var mocks = new MockRepository();
            var ringtoetsEntities = RingtoetsEntitiesHelper.CreateStub(mocks);
            mocks.ReplayAll();

            var probabilityAssessmentInput = new PipingProbabilityAssessmentInput();

            // Call
            TestDelegate test = () => probabilityAssessmentInput.Update(new PersistenceRegistry(), ringtoetsEntities);

            // Assert
            var expectedMessage = String.Format("Het object 'PipingFailureMechanismMetaEntity' met id '{0}' is niet gevonden.", 0);
            EntityNotFoundException exception = Assert.Throws<EntityNotFoundException>(test);
            Assert.AreEqual(expectedMessage, exception.Message);

            mocks.VerifyAll();
        }

        [Test]
        public void Update_ContextWithNoPipingFailureMechanismWithId_EntityNotFoundException()
        {
            // Setup
            MockRepository mocks = new MockRepository();
            var ringtoetsEntities = RingtoetsEntitiesHelper.CreateStub(mocks);
            mocks.ReplayAll();

            var storageId = 1;
            var probabilityAssessmentInput = new PipingProbabilityAssessmentInput
            {
                StorageId = storageId
            };

            ringtoetsEntities.PipingFailureMechanismMetaEntities.Add(new PipingFailureMechanismMetaEntity
            {
                PipingFailureMechanismMetaEntityId = 2
            });

            // Call
            TestDelegate test = () => probabilityAssessmentInput.Update(new PersistenceRegistry(), ringtoetsEntities);

            // Assert
            var expectedMessage = String.Format("Het object 'PipingFailureMechanismMetaEntity' met id '{0}' is niet gevonden.", storageId);
            EntityNotFoundException exception = Assert.Throws<EntityNotFoundException>(test);
            Assert.AreEqual(expectedMessage, exception.Message);

            mocks.VerifyAll();
        }

        [Test]
        public void Update_ContextWithPipingFailureMechanism_PropertiesUpdated()
        {
            // Setup
            MockRepository mocks = new MockRepository();
            var ringtoetsEntities = RingtoetsEntitiesHelper.CreateStub(mocks);
            mocks.ReplayAll();

            double value = 0.64;

            var probabilityAssessmentInput = new PipingProbabilityAssessmentInput
            {
                StorageId = 1,
                A = value
            };

            var pipingFailureMechanismMetaEntity = new PipingFailureMechanismMetaEntity
            {
                PipingFailureMechanismMetaEntityId = probabilityAssessmentInput.StorageId,
                A = 0.3
            };
            ringtoetsEntities.PipingFailureMechanismMetaEntities.Add(pipingFailureMechanismMetaEntity);

            // Call
            probabilityAssessmentInput.Update(new PersistenceRegistry(), ringtoetsEntities);

            // Assert
            Assert.AreEqual(value, pipingFailureMechanismMetaEntity.A);

            mocks.VerifyAll();
        }
    }
}