// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.HydraRing.Data;
using Ringtoets.Revetment.Data.Properties;

namespace Ringtoets.Revetment.Data
{
    /// <summary>
    /// Class that holds all wave conditions calculation specific input parameters.
    /// </summary>
    public class WaveConditionsInput : Observable, ICalculationInput
    {
        private const double designWaterLevelSubstraction = 0.01;

        private ForeshoreProfile foreshoreProfile;
        private RoundedDouble upperBoundaryRevetment;
        private RoundedDouble lowerBoundaryRevetment;
        private WaveConditionsInputStepSize stepSize;
        private RoundedDouble upperBoundaryWaterLevels;
        private RoundedDouble lowerBoundaryWaterLevels;
        private RoundedDouble orientation;

        /// <summary>
        /// Creates a new instance of <see cref="WaveConditionsInput"/>.
        /// </summary>
        public WaveConditionsInput()
        {
            orientation = new RoundedDouble(2);

            upperBoundaryRevetment = new RoundedDouble(2, double.NaN);
            lowerBoundaryRevetment = new RoundedDouble(2, double.NaN);
            stepSize = WaveConditionsInputStepSize.Half;
            upperBoundaryWaterLevels = new RoundedDouble(2, double.NaN);
            lowerBoundaryWaterLevels = new RoundedDouble(2, double.NaN);

            UpdateForeshoreProfileParameters();
        }

        /// <summary>
        /// Gets or sets the hydraulic boundary location from which to use the assessment level.
        /// </summary>
        public HydraulicBoundaryLocation HydraulicBoundaryLocation { get; set; }

        /// <summary>
        /// Gets the assessment level from the current hydraulic boundary location, or <see cref="double.NaN"/> if there is no
        /// location selected.
        /// </summary>
        public RoundedDouble AssessmentLevel
        {
            get
            {
                return HydraulicBoundaryLocation != null ? HydraulicBoundaryLocation.DesignWaterLevel : new RoundedDouble(2, double.NaN);
            }
        }

        /// <summary>
        /// Gets or sets the foreshore profile.
        /// </summary>
        public ForeshoreProfile ForeshoreProfile
        {
            get
            {
                return foreshoreProfile;
            }
            set
            {
                foreshoreProfile = value;
                UpdateForeshoreProfileParameters();
            }
        }

        /// <summary>
        /// Gets or sets the orientation of the foreshore profile geometry with respect to North
        /// in degrees. A positive value equals a clockwise rotation.
        /// </summary>
        public RoundedDouble Orientation
        {
            get
            {
                return orientation;
            }
            set
            {
                orientation = value.ToPrecision(orientation.NumberOfDecimalPlaces);
            }
        }

        /// <summary>
        /// Gets or sets whether <see cref="BreakWater"/> needs to be taken into account.
        /// </summary>
        public bool UseBreakWater { get; set; }

        /// <summary>
        /// Gets the <see cref="BreakWater"/>.
        /// </summary>
        public BreakWater BreakWater { get; private set; }

        /// <summary>
        /// Gets or sets whether the <see cref="ForeshoreGeometry"/> needs to be taken into account.
        /// </summary>
        public bool UseForeshore { get; set; }

        /// <summary>
        /// Gets the geometry of the foreshore.
        /// </summary>
        public RoundedPoint2DCollection ForeshoreGeometry
        {
            get
            {
                return foreshoreProfile != null
                           ? foreshoreProfile.Geometry
                           : new RoundedPoint2DCollection(2, Enumerable.Empty<Point2D>());
            }
        }

        /// <summary>
        /// Gets the upper boundary based on <see cref="HydraRing.Data.HydraulicBoundaryLocation.DesignWaterLevel"/>.
        /// </summary>
        public RoundedDouble UpperBoundaryDesignWaterLevel
        {
            get
            {
                return new RoundedDouble(2, AssessmentLevel - designWaterLevelSubstraction);
            }
        }

        /// <summary>
        /// Gets or sets the lower boundary of the revetment.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when value is larger than or equal to <see cref="UpperBoundaryRevetment"/>.</exception>
        public RoundedDouble LowerBoundaryRevetment
        {
            get
            {
                return lowerBoundaryRevetment;
            }
            set
            {
                var newLowerBoundaryRevetment = value.ToPrecision(lowerBoundaryRevetment.NumberOfDecimalPlaces);

                ValidateRevetmentBoundaries(newLowerBoundaryRevetment, UpperBoundaryRevetment);

                lowerBoundaryRevetment = newLowerBoundaryRevetment;
            }
        }

        /// <summary>
        /// Gets or sets the upper boundary of the revetment.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when value is smaller than or equal to <see cref="LowerBoundaryRevetment"/>.</exception>
        public RoundedDouble UpperBoundaryRevetment
        {
            get
            {
                return upperBoundaryRevetment;
            }
            set
            {
                var newUpperBoundaryRevetment = value.ToPrecision(upperBoundaryRevetment.NumberOfDecimalPlaces);

                ValidateRevetmentBoundaries(LowerBoundaryRevetment, newUpperBoundaryRevetment);

                upperBoundaryRevetment = newUpperBoundaryRevetment;
            }
        }

        /// <summary>
        /// Gets or sets the step size used for determining <see cref="WaterLevels"/>.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when value is smaller than or equal to <c>0</c>.</exception>
        public WaveConditionsInputStepSize StepSize
        {
            get
            {
                return stepSize;
            }
            set
            {
                stepSize = value;
            }
        }

