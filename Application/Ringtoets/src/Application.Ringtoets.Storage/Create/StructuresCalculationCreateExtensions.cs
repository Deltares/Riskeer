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
using Application.Ringtoets.Storage.DbContext;
using Core.Common.Utils.Extensions;
using Ringtoets.ClosingStructures.Data;
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.Structures;
using Ringtoets.HeightStructures.Data;
using Ringtoets.StabilityPointStructures.Data;

namespace Application.Ringtoets.Storage.Create
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
                throw new ArgumentNullException("entityToUpdate");
            }
            if (registry == null)
            {
                throw new ArgumentNullException("registry");
            }

            entityToUpdate.StormDurationMean = input.StormDuration.Mean.Value.ToNaNAsNull();
            entityToUpdate.StructureNormalOrientation = input.StructureNormalOrientation.Value.ToNaNAsNull();
            entityToUpdate.FailureProbabilityStructureWithErosion = input.FailureProbabilityStructureWithErosion;

            if (input.HydraulicBoundaryLocation != null)
            {
                entityToUpdate.HydraulicLocationEntity = registry.Get<HydraulicLocationEntity>(input.HydraulicBoundaryLocation);
            }

            if (input.ForeshoreProfile != null)
            {
                entityToUpdate.ForeshoreProfileEntity = registry.Get(input.ForeshoreProfile);
            }
            entityToUpdate.UseForeshore = Convert.ToByte(input.UseForeshore);

            entityToUpdate.UseBreakWater = Convert.ToByte(input.UseBreakWater);
            entityToUpdate.BreakWaterType = Convert.ToByte(input.BreakWater.Type);
            entityToUpdate.BreakWaterHeight = input.BreakWater.Height.Value.ToNaNAsNull();

            entityToUpdate.AllowedLevelIncreaseStorageMean = input.AllowedLevelIncreaseStorage.Mean.Value.ToNaNAsNull();
            entityToUpdate.AllowedLevelIncreaseStorageStandardDeviation = input.AllowedLevelIncreaseStorage.StandardDeviation.Value.ToNaNAsNull();

            entityToUpdate.StorageStructureAreaMean = input.StorageStructureArea.Mean.Value.ToNaNAsNull();
            entityToUpdate.StorageStructureAreaCoefficientOfVariation = input.StorageStructureArea.CoefficientOfVariation.Value.ToNaNAsNull();

            entityToUpdate.FlowWidthAtBottomProtectionMean = input.FlowWidthAtBottomProtection.Mean.Value.ToNaNAsNull();
            entityToUpdate.FlowWidthAtBottomProtectionStandardDeviation = input.FlowWidthAtBottomProtection.StandardDeviation.Value.ToNaNAsNull();

            entityToUpdate.CriticalOvertoppingDischargeMean = input.CriticalOvertoppingDischarge.Mean.Value.ToNaNAsNull();
            entityToUpdate.CriticalOvertoppingDischargeCoefficientOfVariation = input.CriticalOvertoppingDischarge.CoefficientOfVariation.Value.ToNaNAsNull();

            entityToUpdate.ModelFactorSuperCriticalFlowMean = input.ModelFactorSuperCriticalFlow.Mean.Value.ToNaNAsNull();

            entityToUpdate.WidthFlowAperturesMean = input.WidthFlowApertures.Mean.Value.ToNaNAsNull();
            entityToUpdate.WidthFlowAperturesCoefficientOfVariation = input.WidthFlowApertures.StandardDeviation.Value.ToNaNAsNull();
        }

        #region ClosingStructures

        /// <summary>
        /// Creates a <see cref="ClosingStructuresCalculationEntity"/> based
        /// on the information of the <see cref="StructuresCalculation{T}"/>.
        /// </summary>
        /// <param name="calculation">The calculation to create a database entity for.</param>
        /// <param name="registry">The object keeping track of create operations.</param>
        /// <param name="order">The index at where <paramref name="calculation"/> resides
        /// in its parent container.</param>
        /// <returns>A new <see cref="ClosingStructuresCalculationEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="registry"/> is <c>null</c>.</exception>
        internal static ClosingStructuresCalculationEntity CreateForClosingStructures(this StructuresCalculation<ClosingStructuresInput> calculation,
                                                                                      PersistenceRegistry registry, int order)
        {
            if (registry == null)
            {
                throw new ArgumentNullException("registry");
            }

            var entity = new ClosingStructuresCalculationEntity
            {
                Name = calculation.Name.DeepClone(),
                Comments = calculation.Comments.Body.DeepClone(),
                Order = order
            };
            SetInputValues(entity, calculation.InputParameters, registry);
            SetOutputEntity(calculation, entity);

            registry.Register(entity, calculation);

            return entity;
        }

        private static void SetInputValues(ClosingStructuresCalculationEntity entity, ClosingStructuresInput input,
                                           PersistenceRegistry registry)
        {
            input.Create(entity, registry);

            if (input.Structure != null)
            {
                entity.ClosingStructureEntity = registry.Get(input.Structure);
            }

            entity.InflowModelType = Convert.ToByte(input.InflowModelType);

            entity.InsideWaterLevelMean = input.InsideWaterLevel.Mean.Value.ToNaNAsNull();
            entity.InsideWaterLevelStandardDeviation = input.InsideWaterLevel.StandardDeviation.Value.ToNaNAsNull();

            entity.DeviationWaveDirection = input.DeviationWaveDirection.Value.ToNaNAsNull();

            entity.DrainCoefficientMean = input.DrainCoefficient.Mean.Value.ToNaNAsNull();

            entity.FactorStormDurationOpenStructure = input.FactorStormDurationOpenStructure.Value.ToNaNAsNull();

            entity.ThresholdHeightOpenWeirMean = input.ThresholdHeightOpenWeir.Mean.Value.ToNaNAsNull();
            entity.ThresholdHeightOpenWeirStandardDeviation = input.ThresholdHeightOpenWeir.StandardDeviation.Value.ToNaNAsNull();

            entity.AreaFlowAperturesMean = input.AreaFlowApertures.Mean.Value.ToNaNAsNull();
            entity.AreaFlowAperturesStandardDeviation = input.AreaFlowApertures.StandardDeviation.Value.ToNaNAsNull();

            entity.FailureProbabilityOpenStructure = input.FailureProbabilityOpenStructure;

            entity.FailureProbabilityReparation = input.FailureProbabilityReparation;

            entity.IdenticalApertures = input.IdenticalApertures;

            entity.LevelCrestStructureNotClosingMean = input.LevelCrestStructureNotClosing.Mean.Value.ToNaNAsNull();
            entity.LevelCrestStructureNotClosingStandardDeviation = input.LevelCrestStructureNotClosing.StandardDeviation.Value.ToNaNAsNull();

            entity.ProbabilityOrFrequencyOpenStructureBeforeFlooding = input.ProbabilityOrFrequencyOpenStructureBeforeFlooding;
        }

        private static void SetOutputEntity(StructuresCalculation<ClosingStructuresInput> calculation,
                                            ClosingStructuresCalculationEntity entity)
        {
            if (calculation.HasOutput)
            {
                entity.ClosingStructuresOutputEntities.Add(calculation.Output.Create<ClosingStructuresOutputEntity>());
            }
        }

        #endregion

        #region HeightStructures

        /// <summary>
        /// Creates a <see cref="HeightStructuresCalculationEntity"/> based
        /// on the information of the <see cref="StructuresCalculation{T}"/>.
        /// </summary>
        /// <param name="calculation">The calculation to create a database entity for.</param>
        /// <param name="registry">The object keeping track of create operations.</param>
        /// <param name="order">The index at where <paramref name="calculation"/> resides
        /// in its parent container.</param>
        /// <returns>A new <see cref="HeightStructuresCalculationEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="registry"/> is <c>null</c>.</exception>
        internal static HeightStructuresCalculationEntity CreateForHeightStructures(this StructuresCalculation<HeightStructuresInput> calculation,
                                                                                    PersistenceRegistry registry, int order)
        {
            if (registry == null)
            {
                throw new ArgumentNullException("registry");
            }

            var entity = new HeightStructuresCalculationEntity
            {
                Name = calculation.Name.DeepClone(),
                Comments = calculation.Comments.Body.DeepClone(),
                Order = order
            };
            SetInputValues(entity, calculation.InputParameters, registry);
            SetOutputEntity(calculation, entity);

            registry.Register(entity, calculation);

            return entity;
        }

        private static void SetInputValues(HeightStructuresCalculationEntity entity, HeightStructuresInput input,
                                           PersistenceRegistry registry)
        {
            input.Create(entity, registry);

            if (input.Structure != null)
            {
                entity.HeightStructureEntity = registry.Get(input.Structure);
            }

            entity.LevelCrestStructureMean = input.LevelCrestStructure.Mean.Value.ToNaNAsNull();
            entity.LevelCrestStructureStandardDeviation = input.LevelCrestStructure.StandardDeviation.Value.ToNaNAsNull();

            entity.DeviationWaveDirection = input.DeviationWaveDirection.Value.ToNaNAsNull();
        }

        private static void SetOutputEntity(StructuresCalculation<HeightStructuresInput> calculation,
                                            HeightStructuresCalculationEntity entity)
        {
            if (calculation.HasOutput)
            {
                entity.HeightStructuresOutputEntities.Add(calculation.Output.Create<HeightStructuresOutputEntity>());
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
        internal static StabilityPointStructuresCalculationEntity CreateForStabilityPointStructures(this StructuresCalculation<StabilityPointStructuresInput> calculation,
                                                                                                    PersistenceRegistry registry, int order)
        {
            if (registry == null)
            {
                throw new ArgumentNullException("registry");
            }

            var entity = new StabilityPointStructuresCalculationEntity
            {
                Name = calculation.Name.DeepClone(),
                Comments = calculation.Comments.Body.DeepClone(),
                Order = order
            };
            SetInputValues(entity, calculation.InputParameters, registry);
            SetOutputEntity(calculation, entity);

            registry.Register(entity, calculation);

            return entity;
        }

        private static void SetInputValues(StabilityPointStructuresCalculationEntity entity,
                                           StabilityPointStructuresInput input, PersistenceRegistry registry)
        {
            input.Create(entity, registry);

            if (input.Structure != null)
            {
                entity.StabilityPointStructureEntity = registry.Get(input.Structure);
            }

            entity.InsideWaterLevelMean = input.InsideWaterLevel.Mean.Value.ToNaNAsNull();
            entity.InsideWaterLevelStandardDeviation = input.InsideWaterLevel.StandardDeviation.Value.ToNaNAsNull();

            entity.ThresholdHeightOpenWeirMean = input.ThresholdHeightOpenWeir.Mean.Value.ToNaNAsNull();
            entity.ThresholdHeightOpenWeirStandardDeviation = input.ThresholdHeightOpenWeir.StandardDeviation.Value.ToNaNAsNull();

            entity.ConstructiveStrengthLinearLoadModelMean = input.ConstructiveStrengthLinearLoadModel.Mean.Value.ToNaNAsNull();
            entity.ConstructiveStrengthLinearLoadModelCoefficientOfVariation = input.ConstructiveStrengthLinearLoadModel.CoefficientOfVariation.Value.ToNaNAsNull();

            entity.ConstructiveStrengthQuadraticLoadModelMean = input.ConstructiveStrengthQuadraticLoadModel.Mean.Value.ToNaNAsNull();
            entity.ConstructiveStrengthQuadraticLoadModelCoefficientOfVariation = input.ConstructiveStrengthQuadraticLoadModel.CoefficientOfVariation.Value.ToNaNAsNull();

            entity.BankWidthMean = input.BankWidth.Mean.Value.ToNaNAsNull();
            entity.BankWidthStandardDeviation = input.BankWidth.StandardDeviation.Value.ToNaNAsNull();

            entity.InsideWaterLevelFailureConstructionMean = input.InsideWaterLevelFailureConstruction.Mean.Value.ToNaNAsNull();
            entity.InsideWaterLevelFailureConstructionStandardDeviation = input.InsideWaterLevelFailureConstruction.StandardDeviation.Value.ToNaNAsNull();

            entity.EvaluationLevel = input.EvaluationLevel.Value.ToNaNAsNull();

            entity.LevelCrestStructureMean = input.LevelCrestStructure.Mean.Value.ToNaNAsNull();
            entity.LevelCrestStructureStandardDeviation = input.LevelCrestStructure.StandardDeviation.Value.ToNaNAsNull();

            entity.VerticalDistance = input.VerticalDistance.Value.ToNaNAsNull();

            entity.FailureProbabilityRepairClosure = input.FailureProbabilityRepairClosure;

            entity.FailureCollisionEnergyMean = input.FailureCollisionEnergy.Mean.Value.ToNaNAsNull();
            entity.FailureCollisionEnergyCoefficientOfVariation = input.FailureCollisionEnergy.CoefficientOfVariation.Value.ToNaNAsNull();

            entity.ShipMassMean = input.ShipMass.Mean.Value.ToNaNAsNull();
            entity.ShipMassCoefficientOfVariation = input.ShipMass.CoefficientOfVariation.Value.ToNaNAsNull();

            entity.ShipVelocityMean = input.ShipVelocity.Mean.Value.ToNaNAsNull();
            entity.ShipVelocityCoefficientOfVariation = input.ShipVelocity.CoefficientOfVariation.Value.ToNaNAsNull();

            entity.LevellingCount = input.LevellingCount;

            entity.ProbabilityCollisionSecondaryStructure = input.ProbabilityCollisionSecondaryStructure;

            entity.FlowVelocityStructureClosableMean = input.FlowVelocityStructureClosable.Mean.Value.ToNaNAsNull();
            entity.FlowVelocityStructureClosableStandardDeviation = input.FlowVelocityStructureClosable.StandardDeviation.Value.ToNaNAsNull();

            entity.StabilityLinearLoadModelMean = input.StabilityLinearLoadModel.Mean.Value.ToNaNAsNull();
            entity.StabilityLinearLoadModelCoefficientOfVariation = input.StabilityLinearLoadModel.CoefficientOfVariation.Value.ToNaNAsNull();

            entity.StabilityQuadraticLoadModelMean = input.StabilityQuadraticLoadModel.Mean.Value.ToNaNAsNull();
            entity.StabilityQuadraticLoadModelCoefficientOfVariation = input.StabilityQuadraticLoadModel.CoefficientOfVariation.Value.ToNaNAsNull();

            entity.AreaFlowAperturesMean = input.AreaFlowApertures.Mean.Value.ToNaNAsNull();
            entity.AreaFlowAperturesStandardDeviation = input.AreaFlowApertures.StandardDeviation.Value.ToNaNAsNull();

            entity.InflowModelType = Convert.ToByte(input.InflowModelType);
            entity.LoadSchematizationType = Convert.ToByte(input.LoadSchematizationType);
            entity.VolumicWeightWater = input.VolumicWeightWater.Value.ToNaNAsNull();
            entity.FactorStormDurationOpenStructure = input.FactorStormDurationOpenStructure.Value.ToNaNAsNull();
            entity.DrainCoefficientMean = input.DrainCoefficient.Mean.Value.ToNaNAsNull();
        }

        private static void SetOutputEntity(StructuresCalculation<StabilityPointStructuresInput> calculation,
                                            StabilityPointStructuresCalculationEntity entity)
        {
            if (calculation.HasOutput)
            {
                entity.StabilityPointStructuresOutputEntities.Add(calculation.Output.Create<StabilityPointStructuresOutputEntity>());
            }
        }

        #endregion
    }
}