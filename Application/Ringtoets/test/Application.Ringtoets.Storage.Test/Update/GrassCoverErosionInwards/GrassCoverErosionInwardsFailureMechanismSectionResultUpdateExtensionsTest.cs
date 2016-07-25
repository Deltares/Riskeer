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
using Application.Ringtoets.Storage.Update.GrassCoverErosionInwards;

using Core.Common.Base.Data;

using NUnit.Framework;

using Rhino.Mocks;

using Ringtoets.GrassCoverErosionInwards.Data;

namespace Application.Ringtoets.Storage.Test.Update.GrassCoverErosionInwards
{
    [TestFixture]
    public class GrassCoverErosionInwardsFailureMechanismSectionResultUpdateExtensionsTest
    {
        [Test]
        public void Update_WithoutContext_ArgumentNullException()
        {
            // Setup
            var sectionResult = new GrassCoverErosionInwardsFailureMechanismSectionResult(new TestFailureMechanismSection());

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
            var sectionResult = new GrassCoverErosionInwardsFailureMechanismSectionResult(new TestFailureMechanismSection());

            // Call
            TestDelegate test = () =>
            {
                using (var ringtoetsEntities = new RingtoetsEntities())
                {
                    sectionResult.Update(null, ringtoetsEntities);
                }
            };

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("registry", paramName);
        }

