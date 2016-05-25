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
using System.Linq;

using Application.Ringtoets.Storage.Create;
using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.TestUtil;

using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.HydraRing.Data;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.Data.StandAlone;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Data.TestUtil;
using Ringtoets.Piping.KernelWrapper.TestUtil;
using Ringtoets.Piping.Primitives;

namespace Application.Ringtoets.Storage.Test.Create
{
    [TestFixture]
    public class PersistenceRegistryTest
    {
        [Test]
        public void Contains_WithoutModel_ArgumentNullException()
        {
            // Setup
            var collector = new PersistenceRegistry();

            // Call
            TestDelegate test = () => collector.Contains(null);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("model", paramName);
        }

        [Test]
        public void Contains_SoilProfileAdded_True()
        {
            // Setup
            var collector = new PersistenceRegistry();
            var profile = new TestPipingSoilProfile();
            collector.Register(new SoilProfileEntity(), profile);

            // Call
            var result = collector.Contains(profile);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void Contains_NoSoilProfileAdded_False()
        {
            // Setup
            var collector = new PersistenceRegistry();
            var profile = new TestPipingSoilProfile();

            // Call
            var result = collector.Contains(profile);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Contains_OtherSoilProfileAdded_False()
        {
            // Setup
            var collector = new PersistenceRegistry();
            var profile = new TestPipingSoilProfile();
            collector.Register(new SoilProfileEntity(), new TestPipingSoilProfile());

            // Call
            var result = collector.Contains(profile);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Get_WithoutModel_ArgumentNullException()
        {
            // Setup
            var collector = new PersistenceRegistry();

            // Call
            TestDelegate test = () => collector.Get(null);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("model", paramName);
        }

        [Test]
        public void Get_SoilProfileAdded_ReturnsEntity()
        {
            // Setup
            var collector = new PersistenceRegistry();
            var profile = new TestPipingSoilProfile();
            var entity = new SoilProfileEntity();
            collector.Register(entity, profile);

            // Call
            var result = collector.Get(profile);

            // Assert
            Assert.AreSame(entity, result);
        }

        [Test]
        public void Get_NoSoilProfileAdded_ThrowsInvalidOperationException()
        {
            // Setup
            var collector = new PersistenceRegistry();
            var profile = new TestPipingSoilProfile();

            // Call
            TestDelegate test = () => collector.Get(profile);

            // Assert
            Assert.Throws<InvalidOperationException>(test);
        }

        [Test]
        public void Get_OtherSoilProfileAdded_ThrowsInvalidOperationException()
        {
            // Setup
            var collector = new PersistenceRegistry();
            var profile = new TestPipingSoilProfile();
            collector.Register(new SoilProfileEntity(), new TestPipingSoilProfile());

            // Call
            TestDelegate test = () => collector.Get(profile);

            // Assert
            Assert.Throws<InvalidOperationException>(test);
        }

        [Test]
        public void GetSurfaceLinePoint_SurfaceLinePointAdded_ReturnsEntity()
        {
            // Setup
            var surfaceLineGeometryPoint = new Point3D(1.1, 2.2, 3.3);
            var initializedEntity = new SurfaceLinePointEntity();

            var collector = new PersistenceRegistry();
            collector.Register(initializedEntity, surfaceLineGeometryPoint);

            // Call
            SurfaceLinePointEntity retrievedEntity = collector.GetSurfaceLinePoint(surfaceLineGeometryPoint);

            // Assert
            Assert.AreSame(initializedEntity, retrievedEntity);

        }

        #region Register methods

        [Test]
        public void Register_WithNullProjectEntity_ThrowsArgumentNullException()
        {
            // Setup
            var collector = new PersistenceRegistry();

            // Call
            TestDelegate test = () => collector.Register(null, new Project());

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("entity", paramName);
        }

        [Test]
        public void Register_WithNullProject_ThrowsArgumentNullException()
        {
            // Setup
            var collector = new PersistenceRegistry();

            // Call
            TestDelegate test = () => collector.Register(new ProjectEntity(), null);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("model", paramName);
        }

        [Test]
        public void Register_WithNullAssessmentSectionEntity_ThrowsArgumentNullException()
        {
            // Setup
            var collector = new PersistenceRegistry();
            
            // Call
            TestDelegate test = () => collector.Register(null, new AssessmentSection(AssessmentSectionComposition.Dike));

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("entity", paramName);
        }

        [Test]
        public void Register_WithNullAssessmentSection_ThrowsArgumentNullException()
        {
            // Setup
            var collector = new PersistenceRegistry();
            
            // Call
            TestDelegate test = () => collector.Register(new AssessmentSectionEntity(), null);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("model", paramName);
        }

        [Test]
        public void Register_WithNullFailureMechanismEntity_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var model = mocks.StrictMock<IFailureMechanism>();
            var collector = new PersistenceRegistry();
            
            // Call
            TestDelegate test = () => collector.Register(null, model);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("entity", paramName);
        }

        [Test]
        public void Register_WithNullFailureMechanismBase_ThrowsArgumentNullException()
        {
            // Setup
            var collector = new PersistenceRegistry();
            
            // Call
            TestDelegate test = () => collector.Register(new FailureMechanismEntity(), null);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("model", paramName);
        }

        [Test]
        public void Register_WithNullFailureMechanismSection_ThrowsArgumentNullException()
        {
            // Setup
            var collector = new PersistenceRegistry();
            
            // Call
            TestDelegate test = () => collector.Register(new FailureMechanismSectionEntity(), null);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("model", paramName);
        }

        [Test]
        public void Register_WithNullHydraulicLocationEntity_ThrowsArgumentNullException()
        {
            // Setup
            var collector = new PersistenceRegistry();

            // Call
            TestDelegate test = () => collector.Register(null, new HydraulicBoundaryLocation(-1, "name", 0, 0));

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("entity", paramName);
        }

        [Test]
        public void Register_WithNullHydraulicBoundaryLocation_ThrowsArgumentNullException()
        {
            // Setup
            var collector = new PersistenceRegistry();

            // Call
            TestDelegate test = () => collector.Register(new HydraulicLocationEntity(), null);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("model", paramName);
        }

        [Test]
        public void Register_WithNullCalculationGroupEntity_ThrowsArgumentNullException()
        {
            // Setup
            var collector = new PersistenceRegistry();

            // Call
            TestDelegate test = () => collector.Register(null, new CalculationGroup());

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("entity", paramName);
        }

        [Test]
        public void Register_WithNullCalculationGroup_ThrowsArgumentNullException()
        {
            // Setup
            var collector = new PersistenceRegistry();

            // Call
            TestDelegate test = () => collector.Register(new CalculationGroupEntity(), null);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("model", paramName);
        }

        [Test]
        public void Register_WithNullStochasticSoilModelEntity_ThrowsArgumentNullException()
        {
            // Setup
            var collector = new PersistenceRegistry();

            // Call
            TestDelegate test = () => collector.Register(null, new TestStochasticSoilModel());

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("entity", paramName);
        }

        [Test]
        public void Register_WithNullStochasticSoilModel_ThrowsArgumentNullException()
        {
            // Setup
            var collector = new PersistenceRegistry();

            // Call
            TestDelegate test = () => collector.Register(new StochasticSoilModelEntity(), null);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("model", paramName);
        }

        [Test]
        public void Register_WithNullStochasticSoilProfileEntity_ThrowsArgumentNullException()
        {
            // Setup
            var collector = new PersistenceRegistry();

            // Call
            TestDelegate test = () => collector.Register(null, new StochasticSoilProfile(1, SoilProfileType.SoilProfile1D, -1));

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("entity", paramName);
        }

        [Test]
        public void Register_WithNullStochasticSoilProfile_ThrowsArgumentNullException()
        {
            // Setup
            var collector = new PersistenceRegistry();

            // Call
            TestDelegate test = () => collector.Register(new StochasticSoilProfileEntity(), null);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("model", paramName);
        }

        [Test]
        public void Register_WithNullSoilProfileEntity_ThrowsArgumentNullException()
        {
            // Setup
            var collector = new PersistenceRegistry();

            // Call
            TestDelegate test = () => collector.Register(null, new PipingSoilProfile("name", 0, new [] { new PipingSoilLayer(1) }, SoilProfileType.SoilProfile1D, -1));

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("entity", paramName);
        }

        [Test]
        public void Register_WithNullPipingSoilProfile_ThrowsArgumentNullException()
        {
            // Setup
            var collector = new PersistenceRegistry();

            // Call
            TestDelegate test = () => collector.Register(new SoilProfileEntity(), null);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("model", paramName);
        }

        [Test]
        public void Register_WithNullSoilLayerEntity_ThrowsArgumentNullException()
        {
            // Setup
            var collector = new PersistenceRegistry();

            // Call
            TestDelegate test = () => collector.Register(null, new PipingSoilLayer(0));

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("entity", paramName);
        }

        [Test]
        public void Register_WithNullPipingSoilLayer_ThrowsArgumentNullException()
        {
            // Setup
            var collector = new PersistenceRegistry();

            // Call
            TestDelegate test = () => collector.Register(new SoilLayerEntity(), null);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("model", paramName);
        }

        [Test]
        public void Register_WithNullSurfaceLinePointEntity_ThrowsArgumentNullException()
        {
            // Setup
            var collector = new PersistenceRegistry();

            // Call
            TestDelegate call = () => collector.Register((SurfaceLinePointEntity)null, new Point3D(1.1, 2.2, 3.3));

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("entity", paramName);
        }

        [Test]
        public void Register_WithNullSurfaceLinePoint_ThrowsArgumentNullException()
        {
            // Setup
            var collector = new PersistenceRegistry();

            // Call
            TestDelegate call = () => collector.Register(new SurfaceLinePointEntity(), null);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("model", paramName);
        }

        [Test]
        public void Register_WithNullCharacteristicPointEntity_ThrowsArgumentNullException()
        {
            // Setup
            var collector = new PersistenceRegistry();

            // Call
            TestDelegate call = () => collector.Register((CharacteristicPointEntity)null, new Point3D(1.1, 2.2, 3.3));

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("entity", paramName);
        }

        [Test]
        public void Register_WithNullPoint3DForCharacteristicPoint_ThrowsArgumentNullException()
        {
            // Setup
            var collector = new PersistenceRegistry();

            // Call
            TestDelegate call = () => collector.Register(new CharacteristicPointEntity(), null);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("model", paramName);
        }

        #endregion

        #region TransferId method

        [Test]
        public void TransferId_WithProjectEntityAdded_EqualProjectEntityIdAndProjectStorageId()
        {
            // Setup
            var collector = new PersistenceRegistry();

            long storageId = new Random(21).Next(1,4000);
            var entity = new ProjectEntity
            {
                ProjectEntityId = storageId
            };
            var model = new Project();
            collector.Register(entity, model);

            // Call
            collector.TransferIds();

            // Assert
            Assert.AreEqual(storageId, model.StorageId);
        }

        [Test]
        public void TransferId_WithAssessmentSectionEntityAdded_EqualAssessmentSectionEntityIdAndAssessmentSectionStorageId()
        {
            // Setup
            var collector = new PersistenceRegistry();

            long storageId = new Random(21).Next(1,4000);
            var entity = new AssessmentSectionEntity
            {
                AssessmentSectionEntityId = storageId
            };
            var model = new AssessmentSection(AssessmentSectionComposition.Dike);
            collector.Register(entity, model);

            // Call
            collector.TransferIds();

            // Assert
            Assert.AreEqual(storageId, model.StorageId);
        }

        [Test]
        public void TransferId_WithFailureMechanismEntityAddedWithPipingFailureMechanism_EqualFailureMechanismEntityIdAndPipingFailureMechanismStorageId()
        {
            // Setup
            var collector = new PersistenceRegistry();

            long storageId = new Random(21).Next(1,4000);
            var entity = new FailureMechanismEntity
            {
                FailureMechanismEntityId = storageId
            };
            var model = new PipingFailureMechanism();
            collector.Register(entity, model);

            // Call
            collector.TransferIds();

            // Assert
            Assert.AreEqual(storageId, model.StorageId);
        }

        [Test]
        public void TransferId_WithFailureMechanismEntityAddedWithStandAloneFailureMechanism_EqualFailureMechanismEntityIdAndStandAloneFailureMechanismStorageId()
        {
            // Setup
            var collector = new PersistenceRegistry();

            long storageId = new Random(21).Next(1,4000);
            var entity = new FailureMechanismEntity
            {
                FailureMechanismEntityId = storageId
            };
            var model = new MacrostabilityInwardsFailureMechanism();
            collector.Register(entity, model);

            // Call
            collector.TransferIds();

            // Assert
            Assert.AreEqual(storageId, model.StorageId);
        }

        [Test]
        public void TransferId_WithFailureMechanismSectionEntityAddedWithFailureMechanismSection_EqualFailureMechanismSectionEntityIdAndFailureMechanismSectionStorageId()
        {
            // Setup
            var collector = new PersistenceRegistry();

            long storageId = new Random(21).Next(1,4000);
            var entity = new FailureMechanismSectionEntity
            {
                FailureMechanismSectionEntityId = storageId
            };
            var model = new FailureMechanismSection("name", new [] { new Point2D(0,0) });
            collector.Register(entity, model);

            // Call
            collector.TransferIds();

            // Assert
            Assert.AreEqual(storageId, model.StorageId);
        }

        [Test]
        public void TransferId_WithHydraulicLocationEntityAdded_EqualHydraulicLocationEntityIdAndHydraulicBoundaryLocationStorageId()
        {
            // Setup
            var collector = new PersistenceRegistry();

            long storageId = new Random(21).Next(1,4000);
            var entity = new HydraulicLocationEntity
            {
                HydraulicLocationEntityId = storageId
            };
            var model = new HydraulicBoundaryLocation(-1, "name", 0, 0);
            collector.Register(entity, model);

            // Call
            collector.TransferIds();

            // Assert
            Assert.AreEqual(storageId, model.StorageId);
        }

        [Test]
        public void TransferId_WithCalculationGroupEntityAdded_EqualCalculationGroupEntityIdAndCalculationGroupStorageId()
        {
            // Setup
            var collector = new PersistenceRegistry();

            long storageId = new Random(21).Next(1, 4000);
            var entity = new CalculationGroupEntity
            {
                CalculationGroupEntityId = storageId
            };
            var model = new CalculationGroup();
            collector.Register(entity, model);

            // Call
            collector.TransferIds();

            // Assert
            Assert.AreEqual(storageId, model.StorageId);
        }

        [Test]
        public void TransferId_WithStochasticSoilModelEntityAdded_EqualStochasticSoilModelEntityIdAndStochasticSoilModelStorageId()
        {
            // Setup
            var collector = new PersistenceRegistry();

            long storageId = new Random(21).Next(1,4000);
            var entity = new StochasticSoilModelEntity
            {
                StochasticSoilModelEntityId = storageId
            };
            var model = new StochasticSoilModel(-1, "name", "name");
            collector.Register(entity, model);

            // Call
            collector.TransferIds();

            // Assert
            Assert.AreEqual(storageId, model.StorageId);
        }

        [Test]
        public void TransferId_WithStochasticSoilProfileEntityAdded_EqualStochasticSoilProfileEntityIdAndStochasticSoilProfileStorageId()
        {
            // Setup
            var collector = new PersistenceRegistry();

            long storageId = new Random(21).Next(1,4000);
            var entity = new StochasticSoilProfileEntity
            {
                StochasticSoilProfileEntityId = storageId
            };
            var model = new StochasticSoilProfile(1, SoilProfileType.SoilProfile1D, -1);
            collector.Register(entity, model);

            // Call
            collector.TransferIds();

            // Assert
            Assert.AreEqual(storageId, model.StorageId);
        }

        [Test]
        public void TransferId_WithSoilProfileEntityAdded_EqualSoilProfileEntityIdAndPipingSoilProfileStorageId()
        {
            // Setup
            var collector = new PersistenceRegistry();

            long storageId = new Random(21).Next(1,4000);
            var entity = new SoilProfileEntity
            {
                SoilProfileEntityId = storageId
            };
            var model = new PipingSoilProfile("name", 0, new [] { new PipingSoilLayer(1) }, SoilProfileType.SoilProfile1D, -1);
            collector.Register(entity, model);

            // Call
            collector.TransferIds();

            // Assert
            Assert.AreEqual(storageId, model.StorageId);
        }

        [Test]
        public void TransferId_WithSoilLayerEntityAdded_EqualSoilLayerEntityIdAndPipingSoilLayerStorageId()
        {
            // Setup
            var collector = new PersistenceRegistry();

            long storageId = new Random(21).Next(1,4000);
            var entity = new SoilLayerEntity
            {
                SoilLayerEntityId = storageId
            };
            var model = new PipingSoilLayer(0);
            collector.Register(entity, model);

            // Call
            collector.TransferIds();

            // Assert
            Assert.AreEqual(storageId, model.StorageId);
        }

        [Test]
        public void TransferId_WithSurfaceLineEntityAdded_EqualSurfaceLineEntityIdAndRingtoetsPipingSurfaceLineStorageId()
        {
            // Setup
            var collector = new PersistenceRegistry();

            long storageId = new Random(21).Next(1, 4000);
            var entity = new SurfaceLineEntity
            {
                SurfaceLineEntityId = storageId
            };
            var model = new RingtoetsPipingSurfaceLine();
            collector.Register(entity, model);

            // Call
            collector.TransferIds();

            // Assert
            Assert.AreEqual(storageId, model.StorageId);
        }

        [Test]
        public void TransferId_WithSurfaceLinePointEntityAdded_EqualSurfaceLinePointEntityIdAndPoint3DStorageId()
        {
            // Setup
            var collector = new PersistenceRegistry();

            long storageId = new Random(21).Next(1, 4000);
            var entity = new SurfaceLinePointEntity
            {
                SurfaceLinePointEntityId = storageId
            };
            var model = new Point3D(1.1, 2.2, 3.3);
            collector.Register(entity, model);

            // Call
            collector.TransferIds();

            // Assert
            Assert.AreEqual(storageId, model.StorageId);
        }

        #endregion

        #region RemoveUntouched method

        [Test]
        public void RemoveUntouched_ProjectEntity_OrphanedEntityIsRemovedFromRingtoetsEntities()
        {
            // Setup
            var mocks = new MockRepository();
            IRingtoetsEntities dbContext = RingtoetsEntitiesHelper.CreateStub(mocks);
            mocks.ReplayAll();

            var orphanedEntity = new ProjectEntity
            {
                ProjectEntityId = 1
            };
            var persistentEntity = new ProjectEntity
            {
                ProjectEntityId = 2
            };
            dbContext.ProjectEntities.Add(orphanedEntity);
            dbContext.ProjectEntities.Add(persistentEntity);

            var project = new Project { StorageId = persistentEntity.ProjectEntityId };

            var collector = new PersistenceRegistry();
            collector.Register(persistentEntity, project);

            // Call
            collector.RemoveUntouched(dbContext);

            // Assert
            Assert.AreEqual(1, dbContext.ProjectEntities.Count());
            CollectionAssert.Contains(dbContext.ProjectEntities, persistentEntity);
            mocks.VerifyAll();
        }

        [Test]
        public void RemoveUntouched_AssessmentSectionEntity_OrphanedEntityIsRemovedFromRingtoetsEntities()
        {
            // Setup
            var mocks = new MockRepository();
            IRingtoetsEntities dbContext = RingtoetsEntitiesHelper.CreateStub(mocks);
            mocks.ReplayAll();

            var orphanedEntity = new AssessmentSectionEntity
            {
                AssessmentSectionEntityId = 1
            };
            var persistentEntity = new AssessmentSectionEntity
            {
                AssessmentSectionEntityId = 2
            };
            dbContext.AssessmentSectionEntities.Add(orphanedEntity);
            dbContext.AssessmentSectionEntities.Add(persistentEntity);

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike){ StorageId = persistentEntity.AssessmentSectionEntityId };

            var collector = new PersistenceRegistry();
            collector.Register(persistentEntity, assessmentSection);

            // Call
            collector.RemoveUntouched(dbContext);

            // Assert
            Assert.AreEqual(1, dbContext.AssessmentSectionEntities.Count());
            CollectionAssert.Contains(dbContext.AssessmentSectionEntities, persistentEntity);
            mocks.VerifyAll();
        }

        [Test]
        public void RemoveUntouched_FailureMechanismEntity_OrphanedEntityIsRemovedFromRingtoetsEntities()
        {
            // Setup
            var mocks = new MockRepository();
            IRingtoetsEntities dbContext = RingtoetsEntitiesHelper.CreateStub(mocks);
            var failureMechanismStub = mocks.Stub<IFailureMechanism>();
            mocks.ReplayAll();

            var orphanedEntity = new FailureMechanismEntity
            {
                FailureMechanismEntityId = 1
            };
            var persistentEntity = new FailureMechanismEntity
            {
                FailureMechanismEntityId = 2
            };
            dbContext.FailureMechanismEntities.Add(orphanedEntity);
            dbContext.FailureMechanismEntities.Add(persistentEntity);

            failureMechanismStub.StorageId = persistentEntity.FailureMechanismEntityId;

            var collector = new PersistenceRegistry();
            collector.Register(persistentEntity, failureMechanismStub);

            // Call
            collector.RemoveUntouched(dbContext);

            // Assert
            Assert.AreEqual(1, dbContext.FailureMechanismEntities.Count());
            CollectionAssert.Contains(dbContext.FailureMechanismEntities, persistentEntity);
            mocks.VerifyAll();
        }

        [Test]
        public void RemoveUntouched_FailureMechanismSectionEntity_OrphanedEntityIsRemovedFromRingtoetsEntities()
        {
            // Setup
            var mocks = new MockRepository();
            IRingtoetsEntities dbContext = RingtoetsEntitiesHelper.CreateStub(mocks);
            mocks.ReplayAll();

            var orphanedEntity = new FailureMechanismSectionEntity
            {
                FailureMechanismSectionEntityId = 1
            };
            var persistentEntity = new FailureMechanismSectionEntity
            {
                FailureMechanismSectionEntityId = 2
            };
            dbContext.FailureMechanismSectionEntities.Add(orphanedEntity);
            dbContext.FailureMechanismSectionEntities.Add(persistentEntity);

            var section = new FailureMechanismSection("A", new[]{new Point2D(1, 2) }){ StorageId = persistentEntity.FailureMechanismSectionEntityId };

            var collector = new PersistenceRegistry();
            collector.Register(persistentEntity, section);

            // Call
            collector.RemoveUntouched(dbContext);

            // Assert
            Assert.AreEqual(1, dbContext.FailureMechanismSectionEntities.Count());
            CollectionAssert.Contains(dbContext.FailureMechanismSectionEntities, persistentEntity);
            mocks.VerifyAll();
        }

        [Test]
        public void RemoveUntouched_HydraulicLocationEntity_OrphanedEntityIsRemovedFromRingtoetsEntities()
        {
            // Setup
            var mocks = new MockRepository();
            IRingtoetsEntities dbContext = RingtoetsEntitiesHelper.CreateStub(mocks);
            mocks.ReplayAll();

            var orphanedEntity = new HydraulicLocationEntity
            {
                HydraulicLocationEntityId = 1
            };
            var persistentEntity = new HydraulicLocationEntity
            {
                HydraulicLocationEntityId = 2
            };
            dbContext.HydraulicLocationEntities.Add(orphanedEntity);
            dbContext.HydraulicLocationEntities.Add(persistentEntity);

            var boundaryLocation = new HydraulicBoundaryLocation(123, "A", 1, 2){ StorageId = persistentEntity.HydraulicLocationEntityId };

            var collector = new PersistenceRegistry();
            collector.Register(persistentEntity, boundaryLocation);

            // Call
            collector.RemoveUntouched(dbContext);

            // Assert
            Assert.AreEqual(1, dbContext.HydraulicLocationEntities.Count());
            CollectionAssert.Contains(dbContext.HydraulicLocationEntities, persistentEntity);
            mocks.VerifyAll();
        }

        [Test]
        public void RemoveUntouched_CalculationGroupEntity_OrphanedEntityIsRemovedFromRingtoetsEntities()
        {
            // Setup
            var mocks = new MockRepository();
            IRingtoetsEntities dbContext = RingtoetsEntitiesHelper.CreateStub(mocks);
            mocks.ReplayAll();

            var orphanedEntity = new CalculationGroupEntity
            {
                CalculationGroupEntityId = 1
            };
            var persistentEntity = new CalculationGroupEntity
            {
                CalculationGroupEntityId = 2
            };
            dbContext.CalculationGroupEntities.Add(orphanedEntity);
            dbContext.CalculationGroupEntities.Add(persistentEntity);

            var calculationGroup = new CalculationGroup{ StorageId = persistentEntity.CalculationGroupEntityId };

            var collector = new PersistenceRegistry();
            collector.Register(persistentEntity, calculationGroup);

            // Call
            collector.RemoveUntouched(dbContext);

            // Assert
            Assert.AreEqual(1, dbContext.CalculationGroupEntities.Count());
            CollectionAssert.Contains(dbContext.CalculationGroupEntities, persistentEntity);
            mocks.VerifyAll();
        }

        [Test]
        public void RemoveUntouched_StochasticSoilModelEntity_OrphanedEntityIsRemovedFromRingtoetsEntities()
        {
            // Setup
            var mocks = new MockRepository();
            IRingtoetsEntities dbContext = RingtoetsEntitiesHelper.CreateStub(mocks);
            mocks.ReplayAll();

            var orphanedEntity = new StochasticSoilModelEntity
            {
                StochasticSoilModelEntityId = 1
            };
            var persistentEntity = new StochasticSoilModelEntity
            {
                StochasticSoilModelEntityId = 2
            };
            dbContext.StochasticSoilModelEntities.Add(orphanedEntity);
            dbContext.StochasticSoilModelEntities.Add(persistentEntity);

            var soilModel = new StochasticSoilModel(123, "A", "B"){ StorageId = persistentEntity.StochasticSoilModelEntityId };

            var collector = new PersistenceRegistry();
            collector.Register(persistentEntity, soilModel);

            // Call
            collector.RemoveUntouched(dbContext);

            // Assert
            Assert.AreEqual(1, dbContext.StochasticSoilModelEntities.Count());
            CollectionAssert.Contains(dbContext.StochasticSoilModelEntities, persistentEntity);
            mocks.VerifyAll();
        }

        [Test]
        public void RemoveUntouched_StochasticSoilProfileEntity_OrphanedEntityIsRemovedFromRingtoetsEntities()
        {
            // Setup
            var mocks = new MockRepository();
            IRingtoetsEntities dbContext = RingtoetsEntitiesHelper.CreateStub(mocks);
            mocks.ReplayAll();

            var orphanedEntity = new StochasticSoilProfileEntity
            {
                StochasticSoilProfileEntityId = 1
            };
            var persistentEntity = new StochasticSoilProfileEntity
            {
                StochasticSoilProfileEntityId = 2
            };
            dbContext.StochasticSoilProfileEntities.Add(orphanedEntity);
            dbContext.StochasticSoilProfileEntities.Add(persistentEntity);

            var stochasticSoilProfile = new StochasticSoilProfile(1.0, SoilProfileType.SoilProfile1D, 123){ StorageId = persistentEntity.StochasticSoilProfileEntityId };

            var collector = new PersistenceRegistry();
            collector.Register(persistentEntity, stochasticSoilProfile);

            // Call
            collector.RemoveUntouched(dbContext);

            // Assert
            Assert.AreEqual(1, dbContext.StochasticSoilProfileEntities.Count());
            CollectionAssert.Contains(dbContext.StochasticSoilProfileEntities, persistentEntity);
            mocks.VerifyAll();
        }

        [Test]
        public void RemoveUntouched_SoilProfileEntity_OrphanedEntityIsRemovedFromRingtoetsEntities()
        {
            // Setup
            var mocks = new MockRepository();
            IRingtoetsEntities dbContext = RingtoetsEntitiesHelper.CreateStub(mocks);
            mocks.ReplayAll();

            var orphanedEntity = new SoilProfileEntity
            {
                SoilProfileEntityId = 1
            };
            var persistentEntity = new SoilProfileEntity
            {
                SoilProfileEntityId = 2
            };
            dbContext.SoilProfileEntities.Add(orphanedEntity);
            dbContext.SoilProfileEntities.Add(persistentEntity);

            var soilProfile = new TestPipingSoilProfile
            {
                StorageId = persistentEntity.SoilProfileEntityId
            };

            var collector = new PersistenceRegistry();
            collector.Register(persistentEntity, soilProfile);

            // Call
            collector.RemoveUntouched(dbContext);

            // Assert
            Assert.AreEqual(1, dbContext.SoilProfileEntities.Count());
            CollectionAssert.Contains(dbContext.SoilProfileEntities, persistentEntity);
            mocks.VerifyAll();
        }

        [Test]
        public void RemoveUntouched_SoilLayerEntity_OrphanedEntityIsRemovedFromRingtoetsEntities()
        {
            // Setup
            var mocks = new MockRepository();
            IRingtoetsEntities dbContext = RingtoetsEntitiesHelper.CreateStub(mocks);
            mocks.ReplayAll();

            var orphanedEntity = new SoilLayerEntity
            {
                SoilLayerEntityId = 1
            };
            var persistentEntity = new SoilLayerEntity
            {
                SoilLayerEntityId = 2
            };
            dbContext.SoilLayerEntities.Add(orphanedEntity);
            dbContext.SoilLayerEntities.Add(persistentEntity);

            var soilLayer = new PipingSoilLayer(1)
            {
                StorageId = persistentEntity.SoilLayerEntityId
            };

            var collector = new PersistenceRegistry();
            collector.Register(persistentEntity, soilLayer);

            // Call
            collector.RemoveUntouched(dbContext);

            // Assert
            Assert.AreEqual(1, dbContext.SoilLayerEntities.Count());
            CollectionAssert.Contains(dbContext.SoilLayerEntities, persistentEntity);
            mocks.VerifyAll();
        }

        [Test]
        public void RemoveUntouched_SurfaceLineEntity_OrphanedEntityIsRemovedFromRingtoetsEntities()
        {
            // Setup
            var mocks = new MockRepository();
            IRingtoetsEntities dbContext = RingtoetsEntitiesHelper.CreateStub(mocks);
            mocks.ReplayAll();

            var orphanedEntity = new SurfaceLineEntity
            {
                SurfaceLineEntityId = 1
            };
            var persistentEntity = new SurfaceLineEntity
            {
                SurfaceLineEntityId = 2
            };
            dbContext.SurfaceLineEntities.Add(orphanedEntity);
            dbContext.SurfaceLineEntities.Add(persistentEntity);

            var surfaceLine = new RingtoetsPipingSurfaceLine
            {
                StorageId = persistentEntity.SurfaceLineEntityId
            };

            var collector = new PersistenceRegistry();
            collector.Register(persistentEntity, surfaceLine);

            // Call
            collector.RemoveUntouched(dbContext);

            // Assert
            Assert.AreEqual(1, dbContext.SurfaceLineEntities.Count());
            CollectionAssert.Contains(dbContext.SurfaceLineEntities, persistentEntity);
            mocks.VerifyAll();
        }

        [Test]
        public void RemoveUntouched_SurfaceLinePointEntity_OrphanedEntityIsRemovedFromRingtoetsEntities()
        {
            // Setup
            var mocks = new MockRepository();
            IRingtoetsEntities dbContext = RingtoetsEntitiesHelper.CreateStub(mocks);
            mocks.ReplayAll();

            var orphanedEntity = new SurfaceLinePointEntity
            {
                SurfaceLinePointEntityId = 1
            };
            var persistentEntity = new SurfaceLinePointEntity
            {
                SurfaceLinePointEntityId = 2
            };
            dbContext.SurfaceLinePointEntities.Add(orphanedEntity);
            dbContext.SurfaceLinePointEntities.Add(persistentEntity);

            var geometryPoint = new Point3D(1, 2, 3)
            {
                StorageId = persistentEntity.SurfaceLinePointEntityId
            };

            var collector = new PersistenceRegistry();
            collector.Register(persistentEntity, geometryPoint);

            // Call
            collector.RemoveUntouched(dbContext);

            // Assert
            Assert.AreEqual(1, dbContext.SurfaceLinePointEntities.Count());
            CollectionAssert.Contains(dbContext.SurfaceLinePointEntities, persistentEntity);
            mocks.VerifyAll();
        }

        [Test]
        public void RemoveUntouched_CharacteristicPointEntity_OrphanedEntityIsRemovedFromRingtoetsEntities()
        {
            // Setup
            var mocks = new MockRepository();
            IRingtoetsEntities dbContext = RingtoetsEntitiesHelper.CreateStub(mocks);
            mocks.ReplayAll();

            var orphanedEntity = new CharacteristicPointEntity
            {
                CharacteristicPointEntityId = 1
            };
            var persistentEntity = new CharacteristicPointEntity
            {
                CharacteristicPointEntityId = 2
            };
            dbContext.CharacteristicPointEntities.Add(orphanedEntity);
            dbContext.CharacteristicPointEntities.Add(persistentEntity);

            var geometryPoint = new Point3D(1, 2, 3)
            {
                StorageId = 394624 // Note: ID only has to match a SurfaceLinePointEntity's id!
            };

            var collector = new PersistenceRegistry();
            collector.Register(persistentEntity, geometryPoint);

            // Call
            collector.RemoveUntouched(dbContext);

            // Assert
            Assert.AreEqual(1, dbContext.CharacteristicPointEntities.Count());
            CollectionAssert.Contains(dbContext.CharacteristicPointEntities, persistentEntity);
            mocks.VerifyAll();
        }

        #endregion
    }
}