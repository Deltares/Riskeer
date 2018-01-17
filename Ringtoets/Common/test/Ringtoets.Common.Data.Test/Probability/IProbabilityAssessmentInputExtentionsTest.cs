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
using NUnit.Framework;
using Ringtoets.Common.Data.Probability;

namespace Ringtoets.Common.Data.Test.Probability
{
    [TestFixture]
    public class IProbabilityAssessmentInputExtentionsTest
    {
        [Test]
        public void GetSectionSpecificN_ProbabilityAssessmentInputNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => ((IProbabilityAssessmentInput) null).GetSectionSpecificN(new Random(39).NextDouble());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("probabilityAssessmentInput", exception.ParamName);
        }

        [Test]
        [TestCase(0.2, 100, 100, 1.2)]
        [TestCase(2.5, 750, 300, 2)]
        [TestCase(-4, -200, 750, 16.0)]
        public void GetSectionSpecificN_WithValues_ReturnsExpectedResult(double a, double b, double length, double expectedN)
        {
            // Setup
            var probabilityAssessmentInput = new TestProbabilityAssessmentInput(a, b);
            
            // Call
            double actualN = probabilityAssessmentInput.GetSectionSpecificN(length);

            // Assert
            Assert.AreEqual(expectedN, actualN);
        }

        private class TestProbabilityAssessmentInput : IProbabilityAssessmentInput
        {
            public TestProbabilityAssessmentInput(double a, double b)
            {
                A = a;
                B = b;
            }

            public double A { get; }
            public double B { get; }
        }
    }
}