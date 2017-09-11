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
using Ringtoets.MacroStabilityInwards.Primitives;
using Ringtoets.MacroStabilityInwards.Primitives.MacroStabilityInwardsSoilUnderSurfaceLine;

namespace Ringtoets.MacroStabilityInwards.KernelWrapper
{
    /// <summary>
    /// This class contains all the parameters that are required to perform a macro stability inwards assessment.
    /// </summary>
    public class MacroStabilityInwardsCalculatorInput
    {
        /// <summary>
        /// Constructs a new <see cref="MacroStabilityInwardsCalculatorInput"/>, which contains values for the parameters used
        /// in the macro stability inwards sub calculations.
        /// </summary>
        /// <param name="properties">The object containing the values for the properties 
        /// of the new <see cref="MacroStabilityInwardsCalculatorInput"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="properties"/> is <c>null</c>.</exception>
        public MacroStabilityInwardsCalculatorInput(ConstructionProperties properties)
        {
            if (properties == null)
            {
                throw new ArgumentNullException(nameof(properties));
            }
            AssessmentLevel = properties.AssessmentLevel;
            SurfaceLine = properties.SurfaceLine;
            SoilProfile = properties.SoilProfile;
            WaterLevelRiverAverage = properties.WaterLevelRiverAverage;
            WaterLevelPolder = properties.WaterLevelPolder;
            XCoordinateDrainageConstruction = properties.XCoordinateDrainageConstruction;
            ZCoordinateDrainageConstruction = properties.ZCoordinateDrainageConstruction;
            MinimumLevelPhreaticLineAtDikeTopRiver = properties.MinimumLevelPhreaticLineAtDikeTopRiver;
            MinimumLevelPhreaticLineAtDikeTopPolder = properties.MinimumLevelPhreaticLineAtDikeTopPolder;
            PhreaticLineOffsetBelowDikeTopAtRiver = properties.PhreaticLineOffsetBelowDikeTopAtRiver;
            PhreaticLineOffsetBelowDikeTopAtPolder = properties.PhreaticLineOffsetBelowDikeTopAtPolder;
            PhreaticLineOffsetBelowShoulderBaseInside = properties.PhreaticLineOffsetBelowShoulderBaseInside;
            PhreaticLineOffsetBelowDikeToeAtPolder = properties.PhreaticLineOffsetBelowDikeToeAtPolder;
            LeakageLengthOutwardsPhreaticLine3 = properties.LeakageLengthOutwardsPhreaticLine3;
            LeakageLengthInwardsPhreaticLine3 = properties.LeakageLengthInwardsPhreaticLine3;
            LeakageLengthOutwardsPhreaticLine4 = properties.LeakageLengthOutwardsPhreaticLine4;
            LeakageLengthInwardsPhreaticLine4 = properties.LeakageLengthInwardsPhreaticLine4;
            PiezometricHeadPhreaticLine2Outwards = properties.PiezometricHeadPhreaticLine2Outwards;
            PiezometricHeadPhreaticLine2Inwards = properties.PiezometricHeadPhreaticLine2Inwards;
            PenetrationLength = properties.PenetrationLength;
            DrainageConstructionPresent = properties.DrainageConstructionPresent;
            AdjustPhreaticLine3And4ForUplift = properties.AdjustPhreaticLine3And4ForUplift;
            UseDefaultOffsets = properties.UseDefaultOffsets;
            DikeSoilScenario = properties.DikeSoilScenario;
            MoveGrid = properties.MoveGrid;
            MaximumSliceWidth = properties.MaximumSliceWidth;
            GridAutomaticDetermined = properties.GridAutomaticDetermined;
            LeftGrid = properties.LeftGrid;
            RightGrid = properties.RightGrid;
            TangentLineAutomaticAtBoundaries = properties.TangentLineAutomaticAtBoundaries;
            TangentLineZTop = properties.TangentLineZTop;
            TangentLineZBottom = properties.TangentLineZBottom;
        }

