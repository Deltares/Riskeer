using System;
using Application.Ringtoets.Storage.Create;
using Application.Ringtoets.Storage.DbContext;
using Core.Common.Base.Data;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.HydraRing.Data;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.Data.Placeholders;
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
            collector.Add(new SoilProfileEntity(), profile);

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
            collector.Add(new SoilProfileEntity(), new TestPipingSoilProfile());

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
            collector.Add(entity, profile);

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
            collector.Add(new SoilProfileEntity(), new TestPipingSoilProfile());

            // Call
            TestDelegate test = () => collector.Get(profile);

            // Assert
            Assert.Throws<InvalidOperationException>(test);
        }

        #region Add methods

        [Test]
        public void Add_WithNullProjectEntity_ThrowsArgumentNullException()
        {
            // Setup
            var collector = new CreateConversionCollector();

            // Call
            TestDelegate test = () => collector.Add(null, new Project());

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("entity", paramName);
        }

        [Test]
        public void Add_WithNullProject_ThrowsArgumentNullException()
        {
            // Setup
            var collector = new CreateConversionCollector();

            // Call
            TestDelegate test = () => collector.Add(new ProjectEntity(), null);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("model", paramName);
        }

        [Test]
        public void Add_WithNullAssessmentSectionEntity_ThrowsArgumentNullException()
        {
            // Setup
            var collector = new CreateConversionCollector();
            
            // Call
            TestDelegate test = () => collector.Add(null, new AssessmentSection(AssessmentSectionComposition.Dike));

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("entity", paramName);
        }

        [Test]
        public void Add_WithNullAssessmentSection_ThrowsArgumentNullException()
        {
            // Setup
            var collector = new CreateConversionCollector();
            
            // Call
            TestDelegate test = () => collector.Add(new AssessmentSectionEntity(), null);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("model", paramName);
        }

        [Test]
        public void Add_WithNullFailureMechanismEntity_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var model = mocks.StrictMock<FailureMechanismBase>("name", "code");
            var collector = new CreateConversionCollector();
            
            // Call
            TestDelegate test = () => collector.Add(null, model);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("entity", paramName);
        }

        [Test]
        public void Add_WithNullFailureMechanismBase_ThrowsArgumentNullException()
        {
            // Setup
            var collector = new CreateConversionCollector();
            
            // Call
            TestDelegate test = () => collector.Add(new FailureMechanismEntity(), null);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("model", paramName);
        }

        [Test]
        public void Add_WithNullHydraulicLocationEntity_ThrowsArgumentNullException()
        {
            // Setup
            var collector = new CreateConversionCollector();

            // Call
            TestDelegate test = () => collector.Add(null, new HydraulicBoundaryLocation(-1, "name", 0, 0));

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("entity", paramName);
        }

        [Test]
        public void Add_WithNullHydraulicBoundaryLocation_ThrowsArgumentNullException()
        {
            // Setup
            var collector = new CreateConversionCollector();

            // Call
            TestDelegate test = () => collector.Add(new HydraulicLocationEntity(), null);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("model", paramName);
        }

        [Test]
        public void Add_WithNullStochasticSoilModelEntity_ThrowsArgumentNullException()
        {
            // Setup
            var collector = new CreateConversionCollector();

            // Call
            TestDelegate test = () => collector.Add(null, new TestStochasticSoilModel());

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("entity", paramName);
        }

        [Test]
        public void Add_WithNullStochasticSoilModel_ThrowsArgumentNullException()
        {
            // Setup
            var collector = new CreateConversionCollector();

            // Call
            TestDelegate test = () => collector.Add(new StochasticSoilModelEntity(), null);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("model", paramName);
        }

        [Test]
        public void Add_WithNullStochasticSoilProfileEntity_ThrowsArgumentNullException()
        {
            // Setup
            var collector = new CreateConversionCollector();

            // Call
            TestDelegate test = () => collector.Add(null, new StochasticSoilProfile(1, SoilProfileType.SoilProfile1D, -1));

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("entity", paramName);
        }

        [Test]
        public void Add_WithNullStochasticSoilProfile_ThrowsArgumentNullException()
        {
            // Setup
            var collector = new CreateConversionCollector();

            // Call
            TestDelegate test = () => collector.Add(new StochasticSoilProfileEntity(), null);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("model", paramName);
        }

        [Test]
        public void Add_WithNullSoilProfileEntity_ThrowsArgumentNullException()
        {
            // Setup
            var collector = new CreateConversionCollector();

            // Call
            TestDelegate test = () => collector.Add(null, new PipingSoilProfile("name", 0, new [] { new PipingSoilLayer(1) }, SoilProfileType.SoilProfile1D, -1));

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("entity", paramName);
        }

        [Test]
        public void Add_WithNullPipingSoilProfile_ThrowsArgumentNullException()
        {
            // Setup
            var collector = new CreateConversionCollector();

            // Call
            TestDelegate test = () => collector.Add(new SoilProfileEntity(), null);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("model", paramName);
        }

        [Test]
        public void Add_WithNullSoilLayerEntity_ThrowsArgumentNullException()
        {
            // Setup
            var collector = new CreateConversionCollector();

            // Call
            TestDelegate test = () => collector.Add(null, new PipingSoilLayer(0));

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("entity", paramName);
        }

        [Test]
        public void Add_WithNullPipingSoilLayer_ThrowsArgumentNullException()
        {
            // Setup
            var collector = new CreateConversionCollector();

            // Call
            TestDelegate test = () => collector.Add(new SoilLayerEntity(), null);

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
            collector.Add(entity, model);

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
            collector.Add(entity, model);

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
            collector.Add(entity, model);

            // Call
            collector.TransferIds();

            // Assert
            Assert.AreEqual(storageId, model.StorageId);
        }

        [Test]
        public void TransferId_WithFailureMechanismEntityAddedWithFailureMechanismPlaceholder_EqualFailureMechanismEntityIdAndFailureMechanismPlaceholderStorageId()
        {
            // Setup
            var collector = new CreateConversionCollector();

            long storageId = new Random(21).Next(1,4000);
            var entity = new FailureMechanismEntity
            {
                FailureMechanismEntityId = storageId
            };
            var model = new FailureMechanismPlaceholder("name");
            collector.Add(entity, model);

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
            collector.Add(entity, model);

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
            collector.Add(entity, model);

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
            collector.Add(entity, model);

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
            collector.Add(entity, model);

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
            collector.Add(entity, model);

            // Call
            collector.TransferIds();

            // Assert
            Assert.AreEqual(storageId, model.StorageId);
        }

        #endregion
    }
}