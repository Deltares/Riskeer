﻿// Copyright (C) Stichting Deltares 2022. All rights reserved.
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
using Riskeer.Piping.Data.SemiProbabilistic;

namespace Riskeer.Piping.Data.Test.SemiProbabilistic
{
    [TestFixture]
    public class DerivedSemiProbabilisticPipingOutputTest
    {
        [Test]
        public void Constructor_DefaultPropertiesSet()
        {
            // Setup
            var random = new Random(21);
            double upliftFactorOfSafety = random.NextDouble();
            double upliftReliability = random.NextDouble();
            double upliftProbability = random.NextDouble();
            double heaveFactorOfSafety = random.NextDouble();
            double heaveReliability = random.NextDouble();
            double heaveProbability = random.NextDouble();
            double sellmeijerFactorOfSafety = random.NextDouble();
            double sellmeijerReliability = random.NextDouble();
            double sellmeijerProbability = random.NextDouble();
            double pipingProbability = random.NextDouble();
            double pipingReliability = random.NextDouble();

            // Call
            var output = new DerivedSemiProbabilisticPipingOutput(
                upliftFactorOfSafety,
                upliftReliability,
                upliftProbability,
                heaveFactorOfSafety,
                heaveReliability,
                heaveProbability,
                sellmeijerFactorOfSafety,
                sellmeijerReliability,
                sellmeijerProbability,
                pipingProbability,
                pipingReliability);

            // Assert
            Assert.AreEqual(3, output.HeaveFactorOfSafety.NumberOfDecimalPlaces);
            Assert.AreEqual(upliftFactorOfSafety, output.UpliftFactorOfSafety, output.UpliftFactorOfSafety.GetAccuracy());
            Assert.AreEqual(5, output.UpliftReliability.NumberOfDecimalPlaces);
            Assert.AreEqual(upliftReliability, output.UpliftReliability, output.UpliftReliability.GetAccuracy());
            Assert.AreEqual(upliftProbability, output.UpliftProbability);
            Assert.AreEqual(3, output.HeaveFactorOfSafety.NumberOfDecimalPlaces);
            Assert.AreEqual(heaveFactorOfSafety, output.HeaveFactorOfSafety, output.HeaveFactorOfSafety.GetAccuracy());
            Assert.AreEqual(5, output.HeaveReliability.NumberOfDecimalPlaces);
            Assert.AreEqual(heaveReliability, output.HeaveReliability, output.HeaveReliability.GetAccuracy());
            Assert.AreEqual(heaveProbability, output.HeaveProbability);
            Assert.AreEqual(3, output.SellmeijerFactorOfSafety.NumberOfDecimalPlaces);
            Assert.AreEqual(sellmeijerFactorOfSafety, output.SellmeijerFactorOfSafety, output.SellmeijerFactorOfSafety.GetAccuracy());
            Assert.AreEqual(5, output.SellmeijerReliability.NumberOfDecimalPlaces);
            Assert.AreEqual(sellmeijerReliability, output.SellmeijerReliability, output.SellmeijerReliability.GetAccuracy());
            Assert.AreEqual(sellmeijerProbability, output.SellmeijerProbability);
            Assert.AreEqual(pipingProbability, output.PipingProbability);
            Assert.AreEqual(5, output.PipingReliability.NumberOfDecimalPlaces);
            Assert.AreEqual(pipingReliability, output.PipingReliability, output.PipingReliability.GetAccuracy());
        }

        [Test]
        [TestCase(double.NaN)]
        [TestCase(0.0)]
        [TestCase(0.123456789)]
        [TestCase(1.0)]
        public void Constructor_ValidSellmeijerProbability_ExpectedValues(double sellmeijerProbability)
        {
            // Setup
            var random = new Random(21);
            double upliftFactorOfSafety = random.NextDouble();
            double upliftReliability = random.NextDouble();
            double upliftProbability = random.NextDouble();
            double heaveFactorOfSafety = random.NextDouble();
            double heaveReliability = random.NextDouble();
            double heaveProbability = random.NextDouble();
            double sellmeijerFactorOfSafety = random.NextDouble();
            double sellmeijerReliability = random.NextDouble();
            double pipingProbability = random.NextDouble();
            double pipingReliability = random.NextDouble();

            // Call
            var output = new DerivedSemiProbabilisticPipingOutput(
                upliftFactorOfSafety,
                upliftReliability,
                upliftProbability,
                heaveFactorOfSafety,
                heaveReliability,
                heaveProbability,
                sellmeijerFactorOfSafety,
                sellmeijerReliability,
                sellmeijerProbability,
                pipingProbability,
                pipingReliability);

            // Assert
            Assert.AreEqual(sellmeijerProbability, output.SellmeijerProbability);
        }

