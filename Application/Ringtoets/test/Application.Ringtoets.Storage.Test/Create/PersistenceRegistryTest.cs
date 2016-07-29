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
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Probability;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.HeightStructures.Data;
using Ringtoets.HydraRing.Data;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.Data.StandAlone;
using Ringtoets.Integration.Data.StandAlone.SectionResults;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Data.TestUtil;
using Ringtoets.Piping.KernelWrapper.TestUtil;
using Ringtoets.Piping.Primitives;

namespace Application.Ringtoets.Storage.Test.Create
{
    [TestFixture]
    public class PersistenceRegistryTest
    {
        private static DikeProfile CreateDikeProfile()
        {
            return new DikeProfile(new Point2D(0, 0),
                                   new[]
                                   {
                                       new RoughnessPoint(new Point2D(1, 2), 0.75),
                                       new RoughnessPoint(new Point2D(3, 4), 0.75)
                                   },
                                   new[]
                                   {
                                       new Point2D(5, 6),
                                       new Point2D(7, 8)
                                   },
                                   null, new DikeProfile.ConstructionProperties());
        }

        #region Contains methods

        [Test]
        public void Contains_WithoutPipingSoilProfile_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate test = () => registry.Contains((PipingSoilProfile) null);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("model", paramName);
        }

