﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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

using System.ComponentModel;
using Core.Gui.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.Probabilistics;
using Riskeer.Common.Forms.PropertyClasses;

namespace Riskeer.Common.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class VariationCoefficientLogNormalDistributionPropertiesTest
    {
        [Test]
        public void Constructor_WithData_ReadOnlyProperties()
        {
            // Setup
            var distribution = new VariationCoefficientLogNormalDistribution();

            // Call
            var properties = new VariationCoefficientLogNormalDistributionProperties(distribution);

            // Assert
            Assert.IsInstanceOf<VariationCoefficientDistributionPropertiesBase<VariationCoefficientLogNormalDistribution>>(properties);
            Assert.AreSame(distribution, properties.Data);

            AssertPropertiesInState(properties, true, true);
        }

        [Test]
        public void Constructor_WithParameters_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var handler = mocks.Stub<IObservablePropertyChangeHandler>();
            mocks.ReplayAll();

            var distribution = new VariationCoefficientLogNormalDistribution();

            // Call
            var properties = new VariationCoefficientLogNormalDistributionProperties(
                VariationCoefficientDistributionReadOnlyProperties.None, distribution, handler);

            // Assert
            Assert.IsInstanceOf<VariationCoefficientDistributionPropertiesBase<VariationCoefficientLogNormalDistribution>>(properties);
            Assert.AreSame(distribution, properties.Data);

            AssertPropertiesInState(properties, false, false);

            mocks.VerifyAll();
        }

        private static void AssertPropertiesInState(VariationCoefficientLogNormalDistributionProperties properties, bool meanReadOnly, bool variationCoefficientReadOnly)
        {
            Assert.IsInstanceOf<VariationCoefficientDistributionPropertiesBase<VariationCoefficientLogNormalDistribution>>(properties);
            Assert.AreEqual("Lognormaal", properties.DistributionType);

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

            PropertyDescriptor variationCoefficientProperty = dynamicProperties[2];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(variationCoefficientProperty,
                                                                            "Misc",
                                                                            "Variatiecoëfficiënt",
                                                                            "De variatiecoëfficiënt van de lognormale verdeling.",
                                                                            variationCoefficientReadOnly);
        }
    }
}