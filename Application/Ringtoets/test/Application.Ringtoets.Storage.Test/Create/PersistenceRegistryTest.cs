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
        #region Contains methods

        [Test]
        public void Contains_WithoutModel_ArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate test = () => registry.Contains((PipingSoilProfile)null);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("model", paramName);
        }

        [Test]
        public void Contains_SoilProfileAdded_True()
        {
            // Setup
            var registry = new PersistenceRegistry();
            var profile = new TestPipingSoilProfile();
            registry.Register(new SoilProfileEntity(), profile);

            // Call
            var result = registry.Contains(profile);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void Contains_NoSoilProfileAdded_False()
        {
            // Setup
            var registry = new PersistenceRegistry();
            var profile = new TestPipingSoilProfile();

            // Call
            var result = registry.Contains(profile);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Contains_OtherSoilProfileAdded_False()
        {
            // Setup
            var registry = new PersistenceRegistry();
            var profile = new TestPipingSoilProfile();
            registry.Register(new SoilProfileEntity(), new TestPipingSoilProfile());

            // Call
            var result = registry.Contains(profile);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Contains_WithoutRingtoetsPipingSurfaceLine_ArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate test = () => registry.Contains((RingtoetsPipingSurfaceLine)null);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("model", paramName);
        }

        [Test]
        public void Contains_RingtoetsPipingSurfaceLineAdded_True()
        {
            // Setup
            var registry = new PersistenceRegistry();
            var surfaceLine = new RingtoetsPipingSurfaceLine();
            registry.Register(new SurfaceLineEntity(), surfaceLine);

            // Call
            bool result = registry.Contains(surfaceLine);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void Contains_NoRingtoetsPipingSurfaceLineAdded_False()
        {
            // Setup
            var registry = new PersistenceRegistry();
            var surfaceLine = new RingtoetsPipingSurfaceLine();

            // Call
            bool result = registry.Contains(surfaceLine);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Contains_OtherRingtoetsPipingSurfaceLineAdded_False()
        {
            // Setup
            var registry = new PersistenceRegistry();
            var surfaceLine = new RingtoetsPipingSurfaceLine();
            registry.Register(new SurfaceLineEntity(), new RingtoetsPipingSurfaceLine());

            // Call
            bool result = registry.Contains(surfaceLine);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Contains_WithoutHydraulicBoundaryLocation_ArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate test = () => registry.Contains((HydraulicBoundaryLocation)null);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("model", paramName);
        }

        [Test]
        public void Contains_HydraulicBoundaryLocationAdded_True()
        {
            // Setup
            var registry = new PersistenceRegistry();
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "A", 1, 2);
            registry.Register(new HydraulicLocationEntity(), hydraulicBoundaryLocation);

            // Call
            bool result = registry.Contains(hydraulicBoundaryLocation);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void Contains_NoHydraulicBoundaryLocationAdded_False()
        {
            // Setup
            var registry = new PersistenceRegistry();
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "A", 1, 2);

            // Call
            bool result = registry.Contains(hydraulicBoundaryLocation);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Contains_OtherHydraulicBoundaryLocationAdded_False()
        {
            // Setup
            var registry = new PersistenceRegistry();
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "A", 1, 2);
            registry.Register(new HydraulicLocationEntity(), new HydraulicBoundaryLocation(3, "B", 4, 5));

            // Call
            bool result = registry.Contains(hydraulicBoundaryLocation);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Contains_WithoutStochasticSoilModel_ArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate test = () => registry.Contains((StochasticSoilModel)null);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("model", paramName);
        }

        [Test]
        public void Contains_StochasticSoilModelAdded_True()
        {
            // Setup
            var registry = new PersistenceRegistry();
            var soilModel = new StochasticSoilModel(1, "A", "1");
            registry.Register(new StochasticSoilModelEntity(), soilModel);

            // Call
            bool result = registry.Contains(soilModel);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void Contains_NoStochasticSoilModelAdded_False()
        {
            // Setup
            var registry = new PersistenceRegistry();
            var soilModel = new StochasticSoilModel(1, "A", "1");

            // Call
            bool result = registry.Contains(soilModel);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Contains_OtherStochasticSoilModelAdded_False()
        {
            // Setup
            var registry = new PersistenceRegistry();
            var soilModel = new StochasticSoilModel(1, "A", "B");
            registry.Register(new StochasticSoilModelEntity(), new StochasticSoilModel(3, "B", "4"));

            // Call
            bool result = registry.Contains(soilModel);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Contains_WithoutStochasticSoilProfile_ArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate test = () => registry.Contains((StochasticSoilProfile)null);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("model", paramName);
        }

        [Test]
        public void Contains_StochasticSoilProfileAdded_True()
        {
            // Setup
            var registry = new PersistenceRegistry();
            var stochasticSoilProfile = new StochasticSoilProfile(0.4, SoilProfileType.SoilProfile1D, 1);
            registry.Register(new StochasticSoilProfileEntity(), stochasticSoilProfile);

            // Call
            bool result = registry.Contains(stochasticSoilProfile);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void Contains_NoStochasticSoilProfileAdded_False()
        {
            // Setup
            var registry = new PersistenceRegistry();
            var stochasticSoilProfile = new StochasticSoilProfile(0.4, SoilProfileType.SoilProfile1D, 1);

            // Call
            bool result = registry.Contains(stochasticSoilProfile);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Contains_OtherStochasticSoilProfileAdded_False()
        {
            // Setup
            var registry = new PersistenceRegistry();
            var stochasticSoilProfile = new StochasticSoilProfile(0.4, SoilProfileType.SoilProfile1D, 1);
            registry.Register(new StochasticSoilProfileEntity(), new StochasticSoilProfile(0.7, SoilProfileType.SoilProfile1D, 3));

            // Call
            bool result = registry.Contains(stochasticSoilProfile);

            // Assert
            Assert.IsFalse(result);
        }

        #endregion

        #region Get methods

        [Test]
        public void Get_WithoutModel_ArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate test = () => registry.Get((PipingSoilProfile)null);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("model", paramName);
        }

        [Test]
        public void Get_SoilProfileAdded_ReturnsEntity()
        {
            // Setup
            var registry = new PersistenceRegistry();
            var profile = new TestPipingSoilProfile();
            var entity = new SoilProfileEntity();
            registry.Register(entity, profile);

            // Call
            var result = registry.Get(profile);

            // Assert
            Assert.AreSame(entity, result);
        }

        [Test]
        public void Get_NoSoilProfileAdded_ThrowsInvalidOperationException()
        {
            // Setup
            var registry = new PersistenceRegistry();
            var profile = new TestPipingSoilProfile();

            // Call
            TestDelegate test = () => registry.Get(profile);

            // Assert
            Assert.Throws<InvalidOperationException>(test);
        }

        [Test]
        public void Get_OtherSoilProfileAdded_ThrowsInvalidOperationException()
        {
            // Setup
            var registry = new PersistenceRegistry();
            var profile = new TestPipingSoilProfile();
            registry.Register(new SoilProfileEntity(), new TestPipingSoilProfile());

            // Call
            TestDelegate test = () => registry.Get(profile);

            // Assert
            Assert.Throws<InvalidOperationException>(test);
        }

        [Test]
        public void Get_WithoutRingtoetsPipingSurfaceLine_ArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate test = () => registry.Get((RingtoetsPipingSurfaceLine)null);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("model", paramName);
        }

        [Test]
        public void Get_RingtoetsPipingSurfaceLineAdded_ReturnsEntity()
        {
            // Setup
            var registry = new PersistenceRegistry();
            var surfaceLine = new RingtoetsPipingSurfaceLine();
            var entity = new SurfaceLineEntity();
            registry.Register(entity, surfaceLine);

            // Call
            SurfaceLineEntity result = registry.Get(surfaceLine);

            // Assert
            Assert.AreSame(entity, result);
        }

        [Test]
        public void Get_NoRingtoetsPipingSurfaceLineAdded_ThrowsInvalidOperationException()
        {
            // Setup
            var registry = new PersistenceRegistry();
            var surfaceLine = new RingtoetsPipingSurfaceLine();

            // Call
            TestDelegate test = () => registry.Get(surfaceLine);

            // Assert
            Assert.Throws<InvalidOperationException>(test);
        }

        [Test]
        public void Get_OtherRingtoetsPipingSurfaceLineAdded_ThrowsInvalidOperationException()
        {
            // Setup
            var registry = new PersistenceRegistry();
            var surfaceLine = new RingtoetsPipingSurfaceLine();
            registry.Register(new SurfaceLineEntity(), new RingtoetsPipingSurfaceLine());

            // Call
            TestDelegate test = () => registry.Get(surfaceLine);

            // Assert
            Assert.Throws<InvalidOperationException>(test);
        }

        [Test]
        public void Get_WithoutHydraulicBoundaryLocation_ArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate test = () => registry.Get((HydraulicBoundaryLocation)null);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("model", paramName);
        }

        [Test]
        public void Get_HydraulicBoundaryLocationAdded_ReturnsEntity()
        {
            // Setup
            var registry = new PersistenceRegistry();
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(5, "6", 7, 8);
            var entity = new HydraulicLocationEntity();
            registry.Register(entity, hydraulicBoundaryLocation);

            // Call
            HydraulicLocationEntity result = registry.Get(hydraulicBoundaryLocation);

            // Assert
            Assert.AreSame(entity, result);
        }

        [Test]
        public void Get_NoHydraulicBoundaryLocationAdded_ThrowsInvalidOperationException()
        {
            // Setup
            var registry = new PersistenceRegistry();
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(5, "6", 7, 8);

            // Call
            TestDelegate test = () => registry.Get(hydraulicBoundaryLocation);

            // Assert
            Assert.Throws<InvalidOperationException>(test);
        }

        [Test]
        public void Get_OtherHydraulicBoundaryLocationAdded_ThrowsInvalidOperationException()
        {
            // Setup
            var registry = new PersistenceRegistry();
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(5, "6", 7, 8);
            registry.Register(new HydraulicLocationEntity(), new HydraulicBoundaryLocation(1, "2", 3, 4));

            // Call
            TestDelegate test = () => registry.Get(hydraulicBoundaryLocation);

            // Assert
            Assert.Throws<InvalidOperationException>(test);
        }

        [Test]
        public void Get_WithoutStochasticSoilModelEntity_ArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate test = () => registry.Get((StochasticSoilModel)null);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("model", paramName);
        }

        [Test]
        public void Get_StochasticSoilModelAdded_ReturnsEntity()
        {
            // Setup
            var registry = new PersistenceRegistry();
            var soilModel = new StochasticSoilModel(5, "6", "7");
            var entity = new StochasticSoilModelEntity();
            registry.Register(entity, soilModel);

            // Call
            StochasticSoilModelEntity result = registry.Get(soilModel);

            // Assert
            Assert.AreSame(entity, result);
        }

        [Test]
        public void Get_NoStochasticSoilModelAdded_ThrowsInvalidOperationException()
        {
            // Setup
            var registry = new PersistenceRegistry();
            var soilModel = new StochasticSoilModel(5, "6", "7");

            // Call
            TestDelegate test = () => registry.Get(soilModel);

            // Assert
            Assert.Throws<InvalidOperationException>(test);
        }

        [Test]
        public void Get_OtherStochasticSoilModelAdded_ThrowsInvalidOperationException()
        {
            // Setup
            var registry = new PersistenceRegistry();
            var soilModel = new StochasticSoilModel(5, "6", "7");
            registry.Register(new StochasticSoilModelEntity(), new StochasticSoilModel(1, "2", "3"));

            // Call
            TestDelegate test = () => registry.Get(soilModel);

            // Assert
            Assert.Throws<InvalidOperationException>(test);
        }

        [Test]
        public void Get_WithoutStochasticSoilProfileEntity_ArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate test = () => registry.Get((StochasticSoilProfile)null);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("model", paramName);
        }

        [Test]
        public void Get_StochasticSoilProfileAdded_ReturnsEntity()
        {
            // Setup
            var registry = new PersistenceRegistry();
            var stochasticSoilProfile = new StochasticSoilProfile(0.2, SoilProfileType.SoilProfile1D, 1);
            var entity = new StochasticSoilProfileEntity();
            registry.Register(entity, stochasticSoilProfile);

            // Call
            StochasticSoilProfileEntity result = registry.Get(stochasticSoilProfile);

            // Assert
            Assert.AreSame(entity, result);
        }

        [Test]
        public void Get_NoStochasticSoilProfileAdded_ThrowsInvalidOperationException()
        {
            // Setup
            var registry = new PersistenceRegistry();
            var stochasticSoilProfile = new StochasticSoilProfile(0.2, SoilProfileType.SoilProfile1D, 1);

            // Call
            TestDelegate test = () => registry.Get(stochasticSoilProfile);

            // Assert
            Assert.Throws<InvalidOperationException>(test);
        }

        [Test]
        public void Get_OtherStochasticSoilProfileAdded_ThrowsInvalidOperationException()
        {
            // Setup
            var registry = new PersistenceRegistry();
            var stochasticSoilProfile = new StochasticSoilProfile(0.2, SoilProfileType.SoilProfile1D, 1);
            registry.Register(new StochasticSoilProfileEntity(), new StochasticSoilProfile(0.4, SoilProfileType.SoilProfile1D, 2));

            // Call
            TestDelegate test = () => registry.Get(stochasticSoilProfile);

            // Assert
            Assert.Throws<InvalidOperationException>(test);
        }

        #endregion

        [Test]
        public void GetSurfaceLinePoint_SurfaceLinePointAdded_ReturnsEntity()
        {
            // Setup
            var surfaceLineGeometryPoint = new Point3D(1.1, 2.2, 3.3);
            var initializedEntity = new SurfaceLinePointEntity();

            var registry = new PersistenceRegistry();
            registry.Register(initializedEntity, surfaceLineGeometryPoint);

            // Call
            SurfaceLinePointEntity retrievedEntity = registry.GetSurfaceLinePoint(surfaceLineGeometryPoint);

            // Assert
            Assert.AreSame(initializedEntity, retrievedEntity);

        }

        #region Register methods

        [Test]
        public void Register_WithNullProjectEntity_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate test = () => registry.Register(null, new Project());

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("entity", paramName);
        }

        [Test]
        public void Register_WithNullProject_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate test = () => registry.Register(new ProjectEntity(), null);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("model", paramName);
        }

        [Test]
        public void Register_WithNullAssessmentSectionEntity_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();
            
            // Call
            TestDelegate test = () => registry.Register(null, new AssessmentSection(AssessmentSectionComposition.Dike));

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("entity", paramName);
        }

        [Test]
        public void Register_WithNullAssessmentSection_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();
            
            // Call
            TestDelegate test = () => registry.Register(new AssessmentSectionEntity(), null);

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
            var registry = new PersistenceRegistry();
            
            // Call
            TestDelegate test = () => registry.Register(null, model);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("entity", paramName);
        }

        [Test]
        public void Register_WithNullFailureMechanismBase_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();
            
            // Call
            TestDelegate test = () => registry.Register(new FailureMechanismEntity(), null);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("model", paramName);
        }

        [Test]
        public void Register_WithNullFailureMechanismSection_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();
            
            // Call
            TestDelegate test = () => registry.Register(new FailureMechanismSectionEntity(), null);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("model", paramName);
        }

        [Test]
        public void Register_WithNullHydraulicLocationEntity_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate test = () => registry.Register(null, new HydraulicBoundaryLocation(-1, "name", 0, 0));

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("entity", paramName);
        }

        [Test]
        public void Register_WithNullHydraulicBoundaryLocation_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate test = () => registry.Register(new HydraulicLocationEntity(), null);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("model", paramName);
        }

        [Test]
        public void Register_WithNullCalculationGroupEntity_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate test = () => registry.Register(null, new CalculationGroup());

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("entity", paramName);
        }

        [Test]
        public void Register_WithNullCalculationGroup_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate test = () => registry.Register(new CalculationGroupEntity(), null);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("model", paramName);
        }

        [Test]
        public void Register_WithNullPipingCalculationEntity_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate test = () => registry.Register(null, new PipingCalculationScenario(new GeneralPipingInput()));

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("entity", paramName);
        }

        [Test]
        public void Register_WithNullPipingCalculationScenario_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate test = () => registry.Register(new PipingCalculationEntity(), null);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("model", paramName);
        }

        [Test]
        public void Register_WithNullPipingCalculationOutputEntity_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate test = () => registry.Register(null, new PipingOutput(1,1,1,1,1,1));

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("entity", paramName);
        }

        [Test]
        public void Register_WithNullPipingOutput_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate test = () => registry.Register(new PipingCalculationOutputEntity(), null);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("model", paramName);
        }

        [Test]
        public void Register_WithNullPipingSemiProbabilisticOutputEntity_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();

            PipingSemiProbabilisticOutput output = new PipingSemiProbabilisticOutput(1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1);

            // Call
            TestDelegate test = () => registry.Register(null, output);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("entity", paramName);
        }

        [Test]
        public void Register_WithNullPipingSemiProbabilisticOutput_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate test = () => registry.Register(new PipingSemiProbabilisticOutputEntity(), null);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("model", paramName);
        }

        [Test]
        public void Register_WithNullStochasticSoilModelEntity_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate test = () => registry.Register(null, new TestStochasticSoilModel());

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("entity", paramName);
        }

        [Test]
        public void Register_WithNullStochasticSoilModel_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate test = () => registry.Register(new StochasticSoilModelEntity(), null);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("model", paramName);
        }

        [Test]
        public void Register_WithNullStochasticSoilProfileEntity_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate test = () => registry.Register(null, new StochasticSoilProfile(1, SoilProfileType.SoilProfile1D, -1));

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("entity", paramName);
        }

        [Test]
        public void Register_WithNullStochasticSoilProfile_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate test = () => registry.Register(new StochasticSoilProfileEntity(), null);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("model", paramName);
        }

        [Test]
        public void Register_WithNullSoilProfileEntity_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate test = () => registry.Register(null, new PipingSoilProfile("name", 0, new [] { new PipingSoilLayer(1) }, SoilProfileType.SoilProfile1D, -1));

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("entity", paramName);
        }

        [Test]
        public void Register_WithNullPipingSoilProfile_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate test = () => registry.Register(new SoilProfileEntity(), null);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("model", paramName);
        }

        [Test]
        public void Register_WithNullSoilLayerEntity_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate test = () => registry.Register(null, new PipingSoilLayer(0));

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("entity", paramName);
        }

        [Test]
        public void Register_WithNullPipingSoilLayer_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate test = () => registry.Register(new SoilLayerEntity(), null);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("model", paramName);
        }

        [Test]
        public void Register_WithNullSurfaceLinePointEntity_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate call = () => registry.Register((SurfaceLinePointEntity)null, new Point3D(1.1, 2.2, 3.3));

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("entity", paramName);
        }

        [Test]
        public void Register_WithNullSurfaceLinePoint_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate call = () => registry.Register(new SurfaceLinePointEntity(), null);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("model", paramName);
        }

        [Test]
        public void Register_WithNullCharacteristicPointEntity_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate call = () => registry.Register((CharacteristicPointEntity)null, new Point3D(1.1, 2.2, 3.3));

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("entity", paramName);
        }

        [Test]
        public void Register_WithNullPoint3DForCharacteristicPoint_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate call = () => registry.Register(new CharacteristicPointEntity(), null);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("model", paramName);
        }

        [Test]
        public void Register_WithNullPipingFailureMechanismMetaEntity_ThrowsArgumentNullExcpetion()
        {
            // Setup
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate call = () => registry.Register(null, new PipingProbabilityAssessmentInput());

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("entity", paramName);
        }

        [Test]
        public void Register_WithNullPipingProbabilityAssessmentInput_ThrowsArgumentNullExcpetion()
        {
            // Setup
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate call = () => registry.Register(new PipingFailureMechanismMetaEntity(), null);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("model", paramName);
        }

        #endregion

        #region TransferIds method

        [Test]
        public void TransferIds_WithProjectEntityAdded_EqualProjectEntityIdAndProjectStorageId()
        {
            // Setup
            var registry = new PersistenceRegistry();

            long storageId = new Random(21).Next(1,4000);
            var entity = new ProjectEntity
            {
                ProjectEntityId = storageId
            };
            var model = new Project();
            registry.Register(entity, model);

            // Call
            registry.TransferIds();

            // Assert
            Assert.AreEqual(storageId, model.StorageId);
        }

        [Test]
        public void TransferIds_WithAssessmentSectionEntityAdded_EqualAssessmentSectionEntityIdAndAssessmentSectionStorageId()
        {
            // Setup
            var registry = new PersistenceRegistry();

            long storageId = new Random(21).Next(1,4000);
            var entity = new AssessmentSectionEntity
            {
                AssessmentSectionEntityId = storageId
            };
            var model = new AssessmentSection(AssessmentSectionComposition.Dike);
            registry.Register(entity, model);

            // Call
            registry.TransferIds();

            // Assert
            Assert.AreEqual(storageId, model.StorageId);
        }

        [Test]
        public void TransferIds_WithFailureMechanismEntityAddedWithPipingFailureMechanism_EqualFailureMechanismEntityIdAndPipingFailureMechanismStorageId()
        {
            // Setup
            var registry = new PersistenceRegistry();

            long storageId = new Random(21).Next(1,4000);
            var entity = new FailureMechanismEntity
            {
                FailureMechanismEntityId = storageId
            };
            var model = new PipingFailureMechanism();
            registry.Register(entity, model);

            // Call
            registry.TransferIds();

            // Assert
            Assert.AreEqual(storageId, model.StorageId);
        }

        [Test]
        public void TransferIds_WithFailureMechanismEntityAddedWithStandAloneFailureMechanism_EqualFailureMechanismEntityIdAndStandAloneFailureMechanismStorageId()
        {
            // Setup
            var registry = new PersistenceRegistry();

            long storageId = new Random(21).Next(1,4000);
            var entity = new FailureMechanismEntity
            {
                FailureMechanismEntityId = storageId
            };
            var model = new MacrostabilityInwardsFailureMechanism();
            registry.Register(entity, model);

            // Call
            registry.TransferIds();

            // Assert
            Assert.AreEqual(storageId, model.StorageId);
        }

        [Test]
        public void TransferIds_WithFailureMechanismSectionEntityAddedWithFailureMechanismSection_EqualFailureMechanismSectionEntityIdAndFailureMechanismSectionStorageId()
        {
            // Setup
            var registry = new PersistenceRegistry();

            long storageId = new Random(21).Next(1,4000);
            var entity = new FailureMechanismSectionEntity
            {
                FailureMechanismSectionEntityId = storageId
            };
            var model = new FailureMechanismSection("name", new [] { new Point2D(0,0) });
            registry.Register(entity, model);

            // Call
            registry.TransferIds();

            // Assert
            Assert.AreEqual(storageId, model.StorageId);
        }

        [Test]
        public void TransferIds_WithHydraulicLocationEntityAdded_EqualHydraulicLocationEntityIdAndHydraulicBoundaryLocationStorageId()
        {
            // Setup
            var registry = new PersistenceRegistry();

            long storageId = new Random(21).Next(1,4000);
            var entity = new HydraulicLocationEntity
            {
                HydraulicLocationEntityId = storageId
            };
            var model = new HydraulicBoundaryLocation(-1, "name", 0, 0);
            registry.Register(entity, model);

            // Call
            registry.TransferIds();

            // Assert
            Assert.AreEqual(storageId, model.StorageId);
        }

        [Test]
        public void TransferIds_WithCalculationGroupEntityAdded_EqualCalculationGroupEntityIdAndCalculationGroupStorageId()
        {
            // Setup
            var registry = new PersistenceRegistry();

            long storageId = new Random(21).Next(1, 4000);
            var entity = new CalculationGroupEntity
            {
                CalculationGroupEntityId = storageId
            };
            var model = new CalculationGroup();
            registry.Register(entity, model);

            // Call
            registry.TransferIds();

            // Assert
            Assert.AreEqual(storageId, model.StorageId);
        }

        [Test]
        public void TransferIds_WithPipingCalculationEntityAdded_EqualPipingCalculationEntityIdAndPipingCalculationScenarioStorageId()
        {
            // Setup
            var registry = new PersistenceRegistry();

            long storageId = new Random(21).Next(1, 4000);
            var entity = new PipingCalculationEntity
            {
                PipingCalculationEntityId = storageId
            };
            var model = new PipingCalculationScenario(new GeneralPipingInput());
            registry.Register(entity, model);

            // Call
            registry.TransferIds();

            // Assert
            Assert.AreEqual(storageId, model.StorageId);
        }

        [Test]
        public void TransferIds_WithPipingCalculationOutputEntityAdded_EqualPipingCalculationOutputEntityIdAndPipingOutputStorageId()
        {
            // Setup
            var registry = new PersistenceRegistry();

            long storageId = new Random(21).Next(1, 4000);
            var entity = new PipingCalculationOutputEntity
            {
                PipingCalculationOutputEntityId = storageId
            };
            var model = new PipingOutput(1,2,3,4,5,6);
            registry.Register(entity, model);

            // Call
            registry.TransferIds();

            // Assert
            Assert.AreEqual(storageId, model.StorageId);
        }

        [Test]
        public void TransferIds_WithPipingSemiProbabilisticOutputEntityAdded_EqualPipingSemiProbabilisticOutputEntityIdAndPipingSemiProbabilisticOutputStorageId()
        {
            // Setup
            var registry = new PersistenceRegistry();

            long storageId = new Random(21).Next(1, 4000);
            var entity = new PipingSemiProbabilisticOutputEntity
            {
                PipingSemiProbabilisticOutputEntityId = storageId
            };
            var model = new PipingSemiProbabilisticOutput(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14);
            registry.Register(entity, model);

            // Call
            registry.TransferIds();

            // Assert
            Assert.AreEqual(storageId, model.StorageId);
        }

        [Test]
        public void TransferIds_WithStochasticSoilModelEntityAdded_EqualStochasticSoilModelEntityIdAndStochasticSoilModelStorageId()
        {
            // Setup
            var registry = new PersistenceRegistry();

            long storageId = new Random(21).Next(1,4000);
            var entity = new StochasticSoilModelEntity
            {
                StochasticSoilModelEntityId = storageId
            };
            var model = new StochasticSoilModel(-1, "name", "name");
            registry.Register(entity, model);

            // Call
            registry.TransferIds();

            // Assert
            Assert.AreEqual(storageId, model.StorageId);
        }

        [Test]
        public void TransferIds_WithStochasticSoilProfileEntityAdded_EqualStochasticSoilProfileEntityIdAndStochasticSoilProfileStorageId()
        {
            // Setup
            var registry = new PersistenceRegistry();

            long storageId = new Random(21).Next(1,4000);
            var entity = new StochasticSoilProfileEntity
            {
                StochasticSoilProfileEntityId = storageId
            };
            var model = new StochasticSoilProfile(1, SoilProfileType.SoilProfile1D, -1);
            registry.Register(entity, model);

            // Call
            registry.TransferIds();

            // Assert
            Assert.AreEqual(storageId, model.StorageId);
        }

        [Test]
        public void TransferIds_WithSoilProfileEntityAdded_EqualSoilProfileEntityIdAndPipingSoilProfileStorageId()
        {
            // Setup
            var registry = new PersistenceRegistry();

            long storageId = new Random(21).Next(1,4000);
            var entity = new SoilProfileEntity
            {
                SoilProfileEntityId = storageId
            };
            var model = new PipingSoilProfile("name", 0, new [] { new PipingSoilLayer(1) }, SoilProfileType.SoilProfile1D, -1);
            registry.Register(entity, model);

            // Call
            registry.TransferIds();

            // Assert
            Assert.AreEqual(storageId, model.StorageId);
        }

        [Test]
        public void TransferIds_WithSoilLayerEntityAdded_EqualSoilLayerEntityIdAndPipingSoilLayerStorageId()
        {
            // Setup
            var registry = new PersistenceRegistry();

            long storageId = new Random(21).Next(1,4000);
            var entity = new SoilLayerEntity
            {
                SoilLayerEntityId = storageId
            };
            var model = new PipingSoilLayer(0);
            registry.Register(entity, model);

            // Call
            registry.TransferIds();

            // Assert
            Assert.AreEqual(storageId, model.StorageId);
        }

        [Test]
        public void TransferIds_WithSurfaceLineEntityAdded_EqualSurfaceLineEntityIdAndRingtoetsPipingSurfaceLineStorageId()
        {
            // Setup
            var registry = new PersistenceRegistry();

            long storageId = new Random(21).Next(1, 4000);
            var entity = new SurfaceLineEntity
            {
                SurfaceLineEntityId = storageId
            };
            var model = new RingtoetsPipingSurfaceLine();
            registry.Register(entity, model);

            // Call
            registry.TransferIds();

            // Assert
            Assert.AreEqual(storageId, model.StorageId);
        }

        [Test]
        public void TransferIds_WithSurfaceLinePointEntityAdded_EqualSurfaceLinePointEntityIdAndPoint3DStorageId()
        {
            // Setup
            var registry = new PersistenceRegistry();

            long storageId = new Random(21).Next(1, 4000);
            var entity = new SurfaceLinePointEntity
            {
                SurfaceLinePointEntityId = storageId
            };
            var model = new Point3D(1.1, 2.2, 3.3);
            registry.Register(entity, model);

            // Call
            registry.TransferIds();

            // Assert
            Assert.AreEqual(storageId, model.StorageId);
        }

        [Test]
        public void TransferIds_WithPipingFailureMechanismMetaEntityAdded_EqualFailureMechanismMetaEntityIdAndPipingProbabilityAssessmentInputStorageId()
        {
            // Setup
            var registry = new PersistenceRegistry();

            long storageId = new Random(21).Next(1, 4000);
            var entity = new PipingFailureMechanismMetaEntity
            {
                PipingFailureMechanismMetaEntityId = storageId
            };
            var model = new PipingProbabilityAssessmentInput();
            registry.Register(entity, model);

            // Call
            registry.TransferIds();

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

            var registry = new PersistenceRegistry();
            registry.Register(persistentEntity, project);

            // Call
            registry.RemoveUntouched(dbContext);

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

            var registry = new PersistenceRegistry();
            registry.Register(persistentEntity, assessmentSection);

            // Call
            registry.RemoveUntouched(dbContext);

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

            var registry = new PersistenceRegistry();
            registry.Register(persistentEntity, failureMechanismStub);

            // Call
            registry.RemoveUntouched(dbContext);

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

            var registry = new PersistenceRegistry();
            registry.Register(persistentEntity, section);

            // Call
            registry.RemoveUntouched(dbContext);

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

            var registry = new PersistenceRegistry();
            registry.Register(persistentEntity, boundaryLocation);

            // Call
            registry.RemoveUntouched(dbContext);

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

            var registry = new PersistenceRegistry();
            registry.Register(persistentEntity, calculationGroup);

            // Call
            registry.RemoveUntouched(dbContext);

            // Assert
            Assert.AreEqual(1, dbContext.CalculationGroupEntities.Count());
            CollectionAssert.Contains(dbContext.CalculationGroupEntities, persistentEntity);
            mocks.VerifyAll();
        }

        [Test]
        public void RemoveUntouched_PipingCalculationEntity_OrphanedEntityIsRemovedFromRingtoetsEntities()
        {
            // Setup
            var mocks = new MockRepository();
            IRingtoetsEntities dbContext = RingtoetsEntitiesHelper.CreateStub(mocks);
            mocks.ReplayAll();

            var orphanedEntity = new PipingCalculationEntity
            {
                PipingCalculationEntityId = 1
            };
            var persistentEntity = new PipingCalculationEntity
            {
                PipingCalculationEntityId = 2
            };
            dbContext.PipingCalculationEntities.Add(orphanedEntity);
            dbContext.PipingCalculationEntities.Add(persistentEntity);

            var calculation = new PipingCalculationScenario(new GeneralPipingInput())
            {
                StorageId = persistentEntity.PipingCalculationEntityId
            };

            var registry = new PersistenceRegistry();
            registry.Register(persistentEntity, calculation);

            // Call
            registry.RemoveUntouched(dbContext);

            // Assert
            Assert.AreEqual(1, dbContext.PipingCalculationEntities.Count());
            CollectionAssert.Contains(dbContext.PipingCalculationEntities, persistentEntity);
            mocks.VerifyAll();
        }

        [Test]
        public void RemoveUntouched_PipingCalculationOutputEntity_OrphanedEntityIsRemovedFromRingtoetsEntities()
        {
            // Setup
            var mocks = new MockRepository();
            IRingtoetsEntities dbContext = RingtoetsEntitiesHelper.CreateStub(mocks);
            mocks.ReplayAll();

            var orphanedEntity = new PipingCalculationOutputEntity
            {
                PipingCalculationOutputEntityId = 1
            };
            var persistentEntity = new PipingCalculationOutputEntity
            {
                PipingCalculationOutputEntityId = 2
            };
            dbContext.PipingCalculationOutputEntities.Add(orphanedEntity);
            dbContext.PipingCalculationOutputEntities.Add(persistentEntity);

            var pipingOutput = new PipingOutput(1,2,3,4,5,6)
            {
                StorageId = persistentEntity.PipingCalculationOutputEntityId
            };

            var registry = new PersistenceRegistry();
            registry.Register(persistentEntity, pipingOutput);

            // Call
            registry.RemoveUntouched(dbContext);

            // Assert
            Assert.AreEqual(1, dbContext.PipingCalculationOutputEntities.Count());
            CollectionAssert.Contains(dbContext.PipingCalculationOutputEntities, persistentEntity);
            mocks.VerifyAll();
        }

        [Test]
        public void RemoveUntouched_PipingSemiProbabilisticOutputEntity_OrphanedEntityIsRemovedFromRingtoetsEntities()
        {
            // Setup
            var mocks = new MockRepository();
            IRingtoetsEntities dbContext = RingtoetsEntitiesHelper.CreateStub(mocks);
            mocks.ReplayAll();

            var orphanedEntity = new PipingSemiProbabilisticOutputEntity
            {
                PipingSemiProbabilisticOutputEntityId = 1
            };
            var persistentEntity = new PipingSemiProbabilisticOutputEntity
            {
                PipingSemiProbabilisticOutputEntityId = 2
            };
            dbContext.PipingSemiProbabilisticOutputEntities.Add(orphanedEntity);
            dbContext.PipingSemiProbabilisticOutputEntities.Add(persistentEntity);

            var pipingSemiProbabilisticOutput = new PipingSemiProbabilisticOutput(1, 2, 3, 4, 5, 6, 7,
                                                                     8, 9, 10, 11, 12, 13, 14)
            {
                StorageId = persistentEntity.PipingSemiProbabilisticOutputEntityId
            };

            var registry = new PersistenceRegistry();
            registry.Register(persistentEntity, pipingSemiProbabilisticOutput);

            // Call
            registry.RemoveUntouched(dbContext);

            // Assert
            Assert.AreEqual(1, dbContext.PipingSemiProbabilisticOutputEntities.Count());
            CollectionAssert.Contains(dbContext.PipingSemiProbabilisticOutputEntities, persistentEntity);
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

            var registry = new PersistenceRegistry();
            registry.Register(persistentEntity, soilModel);

            // Call
            registry.RemoveUntouched(dbContext);

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

            var registry = new PersistenceRegistry();
            registry.Register(persistentEntity, stochasticSoilProfile);

            // Call
            registry.RemoveUntouched(dbContext);

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

            var registry = new PersistenceRegistry();
            registry.Register(persistentEntity, soilProfile);

            // Call
            registry.RemoveUntouched(dbContext);

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

            var registry = new PersistenceRegistry();
            registry.Register(persistentEntity, soilLayer);

            // Call
            registry.RemoveUntouched(dbContext);

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

            var registry = new PersistenceRegistry();
            registry.Register(persistentEntity, surfaceLine);

            // Call
            registry.RemoveUntouched(dbContext);

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

            var registry = new PersistenceRegistry();
            registry.Register(persistentEntity, geometryPoint);

            // Call
            registry.RemoveUntouched(dbContext);

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

            var registry = new PersistenceRegistry();
            registry.Register(persistentEntity, geometryPoint);

            // Call
            registry.RemoveUntouched(dbContext);

            // Assert
            Assert.AreEqual(1, dbContext.CharacteristicPointEntities.Count());
            CollectionAssert.Contains(dbContext.CharacteristicPointEntities, persistentEntity);
            mocks.VerifyAll();
        }

        [Test]
        public void RemoveUntouched_PipingFailureMechanismMetaEntity_OrphanedEntityIsRemovedFromRingtoetsEntities()
        {
            // Setup
            var mocks = new MockRepository();
            IRingtoetsEntities dbContext = RingtoetsEntitiesHelper.CreateStub(mocks);
            mocks.ReplayAll();

            var orphanedEntity = new PipingFailureMechanismMetaEntity
            {
                PipingFailureMechanismMetaEntityId = 1
            };
            var persistentEntity = new PipingFailureMechanismMetaEntity
            {
                PipingFailureMechanismMetaEntityId = 2
            };
            dbContext.PipingFailureMechanismMetaEntities.Add(orphanedEntity);
            dbContext.PipingFailureMechanismMetaEntities.Add(persistentEntity);

            var model = new PipingProbabilityAssessmentInput
            {
                StorageId = persistentEntity.PipingFailureMechanismMetaEntityId
            };

            var registry = new PersistenceRegistry();
            registry.Register(persistentEntity, model);

            // Call
            registry.RemoveUntouched(dbContext);

            // Assert
            Assert.AreEqual(1, dbContext.PipingFailureMechanismMetaEntities.Count());
            CollectionAssert.Contains(dbContext.PipingFailureMechanismMetaEntities, persistentEntity);
            mocks.VerifyAll();
        }

        #endregion
    }
}