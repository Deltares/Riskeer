// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Core.Common.Util.Attributes;
using Riskeer.Piping.Primitives;
using TypeConverter = System.ComponentModel.TypeConverterAttribute;
using RingtoetsCommonDataResources = Ringtoets.Common.Data.Properties.Resources;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Riskeer.Piping.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="PipingSurfaceLine"/> for properties panel.
    /// </summary>
    [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.SurfaceLine_DisplayName))]
    public class PipingSurfaceLineProperties : ObjectProperties<PipingSurfaceLine>
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
    }
}