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
using Ringtoets.HydraRing.Calculation.Parsers.IllustrationPoints;

namespace Ringtoets.HydraRing.Calculation.Test.Parsers.IllustrationPoints
{
    [TestFixture]
    public class WindDirectionClosingSituationTest
    {
        [Test]
        public void Constructor_WithoutWindDirection_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new WindDirectionClosingSituation(null, string.Empty);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("windDirection", exception.ParamName);
        }

        [Test]
        public void Constructor_WithoutClosingSituation_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new WindDirectionClosingSituation(new WindDirection(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("closingSituation", exception.ParamName);
        }

        [Test]
        public void Constructor_WithParameters_ReturnNewInstance()
        {
            // Setup
            string closingSituation = string.Empty;
            var windDirection = new WindDirection();

            // Call
            var instance = new WindDirectionClosingSituation(windDirection, closingSituation);

            // Assert
            Assert.NotNull(instance);
            Assert.AreEqual(closingSituation, instance.ClosingSituation);
            Assert.AreSame(windDirection, instance.WindDirection);
        }

        [Test]
        public void Equals_WithNull_ReturnsFalse()
        {
            // Setup
            var instance = new WindDirectionClosingSituation(new WindDirection(), string.Empty);

            // Call
            bool result = instance.Equals(null);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Equals_DiffentType_ReturnsFalse()
        {
            // Setup
            var instance = new WindDirectionClosingSituation(new WindDirection(), string.Empty);

            // Call
            bool result = instance.Equals(new object());

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        [TestCaseSource(nameof(Combinations))]
        public void Equals_DifferentScenarios_ReturnExpectedValue(WindDirectionClosingSituation instance,
                                                                  WindDirectionClosingSituation otherInstance,
                                                                  bool expectedEqual)
        {
            // Call
            bool areEqualOne = instance.Equals(otherInstance);
            bool areEqualTwo = otherInstance.Equals(instance);

            // Assert
            Assert.AreEqual(expectedEqual, areEqualOne);
            Assert.AreEqual(expectedEqual, areEqualTwo);
        }

        [Test]
        public void GetHashCode_InstancesAreEqual_FiltersHashesEqual()
        {
            // Setup
            var windDirection = new WindDirection();
            const string closingSituation = "general";

            var instance = new WindDirectionClosingSituation(windDirection, closingSituation);
            var otherInstance = new WindDirectionClosingSituation(windDirection, closingSituation);

            // Call
            int result = instance.GetHashCode();
            int otherResult = otherInstance.GetHashCode();

            // Assert
            Assert.AreEqual(result, otherResult);
        }

        private static TestCaseData[] Combinations()
        {
            var instanceA = new WindDirectionClosingSituation(new WindDirection { Angle = 123.2, Name = "a" }, "situationA");
            var instanceB = new WindDirectionClosingSituation(new WindDirection { Angle = 123.2, Name = "a" }, "situationA");
            var instanceC = new WindDirectionClosingSituation(new WindDirection { Angle = 3.2, Name = "a" }, "situationA");
            var instanceD = new WindDirectionClosingSituation(new WindDirection { Angle = 123.2, Name = "a" }, "situationB");

            return new[]
            {
                new TestCaseData(instanceA, instanceA, true)
                {
                    TestName = "Equals_InstanceAInstanceA_True"
                },
                new TestCaseData(instanceA, instanceB, true)
                {
                    TestName = "Equals_InstanceAInstanceB_True"
                },
                new TestCaseData(instanceB, instanceC, false)
                {
                    TestName = "Equals_InstanceBInstanceC_False"
                },
                new TestCaseData(instanceA, instanceD, false)
                {
                    TestName = "Equals_InstanceAInstanceD_False"
                }
            };
        }
    }
}