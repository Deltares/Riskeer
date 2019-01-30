// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.Probabilistics;
using Riskeer.Common.Forms.Properties;
using Riskeer.Common.Forms.PropertyClasses;
using Riskeer.Common.Forms.TestUtil;

namespace Riskeer.Common.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class VariationCoefficientDistributionPropertiesBaseTest
    {
        [Test]
        public void Constructor_WithData_ReadOnlyProperties()
        {
            // Setup
            var mocks = new MockRepository();
            var distribution = mocks.Stub<IVariationCoefficientDistribution>();
            mocks.ReplayAll();

            // Call
            var properties = new SimpleDistributionProperties(distribution);

            // Assert
            Assert.AreSame(distribution, properties.Data);

            AssertPropertiesInState(properties, true, true);

            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_ReadOnlyWithData_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var distribution = mocks.Stub<IVariationCoefficientDistribution>();
            mocks.ReplayAll();

            // Call
            var properties = new SimpleDistributionProperties(VariationCoefficientDistributionPropertiesReadOnly.All, distribution, null);

            // Assert
            Assert.IsInstanceOf<VariationCoefficientDistributionPropertiesBase<IVariationCoefficientDistribution>>(properties);
            Assert.AreSame(distribution, properties.Data);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(VariationCoefficientDistributionPropertiesReadOnly.Mean)]
        [TestCase(VariationCoefficientDistributionPropertiesReadOnly.CoefficientOfVariation)]
        [TestCase(VariationCoefficientDistributionPropertiesReadOnly.None)]
        public void Constructor_NoDistributionSetWhileChangesPossible_ThrowArgumentException(
            VariationCoefficientDistributionPropertiesReadOnly flags)
        {
            // Call
            TestDelegate call = () => new SimpleDistributionProperties(flags, null, null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("distribution", paramName);
        }

        [Test]
        [TestCase(VariationCoefficientDistributionPropertiesReadOnly.Mean)]
        [TestCase(VariationCoefficientDistributionPropertiesReadOnly.CoefficientOfVariation)]
        [TestCase(VariationCoefficientDistributionPropertiesReadOnly.None)]
        public void Constructor_NoHandlerSetWhileChangesPossible_ThrowArgumentException(
            VariationCoefficientDistributionPropertiesReadOnly flags)
        {
            // Setup
            var mocks = new MockRepository();
            var distribution = mocks.Stub<IVariationCoefficientDistribution>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => new SimpleDistributionProperties(flags, distribution, null);

            // Assert
            const string message = "Change handler required if changes are possible.";
            var exception = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, message);
            Assert.AreEqual("handler", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(VariationCoefficientDistributionPropertiesReadOnly.All, true, true)]
        [TestCase(VariationCoefficientDistributionPropertiesReadOnly.Mean, true, false)]
        [TestCase(VariationCoefficientDistributionPropertiesReadOnly.None, false, false)]
        [TestCase(VariationCoefficientDistributionPropertiesReadOnly.CoefficientOfVariation, false, true)]
        public void Constructor_Always_PropertiesHaveExpectedAttributesValues(VariationCoefficientDistributionPropertiesReadOnly propertiesReadOnly, bool expectMeanReadOnly, bool expectCoefficientOfVariationReadOnly)
        {
            // Setup
            var mocks = new MockRepository();
            var distribution = mocks.Stub<IVariationCoefficientDistribution>();
            var handler = mocks.Stub<IObservablePropertyChangeHandler>();
            mocks.ReplayAll();

            // Call
            var properties = new SimpleDistributionProperties(propertiesReadOnly, distribution, handler);

            // Assert
            AssertPropertiesInState(properties, expectMeanReadOnly, expectCoefficientOfVariationReadOnly);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(VariationCoefficientDistributionPropertiesReadOnly.All, true, true)]
        [TestCase(VariationCoefficientDistributionPropertiesReadOnly.Mean, true, false)]
        [TestCase(VariationCoefficientDistributionPropertiesReadOnly.None, false, false)]
        [TestCase(VariationCoefficientDistributionPropertiesReadOnly.CoefficientOfVariation, false, true)]
        public void DynamicReadOnlyValidationMethod_VariousReadOnlySet_ExpectedValues(VariationCoefficientDistributionPropertiesReadOnly propertiesReadOnly, bool expectMeanReadOnly, bool expectCoefficientOfVariationReadOnly)
        {
            // Setup
            var mocks = new MockRepository();
            var distribution = mocks.Stub<IVariationCoefficientDistribution>();
            var handler = mocks.Stub<IObservablePropertyChangeHandler>();
            mocks.ReplayAll();

            var properties = new SimpleDistributionProperties(propertiesReadOnly, distribution, handler);

            // Call
            bool meanIsReadOnly = properties.DynamicReadOnlyValidationMethod("Mean");
            bool coefficientOfVariationIsReadOnly = properties.DynamicReadOnlyValidationMethod("CoefficientOfVariation");
            bool doesNotExist = properties.DynamicReadOnlyValidationMethod("DoesNotExist");

            // Assert
            Assert.AreEqual(expectCoefficientOfVariationReadOnly, coefficientOfVariationIsReadOnly);
            Assert.AreEqual(expectMeanReadOnly, meanIsReadOnly);
            Assert.IsFalse(doesNotExist);
            mocks.VerifyAll();
        }

        [Test]
        public void Data_SetNewDistributionContextInstance_ReturnCorrectPropertyValues()
        {
            // Setup
            var mocks = new MockRepository();
            var distribution = mocks.Stub<IVariationCoefficientDistribution>();
            distribution.Mean = new RoundedDouble(1, 1.1);
            distribution.CoefficientOfVariation = new RoundedDouble(2, 2.2);

            var handler = mocks.Stub<IObservablePropertyChangeHandler>();
            mocks.ReplayAll();

            var properties = new SimpleDistributionProperties(VariationCoefficientDistributionPropertiesReadOnly.None, distribution, handler);

            // Call
            properties.Data = distribution;

            // Assert
            Assert.AreEqual(distribution.Mean, properties.Mean);
            Assert.AreEqual(distribution.CoefficientOfVariation, properties.CoefficientOfVariation);
            string expectedToString = $"{distribution.Mean} ({Resources.Distribution_VariationCoefficient_DisplayName} = {distribution.CoefficientOfVariation})";
            Assert.AreEqual(expectedToString, properties.ToString());
            mocks.VerifyAll();
        }

        [Test]
        public void Mean_ReadOnlyWithObserverable_ThrowsArgumentException()
        {
            // Setup
            var mocks = new MockRepository();
            var distribution = mocks.Stub<IVariationCoefficientDistribution>();
            var handler = mocks.Stub<IObservablePropertyChangeHandler>();
            mocks.ReplayAll();

            var properties = new SimpleDistributionProperties(
                VariationCoefficientDistributionPropertiesReadOnly.All,
                distribution,
                handler);

            // Call
            TestDelegate test = () => properties.Mean = new RoundedDouble(2, 20);

            // Assert
            const string expectedMessage = "Mean is set to be read-only.";
            string actualMessage = Assert.Throws<InvalidOperationException>(test).Message;
            Assert.AreEqual(expectedMessage, actualMessage);
            mocks.VerifyAll();
        }

        [Test]
        public void Mean_WithObserverable_ValueSetNotifyObservers()
        {
            // Setup
            var mocks = new MockRepository();
            var distribution = mocks.Stub<IVariationCoefficientDistribution>();
            var observerable = mocks.StrictMock<IObservable>();
            observerable.Expect(o => o.NotifyObservers());
            mocks.ReplayAll();

            var newMeanValue = new RoundedDouble(3, 20);
            var handler = new SetPropertyValueAfterConfirmationParameterTester(new[]
            {
                observerable
            });

            var properties = new SimpleDistributionProperties(
                VariationCoefficientDistributionPropertiesReadOnly.None,
                distribution,
                handler);

            // Call
            properties.Mean = newMeanValue;

            // Assert
            Assert.AreEqual(newMeanValue, properties.Mean);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(VariationCoefficientDistributionPropertiesReadOnly.All)]
        [TestCase(VariationCoefficientDistributionPropertiesReadOnly.CoefficientOfVariation)]
        public void CoefficientOfVariation_ReadOnlyWithoutObserverable_ThrowsArgumentException(VariationCoefficientDistributionPropertiesReadOnly propertiesReadOnly)
        {
            // Setup
            var mocks = new MockRepository();
            var distribution = mocks.Stub<IVariationCoefficientDistribution>();
            var handler = mocks.Stub<IObservablePropertyChangeHandler>();
            mocks.ReplayAll();

            var properties = new SimpleDistributionProperties(propertiesReadOnly, distribution, handler);

            // Call
            TestDelegate test = () => properties.CoefficientOfVariation = new RoundedDouble(2, 20);

            // Assert
            const string expectedMessage = "CoefficientOfVariation is set to be read-only.";
            string actualMessage = Assert.Throws<InvalidOperationException>(test).Message;
            Assert.AreEqual(expectedMessage, actualMessage);
            mocks.VerifyAll();
        }

        [Test]
        public void CoefficientOfVariation_WithObserverable_ValueSetNotifyObservers()
        {
            // Setup
            var mocks = new MockRepository();
            var distribution = mocks.Stub<IVariationCoefficientDistribution>();
            var observerable = mocks.StrictMock<IObservable>();
            observerable.Expect(o => o.NotifyObservers());
            mocks.ReplayAll();

            var newCoefficientOfVariation = new RoundedDouble(3, 20);
            var handler = new SetPropertyValueAfterConfirmationParameterTester(new[]
            {
                observerable
            });

            var properties = new SimpleDistributionProperties(VariationCoefficientDistributionPropertiesReadOnly.None, distribution, handler)
            {
                Data = distribution
            };

            // Call
            properties.CoefficientOfVariation = newCoefficientOfVariation;

            // Assert
            Assert.AreEqual(newCoefficientOfVariation, properties.CoefficientOfVariation);
            mocks.VerifyAll();
        }

        private static void AssertPropertiesInState(SimpleDistributionProperties properties, bool meanReadOnly, bool variationCoefficientReadOnly)
        {
            Assert.IsInstanceOf<VariationCoefficientDistributionPropertiesBase<IVariationCoefficientDistribution>>(properties);

            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(3, dynamicProperties.Count);

            PropertyDescriptor distributionTypeProperty = dynamicProperties[0];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(distributionTypeProperty,
                                                                            "Misc",
                                                                            "Type verdeling",
                                                                            "Het soort kansverdeling waarin deze parameter gedefinieerd wordt.",
                                                                            true);

            PropertyDescriptor meanProperty = dynamicProperties[1];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(meanProperty,
                                                                            "Misc",
                                                                            "Verwachtingswaarde",
                                                                            "",
                                                                            meanReadOnly);

            PropertyDescriptor coefficientOfVariationProperty = dynamicProperties[2];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(coefficientOfVariationProperty,
                                                                            "Misc",
                                                                            "Variatiecoëfficiënt",
                                                                            "",
                                                                            variationCoefficientReadOnly);
        }

        private class SimpleDistributionProperties : VariationCoefficientDistributionPropertiesBase<IVariationCoefficientDistribution>
        {
            public SimpleDistributionProperties(IVariationCoefficientDistribution distribution) : base(distribution) {}

            public SimpleDistributionProperties(VariationCoefficientDistributionPropertiesReadOnly propertiesReadOnly,
                                                IVariationCoefficientDistribution distribution,
                                                IObservablePropertyChangeHandler handler)
                : base(propertiesReadOnly, distribution, handler) {}

            public override string DistributionType
            {
                get
                {
                    return "SimpleDestributionType";
                }
            }
        }
    }
}