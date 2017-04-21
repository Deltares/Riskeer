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
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.TestUtil;

namespace Ringtoets.Common.Data.Test
{
    [TestFixture]
    public class StructureBaseTest
    {
        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void Constructor_NameNullOrWhiteSpace_ThrowArgumentException(string name)
        {
            // Call
            TestDelegate call = () => new TestStructure(new StructureBase.ConstructionProperties
            {
                Name = name,
                Id = "anId",
                Location = new Point2D(0, 0),
                StructureNormalOrientation = (RoundedDouble) 0
            });

            // Assert
            var exception = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, "Name is null, empty or consists of whitespace.");
            Assert.AreEqual("constructionProperties", exception.ParamName);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void Constructor_IdNullOrWhiteSpace_ThrowArgumentException(string id)
        {
            // Call
            TestDelegate call = () => new TestStructure(new StructureBase.ConstructionProperties
            {
                Name = "aName",
                Id = id,
                Location = new Point2D(0, 0),
                StructureNormalOrientation = (RoundedDouble) 0
            });

            // Assert
            var exception = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, "Id is null, empty or consists of whitespace.");
            Assert.AreEqual("constructionProperties", exception.ParamName);
        }

        [Test]
        public void Constructor_LocationNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate call = () => new TestStructure(new StructureBase.ConstructionProperties
            {
                Name = "aName",
                Id = "anId",
                Location = null,
                StructureNormalOrientation = (RoundedDouble) 0
            });

            // Assert
            var exception = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(call, "Location is null.");
            Assert.AreEqual("constructionProperties", exception.ParamName);
        }

        [Test]
        public void Constructor_ValidData_ExpectedValues()
        {
            // Setup
            var location = new Point2D(1.22, 2.333);
            const double structureNormalOrientation = 0.0;

            // Call
            var structure = new TestStructure(new StructureBase.ConstructionProperties
            {
                Name = "aName",
                Id = "anId",
                Location = location,
                StructureNormalOrientation = (RoundedDouble) structureNormalOrientation
            });

            // Assert
            Assert.AreEqual("aName", structure.Name);
            Assert.AreEqual("anId", structure.Id);
            Assert.AreEqual(location.X, structure.Location.X);
            Assert.AreEqual(location.Y, structure.Location.Y);
            Assert.AreEqual("aName", structure.ToString());
            Assert.AreEqual(2, structure.StructureNormalOrientation.NumberOfDecimalPlaces);
            Assert.AreEqual(structureNormalOrientation, structure.StructureNormalOrientation,
                            structure.StructureNormalOrientation.GetAccuracy());
        }

        [Test]
        public void Constructor_DefaultValues_ExpectedValues()
        {
            // Setup
            var location = new Point2D(1.22, 2.333);

            // Call
            var structure = new TestStructure(new StructureBase.ConstructionProperties
            {
                Name = "aName",
                Id = "anId",
                Location = location
            });

            // Assert
            Assert.AreEqual("aName", structure.Name);
            Assert.AreEqual("anId", structure.Id);
            Assert.AreEqual(location.X, structure.Location.X);
            Assert.AreEqual(location.Y, structure.Location.Y);
            Assert.AreEqual("aName", structure.ToString());
            Assert.AreEqual(2, structure.StructureNormalOrientation.NumberOfDecimalPlaces);
            Assert.IsNaN(structure.StructureNormalOrientation);
        }

        private class TestStructure : StructureBase
        {
            public TestStructure(ConstructionProperties constructionProperties) : base(constructionProperties) {}
        }
    }
}