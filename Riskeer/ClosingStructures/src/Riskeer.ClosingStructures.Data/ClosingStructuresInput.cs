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
using Core.Common.Base.Data;
using Ringtoets.ClosingStructures.Data.Properties;
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.Common.Data.Probability;
using Ringtoets.Common.Data.Structures;
using RingtoetsCommonDataResources = Ringtoets.Common.Data.Properties.Resources;

namespace Ringtoets.ClosingStructures.Data
{
    /// <summary>
    /// Class that holds all closing structures calculation specific input parameters.
    /// </summary>
    public class ClosingStructuresInput : StructuresInputBase<ClosingStructure>
    {
        private const int deviationWaveDirectionNumberOfDecimals = 2;

        private static readonly Range<RoundedDouble> deviationWaveDirectionValidityRange = new Range<RoundedDouble>(
            new RoundedDouble(deviationWaveDirectionNumberOfDecimals, -360), new RoundedDouble(deviationWaveDirectionNumberOfDecimals, 360));

        private static readonly Range<int> identicalAperturesValidityRange = new Range<int>(1, int.MaxValue);

        private NormalDistribution thresholdHeightOpenWeir;
        private NormalDistribution modelFactorSuperCriticalFlow;
        private NormalDistribution drainCoefficient;
        private LogNormalDistribution areaFlowApertures;
        private NormalDistribution levelCrestStructureNotClosing;
        private NormalDistribution insideWaterLevel;
        private RoundedDouble factorStormDurationOpenStructure;
        private double failureProbabilityOpenStructure;
        private double failureProbabilityReparation;
        private double probabilityOpenStructureBeforeFlooding;
        private RoundedDouble deviationWaveDirection;
        private int identicalApertures;

        /// <summary>
        /// Creates a new instance of the <see cref="ClosingStructuresInput"/> class.
        /// </summary>
        public ClosingStructuresInput()
        {
            factorStormDurationOpenStructure = new RoundedDouble(2, 1.0);
            deviationWaveDirection = new RoundedDouble(deviationWaveDirectionNumberOfDecimals);

            drainCoefficient = new NormalDistribution(2)
            {
                Mean = (RoundedDouble) 1,
                StandardDeviation = (RoundedDouble) 0.2
            };

            modelFactorSuperCriticalFlow = new NormalDistribution(2)
            {
                Mean = (RoundedDouble) 1.1,
                StandardDeviation = (RoundedDouble) 0.05
            };

            thresholdHeightOpenWeir = new NormalDistribution(2);
            areaFlowApertures = new LogNormalDistribution(2);
            levelCrestStructureNotClosing = new NormalDistribution(2);
            insideWaterLevel = new NormalDistribution(2);

            SetDefaultSchematizationProperties();
        }

        public override bool IsStructureInputSynchronized
        {
            get
            {
                return Structure != null
                       && Equals(StructureNormalOrientation, Structure.StructureNormalOrientation)
                       && Equals(LevelCrestStructureNotClosing, Structure.LevelCrestStructureNotClosing)
                       && Equals(FlowWidthAtBottomProtection, Structure.FlowWidthAtBottomProtection)
                       && Equals(CriticalOvertoppingDischarge, Structure.CriticalOvertoppingDischarge)
                       && Equals(WidthFlowApertures, Structure.WidthFlowApertures)
                       && Equals(StorageStructureArea, Structure.StorageStructureArea)
                       && Equals(AllowedLevelIncreaseStorage, Structure.AllowedLevelIncreaseStorage)
                       && Equals(InflowModelType, Structure.InflowModelType)
                       && Equals(AreaFlowApertures, Structure.AreaFlowApertures)
                       && Equals(FailureProbabilityOpenStructure, Structure.FailureProbabilityOpenStructure)
                       && Equals(FailureProbabilityReparation, Structure.FailureProbabilityReparation)
                       && Equals(IdenticalApertures, Structure.IdenticalApertures)
                       && Equals(InsideWaterLevel, Structure.InsideWaterLevel)
                       && Equals(ProbabilityOpenStructureBeforeFlooding, Structure.ProbabilityOpenStructureBeforeFlooding)
                       && Equals(ThresholdHeightOpenWeir, Structure.ThresholdHeightOpenWeir);
            }
        }

        #region Structure

        /// <summary>
        /// Gets or sets the type of closing structure inflow model.
        /// </summary>
        public ClosingStructureInflowModelType InflowModelType { get; set; }

        #endregion

