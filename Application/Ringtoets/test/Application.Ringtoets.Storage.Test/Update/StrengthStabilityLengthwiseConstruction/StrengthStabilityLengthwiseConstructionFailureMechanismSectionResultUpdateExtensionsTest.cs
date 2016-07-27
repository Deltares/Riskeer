﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Application.Ringtoets.Storage.Update.StrengthStabilityLengthwiseConstruction;
using Core.Common.Base.Data;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Integration.Data.StandAlone.SectionResults;

namespace Application.Ringtoets.Storage.Test.Update.StrengthStabilityLengthwiseConstruction
{
    [TestFixture]
    public class StrengthStabilityLengthwiseConstructionFailureMechanismSectionResultUpdateExtensionsTest
    {
        [Test]
        public void Update_WithoutContext_ArgumentNullException()
        {
            // Setup
            var sectionResult = new StrengthStabilityLengthwiseConstructionFailureMechanismSectionResult(new TestFailureMechanismSection());

            // Call
            TestDelegate test = () => sectionResult.Update(new PersistenceRegistry(), null);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("context", paramName);
        }

        [Test]
        public void Update_WithoutPersistenceRegistry_ArgumentNullException()
        {
            // Setup
            var sectionResult = new StrengthStabilityLengthwiseConstructionFailureMechanismSectionResult(new TestFailureMechanismSection());

            // Call
            TestDelegate test = () =>
            {
                using (var ringtoetsEntities = new RingtoetsEntities())
                    sectionResult.Update(null, ringtoetsEntities);
            };

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("registry", paramName);
        }

        [Test]
        public void Update_ContextWithoutEntity_EntityNotFoundException()
        {
            // Setup
            var sectionResult = new StrengthStabilityLengthwiseConstructionFailureMechanismSectionResult(new TestFailureMechanismSection());

            // Call
            TestDelegate test = () =>
            {
                using (var ringtoetsEntities = new RingtoetsEntities())
                    sectionResult.Update(new PersistenceRegistry(), ringtoetsEntities);
            };

            // Assert
            var expectedMessage = String.Format("Het object 'StrengthStabilityLengthwiseConstructionSectionResultEntity' met id '{0}' is niet gevonden.", 0);
            EntityNotFoundException exception = Assert.Throws<EntityNotFoundException>(test);
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        public void Update_ContextWithOtherEntity_EntityNotFoundException()
        {
            // Setup
            MockRepository mocks = new MockRepository();
            var ringtoetsEntities = RingtoetsEntitiesHelper.CreateStub(mocks);

            mocks.ReplayAll();

            var storageId = 1;
            var sectionResult = new StrengthStabilityLengthwiseConstructionFailureMechanismSectionResult(new TestFailureMechanismSection())
            {
                StorageId = storageId
            };

            ringtoetsEntities.StrengthStabilityLengthwiseConstructionSectionResultEntities.Add(new StrengthStabilityLengthwiseConstructionSectionResultEntity
            {
                StrengthStabilityLengthwiseConstructionSectionResultEntityId = 2
            });

            // Call
            TestDelegate test = () => sectionResult.Update(new PersistenceRegistry(), ringtoetsEntities);

            // Assert
            var expectedMessage = String.Format("Het object 'StrengthStabilityLengthwiseConstructionSectionResultEntity' met id '{0}' is niet gevonden.", storageId);
            EntityNotFoundException exception = Assert.Throws<EntityNotFoundException>(test);
            Assert.AreEqual(expectedMessage, exception.Message);

            mocks.VerifyAll();
        }

        [Test]
        public void Update_WithStrengthStabilityLengthwiseConstructionSectionResult_PropertiesUpdated()
        {
            // Setup
            MockRepository mocks = new MockRepository();
            var ringtoetsEntities = RingtoetsEntitiesHelper.CreateStub(mocks);

            mocks.ReplayAll();

            var sectionResult = new StrengthStabilityLengthwiseConstructionFailureMechanismSectionResult(new TestFailureMechanismSection())
            {
                StorageId = 1,
                AssessmentLayerOne = true,
                AssessmentLayerThree = (RoundedDouble) 4.4
            };

            var sectionResultEntity = new StrengthStabilityLengthwiseConstructionSectionResultEntity
            {
                StrengthStabilityLengthwiseConstructionSectionResultEntityId = sectionResult.StorageId,
                LayerOne = Convert.ToByte(false),
                LayerThree = 1.1
            };

            ringtoetsEntities.StrengthStabilityLengthwiseConstructionSectionResultEntities.Add(sectionResultEntity);

            // Call
            sectionResult.Update(new PersistenceRegistry(), ringtoetsEntities);

            // Assert
            Assert.AreEqual(Convert.ToByte(true), sectionResultEntity.LayerOne);
            Assert.AreEqual(sectionResult.AssessmentLayerThree.Value.ToNaNAsNull(), sectionResultEntity.LayerThree);

            mocks.VerifyAll();
        }

        [Test]
        public void Create_WithNaNResult_ReturnsEntityWithNullResult()
        {
            // Setup
            MockRepository mocks = new MockRepository();
            var ringtoetsEntities = RingtoetsEntitiesHelper.CreateStub(mocks);

            mocks.ReplayAll();

            var sectionResult = new StrengthStabilityLengthwiseConstructionFailureMechanismSectionResult(new TestFailureMechanismSection())
            {
                StorageId = 1,
                AssessmentLayerOne = true,
                AssessmentLayerThree = (RoundedDouble) double.NaN
            };

            var sectionResultEntity = new StrengthStabilityLengthwiseConstructionSectionResultEntity
            {
                StrengthStabilityLengthwiseConstructionSectionResultEntityId = sectionResult.StorageId,
                LayerOne = Convert.ToByte(false),
                LayerThree = 1.1
            };

            ringtoetsEntities.StrengthStabilityLengthwiseConstructionSectionResultEntities.Add(sectionResultEntity);

            // Call
            sectionResult.Update(new PersistenceRegistry(), ringtoetsEntities);

            // Assert
            Assert.IsNull(sectionResultEntity.LayerThree);
        }
    }
}