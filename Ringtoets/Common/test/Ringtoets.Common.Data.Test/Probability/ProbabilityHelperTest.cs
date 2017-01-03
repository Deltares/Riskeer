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

using NUnit.Framework;
using Ringtoets.Common.Data.Probability;

namespace Ringtoets.Common.Data.Test.Probability
{
    [TestFixture]
    public class ProbabilityHelperTest
    {
        [Test]
        [TestCase(0)]
        [TestCase(1.0)]
        [TestCase(double.NaN)]
        public void IsValidProbability_ValidProbability_ReturnTrue(double value)
        {
            // Call
            bool isValid = ProbabilityHelper.IsValidProbability(value);

            // Assert
            Assert.IsTrue(isValid);
        }

        [Test]
        [TestCase(-1e-6)]
        [TestCase(1.0 + 1e-6)]
        public void IsValidProbability_InvalidProbability_ReturnFalse(double value)
        {
            // Call
            bool isValid = ProbabilityHelper.IsValidProbability(value);

            // Assert
            Assert.IsFalse(isValid);
        }
    }
}