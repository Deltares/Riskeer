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

using System;
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
    /// Abstract class that holds all generic wave conditions calculation input parameters.
    /// </summary>
    public abstract class WaveConditionsInput : CloneableObservable, ICalculationInputWithHydraulicBoundaryLocation, IUseBreakWater, IUseForeshore, IHasForeshoreProfile
    {
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
        protected WaveConditionsInput()
        {
            orientation = new RoundedDouble(orientationNumberOfDecimals, double.NaN);

            upperBoundaryRevetment = new RoundedDouble(2, double.NaN);
            lowerBoundaryRevetment = new RoundedDouble(2, double.NaN);
            StepSize = WaveConditionsInputStepSize.Half;
            upperBoundaryWaterLevels = new RoundedDouble(2, double.NaN);
            lowerBoundaryWaterLevels = new RoundedDouble(2, double.NaN);

            SynchronizeForeshoreProfileInput();
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
        /// Gets or sets the step size used for determining water levels.
        /// </summary>
        public WaveConditionsInputStepSize StepSize { get; set; }

        /// <summary>
        /// Gets or sets the lower boundary of the water levels range.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when value is larger than or equal to <see cref="UpperBoundaryWaterLevels"/>.</exception>
        /// <remarks>
        /// <list type="bullet">
        /// <item>Setting this property is optional when it comes to determining water levels; if the value
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
        /// Gets or sets the upper boundary of the water levels range.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when value is smaller than or equal to <see cref="LowerBoundaryWaterLevels"/>.</exception>
        /// <remarks>
        /// <list type="bullet">
        /// <item>Setting this property is optional when it comes to determining water levels; if the value
        /// equals <see cref="double.NaN"/>, only the upper boundary design water level and <see cref="UpperBoundaryRevetment"/>
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

        public HydraulicBoundaryLocation HydraulicBoundaryLocation { get; set; }

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
                SynchronizeForeshoreProfileInput();
            }
        }

        public bool IsForeshoreProfileInputSynchronized
        {
            get
            {
                return foreshoreProfile != null
                       && Orientation == foreshoreProfile.Orientation
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

        public override object Clone()
        {
            var clone = (WaveConditionsInput) base.Clone();

            clone.BreakWater = (BreakWater) BreakWater.Clone();

            return clone;
        }

        public void SynchronizeForeshoreProfileInput()
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

        private static BreakWater GetDefaultBreakWater()
        {
            return new BreakWater(BreakWaterType.Dam, 0.0);
        }
    }
}