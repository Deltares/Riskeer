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
using Riskeer.MacroStabilityInwards.CalculatedInput.Converters;
using Riskeer.MacroStabilityInwards.Data;
using Riskeer.MacroStabilityInwards.KernelWrapper.Calculators.Input;
using Riskeer.MacroStabilityInwards.Primitives;

namespace Riskeer.MacroStabilityInwards.CalculatedInput.Test.Converters
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
        public void Convert_SandDikeAndDrainageConstructionPresentTrue_ReturnDrainageConstruction(MacroStabilityInwardsDikeSoilScenario soilScenario)
        {
            // Setup
            var random = new Random(21);
            var input = new MacroStabilityInwardsInput(new MacroStabilityInwardsInput.ConstructionProperties())
            {
                DrainageConstructionPresent = true,
                XCoordinateDrainageConstruction = random.NextRoundedDouble(),
                ZCoordinateDrainageConstruction = random.NextRoundedDouble(),
                DikeSoilScenario = soilScenario
            };

            // Call
            DrainageConstruction drainageConstruction = DrainageConstructionConverter.Convert(input);

            // Assert
            Assert.IsTrue(drainageConstruction.IsPresent);
            Assert.AreEqual(input.XCoordinateDrainageConstruction, drainageConstruction.XCoordinate);
            Assert.AreEqual(input.ZCoordinateDrainageConstruction, drainageConstruction.ZCoordinate);
        }

        [Test]
        [TestCase(MacroStabilityInwardsDikeSoilScenario.ClayDikeOnSand)]
        [TestCase(MacroStabilityInwardsDikeSoilScenario.ClayDikeOnClay)]
        [TestCase(MacroStabilityInwardsDikeSoilScenario.SandDikeOnSand)]
        [TestCase(MacroStabilityInwardsDikeSoilScenario.SandDikeOnClay)]
        public void Convert_DrainageConstructionPresentFalse_ReturnDrainageConstruction(MacroStabilityInwardsDikeSoilScenario soilScenario)
        {
            // Setup
            var random = new Random(21);
            var input = new MacroStabilityInwardsInput(new MacroStabilityInwardsInput.ConstructionProperties())
            {
                DrainageConstructionPresent = false,
                XCoordinateDrainageConstruction = random.NextRoundedDouble(),
                ZCoordinateDrainageConstruction = random.NextRoundedDouble(),
                DikeSoilScenario = soilScenario
            };

            // Call
            DrainageConstruction drainageConstruction = DrainageConstructionConverter.Convert(input);

            // Assert
            Assert.IsFalse(drainageConstruction.IsPresent);
            Assert.IsNaN(drainageConstruction.XCoordinate);
            Assert.IsNaN(drainageConstruction.ZCoordinate);
        }

        [Test]
        [TestCase(MacroStabilityInwardsDikeSoilScenario.ClayDikeOnSand)]
        [TestCase(MacroStabilityInwardsDikeSoilScenario.ClayDikeOnClay)]
        public void Convert_ClayDikeAndDrainageConstructionPresentTrue_ReturnDrainageConstruction(MacroStabilityInwardsDikeSoilScenario soilScenario)
        {
            // Setup
            var random = new Random(21);
            var input = new MacroStabilityInwardsInput(new MacroStabilityInwardsInput.ConstructionProperties())
            {
                DrainageConstructionPresent = true,
                XCoordinateDrainageConstruction = random.NextRoundedDouble(),
                ZCoordinateDrainageConstruction = random.NextRoundedDouble(),
                DikeSoilScenario = soilScenario
            };

            // Call
            DrainageConstruction drainageConstruction = DrainageConstructionConverter.Convert(input);

            // Assert
            Assert.IsFalse(drainageConstruction.IsPresent);
            Assert.IsNaN(drainageConstruction.XCoordinate);
            Assert.IsNaN(drainageConstruction.ZCoordinate);
        }
    }
}