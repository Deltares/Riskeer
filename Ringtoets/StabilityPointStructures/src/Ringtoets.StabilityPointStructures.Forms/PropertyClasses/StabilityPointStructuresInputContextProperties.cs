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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Core.Common.Base.Data;
using Core.Common.Gui.Attributes;
using Core.Common.Utils;
using Core.Common.Utils.Attributes;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.Forms.ChangeHandlers;
using Ringtoets.Common.Forms.Helpers;
using Ringtoets.Common.Forms.PropertyClasses;
using Ringtoets.Common.Utils;
using Ringtoets.StabilityPointStructures.Data;
using Ringtoets.StabilityPointStructures.Forms.PresentationObjects;
using Ringtoets.StabilityPointStructures.Forms.Properties;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.StabilityPointStructures.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="StabilityPointStructuresInputContext"/> for properties panel.
    /// </summary>
    public class StabilityPointStructuresInputContextProperties : StructuresInputBaseProperties<
        StabilityPointStructure,
        StabilityPointStructuresInput,
        StructuresCalculation<StabilityPointStructuresInput>,
        StabilityPointStructuresFailureMechanism>
    {
        private const int hydraulicBoundaryLocationPropertyIndex = 1;
        private const int volumicWeightWaterPropertyIndex = 2;
        private const int stormDurationPropertyIndex = 3;
        private const int insideWaterLevelPropertyIndex = 4;
        private const int insideWaterLevelFailureConstructionPropertyIndex = 5;
        private const int modelFactorSuperCriticalFlowPropertyIndex = 6;
        private const int drainCoefficientPropertyIndex = 7;
        private const int factorStormDurationOpenStructurePropertyIndex = 8;
        private const int structurePropertyIndex = 9;
        private const int structureLocationPropertyIndex = 10;
        private const int structureNormalOrientationPropertyIndex = 11;
        private const int inflowModelTypePropertyIndex = 12;
        private const int loadSchematizationTypePropertyIndex = 13;
        private const int widthFlowAperturesPropertyIndex = 14;
        private const int areaFlowAperturesPropertyIndex = 15;
        private const int flowWidthAtBottomProtectionPropertyIndex = 16;
        private const int storageStructureAreaPropertyIndex = 17;
        private const int allowedLevelIncreaseStoragePropertyIndex = 18;
        private const int levelCrestStructurePropertyIndex = 19;
        private const int thresholdHeightOpenWeirPropertyIndex = 20;
        private const int criticalOvertoppingDischargePropertyIndex = 21;
        private const int flowVelocityStructureClosablePropertyIndex = 22;
        private const int constructiveStrengthLinearLoadModelPropertyIndex = 23;
        private const int constructiveStrengthQuadraticLoadModelPropertyIndex = 24;
        private const int bankWidthPropertyIndex = 25;
        private const int evaluationLevelPropertyIndex = 26;
        private const int verticalDistancePropertyIndex = 27;
        private const int failureProbabilityRepairClosurePropertyIndex = 28;
        private const int failureCollisionEnergyPropertyIndex = 29;
        private const int shipMassPropertyIndex = 30;
        private const int shipVelocityPropertyIndex = 31;
        private const int levellingCountPropertyIndex = 32;
        private const int probabilityCollisionSecondaryStructurePropertyIndex = 33;
        private const int stabilityLinearLoadModelPropertyIndex = 34;
        private const int stabilityQuadraticLoadModelPropertyIndex = 35;
        private const int failureProbabilityStructureWithErosionPropertyIndex = 36;
        private const int foreshoreProfilePropertyIndex = 37;
        private const int useBreakWaterPropertyIndex = 38;
        private const int useForeshorePropertyIndex = 39;

        /// <summary>
        /// Creates a new instance of the <see cref="StabilityPointStructuresInputContextProperties"/> class.
        /// </summary>
        /// <param name="data">The instance to show the properties of.</param>
        /// <param name="propertyChangeHandler">The handler responsible for handling effects of a property change.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public StabilityPointStructuresInputContextProperties(StabilityPointStructuresInputContext data, IObservablePropertyChangeHandler propertyChangeHandler)
            : base(data, new ConstructionProperties
            {
                StructurePropertyIndex = structurePropertyIndex,
                StructureLocationPropertyIndex = structureLocationPropertyIndex,
                StructureNormalOrientationPropertyIndex = structureNormalOrientationPropertyIndex,
                FlowWidthAtBottomProtectionPropertyIndex = flowWidthAtBottomProtectionPropertyIndex,
                WidthFlowAperturesPropertyIndex = widthFlowAperturesPropertyIndex,
                StorageStructureAreaPropertyIndex = storageStructureAreaPropertyIndex,
                AllowedLevelIncreaseStoragePropertyIndex = allowedLevelIncreaseStoragePropertyIndex,
                CriticalOvertoppingDischargePropertyIndex = criticalOvertoppingDischargePropertyIndex,
                FailureProbabilityStructureWithErosionPropertyIndex = failureProbabilityStructureWithErosionPropertyIndex,
                ForeshoreProfilePropertyIndex = foreshoreProfilePropertyIndex,
                UseBreakWaterPropertyIndex = useBreakWaterPropertyIndex,
                UseForeshorePropertyIndex = useForeshorePropertyIndex,
                ModelFactorSuperCriticalFlowPropertyIndex = modelFactorSuperCriticalFlowPropertyIndex,
                HydraulicBoundaryLocationPropertyIndex = hydraulicBoundaryLocationPropertyIndex,
                StormDurationPropertyIndex = stormDurationPropertyIndex
            }, propertyChangeHandler) {}

        [DynamicVisibleValidationMethod]
        public bool DynamicVisibleValidationMethod(string propertyName)
        {
            // Note: A default initialized calculation doesn't have InflowModelType initialized to any of these 2 values.
            if (data.WrappedData.InflowModelType == StabilityPointStructureInflowModelType.FloodedCulvert ||
                data.WrappedData.InflowModelType == StabilityPointStructureInflowModelType.LowSill)
            {
                if (propertyName == nameof(ModelFactorSuperCriticalFlow))
                {
                    return data.WrappedData.InflowModelType == StabilityPointStructureInflowModelType.LowSill;
                }
                if (propertyName == nameof(DrainCoefficient))
                {
                    return data.WrappedData.InflowModelType == StabilityPointStructureInflowModelType.FloodedCulvert;
                }
                if (propertyName == nameof(AreaFlowApertures))
                {
                    return data.WrappedData.InflowModelType == StabilityPointStructureInflowModelType.FloodedCulvert;
                }
                if (propertyName == nameof(WidthFlowApertures))
                {
                    return data.WrappedData.InflowModelType == StabilityPointStructureInflowModelType.LowSill;
                }
            }

            // Note: A default initialized calculation doesn't have LoadSchematizationType initialized to any of these 2 values.
            if (data.WrappedData.LoadSchematizationType == LoadSchematizationType.Linear ||
                data.WrappedData.LoadSchematizationType == LoadSchematizationType.Quadratic)
            {
                if (propertyName == nameof(ConstructiveStrengthLinearLoadModel))
                {
                    return data.WrappedData.LoadSchematizationType == LoadSchematizationType.Linear;
                }
                if (propertyName == nameof(ConstructiveStrengthQuadraticLoadModel))
                {
                    return data.WrappedData.LoadSchematizationType == LoadSchematizationType.Quadratic;
                }
                if (propertyName == nameof(StabilityLinearLoadModel))
                {
                    return data.WrappedData.LoadSchematizationType == LoadSchematizationType.Linear;
                }
                if (propertyName == nameof(StabilityQuadraticLoadModel))
                {
                    return data.WrappedData.LoadSchematizationType == LoadSchematizationType.Quadratic;
                }
            }

            return true;
        }

        public override IEnumerable<ForeshoreProfile> GetAvailableForeshoreProfiles()
        {
            return data.FailureMechanism.ForeshoreProfiles;
        }

        public override IEnumerable<StabilityPointStructure> GetAvailableStructures()
        {
            return data.FailureMechanism.StabilityPointStructures;
        }

        protected override void AfterSettingStructure()
        {
            StructuresHelper.UpdateCalculationToSectionResultAssignments(
                data.FailureMechanism.SectionResults,
                data.FailureMechanism.Calculations.Cast<StructuresCalculation<StabilityPointStructuresInput>>());
        }

        #region Hydraulic data

        [PropertyOrder(volumicWeightWaterPropertyIndex)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_HydraulicData))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.Structure_VolumicWeightWater_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.Structure_VolumicWeightWater_Description))]
        public RoundedDouble VolumicWeightWater
        {
            get
            {
                return data.WrappedData.VolumicWeightWater;
            }
            set
            {
                ChangePropertyAndNotify(
                    (input, newValue) => data.WrappedData.VolumicWeightWater = newValue, value);
            }
        }

        [PropertyOrder(insideWaterLevelFailureConstructionPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_HydraulicData))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.Structure_InsideWaterLevelFailureConstruction_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.Structure_InsideWaterLevelFailureConstruction_Description))]
        public ConfirmingNormalDistributionProperties<StabilityPointStructuresInput> InsideWaterLevelFailureConstruction
        {
            get
            {
                return new ConfirmingNormalDistributionProperties<StabilityPointStructuresInput>(
                    DistributionPropertiesReadOnly.None,
                    data.WrappedData.InsideWaterLevelFailureConstruction,
                    data.Calculation,
                    data.WrappedData,
                    PropertyChangeHandler);
            }
        }

        [PropertyOrder(insideWaterLevelPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_HydraulicData))]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Structure_InsideWaterLevel_DisplayName))]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Structure_InsideWaterLevel_Description))]
        public ConfirmingNormalDistributionProperties<StabilityPointStructuresInput> InsideWaterLevel
        {
            get
            {
                return new ConfirmingNormalDistributionProperties<StabilityPointStructuresInput>(
                    DistributionPropertiesReadOnly.None,
                    data.WrappedData.InsideWaterLevel,
                    data.Calculation,
                    data.WrappedData,
                    PropertyChangeHandler);
            }
        }

        #endregion

        #region Model factors and critical values

        [DynamicVisible]
        public override ConfirmingNormalDistributionProperties<StabilityPointStructuresInput> ModelFactorSuperCriticalFlow
        {
            get
            {
                return base.ModelFactorSuperCriticalFlow;
            }
        }

        [DynamicVisible]
        [PropertyOrder(drainCoefficientPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_ModelSettings))]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Structure_DrainCoefficient_DisplayName))]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Structure_DrainCoefficient_Description))]
        public ConfirmingNormalDistributionProperties<StabilityPointStructuresInput> DrainCoefficient
        {
            get
            {
                return new ConfirmingNormalDistributionProperties<StabilityPointStructuresInput>(
                    DistributionPropertiesReadOnly.StandardDeviation,
                    data.WrappedData.DrainCoefficient,
                    data.Calculation,
                    data.WrappedData,
                    PropertyChangeHandler);
            }
        }

        [PropertyOrder(factorStormDurationOpenStructurePropertyIndex)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_ModelSettings))]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Structure_FactorStormDurationOpenStructure_DisplayName))]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Structure_FactorStormDurationOpenStructure_Description))]
        public RoundedDouble FactorStormDurationOpenStructure
        {
            get
            {
                return data.WrappedData.FactorStormDurationOpenStructure;
            }
            set
            {
                ChangePropertyAndNotify(
                    (input, newValue) => data.WrappedData.FactorStormDurationOpenStructure = newValue, value);
            }
        }

        #endregion

        #region Schematization

        [DynamicVisible]
        public override ConfirmingNormalDistributionProperties<StabilityPointStructuresInput> WidthFlowApertures
        {
            get
            {
                return base.WidthFlowApertures;
            }
        }

        [PropertyOrder(inflowModelTypePropertyIndex)]
        [TypeConverter(typeof(EnumTypeConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_Schematization))]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Structure_InflowModelType_DisplayName))]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Structure_InflowModelType_Description))]
        public StabilityPointStructureInflowModelType InflowModelType
        {
            get
            {
                return data.WrappedData.InflowModelType;
            }
            set
            {
                ChangePropertyAndNotify(
                    (input, newValue) => data.WrappedData.InflowModelType = newValue, value);
            }
        }

        [PropertyOrder(loadSchematizationTypePropertyIndex)]
        [TypeConverter(typeof(EnumTypeConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_Schematization))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.Structure_LoadSchematizationType_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.Structure_LoadSchematizationType_Description))]
        public LoadSchematizationType LoadSchematizationType
        {
            get
            {
                return data.WrappedData.LoadSchematizationType;
            }
            set
            {
                ChangePropertyAndNotify(
                    (input, newValue) => data.WrappedData.LoadSchematizationType = newValue, value);
            }
        }

        [PropertyOrder(levelCrestStructurePropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_Schematization))]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Structure_LevelCrestStructure_DisplayName))]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Structure_LevelCrestStructure_Description))]
        public ConfirmingNormalDistributionProperties<StabilityPointStructuresInput> LevelCrestStructure
        {
            get
            {
                return new ConfirmingNormalDistributionProperties<StabilityPointStructuresInput>(
                    DistributionPropertiesReadOnly.None,
                    data.WrappedData.LevelCrestStructure,
                    data.Calculation,
                    data.WrappedData,
                    PropertyChangeHandler);
            }
        }

        [PropertyOrder(thresholdHeightOpenWeirPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_Schematization))]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Structure_ThresholdHeightOpenWeir_DisplayName))]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Structure_ThresholdHeightOpenWeir_Description))]
        public ConfirmingNormalDistributionProperties<StabilityPointStructuresInput> ThresholdHeightOpenWeir
        {
            get
            {
                return new ConfirmingNormalDistributionProperties<StabilityPointStructuresInput>(
                    DistributionPropertiesReadOnly.None,
                    data.WrappedData.ThresholdHeightOpenWeir,
                    data.Calculation,
                    data.WrappedData,
                    PropertyChangeHandler);
            }
        }

        [PropertyOrder(flowVelocityStructureClosablePropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_Schematization")]
        [ResourcesDisplayName(typeof(Resources), "Structure_FlowVelocityStructureClosable_DisplayName")]
        [ResourcesDescription(typeof(Resources), "Structure_FlowVelocityStructureClosable_Description")]
        public ConfirmingVariationCoefficientNormalDistributionProperties<StabilityPointStructuresInput> FlowVelocityStructureClosable
        {
            get
            {
                return new ConfirmingVariationCoefficientNormalDistributionProperties<StabilityPointStructuresInput>(
                    VariationCoefficientDistributionPropertiesReadOnly.CoefficientOfVariation,
                    data.WrappedData.FlowVelocityStructureClosable,
                    data.Calculation,
                    data.WrappedData,
                    PropertyChangeHandler);
            }
        }

        [DynamicVisible]
        [PropertyOrder(areaFlowAperturesPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_Schematization))]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Structure_AreaFlowApertures_DisplayName))]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Structure_AreaFlowApertures_Description))]
        public ConfirmingLogNormalDistributionProperties<StabilityPointStructuresInput> AreaFlowApertures
        {
            get
            {
                return new ConfirmingLogNormalDistributionProperties<StabilityPointStructuresInput>(
                    DistributionPropertiesReadOnly.None,
                    data.WrappedData.AreaFlowApertures,
                    data.Calculation,
                    data.WrappedData,
                    PropertyChangeHandler);
            }
        }

        [DynamicVisible]
        [PropertyOrder(constructiveStrengthLinearLoadModelPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_Schematization))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.Structure_ConstructiveStrengthLinearLoadModel_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.Structure_ConstructiveStrengthLinearLoadModel_Description))]
        public ConfirmingVariationCoefficientLogNormalDistributionProperties<StabilityPointStructuresInput> ConstructiveStrengthLinearLoadModel
        {
            get
            {
                return new ConfirmingVariationCoefficientLogNormalDistributionProperties<StabilityPointStructuresInput>(
                    VariationCoefficientDistributionPropertiesReadOnly.None,
                    data.WrappedData.ConstructiveStrengthLinearLoadModel,
                    data.Calculation,
                    data.WrappedData,
                    PropertyChangeHandler);
            }
        }

        [DynamicVisible]
        [PropertyOrder(constructiveStrengthQuadraticLoadModelPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_Schematization))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.Structure_ConstructiveStrengthQuadraticLoadModel_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.Structure_ConstructiveStrengthQuadraticLoadModel_Description))]
        public ConfirmingVariationCoefficientLogNormalDistributionProperties<StabilityPointStructuresInput> ConstructiveStrengthQuadraticLoadModel
        {
            get
            {
                return new ConfirmingVariationCoefficientLogNormalDistributionProperties<StabilityPointStructuresInput>(
                    VariationCoefficientDistributionPropertiesReadOnly.None,
                    data.WrappedData.ConstructiveStrengthQuadraticLoadModel,
                    data.Calculation,
                    data.WrappedData,
                    PropertyChangeHandler);
            }
        }

        [DynamicVisible]
        [PropertyOrder(stabilityLinearLoadModelPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_Schematization))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.Structure_StabilityLinearLoadModel_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.Structure_StabilityLinearLoadModel_Description))]
        public ConfirmingVariationCoefficientLogNormalDistributionProperties<StabilityPointStructuresInput> StabilityLinearLoadModel
        {
            get
            {
                return new ConfirmingVariationCoefficientLogNormalDistributionProperties<StabilityPointStructuresInput>(
                    VariationCoefficientDistributionPropertiesReadOnly.None,
                    data.WrappedData.StabilityLinearLoadModel,
                    data.Calculation,
                    data.WrappedData,
                    PropertyChangeHandler);
            }
        }

        [DynamicVisible]
        [PropertyOrder(stabilityQuadraticLoadModelPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_Schematization))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.Structure_StabilityQuadraticLoadModel_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.Structure_StabilityQuadraticLoadModel_Description))]
        public ConfirmingVariationCoefficientLogNormalDistributionProperties<StabilityPointStructuresInput> StabilityQuadraticLoadModel
        {
            get
            {
                return new ConfirmingVariationCoefficientLogNormalDistributionProperties<StabilityPointStructuresInput>(
                    VariationCoefficientDistributionPropertiesReadOnly.None,
                    data.WrappedData.StabilityQuadraticLoadModel,
                    data.Calculation,
                    data.WrappedData,
                    PropertyChangeHandler);
            }
        }

        [PropertyOrder(failureProbabilityRepairClosurePropertyIndex)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_Schematization))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.Structure_FailureProbabilityRepairClosure_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.Structure_FailureProbabilityRepairClosure_Description))]
        public string FailureProbabilityRepairClosure
        {
            get
            {
                return ProbabilityFormattingHelper.Format(data.WrappedData.FailureProbabilityRepairClosure);
            }
            set
            {
                ChangePropertyAndNotify(
                    (input, newValue) =>
                        SetProbabilityValue(
                            newValue, 
                            data.WrappedData, 
                            (wrappedData, parsedValue) => wrappedData.FailureProbabilityRepairClosure = parsedValue),
                    value);
            }
        }

        [PropertyOrder(failureCollisionEnergyPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_Schematization))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.Structure_FailureCollisionEnergy_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.Structure_FailureCollisionEnergy_Description))]
        public ConfirmingVariationCoefficientLogNormalDistributionProperties<StabilityPointStructuresInput> FailureCollisionEnergy
        {
            get
            {
                return new ConfirmingVariationCoefficientLogNormalDistributionProperties<StabilityPointStructuresInput>(
                    VariationCoefficientDistributionPropertiesReadOnly.None,
                    data.WrappedData.FailureCollisionEnergy,
                    data.Calculation,
                    data.WrappedData,
                    PropertyChangeHandler);
            }
        }

        [PropertyOrder(shipMassPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_Schematization))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.Structure_ShipMass_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.Structure_ShipMass_Description))]
        public ConfirmingVariationCoefficientNormalDistributionProperties<StabilityPointStructuresInput> ShipMass
        {
            get
            {
                return new ConfirmingVariationCoefficientNormalDistributionProperties<StabilityPointStructuresInput>(
                    VariationCoefficientDistributionPropertiesReadOnly.None,
                    data.WrappedData.ShipMass,
                    data.Calculation,
                    data.WrappedData,
                    PropertyChangeHandler);
            }
        }

        [PropertyOrder(shipVelocityPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_Schematization))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.Structure_ShipVelocity_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.Structure_ShipVelocity_Description))]
        public ConfirmingVariationCoefficientNormalDistributionProperties<StabilityPointStructuresInput> ShipVelocity
        {
            get
            {
                return new ConfirmingVariationCoefficientNormalDistributionProperties<StabilityPointStructuresInput>(
                    VariationCoefficientDistributionPropertiesReadOnly.None,
                    data.WrappedData.ShipVelocity,
                    data.Calculation,
                    data.WrappedData,
                    PropertyChangeHandler);
            }
        }

        [PropertyOrder(levellingCountPropertyIndex)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_Schematization))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.Structure_LevellingCount_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.Structure_LevellingCount_Description))]
        public int LevellingCount
        {
            get
            {
                return data.WrappedData.LevellingCount;
            }
            set
            {
                ChangePropertyAndNotify(
                    (input, newValue) => data.WrappedData.LevellingCount = newValue, value);
            }
        }

        [PropertyOrder(probabilityCollisionSecondaryStructurePropertyIndex)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_Schematization))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.Structure_ProbabilityCollisionSecondaryStructure_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.Structure_ProbabilityCollisionSecondaryStructure_Description))]
        public string ProbabilityCollisionSecondaryStructure
        {
            get
            {
                return ProbabilityFormattingHelper.Format(data.WrappedData.ProbabilityCollisionSecondaryStructure);
            }
            set
            {
                ChangePropertyAndNotify(
                    (input, newValue) =>
                        SetProbabilityValue(
                            newValue,
                            data.WrappedData,
                            (wrappedData, parsedValue) => wrappedData.ProbabilityCollisionSecondaryStructure = parsedValue),
                    value);
            }
        }

        [PropertyOrder(bankWidthPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_Schematization))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.Structure_BankWidth_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.Structure_BankWidth_Description))]
        public ConfirmingNormalDistributionProperties<StabilityPointStructuresInput> BankWidth
        {
            get
            {
                return new ConfirmingNormalDistributionProperties<StabilityPointStructuresInput>(
                    DistributionPropertiesReadOnly.None,
                    data.WrappedData.BankWidth,
                    data.Calculation,
                    data.WrappedData,
                    PropertyChangeHandler);
            }
        }

        [PropertyOrder(evaluationLevelPropertyIndex)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_Schematization))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.Structure_EvaluationLevel_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.Structure_EvaluationLevel_Description))]
        public RoundedDouble EvaluationLevel
        {
            get
            {
                return data.WrappedData.EvaluationLevel;
            }
            set
            {
                ChangePropertyAndNotify(
                    (input, newValue) => data.WrappedData.EvaluationLevel = newValue, value);
            }
        }

        [PropertyOrder(verticalDistancePropertyIndex)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_Schematization))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.Structure_VerticalDistance_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.Structure_VerticalDistance_Description))]
        public RoundedDouble VerticalDistance
        {
            get
            {
                return data.WrappedData.VerticalDistance;
            }
            set
            {
                ChangePropertyAndNotify(
                    (input, newValue) => data.WrappedData.VerticalDistance = newValue, value);
            }
        }

        #endregion
    }
}