// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using Core.Common.Util.Extensions;
using Riskeer.ClosingStructures.Data;
using Riskeer.Common.Data;
using Riskeer.Common.Data.Structures;
using Riskeer.HeightStructures.Data;
using Riskeer.StabilityPointStructures.Data;
using Riskeer.Storage.Core.DbContext;

namespace Riskeer.Storage.Core.Create
{
    /// <summary>
    /// Extension methods for <see cref="StructuresCalculation{T}"/> related
    /// to creating structures calculation entities.
    /// </summary>
    internal static class StructuresCalculationCreateExtensions
    {
        private static void Create<T>(this StructuresInputBase<T> input, IStructuresCalculationEntity entityToUpdate,
                                      PersistenceRegistry registry)
            where T : StructureBase
        {
            if (entityToUpdate == null)
            {
                throw new ArgumentNullException(nameof(entityToUpdate));
            }

            if (registry == null)
            {
                throw new ArgumentNullException(nameof(registry));
            }

            entityToUpdate.StormDurationMean = input.StormDuration.Mean.ToNaNAsNull();
            entityToUpdate.StructureNormalOrientation = input.StructureNormalOrientation.ToNaNAsNull();
            entityToUpdate.FailureProbabilityStructureWithErosion = input.FailureProbabilityStructureWithErosion;

            if (input.HydraulicBoundaryLocation != null)
            {
                entityToUpdate.HydraulicLocationEntity = registry.Get(input.HydraulicBoundaryLocation);
            }

            if (input.ForeshoreProfile != null)
            {
                entityToUpdate.ForeshoreProfileEntity = registry.Get(input.ForeshoreProfile);
            }

            entityToUpdate.UseForeshore = Convert.ToByte(input.UseForeshore);

            entityToUpdate.UseBreakWater = Convert.ToByte(input.UseBreakWater);
            entityToUpdate.BreakWaterType = Convert.ToByte(input.BreakWater.Type);
            entityToUpdate.BreakWaterHeight = input.BreakWater.Height.ToNaNAsNull();

            entityToUpdate.AllowedLevelIncreaseStorageMean = input.AllowedLevelIncreaseStorage.Mean.ToNaNAsNull();
            entityToUpdate.AllowedLevelIncreaseStorageStandardDeviation = input.AllowedLevelIncreaseStorage.StandardDeviation.ToNaNAsNull();

            entityToUpdate.StorageStructureAreaMean = input.StorageStructureArea.Mean.ToNaNAsNull();
            entityToUpdate.StorageStructureAreaCoefficientOfVariation = input.StorageStructureArea.CoefficientOfVariation.ToNaNAsNull();

            entityToUpdate.FlowWidthAtBottomProtectionMean = input.FlowWidthAtBottomProtection.Mean.ToNaNAsNull();
            entityToUpdate.FlowWidthAtBottomProtectionStandardDeviation = input.FlowWidthAtBottomProtection.StandardDeviation.ToNaNAsNull();

            entityToUpdate.CriticalOvertoppingDischargeMean = input.CriticalOvertoppingDischarge.Mean.ToNaNAsNull();
            entityToUpdate.CriticalOvertoppingDischargeCoefficientOfVariation = input.CriticalOvertoppingDischarge.CoefficientOfVariation.ToNaNAsNull();

            entityToUpdate.WidthFlowAperturesMean = input.WidthFlowApertures.Mean.ToNaNAsNull();
            entityToUpdate.WidthFlowAperturesStandardDeviation = input.WidthFlowApertures.StandardDeviation.ToNaNAsNull();

            entityToUpdate.ShouldIllustrationPointsBeCalculated = Convert.ToByte(input.ShouldIllustrationPointsBeCalculated);
        }

        #region ClosingStructures

