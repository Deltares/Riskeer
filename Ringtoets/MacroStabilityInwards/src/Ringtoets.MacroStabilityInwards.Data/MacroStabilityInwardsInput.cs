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
using Ringtoets.MacroStabilityInwards.Data.Properties;
using Ringtoets.MacroStabilityInwards.Data.SoilProfile;
using Ringtoets.MacroStabilityInwards.Primitives;

namespace Ringtoets.MacroStabilityInwards.Data
{
    /// <summary>
    /// Class that holds all macro stability inwards calculation specific input parameters, i.e. the values
    /// that can differ across various calculations.
    /// </summary>
    public class MacroStabilityInwardsInput : CloneableObservable, ICalculationInputWithHydraulicBoundaryLocation, IMacroStabilityInwardsWaternetInput
    {
        private static readonly Range<int> tangentLineNumberValidityRange = new Range<int>(1, 50);

        private RoundedDouble assessmentLevel;
        private RoundedDouble slipPlaneMinimumDepth;
        private RoundedDouble slipPlaneMinimumLength;
        private RoundedDouble maximumSliceWidth;
        private RoundedDouble waterLevelRiverAverage;
        private RoundedDouble xCoordinateDrainageConstruction;
        private RoundedDouble zCoordinateDrainageConstruction;
        private RoundedDouble minimumLevelPhreaticLineAtDikeTopRiver;
        private RoundedDouble minimumLevelPhreaticLineAtDikeTopPolder;
        private RoundedDouble leakageLengthOutwardsPhreaticLine3;
        private RoundedDouble leakageLengthInwardsPhreaticLine3;
        private RoundedDouble leakageLengthOutwardsPhreaticLine4;
        private RoundedDouble leakageLengthInwardsPhreaticLine4;
        private RoundedDouble piezometricHeadPhreaticLine2Outwards;
        private RoundedDouble piezometricHeadPhreaticLine2Inwards;
        private RoundedDouble tangentLineZTop;
        private RoundedDouble tangentLineZBottom;
        private RoundedDouble zoneBoundaryLeft;
        private RoundedDouble zoneBoundaryRight;
        private int tangentLineNumber;

