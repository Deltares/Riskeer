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

using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.Read;

using Core.Common.Base.Geometry;

using NUnit.Framework;
using Ringtoets.Piping.KernelWrapper.TestUtil;

namespace Application.Ringtoets.Storage.Test.Read
{
    [TestFixture]
    public class ReadConversionCollectorTest
    {
        #region SoilProfileEntity: Read, Contains, Get

        [Test]
        public void Contains_WithoutEntity_ArgumentNullException()
        {
            // Setup
            var collector = new ReadConversionCollector();

            // Call
            TestDelegate test = () => collector.Contains((SoilProfileEntity)null);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("entity", paramName);
        }

        [Test]
        public void Contains_SoilProfileAdded_True()
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
        public void Contains_NoSoilProfileAdded_False()
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
        public void Contains_OtherSoilProfileEntityAdded_False()
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
        public void Get_WithoutSoilProfileEntity_ThrowArgumentNullException()
        {
            // Setup
            var collector = new ReadConversionCollector();

            // Call
            TestDelegate test = () => collector.Get((SoilProfileEntity)null);

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

        #region SurfaceLinePointEntity: Read, Contains, Get

        [Test]
        public void Contains_SurfaceLinePointsEntityIsNull_ThrowArgumentNullException()
        {
            // Setup
            var collector = new ReadConversionCollector();

            // Call
            TestDelegate call = () => collector.Contains((SurfaceLinePointEntity)null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("entity", paramName);
        }

        [Test]
        public void Contains_SurfaceLinePointEntityAdded_True()
        {
            // Setup
            var entity = new SurfaceLinePointEntity();
            var model = new Point3D(1.1, 2.2, 3.3);

            var collector = new ReadConversionCollector();
            collector.Read(entity, model);

            // Call
            var hasEntity = collector.Contains(entity);

            // Assert
            Assert.IsTrue(hasEntity);
        }

        [Test]
        public void Contains_SurfaceLinePointEntityNotAdded_False()
        {
            // Setup
            var entity = new SurfaceLinePointEntity();

            var collector = new ReadConversionCollector();

            // Call
            var hasEntity = collector.Contains(entity);

            // Assert
            Assert.IsFalse(hasEntity);
        }

        [Test]
        public void Contains_OtherSurfaceLinePointEntityAdded_False()
        {
            // Setup
            var registeredEntity = new SurfaceLinePointEntity();
            var model = new Point3D(1.1, 2.2, 3.3);

            var collector = new ReadConversionCollector();
            collector.Read(registeredEntity, model);

            var unregisteredEntity = new SurfaceLinePointEntity();

            // Call
            var hasEntity = collector.Contains(unregisteredEntity);

            // Assert
            Assert.IsFalse(hasEntity);
        }

        [Test]
        public void Get_SurfaceLinePointEntityIsNull_ThrowArgumentNullException()
        {
            // Setup
            var collector = new ReadConversionCollector();

            // Call
            TestDelegate call = () => collector.Get((SurfaceLinePointEntity)null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("entity", paramName);
        }

        [Test]
        public void Get_SurfaceLinePointEntityAdded_ReturnsPoint3D()
        {
            // Setup
            var entity = new SurfaceLinePointEntity();
            var model = new Point3D(1.1, 2.2, 3.3);

            var collector = new ReadConversionCollector();
            collector.Read(entity, model);

            // Call
            Point3D retrievedGeometryPoint = collector.Get(entity);

            // Assert
            Assert.AreSame(model, retrievedGeometryPoint);
        }

        [Test]
        public void Get_SurfaceLinePointEntityNotAdded_ThrowInvalidOperationException()
        {
            // Setup
            var entity = new SurfaceLinePointEntity();

            var collector = new ReadConversionCollector();

            // Call
            TestDelegate call = () => collector.Get(entity);

            // Assert
            Assert.Throws<InvalidOperationException>(call);
        }

        [Test]
        public void Get_DifferentSurfaceLinePointEntityAdded_ThrowsInvalidOperationException()
        {
            // Setup
            var registeredEntity = new SurfaceLinePointEntity();
            var model = new Point3D(1.1, 2.2, 3.3);

            var collector = new ReadConversionCollector();
            collector.Read(registeredEntity, model);

            var unregisteredEntity = new SurfaceLinePointEntity();

            // Call
            TestDelegate call = () => collector.Get(unregisteredEntity);

            // Assert
            Assert.Throws<InvalidOperationException>(call);
        }

        [Test]
        public void Read_SurfaceLinePointEntityIsNull_ThrowsArgumentNullException()
        {
            // Setup
            var collector = new ReadConversionCollector();

            // Call
            TestDelegate test = () => collector.Read(null, new Point3D(2.3, 4.4, 5.5));

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("entity", paramName);
        }

        [Test]
        public void Read_Point3DIsNull_ThrowsArgumentNullException()
        {
            // Setup
            var collector = new ReadConversionCollector();

            // Call
            TestDelegate test = () => collector.Read(new SurfaceLinePointEntity(), null);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("model", paramName);
        }

        #endregion
    }
}