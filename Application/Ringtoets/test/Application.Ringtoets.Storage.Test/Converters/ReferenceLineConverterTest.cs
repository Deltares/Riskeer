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
using System.Collections.Generic;
using System.Linq;
using Application.Ringtoets.Storage.Converters;
using Application.Ringtoets.Storage.DbContext;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.AssessmentSection;

namespace Application.Ringtoets.Storage.Test.Converters
{
    [TestFixture]
    public class ReferenceLineConverterTest
    {
        [Test]
        public void Constructor_Always_NewInstance()
        {
            // Call
            var converter = new ReferenceLineConverter();

            // Assert
            Assert.IsInstanceOf<IEntityConverter<ReferenceLine, ICollection<ReferenceLinePointEntity>>>(converter);
        }

        [Test]
        public void ConvertEntityToModel_NullEntity_ThrowsArgumentNullException()
        {
            // Setup
            var converter = new ReferenceLineConverter();

            // Call
            TestDelegate test = () => converter.ConvertEntityToModel(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("entityCollection", exception.ParamName);
        }

        [Test]
        public void ConvertEntityToModel_ValidOrderedEntityValidModel_ReturnsTheEntityAsModel()
        {
            // Setup
            var random = new Random(21);

            IList<Point2D> points = new []
            {
                new Point2D(random.NextDouble(), random.NextDouble()), 
                new Point2D(random.NextDouble(), random.NextDouble()), 
                new Point2D(random.NextDouble(), random.NextDouble()) 
            };
            var entityCollection = points.Select((point, i) => new ReferenceLinePointEntity
            {
                X = Convert.ToDecimal(point.X), 
                Y = Convert.ToDecimal(point.Y), 
                Order = i
            }).ToList();
            var converter = new ReferenceLineConverter();

            // Call
            ReferenceLine location = converter.ConvertEntityToModel(entityCollection);

            // Assert
            Assert.AreNotEqual(points, location.Points);
            for (var i = 0; i < entityCollection.Count; i++)
            {
                Assert.AreEqual(Decimal.ToDouble(entityCollection[i].X), points[i].X, 1e-6);
                Assert.AreEqual(Decimal.ToDouble(entityCollection[i].Y), points[i].Y, 1e-6);
            }
        }

        [Test]
        public void ConvertEntityToModel_ValidUnorderedEntityValidModel_ReturnsTheEntityAsModel()
        {
            // Setup
            var random = new Random(21);

            IList<Point2D> points = new []
            {
                new Point2D(random.NextDouble(), random.NextDouble()), 
                new Point2D(random.NextDouble(), random.NextDouble()), 
                new Point2D(random.NextDouble(), random.NextDouble()) 
            };
            var entityCollection = points.Select(p => new ReferenceLinePointEntity { X = Convert.ToDecimal(p.X), Y = Convert.ToDecimal(p.Y) }).ToList();

            entityCollection[0].Order = 1;
            entityCollection[1].Order = 2;
            entityCollection[2].Order = 0;

            var converter = new ReferenceLineConverter();

            // Call
            ReferenceLine location = converter.ConvertEntityToModel(entityCollection);

            // Assert
            Assert.AreNotEqual(points, location.Points);
            for (var i = 0; i < entityCollection.Count; i++)
            {
                Assert.AreEqual(Decimal.ToDouble(entityCollection[i].X), points[i].X, 1e-6);
                Assert.AreEqual(Decimal.ToDouble(entityCollection[i].Y), points[i].Y, 1e-6);
            }
        }

        [Test]
        public void ConvertModelToEntity_NullModel_ThrowsArgumentNullException()
        {
            // Setup
            var converter = new ReferenceLineConverter();

            // Call
            TestDelegate test = () => converter.ConvertModelToEntity(null,new List<ReferenceLinePointEntity>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("modelObject", exception.ParamName);
        }

        [Test]
        public void ConvertModelToEntity_NullEntity_ThrowsArgumentNullException()
        {
            // Setup
            var converter = new ReferenceLineConverter();

            // Call
            TestDelegate test = () => converter.ConvertModelToEntity(new ReferenceLine(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("entityCollection", exception.ParamName);
        }

        [Test]
        public void ConvertModelToEntity_ValidModelValidEntity_ReturnsModelAsEntity()
        {
            // Setup
            var converter = new ReferenceLineConverter();
            var random = new Random(21);
            var entity = new List<ReferenceLinePointEntity>();

            IList<Point2D> points = new []
            {
                new Point2D(random.NextDouble(), random.NextDouble()), 
                new Point2D(random.NextDouble(), random.NextDouble()), 
                new Point2D(random.NextDouble(), random.NextDouble()) 
            };
            var model = new ReferenceLine();
            model.SetGeometry(points);

            // Call
            converter.ConvertModelToEntity(model, entity);

            // Assert
            Assert.AreEqual(3, entity.Count);

            for (var i = 0; i < entity.Count; i++)
            {
                Assert.AreEqual(points[i].X, Decimal.ToDouble(entity[i].X), 1e-6);
                Assert.AreEqual(points[i].Y, Decimal.ToDouble(entity[i].Y), 1e-6);
                Assert.AreEqual(i, entity[i].Order);
            }
        }
    }
}
