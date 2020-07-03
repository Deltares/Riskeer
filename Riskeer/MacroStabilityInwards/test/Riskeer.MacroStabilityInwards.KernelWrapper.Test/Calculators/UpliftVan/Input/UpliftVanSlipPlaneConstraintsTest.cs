// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using NUnit.Framework;
using Riskeer.MacroStabilityInwards.KernelWrapper.Calculators.UpliftVan.Input;

namespace Riskeer.MacroStabilityInwards.KernelWrapper.Test.Calculators.UpliftVan.Input
{
    [TestFixture]
    public class UpliftVanSlipPlaneConstraintsTest
    {
        [Test]
        public void Constructor_WithMinimumLengthDepthAndCreateZones_ExpectedValues()
        {
            // Setup
            var random = new Random(39);
            double slipPlaneMinimumLength = random.NextDouble();
            double slipPlaneMinimumDepth = random.NextDouble();

            // Call
            var slipPlaneConstraints = new UpliftVanSlipPlaneConstraints(slipPlaneMinimumDepth, slipPlaneMinimumLength);

            // Assert
            Assert.IsTrue(slipPlaneConstraints.AutomaticForbiddenZones);
            Assert.AreEqual(slipPlaneMinimumLength, slipPlaneConstraints.SlipPlaneMinimumLength);
            Assert.AreEqual(slipPlaneMinimumDepth, slipPlaneConstraints.SlipPlaneMinimumDepth);
            Assert.IsNaN(slipPlaneConstraints.ZoneBoundaryLeft);
            Assert.IsNaN(slipPlaneConstraints.ZoneBoundaryRight);
        }

        [Test]
        public void Constructor_WithZoneBoundaries_ExpectedValues()
        {
            // Setup
            var random = new Random(39);
            double slipPlaneMinimumLength = random.NextDouble();
            double slipPlaneMinimumDepth = random.NextDouble();
            double zoneBoundaryLeft = random.NextDouble();
            double zoneBoundaryRight = random.NextDouble();

            // Call
            var slipPlaneConstraints = new UpliftVanSlipPlaneConstraints(slipPlaneMinimumDepth, slipPlaneMinimumLength,
                                                                         zoneBoundaryLeft, zoneBoundaryRight);

            // Assert
            Assert.IsFalse(slipPlaneConstraints.AutomaticForbiddenZones);
            Assert.AreEqual(zoneBoundaryLeft, slipPlaneConstraints.ZoneBoundaryLeft);
            Assert.AreEqual(zoneBoundaryRight, slipPlaneConstraints.ZoneBoundaryRight);
            Assert.AreEqual(slipPlaneMinimumDepth, slipPlaneConstraints.SlipPlaneMinimumDepth);
            Assert.AreEqual(slipPlaneMinimumLength, slipPlaneConstraints.SlipPlaneMinimumLength);
        }
    }
}