// Copyright (C) Stichting Deltares 2017. All rights reserved.
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

using System.Drawing;
using Core.Components.Charting.Data;
using Core.Components.Charting.Styles;
using Ringtoets.Common.Forms.Properties;

namespace Ringtoets.Common.Forms.Factories
{
    /// <summary>
    /// Factory for creating <see cref="ChartData"/> based on information used as input.
    /// </summary>
    public static class RingtoetsChartDataFactory
    {
        /// <summary>
        /// Create <see cref="ChartLineData"/> with default styling for a foreshore geometry.
        /// </summary>
        /// <returns>The created <see cref="ChartLineData"/>.</returns>
        public static ChartLineData CreateForeshoreGeometryChartData()
        {
            return new ChartLineData(Resources.Foreshore_DisplayName,
                                     new ChartLineStyle
                                     {
                                         Color = Color.DarkOrange,
                                         Width = 2,
                                         DashStyle = ChartLineDashStyle.Solid
                                     });
        }
    }
}