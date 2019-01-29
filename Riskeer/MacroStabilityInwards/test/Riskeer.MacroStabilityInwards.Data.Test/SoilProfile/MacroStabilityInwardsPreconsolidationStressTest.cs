// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Riskeer.Common.Data.Probabilistics;
using Riskeer.Common.Data.TestUtil;
using Riskeer.MacroStabilityInwards.Data.SoilProfile;
using Riskeer.MacroStabilityInwards.Primitives;

namespace Riskeer.MacroStabilityInwards.Data.Test.SoilProfile
{
    [TestFixture]
    public class MacroStabilityInwardsPreconsolidationStressTest
    {
        [Test]
        public void Constructor_LocationNull_ThrowsArgumentNullException()
        {
            // Setup
            var random = new Random(21);

            var distribution = new VariationCoefficientLogNormalDistribution
            {
                Mean = (RoundedDouble) 0.005,
                CoefficientOfVariation = random.NextRoundedDouble()
            };

            // Call
            TestDelegate call = () => new MacroStabilityInwardsPreconsolidationStress(null, distribution);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("location", exception.ParamName);
        }

        [Test]
        public void Constructor_StressDistributionNull_ThrowsArgumentNullException()
        {
            // Setup
            var random = new Random(21);

            var location = new Point2D(random.NextDouble(), random.NextDouble());

            // Call
            TestDelegate call = () => new MacroStabilityInwardsPreconsolidationStress(location, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("stressDistribution", exception.ParamName);
        }

        [Test]
        public void Constructor_ValidArguments_ReturnsExpectedValues()
        {
            // Setup
            var random = new Random(21);

            var location = new Point2D(random.NextDouble(), random.NextDouble());
            var distribution = new VariationCoefficientLogNormalDistribution
            {
                Mean = random.NextRoundedDouble(0.1, double.MaxValue),
                CoefficientOfVariation = random.NextRoundedDouble()
            };

            // Call
            var stress = new MacroStabilityInwardsPreconsolidationStress(location, distribution);

            // Assert
            Assert.IsInstanceOf<IMacroStabilityInwardsPreconsolidationStress>(stress);
            Assert.AreEqual(location, stress.Location);

            DistributionAssert.AreEqual(new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = distribution.Mean,
                CoefficientOfVariation = distribution.CoefficientOfVariation
            }, stress.Stress);
        }

        [Test]
        [TestCaseSource(nameof(GetConstructorArgumentsNaN))]
        public void Constructor_CoordinateArgumentsNaN_ThrowsArgumentException(double xCoordinate,
                                                                               double zCoordinate,
                                                                               string parameterName)
        {
            // Setup
            var location = new Point2D(xCoordinate, zCoordinate);

            // Call
            TestDelegate call = () => new MacroStabilityInwardsPreconsolidationStress(location, new VariationCoefficientLogNormalDistribution(2));

            // Assert
            string expectedMessage = $"De waarde voor parameter '{parameterName}' voor de grensspanning moet een concreet getal zijn.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);
        }

        [TestFixture]
        private class MacroStabilityInwardsPreconsolidationStressEqualsTest :
            EqualsTestFixture<MacroStabilityInwardsPreconsolidationStress,
                DerivedMacroStabilityInwardsPreconsolidationStress>
        {
            protected override MacroStabilityInwardsPreconsolidationStress CreateObject()
            {
                return CreatePreconsolidationStress();
            }

            protected override DerivedMacroStabilityInwardsPreconsolidationStress CreateDerivedObject()
            {
                return new DerivedMacroStabilityInwardsPreconsolidationStress(CreatePreconsolidationStress());
            }

            private static IEnumerable<TestCaseData> GetUnequalTestCases()
            {
                var random = new Random(30);
                MacroStabilityInwardsPreconsolidationStress baseStress = CreatePreconsolidationStress();
                VariationCoefficientLogNormalDistribution baseDistribution = baseStress.Stress;

                yield return new TestCaseData(new MacroStabilityInwardsPreconsolidationStress(new Point2D(random.NextDouble(),
                                                                                                          random.NextDouble()),
                                                                                              baseDistribution))
                    .SetName("Location");

                yield return new TestCaseData(new MacroStabilityInwardsPreconsolidationStress(baseStress.Location,
                                                                                              new VariationCoefficientLogNormalDistribution
                                                                                              {
                                                                                                  Mean = baseDistribution.Mean + random.NextRoundedDouble(),
                                                                                                  CoefficientOfVariation = baseDistribution.CoefficientOfVariation
                                                                                              }))
                    .SetName("Distribution");
            }

            private static MacroStabilityInwardsPreconsolidationStress CreatePreconsolidationStress()
            {
                var random = new Random(21);

                var baseLocation = new Point2D(random.NextDouble(), random.NextDouble());
                var baseDistribution = new VariationCoefficientLogNormalDistribution
                {
                    Mean = (RoundedDouble) 0.005,
                    CoefficientOfVariation = random.NextRoundedDouble()
                };

                return new MacroStabilityInwardsPreconsolidationStress(baseLocation, baseDistribution);
            }
        }

        private static IEnumerable<TestCaseData> GetConstructorArgumentsNaN()
        {
            var random = new Random(21);

            double xCoordinate = random.NextDouble();
            double zCoordinate = random.NextDouble();

            yield return new TestCaseData(double.NaN, zCoordinate, "X-coördinaat").SetName("Invalid XCoordinate");
            yield return new TestCaseData(xCoordinate, double.NaN, "Z-coördinaat").SetName("Invalid ZCoordinate");
        }

        private class DerivedMacroStabilityInwardsPreconsolidationStress : MacroStabilityInwardsPreconsolidationStress
        {
            public DerivedMacroStabilityInwardsPreconsolidationStress(MacroStabilityInwardsPreconsolidationStress stress)
                : base(stress.Location, stress.Stress) {}
        }
    }
}