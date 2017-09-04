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
using Core.Common.Base;
using Core.Common.Base.Data;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.MacroStabilityInwards.Data.SoilProfile;
using Ringtoets.MacroStabilityInwards.Primitives;

namespace Ringtoets.MacroStabilityInwards.Data
{
    /// <summary>
    /// Class that holds all macro stability inwards calculation specific input parameters, i.e. the values
    /// that can differ across various calculations.
    /// </summary>
    public class MacroStabilityInwardsInput : Observable, ICalculationInput
    {
        private RoundedDouble assessmentLevel;
        private bool useAssessmentLevelManualInput;
        private RoundedDouble slipPlaneMinimumDepth;
        private RoundedDouble slipPlaneMinimumLength;
        private RoundedDouble maximumSliceWidth;
        private RoundedDouble waterLevelRiverAverage;
        private RoundedDouble waterLevelPolder;
        private RoundedDouble xCoordinateDrainageConstruction;
        private RoundedDouble zCoordinateDrainageConstruction;
        private RoundedDouble minimumLevelPhreaticLineAtDikeTopRiver;
        private RoundedDouble minimumLevelPhreaticLineAtDikeTopPolder;
        private RoundedDouble phreaticLineOffsetBelowDikeTopAtRiver;
        private RoundedDouble phreaticLineOffsetBelowDikeTopAtPolder;
        private RoundedDouble phreaticLineOffsetBelowShoulderBaseInside;
        private RoundedDouble phreaticLineOffsetBelowDikeToeAtPolder;
        private RoundedDouble leakageLengthOutwardsPhreaticLine3;
        private RoundedDouble leakageLengthInwardsPhreaticLine3;
        private RoundedDouble leakageLengthOutwardsPhreaticLine4;
        private RoundedDouble leakageLengthInwardsPhreaticLine4;
        private RoundedDouble piezometricHeadPhreaticLine2Outwards;
        private RoundedDouble piezometricHeadPhreaticLine2Inwards;
        private RoundedDouble penetrationLength;
        private RoundedDouble tangentLineZTop;
        private RoundedDouble tangentLineZBottom;
        private MacroStabilityInwardsSurfaceLine surfaceLine;
        private MacroStabilityInwardsStochasticSoilProfile stochasticSoilProfile;

        /// <summary>
        /// Initializes a new instance of the <see cref="MacroStabilityInwardsInput"/> class.
        /// </summary>
        public MacroStabilityInwardsInput()
        {
            assessmentLevel = new RoundedDouble(2, double.NaN);
            useAssessmentLevelManualInput = false;

            slipPlaneMinimumDepth = new RoundedDouble(2, 10);
            slipPlaneMinimumLength = new RoundedDouble(2, 30);
            maximumSliceWidth = new RoundedDouble(2, 5);

            MoveGrid = true;
            DikeSoilScenario = MacroStabilityInwardsDikeSoilScenario.ClayDikeOnClay;

            waterLevelRiverAverage = new RoundedDouble(2, double.NaN);
            waterLevelPolder = new RoundedDouble(2, double.NaN);

            xCoordinateDrainageConstruction = new RoundedDouble(2, double.NaN);
            zCoordinateDrainageConstruction = new RoundedDouble(2, double.NaN);

            minimumLevelPhreaticLineAtDikeTopRiver = new RoundedDouble(2, double.NaN);
            minimumLevelPhreaticLineAtDikeTopPolder = new RoundedDouble(2, double.NaN);

            UseDefaultOffset = true;

            phreaticLineOffsetBelowDikeTopAtRiver = new RoundedDouble(2, double.NaN);
            phreaticLineOffsetBelowDikeTopAtPolder = new RoundedDouble(2, double.NaN);
            phreaticLineOffsetBelowShoulderBaseInside = new RoundedDouble(2, double.NaN);
            phreaticLineOffsetBelowDikeToeAtPolder = new RoundedDouble(2, double.NaN);

            AdjustPhreaticLine3And4ForUplift = true;

            leakageLengthOutwardsPhreaticLine3 = new RoundedDouble(2, double.NaN);
            leakageLengthInwardsPhreaticLine3 = new RoundedDouble(2, double.NaN);
            leakageLengthOutwardsPhreaticLine4 = new RoundedDouble(2, double.NaN);
            leakageLengthInwardsPhreaticLine4 = new RoundedDouble(2, double.NaN);
            piezometricHeadPhreaticLine2Outwards = new RoundedDouble(2, double.NaN);
            piezometricHeadPhreaticLine2Inwards = new RoundedDouble(2, double.NaN);
            penetrationLength = new RoundedDouble(2, double.NaN);

            GridDetermination = MacroStabilityInwardsGridDetermination.Automatic;
            TangentLineDetermination = MacroStabilityInwardsTangentLineDetermination.LayerSeparated;

            tangentLineZTop = new RoundedDouble(2, double.NaN);
            tangentLineZBottom = new RoundedDouble(2, double.NaN);

            LeftGrid = new MacroStabilityInwardsGrid();
            RightGrid = new MacroStabilityInwardsGrid();
        }

