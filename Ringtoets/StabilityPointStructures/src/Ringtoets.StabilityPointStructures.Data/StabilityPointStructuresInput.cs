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
using Core.Common.Base.Data;
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.Common.Data.Probability;
using Ringtoets.Common.Data.Structures;
using RingtoetsDataCommonResources = Ringtoets.Common.Data.Properties.Resources;

namespace Ringtoets.StabilityPointStructures.Data
{
    /// <summary>
    /// Class that holds all stability point structures calculation specific input parameters.
    /// </summary>
    public class StabilityPointStructuresInput : StructuresInputBase<StabilityPointStructure>
    {
        private readonly NormalDistribution insideWaterLevelFailureConstruction;
        private readonly NormalDistribution insideWaterLevel;
        private readonly NormalDistribution drainCoefficient;
        private readonly NormalDistribution levelCrestStructure;
        private readonly NormalDistribution thresholdHeightOpenWeir;
        private readonly LogNormalDistribution areaFlowApertures;
        private readonly VariationCoefficientLogNormalDistribution constructiveStrengthLinearLoadModel;
        private readonly VariationCoefficientLogNormalDistribution constructiveStrengthQuadraticLoadModel;
        private readonly VariationCoefficientLogNormalDistribution stabilityLinearLoadModel;
        private readonly VariationCoefficientLogNormalDistribution stabilityQuadraticLoadModel;
        private readonly VariationCoefficientLogNormalDistribution failureCollisionEnergy;
        private readonly VariationCoefficientNormalDistribution shipMass;
        private readonly VariationCoefficientNormalDistribution shipVelocity;
        private readonly NormalDistribution bankWidth;
        private readonly VariationCoefficientNormalDistribution flowVelocityStructureClosable;
        private RoundedDouble volumicWeightWater;
        private RoundedDouble factorStormDurationOpenStructure;
        private RoundedDouble evaluationLevel;
        private RoundedDouble verticalDistance;
        private double failureProbabilityRepairClosure;
        private double probabilityCollisionSecondaryStructure;

        /// <summary>
        /// Creates a new instance of <see cref="StabilityPointStructuresInput"/>.
        /// </summary>
        public StabilityPointStructuresInput()
        {
            volumicWeightWater = new RoundedDouble(2, 9.81);
            factorStormDurationOpenStructure = new RoundedDouble(2, 1.0);
            failureProbabilityRepairClosure = 0;

            evaluationLevel = new RoundedDouble(2);
            verticalDistance = new RoundedDouble(2);

            drainCoefficient = new NormalDistribution(2)
            {
                Mean = (RoundedDouble) 1.0,
                StandardDeviation = (RoundedDouble) 0.20
            };

            insideWaterLevelFailureConstruction = new NormalDistribution(2);
            insideWaterLevel = new NormalDistribution(2);
            flowVelocityStructureClosable = new VariationCoefficientNormalDistribution(2);

            levelCrestStructure = new NormalDistribution(2);
            thresholdHeightOpenWeir = new NormalDistribution(2);
            areaFlowApertures = new LogNormalDistribution(2);
            constructiveStrengthLinearLoadModel = new VariationCoefficientLogNormalDistribution(2);
            constructiveStrengthQuadraticLoadModel = new VariationCoefficientLogNormalDistribution(2);
            stabilityLinearLoadModel = new VariationCoefficientLogNormalDistribution(2);
            stabilityQuadraticLoadModel = new VariationCoefficientLogNormalDistribution(2);
            failureCollisionEnergy = new VariationCoefficientLogNormalDistribution(2);
            shipMass = new VariationCoefficientNormalDistribution(2);
            shipVelocity = new VariationCoefficientNormalDistribution(2);
            bankWidth = new NormalDistribution(2);

            SetDefaultSchematizationProperties();
        }

