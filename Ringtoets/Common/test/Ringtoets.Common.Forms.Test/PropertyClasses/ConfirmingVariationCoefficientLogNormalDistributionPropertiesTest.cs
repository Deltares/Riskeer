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

using System.ComponentModel;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.Common.Forms.PropertyClasses;

namespace Ringtoets.Common.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class ConfirmingVariationCoefficientLogNormalDistributionPropertiesTest
    {
        private MockRepository mockRepository;

        [SetUp]
        public void SetUp()
        {
            mockRepository = new MockRepository();
        }

        [Test]
        public void Constructor_WithParameters_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var calculation = mocks.Stub<ICalculation>();
            var input = mocks.Stub<ICalculationInput>();
            var handler = mocks.Stub<ICalculationInputPropertyChangeHandler>();
            mockRepository.ReplayAll();

            var distribution = new VariationCoefficientLogNormalDistribution();

            // Call
            var properties = new ConfirmingVariationCoefficientLogNormalDistributionProperties<ICalculationInput>(
                VariationCoefficientDistributionPropertiesReadOnly.None, distribution, calculation, input, handler);

            // Assert
            Assert.IsInstanceOf<ConfirmingVariationCoefficientDistributionPropertiesBase<VariationCoefficientLogNormalDistribution, ICalculationInput>>(properties);
            Assert.AreSame(distribution, properties.Data);
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
                                                                            "De gemiddelde waarde van de lognormale verdeling.");

            PropertyDescriptor standardDeviationProperty = dynamicProperties[2];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(standardDeviationProperty,
                                                                            "Misc",
                                                                            "Variatiecoëfficiënt",
                                                                            "De variatiecoëfficiënt van de lognormale verdeling.");

            mockRepository.VerifyAll();
        }
    }
}