        /// <summary>
        /// Gets or sets the surface line.
        /// </summary>
        public MacroStabilityInwardsSurfaceLine SurfaceLine
        {
            get
            {
                return surfaceLine;
            }
            set
            {
                surfaceLine = value;
                SetSoilProfileUnderSurfaceLine();
            }
        }

        /// <summary>
        /// Gets or sets the stochastic soil model which is linked to the <see cref="StochasticSoilProfile"/>.
        /// </summary>
        public MacroStabilityInwardsStochasticSoilModel StochasticSoilModel { get; set; }

        /// <summary>
        /// Gets or sets the profile which contains a definition of soil layers with properties.
        /// </summary>
        public MacroStabilityInwardsStochasticSoilProfile StochasticSoilProfile
        {
            get
            {
                return stochasticSoilProfile;
            }
            set
            {
                stochasticSoilProfile = value;
                SetSoilProfileUnderSurfaceLine();
            }
        }

        /// <summary>
        /// Gets or sets the hydraulic boundary location from which to use the assessment level.
        /// </summary>
        public HydraulicBoundaryLocation HydraulicBoundaryLocation { get; set; }

        /// <summary>
        /// Gets or sets whether the assessment level is manual input for the calculation.
        /// </summary>
        public bool UseAssessmentLevelManualInput
        {
            get
            {
                return useAssessmentLevelManualInput;
            }
            set
            {
                useAssessmentLevelManualInput = value;

                if (useAssessmentLevelManualInput)
                {
                    HydraulicBoundaryLocation = null;
                }
            }
        }

        #region Derived input

        /// <summary>
        /// Gets or sets the outside high water level.
        /// [m]
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when the user attempts to set the 
        /// assessment level while <see cref="UseAssessmentLevelManualInput"/> is <c>false</c>.</exception>
        public RoundedDouble AssessmentLevel
        {
            get
            {
                if (!UseAssessmentLevelManualInput)
                {
                    return HydraulicBoundaryLocation?.DesignWaterLevel ?? RoundedDouble.NaN;
                }

                return assessmentLevel;
            }
            set
            {
                if (!UseAssessmentLevelManualInput)
                {
                    throw new InvalidOperationException("UseAssessmentLevelManualInput is false");
                }

                assessmentLevel = value.ToPrecision(assessmentLevel.NumberOfDecimalPlaces);
            }
        }

        /// <summary>
        /// Gets the profile which contains a 2 dimensional definition of soil layers with properties.
        /// </summary>
        public MacroStabilityInwardsSoilProfileUnderSurfaceLine SoilProfileUnderSurfaceLine { get; private set; }

