﻿// Copyright (C) Stichting Deltares 2016. All rights preserved.
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
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of 
// Stichting Deltares and remain full property of Stichting Deltares at all times. 
// All rights preserved.

using System.Linq;
using Core.Common.Gui;
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