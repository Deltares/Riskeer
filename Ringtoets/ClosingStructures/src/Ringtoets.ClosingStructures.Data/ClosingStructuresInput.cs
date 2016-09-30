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
using RingtoetsCommonDataResources = Ringtoets.Common.Data.Properties.Resources;

namespace Ringtoets.ClosingStructures.Data
{
    public class ClosingStructuresInput : Observable, ICalculationInput
    {
        private readonly NormalDistribution modelFactorSuperCriticalFlow;
        private readonly NormalDistribution thresholdHeightOpenWeir;
        private readonly NormalDistribution drainCoefficient;
        private readonly LogNormalDistribution areaFlowApertures;
        private readonly NormalDistribution levelCrestStructureNotClosing;
        private readonly NormalDistribution insideWaterLevel;
        private readonly LogNormalDistribution storageStructureArea;
        private readonly LogNormalDistribution allowedLevelIncreaseStorage;
        private readonly LogNormalDistribution flowWidthAtBottomProtection;
        private readonly NormalDistribution widthFlowApertures;
        private readonly LogNormalDistribution stormDuration;
        private readonly LogNormalDistribution criticalOverToppingDischarge;
        private RoundedDouble structureNormalOrientation;
        private RoundedDouble factorStormDurationOpenStructure;
        private double failureProbablityOpenStructure;
        private double failureProbabilityReparation;
        private double failureProbabilityStructureWithErosion;
        private double probabilityOpenStructureBeforeFlooding;
        private RoundedDouble deviationWaveDirection;
        private ForeshoreProfile foreshoreProfile;

        public ClosingStructuresInput()
        {
            structureNormalOrientation = new RoundedDouble(2);
            factorStormDurationOpenStructure = new RoundedDouble(2);
            deviationWaveDirection = new RoundedDouble(2);
            probabilityOpenStructureBeforeFlooding = 1.0;

            modelFactorSuperCriticalFlow = new NormalDistribution(2)
            {
                Mean = (RoundedDouble) 1.1,
                StandardDeviation = (RoundedDouble) 0.03
            };

            thresholdHeightOpenWeir = new NormalDistribution(2)
            {
                Mean = (RoundedDouble) double.NaN,
                StandardDeviation = (RoundedDouble) 0.1
            };

            drainCoefficient = new NormalDistribution(2)
            {
                Mean = (RoundedDouble) 1,
                StandardDeviation = (RoundedDouble) 0.2
            };

            areaFlowApertures = new LogNormalDistribution(2)
            {
                Mean = (RoundedDouble) double.NaN,
                StandardDeviation = (RoundedDouble) 0.01
            };

            levelCrestStructureNotClosing = new NormalDistribution(2)
            {
                Mean = (RoundedDouble) double.NaN,
                StandardDeviation = (RoundedDouble) 0.05
            };

            insideWaterLevel = new NormalDistribution(2)
            {
                Mean = (RoundedDouble) double.NaN,
                StandardDeviation = (RoundedDouble) 0.1
            };

            allowedLevelIncreaseStorage = new LogNormalDistribution(2)
            {
                Mean = (RoundedDouble) double.NaN,
                StandardDeviation = (RoundedDouble) 0.1
            };

            storageStructureArea = new LogNormalDistribution(2)
            {
                Mean = (RoundedDouble) double.NaN
            };
            storageStructureArea.SetStandardDeviationFromVariationCoefficient(0.1);

            flowWidthAtBottomProtection = new LogNormalDistribution(2)
            {
                Mean = (RoundedDouble) double.NaN,
                StandardDeviation = (RoundedDouble) 0.05
            };

            criticalOverToppingDischarge = new LogNormalDistribution(2)
            {
                Mean = (RoundedDouble) double.NaN
            };
            criticalOverToppingDischarge.SetStandardDeviationFromVariationCoefficient(0.15);

            widthFlowApertures = new NormalDistribution(2)
            {
                Mean = (RoundedDouble) double.NaN,
            };
            widthFlowApertures.SetStandardDeviationFromVariationCoefficient(0.05);

            stormDuration = new LogNormalDistribution(2)
            {
                Mean = (RoundedDouble) 7.5
            };
            stormDuration.SetStandardDeviationFromVariationCoefficient(0.25);

            UpdateForeshoreProperties();
        }

        #region Hydraulic Boundary Location

        public HydraulicBoundaryLocation HydraulicBoundaryLocation { get; set; }

        #endregion

        #region Structure properties

        /// <summary>
        /// Gets or sets the closing structure.
        /// </summary>
        public ClosingStructure ClosingStructure { get; set; }

        /// <summary>
        /// Gets or sets the type of closing structure.
        /// </summary>
        public ClosingStructureType ClosingStructureType { get; set; }

        #endregion

        #region Deterministic inputs

        #region Structure Normal orientation

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

        /// <summary>
        /// Gets or sets the storm duration for an open structure.
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
        /// Gets or sets the identical apertures to use during the calculation.
        /// </summary>
        public int IdenticalApertures { get; set; }

        /// <summary>
        /// Gets or sets the failure probability of an open structure.
        /// </summary>
        /// <exception cref="ArgumentException">Thrown when the value of the probability 
        /// is not between [0, 1].</exception>
        public double FailureProbabilityOpenStructure
        {
            get
            {
                return failureProbablityOpenStructure;
            }
            set
            {
                if (value < 0 || value > 1)
                {
                    throw new ArgumentException(RingtoetsCommonDataResources.FailureProbability_Value_needs_to_be_between_0_and_1);
                }
                failureProbablityOpenStructure = value;
            }
        }

