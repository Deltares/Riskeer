// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Ringtoets.Common.Forms.Properties;
using Ringtoets.Common.Forms.PropertyClasses;
using Ringtoets.Common.Forms.TestUtil;

namespace Ringtoets.Common.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class DistributionPropertiesBaseTest
    {
        [Test]
        public void Constructor_WithData_ReadOnlyProperties()
        {
            // Setup
            var mocks = new MockRepository();
            var distribution = mocks.Stub<IDistribution>();
            mocks.ReplayAll();

            // Call
            var properties = new SimpleDistributionProperties(distribution);

            // Assert
            Assert.IsInstanceOf<DistributionPropertiesBase<IDistribution>>(properties);
            Assert.AreSame(distribution, properties.Data);

            AssertPropertiesInState(properties, true, true);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_ReadOnlyWithData_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var distribution = mocks.Stub<IDistribution>();
            mocks.ReplayAll();

            // Call
            var properties = new SimpleDistributionProperties(DistributionPropertiesReadOnly.All, distribution, null);

            // Assert
            Assert.IsInstanceOf<DistributionPropertiesBase<IDistribution>>(properties);
            Assert.AreSame(distribution, properties.Data);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(DistributionPropertiesReadOnly.Mean)]
        [TestCase(DistributionPropertiesReadOnly.StandardDeviation)]
        [TestCase(DistributionPropertiesReadOnly.None)]
        public void Constructor_NoDistributionSetWhileChangesPossible_ThrowArgumentException(
            DistributionPropertiesReadOnly flags)
        {
            // Call
            TestDelegate call = () => new SimpleDistributionProperties(flags, null, null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("distribution", paramName);
        }

        [Test]
        [TestCase(DistributionPropertiesReadOnly.Mean)]
        [TestCase(DistributionPropertiesReadOnly.StandardDeviation)]
        [TestCase(DistributionPropertiesReadOnly.None)]
        public void Constructor_NoHandlerSetWhileChangesPossible_ThrowArgumentException(
            DistributionPropertiesReadOnly flags)
        {
            // Setup
            var mocks = new MockRepository();
            var distribution = mocks.Stub<IDistribution>();
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
        [TestCase(DistributionPropertiesReadOnly.All, true, true)]
        [TestCase(DistributionPropertiesReadOnly.Mean, true, false)]
        [TestCase(DistributionPropertiesReadOnly.None, false, false)]
        [TestCase(DistributionPropertiesReadOnly.StandardDeviation, false, true)]
        public void Constructor_Always_PropertiesHaveExpectedAttributesValues(DistributionPropertiesReadOnly propertiesReadOnly, bool expectMeanReadOnly, bool expectStandardDeviationReadOnly)
        {
            // Setup
            var mocks = new MockRepository();
            var distribution = mocks.Stub<IDistribution>();
            var handler = mocks.Stub<IObservablePropertyChangeHandler>();
            mocks.ReplayAll();

            // Call
            var properties = new SimpleDistributionProperties(propertiesReadOnly, distribution, handler);

            // Assert
            AssertPropertiesInState(properties, expectMeanReadOnly, expectStandardDeviationReadOnly);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(DistributionPropertiesReadOnly.All, true, true)]
        [TestCase(DistributionPropertiesReadOnly.Mean, true, false)]
        [TestCase(DistributionPropertiesReadOnly.None, false, false)]
        [TestCase(DistributionPropertiesReadOnly.StandardDeviation, false, true)]
        public void DynamicReadOnlyValidationMethod_VariousReadOnlySet_ExpectedValues(DistributionPropertiesReadOnly propertiesReadOnly, bool expectMeanReadOnly, bool expectStandardDeviationReadOnly)
        {
            // Setup
            var mocks = new MockRepository();
            var distribution = mocks.Stub<IDistribution>();
            var handler = mocks.Stub<IObservablePropertyChangeHandler>();
            mocks.ReplayAll();

            var properties = new SimpleDistributionProperties(propertiesReadOnly, distribution, handler);

            // Call
            bool meanIsReadOnly = properties.DynamicReadOnlyValidationMethod("Mean");
            bool standardDeviationIsReadOnly = properties.DynamicReadOnlyValidationMethod("StandardDeviation");
            bool doesNotExist = properties.DynamicReadOnlyValidationMethod("DoesNotExist");

            // Assert
            Assert.AreEqual(expectStandardDeviationReadOnly, standardDeviationIsReadOnly);
            Assert.AreEqual(expectMeanReadOnly, meanIsReadOnly);
            Assert.IsFalse(doesNotExist);
            mocks.VerifyAll();
        }

        [Test]
        public void Data_SetNewDistributionContextInstance_ReturnCorrectPropertyValues()
        {
            // Setup
            var mocks = new MockRepository();
            var distribution = mocks.Stub<IDistribution>();
            distribution.Mean = new RoundedDouble(1, 1.1);
            distribution.StandardDeviation = new RoundedDouble(2, 2.2);

            var handler = mocks.Stub<IObservablePropertyChangeHandler>();
            mocks.ReplayAll();

            // Call
            var properties = new SimpleDistributionProperties(DistributionPropertiesReadOnly.None, distribution, handler);

            // Assert
            Assert.AreEqual(distribution.Mean, properties.Mean);
            Assert.AreEqual(distribution.StandardDeviation, properties.StandardDeviation);
            string expectedToString = $"{distribution.Mean} ({Resources.NormalDistribution_StandardDeviation_DisplayName} = {distribution.StandardDeviation})";
            Assert.AreEqual(expectedToString, properties.ToString());
            mocks.VerifyAll();
        }

        [Test]
        public void Mean_ReadOnlyWithObserverable_ThrowsArgumentException()
        {
            // Setup
            var mocks = new MockRepository();
            var distribution = mocks.Stub<IDistribution>();
            var handler = mocks.Stub<IObservablePropertyChangeHandler>();
            mocks.ReplayAll();

            var properties = new SimpleDistributionProperties(
                DistributionPropertiesReadOnly.All,
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
            var distribution = mocks.Stub<IDistribution>();
            var observerable = mocks.StrictMock<IObservable>();
            observerable.Expect(o => o.NotifyObservers());
            mocks.ReplayAll();

            var newMeanValue = new RoundedDouble(3, 20);
            var handler = new SetPropertyValueAfterConfirmationParameterTester(new[]
            {
                observerable
            });

            var properties = new SimpleDistributionProperties(
                DistributionPropertiesReadOnly.None,
                distribution,
                handler);

            // Call
            properties.Mean = newMeanValue;

            // Assert
            Assert.AreEqual(newMeanValue, properties.Mean);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(DistributionPropertiesReadOnly.All)]
        [TestCase(DistributionPropertiesReadOnly.StandardDeviation)]
        public void StandardDeviation_ReadOnlyWithoutObserverable_ThrowsArgumentException(DistributionPropertiesReadOnly propertiesReadOnly)
        {
            // Setup
            var mocks = new MockRepository();
            var distribution = mocks.Stub<IDistribution>();
            var handler = mocks.Stub<IObservablePropertyChangeHandler>();
            mocks.ReplayAll();

            var properties = new SimpleDistributionProperties(
                propertiesReadOnly,
                distribution,
                handler);

            // Call
            TestDelegate test = () => properties.StandardDeviation = new RoundedDouble(2, 20);

            // Assert
            const string expectedMessage = "StandardDeviation is set to be read-only.";
            string actualMessage = Assert.Throws<InvalidOperationException>(test).Message;
            Assert.AreEqual(expectedMessage, actualMessage);
            mocks.VerifyAll();
        }

        [Test]
        public void StandardDeviation_WithObserverable_ValueSetNotifyObservers()
        {
            // Setup
            var mocks = new MockRepository();
            var distribution = mocks.Stub<IDistribution>();
            var observerable = mocks.StrictMock<IObservable>();
            observerable.Expect(o => o.NotifyObservers());
            mocks.ReplayAll();

            var newStandardDeviationValue = new RoundedDouble(3, 20);
            var handler = new SetPropertyValueAfterConfirmationParameterTester(new[]
            {
                observerable
            });

            var properties = new SimpleDistributionProperties(
                DistributionPropertiesReadOnly.None,
                distribution,
                handler);

            // Call
            properties.StandardDeviation = newStandardDeviationValue;

            // Assert
            Assert.AreEqual(newStandardDeviationValue, properties.StandardDeviation);
            mocks.VerifyAll();
        }

        private static void AssertPropertiesInState(SimpleDistributionProperties properties, bool meanReadOnly, bool deviationReadOnly)
        {
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

            PropertyDescriptor standardDeviationProperty = dynamicProperties[2];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(standardDeviationProperty,
                                                                            "Misc",
                                                                            "Standaardafwijking",
                                                                            "",
                                                                            deviationReadOnly);
        }

        private class SimpleDistributionProperties : DistributionPropertiesBase<IDistribution>
        {
            public SimpleDistributionProperties(IDistribution distribution) : base(distribution) {}

            public SimpleDistributionProperties(DistributionPropertiesReadOnly propertiesReadOnly,
                                                IDistribution distribution,
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