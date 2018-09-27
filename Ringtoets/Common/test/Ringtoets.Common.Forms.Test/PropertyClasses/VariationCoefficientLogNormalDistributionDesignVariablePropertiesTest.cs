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
using Core.Common.Base.Data;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.Common.Forms.PropertyClasses;

namespace Ringtoets.Common.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class VariationCoefficientLogNormalDistributionDesignVariablePropertiesTest
    {
        [Test]
        public void SingleParameterConstructor_DesignVariableNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => new VariationCoefficientLogNormalDistributionDesignVariableProperties(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("designVariable", exception.ParamName);
        }

        [Test]
        public void SingleParameterConstructor_ExpectedValues()
        {
            // Setup
            var distribution = new VariationCoefficientLogNormalDistribution();
            var designVariable = new VariationCoefficientLogNormalDistributionDesignVariable(distribution);

            // Call
            var properties = new VariationCoefficientLogNormalDistributionDesignVariableProperties(designVariable);

            // Assert
            Assert.IsInstanceOf<VariationCoefficientDesignVariableProperties<VariationCoefficientLogNormalDistribution>>(properties);
            Assert.AreSame(distribution, properties.Data);
            Assert.AreEqual(distribution.Mean, properties.Mean);
            Assert.AreEqual(distribution.CoefficientOfVariation, properties.CoefficientOfVariation);
            Assert.AreEqual("Lognormaal", properties.DistributionType);
        }

        [Test]
        public void Constructor_DesignVariableNull_ThrowArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var handler = mocks.Stub<IObservablePropertyChangeHandler>();
            mocks.ReplayAll();

            // Call
            TestDelegate test = () => new VariationCoefficientLogNormalDistributionDesignVariableProperties(VariationCoefficientDistributionPropertiesReadOnly.None,
                                                                                                            null,
                                                                                                            handler);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("designVariable", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var mockRepository = new MockRepository();
            var handler = mockRepository.Stub<IObservablePropertyChangeHandler>();
            mockRepository.ReplayAll();

            var distribution = new VariationCoefficientLogNormalDistribution();
            var designVariable = new VariationCoefficientLogNormalDistributionDesignVariable(distribution);

            // Call
            var properties = new VariationCoefficientLogNormalDistributionDesignVariableProperties(VariationCoefficientDistributionPropertiesReadOnly.All,
                                                                                                   designVariable,
                                                                                                   handler);

            // Assert
            Assert.IsInstanceOf<VariationCoefficientDesignVariableProperties<VariationCoefficientLogNormalDistribution>>(properties);
            Assert.AreSame(distribution, properties.Data);
            Assert.AreEqual(distribution.Mean, properties.Mean);
            Assert.AreEqual(distribution.CoefficientOfVariation, properties.CoefficientOfVariation);
            Assert.AreEqual("Lognormaal", properties.DistributionType);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Constructor_Always_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var mockRepository = new MockRepository();
            var handler = mockRepository.Stub<IObservablePropertyChangeHandler>();
            mockRepository.ReplayAll();

            var distribution = new VariationCoefficientLogNormalDistribution();
            var designVariable = new VariationCoefficientLogNormalDistributionDesignVariable(distribution);

            // Call
            var properties = new VariationCoefficientLogNormalDistributionDesignVariableProperties(VariationCoefficientDistributionPropertiesReadOnly.None,
                                                                                                   designVariable,
                                                                                                   handler);

            // Assert
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
                                                                            "De gemiddelde waarde van de lognormale verdeling.");

            PropertyDescriptor standardDeviationProperty = dynamicProperties[2];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(standardDeviationProperty,
                                                                            "Misc",
                                                                            "Variatiecoëfficiënt",
                                                                            "De variatiecoëfficiënt van de lognormale verdeling.");

            PropertyDescriptor designValueProperty = dynamicProperties[3];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(designValueProperty,
                                                                            "Misc",
                                                                            "Rekenwaarde",
                                                                            "De representatieve waarde die gebruikt wordt door de berekening.",
                                                                            true);
            mockRepository.VerifyAll();
        }

        [Test]
        public void GetProperties_WithData_ReturnExpectedValues()
        {
            // Setup
            var mockRepository = new MockRepository();
            var handler = mockRepository.Stub<IObservablePropertyChangeHandler>();
            mockRepository.ReplayAll();

            var distribution = new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = new RoundedDouble(2, 1),
                CoefficientOfVariation = new RoundedDouble(2, 2)
            };
            var designVariable = new VariationCoefficientLogNormalDistributionDesignVariable(distribution);

            // Call
            var properties = new VariationCoefficientLogNormalDistributionDesignVariableProperties(VariationCoefficientDistributionPropertiesReadOnly.None,
                                                                                                   designVariable,
                                                                                                   handler);

            // Assert
            Assert.AreEqual("Lognormaal", properties.DistributionType);
            Assert.AreEqual(distribution.Mean, properties.Mean);
            Assert.AreEqual(distribution.CoefficientOfVariation, properties.CoefficientOfVariation);
            Assert.AreEqual(designVariable.GetDesignValue(), properties.DesignValue);
        }
    }
}