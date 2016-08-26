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
        private readonly RoundedDouble designWaterLevelSubstraction;
        private readonly List<RoundedDouble> waterLevels;

        private DikeProfile dikeProfile;
        private HydraulicBoundaryLocation hydraulicBoundaryLocation;
        private RoundedDouble upperRevetmentLevel;
        private RoundedDouble lowerRevetmentLevel;
        private RoundedDouble lowerWaterLevel;
        private RoundedDouble upperWaterLevel;
        private RoundedDouble stepSize;

        private RoundedDouble upperBoundary;
        private RoundedDouble lowerBoundary;

        /// <summary>
        /// Creates a new instance of <see cref="WaveConditionsInput"/>.
        /// </summary>
        public WaveConditionsInput()
        {
            upperRevetmentLevel = new RoundedDouble(2);
            lowerRevetmentLevel = new RoundedDouble(2);
            lowerWaterLevel = new RoundedDouble(2);
            upperWaterLevel = new RoundedDouble(2);
            stepSize = new RoundedDouble(1);

            upperBoundary = new RoundedDouble(2);
            lowerBoundary = new RoundedDouble(2);

            designWaterLevelSubstraction = new RoundedDouble(2, 0.01);

            waterLevels = new List<RoundedDouble>();

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
                DetermineBoundaries();
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
                DetermineBoundaries();
            }
        }

        /// <summary>
        /// Gets or sets the lower water level.
        /// </summary>
        public RoundedDouble LowerWaterLevel
        {
            get
            {
                return lowerWaterLevel;
            }
            set
            {
                lowerWaterLevel = value.ToPrecision(lowerWaterLevel.NumberOfDecimalPlaces);
                DetermineBoundaries();
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
                DetermineWaterLevels();
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
                DetermineBoundaries();
            }
        }

        /// <summary>
        /// Gets the upper boundary of the waterlevels that should be calculated..
        /// </summary>
        public RoundedDouble UpperBoundary
        {
            get
            {
                return upperBoundary;
            }
            private set
            {
                upperBoundary = value.ToPrecision(upperBoundary.NumberOfDecimalPlaces);
            }
        }

        /// <summary>
        /// Gets the lower boundary of the waterlevels that should be calculated..
        /// </summary>
        public RoundedDouble LowerBoundary
        {
            get
            {
                return lowerBoundary;
            }
            private set
            {
                lowerBoundary = value.ToPrecision(lowerBoundary.NumberOfDecimalPlaces);
            }
        }

        /// <summary>
        /// Gets the water levels to calculate for.
        /// </summary>
        public IEnumerable<RoundedDouble> WaterLevels
        {
            get
            {
                return waterLevels;
            }
        }

        private void UpdateUpperWaterLevel()
        {
            if (hydraulicBoundaryLocation != null && !double.IsNaN(hydraulicBoundaryLocation.DesignWaterLevel))
            {
                UpperWaterLevel = hydraulicBoundaryLocation.DesignWaterLevel - designWaterLevelSubstraction;
            }
            else
            {
                UpperWaterLevel = (RoundedDouble) 0;
            }
        }

        private void DetermineBoundaries()
        {
            UpperBoundary = UpperWaterLevel < UpperRevetmentLevel ? UpperWaterLevel : UpperRevetmentLevel;
            LowerBoundary = LowerWaterLevel > LowerRevetmentLevel ? LowerWaterLevel : LowerRevetmentLevel;

            DetermineWaterLevels();
        }

        private void DetermineWaterLevels()
        {
            waterLevels.Clear();
            
            if (double.IsNaN(UpperBoundary) || 
                double.IsNaN(LowerBoundary) || 
                Math.Abs(LowerBoundary - UpperBoundary) < 1e-6 ||
                Math.Abs(StepSize) < 1e-6)
            {
                return;
            }

            waterLevels.Add(LowerBoundary);

            RoundedDouble currentWaterLevel = new RoundedDouble(2, Math.Floor(lowerBoundary / stepSize) * stepSize + stepSize);

            while (currentWaterLevel < UpperBoundary)
            {
                waterLevels.Add(currentWaterLevel);
                currentWaterLevel = (currentWaterLevel + stepSize).ToPrecision(currentWaterLevel.NumberOfDecimalPlaces);
            }

            waterLevels.Add(upperBoundary);
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