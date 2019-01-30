// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Riskeer.ClosingStructures.Data;
using Riskeer.ClosingStructures.Data.TestUtil;
using Riskeer.Common.Data.DikeProfiles;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.Structures;
using Riskeer.Common.Data.TestUtil;
using Riskeer.DuneErosion.Data;
using Riskeer.DuneErosion.Data.TestUtil;
using Riskeer.GrassCoverErosionInwards.Data;
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
using Riskeer.Storage.Core.DbContext;
using Riskeer.Storage.Core.Read;

namespace Riskeer.Storage.Core.Test.Read
{
    [TestFixture]
    public class ReadConversionCollectorTest
    {
        /// <summary>
        /// Test class to test the <see cref="ReadConversionCollector"/> for the combination of 
        /// <see cref="TDataModel"/> and <see cref="TEntity"/>.
        /// </summary>
        /// <typeparam name="TDataModel">The data model.</typeparam>
        /// <typeparam name="TEntity">The database entity.</typeparam>
        private abstract class CollectorTest<TDataModel, TEntity> where TDataModel : class
                                                                  where TEntity : class, new()
        {
            private readonly Action<ReadConversionCollector, TEntity, TDataModel> registerToCollector;
            private readonly Func<ReadConversionCollector, TEntity, bool> containsInCollector;
            private readonly Func<ReadConversionCollector, TEntity, TDataModel> getFromCollector;

            /// <summary>
            /// Creates a new instance of <see cref="CollectorTest{T,T}"/>.
            /// </summary>
            /// <param name="registerToCollector">The action to perform to register the entity
            /// to the collector.</param>
            /// <param name="containsInCollector">The action to perform to check whether the entity 
            /// is registered in the collector.</param>
            /// <param name="getFromCollector">The action to perform to get the entity from
            ///  the collector.</param>
            /// <exception cref="ArgumentNullException">Thrown when any input parameter is <c>null</c>.</exception>
            /// <example>
            /// <code>public CollectorTest() : base(
            /// (c, e, m) => c.Read(e, m),
            /// (c, e) => c.Contains(e),
            /// (c, e) => c.Get(e)) {}
            /// </code>
            /// </example>
            protected CollectorTest(Action<ReadConversionCollector, TEntity, TDataModel> registerToCollector,
                                    Func<ReadConversionCollector, TEntity, bool> containsInCollector,
                                    Func<ReadConversionCollector, TEntity, TDataModel> getFromCollector)
            {
                if (registerToCollector == null)
                {
                    throw new ArgumentNullException(nameof(registerToCollector));
                }

                if (containsInCollector == null)
                {
                    throw new ArgumentNullException(nameof(containsInCollector));
                }

                if (getFromCollector == null)
                {
                    throw new ArgumentNullException(nameof(getFromCollector));
                }

                this.registerToCollector = registerToCollector;
                this.containsInCollector = containsInCollector;
                this.getFromCollector = getFromCollector;
            }

            [Test]
            public void Contains_EntityNull_ThrowsArgumentNullException()
            {
                // Setup
                var collector = new ReadConversionCollector();

                // Call
                TestDelegate test = () => containsInCollector(collector, null);

                // Assert
                string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
                Assert.AreEqual("entity", paramName);
            }

            [Test]
            public void Contains_DataModelAdded_ReturnsTrue()
            {
                // Setup
                var collector = new ReadConversionCollector();
                var entity = new TEntity();
                registerToCollector(collector, entity, CreateDataModel());

                // Call
                bool result = containsInCollector(collector, entity);

                // Assert
                Assert.IsTrue(result);
            }

            [Test]
            public void Contains_EmptyReadConversionCollector_ReturnsFalse()
            {
                // Setup
                var collector = new ReadConversionCollector();
                var entity = new TEntity();

                // Call
                bool result = containsInCollector(collector, entity);

                // Assert
                Assert.IsFalse(result);
            }

            [Test]
            public void Contains_OtherEntityAdded_ReturnsFalse()
            {
                // Setup
                var collector = new ReadConversionCollector();
                registerToCollector(collector, new TEntity(), CreateDataModel());

                // Call
                bool result = containsInCollector(collector, new TEntity());

                // Assert
                Assert.IsFalse(result);
            }

            [Test]
            public void Get_EntityNull_ThrowsArgumentNullException()
            {
                // Setup
                var collector = new ReadConversionCollector();

                // Call
                TestDelegate test = () => getFromCollector(collector, null);

                // Assert
                string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
                Assert.AreEqual("entity", paramName);
            }

            [Test]
            public void Get_DataModelAdded_ReturnsEntity()
            {
                // Setup
                var collector = new ReadConversionCollector();
                TDataModel dataModel = CreateDataModel();
                var entity = new TEntity();
                registerToCollector(collector, entity, dataModel);

                // Call
                TDataModel result = getFromCollector(collector, entity);

                // Assert
                Assert.AreSame(dataModel, result);
            }

            [Test]
            public void Get_NoDataModelAdded_ThrowsInvalidOperationException()
            {
                // Setup
                var collector = new ReadConversionCollector();
                var entity = new TEntity();

                // Call
                TestDelegate test = () => getFromCollector(collector, entity);

                // Assert
                Assert.Throws<InvalidOperationException>(test);
            }

            [Test]
            public void Get_OtherDataModelAdded_ThrowsInvalidOperationException()
            {
                // Setup
                var collector = new ReadConversionCollector();
                registerToCollector(collector, new TEntity(), CreateDataModel());

                // Call
                TestDelegate test = () => getFromCollector(collector, new TEntity());

                // Assert
                Assert.Throws<InvalidOperationException>(test);
            }

            [Test]
            public void Read_EntityNull_ThrowsArgumentNullException()
            {
                // Setup
                var collector = new ReadConversionCollector();

                // Call
                TestDelegate test = () => registerToCollector(collector, null, CreateDataModel());

                // Assert
                string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
                Assert.AreEqual("entity", paramName);
            }

            [Test]
            public void Read_DataModelNull_ThrowsArgumentNullException()
            {
                // Setup
                var collector = new ReadConversionCollector();

                // Call
                TestDelegate test = () => registerToCollector(collector, new TEntity(), null);

                // Assert
                string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
                Assert.AreEqual("model", paramName);
            }

            /// <summary>
            /// Creates a new instance of <see cref="TDataModel"/>.
            /// </summary>
            /// <returns>An instance of <see cref="TDataModel"/>.</returns>
            protected abstract TDataModel CreateDataModel();
        }