        [Test]
        [Culture("nl-NL")]
        [TestCase(double.PositiveInfinity)]
        [TestCase(double.NegativeInfinity)]
        [TestCase(0.0 - 1e-6)]
        [TestCase(-346587.456)]
        [TestCase(1.0 + 1e-6)]
        [TestCase(346587.456)]
        public void Constructor_InvalidSellmeijerProbability_ThrowArgumentOutOfRangeException(double sellmeijerProbability)
        {
            // Setup
            var random = new Random(21);
            double upliftFactorOfSafety = random.NextDouble();
            double upliftReliability = random.NextDouble();
            double upliftProbability = random.NextDouble();
            double heaveFactorOfSafety = random.NextDouble();
            double heaveReliability = random.NextDouble();
            double heaveProbability = random.NextDouble();
            double sellmeijerFactorOfSafety = random.NextDouble();
            double sellmeijerReliability = random.NextDouble();
            double pipingProbability = random.NextDouble();
            double pipingReliability = random.NextDouble();

            // Call
            void Call() => new DerivedSemiProbabilisticPipingOutput(
                upliftFactorOfSafety,
                upliftReliability,
                upliftProbability,
                heaveFactorOfSafety,
                heaveReliability,
                heaveProbability,
                sellmeijerFactorOfSafety,
                sellmeijerReliability,
                sellmeijerProbability,
                pipingProbability,
                pipingReliability);

            // Assert
            const string expectedMessage = "Kans moet in het bereik [0,0, 1,0] liggen.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(Call, expectedMessage);
        }

        [Test]
        [TestCase(double.NaN)]
        [TestCase(0.0)]
        [TestCase(0.123456789)]
        [TestCase(1.0)]
        public void Constructor_ValidHeaveProbability_ExpectedValues(double heaveProbability)
        {
            // Setup
            var random = new Random(21);
            double upliftFactorOfSafety = random.NextDouble();
            double upliftReliability = random.NextDouble();
            double upliftProbability = random.NextDouble();
            double heaveFactorOfSafety = random.NextDouble();
            double heaveReliability = random.NextDouble();
            double sellmeijerFactorOfSafety = random.NextDouble();
            double sellmeijerReliability = random.NextDouble();
            double sellmeijerProbability = random.NextDouble();
            double pipingProbability = random.NextDouble();
            double pipingReliability = random.NextDouble();

            // Call
            var output = new DerivedSemiProbabilisticPipingOutput(
                upliftFactorOfSafety,
                upliftReliability,
                upliftProbability,
                heaveFactorOfSafety,
                heaveReliability,
                heaveProbability,
                sellmeijerFactorOfSafety,
                sellmeijerReliability,
                sellmeijerProbability,
                pipingProbability,
                pipingReliability);

            // Assert
            Assert.AreEqual(heaveProbability, output.HeaveProbability);
        }

        [Test]
        [Culture("nl-NL")]
        [TestCase(double.PositiveInfinity)]
        [TestCase(double.NegativeInfinity)]
        [TestCase(0.0 - 1e-2)]
        [TestCase(-346587.456)]
        [TestCase(1.0 + 1e-2)]
        [TestCase(346587.456)]
        public void Constructor_InvalidHeaveProbability_ThrowArgumentOutOfRangeException(double heaveProbability)
        {
            // Setup
            var random = new Random(21);
            double upliftFactorOfSafety = random.NextDouble();
            double upliftReliability = random.NextDouble();
            double upliftProbability = random.NextDouble();
            double heaveFactorOfSafety = random.NextDouble();
            double heaveReliability = random.NextDouble();
            double sellmeijerFactorOfSafety = random.NextDouble();
            double sellmeijerReliability = random.NextDouble();
            double sellmeijerProbability = random.NextDouble();
            double pipingProbability = random.NextDouble();
            double pipingReliability = random.NextDouble();

            // Call
            void Call() => new DerivedSemiProbabilisticPipingOutput(
                upliftFactorOfSafety,
                upliftReliability,
                upliftProbability,
                heaveFactorOfSafety,
                heaveReliability,
                heaveProbability,
                sellmeijerFactorOfSafety,
                sellmeijerReliability,
                sellmeijerProbability,
                pipingProbability,
                pipingReliability);

            // Assert
            const string expectedMessage = "Kans moet in het bereik [0,0, 1,0] liggen.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(Call, expectedMessage);
        }

        [Test]
        [TestCase(double.NaN)]
        [TestCase(0.0)]
        [TestCase(0.123456789)]
        [TestCase(1.0)]
        public void Constructor_ValidUpliftProbability_ExpectedValues(double upliftProbability)
        {
            // Setup
            var random = new Random(21);
            double upliftFactorOfSafety = random.NextDouble();
            double upliftReliability = random.NextDouble();
            double heaveFactorOfSafety = random.NextDouble();
            double heaveReliability = random.NextDouble();
            double heaveProbability = random.NextDouble();
            double sellmeijerFactorOfSafety = random.NextDouble();
            double sellmeijerReliability = random.NextDouble();
            double sellmeijerProbability = random.NextDouble();
            double pipingProbability = random.NextDouble();
            double pipingReliability = random.NextDouble();

            // Call
            var output = new DerivedSemiProbabilisticPipingOutput(
                upliftFactorOfSafety,
                upliftReliability,
                upliftProbability,
                heaveFactorOfSafety,
                heaveReliability,
                heaveProbability,
                sellmeijerFactorOfSafety,
                sellmeijerReliability,
                sellmeijerProbability,
                pipingProbability,
                pipingReliability);

            // Assert
            Assert.AreEqual(upliftProbability, output.UpliftProbability);
        }