        [Test]
        public void Contains_SoilProfileAdded_ReturnsTrue()
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
        public void Contains_NoSoilProfileAdded_ReturnsFalse()
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
        public void Contains_OtherSoilProfileAdded_ReturnsFalse()
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
        public void Contains_WithoutRingtoetsPipingSurfaceLine_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate test = () => registry.Contains((RingtoetsPipingSurfaceLine) null);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("model", paramName);
        }

        [Test]
        public void Contains_RingtoetsPipingSurfaceLineAdded_ReturnsTrue()
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
        public void Contains_NoRingtoetsPipingSurfaceLineAdded_ReturnsFalse()
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
        public void Contains_OtherRingtoetsPipingSurfaceLineAdded_ReturnsFalse()
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
        public void Contains_WithoutHydraulicBoundaryLocation_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate test = () => registry.Contains((HydraulicBoundaryLocation) null);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("model", paramName);
        }

        [Test]
        public void Contains_HydraulicBoundaryLocationAdded_ReturnsTrue()
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
        public void Contains_NoHydraulicBoundaryLocationAdded_ReturnsFalse()
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
        public void Contains_OtherHydraulicBoundaryLocationAdded_ReturnsFalse()
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
        public void Contains_WithoutStochasticSoilModel_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate test = () => registry.Contains((StochasticSoilModel) null);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("model", paramName);
        }

        [Test]
        public void Contains_StochasticSoilModelAdded_ReturnsTrue()
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
        public void Contains_NoStochasticSoilModelAdded_ReturnsFalse()
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
        public void Contains_OtherStochasticSoilModelAdded_ReturnsFalse()
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
        public void Contains_WithoutStochasticSoilProfile_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate test = () => registry.Contains((StochasticSoilProfile) null);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("model", paramName);
        }

        [Test]
        public void Contains_StochasticSoilProfileAdded_ReturnsTrue()
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
        public void Contains_NoStochasticSoilProfileAdded_ReturnsFalse()
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
        public void Contains_OtherStochasticSoilProfileAdded_ReturnsFalse()
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

        [Test]
        public void Contains_FailureMechanismSectionAdded_ReturnsTrue()
        {
            // Setup
            var registry = new PersistenceRegistry();
            var failureMechanismSection = new TestFailureMechanismSection();
            registry.Register(new FailureMechanismSectionEntity(), failureMechanismSection);

            // Call
            bool result = registry.Contains(failureMechanismSection);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void Contains_NoFailureMechanismSectionAdded_ReturnsFalse()
        {
            // Setup
            var registry = new PersistenceRegistry();
            var failureMechanismSection = new TestFailureMechanismSection();

            // Call
            bool result = registry.Contains(failureMechanismSection);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Contains_OtherFailureMechanismSectionAdded_ReturnsFalse()
        {
            // Setup
            var registry = new PersistenceRegistry();
            var failureMechanismSection = new TestFailureMechanismSection();
            registry.Register(new FailureMechanismSectionEntity(), new TestFailureMechanismSection());

            // Call
            bool result = registry.Contains(failureMechanismSection);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Contains_WithoutDikeProfile_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate call = () => registry.Contains((DikeProfile) null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("model", paramName);
        }

        [Test]
        public void Contains_DikeProfileAdded_ReturnsTrue()
        {
            // Setup
            var dikeProfile = new DikeProfile(new Point2D(0, 0), new RoughnessPoint[0], new Point2D[0],
                                              null, new DikeProfile.ConstructionProperties());
            var registry = new PersistenceRegistry();
            registry.Register(new DikeProfileEntity(), dikeProfile);

            // Call
            bool result = registry.Contains(dikeProfile);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void Contains_OtherDikeProfileAdded_ReturnsFalse()
        {
            // Setup
            var dikeProfile = new DikeProfile(new Point2D(0, 0), new RoughnessPoint[0], new Point2D[0],
                                              null, new DikeProfile.ConstructionProperties());

            var otherDikeProfile = new DikeProfile(new Point2D(1, 1), new RoughnessPoint[0], new Point2D[0],
                                                   null, new DikeProfile.ConstructionProperties());
            var registry = new PersistenceRegistry();
            registry.Register(new DikeProfileEntity(), otherDikeProfile);

            // Call
            bool result = registry.Contains(dikeProfile);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Contains_NoDikeProfileAdded_ReturnsFalse()
        {
            // Setup
            var dikeProfile = new DikeProfile(new Point2D(0, 0), new RoughnessPoint[0], new Point2D[0],
                                              null, new DikeProfile.ConstructionProperties());

            var registry = new PersistenceRegistry();

            // Call
            bool result = registry.Contains(dikeProfile);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Contains_WithoutGrassCoverErosionInwardsCalculation_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate call = () => registry.Contains((GrassCoverErosionInwardsCalculation) null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("model", paramName);
        }

        [Test]
        public void Contains_GrassCoverErosionInwardsCalculationAdded_ReturnsTrue()
        {
            // Setup
            var calculation = new GrassCoverErosionInwardsCalculation();
            var registry = new PersistenceRegistry();
            registry.Register(new GrassCoverErosionInwardsCalculationEntity(), calculation);

            // Call
            bool result = registry.Contains(calculation);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void Contains_OtherGrassCoverErosionInwardsCalculationAdded_ReturnsFalse()
        {
            // Setup
            var calculation = new GrassCoverErosionInwardsCalculation();

            var otherCalculation = new GrassCoverErosionInwardsCalculation();
            var registry = new PersistenceRegistry();
            registry.Register(new GrassCoverErosionInwardsCalculationEntity(), otherCalculation);

            // Call
            bool result = registry.Contains(calculation);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Contains_NoGrassCoverErosionInwardsCalculationAdded_ReturnsFalse()
        {
            // Setup
            var calculation = new GrassCoverErosionInwardsCalculation();

            var registry = new PersistenceRegistry();

            // Call
            bool result = registry.Contains(calculation);

            // Assert
            Assert.IsFalse(result);
        }

        #endregion

        #region Get methods

        [Test]
        public void Get_WithoutPipingSoilProfile_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate test = () => registry.Get((PipingSoilProfile) null);

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
        public void Get_WithoutRingtoetsPipingSurfaceLine_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate test = () => registry.Get((RingtoetsPipingSurfaceLine) null);

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
        public void Get_WithoutHydraulicBoundaryLocation_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate test = () => registry.Get((HydraulicBoundaryLocation) null);

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
        public void Get_WithoutStochasticSoilModel_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate test = () => registry.Get((StochasticSoilModel) null);

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
        public void Get_WithoutStochasticSoilProfileEntity_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate test = () => registry.Get((StochasticSoilProfile) null);

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

        [Test]
        public void Get_WithoutFailureMechanismSection_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate test = () => registry.Get((FailureMechanismSection) null);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("model", paramName);
        }

        [Test]
        public void Get_FailureMechanismSectionAdded_ReturnsEntity()
        {
            // Setup
            var registry = new PersistenceRegistry();
            var section = new TestFailureMechanismSection();
            var entity = new FailureMechanismSectionEntity();
            registry.Register(entity, section);

            // Call
            FailureMechanismSectionEntity result = registry.Get(section);

            // Assert
            Assert.AreSame(entity, result);
        }

        [Test]
        public void Get_NoFailureMechanismSectionAdded_ThrowsInvalidOperationException()
        {
            // Setup
            var registry = new PersistenceRegistry();
            var section = new TestFailureMechanismSection();

            // Call
            TestDelegate test = () => registry.Get(section);

            // Assert
            Assert.Throws<InvalidOperationException>(test);
        }

        [Test]
        public void Get_OtherFailureMechanismSectionAdded_ThrowsInvalidOperationException()
        {
            // Setup
            var registry = new PersistenceRegistry();
            var section = new TestFailureMechanismSection();
            registry.Register(new FailureMechanismSectionEntity(), new TestFailureMechanismSection());

            // Call
            TestDelegate test = () => registry.Get(section);

            // Assert
            Assert.Throws<InvalidOperationException>(test);
        }

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

        [Test]
        public void Get_WithoutDikeProfile_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate call = () => registry.Get((DikeProfile) null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("model", paramName);
        }

        [Test]
        public void Get_NoDikeProfileAdded_ThrowsInvalidOperationException()
        {
            // Setup
            var dikeProfile = new DikeProfile(new Point2D(0, 0), new RoughnessPoint[0], new Point2D[0],
                                              null, new DikeProfile.ConstructionProperties());
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate call = () => registry.Get(dikeProfile);

            // Assert
            Assert.Throws<InvalidOperationException>(call);
        }

        [Test]
        public void Get_OtherDikeProfileAdded_ThrowsInvalidOperationException()
        {
            // Setup
            var dikeProfile = new DikeProfile(new Point2D(0, 0), new RoughnessPoint[0], new Point2D[0],
                                              null, new DikeProfile.ConstructionProperties());
            var registeredDikeProfile = new DikeProfile(new Point2D(1, 1), new RoughnessPoint[0], new Point2D[0],
                                                        null, new DikeProfile.ConstructionProperties());
            var registeredEntity = new DikeProfileEntity();

            var registry = new PersistenceRegistry();
            registry.Register(registeredEntity, registeredDikeProfile);

            // Call
            TestDelegate call = () => registry.Get(dikeProfile);

            // Assert
            Assert.Throws<InvalidOperationException>(call);
        }

        [Test]
        public void Get_DikeProfileAdded_ReturnsEntity()
        {
            // Setup
            var dikeProfile = new DikeProfile(new Point2D(0, 0), new RoughnessPoint[0], new Point2D[0],
                                              null, new DikeProfile.ConstructionProperties());
            var registeredEntity = new DikeProfileEntity();

            var registry = new PersistenceRegistry();
            registry.Register(registeredEntity, dikeProfile);

            // Call
            DikeProfileEntity retrievedEntity = registry.Get(dikeProfile);

            // Assert
            Assert.AreSame(registeredEntity, retrievedEntity);
        }

        [Test]
        public void Get_WithoutGrassCoverErosionInwardsCalculation_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate call = () => registry.Get((GrassCoverErosionInwardsCalculation) null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("model", paramName);
        }

        [Test]
        public void Get_NoGrassCoverErosionInwardsCalculationAdded_ThrowsInvalidOperationException()
        {
            // Setup
            var calculation = new GrassCoverErosionInwardsCalculation();
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate call = () => registry.Get(calculation);

            // Assert
            Assert.Throws<InvalidOperationException>(call);
        }

        [Test]
        public void Get_OtherGrassCoverErosionInwardsCalculationAdded_ThrowsInvalidOperationException()
        {
            // Setup
            var calculation = new GrassCoverErosionInwardsCalculation();
            var registeredCalculation = new GrassCoverErosionInwardsCalculation();
            var registeredEntity = new GrassCoverErosionInwardsCalculationEntity();

            var registry = new PersistenceRegistry();
            registry.Register(registeredEntity, registeredCalculation);

            // Call
            TestDelegate call = () => registry.Get(calculation);

            // Assert
            Assert.Throws<InvalidOperationException>(call);
        }

        [Test]
        public void Get_GrassCoverErosionInwardsCalculationAdded_ReturnsEntity()
        {
            // Setup
            var calculation = new GrassCoverErosionInwardsCalculation();
            var registeredEntity = new GrassCoverErosionInwardsCalculationEntity();

            var registry = new PersistenceRegistry();
            registry.Register(registeredEntity, calculation);

            // Call
            GrassCoverErosionInwardsCalculationEntity retrievedEntity = registry.Get(calculation);

            // Assert
            Assert.AreSame(registeredEntity, retrievedEntity);
        }

        #endregion

        #region Register methods

        [Test]
        public void Register_WithNullProjectEntity_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate test = () => registry.Register(null, new RingtoetsProject());

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
        public void Register_WithNullFailureMechanismSectionEntity_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate test = () => registry.Register(null, new TestFailureMechanismSection());

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("entity", paramName);
        }

        [Test]
        public void Register_WithNullPipingFailureMechanismSectionResult_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate test = () => registry.Register(new PipingSectionResultEntity(), null);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("model", paramName);
        }

        [Test]
        public void Register_WithNullPipingSectionResultEntity_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate test = () => registry.Register(null, new PipingFailureMechanismSectionResult(new TestFailureMechanismSection()));

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("entity", paramName);
        }

        [Test]
        public void Register_WithNullGeneralGrassCoverErosionInwardsInput_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate test = () => registry.Register(new GrassCoverErosionInwardsFailureMechanismMetaEntity(), null);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("model", paramName);
        }

        [Test]
        public void Register_WithNullGrassCoverErosionInwardsFailureMechanismMetaEntity_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate test = () => registry.Register(null, new GeneralGrassCoverErosionInwardsInput());

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("entity", paramName);
        }

        [Test]
        public void Register_WithNullDikeProfile_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate test = () => registry.Register(new DikeProfileEntity(), null);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("model", paramName);
        }

        [Test]
        public void Register_WithNullDikeProfileEntity_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate test = () => registry.Register(null, CreateDikeProfile());

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("entity", paramName);
        }

        [Test]
        public void Register_WithNullGrassCoverErosionInwardsCalculation_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate test = () => registry.Register(new GrassCoverErosionInwardsCalculationEntity(), null);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("model", paramName);
        }

        [Test]
        public void Register_WithNullGrassCoverErosionInwardsCalculationEntity_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate test = () => registry.Register(null, new GrassCoverErosionInwardsCalculation());

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("entity", paramName);
        }

        [Test]
        public void Register_WithNullGrassCoverErosionInwardsOutput_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate test = () => registry.Register(new GrassCoverErosionInwardsOutputEntity(), null);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("model", paramName);
        }

        [Test]
        public void Register_WithNullGrassCoverErosionInwardsOutputEntity_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();
            var output = new GrassCoverErosionInwardsOutput(1, false, new ProbabilityAssessmentOutput(1, 1, 1, 1, 1), 1);

            // Call
            TestDelegate test = () => registry.Register(null, output);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("entity", paramName);
        }

        [Test]
        public void Register_WithNullProbabilityAssessmentOutput_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate test = () => registry.Register(new ProbabilisticOutputEntity(), null);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("model", paramName);
        }

        [Test]
        public void Register_WithNullProbabilisticOutputEntity_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();
            var output = new ProbabilityAssessmentOutput(1, 1, 1, 1, 1);

            // Call
            TestDelegate test = () => registry.Register(null, output);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("entity", paramName);
        }

        [Test]
        public void Register_WithNullGrassCoverErosionInwardsFailureMechanismSectionResult_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate test = () => registry.Register(new GrassCoverErosionInwardsSectionResultEntity(), null);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("model", paramName);
        }

        [Test]
        public void Register_WithNullGrassCoverErosionInwardsSectionResultEntity_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate test = () => registry.Register(null, new GrassCoverErosionInwardsFailureMechanismSectionResult(new TestFailureMechanismSection()));

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("entity", paramName);
        }

        [Test]
        public void Register_WithNullHeightStructuresFailureMechanismSectionResult_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate test = () => registry.Register(new HeightStructuresSectionResultEntity(), null);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("model", paramName);
        }

        [Test]
        public void Register_WithNullHeightStructuresSectionResultEntity_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate test = () => registry.Register(null, new HeightStructuresFailureMechanismSectionResult(new TestFailureMechanismSection()));

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("entity", paramName);
        }

        [Test]
        public void Register_WithNullStrengthStabilityLengthwiseConstructionFailureMechanismSectionResult_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate test = () => registry.Register(new StrengthStabilityLengthwiseConstructionSectionResultEntity(), null);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("model", paramName);
        }

        [Test]
        public void Register_WithNullStrengthStabilityLengthwiseConstructionSectionResultEntity_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate test = () => registry.Register(null, new StrengthStabilityLengthwiseConstructionFailureMechanismSectionResult(new TestFailureMechanismSection()));

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("entity", paramName);
        }

        [Test]
        public void Register_WithNullTechnicalInnovationFailureMechanismSectionResult_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate test = () => registry.Register(new TechnicalInnovationSectionResultEntity(), null);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("model", paramName);
        }

        [Test]
        public void Register_WithNullTechnicalInnovationSectionResultEntity_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate test = () => registry.Register(null, new TechnicalInnovationFailureMechanismSectionResult(new TestFailureMechanismSection()));

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("entity", paramName);
        }

        [Test]
        public void Register_WithNullWaterPressureAsphaltCoverFailureMechanismSectionResult_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate test = () => registry.Register(new WaterPressureAsphaltCoverSectionResultEntity(), null);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("model", paramName);
        }

        [Test]
        public void Register_WithNullWaterPressureAsphaltCoverSectionResultEntity_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate test = () => registry.Register(null, new WaterPressureAsphaltCoverFailureMechanismSectionResult(new TestFailureMechanismSection()));

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("entity", paramName);
        }

        [Test]
        public void Register_WithNullClosingStructureFailureMechanismSectionResult_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate test = () => registry.Register(new ClosingStructureSectionResultEntity(), null);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("model", paramName);
        }

        [Test]
        public void Register_WithNullClosingStructureSectionResultEntity_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate test = () => registry.Register(null, new ClosingStructureFailureMechanismSectionResult(new TestFailureMechanismSection()));

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("entity", paramName);
        }

        [Test]
        public void Register_WithNullMacrostabilityInwardsFailureMechanismSectionResult_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate test = () => registry.Register(new MacrostabilityInwardsSectionResultEntity(), null);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("model", paramName);
        }

        [Test]
        public void Register_WithNullMacrostabilityInwardsSectionResultEntity_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate test = () => registry.Register(null, new MacrostabilityInwardsFailureMechanismSectionResult(new TestFailureMechanismSection()));

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("entity", paramName);
        }

        [Test]
        public void Register_WithNullMacrostabilityOutwardsFailureMechanismSectionResult_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate test = () => registry.Register(new MacrostabilityOutwardsSectionResultEntity(), null);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("model", paramName);
        }

        [Test]
        public void Register_WithNullMacrostabilityOutwardsSectionResultEntity_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate test = () => registry.Register(null, new MacrostabilityOutwardsFailureMechanismSectionResult(new TestFailureMechanismSection()));

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("entity", paramName);
        }

        [Test]
        public void Register_WithNullWaveImpactAsphaltCoverFailureMechanismSectionResult_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate test = () => registry.Register(new WaveImpactAsphaltCoverSectionResultEntity(), null);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("model", paramName);
        }

        [Test]
        public void Register_WithNullWaveImpactAsphaltCoverSectionResultEntity_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate test = () => registry.Register(null, new WaveImpactAsphaltCoverFailureMechanismSectionResult(new TestFailureMechanismSection()));

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("entity", paramName);
        }

        [Test]
        public void Register_WithNullGrassCoverErosionOutwardsFailureMechanismSectionResult_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate test = () => registry.Register(new GrassCoverErosionOutwardsSectionResultEntity(), null);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("model", paramName);
        }

        [Test]
        public void Register_WithNullGrassCoverErosionOutwardsSectionResultEntity_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate test = () => registry.Register(null, new GrassCoverErosionOutwardsFailureMechanismSectionResult(new TestFailureMechanismSection()));

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("entity", paramName);
        }

        [Test]
        public void Register_WithNullGrassCoverSlipOffInwardsFailureMechanismSectionResult_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate test = () => registry.Register(new GrassCoverSlipOffInwardsSectionResultEntity(), null);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("model", paramName);
        }

        [Test]
        public void Register_WithNullGrassCoverSlipOffInwardsSectionResultEntity_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate test = () => registry.Register(null, new GrassCoverSlipOffInwardsFailureMechanismSectionResult(new TestFailureMechanismSection()));

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("entity", paramName);
        }

        [Test]
        public void Register_WithNullGrassCoverSlipOffOutwardsFailureMechanismSectionResult_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate test = () => registry.Register(new GrassCoverSlipOffOutwardsSectionResultEntity(), null);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("model", paramName);
        }

        [Test]
        public void Register_WithNullGrassCoverSlipOffOutwardsSectionResultEntity_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate test = () => registry.Register(null, new GrassCoverSlipOffOutwardsFailureMechanismSectionResult(new TestFailureMechanismSection()));

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("entity", paramName);
        }

        [Test]
        public void Register_WithNullMicrostabilityFailureMechanismSectionResult_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate test = () => registry.Register(new MicrostabilitySectionResultEntity(), null);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("model", paramName);
        }

        [Test]
        public void Register_WithNullMicrostabilitySectionResultEntity_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate test = () => registry.Register(null, new MicrostabilityFailureMechanismSectionResult(new TestFailureMechanismSection()));

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("entity", paramName);
        }

        [Test]
        public void Register_WithNullPipingStructureFailureMechanismSectionResult_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate test = () => registry.Register(new PipingStructureSectionResultEntity(), null);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("model", paramName);
        }

        [Test]
        public void Register_WithNullPipingStructureSectionResultEntity_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate test = () => registry.Register(null, new PipingStructureFailureMechanismSectionResult(new TestFailureMechanismSection()));

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("entity", paramName);
        }

        [Test]
        public void Register_WithNullDuneErosionFailureMechanismSectionResult_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate test = () => registry.Register(new DuneErosionSectionResultEntity(), null);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("model", paramName);
        }

        [Test]
        public void Register_WithNullDuneErosionSectionResultEntity_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate test = () => registry.Register(null, new DuneErosionFailureMechanismSectionResult(new TestFailureMechanismSection()));

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("entity", paramName);
        }

        [Test]
        public void Register_WithNullStabilityStoneCoverFailureMechanismSectionResult_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate test = () => registry.Register(new StabilityStoneCoverSectionResultEntity(), null);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("model", paramName);
        }

        [Test]
        public void Register_WithNullStabilityStoneCoverSectionResultEntity_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate test = () => registry.Register(null, new StabilityStoneCoverFailureMechanismSectionResult(new TestFailureMechanismSection()));

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("entity", paramName);
        }

        [Test]
        public void Register_WithNullStrengthStabilityPointConstructionFailureMechanismSectionResult_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate test = () => registry.Register(new StrengthStabilityPointConstructionSectionResultEntity(), null);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("model", paramName);
        }

        [Test]
        public void Register_WithNullStrengthStabilityPointConstructionSectionResultEntity_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate test = () => registry.Register(null, new StrengthStabilityPointConstructionFailureMechanismSectionResult(new TestFailureMechanismSection()));

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("entity", paramName);
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
            TestDelegate test = () => registry.Register(null, new PipingOutput(1, 1, 1, 1, 1, 1));

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
            TestDelegate test = () => registry.Register(null, new PipingSoilProfile("name", 0, new[]
            {
                new PipingSoilLayer(1)
            }, SoilProfileType.SoilProfile1D, -1));

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
            TestDelegate call = () => registry.Register((SurfaceLinePointEntity) null, new Point3D(1.1, 2.2, 3.3));

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
            TestDelegate call = () => registry.Register((CharacteristicPointEntity) null, new Point3D(1.1, 2.2, 3.3));

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
        public void Register_WithNullPipingFailureMechanismMetaEntity_ThrowsArgumentNullException()
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
        public void Register_WithNullPipingProbabilityAssessmentInput_ThrowsArgumentNullException()
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

            long storageId = new Random(21).Next(1, 4000);
            var entity = new ProjectEntity
            {
                ProjectEntityId = storageId
            };
            var model = new RingtoetsProject();
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

            long storageId = new Random(21).Next(1, 4000);
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

            long storageId = new Random(21).Next(1, 4000);
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

            long storageId = new Random(21).Next(1, 4000);
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

            long storageId = new Random(21).Next(1, 4000);
            var entity = new FailureMechanismSectionEntity
            {
                FailureMechanismSectionEntityId = storageId
            };
            var model = new TestFailureMechanismSection();
            registry.Register(entity, model);

            // Call
            registry.TransferIds();

            // Assert
            Assert.AreEqual(storageId, model.StorageId);
        }

        [Test]
        public void TransferIds_WithPipingSectionResultEntityAddedWithPipingFailureMechanismSectionResult_EqualPipingSectionEntityIdAndPipingFailureMechanismSectionResultStorageId()
        {
            // Setup
            var registry = new PersistenceRegistry();

            long storageId = new Random(21).Next(1, 4000);
            var entity = new PipingSectionResultEntity
            {
                PipingSectionResultEntityId = storageId
            };
            var model = new PipingFailureMechanismSectionResult(new TestFailureMechanismSection());
            registry.Register(entity, model);

            // Call
            registry.TransferIds();

            // Assert
            Assert.AreEqual(storageId, model.StorageId);
        }

        [Test]
        public void TransferIds_WithGrassCoverErosionInwardsFailureMechanismMetaEntityAddedWithGeneralGrassCoverErosionInwardsInput_EqualGrassCoverErosionInwardsFailureMechanismMetaEntityIdAndGeneralGrassCoverErosionInwardsInputStorageId()
        {
            // Setup
            var registry = new PersistenceRegistry();

            long storageId = new Random(21).Next(1, 4000);
            var entity = new GrassCoverErosionInwardsFailureMechanismMetaEntity
            {
                GrassCoverErosionInwardsFailureMechanismMetaEntityId = storageId
            };
            var model = new GeneralGrassCoverErosionInwardsInput();
            registry.Register(entity, model);

            // Call
            registry.TransferIds();

            // Assert
            Assert.AreEqual(storageId, model.StorageId);
        }

        [Test]
        public void TransferIds_WithDikeProfileEntityAddedWithDikeProfile_EqualDikeProfileEntityIdAndDikeProfileStorageId()
        {
            // Setup
            var registry = new PersistenceRegistry();

            long storageId = new Random(21).Next(1, 4000);
            var entity = new DikeProfileEntity
            {
                DikeProfileEntityId = storageId
            };
            DikeProfile model = CreateDikeProfile();
            registry.Register(entity, model);

            // Call
            registry.TransferIds();

            // Assert
            Assert.AreEqual(storageId, model.StorageId);
        }

        [Test]
        public void TransferIds_WithGrassCoverErosionInwardsCalculationEntityAddedWithGrassCoverErosionInwardsCalculation_EqualGrassCoverErosionInwardsCalculationEntityIdAndGrassCoverErosionInwardsCalculationStorageId()
        {
            // Setup
            var registry = new PersistenceRegistry();

            long storageId = new Random(21).Next(1, 4000);
            var entity = new GrassCoverErosionInwardsCalculationEntity
            {
                GrassCoverErosionInwardsCalculationEntityId = storageId
            };
            var model = new GrassCoverErosionInwardsCalculation();
            registry.Register(entity, model);

            // Call
            registry.TransferIds();

            // Assert
            Assert.AreEqual(storageId, model.StorageId);
        }

        [Test]
        public void TransferIds_WithGrassCoverErosionInwardsOutputEntityAddedWithGrassCoverErosionInwardsOutput_EqualGrassCoverErosionInwardsOutputEntityIdAndGrassCoverErosionInwardsOutputStorageId()
        {
            // Setup
            var registry = new PersistenceRegistry();

            long storageId = new Random(21).Next(1, 4000);
            var entity = new GrassCoverErosionInwardsOutputEntity
            {
                GrassCoverErosionInwardsOutputId = storageId
            };
            var model = new GrassCoverErosionInwardsOutput(1, false, new ProbabilityAssessmentOutput(1, 1, 1, 1, 1), 1);
            registry.Register(entity, model);

            // Call
            registry.TransferIds();

            // Assert
            Assert.AreEqual(storageId, model.StorageId);
        }

        [Test]
        public void TransferIds_WithProbabilisticOutputEntityAddedWithGrassCoverErosionInwardsOutput_EqualProbabilisticOutputEntityIdAndGrassCoverErosionInwardsOutputProbabilityAssessmentOutputStorageId()
        {
            // Setup
            var registry = new PersistenceRegistry();

            long storageId = new Random(21).Next(1, 4000);
            var entity = new ProbabilisticOutputEntity
            {
                ProbabilisticOutputEntityId = storageId
            };
            var model = new ProbabilityAssessmentOutput(1, 1, 1, 1, 1);
            registry.Register(entity, model);

            // Call
            registry.TransferIds();

            // Assert
            Assert.AreEqual(storageId, model.StorageId);
        }

        [Test]
        public void TransferIds_WithGrassCoverErosionInwardsSectionResultEntityAddedWithGrassCoverErosionInwardsFailureMechanismSectionResult_EqualGrassCoverErosionInwardsSectionEntityIdAndGrassCoverErosionInwardsFailureMechanismSectionResultStorageId()
        {
            // Setup
            var registry = new PersistenceRegistry();

            long storageId = new Random(21).Next(1, 4000);
            var entity = new GrassCoverErosionInwardsSectionResultEntity
            {
                GrassCoverErosionInwardsSectionResultEntityId = storageId
            };
            var model = new GrassCoverErosionInwardsFailureMechanismSectionResult(new TestFailureMechanismSection());
            registry.Register(entity, model);

            // Call
            registry.TransferIds();

            // Assert
            Assert.AreEqual(storageId, model.StorageId);
        }

        [Test]
        public void TransferIds_WithHeightStructuresSectionResultEntityAddedWithHeightStructuresFailureMechanismSectionResult_EqualHeightStructuresSectionEntityIdAndHeightStructuresFailureMechanismSectionResultStorageId()
        {
            // Setup
            var registry = new PersistenceRegistry();

            long storageId = new Random(21).Next(1, 4000);
            var entity = new HeightStructuresSectionResultEntity
            {
                HeightStructuresSectionResultEntityId = storageId
            };
            var model = new HeightStructuresFailureMechanismSectionResult(new TestFailureMechanismSection());
            registry.Register(entity, model);

            // Call
            registry.TransferIds();

            // Assert
            Assert.AreEqual(storageId, model.StorageId);
        }

        [Test]
        public void TransferIds_WithStrengthStabilityLengthwiseConstructionSectionResultEntityAddedWithStrengthStabilityLengthwiseConstructionFailureMechanismSectionResult_EqualStrengthStabilityLengthwiseConstructionSectionEntityIdAndStrengthStabilityLengthwiseConstructionFailureMechanismSectionResultStorageId()
        {
            // Setup
            var registry = new PersistenceRegistry();

            long storageId = new Random(21).Next(1, 4000);
            var entity = new StrengthStabilityLengthwiseConstructionSectionResultEntity
            {
                StrengthStabilityLengthwiseConstructionSectionResultEntityId = storageId
            };
            var model = new StrengthStabilityLengthwiseConstructionFailureMechanismSectionResult(new TestFailureMechanismSection());
            registry.Register(entity, model);

            // Call
            registry.TransferIds();

            // Assert
            Assert.AreEqual(storageId, model.StorageId);
        }

        [Test]
        public void TransferIds_WithTechnicalInnovationSectionResultEntityAddedWithTechnicalInnovationFailureMechanismSectionResult_EqualTechnicalInnovationSectionEntityIdAndTechnicalInnovationFailureMechanismSectionResultStorageId()
        {
            // Setup
            var registry = new PersistenceRegistry();

            long storageId = new Random(21).Next(1, 4000);
            var entity = new TechnicalInnovationSectionResultEntity
            {
                TechnicalInnovationSectionResultEntityId = storageId
            };
            var model = new TechnicalInnovationFailureMechanismSectionResult(new TestFailureMechanismSection());
            registry.Register(entity, model);

            // Call
            registry.TransferIds();

            // Assert
            Assert.AreEqual(storageId, model.StorageId);
        }

        [Test]
        public void TransferIds_WithWaterPressureAsphaltCoverSectionResultEntityAddedWithWaterPressureAsphaltCoverFailureMechanismSectionResult_EqualWaterPressureAsphaltCoverSectionEntityIdAndWaterPressureAsphaltCoverFailureMechanismSectionResultStorageId()
        {
            // Setup
            var registry = new PersistenceRegistry();

            long storageId = new Random(21).Next(1, 4000);
            var entity = new WaterPressureAsphaltCoverSectionResultEntity
            {
                WaterPressureAsphaltCoverSectionResultEntityId = storageId
            };
            var model = new WaterPressureAsphaltCoverFailureMechanismSectionResult(new TestFailureMechanismSection());
            registry.Register(entity, model);

            // Call
            registry.TransferIds();

            // Assert
            Assert.AreEqual(storageId, model.StorageId);
        }

        [Test]
        public void TransferIds_WithClosingStructureSectionResultEntityAddedWithClosingStructureFailureMechanismSectionResult_EqualClosingStructureSectionEntityIdAndClosingStructureFailureMechanismSectionResultStorageId()
        {
            // Setup
            var registry = new PersistenceRegistry();

            long storageId = new Random(21).Next(1, 4000);
            var entity = new ClosingStructureSectionResultEntity
            {
                ClosingStructureSectionResultEntityId = storageId
            };
            var model = new ClosingStructureFailureMechanismSectionResult(new TestFailureMechanismSection());
            registry.Register(entity, model);

            // Call
            registry.TransferIds();

            // Assert
            Assert.AreEqual(storageId, model.StorageId);
        }

        [Test]
        public void TransferIds_WithMacrostabilityInwardsSectionResultEntityAddedWithMacrostabilityInwardsFailureMechanismSectionResult_EqualMacrostabilityInwardsSectionEntityIdAndMacrostabilityInwardsFailureMechanismSectionResultStorageId()
        {
            // Setup
            var registry = new PersistenceRegistry();

            long storageId = new Random(21).Next(1, 4000);
            var entity = new MacrostabilityInwardsSectionResultEntity
            {
                MacrostabilityInwardsSectionResultEntityId = storageId
            };
            var model = new MacrostabilityInwardsFailureMechanismSectionResult(new TestFailureMechanismSection());
            registry.Register(entity, model);

            // Call
            registry.TransferIds();

            // Assert
            Assert.AreEqual(storageId, model.StorageId);
        }

        [Test]
        public void TransferIds_WithMacrostabilityOutwardsSectionResultEntityAddedWithMacrostabilityOutwardsFailureMechanismSectionResult_EqualMacrostabilityOutwardsSectionEntityIdAndMacrostabilityOutwardsFailureMechanismSectionResultStorageId()
        {
            // Setup
            var registry = new PersistenceRegistry();

            long storageId = new Random(21).Next(1, 4000);
            var entity = new MacrostabilityOutwardsSectionResultEntity
            {
                MacrostabilityOutwardsSectionResultEntityId = storageId
            };
            var model = new MacrostabilityOutwardsFailureMechanismSectionResult(new TestFailureMechanismSection());
            registry.Register(entity, model);

            // Call
            registry.TransferIds();

            // Assert
            Assert.AreEqual(storageId, model.StorageId);
        }

        [Test]
        public void TransferIds_WithWaveImpactAsphaltCoverSectionResultEntityAddedWithWaveImpactAsphaltCoverFailureMechanismSectionResult_EqualWaveImpactAsphaltCoverSectionEntityIdAndWaveImpactAsphaltCoverFailureMechanismSectionResultStorageId()
        {
            // Setup
            var registry = new PersistenceRegistry();

            long storageId = new Random(21).Next(1, 4000);
            var entity = new WaveImpactAsphaltCoverSectionResultEntity
            {
                WaveImpactAsphaltCoverSectionResultEntityId = storageId
            };
            var model = new WaveImpactAsphaltCoverFailureMechanismSectionResult(new TestFailureMechanismSection());
            registry.Register(entity, model);

            // Call
            registry.TransferIds();

            // Assert
            Assert.AreEqual(storageId, model.StorageId);
        }

        [Test]
        public void TransferIds_WithGrassCoverErosionOutwardsSectionResultEntityAddedWithGrassCoverErosionOutwardsFailureMechanismSectionResult_EqualGrassCoverErosionOutwardsSectionEntityIdAndGrassCoverErosionOutwardsFailureMechanismSectionResultStorageId()
        {
            // Setup
            var registry = new PersistenceRegistry();

            long storageId = new Random(21).Next(1, 4000);
            var entity = new GrassCoverErosionOutwardsSectionResultEntity
            {
                GrassCoverErosionOutwardsSectionResultEntityId = storageId
            };
            var model = new GrassCoverErosionOutwardsFailureMechanismSectionResult(new TestFailureMechanismSection());
            registry.Register(entity, model);

            // Call
            registry.TransferIds();

            // Assert
            Assert.AreEqual(storageId, model.StorageId);
        }

        [Test]
        public void TransferIds_WithGrassCoverSlipOffInwardsSectionResultEntityAddedWithGrassCoverSlipOffInwardsFailureMechanismSectionResult_EqualGrassCoverSlipOffInwardsSectionEntityIdAndGrassCoverSlipOffInwardsFailureMechanismSectionResultStorageId()
        {
            // Setup
            var registry = new PersistenceRegistry();

            long storageId = new Random(21).Next(1, 4000);
            var entity = new GrassCoverSlipOffInwardsSectionResultEntity
            {
                GrassCoverSlipOffInwardsSectionResultEntityId = storageId
            };
            var model = new GrassCoverSlipOffInwardsFailureMechanismSectionResult(new TestFailureMechanismSection());
            registry.Register(entity, model);

            // Call
            registry.TransferIds();

            // Assert
            Assert.AreEqual(storageId, model.StorageId);
        }

        [Test]
        public void TransferIds_WithGrassCoverSlipOffOutwardsSectionResultEntityAddedWithGrassCoverSlipOffOutwardsFailureMechanismSectionResult_EqualGrassCoverSlipOffOutwardsSectionEntityIdAndGrassCoverSlipOffOutwardsFailureMechanismSectionResultStorageId()
        {
            // Setup
            var registry = new PersistenceRegistry();

            long storageId = new Random(21).Next(1, 4000);
            var entity = new GrassCoverSlipOffOutwardsSectionResultEntity
            {
                GrassCoverSlipOffOutwardsSectionResultEntityId = storageId
            };
            var model = new GrassCoverSlipOffOutwardsFailureMechanismSectionResult(new TestFailureMechanismSection());
            registry.Register(entity, model);

            // Call
            registry.TransferIds();

            // Assert
            Assert.AreEqual(storageId, model.StorageId);
        }

        [Test]
        public void TransferIds_WithMicrostabilitySectionResultEntityAddedWithMicrostabilityFailureMechanismSectionResult_EqualMicrostabilitySectionEntityIdAndMicrostabilityFailureMechanismSectionResultStorageId()
        {
            // Setup
            var registry = new PersistenceRegistry();

            long storageId = new Random(21).Next(1, 4000);
            var entity = new MicrostabilitySectionResultEntity
            {
                MicrostabilitySectionResultEntityId = storageId
            };
            var model = new MicrostabilityFailureMechanismSectionResult(new TestFailureMechanismSection());
            registry.Register(entity, model);

            // Call
            registry.TransferIds();

            // Assert
            Assert.AreEqual(storageId, model.StorageId);
        }

        [Test]
        public void TransferIds_WithPipingStructureSectionResultEntityAddedWithPipingStructureFailureMechanismSectionResult_EqualPipingStructureSectionEntityIdAndPipingStructureFailureMechanismSectionResultStorageId()
        {
            // Setup
            var registry = new PersistenceRegistry();

            long storageId = new Random(21).Next(1, 4000);
            var entity = new PipingStructureSectionResultEntity
            {
                PipingStructureSectionResultEntityId = storageId
            };
            var model = new PipingStructureFailureMechanismSectionResult(new TestFailureMechanismSection());
            registry.Register(entity, model);

            // Call
            registry.TransferIds();

            // Assert
            Assert.AreEqual(storageId, model.StorageId);
        }

        [Test]
        public void TransferIds_WithDuneErosionSectionResultEntityAddedWithDuneErosionFailureMechanismSectionResult_EqualDuneErosionSectionEntityIdAndDuneErosionFailureMechanismSectionResultStorageId()
        {
            // Setup
            var registry = new PersistenceRegistry();

            long storageId = new Random(21).Next(1, 4000);
            var entity = new DuneErosionSectionResultEntity
            {
                DuneErosionSectionResultEntityId = storageId
            };
            var model = new DuneErosionFailureMechanismSectionResult(new TestFailureMechanismSection());
            registry.Register(entity, model);

            // Call
            registry.TransferIds();

            // Assert
            Assert.AreEqual(storageId, model.StorageId);
        }

        [Test]
        public void TransferIds_WithStabilityStoneCoverSectionResultEntityAddedWithStabilityStoneCoverFailureMechanismSectionResult_EqualStabilityStoneCoverSectionEntityIdAndStabilityStoneCoverFailureMechanismSectionResultStorageId()
        {
            // Setup
            var registry = new PersistenceRegistry();

            long storageId = new Random(21).Next(1, 4000);
            var entity = new StabilityStoneCoverSectionResultEntity
            {
                StabilityStoneCoverSectionResultEntityId = storageId
            };
            var model = new StabilityStoneCoverFailureMechanismSectionResult(new TestFailureMechanismSection());
            registry.Register(entity, model);

            // Call
            registry.TransferIds();

            // Assert
            Assert.AreEqual(storageId, model.StorageId);
        }

        [Test]
        public void TransferIds_WithStrengthStabilityPointConstructionSectionResultEntityAddedWithStrengthStabilityPointConstructionFailureMechanismSectionResult_EqualStrengthStabilityPointConstructionSectionEntityIdAndStrengthStabilityPointConstructionFailureMechanismSectionResultStorageId()
        {
            // Setup
            var registry = new PersistenceRegistry();

            long storageId = new Random(21).Next(1, 4000);
            var entity = new StrengthStabilityPointConstructionSectionResultEntity
            {
                StrengthStabilityPointConstructionSectionResultEntityId = storageId
            };
            var model = new StrengthStabilityPointConstructionFailureMechanismSectionResult(new TestFailureMechanismSection());
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

            long storageId = new Random(21).Next(1, 4000);
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
            var model = new PipingOutput(1, 2, 3, 4, 5, 6);
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
            var model = new PipingSemiProbabilisticOutput(1, 2, 0.3, 4, 5, 0.6, 7, 8, 0.9, 1.0, 11, 0.3, 13, 14);
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

            long storageId = new Random(21).Next(1, 4000);
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

            long storageId = new Random(21).Next(1, 4000);
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

            long storageId = new Random(21).Next(1, 4000);
            var entity = new SoilProfileEntity
            {
                SoilProfileEntityId = storageId
            };
            var model = new PipingSoilProfile("name", 0, new[]
            {
                new PipingSoilLayer(1)
            }, SoilProfileType.SoilProfile1D, -1);
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

            long storageId = new Random(21).Next(1, 4000);
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

            var project = new RingtoetsProject
            {
                StorageId = persistentEntity.ProjectEntityId
            };

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

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike)
            {
                StorageId = persistentEntity.AssessmentSectionEntityId
            };

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

            var section = new TestFailureMechanismSection
            {
                StorageId = persistentEntity.FailureMechanismSectionEntityId
            };

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
        public void RemoveUntouched_PipingSectionResultEntity_OrphanedEntityIsRemovedFromRingtoetsEntities()
        {
            // Setup
            var mocks = new MockRepository();
            IRingtoetsEntities dbContext = RingtoetsEntitiesHelper.CreateStub(mocks);
            mocks.ReplayAll();

            var orphanedEntity = new PipingSectionResultEntity
            {
                PipingSectionResultEntityId = 1
            };
            var persistentEntity = new PipingSectionResultEntity
            {
                PipingSectionResultEntityId = 2
            };
            dbContext.PipingSectionResultEntities.Add(orphanedEntity);
            dbContext.PipingSectionResultEntities.Add(persistentEntity);

            var section = new PipingFailureMechanismSectionResult(new TestFailureMechanismSection())
            {
                StorageId = persistentEntity.PipingSectionResultEntityId
            };

            var registry = new PersistenceRegistry();
            registry.Register(persistentEntity, section);

            // Call
            registry.RemoveUntouched(dbContext);

            // Assert
            Assert.AreEqual(1, dbContext.PipingSectionResultEntities.Count());
            CollectionAssert.Contains(dbContext.PipingSectionResultEntities, persistentEntity);
            mocks.VerifyAll();
        }

        [Test]
        public void RemoveUntouched_GrassCoverErosionInwardsFailureMechanismMetaEntity_OrphanedEntityIsRemovedFromRingtoetsEntities()
        {
            // Setup
            var mocks = new MockRepository();
            IRingtoetsEntities dbContext = RingtoetsEntitiesHelper.CreateStub(mocks);
            mocks.ReplayAll();

            var orphanedEntity = new GrassCoverErosionInwardsFailureMechanismMetaEntity
            {
                GrassCoverErosionInwardsFailureMechanismMetaEntityId = 1
            };
            var persistentEntity = new GrassCoverErosionInwardsFailureMechanismMetaEntity
            {
                GrassCoverErosionInwardsFailureMechanismMetaEntityId = 2
            };
            dbContext.GrassCoverErosionInwardsFailureMechanismMetaEntities.Add(orphanedEntity);
            dbContext.GrassCoverErosionInwardsFailureMechanismMetaEntities.Add(persistentEntity);

            var section = new GeneralGrassCoverErosionInwardsInput
            {
                StorageId = persistentEntity.GrassCoverErosionInwardsFailureMechanismMetaEntityId
            };

            var registry = new PersistenceRegistry();
            registry.Register(persistentEntity, section);

            // Call
            registry.RemoveUntouched(dbContext);

            // Assert
            Assert.AreEqual(1, dbContext.GrassCoverErosionInwardsFailureMechanismMetaEntities.Count());
            CollectionAssert.Contains(dbContext.GrassCoverErosionInwardsFailureMechanismMetaEntities, persistentEntity);
            mocks.VerifyAll();
        }

        [Test]
        public void RemoveUntouched_DikeProfileEntity_OrphanedEntityIsRemovedFromRingtoetsEntities()
        {
            // Setup
            var mocks = new MockRepository();
            IRingtoetsEntities dbContext = RingtoetsEntitiesHelper.CreateStub(mocks);
            mocks.ReplayAll();

            var orphanedEntity = new DikeProfileEntity
            {
                DikeProfileEntityId = 1
            };
            var persistentEntity = new DikeProfileEntity
            {
                DikeProfileEntityId = 2
            };
            dbContext.DikeProfileEntities.Add(orphanedEntity);
            dbContext.DikeProfileEntities.Add(persistentEntity);

            DikeProfile section = CreateDikeProfile();
            section.StorageId = persistentEntity.DikeProfileEntityId;

            var registry = new PersistenceRegistry();
            registry.Register(persistentEntity, section);

            // Call
            registry.RemoveUntouched(dbContext);

            // Assert
            Assert.AreEqual(1, dbContext.DikeProfileEntities.Count());
            CollectionAssert.Contains(dbContext.DikeProfileEntities, persistentEntity);
            mocks.VerifyAll();
        }

        [Test]
        public void RemoveUntouched_GrassCoverErosionInwardsCalculationEntity_OrphanedEntityIsRemovedFromRingtoetsEntities()
        {
            // Setup
            var mocks = new MockRepository();
            IRingtoetsEntities dbContext = RingtoetsEntitiesHelper.CreateStub(mocks);
            mocks.ReplayAll();

            var orphanedEntity = new GrassCoverErosionInwardsCalculationEntity
            {
                GrassCoverErosionInwardsCalculationEntityId = 1
            };
            var persistentEntity = new GrassCoverErosionInwardsCalculationEntity
            {
                GrassCoverErosionInwardsCalculationEntityId = 2
            };
            dbContext.GrassCoverErosionInwardsCalculationEntities.Add(orphanedEntity);
            dbContext.GrassCoverErosionInwardsCalculationEntities.Add(persistentEntity);

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                StorageId = persistentEntity.GrassCoverErosionInwardsCalculationEntityId
            };

            var registry = new PersistenceRegistry();
            registry.Register(persistentEntity, calculation);

            // Call
            registry.RemoveUntouched(dbContext);

            // Assert
            Assert.AreEqual(1, dbContext.GrassCoverErosionInwardsCalculationEntities.Count());
            CollectionAssert.Contains(dbContext.GrassCoverErosionInwardsCalculationEntities, persistentEntity);
            mocks.VerifyAll();
        }

        [Test]
        public void RemoveUntouched_GrassCoverErosionInwardsOutputEntity_OrphanedEntityIsRemovedFromRingtoetsEntities()
        {
            // Setup
            var mocks = new MockRepository();
            IRingtoetsEntities dbContext = RingtoetsEntitiesHelper.CreateStub(mocks);
            mocks.ReplayAll();

            var orphanedEntity = new GrassCoverErosionInwardsOutputEntity
            {
                GrassCoverErosionInwardsOutputId = 1
            };
            var persistentEntity = new GrassCoverErosionInwardsOutputEntity
            {
                GrassCoverErosionInwardsOutputId = 2
            };
            dbContext.GrassCoverErosionInwardsOutputEntities.Add(orphanedEntity);
            dbContext.GrassCoverErosionInwardsOutputEntities.Add(persistentEntity);

            var calculation = new GrassCoverErosionInwardsOutput(1, false, new ProbabilityAssessmentOutput(1, 1, 1, 1, 1), 1)
            {
                StorageId = persistentEntity.GrassCoverErosionInwardsOutputId
            };

            var registry = new PersistenceRegistry();
            registry.Register(persistentEntity, calculation);

            // Call
            registry.RemoveUntouched(dbContext);

            // Assert
            Assert.AreEqual(1, dbContext.GrassCoverErosionInwardsOutputEntities.Count());
            CollectionAssert.Contains(dbContext.GrassCoverErosionInwardsOutputEntities, persistentEntity);
            mocks.VerifyAll();
        }

        [Test]
        public void RemoveUntouched_ProbabilisticOutputEntity_OrphanedEntityIsRemovedFromRingtoetsEntities()
        {
            // Setup
            var mocks = new MockRepository();
            IRingtoetsEntities dbContext = RingtoetsEntitiesHelper.CreateStub(mocks);
            mocks.ReplayAll();

            var orphanedEntity = new ProbabilisticOutputEntity
            {
                ProbabilisticOutputEntityId = 1
            };
            var persistentEntity = new ProbabilisticOutputEntity
            {
                ProbabilisticOutputEntityId = 2
            };
            dbContext.ProbabilisticOutputEntities.Add(orphanedEntity);
            dbContext.ProbabilisticOutputEntities.Add(persistentEntity);

            var calculation = new ProbabilityAssessmentOutput(1, 1, 1, 1, 1)
            {
                StorageId = persistentEntity.ProbabilisticOutputEntityId
            };

            var registry = new PersistenceRegistry();
            registry.Register(persistentEntity, calculation);

            // Call
            registry.RemoveUntouched(dbContext);

            // Assert
            Assert.AreEqual(1, dbContext.ProbabilisticOutputEntities.Count());
            CollectionAssert.Contains(dbContext.ProbabilisticOutputEntities, persistentEntity);
            mocks.VerifyAll();
        }

        [Test]
        public void RemoveUntouched_GrassCoverErosionInwardsSectionResultEntity_OrphanedEntityIsRemovedFromRingtoetsEntities()
        {
            // Setup
            var mocks = new MockRepository();
            IRingtoetsEntities dbContext = RingtoetsEntitiesHelper.CreateStub(mocks);
            mocks.ReplayAll();

            var orphanedEntity = new GrassCoverErosionInwardsSectionResultEntity
            {
                GrassCoverErosionInwardsSectionResultEntityId = 1
            };
            var persistentEntity = new GrassCoverErosionInwardsSectionResultEntity
            {
                GrassCoverErosionInwardsSectionResultEntityId = 2
            };
            dbContext.GrassCoverErosionInwardsSectionResultEntities.Add(orphanedEntity);
            dbContext.GrassCoverErosionInwardsSectionResultEntities.Add(persistentEntity);

            var section = new GrassCoverErosionInwardsFailureMechanismSectionResult(new TestFailureMechanismSection())
            {
                StorageId = persistentEntity.GrassCoverErosionInwardsSectionResultEntityId
            };

            var registry = new PersistenceRegistry();
            registry.Register(persistentEntity, section);

            // Call
            registry.RemoveUntouched(dbContext);

            // Assert
            Assert.AreEqual(1, dbContext.GrassCoverErosionInwardsSectionResultEntities.Count());
            CollectionAssert.Contains(dbContext.GrassCoverErosionInwardsSectionResultEntities, persistentEntity);
            mocks.VerifyAll();
        }

        [Test]
        public void RemoveUntouched_HeightStructuresSectionResultEntity_OrphanedEntityIsRemovedFromRingtoetsEntities()
        {
            // Setup
            var mocks = new MockRepository();
            IRingtoetsEntities dbContext = RingtoetsEntitiesHelper.CreateStub(mocks);
            mocks.ReplayAll();

            var orphanedEntity = new HeightStructuresSectionResultEntity
            {
                HeightStructuresSectionResultEntityId = 1
            };
            var persistentEntity = new HeightStructuresSectionResultEntity
            {
                HeightStructuresSectionResultEntityId = 2
            };
            dbContext.HeightStructuresSectionResultEntities.Add(orphanedEntity);
            dbContext.HeightStructuresSectionResultEntities.Add(persistentEntity);

            var section = new HeightStructuresFailureMechanismSectionResult(new TestFailureMechanismSection())
            {
                StorageId = persistentEntity.HeightStructuresSectionResultEntityId
            };

            var registry = new PersistenceRegistry();
            registry.Register(persistentEntity, section);

            // Call
            registry.RemoveUntouched(dbContext);

            // Assert
            Assert.AreEqual(1, dbContext.HeightStructuresSectionResultEntities.Count());
            CollectionAssert.Contains(dbContext.HeightStructuresSectionResultEntities, persistentEntity);
            mocks.VerifyAll();
        }

        [Test]
        public void RemoveUntouched_StrengthStabilityLengthwiseConstructionSectionResultEntity_OrphanedEntityIsRemovedFromRingtoetsEntities()
        {
            // Setup
            var mocks = new MockRepository();
            IRingtoetsEntities dbContext = RingtoetsEntitiesHelper.CreateStub(mocks);
            mocks.ReplayAll();

            var orphanedEntity = new StrengthStabilityLengthwiseConstructionSectionResultEntity
            {
                StrengthStabilityLengthwiseConstructionSectionResultEntityId = 1
            };
            var persistentEntity = new StrengthStabilityLengthwiseConstructionSectionResultEntity
            {
                StrengthStabilityLengthwiseConstructionSectionResultEntityId = 2
            };
            dbContext.StrengthStabilityLengthwiseConstructionSectionResultEntities.Add(orphanedEntity);
            dbContext.StrengthStabilityLengthwiseConstructionSectionResultEntities.Add(persistentEntity);

            var section = new StrengthStabilityLengthwiseConstructionFailureMechanismSectionResult(new TestFailureMechanismSection())
            {
                StorageId = persistentEntity.StrengthStabilityLengthwiseConstructionSectionResultEntityId
            };

            var registry = new PersistenceRegistry();
            registry.Register(persistentEntity, section);

            // Call
            registry.RemoveUntouched(dbContext);

            // Assert
            Assert.AreEqual(1, dbContext.StrengthStabilityLengthwiseConstructionSectionResultEntities.Count());
            CollectionAssert.Contains(dbContext.StrengthStabilityLengthwiseConstructionSectionResultEntities, persistentEntity);
            mocks.VerifyAll();
        }

        [Test]
        public void RemoveUntouched_TechnicalInnovationSectionResultEntity_OrphanedEntityIsRemovedFromRingtoetsEntities()
        {
            // Setup
            var mocks = new MockRepository();
            IRingtoetsEntities dbContext = RingtoetsEntitiesHelper.CreateStub(mocks);
            mocks.ReplayAll();

            var orphanedEntity = new TechnicalInnovationSectionResultEntity
            {
                TechnicalInnovationSectionResultEntityId = 1
            };
            var persistentEntity = new TechnicalInnovationSectionResultEntity
            {
                TechnicalInnovationSectionResultEntityId = 2
            };
            dbContext.TechnicalInnovationSectionResultEntities.Add(orphanedEntity);
            dbContext.TechnicalInnovationSectionResultEntities.Add(persistentEntity);

            var section = new TechnicalInnovationFailureMechanismSectionResult(new TestFailureMechanismSection())
            {
                StorageId = persistentEntity.TechnicalInnovationSectionResultEntityId
            };

            var registry = new PersistenceRegistry();
            registry.Register(persistentEntity, section);

            // Call
            registry.RemoveUntouched(dbContext);

            // Assert
            Assert.AreEqual(1, dbContext.TechnicalInnovationSectionResultEntities.Count());
            CollectionAssert.Contains(dbContext.TechnicalInnovationSectionResultEntities, persistentEntity);
            mocks.VerifyAll();
        }

        [Test]
        public void RemoveUntouched_WaterPressureAsphaltCoverSectionResultEntity_OrphanedEntityIsRemovedFromRingtoetsEntities()
        {
            // Setup
            var mocks = new MockRepository();
            IRingtoetsEntities dbContext = RingtoetsEntitiesHelper.CreateStub(mocks);
            mocks.ReplayAll();

            var orphanedEntity = new WaterPressureAsphaltCoverSectionResultEntity
            {
                WaterPressureAsphaltCoverSectionResultEntityId = 1
            };
            var persistentEntity = new WaterPressureAsphaltCoverSectionResultEntity
            {
                WaterPressureAsphaltCoverSectionResultEntityId = 2
            };
            dbContext.WaterPressureAsphaltCoverSectionResultEntities.Add(orphanedEntity);
            dbContext.WaterPressureAsphaltCoverSectionResultEntities.Add(persistentEntity);

            var section = new WaterPressureAsphaltCoverFailureMechanismSectionResult(new TestFailureMechanismSection())
            {
                StorageId = persistentEntity.WaterPressureAsphaltCoverSectionResultEntityId
            };

            var registry = new PersistenceRegistry();
            registry.Register(persistentEntity, section);

            // Call
            registry.RemoveUntouched(dbContext);

            // Assert
            Assert.AreEqual(1, dbContext.WaterPressureAsphaltCoverSectionResultEntities.Count());
            CollectionAssert.Contains(dbContext.WaterPressureAsphaltCoverSectionResultEntities, persistentEntity);
            mocks.VerifyAll();
        }

        [Test]
        public void RemoveUntouched_ClosingStructureSectionResultEntity_OrphanedEntityIsRemovedFromRingtoetsEntities()
        {
            // Setup
            var mocks = new MockRepository();
            IRingtoetsEntities dbContext = RingtoetsEntitiesHelper.CreateStub(mocks);
            mocks.ReplayAll();

            var orphanedEntity = new ClosingStructureSectionResultEntity
            {
                ClosingStructureSectionResultEntityId = 1
            };
            var persistentEntity = new ClosingStructureSectionResultEntity
            {
                ClosingStructureSectionResultEntityId = 2
            };
            dbContext.ClosingStructureSectionResultEntities.Add(orphanedEntity);
            dbContext.ClosingStructureSectionResultEntities.Add(persistentEntity);

            var section = new ClosingStructureFailureMechanismSectionResult(new TestFailureMechanismSection())
            {
                StorageId = persistentEntity.ClosingStructureSectionResultEntityId
            };

            var registry = new PersistenceRegistry();
            registry.Register(persistentEntity, section);

            // Call
            registry.RemoveUntouched(dbContext);

            // Assert
            Assert.AreEqual(1, dbContext.ClosingStructureSectionResultEntities.Count());
            CollectionAssert.Contains(dbContext.ClosingStructureSectionResultEntities, persistentEntity);
            mocks.VerifyAll();
        }

        [Test]
        public void RemoveUntouched_MacrostabilityInwardsSectionResultEntity_OrphanedEntityIsRemovedFromRingtoetsEntities()
        {
            // Setup
            var mocks = new MockRepository();
            IRingtoetsEntities dbContext = RingtoetsEntitiesHelper.CreateStub(mocks);
            mocks.ReplayAll();

            var orphanedEntity = new MacrostabilityInwardsSectionResultEntity
            {
                MacrostabilityInwardsSectionResultEntityId = 1
            };
            var persistentEntity = new MacrostabilityInwardsSectionResultEntity
            {
                MacrostabilityInwardsSectionResultEntityId = 2
            };
            dbContext.MacrostabilityInwardsSectionResultEntities.Add(orphanedEntity);
            dbContext.MacrostabilityInwardsSectionResultEntities.Add(persistentEntity);

            var section = new MacrostabilityInwardsFailureMechanismSectionResult(new TestFailureMechanismSection())
            {
                StorageId = persistentEntity.MacrostabilityInwardsSectionResultEntityId
            };

            var registry = new PersistenceRegistry();
            registry.Register(persistentEntity, section);

            // Call
            registry.RemoveUntouched(dbContext);

            // Assert
            Assert.AreEqual(1, dbContext.MacrostabilityInwardsSectionResultEntities.Count());
            CollectionAssert.Contains(dbContext.MacrostabilityInwardsSectionResultEntities, persistentEntity);
            mocks.VerifyAll();
        }

        [Test]
        public void RemoveUntouched_MacrostabilityOutwardsSectionResultEntity_OrphanedEntityIsRemovedFromRingtoetsEntities()
        {
            // Setup
            var mocks = new MockRepository();
            IRingtoetsEntities dbContext = RingtoetsEntitiesHelper.CreateStub(mocks);
            mocks.ReplayAll();

            var orphanedEntity = new MacrostabilityOutwardsSectionResultEntity
            {
                MacrostabilityOutwardsSectionResultEntityId = 1
            };
            var persistentEntity = new MacrostabilityOutwardsSectionResultEntity
            {
                MacrostabilityOutwardsSectionResultEntityId = 2
            };
            dbContext.MacrostabilityOutwardsSectionResultEntities.Add(orphanedEntity);
            dbContext.MacrostabilityOutwardsSectionResultEntities.Add(persistentEntity);

            var section = new MacrostabilityOutwardsFailureMechanismSectionResult(new TestFailureMechanismSection())
            {
                StorageId = persistentEntity.MacrostabilityOutwardsSectionResultEntityId
            };

            var registry = new PersistenceRegistry();
            registry.Register(persistentEntity, section);

            // Call
            registry.RemoveUntouched(dbContext);

            // Assert
            Assert.AreEqual(1, dbContext.MacrostabilityOutwardsSectionResultEntities.Count());
            CollectionAssert.Contains(dbContext.MacrostabilityOutwardsSectionResultEntities, persistentEntity);
            mocks.VerifyAll();
        }

        [Test]
        public void RemoveUntouched_WaveImpactAsphaltCoverSectionResultEntity_OrphanedEntityIsRemovedFromRingtoetsEntities()
        {
            // Setup
            var mocks = new MockRepository();
            IRingtoetsEntities dbContext = RingtoetsEntitiesHelper.CreateStub(mocks);
            mocks.ReplayAll();

            var orphanedEntity = new WaveImpactAsphaltCoverSectionResultEntity
            {
                WaveImpactAsphaltCoverSectionResultEntityId = 1
            };
            var persistentEntity = new WaveImpactAsphaltCoverSectionResultEntity
            {
                WaveImpactAsphaltCoverSectionResultEntityId = 2
            };
            dbContext.WaveImpactAsphaltCoverSectionResultEntities.Add(orphanedEntity);
            dbContext.WaveImpactAsphaltCoverSectionResultEntities.Add(persistentEntity);

            var section = new WaveImpactAsphaltCoverFailureMechanismSectionResult(new TestFailureMechanismSection())
            {
                StorageId = persistentEntity.WaveImpactAsphaltCoverSectionResultEntityId
            };

            var registry = new PersistenceRegistry();
            registry.Register(persistentEntity, section);

            // Call
            registry.RemoveUntouched(dbContext);

            // Assert
            Assert.AreEqual(1, dbContext.WaveImpactAsphaltCoverSectionResultEntities.Count());
            CollectionAssert.Contains(dbContext.WaveImpactAsphaltCoverSectionResultEntities, persistentEntity);
            mocks.VerifyAll();
        }

        [Test]
        public void RemoveUntouched_GrassCoverErosionOutwardsSectionResultEntity_OrphanedEntityIsRemovedFromRingtoetsEntities()
        {
            // Setup
            var mocks = new MockRepository();
            IRingtoetsEntities dbContext = RingtoetsEntitiesHelper.CreateStub(mocks);
            mocks.ReplayAll();

            var orphanedEntity = new GrassCoverErosionOutwardsSectionResultEntity
            {
                GrassCoverErosionOutwardsSectionResultEntityId = 1
            };
            var persistentEntity = new GrassCoverErosionOutwardsSectionResultEntity
            {
                GrassCoverErosionOutwardsSectionResultEntityId = 2
            };
            dbContext.GrassCoverErosionOutwardsSectionResultEntities.Add(orphanedEntity);
            dbContext.GrassCoverErosionOutwardsSectionResultEntities.Add(persistentEntity);

            var section = new GrassCoverErosionOutwardsFailureMechanismSectionResult(new TestFailureMechanismSection())
            {
                StorageId = persistentEntity.GrassCoverErosionOutwardsSectionResultEntityId
            };

            var registry = new PersistenceRegistry();
            registry.Register(persistentEntity, section);

            // Call
            registry.RemoveUntouched(dbContext);

            // Assert
            Assert.AreEqual(1, dbContext.GrassCoverErosionOutwardsSectionResultEntities.Count());
            CollectionAssert.Contains(dbContext.GrassCoverErosionOutwardsSectionResultEntities, persistentEntity);
            mocks.VerifyAll();
        }

        [Test]
        public void RemoveUntouched_GrassCoverSlipOffInwardsSectionResultEntity_OrphanedEntityIsRemovedFromRingtoetsEntities()
        {
            // Setup
            var mocks = new MockRepository();
            IRingtoetsEntities dbContext = RingtoetsEntitiesHelper.CreateStub(mocks);
            mocks.ReplayAll();

            var orphanedEntity = new GrassCoverSlipOffInwardsSectionResultEntity
            {
                GrassCoverSlipOffInwardsSectionResultEntityId = 1
            };
            var persistentEntity = new GrassCoverSlipOffInwardsSectionResultEntity
            {
                GrassCoverSlipOffInwardsSectionResultEntityId = 2
            };
            dbContext.GrassCoverSlipOffInwardsSectionResultEntities.Add(orphanedEntity);
            dbContext.GrassCoverSlipOffInwardsSectionResultEntities.Add(persistentEntity);

            var section = new GrassCoverSlipOffInwardsFailureMechanismSectionResult(new TestFailureMechanismSection())
            {
                StorageId = persistentEntity.GrassCoverSlipOffInwardsSectionResultEntityId
            };

            var registry = new PersistenceRegistry();
            registry.Register(persistentEntity, section);

            // Call
            registry.RemoveUntouched(dbContext);

            // Assert
            Assert.AreEqual(1, dbContext.GrassCoverSlipOffInwardsSectionResultEntities.Count());
            CollectionAssert.Contains(dbContext.GrassCoverSlipOffInwardsSectionResultEntities, persistentEntity);
            mocks.VerifyAll();
        }

        [Test]
        public void RemoveUntouched_GrassCoverSlipOffOutwardsSectionResultEntity_OrphanedEntityIsRemovedFromRingtoetsEntities()
        {
            // Setup
            var mocks = new MockRepository();
            IRingtoetsEntities dbContext = RingtoetsEntitiesHelper.CreateStub(mocks);
            mocks.ReplayAll();

            var orphanedEntity = new GrassCoverSlipOffOutwardsSectionResultEntity
            {
                GrassCoverSlipOffOutwardsSectionResultEntityId = 1
            };
            var persistentEntity = new GrassCoverSlipOffOutwardsSectionResultEntity
            {
                GrassCoverSlipOffOutwardsSectionResultEntityId = 2
            };
            dbContext.GrassCoverSlipOffOutwardsSectionResultEntities.Add(orphanedEntity);
            dbContext.GrassCoverSlipOffOutwardsSectionResultEntities.Add(persistentEntity);

            var section = new GrassCoverSlipOffOutwardsFailureMechanismSectionResult(new TestFailureMechanismSection())
            {
                StorageId = persistentEntity.GrassCoverSlipOffOutwardsSectionResultEntityId
            };

            var registry = new PersistenceRegistry();
            registry.Register(persistentEntity, section);

            // Call
            registry.RemoveUntouched(dbContext);

            // Assert
            Assert.AreEqual(1, dbContext.GrassCoverSlipOffOutwardsSectionResultEntities.Count());
            CollectionAssert.Contains(dbContext.GrassCoverSlipOffOutwardsSectionResultEntities, persistentEntity);
            mocks.VerifyAll();
        }

        [Test]
        public void RemoveUntouched_MicrostabilitySectionResultEntity_OrphanedEntityIsRemovedFromRingtoetsEntities()
        {
            // Setup
            var mocks = new MockRepository();
            IRingtoetsEntities dbContext = RingtoetsEntitiesHelper.CreateStub(mocks);
            mocks.ReplayAll();

            var orphanedEntity = new MicrostabilitySectionResultEntity
            {
                MicrostabilitySectionResultEntityId = 1
            };
            var persistentEntity = new MicrostabilitySectionResultEntity
            {
                MicrostabilitySectionResultEntityId = 2
            };
            dbContext.MicrostabilitySectionResultEntities.Add(orphanedEntity);
            dbContext.MicrostabilitySectionResultEntities.Add(persistentEntity);

            var section = new MicrostabilityFailureMechanismSectionResult(new TestFailureMechanismSection())
            {
                StorageId = persistentEntity.MicrostabilitySectionResultEntityId
            };

            var registry = new PersistenceRegistry();
            registry.Register(persistentEntity, section);

            // Call
            registry.RemoveUntouched(dbContext);

            // Assert
            Assert.AreEqual(1, dbContext.MicrostabilitySectionResultEntities.Count());
            CollectionAssert.Contains(dbContext.MicrostabilitySectionResultEntities, persistentEntity);
            mocks.VerifyAll();
        }

        [Test]
        public void RemoveUntouched_PipingStructureSectionResultEntity_OrphanedEntityIsRemovedFromRingtoetsEntities()
        {
            // Setup
            var mocks = new MockRepository();
            IRingtoetsEntities dbContext = RingtoetsEntitiesHelper.CreateStub(mocks);
            mocks.ReplayAll();

            var orphanedEntity = new PipingStructureSectionResultEntity
            {
                PipingStructureSectionResultEntityId = 1
            };
            var persistentEntity = new PipingStructureSectionResultEntity
            {
                PipingStructureSectionResultEntityId = 2
            };
            dbContext.PipingStructureSectionResultEntities.Add(orphanedEntity);
            dbContext.PipingStructureSectionResultEntities.Add(persistentEntity);

            var section = new PipingStructureFailureMechanismSectionResult(new TestFailureMechanismSection())
            {
                StorageId = persistentEntity.PipingStructureSectionResultEntityId
            };

            var registry = new PersistenceRegistry();
            registry.Register(persistentEntity, section);

            // Call
            registry.RemoveUntouched(dbContext);

            // Assert
            Assert.AreEqual(1, dbContext.PipingStructureSectionResultEntities.Count());
            CollectionAssert.Contains(dbContext.PipingStructureSectionResultEntities, persistentEntity);
            mocks.VerifyAll();
        }

        [Test]
        public void RemoveUntouched_DuneErosionSectionResultEntity_OrphanedEntityIsRemovedFromRingtoetsEntities()
        {
            // Setup
            var mocks = new MockRepository();
            IRingtoetsEntities dbContext = RingtoetsEntitiesHelper.CreateStub(mocks);
            mocks.ReplayAll();

            var orphanedEntity = new DuneErosionSectionResultEntity
            {
                DuneErosionSectionResultEntityId = 1
            };
            var persistentEntity = new DuneErosionSectionResultEntity
            {
                DuneErosionSectionResultEntityId = 2
            };
            dbContext.DuneErosionSectionResultEntities.Add(orphanedEntity);
            dbContext.DuneErosionSectionResultEntities.Add(persistentEntity);

            var section = new DuneErosionFailureMechanismSectionResult(new TestFailureMechanismSection())
            {
                StorageId = persistentEntity.DuneErosionSectionResultEntityId
            };

            var registry = new PersistenceRegistry();
            registry.Register(persistentEntity, section);

            // Call
            registry.RemoveUntouched(dbContext);

            // Assert
            Assert.AreEqual(1, dbContext.DuneErosionSectionResultEntities.Count());
            CollectionAssert.Contains(dbContext.DuneErosionSectionResultEntities, persistentEntity);
            mocks.VerifyAll();
        }

        [Test]
        public void RemoveUntouched_StabilityStoneCoverSectionResultEntity_OrphanedEntityIsRemovedFromRingtoetsEntities()
        {
            // Setup
            var mocks = new MockRepository();
            IRingtoetsEntities dbContext = RingtoetsEntitiesHelper.CreateStub(mocks);
            mocks.ReplayAll();

            var orphanedEntity = new StabilityStoneCoverSectionResultEntity
            {
                StabilityStoneCoverSectionResultEntityId = 1
            };
            var persistentEntity = new StabilityStoneCoverSectionResultEntity
            {
                StabilityStoneCoverSectionResultEntityId = 2
            };
            dbContext.StabilityStoneCoverSectionResultEntities.Add(orphanedEntity);
            dbContext.StabilityStoneCoverSectionResultEntities.Add(persistentEntity);

            var section = new StabilityStoneCoverFailureMechanismSectionResult(new TestFailureMechanismSection())
            {
                StorageId = persistentEntity.StabilityStoneCoverSectionResultEntityId
            };

            var registry = new PersistenceRegistry();
            registry.Register(persistentEntity, section);

            // Call
            registry.RemoveUntouched(dbContext);

            // Assert
            Assert.AreEqual(1, dbContext.StabilityStoneCoverSectionResultEntities.Count());
            CollectionAssert.Contains(dbContext.StabilityStoneCoverSectionResultEntities, persistentEntity);
            mocks.VerifyAll();
        }

        [Test]
        public void RemoveUntouched_StrengthStabilityPointConstructionSectionResultEntity_OrphanedEntityIsRemovedFromRingtoetsEntities()
        {
            // Setup
            var mocks = new MockRepository();
            IRingtoetsEntities dbContext = RingtoetsEntitiesHelper.CreateStub(mocks);
            mocks.ReplayAll();

            var orphanedEntity = new StrengthStabilityPointConstructionSectionResultEntity
            {
                StrengthStabilityPointConstructionSectionResultEntityId = 1
            };
            var persistentEntity = new StrengthStabilityPointConstructionSectionResultEntity
            {
                StrengthStabilityPointConstructionSectionResultEntityId = 2
            };
            dbContext.StrengthStabilityPointConstructionSectionResultEntities.Add(orphanedEntity);
            dbContext.StrengthStabilityPointConstructionSectionResultEntities.Add(persistentEntity);

            var section = new StrengthStabilityPointConstructionFailureMechanismSectionResult(new TestFailureMechanismSection())
            {
                StorageId = persistentEntity.StrengthStabilityPointConstructionSectionResultEntityId
            };

            var registry = new PersistenceRegistry();
            registry.Register(persistentEntity, section);

            // Call
            registry.RemoveUntouched(dbContext);

            // Assert
            Assert.AreEqual(1, dbContext.StrengthStabilityPointConstructionSectionResultEntities.Count());
            CollectionAssert.Contains(dbContext.StrengthStabilityPointConstructionSectionResultEntities, persistentEntity);
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

            var boundaryLocation = new HydraulicBoundaryLocation(123, "A", 1, 2)
            {
                StorageId = persistentEntity.HydraulicLocationEntityId
            };

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

            var calculationGroup = new CalculationGroup
            {
                StorageId = persistentEntity.CalculationGroupEntityId
            };

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

            var pipingOutput = new PipingOutput(1, 2, 3, 4, 5, 6)
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

            var pipingSemiProbabilisticOutput = new PipingSemiProbabilisticOutput(1, 2, 0.3, 4, 5, 0.6, 7,
                                                                                  8, 0.9, 1.0, 11, 0.3, 13, 14)
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

            var soilModel = new StochasticSoilModel(123, "A", "B")
            {
                StorageId = persistentEntity.StochasticSoilModelEntityId
            };

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

            var stochasticSoilProfile = new StochasticSoilProfile(1.0, SoilProfileType.SoilProfile1D, 123)
            {
                StorageId = persistentEntity.StochasticSoilProfileEntityId
            };

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