        [TestFixture]
        private class HydraulicBoundaryLocationCollectorTest : CollectorTest<HydraulicBoundaryLocation,
            HydraulicLocationEntity>
        {
            public HydraulicBoundaryLocationCollectorTest() : base(
                (c, e, m) => c.Read(e, m),
                (c, e) => c.Contains(e),
                (c, e) => c.Get(e)) {}

            protected override HydraulicBoundaryLocation CreateDataModel()
            {
                return new TestHydraulicBoundaryLocation();
            }
        }

        #region DuneErosion

        [TestFixture]
        private class DuneLocationCollectorTest : CollectorTest<DuneLocation,
            DuneLocationEntity>
        {
            public DuneLocationCollectorTest() : base(
                (c, e, m) => c.Read(e, m),
                (c, e) => c.Contains(e),
                (c, e) => c.Get(e)) {}

            protected override DuneLocation CreateDataModel()
            {
                return new TestDuneLocation();
            }
        }

        #endregion

        [TestFixture]
        private class FailureMechanismSectionCollectorTest : CollectorTest<FailureMechanismSection,
            FailureMechanismSectionEntity>
        {
            public FailureMechanismSectionCollectorTest() : base(
                (c, e, m) => c.Read(e, m),
                (c, e) => c.Contains(e),
                (c, e) => c.Get(e)) {}

            protected override FailureMechanismSection CreateDataModel()
            {
                return FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            }
        }

