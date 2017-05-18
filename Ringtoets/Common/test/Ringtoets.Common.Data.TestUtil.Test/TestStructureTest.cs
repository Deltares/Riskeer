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
using Core.Common.Base.Data;
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
        public void IdConstructor_ValidParameters_ExpectedProperties()
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
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void IdConstructor_InvalidId_ThrowsArgumentException(string id)
        {
            // Call
            TestDelegate call = () => new TestStructure(id);

            // Assert
            Assert.Throws<ArgumentException>(call);
        }

        [Test]
        public void IdNameConstructor_ValidParameters_ExpectedProperties()
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
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void IdNameConstructor_InvalidName_ThrowsArgumentException(string name)
        {
            // Setup
            const string id = "some Id";

            // Call
            TestDelegate call = () => new TestStructure(id, name);

            // Assert
            Assert.Throws<ArgumentException>(call);
        }

        [Test]
        public void IdLocationConstructor_ValidParameters_ExpectedProperties()
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
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void IdLocationConstructor_InvalidId_ThrowsArgumentException(string id)
        {
            // Setup
            var location = new Point2D(1, 1);

            // Call
            TestDelegate call = () => new TestStructure(id, location);

            // Assert
            Assert.Throws<ArgumentException>(call);
        }

        [Test]
        public void IdLocationConstructor_LocationNull_ThrowsArgumentNullException()
        {
            // Setup
            const string id = "some Id";
            Point2D location = null;

            // Call
            TestDelegate call = () => new TestStructure(id, location);

            // Assert
            Assert.Throws<ArgumentNullException>(call);
        }

        [Test]
        public void Point2DConstructor_ValidParameters_ExpectedProperties()
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
        public void Point2DConstructor_LocationNull_ThrowsArgumentNullException()
        {
            // Setup
            Point2D location = null;

            // Call
            TestDelegate call = () => new TestStructure(location);

            // Assert
            Assert.Throws<ArgumentNullException>(call);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void IdNamePoint2DConstructor_InvalidId_ThrowsArgumentException(string id)
        {
            // Setup
            const string name = "some name";
            var location = new Point2D(1, 1);

            // Call
            TestDelegate call = () => new TestStructure(id, name, location);

            // Assert
            Assert.Throws<ArgumentException>(call);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void IdNamePoint2DConstructor_InvalidName_ThrowsArgumentException(string name)
        {
            // Setup
            const string id = "some id";
            var location = new Point2D(1, 1);

            // Call
            TestDelegate call = () => new TestStructure(id, name, location);

            // Assert
            Assert.Throws<ArgumentException>(call);
        }

        [Test]
        public void IdNamePoint2DConstructor_ValidParameters_ExpectedProperties()
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

        [Test]
        public void IdNamePoint2DConstructor_LocationNull_ThrowsArgumentNullException()
        {
            // Setup
            const string id = "some Id";
            const string name = "some name";
            Point2D location = null;

            // Call
            TestDelegate call = () => new TestStructure(id, name, location);

            // Assert
            Assert.Throws<ArgumentNullException>(call);
        }

        [Test]
        public void IdNamePoint2DNormalConstructor_ValidParameters_ExpectedProperties()
        {
            // Setup
            const string id = "some Id";
            const string name = "some name";
            var location = new Point2D(1, 1);
            var normal = (RoundedDouble) 456;

            // Call
            var structure = new TestStructure(id, name, location, normal);

            // Assert
            Assert.AreEqual(id, structure.Id);
            Assert.AreEqual(name, structure.Name);
            Assert.AreSame(location, structure.Location);
            Assert.AreEqual(normal, structure.StructureNormalOrientation,
                            structure.StructureNormalOrientation.GetAccuracy());
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void IdNamePoint2DNormalConstructor_InvalidId_ThrowsArgumentException(string id)
        {
            // Setup
            const string name = "some name";
            var location = new Point2D(1, 1);
            var normal = (RoundedDouble) 456;

            // Call
            TestDelegate call = () => new TestStructure(id, name, location, normal);

            // Assert
            Assert.Throws<ArgumentException>(call);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void IdNamePoint2DNormalConstructor_InvalidName_ThrowsArgumentException(string name)
        {
            // Setup
            const string id = "some Id";
            var location = new Point2D(1, 1);
            var normal = (RoundedDouble) 456;

            // Call
            TestDelegate call = () => new TestStructure(id, name, location, normal);

            // Assert
            Assert.Throws<ArgumentException>(call);
        }

        [Test]
        public void IdNamePoint2DNormalConstructor_LocationNull_ThrowsArgumentNullException()
        {
            // Setup
            const string id = "some Id";
            const string name = "some name";
            Point2D location = null;
            var normal = (RoundedDouble) 456;

            // Call
            TestDelegate call = () => new TestStructure(id, name, location, normal);

            // Assert
            Assert.Throws<ArgumentNullException>(call);
        }
    }
}