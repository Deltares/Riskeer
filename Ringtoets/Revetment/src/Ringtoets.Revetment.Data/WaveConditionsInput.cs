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

namespace Ringtoets.Revetment.Data
{
    /// <summary>
    /// Class that holds all wave conditions calculation specific input paramaters.
    /// </summary>
    public class WaveConditionsInput : Observable, ICalculationInput
    {
        private const double designWaterLevelSubstraction = 0.01;

        private DikeProfile dikeProfile;
        private HydraulicBoundaryLocation hydraulicBoundaryLocation;
        private RoundedDouble upperRevetmentLevel;
        private RoundedDouble lowerRevetmentLevel;
        private RoundedDouble upperWaterLevel;
        private RoundedDouble stepSize;
        private RoundedDouble upperBoundaryCalculatorSeries;
        private RoundedDouble lowerBoundaryCalculatorSeries;

        /// <summary>
        /// Creates a new instance of <see cref="WaveConditionsInput"/>.
        /// </summary>
        public WaveConditionsInput()
        {
            upperRevetmentLevel = new RoundedDouble(2);
            lowerRevetmentLevel = new RoundedDouble(2);
            upperWaterLevel = new RoundedDouble(2);
            stepSize = new RoundedDouble(1);

            upperBoundaryCalculatorSeries = new RoundedDouble(2);
            lowerBoundaryCalculatorSeries = new RoundedDouble(2);

            UpdateDikeProfileParameters();
        }

        /// <summary>
        /// Gets or set the dike profile.
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
        /// Gets or set the hydraulic boundary location from which to use the assessment level.
        /// </summary>
        public HydraulicBoundaryLocation HydraulicBoundaryLocation
        {
            get
            {
                return hydraulicBoundaryLocation;
            }
            set
            {
                hydraulicBoundaryLocation = value;
                UpdateUpperWaterLevel();
            }
        }

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
        public RoundedDouble UpperRevetmentLevel
        {
            get
            {
                return upperRevetmentLevel;
            }
            set
            {
                upperRevetmentLevel = value.ToPrecision(upperRevetmentLevel.NumberOfDecimalPlaces);
            }
        }

        /// <summary>
        /// Gets or sets the lower level of the revetment.
        /// </summary>
        public RoundedDouble LowerRevetmentLevel
        {
            get
            {
                return lowerRevetmentLevel;
            }
            set
            {
                lowerRevetmentLevel = value.ToPrecision(lowerRevetmentLevel.NumberOfDecimalPlaces);
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
        /// Gets the upper water level.
        /// </summary>
        public RoundedDouble UpperWaterLevel
        {
            get
            {
                return upperWaterLevel;
            }
            private set
            {
                upperWaterLevel = value.ToPrecision(upperWaterLevel.NumberOfDecimalPlaces);
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

        /// <summary>
        /// Gets or sets the upper boundary for the calculator series.
        /// </summary>
        public RoundedDouble UpperBoundaryCalculatorSeries
        {
            get
            {
                return upperBoundaryCalculatorSeries;
            }
            set
            {
                upperBoundaryCalculatorSeries = value.ToPrecision(upperBoundaryCalculatorSeries.NumberOfDecimalPlaces);
            }
        }

        /// <summary>
        /// Gets or sets the lower boundary for the calculator series.
        /// </summary>
        public RoundedDouble LowerBoundaryCalculatorSeries
        {
            get
            {
                return lowerBoundaryCalculatorSeries;
            }
            set
            {
                lowerBoundaryCalculatorSeries = value.ToPrecision(lowerBoundaryCalculatorSeries.NumberOfDecimalPlaces);
            }
        }

        private void UpdateUpperWaterLevel()
        {
            if (hydraulicBoundaryLocation != null && !double.IsNaN(hydraulicBoundaryLocation.DesignWaterLevel))
            {
                UpperWaterLevel = (RoundedDouble) (hydraulicBoundaryLocation.DesignWaterLevel - designWaterLevelSubstraction);
            }
            else
            {
                UpperWaterLevel = (RoundedDouble) 0;
            }
        }

        private IEnumerable<RoundedDouble> DetermineWaterLevels()
        {
            RoundedDouble upperBoundary;

            if (UpperWaterLevel < UpperRevetmentLevel)
            {
                upperBoundary = UpperWaterLevel < UpperBoundaryCalculatorSeries ? UpperWaterLevel : upperBoundaryCalculatorSeries;
            }
            else
            {
                upperBoundary = UpperRevetmentLevel < UpperBoundaryCalculatorSeries ? upperRevetmentLevel : upperBoundaryCalculatorSeries;
            }

            var lowerBoundary = LowerBoundaryCalculatorSeries > LowerRevetmentLevel ? LowerBoundaryCalculatorSeries : LowerRevetmentLevel;

            var waterLevels = new List<RoundedDouble>();

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

            if(!waterLevels.Any())
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