        [TestFixture]
        private class DikeProfileCollectorTest : CollectorTest<DikeProfile,
            DikeProfileEntity>
        {
            public DikeProfileCollectorTest() : base(
                (c, e, m) => c.Read(e, m),
                (c, e) => c.Contains(e),
                (c, e) => c.Get(e)) {}

            protected override DikeProfile CreateDataModel()
            {
                return DikeProfileTestFactory.CreateDikeProfile();
            }
        }

        [TestFixture]
        private class ForeshoreProfileCollectorTest : CollectorTest<ForeshoreProfile,
            ForeshoreProfileEntity>
        {
            public ForeshoreProfileCollectorTest() : base(
                (c, e, m) => c.Read(e, m),
                (c, e) => c.Contains(e),
                (c, e) => c.Get(e)) {}

            protected override ForeshoreProfile CreateDataModel()
            {
                return new TestForeshoreProfile();
            }
        }

        #region GrassCoverErosionInwards

        [TestFixture]
        private class GrassCoverErosionInwardsCalculationCollectorTest : CollectorTest<GrassCoverErosionInwardsCalculation,
            GrassCoverErosionInwardsCalculationEntity>
        {
            public GrassCoverErosionInwardsCalculationCollectorTest() : base(
                (c, e, m) => c.Read(e, m),
                (c, e) => c.Contains(e),
                (c, e) => c.Get(e)) {}

            protected override GrassCoverErosionInwardsCalculation CreateDataModel()
            {
                return new GrassCoverErosionInwardsCalculation();
            }
        }

        #endregion

        #region HeightStructure

        [TestFixture]
        private class HeightStructureCollectorTest : CollectorTest<HeightStructure, HeightStructureEntity>
        {
            public HeightStructureCollectorTest() : base(
                (c, e, m) => c.Read(e, m),
                (c, e) => c.Contains(e),
                (c, e) => c.Get(e)) {}

            protected override HeightStructure CreateDataModel()
            {
                return new TestHeightStructure();
            }
        }

        [TestFixture]
        private class HeightStructureCalculationCollectorTest : CollectorTest<StructuresCalculation<HeightStructuresInput>,
            HeightStructuresCalculationEntity>
        {
            public HeightStructureCalculationCollectorTest() : base(
                (c, e, m) => c.Read(e, m),
                (c, e) => c.Contains(e),
                (c, e) => c.Get(e)) {}

            protected override StructuresCalculation<HeightStructuresInput> CreateDataModel()
            {
                return new StructuresCalculation<HeightStructuresInput>();
            }
        }

        #endregion

        #region ClosingStructure

        [TestFixture]
        private class ClosingStructureCollectorTest : CollectorTest<ClosingStructure, ClosingStructureEntity>
        {
            public ClosingStructureCollectorTest() : base(
                (c, e, m) => c.Read(e, m),
                (c, e) => c.Contains(e),
                (c, e) => c.Get(e)) {}

            protected override ClosingStructure CreateDataModel()
            {
                return new TestClosingStructure();
            }
        }

        [TestFixture]
        private class ClosingStructureCalculationCollectorTest : CollectorTest<StructuresCalculation<ClosingStructuresInput>,
            ClosingStructuresCalculationEntity>
        {
            public ClosingStructureCalculationCollectorTest() : base(
                (c, e, m) => c.Read(e, m),
                (c, e) => c.Contains(e),
                (c, e) => c.Get(e)) {}

            protected override StructuresCalculation<ClosingStructuresInput> CreateDataModel()
            {
                return new StructuresCalculation<ClosingStructuresInput>();
            }
        }

        #endregion

        #region StabilityPointStructure

