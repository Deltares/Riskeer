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
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.UpliftVan.Input;
using Ringtoets.MacroStabilityInwards.Service.Converters;

namespace Ringtoets.MacroStabilityInwards.Service.Test.Converters
{
    [TestFixture]
    public class UpliftVanSlipPlaneConstraintsConverterTest
    {
        [Test]
        public void Convert_InputNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => UpliftVanSlipPlaneConstraintsConverter.Convert(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("input", exception.ParamName);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Convert_ZoningDeterminationTypeAutomatic_ReturnsExpectedUpliftVanSlipPlaneConstraints(bool createZones)
        {
            // Setup
            var random = new Random(39);
            var input = new MacroStabilityInwardsInput(new MacroStabilityInwardsInput.ConstructionProperties())
            {
                CreateZones = createZones,
                ZoningBoundariesDeterminationType = MacroStabilityInwardsZoningBoundariesDeterminationType.Automatic,
                SlipPlaneMinimumDepth = random.NextRoundedDouble(),
                SlipPlaneMinimumLength = random.NextRoundedDouble(),
                ZoneBoundaryLeft = random.NextRoundedDouble(),
                ZoneBoundaryRight = random.NextRoundedDouble()
            };

            // Call
            UpliftVanSlipPlaneConstraints constraints = UpliftVanSlipPlaneConstraintsConverter.Convert(input);

            // Assert
            Assert.AreEqual(createZones, constraints.CreateZones);
            Assert.IsTrue(constraints.AutomaticForbiddenZones);
            Assert.AreEqual(input.SlipPlaneMinimumDepth, constraints.SlipPlaneMinimumDepth, input.SlipPlaneMinimumDepth.GetAccuracy());
            Assert.AreEqual(input.SlipPlaneMinimumLength, constraints.SlipPlaneMinimumLength, input.SlipPlaneMinimumLength.GetAccuracy());
            Assert.IsNaN(constraints.ZoneBoundaryLeft);
            Assert.IsNaN(constraints.ZoneBoundaryRight);
        }

        [Test]
        public void Convert_CreateZonesTrueAndZoningDeterminationTypeManual_ReturnsExpectedUpliftVanSlipPlaneConstraints()
        {
            // Setup
            var random = new Random(39);
            var input = new MacroStabilityInwardsInput(new MacroStabilityInwardsInput.ConstructionProperties())
            {
                CreateZones = true,
                ZoningBoundariesDeterminationType = MacroStabilityInwardsZoningBoundariesDeterminationType.Manual,
                SlipPlaneMinimumDepth = random.NextRoundedDouble(),
                SlipPlaneMinimumLength = random.NextRoundedDouble(),
                ZoneBoundaryLeft = random.NextRoundedDouble(),
                ZoneBoundaryRight = random.NextRoundedDouble()
            };

            // Call
            UpliftVanSlipPlaneConstraints constraints = UpliftVanSlipPlaneConstraintsConverter.Convert(input);

            // Assert
            Assert.IsTrue(constraints.CreateZones);
            Assert.IsFalse(constraints.AutomaticForbiddenZones);
            Assert.AreEqual(input.SlipPlaneMinimumDepth, constraints.SlipPlaneMinimumDepth, input.SlipPlaneMinimumDepth.GetAccuracy());
            Assert.AreEqual(input.SlipPlaneMinimumLength, constraints.SlipPlaneMinimumLength, input.SlipPlaneMinimumLength.GetAccuracy());
            Assert.AreEqual(input.ZoneBoundaryLeft, constraints.ZoneBoundaryLeft, input.ZoneBoundaryLeft.GetAccuracy());
            Assert.AreEqual(input.ZoneBoundaryRight, constraints.ZoneBoundaryRight, input.ZoneBoundaryRight.GetAccuracy());
        }
    }
}