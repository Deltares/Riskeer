﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.ClosingStructures.Data;
using Riskeer.ClosingStructures.Data.TestUtil;
using Riskeer.Common.Data.DikeProfiles;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.TestUtil;
using Riskeer.DuneErosion.Data;
using Riskeer.DuneErosion.Data.TestUtil;
using Riskeer.HeightStructures.Data;
using Riskeer.HeightStructures.Data.TestUtil;
using Riskeer.MacroStabilityInwards.Data.SoilProfile;
using Riskeer.MacroStabilityInwards.Data.TestUtil;
using Riskeer.MacroStabilityInwards.Data.TestUtil.SoilProfile;
using Riskeer.MacroStabilityInwards.Primitives;
using Riskeer.Piping.Data.SoilProfile;
using Riskeer.Piping.Data.TestUtil;
using Riskeer.Piping.Primitives;
using Riskeer.Piping.Primitives.TestUtil;
using Riskeer.StabilityPointStructures.Data;
using Riskeer.StabilityPointStructures.Data.TestUtil;
using Riskeer.Storage.Core.Create;
using Riskeer.Storage.Core.DbContext;

namespace Riskeer.Storage.Core.Test.Create
{
    [TestFixture]
    public class PersistenceRegistryTest
    {
        /// <summary>
        /// Test class to test the <see cref="PersistenceRegistry"/> for the combination of 
        /// <see cref="TDataModel"/> and <see cref="TEntity"/>.
        /// </summary>
        /// <typeparam name="TDataModel">The data model.</typeparam>
        /// <typeparam name="TEntity">The database entity.</typeparam>
        private abstract class RegistryTest<TDataModel, TEntity> where TDataModel : class
                                                                 where TEntity : class, new()
        {
            private readonly Action<PersistenceRegistry, TEntity, TDataModel> registerToRegistry;
            private readonly Func<PersistenceRegistry, TDataModel, bool> containsInRegistry;
            private readonly Func<PersistenceRegistry, TDataModel, TEntity> getFromRegistry;

            /// <summary>
            /// Creates a new instance of <see cref="RegistryTest{T,T}"/>.
            /// </summary>
            /// <param name="registerToRegistry">The action to perform to register the data model
            /// to the registry.</param>
            /// <param name="containsInRegistry">The action to perform to check whether the data 
            /// model is registered in the registry.</param>
            /// <param name="getFromRegistry">The action to perform to get the data model from
            ///  the registry.</param>
            /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
            /// <example>
            /// <code>public DerivedRegistryTest() : base(
            /// (r, e, m) => r.Register(e, m),
            /// (r, m) => r.Contains(m),
            /// (r, m) => r.Get(m)) {}
            /// </code>
            /// </example>
            protected RegistryTest(Action<PersistenceRegistry, TEntity, TDataModel> registerToRegistry,
                                   Func<PersistenceRegistry, TDataModel, bool> containsInRegistry,
                                   Func<PersistenceRegistry, TDataModel, TEntity> getFromRegistry)
            {
                if (registerToRegistry == null)
                {
                    throw new ArgumentNullException(nameof(registerToRegistry));
                }

                if (containsInRegistry == null)
                {
                    throw new ArgumentNullException(nameof(containsInRegistry));
                }

                if (getFromRegistry == null)
                {
                    throw new ArgumentNullException(nameof(getFromRegistry));
                }

                this.registerToRegistry = registerToRegistry;
                this.containsInRegistry = containsInRegistry;
                this.getFromRegistry = getFromRegistry;
            }

            [Test]
            public void Register_WithNullEntity_ThrowsArgumentNullException()
            {
                // Setup
                var registry = new PersistenceRegistry();

                // Call
                TestDelegate test = () => registerToRegistry(registry, null, CreateDataModel());

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
                TestDelegate test = () => registerToRegistry(registry, new TEntity(), null);

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
                TestDelegate test = () => containsInRegistry(registry, null);

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
                registerToRegistry(registry, new TEntity(), dataModel);

                // Call
                bool result = containsInRegistry(registry, dataModel);

                // Assert
                Assert.IsTrue(result);
            }

            [Test]
            public void Contains_OtherDataModelAdded_ReturnsFalse()
            {
                // Setup
                var registry = new PersistenceRegistry();
                registerToRegistry(registry, new TEntity(), CreateDataModel());

                // Call
                bool result = containsInRegistry(registry, CreateDataModel());

                // Assert
                Assert.IsFalse(result);
            }

            [Test]
            public void Contains_PersistenceRegistryEmpty_ReturnsFalse()
            {
                // Setup
                var registry = new PersistenceRegistry();
                TDataModel dataModel = CreateDataModel();

                // Call
                bool result = containsInRegistry(registry, dataModel);

                // Assert
                Assert.IsFalse(result);
            }

            [Test]
            public void Get_DataModelNull_ThrowsArgumentNullException()
            {
                // Setup
                var registry = new PersistenceRegistry();

                // Call
                TestDelegate test = () => getFromRegistry(registry, null);

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

                registerToRegistry(registry, entity, dataModel);

                // Call
                TEntity result = getFromRegistry(registry, dataModel);

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
                TestDelegate test = () => getFromRegistry(registry, dataModel);

                // Assert
                Assert.Throws<InvalidOperationException>(test);
            }

            [Test]
            public void Get_OtherDataModelAdded_ThrowsInvalidOperationException()
            {
                // Setup
                var registry = new PersistenceRegistry();
                registerToRegistry(registry, new TEntity(), CreateDataModel());

                // Call
                TestDelegate test = () => getFromRegistry(registry, CreateDataModel());

                // Assert
                Assert.Throws<InvalidOperationException>(test);
            }

            /// <summary>
            /// Creates a new instance of <see cref="TDataModel"/>.
            /// </summary>
            /// <returns>An instance of <see cref="TDataModel"/>.</returns>
            protected abstract TDataModel CreateDataModel();
        }

