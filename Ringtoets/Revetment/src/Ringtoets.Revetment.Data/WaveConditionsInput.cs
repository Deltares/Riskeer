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
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Revetment.Data.Properties;
using RingtoetsCommonDataResources = Ringtoets.Common.Data.Properties.Resources;

namespace Ringtoets.Revetment.Data
{
    /// <summary>
    /// Class that holds all wave conditions calculation specific input parameters.
    /// </summary>
    public class WaveConditionsInput : Observable, ICalculationInput, IUseBreakWater, IUseForeshore, IHasForeshoreProfile
    {
        private const double designWaterLevelSubstraction = 0.01;
        private const int orientationNumberOfDecimals = 2;

        private static readonly Range<RoundedDouble> orientationValidityRange = new Range<RoundedDouble>(new RoundedDouble(orientationNumberOfDecimals),
                                                                                                         new RoundedDouble(orientationNumberOfDecimals, 360));

        private ForeshoreProfile foreshoreProfile;
        private RoundedDouble upperBoundaryRevetment;
        private RoundedDouble lowerBoundaryRevetment;
        private RoundedDouble upperBoundaryWaterLevels;
        private RoundedDouble lowerBoundaryWaterLevels;
        private RoundedDouble orientation;

        /// <summary>
        /// Creates a new instance of <see cref="WaveConditionsInput"/>.
        /// </summary>
        public WaveConditionsInput()
        {
            orientation = new RoundedDouble(orientationNumberOfDecimals, double.NaN);

            upperBoundaryRevetment = new RoundedDouble(2, double.NaN);
            lowerBoundaryRevetment = new RoundedDouble(2, double.NaN);
            StepSize = WaveConditionsInputStepSize.Half;
            upperBoundaryWaterLevels = new RoundedDouble(2, double.NaN);
            lowerBoundaryWaterLevels = new RoundedDouble(2, double.NaN);

            SynchronizeForeshoreProfileParameters();
        }

