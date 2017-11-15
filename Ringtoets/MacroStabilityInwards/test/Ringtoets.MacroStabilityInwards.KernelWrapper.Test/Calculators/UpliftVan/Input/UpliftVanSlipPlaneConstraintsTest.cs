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
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.UpliftVan.Input;

namespace Ringtoets.MacroStabilityInwards.KernelWrapper.Test.Calculators.UpliftVan.Input
{
    [TestFixture]
    public class UpliftVanSlipPlaneConstraintsTest
    {
        [Test]
        public void ParameterlessConstructor_ExpectedValues()
        {
            // Call
            var slipPlaneConstraints = new UpliftVanSlipPlaneConstraints();

            // Assert
            Assert.IsFalse(slipPlaneConstraints.CreateZones);
            Assert.IsFalse(slipPlaneConstraints.AutomaticForbiddenZones);
            Assert.IsNaN(slipPlaneConstraints.SlipPlaneMinimumLength);
            Assert.IsNaN(slipPlaneConstraints.SlipPlaneMinimumDepth);
            Assert.IsNaN(slipPlaneConstraints.ZoneBoundaryLeft);
            Assert.IsNaN(slipPlaneConstraints.ZoneBoundaryRight);
        }

        [Test]
        public void Constructor_WithValues_ReturnsNewInstance()
        {
            // Setup
            var random = new Random(39);
            bool createZones = random.NextBoolean();
            bool automaticForbiddenZones = random.NextBoolean();
            double slipPlaneMinimumLength = random.NextDouble();
            double slipPlaneMinimumDepth = random.NextDouble();
            double zoneBoundaryLeft = random.NextDouble();
            double zoneBoundaryRight = random.NextDouble();

            // Call
            var constraints = new UpliftVanSlipPlaneConstraints(createZones,
                                                                automaticForbiddenZones,
                                                                zoneBoundaryLeft,
                                                                zoneBoundaryRight,
                                                                slipPlaneMinimumDepth,
                                                                slipPlaneMinimumLength);

            // Assert
            Assert.AreEqual(createZones, constraints.CreateZones);
            Assert.AreEqual(automaticForbiddenZones, constraints.AutomaticForbiddenZones);
            Assert.AreEqual(slipPlaneMinimumDepth, constraints.SlipPlaneMinimumDepth);
            Assert.AreEqual(slipPlaneMinimumLength, constraints.SlipPlaneMinimumLength);
            Assert.AreEqual(zoneBoundaryLeft, constraints.ZoneBoundaryLeft);
            Assert.AreEqual(zoneBoundaryRight, constraints.ZoneBoundaryRight);
        }
    }
}