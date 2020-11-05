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

using System.ComponentModel;
using Core.Common.TestUtil;
using Deltares.MacroStability.CSharpWrapper.Input;
using NUnit.Framework;
using Riskeer.MacroStabilityInwards.KernelWrapper.Creators.Input;
using Riskeer.MacroStabilityInwards.Primitives;

namespace Riskeer.MacroStabilityInwards.KernelWrapper.Test.Creators.Input
{
    [TestFixture]
    public class WaternetCreatorInputHelperTest
    {
        [Test]
        public void ConvertDikeSoilScenario_InvalidDikeSoilScenario_ThrowInvalidEnumArgumentException()
        {
            // Setup
            const MacroStabilityInwardsDikeSoilScenario dikeSoilScenario = (MacroStabilityInwardsDikeSoilScenario) 99;

            // Call
            void Call() => WaternetCreatorInputHelper.ConvertDikeSoilScenario(dikeSoilScenario);

            // Assert
            string message = $"The value of argument 'dikeSoilScenario' ({dikeSoilScenario}) is invalid for Enum type '{nameof(MacroStabilityInwardsDikeSoilScenario)}'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(Call, message);
        }

        [Test]
        [TestCase(MacroStabilityInwardsDikeSoilScenario.ClayDikeOnClay, DikeSoilScenario.ClayDikeOnClay)]
        [TestCase(MacroStabilityInwardsDikeSoilScenario.ClayDikeOnSand, DikeSoilScenario.ClayDikeOnSand)]
        [TestCase(MacroStabilityInwardsDikeSoilScenario.SandDikeOnClay, DikeSoilScenario.SandDikeOnClay)]
        [TestCase(MacroStabilityInwardsDikeSoilScenario.SandDikeOnSand, DikeSoilScenario.SandDikeOnSand)]
        public void ConvertDikeSoilScenario_ValidDikeSoilScenario_ReturnExpectedDikeSoilScenario(
            MacroStabilityInwardsDikeSoilScenario macroStabilityInwardsDikeSoilScenario,
            DikeSoilScenario expectedDikeSoilScenario)
        {
            // Call
            DikeSoilScenario convertedDikeSoilScenario = WaternetCreatorInputHelper.ConvertDikeSoilScenario(macroStabilityInwardsDikeSoilScenario);

            // Assert
            Assert.AreEqual(expectedDikeSoilScenario, convertedDikeSoilScenario);
        }
    }
}