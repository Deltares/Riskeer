// Copyright (C) Stichting Deltares 2021. All rights reserved.
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

using NUnit.Framework;
using Riskeer.Common.Data.Probability;
using Riskeer.Common.Data.TestUtil;

namespace Riskeer.Common.Data.Test.Probability
{
    [TestFixture]
    public class ProbabilityAssessmentOutputFactoryTest
    {
        [Test]
        [TestCase(1.23456, 1.23456)]
        [TestCase(789.123, 789.123)]
        public void Reliability_DifferentInputs_ReturnsExpectedValue(double reliability, double expectedResult)
        {
            // Call
            ProbabilityAssessmentOutput probabilityAssessmentOutput = ProbabilityAssessmentOutputFactory.Create(reliability);

            // Assert
            Assert.AreEqual(expectedResult, probabilityAssessmentOutput.Reliability, probabilityAssessmentOutput.Reliability.GetAccuracy());
        }

        [Test]
        [TestCase(4, 0.00003167124)]
        [TestCase(5, 0.00000028665)]
        public void Probability_DifferentInputs_ReturnsExpectedValue(double reliability, double expectedResult)
        {
            // Call
            ProbabilityAssessmentOutput probabilityAssessmentOutput = ProbabilityAssessmentOutputFactory.Create(reliability);

            // Assert
            Assert.AreEqual(expectedResult, probabilityAssessmentOutput.Probability, 1e-6);
        }
    }
}