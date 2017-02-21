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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Core.Common.Base.Data;
using Core.Common.Gui.Attributes;
using Core.Common.Utils;
using Core.Common.Utils.Attributes;
using Core.Common.Utils.Reflection;
using Ringtoets.ClosingStructures.Data;
using Ringtoets.ClosingStructures.Forms.PresentationObjects;
using Ringtoets.ClosingStructures.Forms.Properties;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.Forms.ChangeHandlers;
using Ringtoets.Common.Forms.Helpers;
using Ringtoets.Common.Forms.PropertyClasses;
using Ringtoets.Common.Utils;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.ClosingStructures.Forms.PropertyClasses
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
        private const int hydraulicBoundaryLocationPropertyIndex = 1;
        private const int stormDurationPropertyIndex = 2;
        private const int insideWaterLevelPropertyIndex = 3;
        private const int structurePropertyIndex = 4;
        private const int structureLocationPropertyIndex = 5;
        private const int structureNormalOrientationPropertyIndex = 6;
        private const int inflowModelTypePropertyIndex = 7;
        private const int widthFlowAperturesPropertyIndex = 8;
        private const int areaFlowAperturesPropertyIndex = 9;
        private const int identicalAperturesPropertyIndex = 10;
        private const int flowWidthAtBottomProtectionPropertyIndex = 11;
        private const int storageStructureAreaPropertyIndex = 12;
        private const int allowedLevelIncreaseStoragePropertyIndex = 13;
        private const int levelCrestStructureNotClosingPropertyIndex = 14;
        private const int thresholdHeightOpenWeirPropertyIndex = 15;
        private const int criticalOvertoppingDischargePropertyIndex = 16;
        private const int probabilityOrFrequencyOpenStructureBeforeFloodingPropertyIndex = 17;
        private const int failureProbabilityOpenStructurePropertyIndex = 18;
        private const int failureProbabilityReparationPropertyIndex = 19;
        private const int failureProbabilityStructureWithErosionPropertyIndex = 20;
        private const int foreshoreProfilePropertyIndex = 21;
        private const int useBreakWaterPropertyIndex = 22;
        private const int useForeshorePropertyIndex = 23;
        private const int modelFactorSuperCriticalFlowPropertyIndex = 24;
        private const int drainCoefficientPropertyIndex = 25;
        private const int factorStormDurationOpenStructurePropertyIndex = 26;

        /// <summary>
        /// Creates a new instance of the <see cref="ClosingStructuresInputContextProperties"/> class.
        /// </summary>
        /// <param name="data">The instance to show the properties of.</param>
        /// <param name="propertyChangeHandler">The handler responsible for handling effects of a property change.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public ClosingStructuresInputContextProperties(ClosingStructuresInputContext data, ICalculationInputPropertyChangeHandler propertyChangeHandler) :
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
                ModelFactorSuperCriticalFlowPropertyIndex = modelFactorSuperCriticalFlowPropertyIndex,
                HydraulicBoundaryLocationPropertyIndex = hydraulicBoundaryLocationPropertyIndex,
                StormDurationPropertyIndex = stormDurationPropertyIndex
            }, propertyChangeHandler) {}

        #region Hydraulic data

        [DynamicVisible]
        [PropertyOrder(insideWaterLevelPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_HydraulicData))]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Structure_InsideWaterLevel_DisplayName))]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Structure_InsideWaterLevel_Description))]
        public NormalDistributionProperties InsideWaterLevel
        {
            get
            {
                return new NormalDistributionProperties(DistributionPropertiesReadOnly.None, data.WrappedData, this)
                {
                    Data = data.WrappedData.InsideWaterLevel
                };
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
                if (propertyName == TypeUtils.GetMemberName<ClosingStructuresInputContextProperties>(p => p.InsideWaterLevel))
                {
                    return data.WrappedData.InflowModelType != ClosingStructureInflowModelType.VerticalWall;
                }
                if (propertyName == TypeUtils.GetMemberName<ClosingStructuresInputContextProperties>(p => p.ModelFactorSuperCriticalFlow))
                {
                    return data.WrappedData.InflowModelType != ClosingStructureInflowModelType.FloodedCulvert;
                }
                if (propertyName == TypeUtils.GetMemberName<ClosingStructuresInputContextProperties>(p => p.DrainCoefficient))
                {
                    return data.WrappedData.InflowModelType == ClosingStructureInflowModelType.FloodedCulvert;
                }
                if (propertyName == TypeUtils.GetMemberName<ClosingStructuresInputContextProperties>(p => p.StructureNormalOrientation))
                {
                    return data.WrappedData.InflowModelType == ClosingStructureInflowModelType.VerticalWall;
                }
                if (propertyName == TypeUtils.GetMemberName<ClosingStructuresInputContextProperties>(p => p.ThresholdHeightOpenWeir))
                {
                    return data.WrappedData.InflowModelType == ClosingStructureInflowModelType.LowSill;
                }
                if (propertyName == TypeUtils.GetMemberName<ClosingStructuresInputContextProperties>(p => p.AreaFlowApertures))
                {
                    return data.WrappedData.InflowModelType == ClosingStructureInflowModelType.FloodedCulvert;
                }
                if (propertyName == TypeUtils.GetMemberName<ClosingStructuresInputContextProperties>(p => p.LevelCrestStructureNotClosing))
                {
                    return data.WrappedData.InflowModelType == ClosingStructureInflowModelType.VerticalWall;
                }
                if (propertyName == TypeUtils.GetMemberName<ClosingStructuresInputContextProperties>(p => p.WidthFlowApertures))
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
            StructuresHelper.UpdateCalculationToSectionResultAssignments(
                data.FailureMechanism.SectionResults,
                data.FailureMechanism.Calculations.Cast<StructuresCalculation<ClosingStructuresInput>>());
        }

        #region Model factors

        [DynamicVisible]
        public override ConfirmingNormalDistributionProperties<ClosingStructuresInput> ModelFactorSuperCriticalFlow
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
        public NormalDistributionProperties DrainCoefficient
        {
            get
            {
                return new NormalDistributionProperties(DistributionPropertiesReadOnly.StandardDeviation, data.WrappedData, this)
                {
                    Data = data.WrappedData.DrainCoefficient
                };
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
        public override NormalDistributionProperties WidthFlowApertures
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
        public ClosingStructureInflowModelType InflowModelType
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

        [DynamicVisible]
        [PropertyOrder(thresholdHeightOpenWeirPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_Schematization))]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Structure_ThresholdHeightOpenWeir_DisplayName))]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Structure_ThresholdHeightOpenWeir_Description))]
        public NormalDistributionProperties ThresholdHeightOpenWeir
        {
            get
            {
                return new NormalDistributionProperties(DistributionPropertiesReadOnly.None, data.WrappedData, this)
                {
                    Data = data.WrappedData.ThresholdHeightOpenWeir
                };
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
                return new LogNormalDistributionProperties(DistributionPropertiesReadOnly.None, data.WrappedData, this)
                {
                    Data = data.WrappedData.AreaFlowApertures
                };
            }
        }

        [PropertyOrder(failureProbabilityOpenStructurePropertyIndex)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_Schematization))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.FailureProbabilityOpenStructure_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.FailureProbabilityOpenStructure_Description))]
        public string FailureProbabilityOpenStructure
        {
            get
            {
                return ProbabilityFormattingHelper.Format(data.WrappedData.FailureProbabilityOpenStructure);
            }
            set
            {
                ChangePropertyAndNotify(
                    (input, newValue) =>
                        SetProbabilityValue(
                            newValue,
                            data.WrappedData,
                            (wrappedData, parsedValue) => wrappedData.FailureProbabilityOpenStructure = parsedValue),
                    value);
            }
        }

        [PropertyOrder(failureProbabilityReparationPropertyIndex)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_Schematization))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.FailureProbabilityReparation_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.FailureProbabilityReparation_Description))]
        public string FailureProbabilityReparation
        {
            get
            {
                return ProbabilityFormattingHelper.Format(data.WrappedData.FailureProbabilityReparation);
            }
            set
            {
                ChangePropertyAndNotify(
                    (input, newValue) =>
                        SetProbabilityValue(
                            newValue, 
                            data.WrappedData, 
                            (wrappedData, parsedValue) => wrappedData.FailureProbabilityReparation = parsedValue),
                    value);
            }
        }

        [PropertyOrder(identicalAperturesPropertyIndex)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_Schematization))]
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
                ChangePropertyAndNotify(
                    (input, newValue) => data.WrappedData.IdenticalApertures = newValue, value);
            }
        }

        [DynamicVisible]
        [PropertyOrder(levelCrestStructureNotClosingPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_Schematization))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.LevelCrestStructureNotClosing_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.LevelCrestStructureNotClosing_Description))]
        public NormalDistributionProperties LevelCrestStructureNotClosing
        {
            get
            {
                return new NormalDistributionProperties(DistributionPropertiesReadOnly.None, data.WrappedData, this)
                {
                    Data = data.WrappedData.LevelCrestStructureNotClosing
                };
            }
        }

        [PropertyOrder(probabilityOrFrequencyOpenStructureBeforeFloodingPropertyIndex)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_Schematization))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.ProbabilityOrFrequencyOpenStructureBeforeFlooding_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.ProbabilityOrFrequencyOpenStructureBeforeFlooding_Description))]
        public string ProbabilityOrFrequencyOpenStructureBeforeFlooding
        {
            get
            {
                return ProbabilityFormattingHelper.Format(data.WrappedData.ProbabilityOrFrequencyOpenStructureBeforeFlooding);
            }
            set
            {
                ChangePropertyAndNotify(
                    (input, newValue) =>
                        SetProbabilityValue(
                            newValue, 
                            data.WrappedData, 
                            (wrappedData, parsedValue) => wrappedData.ProbabilityOrFrequencyOpenStructureBeforeFlooding = parsedValue),
                    value);
            }
        }

        #endregion
    }
}