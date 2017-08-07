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
using NUnit.Framework;
using Ringtoets.HydraRing.Calculation.Data.Output.IllustrationPoints;

namespace Ringtoets.HydraRing.Calculation.Test.Data.Output.IllustrationPoints
{
    [TestFixture]
    public class WindDirectionTest
    {
        [Test]
        public void Constructor_NameNull_ThrowsArgumentNullException()
        {
            // Setup
            var random = new Random(21);
            double windDirectionAngle = random.NextDouble();

            // Call
            TestDelegate call = () => new WindDirection(null, windDirectionAngle);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("name", exception.ParamName);
        }

        [Test]
        public void Constructor_ValidParameters_ReturnsExpectedValues()
        {
            // Setup
            const string windDirectionName = "SSE";

            var random = new Random(21);
            double windDirectionAngle = random.NextDouble();

            // Call
            var direction = new WindDirection(windDirectionName, windDirectionAngle);

            // Assert
            Assert.AreEqual(windDirectionName, direction.Name);
            Assert.AreEqual(windDirectionAngle, direction.Angle);
        }

        [Test]
        public void Equals_WithNull_ReturnsFalse()
        {
            // Setup
            const string windDirectionName = "SSE";

            var random = new Random(21);
            double windDirectionAngle = random.NextDouble();
            var direction = new WindDirection(windDirectionName, windDirectionAngle);

            // Call
            bool result = direction.Equals(null);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Equals_DifferentType_ReturnsFalse()
        {
            // Setup
            const string windDirectionName = "SSE";

            var random = new Random(21);
            double windDirectionAngle = random.NextDouble();
            var direction = new WindDirection(windDirectionName, windDirectionAngle);

            // Call
            bool result = direction.Equals(new object());

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        [TestCaseSource(nameof(Combinations))]
        public void Equals_DifferentScenarios_ReturnExpectedValue(WindDirection windDirection,
                                                                  WindDirection otherWindDirection,
                                                                  bool expectedEqual)
        {
            // Call
            bool areEqualOne = windDirection.Equals(otherWindDirection);
            bool areEqualTwo = otherWindDirection.Equals(windDirection);

            // Assert
            Assert.AreEqual(expectedEqual, areEqualOne);
            Assert.AreEqual(expectedEqual, areEqualTwo);
        }

        [Test]
        public void GetHashCode_WindDirectionsAreEqual_FiltersHashesEqual()
        {
            // Setup
            const string name = "general";
            const double angle = 3.4;

            var generator = new WindDirection(name, angle);
            var otherGenerator = new WindDirection(name, angle);

            // Call
            int result = generator.GetHashCode();
            int otherResult = otherGenerator.GetHashCode();

            // Assert
            Assert.AreEqual(result, otherResult);
        }

        private static TestCaseData[] Combinations()
        {
            var windDirectionA = new WindDirection("a", 123.2);
            var windDirectionB = new WindDirection("a", 123.2);
            var windDirectionC = new WindDirection("a", 3.2);
            var windDirectionD = new WindDirection("d", 123.2);

            return new[]
            {
                new TestCaseData(windDirectionA, windDirectionA, true)
                {
                    TestName = "Equals_WindDirectionAWindDirectionA_True"
                },
                new TestCaseData(windDirectionA, windDirectionB, true)
                {
                    TestName = "Equals_WindDirectionAWindDirectionB_True"
                },
                new TestCaseData(windDirectionB, windDirectionC, false)
                {
                    TestName = "Equals_WindDirectionBWindDirectionC_False"
                },
                new TestCaseData(windDirectionC, windDirectionD, false)
                {
                    TestName = "Equals_WindDirectionAWindDirectionD_False"
                }
            };
        }
    }
}