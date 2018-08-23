// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using System.Globalization;
using System.Linq;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Ringtoets.AssemblyTool.IO.Model.Helpers;

namespace Ringtoets.AssemblyTool.IO.Test.Model.Helpers
{
    [TestFixture]
    public class GeometrySerializationFormatterTest
    {
        [Test]
        public void Format_GeometryNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => GeometrySerializationFormatter.Format((IEnumerable<Point2D>) null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("geometry", exception.ParamName);
        }

        [Test]
        public void Format_GeometryEmpty_ThrowsArgumentException()
        {
            // Call
            TestDelegate call = () => GeometrySerializationFormatter.Format(Enumerable.Empty<Point2D>());

            // Assert
            var exception = Assert.Throws<ArgumentException>(call);
            Assert.AreEqual("geometry", exception.ParamName);
            Assert.AreEqual("Geometry cannot be empty.", exception.Message);
        }

        [Test]
        public void Format_WithGeometry_ReturnsFormattedString()
        {
            // Setup
            var random = new Random(39);
            var geometry = new[]
            {
                new Point2D(random.NextDouble(), random.NextDouble()),
                new Point2D(random.NextDouble(), random.NextDouble())
            };

            // Call
            string formattedPoint = GeometrySerializationFormatter.Format(geometry);

            // Assert
            Assert.AreEqual(geometry.Select(point => point.X.ToString(CultureInfo.InvariantCulture) + " " + point.Y.ToString(CultureInfo.InvariantCulture))
                                    .Aggregate((p1, p2) => p1 + " " + p2),
                            formattedPoint);
        }

        [Test]
        public void Format_PointNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => GeometrySerializationFormatter.Format((Point2D) null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("point", exception.ParamName);
        }

        [Test]
        public void Format_WithPoint_ReturnsFormattedString()
        {
            // Setup
            var random = new Random(39);
            var point = new Point2D(random.NextDouble(), random.NextDouble());

            // Call
            string formattedPoint = GeometrySerializationFormatter.Format(point);

            // Assert
            Assert.AreEqual(point.X.ToString(CultureInfo.InvariantCulture) + " " + point.Y.ToString(CultureInfo.InvariantCulture),
                            formattedPoint);
        }
    }
}