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
    /// <summary>
    /// Class that holds all closing structures calculation specific input parameters.
    /// </summary>
    public class ClosingStructuresInput : Observable, ICalculationInput
    {
        private readonly NormalDistribution modelFactorSuperCriticalFlow;
        private readonly NormalDistribution thresholdHeightOpenWeir;
        private readonly NormalDistribution drainCoefficient;
        private readonly LogNormalDistribution areaFlowApertures;
        private readonly NormalDistribution levelCrestStructureNotClosing;
        private readonly NormalDistribution insideWaterLevel;
        private readonly VariationCoefficientLogNormalDistribution storageStructureArea;
        private readonly LogNormalDistribution allowedLevelIncreaseStorage;
        private readonly LogNormalDistribution flowWidthAtBottomProtection;
        private readonly VariationCoefficientNormalDistribution widthFlowApertures;
        private readonly VariationCoefficientLogNormalDistribution stormDuration;
        private readonly VariationCoefficientLogNormalDistribution criticalOvertoppingDischarge;
        private RoundedDouble structureNormalOrientation;
        private RoundedDouble factorStormDurationOpenStructure;
        private double failureProbabilityOpenStructure;
        private double failureProbabilityReparation;
        private double failureProbabilityStructureWithErosion;
        private double probabilityOpenStructureBeforeFlooding;
        private RoundedDouble deviationWaveDirection;
        private ForeshoreProfile foreshoreProfile;

        /// <summary>
        /// Creates a new instance of the <see cref="ClosingStructuresInput"/> class.
        /// </summary>
        public ClosingStructuresInput()
        {
            structureNormalOrientation = new RoundedDouble(2, double.NaN);
            factorStormDurationOpenStructure = new RoundedDouble(2, double.NaN);
            deviationWaveDirection = new RoundedDouble(2, double.NaN);

            failureProbabilityOpenStructure = double.NaN;
            failureProbabilityReparation = double.NaN;
            failureProbabilityStructureWithErosion = double.NaN;
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

            storageStructureArea = new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = (RoundedDouble) double.NaN,
                CoefficientOfVariation = (RoundedDouble) 0.1
            };

            flowWidthAtBottomProtection = new LogNormalDistribution(2)
            {
                Mean = (RoundedDouble) double.NaN,
                StandardDeviation = (RoundedDouble) 0.05
            };

            criticalOvertoppingDischarge = new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = (RoundedDouble) double.NaN,
                CoefficientOfVariation = (RoundedDouble) 0.15
            };

            widthFlowApertures = new VariationCoefficientNormalDistribution(2)
            {
                Mean = (RoundedDouble) double.NaN,
                CoefficientOfVariation = (RoundedDouble) 0.05
            };

            stormDuration = new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = (RoundedDouble) 6.0,
                CoefficientOfVariation = (RoundedDouble) 0.25
            };

            UpdateForeshoreProperties();
        }

        private static bool ValidProbabilityValue(double probability)
        {
            return !double.IsNaN(probability) && probability <= 1 && probability >= 0;
        }

        #region Structure

        /// <summary>
        /// Gets or sets the closing structure.
        /// </summary>
        public ClosingStructure ClosingStructure { get; set; }

        /// <summary>
        /// Gets or sets the type of closing structure inflow model.
        /// </summary>
        public ClosingStructureInflowModelType InflowModelType { get; set; }

        #endregion

        #region Model factors

        /// <summary>
        /// Gets or sets the model factor for the super critical flow.
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

        #endregion

        #region Hydraulic data

        public HydraulicBoundaryLocation HydraulicBoundaryLocation { get; set; }

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

        /// <summary>
        /// Gets or sets the inside water level.
        /// [m+NAP]
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
        /// Gets or sets the deviation of the wave direction.
        /// [degrees]
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
        /// Gets or sets the storm duration.
        /// [hrs]
        /// </summary>
        /// <remarks>Only sets the mean.</remarks>
        public VariationCoefficientLogNormalDistribution StormDuration
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

        #region Schematization

        /// <summary>
        /// Gets or sets the orientation of the normal of the structure.
        /// [degrees]
        /// </summary>
        ///<exception cref="ArgumentOutOfRangeException">Thown when the value for the orientation is not between [0,360] degrees.</exception>
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
                    structureNormalOrientation = value.ToPrecision(structureNormalOrientation.NumberOfDecimalPlaces);
                    return;
                }

                RoundedDouble newOrientationValue = value.ToPrecision(structureNormalOrientation.NumberOfDecimalPlaces);
                if (newOrientationValue < 0 || newOrientationValue > 360)
                {
                    throw new ArgumentOutOfRangeException("value", RingtoetsCommonDataResources.Orientation_Value_needs_to_be_between_0_and_360);
                }
                structureNormalOrientation = newOrientationValue;
            }
        }

        /// <summary>
        /// Gets or sets the threshold height of the open weir.
        /// [m+NAP]
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
        /// Gets or sets the area flow apertures.
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
        /// Gets or sets the failure probability of an open structure.
        /// [1/year]
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the value of the probability 
        /// is not between [0, 1].</exception>
        public double FailureProbabilityOpenStructure
        {
            get
            {
                return failureProbabilityOpenStructure;
            }
            set
            {
                if (!ValidProbabilityValue(value))
                {
                    throw new ArgumentOutOfRangeException("value", RingtoetsCommonDataResources.FailureProbability_Value_needs_to_be_between_0_and_1);
                }
                failureProbabilityOpenStructure = value;
            }
        }

        /// <summary>
        /// Gets or sets the reparation failure probability.
        /// [1/year]
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the value of the probability 
        /// is not between [0, 1].</exception>
        public double FailureProbabilityReparation
        {
            get
            {
                return failureProbabilityReparation;
            }
            set
            {
                if (!ValidProbabilityValue(value))
                {
                    throw new ArgumentOutOfRangeException("value", RingtoetsCommonDataResources.FailureProbability_Value_needs_to_be_between_0_and_1);
                }
                failureProbabilityReparation = value;
            }
        }

        /// <summary>
        /// Gets or sets the amount of identical apertures to use during the calculation.
        /// </summary>
        public int IdenticalApertures { get; set; }

        /// <summary>
        /// Gets or sets the level of crest of the structures that are not closed.
        /// [m+NAP]
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
        /// Gets or sets the allowed increase of level for storage.
        /// [m]
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
        /// Gets or sets the storage structure area.
        /// [m^2]
        /// </summary>
        public VariationCoefficientLogNormalDistribution StorageStructureArea
        {
            get
            {
                return storageStructureArea;
            }
            set
            {
                storageStructureArea.Mean = value.Mean;
                storageStructureArea.CoefficientOfVariation = value.CoefficientOfVariation;
            }
        }

        /// <summary>
        /// Gets or sets the flow width at the bottom protection.
        /// [m]
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
        /// Gets or sets the critical overtopping discharge.
        /// [m^3/s/m]
        /// </summary>
        public VariationCoefficientLogNormalDistribution CriticalOvertoppingDischarge
        {
            get
            {
                return criticalOvertoppingDischarge;
            }
            set
            {
                criticalOvertoppingDischarge.Mean = value.Mean;
                criticalOvertoppingDischarge.CoefficientOfVariation = value.CoefficientOfVariation;
            }
        }

        /// <summary>
        /// Gets or sets the failure probability of structure given erosion.
        /// [1/year]
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the value of the probability 
        /// is not between [0, 1].</exception>
        public double FailureProbabilityStructureWithErosion
        {
            get
            {
                return failureProbabilityStructureWithErosion;
            }
            set
            {
                if (!ValidProbabilityValue(value))
                {
                    throw new ArgumentOutOfRangeException("value", RingtoetsCommonDataResources.FailureProbability_Value_needs_to_be_between_0_and_1);
                }
                failureProbabilityStructureWithErosion = value;
            }
        }

        /// <summary>
        /// Gets or sets the width of flow apertures.
        /// [m]
        /// </summary>
        public VariationCoefficientNormalDistribution WidthFlowApertures
        {
            get
            {
                return widthFlowApertures;
            }
            set
            {
                widthFlowApertures.Mean = value.Mean;
                widthFlowApertures.CoefficientOfVariation = value.CoefficientOfVariation;
            }
        }

        /// <summary>
        /// Gets or sets the failure probability of an open structure before flooding.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the value of the probability 
        /// is not between [0, 1].</exception>
        public double ProbabilityOpenStructureBeforeFlooding
        {
            get
            {
                return probabilityOpenStructureBeforeFlooding;
            }
            set
            {
                if (!ValidProbabilityValue(value))
                {
                    throw new ArgumentOutOfRangeException("value", RingtoetsCommonDataResources.FailureProbability_Value_needs_to_be_between_0_and_1);
                }
                probabilityOpenStructureBeforeFlooding = value;
            }
        }

        #endregion
    }
}