        /// <summary>
        /// Initializes a new instance of the <see cref="MacroStabilityInwardsInput"/> class.
        /// </summary>
        /// <param name="properties">The container of the properties for the
        /// <see cref="MacroStabilityInwardsInput"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="properties"/>
        /// is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when either:
        /// <list type="bullet">
        /// <item><see cref="ConstructionProperties.LeftGridXRight"/> is smaller than <see cref="ConstructionProperties.LeftGridXLeft"/>;</item>
        /// <item><see cref="ConstructionProperties.LeftGridZBottom"/> is larger than <see cref="ConstructionProperties.LeftGridZTop"/>;</item>
        /// <item><see cref="ConstructionProperties.RightGridXRight"/> is smaller than <see cref="ConstructionProperties.RightGridXLeft"/>;</item>
        /// <item><see cref="ConstructionProperties.RightGridZBottom"/> is larger than <see cref="ConstructionProperties.RightGridZTop"/>;</item>
        /// <item><see cref="ConstructionProperties.TangentLineZBottom"/> is larger than <see cref="ConstructionProperties.TangentLineZTop"/>.</item>
        /// </list>
        /// </exception>
        public MacroStabilityInwardsInput(ConstructionProperties properties)
        {
            if (properties == null)
            {
                throw new ArgumentNullException(nameof(properties));
            }

            if (!IsSmallerEqualOrNaN(properties.TangentLineZBottom, properties.TangentLineZTop))
            {
                throw new ArgumentException(Resources.MacroStabilityInwardsInput_TangentLineZTop_should_be_larger_than_or_equal_to_TangentLineZBottom);
            }

            assessmentLevel = new RoundedDouble(2, double.NaN);

            slipPlaneMinimumDepth = new RoundedDouble(2);
            slipPlaneMinimumLength = new RoundedDouble(2);
            maximumSliceWidth = new RoundedDouble(2, 1);

            MoveGrid = true;
            DikeSoilScenario = MacroStabilityInwardsDikeSoilScenario.ClayDikeOnClay;

            waterLevelRiverAverage = new RoundedDouble(2, double.NaN);

            xCoordinateDrainageConstruction = new RoundedDouble(2, double.NaN);
            zCoordinateDrainageConstruction = new RoundedDouble(2, double.NaN);

            minimumLevelPhreaticLineAtDikeTopRiver = new RoundedDouble(2, double.NaN);
            minimumLevelPhreaticLineAtDikeTopPolder = new RoundedDouble(2, double.NaN);

            LocationInputExtreme = new MacroStabilityInwardsLocationInputExtreme();
            LocationInputDaily = new MacroStabilityInwardsLocationInputDaily();

            AdjustPhreaticLine3And4ForUplift = true;

            leakageLengthOutwardsPhreaticLine3 = new RoundedDouble(2, double.NaN);
            leakageLengthInwardsPhreaticLine3 = new RoundedDouble(2, double.NaN);
            leakageLengthOutwardsPhreaticLine4 = new RoundedDouble(2, double.NaN);
            leakageLengthInwardsPhreaticLine4 = new RoundedDouble(2, double.NaN);
            piezometricHeadPhreaticLine2Outwards = new RoundedDouble(2, double.NaN);
            piezometricHeadPhreaticLine2Inwards = new RoundedDouble(2, double.NaN);

            GridDeterminationType = MacroStabilityInwardsGridDeterminationType.Automatic;
            TangentLineDeterminationType = MacroStabilityInwardsTangentLineDeterminationType.LayerSeparated;

            tangentLineZTop = new RoundedDouble(2, properties.TangentLineZTop);
            tangentLineZBottom = new RoundedDouble(2, properties.TangentLineZBottom);
            tangentLineNumber = 1;

            LeftGrid = new MacroStabilityInwardsGrid(properties.LeftGridXLeft,
                                                     properties.LeftGridXRight,
                                                     properties.LeftGridZTop,
                                                     properties.LeftGridZBottom);
            RightGrid = new MacroStabilityInwardsGrid(properties.RightGridXLeft,
                                                      properties.RightGridXRight,
                                                      properties.RightGridZTop,
                                                      properties.RightGridZBottom);

            CreateZones = true;
            ZoningBoundariesDeterminationType = MacroStabilityInwardsZoningBoundariesDeterminationType.Automatic;
            zoneBoundaryLeft = new RoundedDouble(2, double.NaN);
            zoneBoundaryRight = new RoundedDouble(2, double.NaN);
        }

        /// <summary>
        /// Gets or sets the stochastic soil model which is linked to the <see cref="StochasticSoilProfile"/>.
        /// </summary>
        public MacroStabilityInwardsStochasticSoilModel StochasticSoilModel { get; set; }

        /// <summary>
        /// Gets or sets the profile which contains a definition of soil layers with properties.
        /// </summary>
        public MacroStabilityInwardsStochasticSoilProfile StochasticSoilProfile { get; set; }

        /// <summary>
        /// Gets or sets whether the assessment level is manual input for the calculation.
        /// </summary>
        public bool UseAssessmentLevelManualInput { get; set; }

        public HydraulicBoundaryLocation HydraulicBoundaryLocation { get; set; }

        /// <summary>
        /// Gets or sets the surface line.
        /// </summary>
        public MacroStabilityInwardsSurfaceLine SurfaceLine { get; set; }

        public override object Clone()
        {
            var clone = (MacroStabilityInwardsInput) base.Clone();
            clone.LocationInputExtreme = (IMacroStabilityInwardsLocationInputExtreme) ((MacroStabilityInwardsLocationInputExtreme) LocationInputExtreme).Clone();
            clone.LocationInputDaily = (IMacroStabilityInwardsLocationInputDaily) ((MacroStabilityInwardsLocationInputDaily) LocationInputDaily).Clone();
            clone.LeftGrid = (MacroStabilityInwardsGrid) LeftGrid.Clone();
            clone.RightGrid = (MacroStabilityInwardsGrid) RightGrid.Clone();
            return clone;
        }

