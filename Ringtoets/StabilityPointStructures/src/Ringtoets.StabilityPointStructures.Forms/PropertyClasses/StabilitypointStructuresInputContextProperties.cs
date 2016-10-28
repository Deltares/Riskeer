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

using System.Collections.Generic;
using System.ComponentModel;
using Core.Common.Base.Data;
using Core.Common.Gui.Attributes;
using Core.Common.Utils;
using Core.Common.Utils.Attributes;
using Ringtoets.StabilityPointStructures.Data;
using Ringtoets.StabilityPointStructures.Forms.PresentationObjects;
using Ringtoets.StabilityPointStructures.Forms.Properties;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.Forms.PropertyClasses;
using Ringtoets.Common.Utils;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.StabilityPointStructures.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="StabilityPointStructuresInputContext"/> for properties panel.
    /// </summary>
    public class StabilityPointStructuresInputContextProperties : StructuresInputBaseProperties<StabilityPointStructure, StabilityPointStructuresInput, StructuresCalculation<StabilityPointStructuresInput>, StabilityPointStructuresFailureMechanism>
    {
        private const int hydraulicBoundaryLocationPropertyIndex = 1;
        private const int stormDurationPropertyIndex = 2;
        private const int insideWaterLevelPropertyIndex = 4;
        private const int structurePropertyIndex = 5;
        private const int structureLocationPropertyIndex = 6;
        private const int structureNormalOrientationPropertyIndex = 7;
        private const int inflowModelTypePropertyIndex = 8;
        private const int widthFlowAperturesPropertyIndex = 9;
        private const int areaFlowAperturesPropertyIndex = 10;
        private const int flowWidthAtBottomProtectionPropertyIndex = 12;
        private const int storageStructureAreaPropertyIndex = 13;
        private const int allowedLevelIncreaseStoragePropertyIndex = 14;
        private const int thresholdHeightOpenWeirPropertyIndex = 16;
        private const int criticalOvertoppingDischargePropertyIndex = 17;
        private const int failureProbabilityStructureWithErosionPropertyIndex = 21;
        private const int foreshoreProfilePropertyIndex = 22;
        private const int useBreakWaterPropertyIndex = 23;
        private const int useForeshorePropertyIndex = 24;
        private const int modelFactorSuperCriticalFlowPropertyIndex = 25;
        private const int drainCoefficientPropertyIndex = 26;
        private const int factorStormDurationOpenStructurePropertyIndex = 27;

        /// <summary>
        /// Creates a new instance of the <see cref="StabilityPointStructuresInputContextProperties"/> class.
        /// </summary>
        public StabilityPointStructuresInputContextProperties()
            : base(new ConstructionProperties
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
            }) { }

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
            StructuresHelper.Update(data.FailureMechanism.SectionResults, data.Calculation);
        }

        #region Hydraulic data

        [PropertyOrder(insideWaterLevelPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_HydraulicData")]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), "Structure_InsideWaterLevel_DisplayName")]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), "Structure_InsideWaterLevel_Description")]
        public NormalDistributionProperties InsideWaterLevel
        {
            get
            {
                return new NormalDistributionProperties(DistributionPropertiesReadOnly.None, data.WrappedData)
                {
                    Data = data.WrappedData.InsideWaterLevel
                };
            }
        }

        #endregion

        #region Model factors

        [PropertyOrder(drainCoefficientPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_ModelSettings")]
        [ResourcesDisplayName(typeof(Resources), "DrainCoefficient_DisplayName")]
        [ResourcesDescription(typeof(Resources), "DrainCoefficient_Description")]
        public NormalDistributionProperties DrainCoefficient
        {
            get
            {
                return new NormalDistributionProperties(DistributionPropertiesReadOnly.StandardDeviation, data.WrappedData)
                {
                    Data = data.WrappedData.DrainCoefficient
                };
            }
        }

        [PropertyOrder(factorStormDurationOpenStructurePropertyIndex)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_ModelSettings")]
        [ResourcesDisplayName(typeof(Resources), "FactorStormDurationOpenStructure_DisplayName")]
        [ResourcesDescription(typeof(Resources), "FactorStormDurationOpenStructure_Description")]
        public RoundedDouble FactorStormDurationOpenStructure
        {
            get
            {
                return data.WrappedData.FactorStormDurationOpenStructure;
            }
            set
            {
                data.WrappedData.FactorStormDurationOpenStructure = value;
                data.WrappedData.NotifyObservers();
            }
        }

        #endregion

        #region Schematization

        [PropertyOrder(inflowModelTypePropertyIndex)]
        [TypeConverter(typeof(EnumTypeConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_Schematization")]
        [ResourcesDisplayName(typeof(Resources), "StabilityPointStructureInflowModelType_DisplayName")]
        [ResourcesDescription(typeof(Resources), "StabilityPointStructureInflowModelType_Description")]
        public StabilityPointStructureInflowModelType InflowModelType
        {
            get
            {
                return data.WrappedData.InflowModelType;
            }
            set
            {
                data.WrappedData.InflowModelType = value;
                data.WrappedData.NotifyObservers();
            }
        }

        [PropertyOrder(thresholdHeightOpenWeirPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_Schematization")]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), "Structure_ThresholdHeightOpenWeir_DisplayName")]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), "Structure_ThresholdHeightOpenWeir_Description")]
        public NormalDistributionProperties ThresholdHeightOpenWeir
        {
            get
            {
                return new NormalDistributionProperties(DistributionPropertiesReadOnly.None, data.WrappedData)
                {
                    Data = data.WrappedData.ThresholdHeightOpenWeir
                };
            }
        }

        [PropertyOrder(areaFlowAperturesPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_Schematization")]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), "Structure_AreaFlowApertures_DisplayName")]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), "Structure_AreaFlowApertures_Description")]
        public LogNormalDistributionProperties AreaFlowApertures
        {
            get
            {
                return new LogNormalDistributionProperties(DistributionPropertiesReadOnly.None, data.WrappedData)
                {
                    Data = data.WrappedData.AreaFlowApertures
                };
            }
        }

        #endregion
    }
}