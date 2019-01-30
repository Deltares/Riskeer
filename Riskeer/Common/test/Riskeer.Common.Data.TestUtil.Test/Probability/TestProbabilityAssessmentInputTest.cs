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

namespace Riskeer.Common.Data.TestUtil.Test.Probability
{
    [TestFixture]
    public class TestProbabilityAssessmentInputTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var random = new Random(39);
            double a = random.NextDouble();
            double b = random.NextDouble();

            // Call
            var probabilityAssessmentInput = new TestProbabilityAssessmentInput(a, b);

            // Assert
            Assert.IsInstanceOf<ProbabilityAssessmentInput>(probabilityAssessmentInput);
            Assert.AreEqual(a, probabilityAssessmentInput.A);
            Assert.AreEqual(b, probabilityAssessmentInput.B);
        }
    }
}