        private static bool IsSmallerEqualOrNaN(double value, double valueToCompareTo)
        {
            return double.IsNaN(value) || double.IsNaN(valueToCompareTo) || value.CompareTo(valueToCompareTo + 1e-3) <= 0;
        }

        /// <summary>
        /// Container for properties for constructing a <see cref="MacroStabilityInwardsInput"/>.
        /// </summary>
        public class ConstructionProperties
        {
            /// <summary>
            /// Creates a new instance of <see cref="ConstructionProperties"/>.
            /// </summary>
            public ConstructionProperties()
            {
                LeftGridXLeft = double.NaN;
                LeftGridXRight = double.NaN;
                LeftGridZTop = double.NaN;
                LeftGridZBottom = double.NaN;

                RightGridXLeft = double.NaN;
                RightGridXRight = double.NaN;
                RightGridZTop = double.NaN;
                RightGridZBottom = double.NaN;

                TangentLineZTop = double.NaN;
                TangentLineZBottom = double.NaN;
            }

            /// <summary>
            /// Gets or sets the left boundary of the left grid.
            /// </summary>
            public double LeftGridXLeft { internal get; set; }

            /// <summary>
            /// Gets or sets the right boundary of the left grid.
            /// </summary>
            public double LeftGridXRight { internal get; set; }

            /// <summary>
            /// Gets or sets the top boundary of the left grid.
            /// </summary>
            public double LeftGridZTop { internal get; set; }

            /// <summary>
            /// Gets or sets the bottom boundary of the left grid.
            /// </summary>
            public double LeftGridZBottom { internal get; set; }

            /// <summary>
            /// Gets or sets the left boundary of the right grid.
            /// </summary>
            public double RightGridXLeft { internal get; set; }

            /// <summary>
            /// Gets or sets the right boundary of the right grid.
            /// </summary>
            public double RightGridXRight { internal get; set; }

            /// <summary>
            /// Gets or sets the top boundary of the right grid.
            /// </summary>
            public double RightGridZTop { internal get; set; }

            /// <summary>
            /// Gets or sets the bottom boundary of the right grid.
            /// </summary>
            public double RightGridZBottom { internal get; set; }

            /// <summary>
            /// Gets or sets the tangent line top boundary.
            /// </summary>
            public double TangentLineZTop { internal get; set; }

            /// <summary>
            /// Gets or sets the tangent line bottom boundary.
            /// </summary>
            public double TangentLineZBottom { internal get; set; }
        }

        #region Derived input

        /// <summary>
        /// Gets or sets the outside high water level.
        /// [m+NAP]
        /// </summary>
        /// <remarks>This property is only used for calculations when <see cref="UseAssessmentLevelManualInput"/> is <c>true</c>.</remarks>
        public RoundedDouble AssessmentLevel
        {
            get
            {
                return assessmentLevel;
            }
            set
            {
                assessmentLevel = value.ToPrecision(assessmentLevel.NumberOfDecimalPlaces);
            }
        }

        public IMacroStabilityInwardsSoilProfileUnderSurfaceLine SoilProfileUnderSurfaceLine
        {
            get
            {
                return SurfaceLine != null && StochasticSoilProfile != null
                           ? MacroStabilityInwardsSoilProfileUnderSurfaceLineFactory.Create(StochasticSoilProfile.SoilProfile, SurfaceLine)
                           : null;
            }
        }

        #endregion

        #region Settings

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
        /// Gets or sets the grid determination type.
        /// </summary>
        public MacroStabilityInwardsGridDeterminationType GridDeterminationType { get; set; }

        /// <summary>
        /// Gets or sets the tangent line determination type.
        /// </summary>
        public MacroStabilityInwardsTangentLineDeterminationType TangentLineDeterminationType { get; set; }

