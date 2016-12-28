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
using Ringtoets.Common.Forms.Properties;
using Ringtoets.Common.Forms.PropertyClasses;

namespace Ringtoets.Common.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class DistributionPropertiesBaseTest
    {
        [Test]
        public void Constructor_WithParameters_ExpectedValues()
        {
            // Call
            var properties = new SimpleDistributionProperties(DistributionPropertiesReadOnly.All, null, null);

            // Assert
            Assert.IsInstanceOf<DistributionPropertiesBase<IDistribution>>(properties);
            Assert.IsNull(properties.Data);
        }

        [Test]
        [TestCase(DistributionPropertiesReadOnly.Mean)]
        [TestCase(DistributionPropertiesReadOnly.StandardDeviation)]
        [TestCase(DistributionPropertiesReadOnly.None)]
        public void Constructor_NoObservableSetWhileChangesPossible_ThrowArgumentException(
            DistributionPropertiesReadOnly flags)
        {
            // Call
            TestDelegate call = () => new SimpleDistributionProperties(flags, null, null);

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, "Observable must be specified unless no property can be set.");
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
            var observable = mocks.Stub<IObservable>();
            var distribution = mocks.Stub<IDistribution>();
            mocks.ReplayAll();

            // Call
            var properties = new SimpleDistributionProperties(propertiesReadOnly, observable, null)
            {
                Data = distribution
            };

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
            var observable = mocks.Stub<IObservable>();
            mocks.ReplayAll();

            var properties = new SimpleDistributionProperties(propertiesReadOnly, observable, null);

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
            var observable = mocks.Stub<IObservable>();
            var distribution = mocks.Stub<IDistribution>();
            distribution.Mean = new RoundedDouble(1, 1.1);
            distribution.StandardDeviation = new RoundedDouble(2, 2.2);
            mocks.ReplayAll();

            var properties = new SimpleDistributionProperties(DistributionPropertiesReadOnly.None, observable, null);

            // Call
            properties.Data = distribution;

            // Assert
            Assert.AreEqual(distribution.Mean, properties.Mean);
            Assert.AreEqual(distribution.StandardDeviation, properties.StandardDeviation);
            string expectedToString = string.Format("{0} ({1} = {2})",
                                                    distribution.Mean, Resources.NormalDistribution_StandardDeviation_DisplayName, distribution.StandardDeviation);
            Assert.AreEqual(expectedToString, properties.ToString());
            mocks.VerifyAll();
        }

        [Test]
        public void Mean_ReadOnlyWithObserverable_ThrowsArgumentException()
        {
            // Setup
            var mocks = new MockRepository();
            var observable = mocks.Stub<IObservable>();
            var distribution = mocks.Stub<IDistribution>();
            mocks.ReplayAll();

            var properties = new SimpleDistributionProperties(DistributionPropertiesReadOnly.All, observable, null)
            {
                Data = distribution
            };

            // Call
            TestDelegate test = () => properties.Mean = new RoundedDouble(2, 20);

            // Assert
            const string expectedMessage = "Mean is set to be read-only.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(test, expectedMessage);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Mean_WithObserverable_ValueSetNotifyObservers(bool withHandler)
        {
            // Setup
            var mockRepository = new MockRepository();
            var observerableMock = mockRepository.StrictMock<IObservable>();
            observerableMock.Expect(o => o.NotifyObservers());
            DistributionPropertiesBase<IDistribution>.IChangeHandler handler = null;
            if (withHandler)
            {
                handler = mockRepository.StrictMock<DistributionPropertiesBase<IDistribution>.IChangeHandler>();
                handler.Expect(o => o.PropertyChanged());
            }
            var distribution = mockRepository.Stub<IDistribution>();
            mockRepository.ReplayAll();

            var properties = new SimpleDistributionProperties(DistributionPropertiesReadOnly.None, observerableMock, handler)
            {
                Data = distribution
            };
            RoundedDouble newMeanValue = new RoundedDouble(3, 20);

            // Call
            properties.Mean = newMeanValue;

            // Assert
            Assert.AreEqual(newMeanValue, properties.Mean);
            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(DistributionPropertiesReadOnly.All)]
        [TestCase(DistributionPropertiesReadOnly.StandardDeviation)]
        public void StandardDeviation_ReadOnlyWithoutObserverable_ThrowsArgumentException(DistributionPropertiesReadOnly propertiesReadOnly)
        {
            // Setup
            var mocks = new MockRepository();
            var observable = mocks.Stub<IObservable>();
            var distribution = mocks.Stub<IDistribution>();
            mocks.ReplayAll();

            var properties = new SimpleDistributionProperties(propertiesReadOnly, observable, null)
            {
                Data = distribution
            };

            // Call
            TestDelegate test = () => properties.StandardDeviation = new RoundedDouble(2, 20);

            // Assert
            const string expectedMessage = "StandardDeviation is set to be read-only.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(test, expectedMessage);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void StandardDeviation_WithObserverable_ValueSetNotifyObservers(bool withHandler)
        {
            // Setup
            var mockRepository = new MockRepository();
            var observerableMock = mockRepository.StrictMock<IObservable>();
            observerableMock.Expect(o => o.NotifyObservers());
            DistributionPropertiesBase<IDistribution>.IChangeHandler handler = null;
            if (withHandler)
            {
                handler = mockRepository.StrictMock<DistributionPropertiesBase<IDistribution>.IChangeHandler>();
                handler.Expect(o => o.PropertyChanged());
            }
            var distribution = mockRepository.Stub<IDistribution>();
            mockRepository.ReplayAll();

            var properties = new SimpleDistributionProperties(DistributionPropertiesReadOnly.None, observerableMock, handler)
            {
                Data = distribution
            };
            RoundedDouble newStandardDeviationValue = new RoundedDouble(3, 20);

            // Call
            properties.StandardDeviation = newStandardDeviationValue;

            // Assert
            Assert.AreEqual(newStandardDeviationValue, properties.StandardDeviation);
            mockRepository.VerifyAll();
        }

        private class SimpleDistributionProperties : DistributionPropertiesBase<IDistribution>
        {
            public SimpleDistributionProperties(DistributionPropertiesReadOnly propertiesReadOnly, IObservable observable, IChangeHandler handler)
                : base(propertiesReadOnly, observable, handler) {}

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