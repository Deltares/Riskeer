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

namespace Ringtoets.HeightStructures.Data
{
    /// <summary>
    /// Class that holds all height structures calculation specific input parameters.
    /// </summary>
    public class HeightStructuresInput : Observable, ICalculationInput, IUseBreakWater, IUseForeshore
    {
        private readonly NormalDistribution levelCrestStructure;
        private readonly NormalDistribution modelFactorSuperCriticalFlow;
        private readonly LogNormalDistribution allowedLevelIncreaseStorage;
        private readonly VariationCoefficientLogNormalDistribution storageStructureArea;
        private readonly LogNormalDistribution flowWidthAtBottomProtection;
        private readonly VariationCoefficientLogNormalDistribution criticalOvertoppingDischarge;
        private readonly VariationCoefficientNormalDistribution widthFlowApertures;
        private readonly VariationCoefficientLogNormalDistribution stormDuration;
        private RoundedDouble structureNormalOrientation;
        private RoundedDouble deviationWaveDirection;
        private double failureProbabilityStructureWithErosion;
        private ForeshoreProfile foreshoreProfile;
        private HeightStructure heightStructure;

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

            structureNormalOrientation = new RoundedDouble(2, double.NaN);

            modelFactorSuperCriticalFlow = new NormalDistribution(2)
            {
                Mean = (RoundedDouble) 1.1,
                StandardDeviation = (RoundedDouble) 0.03
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

            failureProbabilityStructureWithErosion = double.NaN;

            widthFlowApertures = new VariationCoefficientNormalDistribution(2)
            {
                Mean = (RoundedDouble) double.NaN,
                CoefficientOfVariation = (RoundedDouble) 0.05
            };

            deviationWaveDirection = new RoundedDouble(2, double.NaN);

            stormDuration = new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = (RoundedDouble) 6.0,
                CoefficientOfVariation = (RoundedDouble) 0.25
            };

            UpdateHeightStructureProperties();
            UpdateForeshoreProperties();
        }

        private static bool ValidProbabilityValue(double probability)
        {
            return !double.IsNaN(probability) && probability <= 1 && probability >= 0;
        }

        #region Model factors

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

        #endregion

        #region Hydraulic data

        /// <summary>
        /// Gets or sets the hydraulic boundary location from which to use the assessment level.
        /// </summary>
        public HydraulicBoundaryLocation HydraulicBoundaryLocation { get; set; }

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

        #region Schematisation

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

        /// <summary>
        /// Gets or sets the orientation of the normal of the structure.
        /// [degrees]
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the value for the orientation is not between [0, 360] degrees.</exception>
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
        /// Gets or sets the allowed increase of level for the storage.
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
        /// Gets or sets the height structure.
        /// </summary>
        public HeightStructure HeightStructure
        {
            get
            {
                return heightStructure;
            }
            set
            {
                heightStructure = value;
                UpdateHeightStructureProperties();
            }
        }

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

        public bool UseBreakWater { get; set; }

        public bool UseForeshore { get; set; }

        public RoundedPoint2DCollection ForeshoreGeometry
        {
            get
            {
                return foreshoreProfile != null
                           ? foreshoreProfile.Geometry
                           : new RoundedPoint2DCollection(2, Enumerable.Empty<Point2D>());
            }
        }

        public BreakWater BreakWater { get; private set; }

        private void UpdateHeightStructureProperties()
        {
            if (heightStructure != null)
            {
                StructureNormalOrientation = heightStructure.StructureNormalOrientation;
                LevelCrestStructure = heightStructure.LevelCrestStructure;
                FlowWidthAtBottomProtection = heightStructure.FlowWidthAtBottomProtection;
                CriticalOvertoppingDischarge = heightStructure.CriticalOvertoppingDischarge;
                WidthFlowApertures = heightStructure.WidthFlowApertures;
                FailureProbabilityStructureWithErosion = heightStructure.FailureProbabilityStructureWithErosion;
                StorageStructureArea = heightStructure.StorageStructureArea;
                AllowedLevelIncreaseStorage = heightStructure.AllowedLevelIncreaseStorage;
            }
        }

        private void UpdateForeshoreProperties()
        {
            if (foreshoreProfile == null)
            {
                UseForeshore = false;
                UseBreakWater = false;
                BreakWater = GetDefaultBreakWater();
            }
            else
            {
                UseForeshore = foreshoreProfile.Geometry.Count() > 1;
                UseBreakWater = foreshoreProfile.HasBreakWater;
                BreakWater = foreshoreProfile.HasBreakWater ?
                                 new BreakWater(foreshoreProfile.BreakWater.Type, foreshoreProfile.BreakWater.Height) :
                                 GetDefaultBreakWater();
            }
        }

        private static BreakWater GetDefaultBreakWater()
        {
            return new BreakWater(BreakWaterType.Dam, 0.0);
        }

        #endregion
    }
}