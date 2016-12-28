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
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.Common.Forms.PropertyClasses;

namespace Ringtoets.Common.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class NormalDistributionPropertiesTest
    {
        [Test]
        public void Constructor_WithoutParameters_ExpectedValues()
        {
            // Call
            var properties = new NormalDistributionProperties();

            // Assert
            Assert.IsInstanceOf<DistributionPropertiesBase<NormalDistribution>>(properties);
            Assert.IsNull(properties.Data);
            Assert.AreEqual("Normaal", properties.DistributionType);
        }

        [Test]
        [TestCase(DistributionPropertiesReadOnly.Mean)]
        [TestCase(DistributionPropertiesReadOnly.StandardDeviation)]
        [TestCase(DistributionPropertiesReadOnly.None)]
        public void Constructor_NoObservableSetWhileChangesPossible_ThrowArgumentException(
            DistributionPropertiesReadOnly flags)
        {
            // Call
            TestDelegate call = () => new NormalDistributionProperties(flags, null, null);

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, "Observable must be specified unless no property can be set.");
        }

        [Test]
        public void Constructor_WithParameters_ExpectedValues()
        {
            // Setup
            var mockRepository = new MockRepository();
            var observerableMock = mockRepository.StrictMock<IObservable>();
            mockRepository.ReplayAll();

            // Call
            var properties = new NormalDistributionProperties(DistributionPropertiesReadOnly.None, observerableMock, null);

            // Assert
            Assert.IsInstanceOf<DistributionPropertiesBase<NormalDistribution>>(properties);
            Assert.IsNull(properties.Data);
            Assert.AreEqual("Normaal", properties.DistributionType);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Constructor_Always_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var mockRepository = new MockRepository();
            var observerableMock = mockRepository.StrictMock<IObservable>();
            mockRepository.ReplayAll();

            // Call
            var properties = new NormalDistributionProperties(DistributionPropertiesReadOnly.None, observerableMock, null);

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
                                                                            "De gemiddelde waarde van de normale verdeling.");

            PropertyDescriptor standardProperty = dynamicProperties[2];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(standardProperty,
                                                                            "Misc",
                                                                            "Standaardafwijking",
                                                                            "De standaardafwijking van de normale verdeling.");
            mockRepository.VerifyAll();
        }
    }
}