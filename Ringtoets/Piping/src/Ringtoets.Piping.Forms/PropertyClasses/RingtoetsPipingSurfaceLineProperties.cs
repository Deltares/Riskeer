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

using System.Linq;

using Core.Common.Base.Geometry;
using Core.Common.Gui.Converters;
using Core.Common.Gui.PropertyBag;
using Core.Common.Utils.Attributes;

using Ringtoets.Piping.Data;

using Ringtoets.Piping.Forms.Properties;
using TypeConverter = System.ComponentModel.TypeConverterAttribute;

namespace Ringtoets.Piping.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="RingtoetsPipingSurfaceLine"/> for properties panel.
    /// </summary>
    [ResourcesDisplayName(typeof(Resources), "RingtoetsPipingSurfaceLine_DisplayName")]
    public class RingtoetsPipingSurfaceLineProperties : ObjectProperties<RingtoetsPipingSurfaceLine>
    {
        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "RingtoetsPipingSurfaceLine_Name_DisplayName")]
        [ResourcesDescription(typeof(Resources), "RingtoetsPipingSurfaceLine_Name_Description")]
        public string Name
        {
            get
            {
                return data.Name;
            }
        }

        [ResourcesCategory(typeof(Resources), "Categories_Characteristic_points")]
        [ResourcesDisplayName(typeof(Resources), "RingtoetsPipingSurfaceLine_DikeToeAtRiver_DisplayName")]
        [ResourcesDescription(typeof(Resources), "RingtoetsPipingSurfaceLine_DikeToeAtRiver_Description")]
        public Point3D DikeToeAtRiver
        {
            get
            {
                return data.DikeToeAtRiver;
            }
        }

        [ResourcesCategory(typeof(Resources), "Categories_Characteristic_points")]
        [ResourcesDisplayName(typeof(Resources), "RingtoetsPipingSurfaceLine_DikeToeAtPolder_DisplayName")]
        [ResourcesDescription(typeof(Resources), "RingtoetsPipingSurfaceLine_DikeToeAtPolder_Description")]
        public Point3D DikeToeAtPolder
        {
            get
            {
                return data.DikeToeAtPolder;
            }
        }

        [ResourcesCategory(typeof(Resources), "Categories_Characteristic_points")]
        [ResourcesDisplayName(typeof(Resources), "RingtoetsPipingSurfaceLine_DitchDikeSide_DisplayName")]
        [ResourcesDescription(typeof(Resources), "RingtoetsPipingSurfaceLine_DitchDikeSide_Description")]
        public Point3D DitchDikeSide
        {
            get
            {
                return data.DitchDikeSide;
            }
        }

        [ResourcesCategory(typeof(Resources), "Categories_Characteristic_points")]
        [ResourcesDisplayName(typeof(Resources), "RingtoetsPipingSurfaceLine_BottomDitchDikeSide_DisplayName")]
        [ResourcesDescription(typeof(Resources), "RingtoetsPipingSurfaceLine_BottomDitchDikeSide_Description")]
        public Point3D BottomDitchDikeSide
        {
            get
            {
                return data.BottomDitchDikeSide;
            }
        }

        [ResourcesCategory(typeof(Resources), "Categories_Characteristic_points")]
        [ResourcesDisplayName(typeof(Resources), "RingtoetsPipingSurfaceLine_BottomDitchPolderSide_DisplayName")]
        [ResourcesDescription(typeof(Resources), "RingtoetsPipingSurfaceLine_BottomDitchPolderSide_Description")]
        public Point3D BottomDitchPolderSide
        {
            get
            {
                return data.BottomDitchPolderSide;
            }
        }

        [ResourcesCategory(typeof(Resources), "Categories_Characteristic_points")]
        [ResourcesDisplayName(typeof(Resources), "RingtoetsPipingSurfaceLine_DitchPolderSide_DisplayName")]
        [ResourcesDescription(typeof(Resources), "RingtoetsPipingSurfaceLine_DitchPolderSide_Description")]
        public Point3D DitchPolderSide
        {
            get
            {
                return data.DitchPolderSide;
            }
        }

        [TypeConverter(typeof(ExpandableArrayConverter))]
        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "RingtoetsPipingSurfaceLine_Points_DisplayName")]
        [ResourcesDescription(typeof(Resources), "RingtoetsPipingSurfaceLine_Points_Description")]
        public Point3D[] Points
        {
            get
            {
                return data.Points.ToArray();
            }
        }
    }
}