        [TestFixture]
        private class StabilityPointStructureCollectorTest : CollectorTest<StabilityPointStructure,
            StabilityPointStructureEntity>
        {
            public StabilityPointStructureCollectorTest() : base(
                (c, e, m) => c.Read(e, m),
                (c, e) => c.Contains(e),
                (c, e) => c.Get(e)) {}

            protected override StabilityPointStructure CreateDataModel()
            {
                return new TestStabilityPointStructure();
            }
        }

        [TestFixture]
        private class StabilityPointStructureCalculationCollectorTest : CollectorTest<StructuresCalculation<StabilityPointStructuresInput>,
            StabilityPointStructuresCalculationEntity>
        {
            public StabilityPointStructureCalculationCollectorTest() : base(
                (c, e, m) => c.Read(e, m),
                (c, e) => c.Contains(e),
                (c, e) => c.Get(e)) {}

            protected override StructuresCalculation<StabilityPointStructuresInput> CreateDataModel()
            {
                return new StructuresCalculation<StabilityPointStructuresInput>();
            }
        }

        #endregion

        #region Piping

        [TestFixture]
        private class PipingSoilProfileCollectorTest : CollectorTest<PipingSoilProfile,
            PipingSoilProfileEntity>
        {
            public PipingSoilProfileCollectorTest() : base(
                (c, e, m) => c.Read(e, m),
                (c, e) => c.Contains(e),
                (c, e) => c.Get(e)) {}

            protected override PipingSoilProfile CreateDataModel()
            {
                return PipingSoilProfileTestFactory.CreatePipingSoilProfile();
            }
        }

        [TestFixture]
        private class PipingStochasticSoilProfileCollectorTest : CollectorTest<PipingStochasticSoilProfile,
            PipingStochasticSoilProfileEntity>
        {
            public PipingStochasticSoilProfileCollectorTest() : base(
                (c, e, m) => c.Read(e, m),
                (c, e) => c.Contains(e),
                (c, e) => c.Get(e)) {}

            protected override PipingStochasticSoilProfile CreateDataModel()
            {
                return new PipingStochasticSoilProfile(1, PipingSoilProfileTestFactory.CreatePipingSoilProfile());
            }
        }

        [TestFixture]
        private class PipingStochasticSoilModelCollectorTest : CollectorTest<PipingStochasticSoilModel,
            StochasticSoilModelEntity>
        {
            public PipingStochasticSoilModelCollectorTest() : base(
                (c, e, m) => c.Read(e, m),
                (c, e) => c.ContainsPipingStochasticSoilModel(e),
                (c, e) => c.GetPipingStochasticSoilModel(e)) {}

            protected override PipingStochasticSoilModel CreateDataModel()
            {
                return PipingStochasticSoilModelTestFactory.CreatePipingStochasticSoilModel(nameof(PipingStochasticSoilModel));
            }
        }

        [TestFixture]
        private class PipingSurfaceLineCollectorTest : CollectorTest<PipingSurfaceLine,
            SurfaceLineEntity>
        {
            public PipingSurfaceLineCollectorTest() : base(
                (c, e, m) => c.Read(e, m),
                (c, e) => c.ContainsPipingSurfaceLine(e),
                (c, e) => c.GetPipingSurfaceLine(e)) {}

            protected override PipingSurfaceLine CreateDataModel()
            {
                return new PipingSurfaceLine(nameof(PipingSurfaceLine));
            }
        }

        #endregion

        #region MacroStabilityInwards

        [TestFixture]
        private class MacroStabilityInwardsSoilProfileOneDCollectorTest : CollectorTest<MacroStabilityInwardsSoilProfile1D,
            MacroStabilityInwardsSoilProfileOneDEntity>
        {
            public MacroStabilityInwardsSoilProfileOneDCollectorTest() : base(
                (c, e, m) => c.Read(e, m),
                (c, e) => c.Contains(e),
                (c, e) => c.Get(e)) {}

            protected override MacroStabilityInwardsSoilProfile1D CreateDataModel()
            {
                return new MacroStabilityInwardsSoilProfile1D(nameof(MacroStabilityInwardsSoilProfile1D), 0.0, new[]
                {
                    new MacroStabilityInwardsSoilLayer1D(0.0)
                    {
                        Data =
                        {
                            IsAquifer = true
                        }
                    }
                });
            }
        }

