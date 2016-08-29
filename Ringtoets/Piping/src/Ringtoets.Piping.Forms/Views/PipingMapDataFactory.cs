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
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.Properties;
using Ringtoets.Piping.Primitives;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.Piping.Forms.Views
{
    /// <summary>
    /// Factory for creating <see cref="MapData"/> for data used as input in the piping failure mechanism.
    /// </summary>
    public static class PipingMapDataFactory
    {
        /// <summary>
        /// Create <see cref="MapLineData"/> with default styling for collections of <see cref="RingtoetsPipingSurfaceLine"/>.
        /// </summary>
        /// <returns>The created <see cref="MapLineData"/>.</returns>
        public static MapLineData CreateSurfaceLinesMapData()
        {
            return new MapLineData(Resources.PipingSurfaceLinesCollection_DisplayName)
            {
                Style = new LineStyle(Color.DarkSeaGreen, 2, DashStyle.Solid)
            };
        }

        /// <summary>
        /// Create <see cref="MapLineData"/> with default styling for collections of <see cref="StochasticSoilModel"/>.
        /// </summary>
        /// <returns>The created <see cref="MapLineData"/>.</returns>
        public static MapLineData CreateStochasticSoilModelsMapData()
        {
            return new MapLineData(Resources.StochasticSoilModelCollection_DisplayName)
            {
                Style = new LineStyle(Color.FromArgb(70, Color.SaddleBrown), 5, DashStyle.Solid)
            };
        }

        /// <summary>
        /// Create <see cref="MapLineData"/> with default styling for collections of <see cref="FailureMechanismSection"/>.
        /// </summary>
        /// <returns>The created <see cref="MapLineData"/>.</returns>
        public static MapLineData CreateFailureMechanismSectionsMapData()
        {
            return new MapLineData(RingtoetsCommonFormsResources.FailureMechanism_Sections_DisplayName)
            {
                Style = new LineStyle(Color.Khaki, 3, DashStyle.Dot)
            };
        }

        /// <summary>
        /// Create <see cref="MapPointData"/> with default styling for the start points in collections of <see cref="FailureMechanismSection"/>.
        /// </summary>
        /// <returns>The created <see cref="MapPointData"/>.</returns>
        public static MapPointData CreateFailureMechanismSectionsStartPointMapData()
        {
            var mapDataName = string.Format("{0} ({1})",
                                            RingtoetsCommonFormsResources.FailureMechanism_Sections_DisplayName,
                                            RingtoetsCommonFormsResources.FailureMechanismSections_StartPoints_DisplayName);

            return new MapPointData(mapDataName)
            {
                Style = new PointStyle(Color.DarkKhaki, 15, PointSymbol.Triangle)
            };
        }

        /// <summary>
        /// Create <see cref="MapPointData"/> with default styling for the end points in collections of <see cref="FailureMechanismSection"/>.
        /// </summary>
        /// <returns>The created <see cref="MapPointData"/>.</returns>
        public static MapPointData CreateFailureMechanismSectionsEndPointMapData()
        {
            var mapDataName = string.Format("{0} ({1})",
                                            RingtoetsCommonFormsResources.FailureMechanism_Sections_DisplayName,
                                            RingtoetsCommonFormsResources.FailureMechanismSections_EndPoints_DisplayName);

            return new MapPointData(mapDataName)
            {
                Style = new PointStyle(Color.DarkKhaki, 15, PointSymbol.Triangle)
            };
        }
    }
}