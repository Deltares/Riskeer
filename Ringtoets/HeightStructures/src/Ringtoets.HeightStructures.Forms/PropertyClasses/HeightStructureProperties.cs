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

using System.ComponentModel;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.Gui.Attributes;
using Core.Common.Gui.PropertyBag;
using Core.Common.Utils.Attributes;
using Ringtoets.Common.Forms.Helpers;
using Ringtoets.Common.Forms.PropertyClasses;
using Ringtoets.HeightStructures.Data;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.HeightStructures.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="HeightStructure"/> for properties panel.
    /// </summary>
    public class HeightStructureProperties : ObjectProperties<HeightStructure>
    {
        private const int namePropertyIndex = 1;
        private const int locationPropertyIndex = 2;
        private const int structureNormalOrientationPropertyIndex = 3;
        private const int flowWidthAtBottomProtectionPropertyIndex = 4;
        private const int widthFlowAperturesPropertyIndex = 5;
        private const int storageStructureAreaPropertyIndex = 6;
        private const int allowedLevelIncreaseStoragePropertyIndex = 7;
        private const int levelCrestStructurePropertyIndex = 8;
        private const int criticalOvertoppingDischargePropertyIndex = 9;
        private const int failureProbabilityStructureWithErosionPropertyIndex = 10;

        [PropertyOrder(namePropertyIndex)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Structure_Name_DisplayName))]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Structure_Name_Description))]
        public string Name
        {
            get
            {
                return data.Name;
            }
        }

        [PropertyOrder(locationPropertyIndex)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Structure_Location_DisplayName))]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Structure_Location_Description))]
        public Point2D Location
        {
            get
            {
                return new Point2D(new RoundedDouble(0, data.Location.X),
                                   new RoundedDouble(0, data.Location.Y));
            }
        }

        [PropertyOrder(structureNormalOrientationPropertyIndex)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_Schematization))]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Structure_StructureNormalOrientation_DisplayName))]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Structure_StructureNormalOrientation_Description))]
        public RoundedDouble StructureNormalOrientation
        {
            get
            {
                return data.StructureNormalOrientation;
            }
        }

        [PropertyOrder(flowWidthAtBottomProtectionPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_Schematization))]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Structure_FlowWidthAtBottomProtection_DisplayName))]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Structure_FlowWidthAtBottomProtection_Description))]
        public LogNormalDistributionProperties FlowWidthAtBottomProtection
        {
            get
            {
                return new LogNormalDistributionProperties(data.FlowWidthAtBottomProtection);
            }
        }

        [PropertyOrder(widthFlowAperturesPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_Schematization))]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Structure_WidthFlowApertures_DisplayName))]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Structure_WidthFlowApertures_Description))]
        public NormalDistributionProperties WidthFlowApertures
        {
            get
            {
                return new NormalDistributionProperties(data.WidthFlowApertures);
            }
        }

        [PropertyOrder(storageStructureAreaPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_Schematization))]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Structure_StorageStructureArea_DisplayName))]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Structure_StorageStructureArea_Description))]
        public VariationCoefficientLogNormalDistributionProperties StorageStructureArea
        {
            get
            {
                return new VariationCoefficientLogNormalDistributionProperties(data.StorageStructureArea);
            }
        }

        [PropertyOrder(allowedLevelIncreaseStoragePropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_Schematization))]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Structure_AllowedLevelIncreaseStorage_DisplayName))]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Structure_AllowedLevelIncreaseStorage_Description))]
        public LogNormalDistributionProperties AllowedLevelIncreaseStorage
        {
            get
            {
                return new LogNormalDistributionProperties(data.AllowedLevelIncreaseStorage);
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
                return new NormalDistributionProperties(data.LevelCrestStructure);
            }
        }

        [PropertyOrder(criticalOvertoppingDischargePropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_Schematization))]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Structure_CriticalOvertoppingDischarge_DisplayName))]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Structure_CriticalOvertoppingDischarge_Description))]
        public VariationCoefficientLogNormalDistributionProperties CriticalOvertoppingDischarge
        {
            get
            {
                return new VariationCoefficientLogNormalDistributionProperties(data.CriticalOvertoppingDischarge);
            }
        }

        [PropertyOrder(failureProbabilityStructureWithErosionPropertyIndex)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_Schematization))]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Structure_FailureProbabilityStructureWithErosion_DisplayName))]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Structure_FailureProbabilityStructureWithErosion_Description))]
        public string FailureProbabilityStructureWithErosion
        {
            get
            {
                return ProbabilityFormattingHelper.Format(data.FailureProbabilityStructureWithErosion);
            }
        }
    }
}