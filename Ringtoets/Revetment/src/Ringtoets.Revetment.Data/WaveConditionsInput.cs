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
        private RoundedDouble upperRevetmentLevel;
        private RoundedDouble lowerRevetmentLevel;
        private RoundedDouble stepSize;
        private RoundedDouble upperBoundaryCalculatorSeries;
        private RoundedDouble lowerBoundaryCalculatorSeries;

        /// <summary>
        /// Creates a new instance of <see cref="WaveConditionsInput"/>.
        /// </summary>
        public WaveConditionsInput()
        {
            upperRevetmentLevel = new RoundedDouble(2, double.NaN);
            lowerRevetmentLevel = new RoundedDouble(2, double.NaN);
            stepSize = new RoundedDouble(1, double.NaN);
            upperBoundaryCalculatorSeries = new RoundedDouble(2, double.NaN);
            lowerBoundaryCalculatorSeries = new RoundedDouble(2, double.NaN);

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
        /// Gets or sets the upper level of the revetment.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when value is smaller than or equal to <see cref="LowerRevetmentLevel"/>.</exception>
        public RoundedDouble UpperRevetmentLevel
        {
            get
            {
                return upperRevetmentLevel;
            }
            set
            {
                var newUpperRevetmentLevel = value.ToPrecision(upperRevetmentLevel.NumberOfDecimalPlaces);

                ValidateRevetmentLevels(LowerRevetmentLevel, newUpperRevetmentLevel);

                upperRevetmentLevel = newUpperRevetmentLevel;
            }
        }

        /// <summary>
        /// Gets or sets the lower level of the revetment.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when value is larger than or equal to <see cref="UpperRevetmentLevel"/>.</exception>
        public RoundedDouble LowerRevetmentLevel
        {
            get
            {
                return lowerRevetmentLevel;
            }
            set
            {
                var newLowerRevetmentLevel = value.ToPrecision(lowerRevetmentLevel.NumberOfDecimalPlaces);

                ValidateRevetmentLevels(newLowerRevetmentLevel, UpperRevetmentLevel);

                lowerRevetmentLevel = newLowerRevetmentLevel;
            }
        }

        /// <summary>
        /// Gets or sets the step size for wave conditions calculations.
        /// </summary>
        public RoundedDouble StepSize
        {
            get
            {
                return stepSize;
            }
            set
            {
                stepSize = value.ToPrecision(stepSize.NumberOfDecimalPlaces);
            }
        }

        /// <summary>
        /// Gets or sets the upper boundary for the calculator series.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when value is smaller than or equal to <see cref="LowerBoundaryCalculatorSeries"/>.</exception>
        public RoundedDouble UpperBoundaryCalculatorSeries
        {
            get
            {
                return upperBoundaryCalculatorSeries;
            }
            set
            {
                var newUpperBoundaryCalculatorSeries = value.ToPrecision(upperBoundaryCalculatorSeries.NumberOfDecimalPlaces);

                ValidateCalculatorSeriesBoundaries(LowerBoundaryCalculatorSeries, newUpperBoundaryCalculatorSeries);

                upperBoundaryCalculatorSeries = newUpperBoundaryCalculatorSeries;
            }
        }

        /// <summary>
        /// Gets or sets the lower boundary for the calculator series.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when value is larger than or equal to <see cref="UpperBoundaryCalculatorSeries"/>.</exception>
        public RoundedDouble LowerBoundaryCalculatorSeries
        {
            get
            {
                return lowerBoundaryCalculatorSeries;
            }
            set
            {
                var newLowerBoundaryCalculatorSeries = value.ToPrecision(lowerBoundaryCalculatorSeries.NumberOfDecimalPlaces);

                ValidateCalculatorSeriesBoundaries(newLowerBoundaryCalculatorSeries, UpperBoundaryCalculatorSeries);

                lowerBoundaryCalculatorSeries = newLowerBoundaryCalculatorSeries;
            }
        }

        /// <summary>
        /// Gets the water levels to calculate for.
        /// </summary>
        public IEnumerable<RoundedDouble> WaterLevels
        {
            get
            {
                return DetermineWaterLevels();
            }
        }

        private static void ValidateRevetmentLevels(RoundedDouble lowerRevetmentLevelValue, RoundedDouble upperRevetmentLevelValue)
        {
            if (!double.IsNaN(lowerRevetmentLevelValue)
                && !double.IsNaN(upperRevetmentLevelValue)
                && lowerRevetmentLevelValue >= upperRevetmentLevelValue)
            {
                throw new ArgumentOutOfRangeException(null, Resources.WaveConditionsInput_ValidateRevetmentLevels_Upper_revetment_level_must_be_above_lower_revetment_level);
            }
        }

        private static void ValidateCalculatorSeriesBoundaries(RoundedDouble lowerBoundary, RoundedDouble upperBoundary)
        {
            if (!double.IsNaN(lowerBoundary)
                && !double.IsNaN(upperBoundary)
                && lowerBoundary >= upperBoundary)
            {
                throw new ArgumentOutOfRangeException(null, Resources.WaveConditionsInput_ValidateCalculatorSeriesBoundaries_Calculator_series_upperboundary_must_be_above_lowerboundary);
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
            var upperBoundary = new RoundedDouble(2, Math.Min(DetermineUpperWaterLevel(), Math.Min(UpperRevetmentLevel, UpperBoundaryCalculatorSeries)));
            var lowerBoundary = new RoundedDouble(2, Math.Max(LowerRevetmentLevel, LowerBoundaryCalculatorSeries));

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