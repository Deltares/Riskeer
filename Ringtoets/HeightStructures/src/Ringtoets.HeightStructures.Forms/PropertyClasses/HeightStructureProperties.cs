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
using Core.Common.Utils.Attributes;
using Ringtoets.Common.Forms.PropertyClasses;
using Ringtoets.HeightStructures.Data;
using Ringtoets.HeightStructures.Forms.Properties;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.HeightStructures.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="HeightStructure"/> for properties panel.
    /// </summary>
    public class HeightStructureProperties : ObjectProperties<HeightStructure>
    {
        [PropertyOrder(1)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "HeightStructure_Name_DisplayName")]
        [ResourcesDescription(typeof(Resources), "HeightStructure_Name_Description")]
        public string Name
        {
            get
            {
                return data.Name;
            }
        }

        [PropertyOrder(2)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "HeightStructure_Location_DisplayName")]
        [ResourcesDescription(typeof(Resources), "HeightStructure_Location_Description")]
        public Point2D Location
        {
            get
            {
                return data.Location;
            }
        }

        [PropertyOrder(3)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_Schematization")]
        [ResourcesDisplayName(typeof(Resources), "StructureNormalOrientation_DisplayName")]
        [ResourcesDescription(typeof(Resources), "StructureNormalOrientation_Description")]
        public RoundedDouble StructureNormalOrientation
        {
            get
            {
                return data.StructureNormalOrientation;
            }
        }

        [PropertyOrder(4)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_Schematization")]
        [ResourcesDisplayName(typeof(Resources), "LevelCrestStructure_DisplayName")]
        [ResourcesDescription(typeof(Resources), "LevelCrestStructure_Description")]
        public NormalDistributionProperties LevelCrestStructure
        {
            get
            {
                return new NormalDistributionProperties
                {
                    Data = data.LevelCrestStructure
                };
            }
        }

        [PropertyOrder(5)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_Schematization")]
        [ResourcesDisplayName(typeof(Resources), "AllowedLevelIncreaseStorage_DisplayName")]
        [ResourcesDescription(typeof(Resources), "AllowedLevelIncreaseStorage_Description")]
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

        [PropertyOrder(6)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_Schematization")]
        [ResourcesDisplayName(typeof(Resources), "StorageStructureArea_DisplayName")]
        [ResourcesDescription(typeof(Resources), "StorageStructureArea_Description")]
        public LogNormalDistributionProperties StorageStructureArea
        {
            get
            {
                return new LogNormalDistributionProperties
                {
                    Data = data.StorageStructureArea
                };
            }
        }

        [PropertyOrder(7)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_Schematization")]
        [ResourcesDisplayName(typeof(Resources), "FlowWidthAtBottomProtection_DisplayName")]
        [ResourcesDescription(typeof(Resources), "FlowWidthAtBottomProtection_Description")]
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

        [PropertyOrder(8)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_Schematization")]
        [ResourcesDisplayName(typeof(Resources), "WidthOfFlowApertures_DisplayName")]
        [ResourcesDescription(typeof(Resources), "WidthOfFlowApertures_Description")]
        public NormalDistributionProperties WidthOfFlowApertures
        {
            get
            {
                return new NormalDistributionProperties
                {
                    Data = data.WidthOfFlowApertures
                };
            }
        }

        [PropertyOrder(9)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_Schematization")]
        [ResourcesDisplayName(typeof(Resources), "CriticalOvertoppingDischarge_DisplayName")]
        [ResourcesDescription(typeof(Resources), "CriticalOvertoppingDischarge_Description")]
        public LogNormalDistributionProperties CriticalOvertoppingDischarge
        {
            get
            {
                return new LogNormalDistributionProperties
                {
                    Data = data.CriticalOvertoppingDischarge
                };
            }
        }

        [PropertyOrder(10)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_Schematization")]
        [ResourcesDisplayName(typeof(Resources), "FailureProbabilityOfStructureGivenErosion_DisplayName")]
        [ResourcesDescription(typeof(Resources), "FailureProbabilityOfStructureGivenErosion_Description")]
        public double FailureProbabilityOfStructureGivenErosion
        {
            get
            {
                return data.FailureProbabilityOfStructureGivenErosion;
            }
        }
    }
}