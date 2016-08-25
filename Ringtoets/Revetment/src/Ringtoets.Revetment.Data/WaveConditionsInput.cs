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

using System.Linq;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.HydraRing.Data;

namespace Ringtoets.Revetment.Data
{
    /// <summary>
    /// Class that holds all wave conditions calculation specific input paramaters.
    /// </summary>
    public class WaveConditionsInput
    {
        private DikeProfile dikeProfile;
        private RoundedDouble upperLevel;
        private RoundedDouble lowerLevel;
        private RoundedDouble stepSize;

        /// <summary>
        /// Creates a new instance of <see cref="WaveConditionsInput"/>.
        /// </summary>
        public WaveConditionsInput()
        {
            upperLevel = new RoundedDouble(2);
            lowerLevel = new RoundedDouble(2);
            stepSize = new RoundedDouble(1);

            UpdateProfileParameters();
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
                UpdateProfileParameters();
            }
        }

        /// <summary>
        /// Gets or set the hydraulic boundary location from which to use the assessment level.
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

        public RoundedDouble UpperLevel
        {
            get
            {
                return upperLevel;
            }
            set
            {
                upperLevel = value.ToPrecision(upperLevel.NumberOfDecimalPlaces);
            }
        }

        public RoundedDouble LowerLevel
        {
            get
            {
                return lowerLevel;
            }
            set
            {
                lowerLevel = value.ToPrecision(lowerLevel.NumberOfDecimalPlaces);
            }
        }

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

        private void UpdateProfileParameters()
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