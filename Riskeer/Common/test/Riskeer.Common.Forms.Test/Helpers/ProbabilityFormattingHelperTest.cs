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

using NUnit.Framework;
using Riskeer.Common.Forms.Helpers;

namespace Riskeer.Common.Forms.Test.Helpers
{
    [TestFixture]
    public class ProbabilityFormattingHelperTest
    {
        [Test]
        public void Format_ProbabilityIsZero_ReturnOneOverInfinity()
        {
            // Call
            string text = ProbabilityFormattingHelper.Format(0);

            // Assert
            Assert.AreEqual("1/Oneindig", text);
        }

        [Test]
        [SetCulture("nl-NL")]
        [TestCase(1.0, "1/1")]
        [TestCase(0.5, "1/2")]
        [TestCase(0.6, "1/2")]
        [TestCase(0.0001, "1/10.000")]
        [TestCase(0.000000123456789, "1/8.100.000")]
        [TestCase(-0.0001, "1/-10.000")]
        [TestCase(-0.5, "1/-2")]
        public void Format_ProbabilityNotZero_ReturnOneOverReturnPeriod(double probability, string expectedText)
        {
            // Call
            string text = ProbabilityFormattingHelper.Format(probability);

            // Assert
            Assert.AreEqual(expectedText, text);
        }

        [Test]
        public void FormatFromReturnPeriod_ReturnPeriodIsZero_ReturnOneOverInfinity()
        {
            // Call
            string text = ProbabilityFormattingHelper.FormatFromReturnPeriod(0);

            // Assert
            Assert.AreEqual("1/Oneindig", text);
        }

        [Test]
        [SetCulture("nl-NL")]
        [TestCase(1, "1/1")]
        [TestCase(2, "1/2")]
        [TestCase(10000, "1/10.000")]
        [TestCase(8100000, "1/8.100.000")]
        [TestCase(-1000, "1/-1.000")]
        [TestCase(-2, "1/-2")]
        public void FormatFromReturnPeriod_ReturnPeriodNotZero_ReturnOneOverReturnPeriod(int returnPeriod, string expectedText)
        {
            // Call
            string text = ProbabilityFormattingHelper.FormatFromReturnPeriod(returnPeriod);

            // Assert
            Assert.AreEqual(expectedText, text);
        }
    }
}