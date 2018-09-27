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

using NUnit.Framework;
using Ringtoets.Common.Data.Probability;
using Ringtoets.Common.Data.TestUtil;

namespace Ringtoets.Common.Data.Test.Probability
{
    [TestFixture]
    public class ProbabilityAssessmentOutputFactoryTest
    {
        [Test]
        [TestCase(30000, 100, 2, 0.00001666667)]
        [TestCase(30000, 100, 1, 0.00003333333)]
        [TestCase(30000, 24, 2, 0.00000400000)]
        [TestCase(30000, 24, 1, 0.00000800000)]
        [TestCase(20000, 100, 2, 0.00002500000)]
        [TestCase(20000, 100, 1, 0.00005000000)]
        [TestCase(20000, 24, 2, 0.00000600000)]
        [TestCase(20000, 24, 1, 0.00001200000)]
        public void RequiredProbability_DifferentInputs_ReturnsExpectedValue(int returnPeriod,
                                                                             double contribution,
                                                                             int lengthEffectN,
                                                                             double expectedResult)
        {
            // Setup
            double norm = 1.0 / returnPeriod;

            // Call
            ProbabilityAssessmentOutput probabilityAssessmentOutput = ProbabilityAssessmentOutputFactory.Create(
                norm,
                contribution,
                lengthEffectN,
                double.NaN);

            // Assert
            Assert.AreEqual(expectedResult, probabilityAssessmentOutput.RequiredProbability, 1e-6);
        }

        [Test]
        [TestCase(30000, 100, 2, 4.149409984)]
        [TestCase(30000, 100, 1, 3.987878937)]
        [TestCase(30000, 24, 2, 4.465183916)]
        [TestCase(30000, 24, 1, 4.314451022)]
        [TestCase(20000, 100, 2, 4.055626981)]
        [TestCase(20000, 100, 1, 3.890591886)]
        [TestCase(20000, 24, 2, 4.377587847)]
        [TestCase(20000, 24, 1, 4.2240038)]
        public void RequiredReliability_DifferentInputs_ReturnsExpectedValue(int returnPeriod,
                                                                             double contribution,
                                                                             int lengthEffectN,
                                                                             double expectedResult)
        {
            // Setup
            double norm = 1.0 / returnPeriod;

            // Call
            ProbabilityAssessmentOutput probabilityAssessmentOutput = ProbabilityAssessmentOutputFactory.Create(
                norm,
                contribution,
                lengthEffectN,
                double.NaN);

            // Assert
            Assert.AreEqual(expectedResult, probabilityAssessmentOutput.RequiredReliability,
                            probabilityAssessmentOutput.RequiredReliability.GetAccuracy());
        }

        [Test]
        [TestCase(1.23456, 1.23456)]
        [TestCase(789.123, 789.123)]
        public void Reliability_DifferentInputs_ReturnsExpectedValue(double reliability, double expectedResult)
        {
            // Call
            ProbabilityAssessmentOutput probabilityAssessmentOutput = ProbabilityAssessmentOutputFactory.Create(
                int.MinValue,
                double.NaN,
                double.NaN,
                reliability);

            // Assert
            Assert.AreEqual(expectedResult, probabilityAssessmentOutput.Reliability, probabilityAssessmentOutput.Reliability.GetAccuracy());
        }

        [Test]
        [TestCase(4, 0.00003167124)]
        [TestCase(5, 0.00000028665)]
        public void Probability_DifferentInputs_ReturnsExpectedValue(double reliability, double expectedResult)
        {
            // Call
            ProbabilityAssessmentOutput probabilityAssessmentOutput = ProbabilityAssessmentOutputFactory.Create(
                int.MinValue,
                double.NaN,
                double.NaN,
                reliability);

            // Assert
            Assert.AreEqual(expectedResult, probabilityAssessmentOutput.Probability, 1e-6);
        }

        [Test]
        [TestCase(30000, 100, 2, 4.107479655, 0.989894869)]
        [TestCase(30000, 100, 2, 4.149409984, 1)]
        [TestCase(30000, 24, 2, 4.107479655, 0.919890363)]
        [TestCase(30000, 24, 2, 4.149409984, 0.929280868)]
        [TestCase(20000, 100, 2, 4.107479655, 1.012785366)]
        [TestCase(20000, 100, 2, 4.149409984, 1.023124169)]
        [TestCase(20000, 24, 2, 4.107479655, 0.938297482)]
        [TestCase(20000, 24, 2, 4.149409984, 0.947875892)]
        public void FactorOfSafety_DifferentInputs_ReturnsExpectedValue(int returnPeriod, double contribution,
                                                                        int lengthEffectN, double reliability,
                                                                        double expectedResult)
        {
            // Setup
            double norm = 1.0 / returnPeriod;

            // Call
            ProbabilityAssessmentOutput probabilityAssessmentOutput = ProbabilityAssessmentOutputFactory.Create(
                norm,
                contribution,
                lengthEffectN,
                reliability);

            // Assert
            Assert.AreEqual(expectedResult, probabilityAssessmentOutput.FactorOfSafety, probabilityAssessmentOutput.FactorOfSafety.GetAccuracy());
        }
    }
}