        public class ConstructionProperties
        {
            /// <summary>
            /// Creates new instance of <see cref="ConstructionProperties"/>.
            /// </summary>
            public ConstructionProperties()
            {
                AssessmentLevel = double.NaN;
                WaterLevelRiverAverage = double.NaN;
                WaterLevelPolder = double.NaN;
                XCoordinateDrainageConstruction = double.NaN;
                ZCoordinateDrainageConstruction = double.NaN;
                MinimumLevelPhreaticLineAtDikeTopRiver = double.NaN;
                MinimumLevelPhreaticLineAtDikeTopPolder = double.NaN;
                PhreaticLineOffsetBelowDikeTopAtRiver = double.NaN;
                PhreaticLineOffsetBelowDikeTopAtPolder = double.NaN;
                PhreaticLineOffsetBelowShoulderBaseInside = double.NaN;
                PhreaticLineOffsetBelowDikeToeAtPolder = double.NaN;
                LeakageLengthOutwardsPhreaticLine3 = double.NaN;
                LeakageLengthInwardsPhreaticLine3 = double.NaN;
                LeakageLengthOutwardsPhreaticLine4 = double.NaN;
                LeakageLengthInwardsPhreaticLine4 = double.NaN;
                PiezometricHeadPhreaticLine2Outwards = double.NaN;
                PiezometricHeadPhreaticLine2Inwards = double.NaN;
                PenetrationLength = double.NaN;
                DikeSoilScenario = MacroStabilityInwardsDikeSoilScenario.ClayDikeOnClay;
                MaximumSliceWidth = double.NaN;
                TangentLineZTop = double.NaN;
                TangentLineZBottom = double.NaN;
            }

            #region properties

            /// <summary>
            /// Gets the outside high water level.
            /// [m]
            /// </summary>
            public double AssessmentLevel { internal get; set; }

            /// <summary>
            /// Gets the surface line.
            /// </summary>
            public MacroStabilityInwardsSurfaceLine SurfaceLine { internal get; set; }

            /// <summary>
            /// Gets the profile which contains a definition of soil layers with properties.
            /// </summary>
            public MacroStabilityInwardsSoilProfileUnderSurfaceLine SoilProfile { internal get; set; }

            /// <summary>
            /// Gets or sets the dike soil scenario.
            /// </summary>
            public MacroStabilityInwardsDikeSoilScenario DikeSoilScenario { internal get; set; }

            /// <summary>
            /// Gets or sets the average river water level.
            /// [m+NAP]
            /// </summary>
            public double WaterLevelRiverAverage { internal get; set; }

            /// <summary>
            /// Gets or sets the polder water level.
            /// [m+NAP]
            /// </summary>
            public double WaterLevelPolder { internal get; set; }

            /// <summary>
            /// Gets or sets whether a drainage construction is present.
            /// </summary>
            public bool DrainageConstructionPresent { internal get; set; }

            /// <summary>
            /// Gets or sets the x coordinate of the drainage construction.
            /// [m]
            /// </summary>
            public double XCoordinateDrainageConstruction { internal get; set; }

            /// <summary>
            /// Gets or sets the z coordinate of the drainage construction.
            /// [m+NAP]
            /// </summary>
            public double ZCoordinateDrainageConstruction { internal get; set; }

            /// <summary>
            /// Gets or sets the minimum level phreatic line at dike top river.
            /// [m+NAP]
            /// </summary>
            public double MinimumLevelPhreaticLineAtDikeTopRiver { internal get; set; }

            /// <summary>
            /// Gets or sets the minimum level phreatic line at dike top polder.
            /// [m+NAP]
            /// </summary>
            public double MinimumLevelPhreaticLineAtDikeTopPolder { internal get; set; }

            /// <summary>
            /// Gets or sets whether the default offset should be used.
            /// </summary>
            public bool UseDefaultOffsets { internal get; set; }

            /// <summary>
            /// Gets or sets the offset of the phreatic line below dike top at river.
            /// [m]
            /// </summary>
            public double PhreaticLineOffsetBelowDikeTopAtRiver { internal get; set; }

            /// <summary>
            /// Gets or sets the offset of the phreatic line below dike top at polder.
            /// [m]
            /// </summary>
            public double PhreaticLineOffsetBelowDikeTopAtPolder { internal get; set; }

            /// <summary>
            /// Gets or sets the offset of the phreatic line below shoulder base inside.
            /// [m]
            /// </summary>
            public double PhreaticLineOffsetBelowShoulderBaseInside { internal get; set; }

