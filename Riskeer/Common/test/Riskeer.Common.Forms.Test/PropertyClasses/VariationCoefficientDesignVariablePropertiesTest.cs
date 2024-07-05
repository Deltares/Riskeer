﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
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
using Core.Common.Base.Data;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.Probabilistics;
using Riskeer.Common.Forms.PropertyClasses;

namespace Riskeer.Common.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class VariationCoefficientDesignVariablePropertiesTest
    {
        [Test]
        public void Constructor_DesignVariableNull_ThrowArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var handler = mocks.Stub<IObservablePropertyChangeHandler>();
            mocks.ReplayAll();

            // Call
            TestDelegate test = () => new SimpleDesignVariableProperties(VariationCoefficientDistributionReadOnlyProperties.None,
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
            var distribution = mockRepository.Stub<IVariationCoefficientDistribution>();
            mockRepository.ReplayAll();

            var designVariable = new SimpleVariationCoefficientDesignVariableProperties(distribution, RoundedDouble.NaN);

            // Call
            var properties = new SimpleDesignVariableProperties(VariationCoefficientDistributionReadOnlyProperties.All,
                                                                designVariable,
                                                                handler);

            // Assert
            Assert.IsInstanceOf<VariationCoefficientDistributionPropertiesBase<IVariationCoefficientDistribution>>(properties);
            Assert.AreEqual(designVariable.GetDesignValue(), properties.DesignValue);
            mockRepository.VerifyAll();
        }

        [Test]
        [SetCulture("nl-NL")]
        public void ToString_Always_ReturnDistributionName()
        {
            // Setup
            var mockRepository = new MockRepository();
            var handler = mockRepository.Stub<IObservablePropertyChangeHandler>();
            var distribution = mockRepository.Stub<IVariationCoefficientDistribution>();
            mockRepository.ReplayAll();

            const int numberOfDecimalPlaces = 2;
            distribution.Mean = new RoundedDouble(numberOfDecimalPlaces, 1);
            distribution.CoefficientOfVariation = new RoundedDouble(numberOfDecimalPlaces, 2);

            var designVariable = new SimpleVariationCoefficientDesignVariableProperties(distribution, new RoundedDouble(numberOfDecimalPlaces, 0.45));

            // Call
            var properties = new SimpleDesignVariableProperties(VariationCoefficientDistributionReadOnlyProperties.None,
                                                                designVariable,
                                                                handler);

            // Call
            string propertyName = properties.ToString();

            // Assert
            Assert.AreEqual("0,45 (Verwachtingswaarde = 1,00, Variatiecoëfficiënt = 2,00)", propertyName);
        }

        private class SimpleDesignVariableProperties : VariationCoefficientDesignVariableProperties<IVariationCoefficientDistribution>
        {
            public SimpleDesignVariableProperties(VariationCoefficientDistributionReadOnlyProperties readOnlyProperties,
                                                  VariationCoefficientDesignVariable<IVariationCoefficientDistribution> designVariable,
                                                  IObservablePropertyChangeHandler handler)
                : base(readOnlyProperties, designVariable, handler) {}

            public override string DistributionType { get; }
        }

        private class SimpleVariationCoefficientDesignVariableProperties : VariationCoefficientDesignVariable<IVariationCoefficientDistribution>
        {
            private readonly RoundedDouble designValue;

            public SimpleVariationCoefficientDesignVariableProperties(
                IVariationCoefficientDistribution variationCoefficientLogNormalDistribution,
                RoundedDouble designValue)
                : base(variationCoefficientLogNormalDistribution)
            {
                this.designValue = designValue;
            }

            public override RoundedDouble GetDesignValue()
            {
                return designValue;
            }
        }
    }
}