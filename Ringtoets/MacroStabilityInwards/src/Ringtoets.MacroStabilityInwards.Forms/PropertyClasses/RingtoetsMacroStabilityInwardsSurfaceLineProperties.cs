﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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

using System.Linq;
using Core.Common.Base.Geometry;
using Core.Common.Gui.Converters;
using Core.Common.Gui.PropertyBag;
using Core.Common.Utils.Attributes;
using Ringtoets.MacroStabilityInwards.Forms.Properties;
using Ringtoets.MacroStabilityInwards.Primitives;
using TypeConverter = System.ComponentModel.TypeConverterAttribute;

using RingtoetsCommonDataResources = Ringtoets.Common.Data.Properties.Resources;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.MacroStabilityInwards.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="RingtoetsMacroStabilityInwardsSurfaceLine"/> for properties panel.
    /// </summary>
    [ResourcesDisplayName(typeof(Resources), nameof(Resources.RingtoetsMacroStabilityInwardsSurfaceLine_DisplayName))]
    public class RingtoetsMacroStabilityInwardsSurfaceLineProperties : ObjectProperties<RingtoetsMacroStabilityInwardsSurfaceLine>
    {
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.SurfaceLine_Name_DisplayName))]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.SurfaceLine_Name_Description))]
        public string Name
        {
            get
            {
                return data.Name;
            }
        }

        [TypeConverter(typeof(ExpandableArrayConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.SurfaceLine_Points_DisplayName))]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.SurfaceLine_Points_Description))]
        public Point3D[] Points
        {
            get
            {
                return data.Points.ToArray();
            }
        }

        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_Characteristic_points))]
        [ResourcesDisplayName(typeof(RingtoetsCommonDataResources), nameof(RingtoetsCommonDataResources.CharacteristicPoint_SurfaceLevelOutside))]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.CharacteristicPoint_SurfaceLevelOutside_Description))]
        public Point3D SurfaceLevelOutside
        {
            get
            {
                return data.SurfaceLevelOutside;
            }
        }

        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_Characteristic_points))]
        [ResourcesDisplayName(typeof(RingtoetsCommonDataResources), nameof(RingtoetsCommonDataResources.CharacteristicPoint_DikeToeAtRiver))]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.CharacteristicPoint_DikeToeAtRiver_Description))]
        public Point3D DikeToeAtRiver
        {
            get
            {
                return data.DikeToeAtRiver;
            }
        }

        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_Characteristic_points))]
        [ResourcesDisplayName(typeof(RingtoetsCommonDataResources), nameof(RingtoetsCommonDataResources.CharacteristicPoint_TrafficLoadOutside))]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.CharacteristicPoint_TrafficLoadOutside_Description))]
        public Point3D TrafficLoadOutside
        {
            get
            {
                return data.TrafficLoadOutside;
            }
        }

        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_Characteristic_points))]
        [ResourcesDisplayName(typeof(RingtoetsCommonDataResources), nameof(RingtoetsCommonDataResources.CharacteristicPoint_TrafficLoadInside))]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.CharacteristicPoint_TrafficLoadInside_Description))]
        public Point3D TrafficLoadInside
        {
            get
            {
                return data.TrafficLoadInside;
            }
        }

        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_Characteristic_points))]
        [ResourcesDisplayName(typeof(RingtoetsCommonDataResources), nameof(RingtoetsCommonDataResources.CharacteristicPoint_DikeTopAtPolder))]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.CharacteristicPoint_DikeTopAtPolder_Description))]
        public Point3D DikeTopAtPolder
        {
            get
            {
                return data.DikeTopAtPolder;
            }
        }

        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_Characteristic_points))]
        [ResourcesDisplayName(typeof(RingtoetsCommonDataResources), nameof(RingtoetsCommonDataResources.CharacteristicPoint_ShoulderBaseInside))]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.CharacteristicPoint_ShoulderBaseInside_Description))]
        public Point3D ShoulderBaseInside
        {
            get
            {
                return data.ShoulderBaseInside;
            }
        }

        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_Characteristic_points))]
        [ResourcesDisplayName(typeof(RingtoetsCommonDataResources), nameof(RingtoetsCommonDataResources.CharacteristicPoint_ShoulderTopInside))]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.CharacteristicPoint_ShoulderTopInside_Description))]
        public Point3D ShoulderTopInside
        {
            get
            {
                return data.ShoulderTopInside;
            }
        }

        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_Characteristic_points))]
        [ResourcesDisplayName(typeof(RingtoetsCommonDataResources), nameof(RingtoetsCommonDataResources.CharacteristicPoint_DikeToeAtPolder))]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.CharacteristicPoint_DikeToeAtPolder_Description))]
        public Point3D DikeToeAtPolder
        {
            get
            {
                return data.DikeToeAtPolder;
            }
        }

        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_Characteristic_points))]
        [ResourcesDisplayName(typeof(RingtoetsCommonDataResources), nameof(RingtoetsCommonDataResources.CharacteristicPoint_DitchDikeSide))]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.CharacteristicPoint_DitchDikeSide_Description))]
        public Point3D DitchDikeSide
        {
            get
            {
                return data.DitchDikeSide;
            }
        }

        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_Characteristic_points))]
        [ResourcesDisplayName(typeof(RingtoetsCommonDataResources), nameof(RingtoetsCommonDataResources.CharacteristicPoint_BottomDitchDikeSide))]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.CharacteristicPoint_BottomDitchDikeSide_Description))]
        public Point3D BottomDitchDikeSide
        {
            get
            {
                return data.BottomDitchDikeSide;
            }
        }

        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_Characteristic_points))]
        [ResourcesDisplayName(typeof(RingtoetsCommonDataResources), nameof(RingtoetsCommonDataResources.CharacteristicPoint_BottomDitchPolderSide))]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.CharacteristicPoint_BottomDitchPolderSide_Description))]
        public Point3D BottomDitchPolderSide
        {
            get
            {
                return data.BottomDitchPolderSide;
            }
        }

        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_Characteristic_points))]
        [ResourcesDisplayName(typeof(RingtoetsCommonDataResources), nameof(RingtoetsCommonDataResources.CharacteristicPoint_DitchPolderSide))]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.CharacteristicPoint_DitchPolderSide_Description))]
        public Point3D DitchPolderSide
        {
            get
            {
                return data.DitchPolderSide;
            }
        }

        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_Characteristic_points))]
        [ResourcesDisplayName(typeof(RingtoetsCommonDataResources), nameof(RingtoetsCommonDataResources.CharacteristicPoint_SurfaceLevelInside))]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.CharacteristicPoint_SurfaceLevelInside_Description))]
        public Point3D SurfaceLevelInside
        {
            get
            {
                return data.SurfaceLevelInside;
            }
        }
    }
}