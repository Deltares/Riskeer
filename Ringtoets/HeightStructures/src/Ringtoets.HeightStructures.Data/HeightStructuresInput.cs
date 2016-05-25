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

namespace Ringtoets.HeightStructures.Data
{
    /// <summary>
    /// Class that holds all height structures calculation specific input parameters.
    /// </summary>
    public class HeightStructuresInput : Observable, ICalculationInput
    {
        private readonly NormalDistribution levelOfCrestOfStructure;
        private readonly GeneralHeightStructuresInput generalInputParameters;
        private readonly NormalDistribution modelfactorOvertoppingSuperCriticalFlow;
        private readonly LognormalDistribution allowableIncreaseOfLevelForStorage;
        private readonly LognormalDistribution storageStructureArea;
        private readonly LognormalDistribution flowWidthAtBottomProtection;
        private readonly LognormalDistribution criticalOvertoppingDischarge;
        private readonly NormalDistribution widthOfFlowApertures;
        private readonly LognormalDistribution stormDuration;
        private RoundedDouble orientationOfTheNormalOfTheStructure;
        private RoundedDouble failureProbabilityOfStructureGivenErosion;
        private RoundedDouble deviationOfTheWaveDirection;

        /// <summary>
        /// Creates a new instance of the <see cref="HeightStructuresInput"/> class.
        /// </summary>
        /// <param name="generalInputParameters">General height structures calculation input parameters that apply to each calculation.</param>
        /// <exception cref="ArgumentNullException">When <paramref name="generalInputParameters"/> is <c>null</c>.</exception>
        public HeightStructuresInput(GeneralHeightStructuresInput generalInputParameters)
        {
            if (generalInputParameters == null)
            {
                throw new ArgumentNullException("generalInputParameters");
            }
            this.generalInputParameters = generalInputParameters;

            levelOfCrestOfStructure = new NormalDistribution(2)
            {
                StandardDeviation = new RoundedDouble(2, 0.05)
            };

            OrientationOfTheNormalOfTheStructure = new RoundedDouble(2);

            modelfactorOvertoppingSuperCriticalFlow = new NormalDistribution(2)
            {
                Mean = new RoundedDouble(2, 1.1),
                StandardDeviation = new RoundedDouble(2, 0.03)
            };

            allowableIncreaseOfLevelForStorage = new LognormalDistribution(2)
            {
                StandardDeviation = new RoundedDouble(2, 0.1)
            };

            storageStructureArea = new LognormalDistribution(2)
            {
                StandardDeviation = new RoundedDouble(2, 0.1)
            };

            flowWidthAtBottomProtection = new LognormalDistribution(2)
            {
                StandardDeviation = new RoundedDouble(2, 0.05)
            };

            criticalOvertoppingDischarge = new LognormalDistribution(2)
            {
                StandardDeviation = new RoundedDouble(2, 0.15)
            };

            widthOfFlowApertures = new NormalDistribution(2)
            {
                StandardDeviation = new RoundedDouble(2, 0.05)
            };

            deviationOfTheWaveDirection = new RoundedDouble(2);

            stormDuration = new LognormalDistribution(2)
            {
                Mean = new RoundedDouble(2, 7.5), StandardDeviation = new RoundedDouble(2, 0.25)
            };
        }

        #region Model Factors