        [Test]
        [Culture("nl-NL")]
        [TestCase(double.PositiveInfinity)]
        [TestCase(double.NegativeInfinity)]
        [TestCase(0.0 - 1e-6)]
        [TestCase(-346587.456)]
        [TestCase(1.0 + 1e-6)]
        [TestCase(346587.456)]
        public void Constructor_InvalidUpliftProbability_ThrowArgumentOutOfRangeException(double upliftProbability)
        {
            // Setup
            var random = new Random(21);
            double upliftFactorOfSafety = random.NextDouble();
            double upliftReliability = random.NextDouble();
            double heaveFactorOfSafety = random.NextDouble();
            double heaveReliability = random.NextDouble();
            double heaveProbability = random.NextDouble();
            double sellmeijerFactorOfSafety = random.NextDouble();
            double sellmeijerReliability = random.NextDouble();
            double sellmeijerProbability = random.NextDouble();
            double pipingProbability = random.NextDouble();
            double pipingReliability = random.NextDouble();

            // Call
            void Call() => new DerivedSemiProbabilisticPipingOutput(
                upliftFactorOfSafety,
                upliftReliability,
                upliftProbability,
                heaveFactorOfSafety,
                heaveReliability,
                heaveProbability,
                sellmeijerFactorOfSafety,
                sellmeijerReliability,
                sellmeijerProbability,
                pipingProbability,
                pipingReliability);

            // Assert
            const string expectedMessage = "Kans moet in het bereik [0,0, 1,0] liggen.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(Call, expectedMessage);
        }

        [Test]
        [TestCase(double.NaN)]
        [TestCase(0.0)]
        [TestCase(0.123456789)]
        [TestCase(1.0)]
        public void Constructor_ValidPipingProbability_ExpectedValues(double pipingProbability)
        {
            // Setup
            var random = new Random(21);
            double upliftFactorOfSafety = random.NextDouble();
            double upliftReliability = random.NextDouble();
            double upliftProbability = random.NextDouble();
            double heaveFactorOfSafety = random.NextDouble();
            double heaveReliability = random.NextDouble();
            double heaveProbability = random.NextDouble();
            double sellmeijerFactorOfSafety = random.NextDouble();
            double sellmeijerReliability = random.NextDouble();
            double sellmeijerProbability = random.NextDouble();
            double pipingReliability = random.NextDouble();

            // Call
            var output = new DerivedSemiProbabilisticPipingOutput(
                upliftFactorOfSafety,
                upliftReliability,
                upliftProbability,
                heaveFactorOfSafety,
                heaveReliability,
                heaveProbability,
                sellmeijerFactorOfSafety,
                sellmeijerReliability,
                sellmeijerProbability,
                pipingProbability,
                pipingReliability);

            // Assert
            Assert.AreEqual(pipingProbability, output.PipingProbability);
        }

        [Test]
        [Culture("nl-NL")]
        [TestCase(double.PositiveInfinity)]
        [TestCase(double.NegativeInfinity)]
        [TestCase(0.0 - 1e-2)]
        [TestCase(-346587.456)]
        [TestCase(1.0 + 1e-2)]
        [TestCase(346587.456)]
        public void Constructor_InvalidPipingProbability_ThrowArgumentOutOfRangeException(double pipingProbability)
        {
            // Setup
            var random = new Random(21);
            double upliftFactorOfSafety = random.NextDouble();
            double upliftReliability = random.NextDouble();
            double upliftProbability = random.NextDouble();
            double heaveFactorOfSafety = random.NextDouble();
            double heaveReliability = random.NextDouble();
            double heaveProbability = random.NextDouble();
            double sellmeijerFactorOfSafety = random.NextDouble();
            double sellmeijerReliability = random.NextDouble();
            double sellmeijerProbability = random.NextDouble();
            double pipingReliability = random.NextDouble();

            // Call
            void Call() => new DerivedSemiProbabilisticPipingOutput(
                upliftFactorOfSafety,
                upliftReliability,
                upliftProbability,
                heaveFactorOfSafety,
                heaveReliability,
                heaveProbability,
                sellmeijerFactorOfSafety,
                sellmeijerReliability,
                sellmeijerProbability,
                pipingProbability,
                pipingReliability);

            // Assert
            const string expectedMessage = "Kans moet in het bereik [0,0, 1,0] liggen.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(Call, expectedMessage);
        }
    }
}