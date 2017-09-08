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
using NUnit.Framework;

namespace Ringtoets.MacroStabilityInwards.Primitives.Test
{
    [TestFixture]
    public class MacroStabilityInwardsPreconsolidationStressTest
    {
        [Test]
        public void Constructor_ReturnsExpectedValues()
        {
            // Setup
            var random = new Random(21);

            double xCoordinate = random.NextDouble();
            double zCoordinate = random.NextDouble();
            double stressShift = random.NextDouble();
            double stressMean = random.NextDouble();
            double stressCoefficientOfVariation = random.NextDouble();

            // Call
            var stress = new MacroStabilityInwardsPreconsolidationStress(xCoordinate,
                                                                         zCoordinate,
                                                                         stressMean,
                                                                         stressCoefficientOfVariation,
                                                                         stressShift);
            // Assert
            Assert.AreEqual(xCoordinate, stress.XCoordinate);
            Assert.AreEqual(zCoordinate, stress.ZCoordinate);

            Assert.AreEqual(stressMean, stress.PreconsolidationStressMean);
            Assert.AreEqual(stressCoefficientOfVariation, stress.PreconsolidationStressCoefficientOfVariation);
            Assert.AreEqual(stressShift, stress.PreconsolidationStressShift);
        }

        [Test]
        public void Equals_ToNull_ReturnsFalse()
        {
            // Setup
            var random = new Random(21);

            double xCoordinate = random.NextDouble();
            double zCoordinate = random.NextDouble();
            double stressShift = random.NextDouble();
            double stressMean = random.NextDouble();
            double stressCoefficientOfVariation = random.NextDouble();

            var stress = new MacroStabilityInwardsPreconsolidationStress(xCoordinate,
                                                                         zCoordinate,
                                                                         stressMean,
                                                                         stressCoefficientOfVariation,
                                                                         stressShift);

            // Call
            bool result = stress.Equals(null);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Equals_ToiTself_ReturnsTrue()
        {
            // Setup
            var random = new Random(21);

            double xCoordinate = random.NextDouble();
            double zCoordinate = random.NextDouble();
            double stressShift = random.NextDouble();
            double stressMean = random.NextDouble();
            double stressCoefficientOfVariation = random.NextDouble();

            var stress = new MacroStabilityInwardsPreconsolidationStress(xCoordinate,
                                                                         zCoordinate,
                                                                         stressMean,
                                                                         stressCoefficientOfVariation,
                                                                         stressShift);

            MacroStabilityInwardsPreconsolidationStress sameStress = stress;

            // Call
            bool resultOne = stress.Equals(sameStress);
            bool resultTwo = sameStress.Equals(stress);

            // Assert
            Assert.IsTrue(resultOne);
            Assert.IsTrue(resultTwo);
        }

        [Test]
        public void Equals_ToOtherType_ReturnsFalse()
        {
            // Setup
            var random = new Random(21);

            double xCoordinate = random.NextDouble();
            double zCoordinate = random.NextDouble();
            double stressShift = random.NextDouble();
            double stressMean = random.NextDouble();
            double stressCoefficientOfVariation = random.NextDouble();

            var stress = new MacroStabilityInwardsPreconsolidationStress(xCoordinate,
                                                                         zCoordinate,
                                                                         stressMean,
                                                                         stressCoefficientOfVariation,
                                                                         stressShift);

            // Call
            bool result = stress.Equals(new object());

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Equals_ToDerivedType_ReturnsFalse()
        {
            // Setup
            var random = new Random(21);

            double xCoordinate = random.NextDouble();
            double zCoordinate = random.NextDouble();
            double stressShift = random.NextDouble();
            double stressMean = random.NextDouble();
            double stressCoefficientOfVariation = random.NextDouble();

            var stress = new MacroStabilityInwardsPreconsolidationStress(xCoordinate,
                                                                         zCoordinate,
                                                                         stressMean,
                                                                         stressCoefficientOfVariation,
                                                                         stressShift);

            var derivedStress = new DerivedMacroStabilityInwardsPreconsolidationStress(xCoordinate,
                                                                                       zCoordinate,
                                                                                       stressMean,
                                                                                       stressCoefficientOfVariation,
                                                                                       stressShift);

            // Call
            bool result = stress.Equals(derivedStress);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Equals_AllPropertiesEqual_ReturnsTrue()
        {
            // Setup
            var random = new Random(21);

            double xCoordinate = random.NextDouble();
            double zCoordinate = random.NextDouble();
            double stressShift = random.NextDouble();
            double stressMean = random.NextDouble();
            double stressCoefficientOfVariation = random.NextDouble();

            var stressA = new MacroStabilityInwardsPreconsolidationStress(xCoordinate,
                                                                          zCoordinate,
                                                                          stressMean,
                                                                          stressCoefficientOfVariation,
                                                                          stressShift);

            var stressB = new MacroStabilityInwardsPreconsolidationStress(xCoordinate,
                                                                          zCoordinate,
                                                                          stressMean,
                                                                          stressCoefficientOfVariation,
                                                                          stressShift);

            // Call
            bool isStressAEqualToB = stressA.Equals(stressB);
            bool isStressBEqualToA = stressB.Equals(stressA);

            // Assert
            Assert.IsTrue(isStressAEqualToB);
            Assert.IsTrue(isStressBEqualToA);
        }

        [Test]
        [TestCaseSource(nameof(GetStressCombinations))]
        public void Equals_DifferingProperty_ReturnsFalse(MacroStabilityInwardsPreconsolidationStress stressA,
                                                          MacroStabilityInwardsPreconsolidationStress stressB)
        {
            // Call
            bool isStressAEqualToB = stressA.Equals(stressB);
            bool isStressBEqualToA = stressB.Equals(stressA);

            // Assert
            Assert.IsFalse(isStressAEqualToB);
            Assert.IsFalse(isStressBEqualToA);
        }

        [Test]
        public void GetHashCode_PropertiesAllEqual_ReturnsSameHashCode()
        {
            // Setup
            var random = new Random(21);

            double xCoordinate = random.NextDouble();
            double zCoordinate = random.NextDouble();
            double stressShift = random.NextDouble();
            double stressMean = random.NextDouble();
            double stressCoefficientOfVariation = random.NextDouble();

            var stressA = new MacroStabilityInwardsPreconsolidationStress(xCoordinate,
                                                                          zCoordinate,
                                                                          stressMean,
                                                                          stressCoefficientOfVariation,
                                                                          stressShift);

            var stressB = new MacroStabilityInwardsPreconsolidationStress(xCoordinate,
                                                                          zCoordinate,
                                                                          stressMean,
                                                                          stressCoefficientOfVariation,
                                                                          stressShift);

            // Call
            int hashCodeStressA = stressA.GetHashCode();
            int hashCodeStressB = stressB.GetHashCode();

            // Assert
            Assert.AreEqual(hashCodeStressA, hashCodeStressB);
        }

        private static IEnumerable<TestCaseData> GetStressCombinations()
        {
            var random = new Random(21);

            double xCoordinate = random.NextDouble();
            double zCoordinate = random.NextDouble();
            double stressShift = random.NextDouble();
            double stressMean = random.NextDouble();
            double stressCoefficientOfVariation = random.NextDouble();

            var baseStress = new MacroStabilityInwardsPreconsolidationStress(xCoordinate,
                                                                             zCoordinate,
                                                                             stressMean,
                                                                             stressCoefficientOfVariation,
                                                                             stressShift);

            yield return new TestCaseData(baseStress,
                                          new MacroStabilityInwardsPreconsolidationStress(baseStress.XCoordinate + random.NextDouble(),
                                                                                          baseStress.ZCoordinate,
                                                                                          baseStress.PreconsolidationStressMean,
                                                                                          baseStress.PreconsolidationStressCoefficientOfVariation,
                                                                                          baseStress.PreconsolidationStressShift))
                .SetName("Different X Coordinate");
            yield return new TestCaseData(baseStress,
                                          new MacroStabilityInwardsPreconsolidationStress(baseStress.XCoordinate,
                                                                                          baseStress.ZCoordinate + random.NextDouble(),
                                                                                          baseStress.PreconsolidationStressMean,
                                                                                          baseStress.PreconsolidationStressCoefficientOfVariation,
                                                                                          baseStress.PreconsolidationStressShift))
                .SetName("Different Z Coordinate");
            yield return new TestCaseData(baseStress,
                                          new MacroStabilityInwardsPreconsolidationStress(baseStress.XCoordinate,
                                                                                          baseStress.ZCoordinate,
                                                                                          baseStress.PreconsolidationStressMean + random.NextDouble(),
                                                                                          baseStress.PreconsolidationStressCoefficientOfVariation,
                                                                                          baseStress.PreconsolidationStressShift))
                .SetName("Different Mean");
            yield return new TestCaseData(baseStress,
                                          new MacroStabilityInwardsPreconsolidationStress(baseStress.XCoordinate,
                                                                                          baseStress.ZCoordinate,
                                                                                          baseStress.PreconsolidationStressMean,
                                                                                          baseStress.PreconsolidationStressCoefficientOfVariation + random.NextDouble(),
                                                                                          baseStress.PreconsolidationStressShift))
                .SetName("Different Coefficient of Variation");
            yield return new TestCaseData(baseStress,
                                          new MacroStabilityInwardsPreconsolidationStress(baseStress.XCoordinate,
                                                                                          baseStress.ZCoordinate,
                                                                                          baseStress.PreconsolidationStressMean,
                                                                                          baseStress.PreconsolidationStressCoefficientOfVariation,
                                                                                          baseStress.PreconsolidationStressShift + random.NextDouble()))
                .SetName("Different Shift");
        }

        private class DerivedMacroStabilityInwardsPreconsolidationStress : MacroStabilityInwardsPreconsolidationStress
        {
            public DerivedMacroStabilityInwardsPreconsolidationStress(double xCoordinate,
                                                                      double zCoordinate,
                                                                      double preconsolidationStressMean,
                                                                      double preconsolidationStressCoefficientOfVariation,
                                                                      double preconsolidationStressShift)
                : base(xCoordinate, zCoordinate, preconsolidationStressMean, preconsolidationStressCoefficientOfVariation, preconsolidationStressShift) {}
        }
    }
}