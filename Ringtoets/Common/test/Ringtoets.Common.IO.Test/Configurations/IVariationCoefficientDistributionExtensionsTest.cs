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
using Core.Common.Base.Data;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.Common.IO.Configurations;

namespace Ringtoets.Common.IO.Test.Configurations
{
    [TestFixture]
    public class IVariationCoefficientDistributionExtensionsTest
    {
        [Test]
        public void ToStochastConfiguration_DistributionNull_ThrowsArgumentNullException()
        {
            // Setup
            IVariationCoefficientDistribution distribution = null;
            
            // Call
            TestDelegate test = () => distribution.ToStochastConfiguration();

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("distribution", exception.ParamName);
        }

        [Test]
        public void ToStochastConfiguration_WithDistribution_InstanceWithExpectedParametersSet()
        {
            // Setup
            var mocks = new MockRepository();
            var distribution = mocks.Stub<IVariationCoefficientDistribution>();
            mocks.ReplayAll();

            var random = new Random(21);
            RoundedDouble mean = random.NextRoundedDouble();
            RoundedDouble variationCoefficient = random.NextRoundedDouble();

            distribution.Mean = mean;
            distribution.CoefficientOfVariation = variationCoefficient;

            // Call
            MeanVariationCoefficientStochastConfiguration configuration = distribution.ToStochastConfiguration();

            // Assert
            Assert.AreEqual(mean, configuration.Mean);
            Assert.AreEqual(variationCoefficient, configuration.VariationCoefficient);
        }

        [Test]
        public void ToStochastConfigurationWithMean_DistributionNull_ThrowsArgumentNullException()
        {
            // Setup
            IVariationCoefficientDistribution distribution = null;
            
            // Call
            TestDelegate test = () => distribution.ToStochastConfigurationWithMean();

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("distribution", exception.ParamName);
        }

        [Test]
        public void ToStochastConfigurationWithMean_WithDistribution_InstanceWithExpectedParametersSet()
        {
            // Setup
            var mocks = new MockRepository();
            var distribution = mocks.Stub<IVariationCoefficientDistribution>();
            mocks.ReplayAll();

            var random = new Random(21);
            RoundedDouble mean = random.NextRoundedDouble();

            distribution.Mean = mean;

            // Call
            MeanVariationCoefficientStochastConfiguration configuration = distribution.ToStochastConfigurationWithMean();

            // Assert
            Assert.AreEqual(mean, configuration.Mean);
            Assert.IsNull(configuration.VariationCoefficient);
        }

        [Test]
        public void ToStochastConfigurationWithVariationCoefficient_DistributionNull_ThrowsArgumentNullException()
        {
            // Setup
            IVariationCoefficientDistribution distribution = null;
            
            // Call
            TestDelegate test = () => distribution.ToStochastConfigurationWithVariationCoefficient();

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("distribution", exception.ParamName);
        }

        [Test]
        public void ToStochastConfigurationWithVariationCoefficient_WithDistribution_InstanceWithExpectedParametersSet()
        {
            // Setup
            var mocks = new MockRepository();
            var distribution = mocks.Stub<IVariationCoefficientDistribution>();
            mocks.ReplayAll();

            var random = new Random(21);
            RoundedDouble variationCoefficient = random.NextRoundedDouble();

            distribution.CoefficientOfVariation = variationCoefficient;

            // Call
            MeanVariationCoefficientStochastConfiguration configuration = distribution.ToStochastConfigurationWithVariationCoefficient();

            // Assert
            Assert.IsNull(configuration.Mean);
            Assert.AreEqual(variationCoefficient, configuration.VariationCoefficient);
        }
    }
}