        /// <summary>
        /// Creates a <see cref="ClosingStructuresCalculationEntity"/> based
        /// on the information of the <see cref="StructuresCalculationScenario{T}"/>.
        /// </summary>
        /// <param name="calculation">The calculation to create a database entity for.</param>
        /// <param name="registry">The object keeping track of create operations.</param>
        /// <param name="order">The index at where <paramref name="calculation"/> resides
        /// in its parent container.</param>
        /// <returns>A new <see cref="ClosingStructuresCalculationEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="registry"/> is <c>null</c>.</exception>
        internal static ClosingStructuresCalculationEntity CreateForClosingStructures(this StructuresCalculationScenario<ClosingStructuresInput> calculation,
                                                                                      PersistenceRegistry registry, int order)
        {
            if (registry == null)
            {
                throw new ArgumentNullException(nameof(registry));
            }

            var entity = new ClosingStructuresCalculationEntity
            {
                Name = calculation.Name.DeepClone(),
                RelevantForScenario = Convert.ToByte(calculation.IsRelevant),
                ScenarioContribution = calculation.Contribution.ToNaNAsNull(),
                Comments = calculation.Comments.Body.DeepClone(),
                Order = order
            };
            SetClosingStructuresInputValues(entity, calculation.InputParameters, registry);
            SetClosingStructuresOutputEntity(calculation, entity);

            registry.Register(entity, calculation);

            return entity;
        }

        private static void SetClosingStructuresInputValues(ClosingStructuresCalculationEntity entity, ClosingStructuresInput input,
                                                            PersistenceRegistry registry)
        {
            input.Create(entity, registry);

            if (input.Structure != null)
            {
                entity.ClosingStructureEntity = registry.Get(input.Structure);
            }

            entity.InflowModelType = Convert.ToByte(input.InflowModelType);

            entity.InsideWaterLevelMean = input.InsideWaterLevel.Mean.ToNaNAsNull();
            entity.InsideWaterLevelStandardDeviation = input.InsideWaterLevel.StandardDeviation.ToNaNAsNull();

            entity.DeviationWaveDirection = input.DeviationWaveDirection.ToNaNAsNull();

            entity.ModelFactorSuperCriticalFlowMean = input.ModelFactorSuperCriticalFlow.Mean.ToNaNAsNull();

            entity.DrainCoefficientMean = input.DrainCoefficient.Mean.ToNaNAsNull();

            entity.FactorStormDurationOpenStructure = input.FactorStormDurationOpenStructure.ToNaNAsNull();

            entity.ThresholdHeightOpenWeirMean = input.ThresholdHeightOpenWeir.Mean.ToNaNAsNull();
            entity.ThresholdHeightOpenWeirStandardDeviation = input.ThresholdHeightOpenWeir.StandardDeviation.ToNaNAsNull();

            entity.AreaFlowAperturesMean = input.AreaFlowApertures.Mean.ToNaNAsNull();
            entity.AreaFlowAperturesStandardDeviation = input.AreaFlowApertures.StandardDeviation.ToNaNAsNull();

            entity.FailureProbabilityOpenStructure = input.FailureProbabilityOpenStructure;

            entity.FailureProbabilityReparation = input.FailureProbabilityReparation;

            entity.IdenticalApertures = input.IdenticalApertures;

            entity.LevelCrestStructureNotClosingMean = input.LevelCrestStructureNotClosing.Mean.ToNaNAsNull();
            entity.LevelCrestStructureNotClosingStandardDeviation = input.LevelCrestStructureNotClosing.StandardDeviation.ToNaNAsNull();

            entity.ProbabilityOpenStructureBeforeFlooding = input.ProbabilityOpenStructureBeforeFlooding;
        }

        private static void SetClosingStructuresOutputEntity(StructuresCalculation<ClosingStructuresInput> calculation,
                                                             ClosingStructuresCalculationEntity entity)
        {
            if (calculation.HasOutput)
            {
                entity.ClosingStructuresOutputEntities.Add(calculation
                                                           .Output
                                                           .Create<ClosingStructuresOutputEntity>());
            }
        }

        #endregion

        #region HeightStructures

        /// <summary>
        /// Creates a <see cref="HeightStructuresCalculationEntity"/> based
        /// on the information of the <see cref="StructuresCalculationScenario{T}"/>.
        /// </summary>
        /// <param name="calculation">The calculation to create a database entity for.</param>
        /// <param name="registry">The object keeping track of create operations.</param>
        /// <param name="order">The index at where <paramref name="calculation"/> resides
        /// in its parent container.</param>
        /// <returns>A new <see cref="HeightStructuresCalculationEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="registry"/> is <c>null</c>.</exception>
        internal static HeightStructuresCalculationEntity CreateForHeightStructures(this StructuresCalculationScenario<HeightStructuresInput> calculation,
                                                                                    PersistenceRegistry registry, int order)
        {
            if (registry == null)
            {
                throw new ArgumentNullException(nameof(registry));
            }

            var entity = new HeightStructuresCalculationEntity
            {
                Name = calculation.Name.DeepClone(),
                RelevantForScenario = Convert.ToByte(calculation.IsRelevant),
                ScenarioContribution = calculation.Contribution.ToNaNAsNull(),
                Comments = calculation.Comments.Body.DeepClone(),
                Order = order
            };
            SetHeightStructuresInputValues(entity, calculation.InputParameters, registry);
            SetHeightStructuresOutputEntity(calculation, entity);

            registry.Register(entity, calculation);

            return entity;
        }

