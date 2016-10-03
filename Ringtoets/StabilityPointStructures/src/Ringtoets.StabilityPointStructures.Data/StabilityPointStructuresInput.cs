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
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.HydraRing.Data;

namespace Ringtoets.StabilityPointStructures.Data
{
    /// <summary>
    /// Class that holds all stability point structures calculation specific input parameters.
    /// </summary>
    public class StabilityPointStructuresInput : Observable, ICalculationInput
    {
        private ForeshoreProfile foreshoreProfile;
        private RoundedDouble structureNormalOrientation;
        private RoundedDouble volumicWeightWater;
        private readonly NormalDistribution insideWaterLevelFailureConstruction;
        private readonly NormalDistribution insideWaterLevel;
        private readonly LogNormalDistribution stormDuration;

        /// <summary>
        /// Creates a new instance of <see cref="StabilityPointStructuresInput"/>.
        /// </summary>
        public StabilityPointStructuresInput()
        {
            volumicWeightWater = new RoundedDouble(2, 9.81);
            structureNormalOrientation = new RoundedDouble(2);

            insideWaterLevelFailureConstruction = new NormalDistribution(2)
            {
                Mean = (RoundedDouble) double.NaN,
                StandardDeviation = (RoundedDouble) 0.1
            };

            insideWaterLevel = new NormalDistribution(2)
            {
                Mean = (RoundedDouble) double.NaN,
                StandardDeviation = (RoundedDouble) 0.1
            };

            stormDuration = new LogNormalDistribution(2)
            {
                Mean = (RoundedDouble) 7.5,
            };
            stormDuration.SetStandardDeviationFromVariationCoefficient(0.25);

            UpdateForeshoreProperties();
        }

        #region Structure properties

        /// <summary>
        /// Gets or sets the stability point structure.
        /// </summary>
        public StabilityPointStructure StabilityPointStructure { get; set; }

        #endregion

        #region Hydraulic data and loads

        /// <summary>
        /// Gets or sets the hydraulic boundary location from which to use the assessment level.
        /// </summary>
        public HydraulicBoundaryLocation HydraulicBoundaryLocation { get; set; }

        #region Foreshore profile

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
                UpdateForeshoreProperties();
            }
        }

        /// <summary>
        /// Gets or sets whether the <see cref="BreakWater"/> needs to be taken into account.
        /// </summary>
        public bool UseBreakWater { get; set; }

        /// <summary>
        /// Gets or sets whether the <see cref="ForeshoreProfile"/> needs to be taken into account.
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
        /// Gets the <see cref="BreakWater"/>.
        /// </summary>
        public BreakWater BreakWater { get; private set; }

        private void UpdateForeshoreProperties()
        {
            if (foreshoreProfile == null)
            {
                UseForeshore = false;
                UseBreakWater = false;
                BreakWater = GetDefaultBreakWaterProperties();
            }
            else
            {
                UseForeshore = foreshoreProfile.Geometry.Count() > 1;
                UseBreakWater = foreshoreProfile.HasBreakWater;
                BreakWater = foreshoreProfile.HasBreakWater ?
                                 new BreakWater(foreshoreProfile.BreakWater.Type, foreshoreProfile.BreakWater.Height) :
                                 GetDefaultBreakWaterProperties();
            }
        }

        private BreakWater GetDefaultBreakWaterProperties()
        {
            return new BreakWater(BreakWaterType.Dam, 0.0);
        }

        #endregion

        /// <summary>
        /// Gets or sets the volumic weight of water.
        /// </summary>
        public RoundedDouble VolumicWeightWater
        {
            get
            {
                return volumicWeightWater;
            }
            set
            {
                volumicWeightWater = value.ToPrecision(volumicWeightWater.NumberOfDecimalPlaces);
            }
        }

        /// <summary>
        /// Gets or sets the inside water level failure construction.
        /// </summary>
        public NormalDistribution InsideWaterLevelFailureConstruction
        {
            get
            {
                return insideWaterLevelFailureConstruction;
            }
            set
            {
                insideWaterLevelFailureConstruction.Mean = value.Mean;
                insideWaterLevelFailureConstruction.StandardDeviation = value.StandardDeviation;
            }
        }

        /// <summary>
        /// Gets or sets the inside water level.
        /// </summary>
        public NormalDistribution InsideWaterLevel
        {
            get
            {
                return insideWaterLevel;
            }
            set
            {
                insideWaterLevel.Mean = value.Mean;
                insideWaterLevel.StandardDeviation = value.StandardDeviation;
            }
        }

        /// <summary>
        /// Gets or sets the storm duration.
        /// </summary>
        /// <remarks>Only sets the mean.</remarks>
        public LogNormalDistribution StormDuration
        {
            get
            {
                return stormDuration;
            }
            set
            {
                stormDuration.Mean = value.Mean;
            }
        }

        #endregion

        #region Model Inputs

        #endregion

        #region Schematization

        #region Orientation

        /// <summary>
        /// Gets or sets the orientation of the normal of the structure.
        /// </summary>
        /// <remarks><list type="bullet">
        /// <item>When the value is smaller than 0, it will be set to 0.</item>
        /// <item>When the value is larger than 360, it will be set to 360.</item>
        /// </list></remarks>
        public RoundedDouble StructureNormalOrientation
        {
            get
            {
                return structureNormalOrientation;
            }
            set
            {
                RoundedDouble newOrientationValue = value.ToPrecision(structureNormalOrientation.NumberOfDecimalPlaces);
                newOrientationValue = ValidateStructureNormalOrientationInRange(newOrientationValue);

                structureNormalOrientation = newOrientationValue;
            }
        }

        private RoundedDouble ValidateStructureNormalOrientationInRange(RoundedDouble newOrientationValue)
        {
            const double upperBoundaryRange = 360;
            const double lowerBoundaryRange = 0.0;

            if (newOrientationValue > upperBoundaryRange)
            {
                newOrientationValue = new RoundedDouble(2, upperBoundaryRange);
            }
            else if (newOrientationValue < lowerBoundaryRange)
            {
                newOrientationValue = new RoundedDouble(2, lowerBoundaryRange);
            }

            return newOrientationValue;
        }

        #endregion

        #endregion
    }
}