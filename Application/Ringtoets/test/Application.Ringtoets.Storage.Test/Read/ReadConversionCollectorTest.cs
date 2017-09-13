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
using System.Collections.Generic;
using System.Linq;
using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.Read;
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
using Ringtoets.MacroStabilityInwards.Data.SoilProfile;
using Ringtoets.MacroStabilityInwards.Primitives;
using Ringtoets.MacroStabilityInwards.Primitives.TestUtil;
using Ringtoets.Piping.Data.SoilProfile;
using Ringtoets.Piping.Primitives;
using Ringtoets.Piping.Primitives.TestUtil;
using Ringtoets.StabilityPointStructures.Data;
using Ringtoets.StabilityPointStructures.Data.TestUtil;

namespace Application.Ringtoets.Storage.Test.Read
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
            /// <example>public CollectorTest() : base(
            /// (c, e, m) => c.Read(e, m),
            /// (c, e) => c.Contains(e),
            /// (c, e) => c.Get(e)) {}</example>
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
            public void Contains_NoDataModelAdded_ReturnsFalse()
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
                (c, e) => c.Contains(e),
                (c, e) => c.Get(e))
            { }

            protected override PipingStochasticSoilModel CreateDataModel()
            {
                return new PipingStochasticSoilModel(nameof(PipingStochasticSoilModel));
            }
        }

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
                        Properties =
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
                    new MacroStabilityInwardsSoilLayer2D(outerRing, Enumerable.Empty<Ring>())
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
                return new MacroStabilityInwardsStochasticSoilProfile(1, new TestMacroStabilityInwardsSoilProfile1D());
            }
        }

        #region SurfaceLineEntity: Read, Contains, Get

        [Test]
        public void Contains_SurfaceLineEntityIsNull_ThrowsArgumentNullException()
        {
            // Setup
            var collector = new ReadConversionCollector();

            // Call
            TestDelegate call = () => collector.Contains((SurfaceLineEntity) null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("entity", paramName);
        }

        [Test]
        public void Contains_SurfaceLineEntityAdded_ReturnsTrue()
        {
            // Setup
            var entity = new SurfaceLineEntity();
            var model = new PipingSurfaceLine(string.Empty);

            var collector = new ReadConversionCollector();
            collector.Read(entity, model);

            // Call
            bool hasEntity = collector.Contains(entity);

            // Assert
            Assert.IsTrue(hasEntity);
        }

        [Test]
        public void Contains_SurfaceLineEntityNotAdded_ReturnsFalse()
        {
            // Setup
            var entity = new SurfaceLineEntity();

            var collector = new ReadConversionCollector();

            // Call
            bool hasEntity = collector.Contains(entity);

            // Assert
            Assert.IsFalse(hasEntity);
        }

        [Test]
        public void Contains_OtherSurfaceLineEntityAdded_ReturnsFalse()
        {
            // Setup
            var registeredEntity = new SurfaceLineEntity();
            var model = new PipingSurfaceLine(string.Empty);

            var collector = new ReadConversionCollector();
            collector.Read(registeredEntity, model);

            var unregisteredEntity = new SurfaceLineEntity();

            // Call
            bool hasEntity = collector.Contains(unregisteredEntity);

            // Assert
            Assert.IsFalse(hasEntity);
        }

        [Test]
        public void Get_SurfaceLineEntityIsNull_ThrowsArgumentNullException()
        {
            // Setup
            var collector = new ReadConversionCollector();

            // Call
            TestDelegate call = () => collector.Get((SurfaceLineEntity) null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("entity", paramName);
        }

        [Test]
        public void Get_SurfaceLineEntityAdded_ReturnsPipingSurfaceLine()
        {
            // Setup
            var entity = new SurfaceLineEntity();
            var model = new PipingSurfaceLine(string.Empty);

            var collector = new ReadConversionCollector();
            collector.Read(entity, model);

            // Call
            PipingSurfaceLine retrievedGeometryPoint = collector.Get(entity);

            // Assert
            Assert.AreSame(model, retrievedGeometryPoint);
        }

        [Test]
        public void Get_SurfaceLineEntityNotAdded_ThrowInvalidOperationException()
        {
            // Setup
            var entity = new SurfaceLineEntity();

            var collector = new ReadConversionCollector();

            // Call
            TestDelegate call = () => collector.Get(entity);

            // Assert
            Assert.Throws<InvalidOperationException>(call);
        }

        [Test]
        public void Get_DifferentSurfaceLineEntityAdded_ThrowsInvalidOperationException()
        {
            // Setup
            var registeredEntity = new SurfaceLineEntity();
            var model = new PipingSurfaceLine(string.Empty);

            var collector = new ReadConversionCollector();
            collector.Read(registeredEntity, model);

            var unregisteredEntity = new SurfaceLineEntity();

            // Call
            TestDelegate call = () => collector.Get(unregisteredEntity);

            // Assert
            Assert.Throws<InvalidOperationException>(call);
        }

        [Test]
        public void Read_SurfaceLineEntityIsNull_ThrowsArgumentNullException()
        {
            // Setup
            var collector = new ReadConversionCollector();

            // Call
            TestDelegate test = () => collector.Read(null, new PipingSurfaceLine(string.Empty));

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("entity", paramName);
        }

        [Test]
        public void Read_PipingSurfaceLineIsNull_ThrowsArgumentNullException()
        {
            // Setup
            var collector = new ReadConversionCollector();

            // Call
            TestDelegate test = () => collector.Read(new SurfaceLineEntity(), null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("model", paramName);
        }

        #endregion

        #region HydraulicLocationEntity: Read, Contains, Get

        [Test]
        public void Contains_WithoutHydraulicLocationEntity_ThrowsArgumentNullException()
        {
            // Setup
            var collector = new ReadConversionCollector();

            // Call
            TestDelegate test = () => collector.Contains((HydraulicLocationEntity) null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("entity", paramName);
        }

        [Test]
        public void Contains_HydraulicLocationEntityAdded_ReturnsTrue()
        {
            // Setup
            var collector = new ReadConversionCollector();
            var entity = new HydraulicLocationEntity();
            collector.Read(entity, new HydraulicBoundaryLocation(1, "A", 1, 2));

            // Call
            bool result = collector.Contains(entity);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void Contains_NoHydraulicLocationEntityAdded_ReturnsFalse()
        {
            // Setup
            var collector = new ReadConversionCollector();
            var entity = new HydraulicLocationEntity();

            // Call
            bool result = collector.Contains(entity);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Contains_OtherHydraulicLocationEntityAdded_ReturnsFalse()
        {
            // Setup
            var collector = new ReadConversionCollector();
            var entity = new HydraulicLocationEntity();
            collector.Read(new HydraulicLocationEntity(), new HydraulicBoundaryLocation(1, "A", 2, 3));

            // Call
            bool result = collector.Contains(entity);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Get_WithoutHydraulicLocationEntity_ThrowsArgumentNullException()
        {
            // Setup
            var collector = new ReadConversionCollector();

            // Call
            TestDelegate test = () => collector.Get((HydraulicLocationEntity) null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("entity", paramName);
        }

        [Test]
        public void Get_HydraulicLocationEntityAdded_ReturnsHydraulicBoundaryLocation()
        {
            // Setup
            var collector = new ReadConversionCollector();
            var profile = new HydraulicBoundaryLocation(1, "A", 1, 1);
            var entity = new HydraulicLocationEntity();
            collector.Read(entity, profile);

            // Call
            HydraulicBoundaryLocation result = collector.Get(entity);

            // Assert
            Assert.AreSame(profile, result);
        }

        [Test]
        public void Get_NoHydraulicLocationEntityAdded_ThrowsInvalidOperationException()
        {
            // Setup
            var collector = new ReadConversionCollector();
            var entity = new HydraulicLocationEntity();

            // Call
            TestDelegate test = () => collector.Get(entity);

            // Assert
            Assert.Throws<InvalidOperationException>(test);
        }

        [Test]
        public void Get_OtherHydraulicLocationEntityAdded_ThrowsInvalidOperationException()
        {
            // Setup
            var collector = new ReadConversionCollector();
            var entity = new HydraulicLocationEntity();
            collector.Read(new HydraulicLocationEntity(), new HydraulicBoundaryLocation(1, "A", 1, 1));

            // Call
            TestDelegate test = () => collector.Get(entity);

            // Assert
            Assert.Throws<InvalidOperationException>(test);
        }

        [Test]
        public void Read_WithNullHydraulicLocationEntity_ThrowsArgumentNullException()
        {
            // Setup
            var collector = new ReadConversionCollector();

            // Call
            TestDelegate test = () => collector.Read((HydraulicLocationEntity) null, new HydraulicBoundaryLocation(1, "A", 1, 1));

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("entity", paramName);
        }

        [Test]
        public void Read_WithNullHydraulicBoundaryLocation_ThrowsArgumentNullException()
        {
            // Setup
            var collector = new ReadConversionCollector();

            // Call
            TestDelegate test = () => collector.Read(new HydraulicLocationEntity(), null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("model", paramName);
        }

        #endregion

        #region DuneLocationEntity: Read, Contains, Get

        [Test]
        public void Contains_WithoutDuneLocationEntity_ThrowsArgumentNullException()
        {
            // Setup
            var collector = new ReadConversionCollector();

            // Call
            TestDelegate test = () => collector.Contains((DuneLocationEntity) null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("entity", paramName);
        }

        [Test]
        public void Contains_DuneLocationEntityAdded_ReturnsTrue()
        {
            // Setup
            var collector = new ReadConversionCollector();
            var entity = new DuneLocationEntity();
            collector.Read(entity, new TestDuneLocation());

            // Call
            bool result = collector.Contains(entity);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void Contains_NoDuneLocationEntityAdded_ReturnsFalse()
        {
            // Setup
            var collector = new ReadConversionCollector();
            var entity = new DuneLocationEntity();

            // Call
            bool result = collector.Contains(entity);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Contains_OtherDuneLocationEntityAdded_ReturnsFalse()
        {
            // Setup
            var collector = new ReadConversionCollector();
            var entity = new DuneLocationEntity();
            collector.Read(new DuneLocationEntity(), new TestDuneLocation());

            // Call
            bool result = collector.Contains(entity);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Get_WithoutDuneLocationEntity_ThrowsArgumentNullException()
        {
            // Setup
            var collector = new ReadConversionCollector();

            // Call
            TestDelegate test = () => collector.Get((DuneLocationEntity) null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("entity", paramName);
        }

        [Test]
        public void Get_DuneLocationEntityAdded_ReturnsDuneLocation()
        {
            // Setup
            var collector = new ReadConversionCollector();
            var location = new TestDuneLocation();
            var entity = new DuneLocationEntity();
            collector.Read(entity, location);

            // Call
            DuneLocation result = collector.Get(entity);

            // Assert
            Assert.AreSame(location, result);
        }

        [Test]
        public void Get_NoDuneLocationEntityAdded_ThrowsInvalidOperationException()
        {
            // Setup
            var collector = new ReadConversionCollector();
            var entity = new DuneLocationEntity();

            // Call
            TestDelegate test = () => collector.Get(entity);

            // Assert
            Assert.Throws<InvalidOperationException>(test);
        }

        [Test]
        public void Get_OtherDuneLocationEntityAdded_ThrowsInvalidOperationException()
        {
            // Setup
            var collector = new ReadConversionCollector();
            var entity = new DuneLocationEntity();
            collector.Read(new DuneLocationEntity(), new TestDuneLocation());

            // Call
            TestDelegate test = () => collector.Get(entity);

            // Assert
            Assert.Throws<InvalidOperationException>(test);
        }

        [Test]
        public void Read_WithNullDuneLocationEntity_ThrowsArgumentNullException()
        {
            // Setup
            var collector = new ReadConversionCollector();

            // Call
            TestDelegate test = () => collector.Read(null, new TestDuneLocation());

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("entity", paramName);
        }

        [Test]
        public void Read_WithNullDuneLocation_ThrowsArgumentNullException()
        {
            // Setup
            var collector = new ReadConversionCollector();

            // Call
            TestDelegate test = () => collector.Read(new DuneLocationEntity(), null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("model", paramName);
        }

        #endregion

        #region FailureMechanismSectionEntity: Read, Contains, Get

        [Test]
        public void Contains_WithoutFailureMechanismSectionEntity_ThrowsArgumentNullException()
        {
            // Setup
            var collector = new ReadConversionCollector();

            // Call
            TestDelegate test = () => collector.Contains((FailureMechanismSectionEntity) null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("entity", paramName);
        }

        [Test]
        public void Contains_FailureMechanismSectionEntityAdded_ReturnsTrue()
        {
            // Setup
            var collector = new ReadConversionCollector();
            var entity = new FailureMechanismSectionEntity();
            collector.Read(entity, new TestFailureMechanismSection());

            // Call
            bool result = collector.Contains(entity);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void Contains_NoFailureMechanismSectionEntityAdded_ReturnsFalse()
        {
            // Setup
            var collector = new ReadConversionCollector();
            var entity = new FailureMechanismSectionEntity();

            // Call
            bool result = collector.Contains(entity);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Contains_OtherFailureMechanismSectionEntityAdded_ReturnsFalse()
        {
            // Setup
            var collector = new ReadConversionCollector();
            var entity = new FailureMechanismSectionEntity();
            collector.Read(new FailureMechanismSectionEntity(), new TestFailureMechanismSection());

            // Call
            bool result = collector.Contains(entity);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Get_WithoutFailureMechanismSectionEntity_ThrowsArgumentNullException()
        {
            // Setup
            var collector = new ReadConversionCollector();

            // Call
            TestDelegate test = () => collector.Get((FailureMechanismSectionEntity) null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("entity", paramName);
        }

        [Test]
        public void Get_FailureMechanismSectionEntityAdded_ReturnsHydraulicBoundaryLocation()
        {
            // Setup
            var collector = new ReadConversionCollector();
            var section = new TestFailureMechanismSection();
            var entity = new FailureMechanismSectionEntity();
            collector.Read(entity, section);

            // Call
            FailureMechanismSection result = collector.Get(entity);

            // Assert
            Assert.AreSame(section, result);
        }

        [Test]
        public void Get_NoFailureMechanismSectionEntityAdded_ThrowsInvalidOperationException()
        {
            // Setup
            var collector = new ReadConversionCollector();
            var entity = new FailureMechanismSectionEntity();

            // Call
            TestDelegate test = () => collector.Get(entity);

            // Assert
            Assert.Throws<InvalidOperationException>(test);
        }

        [Test]
        public void Get_OtherFailureMechanismSectionEntityAdded_ThrowsInvalidOperationException()
        {
            // Setup
            var collector = new ReadConversionCollector();
            var entity = new FailureMechanismSectionEntity();
            collector.Read(new FailureMechanismSectionEntity(), new TestFailureMechanismSection());

            // Call
            TestDelegate test = () => collector.Get(entity);

            // Assert
            Assert.Throws<InvalidOperationException>(test);
        }

        [Test]
        public void Read_WithNullFailureMechanismSectionEntity_ThrowsArgumentNullException()
        {
            // Setup
            var collector = new ReadConversionCollector();

            // Call
            TestDelegate test = () => collector.Read(null, new TestFailureMechanismSection());

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("entity", paramName);
        }

        [Test]
        public void Read_WithNullFailureMechanismSection_ThrowsArgumentNullException()
        {
            // Setup
            var collector = new ReadConversionCollector();

            // Call
            TestDelegate test = () => collector.Read(new FailureMechanismSectionEntity(), null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("model", paramName);
        }

        #endregion

        #region DikeProfileEntity: Read, Contains, Get

        [Test]
        public void Contains_WithoutDikeProfileEntity_ThrowsArgumentNullException()
        {
            // Setup
            var collector = new ReadConversionCollector();

            // Call
            TestDelegate test = () => collector.Contains((DikeProfileEntity) null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("entity", paramName);
        }

        [Test]
        public void Contains_DikeProfileEntityAdded_ReturnsTrue()
        {
            // Setup
            var collector = new ReadConversionCollector();
            var entity = new DikeProfileEntity();
            collector.Read(entity, new TestDikeProfile());

            // Call
            bool result = collector.Contains(entity);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void Contains_NoDikeProfileEntityAdded_ReturnsFalse()
        {
            // Setup
            var collector = new ReadConversionCollector();
            var entity = new DikeProfileEntity();

            // Call
            bool result = collector.Contains(entity);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Contains_OtherDikeProfileEntityAdded_ReturnsFalse()
        {
            // Setup
            var collector = new ReadConversionCollector();
            var entity = new DikeProfileEntity();
            collector.Read(new DikeProfileEntity(), new TestDikeProfile());

            // Call
            bool result = collector.Contains(entity);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Get_WithoutDikeProfileEntity_ThrowsArgumentNullException()
        {
            // Setup
            var collector = new ReadConversionCollector();

            // Call
            TestDelegate test = () => collector.Get((DikeProfileEntity) null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("entity", paramName);
        }

        [Test]
        public void Get_DikeProfileEntityAdded_ReturnsDikeProfile()
        {
            // Setup
            var collector = new ReadConversionCollector();
            DikeProfile dikeProfile = new TestDikeProfile();
            var entity = new DikeProfileEntity();
            collector.Read(entity, dikeProfile);

            // Call
            DikeProfile result = collector.Get(entity);

            // Assert
            Assert.AreSame(dikeProfile, result);
        }

        [Test]
        public void Get_NoDikeProfileEntityAdded_ThrowsInvalidOperationException()
        {
            // Setup
            var collector = new ReadConversionCollector();
            var entity = new DikeProfileEntity();

            // Call
            TestDelegate test = () => collector.Get(entity);

            // Assert
            Assert.Throws<InvalidOperationException>(test);
        }

        [Test]
        public void Get_OtherDikeProfileEntityAdded_ThrowsInvalidOperationException()
        {
            // Setup
            var collector = new ReadConversionCollector();
            var entity = new DikeProfileEntity();
            collector.Read(new DikeProfileEntity(), new TestDikeProfile());

            // Call
            TestDelegate test = () => collector.Get(entity);

            // Assert
            Assert.Throws<InvalidOperationException>(test);
        }

        [Test]
        public void Read_WithNullDikeProfileEntity_ThrowsArgumentNullException()
        {
            // Setup
            var collector = new ReadConversionCollector();

            // Call
            TestDelegate test = () => collector.Read(null, new TestDikeProfile());

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("entity", paramName);
        }

        [Test]
        public void Read_WithNullDikeProfile_ThrowsArgumentNullException()
        {
            // Setup
            var collector = new ReadConversionCollector();

            // Call
            TestDelegate test = () => collector.Read(new DikeProfileEntity(), null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("model", paramName);
        }

        #endregion

        #region ForeshoreProfileEntity: Read, Contains, Get

        [Test]
        public void Contains_WithoutForeshoreProfileEntity_ThrowsArgumentNullException()
        {
            // Setup
            var collector = new ReadConversionCollector();

            // Call
            TestDelegate test = () => collector.Contains((ForeshoreProfileEntity) null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("entity", paramName);
        }

        [Test]
        public void Contains_ForeshoreProfileEntityAdded_ReturnsTrue()
        {
            // Setup
            var collector = new ReadConversionCollector();
            var entity = new ForeshoreProfileEntity();
            collector.Read(entity, new TestForeshoreProfile());

            // Call
            bool result = collector.Contains(entity);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void Contains_NoForeshoreProfileEntityAdded_ReturnsFalse()
        {
            // Setup
            var collector = new ReadConversionCollector();
            var entity = new ForeshoreProfileEntity();

            // Call
            bool result = collector.Contains(entity);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Contains_OtherForeshoreProfileEntityAdded_ReturnsFalse()
        {
            // Setup
            var collector = new ReadConversionCollector();
            var entity = new ForeshoreProfileEntity();
            collector.Read(new ForeshoreProfileEntity(), new TestForeshoreProfile());

            // Call
            bool result = collector.Contains(entity);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Get_WithoutForeshoreProfileEntity_ThrowsArgumentNullException()
        {
            // Setup
            var collector = new ReadConversionCollector();

            // Call
            TestDelegate test = () => collector.Get((ForeshoreProfileEntity) null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("entity", paramName);
        }

        [Test]
        public void Get_ForeshoreProfileEntityAdded_ReturnsForeshoreProfile()
        {
            // Setup
            var collector = new ReadConversionCollector();
            ForeshoreProfile foreshoreProfile = new TestForeshoreProfile();
            var entity = new ForeshoreProfileEntity();
            collector.Read(entity, foreshoreProfile);

            // Call
            ForeshoreProfile result = collector.Get(entity);

            // Assert
            Assert.AreSame(foreshoreProfile, result);
        }

        [Test]
        public void Get_NoForeshoreProfileEntityAdded_ThrowsInvalidOperationException()
        {
            // Setup
            var collector = new ReadConversionCollector();
            var entity = new ForeshoreProfileEntity();

            // Call
            TestDelegate test = () => collector.Get(entity);

            // Assert
            Assert.Throws<InvalidOperationException>(test);
        }

        [Test]
        public void Get_OtherForeshoreProfileEntityAdded_ThrowsInvalidOperationException()
        {
            // Setup
            var collector = new ReadConversionCollector();
            var entity = new ForeshoreProfileEntity();
            collector.Read(new ForeshoreProfileEntity(), new TestForeshoreProfile());

            // Call
            TestDelegate test = () => collector.Get(entity);

            // Assert
            Assert.Throws<InvalidOperationException>(test);
        }

        [Test]
        public void Read_WithNullForeshoreProfileEntity_ThrowsArgumentNullException()
        {
            // Setup
            var collector = new ReadConversionCollector();

            // Call
            TestDelegate test = () => collector.Read(null, new TestForeshoreProfile());

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("entity", paramName);
        }

        [Test]
        public void Read_WithNullForeshoreProfile_ThrowsArgumentNullException()
        {
            // Setup
            var collector = new ReadConversionCollector();

            // Call
            TestDelegate test = () => collector.Read(new ForeshoreProfileEntity(), null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("model", paramName);
        }

        #endregion

        #region GrassCoverErosionInwardsCalculationEntity: Read, Contains, Get

        [Test]
        public void Contains_WithoutGrassCoverErosionInwardsCalculationEntity_ThrowsArgumentNullException()
        {
            // Setup
            var collector = new ReadConversionCollector();

            // Call
            TestDelegate test = () => collector.Contains((GrassCoverErosionInwardsCalculationEntity) null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("entity", paramName);
        }

        [Test]
        public void Contains_GrassCoverErosionInwardsCalculationEntityAdded_ReturnsTrue()
        {
            // Setup
            var collector = new ReadConversionCollector();
            var entity = new GrassCoverErosionInwardsCalculationEntity();
            collector.Read(entity, new GrassCoverErosionInwardsCalculation());

            // Call
            bool result = collector.Contains(entity);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void Contains_NoGrassCoverErosionInwardsCalculationEntityAdded_ReturnsFalse()
        {
            // Setup
            var collector = new ReadConversionCollector();
            var entity = new GrassCoverErosionInwardsCalculationEntity();

            // Call
            bool result = collector.Contains(entity);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Contains_OtherGrassCoverErosionInwardsCalculationEntityAdded_ReturnsFalse()
        {
            // Setup
            var collector = new ReadConversionCollector();
            var entity = new GrassCoverErosionInwardsCalculationEntity();
            collector.Read(new GrassCoverErosionInwardsCalculationEntity(), new GrassCoverErosionInwardsCalculation());

            // Call
            bool result = collector.Contains(entity);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Get_WithoutGrassCoverErosionInwardsCalculationEntity_ThrowsArgumentNullException()
        {
            // Setup
            var collector = new ReadConversionCollector();

            // Call
            TestDelegate test = () => collector.Get((GrassCoverErosionInwardsCalculationEntity) null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("entity", paramName);
        }

        [Test]
        public void Get_GrassCoverErosionInwardsCalculationEntityAdded_ReturnsGrassCoverErosionInwardsCalculation()
        {
            // Setup
            var collector = new ReadConversionCollector();
            var calculation = new GrassCoverErosionInwardsCalculation();
            var entity = new GrassCoverErosionInwardsCalculationEntity();
            collector.Read(entity, calculation);

            // Call
            GrassCoverErosionInwardsCalculation result = collector.Get(entity);

            // Assert
            Assert.AreSame(calculation, result);
        }

        [Test]
        public void Get_NoGrassCoverErosionInwardsCalculationEntityAdded_ThrowsInvalidOperationException()
        {
            // Setup
            var collector = new ReadConversionCollector();
            var entity = new GrassCoverErosionInwardsCalculationEntity();

            // Call
            TestDelegate test = () => collector.Get(entity);

            // Assert
            Assert.Throws<InvalidOperationException>(test);
        }

        [Test]
        public void Get_OtherGrassCoverErosionInwardsCalculationEntityAdded_ThrowsInvalidOperationException()
        {
            // Setup
            var collector = new ReadConversionCollector();
            var entity = new GrassCoverErosionInwardsCalculationEntity();
            collector.Read(new GrassCoverErosionInwardsCalculationEntity(), new GrassCoverErosionInwardsCalculation());

            // Call
            TestDelegate test = () => collector.Get(entity);

            // Assert
            Assert.Throws<InvalidOperationException>(test);
        }

        [Test]
        public void Read_WithNullGrassCoverErosionInwardsCalculationEntity_ThrowsArgumentNullException()
        {
            // Setup
            var collector = new ReadConversionCollector();

            // Call
            TestDelegate test = () => collector.Read(null, new GrassCoverErosionInwardsCalculation());

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("entity", paramName);
        }

        [Test]
        public void Read_WithNullGrassCoverErosionInwardsCalculation_ThrowsArgumentNullException()
        {
            // Setup
            var collector = new ReadConversionCollector();

            // Call
            TestDelegate test = () => collector.Read(new GrassCoverErosionInwardsCalculationEntity(), null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("model", paramName);
        }

        #endregion

        #region GrassCoverErosionOutwardsHydraulicLocationEntity: Read, Contains, Get

        [Test]
        public void Contains_WithoutGrassCoverErosionOutwardsHydraulicLocationEntity_ThrowsArgumentNullException()
        {
            // Setup
            var collector = new ReadConversionCollector();

            // Call
            TestDelegate test = () => collector.Contains((GrassCoverErosionOutwardsHydraulicLocationEntity) null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("entity", paramName);
        }

        [Test]
        public void Contains_GrassCoverErosionOutwardsHydraulicLocationEntityAdded_ReturnsTrue()
        {
            // Setup
            var collector = new ReadConversionCollector();
            var entity = new GrassCoverErosionOutwardsHydraulicLocationEntity();
            collector.Read(entity, new HydraulicBoundaryLocation(1, "A", 1, 2));

            // Call
            bool result = collector.Contains(entity);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void Contains_NoGrassCoverErosionOutwardsHydraulicLocationEntityAdded_ReturnsFalse()
        {
            // Setup
            var collector = new ReadConversionCollector();
            var entity = new GrassCoverErosionOutwardsHydraulicLocationEntity();

            // Call
            bool result = collector.Contains(entity);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Contains_OtherGrassCoverErosionOutwardsHydraulicLocationEntityAdded_ReturnsFalse()
        {
            // Setup
            var collector = new ReadConversionCollector();
            var entity = new GrassCoverErosionOutwardsHydraulicLocationEntity();
            collector.Read(new GrassCoverErosionOutwardsHydraulicLocationEntity(), new HydraulicBoundaryLocation(1, "A", 2, 3));

            // Call
            bool result = collector.Contains(entity);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Get_WithoutGrassCoverErosionOutwardsHydraulicLocationEntity_ThrowsArgumentNullException()
        {
            // Setup
            var collector = new ReadConversionCollector();

            // Call
            TestDelegate test = () => collector.Get((GrassCoverErosionOutwardsHydraulicLocationEntity) null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("entity", paramName);
        }

        [Test]
        public void Get_GrassCoverErosionOutwardsHydraulicLocationEntityAdded_ReturnsHydraulicBoundaryLocation()
        {
            // Setup
            var collector = new ReadConversionCollector();
            var profile = new HydraulicBoundaryLocation(1, "A", 1, 1);
            var entity = new GrassCoverErosionOutwardsHydraulicLocationEntity();
            collector.Read(entity, profile);

            // Call
            HydraulicBoundaryLocation result = collector.Get(entity);

            // Assert
            Assert.AreSame(profile, result);
        }

        [Test]
        public void Get_NoGrassCoverErosionOutwardsHydraulicLocationEntityAdded_ThrowsInvalidOperationException()
        {
            // Setup
            var collector = new ReadConversionCollector();
            var entity = new GrassCoverErosionOutwardsHydraulicLocationEntity();

            // Call
            TestDelegate test = () => collector.Get(entity);

            // Assert
            Assert.Throws<InvalidOperationException>(test);
        }

        [Test]
        public void Get_OtherGrassCoverErosionOutwardsHydraulicLocationEntityAdded_ThrowsInvalidOperationException()
        {
            // Setup
            var collector = new ReadConversionCollector();
            var entity = new GrassCoverErosionOutwardsHydraulicLocationEntity();
            collector.Read(new GrassCoverErosionOutwardsHydraulicLocationEntity(), new HydraulicBoundaryLocation(1, "A", 1, 1));

            // Call
            TestDelegate test = () => collector.Get(entity);

            // Assert
            Assert.Throws<InvalidOperationException>(test);
        }

        [Test]
        public void Read_WithNullGrassCoverErosionOutwardsHydraulicLocationEntity_ThrowsArgumentNullException()
        {
            // Setup
            var collector = new ReadConversionCollector();

            // Call
            TestDelegate test = () => collector.Read((GrassCoverErosionOutwardsHydraulicLocationEntity) null, new HydraulicBoundaryLocation(1, "A", 1, 1));

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("entity", paramName);
        }

        [Test]
        public void Read_WithNullHydraulicBoundaryLocationForGrassCoverErosionOutwardsHydraulicLocationEntity_ThrowsArgumentNullException()
        {
            // Setup
            var collector = new ReadConversionCollector();

            // Call
            TestDelegate test = () => collector.Read(new GrassCoverErosionOutwardsHydraulicLocationEntity(), null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("model", paramName);
        }

        #endregion

        #region HeightStructureEntity: Read, Contains, Get

        [Test]
        public void Contains_WithoutHeightStructureEntity_ThrowsArgumentNullException()
        {
            // Setup
            var collector = new ReadConversionCollector();

            // Call
            TestDelegate test = () => collector.Contains((HeightStructureEntity) null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("entity", paramName);
        }

        [Test]
        public void Contains_HeightStructureEntityAdded_ReturnsTrue()
        {
            // Setup
            var collector = new ReadConversionCollector();
            var entity = new HeightStructureEntity();
            collector.Read(entity, new TestHeightStructure());

            // Call
            bool result = collector.Contains(entity);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void Contains_NoHeightStructureEntityAdded_ReturnsFalse()
        {
            // Setup
            var collector = new ReadConversionCollector();
            var entity = new HeightStructureEntity();

            // Call
            bool result = collector.Contains(entity);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Contains_OtherHeightStructureEntityAdded_ReturnsFalse()
        {
            // Setup
            var collector = new ReadConversionCollector();
            var entity = new HeightStructureEntity();
            collector.Read(new HeightStructureEntity(), new TestHeightStructure());

            // Call
            bool result = collector.Contains(entity);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Get_WithoutHeightStructureEntity_ThrowsArgumentNullException()
        {
            // Setup
            var collector = new ReadConversionCollector();

            // Call
            TestDelegate test = () => collector.Get((HeightStructureEntity) null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("entity", paramName);
        }

        [Test]
        public void Get_HeightStructureEntityAdded_ReturnsHeightStructure()
        {
            // Setup
            var collector = new ReadConversionCollector();
            HeightStructure structure = new TestHeightStructure();
            var entity = new HeightStructureEntity();
            collector.Read(entity, structure);

            // Call
            HeightStructure result = collector.Get(entity);

            // Assert
            Assert.AreSame(structure, result);
        }

        [Test]
        public void Get_NoHeightStructureEntityAdded_ThrowsInvalidOperationException()
        {
            // Setup
            var collector = new ReadConversionCollector();
            var entity = new HeightStructureEntity();

            // Call
            TestDelegate test = () => collector.Get(entity);

            // Assert
            Assert.Throws<InvalidOperationException>(test);
        }

        [Test]
        public void Get_OtherHeightStructureEntityAdded_ThrowsInvalidOperationException()
        {
            // Setup
            var collector = new ReadConversionCollector();
            var entity = new HeightStructureEntity();
            collector.Read(new HeightStructureEntity(), new TestHeightStructure());

            // Call
            TestDelegate test = () => collector.Get(entity);

            // Assert
            Assert.Throws<InvalidOperationException>(test);
        }

        [Test]
        public void Read_WithNullHeightStructureEntity_ThrowsArgumentNullException()
        {
            // Setup
            var collector = new ReadConversionCollector();

            // Call
            TestDelegate test = () => collector.Read(null, new TestHeightStructure());

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("entity", paramName);
        }

        [Test]
        public void Read_WithNullHeightStructureForHeightStructureEntity_ThrowsArgumentNullException()
        {
            // Setup
            var collector = new ReadConversionCollector();

            // Call
            TestDelegate test = () => collector.Read(new HeightStructureEntity(), null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("model", paramName);
        }

        #endregion

        #region ClosingStructureEntity: Read, Contains, Get

        [Test]
        public void Contains_WithoutClosingStructureEntity_ThrowsArgumentNullException()
        {
            // Setup
            var collector = new ReadConversionCollector();

            // Call
            TestDelegate test = () => collector.Contains((ClosingStructureEntity) null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("entity", paramName);
        }

        [Test]
        public void Contains_ClosingStructureEntityAdded_ReturnsTrue()
        {
            // Setup
            var collector = new ReadConversionCollector();
            var entity = new ClosingStructureEntity();
            collector.Read(entity, new TestClosingStructure());

            // Call
            bool result = collector.Contains(entity);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void Contains_NoClosingStructureEntityAdded_ReturnsFalse()
        {
            // Setup
            var collector = new ReadConversionCollector();
            var entity = new ClosingStructureEntity();

            // Call
            bool result = collector.Contains(entity);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Contains_OtherClosingStructureEntityAdded_ReturnsFalse()
        {
            // Setup
            var collector = new ReadConversionCollector();
            var entity = new ClosingStructureEntity();
            collector.Read(new ClosingStructureEntity(), new TestClosingStructure());

            // Call
            bool result = collector.Contains(entity);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Get_WithoutClosingStructureEntity_ThrowsArgumentNullException()
        {
            // Setup
            var collector = new ReadConversionCollector();

            // Call
            TestDelegate test = () => collector.Get((ClosingStructureEntity) null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("entity", paramName);
        }

        [Test]
        public void Get_ClosingStructureEntityAdded_ReturnsClosingStructure()
        {
            // Setup
            var collector = new ReadConversionCollector();
            ClosingStructure structure = new TestClosingStructure();
            var entity = new ClosingStructureEntity();
            collector.Read(entity, structure);

            // Call
            ClosingStructure result = collector.Get(entity);

            // Assert
            Assert.AreSame(structure, result);
        }

        [Test]
        public void Get_NoClosingStructureEntityAdded_ThrowsInvalidOperationException()
        {
            // Setup
            var collector = new ReadConversionCollector();
            var entity = new ClosingStructureEntity();

            // Call
            TestDelegate test = () => collector.Get(entity);

            // Assert
            Assert.Throws<InvalidOperationException>(test);
        }

        [Test]
        public void Get_OtherClosingStructureEntityAdded_ThrowsInvalidOperationException()
        {
            // Setup
            var collector = new ReadConversionCollector();
            var entity = new ClosingStructureEntity();
            collector.Read(new ClosingStructureEntity(), new TestClosingStructure());

            // Call
            TestDelegate test = () => collector.Get(entity);

            // Assert
            Assert.Throws<InvalidOperationException>(test);
        }

        [Test]
        public void Read_WithNullClosingStructureEntity_ThrowsArgumentNullException()
        {
            // Setup
            var collector = new ReadConversionCollector();

            // Call
            TestDelegate test = () => collector.Read(null, new TestClosingStructure());

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("entity", paramName);
        }

        [Test]
        public void Read_WithNullClosingStructureForClosingStructureEntity_ThrowsArgumentNullException()
        {
            // Setup
            var collector = new ReadConversionCollector();

            // Call
            TestDelegate test = () => collector.Read(new ClosingStructureEntity(), null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("model", paramName);
        }

        #endregion

        #region StabilityPointStructureEntity: Read, Contains, Get

        [Test]
        public void Contains_WithoutStabilityPointStructureEntity_ThrowsArgumentNullException()
        {
            // Setup
            var collector = new ReadConversionCollector();

            // Call
            TestDelegate test = () => collector.Contains((StabilityPointStructureEntity) null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("entity", paramName);
        }

        [Test]
        public void Contains_StabilityPointStructureEntityAdded_ReturnsTrue()
        {
            // Setup
            var collector = new ReadConversionCollector();
            var entity = new StabilityPointStructureEntity();
            collector.Read(entity, new TestStabilityPointStructure());

            // Call
            bool result = collector.Contains(entity);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void Contains_NoStabilityPointStructureEntityAdded_ReturnsFalse()
        {
            // Setup
            var collector = new ReadConversionCollector();
            var entity = new StabilityPointStructureEntity();

            // Call
            bool result = collector.Contains(entity);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Contains_OtherStabilityPointStructureEntityAdded_ReturnsFalse()
        {
            // Setup
            var collector = new ReadConversionCollector();
            var entity = new StabilityPointStructureEntity();
            collector.Read(new StabilityPointStructureEntity(), new TestStabilityPointStructure());

            // Call
            bool result = collector.Contains(entity);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Get_WithoutStabilityPointStructureEntity_ThrowsArgumentNullException()
        {
            // Setup
            var collector = new ReadConversionCollector();

            // Call
            TestDelegate test = () => collector.Get((StabilityPointStructureEntity) null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("entity", paramName);
        }

        [Test]
        public void Get_StabilityPointStructureEntityAdded_ReturnsHeightStructure()
        {
            // Setup
            var collector = new ReadConversionCollector();
            StabilityPointStructure structure = new TestStabilityPointStructure();
            var entity = new StabilityPointStructureEntity();
            collector.Read(entity, structure);

            // Call
            StabilityPointStructure result = collector.Get(entity);

            // Assert
            Assert.AreSame(structure, result);
        }

        [Test]
        public void Get_NoStabilityPointStructureEntityAdded_ThrowsInvalidOperationException()
        {
            // Setup
            var collector = new ReadConversionCollector();
            var entity = new StabilityPointStructureEntity();

            // Call
            TestDelegate test = () => collector.Get(entity);

            // Assert
            Assert.Throws<InvalidOperationException>(test);
        }

        [Test]
        public void Get_OtherStabilityPointStructureEntityAdded_ThrowsInvalidOperationException()
        {
            // Setup
            var collector = new ReadConversionCollector();
            var entity = new StabilityPointStructureEntity();
            collector.Read(new StabilityPointStructureEntity(), new TestStabilityPointStructure());

            // Call
            TestDelegate test = () => collector.Get(entity);

            // Assert
            Assert.Throws<InvalidOperationException>(test);
        }

        [Test]
        public void Read_WithNullStabilityPointStructureEntity_ThrowsArgumentNullException()
        {
            // Setup
            var collector = new ReadConversionCollector();

            // Call
            TestDelegate test = () => collector.Read(null, new TestStabilityPointStructure());

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("entity", paramName);
        }

        [Test]
        public void Read_WithNullStabilityPointStructureForStabilityPointStructureEntity_ThrowsArgumentNullException()
        {
            // Setup
            var collector = new ReadConversionCollector();

            // Call
            TestDelegate test = () => collector.Read(new StabilityPointStructureEntity(), null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("model", paramName);
        }

        #endregion

        #region HeightStructuresCalculationEntity: Read, Contains, Get

        [Test]
        public void Contains_WithoutHeightStructuresCalculationEntity_ThrowsArgumentNullException()
        {
            // Setup
            var collector = new ReadConversionCollector();

            // Call
            TestDelegate test = () => collector.Contains((HeightStructuresCalculationEntity) null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("entity", paramName);
        }

        [Test]
        public void Contains_HeightStructuresCalculationEntityAdded_ReturnsTrue()
        {
            // Setup
            var collector = new ReadConversionCollector();
            var entity = new HeightStructuresCalculationEntity();
            collector.Read(entity, new StructuresCalculation<HeightStructuresInput>());

            // Call
            bool result = collector.Contains(entity);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void Contains_NoHeightStructuresCalculationEntityAdded_ReturnsFalse()
        {
            // Setup
            var collector = new ReadConversionCollector();
            var entity = new HeightStructuresCalculationEntity();

            // Call
            bool result = collector.Contains(entity);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Contains_OtherHeightStructuresCalculationEntityAdded_ReturnsFalse()
        {
            // Setup
            var collector = new ReadConversionCollector();
            var entity = new HeightStructuresCalculationEntity();
            collector.Read(new HeightStructuresCalculationEntity(), new StructuresCalculation<HeightStructuresInput>());

            // Call
            bool result = collector.Contains(entity);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Get_WithoutHeightStructuresCalculationEntity_ThrowsArgumentNullException()
        {
            // Setup
            var collector = new ReadConversionCollector();

            // Call
            TestDelegate test = () => collector.Get((HeightStructuresCalculationEntity) null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("entity", paramName);
        }

        [Test]
        public void Get_HeightStructuresCalculationEntityAdded_ReturnsHeightStructuresCalculation()
        {
            // Setup
            var collector = new ReadConversionCollector();
            var calculation = new StructuresCalculation<HeightStructuresInput>();
            var entity = new HeightStructuresCalculationEntity();
            collector.Read(entity, calculation);

            // Call
            StructuresCalculation<HeightStructuresInput> result = collector.Get(entity);

            // Assert
            Assert.AreSame(calculation, result);
        }

        [Test]
        public void Get_NoHeightStructuresCalculationEntityAdded_ThrowsInvalidOperationException()
        {
            // Setup
            var collector = new ReadConversionCollector();
            var entity = new HeightStructuresCalculationEntity();

            // Call
            TestDelegate test = () => collector.Get(entity);

            // Assert
            Assert.Throws<InvalidOperationException>(test);
        }

        [Test]
        public void Get_OtherHeightStructuresCalculationEntityAdded_ThrowsInvalidOperationException()
        {
            // Setup
            var collector = new ReadConversionCollector();
            var entity = new HeightStructuresCalculationEntity();
            collector.Read(new HeightStructuresCalculationEntity(), new StructuresCalculation<HeightStructuresInput>());

            // Call
            TestDelegate test = () => collector.Get(entity);

            // Assert
            Assert.Throws<InvalidOperationException>(test);
        }

        [Test]
        public void Read_WithNullHeightStructuresCalculationEntity_ThrowsArgumentNullException()
        {
            // Setup
            var collector = new ReadConversionCollector();

            // Call
            TestDelegate test = () => collector.Read(null, new StructuresCalculation<HeightStructuresInput>());

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("entity", paramName);
        }

        [Test]
        public void Read_WithNullHeightStructuresCalculation_ThrowsArgumentNullException()
        {
            // Setup
            var collector = new ReadConversionCollector();

            // Call
            TestDelegate test = () => collector.Read(new HeightStructuresCalculationEntity(), null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("model", paramName);
        }

        #endregion

        #region ClosingStructuresCalculationEntity: Read, Contains, Get

        [Test]
        public void Contains_WithoutClosingStructuresCalculationEntity_ThrowsArgumentNullException()
        {
            // Setup
            var collector = new ReadConversionCollector();

            // Call
            TestDelegate test = () => collector.Contains((ClosingStructuresCalculationEntity) null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("entity", paramName);
        }

        [Test]
        public void Contains_ClosingStructuresCalculationEntityAdded_ReturnsTrue()
        {
            // Setup
            var collector = new ReadConversionCollector();
            var entity = new ClosingStructuresCalculationEntity();
            collector.Read(entity, new StructuresCalculation<ClosingStructuresInput>());

            // Call
            bool result = collector.Contains(entity);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void Contains_NoClosingStructuresCalculationEntityAdded_ReturnsFalse()
        {
            // Setup
            var collector = new ReadConversionCollector();
            var entity = new ClosingStructuresCalculationEntity();

            // Call
            bool result = collector.Contains(entity);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Contains_OtherClosingStructuresCalculationEntityAdded_ReturnsFalse()
        {
            // Setup
            var collector = new ReadConversionCollector();
            var entity = new ClosingStructuresCalculationEntity();
            collector.Read(new ClosingStructuresCalculationEntity(), new StructuresCalculation<ClosingStructuresInput>());

            // Call
            bool result = collector.Contains(entity);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Get_WithoutClosingStructuresCalculationEntity_ThrowsArgumentNullException()
        {
            // Setup
            var collector = new ReadConversionCollector();

            // Call
            TestDelegate test = () => collector.Get((ClosingStructuresCalculationEntity) null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("entity", paramName);
        }

        [Test]
        public void Get_ClosingStructuresCalculationEntityAdded_ReturnsClosingStructuresCalculation()
        {
            // Setup
            var collector = new ReadConversionCollector();
            var calculation = new StructuresCalculation<ClosingStructuresInput>();
            var entity = new ClosingStructuresCalculationEntity();
            collector.Read(entity, calculation);

            // Call
            StructuresCalculation<ClosingStructuresInput> result = collector.Get(entity);

            // Assert
            Assert.AreSame(calculation, result);
        }

        [Test]
        public void Get_NoClosingStructuresCalculationEntityAdded_ThrowsInvalidOperationException()
        {
            // Setup
            var collector = new ReadConversionCollector();
            var entity = new ClosingStructuresCalculationEntity();

            // Call
            TestDelegate test = () => collector.Get(entity);

            // Assert
            Assert.Throws<InvalidOperationException>(test);
        }

        [Test]
        public void Get_OtherClosingStructuresCalculationEntityAdded_ThrowsInvalidOperationException()
        {
            // Setup
            var collector = new ReadConversionCollector();
            var entity = new ClosingStructuresCalculationEntity();
            collector.Read(new ClosingStructuresCalculationEntity(), new StructuresCalculation<ClosingStructuresInput>());

            // Call
            TestDelegate test = () => collector.Get(entity);

            // Assert
            Assert.Throws<InvalidOperationException>(test);
        }

        [Test]
        public void Read_WithNullClosingStructuresCalculationEntity_ThrowsArgumentNullException()
        {
            // Setup
            var collector = new ReadConversionCollector();

            // Call
            TestDelegate test = () => collector.Read(null, new StructuresCalculation<ClosingStructuresInput>());

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("entity", paramName);
        }

        [Test]
        public void Read_WithNullClosingStructuresCalculation_ThrowsArgumentNullException()
        {
            // Setup
            var collector = new ReadConversionCollector();

            // Call
            TestDelegate test = () => collector.Read(new ClosingStructuresCalculationEntity(), null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("model", paramName);
        }

        #endregion

        #region StabilityPointStructuresCalculationEntity: Read, Contains, Get

        [Test]
        public void Contains_WithoutStabilityPointStructuresCalculationEntity_ThrowsArgumentNullException()
        {
            // Setup
            var collector = new ReadConversionCollector();

            // Call
            TestDelegate test = () => collector.Contains((StabilityPointStructuresCalculationEntity) null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("entity", paramName);
        }

        [Test]
        public void Contains_StabilityPointStructuresCalculationEntityAdded_ReturnsTrue()
        {
            // Setup
            var collector = new ReadConversionCollector();
            var entity = new StabilityPointStructuresCalculationEntity();
            collector.Read(entity, new StructuresCalculation<StabilityPointStructuresInput>());

            // Call
            bool result = collector.Contains(entity);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void Contains_NoStabilityPointStructuresCalculationEntityAdded_ReturnsFalse()
        {
            // Setup
            var collector = new ReadConversionCollector();
            var entity = new StabilityPointStructuresCalculationEntity();

            // Call
            bool result = collector.Contains(entity);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Contains_OtherStabilityPointStructuresCalculationEntityAdded_ReturnsFalse()
        {
            // Setup
            var collector = new ReadConversionCollector();
            var entity = new StabilityPointStructuresCalculationEntity();
            collector.Read(new StabilityPointStructuresCalculationEntity(), new StructuresCalculation<StabilityPointStructuresInput>());

            // Call
            bool result = collector.Contains(entity);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Get_WithoutStabilityPointStructuresCalculationEntity_ThrowsArgumentNullException()
        {
            // Setup
            var collector = new ReadConversionCollector();

            // Call
            TestDelegate test = () => collector.Get((StabilityPointStructuresCalculationEntity) null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("entity", paramName);
        }

        [Test]
        public void Get_StabilityPointStructuresCalculationEntityAdded_ReturnsStabilityPointStructuresCalculation()
        {
            // Setup
            var collector = new ReadConversionCollector();
            var calculation = new StructuresCalculation<StabilityPointStructuresInput>();
            var entity = new StabilityPointStructuresCalculationEntity();
            collector.Read(entity, calculation);

            // Call
            StructuresCalculation<StabilityPointStructuresInput> result = collector.Get(entity);

            // Assert
            Assert.AreSame(calculation, result);
        }

        [Test]
        public void Get_NoStabilityPointStructuresCalculationEntityAdded_ThrowsInvalidOperationException()
        {
            // Setup
            var collector = new ReadConversionCollector();
            var entity = new StabilityPointStructuresCalculationEntity();

            // Call
            TestDelegate test = () => collector.Get(entity);

            // Assert
            Assert.Throws<InvalidOperationException>(test);
        }

        [Test]
        public void Get_OtherStabilityPointStructuresCalculationEntityAdded_ThrowsInvalidOperationException()
        {
            // Setup
            var collector = new ReadConversionCollector();
            var entity = new StabilityPointStructuresCalculationEntity();
            collector.Read(new StabilityPointStructuresCalculationEntity(), new StructuresCalculation<StabilityPointStructuresInput>());

            // Call
            TestDelegate test = () => collector.Get(entity);

            // Assert
            Assert.Throws<InvalidOperationException>(test);
        }

        [Test]
        public void Read_WithNullStabilityPointStructuresCalculationEntity_ThrowsArgumentNullException()
        {
            // Setup
            var collector = new ReadConversionCollector();

            // Call
            TestDelegate test = () => collector.Read(null, new StructuresCalculation<StabilityPointStructuresInput>());

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("entity", paramName);
        }

        [Test]
        public void Read_WithNullStabilityPointStructuresCalculation_ThrowsArgumentNullException()
        {
            // Setup
            var collector = new ReadConversionCollector();

            // Call
            TestDelegate test = () => collector.Read(new StabilityPointStructuresCalculationEntity(), null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("model", paramName);
        }

        #endregion
    }
}