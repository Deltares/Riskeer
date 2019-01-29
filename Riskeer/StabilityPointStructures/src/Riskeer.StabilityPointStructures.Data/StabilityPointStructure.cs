// Copyright (C) Stichting Deltares 2018. All rights reserved.
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

namespace Riskeer.StabilityPointStructures.Data
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
            WidthFlowApertures = new NormalDistribution(2)
            {
                Mean = constructionProperties.WidthFlowApertures.Mean,
                StandardDeviation = constructionProperties.WidthFlowApertures.StandardDeviation
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
            FlowVelocityStructureClosable = new VariationCoefficientNormalDistribution(2)
            {
                Mean = constructionProperties.FlowVelocityStructureClosable.Mean,
                CoefficientOfVariation = (RoundedDouble) 0.2
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
        public VariationCoefficientLogNormalDistribution StorageStructureArea { get; }

        /// <summary>
        /// Gets the allowed increase of level for storage of the stability point structure.
        /// [m]
        /// </summary>
        public LogNormalDistribution AllowedLevelIncreaseStorage { get; }

        /// <summary>
        /// Gets the width of the flow apertures of the stability point structure.
        /// [m]
        /// </summary>
        public NormalDistribution WidthFlowApertures { get; }

        /// <summary>
        /// Gets the interior water level of the stability point structure.
        /// [m+NAP]
        /// </summary>
        public NormalDistribution InsideWaterLevel { get; }

        /// <summary>
        /// Gets the threshold height of the opened stability point structure.
        /// [m+NAP]
        /// </summary>
        public NormalDistribution ThresholdHeightOpenWeir { get; }

        /// <summary>
        /// Gets the critical overtopping discharge per meter of the stability point structure.
        /// [m^3/s/m]
        /// </summary>
        public VariationCoefficientLogNormalDistribution CriticalOvertoppingDischarge { get; }

        /// <summary>
        /// Gets the flow width of the stability point structure at the bottom protection.
        /// [m]
        /// </summary>
        public LogNormalDistribution FlowWidthAtBottomProtection { get; }

        /// <summary>
        /// Gets the constructive strength of the linear load model of the stability point structure.
        /// [kN/m^2]
        /// </summary>
        public VariationCoefficientLogNormalDistribution ConstructiveStrengthLinearLoadModel { get; }

        /// <summary>
        /// Gets the constructive strength of the quadratic load model of the stability point structure.
        /// [kN/m]
        /// </summary>
        public VariationCoefficientLogNormalDistribution ConstructiveStrengthQuadraticLoadModel { get; }

        /// <summary>
        /// Gets the bank width of the stability point structure.
        /// [m]
        /// </summary>
        public NormalDistribution BankWidth { get; }

        /// <summary>
        /// Gets the inside water level failure construction of the stability point structure.
        /// [m+NAP]
        /// </summary>
        public NormalDistribution InsideWaterLevelFailureConstruction { get; }

        /// <summary>
        /// Gets the evaluation level of the stability point structure.
        /// [m+NAP]
        /// </summary>
        public RoundedDouble EvaluationLevel { get; private set; }

        /// <summary>
        /// Gets the crest level of the stability point structure.
        /// [m+NAP]
        /// </summary>
        public NormalDistribution LevelCrestStructure { get; }

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
        public VariationCoefficientLogNormalDistribution FailureCollisionEnergy { get; }

        /// <summary>
        /// Gets the mass of the ship.
        /// [ton]
        /// </summary>
        public VariationCoefficientNormalDistribution ShipMass { get; }

        /// <summary>
        /// Gets the velocity of the ship.
        /// [m/s]
        /// </summary>
        public VariationCoefficientNormalDistribution ShipVelocity { get; }

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
        public VariationCoefficientNormalDistribution FlowVelocityStructureClosable { get; }

        /// <summary>
        /// Gets the stability properties of the linear load model of the stability point structure.
        /// [kN/m^2]
        /// </summary>
        public VariationCoefficientLogNormalDistribution StabilityLinearLoadModel { get; }

        /// <summary>
        /// Gets the stability properties of the quadratic load model of the stability point structure.
        /// [kN/m]
        /// </summary>
        public VariationCoefficientLogNormalDistribution StabilityQuadraticLoadModel { get; }

        /// <summary>
        /// Gets the area of the flow aperture of the stability point structure.
        /// [m^2]
        /// </summary>
        public LogNormalDistribution AreaFlowApertures { get; }

        /// <summary>
        /// Gets the type of stability point structure inflow model.
        /// </summary>
        public StabilityPointStructureInflowModelType InflowModelType { get; private set; }

        /// <summary>
        /// Copies the property values of the <paramref name="fromStructure"/> to the 
        /// <see cref="StabilityPointStructure"/>.
        /// </summary>
        /// <param name="fromStructure">The <see cref="StabilityPointStructure"/> to get the property 
        /// values from.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="fromStructure"/>
        /// is <c>null</c>.</exception>
        public void CopyProperties(StabilityPointStructure fromStructure)
        {
            base.CopyProperties(fromStructure);

            StorageStructureArea.Mean = fromStructure.StorageStructureArea.Mean;
            StorageStructureArea.CoefficientOfVariation = fromStructure.StorageStructureArea.CoefficientOfVariation;

            AllowedLevelIncreaseStorage.Mean = fromStructure.AllowedLevelIncreaseStorage.Mean;
            AllowedLevelIncreaseStorage.StandardDeviation = fromStructure.AllowedLevelIncreaseStorage.StandardDeviation;

            WidthFlowApertures.Mean = fromStructure.WidthFlowApertures.Mean;
            WidthFlowApertures.StandardDeviation = fromStructure.WidthFlowApertures.StandardDeviation;

            InsideWaterLevel.Mean = fromStructure.InsideWaterLevel.Mean;
            InsideWaterLevel.StandardDeviation = fromStructure.InsideWaterLevel.StandardDeviation;

            ThresholdHeightOpenWeir.Mean = fromStructure.ThresholdHeightOpenWeir.Mean;
            ThresholdHeightOpenWeir.StandardDeviation = fromStructure.ThresholdHeightOpenWeir.StandardDeviation;

            CriticalOvertoppingDischarge.Mean = fromStructure.CriticalOvertoppingDischarge.Mean;
            CriticalOvertoppingDischarge.CoefficientOfVariation = fromStructure.CriticalOvertoppingDischarge.CoefficientOfVariation;

            FlowWidthAtBottomProtection.Mean = fromStructure.FlowWidthAtBottomProtection.Mean;
            FlowWidthAtBottomProtection.StandardDeviation = fromStructure.FlowWidthAtBottomProtection.StandardDeviation;

            ConstructiveStrengthLinearLoadModel.Mean = fromStructure.ConstructiveStrengthLinearLoadModel.Mean;
            ConstructiveStrengthLinearLoadModel.CoefficientOfVariation = fromStructure.ConstructiveStrengthLinearLoadModel.CoefficientOfVariation;

            ConstructiveStrengthQuadraticLoadModel.Mean = fromStructure.ConstructiveStrengthQuadraticLoadModel.Mean;
            ConstructiveStrengthQuadraticLoadModel.CoefficientOfVariation = fromStructure.ConstructiveStrengthQuadraticLoadModel.CoefficientOfVariation;

            BankWidth.Mean = fromStructure.BankWidth.Mean;
            BankWidth.StandardDeviation = fromStructure.BankWidth.StandardDeviation;

            InsideWaterLevelFailureConstruction.Mean = fromStructure.InsideWaterLevelFailureConstruction.Mean;
            InsideWaterLevelFailureConstruction.StandardDeviation = fromStructure.InsideWaterLevelFailureConstruction.StandardDeviation;

            EvaluationLevel = fromStructure.EvaluationLevel;

            LevelCrestStructure.Mean = fromStructure.LevelCrestStructure.Mean;
            LevelCrestStructure.StandardDeviation = fromStructure.LevelCrestStructure.StandardDeviation;

            VerticalDistance = fromStructure.VerticalDistance;
            FailureProbabilityRepairClosure = fromStructure.FailureProbabilityRepairClosure;

            FailureCollisionEnergy.Mean = fromStructure.FailureCollisionEnergy.Mean;
            FailureCollisionEnergy.CoefficientOfVariation = fromStructure.FailureCollisionEnergy.CoefficientOfVariation;

            ShipMass.Mean = fromStructure.ShipMass.Mean;
            ShipMass.CoefficientOfVariation = fromStructure.ShipMass.CoefficientOfVariation;

            ShipVelocity.Mean = fromStructure.ShipVelocity.Mean;
            ShipVelocity.CoefficientOfVariation = fromStructure.ShipVelocity.CoefficientOfVariation;

            LevellingCount = fromStructure.LevellingCount;
            ProbabilityCollisionSecondaryStructure = fromStructure.ProbabilityCollisionSecondaryStructure;

            FlowVelocityStructureClosable.Mean = fromStructure.FlowVelocityStructureClosable.Mean;

            StabilityLinearLoadModel.Mean = fromStructure.StabilityLinearLoadModel.Mean;
            StabilityLinearLoadModel.CoefficientOfVariation = fromStructure.StabilityLinearLoadModel.CoefficientOfVariation;

            StabilityQuadraticLoadModel.Mean = fromStructure.StabilityQuadraticLoadModel.Mean;
            StabilityQuadraticLoadModel.CoefficientOfVariation = fromStructure.StabilityQuadraticLoadModel.CoefficientOfVariation;

            AreaFlowApertures.Mean = fromStructure.AreaFlowApertures.Mean;
            AreaFlowApertures.StandardDeviation = fromStructure.AreaFlowApertures.StandardDeviation;

            InflowModelType = fromStructure.InflowModelType;
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj) && Equals((StabilityPointStructure) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = base.GetHashCode();
                hashCode = (hashCode * 397) ^ StorageStructureArea.GetHashCode();
                hashCode = (hashCode * 397) ^ AllowedLevelIncreaseStorage.GetHashCode();
                hashCode = (hashCode * 397) ^ WidthFlowApertures.GetHashCode();
                hashCode = (hashCode * 397) ^ InsideWaterLevel.GetHashCode();
                hashCode = (hashCode * 397) ^ ThresholdHeightOpenWeir.GetHashCode();
                hashCode = (hashCode * 397) ^ CriticalOvertoppingDischarge.GetHashCode();
                hashCode = (hashCode * 397) ^ FlowWidthAtBottomProtection.GetHashCode();
                hashCode = (hashCode * 397) ^ ConstructiveStrengthLinearLoadModel.GetHashCode();
                hashCode = (hashCode * 397) ^ ConstructiveStrengthQuadraticLoadModel.GetHashCode();
                hashCode = (hashCode * 397) ^ BankWidth.GetHashCode();
                hashCode = (hashCode * 397) ^ InsideWaterLevelFailureConstruction.GetHashCode();
                hashCode = (hashCode * 397) ^ LevelCrestStructure.GetHashCode();
                hashCode = (hashCode * 397) ^ FailureCollisionEnergy.GetHashCode();
                hashCode = (hashCode * 397) ^ ShipMass.GetHashCode();
                hashCode = (hashCode * 397) ^ ShipVelocity.GetHashCode();
                hashCode = (hashCode * 397) ^ FlowVelocityStructureClosable.GetHashCode();
                hashCode = (hashCode * 397) ^ StabilityLinearLoadModel.GetHashCode();
                hashCode = (hashCode * 397) ^ StabilityQuadraticLoadModel.GetHashCode();
                hashCode = (hashCode * 397) ^ AreaFlowApertures.GetHashCode();

                return hashCode;
            }
        }

        private bool Equals(StabilityPointStructure other)
        {
            return StorageStructureArea.Equals(other.StorageStructureArea)
                   && AllowedLevelIncreaseStorage.Equals(other.AllowedLevelIncreaseStorage)
                   && WidthFlowApertures.Equals(other.WidthFlowApertures)
                   && InsideWaterLevel.Equals(other.InsideWaterLevel)
                   && ThresholdHeightOpenWeir.Equals(other.ThresholdHeightOpenWeir)
                   && CriticalOvertoppingDischarge.Equals(other.CriticalOvertoppingDischarge)
                   && FlowWidthAtBottomProtection.Equals(other.FlowWidthAtBottomProtection)
                   && ConstructiveStrengthLinearLoadModel.Equals(other.ConstructiveStrengthLinearLoadModel)
                   && ConstructiveStrengthQuadraticLoadModel.Equals(other.ConstructiveStrengthQuadraticLoadModel)
                   && BankWidth.Equals(other.BankWidth)
                   && InsideWaterLevelFailureConstruction.Equals(other.InsideWaterLevelFailureConstruction)
                   && EvaluationLevel.Equals(other.EvaluationLevel)
                   && LevelCrestStructure.Equals(other.LevelCrestStructure)
                   && VerticalDistance.Equals(other.VerticalDistance)
                   && FailureProbabilityRepairClosure.Equals(other.FailureProbabilityRepairClosure)
                   && FailureCollisionEnergy.Equals(other.FailureCollisionEnergy)
                   && ShipMass.Equals(other.ShipMass)
                   && ShipVelocity.Equals(other.ShipVelocity)
                   && LevellingCount.Equals(other.LevellingCount)
                   && ProbabilityCollisionSecondaryStructure.Equals(other.ProbabilityCollisionSecondaryStructure)
                   && FlowVelocityStructureClosable.Equals(other.FlowVelocityStructureClosable)
                   && StabilityLinearLoadModel.Equals(other.StabilityLinearLoadModel)
                   && StabilityQuadraticLoadModel.Equals(other.StabilityQuadraticLoadModel)
                   && AreaFlowApertures.Equals(other.AreaFlowApertures)
                   && InflowModelType.Equals(other.InflowModelType);
        }

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
                StorageStructureArea = new VariationCoefficientLogNormalDistribution(2)
                {
                    Mean = RoundedDouble.NaN,
                    CoefficientOfVariation = (RoundedDouble) 0.1
                };
                AllowedLevelIncreaseStorage = new LogNormalDistribution(2)
                {
                    Mean = RoundedDouble.NaN,
                    StandardDeviation = (RoundedDouble) 0.1
                };
                WidthFlowApertures = new NormalDistribution(2)
                {
                    Mean = RoundedDouble.NaN,
                    StandardDeviation = (RoundedDouble) 0.2
                };
                InsideWaterLevel = new NormalDistribution(2)
                {
                    Mean = RoundedDouble.NaN,
                    StandardDeviation = (RoundedDouble) 0.1
                };
                ThresholdHeightOpenWeir = new NormalDistribution(2)
                {
                    Mean = RoundedDouble.NaN,
                    StandardDeviation = (RoundedDouble) 0.1
                };
                CriticalOvertoppingDischarge = new VariationCoefficientLogNormalDistribution(2)
                {
                    Mean = RoundedDouble.NaN,
                    CoefficientOfVariation = (RoundedDouble) 0.15
                };
                FlowWidthAtBottomProtection = new LogNormalDistribution(2)
                {
                    Mean = RoundedDouble.NaN,
                    StandardDeviation = (RoundedDouble) 0.05
                };
                ConstructiveStrengthLinearLoadModel = new VariationCoefficientLogNormalDistribution(2)
                {
                    Mean = RoundedDouble.NaN,
                    CoefficientOfVariation = (RoundedDouble) 0.1
                };
                ConstructiveStrengthQuadraticLoadModel = new VariationCoefficientLogNormalDistribution(2)
                {
                    Mean = RoundedDouble.NaN,
                    CoefficientOfVariation = (RoundedDouble) 0.1
                };
                BankWidth = new NormalDistribution(2)
                {
                    Mean = RoundedDouble.NaN,
                    StandardDeviation = RoundedDouble.NaN
                };
                InsideWaterLevelFailureConstruction = new NormalDistribution(2)
                {
                    Mean = RoundedDouble.NaN,
                    StandardDeviation = (RoundedDouble) 0.1
                };
                LevelCrestStructure = new NormalDistribution(2)
                {
                    Mean = RoundedDouble.NaN,
                    StandardDeviation = (RoundedDouble) 0.05
                };
                VerticalDistance = double.NaN;
                FailureProbabilityRepairClosure = 1;
                FailureCollisionEnergy = new VariationCoefficientLogNormalDistribution(2)
                {
                    Mean = RoundedDouble.NaN,
                    CoefficientOfVariation = (RoundedDouble) 0.3
                };
                ShipMass = new VariationCoefficientNormalDistribution(2)
                {
                    Mean = RoundedDouble.NaN,
                    CoefficientOfVariation = (RoundedDouble) 0.2
                };
                ShipVelocity = new VariationCoefficientNormalDistribution(2)
                {
                    Mean = RoundedDouble.NaN,
                    CoefficientOfVariation = (RoundedDouble) 0.2
                };
                LevellingCount = 1;
                ProbabilityCollisionSecondaryStructure = 1;
                FlowVelocityStructureClosable = new VariationCoefficientNormalDistribution(2)
                {
                    Mean = RoundedDouble.NaN,
                    CoefficientOfVariation = (RoundedDouble) 0.2
                };
                StabilityLinearLoadModel = new VariationCoefficientLogNormalDistribution(2)
                {
                    Mean = RoundedDouble.NaN,
                    CoefficientOfVariation = (RoundedDouble) 0.1
                };
                StabilityQuadraticLoadModel = new VariationCoefficientLogNormalDistribution(2)
                {
                    Mean = RoundedDouble.NaN,
                    CoefficientOfVariation = (RoundedDouble) 0.1
                };
                AreaFlowApertures = new LogNormalDistribution(2)
                {
                    Mean = RoundedDouble.NaN,
                    StandardDeviation = (RoundedDouble) 0.01
                };
                InflowModelType = StabilityPointStructureInflowModelType.LowSill;
            }

            /// <summary>
            /// Gets the storage area of the stability point structure.
            /// [m^2]
            /// </summary>
            public VariationCoefficientLogNormalDistribution StorageStructureArea { get; }

            /// <summary>
            /// Gets the allowed increase of level for storage of the stability point structure.
            /// [m]
            /// </summary>
            public LogNormalDistribution AllowedLevelIncreaseStorage { get; }

            /// <summary>
            /// Gets the width of the flow apertures of the stability point structure.
            /// [m]
            /// </summary>
            public NormalDistribution WidthFlowApertures { get; }

            /// <summary>
            /// Gets the interior water level of the stability point structure.
            /// [m+NAP]
            /// </summary>
            public NormalDistribution InsideWaterLevel { get; }

            /// <summary>
            /// Gets the threshold height of the opened stability point structure.
            /// [m+NAP]
            /// </summary>
            public NormalDistribution ThresholdHeightOpenWeir { get; }

            /// <summary>
            /// Gets the critical overtopping discharge per meter of the stability point structure.
            /// [m^3/s/m]
            /// </summary>
            public VariationCoefficientLogNormalDistribution CriticalOvertoppingDischarge { get; }

            /// <summary>
            /// Gets the flow width of the stability point structure at the bottom protection.
            /// [m]
            /// </summary>
            public LogNormalDistribution FlowWidthAtBottomProtection { get; }

            /// <summary>
            /// Gets the constructive strength of the linear load model of the stability point structure.
            /// [kN/m^2]
            /// </summary>
            public VariationCoefficientLogNormalDistribution ConstructiveStrengthLinearLoadModel { get; }

            /// <summary>
            /// Gets the constructive strength of the quadratic load model of the stability point structure.
            /// [kN/m]
            /// </summary>
            public VariationCoefficientLogNormalDistribution ConstructiveStrengthQuadraticLoadModel { get; }

            /// <summary>
            /// Gets the bank width of the stability point structure.
            /// [m]
            /// </summary>
            public NormalDistribution BankWidth { get; }

            /// <summary>
            /// Gets the inside water level failure construction of the stability point structure.
            /// [m+NAP]
            /// </summary>
            public NormalDistribution InsideWaterLevelFailureConstruction { get; }

            /// <summary>
            /// Gets or sets the evaluation level of the stability point structure.
            /// [m+NAP]
            /// </summary>
            public double EvaluationLevel { get; set; }

            /// <summary>
            /// Gets the crest level of the stability point structure.
            /// [m+NAP]
            /// </summary>
            public NormalDistribution LevelCrestStructure { get; }

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
            public VariationCoefficientLogNormalDistribution FailureCollisionEnergy { get; }

            /// <summary>
            /// Gets the mass of the ship.
            /// [ton]
            /// </summary>
            public VariationCoefficientNormalDistribution ShipMass { get; }

            /// <summary>
            /// Gets the velocity of the ship.
            /// [m/s]
            /// </summary>
            public VariationCoefficientNormalDistribution ShipVelocity { get; }

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
            public VariationCoefficientNormalDistribution FlowVelocityStructureClosable { get; }

            /// <summary>
            /// Gets the stability properties of the linear load model of the stability point structure.
            /// [kN/m^2]
            /// </summary>
            public VariationCoefficientLogNormalDistribution StabilityLinearLoadModel { get; }

            /// <summary>
            /// Gets the stability properties of the quadratic load model of the stability point structure.
            /// [kN/m]
            /// </summary>
            public VariationCoefficientLogNormalDistribution StabilityQuadraticLoadModel { get; }

            /// <summary>
            /// Gets the area of the flow aperture of the stability point structure.
            /// [m^2]
            /// </summary>
            public LogNormalDistribution AreaFlowApertures { get; }

            /// <summary>
            /// Gets or sets the type of stability point structure inflow model.
            /// </summary>
            public StabilityPointStructureInflowModelType InflowModelType { get; set; }
        }
    }
}