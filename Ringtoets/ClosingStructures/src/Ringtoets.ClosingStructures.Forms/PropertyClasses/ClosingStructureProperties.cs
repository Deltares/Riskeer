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

using System.ComponentModel;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.Gui.Attributes;
using Core.Common.Gui.PropertyBag;
using Core.Common.Utils;
using Core.Common.Utils.Attributes;
using Ringtoets.ClosingStructures.Data;
using Ringtoets.ClosingStructures.Forms.Properties;
using Ringtoets.Common.Forms.Helpers;
using Ringtoets.Common.Forms.PropertyClasses;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.ClosingStructures.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="ClosingStructure"/> for properties panel.
    /// </summary>
    public class ClosingStructureProperties : ObjectProperties<ClosingStructure>
    {
        private const int namePropertyIndex = 1;
        private const int locationPropertyIndex = 2;
        private const int structureNormalOrientationPropertyIndex = 3;
        private const int inflowModelTypePropertyIndex = 4;
        private const int widthFlowAperturesPropertyIndex = 5;
        private const int areaFlowAperturesPropertyIndex = 6;
        private const int identicalAperturesPropertyIndex = 7;
        private const int flowWidthAtBottomProtectionPropertyIndex = 8;
        private const int storageStructureAreaPropertyIndex = 9;
        private const int allowedLevelIncreaseStoragePropertyIndex = 10;
        private const int levelCrestStructureNotClosingPropertyIndex = 11;
        private const int thresholdHeightOpenWeirPropertyIndex = 12;
        private const int insideWaterLevelPropertyIndex = 13;
        private const int criticalOvertoppingDischargePropertyIndex = 14;
        private const int probabilityOpenStructureBeforeFloodingPropertyIndex = 15;
        private const int failureProbabilityOpenStructurePropertyIndex = 16;
        private const int failureProbabilityReparationPropertyIndex = 17;

        [PropertyOrder(namePropertyIndex)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_General")]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), "Structure_Name_DisplayName")]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), "Structure_Name_Description")]
        public string Name
        {
            get
            {
                return data.Name;
            }
        }

        [PropertyOrder(locationPropertyIndex)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_General")]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), "Structure_Location_DisplayName")]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), "Structure_Location_Description")]
        public Point2D Location
        {
            get
            {
                return new Point2D(new RoundedDouble(0, data.Location.X),
                                   new RoundedDouble(0, data.Location.Y));
            }
        }

        [PropertyOrder(structureNormalOrientationPropertyIndex)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_Schematization")]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), "Structure_StructureNormalOrientation_DisplayName")]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), "Structure_StructureNormalOrientation_Description")]
        public RoundedDouble StructureNormalOrientation
        {
            get
            {
                return data.StructureNormalOrientation;
            }
        }

        [PropertyOrder(inflowModelTypePropertyIndex)]
        [TypeConverter(typeof(EnumTypeConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_Schematization")]
        [ResourcesDisplayName(typeof(Resources), "ClosingStructureInflowModelType_DisplayName")]
        [ResourcesDescription(typeof(Resources), "ClosingStructureInflowModelType_Description")]
        public ClosingStructureInflowModelType InflowModelType
        {
            get
            {
                return data.InflowModelType;
            }
        }

        [PropertyOrder(widthFlowAperturesPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_Schematization")]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), "Structure_WidthFlowApertures_DisplayName")]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), "Structure_WidthFlowApertures_Description")]
        public NormalDistributionVariationProperties WidthFlowApertures
        {
            get
            {
                return new NormalDistributionVariationProperties
                {
                    Data = data.WidthFlowApertures
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
                return new LogNormalDistributionProperties
                {
                    Data = data.AreaFlowApertures
                };
            }
        }

        [PropertyOrder(identicalAperturesPropertyIndex)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_Schematization")]
        [ResourcesDisplayName(typeof(Resources), "IdenticalApertures_DisplayName")]
        [ResourcesDescription(typeof(Resources), "IdenticalApertures_Description")]
        public int IdenticalApertures
        {
            get
            {
                return data.IdenticalApertures;
            }
        }

        [PropertyOrder(flowWidthAtBottomProtectionPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_Schematization")]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), "Structure_FlowWidthAtBottomProtection_DisplayName")]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), "Structure_FlowWidthAtBottomProtection_Description")]
        public LogNormalDistributionProperties FlowWidthAtBottomProtection
        {
            get
            {
                return new LogNormalDistributionProperties
                {
                    Data = data.FlowWidthAtBottomProtection
                };
            }
        }

        [PropertyOrder(storageStructureAreaPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_Schematization")]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), "Structure_StorageStructureArea_DisplayName")]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), "Structure_StorageStructureArea_Description")]
        public LogNormalDistributionVariationProperties StorageStructureArea
        {
            get
            {
                return new LogNormalDistributionVariationProperties
                {
                    Data = data.StorageStructureArea
                };
            }
        }

        [PropertyOrder(allowedLevelIncreaseStoragePropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_Schematization")]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), "Structure_AllowedLevelIncreaseStorage_DisplayName")]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), "Structure_AllowedLevelIncreaseStorage_Description")]
        public LogNormalDistributionProperties AllowedLevelIncreaseStorage
        {
            get
            {
                return new LogNormalDistributionProperties
                {
                    Data = data.AllowedLevelIncreaseStorage
                };
            }
        }

        [PropertyOrder(levelCrestStructureNotClosingPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_Schematization")]
        [ResourcesDisplayName(typeof(Resources), "LevelCrestStructureNotClosing_DisplayName")]
        [ResourcesDescription(typeof(Resources), "LevelCrestStructureNotClosing_Description")]
        public NormalDistributionProperties LevelCrestStructureNotClosing
        {
            get
            {
                return new NormalDistributionProperties
                {
                    Data = data.LevelCrestStructureNotClosing
                };
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
                return new NormalDistributionProperties
                {
                    Data = data.ThresholdHeightOpenWeir
                };
            }
        }

        [PropertyOrder(insideWaterLevelPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_HydraulicData")]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), "Structure_InsideWaterLevel_DisplayName")]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), "Structure_InsideWaterLevel_Description")]
        public NormalDistributionProperties InsideWaterLevel
        {
            get
            {
                return new NormalDistributionProperties
                {
                    Data = data.InsideWaterLevel
                };
            }
        }

        [PropertyOrder(criticalOvertoppingDischargePropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_Schematization")]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), "Structure_CriticalOvertoppingDischarge_DisplayName")]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), "Structure_CriticalOvertoppingDischarge_Description")]
        public LogNormalDistributionVariationProperties CriticalOvertoppingDischarge
        {
            get
            {
                return new LogNormalDistributionVariationProperties
                {
                    Data = data.CriticalOvertoppingDischarge
                };
            }
        }

        [PropertyOrder(probabilityOpenStructureBeforeFloodingPropertyIndex)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_Schematization")]
        [ResourcesDisplayName(typeof(Resources), "ProbabilityOpenStructureBeforeFlooding_DisplayName")]
        [ResourcesDescription(typeof(Resources), "ProbabilityOpenStructureBeforeFlooding_Description")]
        public string ProbabilityOpenStructureBeforeFlooding
        {
            get
            {
                return ProbabilityFormattingHelper.Format(data.ProbabilityOpenStructureBeforeFlooding);
            }
        }

        [PropertyOrder(failureProbabilityOpenStructurePropertyIndex)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_Schematization")]
        [ResourcesDisplayName(typeof(Resources), "FailureProbabilityOpenStructure_DisplayName")]
        [ResourcesDescription(typeof(Resources), "FailureProbabilityOpenStructure_Description")]
        public string FailureProbabilityOpenStructure
        {
            get
            {
                return ProbabilityFormattingHelper.Format(data.FailureProbabilityOpenStructure);
            }
        }

        [PropertyOrder(failureProbabilityReparationPropertyIndex)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_Schematization")]
        [ResourcesDisplayName(typeof(Resources), "FailureProbabilityReparation_DisplayName")]
        [ResourcesDescription(typeof(Resources), "FailureProbabilityReparation_Description")]
        public string FailureProbabilityReparation
        {
            get
            {
                return ProbabilityFormattingHelper.Format(data.FailureProbabilityReparation);
            }
        }
    }
}