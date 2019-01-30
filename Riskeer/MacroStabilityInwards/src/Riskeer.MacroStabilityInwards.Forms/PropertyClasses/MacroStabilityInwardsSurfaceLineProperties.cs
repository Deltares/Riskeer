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

using System.Linq;
using Core.Common.Base.Geometry;
using Core.Common.Gui.Converters;
using Core.Common.Gui.PropertyBag;
using Core.Common.Util.Attributes;
using Riskeer.MacroStabilityInwards.Primitives;
using TypeConverter = System.ComponentModel.TypeConverterAttribute;
using RiskeerCommonDataResources = Riskeer.Common.Data.Properties.Resources;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.MacroStabilityInwards.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="MacroStabilityInwardsSurfaceLine"/> for properties panel.
    /// </summary>
    [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.SurfaceLine_DisplayName))]
    public class MacroStabilityInwardsSurfaceLineProperties : ObjectProperties<MacroStabilityInwardsSurfaceLine>
    {
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.SurfaceLine_Name_DisplayName))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.SurfaceLine_Name_Description))]
        public string Name
        {
            get
            {
                return data.Name;
            }
        }

        [TypeConverter(typeof(ExpandableArrayConverter))]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.SurfaceLine_Points_DisplayName))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.SurfaceLine_Points_Description))]
        public Point3D[] Points
        {
            get
            {
                return data.Points.ToArray();
            }
        }

        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_Characteristic_points))]
        [ResourcesDisplayName(typeof(RiskeerCommonDataResources), nameof(RiskeerCommonDataResources.CharacteristicPoint_SurfaceLevelOutside))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.CharacteristicPoint_SurfaceLevelOutside_Description))]
        public Point3D SurfaceLevelOutside
        {
            get
            {
                return data.SurfaceLevelOutside;
            }
        }

        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_Characteristic_points))]
        [ResourcesDisplayName(typeof(RiskeerCommonDataResources), nameof(RiskeerCommonDataResources.CharacteristicPoint_DikeToeAtRiver))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.CharacteristicPoint_DikeToeAtRiver_Description))]
        public Point3D DikeToeAtRiver
        {
            get
            {
                return data.DikeToeAtRiver;
            }
        }

        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_Characteristic_points))]
        [ResourcesDisplayName(typeof(RiskeerCommonDataResources), nameof(RiskeerCommonDataResources.CharacteristicPoint_DikeTopAtRiver))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.CharacteristicPoint_DikeTopAtRiver_Description))]
        public Point3D DikeTopAtRiver
        {
            get
            {
                return data.DikeTopAtRiver;
            }
        }

        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_Characteristic_points))]
        [ResourcesDisplayName(typeof(RiskeerCommonDataResources), nameof(RiskeerCommonDataResources.CharacteristicPoint_DikeTopAtPolder))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.CharacteristicPoint_DikeTopAtPolder_Description))]
        public Point3D DikeTopAtPolder
        {
            get
            {
                return data.DikeTopAtPolder;
            }
        }

        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_Characteristic_points))]
        [ResourcesDisplayName(typeof(RiskeerCommonDataResources), nameof(RiskeerCommonDataResources.CharacteristicPoint_ShoulderBaseInside))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.CharacteristicPoint_ShoulderBaseInside_Description))]
        public Point3D ShoulderBaseInside
        {
            get
            {
                return data.ShoulderBaseInside;
            }
        }

        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_Characteristic_points))]
        [ResourcesDisplayName(typeof(RiskeerCommonDataResources), nameof(RiskeerCommonDataResources.CharacteristicPoint_ShoulderTopInside))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.CharacteristicPoint_ShoulderTopInside_Description))]
        public Point3D ShoulderTopInside
        {
            get
            {
                return data.ShoulderTopInside;
            }
        }

        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_Characteristic_points))]
        [ResourcesDisplayName(typeof(RiskeerCommonDataResources), nameof(RiskeerCommonDataResources.CharacteristicPoint_DikeToeAtPolder))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.CharacteristicPoint_DikeToeAtPolder_Description))]
        public Point3D DikeToeAtPolder
        {
            get
            {
                return data.DikeToeAtPolder;
            }
        }

        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_Characteristic_points))]
        [ResourcesDisplayName(typeof(RiskeerCommonDataResources), nameof(RiskeerCommonDataResources.CharacteristicPoint_DitchDikeSide))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.CharacteristicPoint_DitchDikeSide_Description))]
        public Point3D DitchDikeSide
        {
            get
            {
                return data.DitchDikeSide;
            }
        }

        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_Characteristic_points))]
        [ResourcesDisplayName(typeof(RiskeerCommonDataResources), nameof(RiskeerCommonDataResources.CharacteristicPoint_BottomDitchDikeSide))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.CharacteristicPoint_BottomDitchDikeSide_Description))]
        public Point3D BottomDitchDikeSide
        {
            get
            {
                return data.BottomDitchDikeSide;
            }
        }

        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_Characteristic_points))]
        [ResourcesDisplayName(typeof(RiskeerCommonDataResources), nameof(RiskeerCommonDataResources.CharacteristicPoint_BottomDitchPolderSide))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.CharacteristicPoint_BottomDitchPolderSide_Description))]
        public Point3D BottomDitchPolderSide
        {
            get
            {
                return data.BottomDitchPolderSide;
            }
        }

        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_Characteristic_points))]
        [ResourcesDisplayName(typeof(RiskeerCommonDataResources), nameof(RiskeerCommonDataResources.CharacteristicPoint_DitchPolderSide))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.CharacteristicPoint_DitchPolderSide_Description))]
        public Point3D DitchPolderSide
        {
            get
            {
                return data.DitchPolderSide;
            }
        }

        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_Characteristic_points))]
        [ResourcesDisplayName(typeof(RiskeerCommonDataResources), nameof(RiskeerCommonDataResources.CharacteristicPoint_SurfaceLevelInside))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.CharacteristicPoint_SurfaceLevelInside_Description))]
        public Point3D SurfaceLevelInside
        {
            get
            {
                return data.SurfaceLevelInside;
            }
        }
    }
}