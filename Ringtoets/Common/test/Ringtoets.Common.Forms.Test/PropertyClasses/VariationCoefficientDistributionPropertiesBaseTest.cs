﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Ringtoets.Common.Forms.PropertyClasses;

namespace Ringtoets.Common.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class VariationCoefficientDistributionPropertiesBaseTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var readOnlyFlags = VariationCoefficientDistributionPropertiesReadOnly.All;

            // Call
            var properties = new SimpleVariationCoefficientDistributionProperties(readOnlyFlags, null);

            // Assert
            Assert.IsInstanceOf<ObjectProperties<IVariationCoefficientDistribution>>(properties);
            Assert.IsNull(properties.Data);
        }

        [Test]
        [TestCase(VariationCoefficientDistributionPropertiesReadOnly.Mean)]
        [TestCase(VariationCoefficientDistributionPropertiesReadOnly.CoefficientOfVariation)]
        [TestCase(VariationCoefficientDistributionPropertiesReadOnly.None)]
        public void Constructor_NoObservableSetWhileChangesPossible_ThrowArgumentException(
            VariationCoefficientDistributionPropertiesReadOnly flags)
        {
            // Call
            TestDelegate call = () => new SimpleVariationCoefficientDistributionProperties(flags, null);

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, "Observable must be specified unless no property can be set.");
        }

        [Test]
        [TestCase(VariationCoefficientDistributionPropertiesReadOnly.None, false, false)]
        [TestCase(VariationCoefficientDistributionPropertiesReadOnly.Mean, true, false)]
        [TestCase(VariationCoefficientDistributionPropertiesReadOnly.CoefficientOfVariation, false, true)]
        [TestCase(VariationCoefficientDistributionPropertiesReadOnly.All, true, true)]
        public void DynamicReadOnlyValidationMethod_VariousReadOnlyCases_ExpectedValues(
            VariationCoefficientDistributionPropertiesReadOnly flags,
            bool expectMeanReadOnly, bool expectVariationCoefficientReadOnly)
        {
            // Setup
            var mocks = new MockRepository();
            var observable = mocks.Stub<IObservable>();
            mocks.ReplayAll();

            var properties = new SimpleVariationCoefficientDistributionProperties(flags, observable);

            // Call
            bool isMeanReadonly = properties.DynamicReadOnlyValidationMethod("Mean");
            bool isVariationCoefficientReadOnly = properties.DynamicReadOnlyValidationMethod("CoefficientOfVariation");

            // Assert
            Assert.AreEqual(expectMeanReadOnly, isMeanReadonly);
            Assert.AreEqual(expectVariationCoefficientReadOnly, isVariationCoefficientReadOnly);
            mocks.VerifyAll();
        }

        [Test]
        public void DynamicReadOnlyValidationMethod_AllOtherCases_ReturnFalse()
        {
            // Setup
            var properties = new SimpleVariationCoefficientDistributionProperties(VariationCoefficientDistributionPropertiesReadOnly.All, null);

            // Call
            bool isReadonly = properties.DynamicReadOnlyValidationMethod(null);

            // Assert
            Assert.IsFalse(isReadonly);
        }

        [Test]
        public void Data_SetNewDistribution_PropertiesReturnCorrectValues()
        {
            // Setup
            var mocks = new MockRepository();
            var distribution = mocks.Stub<IVariationCoefficientDistribution>();
            distribution.Mean = new RoundedDouble(1, 2.3);
            distribution.CoefficientOfVariation = new RoundedDouble(4, 5.6789);
            mocks.ReplayAll();

            var properties = new SimpleVariationCoefficientDistributionProperties(VariationCoefficientDistributionPropertiesReadOnly.All, null);

            // Call
            properties.Data = distribution;

            // Assert
            Assert.AreEqual(distribution.Mean, properties.Mean);
            Assert.AreEqual(distribution.CoefficientOfVariation, properties.CoefficientOfVariation);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(VariationCoefficientDistributionPropertiesReadOnly.Mean)]
        [TestCase(VariationCoefficientDistributionPropertiesReadOnly.All)]
        public void Mean_SetValueWhileReadOnly_ThrowsInvalidOperationException(
            VariationCoefficientDistributionPropertiesReadOnly flags)
        {
            // Setup
            var mocks = new MockRepository();
            var distribution = mocks.Stub<IVariationCoefficientDistribution>();
            var observable = mocks.Stub<IObservable>();
            mocks.ReplayAll();

            var properties = new SimpleVariationCoefficientDistributionProperties(flags, observable)
            {
                Data = distribution
            };

            // Call
            TestDelegate call = () => properties.Mean = new RoundedDouble(1, 2.3);

            // Assert
            string message = Assert.Throws<InvalidOperationException>(call).Message;
            Assert.AreEqual("Mean is set to be read-only.", message);
            mocks.VerifyAll();
        }

        [Test]
        public void Mean_SetValue_ValueChangedAndObservableNotifies()
        {
            // Setup
            var mocks = new MockRepository();
            var distribution = mocks.Stub<IVariationCoefficientDistribution>();
            var observable = mocks.StrictMock<IObservable>();
            observable.Expect(o => o.NotifyObservers());
            mocks.ReplayAll();

            var properties = new SimpleVariationCoefficientDistributionProperties(
                VariationCoefficientDistributionPropertiesReadOnly.None, observable)
            {
                Data = distribution
            };

            var newMeanValue = new RoundedDouble(1, 2.3);

            // Call
            properties.Mean = newMeanValue;

            // Assert
            Assert.AreEqual(newMeanValue, distribution.Mean);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(VariationCoefficientDistributionPropertiesReadOnly.CoefficientOfVariation)]
        [TestCase(VariationCoefficientDistributionPropertiesReadOnly.All)]
        public void CoefficientOfVariation_SetValueWhileReadOnly_ThrowsInvalidOperationException(
            VariationCoefficientDistributionPropertiesReadOnly flags)
        {
            // Setup
            var mocks = new MockRepository();
            var distribution = mocks.Stub<IVariationCoefficientDistribution>();
            var observable = mocks.Stub<IObservable>();
            mocks.ReplayAll();

            var properties = new SimpleVariationCoefficientDistributionProperties(flags, observable)
            {
                Data = distribution
            };

            // Call
            TestDelegate call = () => properties.CoefficientOfVariation = new RoundedDouble(1, 2.3);

            // Assert
            string message = Assert.Throws<InvalidOperationException>(call).Message;
            Assert.AreEqual("CoefficientOfVariation is set to be read-only.", message);
            mocks.VerifyAll();
        }

        [Test]
        public void CoefficientOfVariation_SetValue_ValueChangedAndObservableNotifies()
        {
            // Setup
            var mocks = new MockRepository();
            var distribution = mocks.Stub<IVariationCoefficientDistribution>();
            var observable = mocks.StrictMock<IObservable>();
            observable.Expect(o => o.NotifyObservers());
            mocks.ReplayAll();

            var properties = new SimpleVariationCoefficientDistributionProperties(
                VariationCoefficientDistributionPropertiesReadOnly.None, observable)
            {
                Data = distribution
            };

            var newMeanValue = new RoundedDouble(1, 2.3);

            // Call
            properties.CoefficientOfVariation = newMeanValue;

            // Assert
            Assert.AreEqual(newMeanValue, distribution.CoefficientOfVariation);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(VariationCoefficientDistributionPropertiesReadOnly.All, true, true)]
        [TestCase(VariationCoefficientDistributionPropertiesReadOnly.Mean, true, false)]
        [TestCase(VariationCoefficientDistributionPropertiesReadOnly.None, false, false)]
        [TestCase(VariationCoefficientDistributionPropertiesReadOnly.CoefficientOfVariation, false, true)]
        public void PropertyAttributes_VariousReadOnlyScenarios_ReturnExpectedValues(
            VariationCoefficientDistributionPropertiesReadOnly flags, bool expectMeanReadOnly, bool expectVariationCoefficientReadOnly)
        {
            // Setup
            var mocks = new MockRepository();
            var observable = mocks.Stub<IObservable>();
            var distribution = mocks.Stub<IVariationCoefficientDistribution>();
            mocks.ReplayAll();

            // Call
            var properties = new SimpleVariationCoefficientDistributionProperties(flags, observable)
            {
                Data = distribution
            };

            // Assert
            var dynamicPropertyBag = new DynamicPropertyBag(properties);
            PropertyDescriptorCollection dynamicProperties = dynamicPropertyBag.GetProperties(new Attribute[]
            {
                new BrowsableAttribute(true)
            });
            Assert.AreEqual(3, dynamicProperties.Count);

            var distributionTypePropertyName = TypeUtils.GetMemberName<SimpleVariationCoefficientDistributionProperties>(ndp => ndp.DistributionType);
            PropertyDescriptor distributionTypeProperty = dynamicProperties.Find(distributionTypePropertyName, false);
            Assert.IsTrue(distributionTypeProperty.IsReadOnly);
            AssertAttributeProperty<ResourcesDisplayNameAttribute, string>(distributionTypeProperty.Attributes, "Type verdeling",
                                                                           attribute => attribute.DisplayName);
            AssertAttributeProperty<ResourcesDescriptionAttribute, string>(distributionTypeProperty.Attributes,
                                                                           "Het soort kansverdeling waarin deze parameter gedefinieerd wordt.",
                                                                           attribute => attribute.Description);

            var meanPropertyName = TypeUtils.GetMemberName<SimpleVariationCoefficientDistributionProperties>(ndp => ndp.Mean);
            PropertyDescriptor meanProperty = dynamicProperties.Find(meanPropertyName, false);
            Assert.AreEqual(expectMeanReadOnly, meanProperty.IsReadOnly);
            AssertAttributeProperty<ResourcesDisplayNameAttribute, string>(meanProperty.Attributes, "Verwachtingswaarde",
                                                                           attribute => attribute.DisplayName);

            var standardDeviationPropertyName = TypeUtils.GetMemberName<SimpleVariationCoefficientDistributionProperties>(ndp => ndp.CoefficientOfVariation);
            PropertyDescriptor standardDeviationProperty = dynamicProperties.Find(standardDeviationPropertyName, false);
            Assert.AreEqual(expectVariationCoefficientReadOnly, standardDeviationProperty.IsReadOnly);
            AssertAttributeProperty<ResourcesDisplayNameAttribute, string>(standardDeviationProperty.Attributes, "Variatiecoëfficiënt",
                                                                           attribute => attribute.DisplayName);

            mocks.VerifyAll();
        }

        [Test]
        public void ToString_DataIsNull_ReturnEmptyString()
        {
            // Setup
            var properties = new SimpleVariationCoefficientDistributionProperties(VariationCoefficientDistributionPropertiesReadOnly.All, null);

            // Precondition
            Assert.IsNull(properties.Data);

            // Call
            string text = properties.ToString();

            // Assert
            Assert.AreEqual(string.Empty, text);
        }

        [Test]
        [SetCulture("en-US")]
        public void ToString_DataSet_ReturnValuesDescriptionText()
        {
            // Setup
            var mocks = new MockRepository();
            var distribution = mocks.Stub<IVariationCoefficientDistribution>();
            distribution.Mean = new RoundedDouble(2, 1.23);
            distribution.CoefficientOfVariation = new RoundedDouble(2, 4.56);
            mocks.ReplayAll();

            var properties = new SimpleVariationCoefficientDistributionProperties(VariationCoefficientDistributionPropertiesReadOnly.All, null)
            {
                Data = distribution
            };

            // Precondition
            Assert.IsNotNull(properties.Data);

            // Call
            string text = properties.ToString();

            // Assert
            Assert.AreEqual("1.23 (Variatiecoëfficiënt = 4.56)", text);
            mocks.VerifyAll();
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

        private class SimpleVariationCoefficientDistributionProperties : VariationCoefficientDistributionPropertiesBase<IVariationCoefficientDistribution>
        {
            public SimpleVariationCoefficientDistributionProperties(VariationCoefficientDistributionPropertiesReadOnly propertiesReadOnly, IObservable observable) : base(propertiesReadOnly, observable) {}

            public override string DistributionType
            {
                get
                {
                    return "A";
                }
            }
        }
    }
}