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
using System.ComponentModel;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.Common.Forms.Properties;
using Ringtoets.Common.Forms.PropertyClasses;
using Ringtoets.Common.Forms.TestUtil;

namespace Ringtoets.Common.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class ConfirmingVariationCoefficientDistributionPropertiesBaseTest
    {
        [Test]
        public void Constructor_ReadOnlyWithData_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var distribution = mocks.Stub<IVariationCoefficientDistribution>();
            mocks.ReplayAll();

            // Call
            var properties = new SimpleDistributionProperties(VariationCoefficientDistributionPropertiesReadOnly.All, distribution, null, null, null);

            // Assert
            Assert.IsInstanceOf<ConfirmingVariationCoefficientDistributionPropertiesBase<IVariationCoefficientDistribution, ICalculationInput>>(properties);
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
            TestDelegate call = () => new SimpleDistributionProperties(flags, null, null, null, null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("data", paramName);
        }

        [Test]
        [TestCase(VariationCoefficientDistributionPropertiesReadOnly.Mean)]
        [TestCase(VariationCoefficientDistributionPropertiesReadOnly.CoefficientOfVariation)]
        [TestCase(VariationCoefficientDistributionPropertiesReadOnly.None)]
        public void Constructor_NoCalculationSetWhileChangesPossible_ThrowArgumentException(
            VariationCoefficientDistributionPropertiesReadOnly flags)
        {
            // Setup
            var mocks = new MockRepository();
            var distribution = mocks.Stub<IVariationCoefficientDistribution>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => new SimpleDistributionProperties(flags, distribution, null, null, null);

            // Assert
            var message = "Calculation required if changes are possible.";
            var exception = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, message);
            Assert.AreEqual("calculation", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(VariationCoefficientDistributionPropertiesReadOnly.Mean)]
        [TestCase(VariationCoefficientDistributionPropertiesReadOnly.CoefficientOfVariation)]
        [TestCase(VariationCoefficientDistributionPropertiesReadOnly.None)]
        public void Constructor_NoInputSetWhileChangesPossible_ThrowArgumentException(
            VariationCoefficientDistributionPropertiesReadOnly flags)
        {
            // Setup
            var mocks = new MockRepository();
            var distribution = mocks.Stub<IVariationCoefficientDistribution>();
            var calculation = mocks.Stub<ICalculation>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => new SimpleDistributionProperties(flags, distribution, calculation, null, null);

            // Assert
            var message = "CalculationInput required if changes are possible.";
            var exception = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, message);
            Assert.AreEqual("calculationInput", exception.ParamName);
            mocks.VerifyAll();
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
            var calculation = mocks.Stub<ICalculation>();
            var input = mocks.Stub<ICalculationInput>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => new SimpleDistributionProperties(flags, distribution, calculation, input, null);

            // Assert
            var message = "Change handler required if changes are possible.";
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
            var calculation = mocks.Stub<ICalculation>();
            var input = mocks.Stub<ICalculationInput>();
            var handler = mocks.Stub<ICalculationInputPropertyChangeHandler>();
            mocks.ReplayAll();

            // Call
            var properties = new SimpleDistributionProperties(propertiesReadOnly, distribution, calculation, input, handler);

            // Assert
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
                                                                            expectMeanReadOnly);

            PropertyDescriptor coefficientOfVariationProperty = dynamicProperties[2];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(coefficientOfVariationProperty,
                                                                            "Misc",
                                                                            "Variatiecoëfficiënt",
                                                                            "",
                                                                            expectCoefficientOfVariationReadOnly);
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
            var calculation = mocks.Stub<ICalculation>();
            var input = mocks.Stub<ICalculationInput>();
            var handler = mocks.Stub<ICalculationInputPropertyChangeHandler>();
            mocks.ReplayAll();

            var properties = new SimpleDistributionProperties(propertiesReadOnly, distribution, calculation, input, handler);

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

            var calculation = mocks.Stub<ICalculation>();
            var input = mocks.Stub<ICalculationInput>();
            var handler = mocks.Stub<ICalculationInputPropertyChangeHandler>();
            mocks.ReplayAll();

            var properties = new SimpleDistributionProperties(VariationCoefficientDistributionPropertiesReadOnly.None, distribution, calculation, input, handler);

            // Call
            properties.Data = distribution;

            // Assert
            Assert.AreEqual(distribution.Mean, properties.Mean);
            Assert.AreEqual(distribution.CoefficientOfVariation, properties.CoefficientOfVariation);
            string expectedToString = string.Format("{0} ({1} = {2})",
                                                    distribution.Mean, 
                                                    Resources.Distribution_VariationCoefficient_DisplayName, 
                                                    distribution.CoefficientOfVariation);
            Assert.AreEqual(expectedToString, properties.ToString());
            mocks.VerifyAll();
        }

        [Test]
        public void Mean_ReadOnlyWithObserverable_ThrowsArgumentException()
        {
            // Setup
            var mocks = new MockRepository();
            var distribution = mocks.Stub<IVariationCoefficientDistribution>();
            var calculation = mocks.Stub<ICalculation>();
            var input = mocks.Stub<ICalculationInput>();
            var handler = mocks.Stub<ICalculationInputPropertyChangeHandler>();
            mocks.ReplayAll();

            var properties = new SimpleDistributionProperties(
                VariationCoefficientDistributionPropertiesReadOnly.All,
                distribution,
                calculation,
                input,
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
            var calculation = mocks.Stub<ICalculation>();
            var input = mocks.Stub<ICalculationInput>();
            var observerableMock = mocks.StrictMock<IObservable>();
            observerableMock.Expect(o => o.NotifyObservers());
            mocks.ReplayAll();

            var newMeanValue = new RoundedDouble(3, 20);
            var handler = new CalculationInputSetPropertyValueAfterConfirmationParameterTester<RoundedDouble>(
                input, calculation, newMeanValue, new[]
                {
                    observerableMock
                });

            var properties = new SimpleDistributionProperties(
                VariationCoefficientDistributionPropertiesReadOnly.None,
                distribution,
                calculation,
                input,
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
            var calculation = mocks.Stub<ICalculation>();
            var input = mocks.Stub<ICalculationInput>();
            var handler = mocks.Stub<ICalculationInputPropertyChangeHandler>();
            mocks.ReplayAll();

            var properties = new SimpleDistributionProperties(propertiesReadOnly, distribution, calculation, input, handler);

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
            var calculation = mocks.Stub<ICalculation>();
            var input = mocks.Stub<ICalculationInput>();
            var observerableMock = mocks.StrictMock<IObservable>();
            observerableMock.Expect(o => o.NotifyObservers());
            mocks.ReplayAll();

            var newCoefficientOfVariation = new RoundedDouble(3, 20);
            var handler = new CalculationInputSetPropertyValueAfterConfirmationParameterTester<RoundedDouble>(
                input, calculation, newCoefficientOfVariation, new[]
                {
                    observerableMock
                });

            var properties = new SimpleDistributionProperties(VariationCoefficientDistributionPropertiesReadOnly.None, distribution, calculation, input, handler)
            {
                Data = distribution
            };

            // Call
            properties.CoefficientOfVariation = newCoefficientOfVariation;

            // Assert
            Assert.AreEqual(newCoefficientOfVariation, properties.CoefficientOfVariation);
            mocks.VerifyAll();
        }

        private class SimpleDistributionProperties : ConfirmingVariationCoefficientDistributionPropertiesBase<IVariationCoefficientDistribution, ICalculationInput>
        {
            public SimpleDistributionProperties(VariationCoefficientDistributionPropertiesReadOnly propertiesReadOnly,
                                                IVariationCoefficientDistribution distribution, ICalculation calculation,
                                                ICalculationInput input, ICalculationInputPropertyChangeHandler handler)
                : base(propertiesReadOnly, distribution, calculation, input, handler) {}

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