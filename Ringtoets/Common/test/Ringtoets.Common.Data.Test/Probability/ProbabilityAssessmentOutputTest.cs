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
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.Probability;
using Ringtoets.Common.Data.TestUtil;

namespace Ringtoets.Common.Data.Test.Probability
{
    [TestFixture]
    public class ProbabilityAssessmentOutputTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var random = new Random(5);
            double requiredProbability = random.NextDouble();
            double requiredReliability = random.NextDouble();
            double probability = random.NextDouble();
            double reliability = random.NextDouble();
            double factorOfSafety = random.NextDouble();

            // Call
            var probabilityAssessmentOutput = new ProbabilityAssessmentOutput(requiredProbability, requiredReliability, probability, reliability, factorOfSafety);

            // Assert
            Assert.AreEqual(requiredProbability, probabilityAssessmentOutput.RequiredProbability);
            Assert.AreEqual(5, probabilityAssessmentOutput.RequiredReliability.NumberOfDecimalPlaces);
            Assert.AreEqual(requiredReliability, probabilityAssessmentOutput.RequiredReliability, probabilityAssessmentOutput.RequiredReliability.GetAccuracy());
            Assert.AreEqual(probability, probabilityAssessmentOutput.Probability);
            Assert.AreEqual(5, probabilityAssessmentOutput.Reliability.NumberOfDecimalPlaces);
            Assert.AreEqual(reliability, probabilityAssessmentOutput.Reliability, probabilityAssessmentOutput.Reliability.GetAccuracy());
            Assert.AreEqual(3, probabilityAssessmentOutput.FactorOfSafety.NumberOfDecimalPlaces);
            Assert.AreEqual(factorOfSafety, probabilityAssessmentOutput.FactorOfSafety, probabilityAssessmentOutput.FactorOfSafety.GetAccuracy());
        }

        [Test]
        [TestCase(double.NaN)]
        [TestCase(0.0)]
        [TestCase(0.123456789)]
        [TestCase(1.0)]
        public void RequiredProbability_SetToValidValue_GetValidValue(double requiredProbability)
        {
            // Setup
            var random = new Random(5);
            double requiredReliability = random.NextDouble();
            double probability = random.NextDouble();
            double reliability = random.NextDouble();
            double factorOfSafety = random.NextDouble();

            // Call
            var output = new ProbabilityAssessmentOutput(requiredProbability,
                                                         requiredReliability,
                                                         probability,
                                                         reliability,
                                                         factorOfSafety);

            // Assert
            Assert.AreEqual(requiredProbability, output.RequiredProbability);
        }

        [Test]
        [SetCulture("nl-NL")]
        [TestCase(double.PositiveInfinity)]
        [TestCase(double.NegativeInfinity)]
        [TestCase(-1e-6)]
        [TestCase(-346587.456)]
        [TestCase(1.0 + 1e-6)]
        [TestCase(346587.456)]
        public void RequiredProbability_SetToInvalidValue_ThrowArgumentOutOfRangeException(double requiredProbability)
        {
            // Setup
            var random = new Random(5);
            double requiredReliability = random.NextDouble();
            double probability = random.NextDouble();
            double reliability = random.NextDouble();
            double factorOfSafety = random.NextDouble();

            // Call
            TestDelegate call = () => new ProbabilityAssessmentOutput(requiredProbability,
                                                                      requiredReliability,
                                                                      probability,
                                                                      reliability,
                                                                      factorOfSafety);

            // Assert
            const string expectedMessage = "Kans moet in het bereik [0,0, 1,0] liggen.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(call, expectedMessage);
        }

        [Test]
        [TestCase(double.NaN)]
        [TestCase(0.0)]
        [TestCase(0.123456789)]
        [TestCase(1.0)]
        public void Probability_SetToValidValue_GetValidValue(double probability)
        {
            // Setup
            var random = new Random(5);
            double requiredProbability = random.NextDouble();
            double requiredReliability = random.NextDouble();
            double reliability = random.NextDouble();
            double factorOfSafety = random.NextDouble();

            // Call
            var output = new ProbabilityAssessmentOutput(requiredProbability,
                                                         requiredReliability,
                                                         probability,
                                                         reliability,
                                                         factorOfSafety);

            // Assert
            Assert.AreEqual(requiredProbability, output.RequiredProbability);
        }

        [Test]
        [SetCulture("nl-NL")]
        [TestCase(double.PositiveInfinity)]
        [TestCase(double.NegativeInfinity)]
        [TestCase(-1e-6)]
        [TestCase(-346587.456)]
        [TestCase(1.0 + 1e-6)]
        [TestCase(346587.456)]
        public void Probability_SetToInvalidValue_ThrowArgumentOutOfRangeException(double probability)
        {
            // Setup
            var random = new Random(5);
            double requiredProbability = random.NextDouble();
            double requiredReliability = random.NextDouble();
            double reliability = random.NextDouble();
            double factorOfSafety = random.NextDouble();

            // Call
            TestDelegate call = () => new ProbabilityAssessmentOutput(requiredProbability,
                                                                      requiredReliability,
                                                                      probability,
                                                                      reliability,
                                                                      factorOfSafety);

            // Assert
            const string expectedMessage = "Kans moet in het bereik [0,0, 1,0] liggen.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(call, expectedMessage);
        }
    }
}