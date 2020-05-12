﻿// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using Deltares.MacroStability.WaternetCreator;
using NUnit.Framework;
using Riskeer.MacroStabilityInwards.KernelWrapper.Creators.Input;
using Riskeer.MacroStabilityInwards.Primitives;
using PlLineCreationMethod = Riskeer.MacroStabilityInwards.KernelWrapper.Calculators.Input.PlLineCreationMethod;
using WaternetCreationMode = Riskeer.MacroStabilityInwards.KernelWrapper.Calculators.Input.WaternetCreationMode;
using WtiStabilityPlLineCreationMethod = Deltares.MacroStability.WaternetCreator.PlLineCreationMethod;
using WtiStabilityWaternetCreationMethod = Deltares.MacroStability.WaternetCreator.WaternetCreationMode;

namespace Riskeer.MacroStabilityInwards.KernelWrapper.Test.Creators.Input
{
    [TestFixture]
    public class LocationCreatorHelperTest
    {
        [Test]
        public void ConvertDikeSoilScenario_InvalidDikeSoilScenario_ThrowInvalidEnumArgumentException()
        {
            // Call
            TestDelegate test = () => LocationCreatorHelper.ConvertDikeSoilScenario((MacroStabilityInwardsDikeSoilScenario) 99);

            // Assert
            string message = $"The value of argument 'dikeSoilScenario' ({99}) is invalid for Enum type '{typeof(MacroStabilityInwardsDikeSoilScenario).Name}'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(test, message);
        }

        [Test]
        [TestCase(MacroStabilityInwardsDikeSoilScenario.ClayDikeOnClay, DikeSoilScenario.ClayDikeOnClay)]
        [TestCase(MacroStabilityInwardsDikeSoilScenario.ClayDikeOnSand, DikeSoilScenario.ClayDikeOnSand)]
        [TestCase(MacroStabilityInwardsDikeSoilScenario.SandDikeOnClay, DikeSoilScenario.SandDikeOnClay)]
        [TestCase(MacroStabilityInwardsDikeSoilScenario.SandDikeOnSand, DikeSoilScenario.SandDikeOnSand)]
        public void ConvertDikeSoilScenario_ValidDikeSoilScenario_ReturnExpectedDikeSoilScenario(MacroStabilityInwardsDikeSoilScenario macroStabilityInwardsDikeSoilScenario,
                                                                                                 DikeSoilScenario expectedDikeSoilScenario)
        {
            // Call
            DikeSoilScenario convertedDikeSoilScenario = LocationCreatorHelper.ConvertDikeSoilScenario(macroStabilityInwardsDikeSoilScenario);

            // Assert
            Assert.AreEqual(expectedDikeSoilScenario, convertedDikeSoilScenario);
        }

        [Test]
        public void ConvertWaternetCreationMode_InvalidWaternetCreationMode_ThrowInvalidEnumArgumentException()
        {
            // Call
            TestDelegate test = () => LocationCreatorHelper.ConvertWaternetCreationMode((WaternetCreationMode) 99);

            // Assert
            string message = $"The value of argument 'waternetCreationMode' ({99}) is invalid for Enum type '{typeof(WaternetCreationMode).Name}'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(test, message);
        }

        [Test]
        [TestCase(WaternetCreationMode.CreateWaternet, WtiStabilityWaternetCreationMethod.CreateWaternet)]
        [TestCase(WaternetCreationMode.FillInWaternetValues, WtiStabilityWaternetCreationMethod.FillInWaternetValues)]
        public void ConvertWaternetCreationMode_ValidWaternetCreationMode_ReturnExpectedWtiStabilityWaternetCreationMode(WaternetCreationMode waternetCreationMode,
                                                                                                                         WtiStabilityWaternetCreationMethod expectedWaternetCreationMode)
        {
            // Call
            WtiStabilityWaternetCreationMethod convertedWaternetCreationMode = LocationCreatorHelper.ConvertWaternetCreationMode(waternetCreationMode);

            // Assert
            Assert.AreEqual(expectedWaternetCreationMode, convertedWaternetCreationMode);
        }

        [Test]
        public void ConvertPlLineCreationMethod_InvalidPlLineCreationMethod_ThrowInvalidEnumArgumentException()
        {
            // Call
            TestDelegate test = () => LocationCreatorHelper.ConvertPlLineCreationMethod((PlLineCreationMethod) 99);

            // Assert
            string message = $"The value of argument 'plLineCreationMethod' ({99}) is invalid for Enum type '{typeof(PlLineCreationMethod).Name}'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(test, message);
        }

        [Test]
        [TestCase(PlLineCreationMethod.RingtoetsWti2017, WtiStabilityPlLineCreationMethod.RingtoetsWti2017)]
        [TestCase(PlLineCreationMethod.None, WtiStabilityPlLineCreationMethod.None)]
        public void ConvertPlLineCreationMethod_ValidPlLineCreationMethod_ReturnExpectedWtiStabilityPlLineCreationMethod(PlLineCreationMethod plLineCreationMethod,
                                                                                                                         WtiStabilityPlLineCreationMethod expectedPlLineCreationMethod)
        {
            // Call
            WtiStabilityPlLineCreationMethod actualPlLineCreationMethod = LocationCreatorHelper.ConvertPlLineCreationMethod(plLineCreationMethod);

            // Assert
            Assert.AreEqual(expectedPlLineCreationMethod, actualPlLineCreationMethod);
        }
    }
}