        private void SetSoilProfileUnderSurfaceLine()
        {
            SoilProfileUnderSurfaceLine = SurfaceLine != null && StochasticSoilProfile != null
                                              ? MacroStabilityInwardsSoilProfileUnderSurfaceLineFactory.Create(StochasticSoilProfile.SoilProfile, SurfaceLine)
                                              : null;
        }

        #endregion

        #region settings

        /// <summary>
        /// Gets or sets the minimum depth of the slip plane.
        /// [m]
        /// </summary>
        public RoundedDouble SlipPlaneMinimumDepth
        {
            get
            {
                return slipPlaneMinimumDepth;
            }
            set
            {
                slipPlaneMinimumDepth = value.ToPrecision(slipPlaneMinimumDepth.NumberOfDecimalPlaces);
            }
        }

        /// <summary>
        /// Gets or sets the minimum length of the slip plane.
        /// [m]
        /// </summary>
        public RoundedDouble SlipPlaneMinimumLength
        {
            get
            {
                return slipPlaneMinimumLength;
            }
            set
            {
                slipPlaneMinimumLength = value.ToPrecision(slipPlaneMinimumLength.NumberOfDecimalPlaces);
            }
        }

        /// <summary>
        /// Gets or sets the maximum slice width.
        /// [m]
        /// </summary>
        public RoundedDouble MaximumSliceWidth
        {
            get
            {
                return maximumSliceWidth;
            }
            set
            {
                maximumSliceWidth = value.ToPrecision(maximumSliceWidth.NumberOfDecimalPlaces);
            }
        }

        /// <summary>
        /// Gets or sets the value whether the grid should be moved.
        /// </summary>
        public bool MoveGrid { get; set; }

        /// <summary>
        /// Gets the grid determination type.
        /// </summary>
        public MacroStabilityInwardsGridDetermination GridDetermination { get; set; }

        /// <summary>
        /// Gets the tangent line determination type.
        /// </summary>
        public MacroStabilityInwardsTangentLineDetermination TangentLineDetermination { get; set; }

        /// <summary>
        /// Gets or sets the tangent line z top.
        /// [m+NAP]
        /// </summary>
        public RoundedDouble TangentLineZTop
        {
            get
            {
                return tangentLineZTop;
            }
            set
            {
                tangentLineZTop = value.ToPrecision(tangentLineZTop.NumberOfDecimalPlaces);
            }
        }

        /// <summary>
        /// Gets or sets the tangent line z bottom.
        /// [m+NAP]
        /// </summary>
        public RoundedDouble TangentLineZBottom
        {
            get
            {
                return tangentLineZBottom;
            }
            set
            {
                tangentLineZBottom = value.ToPrecision(tangentLineZBottom.NumberOfDecimalPlaces);
            }
        }

        /// <summary>
        /// Gets the left grid.
        /// </summary>
        public MacroStabilityInwardsGrid LeftGrid { get; }

        /// <summary>
        /// Gets the right grid.
        /// </summary>
        public MacroStabilityInwardsGrid RightGrid { get; }

        #endregion

        #region Hydraulics

        /// <summary>
        /// Gets or sets the dike soil scenario.
        /// </summary>
        public MacroStabilityInwardsDikeSoilScenario DikeSoilScenario { get; set; }

        /// <summary>
        /// Gets or sets the average river water level.
        /// [m+NAP]
        /// </summary>
        public RoundedDouble WaterLevelRiverAverage
        {
            get
            {
                return waterLevelRiverAverage;
            }
            set
            {
                waterLevelRiverAverage = value.ToPrecision(waterLevelRiverAverage.NumberOfDecimalPlaces);
            }
        }

        /// <summary>
        /// Gets or sets the polder water level.
        /// [m+NAP]
        /// </summary>
        public RoundedDouble WaterLevelPolder
        {
            get
            {
                return waterLevelPolder;
            }
            set
            {
                waterLevelPolder = value.ToPrecision(waterLevelPolder.NumberOfDecimalPlaces);
            }
        }

