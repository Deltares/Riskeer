// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using Core.Common.Util.Attributes;
using Core.Components.Gis.Properties;

namespace Core.Components.Gis.Style
{
    /// <summary>
    /// All dash styles supported by <see cref="LineStyle"/>.
    /// </summary>
    public enum LineDashStyle
    {
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.LineDashStyle_Solid_DisplayName))]
        Solid,

        [ResourcesDisplayName(typeof(Resources), nameof(Resources.LineDashStyle_Dash_DisplayName))]
        Dash,

        [ResourcesDisplayName(typeof(Resources), nameof(Resources.LineDashStyle_Dot_DisplayName))]
        Dot,

        [ResourcesDisplayName(typeof(Resources), nameof(Resources.LineDashStyle_DashDot_DisplayName))]
        DashDot,

        [ResourcesDisplayName(typeof(Resources), nameof(Resources.LineDashStyle_DashDotDot_DisplayName))]
        DashDotDot
    }
}