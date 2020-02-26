// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.TestUtil;

namespace Riskeer.MacroStabilityInwards.Data.Test
{
    [TestFixture]
    public class DerivedMacroStabilityInwardsOutputTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var random = new Random(21);
            double factorOfStability = random.NextDouble();
            double requiredProbability = random.NextDouble();
            double requiredReliability = random.NextDouble();
            double macroStabilityInwardsProbability = random.NextDouble();
            double macroStabilityInwardsReliability = random.NextDouble();
            double macroStabilityInwardsFactorOfSafety = random.NextDouble();

            // Call
            var output = new DerivedMacroStabilityInwardsOutput(
                factorOfStability,
                requiredProbability,
                requiredReliability,
                macroStabilityInwardsProbability,
                macroStabilityInwardsReliability,
                macroStabilityInwardsFactorOfSafety);

            // Assert
            Assert.AreEqual(factorOfStability, output.FactorOfStability, output.FactorOfStability.GetAccuracy());
            Assert.AreEqual(3, output.FactorOfStability.NumberOfDecimalPlaces);
            Assert.AreEqual(requiredProbability, output.RequiredProbability);
            Assert.AreEqual(5, output.RequiredReliability.NumberOfDecimalPlaces);
            Assert.AreEqual(requiredReliability, output.RequiredReliability, output.RequiredReliability.GetAccuracy());
            Assert.AreEqual(macroStabilityInwardsProbability, output.MacroStabilityInwardsProbability);
            Assert.AreEqual(5, output.MacroStabilityInwardsReliability.NumberOfDecimalPlaces);
            Assert.AreEqual(macroStabilityInwardsReliability, output.MacroStabilityInwardsReliability, output.MacroStabilityInwardsReliability.GetAccuracy());
            Assert.AreEqual(3, output.MacroStabilityInwardsFactorOfSafety.NumberOfDecimalPlaces);
            Assert.AreEqual(macroStabilityInwardsFactorOfSafety, output.MacroStabilityInwardsFactorOfSafety, output.MacroStabilityInwardsFactorOfSafety.GetAccuracy());
        }

        [Test]
        [TestCase(double.NaN)]
        [TestCase(0.0)]
        [TestCase(0.123456789)]
        [TestCase(1.0)]
        public void Constructor_ValidRequiredProbability_ExpectedValues(double requiredProbability)
        {
            // Setup
            var random = new Random(21);
            double factorOfStability = random.NextDouble();
            double requiredReliability = random.NextDouble();
            double macroStabilityInwardsProbability = random.NextDouble();
            double macroStabilityInwardsReliability = random.NextDouble();
            double macroStabilityInwardsFactorOfSafety = random.NextDouble();

            // Call
            var output = new DerivedMacroStabilityInwardsOutput(
                factorOfStability,
                requiredProbability,
                requiredReliability,
                macroStabilityInwardsProbability,
                macroStabilityInwardsReliability,
                macroStabilityInwardsFactorOfSafety);

            // Assert
            Assert.AreEqual(requiredProbability, output.RequiredProbability);
        }

        [Test]
        [SetCulture("nl-NL")]
        [TestCase(double.PositiveInfinity)]
        [TestCase(double.NegativeInfinity)]
        [TestCase(0.0 - 1e-6)]
        [TestCase(-346587.456)]
        [TestCase(1.0 + 1e-6)]
        [TestCase(346587.456)]
        public void Constructor_InvalidRequiredProbability_ThrowsArgumentOutOfRangeException(double requiredProbability)
        {
            // Setup
            var random = new Random(21);
            double factorOfStability = random.NextDouble();
            double requiredReliability = random.NextDouble();
            double macroStabilityInwardsProbability = random.NextDouble();
            double macroStabilityInwardsReliability = random.NextDouble();
            double macroStabilityInwardsFactorOfSafety = random.NextDouble();

            // Call
            TestDelegate call = () => new DerivedMacroStabilityInwardsOutput(
                factorOfStability,
                requiredProbability,
                requiredReliability,
                macroStabilityInwardsProbability,
                macroStabilityInwardsReliability,
                macroStabilityInwardsFactorOfSafety);

            // Assert
            const string expectedMessage = "Kans moet in het bereik [0,0, 1,0] liggen.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(call, expectedMessage);
        }

        [Test]
        [TestCase(double.NaN)]
        [TestCase(0.0)]
        [TestCase(0.123456789)]
        [TestCase(1.0)]
        public void Constructor_ValidMacroStabilityInwardsProbability_ExpectedValues(double macroStabilityInwardsProbability)
        {
            // Setup
            var random = new Random(21);
            double factorOfStability = random.NextDouble();
            double requiredProbability = random.NextDouble();
            double requiredReliability = random.NextDouble();
            double macroStabilityInwardsReliability = random.NextDouble();
            double macroStabilityInwardsFactorOfSafety = random.NextDouble();

            // Call
            var output = new DerivedMacroStabilityInwardsOutput(
                factorOfStability,
                requiredProbability,
                requiredReliability,
                macroStabilityInwardsProbability,
                macroStabilityInwardsReliability,
                macroStabilityInwardsFactorOfSafety);

            // Assert
            Assert.AreEqual(macroStabilityInwardsProbability, output.MacroStabilityInwardsProbability);
        }

        [Test]
        [SetCulture("nl-NL")]
        [TestCase(double.PositiveInfinity)]
        [TestCase(double.NegativeInfinity)]
        [TestCase(0.0 - 1e-2)]
        [TestCase(-346587.456)]
        [TestCase(1.0 + 1e-2)]
        [TestCase(346587.456)]
        public void Constructor_InvalidMacroStabilityInwardsProbability_ThrowsArgumentOutOfRangeException(double macroStabilityInwardsProbability)
        {
            // Setup
            var random = new Random(21);
            double factorOfStability = random.NextDouble();
            double requiredProbability = random.NextDouble();
            double requiredReliability = random.NextDouble();
            double macroStabilityInwardsReliability = random.NextDouble();
            double macroStabilityInwardsFactorOfSafety = random.NextDouble();

            // Call
            TestDelegate call = () => new DerivedMacroStabilityInwardsOutput(
                factorOfStability,
                requiredProbability,
                requiredReliability,
                macroStabilityInwardsProbability,
                macroStabilityInwardsReliability,
                macroStabilityInwardsFactorOfSafety);

            // Assert
            const string expectedMessage = "Kans moet in het bereik [0,0, 1,0] liggen.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(call, expectedMessage);
        }
    }
}