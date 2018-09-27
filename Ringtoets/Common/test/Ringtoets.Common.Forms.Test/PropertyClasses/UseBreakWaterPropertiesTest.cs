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
using System.ComponentModel;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.TestUtil;
using Core.Common.Util;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.PropertyClasses;
using Ringtoets.Common.Forms.TestUtil;
using Ringtoets.Common.Forms.TypeConverters;

namespace Ringtoets.Common.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class UseBreakWaterPropertiesTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var properties = new UseBreakWaterProperties();

            // Assert
            Assert.IsFalse(properties.UseBreakWater);
            Assert.IsNull(properties.BreakWaterType);
            Assert.IsNaN(properties.BreakWaterHeight);
            Assert.IsEmpty(properties.ToString());
        }

        [Test]
        public void DefaultConstructor_Always_ReadOnlyProperties()
        {
            // Call
            var properties = new UseBreakWaterProperties();

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(3, dynamicProperties.Count);

            PropertyDescriptor useBreakWaterProperty = dynamicProperties[0];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(useBreakWaterProperty,
                                                                            "Misc",
                                                                            "Gebruik",
                                                                            "Moet de dam worden gebruikt tijdens de berekening?",
                                                                            true);

            PropertyDescriptor breakWaterTypeProperty = dynamicProperties[1];
            Assert.IsInstanceOf<NullableEnumConverter>(breakWaterTypeProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(breakWaterTypeProperty,
                                                                            "Misc",
                                                                            "Type",
                                                                            "Het type van de dam.",
                                                                            true);

            PropertyDescriptor breakWaterHeightProperty = dynamicProperties[2];
            Assert.IsInstanceOf<NoValueRoundedDoubleConverter>(breakWaterHeightProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(breakWaterHeightProperty,
                                                                            "Misc",
                                                                            "Hoogte [m+NAP]",
                                                                            "De hoogte van de dam.",
                                                                            true);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Constructor_WithBreakWaterAndCalculationUseBreakWater_ReturnExpectedProperties(bool useBreakWater)
        {
            // Setup
            var mocks = new MockRepository();
            var handler = mocks.Stub<IObservablePropertyChangeHandler>();
            mocks.ReplayAll();

            var testUseBreakWater = new TestUseBreakWater
            {
                UseBreakWater = useBreakWater
            };

            // Call
            var properties = new UseBreakWaterProperties(testUseBreakWater, handler);

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(3, dynamicProperties.Count);

            PropertyDescriptor useBreakWaterProperty = dynamicProperties[0];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(useBreakWaterProperty,
                                                                            "Misc",
                                                                            "Gebruik",
                                                                            "Moet de dam worden gebruikt tijdens de berekening?");

            PropertyDescriptor breakWaterTypeProperty = dynamicProperties[1];
            Assert.IsInstanceOf<NullableEnumConverter>(breakWaterTypeProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(breakWaterTypeProperty,
                                                                            "Misc",
                                                                            "Type",
                                                                            "Het type van de dam.",
                                                                            !useBreakWater);

            PropertyDescriptor breakWaterHeightProperty = dynamicProperties[2];
            Assert.IsInstanceOf<NoValueRoundedDoubleConverter>(breakWaterHeightProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(breakWaterHeightProperty,
                                                                            "Misc",
                                                                            "Hoogte [m+NAP]",
                                                                            "De hoogte van de dam.",
                                                                            !useBreakWater);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_UseBreakWaterDataNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var handler = mocks.Stub<IObservablePropertyChangeHandler>();
            mocks.ReplayAll();

            // Call
            TestDelegate test = () => new UseBreakWaterProperties(null, handler);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("useBreakWaterData", paramName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_HandlerNull_ThrowsArgumentNullException()
        {
            // Setup
            var testUseBreakWater = new TestUseBreakWater();

            // Call
            TestDelegate test = () => new UseBreakWaterProperties(testUseBreakWater, null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("handler", paramName);
        }

        [Test]
        public void Constructor_WithUseBreakWaterData_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var handler = mocks.Stub<IObservablePropertyChangeHandler>();
            mocks.ReplayAll();

            var useBreakWaterData = new TestUseBreakWater
            {
                UseBreakWater = true,
                BreakWater = new BreakWater(BreakWaterType.Caisson, 10)
            };

            // Call
            var properties = new UseBreakWaterProperties(useBreakWaterData, handler);

            // Assert
            Assert.IsTrue(properties.UseBreakWater);
            Assert.AreEqual(BreakWaterType.Caisson, properties.BreakWaterType);
            Assert.AreEqual(10, properties.BreakWaterHeight, properties.BreakWaterHeight.GetAccuracy());
            Assert.IsEmpty(properties.ToString());
            mocks.VerifyAll();
        }

        [Test]
        public void DikeHeight_Always_InputNotifiedAndPropertyChangedCalled()
        {
            RoundedDouble breakWaterHeight = new Random(21).NextRoundedDouble();
            SetPropertyAndVerifyNotificationsAndOutputForCalculation(properties => properties.BreakWaterHeight = breakWaterHeight,
                                                                     new TestUseBreakWater());
        }

        [Test]
        public void BreakWaterType_Always_InputNotifiedAndPropertyChangedCalled()
        {
            var type = new Random(21).NextEnumValue<BreakWaterType>();
            SetPropertyAndVerifyNotificationsAndOutputForCalculation(properties => properties.BreakWaterType = type,
                                                                     new TestUseBreakWater());
        }

        [Test]
        public void UseBreakWater_Always_InputNotifiedAndPropertyChangedCalled()
        {
            bool useBreakWater = new Random(21).NextBoolean();
            SetPropertyAndVerifyNotificationsAndOutputForCalculation(properties => properties.UseBreakWater = useBreakWater,
                                                                     new TestUseBreakWater());
        }

        private class TestUseBreakWater : CloneableObservable, ICalculationInput, IUseBreakWater
        {
            public TestUseBreakWater()
            {
                BreakWater = new BreakWater(BreakWaterType.Caisson, 2);
            }

            public bool UseBreakWater { get; set; }
            public BreakWater BreakWater { get; set; }
        }

        private static void SetPropertyAndVerifyNotificationsAndOutputForCalculation(
            Action<UseBreakWaterProperties> setProperty,
            TestUseBreakWater input)
        {
            // Setup
            var mocks = new MockRepository();
            var observable = mocks.StrictMock<IObservable>();
            observable.Expect(o => o.NotifyObservers());
            mocks.ReplayAll();

            var handler = new SetPropertyValueAfterConfirmationParameterTester(new[]
            {
                observable
            });

            var properties = new UseBreakWaterProperties(input, handler);

            // Call
            setProperty(properties);

            // Assert
            Assert.IsTrue(handler.Called);
            mocks.VerifyAll();
        }
    }
}