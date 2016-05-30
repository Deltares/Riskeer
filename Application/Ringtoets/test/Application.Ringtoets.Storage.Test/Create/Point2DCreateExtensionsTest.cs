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

using Core.Common.Base.Geometry;
using NUnit.Framework;

namespace Application.Ringtoets.Storage.Test.Create
{
    [TestFixture]
    public class Point2DCreateExtensionsTest
    {
        [Test]
        [TestCase(0)]
        [TestCase(100)]
        [TestCase(Int32.MaxValue)]
        [TestCase(Int32.MinValue)]
        public void CreateReferenceLinePointEntity_Always_NewReferenceLinePointEntityWithPropertiesSet(int order)
        {
            // Setup
            var random = new Random(21);
            double x = random.NextDouble();
            double y = random.NextDouble();
            var point = new Point2D(x, y);

            // Call
            var entity = point.CreateReferenceLinePointEntity(order);

            // Assert
            Assert.AreEqual(Convert.ToDecimal(x), entity.X);
            Assert.AreEqual(Convert.ToDecimal(y), entity.Y);
            Assert.AreEqual(order, entity.Order);
        }

        [Test]
        [TestCase(0)]
        [TestCase(100)]
        [TestCase(Int32.MaxValue)]
        [TestCase(Int32.MinValue)]
        public void CreateFailureMechanismSectionPointEntity_Always_NewFailureMechanismSectionPointEntityWithPropertiesSet(int order)
        {
            // Setup
            var random = new Random(21);
            double x = random.NextDouble();
            double y = random.NextDouble();
            var point = new Point2D(x, y);

            // Call
            var entity = point.CreateFailureMechanismSectionPointEntity(order);

            // Assert
            Assert.AreEqual(Convert.ToDecimal(x), entity.X);
            Assert.AreEqual(Convert.ToDecimal(y), entity.Y);
            Assert.AreEqual(order, entity.Order);
        }

        [Test]
        [TestCase(0)]
        [TestCase(100)]
        [TestCase(Int32.MaxValue)]
        [TestCase(Int32.MinValue)]
        public void CreateStochasticSoilModelSegmentPointEntity_Always_NewStochasticSoilModelSegmentPointEntityWithPropertiesSet(int order)
        {
            // Setup
            var random = new Random(21);
            double x = random.NextDouble();
            double y = random.NextDouble();
            var point = new Point2D(x, y);

            // Call
            StochasticSoilModelSegmentPointEntity entity = point.CreateStochasticSoilModelSegmentPointEntity(order);

            // Assert
            Assert.AreEqual(Convert.ToDecimal(x), entity.X);
            Assert.AreEqual(Convert.ToDecimal(y), entity.Y);
            Assert.AreEqual(order, entity.Order);

            Assert.AreEqual(0, entity.StochasticSoilModelSegmentPointEntityId);
            Assert.AreEqual(0, entity.StochasticSoilModelEntityId);
        }
    }
}