            /// <summary>
            /// Gets or sets the offset of the phreatic line below dike toe at polder.
            /// [m]
            /// </summary>
            public double PhreaticLineOffsetBelowDikeToeAtPolder { internal get; set; }

            /// <summary>
            /// Gets or sets whether phreatic line 3 and 4 should be adjusted for uplift.
            /// </summary>
            public bool AdjustPhreaticLine3And4ForUplift { internal get; set; }

            /// <summary>
            /// Gets or sets the leakage length outwards of phreatic line 3.
            /// [m]
            /// </summary>
            public double LeakageLengthOutwardsPhreaticLine3 { internal get; set; }

            /// <summary>
            /// Gets or sets the leakage length inwards of phreatic line 3.
            /// [m]
            /// </summary>
            public double LeakageLengthInwardsPhreaticLine3 { internal get; set; }

            /// <summary>
            /// Gets or sets the leakage length outwards of phreatic line 4.
            /// [m]
            /// </summary>
            public double LeakageLengthOutwardsPhreaticLine4 { internal get; set; }

            /// <summary>
            /// Gets or sets the leakage length inwards of phreatic line 4.
            /// [m]
            /// </summary>
            public double LeakageLengthInwardsPhreaticLine4 { internal get; set; }

            /// <summary>
            /// Gets or sets the piezometric head of the phreatic line 2 outwards.
            /// [m+NAP]
            /// </summary>
            public double PiezometricHeadPhreaticLine2Outwards { internal get; set; }

            /// <summary>
            /// Gets or sets the piezometric head of the phreatic line 2 inwards.
            /// [m+NAP]
            /// </summary>
            public double PiezometricHeadPhreaticLine2Inwards { internal get; set; }

            /// <summary>
            /// Gets or sets the penetration length.
            /// [m]
            /// </summary>
            public double PenetrationLength { internal get; set; }

            /// <summary>
            /// Gets or sets the value whether the grid should be moved.
            /// </summary>
            public bool MoveGrid { internal get; set; }

            /// <summary>
            /// Gets or sets the maximum slice width.
            /// [m]
            /// </summary>
            public double MaximumSliceWidth { internal get; set; }

            /// <summary>
            /// Gets or sets whether the grid is automatic determined or not.
            /// </summary>
            public bool GridAutomaticDetermined { internal get; set; }

            /// <summary>
            /// Gets or sets the left grid.
            /// </summary>
            public MacroStabilityInwardsGrid LeftGrid { internal get; set; }
            
            /// <summary>
            /// Gets or sets the right grid.
            /// </summary>
            public MacroStabilityInwardsGrid RightGrid { internal get; set; }

            /// <summary>
            /// Gets or sets whether the tangent line boundaries should be defined automatically.
            /// </summary>
            public bool TangentLineAutomaticAtBoundaries { internal get; set; }

            /// <summary>
            /// Gets or sets the tangent line z top.
            /// [m+NAP]
            /// </summary>
            public double TangentLineZTop { internal get; set; }

            /// <summary>
            /// Gets or sets the tangent line z bottom.
            /// [m+NAP]
            /// </summary>
            public double TangentLineZBottom { internal get; set; }

            #endregion
        }

        #region properties

        /// <summary>
        /// Gets the outside high water level.
        /// [m]
        /// </summary>
        public double AssessmentLevel { get; }

        /// <summary>
        /// Gets the surface line.
        /// </summary>
        public MacroStabilityInwardsSurfaceLine SurfaceLine { get; }

        /// <summary>
        /// Gets the profile which contains a definition of soil layers with properties.
        /// </summary>
        public MacroStabilityInwardsSoilProfileUnderSurfaceLine SoilProfile { get; }

        /// <summary>
        /// Gets the dike soil scenario.
        /// </summary>
        public MacroStabilityInwardsDikeSoilScenario DikeSoilScenario { get; }

        /// <summary>
        /// Gets the average river water level.
        /// [m+NAP]
        /// </summary>
        public double WaterLevelRiverAverage { get; }

        /// <summary>
        /// Gets the polder water level.
        /// [m+NAP]
        /// </summary>
        public double WaterLevelPolder { get; }

        /// <summary>
        /// Gets whether a drainage construction is present.
        /// </summary>
        public bool DrainageConstructionPresent { get; }

        /// <summary>
        /// Gets the x coordinate of the drainage construction.
        /// [m]
        /// </summary>
        public double XCoordinateDrainageConstruction { get; }

