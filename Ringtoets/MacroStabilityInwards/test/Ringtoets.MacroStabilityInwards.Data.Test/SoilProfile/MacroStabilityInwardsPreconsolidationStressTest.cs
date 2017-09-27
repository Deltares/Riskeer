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
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.MacroStabilityInwards.Data.SoilProfile;

namespace Ringtoets.MacroStabilityInwards.Data.Test.SoilProfile
{
    [TestFixture]
    public class MacroStabilityInwardsPreconsolidationStressTest
    {
        [Test]
        public void Constructor_ValidArguments_ReturnsExpectedValues()
        {
            // Setup
            var random = new Random(21);

            var location = new Point2D(random.NextDouble(), random.NextDouble());
            var distribution = new VariationCoefficientLogNormalDistribution
            {
                Mean = (RoundedDouble) 0.005,
                CoefficientOfVariation = random.NextRoundedDouble()
            };

            // Call
            var stress = new MacroStabilityInwardsPreconsolidationStress(location, distribution);

            // Assert
            Assert.AreEqual(location, stress.Location);

            DistributionAssert.AreEqual(new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = distribution.Mean,
                CoefficientOfVariation = distribution.CoefficientOfVariation
            }, stress.Stress);
        }

        [Test]
        [TestCaseSource(nameof(GetConstructorArgumentsNaN))]
        public void Constructor_ArgumentsNaN_ThrowsArgumentException(double xCoordinate,
                                                                     double zCoordinate,
                                                                     double stressMean,
                                                                     double stressCoefficientOfVariation,
                                                                     string parameterName)
        {
            // Setup
            var location = new Point2D(xCoordinate, zCoordinate);
            var stressDistribution = new VariationCoefficientLogNormalDistribution
            {
                Mean = (RoundedDouble) stressMean,
                CoefficientOfVariation = (RoundedDouble) stressCoefficientOfVariation
            };

            // Call
            TestDelegate call = () => new MacroStabilityInwardsPreconsolidationStress(location, stressDistribution);

            // Assert
            string expectedMessage = $"De waarde voor parameter '{parameterName}' voor de grensspanning moet een concreet getal zijn.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);
        }

