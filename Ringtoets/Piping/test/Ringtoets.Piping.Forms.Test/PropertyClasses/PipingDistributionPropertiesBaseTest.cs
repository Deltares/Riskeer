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
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.Common.Forms.PropertyClasses;
using Ringtoets.Common.Forms.TestUtil;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Data.TestUtil;
using Ringtoets.Piping.Forms.PropertyClasses;

namespace Ringtoets.Piping.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class PipingDistributionPropertiesBaseTest
    {
        [Test]
        public void Constructor_WithParameters_ExpectedValues()
        {
            // Setup
            var mockRepository = new MockRepository();
            var distribution = mockRepository.Stub<IDistribution>();
            var handler = mockRepository.Stub<ICalculationInputPropertyChangeHandler<PipingInput, PipingCalculationScenario>>();
            mockRepository.ReplayAll();

            PipingCalculationScenario calculationScenario = PipingCalculationScenarioFactory.CreatePipingCalculationScenarioWithValidInput();

            // Call
            var properties = new SimpleDistributionProperties(DistributionPropertiesReadOnly.All,
                                                              distribution,
                                                              calculationScenario,
                                                              calculationScenario.InputParameters,
                                                              handler);

            // Assert
            Assert.IsInstanceOf<PipingDistributionPropertiesBase<IDistribution, PipingInput, PipingCalculationScenario>>(properties);
            Assert.AreSame(distribution, properties.Data);
            Assert.AreEqual(distribution.Mean, properties.Mean);
            Assert.AreEqual(distribution.StandardDeviation, properties.StandardDeviation);
            Assert.AreEqual("SimpleDestributionType", properties.DistributionType);
            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(DistributionPropertiesReadOnly.All)]
        [TestCase(DistributionPropertiesReadOnly.Mean)]
        [TestCase(DistributionPropertiesReadOnly.StandardDeviation)]
        [TestCase(DistributionPropertiesReadOnly.None)]
        public void Constructor_DistributionNull_ThrowArgumentNullException(DistributionPropertiesReadOnly flags)
        {
            // Setup
            var mockRepository = new MockRepository();
            var handler = mockRepository.Stub<ICalculationInputPropertyChangeHandler<PipingInput, PipingCalculationScenario>>();
            mockRepository.ReplayAll();

            PipingCalculationScenario scenario = PipingCalculationScenarioFactory.CreatePipingCalculationScenarioWithValidInput();

            // Call
            TestDelegate call = () => new SimpleDistributionProperties(flags, null, scenario, scenario.InputParameters, handler);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("data", exception.ParamName);
            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(DistributionPropertiesReadOnly.Mean)]
        [TestCase(DistributionPropertiesReadOnly.StandardDeviation)]
        [TestCase(DistributionPropertiesReadOnly.None)]
        public void Constructor_CalculationNullWhileChangesPossible_ThrowArgumentNullException(DistributionPropertiesReadOnly flags)
        {
            // Setup
            var mockRepository = new MockRepository();
            var distribution = mockRepository.Stub<IDistribution>();
            var handler = mockRepository.Stub<ICalculationInputPropertyChangeHandler<PipingInput, PipingCalculationScenario>>();
            mockRepository.ReplayAll();

            PipingCalculationScenario scenario = PipingCalculationScenarioFactory.CreatePipingCalculationScenarioWithValidInput();

            // Call
            TestDelegate call = () => new SimpleDistributionProperties(flags, distribution, null, scenario.InputParameters, handler);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("calculation", exception.ParamName);
            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(DistributionPropertiesReadOnly.Mean)]
        [TestCase(DistributionPropertiesReadOnly.StandardDeviation)]
        [TestCase(DistributionPropertiesReadOnly.None)]
        public void Constructor_CalculationInputNullWhileChangesPossible_ThrowArgumentNullException(DistributionPropertiesReadOnly flags)
        {
            // Setup
            var mockRepository = new MockRepository();
            var distribution = mockRepository.Stub<IDistribution>();
            var handler = mockRepository.Stub<ICalculationInputPropertyChangeHandler<PipingInput, PipingCalculationScenario>>();
            mockRepository.ReplayAll();

            PipingCalculationScenario scenario = PipingCalculationScenarioFactory.CreatePipingCalculationScenarioWithValidInput();

            // Call
            TestDelegate call = () => new SimpleDistributionProperties(flags, distribution, scenario, null, handler);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("calculationInput", exception.ParamName);
            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(DistributionPropertiesReadOnly.Mean)]
        [TestCase(DistributionPropertiesReadOnly.StandardDeviation)]
        [TestCase(DistributionPropertiesReadOnly.None)]
        public void Constructor_ChangeHandlerNullWhileChangesPossible_ThrowArgumentNullException(DistributionPropertiesReadOnly flags)
        {
            // Setup
            var mockRepository = new MockRepository();
            var distribution = mockRepository.Stub<IDistribution>();
            mockRepository.ReplayAll();

            PipingCalculationScenario scenario = PipingCalculationScenarioFactory.CreatePipingCalculationScenarioWithValidInput();

            // Call
            TestDelegate call = () => new SimpleDistributionProperties(flags, distribution, scenario, scenario.InputParameters, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("handler", exception.ParamName);
            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(DistributionPropertiesReadOnly.All, true, true)]
        [TestCase(DistributionPropertiesReadOnly.Mean, true, false)]
        [TestCase(DistributionPropertiesReadOnly.None, false, false)]
        [TestCase(DistributionPropertiesReadOnly.StandardDeviation, false, true)]
        public void Constructor_Always_PropertiesHaveExpectedAttributesValues(DistributionPropertiesReadOnly propertiesReadOnly,
            bool expectMeanReadOnly,
            bool expectStandardDeviationReadOnly)
        {
            // Setup
            var mockRepository = new MockRepository();
            var distribution = mockRepository.Stub<IDistribution>();
            var handler = mockRepository.Stub<ICalculationInputPropertyChangeHandler<PipingInput, PipingCalculationScenario>>();
            mockRepository.ReplayAll();

            PipingCalculationScenario calculationScenario = PipingCalculationScenarioFactory.CreatePipingCalculationScenarioWithValidInput();

            // Call
            var properties = new SimpleDistributionProperties(propertiesReadOnly,
                                                              distribution,
                                                              calculationScenario,
                                                              calculationScenario.InputParameters,
                                                              handler);

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

            PropertyDescriptor standardDeviationProperty = dynamicProperties[2];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(standardDeviationProperty,
                                                                            "Misc",
                                                                            "Standaardafwijking",
                                                                            "",
                                                                            expectStandardDeviationReadOnly);
            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(DistributionPropertiesReadOnly.All)]
        [TestCase(DistributionPropertiesReadOnly.Mean)]
        public void GivenReadOnlyMean_WhenSettingMean_ThenThrowArgumentException(DistributionPropertiesReadOnly propertiesReadOnly)
        {
            // Given
            var mockRepository = new MockRepository();
            var distribution = mockRepository.Stub<IDistribution>();
            var handler = mockRepository.Stub<ICalculationInputPropertyChangeHandler<PipingInput, PipingCalculationScenario>>();
            mockRepository.ReplayAll();

            PipingCalculationScenario calculationScenario = PipingCalculationScenarioFactory.CreatePipingCalculationScenarioWithValidInput();

            var properties = new SimpleDistributionProperties(propertiesReadOnly,
                                                              distribution,
                                                              calculationScenario,
                                                              calculationScenario.InputParameters,
                                                              handler);

            // When
            TestDelegate test = () => properties.Mean = new RoundedDouble(2, 4);

            // Then
            const string expectedMessage = "Mean is set to be read-only.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(test, expectedMessage);
            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(DistributionPropertiesReadOnly.All)]
        [TestCase(DistributionPropertiesReadOnly.StandardDeviation)]
        public void GivenReadOnlyStandardDeviation_WhenSettingStandardDeviation_ThenThrowArgumentException(DistributionPropertiesReadOnly propertiesReadOnly)
        {
            // Given
            var mockRepository = new MockRepository();
            var distribution = mockRepository.Stub<IDistribution>();
            var handler = mockRepository.Stub<ICalculationInputPropertyChangeHandler<PipingInput, PipingCalculationScenario>>();
            mockRepository.ReplayAll();

            PipingCalculationScenario calculationScenario = PipingCalculationScenarioFactory.CreatePipingCalculationScenarioWithValidInput();

            var properties = new SimpleDistributionProperties(propertiesReadOnly,
                                                              distribution,
                                                              calculationScenario,
                                                              calculationScenario.InputParameters,
                                                              handler);

            // When
            TestDelegate test = () => properties.StandardDeviation = new RoundedDouble(2, 4);

            // Then
            const string expectedMessage = "StandardDeviation is set to be read-only.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(test, expectedMessage);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Mean_SetValidValue_SetsValueAndUpdatesObserver()
        {
            // Setup
            var mean = new RoundedDouble(2, 3);

            // Call & Assert
            SetPropertyAndVerifyNotifcationsAndOutputForCalculation(properties => properties.Mean = mean, mean);
        }

        [Test]
        public void StandardDeviation_SetValidValue_SetsValueAndUpdatesObserver()
        {
            // Setup
            var standardDeviation = new RoundedDouble(2, 3);

            // Call & Assert
            SetPropertyAndVerifyNotifcationsAndOutputForCalculation(properties => properties.StandardDeviation = standardDeviation, standardDeviation);
        }

        [Test]
        [SetCulture("nl-NL")]
        public void ToString_Always_ReturnDistributionName()
        {
            // Setup
            // Setup
            var mockRepository = new MockRepository();
            var distribution = mockRepository.Stub<IDistribution>();
            distribution.Mean = new RoundedDouble(2, 1);
            distribution.StandardDeviation = new RoundedDouble(2, 2);
            var handler = mockRepository.Stub<ICalculationInputPropertyChangeHandler<PipingInput, PipingCalculationScenario>>();
            mockRepository.ReplayAll();

            PipingCalculationScenario calculationScenario = PipingCalculationScenarioFactory.CreatePipingCalculationScenarioWithValidInput();

            var properties = new SimpleDistributionProperties(DistributionPropertiesReadOnly.None, 
                                                              distribution,
                                                              calculationScenario,
                                                              calculationScenario.InputParameters,
                                                              handler);

            // Call
            var propertyName = properties.ToString();

            // Assert
            Assert.AreEqual("1,00 (Standaardafwijking = 2,00)", propertyName);
        }

        private void SetPropertyAndVerifyNotifcationsAndOutputForCalculation<TPropertyValue>(
            Action<SimpleDistributionProperties> setProperty,
            TPropertyValue value)
        {
            // Setup
            var mocks = new MockRepository();
            var distribution = mocks.Stub<IDistribution>();
            var observable = mocks.StrictMock<IObservable>();
            observable.Expect(o => o.NotifyObservers());
            mocks.ReplayAll();

            PipingCalculationScenario calculationScenario = PipingCalculationScenarioFactory.CreatePipingCalculationScenarioWithValidInput();

            var handler = new CalculationInputSetPropertyValueAfterConfirmationParameterTester<PipingInput, PipingCalculationScenario, TPropertyValue>(
                calculationScenario.InputParameters,
                calculationScenario,
                value,
                new[]
                {
                    observable
                });

            var properties = new SimpleDistributionProperties(DistributionPropertiesReadOnly.None,
                                                             distribution,
                                                             calculationScenario,
                                                             calculationScenario.InputParameters,
                                                             handler);

            // Call
            setProperty(properties);

            // Assert
            Assert.IsTrue(handler.Called);
            mocks.VerifyAll();
        }

        private class SimpleDistributionProperties : PipingDistributionPropertiesBase<IDistribution, PipingInput, PipingCalculationScenario>
        {
            public SimpleDistributionProperties(DistributionPropertiesReadOnly propertiesReadOnly,
                                                IDistribution distribution,
                                                PipingCalculationScenario scenario,
                                                PipingInput calculationInput,
                                                ICalculationInputPropertyChangeHandler<PipingInput, PipingCalculationScenario> handler)
                : base(propertiesReadOnly, distribution, scenario, calculationInput, handler) {}

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