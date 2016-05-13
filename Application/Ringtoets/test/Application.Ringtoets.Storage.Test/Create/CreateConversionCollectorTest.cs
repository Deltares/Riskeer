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
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
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
    public class CreateConversionCollectorTest
    {
        [Test]
        public void Contains_WithoutModel_ArgumentNullException()
        {
            // Setup
            var collector = new CreateConversionCollector();

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
            var collector = new CreateConversionCollector();
            var profile = new TestPipingSoilProfile();
            collector.Create(new SoilProfileEntity(), profile);

            // Call
            var result = collector.Contains(profile);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void Contains_NoSoilProfileAdded_False()
        {
            // Setup
            var collector = new CreateConversionCollector();
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
            var collector = new CreateConversionCollector();
            var profile = new TestPipingSoilProfile();
            collector.Create(new SoilProfileEntity(), new TestPipingSoilProfile());

            // Call
            var result = collector.Contains(profile);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Get_WithoutModel_ArgumentNullException()
        {
            // Setup
            var collector = new CreateConversionCollector();

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
            var collector = new CreateConversionCollector();
            var profile = new TestPipingSoilProfile();
            var entity = new SoilProfileEntity();
            collector.Create(entity, profile);

            // Call
            var result = collector.Get(profile);

            // Assert
            Assert.AreSame(entity, result);
        }

        [Test]
        public void Get_NoSoilProfileAdded_ThrowsInvalidOperationException()
        {
            // Setup
            var collector = new CreateConversionCollector();
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
            var collector = new CreateConversionCollector();
            var profile = new TestPipingSoilProfile();
            collector.Create(new SoilProfileEntity(), new TestPipingSoilProfile());

            // Call
            TestDelegate test = () => collector.Get(profile);

            // Assert
            Assert.Throws<InvalidOperationException>(test);
        }

        #region Create methods

        [Test]
        public void Create_WithNullProjectEntity_ThrowsArgumentNullException()
        {
            // Setup
            var collector = new CreateConversionCollector();

            // Call
            TestDelegate test = () => collector.Create(null, new Project());

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("entity", paramName);
        }

        [Test]
        public void Create_WithNullProject_ThrowsArgumentNullException()
        {
            // Setup
            var collector = new CreateConversionCollector();

            // Call
            TestDelegate test = () => collector.Create(new ProjectEntity(), null);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("model", paramName);
        }

        [Test]
        public void Create_WithNullAssessmentSectionEntity_ThrowsArgumentNullException()
        {
            // Setup
            var collector = new CreateConversionCollector();
            
            // Call
            TestDelegate test = () => collector.Create(null, new AssessmentSection(AssessmentSectionComposition.Dike));

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("entity", paramName);
        }

        [Test]
        public void Create_WithNullAssessmentSection_ThrowsArgumentNullException()
        {
            // Setup
            var collector = new CreateConversionCollector();
            
            // Call
            TestDelegate test = () => collector.Create(new AssessmentSectionEntity(), null);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("model", paramName);
        }

        [Test]
        public void Create_WithNullFailureMechanismEntity_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var model = mocks.StrictMock<IFailureMechanism>();
            var collector = new CreateConversionCollector();
            
            // Call
            TestDelegate test = () => collector.Create(null, model);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("entity", paramName);
        }

        [Test]
        public void Create_WithNullFailureMechanismBase_ThrowsArgumentNullException()
        {
            // Setup
            var collector = new CreateConversionCollector();
            
            // Call
            TestDelegate test = () => collector.Create(new FailureMechanismEntity(), null);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("model", paramName);
        }

        [Test]
        public void Create_WithNullFailureMechanismSection_ThrowsArgumentNullException()
        {
            // Setup
            var collector = new CreateConversionCollector();
            
            // Call
            TestDelegate test = () => collector.Create(new FailureMechanismSectionEntity(), null);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("model", paramName);
        }

        [Test]
        public void Create_WithNullHydraulicLocationEntity_ThrowsArgumentNullException()
        {
            // Setup
            var collector = new CreateConversionCollector();

            // Call
            TestDelegate test = () => collector.Create(null, new HydraulicBoundaryLocation(-1, "name", 0, 0));

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("entity", paramName);
        }

        [Test]
        public void Create_WithNullHydraulicBoundaryLocation_ThrowsArgumentNullException()
        {
            // Setup
            var collector = new CreateConversionCollector();

            // Call
            TestDelegate test = () => collector.Create(new HydraulicLocationEntity(), null);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("model", paramName);
        }

        [Test]
        public void Create_WithNullStochasticSoilModelEntity_ThrowsArgumentNullException()
        {
            // Setup
            var collector = new CreateConversionCollector();

            // Call
            TestDelegate test = () => collector.Create(null, new TestStochasticSoilModel());

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("entity", paramName);
        }

        [Test]
        public void Create_WithNullStochasticSoilModel_ThrowsArgumentNullException()
        {
            // Setup
            var collector = new CreateConversionCollector();

            // Call
            TestDelegate test = () => collector.Create(new StochasticSoilModelEntity(), null);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("model", paramName);
        }

        [Test]
        public void Create_WithNullStochasticSoilProfileEntity_ThrowsArgumentNullException()
        {
            // Setup
            var collector = new CreateConversionCollector();

            // Call
            TestDelegate test = () => collector.Create(null, new StochasticSoilProfile(1, SoilProfileType.SoilProfile1D, -1));

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("entity", paramName);
        }

        [Test]
        public void Create_WithNullStochasticSoilProfile_ThrowsArgumentNullException()
        {
            // Setup
            var collector = new CreateConversionCollector();

            // Call
            TestDelegate test = () => collector.Create(new StochasticSoilProfileEntity(), null);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("model", paramName);
        }

        [Test]
        public void Create_WithNullSoilProfileEntity_ThrowsArgumentNullException()
        {
            // Setup
            var collector = new CreateConversionCollector();

            // Call
            TestDelegate test = () => collector.Create(null, new PipingSoilProfile("name", 0, new [] { new PipingSoilLayer(1) }, SoilProfileType.SoilProfile1D, -1));

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("entity", paramName);
        }

        [Test]
        public void Create_WithNullPipingSoilProfile_ThrowsArgumentNullException()
        {
            // Setup
            var collector = new CreateConversionCollector();

            // Call
            TestDelegate test = () => collector.Create(new SoilProfileEntity(), null);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("model", paramName);
        }

        [Test]
        public void Create_WithNullSoilLayerEntity_ThrowsArgumentNullException()
        {
            // Setup
            var collector = new CreateConversionCollector();

            // Call
            TestDelegate test = () => collector.Create(null, new PipingSoilLayer(0));

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("entity", paramName);
        }

        [Test]
        public void Create_WithNullPipingSoilLayer_ThrowsArgumentNullException()
        {
            // Setup
            var collector = new CreateConversionCollector();

            // Call
            TestDelegate test = () => collector.Create(new SoilLayerEntity(), null);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("model", paramName);
        }

        #endregion

        #region TransferId method

        [Test]
        public void TransferId_WithProjectEntityAdded_EqualProjectEntityIdAndProjectStorageId()
        {
            // Setup
            var collector = new CreateConversionCollector();

            long storageId = new Random(21).Next(1,4000);
            var entity = new ProjectEntity
            {
                ProjectEntityId = storageId
            };
            var model = new Project();
            collector.Create(entity, model);

            // Call
            collector.TransferIds();

            // Assert
            Assert.AreEqual(storageId, model.StorageId);
        }

        [Test]
        public void TransferId_WithAssessmentSectionEntityAdded_EqualAssessmentSectionEntityIdAndAssessmentSectionStorageId()
        {
            // Setup
            var collector = new CreateConversionCollector();

            long storageId = new Random(21).Next(1,4000);
            var entity = new AssessmentSectionEntity
            {
                AssessmentSectionEntityId = storageId
            };
            var model = new AssessmentSection(AssessmentSectionComposition.Dike);
            collector.Create(entity, model);

            // Call
            collector.TransferIds();

            // Assert
            Assert.AreEqual(storageId, model.StorageId);
        }

        [Test]
        public void TransferId_WithFailureMechanismEntityAddedWithPipingFailureMechanism_EqualFailureMechanismEntityIdAndPipingFailureMechanismStorageId()
        {
            // Setup
            var collector = new CreateConversionCollector();

            long storageId = new Random(21).Next(1,4000);
            var entity = new FailureMechanismEntity
            {
                FailureMechanismEntityId = storageId
            };
            var model = new PipingFailureMechanism();
            collector.Create(entity, model);

            // Call
            collector.TransferIds();

            // Assert
            Assert.AreEqual(storageId, model.StorageId);
        }

        [Test]
        public void TransferId_WithFailureMechanismEntityAddedWithStandAloneFailureMechanism_EqualFailureMechanismEntityIdAndStandAloneFailureMechanismStorageId()
        {
            // Setup
            var collector = new CreateConversionCollector();

            long storageId = new Random(21).Next(1,4000);
            var entity = new FailureMechanismEntity
            {
                FailureMechanismEntityId = storageId
            };
            var model = new StandAloneFailureMechanism("name", "code");
            collector.Create(entity, model);

            // Call
            collector.TransferIds();

            // Assert
            Assert.AreEqual(storageId, model.StorageId);
        }

        [Test]
        public void TransferId_WithFailureMechanismSectionEntityAddedWithFailureMechanismSection_EqualFailureMechanismSectionEntityIdAndFailureMechanismSectionStorageId()
        {
            // Setup
            var collector = new CreateConversionCollector();

            long storageId = new Random(21).Next(1,4000);
            var entity = new FailureMechanismSectionEntity
            {
                FailureMechanismSectionEntityId = storageId
            };
            var model = new FailureMechanismSection("name", new [] { new Point2D(0,0) });
            collector.Create(entity, model);

            // Call
            collector.TransferIds();

            // Assert
            Assert.AreEqual(storageId, model.StorageId);
        }

        [Test]
        public void TransferId_WithHydraulicLocationEntityAdded_EqualHydraulicLocationEntityIdAndHydraulicBoundaryLocationStorageId()
        {
            // Setup
            var collector = new CreateConversionCollector();

            long storageId = new Random(21).Next(1,4000);
            var entity = new HydraulicLocationEntity
            {
                HydraulicLocationEntityId = storageId
            };
            var model = new HydraulicBoundaryLocation(-1, "name", 0, 0);
            collector.Create(entity, model);

            // Call
            collector.TransferIds();

            // Assert
            Assert.AreEqual(storageId, model.StorageId);
        }

        [Test]
        public void TransferId_WithStochasticSoilModelEntityAdded_EqualStochasticSoilModelEntityIdAndStochasticSoilModelStorageId()
        {
            // Setup
            var collector = new CreateConversionCollector();

            long storageId = new Random(21).Next(1,4000);
            var entity = new StochasticSoilModelEntity
            {
                StochasticSoilModelEntityId = storageId
            };
            var model = new StochasticSoilModel(-1, "name", "name");
            collector.Create(entity, model);

            // Call
            collector.TransferIds();

            // Assert
            Assert.AreEqual(storageId, model.StorageId);
        }

        [Test]
        public void TransferId_WithStochasticSoilProfileEntityAdded_EqualStochasticSoilProfileEntityIdAndStochasticSoilProfileStorageId()
        {
            // Setup
            var collector = new CreateConversionCollector();

            long storageId = new Random(21).Next(1,4000);
            var entity = new StochasticSoilProfileEntity
            {
                StochasticSoilProfileEntityId = storageId
            };
            var model = new StochasticSoilProfile(1, SoilProfileType.SoilProfile1D, -1);
            collector.Create(entity, model);

            // Call
            collector.TransferIds();

            // Assert
            Assert.AreEqual(storageId, model.StorageId);
        }

        [Test]
        public void TransferId_WithSoilProfileEntityAdded_EqualSoilProfileEntityIdAndPipingSoilProfileStorageId()
        {
            // Setup
            var collector = new CreateConversionCollector();

            long storageId = new Random(21).Next(1,4000);
            var entity = new SoilProfileEntity
            {
                SoilProfileEntityId = storageId
            };
            var model = new PipingSoilProfile("name", 0, new [] { new PipingSoilLayer(1) }, SoilProfileType.SoilProfile1D, -1);
            collector.Create(entity, model);

            // Call
            collector.TransferIds();

            // Assert
            Assert.AreEqual(storageId, model.StorageId);
        }

        [Test]
        public void TransferId_WithSoilLayerEntityAdded_EqualSoilLayerEntityIdAndPipingSoilLayerStorageId()
        {
            // Setup
            var collector = new CreateConversionCollector();

            long storageId = new Random(21).Next(1,4000);
            var entity = new SoilLayerEntity
            {
                SoilLayerEntityId = storageId
            };
            var model = new PipingSoilLayer(0);
            collector.Create(entity, model);

            // Call
            collector.TransferIds();

            // Assert
            Assert.AreEqual(storageId, model.StorageId);
        }

        #endregion
    }
}