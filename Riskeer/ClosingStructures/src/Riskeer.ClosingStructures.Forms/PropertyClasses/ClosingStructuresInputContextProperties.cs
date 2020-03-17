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
using Riskeer.ClosingStructures.Data;
using Riskeer.ClosingStructures.Forms.PresentationObjects;
using Riskeer.ClosingStructures.Forms.Properties;
using Riskeer.ClosingStructures.Util;
using Riskeer.Common.Data.DikeProfiles;
using Riskeer.Common.Data.Structures;
using Riskeer.Common.Forms.ChangeHandlers;
using Riskeer.Common.Forms.PresentationObjects;
using Riskeer.Common.Forms.PropertyClasses;
using Riskeer.Common.Forms.TypeConverters;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.ClosingStructures.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="ClosingStructuresInputContext"/> for properties panel.
    /// </summary>
    public class ClosingStructuresInputContextProperties : StructuresInputBaseProperties<
        ClosingStructure,
        ClosingStructuresInput,
        StructuresCalculation<ClosingStructuresInput>,
        ClosingStructuresFailureMechanism>
    {
        private const int generalCategoryIndex = 1;
        private const int modelSettingsCategoryIndex = 2;
        private const int schematizationClosureCategoryIndex = 3;
        private const int schematizationIncomingFlowCategoryIndex = 4;
        private const int schematizationGroundErosionCategoryIndex = 5;
        private const int schematizationStorageStructureCategoryIndex = 6;
        private const int foreshoreCategoryIndex = 7;
        private const int outputSettingsCategoryIndex = 8;
        private const int totalNrOfCategories = 8;

        private const int structurePropertyIndex = 0;
        private const int structureLocationPropertyIndex = 1;
        private const int hydraulicBoundaryLocationPropertyIndex = 2;

        private const int modelFactorSuperCriticalFlowPropertyIndex = 3;

        private const int identicalAperturesPropertyIndex = 4;
        private const int failureProbabilityOpenStructurePropertyIndex = 5;
        private const int probabilityOpenStructureBeforeFloodingPropertyIndex = 6;
        private const int failureProbabilityReparationPropertyIndex = 7;

        private const int inflowModelTypePropertyIndex = 8;
        private const int structureNormalOrientationPropertyIndex = 9;
        private const int levelCrestStructureNotClosingPropertyIndex = 10;
        private const int insideWaterLevelPropertyIndex = 11;
        private const int thresholdHeightOpenWeirPropertyIndex = 12;
        private const int areaFlowAperturesPropertyIndex = 13;
        private const int drainCoefficientPropertyIndex = 14;
        private const int widthFlowAperturesPropertyIndex = 15;
        private const int stormDurationPropertyIndex = 16;
        private const int factorStormDurationOpenStructurePropertyIndex = 17;

        private const int criticalOvertoppingDischargePropertyIndex = 18;
        private const int flowWidthAtBottomProtectionPropertyIndex = 19;
        private const int failureProbabilityStructureWithErosionPropertyIndex = 20;

        private const int storageStructureAreaPropertyIndex = 21;
        private const int allowedLevelIncreaseStoragePropertyIndex = 22;

        private const int foreshoreProfilePropertyIndex = 23;
        private const int useBreakWaterPropertyIndex = 24;
        private const int useForeshorePropertyIndex = 25;

        /// <summary>
        /// Creates a new instance of the <see cref="ClosingStructuresInputContextProperties"/> class.
        /// </summary>
        /// <param name="data">The instance to show the properties of.</param>
        /// <param name="propertyChangeHandler">The handler responsible for handling effects of a property change.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public ClosingStructuresInputContextProperties(ClosingStructuresInputContext data, IObservablePropertyChangeHandler propertyChangeHandler) :
            base(data, new ConstructionProperties
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

        #region Model factors

        [DynamicVisible]
        [PropertyOrder(modelFactorSuperCriticalFlowPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_ModelSettings),
            modelSettingsCategoryIndex, totalNrOfCategories)]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Structure_ModelFactorSuperCriticalFlow_DisplayName))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Structure_ModelFactorSuperCriticalFlow_Description))]
        public NormalDistributionProperties ModelFactorSuperCriticalFlow
        {
            get
            {
                return new NormalDistributionProperties(
                    DistributionReadOnlyProperties.StandardDeviation,
                    data.WrappedData.ModelFactorSuperCriticalFlow,
                    PropertyChangeHandler);
            }
        }

        #endregion

        [DynamicVisibleValidationMethod]
        public bool DynamicVisibleValidationMethod(string propertyName)
        {
            // Note: A default initialized calculation doesn't have InflowModelType initialized to any of these 3 values.
            if (data.WrappedData.InflowModelType == ClosingStructureInflowModelType.VerticalWall ||
                data.WrappedData.InflowModelType == ClosingStructureInflowModelType.FloodedCulvert ||
                data.WrappedData.InflowModelType == ClosingStructureInflowModelType.LowSill)
            {
                if (propertyName == nameof(InsideWaterLevel))
                {
                    return data.WrappedData.InflowModelType != ClosingStructureInflowModelType.VerticalWall;
                }

                if (propertyName == nameof(ModelFactorSuperCriticalFlow))
                {
                    return data.WrappedData.InflowModelType == ClosingStructureInflowModelType.VerticalWall;
                }

                if (propertyName == nameof(DrainCoefficient))
                {
                    return data.WrappedData.InflowModelType == ClosingStructureInflowModelType.FloodedCulvert;
                }

                if (propertyName == nameof(StructureNormalOrientation))
                {
                    return data.WrappedData.InflowModelType == ClosingStructureInflowModelType.VerticalWall;
                }

                if (propertyName == nameof(ThresholdHeightOpenWeir))
                {
                    return data.WrappedData.InflowModelType == ClosingStructureInflowModelType.LowSill;
                }

                if (propertyName == nameof(AreaFlowApertures))
                {
                    return data.WrappedData.InflowModelType == ClosingStructureInflowModelType.FloodedCulvert;
                }

                if (propertyName == nameof(LevelCrestStructureNotClosing))
                {
                    return data.WrappedData.InflowModelType == ClosingStructureInflowModelType.VerticalWall;
                }

                if (propertyName == nameof(WidthFlowApertures))
                {
                    return data.WrappedData.InflowModelType != ClosingStructureInflowModelType.FloodedCulvert;
                }
            }

            return true;
        }

        public override IEnumerable<ForeshoreProfile> GetAvailableForeshoreProfiles()
        {
            return data.FailureMechanism.ForeshoreProfiles;
        }

        public override IEnumerable<ClosingStructure> GetAvailableStructures()
        {
            return data.FailureMechanism.ClosingStructures;
        }

        protected override void AfterSettingStructure()
        {
            ClosingStructuresHelper.UpdateCalculationToSectionResultAssignments(
                data.FailureMechanism);
        }

        protected override bool ShouldPropertyBeReadOnlyInAbsenseOfStructure(string property)
        {
            return nameof(InflowModelType).Equals(property)
                   || nameof(IdenticalApertures).Equals(property)
                   || nameof(ProbabilityOpenStructureBeforeFlooding).Equals(property)
                   || nameof(FailureProbabilityOpenStructure).Equals(property)
                   || nameof(FailureProbabilityReparation).Equals(property)
                   || base.ShouldPropertyBeReadOnlyInAbsenseOfStructure(property);
        }

        #region General data

        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_General),
            generalCategoryIndex, totalNrOfCategories)]
        public override ClosingStructure Structure
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

        #endregion

        #region Schematization

        #region Closure

        [DynamicReadOnly]
        [PropertyOrder(identicalAperturesPropertyIndex)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_Schematization_Closure),
            schematizationClosureCategoryIndex, totalNrOfCategories)]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.IdenticalApertures_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.IdenticalApertures_Description))]
        public int IdenticalApertures
        {
            get
            {
                return data.WrappedData.IdenticalApertures;
            }
            set
            {
                PropertyChangeHelper.ChangePropertyAndNotify(() => data.WrappedData.IdenticalApertures = value, PropertyChangeHandler);
            }
        }

        [DynamicReadOnly]
        [PropertyOrder(failureProbabilityOpenStructurePropertyIndex)]
        [TypeConverter(typeof(NoProbabilityValueDoubleConverter))]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_Schematization_Closure),
            schematizationClosureCategoryIndex, totalNrOfCategories)]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.FailureProbabilityOpenStructure_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.FailureProbabilityOpenStructure_Description))]
        public double FailureProbabilityOpenStructure
        {
            get
            {
                return data.WrappedData.FailureProbabilityOpenStructure;
            }
            set
            {
                PropertyChangeHelper.ChangePropertyAndNotify(() => data.WrappedData.FailureProbabilityOpenStructure = value, PropertyChangeHandler);
            }
        }

        [DynamicReadOnly]
        [PropertyOrder(probabilityOpenStructureBeforeFloodingPropertyIndex)]
        [TypeConverter(typeof(NoProbabilityValueDoubleConverter))]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_Schematization_Closure),
            schematizationClosureCategoryIndex, totalNrOfCategories)]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.ProbabilityOpenStructureBeforeFlooding_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.ProbabilityOpenStructureBeforeFlooding_Description))]
        public double ProbabilityOpenStructureBeforeFlooding
        {
            get
            {
                return data.WrappedData.ProbabilityOpenStructureBeforeFlooding;
            }
            set
            {
                PropertyChangeHelper.ChangePropertyAndNotify(() => data.WrappedData.ProbabilityOpenStructureBeforeFlooding = value, PropertyChangeHandler);
            }
        }

        [DynamicReadOnly]
        [PropertyOrder(failureProbabilityReparationPropertyIndex)]
        [TypeConverter(typeof(NoProbabilityValueDoubleConverter))]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_Schematization_Closure),
            schematizationClosureCategoryIndex, totalNrOfCategories)]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.FailureProbabilityReparation_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.FailureProbabilityReparation_Description))]
        public double FailureProbabilityReparation
        {
            get
            {
                return data.WrappedData.FailureProbabilityReparation;
            }
            set
            {
                PropertyChangeHelper.ChangePropertyAndNotify(() => data.WrappedData.FailureProbabilityReparation = value, PropertyChangeHandler);
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
        public ClosingStructureInflowModelType InflowModelType
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

        [DynamicVisible]
        [PropertyOrder(levelCrestStructureNotClosingPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_Schematization_Incoming_flow),
            schematizationIncomingFlowCategoryIndex, totalNrOfCategories)]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.LevelCrestStructureNotClosing_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.LevelCrestStructureNotClosing_Description))]
        public NormalDistributionProperties LevelCrestStructureNotClosing
        {
            get
            {
                return new NormalDistributionProperties(
                    HasStructure()
                        ? DistributionReadOnlyProperties.None
                        : DistributionReadOnlyProperties.All,
                    data.WrappedData.LevelCrestStructureNotClosing,
                    PropertyChangeHandler);
            }
        }

        [DynamicVisible]
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

        [DynamicVisible]
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
        public override VariationCoefficientLogNormalDistributionProperties StormDuration
        {
            get
            {
                return base.StormDuration;
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
    }
}