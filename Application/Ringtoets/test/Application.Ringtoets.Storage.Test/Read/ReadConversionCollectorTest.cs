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
using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.Read;
using Application.Ringtoets.Storage.TestUtil;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.HydraRing.Data;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.KernelWrapper.TestUtil;
using Ringtoets.Piping.Primitives;

namespace Application.Ringtoets.Storage.Test.Read
{
    [TestFixture]
    public class ReadConversionCollectorTest
    {
        #region StochasticSoilProfileEntity: Read, Contains, Get

        [Test]
        public void Contains_WithoutStochasticSoilProfileEntity_ThrowsArgumentNullException()
        {
            // Setup
            var collector = new ReadConversionCollector();

            // Call
            TestDelegate test = () => collector.Contains((StochasticSoilProfileEntity) null);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("entity", paramName);
        }

        [Test]
        public void Contains_StochasticSoilProfileEntityAdded_ReturnsTrue()
        {
            // Setup
            var collector = new ReadConversionCollector();
            var entity = new StochasticSoilProfileEntity();
            collector.Read(entity, new StochasticSoilProfile(1, SoilProfileType.SoilProfile1D, 1));

            // Call
            var result = collector.Contains(entity);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void Contains_NoStochasticSoilProfileEntityAdded_ReturnsFalse()
        {
            // Setup
            var collector = new ReadConversionCollector();
            var entity = new StochasticSoilProfileEntity();

            // Call
            var result = collector.Contains(entity);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Contains_OtherStochasticSoilProfileEntityAdded_ReturnsFalse()
        {
            // Setup
            var collector = new ReadConversionCollector();
            var entity = new StochasticSoilProfileEntity();
            collector.Read(new StochasticSoilProfileEntity(), new StochasticSoilProfile(0.4, SoilProfileType.SoilProfile2D, 2));

            // Call
            var result = collector.Contains(entity);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Get_WithoutStochasticSoilProfileEntity_ThrowsArgumentNullException()
        {
            // Setup
            var collector = new ReadConversionCollector();

            // Call
            TestDelegate test = () => collector.Get((StochasticSoilProfileEntity) null);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("entity", paramName);
        }

        [Test]
        public void Get_StochasticSoilProfileEntityAdded_ReturnsReadStochasticSoilProfile()
        {
            // Setup
            var collector = new ReadConversionCollector();
            var profile = new StochasticSoilProfile(0.5, SoilProfileType.SoilProfile2D, 2);
            var entity = new StochasticSoilProfileEntity();
            collector.Read(entity, profile);

            // Call
            StochasticSoilProfile result = collector.Get(entity);

            // Assert
            Assert.AreSame(profile, result);
        }

        [Test]
        public void Get_NoStochasticSoilProfileEntityAdded_ThrowsInvalidOperationException()
        {
            // Setup
            var collector = new ReadConversionCollector();
            var entity = new StochasticSoilProfileEntity();

            // Call
            TestDelegate test = () => collector.Get(entity);

            // Assert
            Assert.Throws<InvalidOperationException>(test);
        }

        [Test]
        public void Get_OtherStochasticSoilProfileEntityAdded_ThrowsInvalidOperationException()
        {
            // Setup
            var collector = new ReadConversionCollector();
            var entity = new StochasticSoilProfileEntity();
            collector.Read(new StochasticSoilProfileEntity(), new StochasticSoilProfile(0.7, SoilProfileType.SoilProfile1D, 6));

            // Call
            TestDelegate test = () => collector.Get(entity);

            // Assert
            Assert.Throws<InvalidOperationException>(test);
        }

        [Test]
        public void Read_WithNullStochasticSoilProfileEntity_ThrowsArgumentNullException()
        {
            // Setup
            var collector = new ReadConversionCollector();

            // Call
            TestDelegate test = () => collector.Read(null, new StochasticSoilProfile(0.7, SoilProfileType.SoilProfile1D, 6));

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("entity", paramName);
        }

        [Test]
        public void Read_WithNullStochasticSoilProfile_ThrowsArgumentNullException()
        {
            // Setup
            var collector = new ReadConversionCollector();

            // Call
            TestDelegate test = () => collector.Read(new StochasticSoilProfileEntity(), null);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("model", paramName);
        }

        #endregion

        #region SoilProfileEntity: Read, Contains, Get

        [Test]
        public void Contains_WithoutEntity_ThrowsArgumentNullException()
        {
            // Setup
            var collector = new ReadConversionCollector();

            // Call
            TestDelegate test = () => collector.Contains((SoilProfileEntity) null);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("entity", paramName);
        }

        [Test]
        public void Contains_SoilProfileAdded_ReturnsTrue()
        {
            // Setup
            var collector = new ReadConversionCollector();
            var entity = new SoilProfileEntity();
            collector.Read(entity, new TestPipingSoilProfile());

            // Call
            var result = collector.Contains(entity);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void Contains_NoSoilProfileAdded_ReturnsFalse()
        {
            // Setup
            var collector = new ReadConversionCollector();
            var entity = new SoilProfileEntity();

            // Call
            var result = collector.Contains(entity);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Contains_OtherSoilProfileEntityAdded_ReturnsFalse()
        {
            // Setup
            var collector = new ReadConversionCollector();
            var entity = new SoilProfileEntity();
            collector.Read(new SoilProfileEntity(), new TestPipingSoilProfile());

            // Call
            var result = collector.Contains(entity);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Get_WithoutSoilProfileEntity_ThrowsArgumentNullException()
        {
            // Setup
            var collector = new ReadConversionCollector();

            // Call
            TestDelegate test = () => collector.Get((SoilProfileEntity) null);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("entity", paramName);
        }

        [Test]
        public void Get_SoilProfileAdded_ReturnsEntity()
        {
            // Setup
            var collector = new ReadConversionCollector();
            var profile = new TestPipingSoilProfile();
            var entity = new SoilProfileEntity();
            collector.Read(entity, profile);

            // Call
            var result = collector.Get(entity);

            // Assert
            Assert.AreSame(profile, result);
        }

        [Test]
        public void Get_NoSoilProfileAdded_ThrowsInvalidOperationException()
        {
            // Setup
            var collector = new ReadConversionCollector();
            var entity = new SoilProfileEntity();

            // Call
            TestDelegate test = () => collector.Get(entity);

            // Assert
            Assert.Throws<InvalidOperationException>(test);
        }

        [Test]
        public void Get_OtherSoilProfileAdded_ThrowsInvalidOperationException()
        {
            // Setup
            var collector = new ReadConversionCollector();
            var entity = new SoilProfileEntity();
            collector.Read(new SoilProfileEntity(), new TestPipingSoilProfile());

            // Call
            TestDelegate test = () => collector.Get(entity);

            // Assert
            Assert.Throws<InvalidOperationException>(test);
        }

        [Test]
        public void Read_WithNullSoilProfileEntity_ThrowsArgumentNullException()
        {
            // Setup
            var collector = new ReadConversionCollector();

            // Call
            TestDelegate test = () => collector.Read(null, new TestPipingSoilProfile());

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("entity", paramName);
        }

        [Test]
        public void Read_WithNullProfileEntity_ThrowsArgumentNullException()
        {
            // Setup
            var collector = new ReadConversionCollector();

            // Call
            TestDelegate test = () => collector.Read(new SoilProfileEntity(), null);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("model", paramName);
        }

        #endregion

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
            var model = new RingtoetsPipingSurfaceLine();

            var collector = new ReadConversionCollector();
            collector.Read(entity, model);

            // Call
            var hasEntity = collector.Contains(entity);

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
            var hasEntity = collector.Contains(entity);

            // Assert
            Assert.IsFalse(hasEntity);
        }

        [Test]
        public void Contains_OtherSurfaceLineEntityAdded_ReturnsFalse()
        {
            // Setup
            var registeredEntity = new SurfaceLineEntity();
            var model = new RingtoetsPipingSurfaceLine();

            var collector = new ReadConversionCollector();
            collector.Read(registeredEntity, model);

            var unregisteredEntity = new SurfaceLineEntity();

            // Call
            var hasEntity = collector.Contains(unregisteredEntity);

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
        public void Get_SurfaceLineEntityAdded_ReturnsRingtoetsPipingSurfaceLine()
        {
            // Setup
            var entity = new SurfaceLineEntity();
            var model = new RingtoetsPipingSurfaceLine();

            var collector = new ReadConversionCollector();
            collector.Read(entity, model);

            // Call
            RingtoetsPipingSurfaceLine retrievedGeometryPoint = collector.Get(entity);

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
            var model = new RingtoetsPipingSurfaceLine();

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
            TestDelegate test = () => collector.Read(null, new RingtoetsPipingSurfaceLine());

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("entity", paramName);
        }

        [Test]
        public void Read_RingtoetsPipingSurfaceLineIsNull_ThrowsArgumentNullException()
        {
            // Setup
            var collector = new ReadConversionCollector();

            // Call
            TestDelegate test = () => collector.Read(new SurfaceLineEntity(), null);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
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
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
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
            var result = collector.Contains(entity);

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
            var result = collector.Contains(entity);

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
            var result = collector.Contains(entity);

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
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
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
            var result = collector.Get(entity);

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
            TestDelegate test = () => collector.Read(null, new HydraulicBoundaryLocation(1, "A", 1, 1));

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
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
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
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
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
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
            var result = collector.Contains(entity);

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
            var result = collector.Contains(entity);

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
            var result = collector.Contains(entity);

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
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
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
            var result = collector.Get(entity);

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
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
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
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
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
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("entity", paramName);
        }

        [Test]
        public void Contains_DikeProfileEntityAdded_ReturnsTrue()
        {
            // Setup
            var collector = new ReadConversionCollector();
            var entity = new DikeProfileEntity();
            collector.Read(entity, CreateDikeProfile());

            // Call
            var result = collector.Contains(entity);

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
            var result = collector.Contains(entity);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Contains_OtherDikeProfileEntityAdded_ReturnsFalse()
        {
            // Setup
            var collector = new ReadConversionCollector();
            var entity = new DikeProfileEntity();
            collector.Read(new DikeProfileEntity(), CreateDikeProfile());

            // Call
            var result = collector.Contains(entity);

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
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("entity", paramName);
        }

        [Test]
        public void Get_DikeProfileEntityAdded_ReturnsDikeProfile()
        {
            // Setup
            var collector = new ReadConversionCollector();
            DikeProfile dikeProfile = CreateDikeProfile();
            var entity = new DikeProfileEntity();
            collector.Read(entity, dikeProfile);

            // Call
            DikeProfile result = collector.Get(entity);

            // Assert
            Assert.AreSame(dikeProfile, result);
        }

        private static DikeProfile CreateDikeProfile()
        {
            return new DikeProfile(new Point2D(0, 0), new RoughnessPoint[0], new Point2D[0],
                                   null, new DikeProfile.ConstructionProperties());
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
            collector.Read(new DikeProfileEntity(), CreateDikeProfile());

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
            TestDelegate test = () => collector.Read(null, CreateDikeProfile());

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
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
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
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
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
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
            var result = collector.Contains(entity);

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
            var result = collector.Contains(entity);

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
            var result = collector.Contains(entity);

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
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
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
            var result = collector.Get(entity);

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
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
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
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("model", paramName);
        }

        #endregion
    }
}