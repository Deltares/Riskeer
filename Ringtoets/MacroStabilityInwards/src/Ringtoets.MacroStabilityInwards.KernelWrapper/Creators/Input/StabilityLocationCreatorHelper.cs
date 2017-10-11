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
using System.ComponentModel;
using Deltares.WaternetCreator;
using Ringtoets.MacroStabilityInwards.Primitives;
using PlLineCreationMethod = Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.Input.PlLineCreationMethod;
using WaternetCreationMode = Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.Input.WaternetCreationMode;
using WTIStabilityPlLineCreationMethod = Deltares.WaternetCreator.PlLineCreationMethod;
using WTIStabilityWaternetCreationMode = Deltares.WaternetCreator.WaternetCreationMode;

namespace Ringtoets.MacroStabilityInwards.KernelWrapper.Creators.Input
{
    /// <summary>
    /// Helper class to convert properties needed in the <see cref="StabilityLocationCreator"/>.
    /// </summary>
    public static class StabilityLocationCreatorHelper
    {
        /// <summary>
        /// Converts a <see cref="MacroStabilityInwardsDikeSoilScenario"/> into a <see cref="DikeSoilScenario"/>.
        /// </summary>
        /// <param name="dikeSoilScenario">The <see cref="MacroStabilityInwardsDikeSoilScenario"/> to convert.</param>
        /// <returns>A <see cref="DikeSoilScenario"/> based on <paramref name="dikeSoilScenario"/>.</returns>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="dikeSoilScenario"/>
        /// is an invalid value.</exception>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="dikeSoilScenario"/>
        /// is a valid value but unsupported.</exception>
        public static DikeSoilScenario ConvertDikeSoilScenario(MacroStabilityInwardsDikeSoilScenario dikeSoilScenario)
        {
            if (!Enum.IsDefined(typeof(MacroStabilityInwardsDikeSoilScenario), dikeSoilScenario))
            {
                throw new InvalidEnumArgumentException(nameof(dikeSoilScenario),
                                                       (int)dikeSoilScenario,
                                                       typeof(MacroStabilityInwardsDikeSoilScenario));
            }

            switch (dikeSoilScenario)
            {
                case MacroStabilityInwardsDikeSoilScenario.ClayDikeOnClay:
                    return DikeSoilScenario.ClayDikeOnClay;
                case MacroStabilityInwardsDikeSoilScenario.SandDikeOnClay:
                    return DikeSoilScenario.SandDikeOnClay;
                case MacroStabilityInwardsDikeSoilScenario.ClayDikeOnSand:
                    return DikeSoilScenario.ClayDikeOnSand;
                case MacroStabilityInwardsDikeSoilScenario.SandDikeOnSand:
                    return DikeSoilScenario.SandDikeOnSand;
                default:
                    throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Converts a <see cref="WaternetCreationMode"/> into a <see cref="WTIStabilityWaternetCreationMode"/>.
        /// </summary>
        /// <param name="waternetCreationMode">The <see cref="WaternetCreationMode"/> to convert.</param>
        /// <returns>A <see cref="WTIStabilityWaternetCreationMode"/> based on <paramref name="waternetCreationMode"/>.</returns>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="waternetCreationMode"/>
        /// is an invalid value.</exception>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="waternetCreationMode"/>
        /// is a valid value but unsupported.</exception>
        public static WTIStabilityWaternetCreationMode ConvertWaternetCreationMode(WaternetCreationMode waternetCreationMode)
        {
            if (!Enum.IsDefined(typeof(WaternetCreationMode), waternetCreationMode))
            {
                throw new InvalidEnumArgumentException(nameof(waternetCreationMode),
                                                       (int)waternetCreationMode,
                                                       typeof(WaternetCreationMode));
            }

            switch (waternetCreationMode)
            {
                case WaternetCreationMode.CreateWaternet:
                    return WTIStabilityWaternetCreationMode.CreateWaternet;
                case WaternetCreationMode.FillInWaternetValues:
                    return WTIStabilityWaternetCreationMode.FillInWaternetValues;
                default:
                    throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Converts a <see cref="PlLineCreationMethod"/> into a <see cref="WTIStabilityPlLineCreationMethod"/>.
        /// </summary>
        /// <param name="plLineCreationMethod">The <see cref="PlLineCreationMethod"/> to convert.</param>
        /// <returns>A <see cref="WTIStabilityPlLineCreationMethod"/> based on <paramref name="plLineCreationMethod"/>.</returns>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="plLineCreationMethod"/>
        /// is an invalid value.</exception>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="plLineCreationMethod"/>
        /// is a valid value but unsupported.</exception>
        public static WTIStabilityPlLineCreationMethod ConvertPlLineCreationMethod(PlLineCreationMethod plLineCreationMethod)
        {
            if (!Enum.IsDefined(typeof(PlLineCreationMethod), plLineCreationMethod))
            {
                throw new InvalidEnumArgumentException(nameof(plLineCreationMethod),
                                                       (int)plLineCreationMethod,
                                                       typeof(PlLineCreationMethod));
            }

            switch (plLineCreationMethod)
            {
                case PlLineCreationMethod.ExpertKnowledgeRrd:
                    return WTIStabilityPlLineCreationMethod.ExpertKnowledgeRrd;
                case PlLineCreationMethod.ExpertKnowledgeLinearInDike:
                    return WTIStabilityPlLineCreationMethod.ExpertKnowledgeLinearInDike;
                case PlLineCreationMethod.RingtoetsWti2017:
                    return WTIStabilityPlLineCreationMethod.RingtoetsWti2017;
                case PlLineCreationMethod.DupuitStatic:
                    return WTIStabilityPlLineCreationMethod.DupuitStatic;
                case PlLineCreationMethod.DupuitDynamic:
                    return WTIStabilityPlLineCreationMethod.DupuitDynamic;
                case PlLineCreationMethod.Sensors:
                    return WTIStabilityPlLineCreationMethod.Sensors;
                case PlLineCreationMethod.None:
                    return WTIStabilityPlLineCreationMethod.None;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}