        public override void SynchronizeStructureInput()
        {
            if (Structure != null)
            {
                StructureNormalOrientation = Structure.StructureNormalOrientation;
                LevelCrestStructureNotClosing = Structure.LevelCrestStructureNotClosing;
                FlowWidthAtBottomProtection = Structure.FlowWidthAtBottomProtection;
                CriticalOvertoppingDischarge = Structure.CriticalOvertoppingDischarge;
                WidthFlowApertures = Structure.WidthFlowApertures;
                StorageStructureArea = Structure.StorageStructureArea;
                AllowedLevelIncreaseStorage = Structure.AllowedLevelIncreaseStorage;
                InflowModelType = Structure.InflowModelType;
                AreaFlowApertures = Structure.AreaFlowApertures;
                FailureProbabilityOpenStructure = Structure.FailureProbabilityOpenStructure;
                FailureProbabilityReparation = Structure.FailureProbabilityReparation;
                IdenticalApertures = Structure.IdenticalApertures;
                InsideWaterLevel = Structure.InsideWaterLevel;
                ProbabilityOpenStructureBeforeFlooding = Structure.ProbabilityOpenStructureBeforeFlooding;
                ThresholdHeightOpenWeir = Structure.ThresholdHeightOpenWeir;
            }
            else
            {
                SetDefaultSchematizationProperties();
            }
        }

        public override object Clone()
        {
            var clone = (ClosingStructuresInput) base.Clone();

            clone.thresholdHeightOpenWeir = (NormalDistribution) ThresholdHeightOpenWeir.Clone();
            clone.modelFactorSuperCriticalFlow = (NormalDistribution) ModelFactorSuperCriticalFlow.Clone();
            clone.drainCoefficient = (NormalDistribution) DrainCoefficient.Clone();
            clone.areaFlowApertures = (LogNormalDistribution) AreaFlowApertures.Clone();
            clone.levelCrestStructureNotClosing = (NormalDistribution) LevelCrestStructureNotClosing.Clone();
            clone.insideWaterLevel = (NormalDistribution) InsideWaterLevel.Clone();

            return clone;
        }

        private void SetDefaultSchematizationProperties()
        {
            FailureProbabilityOpenStructure = 0;
            FailureProbabilityReparation = 0;
            ProbabilityOpenStructureBeforeFlooding = 1.0;

            ThresholdHeightOpenWeir = new NormalDistribution
            {
                Mean = RoundedDouble.NaN,
                StandardDeviation = RoundedDouble.NaN
            };

            AreaFlowApertures = new LogNormalDistribution
            {
                Mean = RoundedDouble.NaN,
                StandardDeviation = RoundedDouble.NaN
            };

            LevelCrestStructureNotClosing = new NormalDistribution
            {
                Mean = RoundedDouble.NaN,
                StandardDeviation = RoundedDouble.NaN
            };

            InsideWaterLevel = new NormalDistribution
            {
                Mean = RoundedDouble.NaN,
                StandardDeviation = RoundedDouble.NaN
            };

            IdenticalApertures = 1;
            InflowModelType = 0;
        }

        #region Hydraulic data

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
                RoundedDouble newDeviationWaveDirection = value.ToPrecision(deviationWaveDirectionNumberOfDecimals);
                if (!double.IsNaN(newDeviationWaveDirection) && !deviationWaveDirectionValidityRange.InRange(newDeviationWaveDirection))
                {
                    throw new ArgumentOutOfRangeException(nameof(value), string.Format(RingtoetsCommonDataResources.DeviationWaveDirection_Value_needs_to_be_in_Range_0_,
                                                                                       deviationWaveDirectionValidityRange));
                }

                deviationWaveDirection = newDeviationWaveDirection;
            }
        }

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
        /// Gets or sets the factor for the storm duration for an open structure.
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

        #region Schematization

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
        /// is not in the interval [0, 1].</exception>
        public double FailureProbabilityOpenStructure
        {
            get
            {
                return failureProbabilityOpenStructure;
            }
            set
            {
                ProbabilityHelper.ValidateProbability(value, null, RingtoetsCommonDataResources.FailureProbability_Value_needs_to_be_in_Range_0_);
                failureProbabilityOpenStructure = value;
            }
        }

        /// <summary>
        /// Gets or sets the reparation failure probability.
        /// [1/year]
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the value of the probability 
        /// is not in the interval [0, 1].</exception>
        public double FailureProbabilityReparation
        {
            get
            {
                return failureProbabilityReparation;
            }
            set
            {
                ProbabilityHelper.ValidateProbability(value, null, RingtoetsCommonDataResources.FailureProbability_Value_needs_to_be_in_Range_0_);
                failureProbabilityReparation = value;
            }
        }

        /// <summary>
        /// Gets or sets the amount of identical apertures.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the value is smaller than 1.</exception>
        public int IdenticalApertures
        {
            get
            {
                return identicalApertures;
            }
            set
            {
                if (!identicalAperturesValidityRange.InRange(value))
                {
                    throw new ArgumentOutOfRangeException(nameof(IdenticalApertures),
                                                          Resources.ClosingStructuresInput_IdenticalApertures_must_be_equal_or_greater_to_one);
                }

                identicalApertures = value;
            }
        }

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
        /// Gets or sets the failure probability of an open structure before flooding.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the value of the probability 
        /// is not in the interval [0, 1].</exception>
        public double ProbabilityOpenStructureBeforeFlooding
        {
            get
            {
                return probabilityOpenStructureBeforeFlooding;
            }
            set
            {
                ProbabilityHelper.ValidateProbability(value, null, RingtoetsCommonDataResources.FailureProbability_Value_needs_to_be_in_Range_0_);
                probabilityOpenStructureBeforeFlooding = value;
            }
        }

        #endregion
    }
}