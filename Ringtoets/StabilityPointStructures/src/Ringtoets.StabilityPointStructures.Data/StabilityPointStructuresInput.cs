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
using System.Linq;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.HydraRing.Data;
using RingtoetsDataCommonProperties = Ringtoets.Common.Data.Properties.Resources;

namespace Ringtoets.StabilityPointStructures.Data
{
    /// <summary>
    /// Class that holds all stability point structures calculation specific input parameters.
    /// </summary>
    public class StabilityPointStructuresInput : Observable, ICalculationInput
    {
        private readonly NormalDistribution insideWaterLevelFailureConstruction;
        private readonly NormalDistribution insideWaterLevel;
        private readonly LogNormalDistribution stormDuration;
        private readonly NormalDistribution modelFactorSuperCriticalFlow;
        private readonly NormalDistribution drainCoefficient;
        private readonly NormalDistribution levelCrestStructure;
        private readonly NormalDistribution thresholdHeightOpenWeir;
        private readonly LogNormalDistribution areaFlowApertures;
        private readonly LogNormalDistribution constructiveStrengthLinearModel;
        private readonly LogNormalDistribution constructiveStrengthQuadraticModel;
        private ForeshoreProfile foreshoreProfile;
        private RoundedDouble structureNormalOrientation;
        private RoundedDouble volumicWeightWater;
        private RoundedDouble factorStormDurationOpenStructure;

        /// <summary>
        /// Creates a new instance of <see cref="StabilityPointStructuresInput"/>.
        /// </summary>
        public StabilityPointStructuresInput()
        {
            volumicWeightWater = new RoundedDouble(2, 9.81);
            structureNormalOrientation = new RoundedDouble(2);
            factorStormDurationOpenStructure = new RoundedDouble(2, double.NaN);

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
                Mean = (RoundedDouble) 6.0
            };
            stormDuration.SetStandardDeviationFromVariationCoefficient(0.25);

            modelFactorSuperCriticalFlow = new NormalDistribution(2)
            {
                Mean = (RoundedDouble) 1.1,
                StandardDeviation = (RoundedDouble) 0.03
            };

            drainCoefficient = new NormalDistribution(2)
            {
                Mean = (RoundedDouble) 1.0,
                StandardDeviation = (RoundedDouble) 0.2
            };

            levelCrestStructure = new NormalDistribution(2)
            {
                Mean = (RoundedDouble) double.NaN,
                StandardDeviation = (RoundedDouble) 0.05
            };

            thresholdHeightOpenWeir = new NormalDistribution(2)
            {
                Mean = (RoundedDouble) double.NaN,
                StandardDeviation = (RoundedDouble) 0.1
            };

            areaFlowApertures = new LogNormalDistribution(2)
            {
                Mean = (RoundedDouble) double.NaN,
                StandardDeviation = (RoundedDouble) 0.01
            };

            constructiveStrengthLinearModel = new LogNormalDistribution(2)
            {
                Mean = (RoundedDouble) double.NaN
            };
            constructiveStrengthLinearModel.SetStandardDeviationFromVariationCoefficient(0.1);

            constructiveStrengthQuadraticModel = new LogNormalDistribution(2)
            {
                Mean = (RoundedDouble) double.NaN
            };
            constructiveStrengthQuadraticModel.SetStandardDeviationFromVariationCoefficient(0.1);

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

        /// <summary>
        /// Gets or sets the model factor for super critical flow. 
        /// </summary>
        /// <remarks>Only sets the mean.</remarks>
        public NormalDistribution ModelFactorSuperCriticalFlow
        {
            get
            {
                return modelFactorSuperCriticalFlow;
            }
            set
            {
                modelFactorSuperCriticalFlow.Mean = value.Mean;
            }
        }

        /// <summary>
        /// Gets or sets the factor for the storm duration open structure.
        /// </summary>
        public RoundedDouble FactorStormDurationOpenStructure
        {
            get
            {
                return factorStormDurationOpenStructure;
            }
            set
            {
                factorStormDurationOpenStructure = value.ToPrecision(factorStormDurationOpenStructure.NumberOfDecimalPlaces);
            }
        }

        /// <summary>
        /// Gets or sets the drain coefficient.
        /// </summary>
        /// <remarks>Only sets the mean.</remarks>
        public NormalDistribution DrainCoefficient
        {
            get
            {
                return drainCoefficient;
            }
            set
            {
                drainCoefficient.Mean = value.Mean;
            }
        }

        #endregion

        #region Schematization

        /// <summary>
        /// Gets or sets the orientation of the normal of the structure.
        /// [degrees]
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the value of the orientation is not between [0, 360] degrees.</exception>
        public RoundedDouble StructureNormalOrientation
        {
            get
            {
                return structureNormalOrientation;
            }
            set
            {
                if (double.IsNaN(value))
                {
                    structureNormalOrientation = new RoundedDouble(2, double.NaN);
                    return;
                }

                RoundedDouble newOrientation = value.ToPrecision(structureNormalOrientation.NumberOfDecimalPlaces);
                if (newOrientation < 0 || newOrientation > 360)
                {
                    throw new ArgumentOutOfRangeException("value", RingtoetsDataCommonProperties.Orientation_Value_needs_to_be_between_0_and_360);
                }
                structureNormalOrientation = newOrientation;
            }
        }

        /// <summary>
        /// Gets or sets the level crest of the structure.
        /// [m+NAP]
        /// </summary>
        public NormalDistribution LevelCrestStructure
        {
            get
            {
                return levelCrestStructure;
            }
            set
            {
                levelCrestStructure.Mean = value.Mean;
                levelCrestStructure.StandardDeviation = value.StandardDeviation;
            }
        }

        /// <summary>
        /// Gets or sets the threshold height of the open weir.
        /// [m+MAP]
        /// </summary>
        public NormalDistribution ThresholdHeightOpenWeir
        {
            get
            {
                return thresholdHeightOpenWeir;
            }
            set
            {
                thresholdHeightOpenWeir.Mean = value.Mean;
                thresholdHeightOpenWeir.StandardDeviation = value.StandardDeviation;
            }
        }

        /// <summary>
        /// Gets or sets the area flow apertures
        /// [m^2]
        /// </summary>
        public LogNormalDistribution AreaFlowApertures
        {
            get
            {
                return areaFlowApertures;
            }
            set
            {
                areaFlowApertures.Mean = value.Mean;
                areaFlowApertures.StandardDeviation = value.StandardDeviation;
            }
        }

        /// <summary>
        /// Gets or sets the constructive strength of the linear load model.
        /// [kN/m^2]
        /// </summary>
        public LogNormalDistribution ConstructiveStrengthLinearLoadModel
        {
            get
            {
                return constructiveStrengthLinearModel;
            }
            set
            {
                constructiveStrengthLinearModel.Mean = value.Mean;
                constructiveStrengthLinearModel.StandardDeviation = value.StandardDeviation;
            }
        }

        /// <summary>
        /// Gets or sets the constructive strength of the quadratic load model.
        /// [kN/m]
        /// </summary>
        public LogNormalDistribution ConstructiveStrengthQuadraticLoadModel
        {
            get
            {
                return constructiveStrengthQuadraticModel;
            }
            set
            {
                constructiveStrengthQuadraticModel.Mean = value.Mean;
                constructiveStrengthQuadraticModel.StandardDeviation = value.StandardDeviation;
            }
        }

        #endregion
    }
}