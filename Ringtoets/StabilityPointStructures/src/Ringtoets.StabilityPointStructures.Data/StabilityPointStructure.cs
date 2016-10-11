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
using Core.Common.Base.Geometry;
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.Probabilistics;

namespace Ringtoets.StabilityPointStructures.Data
{
    /// <summary>
    /// Definition of a stability point structure for the <see cref="StabilityPointStructuresFailureMechanism"/>.
    /// </summary>
    public class StabilityPointStructure : StructureBase
    {
        /// <summary>
        /// Creates a new instance of <see cref="StabilityPointStructure"/>.
        /// </summary>
        /// <param name="name">The name of the structure.</param>
        /// <param name="id">The identifier of the structure.</param>
        /// <param name="location">The location of the structure.</param>
        /// <param name="structureNormalOrientation">The orientation of the stability point structure, relative to north.</param>
        /// <param name="storageStructureAreaMean">The mean of the storage area of the stability point structure.</param>
        /// <param name="storageStructureAreaCoefficientOfVariation">The coefficient of variation of the storage area of the stability point structure.</param>
        /// <param name="allowedLevelIncreaseStorageMean">The mean of the allowed increase of level for storage of the stability point structure.</param>
        /// <param name="allowedLevelIncreaseStorageStandardDeviation">The standard deviation of the allowed increase of level for storage of the stability point structure.</param>
        /// <param name="widthFlowAperturesMean">The mean of the width of the flow apertures of the stability point structure.</param>
        /// <param name="widthFlowAperturesCoefficientOfVariation">The coefficient of variation of the width of the flow apertures of the stability point structure.</param>
        /// <param name="insideWaterLevelMean">The mean of the interior water level of the stability point structure.</param>
        /// <param name="insideWaterLevelStandardDeviation">The standard deviation of the interior water level of the stability point structure.</param>
        /// <param name="thresholdHeightOpenWeirMean">The mean of the threshold height of the opened stability point structure.</param>
        /// <param name="thresholdHeightOpenWeirStandardDeviation">The standard deviation of the threshold height of the opened stability point structure.</param>
        /// <param name="criticalOvertoppingDischargeMean">The mean of the critical overtopping discharge of the stability point structure.</param>
        /// <param name="criticalOvertoppingDischargeCoefficientOfVariation">The coefficient of variation of the critical overtopping discharge of the stability point structure.</param>
        /// <param name="flowWidthAtBottomProtectionMean">The mean of the flow width of the stability point structure at the bottom protection.</param>
        /// <param name="flowWidthAtBottomProtectionStandardDeviation">The standard deviation of the flow width of the stability point structure at the bottom protection.</param>
        /// <param name="constructiveStrengthLinearModelMean">The mean of the constructive strength of the linear load model of the stability point structure.</param>
        /// <param name="constructiveStrengthLinearModelCoefficientOfVariation">The coefficient of variation of the constructive strength of the linear load model of the stability point structure.</param>
        /// <param name="constructiveStrengthQuadraticModelMean">The mean of the constructive strength of the quadratic load model of the stability point structure.</param>
        /// <param name="constructiveStrengthQuadraticModelCoefficientOfVariation">The coefficient of variation of the constructive strength of the quadratic load model of the stability point structure.</param>
        /// <param name="bankWidthMean">The mean of the bank width of the stability point structure.</param>
        /// <param name="bankWidthStandardDeviation">The standard deviation of the bank width of the stability point structure.</param>
        /// <param name="insideWaterLevelFailureConstructionMean">The mean of the inside water level failure construction of the stability point structure.</param>
        /// <param name="insideWaterLevelFailureConstructionStandardDeviation">The standard deviation of the inside water level failure construction of the stability point structure.</param>
        /// <param name="evaluationLevel">The evaluation level of the stability point structure.</param>
        /// <param name="levelCrestStructureMean">The mean of the crest level of the stability point structure.</param>
        /// <param name="levelCrestStructureStandardDeviation">The standard deviation of the crest level of the stability point structure.</param>
        /// <param name="verticalDistance">The vertical distance of the stability point structure.</param>
        /// <param name="failureProbabilityReparation">The probability of failing to repair a failed closure of the stability point structure.</param>
        /// <param name="failureCollisionEnergyMean">The mean of the failure collision energy of the stability point structure.</param>
        /// <param name="failureCollisionEnergyCoefficientOfVariation">The coefficient of variation of the failure collision energy of the stability point structure.</param>
        /// <param name="shipMassMean">The mean of the mass of the ship.</param>
        /// <param name="shipMassCoefficientOfVariation">The coefficient of variation of the mass of the ship.</param>
        /// <param name="shipVelocityMean">The mean of the velocity of the ship.</param>
        /// <param name="shipVelocityCoefficientOfVariation">The coefficient of variation of the velocity of the ship.</param>
        /// <param name="levellingCount">The levelling count.</param>
        /// <param name="probabilityCollisionSecondaryStructure">The probability of a secondary collision on the stability point structure.</param>
        /// <param name="flowVelocityStructureClosableMean">The mean of the maximum flow velocity at which the structure is closable.</param>
        /// <param name="flowVelocityStructureClosableStandardDeviation">The standard deviation of the maximum flow velocity at which the structure is closable.</param>
        /// <param name="stabilityLinearModelMean">The mean of the stability properties of the linear model of the stability point structure.</param>
        /// <param name="stabilityLinearModelCoefficientOfVariation">The coefficient of variation of the stability properties of the linear model of the stability point structure.</param>
        /// <param name="stabilityQuadraticModelMean">The mean of the stability properties of the quadratic model of the stability point structure.</param>
        /// <param name="stabilityQuadraticModelCoefficientOfVariation">The coefficient of variation of the stability properties of the quadratic model of the stability point structure.</param>
        /// <param name="areaFlowAperturesMean">The mean of the area of the flow aperture of the stability point structure.</param>
        /// <param name="areaFlowAperturesStandardDeviation">The standard deviation of the area of the flow aperture of the stability point structure.</param>
        /// <param name="inflowModelType">The type of stability point structure inflow model.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="name"/> or <paramref name="id"/> is <c>null</c>
        /// , empty or consists of whitespace.</exception>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="location"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">When any stochastic variable parameter is out of its valid domain.</exception>
        public StabilityPointStructure(string name, string id, Point2D location,
                                       double structureNormalOrientation,
                                       double storageStructureAreaMean, double storageStructureAreaCoefficientOfVariation,
                                       double allowedLevelIncreaseStorageMean, double allowedLevelIncreaseStorageStandardDeviation,
                                       double widthFlowAperturesMean, double widthFlowAperturesCoefficientOfVariation,
                                       double insideWaterLevelMean, double insideWaterLevelStandardDeviation,
                                       double thresholdHeightOpenWeirMean, double thresholdHeightOpenWeirStandardDeviation,
                                       double criticalOvertoppingDischargeMean, double criticalOvertoppingDischargeCoefficientOfVariation,
                                       double flowWidthAtBottomProtectionMean, double flowWidthAtBottomProtectionStandardDeviation,
                                       double constructiveStrengthLinearModelMean, double constructiveStrengthLinearModelCoefficientOfVariation,
                                       double constructiveStrengthQuadraticModelMean, double constructiveStrengthQuadraticModelCoefficientOfVariation,
                                       double bankWidthMean, double bankWidthStandardDeviation,
                                       double insideWaterLevelFailureConstructionMean, double insideWaterLevelFailureConstructionStandardDeviation,
                                       double evaluationLevel,
                                       double levelCrestStructureMean, double levelCrestStructureStandardDeviation,
                                       double verticalDistance,
                                       double failureProbabilityReparation,
                                       double failureCollisionEnergyMean, double failureCollisionEnergyCoefficientOfVariation,
                                       double shipMassMean, double shipMassCoefficientOfVariation,
                                       double shipVelocityMean, double shipVelocityCoefficientOfVariation,
                                       int levellingCount,
                                       double probabilityCollisionSecondaryStructure,
                                       double flowVelocityStructureClosableMean, double flowVelocityStructureClosableStandardDeviation,
                                       double stabilityLinearModelMean, double stabilityLinearModelCoefficientOfVariation,
                                       double stabilityQuadraticModelMean, double stabilityQuadraticModelCoefficientOfVariation,
                                       double areaFlowAperturesMean, double areaFlowAperturesStandardDeviation,
                                       StabilityPointStructureInflowModelType inflowModelType
            )
            : base(name, id, location, structureNormalOrientation)
        {
            StorageStructureArea = new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = (RoundedDouble) storageStructureAreaMean,
                CoefficientOfVariation = (RoundedDouble) storageStructureAreaCoefficientOfVariation
            };
            AllowedLevelIncreaseStorage = new LogNormalDistribution(2)
            {
                Mean = (RoundedDouble) allowedLevelIncreaseStorageMean,
                StandardDeviation = (RoundedDouble) allowedLevelIncreaseStorageStandardDeviation
            };
            WidthFlowApertures = new VariationCoefficientNormalDistribution(2)
            {
                Mean = (RoundedDouble) widthFlowAperturesMean,
                CoefficientOfVariation = (RoundedDouble) widthFlowAperturesCoefficientOfVariation
            };
            InsideWaterLevel = new NormalDistribution(2)
            {
                Mean = (RoundedDouble) insideWaterLevelMean,
                StandardDeviation = (RoundedDouble) insideWaterLevelStandardDeviation
            };
            ThresholdHeightOpenWeir = new NormalDistribution(2)
            {
                Mean = (RoundedDouble) thresholdHeightOpenWeirMean,
                StandardDeviation = (RoundedDouble) thresholdHeightOpenWeirStandardDeviation
            };
            CriticalOvertoppingDischarge = new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = (RoundedDouble) criticalOvertoppingDischargeMean,
                CoefficientOfVariation = (RoundedDouble) criticalOvertoppingDischargeCoefficientOfVariation
            };
            FlowWidthAtBottomProtection = new LogNormalDistribution(2)
            {
                Mean = (RoundedDouble) flowWidthAtBottomProtectionMean,
                StandardDeviation = (RoundedDouble) flowWidthAtBottomProtectionStandardDeviation
            };
            ConstructiveStrengthLinearModel = new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = (RoundedDouble) constructiveStrengthLinearModelMean,
                CoefficientOfVariation = (RoundedDouble) constructiveStrengthLinearModelCoefficientOfVariation
            };
            ConstructiveStrengthQuadraticModel = new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = (RoundedDouble) constructiveStrengthQuadraticModelMean,
                CoefficientOfVariation = (RoundedDouble) constructiveStrengthQuadraticModelCoefficientOfVariation
            };
            BankWidth = new NormalDistribution(2)
            {
                Mean = (RoundedDouble) bankWidthMean,
                StandardDeviation = (RoundedDouble) bankWidthStandardDeviation
            };
            InsideWaterLevelFailureConstruction = new NormalDistribution(2)
            {
                Mean = (RoundedDouble) insideWaterLevelFailureConstructionMean,
                StandardDeviation = (RoundedDouble) insideWaterLevelFailureConstructionStandardDeviation
            };
            EvaluationLevel = new RoundedDouble(2, evaluationLevel);
            LevelCrestStructure = new NormalDistribution(2)
            {
                Mean = (RoundedDouble) levelCrestStructureMean,
                StandardDeviation = (RoundedDouble) levelCrestStructureStandardDeviation
            };
            VerticalDistance = new RoundedDouble(2, verticalDistance);
            FailureProbabilityReparation = new RoundedDouble(2, failureProbabilityReparation);
            FailureCollisionEnergy = new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = (RoundedDouble) failureCollisionEnergyMean,
                CoefficientOfVariation = (RoundedDouble) failureCollisionEnergyCoefficientOfVariation
            };
            ShipMass = new VariationCoefficientNormalDistribution(2)
            {
                Mean = (RoundedDouble) shipMassMean,
                CoefficientOfVariation = (RoundedDouble) shipMassCoefficientOfVariation
            };
            ShipVelocity = new VariationCoefficientNormalDistribution(2)
            {
                Mean = (RoundedDouble) shipVelocityMean,
                CoefficientOfVariation = (RoundedDouble) shipVelocityCoefficientOfVariation
            };
            LevellingCount = levellingCount;
            ProbabilityCollisionSecondaryStructure = new RoundedDouble(2, probabilityCollisionSecondaryStructure);
            FlowVelocityStructureClosable = new NormalDistribution(2)
            {
                Mean = (RoundedDouble) flowVelocityStructureClosableMean,
                StandardDeviation = (RoundedDouble) flowVelocityStructureClosableStandardDeviation
            };
            StabilityLinearModel = new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = (RoundedDouble) stabilityLinearModelMean,
                CoefficientOfVariation = (RoundedDouble) stabilityLinearModelCoefficientOfVariation
            };
            StabilityQuadraticModel = new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = (RoundedDouble) stabilityQuadraticModelMean,
                CoefficientOfVariation = (RoundedDouble) stabilityQuadraticModelCoefficientOfVariation
            };
            AreaFlowApertures = new LogNormalDistribution(2)
            {
                Mean = (RoundedDouble) areaFlowAperturesMean,
                StandardDeviation = (RoundedDouble) areaFlowAperturesStandardDeviation
            };
            InflowModelType = inflowModelType;
        }

        /// <summary>
        /// Gets the storage area of the stability point structure.
        /// </summary>
        public VariationCoefficientLogNormalDistribution StorageStructureArea { get; private set; }

        /// <summary>
        /// Gets the allowed increase of level for storage of the stability point structure.
        /// </summary>
        public LogNormalDistribution AllowedLevelIncreaseStorage { get; private set; }

        /// <summary>
        /// Gets the width of the flow apertures of the stability point structure.
        /// </summary>
        public VariationCoefficientNormalDistribution WidthFlowApertures { get; private set; }

        /// <summary>
        /// Gets the interior water level of the stability point structure.
        /// </summary>
        public NormalDistribution InsideWaterLevel { get; private set; }

        /// <summary>
        /// Gets the threshold height of the opened stability point structure.
        /// </summary>
        public NormalDistribution ThresholdHeightOpenWeir { get; private set; }

        /// <summary>
        /// Gets the critical overtopping discharge of the stability point structure.
        /// </summary>
        public VariationCoefficientLogNormalDistribution CriticalOvertoppingDischarge { get; private set; }

        /// <summary>
        /// Gets the flow width of the stability point structure at the bottom protection.
        /// </summary>
        public LogNormalDistribution FlowWidthAtBottomProtection { get; private set; }

        /// <summary>
        /// Gets the constructive strength of the linear load model of the stability point structure.
        /// </summary>
        public VariationCoefficientLogNormalDistribution ConstructiveStrengthLinearModel { get; private set; }

        /// <summary>
        /// Gets the constructive strength of the quadratic load model of the stability point structure.
        /// </summary>
        public VariationCoefficientLogNormalDistribution ConstructiveStrengthQuadraticModel { get; private set; }

        /// <summary>
        /// Gets the bank width of the stability point structure.
        /// </summary>
        public NormalDistribution BankWidth { get; private set; }

        /// <summary>
        /// Gets the inside water level failure construction of the stability point structure.
        /// </summary>
        public NormalDistribution InsideWaterLevelFailureConstruction { get; private set; }

        /// <summary>
        /// Gets the evaluation level of the stability point structure.
        /// </summary>
        public RoundedDouble EvaluationLevel { get; private set; }

        /// <summary>
        /// Gets the crest level of the stability point structure.
        /// </summary>
        public NormalDistribution LevelCrestStructure { get; private set; }

        /// <summary>
        /// Gets the vertical distance of the stability point structure.
        /// </summary>
        public RoundedDouble VerticalDistance { get; private set; }

        /// <summary>
        /// Gets the probability of failing to repair a failed closure of the stability point structure.
        /// </summary>
        public RoundedDouble FailureProbabilityReparation { get; private set; }

        /// <summary>
        /// Gets the failure collision energy of the stability point structure.
        /// </summary>
        public VariationCoefficientLogNormalDistribution FailureCollisionEnergy { get; private set; }

        /// <summary>
        /// Gets the mass of the ship.
        /// </summary>
        public VariationCoefficientNormalDistribution ShipMass { get; private set; }

        /// <summary>
        /// Gets the velocity of the ship.
        /// </summary>
        public VariationCoefficientNormalDistribution ShipVelocity { get; private set; }

        /// <summary>
        /// Gets the levelling count.
        /// </summary>
        public int LevellingCount { get; private set; }

        /// <summary>
        /// Gets the probability of a secondary collision on the structure.
        /// </summary>
        public RoundedDouble ProbabilityCollisionSecondaryStructure { get; private set; }

        /// <summary>
        /// Gets the maximum flow velocity at which the structure is closable.
        /// </summary>
        public NormalDistribution FlowVelocityStructureClosable { get; private set; }

        /// <summary>
        /// Gets the stability properties of the linear model of the stability point structure.
        /// </summary>
        public VariationCoefficientLogNormalDistribution StabilityLinearModel { get; private set; }

        /// <summary>
        /// Gets the stability properties of the quadratic model of the stability point structure.
        /// </summary>
        public VariationCoefficientLogNormalDistribution StabilityQuadraticModel { get; private set; }

        /// <summary>
        /// Gets the area of the flow aperture of the stability point structure.
        /// </summary>
        public LogNormalDistribution AreaFlowApertures { get; private set; }

        /// <summary>
        /// Gets the type of stability point structure inflow model.
        /// </summary>
        public StabilityPointStructureInflowModelType InflowModelType { get; private set; }
    }
}