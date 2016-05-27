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
using System.ComponentModel;
using System.Linq;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Gui.PropertyBag;
using Core.Common.Utils.Attributes;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.Common.Forms.PropertyClasses;

namespace Ringtoets.Common.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class LogNormalDistributionPropertiesTest
    {
        private MockRepository mockRepository;

        [SetUp]
        public void SetUp()
        {
            mockRepository = new MockRepository();
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var observerableMock = mockRepository.StrictMock<IObservable>();
            mockRepository.ReplayAll();

            // Call
            var properties = new LogNormalDistributionProperties(observerableMock);

            // Assert
            Assert.IsInstanceOf<DistributionProperties>(properties);
            Assert.IsNull(properties.Data);
            Assert.AreEqual("Lognormaal", properties.DistributionType);
            mockRepository.VerifyAll();
        }

        [Test]
        public void PropertyAttributes_ReturnExpectedValues()
        {
            // Setup
            var observerableMock = mockRepository.StrictMock<IObservable>();
            mockRepository.ReplayAll();

            // Call
            var properties = new LogNormalDistributionProperties(observerableMock);

            // Assert
            TypeConverter classTypeConverter = TypeDescriptor.GetConverter(properties, true);
            Assert.IsInstanceOf<ExpandableObjectConverter>(classTypeConverter);

            var dynamicPropertyBag = new DynamicPropertyBag(properties);
            PropertyDescriptorCollection dynamicProperties = dynamicPropertyBag.GetProperties();
            Assert.AreEqual(4, dynamicProperties.Count);

            var meanAttributes = Attribute.GetCustomAttributes(properties.GetType().GetProperty("Mean"));
            Assert.IsNotNull(meanAttributes);
            AssertAttributesOfType<ResourcesDisplayNameAttribute, string>(meanAttributes, "Verwachtingswaarde",
                                                                          attribute => attribute.DisplayName);
            AssertAttributesOfType<ResourcesDescriptionAttribute, string>(meanAttributes,
                                                                          "De gemiddelde waarde van de lognormale verdeling.",
                                                                          attribute => attribute.Description);

            var standardAttributes = Attribute.GetCustomAttributes(properties.GetType().GetProperty("StandardDeviation"));
            Assert.IsNotNull(standardAttributes);
            AssertAttributesOfType<ResourcesDisplayNameAttribute, string>(standardAttributes, "Standaardafwijking",
                                                                          attribute => attribute.DisplayName);
            AssertAttributesOfType<ResourcesDescriptionAttribute, string>(standardAttributes,
                                                                          "De standaardafwijking van de lognormale verdeling.",
                                                                          attribute => attribute.Description);

            mockRepository.VerifyAll();
        }


        [Test]
        public void SetProperties_MeanWithoutObserverable_ThrowsArgumentException()
        {
            // Setup
            var properties = new LogNormalDistributionProperties(null)
            {
                Data = new LogNormalDistribution(2),
            };

            // Call
            TestDelegate test = () => properties.Mean = new RoundedDouble(2, 20);

            // Assert
            Assert.Throws<ArgumentException>(test);
        }

        [Test]
        public void SetProperties_StandardDeviationWithoutObserverable_ThrowsArgumentException()
        {
            // Setup
            var properties = new LogNormalDistributionProperties(null)
            {
                Data = new LogNormalDistribution(2)
            };

            // Call
            TestDelegate test = () => properties.StandardDeviation = new RoundedDouble(2, 20);

            // Assert
            Assert.Throws<ArgumentException>(test);
        }



        [Test]
        public void SetProperties_MeanWithObserverable_ValueSetNotifyObservers()
        {
            // Setup
            var observerableMock = mockRepository.StrictMock<IObservable>();
            observerableMock.Expect(o => o.NotifyObservers()).Repeat.Once();
            var properties = new LogNormalDistributionProperties(observerableMock)
            {
                Data = new LogNormalDistribution(3)
            };
            mockRepository.ReplayAll();
            RoundedDouble newMeanValue = new RoundedDouble(3, 20);

            // Call
            properties.Mean = newMeanValue;

            // Assert
            Assert.AreEqual(newMeanValue, properties.Mean);
            mockRepository.VerifyAll();
        }

        [Test]
        public void SetProperties_StandardDeviationWithObserverable_ValueSetNotifyObservers()
        {
            // Setup
            var observerableMock = mockRepository.StrictMock<IObservable>();
            observerableMock.Expect(o => o.NotifyObservers()).Repeat.Once();
            var properties = new LogNormalDistributionProperties(observerableMock)
            {
                Data = new LogNormalDistribution(3)
            };
            mockRepository.ReplayAll();
            RoundedDouble newStandardDeviationValue = new RoundedDouble(3, 20);

            // Call
            properties.StandardDeviation = newStandardDeviationValue;

            // Assert
            Assert.AreEqual(newStandardDeviationValue, properties.StandardDeviation);
            mockRepository.VerifyAll();
        }

        private static void AssertAttributesOfType<T, TR>(IEnumerable<Attribute> attributes, TR expectedValue,
                                                          Func<T, TR> action)
        {
            var meanDisplayNameAttribute = attributes.OfType<T>();
            Assert.IsNotNull(meanDisplayNameAttribute);
            var e = meanDisplayNameAttribute.FirstOrDefault();
            Assert.IsNotNull(e);
            Assert.AreEqual(expectedValue, action(e));
        }
    }
}