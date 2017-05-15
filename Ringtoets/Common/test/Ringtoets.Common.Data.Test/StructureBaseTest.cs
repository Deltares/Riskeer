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
        private static IEnumerable<TestCaseData> StructureCombinations
        {
            get
            {
                StructureBase structure = CreateFullyDefinedStructure();
                yield return new TestCaseData(structure, structure, true)
                    .SetName("SameStructure");
                yield return new TestCaseData(structure, CreateFullyDefinedStructure(), true)
                    .SetName("EqualStructure");

                StructureBase.ConstructionProperties differentId = CreateFullyConfiguredConstructionProperties();
                differentId.Id = "differentId";
                yield return new TestCaseData(structure, new TestStructure(differentId), false)
                    .SetName(nameof(differentId));

                StructureBase.ConstructionProperties differentName = CreateFullyConfiguredConstructionProperties();
                differentName.Name = "differentName";
                yield return new TestCaseData(structure, new TestStructure(differentName), false)
                    .SetName(nameof(differentName));

                StructureBase.ConstructionProperties differentLocation = CreateFullyConfiguredConstructionProperties();
                differentLocation.Location = new Point2D(9, 9);
                yield return new TestCaseData(structure, new TestStructure(differentLocation), false)
                    .SetName(nameof(differentLocation));

                StructureBase.ConstructionProperties differentOrientation = CreateFullyConfiguredConstructionProperties();
                differentOrientation.StructureNormalOrientation = (RoundedDouble) 90;
                yield return new TestCaseData(structure, new TestStructure(differentOrientation), false)
                    .SetName(nameof(differentOrientation));
            }
        }

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

        [Test]
        public void CopyProperties_FromStructureNull_ThrowsArgumentNullException()
        {
            // Setup
            var structure = new TestStructure(new StructureBase.ConstructionProperties
            {
                Name = "aName",
                Id = "anId",
                Location = new Point2D(0, 0)
            });

            // Call
            TestDelegate call = () => structure.CopyProperties(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("fromStructure", paramName);
        }

        [Test]
        public void CopyProperties_FromStructure_UpdatesProperties()
        {
            // Setup
            var structure = new TestStructure(new StructureBase.ConstructionProperties
            {
                Name = "aName",
                Id = "anId",
                Location = new Point2D(0, 0),
                StructureNormalOrientation = RoundedDouble.NaN
            });

            var otherStructure = new TestStructure(new StructureBase.ConstructionProperties
            {
                Name = "otherName",
                Id = "otherId",
                Location = new Point2D(1, 1),
                StructureNormalOrientation = (RoundedDouble) 89
            });

            // Call
            structure.CopyProperties(otherStructure);

            // Assert
            Assert.AreNotEqual(otherStructure.Id, structure.Id);
            Assert.AreEqual(otherStructure.Name, structure.Name);
            Assert.AreEqual(otherStructure.Location, structure.Location);
            Assert.AreEqual(otherStructure.StructureNormalOrientation, structure.StructureNormalOrientation);

            Assert.AreNotSame(otherStructure.Location, structure.Location);
            Assert.AreNotSame(otherStructure.StructureNormalOrientation, structure.StructureNormalOrientation);
        }

        [Test]
        [TestCase(null)]
        [TestCase("string")]
        public void Equals_ToDifferentTypeOrNull_ReturnsFalse(object other)
        {
            // Setup
            StructureBase structure = CreateFullyDefinedStructure();

            // Call
            bool isEqualToDifferentObject = structure.Equals(other);

            // Assert
            Assert.IsFalse(isEqualToDifferentObject);
        }

        [Test]
        public void Equals_TransitivePropertyAllPropertiesEqual_ReturnsTrue()
        {
            // Setup
            StructureBase structureX = CreateFullyDefinedStructure();
            StructureBase structureY = CreateFullyDefinedStructure();
            StructureBase structureZ = CreateFullyDefinedStructure();

            // Call
            bool isXEqualToY = structureX.Equals(structureY);
            bool isYEqualToZ = structureY.Equals(structureZ);
            bool isXEqualToZ = structureX.Equals(structureZ);

            // Assert
            Assert.IsTrue(isXEqualToY);
            Assert.IsTrue(isYEqualToZ);
            Assert.IsTrue(isXEqualToZ);
        }

        [Test]
        [TestCaseSource(nameof(StructureCombinations))]
        public void Equal_DifferentProperty_ReturnsIsEqual(StructureBase structure,
                                                           StructureBase otherStructure,
                                                           bool expectedToBeEqual)
        {
            // Call
            bool isStructureEqualToOther = structure.Equals(otherStructure);
            bool isOtherEqualToStructure = otherStructure.Equals(structure);

            // Assert
            Assert.AreEqual(expectedToBeEqual, isStructureEqualToOther);
            Assert.AreEqual(expectedToBeEqual, isOtherEqualToStructure);
        }

        [Test]
        public void GetHashCode_EqualStructures_ReturnsSameHashCode()
        {
            // Setup
            StructureBase structureOne = CreateFullyDefinedStructure();
            StructureBase structureTwo = CreateFullyDefinedStructure();

            // Call
            int hashCodeOne = structureOne.GetHashCode();
            int hashCodeTwo = structureTwo.GetHashCode();

            // Assert
            Assert.AreEqual(hashCodeOne, hashCodeTwo);
        }

        private static StructureBase CreateFullyDefinedStructure()
        {
            return new TestStructure(CreateFullyConfiguredConstructionProperties());
        }

        private static StructureBase.ConstructionProperties CreateFullyConfiguredConstructionProperties()
        {
            const string id = "structure id";
            const string name = "Structure name";
            return new StructureBase.ConstructionProperties
            {
                Id = id,
                Name = name,
                Location = new Point2D(1, 1),
                StructureNormalOrientation = (RoundedDouble) 25
            };
        }

        private class TestStructure : StructureBase
        {
            public TestStructure(ConstructionProperties constructionProperties) : base(constructionProperties) {}

            public void CopyProperties(TestStructure fromStructure)
            {
                base.CopyProperties(fromStructure);
            }
        }
    }
}