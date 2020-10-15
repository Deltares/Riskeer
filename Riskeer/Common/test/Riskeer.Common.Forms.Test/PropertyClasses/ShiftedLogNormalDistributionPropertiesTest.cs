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
using Core.Common.Base.Data;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.Probabilistics;
using Riskeer.Common.Forms.PropertyClasses;

namespace Riskeer.Common.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class ShiftedLogNormalDistributionPropertiesTest
    {
        [Test]
        public void SingleParameterConstructor_DistributionNull_ThrowArgumentNullException()
        {
            // Call
            void Call() => new ShiftedLogNormalDistributionProperties(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("distribution", exception.ParamName);
        }

        [Test]
        public void SingleParameterConstructor_ExpectedValues()
        {
            // Setup
            var distribution = new LogNormalDistribution();

            // Call
            var properties = new ShiftedLogNormalDistributionProperties(distribution);

            // Assert
            Assert.IsInstanceOf<LogNormalDistributionProperties>(properties);
            Assert.AreSame(distribution, properties.Data);
            Assert.AreEqual(distribution.Mean, properties.Mean);
            Assert.AreEqual(distribution.StandardDeviation, properties.StandardDeviation);
            Assert.AreEqual(distribution.Shift, properties.Shift);
            Assert.AreEqual("Lognormaal", properties.DistributionType);
        }

        [Test]
        public void Constructor_DistributionNull_ThrowArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var handler = mocks.Stub<IObservablePropertyChangeHandler>();
            mocks.ReplayAll();

            // Call
            void Call() => new ShiftedLogNormalDistributionProperties(DistributionReadOnlyProperties.None, null, handler);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("distribution", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_HandlerNullAndReadOnlyPropertiesNone_ThrowArgumentException()
        {
            // Call
            void Call() => new ShiftedLogNormalDistributionProperties(DistributionReadOnlyProperties.None, new LogNormalDistribution(), null);

            // Assert
            var exception = Assert.Throws<ArgumentException>(Call);
            Assert.AreEqual("handler", exception.ParamName);
            Assert.AreEqual("Change handler required if changes are possible.\r\nParameter name: handler", exception.Message);
        }

        [Test]
        public void Constructor_WithDistribution_ExpectedValues()
        {
            // Setup
            var distribution = new LogNormalDistribution();

            // Call
            var properties = new ShiftedLogNormalDistributionProperties(distribution);

            // Assert
            Assert.IsInstanceOf<DistributionPropertiesBase<LogNormalDistribution>>(properties);
            Assert.AreSame(distribution, properties.Data);
            Assert.AreEqual("Lognormaal", properties.DistributionType);

            AssertPropertiesInState(properties, true, true);
        }

        [Test]
        public void Constructor_WithParameters_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var handler = mocks.Stub<IObservablePropertyChangeHandler>();
            mocks.ReplayAll();

            var distribution = new LogNormalDistribution();

            // Call
            var properties = new ShiftedLogNormalDistributionProperties(
                DistributionReadOnlyProperties.None, distribution, handler);

            // Assert
            Assert.IsInstanceOf<DistributionPropertiesBase<LogNormalDistribution>>(properties);
            Assert.AreSame(distribution, properties.Data);
            Assert.AreEqual("Lognormaal", properties.DistributionType);

            AssertPropertiesInState(properties, false, false);

            mocks.VerifyAll();
        }

        [Test]
        public void GetProperties_WithData_ReturnExpectedValues()
        {
            // Setup
            var mockRepository = new MockRepository();
            var handler = mockRepository.Stub<IObservablePropertyChangeHandler>();
            mockRepository.ReplayAll();

            var distribution = new LogNormalDistribution(2)
            {
                Mean = new RoundedDouble(2, 1),
                StandardDeviation = new RoundedDouble(2, 2),
                Shift = new RoundedDouble(2, 0.2)
            };

            // Call
            var properties = new ShiftedLogNormalDistributionProperties(DistributionReadOnlyProperties.None,
                                                                        distribution,
                                                                        handler);

            // Assert
            Assert.AreEqual("Lognormaal", properties.DistributionType);
            Assert.AreEqual(distribution.Mean, properties.Mean);
            Assert.AreEqual(distribution.StandardDeviation, properties.StandardDeviation);
            Assert.AreEqual(distribution.Shift, properties.Shift);
        }

        [Test]
        [SetCulture("nl-NL")]
        public void ToString_Always_ReturnDistributionName()
        {
            // Setup
            var mockRepository = new MockRepository();
            var handler = mockRepository.Stub<IObservablePropertyChangeHandler>();
            mockRepository.ReplayAll();

            var distribution = new LogNormalDistribution(2)
            {
                Mean = new RoundedDouble(2, 1),
                StandardDeviation = new RoundedDouble(2, 2),
                Shift = new RoundedDouble(2, 0.3)
            };

            // Call
            var properties = new ShiftedLogNormalDistributionProperties(DistributionReadOnlyProperties.None,
                                                                        distribution,
                                                                        handler);

            // Call
            var propertyName = properties.ToString();

            // Assert
            Assert.AreEqual("1,00 (Standaardafwijking = 2,00) (Verschuiving = 0,30)", propertyName);
        }

        private static void AssertPropertiesInState(ShiftedLogNormalDistributionProperties properties, bool meanReadOnly, bool deviationReadOnly)
        {
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(4, dynamicProperties.Count);

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
                                                                            "De gemiddelde waarde van de lognormale verdeling.",
                                                                            meanReadOnly);

            PropertyDescriptor standardDeviationProperty = dynamicProperties[2];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(standardDeviationProperty,
                                                                            "Misc",
                                                                            "Standaardafwijking",
                                                                            "De standaardafwijking van de lognormale verdeling.",
                                                                            deviationReadOnly);

            PropertyDescriptor shiftProperty = dynamicProperties[3];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(shiftProperty,
                                                                            "Misc",
                                                                            "Verschuiving",
                                                                            "De hoeveelheid waarmee de kansverdeling naar rechts (richting van positieve X-as) verschoven is.",
                                                                            true);
        }
    }
}