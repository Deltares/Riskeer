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
using System.Collections.Generic;
using System.ComponentModel;
using Core.Common.Base.Data;
using Core.Common.Gui.Attributes;
using Core.Common.Util;
using Core.Common.Util.Attributes;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.Forms.ChangeHandlers;
using Ringtoets.Common.Forms.PropertyClasses;
using Ringtoets.Common.Forms.TypeConverters;
using Ringtoets.StabilityPointStructures.Data;
using Ringtoets.StabilityPointStructures.Forms.PresentationObjects;
using Ringtoets.StabilityPointStructures.Forms.Properties;
using Ringtoets.StabilityPointStructures.Util;
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
        private const int drainCoefficientPropertyIndex = 6;
        private const int factorStormDurationOpenStructurePropertyIndex = 7;
        private const int structurePropertyIndex = 8;
        private const int structureLocationPropertyIndex = 9;
        private const int structureNormalOrientationPropertyIndex = 10;
        private const int inflowModelTypePropertyIndex = 11;
        private const int loadSchematizationTypePropertyIndex = 12;
        private const int widthFlowAperturesPropertyIndex = 13;
        private const int areaFlowAperturesPropertyIndex = 14;
        private const int flowWidthAtBottomProtectionPropertyIndex = 15;
        private const int storageStructureAreaPropertyIndex = 16;
        private const int allowedLevelIncreaseStoragePropertyIndex = 17;
        private const int levelCrestStructurePropertyIndex = 18;
        private const int thresholdHeightOpenWeirPropertyIndex = 19;
        private const int criticalOvertoppingDischargePropertyIndex = 20;
        private const int flowVelocityStructureClosablePropertyIndex = 21;
        private const int constructiveStrengthLinearLoadModelPropertyIndex = 22;
        private const int constructiveStrengthQuadraticLoadModelPropertyIndex = 23;
        private const int bankWidthPropertyIndex = 24;
        private const int evaluationLevelPropertyIndex = 25;
        private const int verticalDistancePropertyIndex = 26;
        private const int failureProbabilityRepairClosurePropertyIndex = 27;
        private const int failureCollisionEnergyPropertyIndex = 28;
        private const int shipMassPropertyIndex = 29;
        private const int shipVelocityPropertyIndex = 30;
        private const int levellingCountPropertyIndex = 31;
        private const int probabilityCollisionSecondaryStructurePropertyIndex = 32;
        private const int stabilityLinearLoadModelPropertyIndex = 33;
        private const int stabilityQuadraticLoadModelPropertyIndex = 34;
        private const int failureProbabilityStructureWithErosionPropertyIndex = 35;
        private const int foreshoreProfilePropertyIndex = 36;
        private const int useBreakWaterPropertyIndex = 37;
        private const int useForeshorePropertyIndex = 38;

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
            StabilityPointStructuresHelper.UpdateCalculationToSectionResultAssignments(data.FailureMechanism);
        }

        protected override bool ShouldPropertyBeReadOnlyInAbsenseOfStructure(string property)
        {
            return nameof(InflowModelType).Equals(property)
                   || nameof(LoadSchematizationType).Equals(property)
                   || nameof(LevellingCount).Equals(property)
                   || nameof(EvaluationLevel).Equals(property)
                   || nameof(VerticalDistance).Equals(property)
                   || nameof(FailureProbabilityRepairClosure).Equals(property)
                   || nameof(ProbabilityCollisionSecondaryStructure).Equals(property)
                   || base.ShouldPropertyBeReadOnlyInAbsenseOfStructure(property);
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
                PropertyChangeHelper.ChangePropertyAndNotify(() => data.WrappedData.VolumicWeightWater = value, PropertyChangeHandler);
            }
        }

        [PropertyOrder(insideWaterLevelFailureConstructionPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_HydraulicData))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.Structure_InsideWaterLevelFailureConstruction_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.Structure_InsideWaterLevelFailureConstruction_Description))]
        public NormalDistributionProperties InsideWaterLevelFailureConstruction
        {
            get
            {
                return new NormalDistributionProperties(
                    HasStructure()
                        ? DistributionPropertiesReadOnly.None
                        : DistributionPropertiesReadOnly.All,
                    data.WrappedData.InsideWaterLevelFailureConstruction,
                    PropertyChangeHandler);
            }
        }

        [PropertyOrder(insideWaterLevelPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_HydraulicData))]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Structure_InsideWaterLevel_DisplayName))]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Structure_InsideWaterLevel_Description))]
        public NormalDistributionProperties InsideWaterLevel
        {
            get
            {
                return new NormalDistributionProperties(
                    HasStructure()
                        ? DistributionPropertiesReadOnly.None
                        : DistributionPropertiesReadOnly.All,
                    data.WrappedData.InsideWaterLevel,
                    PropertyChangeHandler);
            }
        }

        #endregion

        #region Model factors and critical values

        [DynamicVisible]
        [PropertyOrder(drainCoefficientPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_ModelSettings))]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Structure_DrainCoefficient_DisplayName))]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Structure_DrainCoefficient_Description))]
        public NormalDistributionProperties DrainCoefficient
        {
            get
            {
                return new NormalDistributionProperties(
                    DistributionPropertiesReadOnly.StandardDeviation,
                    data.WrappedData.DrainCoefficient,
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
                PropertyChangeHelper.ChangePropertyAndNotify(() => data.WrappedData.FactorStormDurationOpenStructure = value, PropertyChangeHandler);
            }
        }

        #endregion

        #region Schematization

        [DynamicVisible]
        public override NormalDistributionProperties WidthFlowApertures
        {
            get
            {
                return base.WidthFlowApertures;
            }
        }

        [DynamicReadOnly]
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
                PropertyChangeHelper.ChangePropertyAndNotify(() => data.WrappedData.InflowModelType = value, PropertyChangeHandler);
            }
        }

        [DynamicReadOnly]
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
                PropertyChangeHelper.ChangePropertyAndNotify(() => data.WrappedData.LoadSchematizationType = value, PropertyChangeHandler);
            }
        }

        [PropertyOrder(levelCrestStructurePropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_Schematization))]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Structure_LevelCrestStructure_DisplayName))]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Structure_LevelCrestStructure_Description))]
        public NormalDistributionProperties LevelCrestStructure
        {
            get
            {
                return new NormalDistributionProperties(
                    HasStructure()
                        ? DistributionPropertiesReadOnly.None
                        : DistributionPropertiesReadOnly.All,
                    data.WrappedData.LevelCrestStructure,
                    PropertyChangeHandler);
            }
        }

        [PropertyOrder(thresholdHeightOpenWeirPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_Schematization))]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Structure_ThresholdHeightOpenWeir_DisplayName))]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Structure_ThresholdHeightOpenWeir_Description))]
        public NormalDistributionProperties ThresholdHeightOpenWeir
        {
            get
            {
                return new NormalDistributionProperties(
                    HasStructure()
                        ? DistributionPropertiesReadOnly.None
                        : DistributionPropertiesReadOnly.All,
                    data.WrappedData.ThresholdHeightOpenWeir,
                    PropertyChangeHandler);
            }
        }

        [PropertyOrder(flowVelocityStructureClosablePropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_Schematization))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.Structure_FlowVelocityStructureClosable_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.Structure_FlowVelocityStructureClosable_Description))]
        public VariationCoefficientNormalDistributionProperties FlowVelocityStructureClosable
        {
            get
            {
                return new VariationCoefficientNormalDistributionProperties(
                    HasStructure()
                        ? VariationCoefficientDistributionPropertiesReadOnly.CoefficientOfVariation
                        : VariationCoefficientDistributionPropertiesReadOnly.All,
                    data.WrappedData.FlowVelocityStructureClosable,
                    PropertyChangeHandler);
            }
        }

        [DynamicVisible]
        [PropertyOrder(areaFlowAperturesPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_Schematization))]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Structure_AreaFlowApertures_DisplayName))]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Structure_AreaFlowApertures_Description))]
        public LogNormalDistributionProperties AreaFlowApertures
        {
            get
            {
                return new LogNormalDistributionProperties(
                    HasStructure()
                        ? DistributionPropertiesReadOnly.None
                        : DistributionPropertiesReadOnly.All,
                    data.WrappedData.AreaFlowApertures,
                    PropertyChangeHandler);
            }
        }

        [DynamicVisible]
        [PropertyOrder(constructiveStrengthLinearLoadModelPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_Schematization))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.Structure_ConstructiveStrengthLinearLoadModel_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.Structure_ConstructiveStrengthLinearLoadModel_Description))]
        public VariationCoefficientLogNormalDistributionProperties ConstructiveStrengthLinearLoadModel
        {
            get
            {
                return new VariationCoefficientLogNormalDistributionProperties(
                    HasStructure()
                        ? VariationCoefficientDistributionPropertiesReadOnly.None
                        : VariationCoefficientDistributionPropertiesReadOnly.All,
                    data.WrappedData.ConstructiveStrengthLinearLoadModel,
                    PropertyChangeHandler);
            }
        }

        [DynamicVisible]
        [PropertyOrder(constructiveStrengthQuadraticLoadModelPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_Schematization))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.Structure_ConstructiveStrengthQuadraticLoadModel_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.Structure_ConstructiveStrengthQuadraticLoadModel_Description))]
        public VariationCoefficientLogNormalDistributionProperties ConstructiveStrengthQuadraticLoadModel
        {
            get
            {
                return new VariationCoefficientLogNormalDistributionProperties(
                    HasStructure()
                        ? VariationCoefficientDistributionPropertiesReadOnly.None
                        : VariationCoefficientDistributionPropertiesReadOnly.All,
                    data.WrappedData.ConstructiveStrengthQuadraticLoadModel,
                    PropertyChangeHandler);
            }
        }

        [DynamicVisible]
        [PropertyOrder(stabilityLinearLoadModelPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_Schematization))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.Structure_StabilityLinearLoadModel_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.Structure_StabilityLinearLoadModel_Description))]
        public VariationCoefficientLogNormalDistributionProperties StabilityLinearLoadModel
        {
            get
            {
                return new VariationCoefficientLogNormalDistributionProperties(
                    HasStructure()
                        ? VariationCoefficientDistributionPropertiesReadOnly.None
                        : VariationCoefficientDistributionPropertiesReadOnly.All,
                    data.WrappedData.StabilityLinearLoadModel,
                    PropertyChangeHandler);
            }
        }

        [DynamicVisible]
        [PropertyOrder(stabilityQuadraticLoadModelPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_Schematization))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.Structure_StabilityQuadraticLoadModel_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.Structure_StabilityQuadraticLoadModel_Description))]
        public VariationCoefficientLogNormalDistributionProperties StabilityQuadraticLoadModel
        {
            get
            {
                return new VariationCoefficientLogNormalDistributionProperties(
                    HasStructure()
                        ? VariationCoefficientDistributionPropertiesReadOnly.None
                        : VariationCoefficientDistributionPropertiesReadOnly.All,
                    data.WrappedData.StabilityQuadraticLoadModel,
                    PropertyChangeHandler);
            }
        }

        [DynamicReadOnly]
        [PropertyOrder(failureProbabilityRepairClosurePropertyIndex)]
        [TypeConverter(typeof(NoProbabilityValueDoubleConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_Schematization))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.Structure_FailureProbabilityRepairClosure_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.Structure_FailureProbabilityRepairClosure_Description))]
        public double FailureProbabilityRepairClosure
        {
            get
            {
                return data.WrappedData.FailureProbabilityRepairClosure;
            }
            set
            {
                PropertyChangeHelper.ChangePropertyAndNotify(() => data.WrappedData.FailureProbabilityRepairClosure = value, PropertyChangeHandler);
            }
        }

        [PropertyOrder(failureCollisionEnergyPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_Schematization))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.Structure_FailureCollisionEnergy_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.Structure_FailureCollisionEnergy_Description))]
        public VariationCoefficientLogNormalDistributionProperties FailureCollisionEnergy
        {
            get
            {
                return new VariationCoefficientLogNormalDistributionProperties(
                    HasStructure()
                        ? VariationCoefficientDistributionPropertiesReadOnly.None
                        : VariationCoefficientDistributionPropertiesReadOnly.All,
                    data.WrappedData.FailureCollisionEnergy,
                    PropertyChangeHandler);
            }
        }

        [PropertyOrder(shipMassPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_Schematization))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.Structure_ShipMass_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.Structure_ShipMass_Description))]
        public VariationCoefficientNormalDistributionProperties ShipMass
        {
            get
            {
                return new VariationCoefficientNormalDistributionProperties(
                    HasStructure()
                        ? VariationCoefficientDistributionPropertiesReadOnly.None
                        : VariationCoefficientDistributionPropertiesReadOnly.All,
                    data.WrappedData.ShipMass,
                    PropertyChangeHandler);
            }
        }

        [PropertyOrder(shipVelocityPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_Schematization))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.Structure_ShipVelocity_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.Structure_ShipVelocity_Description))]
        public VariationCoefficientNormalDistributionProperties ShipVelocity
        {
            get
            {
                return new VariationCoefficientNormalDistributionProperties(
                    HasStructure()
                        ? VariationCoefficientDistributionPropertiesReadOnly.None
                        : VariationCoefficientDistributionPropertiesReadOnly.All,
                    data.WrappedData.ShipVelocity,
                    PropertyChangeHandler);
            }
        }

        [DynamicReadOnly]
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
                PropertyChangeHelper.ChangePropertyAndNotify(() => data.WrappedData.LevellingCount = value, PropertyChangeHandler);
            }
        }

        [DynamicReadOnly]
        [PropertyOrder(probabilityCollisionSecondaryStructurePropertyIndex)]
        [TypeConverter(typeof(NoProbabilityValueDoubleConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_Schematization))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.Structure_ProbabilityCollisionSecondaryStructure_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.Structure_ProbabilityCollisionSecondaryStructure_Description))]
        public double ProbabilityCollisionSecondaryStructure
        {
            get
            {
                return data.WrappedData.ProbabilityCollisionSecondaryStructure;
            }
            set
            {
                PropertyChangeHelper.ChangePropertyAndNotify(() => data.WrappedData.ProbabilityCollisionSecondaryStructure = value, PropertyChangeHandler);
            }
        }

        [PropertyOrder(bankWidthPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_Schematization))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.Structure_BankWidth_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.Structure_BankWidth_Description))]
        public NormalDistributionProperties BankWidth
        {
            get
            {
                return new NormalDistributionProperties(
                    HasStructure()
                        ? DistributionPropertiesReadOnly.None
                        : DistributionPropertiesReadOnly.All,
                    data.WrappedData.BankWidth,
                    PropertyChangeHandler);
            }
        }

        [DynamicReadOnly]
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
                PropertyChangeHelper.ChangePropertyAndNotify(() => data.WrappedData.EvaluationLevel = value, PropertyChangeHandler);
            }
        }

        [DynamicReadOnly]
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
                PropertyChangeHelper.ChangePropertyAndNotify(() => data.WrappedData.VerticalDistance = value, PropertyChangeHandler);
            }
        }

        #endregion
    }
}