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

        private DikeProfile dikeProfile;
        private RoundedDouble upperBoundaryRevetment;
        private RoundedDouble lowerBoundaryRevetment;
        private RoundedDouble stepSize;
        private RoundedDouble upperBoundaryWaterLevels;
        private RoundedDouble lowerBoundaryWaterLevels;

        /// <summary>
        /// Creates a new instance of <see cref="WaveConditionsInput"/>.
        /// </summary>
        public WaveConditionsInput()
        {
            upperBoundaryRevetment = new RoundedDouble(2, double.NaN);
            lowerBoundaryRevetment = new RoundedDouble(2, double.NaN);
            stepSize = new RoundedDouble(1, double.NaN);
            upperBoundaryWaterLevels = new RoundedDouble(2, double.NaN);
            lowerBoundaryWaterLevels = new RoundedDouble(2, double.NaN);

            UpdateDikeProfileParameters();
        }

        /// <summary>
        /// Gets or sets the dike profile.
        /// </summary>
        public DikeProfile DikeProfile
        {
            get
            {
                return dikeProfile;
            }
            set
            {
                dikeProfile = value;
                UpdateDikeProfileParameters();
            }
        }

        /// <summary>
        /// Gets or sets the hydraulic boundary location from which to use the assessment level.
        /// </summary>
        public HydraulicBoundaryLocation HydraulicBoundaryLocation { get; set; }

        /// <summary>
        /// Gets or sets if <see cref="BreakWater"/> needs to be taken into account.
        /// </summary>
        public bool UseBreakWater { get; set; }

        /// <summary>
        /// Gets the <see cref="BreakWater"/>.
        /// </summary>
        public BreakWater BreakWater { get; private set; }

        /// <summary>
        /// Gets or sets if the <see cref="ForeshoreGeometry"/> needs to be taken into account.
        /// </summary>
        public bool UseForeshore { get; set; }

        /// <summary>
        /// Gets the geometry of the foreshore.
        /// </summary>
        public RoundedPoint2DCollection ForeshoreGeometry
        {
            get
            {
                return dikeProfile != null
                           ? dikeProfile.ForeshoreGeometry
                           : new RoundedPoint2DCollection(2, Enumerable.Empty<Point2D>());
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
        public RoundedDouble StepSize
        {
            get
            {
                return stepSize;
            }
            set
            {
                var newStepSize = value.ToPrecision(stepSize.NumberOfDecimalPlaces);

                if (!double.IsNaN(newStepSize) && newStepSize <= 0)
                {
                    throw new ArgumentOutOfRangeException(null, Resources.WaveConditionsInput_StepSize_Should_be_greater_than_zero);
                }

                stepSize = newStepSize;
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
        /// equals <see cref="double.NaN"/>, only <see cref="UpperBoundaryRevetment"/> will be taken into account.
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

        private double DetermineUpperWaterLevel()
        {
            return HydraulicBoundaryLocation != null && !double.IsNaN(HydraulicBoundaryLocation.DesignWaterLevel)
                       ? HydraulicBoundaryLocation.DesignWaterLevel - designWaterLevelSubstraction
                       : double.NaN;
        }

        private IEnumerable<RoundedDouble> DetermineWaterLevels()
        {
            var waterLevels = new List<RoundedDouble>();
            var upperBoundary = new RoundedDouble(2, Math.Min(DetermineUpperWaterLevel(), Math.Min(UpperBoundaryRevetment, UpperBoundaryWaterLevels)));
            var lowerBoundary = new RoundedDouble(2, Math.Max(LowerBoundaryRevetment, LowerBoundaryWaterLevels));

            if (double.IsNaN(upperBoundary) ||
                double.IsNaN(lowerBoundary) ||
                Math.Abs(lowerBoundary - upperBoundary) < 1e-6 ||
                Math.Abs(StepSize) < 1e-6)
            {
                return waterLevels;
            }

            RoundedDouble currentWaterLevel = new RoundedDouble(2, Math.Floor(lowerBoundary/stepSize)*stepSize + stepSize);

            while (currentWaterLevel < upperBoundary)
            {
                waterLevels.Add(currentWaterLevel);
                currentWaterLevel = (currentWaterLevel + stepSize).ToPrecision(currentWaterLevel.NumberOfDecimalPlaces);
            }

            if (!waterLevels.Any())
            {
                return waterLevels;
            }

            waterLevels.Insert(0, lowerBoundary);
            waterLevels.Add(upperBoundary);

            return waterLevels;
        }

        private void UpdateDikeProfileParameters()
        {
            if (dikeProfile == null)
            {
                UseForeshore = false;
                UseBreakWater = false;
                BreakWater = GetDefaultBreakWater();
            }
            else
            {
                UseForeshore = dikeProfile.ForeshoreGeometry.Count() > 1;
                UseBreakWater = dikeProfile.HasBreakWater;
                BreakWater = dikeProfile.HasBreakWater
                                 ? new BreakWater(dikeProfile.BreakWater.Type, dikeProfile.BreakWater.Height)
                                 : GetDefaultBreakWater();
            }
        }

        private static BreakWater GetDefaultBreakWater()
        {
            return new BreakWater(BreakWaterType.Dam, 0.0);
        }
    }
}