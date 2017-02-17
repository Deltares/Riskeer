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
using System.ComponentModel;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Gui.PropertyBag;
using Core.Common.TestUtil;
using Core.Common.Utils;
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
    public class ConfirmingUseBreakWaterPropertiesTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var properties = new ConfirmingUseBreakWaterProperties<ICalculation, TestUseBreakWater>();

            // Assert
            Assert.IsFalse(properties.UseBreakWater);
            Assert.IsNull(properties.BreakWaterType);
            Assert.AreEqual(RoundedDouble.NaN, properties.BreakWaterHeight);
            Assert.AreEqual(string.Empty, properties.ToString());
        }

        [Test]
        public void DefaultConstructor_Always_ReadOnlyProperties()
        {
            // Call
            var properties = new ConfirmingUseBreakWaterProperties<ICalculation, TestUseBreakWater>();

            // Assert
            var dynamicPropertyBag = new DynamicPropertyBag(properties);
            PropertyDescriptorCollection dynamicProperties = dynamicPropertyBag.GetProperties(new Attribute[]
            {
                new BrowsableAttribute(true)
            });
            Assert.AreEqual(3, dynamicProperties.Count);

            PropertyDescriptor useBreakWaterProperty = dynamicProperties[0];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(useBreakWaterProperty,
                                                                            "Misc",
                                                                            "Gebruik",
                                                                            "Moet de dam worden gebruikt tijdens de berekening?",
                                                                            true);

            PropertyDescriptor breakWaterTypeProperty = dynamicProperties[1];
            Assert.IsInstanceOf<NullableEnumTypeConverter>(breakWaterTypeProperty.Converter);
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
            var calculation = mocks.Stub<ICalculation>();
            var handler = mocks.Stub<ICalculationInputPropertyChangeHandler<TestUseBreakWater, ICalculation>>();
            mocks.ReplayAll();

            TestUseBreakWater testUseBreakWater = new TestUseBreakWater
            {
                UseBreakWater = useBreakWater
            };

            // Call
            var properties = new ConfirmingUseBreakWaterProperties<ICalculation, TestUseBreakWater>(testUseBreakWater, calculation, handler);

            // Assert
            var dynamicPropertyBag = new DynamicPropertyBag(properties);
            PropertyDescriptorCollection dynamicProperties = dynamicPropertyBag.GetProperties(new Attribute[]
            {
                new BrowsableAttribute(true)
            });
            Assert.AreEqual(3, dynamicProperties.Count);

            PropertyDescriptor useBreakWaterProperty = dynamicProperties[0];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(useBreakWaterProperty,
                                                                            "Misc",
                                                                            "Gebruik",
                                                                            "Moet de dam worden gebruikt tijdens de berekening?");

            PropertyDescriptor breakWaterTypeProperty = dynamicProperties[1];
            Assert.IsInstanceOf<NullableEnumTypeConverter>(breakWaterTypeProperty.Converter);
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
            var calculation = mocks.Stub<ICalculation>();
            var handler = mocks.Stub<ICalculationInputPropertyChangeHandler<TestUseBreakWater, ICalculation>>();
            mocks.ReplayAll();

            // Call
            TestDelegate test = () => new ConfirmingUseBreakWaterProperties<ICalculation, TestUseBreakWater>(null, calculation, handler);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("useBreakWaterData", paramName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_CalculationNull_ThrowsArgumentNullException()
        {
            // Setup
            TestUseBreakWater testUseBreakWater = new TestUseBreakWater();

            var mocks = new MockRepository();
            var handler = mocks.Stub<ICalculationInputPropertyChangeHandler<TestUseBreakWater, ICalculation>>();
            mocks.ReplayAll();

            // Call
            TestDelegate test = () => new ConfirmingUseBreakWaterProperties<ICalculation, TestUseBreakWater>(testUseBreakWater, null, handler);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("calculation", paramName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_HandlerNull_ThrowsArgumentNullException()
        {
            // Setup
            TestUseBreakWater testUseBreakWater = new TestUseBreakWater();

            var mocks = new MockRepository();
            var calculation = mocks.Stub<ICalculation>();
            mocks.ReplayAll();

            // Call
            TestDelegate test = () => new ConfirmingUseBreakWaterProperties<ICalculation, TestUseBreakWater>(testUseBreakWater, calculation, null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("handler", paramName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_WithUseBreakWaterData_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var calculation = mocks.Stub<ICalculation>();
            var handler = mocks.Stub<ICalculationInputPropertyChangeHandler<TestUseBreakWater, ICalculation>>();
            mocks.ReplayAll();

            var useBreakWaterData = new TestUseBreakWater
            {
                UseBreakWater = true,
                BreakWater = new BreakWater(BreakWaterType.Caisson, 10)
            };

            // Call
            var properties = new ConfirmingUseBreakWaterProperties<ICalculation, TestUseBreakWater>(useBreakWaterData, calculation, handler);

            // Assert
            Assert.IsTrue(properties.UseBreakWater);
            Assert.AreEqual(BreakWaterType.Caisson, properties.BreakWaterType);
            Assert.AreEqual(10, properties.BreakWaterHeight, properties.BreakWaterHeight.GetAccuracy());
            Assert.AreEqual(string.Empty, properties.ToString());
            mocks.VerifyAll();
        }

        [Test]
        public void DikeHeight_Always_InputNotifiedAndPropertyChangedCalled()
        {
            var breakWaterHeight = new Random(21).NextRoundedDouble();
            SetPropertyAndVerifyNotifcationsAndOutputForCalculation(properties => properties.BreakWaterHeight = breakWaterHeight,
                                                                    breakWaterHeight,
                                                                    new TestUseBreakWater());
        }

        [Test]
        public void BreakWaterType_Always_InputNotifiedAndPropertyChangedCalled()
        {
            var type = new Random(21).NextEnumValue<BreakWaterType>();
            SetPropertyAndVerifyNotifcationsAndOutputForCalculation(properties => properties.BreakWaterType = type,
                                                                    type,
                                                                    new TestUseBreakWater());
        }

        [Test]
        public void UseBreakWater_Always_InputNotifiedAndPropertyChangedCalled()
        {
            var useBreakWater = new Random(21).NextBoolean();
            SetPropertyAndVerifyNotifcationsAndOutputForCalculation(properties => properties.UseBreakWater = useBreakWater,
                                                                    useBreakWater,
                                                                    new TestUseBreakWater());
        }

        public class TestUseBreakWater : Observable, ICalculationInput, IUseBreakWater
        {
            public TestUseBreakWater()
            {
                BreakWater = new BreakWater(BreakWaterType.Caisson, 2);
            }

            public bool UseBreakWater { get; set; }
            public BreakWater BreakWater { get; set; }
        }

        private void SetPropertyAndVerifyNotifcationsAndOutputForCalculation<TPropertyValue>(
            Action<ConfirmingUseBreakWaterProperties<ICalculation, TestUseBreakWater>> setProperty,
            TPropertyValue expectedValueSet,
            TestUseBreakWater input)
        {
            // Setup
            var mocks = new MockRepository();
            var calculation = mocks.Stub<ICalculation>();
            var observable = mocks.StrictMock<IObservable>();
            observable.Expect(o => o.NotifyObservers());
            mocks.ReplayAll();

            var handler = new CalculationInputSetPropertyValueAfterConfirmationParameterTester<TestUseBreakWater, ICalculation, TPropertyValue>(
                input,
                calculation,
                expectedValueSet,
                new[]
                {
                    observable
                });

            var properties = new ConfirmingUseBreakWaterProperties<ICalculation, TestUseBreakWater>(input, calculation, handler);

            // Call
            setProperty(properties);

            // Assert
            Assert.IsTrue(handler.Called);
            mocks.VerifyAll();
        }
    }
}