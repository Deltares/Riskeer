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
        private readonly NormalDistribution levelCrestStructure;
        private RoundedDouble deviationWaveDirection;

        /// <summary>
        /// Creates a new instance of the <see cref="HeightStructuresInput"/> class.
        /// </summary>
        public HeightStructuresInput()
        {
            levelCrestStructure = new NormalDistribution(2)
            {
                Mean = (RoundedDouble) double.NaN,
                StandardDeviation = (RoundedDouble) 0.05
            };

            deviationWaveDirection = new RoundedDouble(2, double.NaN);
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
                deviationWaveDirection = value.ToPrecision(deviationWaveDirection.NumberOfDecimalPlaces);
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

        protected override void UpdateStructureParameters()
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
        }
    }
}