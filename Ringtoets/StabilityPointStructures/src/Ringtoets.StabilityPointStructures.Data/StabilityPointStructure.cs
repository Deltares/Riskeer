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
using Core.Common.Base.Data;
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.Probabilistics;
using BaseConstructionProperties = Ringtoets.Common.Data.StructureBase.ConstructionProperties;

namespace Ringtoets.StabilityPointStructures.Data
{
    /// <summary>
    /// Definition of a stability point structure for the <see cref="StabilityPointStructuresFailureMechanism"/>.
    /// </summary>
    public class StabilityPointStructure : StructureBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StabilityPointStructure"/> class.
        /// </summary>
        /// <param name="constructionProperties">The construction properties.</param>
        /// <exception cref="ArgumentException">Thrown when <see cref="ConstructionProperties.Name"/>
        /// or <see cref="ConstructionProperties.Id"/> is <c>null</c>, empty or consists of whitespace.</exception>
        /// <exception cref="ArgumentNullException">Thrown when <see cref="ConstructionProperties.Location"/> is <c>null</c>.</exception>
        public StabilityPointStructure(ConstructionProperties constructionProperties) : base(constructionProperties)
        {
            StorageStructureArea = new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = constructionProperties.StorageStructureArea.Mean,
                CoefficientOfVariation = constructionProperties.StorageStructureArea.CoefficientOfVariation
            };
            AllowedLevelIncreaseStorage = new LogNormalDistribution(2)
            {
                Mean = constructionProperties.AllowedLevelIncreaseStorage.Mean,
                StandardDeviation = constructionProperties.AllowedLevelIncreaseStorage.StandardDeviation
            };
            WidthFlowApertures = new VariationCoefficientNormalDistribution(2)
            {
                Mean = constructionProperties.WidthFlowApertures.Mean,
                CoefficientOfVariation = constructionProperties.WidthFlowApertures.CoefficientOfVariation
            };
            InsideWaterLevel = new NormalDistribution(2)
            {
                Mean = constructionProperties.InsideWaterLevel.Mean,
                StandardDeviation = constructionProperties.InsideWaterLevel.StandardDeviation
            };
            ThresholdHeightOpenWeir = new NormalDistribution(2)
            {
                Mean = constructionProperties.ThresholdHeightOpenWeir.Mean,
                StandardDeviation = constructionProperties.ThresholdHeightOpenWeir.StandardDeviation
            };
            CriticalOvertoppingDischarge = new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = constructionProperties.CriticalOvertoppingDischarge.Mean,
                CoefficientOfVariation = constructionProperties.CriticalOvertoppingDischarge.CoefficientOfVariation
            };
            FlowWidthAtBottomProtection = new LogNormalDistribution(2)
            {
                Mean = constructionProperties.FlowWidthAtBottomProtection.Mean,
                StandardDeviation = constructionProperties.FlowWidthAtBottomProtection.StandardDeviation
            };
            ConstructiveStrengthLinearLoadModel = new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = constructionProperties.ConstructiveStrengthLinearLoadModel.Mean,
                CoefficientOfVariation = constructionProperties.ConstructiveStrengthLinearLoadModel.CoefficientOfVariation
            };
            ConstructiveStrengthQuadraticLoadModel = new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = constructionProperties.ConstructiveStrengthQuadraticLoadModel.Mean,
                CoefficientOfVariation = constructionProperties.ConstructiveStrengthQuadraticLoadModel.CoefficientOfVariation
            };
            BankWidth = new NormalDistribution(2)
            {
                Mean = constructionProperties.BankWidth.Mean,
                StandardDeviation = constructionProperties.BankWidth.StandardDeviation
            };
            InsideWaterLevelFailureConstruction = new NormalDistribution(2)
            {
                Mean = constructionProperties.InsideWaterLevelFailureConstruction.Mean,
                StandardDeviation = constructionProperties.InsideWaterLevelFailureConstruction.StandardDeviation
            };
            EvaluationLevel = new RoundedDouble(2, constructionProperties.EvaluationLevel);
            LevelCrestStructure = new NormalDistribution(2)
            {
                Mean = constructionProperties.LevelCrestStructure.Mean,
                StandardDeviation = constructionProperties.LevelCrestStructure.StandardDeviation
            };
            VerticalDistance = new RoundedDouble(2, constructionProperties.VerticalDistance);
            FailureProbabilityRepairClosure = constructionProperties.FailureProbabilityRepairClosure;
            FailureCollisionEnergy = new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = constructionProperties.FailureCollisionEnergy.Mean,
                CoefficientOfVariation = constructionProperties.FailureCollisionEnergy.CoefficientOfVariation
            };
            ShipMass = new VariationCoefficientNormalDistribution(2)
            {
                Mean = constructionProperties.ShipMass.Mean,
                CoefficientOfVariation = constructionProperties.ShipMass.CoefficientOfVariation
            };
            ShipVelocity = new VariationCoefficientNormalDistribution(2)
            {
                Mean = constructionProperties.ShipVelocity.Mean,
                CoefficientOfVariation = constructionProperties.ShipVelocity.CoefficientOfVariation
            };
            LevellingCount = constructionProperties.LevellingCount;
            ProbabilityCollisionSecondaryStructure = constructionProperties.ProbabilityCollisionSecondaryStructure;
            FlowVelocityStructureClosable = new NormalDistribution(2)
            {
                Mean = constructionProperties.FlowVelocityStructureClosable.Mean,
                StandardDeviation = constructionProperties.FlowVelocityStructureClosable.StandardDeviation
            };
            StabilityLinearLoadModel = new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = constructionProperties.StabilityLinearLoadModel.Mean,
                CoefficientOfVariation = constructionProperties.StabilityLinearLoadModel.CoefficientOfVariation
            };
            StabilityQuadraticLoadModel = new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = constructionProperties.StabilityQuadraticLoadModel.Mean,
                CoefficientOfVariation = constructionProperties.StabilityQuadraticLoadModel.CoefficientOfVariation
            };
            AreaFlowApertures = new LogNormalDistribution(2)
            {
                Mean = constructionProperties.AreaFlowApertures.Mean,
                StandardDeviation = constructionProperties.AreaFlowApertures.StandardDeviation
            };
            InflowModelType = constructionProperties.InflowModelType;
        }

        /// <summary>
        /// Gets the storage area of the stability point structure.
        /// [m^2]
        /// </summary>
        public VariationCoefficientLogNormalDistribution StorageStructureArea { get; private set; }

        /// <summary>
        /// Gets the allowed increase of level for storage of the stability point structure.
        /// [m]
        /// </summary>
        public LogNormalDistribution AllowedLevelIncreaseStorage { get; private set; }

        /// <summary>
        /// Gets the width of the flow apertures of the stability point structure.
        /// [m]
        /// </summary>
        public VariationCoefficientNormalDistribution WidthFlowApertures { get; private set; }

        /// <summary>
        /// Gets the interior water level of the stability point structure.
        /// [m+NAP]
        /// </summary>
        public NormalDistribution InsideWaterLevel { get; private set; }

        /// <summary>
        /// Gets the threshold height of the opened stability point structure.
        /// [m+NAP]
        /// </summary>
        public NormalDistribution ThresholdHeightOpenWeir { get; private set; }

        /// <summary>
        /// Gets the critical overtopping discharge per meter of the stability point structure.
        /// [m^3/s/m]
        /// </summary>
        public VariationCoefficientLogNormalDistribution CriticalOvertoppingDischarge { get; private set; }

        /// <summary>
        /// Gets the flow width of the stability point structure at the bottom protection.
        /// [m]
        /// </summary>
        public LogNormalDistribution FlowWidthAtBottomProtection { get; private set; }

        /// <summary>
        /// Gets the constructive strength of the linear load model of the stability point structure.
        /// [kN/m^2]
        /// </summary>
        public VariationCoefficientLogNormalDistribution ConstructiveStrengthLinearLoadModel { get; private set; }

        /// <summary>
        /// Gets the constructive strength of the quadratic load model of the stability point structure.
        /// [kN/m]
        /// </summary>
        public VariationCoefficientLogNormalDistribution ConstructiveStrengthQuadraticLoadModel { get; private set; }

        /// <summary>
        /// Gets the bank width of the stability point structure.
        /// [m]
        /// </summary>
        public NormalDistribution BankWidth { get; private set; }

        /// <summary>
        /// Gets the inside water level failure construction of the stability point structure.
        /// [m+NAP]
        /// </summary>
        public NormalDistribution InsideWaterLevelFailureConstruction { get; private set; }

        /// <summary>
        /// Gets the evaluation level of the stability point structure.
        /// [m+NAP]
        /// </summary>
        public RoundedDouble EvaluationLevel { get; private set; }

        /// <summary>
        /// Gets the crest level of the stability point structure.
        /// [m+NAP]
        /// </summary>
        public NormalDistribution LevelCrestStructure { get; private set; }

        /// <summary>
        /// Gets the vertical distance of the stability point structure.
        /// [m]
        /// </summary>
        public RoundedDouble VerticalDistance { get; private set; }

        /// <summary>
        /// Gets the probability of failing to repair a failed closure of the stability point structure.
        /// [1/year]
        /// </summary>
        public double FailureProbabilityRepairClosure { get; private set; }

        /// <summary>
        /// Gets the failure collision energy of the stability point structure.
        /// [kN m]
        /// </summary>
        public VariationCoefficientLogNormalDistribution FailureCollisionEnergy { get; private set; }

        /// <summary>
        /// Gets the mass of the ship.
        /// [ton]
        /// </summary>
        public VariationCoefficientNormalDistribution ShipMass { get; private set; }

        /// <summary>
        /// Gets the velocity of the ship.
        /// [m/s]
        /// </summary>
        public VariationCoefficientNormalDistribution ShipVelocity { get; private set; }

        /// <summary>
        /// Gets the levelling count.
        /// [1/year]
        /// </summary>
        public int LevellingCount { get; private set; }

        /// <summary>
        /// Gets the probability of a secondary collision on the structure per levelling.
        /// [1/year/levelling]
        /// </summary>
        public double ProbabilityCollisionSecondaryStructure { get; private set; }

        /// <summary>
        /// Gets the maximum flow velocity at which the structure is closable.
        /// [m/s]
        /// </summary>
        public NormalDistribution FlowVelocityStructureClosable { get; private set; }

        /// <summary>
        /// Gets the stability properties of the linear load model of the stability point structure.
        /// [kN/m^2]
        /// </summary>
        public VariationCoefficientLogNormalDistribution StabilityLinearLoadModel { get; private set; }

        /// <summary>
        /// Gets the stability properties of the quadratic load model of the stability point structure.
        /// [kN/m]
        /// </summary>
        public VariationCoefficientLogNormalDistribution StabilityQuadraticLoadModel { get; private set; }

        /// <summary>
        /// Gets the area of the flow aperture of the stability point structure.
        /// [m^2]
        /// </summary>
        public LogNormalDistribution AreaFlowApertures { get; private set; }

        /// <summary>
        /// Gets the type of stability point structure inflow model.
        /// </summary>
        public StabilityPointStructureInflowModelType InflowModelType { get; private set; }

        /// <summary>
        /// Class holding the various construction parameters for <see cref="StabilityPointStructure"/>.
        /// </summary>
        public new class ConstructionProperties : BaseConstructionProperties
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="ConstructionProperties"/> class.
            /// </summary>
            public ConstructionProperties()
            {
                StorageStructureArea = new VariationCoefficientLogNormalDistribution(2);
                AllowedLevelIncreaseStorage = new LogNormalDistribution(2);
                WidthFlowApertures = new VariationCoefficientNormalDistribution(2);
                InsideWaterLevel = new NormalDistribution(2);
                ThresholdHeightOpenWeir = new NormalDistribution(2);
                CriticalOvertoppingDischarge = new VariationCoefficientLogNormalDistribution(2);
                FlowWidthAtBottomProtection = new LogNormalDistribution(2);
                ConstructiveStrengthLinearLoadModel = new VariationCoefficientLogNormalDistribution(2);
                ConstructiveStrengthQuadraticLoadModel = new VariationCoefficientLogNormalDistribution(2);
                BankWidth = new NormalDistribution(2);
                InsideWaterLevelFailureConstruction = new NormalDistribution(2);
                LevelCrestStructure = new NormalDistribution(2);
                FailureCollisionEnergy = new VariationCoefficientLogNormalDistribution(2);
                ShipMass = new VariationCoefficientNormalDistribution(2);
                ShipVelocity = new VariationCoefficientNormalDistribution(2);
                FlowVelocityStructureClosable = new NormalDistribution(2);
                StabilityLinearLoadModel = new VariationCoefficientLogNormalDistribution(2);
                StabilityQuadraticLoadModel = new VariationCoefficientLogNormalDistribution(2);
                AreaFlowApertures = new LogNormalDistribution(2);
            }

            /// <summary>
            /// Gets the storage area of the stability point structure.
            /// [m^2]
            /// </summary>
            public VariationCoefficientLogNormalDistribution StorageStructureArea { get; private set; }

            /// <summary>
            /// Gets the allowed increase of level for storage of the stability point structure.
            /// [m]
            /// </summary>
            public LogNormalDistribution AllowedLevelIncreaseStorage { get; private set; }

            /// <summary>
            /// Gets the width of the flow apertures of the stability point structure.
            /// [m]
            /// </summary>
            public VariationCoefficientNormalDistribution WidthFlowApertures { get; private set; }

            /// <summary>
            /// Gets the interior water level of the stability point structure.
            /// [m+NAP]
            /// </summary>
            public NormalDistribution InsideWaterLevel { get; private set; }

            /// <summary>
            /// Gets the threshold height of the opened stability point structure.
            /// [m+NAP]
            /// </summary>
            public NormalDistribution ThresholdHeightOpenWeir { get; private set; }

            /// <summary>
            /// Gets the critical overtopping discharge per meter of the stability point structure.
            /// [m^3/s/m]
            /// </summary>
            public VariationCoefficientLogNormalDistribution CriticalOvertoppingDischarge { get; private set; }

            /// <summary>
            /// Gets the flow width of the stability point structure at the bottom protection.
            /// [m]
            /// </summary>
            public LogNormalDistribution FlowWidthAtBottomProtection { get; private set; }

            /// <summary>
            /// Gets the constructive strength of the linear load model of the stability point structure.
            /// [kN/m^2]
            /// </summary>
            public VariationCoefficientLogNormalDistribution ConstructiveStrengthLinearLoadModel { get; private set; }

            /// <summary>
            /// Gets the constructive strength of the quadratic load model of the stability point structure.
            /// [kN/m]
            /// </summary>
            public VariationCoefficientLogNormalDistribution ConstructiveStrengthQuadraticLoadModel { get; private set; }

            /// <summary>
            /// Gets the bank width of the stability point structure.
            /// [m]
            /// </summary>
            public NormalDistribution BankWidth { get; private set; }

            /// <summary>
            /// Gets the inside water level failure construction of the stability point structure.
            /// [m+NAP]
            /// </summary>
            public NormalDistribution InsideWaterLevelFailureConstruction { get; private set; }

            /// <summary>
            /// Gets or sets the evaluation level of the stability point structure.
            /// [m+NAP]
            /// </summary>
            public double EvaluationLevel { get; set; }

            /// <summary>
            /// Gets the crest level of the stability point structure.
            /// [m+NAP]
            /// </summary>
            public NormalDistribution LevelCrestStructure { get; private set; }

            /// <summary>
            /// Gets or sets the vertical distance of the stability point structure.
            /// [m]
            /// </summary>
            public double VerticalDistance { get; set; }

            /// <summary>
            /// Gets or sets the probability of failing to repair a failed closure of the stability point structure.
            /// [1/year]
            /// </summary>
            public double FailureProbabilityRepairClosure { get; set; }

            /// <summary>
            /// Gets the failure collision energy of the stability point structure.
            /// [kN m]
            /// </summary>
            public VariationCoefficientLogNormalDistribution FailureCollisionEnergy { get; private set; }

            /// <summary>
            /// Gets the mass of the ship.
            /// [ton]
            /// </summary>
            public VariationCoefficientNormalDistribution ShipMass { get; private set; }

            /// <summary>
            /// Gets the velocity of the ship.
            /// [m/s]
            /// </summary>
            public VariationCoefficientNormalDistribution ShipVelocity { get; private set; }

            /// <summary>
            /// Gets or sets the levelling count.
            /// [1/year]
            /// </summary>
            public int LevellingCount { get; set; }

            /// <summary>
            /// Gets or sets the probability of a secondary collision on the structure per levelling.
            /// [1/year/levelling]
            /// </summary>
            public double ProbabilityCollisionSecondaryStructure { get; set; }

            /// <summary>
            /// Gets the maximum flow velocity at which the structure is closable.
            /// [m/s]
            /// </summary>
            public NormalDistribution FlowVelocityStructureClosable { get; private set; }

            /// <summary>
            /// Gets the stability properties of the linear load model of the stability point structure.
            /// [kN/m^2]
            /// </summary>
            public VariationCoefficientLogNormalDistribution StabilityLinearLoadModel { get; private set; }

            /// <summary>
            /// Gets the stability properties of the quadratic load model of the stability point structure.
            /// [kN/m]
            /// </summary>
            public VariationCoefficientLogNormalDistribution StabilityQuadraticLoadModel { get; private set; }

            /// <summary>
            /// Gets the area of the flow aperture of the stability point structure.
            /// [m^2]
            /// </summary>
            public LogNormalDistribution AreaFlowApertures { get; private set; }

            /// <summary>
            /// Gets or sets the type of stability point structure inflow model.
            /// </summary>
            public StabilityPointStructureInflowModelType InflowModelType { get; set; }
        }
    }
}