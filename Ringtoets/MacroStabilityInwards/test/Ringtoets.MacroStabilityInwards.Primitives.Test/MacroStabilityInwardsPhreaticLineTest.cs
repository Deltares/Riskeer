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
using Core.Common.Base.Geometry;
using NUnit.Framework;

namespace Ringtoets.MacroStabilityInwards.Primitives.Test
{
    public class MacroStabilityInwardsPhreaticLineTest
    {
        [Test]
        public void Constructor_NameNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new MacroStabilityInwardsPhreaticLine(null, new List<Point2D>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("name", exception.ParamName);
        }

        [Test]
        public void Constructor_GeometryNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new MacroStabilityInwardsPhreaticLine("name", null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("geometry", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            const string name = "<some name>";
            var geometry = new List<Point2D>
            {
                new Point2D(0.0, 0.0),
                new Point2D(15.0, 15.0)
            };

            // Call
            var phreaticLine = new MacroStabilityInwardsPhreaticLine(name, geometry);

            // Assert
            Assert.AreEqual(name, phreaticLine.Name);
            CollectionAssert.AreEqual(new List<Point2D>
            {
                new Point2D(0.0, 0.0),
                new Point2D(15.0, 15.0)
            }, phreaticLine.Geometry);
        }

        [Test]
        [TestCase(null)]
        [TestCase("string")]
        public void Equals_ToDifferentTypeOrNull_ReturnsFalse(object other)
        {
            // Setup
            MacroStabilityInwardsPhreaticLine phreaticLine = CreatePhreaticLine();

            // Call
            bool isEqualToDifferentObject = phreaticLine.Equals(other);

            // Assert
            Assert.IsFalse(isEqualToDifferentObject);
        }

        [Test]
        public void Equals_AllPropertiesEqual_ReturnsTrue()
        {
            // Setup
            MacroStabilityInwardsPhreaticLine phreaticLineX = CreatePhreaticLine();
            MacroStabilityInwardsPhreaticLine phreaticLineY = CreatePhreaticLine();

            // Call
            bool isXEqualToY = phreaticLineX.Equals(phreaticLineY);
            bool isYEqualToZ = phreaticLineY.Equals(phreaticLineX);

            // Assert
            Assert.IsTrue(isXEqualToY);
            Assert.IsTrue(isYEqualToZ);
        }

        [Test]
        [TestCaseSource(nameof(GetPhreaticLineCombinations))]
        public void Equals_DifferentProperty_ReturnsFalse(MacroStabilityInwardsPhreaticLine phreaticLine,
                                                          MacroStabilityInwardsPhreaticLine otherPhreaticLine)
        {
            // Call
            bool isStructureEqualToOther = phreaticLine.Equals(otherPhreaticLine);
            bool isOtherEqualToStructure = otherPhreaticLine.Equals(phreaticLine);

            // Assert
            Assert.IsFalse(isStructureEqualToOther);
            Assert.IsFalse(isOtherEqualToStructure);
        }

        [Test]
        public void GetHashCode_EqualStructures_ReturnsSameHashCode()
        {
            // Setup
            MacroStabilityInwardsPhreaticLine phreaticLineX = CreatePhreaticLine();
            MacroStabilityInwardsPhreaticLine phreaticLineY = CreatePhreaticLine();

            // Call
            int hashCodeOne = phreaticLineX.GetHashCode();
            int hashCodeTwo = phreaticLineY.GetHashCode();

            // Assert
            Assert.AreEqual(hashCodeOne, hashCodeTwo);
        }

        private static IEnumerable<TestCaseData> GetPhreaticLineCombinations()
        {
            yield return new TestCaseData(CreatePhreaticLine(),
                                          new MacroStabilityInwardsPhreaticLine("Other name", new[]
                                          {
                                              new Point2D(0, 0),
                                              new Point2D(1, 1)
                                          })).SetName("Other name");

            yield return new TestCaseData(CreatePhreaticLine(),
                                          new MacroStabilityInwardsPhreaticLine("Test", new[]
                                          {
                                              new Point2D(0, 0),
                                              new Point2D(1, 1),
                                              new Point2D(1, 1)
                                          })).SetName("Geometry not same length");

            yield return new TestCaseData(CreatePhreaticLine(),
                                          new MacroStabilityInwardsPhreaticLine("Test", new[]
                                          {
                                              new Point2D(0, 0),
                                              new Point2D(2, 2)
                                          })).SetName("Geometry not same coordinates");
        }

        private static MacroStabilityInwardsPhreaticLine CreatePhreaticLine()
        {
            return new MacroStabilityInwardsPhreaticLine(
                "Test",
                new[]
                {
                    new Point2D(0, 0),
                    new Point2D(1, 1)
                });
        }
    }
}