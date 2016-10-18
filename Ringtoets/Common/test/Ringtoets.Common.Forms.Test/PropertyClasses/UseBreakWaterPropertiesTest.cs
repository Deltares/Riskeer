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
using Ringtoets.Common.Forms.TypeConverters;

namespace Ringtoets.Common.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class UseBreakWaterPropertiesTest
    {
        [Test]
        public void Constructor_IBreakWaterNull_ExpectedValues()
        {
            // Call
            var properties = new UseBreakWaterProperties(null);

            // Assert
            Assert.IsFalse(properties.UseBreakWater);
            Assert.IsNull(properties.BreakWaterType);
            Assert.AreEqual((RoundedDouble) double.NaN, properties.BreakWaterHeight);
            Assert.AreEqual(string.Empty, properties.ToString());
        }

        [Test]
        public void Constructor_ValidData_ExpectedValues()
        {
            // Setup
            var useBreakWaterData = new TestUseBreakWater
            {
                UseBreakWater = true,
                BreakWater = new BreakWater(BreakWaterType.Caisson, 10)
            };

            // Call
            var properties = new UseBreakWaterProperties(useBreakWaterData);

            // Assert
            Assert.IsTrue(properties.UseBreakWater);
            Assert.AreEqual(BreakWaterType.Caisson, properties.BreakWaterType);
            Assert.AreEqual(10, properties.BreakWaterHeight, properties.BreakWaterHeight.GetAccuracy());
            Assert.AreEqual(string.Empty, properties.ToString());
        }

        [Test]
        public void SetProperties_IndividualProperties_UpdateDataAndNotifyObservers()
        {
            // Setup
            var mockRepository = new MockRepository();
            var observerMock = mockRepository.StrictMock<IObserver>();
            observerMock.Expect(o => o.UpdateObserver()).Repeat.Times(3);
            mockRepository.ReplayAll();

            var breakWater = new BreakWater(BreakWaterType.Caisson, 2.2);
            var testUseBreakWater = new TestUseBreakWater
            {
                BreakWater = breakWater
            };
            var properties = new UseBreakWaterProperties(testUseBreakWater);

            testUseBreakWater.Attach(observerMock);

            // Call
            properties.UseBreakWater = true;
            properties.BreakWaterType = BreakWaterType.Dam;
            properties.BreakWaterHeight = (RoundedDouble) 1.1;

            // Assert
            Assert.IsTrue(properties.UseBreakWater);
            Assert.AreEqual(BreakWaterType.Dam, properties.BreakWaterType);
            Assert.AreEqual(1.1, properties.BreakWaterHeight, properties.BreakWaterHeight.GetAccuracy());
            Assert.AreEqual(string.Empty, properties.ToString());
            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(true, true)]
        [TestCase(true, false)]
        [TestCase(false, true)]
        [TestCase(false, false)]
        public void PropertyAttributes_UseBreakWater_ReturnExpectedValues(bool useBreakWater, bool useBreakWaterEnabled)
        {
            // Setup
            TestUseBreakWater testUseBreakWater = useBreakWaterEnabled ? new TestUseBreakWater
            {
                UseBreakWater = useBreakWater
            } : null;

            // Call
            var properties = new UseBreakWaterProperties(testUseBreakWater);

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
                                                                            !useBreakWaterEnabled);

            PropertyDescriptor breakWaterTypeProperty = dynamicProperties[1];
            Assert.IsInstanceOf<NullableEnumTypeConverter>(breakWaterTypeProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(breakWaterTypeProperty,
                                                                            "Misc",
                                                                            "Type",
                                                                            "Het type van de dam.",
                                                                            !useBreakWaterEnabled || !useBreakWater);

            PropertyDescriptor breakWaterHeightProperty = dynamicProperties[2];
            Assert.IsInstanceOf<NoValueRoundedDoubleConverter>(breakWaterHeightProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(breakWaterHeightProperty,
                                                                            "Misc",
                                                                            "Hoogte [m+NAP]",
                                                                            "De hoogte van de dam.",
                                                                            !useBreakWaterEnabled || !useBreakWater);
        }

        private class TestUseBreakWater : Observable, IUseBreakWater
        {
            public bool UseBreakWater { get; set; }
            public BreakWater BreakWater { get; set; }
        }
    }
}