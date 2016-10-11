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
using RingtoetsDataCommonProperties = Ringtoets.Common.Data.Properties.Resources;

namespace Ringtoets.StabilityPointStructures.Data
{
    /// <summary>
    /// Class that holds all stability point structures calculation specific input parameters.
    /// </summary>
    public class StabilityPointStructuresInput : Observable, ICalculationInput
    {
        private readonly NormalDistribution insideWaterLevelFailureConstruction;
        private readonly NormalDistribution insideWaterLevel;
        private readonly VariationCoefficientLogNormalDistribution stormDuration;
        private readonly NormalDistribution modelFactorSuperCriticalFlow;
        private readonly NormalDistribution drainCoefficient;
        private readonly NormalDistribution levelCrestStructure;
        private readonly NormalDistribution thresholdHeightOpenWeir;
        private readonly LogNormalDistribution areaFlowApertures;
        private readonly VariationCoefficientLogNormalDistribution constructiveStrengthLinearModel;
        private readonly VariationCoefficientLogNormalDistribution constructiveStrengthQuadraticModel;
        private readonly VariationCoefficientLogNormalDistribution stabilityLinearModel;
        private readonly VariationCoefficientLogNormalDistribution stabilityQuadraticModel;
        private readonly VariationCoefficientLogNormalDistribution failureCollisionEnergy;
        private readonly VariationCoefficientNormalDistribution shipMass;
        private readonly VariationCoefficientNormalDistribution shipVelocity;
        private readonly LogNormalDistribution allowedLevelIncreaseStorage;
        private readonly VariationCoefficientLogNormalDistribution storageStructureArea;
        private readonly LogNormalDistribution flowWidthAtBottomProtection;
        private readonly VariationCoefficientLogNormalDistribution criticalOvertoppingDischarge;
        private readonly VariationCoefficientNormalDistribution widthFlowApertures;
        private readonly NormalDistribution bankWidth;
        private readonly NormalDistribution flowVelocityStructureClosable;
        private ForeshoreProfile foreshoreProfile;
        private RoundedDouble structureNormalOrientation;
        private RoundedDouble volumicWeightWater;
        private RoundedDouble factorStormDurationOpenStructure;
        private RoundedDouble evaluationLevel;
        private RoundedDouble verticalDistance;
        private double failureProbabilityRepairClosure;
        private double probabilityCollisionSecondaryStructure;
        private double failureProbabilityStructureWithErosion;

        /// <summary>
        /// Creates a new instance of <see cref="StabilityPointStructuresInput"/>.
        /// </summary>
        public StabilityPointStructuresInput()
        {
            volumicWeightWater = new RoundedDouble(2, 9.81);
            structureNormalOrientation = new RoundedDouble(2, double.NaN);
            factorStormDurationOpenStructure = new RoundedDouble(2, double.NaN);
            failureProbabilityRepairClosure = double.NaN;
            probabilityCollisionSecondaryStructure = double.NaN;
            failureProbabilityStructureWithErosion = double.NaN;
            evaluationLevel = new RoundedDouble(2, 0);
            verticalDistance = new RoundedDouble(2, double.NaN);

            insideWaterLevelFailureConstruction = new NormalDistribution(2)
            {
                Mean = (RoundedDouble) double.NaN,
                StandardDeviation = (RoundedDouble) 0.1
            };

            insideWaterLevel = new NormalDistribution(2)
            {
                Mean = (RoundedDouble) double.NaN,
                StandardDeviation = (RoundedDouble) 0.1
            };

            stormDuration = new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = (RoundedDouble) 6.0,
                CoefficientOfVariation = (RoundedDouble) 0.25
            };

            modelFactorSuperCriticalFlow = new NormalDistribution(2)
            {
                Mean = (RoundedDouble) 1.1,
                StandardDeviation = (RoundedDouble) 0.03
            };

            drainCoefficient = new NormalDistribution(2)
            {
                Mean = (RoundedDouble) 1.0,
                StandardDeviation = (RoundedDouble) 0.2
            };

            flowVelocityStructureClosable = new NormalDistribution(2)
            {
                Mean = (RoundedDouble) double.NaN,
                StandardDeviation = (RoundedDouble) 1
            };

            levelCrestStructure = new NormalDistribution(2)
            {
                Mean = (RoundedDouble) double.NaN,
                StandardDeviation = (RoundedDouble) 0.05
            };

            thresholdHeightOpenWeir = new NormalDistribution(2)
            {
                Mean = (RoundedDouble) double.NaN,
                StandardDeviation = (RoundedDouble) 0.1
            };

            areaFlowApertures = new LogNormalDistribution(2)
            {
                Mean = (RoundedDouble) double.NaN,
                StandardDeviation = (RoundedDouble) 0.01
            };

            constructiveStrengthLinearModel = new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = (RoundedDouble) double.NaN,
                CoefficientOfVariation = (RoundedDouble) 0.1
            };

