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
using System.Linq;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.Common.Data.Probability;
using Ringtoets.Common.Data.Properties;

namespace Ringtoets.Common.Data.Structures
{
    /// <summary>
    /// Base class that holds generic structures calculation input parameters.
    /// </summary>
    public abstract class StructuresInputBase<T> : Observable, IStructuresCalculationInput<T>, IUseBreakWater, IUseForeshore, IHasForeshoreProfile
        where T : StructureBase
    {
        private const int structureNormalOrientationNumberOfDecimals = 2;

        private static readonly Range<RoundedDouble> structureNormalOrientationValidityRange = new Range<RoundedDouble>(new RoundedDouble(structureNormalOrientationNumberOfDecimals),
                                                                                                                        new RoundedDouble(structureNormalOrientationNumberOfDecimals, 360));

        private readonly NormalDistribution modelFactorSuperCriticalFlow;
        private readonly LogNormalDistribution allowedLevelIncreaseStorage;
        private readonly VariationCoefficientLogNormalDistribution storageStructureArea;
        private readonly LogNormalDistribution flowWidthAtBottomProtection;
        private readonly VariationCoefficientLogNormalDistribution criticalOvertoppingDischarge;
        private readonly NormalDistribution widthFlowApertures;
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
            modelFactorSuperCriticalFlow = new NormalDistribution(2)
            {
                Mean = (RoundedDouble) 1.1,
                StandardDeviation = (RoundedDouble) 0.03
            };

            stormDuration = new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = (RoundedDouble) 6.0,
                CoefficientOfVariation = (RoundedDouble) 0.25
            };

            structureNormalOrientation = new RoundedDouble(structureNormalOrientationNumberOfDecimals);
            allowedLevelIncreaseStorage = new LogNormalDistribution(2);
            storageStructureArea = new VariationCoefficientLogNormalDistribution(2);
            flowWidthAtBottomProtection = new LogNormalDistribution(2);
            criticalOvertoppingDischarge = new VariationCoefficientLogNormalDistribution(2);
            failureProbabilityStructureWithErosion = 1.0;
            widthFlowApertures = new NormalDistribution(2);

            SetDefaultCommonStructureSchematizationProperties();
            SynchronizeForeshoreProfileParameters();
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
        /// Gets whether the structure input parameters are synchronized with the set <see cref="StructuresInputBase{T}.Structure"/>.
        /// </summary>
        /// <remarks>Always returns <c>false</c> in case no structure is present.</remarks>
        public abstract bool StructureParametersSynchronized { get; }

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
                if (structure == null)
                {
                    SetDefaultCommonStructureSchematizationProperties();
                }

                SynchronizeStructureParameters();
            }
        }

        /// <summary>
        /// Synchronizes the input parameters with the parameters of the structure.
        /// </summary>
        /// <remarks>When no structure is present, the input parameters are set to default values.</remarks>
        public abstract void SynchronizeStructureParameters();

        private void SetDefaultCommonStructureSchematizationProperties()
        {
            StructureNormalOrientation = RoundedDouble.NaN;

            AllowedLevelIncreaseStorage = new LogNormalDistribution
            {
                Mean = RoundedDouble.NaN,
                StandardDeviation = RoundedDouble.NaN
            };

            StorageStructureArea = new VariationCoefficientLogNormalDistribution
            {
                Mean = RoundedDouble.NaN,
                CoefficientOfVariation = RoundedDouble.NaN
            };

            FlowWidthAtBottomProtection = new LogNormalDistribution
            {
                Mean = RoundedDouble.NaN,
                StandardDeviation = RoundedDouble.NaN
            };

            CriticalOvertoppingDischarge = new VariationCoefficientLogNormalDistribution
            {
                Mean = RoundedDouble.NaN,
                CoefficientOfVariation = RoundedDouble.NaN
            };

            WidthFlowApertures = new NormalDistribution
            {
                Mean = RoundedDouble.NaN,
                StandardDeviation = RoundedDouble.NaN
            };
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
                if (!double.IsNaN(newStructureNormalOrientation) && !structureNormalOrientationValidityRange.InRange(newStructureNormalOrientation))
                {
                    throw new ArgumentOutOfRangeException(null, string.Format(Resources.Orientation_Value_needs_to_be_in_Range_0_,
                                                                              structureNormalOrientationValidityRange));
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
                ProbabilityHelper.ValidateProbability(value, null, Resources.FailureProbability_Value_needs_to_be_in_Range_0_);
                failureProbabilityStructureWithErosion = value;
            }
        }

        /// <summary>
        /// Gets or sets the width of flow apertures.
        /// [m]
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
                SynchronizeForeshoreProfileParameters();
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

        private static BreakWater GetDefaultBreakWater()
        {
            return new BreakWater(BreakWaterType.Dam, 0.0);
        }

        public void ClearStructure()
        {
            Structure = null;
        }

        public bool IsForeshoreProfileParametersSynchronized
        {
            get
            {
                if (foreshoreProfile == null)
                {
                    return false;
                }

                return
                    UseForeshore == foreshoreProfile.Geometry.Count() > 1
                    && UseBreakWater == foreshoreProfile.HasBreakWater
                    && BreakWater.Equals(foreshoreProfile.BreakWater);
            }
        }

        public void SynchronizeForeshoreProfileParameters()
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

        #endregion
    }
}