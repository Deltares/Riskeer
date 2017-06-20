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

using NUnit.Framework;
using Ringtoets.HydraRing.Calculation.Parsers.IllustrationPoints;

namespace Ringtoets.HydraRing.Calculation.Test.Parsers.IllustrationPoints
{
    [TestFixture]
    public class WindDirectionTest
    {
        [Test]
        public void Constructor_Always_ReturnsNewInstance()
        {
            // Call
            var direction = new WindDirection();

            // Assert
            Assert.IsNull(direction.Name);
            Assert.IsNaN(direction.Angle);
        }

        [Test]
        public void Equals_WithNull_ReturnsFalse()
        {
            // Setup
            var direction = new WindDirection();

            // Call
            bool result = direction.Equals(null);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Equals_DiffentType_ReturnsFalse()
        {
            // Setup
            var direction = new WindDirection();

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

            var generator = new WindDirection
            {
                Name = name,
                Angle = angle
            };
            var otherGenerator = new WindDirection
            {
                Name = name,
                Angle = angle
            };

            // Call
            int result = generator.GetHashCode();
            int otherResult = otherGenerator.GetHashCode();

            // Assert
            Assert.AreEqual(result, otherResult);
        }

        private static TestCaseData[] Combinations()
        {
            var windDirectionA = new WindDirection
            {
                Angle = 123.2, Name = "a"
            };
            var windDirectionB = new WindDirection
            {
                Angle = 123.2, Name = "a"
            };
            var windDirectionC = new WindDirection
            {
                Angle = 3.2, Name = "a"
            };

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
                new TestCaseData(windDirectionA, windDirectionC, false)
                {
                    TestName = "Equals_WindDirectionAWindDirectionC_False"
                }
            };
        }
    }
}