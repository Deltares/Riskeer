// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.Common.Data.Structures;
using RingtoetsCommonDataResources = Ringtoets.Common.Data.Properties.Resources;

namespace Ringtoets.HeightStructures.Data
{
    /// <summary>
    /// Class that holds all height structures calculation specific input parameters.
    /// </summary>
    public class HeightStructuresInput : StructuresInputBase<HeightStructure>
    {
        private const int deviationWaveDirectionNumberOfDecimals = 2;

        private static readonly Range<RoundedDouble> deviationWaveDirectionValidityRange = new Range<RoundedDouble>(new RoundedDouble(deviationWaveDirectionNumberOfDecimals, -360),
                                                                                                                    new RoundedDouble(deviationWaveDirectionNumberOfDecimals, 360));

        private RoundedDouble deviationWaveDirection;
        private NormalDistribution modelFactorSuperCriticalFlow;
        private NormalDistribution levelCrestStructure;

        /// <summary>
        /// Creates a new instance of the <see cref="HeightStructuresInput"/> class.
        /// </summary>
        public HeightStructuresInput()
        {
            deviationWaveDirection = new RoundedDouble(deviationWaveDirectionNumberOfDecimals);

            modelFactorSuperCriticalFlow = new NormalDistribution(2)
            {
                Mean = (RoundedDouble) 1.1,
                StandardDeviation = (RoundedDouble) 0.05
            };

            levelCrestStructure = new NormalDistribution(2);

            SetDefaultSchematizationProperties();
        }

        public override bool IsStructureInputSynchronized
        {
            get
            {
                return Structure != null
                       && Equals(StructureNormalOrientation, Structure.StructureNormalOrientation)
                       && Equals(LevelCrestStructure, Structure.LevelCrestStructure)
                       && Equals(FlowWidthAtBottomProtection, Structure.FlowWidthAtBottomProtection)
                       && Equals(CriticalOvertoppingDischarge, Structure.CriticalOvertoppingDischarge)
                       && Equals(WidthFlowApertures, Structure.WidthFlowApertures)
                       && Equals(FailureProbabilityStructureWithErosion, Structure.FailureProbabilityStructureWithErosion)
                       && Equals(StorageStructureArea, Structure.StorageStructureArea)
                       && Equals(AllowedLevelIncreaseStorage, Structure.AllowedLevelIncreaseStorage);
            }
        }

        #region Hydraulic data

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
                RoundedDouble newDeviationWaveDirection = value.ToPrecision(deviationWaveDirection.NumberOfDecimalPlaces);
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

        #endregion

        #region Schematization

        /// <summary>
        /// Gets or sets the crest level of the structure.
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

        #endregion

        public override object Clone()
        {
            var clone = (HeightStructuresInput) base.Clone();

            clone.levelCrestStructure = (NormalDistribution) LevelCrestStructure.Clone();

            return clone;
        }

        public override void SynchronizeStructureInput()
        {
            if (Structure != null)
            {
                StructureNormalOrientation = Structure.StructureNormalOrientation;
                LevelCrestStructure = Structure.LevelCrestStructure;
                FlowWidthAtBottomProtection = Structure.FlowWidthAtBottomProtection;
                CriticalOvertoppingDischarge = Structure.CriticalOvertoppingDischarge;
                WidthFlowApertures = Structure.WidthFlowApertures;
                FailureProbabilityStructureWithErosion = Structure.FailureProbabilityStructureWithErosion;
                StorageStructureArea = Structure.StorageStructureArea;
                AllowedLevelIncreaseStorage = Structure.AllowedLevelIncreaseStorage;
            }
            else
            {
                SetDefaultSchematizationProperties();
            }
        }

        private void SetDefaultSchematizationProperties()
        {
            LevelCrestStructure = new NormalDistribution
            {
                Mean = RoundedDouble.NaN,
                StandardDeviation = RoundedDouble.NaN
            };

            FailureProbabilityStructureWithErosion = 1.0;
        }
    }
}