        /// <summary>
        /// Gets or sets the lower boundary of the <see cref="WaterLevels"/> range.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when value is larger than or equal to <see cref="UpperBoundaryWaterLevels"/>.</exception>
        /// <remarks>
        /// Setting this property is optional when it comes to determining <see cref="WaterLevels"/>; if the value
        /// equals <see cref="double.NaN"/>, only <see cref="LowerBoundaryRevetment"/> will be taken into account.
        /// </remarks>
        public RoundedDouble LowerBoundaryWaterLevels
        {
            get
            {
                return lowerBoundaryWaterLevels;
            }
            set
            {
                var newLowerBoundaryWaterLevels = value.ToPrecision(lowerBoundaryWaterLevels.NumberOfDecimalPlaces);

                ValidateWaterLevelBoundaries(newLowerBoundaryWaterLevels, UpperBoundaryWaterLevels);

                lowerBoundaryWaterLevels = newLowerBoundaryWaterLevels;
            }
        }

        /// <summary>
        /// Gets or sets the upper boundary of the <see cref="WaterLevels"/> range.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when value is smaller than or equal to <see cref="LowerBoundaryWaterLevels"/>.</exception>
        /// <remarks>
        /// Setting this property is optional when it comes to determining <see cref="WaterLevels"/>; if the value
        /// equals <see cref="double.NaN"/>, only <see cref="UpperBoundaryDesignWaterLevel"/> and <see cref="UpperBoundaryRevetment"/>
        /// will be taken into account.
        /// </remarks>
        public RoundedDouble UpperBoundaryWaterLevels
        {
            get
            {
                return upperBoundaryWaterLevels;
            }
            set
            {
                var newUpperBoundaryWaterLevels = value.ToPrecision(upperBoundaryWaterLevels.NumberOfDecimalPlaces);

                ValidateWaterLevelBoundaries(LowerBoundaryWaterLevels, newUpperBoundaryWaterLevels);

                upperBoundaryWaterLevels = newUpperBoundaryWaterLevels;
            }
        }

        /// <summary>
        /// Gets the water levels to perform a wave conditions calculation for.
        /// </summary>
        public IEnumerable<RoundedDouble> WaterLevels
        {
            get
            {
                return DetermineWaterLevels();
            }
        }

        private static void ValidateRevetmentBoundaries(RoundedDouble lowerBoundary, RoundedDouble upperBoundary)
        {
            ValidateBoundaries(lowerBoundary, upperBoundary, Resources.WaveConditionsInput_ValidateRevetmentBoundaries_Upper_boundary_revetment_must_be_above_lower_boundary_revetment);
        }

        private static void ValidateWaterLevelBoundaries(RoundedDouble lowerBoundary, RoundedDouble upperBoundary)
        {
            ValidateBoundaries(lowerBoundary, upperBoundary, Resources.WaveConditionsInput_ValidateWaterLevelBoundaries_Upper_boundary_water_levels_must_be_above_lower_boundary_water_levels);
        }

        private static void ValidateBoundaries(RoundedDouble lowerBoundary, RoundedDouble upperBoundary, string exceptionMessage)
        {
            if (!double.IsNaN(lowerBoundary)
                && !double.IsNaN(upperBoundary)
                && lowerBoundary >= upperBoundary)
            {
                throw new ArgumentOutOfRangeException(null, exceptionMessage);
            }
        }

        private IEnumerable<RoundedDouble> DetermineWaterLevels()
        {
            var waterLevels = new List<RoundedDouble>();

            var upperBoundary = new RoundedDouble(2, Math.Min(UpperBoundaryDesignWaterLevel,
                                                              Math.Min(UpperBoundaryRevetment,
                                                                       !double.IsNaN(UpperBoundaryWaterLevels)
                                                                           ? UpperBoundaryWaterLevels
                                                                           : double.MaxValue)));

            var lowerBoundary = new RoundedDouble(2, Math.Max(LowerBoundaryRevetment,
                                                              !double.IsNaN(LowerBoundaryWaterLevels)
                                                                  ? LowerBoundaryWaterLevels
                                                                  : double.MinValue));

            if (double.IsNaN(upperBoundary)
                || double.IsNaN(lowerBoundary))
            {
                return waterLevels;
            }

            waterLevels.Add(lowerBoundary);

            double stepSizeValue = stepSize.AsValue();
            RoundedDouble currentWaterLevel = new RoundedDouble(2, Math.Floor(lowerBoundary/stepSizeValue)*stepSizeValue + stepSizeValue);

            while (currentWaterLevel < upperBoundary)
            {
                waterLevels.Add(currentWaterLevel);
                currentWaterLevel = new RoundedDouble(currentWaterLevel.NumberOfDecimalPlaces, currentWaterLevel + stepSizeValue);
            }

            waterLevels.Add(upperBoundary);

            return waterLevels;
        }

        private void UpdateForeshoreProfileParameters()
        {
            if (foreshoreProfile == null)
            {
                Orientation = (RoundedDouble)0.0;
                UseForeshore = false;
                UseBreakWater = false;
                BreakWater = GetDefaultBreakWater();
            }
            else
            {
                Orientation = foreshoreProfile.Orientation;
                UseForeshore = foreshoreProfile.Geometry.Count() > 1;
                UseBreakWater = foreshoreProfile.HasBreakWater;
                BreakWater = foreshoreProfile.HasBreakWater
                                 ? new BreakWater(foreshoreProfile.BreakWater.Type, foreshoreProfile.BreakWater.Height)
                                 : GetDefaultBreakWater();
            }
        }

        private static BreakWater GetDefaultBreakWater()
        {
            return new BreakWater(BreakWaterType.Dam, 0.0);
        }
    }
}