        [TestFixture]
        private class HydraulicBoundaryLocationRegistryTest : RegistryTest<HydraulicBoundaryLocation,
            HydraulicLocationEntity>
        {
            public HydraulicBoundaryLocationRegistryTest() : base(
                (r, e, m) => r.Register(e, m),
                (r, m) => r.Contains(m),
                (r, m) => r.Get(m)) {}

            protected override HydraulicBoundaryLocation CreateDataModel()
            {
                return new TestHydraulicBoundaryLocation(nameof(HydraulicBoundaryLocation));
            }
        }

        [TestFixture]
        private class HydraulicBoundaryLocationCalculationsForTargetProbabilityRegistryTest : RegistryTest<
            HydraulicBoundaryLocationCalculationsForTargetProbability, HydraulicLocationCalculationForTargetProbabilityCollectionEntity>
        {
            public HydraulicBoundaryLocationCalculationsForTargetProbabilityRegistryTest() : base(
                (r, e, m) => r.Register(e, m), 
                (r, m) => r.Contains(m),
                (r, m) => r.Get(m)) {}
            protected override HydraulicBoundaryLocationCalculationsForTargetProbability CreateDataModel()
            {
                return new HydraulicBoundaryLocationCalculationsForTargetProbability(0.05);
            }
        }
        
        [TestFixture]
        private class FailureMechanismSectionRegistryTest : RegistryTest<FailureMechanismSection,
            FailureMechanismSectionEntity>
        {
            public FailureMechanismSectionRegistryTest() : base(
                (r, e, m) => r.Register(e, m),
                (r, m) => r.Contains(m),
                (r, m) => r.Get(m)) {}

            protected override FailureMechanismSection CreateDataModel()
            {
                return FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            }
        }

        [TestFixture]
        private class DikeProfileRegistryTest : RegistryTest<DikeProfile,
            DikeProfileEntity>
        {
            public DikeProfileRegistryTest() : base(
                (r, e, m) => r.Register(e, m),
                (r, m) => r.Contains(m),
                (r, m) => r.Get(m)) {}

            protected override DikeProfile CreateDataModel()
            {
                return DikeProfileTestFactory.CreateDikeProfile();
            }
        }

        [TestFixture]
        private class ForeshoreProfileRegistryTest : RegistryTest<ForeshoreProfile,
            ForeshoreProfileEntity>
        {
            public ForeshoreProfileRegistryTest() : base(
                (r, e, m) => r.Register(e, m),
                (r, m) => r.Contains(m),
                (r, m) => r.Get(m)) {}

            protected override ForeshoreProfile CreateDataModel()
            {
                return new TestForeshoreProfile();
            }
        }