        /// <summary>
        /// Gets or sets the model factor overtopping critical flow.
        /// </summary>
        public NormalDistribution ModelfactorOvertoppingSuperCriticalFlow
        {
            get
            {
                return modelfactorOvertoppingSuperCriticalFlow;
            }
            set
            {
                modelfactorOvertoppingSuperCriticalFlow.Mean = value.Mean.ToPrecision(modelfactorOvertoppingSuperCriticalFlow.Mean.NumberOfDecimalPlaces);
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
        public LognormalDistribution StormDuration
        {
            get
            {
                return stormDuration;
            }
            set
            {
                stormDuration.Mean = value.Mean.ToPrecision(stormDuration.Mean.NumberOfDecimalPlaces);
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
                levelOfCrestOfStructure.Mean = value.Mean.ToPrecision(levelOfCrestOfStructure.Mean.NumberOfDecimalPlaces);
                levelOfCrestOfStructure.StandardDeviation = value.StandardDeviation.ToPrecision(levelOfCrestOfStructure.StandardDeviation.NumberOfDecimalPlaces);
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
        public LognormalDistribution AllowableIncreaseOfLevelForStorage
        {
            get
            {
                return allowableIncreaseOfLevelForStorage;
            }
            set
            {
                allowableIncreaseOfLevelForStorage.Mean = value.Mean.ToPrecision(allowableIncreaseOfLevelForStorage.Mean.NumberOfDecimalPlaces);
                allowableIncreaseOfLevelForStorage.StandardDeviation = value.StandardDeviation.ToPrecision(allowableIncreaseOfLevelForStorage.StandardDeviation.NumberOfDecimalPlaces);
            }
        }

        /// <summary>
        /// Gets or sets the storage structure area.
        /// </summary>
        public LognormalDistribution StorageStructureArea
        {
            get
            {
                return storageStructureArea;
            }
            set
            {
                storageStructureArea.Mean = value.Mean.ToPrecision(storageStructureArea.Mean.NumberOfDecimalPlaces);
                storageStructureArea.StandardDeviation = value.StandardDeviation.ToPrecision(storageStructureArea.StandardDeviation.NumberOfDecimalPlaces);
            }
        }

        /// <summary>
        /// Gets or sets the flow width at bottom protection.
        /// </summary>
        public LognormalDistribution FlowWidthAtBottomProtection
        {
            get
            {
                return flowWidthAtBottomProtection;
            }
            set
            {
                flowWidthAtBottomProtection.Mean = value.Mean.ToPrecision(flowWidthAtBottomProtection.Mean.NumberOfDecimalPlaces);
                flowWidthAtBottomProtection.StandardDeviation = value.StandardDeviation.ToPrecision(flowWidthAtBottomProtection.StandardDeviation.NumberOfDecimalPlaces);
            }
        }

        /// <summary>
        /// Gets or sets the critical overtopping discharge.
        /// </summary>
        public LognormalDistribution CriticalOvertoppingDischarge
        {
            get
            {
                return criticalOvertoppingDischarge;
            }
            set
            {
                criticalOvertoppingDischarge.Mean = value.Mean.ToPrecision(criticalOvertoppingDischarge.Mean.NumberOfDecimalPlaces);
                criticalOvertoppingDischarge.StandardDeviation = value.StandardDeviation.ToPrecision(criticalOvertoppingDischarge.StandardDeviation.NumberOfDecimalPlaces);
            }
        }

        /// <summary>
        /// Gets or sets the failure probability of structure given erosion.
        /// </summary>
        public RoundedDouble FailureProbabilityOfStructureGivenErosion
        {
            get
            {
                return failureProbabilityOfStructureGivenErosion;
            }
            set
            {
                failureProbabilityOfStructureGivenErosion = value.ToPrecision(failureProbabilityOfStructureGivenErosion.NumberOfDecimalPlaces);
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
                widthOfFlowApertures.Mean = value.Mean.ToPrecision(widthOfFlowApertures.Mean.NumberOfDecimalPlaces);
                widthOfFlowApertures.StandardDeviation = value.StandardDeviation.ToPrecision(widthOfFlowApertures.StandardDeviation.NumberOfDecimalPlaces);
            }
        }

        #endregion

        #region General input parameters

        /// <summary>
        /// Gets the gravitational acceleration.
        /// </summary>
        public RoundedDouble GravitationalAcceleration
        {
            get
            {
                return generalInputParameters.GravitationalAcceleration;
            }
        }

        /// <summary>
        /// Gets the model factor overtopping flow.
        /// </summary>
        public LognormalDistribution ModelfactorOvertoppingFlow
        {
            get
            {
                return generalInputParameters.ModelfactorOvertoppingFlow;
            }
        }

        /// <summary>
        /// Gets the model factor for storage volume.
        /// </summary>
        public LognormalDistribution ModelFactorForStorageVolume
        {
            get
            {
                return generalInputParameters.ModelFactorForStorageVolume;
            }
        }

        /// <summary>
        /// Gets the model factor for incoming flow volume.
        /// </summary>
        public RoundedDouble ModelFactorForIncomingFlowVolume
        {
            get
            {
                return generalInputParameters.ModelFactorForIncomingFlowVolume;
            }
        }

        #endregion
    }
}