        /// <summary>
        /// Gets or sets whether a drainage construction is present.
        /// </summary>
        public bool DrainageConstructionPresent { get; set; }

        /// <summary>
        /// Gets or sets the x coordinate of the drainage construction.
        /// [m]
        /// </summary>
        public RoundedDouble XCoordinateDrainageConstruction
        {
            get
            {
                return xCoordinateDrainageConstruction;
            }
            set
            {
                xCoordinateDrainageConstruction = value.ToPrecision(xCoordinateDrainageConstruction.NumberOfDecimalPlaces);
            }
        }

        /// <summary>
        /// Gets or sets the z coordinate of the drainage construction.
        /// [m+NAP]
        /// </summary>
        public RoundedDouble ZCoordinateDrainageConstruction
        {
            get
            {
                return zCoordinateDrainageConstruction;
            }
            set
            {
                zCoordinateDrainageConstruction = value.ToPrecision(zCoordinateDrainageConstruction.NumberOfDecimalPlaces);
            }
        }

        /// <summary>
        /// Gets or sets the minimum level phreatic line at dike top river.
        /// [m+NAP]
        /// </summary>
        public RoundedDouble MinimumLevelPhreaticLineAtDikeTopRiver
        {
            get
            {
                return minimumLevelPhreaticLineAtDikeTopRiver;
            }
            set
            {
                minimumLevelPhreaticLineAtDikeTopRiver = value.ToPrecision(minimumLevelPhreaticLineAtDikeTopRiver.NumberOfDecimalPlaces);
            }
        }

        /// <summary>
        /// Gets or sets the minimum level phreatic line at dike top polder.
        /// [m+NAP]
        /// </summary>
        public RoundedDouble MinimumLevelPhreaticLineAtDikeTopPolder
        {
            get
            {
                return minimumLevelPhreaticLineAtDikeTopPolder;
            }
            set
            {
                minimumLevelPhreaticLineAtDikeTopPolder = value.ToPrecision(minimumLevelPhreaticLineAtDikeTopPolder.NumberOfDecimalPlaces);
            }
        }

        /// <summary>
        /// Gets or sets whether the default offset should be used.
        /// </summary>
        public bool UseDefaultOffset { get; set; }

        /// <summary>
        /// Gets or sets the offset of the phreatic line below dike top at river.
        /// [m]
        /// </summary>
        public RoundedDouble PhreaticLineOffsetBelowDikeTopAtRiver
        {
            get
            {
                return phreaticLineOffsetBelowDikeTopAtRiver;
            }
            set
            {
                phreaticLineOffsetBelowDikeTopAtRiver = value.ToPrecision(phreaticLineOffsetBelowDikeTopAtRiver.NumberOfDecimalPlaces);
            }
        }

        /// <summary>
        /// Gets or sets the offset of the phreatic line below dike top at polder.
        /// [m]
        /// </summary>
        public RoundedDouble PhreaticLineOffsetBelowDikeTopAtPolder
        {
            get
            {
                return phreaticLineOffsetBelowDikeTopAtPolder;
            }
            set
            {
                phreaticLineOffsetBelowDikeTopAtPolder = value.ToPrecision(phreaticLineOffsetBelowDikeTopAtPolder.NumberOfDecimalPlaces);
            }
        }

        /// <summary>
        /// Gets or sets the offset of the phreatic line below shoulder base inside.
        /// [m]
        /// </summary>
        public RoundedDouble PhreaticLineOffsetBelowShoulderBaseInside
        {
            get
            {
                return phreaticLineOffsetBelowShoulderBaseInside;
            }
            set
            {
                phreaticLineOffsetBelowShoulderBaseInside = value.ToPrecision(phreaticLineOffsetBelowShoulderBaseInside.NumberOfDecimalPlaces);
            }
        }

