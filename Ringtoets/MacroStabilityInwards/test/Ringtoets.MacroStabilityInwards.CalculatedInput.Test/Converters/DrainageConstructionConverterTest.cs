// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using NUnit.Framework;
using Ringtoets.MacroStabilityInwards.CalculatedInput.Converters;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.Input;
using Ringtoets.MacroStabilityInwards.Primitives;

namespace Ringtoets.MacroStabilityInwards.CalculatedInput.Test.Converters
{
    [TestFixture]
    public class DrainageConstructionConverterTest
    {
        [Test]
        public void Convert_InputNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => DrainageConstructionConverter.Convert(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("input", exception.ParamName);
        }

        [Test]
        [TestCase(MacroStabilityInwardsDikeSoilScenario.SandDikeOnSand)]
        [TestCase(MacroStabilityInwardsDikeSoilScenario.SandDikeOnClay)]
        public void Convert_SandDikeAndDrainageConstructionPresentTrue_ReturnUpliftVanDrainageConstruction(MacroStabilityInwardsDikeSoilScenario soilScenario)
        {
            // Setup
            var input = new MacroStabilityInwardsInput
            {
                DrainageConstructionPresent = true,
                XCoordinateDrainageConstruction = (RoundedDouble) 2,
                ZCoordinateDrainageConstruction = (RoundedDouble) 4,
                DikeSoilScenario = soilScenario
            };

            // Call
            DrainageConstruction drainageConstruction = DrainageConstructionConverter.Convert(input);

            // Assert
            Assert.IsTrue((bool) drainageConstruction.IsPresent);
            Assert.AreEqual(input.XCoordinateDrainageConstruction, drainageConstruction.XCoordinate);
            Assert.AreEqual(input.ZCoordinateDrainageConstruction, drainageConstruction.ZCoordinate);
        }

        [TestCase(MacroStabilityInwardsDikeSoilScenario.ClayDikeOnSand)]
        [TestCase(MacroStabilityInwardsDikeSoilScenario.ClayDikeOnClay)]
        [TestCase(MacroStabilityInwardsDikeSoilScenario.SandDikeOnSand)]
        [TestCase(MacroStabilityInwardsDikeSoilScenario.SandDikeOnClay)]
        public void Convert_DrainageConstructionPresentFalse_ReturnUpliftVanDrainageConstruction(MacroStabilityInwardsDikeSoilScenario soilScenario)
        {
            // Setup
            var input = new MacroStabilityInwardsInput
            {
                DrainageConstructionPresent = false,
                XCoordinateDrainageConstruction = (RoundedDouble) 2,
                ZCoordinateDrainageConstruction = (RoundedDouble) 4,
                DikeSoilScenario = soilScenario
            };

            // Call
            DrainageConstruction drainageConstruction = DrainageConstructionConverter.Convert(input);

            // Assert
            Assert.IsFalse((bool) drainageConstruction.IsPresent);
            Assert.IsNaN((double) drainageConstruction.XCoordinate);
            Assert.IsNaN((double) drainageConstruction.ZCoordinate);
        }

        [TestCase(MacroStabilityInwardsDikeSoilScenario.ClayDikeOnSand)]
        [TestCase(MacroStabilityInwardsDikeSoilScenario.ClayDikeOnClay)]
        public void Convert_ClayDikeAndDrainageConstructionPresentTrue_ReturnUpliftVanDrainageConstruction(MacroStabilityInwardsDikeSoilScenario soilScenario)
        {
            // Setup
            var input = new MacroStabilityInwardsInput
            {
                DrainageConstructionPresent = true,
                XCoordinateDrainageConstruction = (RoundedDouble) 2,
                ZCoordinateDrainageConstruction = (RoundedDouble) 4,
                DikeSoilScenario = soilScenario
            };

            // Call
            DrainageConstruction drainageConstruction = DrainageConstructionConverter.Convert(input);

            // Assert
            Assert.IsFalse((bool) drainageConstruction.IsPresent);
            Assert.IsNaN((double) drainageConstruction.XCoordinate);
            Assert.IsNaN((double) drainageConstruction.ZCoordinate);
        }
    }
}