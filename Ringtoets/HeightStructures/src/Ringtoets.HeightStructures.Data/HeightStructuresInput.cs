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
using Core.Common.Base;
using Core.Common.Base.Data;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.HydraRing.Data;
using RingtoetsCommonDataResources = Ringtoets.Common.Data.Properties.Resources;

namespace Ringtoets.HeightStructures.Data
{
    /// <summary>
    /// Class that holds all height structures calculation specific input parameters.
    /// </summary>
    public class HeightStructuresInput : Observable, ICalculationInput
    {
        private readonly NormalDistribution levelOfCrestOfStructure;
        private readonly NormalDistribution modelFactorOvertoppingSuperCriticalFlow;
        private readonly LogNormalDistribution allowableIncreaseOfLevelForStorage;
        private readonly LogNormalDistribution storageStructureArea;
        private readonly LogNormalDistribution flowWidthAtBottomProtection;
        private readonly LogNormalDistribution criticalOvertoppingDischarge;
        private readonly NormalDistribution widthOfFlowApertures;
        private readonly LogNormalDistribution stormDuration;
        private RoundedDouble orientationOfTheNormalOfTheStructure;
        private RoundedDouble deviationOfTheWaveDirection;
        private double failureProbabilityOfStructureGivenErosion;

        /// <summary>
        /// Creates a new instance of the <see cref="HeightStructuresInput"/> class.
        /// </summary>
        public HeightStructuresInput()
        {
            failureProbabilityOfStructureGivenErosion = 1.0;

            levelOfCrestOfStructure = new NormalDistribution(2)
            {
                StandardDeviation = (RoundedDouble) 0.05
            };

            orientationOfTheNormalOfTheStructure = new RoundedDouble(2);

            modelFactorOvertoppingSuperCriticalFlow = new NormalDistribution(2)
            {
                Mean = (RoundedDouble) 1.1,
                StandardDeviation = (RoundedDouble) 0.03
            };

            allowableIncreaseOfLevelForStorage = new LogNormalDistribution(2)
            {
                Mean = (RoundedDouble) double.NaN,
                StandardDeviation = (RoundedDouble) 0.1
            };

            storageStructureArea = new LogNormalDistribution(2)
            {
                Mean = (RoundedDouble) 1.0
            };
            storageStructureArea.SetStandardDeviationFromVariationCoefficient(0.1);

            flowWidthAtBottomProtection = new LogNormalDistribution(2)
            {
                Mean = (RoundedDouble) double.NaN,
                StandardDeviation = (RoundedDouble) 0.05
            };

            criticalOvertoppingDischarge = new LogNormalDistribution(2)
            {
                Mean = (RoundedDouble) 1.0
            };
            criticalOvertoppingDischarge.SetStandardDeviationFromVariationCoefficient(0.15);

            widthOfFlowApertures = new NormalDistribution(2)
            {
                StandardDeviation = (RoundedDouble) 0.05
            };

            deviationOfTheWaveDirection = new RoundedDouble(2);

            stormDuration = new LogNormalDistribution(2)
            {
                Mean = (RoundedDouble) 7.5
            };
            stormDuration.SetStandardDeviationFromVariationCoefficient(0.25);
        }

        #region Model Factors

        /// <summary>
        /// Gets or sets the model factor overtopping critical flow.
        /// </summary>
        /// <remarks>Only sets the mean.</remarks>
        public NormalDistribution ModelFactorOvertoppingSuperCriticalFlow
        {
            get
            {
                return modelFactorOvertoppingSuperCriticalFlow;
            }
            set
            {
                modelFactorOvertoppingSuperCriticalFlow.Mean = value.Mean;
            }
        }

        #endregion

        #region Hydraulic pressure

        /// <summary>
        /// Gets or sets the hydraulic boundary location from which to use the assessment level.
        /// </summary>
        public HydraulicBoundaryLocation HydraulicBoundaryLocation { get; set; }

        /// <summary>
        /// Gets or sets the deviation of the wave's direction.
        /// </summary>
        public RoundedDouble DeviationOfTheWaveDirection
        {
            get
            {
                return deviationOfTheWaveDirection;
            }
            set
            {
                deviationOfTheWaveDirection = value.ToPrecision(deviationOfTheWaveDirection.NumberOfDecimalPlaces);
            }
        }

        /// <summary>
        /// Gets or sets the storm duration
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

        #region Schematisation

        /// <summary>
        /// Gets or sets the level of crest of the structure.
        /// </summary>
        public NormalDistribution LevelOfCrestOfStructure
        {
            get
            {
                return levelOfCrestOfStructure;
            }
            set
            {
                levelOfCrestOfStructure.Mean = value.Mean;
                levelOfCrestOfStructure.StandardDeviation = value.StandardDeviation;
            }
        }

        /// <summary>
        /// Gets or sets the orientation of the normal of the structure.
        /// </summary>
        public RoundedDouble OrientationOfTheNormalOfTheStructure
        {
            get
            {
                return orientationOfTheNormalOfTheStructure;
            }
            set
            {
                orientationOfTheNormalOfTheStructure = value.ToPrecision(orientationOfTheNormalOfTheStructure.NumberOfDecimalPlaces);
            }
        }

        /// <summary>
        /// Gets or sets the allowable increase of level for the storage.
        /// </summary>
        public LogNormalDistribution AllowableIncreaseOfLevelForStorage
        {
            get
            {
                return allowableIncreaseOfLevelForStorage;
            }
            set
            {
                allowableIncreaseOfLevelForStorage.Mean = value.Mean;
                allowableIncreaseOfLevelForStorage.StandardDeviation = value.StandardDeviation;
            }
        }

        /// <summary>
        /// Gets or sets the storage structure area.
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
        /// Gets or sets the flow width at bottom protection.
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
        /// </summary>
        public LogNormalDistribution CriticalOvertoppingDischarge
        {
            get
            {
                return criticalOvertoppingDischarge;
            }
            set
            {
                criticalOvertoppingDischarge.Mean = value.Mean;
                criticalOvertoppingDischarge.StandardDeviation = value.StandardDeviation;
            }
        }

        /// <summary>
        /// Gets or sets the failure probability of structure given erosion.
        /// </summary>
        /// <exception cref="ArgumentException">Thrown when <paramref name="value"/> is not in range [0, 1].</exception>
        public double FailureProbabilityOfStructureGivenErosion
        {
            get
            {
                return failureProbabilityOfStructureGivenErosion;
            }
            set
            {
                if (value < 0 || value > 1)
                {
                    throw new ArgumentException(RingtoetsCommonDataResources.FailureProbability_Value_needs_to_be_between_0_and_1);
                }
                failureProbabilityOfStructureGivenErosion = value;
            }
        }

        /// <summary>
        /// Gets or sets the width of flow apertures.
        /// </summary>
        public NormalDistribution WidthOfFlowApertures
        {
            get
            {
                return widthOfFlowApertures;
            }
            set
            {
                widthOfFlowApertures.Mean = value.Mean;
                widthOfFlowApertures.StandardDeviation = value.StandardDeviation;
            }
        }

        #endregion
    }
}