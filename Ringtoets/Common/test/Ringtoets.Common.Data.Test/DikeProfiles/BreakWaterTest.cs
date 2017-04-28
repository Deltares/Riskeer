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

using Core.Common.Base.Data;
using NUnit.Framework;
using Ringtoets.Common.Data.DikeProfiles;

namespace Ringtoets.Common.Data.Test.DikeProfiles
{
    [TestFixture]
    public class BreakWaterTest
    {
        [Test]
        public void Constructor_Always_ExpectedValues()
        {
            // Setup
            const BreakWaterType type = BreakWaterType.Caisson;
            const double height = 100.1;

            // Call
            var breakWater = new BreakWater(type, height);

            // Assert
            Assert.AreEqual(type, breakWater.Type);
            Assert.AreEqual(height, breakWater.Height, 1e-6);
            Assert.AreEqual(2, breakWater.Height.NumberOfDecimalPlaces);
        }

        [Test]
        [TestCase(BreakWaterType.Dam)]
        [TestCase(BreakWaterType.Wall)]
        public void Properties_Type_ReturnsExpectedValue(BreakWaterType newType)
        {
            // Setup
            const BreakWaterType type = BreakWaterType.Caisson;
            const double height = 100.1;
            var breakWater = new BreakWater(type, height);

            // Call
            breakWater.Type = newType;

            // Assert
            Assert.AreEqual(newType, breakWater.Type);
        }

        [Test]
        public void Properties_Height_ReturnsExpectedValue()
        {
            // Setup
            const BreakWaterType type = BreakWaterType.Caisson;
            const double height = 100.10;
            var breakWater = new BreakWater(type, height);

            // Call
            breakWater.Height = (RoundedDouble) 10.00;

            // Assert
            Assert.AreEqual(10.0, breakWater.Height.Value);
        }

        [Test]
        public void Equals_ToItself_ReturnsTrue()
        {
            // Setup
            var breakWater = new BreakWater(BreakWaterType.Caisson, 100.10);

            // Call
            bool isBreakWaterEqualToItself = breakWater.Equals(breakWater);

            // Assert
            Assert.IsTrue(isBreakWaterEqualToItself);
        }

        [Test]
        public void Equals_AllPropertiesEqual_ReturnsTrue()
        {
            // Setup 
            const BreakWaterType type = BreakWaterType.Caisson;
            const double height = 100.10;

            var breakWaterOne = new BreakWater(type, height);
            var breakWaterTwo = new BreakWater(type, height);

            // Call
            bool isBreakWaterOneEqualToTwo = breakWaterOne.Equals(breakWaterTwo);
            bool isBreakWaterTwoEqualToOne = breakWaterTwo.Equals(breakWaterOne);

            // Assert
            Assert.IsTrue(isBreakWaterOneEqualToTwo);
            Assert.IsTrue(isBreakWaterTwoEqualToOne);
        }

        [Test]
        public void Equals_ToSameReference_ReturnsTrue()
        {
            // Setup
            var breakWaterOne = new BreakWater(BreakWaterType.Caisson, 100.10);
            BreakWater breakWaterTwo = breakWaterOne;

            // Call
            bool isBreakWaterOneEqualToTwo = breakWaterOne.Equals(breakWaterTwo);
            bool isBreakWaterTwoEqualToOne = breakWaterTwo.Equals(breakWaterOne);

            // Assert
            Assert.IsTrue(isBreakWaterOneEqualToTwo);
            Assert.IsTrue(isBreakWaterTwoEqualToOne);
        }

        [Test]
        public void Equals_TransitivePropertyEqualBreakWater_ReturnsTrue()
        {
            // Setup 
            const BreakWaterType type = BreakWaterType.Caisson;
            const double height = 100.10;

            var breakWaterOne = new BreakWater(type, height);
            var breakWaterTwo = new BreakWater(type, height);
            var breakWaterThree = new BreakWater(type, height);

            // Call
            bool isBreakWaterOneEqualToTwo = breakWaterOne.Equals(breakWaterTwo);
            bool isBreakWaterTwoEqualToThree = breakWaterTwo.Equals(breakWaterThree);
            bool isBreakWaterOneEqualToThree = breakWaterOne.Equals(breakWaterThree);

            // Assert
            Assert.IsTrue(isBreakWaterOneEqualToTwo);
            Assert.IsTrue(isBreakWaterTwoEqualToThree);
            Assert.IsTrue(isBreakWaterOneEqualToThree);
        }

        [Test]
        public void Equals_ToNull_ReturnsFalse()
        {
            // Setup
            var breakWater = new BreakWater(BreakWaterType.Caisson, 100.10);

            // Call
            bool isBreakWaterEqualToNull = breakWater.Equals(null);

            // Assert
            Assert.IsFalse(isBreakWaterEqualToNull);
        }

        [Test]
        public void Equals_ToDifferentType_ReturnsFalse()
        {
            // Setup
            var breakWater = new BreakWater(BreakWaterType.Caisson, 100.10);
            var differentType = new object();

            // Call
            bool isBreakWaterEqualToDifferentType = breakWater.Equals(differentType);

            // Assert
            Assert.IsFalse(isBreakWaterEqualToDifferentType);
        }

        [Test]
        public void Equals_DifferentBreakWaterType_ReturnsFalse()
        {
            // Setup 
            const double height = 100.10;

            var breakWaterOne = new BreakWater(BreakWaterType.Caisson, height);
            var breakWaterTwo = new BreakWater(BreakWaterType.Wall, height);

            // Call
            bool isBreakWaterOneEqualToTwo = breakWaterOne.Equals(breakWaterTwo);
            bool isBreakWaterTwoEqualToOne = breakWaterTwo.Equals(breakWaterOne);

            // Assert
            Assert.IsFalse(isBreakWaterOneEqualToTwo);
            Assert.IsFalse(isBreakWaterTwoEqualToOne);
        }

        [Test]
        public void Equals_DifferentHeight_ReturnsFalse()
        {
            // Setup 
            const BreakWaterType type = BreakWaterType.Caisson;

            var breakWaterOne = new BreakWater(type, 100.10);
            var breakWaterTwo = new BreakWater(type, 100.20);

            // Call
            bool isBreakWaterOneEqualToTwo = breakWaterOne.Equals(breakWaterTwo);
            bool isBreakWaterTwoEqualToOne = breakWaterTwo.Equals(breakWaterOne);

            // Assert
            Assert.IsFalse(isBreakWaterOneEqualToTwo);
            Assert.IsFalse(isBreakWaterTwoEqualToOne);
        }

        [Test]
        public void GetHashCode_EqualBreakWater_ReturnsSameHashCode()
        {
            // Setup 
            const BreakWaterType type = BreakWaterType.Caisson;
            const double height = 100.10;

            var breakWaterOne = new BreakWater(type, height);
            var breakWaterTwo = new BreakWater(type, height);

            // Call
            int hashCodeOne = breakWaterOne.GetHashCode();
            int hashCodeTwo = breakWaterTwo.GetHashCode();

            // Assert
            Assert.AreEqual(hashCodeOne, hashCodeTwo);
        }
    }
}