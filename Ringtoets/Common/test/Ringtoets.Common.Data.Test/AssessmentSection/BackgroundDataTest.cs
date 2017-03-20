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

using System;
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.Data;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.AssessmentSection;

namespace Ringtoets.Common.Data.Test.AssessmentSection
{
    [TestFixture]
    public class BackgroundDataTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call 
            var backgroundData = new BackgroundData();

            // Assert
            Assert.IsNull(backgroundData.Name);
            Assert.IsTrue(backgroundData.IsVisible);
            Assert.AreEqual(2, backgroundData.Transparency.NumberOfDecimalPlaces);
            Assert.AreEqual(0, backgroundData.Transparency.Value);
            Assert.AreEqual(0, (int) backgroundData.BackgroundMapDataType);
            Assert.IsFalse(backgroundData.IsConfigured);
            CollectionAssert.IsEmpty(backgroundData.Parameters);
        }
        
        [Test]
        [TestCase(0)]
        [TestCase(0.8)]
        [TestCase(1)]
        public void Transparency_ValidValues_ReturnNewlySetValue(double newValue)
        {
            // Setup
            var backgroundData = new BackgroundData();
            var originalNumberOfDecimals = backgroundData.Transparency.NumberOfDecimalPlaces;

            // Call
            backgroundData.Transparency = (RoundedDouble)newValue;

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
            var backgroundData = new BackgroundData();

            // Call
            TestDelegate call = () => backgroundData.Transparency = (RoundedDouble)invalidTransparency;

            // Assert
            var message = "De transparantie moet in het bereik [0,00, 1,00] liggen.";
            string paramName = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(call, message).ParamName;
            Assert.AreEqual("value", paramName);
        }

        [Test]
        [TestCase("SourceCapabilitiesUrl")]
        [TestCase("SelectedCapabilityIdentifier")]
        [TestCase("PreferredFormat")]
        [TestCase("WellKnownTileSource")]
        public void Parameters_AllowedKeys_ItemAddedToDictionary(string allowedKey)
        {
            // Setup
            const string value = "some value";
            var backgroundData = new BackgroundData();

            // Precondition
            CollectionAssert.IsEmpty(backgroundData.Parameters);

            // Call
            backgroundData.Parameters.Add(allowedKey, value);

            // Assert
            Assert.AreEqual(1, backgroundData.Parameters.Count);
            KeyValuePair<string, string> item = backgroundData.Parameters.First();
            Assert.AreEqual(allowedKey, item.Key);
            Assert.AreEqual(value, item.Value);
        }

        [Test]
        public void Parameters_AddOtherThanAllowed_ThrowInvalidOperationException()
        {
            // Setup
            var backgroundData = new BackgroundData();
            
            // Precondition
            CollectionAssert.IsEmpty(backgroundData.Parameters);

            // Call
            TestDelegate test = () => backgroundData.Parameters.Add("invalid key", "test");

            // Assert
            var exception = Assert.Throws<InvalidOperationException>(test);
            Assert.AreEqual("Key 'invalid key' is not allowed to add to the dictionary.", exception.Message);
            CollectionAssert.IsEmpty(backgroundData.Parameters);
        }

        [Test]
        public void Parameters_AddOtherThanAllowed_ThrowInvalidOperationExceptions()
        {
            // Setup
            var backgroundData = new BackgroundData();
            
            // Precondition
            CollectionAssert.IsEmpty(backgroundData.Parameters);

            // Call
            TestDelegate test = () => backgroundData.Parameters["invalid key"] = "test";

            // Assert
            var exception = Assert.Throws<InvalidOperationException>(test);
            Assert.AreEqual("Key 'invalid key' is not allowed to add to the dictionary.", exception.Message);
            CollectionAssert.IsEmpty(backgroundData.Parameters);
        }
    }
}