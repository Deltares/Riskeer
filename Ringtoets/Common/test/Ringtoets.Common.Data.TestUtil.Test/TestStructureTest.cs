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

using Core.Common.Base.Geometry;
using NUnit.Framework;

namespace Ringtoets.Common.Data.TestUtil.Test
{
    [TestFixture]
    public class TestStructureTest
    {
        [Test]
        public void Constructor_ExpectedProperties()
        {
            // Call
            var structure = new TestStructure();

            // Assert
            Assert.AreEqual("id", structure.Id);
            Assert.AreEqual("name", structure.Name);
            Assert.AreEqual(new Point2D(0.0, 0.0), structure.Location);
            Assert.AreEqual(0.12345, structure.StructureNormalOrientation,
                            structure.StructureNormalOrientation.GetAccuracy());
        }

        [Test]
        public void IdConstructor_ExpectedProperties()
        {
            // Setup
            const string id = "some Id";

            // Call
            var structure = new TestStructure(id);

            // Assert
            Assert.AreEqual(id, structure.Id);
            Assert.AreEqual("name", structure.Name);
            Assert.AreEqual(new Point2D(0.0, 0.0), structure.Location);
            Assert.AreEqual(0.12345, structure.StructureNormalOrientation,
                            structure.StructureNormalOrientation.GetAccuracy());
        }

        [Test]
        public void IdNameConstructor_ExpectedProperties()
        {
            // Setup
            const string id = "some Id";
            const string name = "some name";

            // Call
            var structure = new TestStructure(id, name);

            // Assert
            Assert.AreEqual(id, structure.Id);
            Assert.AreEqual(name, structure.Name);
            Assert.AreEqual(new Point2D(0.0, 0.0), structure.Location);
            Assert.AreEqual(0.12345, structure.StructureNormalOrientation,
                            structure.StructureNormalOrientation.GetAccuracy());
        }

        [Test]
        public void IdLocationConstructor_ExpectedProperties()
        {
            // Setup
            const string id = "some Id";
            var location = new Point2D(1, 1);

            // Call
            var structure = new TestStructure(id, location);

            // Assert
            Assert.AreEqual(id, structure.Id);
            Assert.AreEqual("name", structure.Name);
            Assert.AreSame(location, structure.Location);
            Assert.AreEqual(0.12345, structure.StructureNormalOrientation,
                            structure.StructureNormalOrientation.GetAccuracy());
        }

        [Test]
        public void Point2DConstructor_ExpectedProperties()
        {
            // Setup
            var location = new Point2D(1, 1);

            // Call
            var structure = new TestStructure(location);

            // Assert
            Assert.AreEqual("id", structure.Id);
            Assert.AreEqual("name", structure.Name);
            Assert.AreSame(location, structure.Location);
            Assert.AreEqual(0.12345, structure.StructureNormalOrientation,
                            structure.StructureNormalOrientation.GetAccuracy());
        }

        [Test]
        public void IdNamePoint2DConstructor_ExpectedProperties()
        {
            // Setup
            const string id = "some Id";
            const string name = "some name";
            var location = new Point2D(1, 1);

            // Call
            var structure = new TestStructure(id, name, location);

            // Assert
            Assert.AreEqual(id, structure.Id);
            Assert.AreEqual(name, structure.Name);
            Assert.AreSame(location, structure.Location);
            Assert.AreEqual(0.12345, structure.StructureNormalOrientation,
                            structure.StructureNormalOrientation.GetAccuracy());
        }
    }
}