        /// <summary>
        /// Gets or sets the tangent line top boundary.
        /// [m+NAP]
        /// </summary>
        /// <exception cref="ArgumentException">Thrown when <paramref name="value"/> is smaller 
        /// than <see cref="TangentLineZBottom"/> and is not <see cref="double.NaN"/>.</exception>
        public RoundedDouble TangentLineZTop
        {
            get
            {
                return tangentLineZTop;
            }
            set
            {
                if (!IsSmallerEqualOrNaN(TangentLineZBottom, value))
                {
                    throw new ArgumentException(Resources.MacroStabilityInwardsInput_TangentLineZTop_should_be_larger_than_or_equal_to_TangentLineZBottom);
                }

                tangentLineZTop = value.ToPrecision(tangentLineZTop.NumberOfDecimalPlaces);
            }
        }

        /// <summary>
        /// Gets or sets the tangent line bottom boundary.
        /// [m+NAP]
        /// </summary>
        /// <exception cref="ArgumentException">Thrown when <paramref name="value"/> is larger 
        /// than <see cref="TangentLineZTop"/> and is not <see cref="double.NaN"/>.</exception>
        public RoundedDouble TangentLineZBottom
        {
            get
            {
                return tangentLineZBottom;
            }
            set
            {
                if (!IsSmallerEqualOrNaN(value, TangentLineZTop))
                {
                    throw new ArgumentException(Resources.MacroStabilityInwardsInput_TangentLineZBottom_should_be_smaller_than_or_equal_to_TangentLineZTop);
                }

                tangentLineZBottom = value.ToPrecision(tangentLineZBottom.NumberOfDecimalPlaces);
            }
        }

        /// <summary>
        /// Gets or sets the number of tangent lines.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="value"/>
        /// is not in the [1, 50] interval.</exception>
        public int TangentLineNumber
        {
            get
            {
                return tangentLineNumber;
            }
            set
            {
                if (!tangentLineNumberValidityRange.InRange(value))
                {
                    throw new ArgumentOutOfRangeException(null, string.Format(Resources.TangentLineNumber_Value_needs_to_be_in_Range_0_,
                                                                              tangentLineNumberValidityRange));
                }

                tangentLineNumber = value;
            }
        }

        /// <summary>
        /// Gets the left grid.
        /// </summary>
        public MacroStabilityInwardsGrid LeftGrid { get; private set; }

        /// <summary>
        /// Gets the right grid.
        /// </summary>
        public MacroStabilityInwardsGrid RightGrid { get; private set; }

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
        /// Gets or sets whether phreatic line 3 and 4 should be adjusted for Uplift.
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

        public IMacroStabilityInwardsLocationInputExtreme LocationInputExtreme { get; private set; }

        public IMacroStabilityInwardsLocationInputDaily LocationInputDaily { get; private set; }

        /// <summary>
        /// Gets or sets whether zones should be created.
        /// </summary>
        public bool CreateZones { get; set; }

        /// <summary>
        /// Gets or sets the zoning boundaries determination type.
        /// </summary>
        public MacroStabilityInwardsZoningBoundariesDeterminationType ZoningBoundariesDeterminationType { get; set; }

        /// <summary>
        /// Gets or sets the left zone boundary.
        /// [m]
        /// </summary>
        public RoundedDouble ZoneBoundaryLeft
        {
            get
            {
                return zoneBoundaryLeft;
            }
            set
            {
                zoneBoundaryLeft = value.ToPrecision(zoneBoundaryLeft.NumberOfDecimalPlaces);
            }
        }

        /// <summary>
        /// Gets or sets the right zone boundary.
        /// [m]
        /// </summary>
        public RoundedDouble ZoneBoundaryRight
        {
            get
            {
                return zoneBoundaryRight;
            }
            set
            {
                zoneBoundaryRight = value.ToPrecision(zoneBoundaryRight.NumberOfDecimalPlaces);
            }
        }

        #endregion
    }
}