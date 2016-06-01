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
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.Common.Forms.Properties;
using Ringtoets.Common.Forms.PropertyClasses;

namespace Ringtoets.Common.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class NormalDistributionVariationPropertiesTest
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
            var properties = new NormalDistributionVariationProperties();

            // Assert
            Assert.IsInstanceOf<ObjectProperties<IDistribution>>(properties);
            Assert.IsNull(properties.Data);
            Assert.AreEqual("Variatiecoëfficiënt", properties.ToString());
            Assert.AreEqual("Normale verdeling", properties.DistributionType);
        }

        [Test]
        public void Constructor_WithParameters_ExpectedValues()
        {
            // Call
            var observerableMock = mockRepository.StrictMock<IObservable>();
            mockRepository.ReplayAll();
            var properties = new NormalDistributionVariationProperties(observerableMock, DistributionPropertiesReadOnly.None);

            // Assert
            Assert.IsNull(properties.Data);
            Assert.AreEqual("Variatiecoëfficiënt", properties.ToString());
            Assert.AreEqual("Normale verdeling", properties.DistributionType);
            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(DistributionPropertiesReadOnly.All, true, true)]
        [TestCase(DistributionPropertiesReadOnly.Mean, true, false)]
        [TestCase(DistributionPropertiesReadOnly.None, false, false)]
        [TestCase(DistributionPropertiesReadOnly.StandardDeviation, false, false)]
        [TestCase(DistributionPropertiesReadOnly.VariationCoefficient, false, true)]
        public void DynamicReadOnlyValidationMethod_VariousReadOnlySet_ExpectedValues(DistributionPropertiesReadOnly propertiesReadOnly, bool expectMeanReadOnly, bool expectVariarionCoefficient)
        {
            // Setup
            var observerableMock = mockRepository.StrictMock<IObservable>();
            mockRepository.ReplayAll();
            var properties = new NormalDistributionVariationProperties(observerableMock, propertiesReadOnly);

            // Call
            var meanIsReadOnly = properties.DynamicReadOnlyValidationMethod("Mean");
            var variarionCoefficientIsReadOnly = properties.DynamicReadOnlyValidationMethod("VariationCoefficient");
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
            var observerableMock = mockRepository.StrictMock<IObservable>();
            mockRepository.ReplayAll();

            var properties = new NormalDistributionVariationProperties(observerableMock, DistributionPropertiesReadOnly.None);
            var distribution = new NormalDistribution(2)
            {
                Mean = (RoundedDouble) 2
            };

            // Call
            properties.Data = distribution;

            // Assert
            Assert.AreEqual(distribution.Mean, properties.Mean);
            Assert.AreEqual(distribution.GetVariationCoefficient(), properties.VariationCoefficient);
            var expectedToString = string.Format("{0} ({1} = {2})", distribution.Mean, Resources.Distribution_VariationCoefficient_DisplayName,
                                                 distribution.GetVariationCoefficient());
            Assert.AreEqual(expectedToString, properties.ToString());
            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(DistributionPropertiesReadOnly.None)]
        [TestCase(DistributionPropertiesReadOnly.StandardDeviation)]
        [TestCase(DistributionPropertiesReadOnly.VariationCoefficient)]
        public void SetProperties_EditableMeanWithoutObserverable_ThrowsArgumentException(DistributionPropertiesReadOnly propertiesReadOnly)
        {
            // Setup
            var properties = new NormalDistributionVariationProperties(null, propertiesReadOnly)
            {
                Data = new NormalDistribution(2)
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
            var observerableMock = mockRepository.StrictMock<IObservable>();
            mockRepository.ReplayAll();

            var properties = new NormalDistributionVariationProperties(observerableMock, propertiesReadOnly)
            {
                Data = new NormalDistribution(2)
            };

            // Call
            TestDelegate test = () => properties.Mean = new RoundedDouble(2, 20);

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(test, "Mean is set to be read-only.");
            mockRepository.VerifyAll();
        }

        [Test]
        public void SetProperties_MeanWithObserverable_ValueSetNotifyObservers()
        {
            // Setup
            var observerableMock = mockRepository.StrictMock<IObservable>();
            observerableMock.Expect(o => o.NotifyObservers()).Repeat.Once();
            var properties = new NormalDistributionVariationProperties(observerableMock, DistributionPropertiesReadOnly.None)
            {
                Data = new NormalDistribution(2)
            };
            mockRepository.ReplayAll();
            RoundedDouble newMeanValue = new RoundedDouble(3, 5);

            // Call
            properties.Mean = newMeanValue;

            // Assert
            Assert.AreEqual(newMeanValue, properties.Mean);
            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(DistributionPropertiesReadOnly.None)]
        [TestCase(DistributionPropertiesReadOnly.Mean)]
        public void SetProperties_EditableVariationCoefficientWithoutObserverable_ThrowsArgumentException(DistributionPropertiesReadOnly propertiesReadOnly)
        {
            // Setup
            var properties = new NormalDistributionVariationProperties(null, propertiesReadOnly)
            {
                Data = new NormalDistribution(2)
            };

            // Call
            TestDelegate test = () => properties.VariationCoefficient = new RoundedDouble(2, 20);

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(test, "No observerable object set.");
        }

        [Test]
        [TestCase(DistributionPropertiesReadOnly.All)]
        [TestCase(DistributionPropertiesReadOnly.VariationCoefficient)]
        public void SetProperties_ReadOnlyVariationCoefficientWithoutObserverable_ThrowsArgumentException(DistributionPropertiesReadOnly propertiesReadOnly)
        {
            // Setup
            var properties = new NormalDistributionVariationProperties(null, propertiesReadOnly)
            {
                Data = new NormalDistribution(2)
            };

            // Call
            TestDelegate test = () => properties.VariationCoefficient = new RoundedDouble(2, 20);

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(test, "Variation coefficient is set to be read-only.");
        }

        [Test]
        public void SetProperties_VariationCoefficientWithObserverable_ValueSetNotifyObservers()
        {
            // Setup
            var observerableMock = mockRepository.StrictMock<IObservable>();
            observerableMock.Expect(o => o.NotifyObservers()).Repeat.Once();
            var properties = new NormalDistributionVariationProperties(observerableMock, DistributionPropertiesReadOnly.None)
            {
                Data = new NormalDistribution(2)
                {
                    Mean = (RoundedDouble) 2
                }
            };
            mockRepository.ReplayAll();
            RoundedDouble newVariationCoefficientValue = new RoundedDouble(2, 20);

            // Call
            properties.VariationCoefficient = newVariationCoefficientValue;

            // Assert
            Assert.AreEqual(newVariationCoefficientValue, properties.VariationCoefficient);
            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(DistributionPropertiesReadOnly.All, true, true)]
        [TestCase(DistributionPropertiesReadOnly.Mean, true, false)]
        [TestCase(DistributionPropertiesReadOnly.None, false, false)]
        [TestCase(DistributionPropertiesReadOnly.VariationCoefficient, false, true)]
        public void PropertyAttributes_ReturnExpectedValues(DistributionPropertiesReadOnly propertiesReadOnly, bool expectMeanReadOnly, bool expectVariationCoefficientReadOnly)
        {
            // Setup
            var observerableMock = mockRepository.StrictMock<IObservable>();
            mockRepository.ReplayAll();

            // Call
            var properties = new NormalDistributionVariationProperties(observerableMock, propertiesReadOnly)
            {
                Data = new NormalDistribution(2)
            };

            // Assert
            TypeConverter classTypeConverter = TypeDescriptor.GetConverter(properties, true);
            Assert.IsNotInstanceOf<ExpandableObjectConverter>(classTypeConverter);

            var dynamicPropertyBag = new DynamicPropertyBag(properties);
            PropertyDescriptorCollection dynamicProperties = dynamicPropertyBag.GetProperties(new Attribute[]
            {
                new BrowsableAttribute(true)
            });
            Assert.AreEqual(3, dynamicProperties.Count);

            PropertyDescriptor distributionTypeProperty = dynamicProperties[0];
            Assert.IsNotNull(distributionTypeProperty);
            Assert.IsTrue(distributionTypeProperty.IsReadOnly);
            Assert.AreEqual("Type verdeling", distributionTypeProperty.DisplayName);
            Assert.AreEqual("Het soort kansverdeling waarin deze parameter gedefinieerd wordt.", distributionTypeProperty.Description);

            PropertyDescriptor meanProperty = dynamicProperties[1];
            Assert.IsNotNull(meanProperty);
            Assert.AreEqual(expectMeanReadOnly, meanProperty.IsReadOnly);
            Assert.AreEqual("Verwachtingswaarde", meanProperty.DisplayName);
            Assert.AreEqual("De gemiddelde waarde van de normale verdeling.", meanProperty.Description);

            PropertyDescriptor variationCoefficientProperty = dynamicProperties[2];
            Assert.IsNotNull(variationCoefficientProperty);
            Assert.AreEqual(expectVariationCoefficientReadOnly, variationCoefficientProperty.IsReadOnly);
            Assert.AreEqual("Variatiecoëfficiënt", variationCoefficientProperty.DisplayName);
            Assert.AreEqual("De variatiecoëfficiënt van de normale verdeling.", variationCoefficientProperty.Description);
            mockRepository.VerifyAll();
        }
    }
}