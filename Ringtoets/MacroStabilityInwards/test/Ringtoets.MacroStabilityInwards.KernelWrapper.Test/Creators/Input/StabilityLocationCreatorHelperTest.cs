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

using System.ComponentModel;
using Core.Common.TestUtil;
using Deltares.WaternetCreator;
using NUnit.Framework;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Creators.Input;
using Ringtoets.MacroStabilityInwards.Primitives;
using PlLineCreationMethod = Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.Input.PlLineCreationMethod;
using WaternetCreationMode = Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.Input.WaternetCreationMode;
using WTIStabilityPlLineCreationMethod = Deltares.WaternetCreator.PlLineCreationMethod;
using WTIStabilityWaternetCreationMethod = Deltares.WaternetCreator.WaternetCreationMode;

namespace Ringtoets.MacroStabilityInwards.KernelWrapper.Test.Creators.Input
{
    [TestFixture]
    public class StabilityLocationCreatorHelperTest
    {
        [Test]
        public void ConvertDikeSoilScenario_InvalidDikeSoilScenario_ThrowInvalidEnumArgumentException()
        {
            // Call
            TestDelegate test = () => StabilityLocationCreatorHelper.ConvertDikeSoilScenario((MacroStabilityInwardsDikeSoilScenario) 99);

            // Assert
            string message = $"The value of argument 'dikeSoilScenario' ({99}) is invalid for Enum type '{typeof(MacroStabilityInwardsDikeSoilScenario).Name}'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(test, message);
        }

        [Test]
        [TestCase(MacroStabilityInwardsDikeSoilScenario.ClayDikeOnClay, DikeSoilScenario.ClayDikeOnClay)]
        [TestCase(MacroStabilityInwardsDikeSoilScenario.ClayDikeOnSand, DikeSoilScenario.ClayDikeOnSand)]
        [TestCase(MacroStabilityInwardsDikeSoilScenario.SandDikeOnClay, DikeSoilScenario.SandDikeOnClay)]
        [TestCase(MacroStabilityInwardsDikeSoilScenario.SandDikeOnSand, DikeSoilScenario.SandDikeOnSand)]
        public void ConvertDikeSoilScenario_ValidDikeSoilScenario_ReturnStabilityLocationWithDikeSoilScenario(MacroStabilityInwardsDikeSoilScenario macroStabilityInwardsDikeSoilScenario,
                                                                                                              DikeSoilScenario expectedDikeSoilScenario)
        {
            // Call
            DikeSoilScenario convertedDikeSoilScenario = StabilityLocationCreatorHelper.ConvertDikeSoilScenario(macroStabilityInwardsDikeSoilScenario);

            // Assert
            Assert.AreEqual(expectedDikeSoilScenario, convertedDikeSoilScenario);
        }

        [Test]
        public void ConvertWaternetCreationMode_InvalidWaternetCreationMode_ThrowInvalidEnumArgumentException()
        {
            // Call
            TestDelegate test = () => StabilityLocationCreatorHelper.ConvertWaternetCreationMode((WaternetCreationMode) 99);

            // Assert
            string message = $"The value of argument 'waternetCreationMode' ({99}) is invalid for Enum type '{typeof(WaternetCreationMode).Name}'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(test, message);
        }

        [Test]
        [TestCase(WaternetCreationMode.CreateWaternet, WTIStabilityWaternetCreationMethod.CreateWaternet)]
        [TestCase(WaternetCreationMode.FillInWaternetValues, WTIStabilityWaternetCreationMethod.FillInWaternetValues)]
        public void ConvertWaternetCreationMode_ValidWaternetCreationMode_ReturnStabilityLocationWithWaternetCreationMode(WaternetCreationMode waternetCreationMode,
                                                                                                                          WTIStabilityWaternetCreationMethod expectedWaternetCreationMode)
        {
            // Call
            WTIStabilityWaternetCreationMethod convertedWaternetCreationMode = StabilityLocationCreatorHelper.ConvertWaternetCreationMode(waternetCreationMode);

            // Assert
            Assert.AreEqual(expectedWaternetCreationMode, convertedWaternetCreationMode);
        }

        [Test]
        public void ConvertPlLineCreationMethod_InvalidPlLineCreationMethod_ThrowInvalidEnumArgumentException()
        {
            // Call
            TestDelegate test = () => StabilityLocationCreatorHelper.ConvertPlLineCreationMethod((PlLineCreationMethod) 99);

            // Assert
            string message = $"The value of argument 'plLineCreationMethod' ({99}) is invalid for Enum type '{typeof(PlLineCreationMethod).Name}'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(test, message);
        }

        [Test]
        [TestCase(PlLineCreationMethod.ExpertKnowledgeRrd, WTIStabilityPlLineCreationMethod.ExpertKnowledgeRrd)]
        [TestCase(PlLineCreationMethod.ExpertKnowledgeLinearInDike, WTIStabilityPlLineCreationMethod.ExpertKnowledgeLinearInDike)]
        [TestCase(PlLineCreationMethod.RingtoetsWti2017, WTIStabilityPlLineCreationMethod.RingtoetsWti2017)]
        [TestCase(PlLineCreationMethod.DupuitStatic, WTIStabilityPlLineCreationMethod.DupuitStatic)]
        [TestCase(PlLineCreationMethod.DupuitDynamic, WTIStabilityPlLineCreationMethod.DupuitDynamic)]
        [TestCase(PlLineCreationMethod.Sensors, WTIStabilityPlLineCreationMethod.Sensors)]
        [TestCase(PlLineCreationMethod.None, WTIStabilityPlLineCreationMethod.None)]
        public void ConvertPlLineCreationMethod_ValidPlLineCreationMethod_ReturnStabilityLocationWithWaternetCreationMode(PlLineCreationMethod plLineCreationMethod,
                                                                                                                          WTIStabilityPlLineCreationMethod expectedPlLineCreationMethod)
        {
            // Call
            WTIStabilityPlLineCreationMethod actualPlLineCreationMethod = StabilityLocationCreatorHelper.ConvertPlLineCreationMethod(plLineCreationMethod);

            // Assert
            Assert.AreEqual(expectedPlLineCreationMethod, actualPlLineCreationMethod);
        }
    }
}