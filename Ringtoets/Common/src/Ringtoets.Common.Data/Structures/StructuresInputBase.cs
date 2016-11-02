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
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.Common.Data.Properties;
using Ringtoets.HydraRing.Data;

namespace Ringtoets.Common.Data.Structures
{
    /// <summary>
    /// Base class that holds generic structures calculation input parameters.
    /// </summary>
    public abstract class StructuresInputBase<T> : Observable, IStructuresCalculationInput<T>, IUseBreakWater, IUseForeshore
        where T : StructureBase
    {
        private readonly NormalDistribution modelFactorSuperCriticalFlow;
        private readonly LogNormalDistribution allowedLevelIncreaseStorage;
        private readonly VariationCoefficientLogNormalDistribution storageStructureArea;
        private readonly LogNormalDistribution flowWidthAtBottomProtection;
        private readonly VariationCoefficientLogNormalDistribution criticalOvertoppingDischarge;
        private readonly VariationCoefficientNormalDistribution widthFlowApertures;
        private readonly VariationCoefficientLogNormalDistribution stormDuration;

        private T structure;

        private RoundedDouble structureNormalOrientation;
        private double failureProbabilityStructureWithErosion;

        private ForeshoreProfile foreshoreProfile;

        /// <summary>
        /// Creates a new instance of the <see cref="StructuresInputBase{T}"/> class.
        /// </summary>
        protected StructuresInputBase()
        {
            structureNormalOrientation = new RoundedDouble(2, double.NaN);

            modelFactorSuperCriticalFlow = new NormalDistribution(2)
            {
                Mean = (RoundedDouble) 1.1,
                StandardDeviation = (RoundedDouble) 0.03
            };

            allowedLevelIncreaseStorage = new LogNormalDistribution(2)
            {
                Mean = RoundedDouble.NaN,
                StandardDeviation = (RoundedDouble) 0.1
            };

            storageStructureArea = new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = RoundedDouble.NaN,
                CoefficientOfVariation = (RoundedDouble) 0.1
            };

            flowWidthAtBottomProtection = new LogNormalDistribution(2)
            {
                Mean = RoundedDouble.NaN,
                StandardDeviation = (RoundedDouble) 0.05
            };

            criticalOvertoppingDischarge = new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = RoundedDouble.NaN,
                CoefficientOfVariation = (RoundedDouble) 0.15
            };

            failureProbabilityStructureWithErosion = 1.0;

            widthFlowApertures = new VariationCoefficientNormalDistribution(2)
            {
                Mean = RoundedDouble.NaN,
                CoefficientOfVariation = (RoundedDouble) 0.05
            };

            stormDuration = new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = (RoundedDouble) 6.0,
                CoefficientOfVariation = (RoundedDouble) 0.25
            };

            UpdateProfileParameters();
        }

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

        /// <summary>
        /// Gets or sets the structure.
        /// </summary>
        public T Structure
        {
            get
            {
                return structure;
            }
            set
            {
                structure = value;
                UpdateStructureParameters();
            }
        }

        /// <summary>
        /// Synchronizes the input parameters with the parameters of the structure.
        /// </summary>
        protected abstract void UpdateStructureParameters();

        /// <summary>
        /// Validates the provided probability value.
        /// </summary>
        /// <param name="probability">The probability value to validate.</param>
        /// <returns><c>True</c> when the provided value is valid, <c>false</c> otherwise.</returns>
        protected static bool ValidProbabilityValue(double probability)
        {
            return !double.IsNaN(probability) && probability <= 1 && probability >= 0;
        }

        #region Hydraulic data

        /// <summary>
        /// Gets or sets the hydraulic boundary location.
        /// </summary>
        public HydraulicBoundaryLocation HydraulicBoundaryLocation { get; set; }

        /// <summary>
        /// Gets or sets the storm duration.
        /// [h]
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
        ///<exception cref="ArgumentOutOfRangeException">Thrown when the value of the orientation
        /// is not in the interval [0, 360].</exception>
        public RoundedDouble StructureNormalOrientation
        {
            get
            {
                return structureNormalOrientation;
            }
            set
            {
                RoundedDouble newStructureNormalOrientation = value.ToPrecision(structureNormalOrientation.NumberOfDecimalPlaces);
                if (!double.IsNaN(newStructureNormalOrientation) && (newStructureNormalOrientation < 0 || newStructureNormalOrientation > 360))
                {
                    throw new ArgumentOutOfRangeException("value", Resources.Orientation_Value_needs_to_be_between_0_and_360);
                }
                structureNormalOrientation = newStructureNormalOrientation;
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
        /// Gets or sets the critical overtopping discharge per meter.
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
        /// is not in the interval [0, 1].</exception>
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
                    throw new ArgumentOutOfRangeException("value", Resources.FailureProbability_Value_needs_to_be_between_0_and_1);
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

        #endregion

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
                UpdateProfileParameters();
            }
        }

        public bool UseBreakWater { get; set; }

        public BreakWater BreakWater { get; private set; }

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

        private void UpdateProfileParameters()
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