            constructiveStrengthQuadraticModel = new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = (RoundedDouble) double.NaN,
                CoefficientOfVariation = (RoundedDouble) 0.1
            };

            stabilityLinearModel = new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = (RoundedDouble) double.NaN,
                CoefficientOfVariation = (RoundedDouble) 0.1
            };

            stabilityQuadraticModel = new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = (RoundedDouble) double.NaN,
                CoefficientOfVariation = (RoundedDouble) 0.1
            };

            failureCollisionEnergy = new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = (RoundedDouble) double.NaN,
                CoefficientOfVariation = (RoundedDouble) 0.3
            };

            shipMass = new VariationCoefficientNormalDistribution(2)
            {
                Mean = (RoundedDouble) double.NaN,
                CoefficientOfVariation = (RoundedDouble) 0.2
            };

            shipVelocity = new VariationCoefficientNormalDistribution(2)
            {
                Mean = (RoundedDouble) double.NaN,
                CoefficientOfVariation = (RoundedDouble) 0.2
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

            bankWidth = new NormalDistribution(2)
            {
                Mean = (RoundedDouble) double.NaN,
                StandardDeviation = (RoundedDouble) double.NaN
            };

            UpdateForeshoreProperties();
        }

        private static bool ValidProbabilityValue(double probability)
        {
            return !double.IsNaN(probability) && probability >= 0 && probability <= 1;
        }

        #region Structure / calculation properties

        /// <summary>
        /// Gets or sets the stability point structure.
        /// </summary>
        public StabilityPointStructure StabilityPointStructure { get; set; }

        /// <summary>
        /// Gets or sets the type of stability point structure inflow model.
        /// </summary>
        public StabilityPointStructureInflowModelType InflowModelType { get; set; }

        /// <summary>
        /// Gets or sets the type of load schematization to use for the calculations.
        /// </summary>
        public LoadSchematizationType LoadSchematizationType { get; set; }

        #endregion

        #region Hydraulic data and loads

        /// <summary>
        /// Gets or sets the hydraulic boundary location from which to use the assessment level.
        /// </summary>
        public HydraulicBoundaryLocation HydraulicBoundaryLocation { get; set; }

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
        /// Gets or sets the volumic weight of water.
        /// [kN/m^3]
        /// </summary>
        public RoundedDouble VolumicWeightWater
        {
            get
            {
                return volumicWeightWater;
            }
            set
            {
                volumicWeightWater = value.ToPrecision(volumicWeightWater.NumberOfDecimalPlaces);
            }
        }

        /// <summary>
        /// Gets or sets the inside water level failure construction.
        /// [m+NAP]
        /// </summary>
        public NormalDistribution InsideWaterLevelFailureConstruction
        {
            get
            {
                return insideWaterLevelFailureConstruction;
            }
            set
            {
                insideWaterLevelFailureConstruction.Mean = value.Mean;
                insideWaterLevelFailureConstruction.StandardDeviation = value.StandardDeviation;
            }
        }

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

        #region Model Inputs and critical values

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

        /// <summary>
        /// Gets or sets the factor for the storm duration open structure.
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
        /// Gets or sets the flow velocity structure closable.
        /// [m/s]
        /// </summary>
        public NormalDistribution FlowVelocityStructureClosable
        {
            get
            {
                return flowVelocityStructureClosable;
            }
            set
            {
                flowVelocityStructureClosable.Mean = value.Mean;
                flowVelocityStructureClosable.StandardDeviation = value.StandardDeviation;
            }
        }

        #endregion

        #region Schematization

        /// <summary>
        /// Gets or sets the level crest of the structure.
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
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the value of the orientation is not between [0, 360] degrees.</exception>
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

                RoundedDouble newOrientation = value.ToPrecision(structureNormalOrientation.NumberOfDecimalPlaces);
                if (newOrientation < 0 || newOrientation > 360)
                {
                    throw new ArgumentOutOfRangeException("value", RingtoetsDataCommonProperties.Orientation_Value_needs_to_be_between_0_and_360);
                }
                structureNormalOrientation = newOrientation;
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
        /// Gets or sets the area flow apertures
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
        /// Gets or sets the constructive strength of the linear load model.
        /// [kN/m^2]
        /// </summary>
        public VariationCoefficientLogNormalDistribution ConstructiveStrengthLinearLoadModel
        {
            get
            {
                return constructiveStrengthLinearModel;
            }
            set
            {
                constructiveStrengthLinearModel.Mean = value.Mean;
                constructiveStrengthLinearModel.CoefficientOfVariation = value.CoefficientOfVariation;
            }
        }

        /// <summary>
        /// Gets or sets the constructive strength of the quadratic load model.
        /// [kN/m]
        /// </summary>
        public VariationCoefficientLogNormalDistribution ConstructiveStrengthQuadraticLoadModel
        {
            get
            {
                return constructiveStrengthQuadraticModel;
            }
            set
            {
                constructiveStrengthQuadraticModel.Mean = value.Mean;
                constructiveStrengthQuadraticModel.CoefficientOfVariation = value.CoefficientOfVariation;
            }
        }

        /// <summary>
        /// Gets or sets the stability properties of the linear model.
        /// [kN/m^2]
        /// </summary>
        public VariationCoefficientLogNormalDistribution StabilityLinearModel
        {
            get
            {
                return stabilityLinearModel;
            }
            set
            {
                stabilityLinearModel.Mean = value.Mean;
                stabilityLinearModel.CoefficientOfVariation = value.CoefficientOfVariation;
            }
        }

        /// <summary>
        /// Gets or sets the stability properties of the quadratic model.
        /// [kN/m]
        /// </summary>
        public VariationCoefficientLogNormalDistribution StabilityQuadraticModel
        {
            get
            {
                return stabilityQuadraticModel;
            }
            set
            {
                stabilityQuadraticModel.Mean = value.Mean;
                stabilityQuadraticModel.CoefficientOfVariation = value.CoefficientOfVariation;
            }
        }

        /// <summary>
        /// Gets or sets the failure probability of repairing a closure.
        /// [1/year]
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the probability is not in interval [0,1].</exception>
        public double FailureProbabilityRepairClosure
        {
            get
            {
                return failureProbabilityRepairClosure;
            }
            set
            {
                if (!ValidProbabilityValue(value))
                {
                    throw new ArgumentOutOfRangeException("value", RingtoetsDataCommonProperties.FailureProbability_Value_needs_to_be_between_0_and_1);
                }
                failureProbabilityRepairClosure = value;
            }
        }

        /// <summary>
        /// Gets or sets the failure collision energy.
        /// [kNm]
        /// </summary>
        public VariationCoefficientLogNormalDistribution FailureCollisionEnergy
        {
            get
            {
                return failureCollisionEnergy;
            }
            set
            {
                failureCollisionEnergy.Mean = value.Mean;
                failureCollisionEnergy.CoefficientOfVariation = value.CoefficientOfVariation;
            }
        }

        /// <summary>
        /// Gets or sets the mass of the ship.
        /// [tons]
        /// </summary>
        public VariationCoefficientNormalDistribution ShipMass
        {
            get
            {
                return shipMass;
            }
            set
            {
                shipMass.Mean = value.Mean;
                shipMass.CoefficientOfVariation = value.CoefficientOfVariation;
            }
        }

        /// <summary>
        /// Gets or sets the velocity of the ship.
        /// [m/s]
        /// </summary>
        public VariationCoefficientNormalDistribution ShipVelocity
        {
            get
            {
                return shipVelocity;
            }
            set
            {
                shipVelocity.Mean = value.Mean;
                shipVelocity.CoefficientOfVariation = value.CoefficientOfVariation;
            }
        }

        /// <summary>
        /// Gets or sets the levelling count.
        /// [1/year]
        /// </summary>
        public int LevellingCount { get; set; }

        /// <summary>
        /// Gets or sets the probability of a secondary collision on the structure.
        /// [1/levelling]
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the probability is not in interval [0,1].</exception>
        public double ProbabilityCollisionSecondaryStructure
        {
            get
            {
                return probabilityCollisionSecondaryStructure;
            }
            set
            {
                if (!ValidProbabilityValue(value))
                {
                    throw new ArgumentOutOfRangeException("value", RingtoetsDataCommonProperties.Probability_Must_be_in_range_zero_to_one);
                }
                probabilityCollisionSecondaryStructure = value;
            }
        }

        /// <summary>
        /// Gets or sets the allowed level increase of the storage volume.
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
        /// Gets or sets the failure probability of a structure with erosion.
        /// [1/year]
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the probability is not in interval [0,1].</exception>
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
                    throw new ArgumentOutOfRangeException("value", RingtoetsDataCommonProperties.FailureProbability_Value_needs_to_be_between_0_and_1);
                }
                failureProbabilityStructureWithErosion = value;
            }
        }

        /// <summary>
        /// Gets or sets the width of the flow apertures.
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
        /// Gets or sets the bank width.
        /// [m]
        /// </summary>
        public NormalDistribution BankWidth
        {
            get
            {
                return bankWidth;
            }
            set
            {
                bankWidth.Mean = value.Mean;
                bankWidth.StandardDeviation = value.StandardDeviation;
            }
        }

        /// <summary>
        /// Gets or sets the evaluation level.
        /// [m+NAP]
        /// </summary>
        public RoundedDouble EvaluationLevel
        {
            get
            {
                return evaluationLevel;
            }
            set
            {
                evaluationLevel = value.ToPrecision(evaluationLevel.NumberOfDecimalPlaces);
            }
        }

        /// <summary>
        /// Gets or sets the vertical distance.
        /// [m]
        /// </summary>
        public RoundedDouble VerticalDistance
        {
            get
            {
                return verticalDistance;
            }
            set
            {
                verticalDistance = value.ToPrecision(verticalDistance.NumberOfDecimalPlaces);
            }
        }

        #endregion
    }
}