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

using System.ComponentModel;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.Common.Forms.PropertyClasses;

namespace Ringtoets.Common.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class LogNormalDistributionPropertiesTest
    {
        [Test]
        public void Constructor_WithDistribution_ExpectedValues()
        {
            // Setup
            var distribution = new LogNormalDistribution();

            // Call
            var properties = new LogNormalDistributionProperties(distribution);

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
            var properties = new LogNormalDistributionProperties(
                DistributionPropertiesReadOnly.None, distribution, handler);

            // Assert
            Assert.IsInstanceOf<DistributionPropertiesBase<LogNormalDistribution>>(properties);
            Assert.AreSame(distribution, properties.Data);
            Assert.AreEqual("Lognormaal", properties.DistributionType);

            AssertPropertiesInState(properties, false, false);

            mocks.VerifyAll();
        }

        private static void AssertPropertiesInState(LogNormalDistributionProperties properties, bool meanReadOnly, bool deviationReadOnly)
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
                                                                            "De gemiddelde waarde van de lognormale verdeling.",
                                                                            meanReadOnly);

            PropertyDescriptor standardDeviationProperty = dynamicProperties[2];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(standardDeviationProperty,
                                                                            "Misc",
                                                                            "Standaardafwijking",
                                                                            "De standaardafwijking van de lognormale verdeling.",
                                                                            deviationReadOnly);
        }
    }
}