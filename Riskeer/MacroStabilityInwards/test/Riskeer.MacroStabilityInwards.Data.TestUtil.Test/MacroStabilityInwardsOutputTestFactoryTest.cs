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
using System.Linq;
using Core.Common.Base.Data;
using NUnit.Framework;

namespace Riskeer.MacroStabilityInwards.Data.TestUtil.Test
{
    [TestFixture]
    public class MacroStabilityInwardsOutputTestFactoryTest
    {
        [Test]
        public void CreateOutput_WithoutParameters_ReturnsOutputWithExpectedProperties()
        {
            // Call
            MacroStabilityInwardsOutput output = MacroStabilityInwardsOutputTestFactory.CreateOutput();

            // Assert
            Assert.IsNotNull(output);
            Assert.IsNotNull(output.SlidingCurve);
            CollectionAssert.IsNotEmpty(output.SlidingCurve.Slices);
            Assert.IsNotNull(output.SlipPlane);
        }

        [Test]
        public void CreateOutputWithoutSlices_ReturnsOutputWithExpectedProperties()
        {
            // Call
            MacroStabilityInwardsOutput output = MacroStabilityInwardsOutputTestFactory.CreateOutputWithoutSlices();

            // Assert
            Assert.IsNotNull(output);
            Assert.IsNotNull(output.SlidingCurve);
            Assert.IsNotNull(output.SlipPlane);
            CollectionAssert.IsEmpty(output.SlidingCurve.Slices);
        }

        [Test]
        public void CreateOutput_WithParameters_ReturnsOutputWithExpectedProperties()
        {
            // Setup
            var random = new Random(11);
            double factorOfStability = random.NextDouble();
            double zValue = random.NextDouble();
            double forbiddenZonesXEntryMax = random.NextDouble();
            double forbiddenZonesXEntryMin = random.NextDouble();

            var properties = new MacroStabilityInwardsOutput.ConstructionProperties
            {
                FactorOfStability = factorOfStability,
                ZValue = zValue,
                ForbiddenZonesXEntryMax = forbiddenZonesXEntryMax,
                ForbiddenZonesXEntryMin = forbiddenZonesXEntryMin
            };

            // Call
            MacroStabilityInwardsOutput output = MacroStabilityInwardsOutputTestFactory.CreateOutput(properties);

            // Assert
            Assert.IsNotNull(output);
            Assert.AreEqual(factorOfStability, output.FactorOfStability);
            Assert.AreEqual(zValue, output.ZValue);
            Assert.AreEqual(forbiddenZonesXEntryMax, output.ForbiddenZonesXEntryMax);
            Assert.AreEqual(forbiddenZonesXEntryMin, output.ForbiddenZonesXEntryMin);
            Assert.IsNotNull(output.SlidingCurve);
            Assert.AreEqual(3, output.SlidingCurve.Slices.Count());
            Assert.IsNotNull(output.SlipPlane);
            CollectionAssert.AreEqual(new[]
            {
                (RoundedDouble) (-3.5),
                (RoundedDouble) 0.0,
                (RoundedDouble) 2.0
            }, output.SlipPlane.TangentLines);
        }

        [Test]
        public void CreateRandomOutput_WithoutParameters_ReturnsOutputWithExpectedProperties()
        {
            // Call
            MacroStabilityInwardsOutput output = MacroStabilityInwardsOutputTestFactory.CreateRandomOutput();

            // Assert
            Assert.IsNotNull(output);
            Assert.IsTrue(IsValidDouble(output.FactorOfStability));
            Assert.IsTrue(IsValidDouble(output.ZValue));
            Assert.IsTrue(IsValidDouble(output.ForbiddenZonesXEntryMax));
            Assert.IsTrue(IsValidDouble(output.ForbiddenZonesXEntryMin));
            Assert.IsNotNull(output.SlidingCurve);
            Assert.IsNotNull(output.SlipPlane);
        }

        private static bool IsValidDouble(double value)
        {
            return !double.IsNaN(value) && !double.IsInfinity(value);
        }
    }
}