﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

namespace Ringtoets.ClosingStructures.Data
{
    public class ClosingStructuresInput : Observable, ICalculationInput
    {
        private RoundedDouble orientationOfTheNormalOfTheStructure;
        private RoundedDouble factorStormDurationOpenStructure;

        private readonly NormalDistribution thresholdLowWeirHeight;
        private readonly NormalDistribution drainCoefficient;
        private readonly LogNormalDistribution areaFlowApertures;
        private double failureProbablityOpenStructure;
        private double failureProbabilityReparation;
        private readonly NormalDistribution levelCrestOfStructureNotClosing;
        private readonly NormalDistribution waterLevelInside;
        private readonly LogNormalDistribution storageStructureArea;
        private readonly LogNormalDistribution allowableIncreaseOfLevelForStorage;
        private readonly LogNormalDistribution flowWidthAtBottomProtection;
        private double failureProbabilityOfStructureGivenErosion;
        private readonly NormalDistribution widthOfFlowApertures;
        private readonly LogNormalDistribution stormDuration;
        private double probabilityOpenStructureBeforeFlooding;

        public ClosingStructuresInput()
        {
            orientationOfTheNormalOfTheStructure = new RoundedDouble(2);
            factorStormDurationOpenStructure = new RoundedDouble(2);

            thresholdLowWeirHeight = new NormalDistribution(2)
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

            levelCrestOfStructureNotClosing = new NormalDistribution(2)
            {
                Mean = (RoundedDouble) double.NaN,
                StandardDeviation = (RoundedDouble) 0.05
            };

            waterLevelInside = new NormalDistribution(2)
            {
                Mean = (RoundedDouble) double.NaN,
                StandardDeviation = (RoundedDouble) 0.1
            };

            allowableIncreaseOfLevelForStorage = new LogNormalDistribution(2)
            {
                Mean = (RoundedDouble) double.NaN,
                StandardDeviation = (RoundedDouble) 0.1
            };

            storageStructureArea = new LogNormalDistribution(2)
            {
                Mean = (RoundedDouble) double.NaN,
                StandardDeviation = (RoundedDouble) 0.1
            };

            flowWidthAtBottomProtection = new LogNormalDistribution(2)
            {
                Mean = (RoundedDouble) double.NaN,
                StandardDeviation = (RoundedDouble) 0.05
            };

            widthOfFlowApertures = new NormalDistribution(2)
            {
                Mean = (RoundedDouble) double.NaN,
            };
            widthOfFlowApertures.SetStandardDeviationFromVariationCoefficient(0.05);

            stormDuration = new LogNormalDistribution(2)
            {
                Mean = (RoundedDouble) 7.5
            };
            stormDuration.SetStandardDeviationFromVariationCoefficient(0.25);

            probabilityOpenStructureBeforeFlooding = 1.0;
        }

        #region Deterministic inputs

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
        /// Gets or sets the identical apertures to use during the calculation.
        /// </summary>
        public int IdenticalAperture { get; set; }

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
                    // TODO: refactor this in HeightStructures input to Common Forms Resources
                    throw new ArgumentException("De waarde voor de faalkans moet in het bereik tussen [0, 1] liggen.");
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
                    // TODO: refactor this in HeightStructures input to Common Forms Resources
                    throw new ArgumentException("De waarde voor de faalkans moet in het bereik tussen [0, 1] liggen.");
                }
                failureProbabilityReparation = value;
            }
        }

        /// <summary>
        /// Gets or sets the failure probability of structure given erosion.
        /// </summary>
        /// <exception cref="ArgumentException">Thrown when the value of the probability 
        /// is not between [0, 1].</exception>
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
                    // TODO: refactor this in HeightStructures input to Common Forms Resources
                    throw new ArgumentException("De waarde voor de faalkans moet in het bereik tussen [0, 1] liggen.");
                }
                failureProbabilityOfStructureGivenErosion = value;
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
                    // TODO: refactor this in HeightStructures input to Common Forms Resources
                    throw new ArgumentException("De waarde voor de faalkans moet in het bereik tussen [0, 1] liggen.");
                }
                probabilityOpenStructureBeforeFlooding = value;
            }
        }

        #endregion

        #region Probabilistic inputs

        /// <summary>
        /// Gets the drain coefficient normal distribution and sets the the drain coefficient mean.
        /// </summary>
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
        /// Gets the threshold low weir height normal distribution and sets the threshold low weir height mean.
        /// </summary>
        public NormalDistribution ThresholdLowWeirHeight
        {
            get
            {
                return thresholdLowWeirHeight;
            }
            set
            {
                thresholdLowWeirHeight.Mean = value.Mean;
            }
        }

        /// <summary>
        /// Gets and sets the area flow apertures normal distribution.
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
        public NormalDistribution LevelCrestOfStructureNotClosing
        {
            get
            {
                return levelCrestOfStructureNotClosing;
            }
            set
            {
                levelCrestOfStructureNotClosing.Mean = value.Mean;
                levelCrestOfStructureNotClosing.StandardDeviation = value.StandardDeviation;
            }
        }

        /// <summary>
        /// Gets or sets the water level inside normal distribution.
        /// </summary>
        public NormalDistribution WaterLevelInside
        {
            get
            {
                return waterLevelInside;
            }
            set
            {
                waterLevelInside.Mean = value.Mean;
                waterLevelInside.StandardDeviation = value.StandardDeviation;
            }
        }

        /// <summary>
        /// Gets or sets the allowable increase of level for storage log normal distribution.
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
        /// Gets or sets the width of flow apertures normal distribution.
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

        /// <summary>
        /// Gets the storm duration log normal distribution and sets the storm duration mean.
        /// </summary>
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
    }
}