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
    public class DistributionPropertiesTest
    {
        private MockRepository mockRepository;

        [SetUp]
        public void SetUp()
        {
            mockRepository = new MockRepository();
        }

        [Test]
        public void Constructor_WithParameters_ExpectedValues()
        {
            // Call
            var properties = new SimpleDistributionProperties(DistributionPropertiesReadOnly.None);

            // Assert
            Assert.IsInstanceOf<DistributionProperties>(properties);
            Assert.IsNull(properties.Data);
            Assert.AreEqual("Standaardafwijking", properties.ToString());
            Assert.AreEqual("SimpleDestributionType", properties.DistributionType);
        }

        [Test]
        [TestCase(DistributionPropertiesReadOnly.All, true, true)]
        [TestCase(DistributionPropertiesReadOnly.Mean, true, false)]
        [TestCase(DistributionPropertiesReadOnly.None, false, false)]
        [TestCase(DistributionPropertiesReadOnly.StandardDeviation, false, true)]
        [TestCase(DistributionPropertiesReadOnly.VariationCoefficient, false, false)]
        public void DynamicReadOnlyValidationMethod_VariousReadOnlySet_ExpectedValues(DistributionPropertiesReadOnly propertiesReadOnly, bool expectMeanReadOnly, bool expectStandardDeviationReadOnly)
        {
            // Setup
            var properties = new SimpleDistributionProperties(propertiesReadOnly);

            // Call
            var meanIsReadOnly = properties.DynamicReadOnlyValidationMethod("Mean");
            var standardDeviationIsReadOnly = properties.DynamicReadOnlyValidationMethod("StandardDeviation");
            var doesNotExist = properties.DynamicReadOnlyValidationMethod("DoesNotExist");

            // Assert
            Assert.AreEqual(expectStandardDeviationReadOnly, standardDeviationIsReadOnly);
            Assert.AreEqual(expectMeanReadOnly, meanIsReadOnly);
            Assert.IsFalse(doesNotExist);
        }

        [Test]
        public void Data_SetNewDistributionContextInstance_ReturnCorrectPropertyValues()
        {
            // Setup
            var distribution = new SimpleDistribution
            {
                Mean = new RoundedDouble(1, 1.1),
                StandardDeviation = new RoundedDouble(2, 2.2)
            };
            var propertiesReadOnly = DistributionPropertiesReadOnly.None;

            var properties = new SimpleDistributionProperties(propertiesReadOnly);

            // Call
            properties.Data = distribution;

            // Assert
            Assert.AreEqual(distribution.Mean, properties.Mean);
            Assert.AreEqual(distribution.StandardDeviation, properties.StandardDeviation);
            var expectedToString = string.Format("{0} ({1} = {2})", distribution.Mean, Resources.NormalDistribution_StandardDeviation_DisplayName, distribution.StandardDeviation);
            Assert.AreEqual(expectedToString, properties.ToString());
        }

        [Test]
        [TestCase(DistributionPropertiesReadOnly.None)]
        [TestCase(DistributionPropertiesReadOnly.StandardDeviation)]
        public void SetProperties_EditableMeanWithoutObserverable_ThrowsArgumentException(DistributionPropertiesReadOnly propertiesReadOnly)
        {
            // Setup
            var properties = new SimpleDistributionProperties(propertiesReadOnly)
            {
                Data = new SimpleDistribution()
            };

            // Call
            TestDelegate test = () => properties.Mean = new RoundedDouble(2, 20);

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(test, "No observerable object set.");
        }

        [Test]
        [TestCase(DistributionPropertiesReadOnly.All)]
        [TestCase(DistributionPropertiesReadOnly.Mean)]
        public void SetProperties_ReadOnlyMeanWithObserverable_ThrowsArgumentException(DistributionPropertiesReadOnly propertiesReadOnly)
        {
            // Setup
            var properties = new SimpleDistributionProperties(propertiesReadOnly)
            {
                Data = new SimpleDistribution()
            };

            // Call
            TestDelegate test = () => properties.Mean = new RoundedDouble(2, 20);

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(test, "Mean is set to be read-only.");
        }

        [Test]
        public void SetProperties_MeanWithObserverable_ValueSetNotifyObservers()
        {
            // Setup
            var observerableMock = mockRepository.StrictMock<IObservable>();
            observerableMock.Expect(o => o.NotifyObservers()).Repeat.Once();
            var properties = new SimpleObserverableDistributionProperties(observerableMock, DistributionPropertiesReadOnly.None)
            {
                Data = new SimpleDistribution()
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
        [TestCase(DistributionPropertiesReadOnly.None)]
        [TestCase(DistributionPropertiesReadOnly.Mean)]
        public void SetProperties_EditableStandardDeviationWithoutObserverable_ThrowsArgumentException(DistributionPropertiesReadOnly propertiesReadOnly)
        {
            // Setup
            var properties = new SimpleDistributionProperties(propertiesReadOnly)
            {
                Data = new SimpleDistribution()
            };

            // Call
            TestDelegate test = () => properties.StandardDeviation = new RoundedDouble(2, 20);

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(test, "No observerable object set.");
        }

        [Test]
        [TestCase(DistributionPropertiesReadOnly.All)]
        [TestCase(DistributionPropertiesReadOnly.StandardDeviation)]
        public void SetProperties_ReadOnlyStandardDeviationWithoutObserverable_ThrowsArgumentException(DistributionPropertiesReadOnly propertiesReadOnly)
        {
            // Setup
            var properties = new SimpleDistributionProperties(propertiesReadOnly)
            {
                Data = new SimpleDistribution()
            };

            // Call
            TestDelegate test = () => properties.StandardDeviation = new RoundedDouble(2, 20);

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(test, "StandardDeviation is set to be read-only.");
        }

        [Test]
        public void SetProperties_StandardDeviationWithObserverable_ValueSetNotifyObservers()
        {
            // Setup
            var observerableMock = mockRepository.StrictMock<IObservable>();
            observerableMock.Expect(o => o.NotifyObservers()).Repeat.Once();
            var properties = new SimpleObserverableDistributionProperties(observerableMock, DistributionPropertiesReadOnly.None)
            {
                Data = new SimpleDistribution()
            };
            mockRepository.ReplayAll();
            RoundedDouble newStandardDeviationValue = new RoundedDouble(3, 20);

            // Call
            properties.StandardDeviation = newStandardDeviationValue;

            // Assert
            Assert.AreEqual(newStandardDeviationValue, properties.StandardDeviation);
            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(DistributionPropertiesReadOnly.All, true, true)]
        [TestCase(DistributionPropertiesReadOnly.Mean, true, false)]
        [TestCase(DistributionPropertiesReadOnly.None, false, false)]
        [TestCase(DistributionPropertiesReadOnly.StandardDeviation, false, true)]
        public void PropertyAttributes_ReturnExpectedValues(DistributionPropertiesReadOnly propertiesReadOnly, bool expectMeanReadOnly, bool expectStandardDeviationReadOnly)
        {
            // Call
            var properties = new SimpleDistributionProperties(propertiesReadOnly)
            {
                Data = new SimpleDistribution()
            };

            // Assert
            var dynamicPropertyBag = new DynamicPropertyBag(properties);
            PropertyDescriptorCollection dynamicProperties = dynamicPropertyBag.GetProperties(new Attribute[]
            {
                new BrowsableAttribute(true)
            });
            Assert.AreEqual(3, dynamicProperties.Count);

            var distributionTypePropertyName = TypeUtils.GetMemberName<SimpleDistributionProperties>(ndp => ndp.DistributionType);
            PropertyDescriptor distributionTypeProperty = dynamicProperties.Find(distributionTypePropertyName, false);
            Assert.IsTrue(distributionTypeProperty.IsReadOnly);
            AssertAttributeProperty<ResourcesDisplayNameAttribute, string>(distributionTypeProperty.Attributes, "Type verdeling",
                                                                           attribute => attribute.DisplayName);
            AssertAttributeProperty<ResourcesDescriptionAttribute, string>(distributionTypeProperty.Attributes,
                                                                           "Het soort kansverdeling waarin deze parameter gedefinieerd wordt.",
                                                                           attribute => attribute.Description);

            var meanPropertyName = TypeUtils.GetMemberName<SimpleDistributionProperties>(ndp => ndp.Mean);
            PropertyDescriptor meanProperty = dynamicProperties.Find(meanPropertyName, false);
            Assert.AreEqual(expectMeanReadOnly, meanProperty.IsReadOnly);
            AssertAttributeProperty<ResourcesDisplayNameAttribute, string>(meanProperty.Attributes, "Verwachtingswaarde",
                                                                           attribute => attribute.DisplayName);
            AssertAttributeProperty<ResourcesDescriptionAttribute, string>(meanProperty.Attributes,
                                                                           "De gemiddelde waarde van de normale verdeling.",
                                                                           attribute => attribute.Description);

            var standardDeviationPropertyName = TypeUtils.GetMemberName<SimpleDistributionProperties>(ndp => ndp.StandardDeviation);
            PropertyDescriptor standardDeviationProperty = dynamicProperties.Find(standardDeviationPropertyName, false);
            Assert.AreEqual(expectStandardDeviationReadOnly, standardDeviationProperty.IsReadOnly);
            AssertAttributeProperty<ResourcesDisplayNameAttribute, string>(standardDeviationProperty.Attributes, "Standaardafwijking",
                                                                           attribute => attribute.DisplayName);
            AssertAttributeProperty<ResourcesDescriptionAttribute, string>(standardDeviationProperty.Attributes,
                                                                           "De standaardafwijking van de normale verdeling.",
                                                                           attribute => attribute.Description);
        }

        private class SimpleDistributionProperties : DistributionProperties
        {
            public SimpleDistributionProperties(DistributionPropertiesReadOnly propertiesReadOnly)
                : base(propertiesReadOnly) {}

            public override string DistributionType
            {
                get
                {
                    return "SimpleDestributionType";
                }
            }
        }

        private class SimpleObserverableDistributionProperties : DistributionProperties
        {
            public SimpleObserverableDistributionProperties(IObservable observerable, DistributionPropertiesReadOnly propertiesReadOnly)
                : base(propertiesReadOnly)
            {
                Observerable = observerable;
            }

            public override string DistributionType
            {
                get
                {
                    return "SimpleDestributionType";
                }
            }
        }

        private class SimpleDistribution : IDistribution
        {
            public RoundedDouble Mean { get; set; }
            public RoundedDouble StandardDeviation { get; set; }
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