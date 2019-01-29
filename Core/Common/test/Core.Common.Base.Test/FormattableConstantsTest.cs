// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System.Globalization;
using NUnit.Framework;

namespace Core.Common.Base.Test
{
    [TestFixture]
    public class FormattableConstantsTest
    {
        [Test]
        [SetCulture("nl-NL")]
        [TestCase(123.456, "123,456")]
        [TestCase(0, "0,0")]
        [TestCase(.700, "0,7")]
        public void GivenNumber_WhenFormattingToTextUsingShowAtLeastOneDecimal_ThenReturnExpectedResult(double value, string expectedText)
        {
            // When
            string text = value.ToString(FormattableConstants.ShowAtLeastOneDecimal, CultureInfo.CurrentCulture);

            // Then
            Assert.AreEqual(expectedText, text);
        }
    }
}