        [Test]
        public void Update_ContextWithNoCoverErosionInwardsSectionResult_EntityNotFoundException()
        {
            // Setup
            var sectionResult = new GrassCoverErosionInwardsFailureMechanismSectionResult(new TestFailureMechanismSection());

            // Call
            TestDelegate test = () =>
            {
                using (var ringtoetsEntities = new RingtoetsEntities())
                {
                    sectionResult.Update(new PersistenceRegistry(), ringtoetsEntities);
                }
            };

            // Assert
            var expectedMessage = String.Format("Het object 'GrassCoverErosionInwardsSectionResultEntity' met id '{0}' is niet gevonden.", 0);
            EntityNotFoundException exception = Assert.Throws<EntityNotFoundException>(test);
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        public void Update_ContextWithNoCoverErosionInwardsSectionResultWithId_EntityNotFoundException()
        {
            // Setup
            MockRepository mocks = new MockRepository();
            var ringtoetsEntities = RingtoetsEntitiesHelper.CreateStub(mocks);

            mocks.ReplayAll();

            var storageId = 1;
            var sectionResult = new GrassCoverErosionInwardsFailureMechanismSectionResult(new TestFailureMechanismSection())
            {
                StorageId = storageId
            };

            ringtoetsEntities.GrassCoverErosionInwardsSectionResultEntities.Add(new GrassCoverErosionInwardsSectionResultEntity
            {
                GrassCoverErosionInwardsSectionResultEntityId = 2
            });

            // Call
            TestDelegate test = () => sectionResult.Update(new PersistenceRegistry(), ringtoetsEntities);

            // Assert
            var expectedMessage = String.Format("Het object 'GrassCoverErosionInwardsSectionResultEntity' met id '{0}' is niet gevonden.", storageId);
            EntityNotFoundException exception = Assert.Throws<EntityNotFoundException>(test);
            Assert.AreEqual(expectedMessage, exception.Message);

            mocks.VerifyAll();
        }

        [Test]
        public void Update_WithGrassCoverErosionInwardsSectionResult_PropertiesUpdated()
        {
            // Setup
            MockRepository mocks = new MockRepository();
            var ringtoetsEntities = RingtoetsEntitiesHelper.CreateStub(mocks);

            mocks.ReplayAll();

            var sectionResult = new GrassCoverErosionInwardsFailureMechanismSectionResult(new TestFailureMechanismSection())
            {
                StorageId = 1,
                AssessmentLayerOne = true,
                AssessmentLayerThree = (RoundedDouble) 4.4
            };

            var sectionResultEntity = new GrassCoverErosionInwardsSectionResultEntity
            {
                GrassCoverErosionInwardsSectionResultEntityId = sectionResult.StorageId,
                LayerOne = Convert.ToByte(false),
                LayerThree = 1.1m,
            };

            ringtoetsEntities.GrassCoverErosionInwardsSectionResultEntities.Add(sectionResultEntity);

            // Call
            sectionResult.Update(new PersistenceRegistry(), ringtoetsEntities);

            // Assert
            Assert.AreEqual(Convert.ToByte(true), sectionResultEntity.LayerOne);
            Assert.AreEqual(sectionResult.AssessmentLayerThree.Value.ToNullableDecimal(), sectionResultEntity.LayerThree);

            mocks.VerifyAll();
        }

        [Test]
        public void Update_WithNaNResult_ReturnsEntityWithNullResult()
        {
            // Setup
            MockRepository mocks = new MockRepository();
            var ringtoetsEntities = RingtoetsEntitiesHelper.CreateStub(mocks);

            mocks.ReplayAll();

            var sectionResult = new GrassCoverErosionInwardsFailureMechanismSectionResult(new TestFailureMechanismSection())
            {
                StorageId = 1,
                AssessmentLayerOne = true,
                AssessmentLayerThree = (RoundedDouble)double.NaN
            };

            var sectionResultEntity = new GrassCoverErosionInwardsSectionResultEntity
            {
                GrassCoverErosionInwardsSectionResultEntityId = sectionResult.StorageId,
                LayerOne = Convert.ToByte(false),
                LayerThree = 1.1m,
            };

            ringtoetsEntities.GrassCoverErosionInwardsSectionResultEntities.Add(sectionResultEntity);

            // Call
            sectionResult.Update(new PersistenceRegistry(), ringtoetsEntities);

            // Assert
            Assert.IsNull(sectionResultEntity.LayerThree);
        }

        [Test]
        public void Update_WithNewCalculation_EntityHasCalculationEntitySet()
        {
            // Setup
            var mocks = new MockRepository();
            var ringtoetsEntities = RingtoetsEntitiesHelper.CreateStub(mocks);
            mocks.ReplayAll();

            var calculation = new GrassCoverErosionInwardsCalculation();
            var calculationEntity = new GrassCoverErosionInwardsCalculationEntity();

            var registry = new PersistenceRegistry();
            registry.Register(calculationEntity, calculation);

            var sectionResult = new GrassCoverErosionInwardsFailureMechanismSectionResult(new TestFailureMechanismSection())
            {
                StorageId = 4534,
                Calculation = calculation
            };
            var entity = new GrassCoverErosionInwardsSectionResultEntity
            {
                GrassCoverErosionInwardsSectionResultEntityId = sectionResult.StorageId,
                GrassCoverErosionInwardsCalculationEntity = null
            };

            ringtoetsEntities.GrassCoverErosionInwardsSectionResultEntities.Add(entity);

            // Call
            sectionResult.Update(registry, ringtoetsEntities);

            // Assert
            Assert.AreSame(calculationEntity, entity.GrassCoverErosionInwardsCalculationEntity);
            mocks.VerifyAll();
        }

        [Test]
        public void Update_WithDeletedCalculation_EntityHasCalculationEntitySetToNulll()
        {
            // Setup
            var mocks = new MockRepository();
            var ringtoetsEntities = RingtoetsEntitiesHelper.CreateStub(mocks);
            mocks.ReplayAll();

            var calculationEntity = new GrassCoverErosionInwardsCalculationEntity();

            var sectionResult = new GrassCoverErosionInwardsFailureMechanismSectionResult(new TestFailureMechanismSection())
            {
                StorageId = 4534,
                Calculation = null
            };
            var entity = new GrassCoverErosionInwardsSectionResultEntity
            {
                GrassCoverErosionInwardsSectionResultEntityId = sectionResult.StorageId,
                GrassCoverErosionInwardsCalculationEntity = calculationEntity
            };

            ringtoetsEntities.GrassCoverErosionInwardsSectionResultEntities.Add(entity);

            var registry = new PersistenceRegistry();

            // Call
            sectionResult.Update(registry, ringtoetsEntities);

            // Assert
            Assert.IsNull(entity.GrassCoverErosionInwardsCalculationEntity);
            mocks.VerifyAll();
        }
    }
}