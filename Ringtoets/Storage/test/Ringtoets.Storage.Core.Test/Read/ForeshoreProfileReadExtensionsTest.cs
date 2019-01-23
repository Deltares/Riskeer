// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.TestUtil;
using Riskeer.Storage.Core.DbContext;
using Riskeer.Storage.Core.Read;
using Riskeer.Storage.Core.Serializers;

namespace Riskeer.Storage.Core.Test.Read
{
    [TestFixture]
    public class ForeshoreProfileReadExtensionsTest
    {
        [Test]
        public void Read_CollectorNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new ForeshoreProfileEntity().Read(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("collector", paramName);
        }

        [Test]
        public void Read_EntityNotReadBefore_EntityRegistered()
        {
            // Setup
            var collector = new ReadConversionCollector();

            var entity = new ForeshoreProfileEntity
            {
                Id = "id",
                GeometryXml = new Point2DCollectionXmlSerializer().ToXml(new Point2D[0])
            };

            // Precondition
            Assert.IsFalse(collector.Contains(entity));

            // Call
            entity.Read(collector);

            // Assert
            Assert.IsTrue(collector.Contains(entity));
        }

        [Test]
        [TestCase("")]
        [TestCase(null)]
        public void Read_GeometryXmlNullOrEmpty_ThrowsArgumentException(string xml)
        {
            // Setup
            var entity = new ForeshoreProfileEntity
            {
                GeometryXml = xml
            };

            // Call
            TestDelegate test = () => entity.Read(new ReadConversionCollector());

            // Assert
            string paramName = Assert.Throws<ArgumentException>(test).ParamName;
            Assert.AreEqual("xml", paramName);
        }

        [Test]
        public void Read_EmptyGeometryBreakWaterTypeAndNullableValuesAreNull_ForeshoreProfileWithoutBreakWaterNaNValues()
        {
            // Setup
            const string name = "testName";
            const string id = "testId";
            string pointXml = new Point2DCollectionXmlSerializer().ToXml(Enumerable.Empty<Point2D>());
            var entity = new ForeshoreProfileEntity
            {
                Id = id,
                Name = name,
                GeometryXml = pointXml
            };

            var readConversionCollector = new ReadConversionCollector();

            // Call
            ForeshoreProfile foreshoreProfile = entity.Read(readConversionCollector);

            // Assert
            Assert.IsNotNull(foreshoreProfile);
            Assert.AreEqual(id, foreshoreProfile.Id);
            Assert.AreEqual(name, foreshoreProfile.Name);
            Assert.IsNaN(foreshoreProfile.Orientation);
            Assert.IsNaN(foreshoreProfile.X0);
            Assert.IsNull(foreshoreProfile.BreakWater);
            Assert.IsFalse(foreshoreProfile.HasBreakWater);
            CollectionAssert.IsEmpty(foreshoreProfile.Geometry);
        }

        [Test]
        public void Read_WithGeometryAndBreakWaterTypeAndValues_CompleteForeshoreProfile()
        {
            // Setup
            const string name = "testName";
            const string id = "testId";
            var random = new Random(21);
            int order = random.Next();
            double orientation = random.NextDouble();
            double x0 = random.NextDouble();
            double height = random.NextDouble();
            const BreakWaterType breakWaterType = BreakWaterType.Wall;

            var points = new[]
            {
                new Point2D(0, 0)
            };
            string pointXml = new Point2DCollectionXmlSerializer().ToXml(points);
            var entity = new ForeshoreProfileEntity
            {
                Order = order,
                Id = id,
                Name = name,
                Orientation = orientation,
                X0 = x0,
                BreakWaterType = Convert.ToByte(breakWaterType),
                BreakWaterHeight = height,
                GeometryXml = pointXml
            };

            var readConversionCollector = new ReadConversionCollector();

            // Call
            ForeshoreProfile foreshoreProfile = entity.Read(readConversionCollector);

            // Assert
            Assert.IsNotNull(foreshoreProfile);
            Assert.AreEqual(name, foreshoreProfile.Name);
            Assert.AreEqual(order, entity.Order);
            Assert.AreEqual(id, foreshoreProfile.Id);
            Assert.AreEqual(name, entity.Name);
            Assert.AreEqual(orientation, foreshoreProfile.Orientation, foreshoreProfile.Orientation.GetAccuracy());
            Assert.AreEqual(x0, entity.X0);
            Assert.AreEqual(breakWaterType, foreshoreProfile.BreakWater.Type);
            Assert.IsTrue(foreshoreProfile.HasBreakWater);
            CollectionAssert.AreEqual(points, foreshoreProfile.Geometry);
        }
    }
}