        private static void SetHeightStructuresInputValues(HeightStructuresCalculationEntity entity, HeightStructuresInput input,
                                                           PersistenceRegistry registry)
        {
            input.Create(entity, registry);

            if (input.Structure != null)
            {
                entity.HeightStructureEntity = registry.Get(input.Structure);
            }

            entity.DeviationWaveDirection = input.DeviationWaveDirection.ToNaNAsNull();

            entity.ModelFactorSuperCriticalFlowMean = input.ModelFactorSuperCriticalFlow.Mean.ToNaNAsNull();

            entity.LevelCrestStructureMean = input.LevelCrestStructure.Mean.ToNaNAsNull();
            entity.LevelCrestStructureStandardDeviation = input.LevelCrestStructure.StandardDeviation.ToNaNAsNull();
        }

        private static void SetHeightStructuresOutputEntity(StructuresCalculation<HeightStructuresInput> calculation,
                                                            HeightStructuresCalculationEntity entity)
        {
            if (calculation.HasOutput)
            {
                entity.HeightStructuresOutputEntities.Add(calculation
                                                          .Output
                                                          .Create<HeightStructuresOutputEntity>());
            }
        }

        #endregion

        #region StabilityPointStructures

        /// <summary>
        /// Creates a <see cref="StabilityPointStructuresCalculationEntity"/> based
        /// on the information of the <see cref="StructuresCalculation{T}"/>.
        /// </summary>
        /// <param name="calculation">The calculation to create a database entity for.</param>
        /// <param name="registry">The object keeping track of create operations.</param>
        /// <param name="order">The index at where <paramref name="calculation"/> resides
        /// in its parent container.</param>
        /// <returns>A new <see cref="StabilityPointStructuresCalculationEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="registry"/> is <c>null</c>.</exception>
        internal static StabilityPointStructuresCalculationEntity CreateForStabilityPointStructures(this StructuresCalculationScenario<StabilityPointStructuresInput> calculation,
                                                                                                    PersistenceRegistry registry, int order)
        {
            if (registry == null)
            {
                throw new ArgumentNullException(nameof(registry));
            }

            var entity = new StabilityPointStructuresCalculationEntity
            {
                Name = calculation.Name.DeepClone(),
                Comments = calculation.Comments.Body.DeepClone(),
                Order = order
            };
            SetStabilityPointStructuresInputValues(entity, calculation.InputParameters, registry);
            SetStabilityPointStructuresOutputEntity(calculation, entity);

            registry.Register(entity, calculation);

            return entity;
        }

