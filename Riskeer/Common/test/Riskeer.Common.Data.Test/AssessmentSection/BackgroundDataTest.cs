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
using Core.Common.Base.Data;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.TestUtil;

namespace Riskeer.Common.Data.Test.AssessmentSection
{
    [TestFixture]
    public class BackgroundDataTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var configuration = new TestBackgroundDataConfiguration();

            // Call 
            var backgroundData = new BackgroundData(configuration);

            // Assert
            Assert.IsNull(backgroundData.Name);
            Assert.IsTrue(backgroundData.IsVisible);
            Assert.AreEqual(2, backgroundData.Transparency.NumberOfDecimalPlaces);
            Assert.AreEqual(0.60, backgroundData.Transparency.Value);
            Assert.AreSame(configuration, backgroundData.Configuration);
        }

        [Test]
        public void Constructor_ConfigurationNull_ThrowArgumentNullException()
        {
            // Call 
            TestDelegate test = () => new BackgroundData(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("configuration", exception.ParamName);
        }

        [Test]
        [TestCase(0)]
        [TestCase(0.8)]
        [TestCase(1)]
        public void Transparency_ValidValues_ReturnNewlySetValue(double newValue)
        {
            // Setup
            var backgroundData = new BackgroundData(new TestBackgroundDataConfiguration());
            int originalNumberOfDecimals = backgroundData.Transparency.NumberOfDecimalPlaces;

            // Call
            backgroundData.Transparency = (RoundedDouble) newValue;

            // Assert
            Assert.AreEqual(newValue, backgroundData.Transparency.Value);
            Assert.AreEqual(originalNumberOfDecimals, backgroundData.Transparency.NumberOfDecimalPlaces);
        }

        [Test]
        [SetCulture("nl-NL")]
        [TestCase(-123.56)]
        [TestCase(0.0 - 1e-2)]
        [TestCase(1.0 + 1e-2)]
        [TestCase(456.876)]
        [TestCase(double.NaN)]
        public void Transparency_SetInvalidValue_ThrowArgumentOutOfRangeException(double invalidTransparency)
        {
            // Setup
            var backgroundData = new BackgroundData(new TestBackgroundDataConfiguration());

            // Call
            TestDelegate call = () => backgroundData.Transparency = (RoundedDouble) invalidTransparency;

            // Assert
            const string message = "De transparantie moet in het bereik [0,00, 1,00] liggen.";
            string paramName = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(call, message).ParamName;
            Assert.AreEqual("value", paramName);
        }
    }
}