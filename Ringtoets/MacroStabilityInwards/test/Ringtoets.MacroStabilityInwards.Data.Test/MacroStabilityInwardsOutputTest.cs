﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Core.Common.Base;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.Calculation;

namespace Ringtoets.MacroStabilityInwards.Data.Test
{
    [TestFixture]
    public class MacroStabilityInwardsOutputTest
    {
        [Test]
        public void Constructor_ConstructionPropertiesNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new MacroStabilityInwardsOutput(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("properties", exception.ParamName);
        }

        [Test]
        public void Constructor_ConstructionPropertiesWithoutValuesSet_PropertiesAreDefault()
        {
            // Call
            var output = new MacroStabilityInwardsOutput(new MacroStabilityInwardsOutput.ConstructionProperties());

            // Assert
            Assert.IsNaN(output.FactorOfStability);
            Assert.IsNaN(output.ZValue);
            Assert.IsNaN(output.ForbiddenZonesXEntryMin);
            Assert.IsNaN(output.ForbiddenZonesXEntryMax);
            Assert.IsFalse(output.ForbiddenZonesAutomaticallyCalculated);
            Assert.IsFalse(output.GridAutomaticallyCalculated);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var random = new Random(21);
            double factorOfStability = random.NextDouble();
            double zValue = random.NextDouble();
            double xEntryMin = random.NextDouble();
            double xEntryMax = random.NextDouble();
            bool forbiddenZonesAutomaticallyCalculated = random.NextBoolean();
            bool gridAutomaticallyCalculated = random.NextBoolean();

            var properties = new MacroStabilityInwardsOutput.ConstructionProperties
            {
                FactorOfStability = factorOfStability,
                ZValue = zValue,
                ForbiddenZonesXEntryMin = xEntryMin,
                ForbiddenZonesXEntryMax = xEntryMax,
                ForbiddenZonesAutomaticallyCalculated = forbiddenZonesAutomaticallyCalculated,
                GridAutomaticallyCalculated = gridAutomaticallyCalculated
            };

            // Call
            var output = new MacroStabilityInwardsOutput(properties);

            // Assert
            Assert.IsInstanceOf<Observable>(output);
            Assert.IsInstanceOf<ICalculationOutput>(output);

            Assert.AreEqual(factorOfStability, output.FactorOfStability);
            Assert.AreEqual(zValue, output.ZValue);
            Assert.AreEqual(xEntryMin, output.ForbiddenZonesXEntryMin);
            Assert.AreEqual(xEntryMax, output.ForbiddenZonesXEntryMax);
            Assert.AreEqual(forbiddenZonesAutomaticallyCalculated, output.ForbiddenZonesAutomaticallyCalculated);
            Assert.AreEqual(gridAutomaticallyCalculated, output.GridAutomaticallyCalculated);
        }
    }
}