        private static void SetStabilityPointStructuresInputValues(StabilityPointStructuresCalculationEntity entity,
                                                                   StabilityPointStructuresInput input, PersistenceRegistry registry)
        {
            input.Create(entity, registry);

            if (input.Structure != null)
            {
                entity.StabilityPointStructureEntity = registry.Get(input.Structure);
            }

            entity.InsideWaterLevelMean = input.InsideWaterLevel.Mean.ToNaNAsNull();
            entity.InsideWaterLevelStandardDeviation = input.InsideWaterLevel.StandardDeviation.ToNaNAsNull();

            entity.ThresholdHeightOpenWeirMean = input.ThresholdHeightOpenWeir.Mean.ToNaNAsNull();
            entity.ThresholdHeightOpenWeirStandardDeviation = input.ThresholdHeightOpenWeir.StandardDeviation.ToNaNAsNull();

            entity.ConstructiveStrengthLinearLoadModelMean = input.ConstructiveStrengthLinearLoadModel.Mean.ToNaNAsNull();
            entity.ConstructiveStrengthLinearLoadModelCoefficientOfVariation = input.ConstructiveStrengthLinearLoadModel.CoefficientOfVariation.ToNaNAsNull();

            entity.ConstructiveStrengthQuadraticLoadModelMean = input.ConstructiveStrengthQuadraticLoadModel.Mean.ToNaNAsNull();
            entity.ConstructiveStrengthQuadraticLoadModelCoefficientOfVariation = input.ConstructiveStrengthQuadraticLoadModel.CoefficientOfVariation.ToNaNAsNull();

            entity.BankWidthMean = input.BankWidth.Mean.ToNaNAsNull();
            entity.BankWidthStandardDeviation = input.BankWidth.StandardDeviation.ToNaNAsNull();

            entity.InsideWaterLevelFailureConstructionMean = input.InsideWaterLevelFailureConstruction.Mean.ToNaNAsNull();
            entity.InsideWaterLevelFailureConstructionStandardDeviation = input.InsideWaterLevelFailureConstruction.StandardDeviation.ToNaNAsNull();

            entity.EvaluationLevel = input.EvaluationLevel.ToNaNAsNull();

            entity.LevelCrestStructureMean = input.LevelCrestStructure.Mean.ToNaNAsNull();
            entity.LevelCrestStructureStandardDeviation = input.LevelCrestStructure.StandardDeviation.ToNaNAsNull();

            entity.VerticalDistance = input.VerticalDistance.ToNaNAsNull();

            entity.FailureProbabilityRepairClosure = input.FailureProbabilityRepairClosure;

            entity.FailureCollisionEnergyMean = input.FailureCollisionEnergy.Mean.ToNaNAsNull();
            entity.FailureCollisionEnergyCoefficientOfVariation = input.FailureCollisionEnergy.CoefficientOfVariation.ToNaNAsNull();

            entity.ShipMassMean = input.ShipMass.Mean.ToNaNAsNull();
            entity.ShipMassCoefficientOfVariation = input.ShipMass.CoefficientOfVariation.ToNaNAsNull();

            entity.ShipVelocityMean = input.ShipVelocity.Mean.ToNaNAsNull();
            entity.ShipVelocityCoefficientOfVariation = input.ShipVelocity.CoefficientOfVariation.ToNaNAsNull();

            entity.LevellingCount = input.LevellingCount;

            entity.ProbabilityCollisionSecondaryStructure = input.ProbabilityCollisionSecondaryStructure;

            entity.FlowVelocityStructureClosableMean = input.FlowVelocityStructureClosable.Mean.ToNaNAsNull();

            entity.StabilityLinearLoadModelMean = input.StabilityLinearLoadModel.Mean.ToNaNAsNull();
            entity.StabilityLinearLoadModelCoefficientOfVariation = input.StabilityLinearLoadModel.CoefficientOfVariation.ToNaNAsNull();

            entity.StabilityQuadraticLoadModelMean = input.StabilityQuadraticLoadModel.Mean.ToNaNAsNull();
            entity.StabilityQuadraticLoadModelCoefficientOfVariation = input.StabilityQuadraticLoadModel.CoefficientOfVariation.ToNaNAsNull();

            entity.AreaFlowAperturesMean = input.AreaFlowApertures.Mean.ToNaNAsNull();
            entity.AreaFlowAperturesStandardDeviation = input.AreaFlowApertures.StandardDeviation.ToNaNAsNull();

            entity.InflowModelType = Convert.ToByte(input.InflowModelType);
            entity.LoadSchematizationType = Convert.ToByte(input.LoadSchematizationType);
            entity.VolumicWeightWater = input.VolumicWeightWater.ToNaNAsNull();
            entity.FactorStormDurationOpenStructure = input.FactorStormDurationOpenStructure.ToNaNAsNull();
            entity.DrainCoefficientMean = input.DrainCoefficient.Mean.ToNaNAsNull();
        }

        private static void SetStabilityPointStructuresOutputEntity(StructuresCalculation<StabilityPointStructuresInput> calculation,
                                                                    StabilityPointStructuresCalculationEntity entity)
        {
            if (calculation.HasOutput)
            {
                entity.StabilityPointStructuresOutputEntities.Add(calculation
                                                                  .Output
                                                                  .Create<StabilityPointStructuresOutputEntity>());
            }
        }

        #endregion
    }
}