        [Test]
        public void Equals_ToNull_ReturnsFalse()
        {
            // Setup
            var random = new Random(21);

            var location = new Point2D(random.NextDouble(), random.NextDouble());
            var distribution = new VariationCoefficientLogNormalDistribution
            {
                Mean = (RoundedDouble) 0.005,
                CoefficientOfVariation = random.NextRoundedDouble()
            };

            var stress = new MacroStabilityInwardsPreconsolidationStress(location, distribution);

            // Call
            bool result = stress.Equals(null);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Equals_ToItself_ReturnsTrue()
        {
            // Setup
            var random = new Random(21);
            var location = new Point2D(random.NextDouble(), random.NextDouble());
            var distribution = new VariationCoefficientLogNormalDistribution
            {
                Mean = (RoundedDouble) 0.005,
                CoefficientOfVariation = random.NextRoundedDouble()
            };

            var stress = new MacroStabilityInwardsPreconsolidationStress(location, distribution);

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

            var location = new Point2D(random.NextDouble(), random.NextDouble());
            var distribution = new VariationCoefficientLogNormalDistribution
            {
                Mean = (RoundedDouble) 0.005,
                CoefficientOfVariation = random.NextRoundedDouble()
            };

            var stress = new MacroStabilityInwardsPreconsolidationStress(location, distribution);

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

            var location = new Point2D(random.NextDouble(), random.NextDouble());
            var distribution = new VariationCoefficientLogNormalDistribution
            {
                Mean = (RoundedDouble) 0.005,
                CoefficientOfVariation = random.NextRoundedDouble()
            };

            var stress = new MacroStabilityInwardsPreconsolidationStress(location, distribution);
            var derivedStress = new DerivedMacroStabilityInwardsPreconsolidationStress(location, distribution);

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

            var location = new Point2D(random.NextDouble(), random.NextDouble());
            var distribution = new VariationCoefficientLogNormalDistribution
            {
                Mean = (RoundedDouble) 0.005,
                CoefficientOfVariation = random.NextRoundedDouble()
            };

            var stressA = new MacroStabilityInwardsPreconsolidationStress(location, distribution);
            var stressB = new MacroStabilityInwardsPreconsolidationStress(location, distribution);

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
        public void Equals_TransitiveProperty_ReturnsTrue()
        {
            // Setup
            var random = new Random(21);

            var location = new Point2D(random.NextDouble(), random.NextDouble());
            var distribution = new VariationCoefficientLogNormalDistribution
            {
                Mean = (RoundedDouble) 0.005,
                CoefficientOfVariation = random.NextRoundedDouble()
            };

            var stressA = new MacroStabilityInwardsPreconsolidationStress(location, distribution);
            var stressB = new MacroStabilityInwardsPreconsolidationStress(location, distribution);
            var stressC = new MacroStabilityInwardsPreconsolidationStress(location, distribution);

            // Call
            bool aEqualsB = stressA.Equals(stressB);
            bool bEqualsC = stressB.Equals(stressC);
            bool aEqualsC = stressA.Equals(stressC);

            // Assert
            Assert.IsTrue(aEqualsB);
            Assert.IsTrue(bEqualsC);
            Assert.IsTrue(aEqualsC);
        }

        [Test]
        public void GetHashCode_PropertiesAllEqual_ReturnsSameHashCode()
        {
            // Setup
            var random = new Random(21);

            var location = new Point2D(random.NextDouble(), random.NextDouble());
            var distribution = new VariationCoefficientLogNormalDistribution
            {
                Mean = (RoundedDouble) 0.005,
                CoefficientOfVariation = random.NextRoundedDouble()
            };

            var stressA = new MacroStabilityInwardsPreconsolidationStress(location, distribution);
            var stressB = new MacroStabilityInwardsPreconsolidationStress(location, distribution);

            // Call
            int hashCodeStressA = stressA.GetHashCode();
            int hashCodeStressB = stressB.GetHashCode();

            // Assert
            Assert.AreEqual(hashCodeStressA, hashCodeStressB);
        }

        private static IEnumerable<TestCaseData> GetStressCombinations()
        {
            var random = new Random(21);

            var baseLocation = new Point2D(random.NextDouble(), random.NextDouble());
            var baseDistribution = new VariationCoefficientLogNormalDistribution
            {
                Mean = (RoundedDouble) 0.005,
                CoefficientOfVariation = random.NextRoundedDouble()
            };

            var baseStress = new MacroStabilityInwardsPreconsolidationStress(baseLocation, baseDistribution);

            yield return new TestCaseData(baseStress,
                                          new MacroStabilityInwardsPreconsolidationStress(new Point2D(baseLocation.X + random.NextDouble(),
                                                                                                      baseLocation.Y),
                                                                                          baseDistribution))
                .SetName("Different X Coordinate");
            yield return new TestCaseData(baseStress,
                                          new MacroStabilityInwardsPreconsolidationStress(new Point2D(baseLocation.X,
                                                                                                      baseLocation.Y + random.NextDouble()),
                                                                                          baseDistribution))
                .SetName("Different Z Coordinate");
            yield return new TestCaseData(baseStress,
                                          new MacroStabilityInwardsPreconsolidationStress(baseLocation,
                                                                                          new VariationCoefficientLogNormalDistribution
                                                                                          {
                                                                                              Mean = baseDistribution.Mean + random.NextRoundedDouble(),
                                                                                              CoefficientOfVariation = baseDistribution.CoefficientOfVariation
                                                                                          }))
                .SetName("Different Mean");
            yield return new TestCaseData(baseStress,
                                          new MacroStabilityInwardsPreconsolidationStress(baseLocation,
                                                                                          new VariationCoefficientLogNormalDistribution
                                                                                          {
                                                                                              Mean = baseDistribution.Mean,
                                                                                              CoefficientOfVariation = baseDistribution.CoefficientOfVariation + random.NextRoundedDouble()
                                                                                          }))
                .SetName("Different Coefficient of Variation");
        }

        private static IEnumerable<TestCaseData> GetConstructorArgumentsNaN()
        {
            var random = new Random(21);

            double xCoordinate = random.NextDouble();
            double zCoordinate = random.NextDouble();
            const double stressMean = 0.005;
            double stressCoefficientOfVariation = random.NextDouble();

            yield return new TestCaseData(double.NaN, zCoordinate, stressMean, stressCoefficientOfVariation, "X-coördinaat").SetName("Invalid XCoordinate");
            yield return new TestCaseData(xCoordinate, double.NaN, stressMean, stressCoefficientOfVariation, "Z-coördinaat").SetName("Invalid ZCoordinate");
            yield return new TestCaseData(xCoordinate, zCoordinate, double.NaN, stressCoefficientOfVariation, "gemiddelde").SetName("Invalid Mean");
            yield return new TestCaseData(xCoordinate, zCoordinate, stressMean, double.NaN, "variatiecoëfficient").SetName("Invalid Coefficient of Variation");
        }

        private class DerivedMacroStabilityInwardsPreconsolidationStress : MacroStabilityInwardsPreconsolidationStress
        {
            public DerivedMacroStabilityInwardsPreconsolidationStress(Point2D location, VariationCoefficientLogNormalDistribution distribution)
                : base(location, distribution) {}
        }
    }
}