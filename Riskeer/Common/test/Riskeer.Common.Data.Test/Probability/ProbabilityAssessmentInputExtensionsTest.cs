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
using NUnit.Framework;
using Riskeer.Common.Data.Probability;
using Riskeer.Common.Data.TestUtil.Probability;

namespace Riskeer.Common.Data.Test.Probability
{
    [TestFixture]
    public class ProbabilityAssessmentInputExtensionsTest
    {
        [Test]
        public void GetN_ProbabilityAssessmentInputNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => ((ProbabilityAssessmentInput) null).GetN(new Random(39).NextDouble());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("probabilityAssessmentInput", exception.ParamName);
        }

        [Test]
        [TestCase(0.2, 100, 100, 1.2)]
        [TestCase(0.5, 750, 300, 1.2)]
        [TestCase(0.9, -200, 750, -2.375)]
        [TestCase(0.8, 0, 100, double.PositiveInfinity)]
        public void GetN_WithValues_ReturnsExpectedResult(double a, double b, double length, double expectedN)
        {
            // Setup
            var input = new TestProbabilityAssessmentInput(a, b);

            // Call
            double actualN = input.GetN(length);

            // Assert
            Assert.AreEqual(expectedN, actualN);
        }
    }
}