        /// <summary>
        /// Gets the z coordinate of the drainage construction.
        /// [m+NAP]
        /// </summary>
        public double ZCoordinateDrainageConstruction { get; }

        /// <summary>
        /// Gets the minimum level phreatic line at dike top river.
        /// [m+NAP]
        /// </summary>
        public double MinimumLevelPhreaticLineAtDikeTopRiver { get; }

        /// <summary>
        /// Gets the minimum level phreatic line at dike top polder.
        /// [m+NAP]
        /// </summary>
        public double MinimumLevelPhreaticLineAtDikeTopPolder { get; }

        /// <summary>
        /// Gets whether the default offset should be used.
        /// </summary>
        public bool UseDefaultOffsets { get; }

        /// <summary>
        /// Gets the offset of the phreatic line below dike top at river.
        /// [m]
        /// </summary>
        public double PhreaticLineOffsetBelowDikeTopAtRiver { get; }

        /// <summary>
        /// Gets the offset of the phreatic line below dike top at polder.
        /// [m]
        /// </summary>
        public double PhreaticLineOffsetBelowDikeTopAtPolder { get; }

        /// <summary>
        /// Gets the offset of the phreatic line below shoulder base inside.
        /// [m]
        /// </summary>
        public double PhreaticLineOffsetBelowShoulderBaseInside { get; }

        /// <summary>
        /// Gets the offset of the phreatic line below dike toe at polder.
        /// [m]
        /// </summary>
        public double PhreaticLineOffsetBelowDikeToeAtPolder { get; }

        /// <summary>
        /// Gets whether phreatic line 3 and 4 should be adjusted for uplift.
        /// </summary>
        public bool AdjustPhreaticLine3And4ForUplift { get; }

        /// <summary>
        /// Gets the leakage length outwards of phreatic line 3.
        /// [m]
        /// </summary>
        public double LeakageLengthOutwardsPhreaticLine3 { get; }

        /// <summary>
        /// Gets the leakage length inwards of phreatic line 3.
        /// [m]
        /// </summary>
        public double LeakageLengthInwardsPhreaticLine3 { get; }

        /// <summary>
        /// Gets the leakage length outwards of phreatic line 4.
        /// [m]
        /// </summary>
        public double LeakageLengthOutwardsPhreaticLine4 { get; }

        /// <summary>
        /// Gets the leakage length inwards of phreatic line 4.
        /// [m]
        /// </summary>
        public double LeakageLengthInwardsPhreaticLine4 { get; }

        /// <summary>
        /// Gets the piezometric head of the phreatic line 2 outwards.
        /// [m+NAP]
        /// </summary>
        public double PiezometricHeadPhreaticLine2Outwards { get; }

        /// <summary>
        /// Gets the piezometric head of the phreatic line 2 inwards.
        /// [m+NAP]
        /// </summary>
        public double PiezometricHeadPhreaticLine2Inwards { get; }

        /// <summary>
        /// Gets the penetration length.
        /// [m]
        /// </summary>
        public double PenetrationLength { get; }

        /// <summary>
        /// Gets the value whether the grid should be moved.
        /// </summary>
        public bool MoveGrid { get; }

        /// <summary>
        /// Gets the maximum slice width.
        /// [m]
        /// </summary>
        public double MaximumSliceWidth { get; }

        /// <summary>
        /// Gets whether the grid is automatic determined or not.
        /// </summary>
        public bool GridAutomaticDetermined { get; }

        /// <summary>
        /// Gets the left grid.
        /// </summary>
        public MacroStabilityInwardsGrid LeftGrid { get; }

        /// <summary>
        /// Gets the right grid.
        /// </summary>
        public MacroStabilityInwardsGrid RightGrid { get; }

        /// <summary>
        /// Gets whether the tangent line boundaries should be defined automatically.
        /// </summary>
        public bool TangentLineAutomaticAtBoundaries { get; }

        /// <summary>
        /// Gets the tangent line z top.
        /// [m+NAP]
        /// </summary>
        public double TangentLineZTop { get; }

        /// <summary>
        /// Gets the tangent line z bottom.
        /// [m+NAP]
        /// </summary>
        public double TangentLineZBottom { get; }

        #endregion
    }
}