        /// <summary>
        /// Gets or sets the offset of the phreatic line below dike toe at polder.
        /// [m]
        /// </summary>
        public RoundedDouble PhreaticLineOffsetBelowDikeToeAtPolder
        {
            get
            {
                return phreaticLineOffsetBelowDikeToeAtPolder;
            }
            set
            {
                phreaticLineOffsetBelowDikeToeAtPolder = value.ToPrecision(phreaticLineOffsetBelowDikeToeAtPolder.NumberOfDecimalPlaces);
            }
        }

        /// <summary>
        /// Gets or sets whether phreatic line 3 and 4 should be adjusted for uplift.
        /// </summary>
        public bool AdjustPhreaticLine3And4ForUplift { get; set; }

        /// <summary>
        /// Gets or sets the leakage length outwards of phreatic line 3.
        /// [m]
        /// </summary>
        public RoundedDouble LeakageLengthOutwardsPhreaticLine3
        {
            get
            {
                return leakageLengthOutwardsPhreaticLine3;
            }
            set
            {
                leakageLengthOutwardsPhreaticLine3 = value.ToPrecision(leakageLengthOutwardsPhreaticLine3.NumberOfDecimalPlaces);
            }
        }

        /// <summary>
        /// Gets or sets the leakage length inwards of phreatic line 3.
        /// [m]
        /// </summary>
        public RoundedDouble LeakageLengthInwardsPhreaticLine3
        {
            get
            {
                return leakageLengthInwardsPhreaticLine3;
            }
            set
            {
                leakageLengthInwardsPhreaticLine3 = value.ToPrecision(leakageLengthInwardsPhreaticLine3.NumberOfDecimalPlaces);
            }
        }

        /// <summary>
        /// Gets or sets the leakage length outwards of phreatic line 4.
        /// [m]
        /// </summary>
        public RoundedDouble LeakageLengthOutwardsPhreaticLine4
        {
            get
            {
                return leakageLengthOutwardsPhreaticLine4;
            }
            set
            {
                leakageLengthOutwardsPhreaticLine4 = value.ToPrecision(leakageLengthOutwardsPhreaticLine4.NumberOfDecimalPlaces);
            }
        }

        /// <summary>
        /// Gets or sets the leakage length inwards of phreatic line 4.
        /// [m]
        /// </summary>
        public RoundedDouble LeakageLengthInwardsPhreaticLine4
        {
            get
            {
                return leakageLengthInwardsPhreaticLine4;
            }
            set
            {
                leakageLengthInwardsPhreaticLine4 = value.ToPrecision(leakageLengthInwardsPhreaticLine4.NumberOfDecimalPlaces);
            }
        }

        /// <summary>
        /// Gets or sets the piezometric head of the phreatic line 2 outwards.
        /// [m+NAP]
        /// </summary>
        public RoundedDouble PiezometricHeadPhreaticLine2Outwards
        {
            get
            {
                return piezometricHeadPhreaticLine2Outwards;
            }
            set
            {
                piezometricHeadPhreaticLine2Outwards = value.ToPrecision(piezometricHeadPhreaticLine2Outwards.NumberOfDecimalPlaces);
            }
        }

        /// <summary>
        /// Gets or sets the piezometric head of the phreatic line 2 inwards.
        /// [m+NAP]
        /// </summary>
        public RoundedDouble PiezometricHeadPhreaticLine2Inwards
        {
            get
            {
                return piezometricHeadPhreaticLine2Inwards;
            }
            set
            {
                piezometricHeadPhreaticLine2Inwards = value.ToPrecision(piezometricHeadPhreaticLine2Inwards.NumberOfDecimalPlaces);
            }
        }

        /// <summary>
        /// Gets or sets the penetration length.
        /// [m]
        /// </summary>
        public RoundedDouble PenetrationLength
        {
            get
            {
                return penetrationLength;
            }
            set
            {
                penetrationLength = value.ToPrecision(penetrationLength.NumberOfDecimalPlaces);
            }
        }

        #endregion
    }
}