        [TestFixture]
        private class MacroStabilityInwardsSoilProfileTwoDCollectorTest : CollectorTest<MacroStabilityInwardsSoilProfile2D,
            MacroStabilityInwardsSoilProfileTwoDEntity>
        {
            public MacroStabilityInwardsSoilProfileTwoDCollectorTest() : base(
                (c, e, m) => c.Read(e, m),
                (c, e) => c.Contains(e),
                (c, e) => c.Get(e)) {}

            protected override MacroStabilityInwardsSoilProfile2D CreateDataModel()
            {
                return new MacroStabilityInwardsSoilProfile2D(nameof(MacroStabilityInwardsSoilProfile1D),
                                                              CreateLayers2D(),
                                                              Enumerable.Empty<MacroStabilityInwardsPreconsolidationStress>());
            }

            private static IEnumerable<MacroStabilityInwardsSoilLayer2D> CreateLayers2D()
            {
                var outerRing = new Ring(new[]
                {
                    new Point2D(3, 2),
                    new Point2D(3, 5)
                });
                return new[]
                {
                    new MacroStabilityInwardsSoilLayer2D(outerRing)
                };
            }
        }

        [TestFixture]
        private class MacroStabilityInwardsStochasticSoilProfileCollectorTest : CollectorTest<MacroStabilityInwardsStochasticSoilProfile,
            MacroStabilityInwardsStochasticSoilProfileEntity>
        {
            public MacroStabilityInwardsStochasticSoilProfileCollectorTest() : base(
                (c, e, m) => c.Read(e, m),
                (c, e) => c.Contains(e),
                (c, e) => c.Get(e)) {}

            protected override MacroStabilityInwardsStochasticSoilProfile CreateDataModel()
            {
                return new MacroStabilityInwardsStochasticSoilProfile(1, MacroStabilityInwardsSoilProfile1DTestFactory.CreateMacroStabilityInwardsSoilProfile1D());
            }
        }

        [TestFixture]
        private class MacroStabilityInwardsStochasticSoilModelCollectorTest : CollectorTest<MacroStabilityInwardsStochasticSoilModel,
            StochasticSoilModelEntity>
        {
            public MacroStabilityInwardsStochasticSoilModelCollectorTest() : base(
                (c, e, m) => c.Read(e, m),
                (c, e) => c.ContainsMacroStabilityInwardsStochasticSoilModel(e),
                (c, e) => c.GetMacroStabilityInwardsStochasticSoilModel(e)) {}

            protected override MacroStabilityInwardsStochasticSoilModel CreateDataModel()
            {
                return MacroStabilityInwardsStochasticSoilModelTestFactory.CreateValidStochasticSoilModel(nameof(MacroStabilityInwardsStochasticSoilModel));
            }
        }

        [TestFixture]
        private class MacroStabilityInwardsSurfaceLineCollectorTest : CollectorTest<MacroStabilityInwardsSurfaceLine,
            SurfaceLineEntity>
        {
            public MacroStabilityInwardsSurfaceLineCollectorTest() : base(
                (c, e, m) => c.Read(e, m),
                (c, e) => c.ContainsMacroStabilityInwardsSurfaceLine(e),
                (c, e) => c.GetMacroStabilityInwardsSurfaceLine(e)) {}

            protected override MacroStabilityInwardsSurfaceLine CreateDataModel()
            {
                return new MacroStabilityInwardsSurfaceLine(nameof(MacroStabilityInwardsSurfaceLine));
            }
        }

        #endregion
    }
}