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
using Deltares.MacroStability.CSharpWrapper.Input;
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
            void Call() => SlipPlaneConstraintsCreator.Create(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("input", exception.ParamName);
        }

        [Test]
        public void Create_WithUpliftVanSlipPlaneConstraints_ReturnsExpectedSlipPlaneConstraints()
        {
            // Setup
            var random = new Random(39);
            var upliftVanSlipPlaneConstraints = new UpliftVanSlipPlaneConstraints(random.NextDouble(), random.NextDouble(),
                                                                                  random.NextDouble(), random.NextDouble());

            // Call
            SlipPlaneConstraints slipPlaneConstraints = SlipPlaneConstraintsCreator.Create(upliftVanSlipPlaneConstraints);

            // Assert
            Assert.AreEqual(upliftVanSlipPlaneConstraints.SlipPlaneMinimumDepth, slipPlaneConstraints.SlipPlaneMinDepth);
            Assert.AreEqual(upliftVanSlipPlaneConstraints.SlipPlaneMinimumLength, slipPlaneConstraints.SlipPlaneMinLength);
            Assert.AreEqual(upliftVanSlipPlaneConstraints.ZoneBoundaryLeft, slipPlaneConstraints.XEntryMin);
            Assert.AreEqual(upliftVanSlipPlaneConstraints.ZoneBoundaryRight, slipPlaneConstraints.XEntryMax);
            Assert.IsNaN(slipPlaneConstraints.XExitMin); // Irrelevant
            Assert.IsNaN(slipPlaneConstraints.XExitMax); // Irrelevant
        }
    }
}