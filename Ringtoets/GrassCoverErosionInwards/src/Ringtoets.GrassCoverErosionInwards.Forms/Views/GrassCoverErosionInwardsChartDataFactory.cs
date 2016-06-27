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

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using Core.Components.Charting.Data;
using Core.Components.Charting.Styles;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionInwards.Forms.Properties;

namespace Ringtoets.GrassCoverErosionInwards.Forms.Views
{
    /// <summary>
    /// Factory for creating <see cref="ChartData"/> based on information used as input
    /// in the grass cover erosion inwards failure mechanism.
    /// </summary>
    public static class GrassCoverErosionInwardsChartDataFactory
    {
        /// <summary>
        /// Create <see cref="ChartData"/> with default styling based on the <paramref name="dikeProfile"/>.
        /// </summary>
        /// <param name="dikeProfile">The <see cref="DikeProfile"/> for which to create <see cref="ChartData"/>.</param>
        /// <returns><see cref="ChartData"/> based on the <paramref name="dikeProfile"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="dikeProfile"/> is <c>null</c>.</exception>
        public static ChartData Create(DikeProfile dikeProfile)
        {
            if (dikeProfile == null)
            {
                throw new ArgumentNullException("dikeProfile");
            }

            return new ChartLineData(dikeProfile.DikeGeometry.Select(dg => dg.Point), Resources.DikeProfile_DisplayName)
            {
                Style = new ChartLineStyle(Color.SaddleBrown, 2, DashStyle.Solid)
            };
        }
    }
}