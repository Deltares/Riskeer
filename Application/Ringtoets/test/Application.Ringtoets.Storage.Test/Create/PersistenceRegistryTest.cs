﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Ringtoets.ClosingStructures.Data;
using Ringtoets.ClosingStructures.Data.TestUtil;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.DuneErosion.Data;
using Ringtoets.DuneErosion.Data.TestUtil;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.HeightStructures.Data;
using Ringtoets.HeightStructures.Data.TestUtil;
using Ringtoets.MacroStabilityInwards.Primitives;
using Ringtoets.MacroStabilityInwards.Primitives.TestUtil;
using Ringtoets.Piping.Data.SoilProfile;
using Ringtoets.Piping.Primitives;
using Ringtoets.Piping.Primitives.TestUtil;
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
                                   null, new DikeProfile.ConstructionProperties
                                   {
                                       Id = "id"
                                   });
        }

        [TestFixture]
        private class PipingSoilProfileTest : RegistryTest<PipingSoilProfile,
            PipingSoilProfileEntity>
        {
            protected override PipingSoilProfile CreateDataModel()
            {
                return PipingSoilProfileTestFactory.CreatePipingSoilProfile();
            }

            protected override PipingSoilProfileEntity Get(PersistenceRegistry registry,
                                                           PipingSoilProfile model)
            {
                return registry.Get(model);
            }

            protected override bool Contains(PersistenceRegistry registry, PipingSoilProfile model)
            {
                return registry.Contains(model);
            }

            protected override void Register(PersistenceRegistry registry, PipingSoilProfileEntity entity,
                                             PipingSoilProfile model)
            {
                registry.Register(entity, model);
            }
        }

        [TestFixture]
        private class MacroStabilityInwardsSoilProfile1DTest : RegistryTest<MacroStabilityInwardsSoilProfile1D,
            MacroStabilityInwardsSoilProfile1DEntity>
        {
            protected override MacroStabilityInwardsSoilProfile1D CreateDataModel()
            {
                return new TestMacroStabilityInwardsSoilProfile1D();
            }

            protected override MacroStabilityInwardsSoilProfile1DEntity Get(PersistenceRegistry registry,
                                                                            MacroStabilityInwardsSoilProfile1D model)
            {
                return registry.Get(model);
            }

            protected override bool Contains(PersistenceRegistry registry, MacroStabilityInwardsSoilProfile1D model)
            {
                return registry.Contains(model);
            }

            protected override void Register(PersistenceRegistry registry, MacroStabilityInwardsSoilProfile1DEntity entity,
                                             MacroStabilityInwardsSoilProfile1D model)
            {
                registry.Register(entity, model);
            }
        }

        [TestFixture]
        private class MacroStabilityInwardsSoilProfile2DTest : RegistryTest<MacroStabilityInwardsSoilProfile2D,
            MacroStabilityInwardsSoilProfile2DEntity>
        {
            protected override MacroStabilityInwardsSoilProfile2D CreateDataModel()
            {
                return new MacroStabilityInwardsSoilProfile2D("", new[]
                {
                    new MacroStabilityInwardsSoilLayer2D(new Ring(new[]
                    {
                        new Point2D(0, 0),
                        new Point2D(1, 1)
                    }), new Ring[0])
                }, Enumerable.Empty<MacroStabilityInwardsPreconsolidationStress>());
            }

            protected override MacroStabilityInwardsSoilProfile2DEntity Get(PersistenceRegistry registry,
                                                                            MacroStabilityInwardsSoilProfile2D model)
            {
                return registry.Get(model);
            }

            protected override bool Contains(PersistenceRegistry registry, MacroStabilityInwardsSoilProfile2D model)
            {
                return registry.Contains(model);
            }

            protected override void Register(PersistenceRegistry registry, MacroStabilityInwardsSoilProfile2DEntity entity,
                                             MacroStabilityInwardsSoilProfile2D model)
            {
                registry.Register(entity, model);
            }
        }

        [TestFixture]
        private abstract class RegistryTest<TDataModel, TEntity> where TDataModel : class
                                                                 where TEntity : class, new()
        {
            [Test]
            public void Register_WithNullEntity_ThrowsArgumentNullException()
            {
                // Setup
                var registry = new PersistenceRegistry();

                // Call
                TestDelegate test = () => Register(registry, null, CreateDataModel());

                // Assert
                string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
                Assert.AreEqual("entity", paramName);
            }

            [Test]
            public void Register_WithNullDataModel_ThrowsArgumentNullException()
            {
                // Setup
                var registry = new PersistenceRegistry();

                // Call
                TestDelegate test = () => Register(registry, new TEntity(), null);

                // Assert
                string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
                Assert.AreEqual("model", paramName);
            }

            [Test]
            public void Contains_DataModelNull_ThrowsArgumentNullException()
            {
                // Setup
                var registry = new PersistenceRegistry();

                // Call
                TestDelegate test = () => Contains(registry, null);

                // Assert
                string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
                Assert.AreEqual("model", paramName);
            }

            [Test]
            public void Contains_DataModelAdded_ReturnsTrue()
            {
                // Setup
                var registry = new PersistenceRegistry();
                TDataModel dataModel = CreateDataModel();
                Register(registry, new TEntity(), dataModel);

                // Call
                bool result = Contains(registry, dataModel);

                // Assert
                Assert.IsTrue(result);
            }

            [Test]
            public void Contains_NoDataModelAdded_ReturnsFalse()
            {
                // Setup
                var registry = new PersistenceRegistry();
                TDataModel dataModel = CreateDataModel();

                // Call
                bool result = Contains(registry, dataModel);

                // Assert
                Assert.IsFalse(result);
            }

            [Test]
            public void Get_DataModelNull_ThrowsArgumentNullException()
            {
                // Setup
                var registry = new PersistenceRegistry();

                // Call
                TestDelegate test = () => Get(registry, null);

                // Assert
                string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
                Assert.AreEqual("model", paramName);
            }

            [Test]
            public void Get_DataModelAdded_ReturnsEntity()
            {
                // Setup
                var registry = new PersistenceRegistry();
                TDataModel dataModel = CreateDataModel();
                var entity = new TEntity();

                Register(registry, entity, dataModel);

                // Call
                TEntity result = Get(registry, dataModel);

                // Assert
                Assert.AreSame(entity, result);
            }

            [Test]
            public void Get_NoDataModelAdded_ThrowsInvalidOperationException()
            {
                // Setup
                var registry = new PersistenceRegistry();
                TDataModel dataModel = CreateDataModel();

                // Call
                TestDelegate test = () => Get(registry, dataModel);

                // Assert
                Assert.Throws<InvalidOperationException>(test);
            }

            [Test]
            public void Get_OtherDataModelAdded_ThrowsInvalidOperationException()
            {
                // Setup
                var registry = new PersistenceRegistry();
                Register(registry, new TEntity(), CreateDataModel());

                // Call
                TestDelegate test = () => Get(registry, CreateDataModel());

                // Assert
                Assert.Throws<InvalidOperationException>(test);
            }

            /// <summary>
            /// Creates a new instance of <see cref="TDataModel"/>.
            /// </summary>
            /// <returns></returns>
            protected abstract TDataModel CreateDataModel();

            /// <summary>
            /// Obtains the <see cref="TEntity"/> which was registered for the given <paramref name="model"/>.
            /// </summary>
            /// <param name="registry">The registry to use.</param>
            /// <param name="model">The <see cref="TEntity"/> for which a create operation has 
            /// been registered.</param>
            /// <returns>The constructed <see cref="TDataModel"/>.</returns>
            /// <exception cref="ArgumentNullException">Thrown when <paramref name="model"/> is 
            /// <c>null</c>.</exception>
            /// <exception cref="InvalidOperationException">Thrown when no create operation has 
            /// been registered for <paramref name="model"/>.</exception>
            protected abstract TEntity Get(PersistenceRegistry registry, TDataModel model);

            /// <summary>
            /// Checks whether a create operations has been registered for the given <paramref name="model"/>.
            /// </summary>
            /// <param name="registry">The registry to use.</param>
            /// <param name="model">The <see cref="TDataModel"/> to check for.</param>
            /// <returns><c>true</c> if the <see cref="model"/> was registered before, <c>false</c> 
            /// otherwise.</returns>
            /// <exception cref="ArgumentNullException">Thrown when <paramref name="model"/> is 
            /// <c>null</c>.</exception>
            protected abstract bool Contains(PersistenceRegistry registry, TDataModel model);

            /// <summary>
            /// Registers a create operation for <paramref name="model"/> and the <paramref name="entity"/>
            /// that was constructed with the information.
            /// </summary>
            /// <param name="registry">The registry to use.</param>
            /// <param name="entity">The <see cref="MacroStabilityInwardsSoilProfile1DEntity"/> 
            /// to be registered.</param>
            /// <param name="model">The <see cref="MacroStabilityInwardsSoilProfile1D"/> to be 
            /// registered.</param>
            /// <exception cref="ArgumentNullException">Thrown any of the input parameters is 
            /// <c>null</c>.</exception>
            protected abstract void Register(PersistenceRegistry registry, TEntity entity, TDataModel model);
        }

        #region Contains methods

        [Test]
        public void Contains_WithoutPipingSurfaceLine_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate test = () => registry.Contains((PipingSurfaceLine) null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("model", paramName);
        }

        [Test]
        public void Contains_PipingSurfaceLineAdded_ReturnsTrue()
        {
            // Setup
            var registry = new PersistenceRegistry();
            var surfaceLine = new PipingSurfaceLine(string.Empty);
            registry.Register(new SurfaceLineEntity(), surfaceLine);

            // Call
            bool result = registry.Contains(surfaceLine);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void Contains_NoPipingSurfaceLineAdded_ReturnsFalse()
        {
            // Setup
            var registry = new PersistenceRegistry();
            var surfaceLine = new PipingSurfaceLine(string.Empty);

            // Call
            bool result = registry.Contains(surfaceLine);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Contains_OtherPipingSurfaceLineAdded_ReturnsFalse()
        {
            // Setup
            var registry = new PersistenceRegistry();
            var surfaceLine = new PipingSurfaceLine(string.Empty);
            registry.Register(new SurfaceLineEntity(), new PipingSurfaceLine(string.Empty));

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
        public void Contains_WithoutDuneLocation_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate test = () => registry.Contains((DuneLocation) null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("model", paramName);
        }

        [Test]
        public void Contains_DuneLocationAdded_ReturnsTrue()
        {
            // Setup
            var registry = new PersistenceRegistry();
            var duneLocation = new TestDuneLocation();
            registry.Register(new DuneLocationEntity(), duneLocation);

            // Call
            bool result = registry.Contains(duneLocation);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void Contains_NoDuneLocationAdded_ReturnsFalse()
        {
            // Setup
            var registry = new PersistenceRegistry();
            var duneLocation = new TestDuneLocation();

            // Call
            bool result = registry.Contains(duneLocation);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Contains_OtherDuneLocationAdded_ReturnsFalse()
        {
            // Setup
            var registry = new PersistenceRegistry();
            var duneLocation = new TestDuneLocation();
            registry.Register(new DuneLocationEntity(), new TestDuneLocation());

            // Call
            bool result = registry.Contains(duneLocation);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Contains_WithoutPipingStochasticSoilModel_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate test = () => registry.Contains((PipingStochasticSoilModel) null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("model", paramName);
        }

        [Test]
        public void Contains_PipingStochasticSoilModelAdded_ReturnsTrue()
        {
            // Setup
            var registry = new PersistenceRegistry();
            var soilModel = new PipingStochasticSoilModel("A");
            registry.Register(new StochasticSoilModelEntity(), soilModel);

            // Call
            bool result = registry.Contains(soilModel);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void Contains_NoPipingStochasticSoilModelAdded_ReturnsFalse()
        {
            // Setup
            var registry = new PersistenceRegistry();
            var soilModel = new PipingStochasticSoilModel("A");

            // Call
            bool result = registry.Contains(soilModel);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Contains_OtherPipingPipingStochasticSoilModelAdded_ReturnsFalse()
        {
            // Setup
            var registry = new PersistenceRegistry();
            var soilModel = new PipingStochasticSoilModel("A");
            registry.Register(new StochasticSoilModelEntity(), new PipingStochasticSoilModel("B"));

            // Call
            bool result = registry.Contains(soilModel);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Contains_WithoutPipingStochasticSoilProfile_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate test = () => registry.Contains((PipingStochasticSoilProfile) null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("model", paramName);
        }

        [Test]
        public void Contains_PipingStochasticSoilProfileAdded_ReturnsTrue()
        {
            // Setup
            var registry = new PersistenceRegistry();
            var stochasticSoilProfile = new PipingStochasticSoilProfile(0.4, PipingSoilProfileTestFactory.CreatePipingSoilProfile());
            registry.Register(new PipingStochasticSoilProfileEntity(), stochasticSoilProfile);

            // Call
            bool result = registry.Contains(stochasticSoilProfile);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void Contains_NoPipingStochasticSoilProfileAdded_ReturnsFalse()
        {
            // Setup
            var registry = new PersistenceRegistry();
            var stochasticSoilProfile = new PipingStochasticSoilProfile(0.4, PipingSoilProfileTestFactory.CreatePipingSoilProfile());

            // Call
            bool result = registry.Contains(stochasticSoilProfile);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Contains_OtherPipingStochasticSoilProfileAdded_ReturnsFalse()
        {
            // Setup
            var registry = new PersistenceRegistry();
            var stochasticSoilProfile = new PipingStochasticSoilProfile(0.4, PipingSoilProfileTestFactory.CreatePipingSoilProfile());
            registry.Register(new PipingStochasticSoilProfileEntity(), new PipingStochasticSoilProfile(0.7, PipingSoilProfileTestFactory.CreatePipingSoilProfile()));

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
            DikeProfile dikeProfile = new TestDikeProfile();
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
            DikeProfile dikeProfile = CreateDikeProfile();
            DikeProfile otherDikeProfile = CreateDikeProfile();

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
            DikeProfile dikeProfile = CreateDikeProfile();

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
        public void Get_WithoutPipingSurfaceLine_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate test = () => registry.Get((PipingSurfaceLine) null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("model", paramName);
        }

        [Test]
        public void Get_PipingSurfaceLineAdded_ReturnsEntity()
        {
            // Setup
            var registry = new PersistenceRegistry();
            var surfaceLine = new PipingSurfaceLine(string.Empty);
            var entity = new SurfaceLineEntity();
            registry.Register(entity, surfaceLine);

            // Call
            SurfaceLineEntity result = registry.Get(surfaceLine);

            // Assert
            Assert.AreSame(entity, result);
        }

        [Test]
        public void Get_NoPipingSurfaceLineAdded_ThrowsInvalidOperationException()
        {
            // Setup
            var registry = new PersistenceRegistry();
            var surfaceLine = new PipingSurfaceLine(string.Empty);

            // Call
            TestDelegate test = () => registry.Get(surfaceLine);

            // Assert
            Assert.Throws<InvalidOperationException>(test);
        }

        [Test]
        public void Get_OtherPipingSurfaceLineAdded_ThrowsInvalidOperationException()
        {
            // Setup
            var registry = new PersistenceRegistry();
            var surfaceLine = new PipingSurfaceLine(string.Empty);
            registry.Register(new SurfaceLineEntity(), new PipingSurfaceLine(string.Empty));

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
            var result = registry.Get<HydraulicLocationEntity>(hydraulicBoundaryLocation);

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
        public void Get_WithoutDuneLocation_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate test = () => registry.Get<DuneLocation>(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("model", paramName);
        }

        [Test]
        public void Get_DuneLocationAdded_ReturnsEntity()
        {
            // Setup
            var registry = new PersistenceRegistry();
            var duneLocation = new TestDuneLocation();
            var entity = new DuneLocationEntity();
            registry.Register(entity, duneLocation);

            // Call
            DuneLocationEntity result = registry.Get(duneLocation);

            // Assert
            Assert.AreSame(entity, result);
        }

        [Test]
        public void Get_NoDuneLocationAdded_ThrowsInvalidOperationException()
        {
            // Setup
            var registry = new PersistenceRegistry();
            var duneLocation = new TestDuneLocation();

            // Call
            TestDelegate test = () => registry.Get(duneLocation);

            // Assert
            Assert.Throws<InvalidOperationException>(test);
        }

        [Test]
        public void Get_OtherDuneLocationAdded_ThrowsInvalidOperationException()
        {
            // Setup
            var registry = new PersistenceRegistry();
            var duneLocation = new TestDuneLocation();
            registry.Register(new DuneLocationEntity(), new TestDuneLocation());

            // Call
            TestDelegate test = () => registry.Get(duneLocation);

            // Assert
            Assert.Throws<InvalidOperationException>(test);
        }

        [Test]
        public void Get_WithoutPipingStochasticSoilModel_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate test = () => registry.Get((PipingStochasticSoilModel) null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("model", paramName);
        }

        [Test]
        public void Get_PipingStochasticSoilModelAdded_ReturnsEntity()
        {
            // Setup
            var registry = new PersistenceRegistry();
            var soilModel = new PipingStochasticSoilModel("6");
            var entity = new StochasticSoilModelEntity();
            registry.Register(entity, soilModel);

            // Call
            StochasticSoilModelEntity result = registry.Get(soilModel);

            // Assert
            Assert.AreSame(entity, result);
        }

        [Test]
        public void Get_NoPipingStochasticSoilModelAdded_ThrowsInvalidOperationException()
        {
            // Setup
            var registry = new PersistenceRegistry();
            var soilModel = new PipingStochasticSoilModel("6");

            // Call
            TestDelegate test = () => registry.Get(soilModel);

            // Assert
            Assert.Throws<InvalidOperationException>(test);
        }

        [Test]
        public void Get_OtherPipingStochasticSoilModelAdded_ThrowsInvalidOperationException()
        {
            // Setup
            var registry = new PersistenceRegistry();
            var soilModel = new PipingStochasticSoilModel("6");
            registry.Register(new StochasticSoilModelEntity(), new PipingStochasticSoilModel("2"));

            // Call
            TestDelegate test = () => registry.Get(soilModel);

            // Assert
            Assert.Throws<InvalidOperationException>(test);
        }

        [Test]
        public void Get_WithoutPipingStochasticSoilProfileEntity_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate test = () => registry.Get((PipingStochasticSoilProfile) null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("model", paramName);
        }

        [Test]
        public void Get_PipingStochasticSoilProfileAdded_ReturnsEntity()
        {
            // Setup
            var registry = new PersistenceRegistry();
            var stochasticSoilProfile = new PipingStochasticSoilProfile(0.2, PipingSoilProfileTestFactory.CreatePipingSoilProfile());
            var entity = new PipingStochasticSoilProfileEntity();
            registry.Register(entity, stochasticSoilProfile);

            // Call
            PipingStochasticSoilProfileEntity result = registry.Get(stochasticSoilProfile);

            // Assert
            Assert.AreSame(entity, result);
        }

        [Test]
        public void Get_NoPipingStochasticSoilProfileAdded_ThrowsInvalidOperationException()
        {
            // Setup
            var registry = new PersistenceRegistry();
            var stochasticSoilProfile = new PipingStochasticSoilProfile(0.2, PipingSoilProfileTestFactory.CreatePipingSoilProfile());

            // Call
            TestDelegate test = () => registry.Get(stochasticSoilProfile);

            // Assert
            Assert.Throws<InvalidOperationException>(test);
        }

        [Test]
        public void Get_OtherPipingStochasticSoilProfileAdded_ThrowsInvalidOperationException()
        {
            // Setup
            var registry = new PersistenceRegistry();
            var stochasticSoilProfile = new PipingStochasticSoilProfile(0.2, PipingSoilProfileTestFactory.CreatePipingSoilProfile());
            registry.Register(new PipingStochasticSoilProfileEntity(), new PipingStochasticSoilProfile(0.4, PipingSoilProfileTestFactory.CreatePipingSoilProfile()));

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
            DikeProfile dikeProfile = CreateDikeProfile();
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
            DikeProfile dikeProfile = CreateDikeProfile();
            DikeProfile registeredDikeProfile = CreateDikeProfile();
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
            DikeProfile dikeProfile = CreateDikeProfile();
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
        public void Register_WithNullDuneLocationEntity_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate test = () => registry.Register(null, new TestDuneLocation());

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("entity", paramName);
        }

        [Test]
        public void Register_WithNullDuneLocation_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate test = () => registry.Register(new DuneLocationEntity(), null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("model", paramName);
        }

        [Test]
        public void Register_WithNullPipingStochasticSoilModelEntity_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate test = () => registry.Register(null, new PipingStochasticSoilModel("some name"));

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("entity", paramName);
        }

        [Test]
        public void Register_WithNullPipingStochasticSoilModel_ThrowsArgumentNullException()
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
        public void Register_WithNullPipingStochasticSoilProfileEntity_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate test = () => registry.Register(null, new PipingStochasticSoilProfile(1, PipingSoilProfileTestFactory.CreatePipingSoilProfile()));

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("entity", paramName);
        }

        [Test]
        public void Register_WithNullPipingStochasticSoilProfile_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate test = () => registry.Register(new PipingStochasticSoilProfileEntity(), null);

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