        #region DuneErosion

        [TestFixture]
        private class DuneLocationRegistryTest : RegistryTest<DuneLocation,
            DuneLocationEntity>
        {
            public DuneLocationRegistryTest() : base(
                (r, e, m) => r.Register(e, m),
                (r, m) => r.Contains(m),
                (r, m) => r.Get(m)) {}

            protected override DuneLocation CreateDataModel()
            {
                return new TestDuneLocation(nameof(DuneLocation));
            }
        }

        #endregion

        #region HeightStructures

        [TestFixture]
        private class HeightStructureRegistryTest : RegistryTest<
            HeightStructure, HeightStructureEntity>
        {
            public HeightStructureRegistryTest() : base(
                (r, e, m) => r.Register(e, m),
                (r, m) => r.Contains(m),
                (r, m) => r.Get(m)) {}

            protected override HeightStructure CreateDataModel()
            {
                return new TestHeightStructure();
            }
        }

        #endregion

        #region ClosingStructures

        [TestFixture]
        private class ClosingStructureRegistryTest : RegistryTest<
            ClosingStructure, ClosingStructureEntity>
        {
            public ClosingStructureRegistryTest() : base(
                (r, e, m) => r.Register(e, m),
                (r, m) => r.Contains(m),
                (r, m) => r.Get(m)) {}

            protected override ClosingStructure CreateDataModel()
            {
                return new TestClosingStructure();
            }
        }

        #endregion

        #region StabilityPointStructures

        [TestFixture]
        private class StabilityPointStructureStructureRegistryTest : RegistryTest<
            StabilityPointStructure, StabilityPointStructureEntity>
        {
            public StabilityPointStructureStructureRegistryTest() : base(
                (r, e, m) => r.Register(e, m),
                (r, m) => r.Contains(m),
                (r, m) => r.Get(m)) {}

            protected override StabilityPointStructure CreateDataModel()
            {
                return new TestStabilityPointStructure();
            }
        }

        #endregion

        #region Piping

        [TestFixture]
        private class PipingStochasticSoilModelRegistryTest : RegistryTest<PipingStochasticSoilModel,
            StochasticSoilModelEntity>
        {
            public PipingStochasticSoilModelRegistryTest() : base(
                (r, e, m) => r.Register(e, m),
                (r, m) => r.Contains(m),
                (r, m) => r.Get(m)) {}

            protected override PipingStochasticSoilModel CreateDataModel()
            {
                return PipingStochasticSoilModelTestFactory.CreatePipingStochasticSoilModel(nameof(PipingStochasticSoilModel));
            }
        }

        [TestFixture]
        private class PipingStochasticSoilProfileRegistryTest : RegistryTest<PipingStochasticSoilProfile,
            PipingStochasticSoilProfileEntity>
        {
            public PipingStochasticSoilProfileRegistryTest() : base(
                (r, e, m) => r.Register(e, m),
                (r, m) => r.Contains(m),
                (r, m) => r.Get(m)) {}

            protected override PipingStochasticSoilProfile CreateDataModel()
            {
                return new PipingStochasticSoilProfile(0.2, PipingSoilProfileTestFactory.CreatePipingSoilProfile());
            }
        }

        [TestFixture]
        private class PipingSoilProfileRegistryTest : RegistryTest<PipingSoilProfile,
            PipingSoilProfileEntity>
        {
            public PipingSoilProfileRegistryTest() : base(
                (r, e, m) => r.Register(e, m),
                (r, m) => r.Contains(m),
                (r, m) => r.Get(m)) {}

            protected override PipingSoilProfile CreateDataModel()
            {
                return PipingSoilProfileTestFactory.CreatePipingSoilProfile();
            }
        }

        [TestFixture]
        private class PipingSurfaceLineRegistryTest : RegistryTest<PipingSurfaceLine,
            SurfaceLineEntity>
        {
            public PipingSurfaceLineRegistryTest() : base(
                (r, e, m) => r.Register(e, m),
                (r, m) => r.Contains(m),
                (r, m) => r.Get(m)) {}

            protected override PipingSurfaceLine CreateDataModel()
            {
                return new PipingSurfaceLine(nameof(PipingSurfaceLine));
            }
        }

