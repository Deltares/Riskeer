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
using Core.Common.Base.Data;
using Core.Common.Gui.PropertyBag;
using Core.Common.TestUtil;
using Core.Common.Utils.Attributes;
using Core.Common.Utils.Reflection;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.Common.Forms.Properties;
using Ringtoets.Common.Forms.PropertyClasses;

namespace Ringtoets.Common.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class LogNormalDistributionVariationPropertiesTest
    {
        [Test]
        public void Constructor_WithoutParameters_ExpectedValues()
        {
            // Call
            var properties = new LogNormalDistributionVariationProperties();

            // Assert
            Assert.IsInstanceOf<VariationCoefficientDistributionPropertiesBase<VariationCoefficientLogNormalDistribution>>(properties);
            Assert.IsNull(properties.Data);
            Assert.AreEqual("Lognormaal", properties.DistributionType);
        }

        [Test]
        public void Constructor_WithParameters_ExpectedValues()
        {
            // Call
            var mockRepository = new MockRepository();
            var observerableMock = mockRepository.StrictMock<IObservable>();
            mockRepository.ReplayAll();

            var properties = new LogNormalDistributionVariationProperties(VariationCoefficientDistributionPropertiesReadOnly.None, observerableMock);

            // Assert
            Assert.IsNull(properties.Data);
            Assert.AreEqual("Lognormaal", properties.DistributionType);
            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(VariationCoefficientDistributionPropertiesReadOnly.Mean)]
        [TestCase(VariationCoefficientDistributionPropertiesReadOnly.CoefficientOfVariation)]
        [TestCase(VariationCoefficientDistributionPropertiesReadOnly.None)]
        public void Constructor_EditableFieldsAndWithoutObervable_ThrowArgumentException(VariationCoefficientDistributionPropertiesReadOnly flags)
        {
            // Call
            TestDelegate call = () => new LogNormalDistributionVariationProperties(flags, null);

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, "Observable must be specified unless no property can be set.");
        }

        [Test]
        [TestCase(VariationCoefficientDistributionPropertiesReadOnly.All, true, true)]
        [TestCase(VariationCoefficientDistributionPropertiesReadOnly.Mean, true, false)]
        [TestCase(VariationCoefficientDistributionPropertiesReadOnly.None, false, false)]
        [TestCase(VariationCoefficientDistributionPropertiesReadOnly.CoefficientOfVariation, false, true)]
        public void DynamicReadOnlyValidationMethod_VariousReadOnlySet_ExpectedValues(VariationCoefficientDistributionPropertiesReadOnly propertiesReadOnly,
                                                                                      bool expectMeanReadOnly,
                                                                                      bool expectVariarionCoefficient)
        {
            // Setup
            var mockRepository = new MockRepository();
            var observerableMock = mockRepository.StrictMock<IObservable>();
            mockRepository.ReplayAll();

            var properties = new LogNormalDistributionVariationProperties(propertiesReadOnly, observerableMock);

            var meanPropertyName = TypeUtils.GetMemberName<LogNormalDistributionVariationProperties>(lndvp => lndvp.Mean);
            var variationCoefficientPropertyName = TypeUtils.GetMemberName<LogNormalDistributionVariationProperties>(lndvp => lndvp.CoefficientOfVariation);

            // Call
            var meanIsReadOnly = properties.DynamicReadOnlyValidationMethod(meanPropertyName);
            var variarionCoefficientIsReadOnly = properties.DynamicReadOnlyValidationMethod(variationCoefficientPropertyName);
            var doesNotExist = properties.DynamicReadOnlyValidationMethod("DoesNotExist");

            // Assert
            Assert.AreEqual(expectVariarionCoefficient, variarionCoefficientIsReadOnly);
            Assert.AreEqual(expectMeanReadOnly, meanIsReadOnly);
            Assert.IsFalse(doesNotExist);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Data_SetNewDistributionContextInstance_ReturnCorrectPropertyValues()
        {
            // Setup
            var mockRepository = new MockRepository();
            var observerableMock = mockRepository.StrictMock<IObservable>();
            mockRepository.ReplayAll();

            var properties = new LogNormalDistributionVariationProperties(VariationCoefficientDistributionPropertiesReadOnly.None, observerableMock);
            var distribution = new VariationCoefficientLogNormalDistribution(2);

            // Call
            properties.Data = distribution;

            // Assert
            Assert.AreEqual(distribution.Mean, properties.Mean);
            Assert.AreEqual(distribution.CoefficientOfVariation, properties.CoefficientOfVariation);
            var expectedToString = string.Format("{0} ({1} = {2})", distribution.Mean, Resources.Distribution_VariationCoefficient_DisplayName,
                                                 distribution.CoefficientOfVariation);
            Assert.AreEqual(expectedToString, properties.ToString());
            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(VariationCoefficientDistributionPropertiesReadOnly.All)]
        [TestCase(VariationCoefficientDistributionPropertiesReadOnly.Mean)]
        public void SetProperties_ReadOnlyMeanWithObserverable_ThrowsArgumentException(VariationCoefficientDistributionPropertiesReadOnly propertiesReadOnly)
        {
            // Setup
            var mockRepository = new MockRepository();
            var observerableMock = mockRepository.StrictMock<IObservable>();
            mockRepository.ReplayAll();

            var properties = new LogNormalDistributionVariationProperties(propertiesReadOnly, observerableMock)
            {
                Data = new VariationCoefficientLogNormalDistribution(2)
            };

            // Call
            TestDelegate test = () => properties.Mean = new RoundedDouble(2, 20);

            // Assert
            string message = Assert.Throws<InvalidOperationException>(test).Message;
            Assert.AreEqual("Mean is set to be read-only.", message);
            mockRepository.VerifyAll();
        }

        [Test]
        public void SetProperties_MeanWithObserverable_ValueSetNotifyObservers()
        {
            // Setup
            var mockRepository = new MockRepository();
            var observerableMock = mockRepository.StrictMock<IObservable>();
            observerableMock.Expect(o => o.NotifyObservers());
            mockRepository.ReplayAll();

            var properties = new LogNormalDistributionVariationProperties(VariationCoefficientDistributionPropertiesReadOnly.None, observerableMock)
            {
                Data = new VariationCoefficientLogNormalDistribution(2)
            };
            RoundedDouble newMeanValue = new RoundedDouble(3, 20);

            // Call
            properties.Mean = newMeanValue;

            // Assert
            Assert.AreEqual(newMeanValue, properties.Mean);
            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(VariationCoefficientDistributionPropertiesReadOnly.All)]
        [TestCase(VariationCoefficientDistributionPropertiesReadOnly.CoefficientOfVariation)]
        public void SetProperties_ReadOnlyVariationCoefficientWithoutObserverable_ThrowsArgumentException(VariationCoefficientDistributionPropertiesReadOnly propertiesReadOnly)
        {
            // Setup
            var mocks = new MockRepository();
            var observable = mocks.Stub<IObservable>();
            mocks.ReplayAll();

            var properties = new LogNormalDistributionVariationProperties(propertiesReadOnly, observable)
            {
                Data = new VariationCoefficientLogNormalDistribution(2)
            };

            // Call
            TestDelegate test = () => properties.CoefficientOfVariation = new RoundedDouble(2, 20);

            // Assert
            string message = Assert.Throws<InvalidOperationException>(test).Message;
            Assert.AreEqual("CoefficientOfVariation is set to be read-only.", message);
            mocks.VerifyAll();
        }

        [Test]
        public void SetProperties_VariationCoefficientWithObserverable_ValueSetNotifyObservers()
        {
            // Setup
            var mockRepository = new MockRepository();
            var observerableMock = mockRepository.StrictMock<IObservable>();
            observerableMock.Expect(o => o.NotifyObservers()).Repeat.Once();
            mockRepository.ReplayAll();

            var properties = new LogNormalDistributionVariationProperties(VariationCoefficientDistributionPropertiesReadOnly.None, observerableMock)
            {
                Data = new VariationCoefficientLogNormalDistribution(2)
            };
            RoundedDouble newVariationCoefficientValue = new RoundedDouble(2, 20);

            // Call
            properties.CoefficientOfVariation = newVariationCoefficientValue;

            // Assert
            Assert.AreEqual(newVariationCoefficientValue, properties.CoefficientOfVariation);
            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(VariationCoefficientDistributionPropertiesReadOnly.All, true, true)]
        [TestCase(VariationCoefficientDistributionPropertiesReadOnly.Mean, true, false)]
        [TestCase(VariationCoefficientDistributionPropertiesReadOnly.None, false, false)]
        [TestCase(VariationCoefficientDistributionPropertiesReadOnly.CoefficientOfVariation, false, true)]
        public void PropertyAttributes_ReturnExpectedValues(VariationCoefficientDistributionPropertiesReadOnly propertiesReadOnly, bool expectMeanReadOnly, bool expectVariationCoefficientReadOnly)
        {
            // Setup
            var mockRepository = new MockRepository();
            var observerableMock = mockRepository.StrictMock<IObservable>();
            mockRepository.ReplayAll();

            // Call
            var properties = new LogNormalDistributionVariationProperties(propertiesReadOnly, observerableMock)
            {
                Data = new VariationCoefficientLogNormalDistribution(2)
            };

            // Assert
            var dynamicPropertyBag = new DynamicPropertyBag(properties);
            PropertyDescriptorCollection dynamicProperties = dynamicPropertyBag.GetProperties(new Attribute[]
            {
                new BrowsableAttribute(true)
            });
            Assert.AreEqual(3, dynamicProperties.Count);

            var distributionTypePropertyName = TypeUtils.GetMemberName<LogNormalDistributionVariationProperties>(ndp => ndp.DistributionType);
            PropertyDescriptor distributionTypeProperty = dynamicProperties.Find(distributionTypePropertyName, false);
            Assert.IsTrue(distributionTypeProperty.IsReadOnly);
            AssertAttributeProperty<ResourcesDisplayNameAttribute, string>(distributionTypeProperty.Attributes, "Type verdeling",
                                                                           attribute => attribute.DisplayName);
            AssertAttributeProperty<ResourcesDescriptionAttribute, string>(distributionTypeProperty.Attributes,
                                                                           "Het soort kansverdeling waarin deze parameter gedefinieerd wordt.",
                                                                           attribute => attribute.Description);

            var meanPropertyName = TypeUtils.GetMemberName<LogNormalDistributionVariationProperties>(ndp => ndp.Mean);
            PropertyDescriptor meanProperty = dynamicProperties.Find(meanPropertyName, false);
            Assert.AreEqual(expectMeanReadOnly, meanProperty.IsReadOnly);
            AssertAttributeProperty<ResourcesDisplayNameAttribute, string>(meanProperty.Attributes, "Verwachtingswaarde",
                                                                           attribute => attribute.DisplayName);
            AssertAttributeProperty<ResourcesDescriptionAttribute, string>(meanProperty.Attributes,
                                                                           "De gemiddelde waarde van de lognormale verdeling.",
                                                                           attribute => attribute.Description);

            var variationCoefficientPropertyName = TypeUtils.GetMemberName<LogNormalDistributionVariationProperties>(ndp => ndp.CoefficientOfVariation);
            PropertyDescriptor variationCoefficientProperty = dynamicProperties.Find(variationCoefficientPropertyName, false);
            Assert.AreEqual(expectVariationCoefficientReadOnly, variationCoefficientProperty.IsReadOnly);
            AssertAttributeProperty<ResourcesDisplayNameAttribute, string>(variationCoefficientProperty.Attributes, "Variatiecoëfficiënt",
                                                                           attribute => attribute.DisplayName);
            AssertAttributeProperty<ResourcesDescriptionAttribute, string>(variationCoefficientProperty.Attributes,
                                                                           "De variatiecoëfficiënt van de lognormale verdeling.",
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