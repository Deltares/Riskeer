// Copyright (C) Stichting Deltares 2022. All rights reserved.
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
using System.Globalization;
using System.Linq;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.AssemblyTool.IO.Helpers;

namespace Riskeer.AssemblyTool.IO.Test.Helpers
{
    [TestFixture]
    public class GeometryGmlFormatHelperTest
    {
        [Test]
        public void Format_GeometryNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => GeometryGmlFormatHelper.Format(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("geometry", exception.ParamName);
        }

        [Test]
        public void Format_GeometryEmpty_ThrowsArgumentException()
        {
            // Call
            void Call() => GeometryGmlFormatHelper.Format(Enumerable.Empty<Point2D>());

            // Assert
            const string message = "Geometry cannot be empty.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(Call, message);
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
            string formattedPoint = GeometryGmlFormatHelper.Format(geometry);

            // Assert
            Assert.AreEqual(geometry.Select(point => point.X.ToString(CultureInfo.InvariantCulture) + " " + point.Y.ToString(CultureInfo.InvariantCulture))
                                    .Aggregate((p1, p2) => p1 + " " + p2),
                            formattedPoint);
        }
    }
}