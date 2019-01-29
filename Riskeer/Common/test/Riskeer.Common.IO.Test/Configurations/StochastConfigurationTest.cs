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
using NUnit.Framework;
using Riskeer.Common.IO.Configurations;

namespace Riskeer.Common.IO.Test.Configurations
{
    [TestFixture]
    public class StochastConfigurationTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var configuration = new StochastConfiguration();

            // Assert
            Assert.IsNull(configuration.Mean);
            Assert.IsNull(configuration.StandardDeviation);
            Assert.IsNull(configuration.VariationCoefficient);
        }

        [Test]
        public void SimpleProperties_SetNewValue_GetsNewlySetValue()
        {
            // Setup
            var random = new Random(236789);
            var configuration = new StochastConfiguration();

            double mean = random.NextDouble();
            double standardDeviation = random.NextDouble();
            double variationCoefficient = random.NextDouble();

            // Call
            configuration.Mean = mean;
            configuration.StandardDeviation = standardDeviation;
            configuration.VariationCoefficient = variationCoefficient;

            // Assert 
            Assert.AreEqual(mean, configuration.Mean);
            Assert.AreEqual(standardDeviation, configuration.StandardDeviation);
            Assert.AreEqual(variationCoefficient, configuration.VariationCoefficient);
        }
    }
}