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

using System.ComponentModel;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Forms.PropertyClasses;
using Riskeer.MacroStabilityInwards.Data;
using Riskeer.MacroStabilityInwards.Forms.PropertyClasses;

namespace Riskeer.MacroStabilityInwards.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class MacroStabilityInwardsLocationInputDailyPropertiesTest
    {
        private const int expectedwaterLevelPolderPropertyIndex = 0;
        private const int expectedOffsetPropertyIndex = 1;
        private const int expectedPenetrationLengthPropertyIndex = 2;

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var changeHandler = mocks.Stub<IObservablePropertyChangeHandler>();
            mocks.ReplayAll();

            var input = new MacroStabilityInwardsLocationInputDaily();

            // Call
            var properties = new MacroStabilityInwardsLocationInputDailyProperties(input, changeHandler);

            // Assert
            Assert.IsInstanceOf<MacroStabilityInwardsLocationInputBaseProperties<MacroStabilityInwardsLocationInputDaily>>(properties);
            Assert.AreSame(input, properties.Data);

            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_ValidData_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var mocks = new MockRepository();
            var changeHandler = mocks.Stub<IObservablePropertyChangeHandler>();
            mocks.ReplayAll();

            var input = new MacroStabilityInwardsLocationInputDaily();

            // Call
            var properties = new MacroStabilityInwardsLocationInputDailyProperties(input, changeHandler);

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);

            Assert.AreEqual(3, dynamicProperties.Count);

            const string waterStressesCategory = "Waterspanningen";

            PropertyDescriptor waterLevelPolderProperty = dynamicProperties[expectedwaterLevelPolderPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                waterLevelPolderProperty,
                waterStressesCategory,
                "Polderpeil [m+NAP]",
                "Het niveau van het oppervlaktewater binnen een beheersgebied.");

            PropertyDescriptor offsetProperty = dynamicProperties[expectedOffsetPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                offsetProperty,
                waterStressesCategory,
                "Offsets PL 1",
                "Eigenschappen van offsets PL 1.",
                true);

            PropertyDescriptor penetrationLengthProperty = dynamicProperties[expectedPenetrationLengthPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                penetrationLengthProperty,
                waterStressesCategory,
                "Indringingslengte [m]",
                "De verticale afstand waarover de waterspanning in de deklaag verandert bij waterspanningsvariaties in de watervoerende zandlaag.",
                true);

            mocks.VerifyAll();
        }

        [Test]
        public void GetProperties_WithData_ReturnExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var changeHandler = mocks.Stub<IObservablePropertyChangeHandler>();
            mocks.ReplayAll();

            var input = new MacroStabilityInwardsLocationInputDaily();

            // Call
            var properties = new MacroStabilityInwardsLocationInputDailyProperties(input, changeHandler);

            // Assert
            Assert.AreEqual(input.PenetrationLength, properties.PenetrationLength);

            mocks.VerifyAll();
        }
    }
}