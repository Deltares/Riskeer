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

using System.ComponentModel;
using System.Linq;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.Gui.Converters;
using Core.Common.Gui.PropertyBag;
using Core.Common.Util.Attributes;
using Riskeer.Common.Data.DikeProfiles;
using Riskeer.Common.Forms.Properties;

namespace Riskeer.Common.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="ForeshoreProfile.Geometry"/> for properties panel.
    /// </summary>
    public class ForeshoreGeometryProperties : ObjectProperties<ForeshoreProfile>
    {
        [TypeConverter(typeof(ExpandableReadOnlyArrayConverter))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.Geometry_Coordinates_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.Geometry_Coordinates_Description))]
        public Point2D[] Coordinates
        {
            get
            {
                return data.Geometry.Select(geometry => new Point2D(new RoundedDouble(0, geometry.X),
                                                                    new RoundedDouble(0, geometry.Y))).ToArray();
            }
        }

        public override string ToString()
        {
            return string.Empty;
        }
    }
}