        protected override void UpdateStructureParameters()
        {
            if (Structure != null)
            {
                AllowedLevelIncreaseStorage = Structure.AllowedLevelIncreaseStorage;
                AreaFlowApertures = Structure.AreaFlowApertures;
                BankWidth = Structure.BankWidth;
                ConstructiveStrengthLinearLoadModel = Structure.ConstructiveStrengthLinearLoadModel;
                ConstructiveStrengthQuadraticLoadModel = Structure.ConstructiveStrengthQuadraticLoadModel;
                CriticalOvertoppingDischarge = Structure.CriticalOvertoppingDischarge;
                EvaluationLevel = Structure.EvaluationLevel;
                FailureCollisionEnergy = Structure.FailureCollisionEnergy;
                FailureProbabilityRepairClosure = Structure.FailureProbabilityRepairClosure;
                FlowVelocityStructureClosable = Structure.FlowVelocityStructureClosable;
                FlowWidthAtBottomProtection = Structure.FlowWidthAtBottomProtection;
                InflowModelType = Structure.InflowModelType;
                InsideWaterLevel = Structure.InsideWaterLevel;
                InsideWaterLevelFailureConstruction = Structure.InsideWaterLevelFailureConstruction;
                LevelCrestStructure = Structure.LevelCrestStructure;
                LevellingCount = Structure.LevellingCount;
                ProbabilityCollisionSecondaryStructure = Structure.ProbabilityCollisionSecondaryStructure;
                ShipMass = Structure.ShipMass;
                ShipVelocity = Structure.ShipVelocity;
                StabilityLinearLoadModel = Structure.StabilityLinearLoadModel;
                StabilityQuadraticLoadModel = Structure.StabilityQuadraticLoadModel;
                StorageStructureArea = Structure.StorageStructureArea;
                ThresholdHeightOpenWeir = Structure.ThresholdHeightOpenWeir;
                VerticalDistance = Structure.VerticalDistance;
                WidthFlowApertures = Structure.WidthFlowApertures;
                StructureNormalOrientation = Structure.StructureNormalOrientation;
            }
            else
            {
                SetDefaultSchematizationProperties();
            }
        }

        private void SetDefaultSchematizationProperties()
        {
            EvaluationLevel = (RoundedDouble) double.NaN;
            FailureProbabilityRepairClosure = (RoundedDouble) 0.0;
            InflowModelType = 0;
            LoadSchematizationType = LoadSchematizationType.Linear;
            StructureNormalOrientation = (RoundedDouble) double.NaN;
            VerticalDistance = (RoundedDouble) double.NaN;
            LevellingCount = 0;
            ProbabilityCollisionSecondaryStructure = 0.0;

            LevelCrestStructure = new NormalDistribution
            {
                Mean = RoundedDouble.NaN,
                StandardDeviation = RoundedDouble.NaN
            };

            ThresholdHeightOpenWeir = new NormalDistribution
            {
                Mean = RoundedDouble.NaN,
                StandardDeviation = RoundedDouble.NaN
            };

            AreaFlowApertures = new LogNormalDistribution(2)
            {
                Mean = RoundedDouble.NaN,
                StandardDeviation = RoundedDouble.NaN
            };

            ConstructiveStrengthLinearLoadModel = new VariationCoefficientLogNormalDistribution
            {
                Mean = RoundedDouble.NaN,
                CoefficientOfVariation = RoundedDouble.NaN
            };

            ConstructiveStrengthQuadraticLoadModel = new VariationCoefficientLogNormalDistribution
            {
                Mean = RoundedDouble.NaN,
                CoefficientOfVariation = RoundedDouble.NaN
            };

            StabilityLinearLoadModel = new VariationCoefficientLogNormalDistribution
            {
                Mean = RoundedDouble.NaN,
                CoefficientOfVariation = RoundedDouble.NaN
            };

            StabilityQuadraticLoadModel = new VariationCoefficientLogNormalDistribution
            {
                Mean = RoundedDouble.NaN,
                CoefficientOfVariation = RoundedDouble.NaN
            };

            FailureCollisionEnergy = new VariationCoefficientLogNormalDistribution
            {
                Mean = RoundedDouble.NaN,
                CoefficientOfVariation = RoundedDouble.NaN
            };

            ShipMass = new VariationCoefficientNormalDistribution
            {
                Mean = RoundedDouble.NaN,
                CoefficientOfVariation = RoundedDouble.NaN
            };

            ShipVelocity = new VariationCoefficientNormalDistribution
            {
                Mean = RoundedDouble.NaN,
                CoefficientOfVariation = RoundedDouble.NaN
            };

            BankWidth = new NormalDistribution
            {
                Mean = RoundedDouble.NaN,
                StandardDeviation = RoundedDouble.NaN
            };

            InsideWaterLevel = new NormalDistribution
            {
                Mean = RoundedDouble.NaN,
                StandardDeviation = RoundedDouble.NaN
            };

            InsideWaterLevelFailureConstruction = new NormalDistribution
            {
                Mean = RoundedDouble.NaN,
                StandardDeviation = RoundedDouble.NaN
            };

            FlowVelocityStructureClosable = new VariationCoefficientNormalDistribution
            {
                Mean = RoundedDouble.NaN
            };
            flowVelocityStructureClosable.CoefficientOfVariation = (RoundedDouble) 0.2;
        }

