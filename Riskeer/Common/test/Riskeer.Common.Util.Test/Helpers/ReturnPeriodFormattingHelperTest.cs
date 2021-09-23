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

using System;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Util.Helpers;

namespace Riskeer.Common.Util.Test.Helpers
{
    [TestFixture]
    public class ReturnPeriodFormattingHelperTest
    {
        [Test]
        [TestCase(0)]
        [TestCase(-0.1)]
        public void FormatFromProbability_InvalidValue_ThrowsArgumentOutOfRangeException(double probability)
        {
            // Call
            void Call() => ReturnPeriodFormattingHelper.FormatFromProbability(probability);
            
            // Assert
            const string message = "Probability must be larger than 0.0.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(Call, message);
        }

        [Test]
        [TestCase(1.0, "1")]
        [TestCase(0.5, "2")]
        [TestCase(0.6, "2")]
        [TestCase(0.0001, "10.000")]
        [TestCase(0.000000123456789, "8.100.000")]
        public void FormatFromProbability_ValidValue_ReturnsReturnPeriod(double probability, string expectedText)
        {
            // Call
            string returnPeriod = ReturnPeriodFormattingHelper.FormatFromProbability(probability);
            
            // Assert
            Assert.AreEqual(expectedText, returnPeriod);
        }
    }
}