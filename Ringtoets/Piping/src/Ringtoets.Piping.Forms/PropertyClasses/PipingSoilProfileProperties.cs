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

using System.Linq;
using Core.Common.Gui;
using Core.Common.Gui.Converters;
using Core.Common.Gui.PropertyBag;
using Core.Common.Utils.Attributes;

using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.Properties;
using TypeConverter = System.ComponentModel.TypeConverterAttribute;

namespace Ringtoets.Piping.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="PipingSoilProfile"/> for properties panel.
    /// </summary>
    [ResourcesDisplayName(typeof(Resources), "PipingSoilProfileProperties_DisplayName")]
    public class PipingSoilProfileProperties : ObjectProperties<PipingSoilProfile>
    {
        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "PipingSoilProfile_Name_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingSoilProfile_Name_Description")]
        public string Name
        {
            get
            {
                return data.Name;
            }
        }

        [TypeConverter(typeof(ExpandableArrayConverter))]
        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "PipingSoilProfile_Tops_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingSoilProfile_Tops_Description")]
        public double[] TopLevels
        {
            get
            {
                return data.Layers.Select(l => l.Top).ToArray();
            }
        }

        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "PipingSoilProfile_Bottom_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingSoilProfile_Bottom_Description")]
        public double Bottom
        {
            get
            {
                return data.Bottom;
            }
        }
    }
}