        /// <summary>
        /// Gets or sets the hydraulic boundary location.
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
                return HydraulicBoundaryLocation?.DesignWaterLevel ?? new RoundedDouble(2, double.NaN);
            }
        }

        /// <summary>
        /// Gets or sets the orientation of the foreshore profile geometry with respect to North
        /// in degrees. A positive value equals a clockwise rotation.
        /// </summary>
        ///<exception cref="ArgumentOutOfRangeException">Thrown when the value of the orientation
        /// is not in the interval [0, 360].</exception>
        public RoundedDouble Orientation
        {
            get
            {
                return orientation;
            }
            set
            {
                RoundedDouble newOrientation = value.ToPrecision(orientation.NumberOfDecimalPlaces);
                if (!double.IsNaN(newOrientation) && !orientationValidityRange.InRange(newOrientation))
                {
                    throw new ArgumentOutOfRangeException(null, string.Format(RingtoetsCommonDataResources.Orientation_Value_needs_to_be_in_Range_0_,
                                                                              orientationValidityRange));
                }
                orientation = newOrientation;
            }
        }

        /// <summary>
        /// Gets the upper boundary based on <see cref="Common.Data.Hydraulics.HydraulicBoundaryLocation.DesignWaterLevel"/>.
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
        /// <remarks>When the value is smaller than -50, it will be set to -50.</remarks>
        public RoundedDouble LowerBoundaryRevetment
        {
            get
            {
                return lowerBoundaryRevetment;
            }
            set
            {
                RoundedDouble newLowerBoundaryRevetment = value.ToPrecision(lowerBoundaryRevetment.NumberOfDecimalPlaces);

                newLowerBoundaryRevetment = ValidateLowerBoundaryInRange(newLowerBoundaryRevetment);

                ValidateRevetmentBoundaries(newLowerBoundaryRevetment, UpperBoundaryRevetment);

                lowerBoundaryRevetment = newLowerBoundaryRevetment;
            }
        }

        /// <summary>
        /// Gets or sets the upper boundary of the revetment.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when value is smaller than or equal to <see cref="LowerBoundaryRevetment"/>.</exception>
        /// <remarks>When the value is larger than 1000, it will be set to 1000.</remarks>
        public RoundedDouble UpperBoundaryRevetment
        {
            get
            {
                return upperBoundaryRevetment;
            }
            set
            {
                RoundedDouble newUpperBoundaryRevetment = value.ToPrecision(upperBoundaryRevetment.NumberOfDecimalPlaces);

                newUpperBoundaryRevetment = ValidateUpperBoundaryInRange(newUpperBoundaryRevetment);

                ValidateRevetmentBoundaries(LowerBoundaryRevetment, newUpperBoundaryRevetment);

                upperBoundaryRevetment = newUpperBoundaryRevetment;
            }
        }

        /// <summary>
        /// Gets or sets the step size used for determining <see cref="WaterLevels"/>.
        /// </summary>
        public WaveConditionsInputStepSize StepSize { get; set; }

        /// <summary>
        /// Gets or sets the lower boundary of the <see cref="WaterLevels"/> range.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when value is larger than or equal to <see cref="UpperBoundaryWaterLevels"/>.</exception>
        /// <remarks>
        /// <list type="bullet">
        /// <item>Setting this property is optional when it comes to determining <see cref="WaterLevels"/>; if the value
        /// equals <see cref="double.NaN"/>, only <see cref="LowerBoundaryRevetment"/> will be taken into account.</item>
        /// <item>When the value is smaller than -50, it will be set to -50.</item>
        /// </list>
        /// </remarks>
        public RoundedDouble LowerBoundaryWaterLevels
        {
            get
            {
                return lowerBoundaryWaterLevels;
            }
            set
            {
                RoundedDouble newLowerBoundaryWaterLevels = value.ToPrecision(lowerBoundaryWaterLevels.NumberOfDecimalPlaces);

                newLowerBoundaryWaterLevels = ValidateLowerBoundaryInRange(newLowerBoundaryWaterLevels);

                ValidateWaterLevelBoundaries(newLowerBoundaryWaterLevels, UpperBoundaryWaterLevels);

                lowerBoundaryWaterLevels = newLowerBoundaryWaterLevels;
            }
        }

        /// <summary>
        /// Gets or sets the upper boundary of the <see cref="WaterLevels"/> range.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when value is smaller than or equal to <see cref="LowerBoundaryWaterLevels"/>.</exception>
        /// <remarks>
        /// <list type="bullet">
        /// <item>Setting this property is optional when it comes to determining <see cref="WaterLevels"/>; if the value
        /// equals <see cref="double.NaN"/>, only <see cref="UpperBoundaryDesignWaterLevel"/> and <see cref="UpperBoundaryRevetment"/>
        /// will be taken into account.</item>
        /// <item>When the value is larger than 1000, it will be set to 1000.</item>
        /// </list>
        /// </remarks>
        public RoundedDouble UpperBoundaryWaterLevels
        {
            get
            {
                return upperBoundaryWaterLevels;
            }
            set
            {
                RoundedDouble newUpperBoundaryWaterLevels = value.ToPrecision(upperBoundaryWaterLevels.NumberOfDecimalPlaces);

                newUpperBoundaryWaterLevels = ValidateUpperBoundaryInRange(newUpperBoundaryWaterLevels);

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
                SynchronizeForeshoreProfileParameters();
            }
        }

        public bool IsForeshoreProfileParametersSynchronized
        {
            get
            {
                if (foreshoreProfile == null)
                {
                    return false;
                }

                return
                    Orientation == foreshoreProfile.Orientation
                    && UseForeshore == foreshoreProfile.Geometry.Count() > 1
                    && UseBreakWater == foreshoreProfile.HasBreakWater
                    && BreakWater.Equals(foreshoreProfile.HasBreakWater
                                             ? foreshoreProfile.BreakWater
                                             : GetDefaultBreakWater());
            }
        }

        public bool UseBreakWater { get; set; }

        public BreakWater BreakWater { get; private set; }

        public bool UseForeshore { get; set; }

        public RoundedPoint2DCollection ForeshoreGeometry
        {
            get
            {
                return foreshoreProfile != null
                           ? foreshoreProfile.Geometry
                           : new RoundedPoint2DCollection(2, Enumerable.Empty<Point2D>());
            }
        }

        public void SynchronizeForeshoreProfileParameters()
        {
            if (foreshoreProfile == null)
            {
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

        private static RoundedDouble ValidateUpperBoundaryInRange(RoundedDouble boundary)
        {
            if (boundary > 1000)
            {
                boundary = new RoundedDouble(boundary.NumberOfDecimalPlaces, 1000);
            }
            return boundary;
        }

        private static RoundedDouble ValidateLowerBoundaryInRange(RoundedDouble boundary)
        {
            if (boundary < -50)
            {
                boundary = new RoundedDouble(boundary.NumberOfDecimalPlaces, -50);
            }
            return boundary;
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
            if (!double.IsNaN(lowerBoundary) &&
                !double.IsNaN(upperBoundary) &&
                lowerBoundary >= upperBoundary)
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

            if (double.IsNaN(upperBoundary) ||
                double.IsNaN(lowerBoundary) ||
                lowerBoundary >= upperBoundary)
            {
                return waterLevels;
            }

            waterLevels.Add(upperBoundary);

            double stepSizeValue = StepSize.AsValue();
            var currentWaterLevel = new RoundedDouble(2, Math.Ceiling(upperBoundary / stepSizeValue) * stepSizeValue - stepSizeValue);

            while (currentWaterLevel > lowerBoundary)
            {
                waterLevels.Add(currentWaterLevel);
                currentWaterLevel = new RoundedDouble(currentWaterLevel.NumberOfDecimalPlaces, currentWaterLevel - stepSizeValue);
            }

            waterLevels.Add(lowerBoundary);

            return waterLevels;
        }

        private static BreakWater GetDefaultBreakWater()
        {
            return new BreakWater(BreakWaterType.Dam, 0.0);
        }
    }
}