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
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Ringtoets.ClosingStructures.Data;
using Ringtoets.ClosingStructures.Data.TestUtil;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.HeightStructures.Data;
using Ringtoets.HeightStructures.Data.TestUtil;
using Ringtoets.HydraRing.Data;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Data.TestUtil;
using Ringtoets.Piping.KernelWrapper.TestUtil;
using Ringtoets.Piping.Primitives;
using Ringtoets.StabilityPointStructures.Data;
using Ringtoets.StabilityPointStructures.Data.TestUtil;

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
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
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
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
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
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
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
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
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
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
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
            var dikeProfile = CreateDikeProfile();
            var otherDikeProfile = CreateDikeProfile();

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
            var dikeProfile = CreateDikeProfile();

            var registry = new PersistenceRegistry();

            // Call
            bool result = registry.Contains(dikeProfile);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Contains_WithoutForeshoreProfile_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate call = () => registry.Contains((ForeshoreProfile) null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("model", paramName);
        }

        [Test]
        public void Contains_ForeshoreProfileAdded_ReturnsTrue()
        {
            // Setup
            var foreshoreProfile = new TestForeshoreProfile();
            var registry = new PersistenceRegistry();
            registry.Register(new ForeshoreProfileEntity(), foreshoreProfile);

            // Call
            bool result = registry.Contains(foreshoreProfile);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void Contains_OtherForeshoreProfileAdded_ReturnsFalse()
        {
            // Setup
            var foreshoreProfile = new TestForeshoreProfile();
            var otherForeshoreProfile = new TestForeshoreProfile();

            var registry = new PersistenceRegistry();
            registry.Register(new ForeshoreProfileEntity(), otherForeshoreProfile);

            // Call
            bool result = registry.Contains(foreshoreProfile);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Contains_NoForeshoreProfileAdded_ReturnsFalse()
        {
            // Setup
            var foreshoreProfile = new TestForeshoreProfile();

            var registry = new PersistenceRegistry();

            // Call
            bool result = registry.Contains(foreshoreProfile);

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

        [Test]
        public void Contains_WithoutHeightStructure_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate call = () => registry.Contains((HeightStructure) null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("model", paramName);
        }

        [Test]
        public void Contains_HeightStructureAdded_ReturnsTrue()
        {
            // Setup
            HeightStructure heightStructure = new TestHeightStructure();
            var registry = new PersistenceRegistry();
            registry.Register(new HeightStructureEntity(), heightStructure);

            // Call
            bool result = registry.Contains(heightStructure);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void Contains_OtherHeightStructureAdded_ReturnsFalse()
        {
            // Setup
            HeightStructure heightStructure = new TestHeightStructure();

            HeightStructure otherStructure = new TestHeightStructure();
            var registry = new PersistenceRegistry();
            registry.Register(new HeightStructureEntity(), otherStructure);

            // Call
            bool result = registry.Contains(heightStructure);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Contains_NoHeightStructureAdded_ReturnsFalse()
        {
            // Setup
            HeightStructure heightStructure = new TestHeightStructure();

            var registry = new PersistenceRegistry();

            // Call
            bool result = registry.Contains(heightStructure);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Contains_WithoutClosingStructure_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate call = () => registry.Contains((ClosingStructure) null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("model", paramName);
        }

        [Test]
        public void Contains_ClosingStructureAdded_ReturnsTrue()
        {
            // Setup
            ClosingStructure closingStructure = new TestClosingStructure();
            var registry = new PersistenceRegistry();
            registry.Register(new ClosingStructureEntity(), closingStructure);

            // Call
            bool result = registry.Contains(closingStructure);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void Contains_OtherClosingStructureAdded_ReturnsFalse()
        {
            // Setup
            ClosingStructure closingStructure = new TestClosingStructure();

            ClosingStructure otherStructure = new TestClosingStructure();
            var registry = new PersistenceRegistry();
            registry.Register(new ClosingStructureEntity(), otherStructure);

            // Call
            bool result = registry.Contains(closingStructure);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Contains_NoClosingStructureAdded_ReturnsFalse()
        {
            // Setup
            ClosingStructure closingStructure = new TestClosingStructure();

            var registry = new PersistenceRegistry();

            // Call
            bool result = registry.Contains(closingStructure);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Contains_WithoutStabilityPointStructure_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate call = () => registry.Contains((StabilityPointStructure) null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("model", paramName);
        }

        [Test]
        public void Contains_StabilityPointStructureAdded_ReturnsTrue()
        {
            // Setup
            StabilityPointStructure stabilityPointStructure = new TestStabilityPointStructure();
            var registry = new PersistenceRegistry();
            registry.Register(new StabilityPointStructureEntity(), stabilityPointStructure);

            // Call
            bool result = registry.Contains(stabilityPointStructure);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void Contains_OtherStabilityPointStructureAdded_ReturnsFalse()
        {
            // Setup
            StabilityPointStructure stabilityPointStructure = new TestStabilityPointStructure();

            StabilityPointStructure otherStructure = new TestStabilityPointStructure();
            var registry = new PersistenceRegistry();
            registry.Register(new StabilityPointStructureEntity(), otherStructure);

            // Call
            bool result = registry.Contains(stabilityPointStructure);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Contains_NoStabilityPointStructureAdded_ReturnsFalse()
        {
            // Setup
            StabilityPointStructure stabilityPointStructure = new TestStabilityPointStructure();

            var registry = new PersistenceRegistry();

            // Call
            bool result = registry.Contains(stabilityPointStructure);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Contains_WithoutHeightStructuresCalculation_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate call = () => registry.Contains((StructuresCalculation<HeightStructuresInput>) null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("model", paramName);
        }

        [Test]
        public void Contains_HeightStructuresCalculationAdded_ReturnsTrue()
        {
            // Setup
            var calculation = new StructuresCalculation<HeightStructuresInput>();
            var registry = new PersistenceRegistry();
            registry.Register(new HeightStructuresCalculationEntity(), calculation);

            // Call
            bool result = registry.Contains(calculation);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void Contains_OtherHeightStructuresCalculationAdded_ReturnsFalse()
        {
            // Setup
            var calculation = new StructuresCalculation<HeightStructuresInput>();

            var otherCalculation = new StructuresCalculation<HeightStructuresInput>();
            var registry = new PersistenceRegistry();
            registry.Register(new HeightStructuresCalculationEntity(), otherCalculation);

            // Call
            bool result = registry.Contains(calculation);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Contains_NoHeightStructuresCalculationAdded_ReturnsFalse()
        {
            // Setup
            var calculation = new StructuresCalculation<HeightStructuresInput>();

            var registry = new PersistenceRegistry();

            // Call
            bool result = registry.Contains(calculation);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Contains_WithoutClosingStructuresCalculation_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate call = () => registry.Contains((StructuresCalculation<ClosingStructuresInput>) null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("model", paramName);
        }

        [Test]
        public void Contains_ClosingStructuresCalculationAdded_ReturnsTrue()
        {
            // Setup
            var calculation = new StructuresCalculation<ClosingStructuresInput>();
            var registry = new PersistenceRegistry();
            registry.Register(new ClosingStructuresCalculationEntity(), calculation);

            // Call
            bool result = registry.Contains(calculation);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void Contains_OtherClosingStructuresCalculationAdded_ReturnsFalse()
        {
            // Setup
            var calculation = new StructuresCalculation<ClosingStructuresInput>();

            var otherCalculation = new StructuresCalculation<ClosingStructuresInput>();
            var registry = new PersistenceRegistry();
            registry.Register(new ClosingStructuresCalculationEntity(), otherCalculation);

            // Call
            bool result = registry.Contains(calculation);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Contains_NoClosingStructuresCalculationAdded_ReturnsFalse()
        {
            // Setup
            var calculation = new StructuresCalculation<ClosingStructuresInput>();
            var registry = new PersistenceRegistry();

            // Call
            bool result = registry.Contains(calculation);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Contains_WithoutStabilityPointStructuresCalculation_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate call = () => registry.Contains((StructuresCalculation<StabilityPointStructuresInput>) null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("model", paramName);
        }

        [Test]
        public void Contains_StabilityPointStructuresCalculationAdded_ReturnsTrue()
        {
            // Setup
            var calculation = new StructuresCalculation<StabilityPointStructuresInput>();
            var registry = new PersistenceRegistry();
            registry.Register(new StabilityPointStructuresCalculationEntity(), calculation);

            // Call
            bool result = registry.Contains(calculation);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void Contains_OtherStabilityPointStructuresCalculationAdded_ReturnsFalse()
        {
            // Setup
            var calculation = new StructuresCalculation<StabilityPointStructuresInput>();

            var otherCalculation = new StructuresCalculation<StabilityPointStructuresInput>();
            var registry = new PersistenceRegistry();
            registry.Register(new StabilityPointStructuresCalculationEntity(), otherCalculation);

            // Call
            bool result = registry.Contains(calculation);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Contains_NoStabilityPointStructuresCalculationAdded_ReturnsFalse()
        {
            // Setup
            var calculation = new StructuresCalculation<StabilityPointStructuresInput>();
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
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
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
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
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
            TestDelegate test = () => registry.Get<HydraulicBoundaryLocation>(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
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
            HydraulicLocationEntity result = registry.Get<HydraulicLocationEntity>(hydraulicBoundaryLocation);

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
            TestDelegate test = () => registry.Get<HydraulicLocationEntity>(hydraulicBoundaryLocation);

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
            TestDelegate test = () => registry.Get<HydraulicLocationEntity>(hydraulicBoundaryLocation);

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
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
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
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
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
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
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
            var dikeProfile = CreateDikeProfile();
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
            var dikeProfile = CreateDikeProfile();
            var registeredDikeProfile = CreateDikeProfile();
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
            var dikeProfile = CreateDikeProfile();
            var registeredEntity = new DikeProfileEntity();

            var registry = new PersistenceRegistry();
            registry.Register(registeredEntity, dikeProfile);

            // Call
            DikeProfileEntity retrievedEntity = registry.Get(dikeProfile);

            // Assert
            Assert.AreSame(registeredEntity, retrievedEntity);
        }

        [Test]
        public void Get_WithoutForeshoreProfile_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate call = () => registry.Get((ForeshoreProfile) null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("model", paramName);
        }

        [Test]
        public void Get_NoForeshoreProfileAdded_ThrowsInvalidOperationException()
        {
            // Setup
            var foreshoreProfile = new TestForeshoreProfile();
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate call = () => registry.Get(foreshoreProfile);

            // Assert
            Assert.Throws<InvalidOperationException>(call);
        }

        [Test]
        public void Get_OtherForeshoreProfileAdded_ThrowsInvalidOperationException()
        {
            // Setup
            var foreshoreProfile = new TestForeshoreProfile();
            var registeredForeshoreProfile = new TestForeshoreProfile();
            var registeredEntity = new ForeshoreProfileEntity();

            var registry = new PersistenceRegistry();
            registry.Register(registeredEntity, registeredForeshoreProfile);

            // Call
            TestDelegate call = () => registry.Get(foreshoreProfile);

            // Assert
            Assert.Throws<InvalidOperationException>(call);
        }

        [Test]
        public void Get_ForeshoreProfileAdded_ReturnsEntity()
        {
            // Setup
            var foreshoreProfile = new TestForeshoreProfile();
            var registeredEntity = new ForeshoreProfileEntity();

            var registry = new PersistenceRegistry();
            registry.Register(registeredEntity, foreshoreProfile);

            // Call
            ForeshoreProfileEntity retrievedEntity = registry.Get(foreshoreProfile);

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

        [Test]
        public void Get_WithoutHeightStructure_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate call = () => registry.Get((HeightStructure) null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("model", paramName);
        }

        [Test]
        public void Get_NoHeightStructureAdded_ThrowsInvalidOperationException()
        {
            // Setup
            HeightStructure heightStructure = new TestHeightStructure();
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate call = () => registry.Get(heightStructure);

            // Assert
            Assert.Throws<InvalidOperationException>(call);
        }

        [Test]
        public void Get_OtherHeightStructureAdded_ThrowsInvalidOperationException()
        {
            // Setup
            HeightStructure heightStructure = new TestHeightStructure();
            HeightStructure registeredStructure = new TestHeightStructure();
            var registeredEntity = new HeightStructureEntity();

            var registry = new PersistenceRegistry();
            registry.Register(registeredEntity, registeredStructure);

            // Call
            TestDelegate call = () => registry.Get(heightStructure);

            // Assert
            Assert.Throws<InvalidOperationException>(call);
        }

        [Test]
        public void Get_HeightStructureAdded_ReturnsEntity()
        {
            // Setup
            HeightStructure heightStructure = new TestHeightStructure();
            var registeredEntity = new HeightStructureEntity();

            var registry = new PersistenceRegistry();
            registry.Register(registeredEntity, heightStructure);

            // Call
            HeightStructureEntity retrievedEntity = registry.Get(heightStructure);

            // Assert
            Assert.AreSame(registeredEntity, retrievedEntity);
        }

        [Test]
        public void Get_WithoutStabilityPointStructure_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate call = () => registry.Get((StabilityPointStructure) null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("model", paramName);
        }

        [Test]
        public void Get_NoStabilityPointStructureAdded_ThrowsInvalidOperationException()
        {
            // Setup
            StabilityPointStructure stabilityPointStructure = new TestStabilityPointStructure();
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate call = () => registry.Get(stabilityPointStructure);

            // Assert
            Assert.Throws<InvalidOperationException>(call);
        }

        [Test]
        public void Get_OtherStabilityPointStructureAdded_ThrowsInvalidOperationException()
        {
            // Setup
            StabilityPointStructure stabilityPointStructure = new TestStabilityPointStructure();
            StabilityPointStructure registeredStructure = new TestStabilityPointStructure();
            var registeredEntity = new StabilityPointStructureEntity();

            var registry = new PersistenceRegistry();
            registry.Register(registeredEntity, registeredStructure);

            // Call
            TestDelegate call = () => registry.Get(stabilityPointStructure);

            // Assert
            Assert.Throws<InvalidOperationException>(call);
        }

        [Test]
        public void Get_StabilityPointStructureAdded_ReturnsEntity()
        {
            // Setup
            StabilityPointStructure stabilityPointStructure = new TestStabilityPointStructure();
            var registeredEntity = new StabilityPointStructureEntity();

            var registry = new PersistenceRegistry();
            registry.Register(registeredEntity, stabilityPointStructure);

            // Call
            StabilityPointStructureEntity retrievedEntity = registry.Get(stabilityPointStructure);

            // Assert
            Assert.AreSame(registeredEntity, retrievedEntity);
        }

        [Test]
        public void Get_WithoutHeightStructuresCalculation_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate call = () => registry.Get((StructuresCalculation<HeightStructuresInput>) null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("model", paramName);
        }

        [Test]
        public void Get_NoHeightStructuresCalculationAdded_ThrowsInvalidOperationException()
        {
            // Setup
            var calculation = new StructuresCalculation<HeightStructuresInput>();
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate call = () => registry.Get(calculation);

            // Assert
            Assert.Throws<InvalidOperationException>(call);
        }

        [Test]
        public void Get_OtherHeightStructuresCalculationAdded_ThrowsInvalidOperationException()
        {
            // Setup
            var calculation = new StructuresCalculation<HeightStructuresInput>();
            var registeredCalculation = new StructuresCalculation<HeightStructuresInput>();
            var registeredEntity = new HeightStructuresCalculationEntity();

            var registry = new PersistenceRegistry();
            registry.Register(registeredEntity, registeredCalculation);

            // Call
            TestDelegate call = () => registry.Get(calculation);

            // Assert
            Assert.Throws<InvalidOperationException>(call);
        }

        [Test]
        public void Get_HeightStructuresCalculationAdded_ReturnsEntity()
        {
            // Setup
            var calculation = new StructuresCalculation<HeightStructuresInput>();
            var registeredEntity = new HeightStructuresCalculationEntity();

            var registry = new PersistenceRegistry();
            registry.Register(registeredEntity, calculation);

            // Call
            HeightStructuresCalculationEntity retrievedEntity = registry.Get(calculation);

            // Assert
            Assert.AreSame(registeredEntity, retrievedEntity);
        }

        [Test]
        public void Get_WithoutClosingStructuresCalculation_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate call = () => registry.Get((StructuresCalculation<ClosingStructuresInput>) null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("model", paramName);
        }

        [Test]
        public void Get_NoClosingStructuresCalculationAdded_ThrowsInvalidOperationException()
        {
            // Setup
            var calculation = new StructuresCalculation<ClosingStructuresInput>();
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate call = () => registry.Get(calculation);

            // Assert
            Assert.Throws<InvalidOperationException>(call);
        }

        [Test]
        public void Get_OtherClosingStructuresCalculationAdded_ThrowsInvalidOperationException()
        {
            // Setup
            var calculation = new StructuresCalculation<ClosingStructuresInput>();
            var registeredCalculation = new StructuresCalculation<ClosingStructuresInput>();
            var registeredEntity = new ClosingStructuresCalculationEntity();

            var registry = new PersistenceRegistry();
            registry.Register(registeredEntity, registeredCalculation);

            // Call
            TestDelegate call = () => registry.Get(calculation);

            // Assert
            Assert.Throws<InvalidOperationException>(call);
        }

        [Test]
        public void Get_ClosingStructuresCalculationAdded_ReturnsEntity()
        {
            // Setup
            var calculation = new StructuresCalculation<ClosingStructuresInput>();
            var registeredEntity = new ClosingStructuresCalculationEntity();

            var registry = new PersistenceRegistry();
            registry.Register(registeredEntity, calculation);

            // Call
            ClosingStructuresCalculationEntity retrievedEntity = registry.Get(calculation);

            // Assert
            Assert.AreSame(registeredEntity, retrievedEntity);
        }

        [Test]
        public void Get_WithoutStabilityPointStructuresCalculation_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate call = () => registry.Get((StructuresCalculation<StabilityPointStructuresInput>) null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("model", paramName);
        }

        [Test]
        public void Get_NoStabilityPointStructuresCalculationAdded_ThrowsInvalidOperationException()
        {
            // Setup
            var calculation = new StructuresCalculation<StabilityPointStructuresInput>();
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate call = () => registry.Get(calculation);

            // Assert
            Assert.Throws<InvalidOperationException>(call);
        }

        [Test]
        public void Get_OtherStabilityPointStructuresCalculationAdded_ThrowsInvalidOperationException()
        {
            // Setup
            var calculation = new StructuresCalculation<StabilityPointStructuresInput>();
            var registeredCalculation = new StructuresCalculation<StabilityPointStructuresInput>();
            var registeredEntity = new StabilityPointStructuresCalculationEntity();

            var registry = new PersistenceRegistry();
            registry.Register(registeredEntity, registeredCalculation);

            // Call
            TestDelegate call = () => registry.Get(calculation);

            // Assert
            Assert.Throws<InvalidOperationException>(call);
        }

        [Test]
        public void Get_StabilityPointStructuresCalculationAdded_ReturnsEntity()
        {
            // Setup
            var calculation = new StructuresCalculation<StabilityPointStructuresInput>();
            var registeredEntity = new StabilityPointStructuresCalculationEntity();

            var registry = new PersistenceRegistry();
            registry.Register(registeredEntity, calculation);

            // Call
            StabilityPointStructuresCalculationEntity retrievedEntity = registry.Get(calculation);

            // Assert
            Assert.AreSame(registeredEntity, retrievedEntity);
        }

        #endregion

        #region Register methods

        [Test]
        public void Register_WithNullFailureMechanismSection_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate test = () => registry.Register(new FailureMechanismSectionEntity(), null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
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
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
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
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
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
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("entity", paramName);
        }

        [Test]
        public void Register_WithNullForeshoreProfile_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate test = () => registry.Register(new ForeshoreProfileEntity(), null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("model", paramName);
        }

        [Test]
        public void Register_WithNullForeshoreProfileEntity_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate test = () => registry.Register(null, new TestForeshoreProfile());

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
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
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
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
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("entity", paramName);
        }

        [Test]
        public void Register_WithNullHydraulicLocationEntity_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate test = () => registry.Register((HydraulicLocationEntity) null, new HydraulicBoundaryLocation(-1, "name", 0, 0));

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
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
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
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
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
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
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
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
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
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
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
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
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
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
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("model", paramName);
        }

        [Test]
        public void Register_WithNullHeightStructureEntity_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate test = () => registry.Register(null, new TestHeightStructure());

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("entity", paramName);
        }

        [Test]
        public void Register_WithNullHeightStructure_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate test = () => registry.Register(new HeightStructureEntity(), null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("model", paramName);
        }

        [Test]
        public void Register_WithNullStabilityPointStructureEntity_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate test = () => registry.Register(null, new TestStabilityPointStructure());

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("entity", paramName);
        }

        [Test]
        public void Register_WithNullStabilityPointStructure_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate test = () => registry.Register(new StabilityPointStructureEntity(), null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("model", paramName);
        }

        [Test]
        public void Register_WithNullHeightStructuresCalculationEntity_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate test = () => registry.Register(null, new TestHeightStructuresCalculation());

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("entity", paramName);
        }

        [Test]
        public void Register_WithNullHeightStructuresCalculation_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate test = () => registry.Register(new HeightStructuresCalculationEntity(), null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("model", paramName);
        }

        [Test]
        public void Register_WithNullClosingStructuresCalculationEntity_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate test = () => registry.Register(null, new TestClosingStructuresCalculation());

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("entity", paramName);
        }

        [Test]
        public void Register_WithNullClosingStructuresCalculation_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate test = () => registry.Register(new ClosingStructuresCalculationEntity(), null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("model", paramName);
        }

        [Test]
        public void Register_WithNullStabilityPointStructuresCalculationEntity_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate test = () => registry.Register(null, new TestStabilityPointStructuresCalculation());

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("entity", paramName);
        }

        [Test]
        public void Register_WithNullStabilityPointStructuresCalculation_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate test = () => registry.Register(new StabilityPointStructuresCalculationEntity(), null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("model", paramName);
        }

        #endregion
    }
}