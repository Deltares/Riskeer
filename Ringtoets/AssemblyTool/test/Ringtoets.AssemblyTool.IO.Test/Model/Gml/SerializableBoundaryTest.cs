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
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Ringtoets.AssemblyTool.IO.Model.Gml;
using Ringtoets.AssemblyTool.IO.TestUtil;

namespace Ringtoets.AssemblyTool.IO.Test.Model.Gml
{
    public class SerializableBoundaryTest
    {
        [Test]
        public void DefaultConstructor_ReturnsDefaultValues()
        {
            // Call
            var boundary = new SerializableBoundary();

            // Assert
            Assert.IsNull(boundary.Envelope);

            SerializableAttributeTestHelper.AssertXmlElementAttribute<SerializableBoundary>(
                nameof(SerializableBoundary.Envelope), "Envelope");
        }

        [Test]
        public void Constructor_LowerCornerNull_ThrowsArgumentNullException()
        {
            // Setup
            var random = new Random(39);

            // Call
            TestDelegate call = () => new SerializableBoundary(null, new Point2D(random.NextDouble(), random.NextDouble()));

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("lowerCorner", exception.ParamName);
        }

        [Test]
        public void Constructor_UpperCornerNull_ThrowsArgumentNullException()
        {
            // Setup
            var random = new Random(39);

            // Call
            TestDelegate call = () => new SerializableBoundary(new Point2D(random.NextDouble(), random.NextDouble()), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("upperCorner", exception.ParamName);
        }

        [Test]
        public void Constructor_WithCorners_ReturnsExpectedValues()
        {
            // Setup
            var random = new Random(39);
            var lowerCorner = new Point2D(random.NextDouble(), random.NextDouble());
            var upperCorner = new Point2D(random.NextDouble(), random.NextDouble());

            // Call
            var boundary = new SerializableBoundary(lowerCorner, upperCorner);

            // Assert
            Assert.AreEqual(lowerCorner.X + " " + lowerCorner.Y, boundary.Envelope.LowerCorner);
            Assert.AreEqual(upperCorner.X + " " + upperCorner.Y, boundary.Envelope.UpperCorner);
        }
    }
}