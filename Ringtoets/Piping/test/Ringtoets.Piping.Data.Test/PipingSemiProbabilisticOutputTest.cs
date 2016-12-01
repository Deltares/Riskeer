// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.TestUtil;

namespace Ringtoets.Piping.Data.Test
{
    [TestFixture]
    public class PipingSemiProbabilisticOutputTest
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
            double requiredProbability = random.NextDouble();
            double requiredReliability = random.NextDouble();
            double pipingProbability = random.NextDouble();
            double pipingReliability = random.NextDouble();
            double pipingFactorOfSafety = random.NextDouble();

            // Call
            var output = new PipingSemiProbabilisticOutput(
                upliftFactorOfSafety,
                upliftReliability,
                upliftProbability,
                heaveFactorOfSafety,
                heaveReliability,
                heaveProbability,
                sellmeijerFactorOfSafety,
                sellmeijerReliability,
                sellmeijerProbability,
                requiredProbability,
                requiredReliability,
                pipingProbability,
                pipingReliability,
                pipingFactorOfSafety);

            // Assert
            Assert.AreEqual(upliftFactorOfSafety, output.UpliftFactorOfSafety, output.UpliftFactorOfSafety.GetAccuracy());
            Assert.AreEqual(upliftReliability, output.UpliftReliability, output.UpliftReliability.GetAccuracy());
            Assert.AreEqual(upliftProbability, output.UpliftProbability);
            Assert.AreEqual(heaveFactorOfSafety, output.HeaveFactorOfSafety, output.HeaveFactorOfSafety.GetAccuracy());
            Assert.AreEqual(heaveReliability, output.HeaveReliability, output.HeaveReliability.GetAccuracy());
            Assert.AreEqual(heaveProbability, output.HeaveProbability);
            Assert.AreEqual(sellmeijerFactorOfSafety, output.SellmeijerFactorOfSafety, output.SellmeijerFactorOfSafety.GetAccuracy());
            Assert.AreEqual(sellmeijerReliability, output.SellmeijerReliability, output.SellmeijerReliability.GetAccuracy());
            Assert.AreEqual(sellmeijerProbability, output.SellmeijerProbability);
            Assert.AreEqual(requiredProbability, output.RequiredProbability);
            Assert.AreEqual(5, output.RequiredReliability.NumberOfDecimalPlaces);
            Assert.AreEqual(requiredReliability, output.RequiredReliability, output.RequiredReliability.GetAccuracy());
            Assert.AreEqual(pipingProbability, output.PipingProbability);
            Assert.AreEqual(5, output.PipingReliability.NumberOfDecimalPlaces);
            Assert.AreEqual(pipingReliability, output.PipingReliability, output.PipingReliability.GetAccuracy());
            Assert.AreEqual(3, output.PipingFactorOfSafety.NumberOfDecimalPlaces);
            Assert.AreEqual(pipingFactorOfSafety, output.PipingFactorOfSafety, output.PipingFactorOfSafety.GetAccuracy());
        }

        [Test]
        [TestCase(double.NaN)]
        [TestCase(0.0)]
        [TestCase(0.123456789)]
        [TestCase(1.0)]
        public void RequiredProbability_SetValidValues_ReturnNewlySetValue(double requiredProbability)
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
            double requiredReliability = random.NextDouble();
            double pipingProbability = random.NextDouble();
            double pipingReliability = random.NextDouble();
            double pipingFactorOfSafety = random.NextDouble();

            // Call
            var output = new PipingSemiProbabilisticOutput(
                upliftFactorOfSafety,
                upliftReliability,
                upliftProbability,
                heaveFactorOfSafety,
                heaveReliability,
                heaveProbability,
                sellmeijerFactorOfSafety,
                sellmeijerReliability,
                sellmeijerProbability,
                requiredProbability,
                requiredReliability,
                pipingProbability,
                pipingReliability,
                pipingFactorOfSafety);

            // Assert
            Assert.AreEqual(requiredProbability, output.RequiredProbability);
        }

        [Test]
        [TestCase(double.PositiveInfinity)]
        [TestCase(double.NegativeInfinity)]
        [TestCase(0.0 - 1e-6)]
        [TestCase(-346587.456)]
        [TestCase(1.0 + 1e-6)]
        [TestCase(346587.456)]
        public void RequiredProbability_SetInvalidValues_ThrowArgumentOutOfRangeException(double requiredProbability)
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
            double requiredReliability = random.NextDouble();
            double pipingProbability = random.NextDouble();
            double pipingReliability = random.NextDouble();
            double pipingFactorOfSafety = random.NextDouble();

            // Call
            TestDelegate call = () => new PipingSemiProbabilisticOutput(
                                          upliftFactorOfSafety,
                                          upliftReliability,
                                          upliftProbability,
                                          heaveFactorOfSafety,
                                          heaveReliability,
                                          heaveProbability,
                                          sellmeijerFactorOfSafety,
                                          sellmeijerReliability,
                                          sellmeijerProbability,
                                          requiredProbability,
                                          requiredReliability,
                                          pipingProbability,
                                          pipingReliability,
                                          pipingFactorOfSafety);

            // Assert
            const string expectedMessage = "Kans moet in het bereik [0, 1] liggen.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(call, expectedMessage);
        }

        [Test]
        [TestCase(double.NaN)]
        [TestCase(0.0)]
        [TestCase(0.123456789)]
        [TestCase(1.0)]
        public void SellmeijerProbability_SetValidValues_ReturnNewlySetValue(double sellmeijerProbability)
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
            double requiredProbability = random.NextDouble();
            double requiredReliability = random.NextDouble();
            double pipingProbability = random.NextDouble();
            double pipingReliability = random.NextDouble();
            double pipingFactorOfSafety = random.NextDouble();

            // Call
            var output = new PipingSemiProbabilisticOutput(
                upliftFactorOfSafety,
                upliftReliability,
                upliftProbability,
                heaveFactorOfSafety,
                heaveReliability,
                heaveProbability,
                sellmeijerFactorOfSafety,
                sellmeijerReliability,
                sellmeijerProbability,
                requiredProbability,
                requiredReliability,
                pipingProbability,
                pipingReliability,
                pipingFactorOfSafety);

            // Assert
            Assert.AreEqual(sellmeijerProbability, output.SellmeijerProbability);
        }

        [Test]
        [TestCase(double.PositiveInfinity)]
        [TestCase(double.NegativeInfinity)]
        [TestCase(0.0 - 1e-6)]
        [TestCase(-346587.456)]
        [TestCase(1.0 + 1e-6)]
        [TestCase(346587.456)]
        public void SellmeijerProbability_SetInvalidValues_ThrowArgumentOutOfRangeException(double sellmeijerProbability)
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
            double requiredProbability = random.NextDouble();
            double requiredReliability = random.NextDouble();
            double pipingProbability = random.NextDouble();
            double pipingReliability = random.NextDouble();
            double pipingFactorOfSafety = random.NextDouble();

            // Call
            TestDelegate call = () => new PipingSemiProbabilisticOutput(
                                          upliftFactorOfSafety,
                                          upliftReliability,
                                          upliftProbability,
                                          heaveFactorOfSafety,
                                          heaveReliability,
                                          heaveProbability,
                                          sellmeijerFactorOfSafety,
                                          sellmeijerReliability,
                                          sellmeijerProbability,
                                          requiredProbability,
                                          requiredReliability,
                                          pipingProbability,
                                          pipingReliability,
                                          pipingFactorOfSafety);

            // Assert
            const string expectedMessage = "Kans moet in het bereik [0, 1] liggen.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(call, expectedMessage);
        }

        [Test]
        [TestCase(double.NaN)]
        [TestCase(0.0)]
        [TestCase(0.123456789)]
        [TestCase(1.0)]
        public void HeaveProbability_SetValidValues_ReturnNewlySetValue(double heaveProbability)
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
            double requiredProbability = random.NextDouble();
            double requiredReliability = random.NextDouble();
            double pipingProbability = random.NextDouble();
            double pipingReliability = random.NextDouble();
            double pipingFactorOfSafety = random.NextDouble();

            // Call
            var output = new PipingSemiProbabilisticOutput(
                upliftFactorOfSafety,
                upliftReliability,
                upliftProbability,
                heaveFactorOfSafety,
                heaveReliability,
                heaveProbability,
                sellmeijerFactorOfSafety,
                sellmeijerReliability,
                sellmeijerProbability,
                requiredProbability,
                requiredReliability,
                pipingProbability,
                pipingReliability,
                pipingFactorOfSafety);

            // Assert
            Assert.AreEqual(heaveProbability, output.HeaveProbability);
        }

        [Test]
        [TestCase(double.PositiveInfinity)]
        [TestCase(double.NegativeInfinity)]
        [TestCase(0.0 - 1e-2)]
        [TestCase(-346587.456)]
        [TestCase(1.0 + 1e-2)]
        [TestCase(346587.456)]
        public void HeaveProbability_SetInvalidValues_ThrowArgumentOutOfRangeException(double heaveProbability)
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
            double requiredProbability = random.NextDouble();
            double requiredReliability = random.NextDouble();
            double pipingProbability = random.NextDouble();
            double pipingReliability = random.NextDouble();
            double pipingFactorOfSafety = random.NextDouble();

            // Call
            TestDelegate call = () => new PipingSemiProbabilisticOutput(
                                          upliftFactorOfSafety,
                                          upliftReliability,
                                          upliftProbability,
                                          heaveFactorOfSafety,
                                          heaveReliability,
                                          heaveProbability,
                                          sellmeijerFactorOfSafety,
                                          sellmeijerReliability,
                                          sellmeijerProbability,
                                          requiredProbability,
                                          requiredReliability,
                                          pipingProbability,
                                          pipingReliability,
                                          pipingFactorOfSafety);

            // Assert
            const string expectedMessage = "Kans moet in het bereik [0, 1] liggen.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(call, expectedMessage);
        }

        [Test]
        [TestCase(double.NaN)]
        [TestCase(0.0)]
        [TestCase(0.123456789)]
        [TestCase(1.0)]
        public void UpliftProbability_SetValidValues_ReturnNewlySetValue(double upliftProbability)
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
            double requiredProbability = random.NextDouble();
            double requiredReliability = random.NextDouble();
            double pipingProbability = random.NextDouble();
            double pipingReliability = random.NextDouble();
            double pipingFactorOfSafety = random.NextDouble();

            // Call
            var output = new PipingSemiProbabilisticOutput(
                upliftFactorOfSafety,
                upliftReliability,
                upliftProbability,
                heaveFactorOfSafety,
                heaveReliability,
                heaveProbability,
                sellmeijerFactorOfSafety,
                sellmeijerReliability,
                sellmeijerProbability,
                requiredProbability,
                requiredReliability,
                pipingProbability,
                pipingReliability,
                pipingFactorOfSafety);

            // Assert
            Assert.AreEqual(upliftProbability, output.UpliftProbability);
        }

        [Test]
        [TestCase(double.PositiveInfinity)]
        [TestCase(double.NegativeInfinity)]
        [TestCase(0.0 - 1e-6)]
        [TestCase(-346587.456)]
        [TestCase(1.0 + 1e-6)]
        [TestCase(346587.456)]
        public void UpliftProbability_SetInvalidValues_ThrowArgumentOutOfRangeException(double upliftProbability)
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
            double requiredProbability = random.NextDouble();
            double requiredReliability = random.NextDouble();
            double pipingProbability = random.NextDouble();
            double pipingReliability = random.NextDouble();
            double pipingFactorOfSafety = random.NextDouble();

            // Call
            TestDelegate call = () => new PipingSemiProbabilisticOutput(
                                          upliftFactorOfSafety,
                                          upliftReliability,
                                          upliftProbability,
                                          heaveFactorOfSafety,
                                          heaveReliability,
                                          heaveProbability,
                                          sellmeijerFactorOfSafety,
                                          sellmeijerReliability,
                                          sellmeijerProbability,
                                          requiredProbability,
                                          requiredReliability,
                                          pipingProbability,
                                          pipingReliability,
                                          pipingFactorOfSafety);

            // Assert
            const string expectedMessage = "Kans moet in het bereik [0, 1] liggen.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(call, expectedMessage);
        }

        [Test]
        [TestCase(double.NaN)]
        [TestCase(0.0)]
        [TestCase(0.123456789)]
        [TestCase(1.0)]
        public void PipingProbability_SetValidValues_ReturnNewlySetValue(double pipingProbability)
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
            double requiredProbability = random.NextDouble();
            double requiredReliability = random.NextDouble();
            double pipingReliability = random.NextDouble();
            double pipingFactorOfSafety = random.NextDouble();

            // Call
            var output = new PipingSemiProbabilisticOutput(
                upliftFactorOfSafety,
                upliftReliability,
                upliftProbability,
                heaveFactorOfSafety,
                heaveReliability,
                heaveProbability,
                sellmeijerFactorOfSafety,
                sellmeijerReliability,
                sellmeijerProbability,
                requiredProbability,
                requiredReliability,
                pipingProbability,
                pipingReliability,
                pipingFactorOfSafety);

            // Assert
            Assert.AreEqual(pipingProbability, output.PipingProbability);
        }

        [Test]
        [TestCase(double.PositiveInfinity)]
        [TestCase(double.NegativeInfinity)]
        [TestCase(0.0 - 1e-2)]
        [TestCase(-346587.456)]
        [TestCase(1.0 + 1e-2)]
        [TestCase(346587.456)]
        public void PipingProbability_SetInvalidValues_ThrowArgumentOutOfRangeException(double pipingProbability)
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
            double requiredProbability = random.NextDouble();
            double requiredReliability = random.NextDouble();
            double pipingReliability = random.NextDouble();
            double pipingFactorOfSafety = random.NextDouble();

            // Call
            TestDelegate call = () => new PipingSemiProbabilisticOutput(
                                          upliftFactorOfSafety,
                                          upliftReliability,
                                          upliftProbability,
                                          heaveFactorOfSafety,
                                          heaveReliability,
                                          heaveProbability,
                                          sellmeijerFactorOfSafety,
                                          sellmeijerReliability,
                                          sellmeijerProbability,
                                          requiredProbability,
                                          requiredReliability,
                                          pipingProbability,
                                          pipingReliability,
                                          pipingFactorOfSafety);

            // Assert
            const string expectedMessage = "Kans moet in het bereik [0, 1] liggen.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(call, expectedMessage);
        }
    }
}