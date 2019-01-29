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
using Deltares.WTIStability;
using NUnit.Framework;
using Riskeer.MacroStabilityInwards.KernelWrapper.Calculators.UpliftVan.Input;
using Riskeer.MacroStabilityInwards.KernelWrapper.Creators.Input;

namespace Riskeer.MacroStabilityInwards.KernelWrapper.Test.Creators.Input
{
    public class SlipPlaneConstraintsCreatorTest
    {
        [Test]
        public void Create_UpliftVanSlipPlaneConstraintsNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => SlipPlaneConstraintsCreator.Create(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("input", paramName);
        }

        [Test]
        public void Create_WithUpliftVanSlipPlaneConstraints_ReturnsExpectedSlipPlaneConstraints()
        {
            // Setup
            var random = new Random(39);
            var upliftVanSlipPlaneConstraints = new UpliftVanSlipPlaneConstraints(random.NextDouble(), random.NextDouble(),
                                                                                  random.NextDouble(), random.NextDouble());

            // Call
            SlipPlaneConstraints constraints = SlipPlaneConstraintsCreator.Create(upliftVanSlipPlaneConstraints);

            // Assert
            Assert.AreEqual(upliftVanSlipPlaneConstraints.CreateZones, constraints.CreateZones);
            Assert.AreEqual(upliftVanSlipPlaneConstraints.AutomaticForbiddenZones, constraints.AutomaticForbiddenZones);
            Assert.AreEqual(upliftVanSlipPlaneConstraints.SlipPlaneMinimumLength, constraints.SlipPlaneMinLength);
            Assert.AreEqual(upliftVanSlipPlaneConstraints.SlipPlaneMinimumDepth, constraints.SlipPlaneMinDepth);
            Assert.AreEqual(upliftVanSlipPlaneConstraints.ZoneBoundaryLeft, constraints.XEntryMin);
            Assert.AreEqual(upliftVanSlipPlaneConstraints.ZoneBoundaryRight, constraints.XEntryMax);
        }
    }
}