        /// <summary>
        /// Gets or sets the reparation failure probability.
        /// </summary>
        /// <exception cref="ArgumentException">Thrown when the value of the probability 
        /// is not between [0, 1].</exception>
        public double FailureProbablityReparation
        {
            get
            {
                return failureProbabilityReparation;
            }
            set
            {
                if (value < 0 || value > 1)
                {
                    throw new ArgumentException(RingtoetsCommonDataResources.FailureProbability_Value_needs_to_be_between_0_and_1);
                }
                failureProbabilityReparation = value;
            }
        }

        /// <summary>
        /// Gets or sets the failure probability of structure given erosion.
        /// </summary>
        /// <exception cref="ArgumentException">Thrown when the value of the probability 
        /// is not between [0, 1].</exception>
        public double FailureProbabilityStructureWithErosion
        {
            get
            {
                return failureProbabilityStructureWithErosion;
            }
            set
            {
                if (value < 0 || value > 1)
                {
                    throw new ArgumentException(RingtoetsCommonDataResources.FailureProbability_Value_needs_to_be_between_0_and_1);
                }
                failureProbabilityStructureWithErosion = value;
            }
        }

        /// <summary>
        /// Gets or sets the deviation of the wave direction.
        /// </summary>
        public RoundedDouble DeviationWaveDirection
        {
            get
            {
                return deviationWaveDirection;
            }
            set
            {
                deviationWaveDirection = value.ToPrecision(deviationWaveDirection.NumberOfDecimalPlaces);
            }
        }

        /// <summary>
        /// Gets or sets the failure probability of an open structure before flooding.
        /// </summary>
        /// <exception cref="ArgumentException">Thrown when the value of the probability 
        /// is not between [0, 1].</exception>
        public double ProbabilityOpenStructureBeforeFlooding
        {
            get
            {
                return probabilityOpenStructureBeforeFlooding;
            }
            set
            {
                if (value < 0 || value > 1)
                {
                    throw new ArgumentException(RingtoetsCommonDataResources.FailureProbability_Value_needs_to_be_between_0_and_1);
                }
                probabilityOpenStructureBeforeFlooding = value;
            }
        }

        #endregion

        #region Probabilistic inputs

        /// <summary>
        /// Gets or sets the drain coefficient.
        /// </summary>
        /// <remarks>Only sets the mean</remarks>
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

        /// <summary>
        /// Gets or sets the model factor super critical flow.
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
        /// Gets or sets the threshold low weir height.
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
        /// Gets or sets the area flow apertures normal distribution.
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
        /// Gets or sets the level crest of structure not closing normal distribution.
        /// </summary>
        public NormalDistribution LevelCrestStructureNotClosing
        {
            get
            {
                return levelCrestStructureNotClosing;
            }
            set
            {
                levelCrestStructureNotClosing.Mean = value.Mean;
                levelCrestStructureNotClosing.StandardDeviation = value.StandardDeviation;
            }
        }

        /// <summary>
        /// Gets or sets the water level inside normal distribution.
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
        /// Gets or sets the allowable increase of level for storage log normal distribution.
        /// </summary>
        public LogNormalDistribution AllowedLevelIncreaseStorage
        {
            get
            {
                return allowedLevelIncreaseStorage;
            }
            set
            {
                allowedLevelIncreaseStorage.Mean = value.Mean;
                allowedLevelIncreaseStorage.StandardDeviation = value.StandardDeviation;
            }
        }

        /// <summary>
        /// Gets or sets the storage structure area log normal distribution.
        /// </summary>
        public LogNormalDistribution StorageStructureArea
        {
            get
            {
                return storageStructureArea;
            }
            set
            {
                storageStructureArea.Mean = value.Mean;
                storageStructureArea.StandardDeviation = value.StandardDeviation;
            }
        }

        /// <summary>
        /// Gets or sets the flow widt at bottom protection log normal distribution.
        /// </summary>
        public LogNormalDistribution FlowWidthAtBottomProtection
        {
            get
            {
                return flowWidthAtBottomProtection;
            }
            set
            {
                flowWidthAtBottomProtection.Mean = value.Mean;
                flowWidthAtBottomProtection.StandardDeviation = value.StandardDeviation;
            }
        }

        /// <summary>
        /// Gets or sets the critical overtopping discharge normal distribution.
        /// </summary>
        public LogNormalDistribution CriticalOverToppingDischarge
        {
            get
            {
                return criticalOverToppingDischarge;
            }
            set
            {
                criticalOverToppingDischarge.Mean = value.Mean;
                criticalOverToppingDischarge.StandardDeviation = value.StandardDeviation;
            }
        }

        /// <summary>
        /// Gets or sets the width of flow apertures normal distribution.
        /// </summary>
        public NormalDistribution WidthFlowApertures
        {
            get
            {
                return widthFlowApertures;
            }
            set
            {
                widthFlowApertures.Mean = value.Mean;
                widthFlowApertures.StandardDeviation = value.StandardDeviation;
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

        #region Foreshore Profile

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
    }
}