        #region Structure / calculation

        /// <summary>
        /// Gets or sets the type of stability point structure inflow model.
        /// </summary>
        public StabilityPointStructureInflowModelType InflowModelType { get; set; }

        /// <summary>
        /// Gets or sets the type of load schematization to use for the calculations.
        /// </summary>
        public LoadSchematizationType LoadSchematizationType { get; set; }

        #endregion

        #region Hydraulic data

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

        #endregion

        #region Model factors and critical values

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
        /// <remarks>Only sets the mean.</remarks>
        public VariationCoefficientNormalDistribution FlowVelocityStructureClosable
        {
            get
            {
                return flowVelocityStructureClosable;
            }
            set
            {
                flowVelocityStructureClosable.Mean = value.Mean;
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
                return constructiveStrengthLinearLoadModel;
            }
            set
            {
                constructiveStrengthLinearLoadModel.Mean = value.Mean;
                constructiveStrengthLinearLoadModel.CoefficientOfVariation = value.CoefficientOfVariation;
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
                return constructiveStrengthQuadraticLoadModel;
            }
            set
            {
                constructiveStrengthQuadraticLoadModel.Mean = value.Mean;
                constructiveStrengthQuadraticLoadModel.CoefficientOfVariation = value.CoefficientOfVariation;
            }
        }

        /// <summary>
        /// Gets or sets the stability properties of the linear load model.
        /// [kN/m^2]
        /// </summary>
        public VariationCoefficientLogNormalDistribution StabilityLinearLoadModel
        {
            get
            {
                return stabilityLinearLoadModel;
            }
            set
            {
                stabilityLinearLoadModel.Mean = value.Mean;
                stabilityLinearLoadModel.CoefficientOfVariation = value.CoefficientOfVariation;
            }
        }

        /// <summary>
        /// Gets or sets the stability properties of the quadratic load model.
        /// [kN/m]
        /// </summary>
        public VariationCoefficientLogNormalDistribution StabilityQuadraticLoadModel
        {
            get
            {
                return stabilityQuadraticLoadModel;
            }
            set
            {
                stabilityQuadraticLoadModel.Mean = value.Mean;
                stabilityQuadraticLoadModel.CoefficientOfVariation = value.CoefficientOfVariation;
            }
        }

        /// <summary>
        /// Gets or sets the failure probability of repairing a closure.
        /// [1/year]
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the value of the probability
        /// is not in the interval [0, 1].</exception>
        public double FailureProbabilityRepairClosure
        {
            get
            {
                return failureProbabilityRepairClosure;
            }
            set
            {
                ProbabilityHelper.ValidateProbability(value, null, RingtoetsDataCommonResources.FailureProbability_Value_needs_to_be_in_Range_0_);
                failureProbabilityRepairClosure = value;
            }
        }

        /// <summary>
        /// Gets or sets the failure collision energy.
        /// [kN m]
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
        /// [ton]
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
        /// Gets or sets the probability of a secondary collision on the structure per levelling.
        /// [1/year/levelling]
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the value of the probability
        /// is not in the interval [0, 1].</exception>
        public double ProbabilityCollisionSecondaryStructure
        {
            get
            {
                return probabilityCollisionSecondaryStructure;
            }
            set
            {
                ProbabilityHelper.ValidateProbability(value, nameof(value));
                probabilityCollisionSecondaryStructure = value;
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