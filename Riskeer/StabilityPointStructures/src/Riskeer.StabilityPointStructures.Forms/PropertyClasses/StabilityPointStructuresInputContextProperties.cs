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
using System.Collections.Generic;
using System.ComponentModel;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.Gui.Attributes;
using Core.Common.Util;
using Core.Common.Util.Attributes;
using Riskeer.Common.Data.DikeProfiles;
using Riskeer.Common.Data.Structures;
using Riskeer.Common.Forms.ChangeHandlers;
using Riskeer.Common.Forms.PresentationObjects;
using Riskeer.Common.Forms.PropertyClasses;
using Riskeer.Common.Forms.TypeConverters;
using Riskeer.StabilityPointStructures.Data;
using Riskeer.StabilityPointStructures.Forms.PresentationObjects;
using Riskeer.StabilityPointStructures.Forms.Properties;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.StabilityPointStructures.Forms.PropertyClasses
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
        private const int generalCategoryIndex = 1;
        private const int hydraulicLoadsCategoryIndex = 2;
        private const int constructiveFailureCategoryIndex = 3;
        private const int schematizationIncomingFlowCategoryIndex = 4;
        private const int schematizationGroundErosionCategoryIndex = 5;
        private const int schematizationStorageStructureCategoryIndex = 6;
        private const int constructiveInstabilityCategoryIndex = 7;
        private const int collisionSecondaryStructureCategoryIndex = 8;
        private const int foreshoreCategoryIndex = 9;
        private const int outputSettingsCategoryIndex = 10;
        private const int totalNrOfCategories = 10;

        private const int structurePropertyIndex = 0;
        private const int structureLocationPropertyIndex = 1;
        private const int hydraulicBoundaryLocationPropertyIndex = 2;

        private const int volumicWeightWaterPropertyIndex = 3;
        private const int insideWaterLevelFailureConstructionPropertyIndex = 4;
        private const int bankWidthPropertyIndex = 5;
        private const int verticalDistancePropertyIndex = 6;

        private const int loadSchematizationTypePropertyIndex = 7;
        private const int constructiveStrengthLinearLoadModelPropertyIndex = 8;
        private const int constructiveStrengthQuadraticLoadModelPropertyIndex = 9;
        private const int evaluationLevelPropertyIndex = 10;
        private const int failureProbabilityRepairClosurePropertyIndex = 11;

        private const int inflowModelTypePropertyIndex = 12;
        private const int structureNormalOrientationPropertyIndex = 13;
        private const int levelCrestStructurePropertyIndex = 14;
        private const int insideWaterLevelPropertyIndex = 15;
        private const int thresholdHeightOpenWeirPropertyIndex = 16;
        private const int widthFlowAperturesPropertyIndex = 17;
        private const int areaFlowAperturesPropertyIndex = 18;
        private const int drainCoefficientPropertyIndex = 19;
        private const int stormDurationPropertyIndex = 20;
        private const int factorStormDurationOpenStructurePropertyIndex = 21;

        private const int criticalOvertoppingDischargePropertyIndex = 22;
        private const int flowWidthAtBottomProtectionPropertyIndex = 23;
        private const int failureProbabilityStructureWithErosionPropertyIndex = 24;

        private const int storageStructureAreaPropertyIndex = 25;
        private const int allowedLevelIncreaseStoragePropertyIndex = 26;

        private const int stabilityLinearLoadModelPropertyIndex = 27;
        private const int stabilityQuadraticLoadModelPropertyIndex = 28;

        private const int failureCollisionEnergyPropertyIndex = 29;
        private const int shipMassPropertyIndex = 30;
        private const int shipVelocityPropertyIndex = 31;
        private const int levellingCountPropertyIndex = 32;
        private const int probabilityCollisionSecondaryStructurePropertyIndex = 33;
        private const int flowVelocityStructureClosablePropertyIndex = 34;

        private const int foreshoreProfilePropertyIndex = 35;
        private const int useBreakWaterPropertyIndex = 36;
        private const int useForeshorePropertyIndex = 37;

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

        #region Output Settings

        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_OutputSettings),
            outputSettingsCategoryIndex, totalNrOfCategories)]
        public override bool ShouldIllustrationPointsBeCalculated
        {
            get
            {
                return base.ShouldIllustrationPointsBeCalculated;
            }
            set
            {
                base.ShouldIllustrationPointsBeCalculated = value;
            }
        }

        #endregion

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

        #region General data

        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_General),
            generalCategoryIndex, totalNrOfCategories)]
        public override SelectableHydraulicBoundaryLocation SelectedHydraulicBoundaryLocation
        {
            get
            {
                return base.SelectedHydraulicBoundaryLocation;
            }
            set
            {
                base.SelectedHydraulicBoundaryLocation = value;
            }
        }

        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_General),
            generalCategoryIndex, totalNrOfCategories)]
        public override StabilityPointStructure Structure
        {
            get
            {
                return base.Structure;
            }
            set
            {
                base.Structure = value;
            }
        }

        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_General),
            generalCategoryIndex, totalNrOfCategories)]
        public override Point2D StructureLocation
        {
            get
            {
                return base.StructureLocation;
            }
        }

        #endregion

        #region Schematization

        #region Hydraulic loads

        [PropertyOrder(volumicWeightWaterPropertyIndex)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_HydraulicLoads),
            hydraulicLoadsCategoryIndex, totalNrOfCategories)]
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
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_HydraulicLoads),
            hydraulicLoadsCategoryIndex, totalNrOfCategories)]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.Structure_InsideWaterLevelFailureConstruction_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.Structure_InsideWaterLevelFailureConstruction_Description))]
        public NormalDistributionProperties InsideWaterLevelFailureConstruction
        {
            get
            {
                return new NormalDistributionProperties(
                    HasStructure()
                        ? DistributionReadOnlyProperties.None
                        : DistributionReadOnlyProperties.All,
                    data.WrappedData.InsideWaterLevelFailureConstruction,
                    PropertyChangeHandler);
            }
        }

        [PropertyOrder(bankWidthPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_HydraulicLoads),
            hydraulicLoadsCategoryIndex, totalNrOfCategories)]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.Structure_BankWidth_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.Structure_BankWidth_Description))]
        public NormalDistributionProperties BankWidth
        {
            get
            {
                return new NormalDistributionProperties(
                    HasStructure()
                        ? DistributionReadOnlyProperties.None
                        : DistributionReadOnlyProperties.All,
                    data.WrappedData.BankWidth,
                    PropertyChangeHandler);
            }
        }

        [DynamicReadOnly]
        [PropertyOrder(verticalDistancePropertyIndex)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_HydraulicLoads),
            hydraulicLoadsCategoryIndex, totalNrOfCategories)]
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

        #region Constructive failure

        [DynamicReadOnly]
        [PropertyOrder(loadSchematizationTypePropertyIndex)]
        [TypeConverter(typeof(EnumTypeConverter))]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_ConstructiveFailure),
            constructiveFailureCategoryIndex, totalNrOfCategories)]
        [ResourcesDisplayName(typeof(Resources), nameof(RiskeerCommonFormsResources.LoadSchematizationType_DisplayName))]
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

        [DynamicVisible]
        [PropertyOrder(constructiveStrengthLinearLoadModelPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_ConstructiveFailure),
            constructiveFailureCategoryIndex, totalNrOfCategories)]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.Structure_ConstructiveStrengthLinearLoadModel_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.Structure_ConstructiveStrengthLinearLoadModel_Description))]
        public VariationCoefficientLogNormalDistributionProperties ConstructiveStrengthLinearLoadModel
        {
            get
            {
                return new VariationCoefficientLogNormalDistributionProperties(
                    HasStructure()
                        ? VariationCoefficientDistributionReadOnlyProperties.None
                        : VariationCoefficientDistributionReadOnlyProperties.All,
                    data.WrappedData.ConstructiveStrengthLinearLoadModel,
                    PropertyChangeHandler);
            }
        }

        [DynamicVisible]
        [PropertyOrder(constructiveStrengthQuadraticLoadModelPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_ConstructiveFailure),
            constructiveFailureCategoryIndex, totalNrOfCategories)]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.Structure_ConstructiveStrengthQuadraticLoadModel_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.Structure_ConstructiveStrengthQuadraticLoadModel_Description))]
        public VariationCoefficientLogNormalDistributionProperties ConstructiveStrengthQuadraticLoadModel
        {
            get
            {
                return new VariationCoefficientLogNormalDistributionProperties(
                    HasStructure()
                        ? VariationCoefficientDistributionReadOnlyProperties.None
                        : VariationCoefficientDistributionReadOnlyProperties.All,
                    data.WrappedData.ConstructiveStrengthQuadraticLoadModel,
                    PropertyChangeHandler);
            }
        }

        [DynamicReadOnly]
        [PropertyOrder(evaluationLevelPropertyIndex)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_ConstructiveFailure),
            constructiveFailureCategoryIndex, totalNrOfCategories)]
        [ResourcesDisplayName(typeof(Resources), nameof(RiskeerCommonFormsResources.Evaluation_Level_DisplayName))]
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
        [PropertyOrder(failureProbabilityRepairClosurePropertyIndex)]
        [TypeConverter(typeof(NoProbabilityValueDoubleConverter))]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_ConstructiveFailure),
            constructiveFailureCategoryIndex, totalNrOfCategories)]
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

        #endregion

        #region Incoming flow

        [DynamicReadOnly]
        [PropertyOrder(inflowModelTypePropertyIndex)]
        [TypeConverter(typeof(EnumTypeConverter))]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_Schematization_Incoming_flow),
            schematizationIncomingFlowCategoryIndex, totalNrOfCategories)]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Structure_InflowModelType_DisplayName))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Structure_InflowModelType_Description))]
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

        [DynamicVisible]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_Schematization_Incoming_flow),
            schematizationIncomingFlowCategoryIndex, totalNrOfCategories)]
        public override NormalDistributionProperties WidthFlowApertures
        {
            get
            {
                return base.WidthFlowApertures;
            }
        }

        [PropertyOrder(levelCrestStructurePropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_Schematization_Incoming_flow),
            schematizationIncomingFlowCategoryIndex, totalNrOfCategories)]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Structure_LevelCrestStructure_DisplayName))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Structure_LevelCrestStructure_Description))]
        public NormalDistributionProperties LevelCrestStructure
        {
            get
            {
                return new NormalDistributionProperties(
                    HasStructure()
                        ? DistributionReadOnlyProperties.None
                        : DistributionReadOnlyProperties.All,
                    data.WrappedData.LevelCrestStructure,
                    PropertyChangeHandler);
            }
        }

        [PropertyOrder(insideWaterLevelPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_Schematization_Incoming_flow),
            schematizationIncomingFlowCategoryIndex, totalNrOfCategories)]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Structure_InsideWaterLevel_DisplayName))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Structure_InsideWaterLevel_Description))]
        public NormalDistributionProperties InsideWaterLevel
        {
            get
            {
                return new NormalDistributionProperties(
                    HasStructure()
                        ? DistributionReadOnlyProperties.None
                        : DistributionReadOnlyProperties.All,
                    data.WrappedData.InsideWaterLevel,
                    PropertyChangeHandler);
            }
        }

        [PropertyOrder(thresholdHeightOpenWeirPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_Schematization_Incoming_flow),
            schematizationIncomingFlowCategoryIndex, totalNrOfCategories)]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Structure_ThresholdHeightOpenWeir_DisplayName))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Structure_ThresholdHeightOpenWeir_Description))]
        public NormalDistributionProperties ThresholdHeightOpenWeir
        {
            get
            {
                return new NormalDistributionProperties(
                    HasStructure()
                        ? DistributionReadOnlyProperties.None
                        : DistributionReadOnlyProperties.All,
                    data.WrappedData.ThresholdHeightOpenWeir,
                    PropertyChangeHandler);
            }
        }

        [DynamicVisible]
        [PropertyOrder(areaFlowAperturesPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_Schematization_Incoming_flow),
            schematizationIncomingFlowCategoryIndex, totalNrOfCategories)]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Structure_AreaFlowApertures_DisplayName))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Structure_AreaFlowApertures_Description))]
        public LogNormalDistributionProperties AreaFlowApertures
        {
            get
            {
                return new LogNormalDistributionProperties(
                    HasStructure()
                        ? DistributionReadOnlyProperties.None
                        : DistributionReadOnlyProperties.All,
                    data.WrappedData.AreaFlowApertures,
                    PropertyChangeHandler);
            }
        }

        [DynamicVisible]
        [PropertyOrder(drainCoefficientPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_Schematization_Incoming_flow),
            schematizationIncomingFlowCategoryIndex, totalNrOfCategories)]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Structure_DrainCoefficient_DisplayName))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Structure_DrainCoefficient_Description))]
        public NormalDistributionProperties DrainCoefficient
        {
            get
            {
                return new NormalDistributionProperties(
                    DistributionReadOnlyProperties.StandardDeviation,
                    data.WrappedData.DrainCoefficient,
                    PropertyChangeHandler);
            }
        }

        [PropertyOrder(factorStormDurationOpenStructurePropertyIndex)]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_Schematization_Incoming_flow),
            schematizationIncomingFlowCategoryIndex, totalNrOfCategories)]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Structure_FactorStormDurationOpenStructure_DisplayName))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Structure_FactorStormDurationOpenStructure_Description))]
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

        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_Schematization_Incoming_flow),
            schematizationIncomingFlowCategoryIndex, totalNrOfCategories)]
        public override RoundedDouble StructureNormalOrientation
        {
            get
            {
                return base.StructureNormalOrientation;
            }
            set
            {
                base.StructureNormalOrientation = value;
            }
        }

        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_Schematization_Incoming_flow),
            schematizationIncomingFlowCategoryIndex, totalNrOfCategories)]
        public override VariationCoefficientLogNormalDistributionProperties StormDuration
        {
            get
            {
                return base.StormDuration;
            }
        }

        #endregion

        #region Constructive Instability

        [DynamicVisible]
        [PropertyOrder(stabilityLinearLoadModelPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_ConstructionInstability),
            constructiveInstabilityCategoryIndex, totalNrOfCategories)]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.Structure_StabilityLinearLoadModel_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.Structure_StabilityLinearLoadModel_Description))]
        public VariationCoefficientLogNormalDistributionProperties StabilityLinearLoadModel
        {
            get
            {
                return new VariationCoefficientLogNormalDistributionProperties(
                    HasStructure()
                        ? VariationCoefficientDistributionReadOnlyProperties.None
                        : VariationCoefficientDistributionReadOnlyProperties.All,
                    data.WrappedData.StabilityLinearLoadModel,
                    PropertyChangeHandler);
            }
        }

        [DynamicVisible]
        [PropertyOrder(stabilityQuadraticLoadModelPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_ConstructionInstability),
            constructiveInstabilityCategoryIndex, totalNrOfCategories)]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.Structure_StabilityQuadraticLoadModel_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.Structure_StabilityQuadraticLoadModel_Description))]
        public VariationCoefficientLogNormalDistributionProperties StabilityQuadraticLoadModel
        {
            get
            {
                return new VariationCoefficientLogNormalDistributionProperties(
                    HasStructure()
                        ? VariationCoefficientDistributionReadOnlyProperties.None
                        : VariationCoefficientDistributionReadOnlyProperties.All,
                    data.WrappedData.StabilityQuadraticLoadModel,
                    PropertyChangeHandler);
            }
        }

        #endregion

        #region Collision secondary structure

        [PropertyOrder(failureCollisionEnergyPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categoties_CollisionSecondaryStructure),
            collisionSecondaryStructureCategoryIndex, totalNrOfCategories)]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.Structure_FailureCollisionEnergy_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.Structure_FailureCollisionEnergy_Description))]
        public VariationCoefficientLogNormalDistributionProperties FailureCollisionEnergy
        {
            get
            {
                return new VariationCoefficientLogNormalDistributionProperties(
                    HasStructure()
                        ? VariationCoefficientDistributionReadOnlyProperties.None
                        : VariationCoefficientDistributionReadOnlyProperties.All,
                    data.WrappedData.FailureCollisionEnergy,
                    PropertyChangeHandler);
            }
        }

        [PropertyOrder(shipMassPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categoties_CollisionSecondaryStructure),
            collisionSecondaryStructureCategoryIndex, totalNrOfCategories)]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.Structure_ShipMass_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.Structure_ShipMass_Description))]
        public VariationCoefficientNormalDistributionProperties ShipMass
        {
            get
            {
                return new VariationCoefficientNormalDistributionProperties(
                    HasStructure()
                        ? VariationCoefficientDistributionReadOnlyProperties.None
                        : VariationCoefficientDistributionReadOnlyProperties.All,
                    data.WrappedData.ShipMass,
                    PropertyChangeHandler);
            }
        }

        [PropertyOrder(shipVelocityPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categoties_CollisionSecondaryStructure),
            collisionSecondaryStructureCategoryIndex, totalNrOfCategories)]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.Structure_ShipVelocity_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.Structure_ShipVelocity_Description))]
        public VariationCoefficientNormalDistributionProperties ShipVelocity
        {
            get
            {
                return new VariationCoefficientNormalDistributionProperties(
                    HasStructure()
                        ? VariationCoefficientDistributionReadOnlyProperties.None
                        : VariationCoefficientDistributionReadOnlyProperties.All,
                    data.WrappedData.ShipVelocity,
                    PropertyChangeHandler);
            }
        }

        [DynamicReadOnly]
        [PropertyOrder(levellingCountPropertyIndex)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categoties_CollisionSecondaryStructure),
            collisionSecondaryStructureCategoryIndex, totalNrOfCategories)]
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
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categoties_CollisionSecondaryStructure),
            collisionSecondaryStructureCategoryIndex, totalNrOfCategories)]
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

        [PropertyOrder(flowVelocityStructureClosablePropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categoties_CollisionSecondaryStructure),
            collisionSecondaryStructureCategoryIndex, totalNrOfCategories)]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.Structure_FlowVelocityStructureClosable_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.Structure_FlowVelocityStructureClosable_Description))]
        public VariationCoefficientNormalDistributionProperties FlowVelocityStructureClosable
        {
            get
            {
                return new VariationCoefficientNormalDistributionProperties(
                    HasStructure()
                        ? VariationCoefficientDistributionReadOnlyProperties.CoefficientOfVariation
                        : VariationCoefficientDistributionReadOnlyProperties.All,
                    data.WrappedData.FlowVelocityStructureClosable,
                    PropertyChangeHandler);
            }
        }

        #endregion

        #region Ground erosion

        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_Schematization_Ground_erosion),
            schematizationGroundErosionCategoryIndex, totalNrOfCategories)]
        public override VariationCoefficientLogNormalDistributionProperties CriticalOvertoppingDischarge
        {
            get
            {
                return base.CriticalOvertoppingDischarge;
            }
        }

        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_Schematization_Ground_erosion),
            schematizationGroundErosionCategoryIndex, totalNrOfCategories)]
        public override LogNormalDistributionProperties FlowWidthAtBottomProtection
        {
            get
            {
                return base.FlowWidthAtBottomProtection;
            }
        }

        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_Schematization_Ground_erosion),
            schematizationGroundErosionCategoryIndex, totalNrOfCategories)]
        public override double FailureProbabilityStructureWithErosion
        {
            get
            {
                return base.FailureProbabilityStructureWithErosion;
            }
            set
            {
                base.FailureProbabilityStructureWithErosion = value;
            }
        }

        #endregion

        #region Storage structure

        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_Schematization_Storage_structure),
            schematizationStorageStructureCategoryIndex, totalNrOfCategories)]
        public override VariationCoefficientLogNormalDistributionProperties StorageStructureArea
        {
            get
            {
                return base.StorageStructureArea;
            }
        }

        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_Schematization_Storage_structure),
            schematizationStorageStructureCategoryIndex, totalNrOfCategories)]
        public override LogNormalDistributionProperties AllowedLevelIncreaseStorage
        {
            get
            {
                return base.AllowedLevelIncreaseStorage;
            }
        }

        #endregion

        #region Foreshore profile

        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_Foreshore),
            foreshoreCategoryIndex, totalNrOfCategories)]
        public override ForeshoreProfile ForeshoreProfile
        {
            get
            {
                return base.ForeshoreProfile;
            }
            set
            {
                base.ForeshoreProfile = value;
            }
        }

        [DynamicPropertyOrder]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_Foreshore),
            foreshoreCategoryIndex, totalNrOfCategories)]
        public override UseBreakWaterProperties UseBreakWater
        {
            get
            {
                return base.UseBreakWater;
            }
        }

        [DynamicPropertyOrder]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_Foreshore),
            foreshoreCategoryIndex, totalNrOfCategories)]
        public override UseForeshoreProperties UseForeshore
        {
            get
            {
                return base.UseForeshore;
            }
        }

        #endregion

        #endregion
    }
}