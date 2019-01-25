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
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Riskeer.AssemblyTool.IO.Model.Gml;
using Riskeer.AssemblyTool.IO.Model.Helpers;
using Riskeer.AssemblyTool.IO.TestUtil;

namespace Riskeer.AssemblyTool.IO.Test.Model.Gml
{
    [TestFixture]
    public class SerializableLineStringTest
    {
        [Test]
        public void DefaultConstructor_ReturnsDefaultValues()
        {
            // Call
            var lineString = new SerializableLineString();

            // Assert
            Assert.AreEqual("EPSG:28992", lineString.CoordinateSystem);
            Assert.IsNull(lineString.Geometry);

            SerializableAttributeTestHelper.AssertXmlAttributeAttribute<SerializableLineString>(
                nameof(SerializableLineString.CoordinateSystem), "srsName");

            SerializableAttributeTestHelper.AssertXmlElementAttribute<SerializableLineString>(
                nameof(SerializableLineString.Geometry), "posList");
        }

        [Test]
        public void Constructor_GeometryNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new SerializableLineString(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("geometry", exception.ParamName);
        }

        [Test]
        public void Constructor_WithValidData_ReturnsExpectedValues()
        {
            // Setup
            var random = new Random(39);
            var geometry = new[]
            {
                new Point2D(random.NextDouble(), random.NextDouble()),
                new Point2D(random.NextDouble(), random.NextDouble())
            };

            // Call
            var lineString = new SerializableLineString(geometry);

            // Assert
            Assert.AreEqual("EPSG:28992", lineString.CoordinateSystem);
            Assert.AreEqual(GeometrySerializationFormatter.Format(geometry), lineString.Geometry);
        }
    }
}