        #endregion

        #region MacroStabilityInwards

        [TestFixture]
        private class MacroStabilityInwardsStochasticSoilModelRegistryTest : RegistryTest<MacroStabilityInwardsStochasticSoilModel,
            StochasticSoilModelEntity>
        {
            public MacroStabilityInwardsStochasticSoilModelRegistryTest() : base(
                (r, e, m) => r.Register(e, m),
                (r, m) => r.Contains(m),
                (r, m) => r.Get(m)) {}

            protected override MacroStabilityInwardsStochasticSoilModel CreateDataModel()
            {
                return MacroStabilityInwardsStochasticSoilModelTestFactory.CreateValidStochasticSoilModel(nameof(MacroStabilityInwardsStochasticSoilModel));
            }
        }

        [TestFixture]
        private class MacroStabilityInwardsStochasticSoilProfileRegistryTest : RegistryTest<MacroStabilityInwardsStochasticSoilProfile,
            MacroStabilityInwardsStochasticSoilProfileEntity>
        {
            private MockRepository mockRepository;

            [SetUp]
            public void Setup()
            {
                mockRepository = new MockRepository();
            }

            [TearDown]
            public void TearDown()
            {
                mockRepository.VerifyAll();
            }

            public MacroStabilityInwardsStochasticSoilProfileRegistryTest() : base(
                (r, e, m) => r.Register(e, m),
                (r, m) => r.Contains(m),
                (r, m) => r.Get(m)) {}

            protected override MacroStabilityInwardsStochasticSoilProfile CreateDataModel()
            {
                var soilProfile = mockRepository.Stub<IMacroStabilityInwardsSoilProfile<IMacroStabilityInwardsSoilLayer>>();
                mockRepository.ReplayAll();

                return new MacroStabilityInwardsStochasticSoilProfile(0, soilProfile);
            }
        }

        [TestFixture]
        private class MacroStabilityInwardsSoilProfileOneDRegistryTest : RegistryTest<MacroStabilityInwardsSoilProfile1D,
            MacroStabilityInwardsSoilProfileOneDEntity>
        {
            public MacroStabilityInwardsSoilProfileOneDRegistryTest() : base(
                (r, e, m) => r.Register(e, m),
                (r, m) => r.Contains(m),
                (r, m) => r.Get(m)) {}

            protected override MacroStabilityInwardsSoilProfile1D CreateDataModel()
            {
                return MacroStabilityInwardsSoilProfile1DTestFactory.CreateMacroStabilityInwardsSoilProfile1D();
            }
        }

        [TestFixture]
        private class MacroStabilityInwardsSoilProfileTwoDRegistryTest : RegistryTest<MacroStabilityInwardsSoilProfile2D,
            MacroStabilityInwardsSoilProfileTwoDEntity>
        {
            public MacroStabilityInwardsSoilProfileTwoDRegistryTest() : base(
                (r, e, m) => r.Register(e, m),
                (r, m) => r.Contains(m),
                (r, m) => r.Get(m)) {}

            protected override MacroStabilityInwardsSoilProfile2D CreateDataModel()
            {
                return new MacroStabilityInwardsSoilProfile2D("", new[]
                {
                    new MacroStabilityInwardsSoilLayer2D(new Ring(new[]
                    {
                        new Point2D(0, 0),
                        new Point2D(1, 1)
                    }))
                }, Enumerable.Empty<MacroStabilityInwardsPreconsolidationStress>());
            }
        }

        [TestFixture]
        private class MacroStabilityInwardsSurfaceLineRegistryTest : RegistryTest<MacroStabilityInwardsSurfaceLine,
            SurfaceLineEntity>
        {
            public MacroStabilityInwardsSurfaceLineRegistryTest() : base(
                (r, e, m) => r.Register(e, m),
                (r, m) => r.Contains(m),
                (r, m) => r.Get(m)) {}

            protected override MacroStabilityInwardsSurfaceLine CreateDataModel()
            {
                return new MacroStabilityInwardsSurfaceLine(nameof(MacroStabilityInwardsSurfaceLine));
            }
        }

        #endregion
    }
}