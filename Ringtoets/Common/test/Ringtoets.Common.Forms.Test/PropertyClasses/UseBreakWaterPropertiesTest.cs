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
            Assert.AreEqual(RoundedDouble.NaN, properties.BreakWaterHeight);
            Assert.AreEqual(string.Empty, properties.ToString());
        }

        [Test]
        public void DefaultConstructor_Always_ReadOnlyProperties()
        {
            // Call
            var properties = new UseBreakWaterProperties();

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
        public void Constructor_UseBreakWaterDataNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new UseBreakWaterProperties(null, null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("useBreakWaterData", paramName);
        }

        [Test]
        public void Constructor_WithUseBreakWaterData_ExpectedValues()
        {
            // Setup
            var useBreakWaterData = new TestUseBreakWater
            {
                UseBreakWater = true,
                BreakWater = new BreakWater(BreakWaterType.Caisson, 10)
            };

            // Call
            var properties = new UseBreakWaterProperties(useBreakWaterData, null);

            // Assert
            Assert.IsTrue(properties.UseBreakWater);
            Assert.AreEqual(BreakWaterType.Caisson, properties.BreakWaterType);
            Assert.AreEqual(10, properties.BreakWaterHeight, properties.BreakWaterHeight.GetAccuracy());
            Assert.AreEqual(string.Empty, properties.ToString());
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Constructor_WithBreakWaterAndCalculationUseBreakWater_ReturnExpectedProperties(bool useBreakWater)
        {
            // Setup
            TestUseBreakWater testUseBreakWater = new TestUseBreakWater
            {
                UseBreakWater = useBreakWater
            };

            // Call
            var properties = new UseBreakWaterProperties(testUseBreakWater, null);

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
        }

        [Test]
        public void SetProperties_IndividualProperties_UpdateData()
        {
            // Setup
            var breakWater = new BreakWater(BreakWaterType.Caisson, 2.2);
            var testUseBreakWater = new TestUseBreakWater
            {
                BreakWater = breakWater
            };
            var properties = new UseBreakWaterProperties(testUseBreakWater, null);

            // Call
            properties.UseBreakWater = true;
            properties.BreakWaterType = BreakWaterType.Dam;
            properties.BreakWaterHeight = (RoundedDouble) 1.1;

            // Assert
            Assert.IsTrue(properties.UseBreakWater);
            Assert.AreEqual(BreakWaterType.Dam, properties.BreakWaterType);
            Assert.AreEqual(1.1, properties.BreakWaterHeight, properties.BreakWaterHeight.GetAccuracy());
            Assert.AreEqual(string.Empty, properties.ToString());
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void DikeHeight_WithOrWithoutOutput_InputNotifiedAndPropertyChangedCalled(bool hasOutput)
        {
            SetPropertyAndVerifyNotifcationsAndHandlerCall(hasOutput, properties => properties.BreakWaterHeight = new Random(21).NextRoundedDouble());
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void BreakWaterType_WithOrWithoutOutput_InputNotifiedAndPropertyChangedCalled(bool hasOutput)
        {
            SetPropertyAndVerifyNotifcationsAndHandlerCall(hasOutput, properties => properties.BreakWaterType = new Random(21).NextEnumValue<BreakWaterType>());
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void UseBreakWater_WithOrWithoutOutput_InputNotifiedAndPropertyChangedCalled(bool hasOutput)
        {
            SetPropertyAndVerifyNotifcationsAndHandlerCall(hasOutput, properties => properties.UseBreakWater = new Random(21).NextBoolean());
        }

        private class TestUseBreakWater : Observable, IUseBreakWater
        {
            public bool UseBreakWater { get; set; }
            public BreakWater BreakWater { get; set; }
        }

        private void SetPropertyAndVerifyNotifcationsAndHandlerCall(
            bool hasOutput,
            Action<UseBreakWaterProperties> setProperty)
        {
            // Setup
            var mocks = new MockRepository();
            var calculationObserver = mocks.StrictMock<IObserver>();
            var inputObserver = mocks.StrictMock<IObserver>();
            inputObserver.Expect(o => o.UpdateObserver());
            var handler = mocks.StrictMock<IPropertyChangeHandler>();
            handler.Expect(o => o.PropertyChanged());
            mocks.ReplayAll();

            var calculation = new TestCalculation();
            if (hasOutput)
            {
                calculation.Output = new object();
            }

            calculation.Attach(calculationObserver);
            var input = new TestUseBreakWater();
            input.Attach(inputObserver);
            input.BreakWater = new BreakWater(BreakWaterType.Caisson, 3.2);

            var properties = new UseBreakWaterProperties(input, handler);

            // Call
            setProperty(properties);

            // Assert
            mocks.VerifyAll();
        }
    }
}