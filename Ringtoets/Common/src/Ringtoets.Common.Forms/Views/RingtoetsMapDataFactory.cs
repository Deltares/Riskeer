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

using System.Drawing;
using System.Drawing.Drawing2D;
using Core.Components.Gis.Data;
using Core.Components.Gis.Style;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Forms.Properties;
using Ringtoets.HydraRing.Data;
using RingtoetsCommonDataResources = Ringtoets.Common.Data.Properties.Resources;

namespace Ringtoets.Common.Forms.Views
{
    /// <summary>
    /// Factory for creating <see cref="MapData"/> for data used as input in the assessment section.
    /// </summary>
    public static class RingtoetsMapDataFactory
    {
        /// <summary>
        /// Create <see cref="MapLineData"/> with default styling for a <see cref="ReferenceLine"/>.
        /// </summary>
        /// <returns>The created <see cref="MapLineData"/>.</returns>
        public static MapLineData CreateReferenceLineMapData()
        {
            return new MapLineData(RingtoetsCommonDataResources.ReferenceLine_DisplayName)
            {
                Style = new LineStyle(Color.Red, 3, DashStyle.Solid)
            };
        }

        /// <summary>
        /// Create <see cref="MapPointData"/> with default styling for a <see cref="HydraulicBoundaryDatabase"/>.
        /// </summary>
        /// <returns>The created <see cref="MapPointData"/>.</returns>
        public static MapPointData CreateHydraulicBoundaryDatabaseMapData()
        {
            return new MapPointData(Resources.HydraulicBoundaryConditions_DisplayName)
            {
                Style = new PointStyle(Color.DarkBlue, 6, PointSymbol.Circle)
            };
        }
    }
}