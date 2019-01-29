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

using Core.Common.Base.Data;

namespace Riskeer.MacroStabilityInwards.Primitives
{
    /// <summary>
    /// Interface that holds macro stability inwards calculation specific input parameters for
    /// a Waternet calculation.
    /// </summary>
    public interface IMacroStabilityInwardsWaternetInput
    {
        /// <summary>
        /// Gets the surface line.
        /// </summary>
        MacroStabilityInwardsSurfaceLine SurfaceLine { get; }

        /// <summary>
        /// Gets the profile which contains a 2 dimensional definition of soil layers with properties.
        /// </summary>
        IMacroStabilityInwardsSoilProfileUnderSurfaceLine SoilProfileUnderSurfaceLine { get; }

        /// <summary>
        /// Gets the dike soil scenario.
        /// </summary>
        MacroStabilityInwardsDikeSoilScenario DikeSoilScenario { get; }

        /// <summary>
        /// Gets the average river water level.
        /// [m+NAP]
        /// </summary>
        RoundedDouble WaterLevelRiverAverage { get; }

        /// <summary>
        /// Gets whether a drainage construction is present.
        /// </summary>
        bool DrainageConstructionPresent { get; }

        /// <summary>
        /// Gets the x coordinate of the drainage construction.
        /// [m]
        /// </summary>
        RoundedDouble XCoordinateDrainageConstruction { get; }

        /// <summary>
        /// Gets the z coordinate of the drainage construction.
        /// [m+NAP]
        /// </summary>
        RoundedDouble ZCoordinateDrainageConstruction { get; }

        /// <summary>
        /// Gets the minimum level phreatic line at dike top river.
        /// [m+NAP]
        /// </summary>
        RoundedDouble MinimumLevelPhreaticLineAtDikeTopRiver { get; }

        /// <summary>
        /// Gets the minimum level phreatic line at dike top polder.
        /// [m+NAP]
        /// </summary>
        RoundedDouble MinimumLevelPhreaticLineAtDikeTopPolder { get; }

        /// <summary>
        /// Gets whether phreatic line 3 and 4 should be adjusted for Uplift.
        /// </summary>
        bool AdjustPhreaticLine3And4ForUplift { get; }

        /// <summary>
        /// Gets the leakage length outwards of phreatic line 3.
        /// [m]
        /// </summary>
        RoundedDouble LeakageLengthOutwardsPhreaticLine3 { get; }

        /// <summary>
        /// Gets the leakage length inwards of phreatic line 3.
        /// [m]
        /// </summary>
        RoundedDouble LeakageLengthInwardsPhreaticLine3 { get; }

        /// <summary>
        /// Gets the leakage length outwards of phreatic line 4.
        /// [m]
        /// </summary>
        RoundedDouble LeakageLengthOutwardsPhreaticLine4 { get; }

        /// <summary>
        /// Gets the leakage length inwards of phreatic line 4.
        /// [m]
        /// </summary>
        RoundedDouble LeakageLengthInwardsPhreaticLine4 { get; }

        /// <summary>
        /// Gets the piezometric head of the phreatic line 2 outwards.
        /// [m+NAP]
        /// </summary>
        RoundedDouble PiezometricHeadPhreaticLine2Outwards { get; }

        /// <summary>
        /// Gets the piezometric head of the phreatic line 2 inwards.
        /// [m+NAP]
        /// </summary>
        RoundedDouble PiezometricHeadPhreaticLine2Inwards { get; }

        /// <summary>
        /// Gets the locations input values for extreme conditions.
        /// </summary>
        IMacroStabilityInwardsLocationInputExtreme LocationInputExtreme { get; }

        /// <summary>
        /// Gets the locations input values for daily conditions.
        /// </summary>
        IMacroStabilityInwardsLocationInputDaily LocationInputDaily { get; }
    }
}