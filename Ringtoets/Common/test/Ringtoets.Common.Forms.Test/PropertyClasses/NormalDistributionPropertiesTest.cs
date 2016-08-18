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
using System.Collections;
using System.ComponentModel;
using System.Linq;
using Core.Common.Base;
using Core.Common.Gui.PropertyBag;
using Core.Common.Utils.Attributes;
using Core.Common.Utils.Reflection;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Forms.PropertyClasses;

namespace Ringtoets.Common.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class NormalDistributionPropertiesTest
    {
        private MockRepository mockRepository;

        [SetUp]
        public void SetUp()
        {
            mockRepository = new MockRepository();
        }

        [Test]
        public void Constructor_WithoutParameters_ExpectedValues()
        {
            // Call
            var properties = new NormalDistributionProperties();

            // Assert
            Assert.IsInstanceOf<DistributionProperties>(properties);
            Assert.IsNull(properties.Data);
            Assert.AreEqual("Normaal", properties.DistributionType);
        }

        [Test]
        public void Constructor_WithParameters_ExpectedValues()
        {
            // Setup
            var observerableMock = mockRepository.StrictMock<IObservable>();
            mockRepository.ReplayAll();

            // Call
            var properties = new NormalDistributionProperties(observerableMock, DistributionPropertiesReadOnly.None);

            // Assert
            Assert.IsInstanceOf<DistributionProperties>(properties);
            Assert.IsNull(properties.Data);
            Assert.AreEqual("Normaal", properties.DistributionType);
            mockRepository.VerifyAll();
        }

        [Test]
        public void PropertyAttributes_ReturnExpectedValues()
        {
            // Setup
            var observerableMock = mockRepository.StrictMock<IObservable>();
            mockRepository.ReplayAll();

            // Call
            var properties = new NormalDistributionProperties(observerableMock, DistributionPropertiesReadOnly.None);

            // Assert
            var dynamicPropertyBag = new DynamicPropertyBag(properties);
            PropertyDescriptorCollection dynamicProperties = dynamicPropertyBag.GetProperties(new Attribute[]
            {
                new BrowsableAttribute(true)
            });
            Assert.AreEqual(3, dynamicProperties.Count);

            var distributionTypePropertyName = TypeUtils.GetMemberName<NormalDistributionProperties>(ndp => ndp.DistributionType);
            PropertyDescriptor distributionTypeProperty = dynamicProperties.Find(distributionTypePropertyName, false);
            Assert.IsTrue(distributionTypeProperty.IsReadOnly);
            AssertAttributeProperty<ResourcesDisplayNameAttribute, string>(distributionTypeProperty.Attributes, "Type verdeling",
                                                                           attribute => attribute.DisplayName);
            AssertAttributeProperty<ResourcesDescriptionAttribute, string>(distributionTypeProperty.Attributes,
                                                                           "Het soort kansverdeling waarin deze parameter gedefinieerd wordt.",
                                                                           attribute => attribute.Description);

            var meanPropertyName = TypeUtils.GetMemberName<NormalDistributionProperties>(ndp => ndp.Mean);
            PropertyDescriptor meanProperty = dynamicProperties.Find(meanPropertyName, false);
            Assert.IsFalse(meanProperty.IsReadOnly);
            AssertAttributeProperty<ResourcesDisplayNameAttribute, string>(meanProperty.Attributes, "Verwachtingswaarde",
                                                                           attribute => attribute.DisplayName);
            AssertAttributeProperty<ResourcesDescriptionAttribute, string>(meanProperty.Attributes,
                                                                           "De gemiddelde waarde van de normale verdeling.",
                                                                           attribute => attribute.Description);

            var standardDeviationPropertyName = TypeUtils.GetMemberName<NormalDistributionProperties>(ndp => ndp.StandardDeviation);
            PropertyDescriptor standardProperty = dynamicProperties.Find(standardDeviationPropertyName, false);
            Assert.IsFalse(standardProperty.IsReadOnly);
            AssertAttributeProperty<ResourcesDisplayNameAttribute, string>(standardProperty.Attributes, "Standaardafwijking",
                                                                           attribute => attribute.DisplayName);
            AssertAttributeProperty<ResourcesDescriptionAttribute, string>(standardProperty.Attributes,
                                                                           "De standaardafwijking van de normale verdeling.",
                                                                           attribute => attribute.Description);
            mockRepository.VerifyAll();
        }

        private static void AssertAttributeProperty<TAttributeType, TAttributePropertyValueType>(
            IEnumerable attributes,
            TAttributePropertyValueType expectedValue,
            Func<TAttributeType, TAttributePropertyValueType> getAttributePropertyValue)
        {
            var attributesOfTypeTAttributeType = attributes.OfType<TAttributeType>();
            Assert.IsNotNull(attributesOfTypeTAttributeType);
            var attribute = attributesOfTypeTAttributeType.FirstOrDefault();
            Assert.IsNotNull(attribute, string.Format("Attribute type '{0} not found in {1}'", typeof(TAttributeType), attributes));
            Assert.AreEqual(expectedValue, getAttributePropertyValue(attribute));
        }
    }
}