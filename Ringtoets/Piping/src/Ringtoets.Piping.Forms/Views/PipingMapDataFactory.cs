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
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using Core.Common.Base.Geometry;
using Core.Components.Gis.Data;
using Core.Components.Gis.Features;
using Core.Components.Gis.Geometries;
using Core.Components.Gis.Style;
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.HydraRing.Data;
using Ringtoets.Piping.Forms.Properties;
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.Forms.Views
{
    /// <summary>
    /// Factory for creating <see cref="MapData"/> based on information used as input in the piping failure mechanism.
    /// </summary>
    public static class PipingMapDataFactory
    {
        /// <summary>
        /// Create <see cref="MapData"/> with default styling based on the <paramref name="surfaceLines"/>.
        /// </summary>
        /// <param name="surfaceLines">The <see cref="RingtoetsPipingSurfaceLine"/> collection for which to create <see cref="MapData"/>.</param>
        /// <returns><see cref="MapData"/> based on <paramref name="surfaceLines"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="surfaceLines"/> is <c>null</c>.</exception>
        public static MapData Create(IEnumerable<RingtoetsPipingSurfaceLine> surfaceLines)
        {
            if (surfaceLines == null)
            {
                throw new ArgumentNullException("surfaceLines");
            }

            var mapFeatures = new List<MapFeature>
            {
                new MapFeature(surfaceLines.Select(surfaceLine => new MapGeometry(surfaceLine.Points.Select(p => new Point2D(p.X, p.Y)))))
            };

            return new MapLineData(mapFeatures, Resources.PipingSurfaceLinesCollection_DisplayName)
            {
                Style = new LineStyle(Color.DarkSeaGreen, 2, DashStyle.Solid)
            };
        }

        /// <summary>
        /// Create <see cref="MapData"/> with default styling based on the <paramref name="sections"/>.
        /// </summary>
        /// <param name="sections">The <see cref="FailureMechanismSection"/> collection for which to create <see cref="MapData"/>.</param>
        /// <returns><see cref="MapData"/> based on <paramref name="sections"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="sections"/> is <c>null</c>.</exception>
        public static MapData Create(IEnumerable<FailureMechanismSection> sections)
        {
            if (sections == null)
            {
                throw new ArgumentNullException("sections");
            }

            var mapFeatures = new List<MapFeature>
            {
                new MapFeature(sections.Select(section => new MapGeometry(section.Points.Select(p => new Point2D(p.X, p.Y)))))
            };

            return new MapLineData(mapFeatures, Common.Forms.Properties.Resources.FailureMechanism_Sections_DisplayName)
            {
                Style = new LineStyle(Color.Khaki, 3, DashStyle.Dot)
            };
        }

        /// <summary>
        /// Create <see cref="MapData"/> with default styling based on the start points of <paramref name="sections"/>.
        /// </summary>
        /// <param name="sections">The <see cref="FailureMechanismSection"/> collection which's start points will be used to create <see cref="MapData"/>.</param>
        /// <returns><see cref="MapData"/> based on the start points of <paramref name="sections"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="sections"/> is <c>null</c>.</exception>
        public static MapData CreateStartPoints(IEnumerable<FailureMechanismSection> sections)
        {
            if (sections == null)
            {
                throw new ArgumentNullException("sections");
            }

            IEnumerable<Point2D> startPoints = sections.Select(sl => sl.GetStart());
            string mapDataName = string.Format("{0} ({1})",
                                               Common.Forms.Properties.Resources.FailureMechanism_Sections_DisplayName,
                                               Common.Forms.Properties.Resources.FailureMechanismSections_StartPoints_DisplayName);
            return new MapPointData(GetMapFeature(startPoints), mapDataName)
            {
                Style = new PointStyle(Color.DarkKhaki, 15, PointSymbol.Triangle)
            };
        }

        /// <summary>
        /// Create <see cref="MapData"/> with default styling based on the end points of <paramref name="sections"/>.
        /// </summary>
        /// <param name="sections">The <see cref="FailureMechanismSection"/> collection which's end points will be used to create <see cref="MapData"/>.</param>
        /// <returns><see cref="MapData"/> based on the end points of <paramref name="sections"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="sections"/> is <c>null</c>.</exception>
        public static MapData CreateEndPoints(IEnumerable<FailureMechanismSection> sections)
        {
            if (sections == null)
            {
                throw new ArgumentNullException("sections");
            }

            IEnumerable<Point2D> startPoints = sections.Select(sl => sl.GetLast());
            string mapDataName = string.Format("{0} ({1})",
                                               Common.Forms.Properties.Resources.FailureMechanism_Sections_DisplayName,
                                               Common.Forms.Properties.Resources.FailureMechanismSections_EndPoints_DisplayName);
            return new MapPointData(GetMapFeature(startPoints), mapDataName)
            {
                Style = new PointStyle(Color.DarkKhaki, 15, PointSymbol.Triangle)
            };
        }

        /// <summary>
        /// Create <see cref="MapData"/> with default styling based on the <paramref name="referenceLine"/>.
        /// </summary>
        /// <param name="referenceLine">The <see cref="ReferenceLine"/> for which to create <see cref="MapData"/>.</param>
        /// <returns><see cref="MapData"/> based on the <paramref name="referenceLine"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="referenceLine"/> is <c>null</c>.</exception>
        public static MapData Create(ReferenceLine referenceLine)
        {
            if (referenceLine == null)
            {
                throw new ArgumentNullException("referenceLine");
            }

            var features = GetMapFeature(referenceLine.Points);

            return new MapLineData(features, Common.Data.Properties.Resources.ReferenceLine_DisplayName)
            {
                Style = new LineStyle(Color.Red, 3, DashStyle.Solid)
            };
        }

        /// <summary>
        /// Create <see cref="MapData"/> with default styling based on the locations of <paramref name="hydraulicBoundaryDatabase"/>.
        /// </summary>
        /// <param name="hydraulicBoundaryDatabase">The <see cref="HydraulicBoundaryDatabase"/> for which to create <see cref="MapData"/>.</param>
        /// <returns><see cref="MapData"/> based on the locations in the <paramref name="hydraulicBoundaryDatabase"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="hydraulicBoundaryDatabase"/> is <c>null</c>.</exception>
        public static MapData Create(HydraulicBoundaryDatabase hydraulicBoundaryDatabase)
        {
            if (hydraulicBoundaryDatabase == null)
            {
                throw new ArgumentNullException("hydraulicBoundaryDatabase");
            }

            IEnumerable<Point2D> locations = hydraulicBoundaryDatabase.Locations.Select(h => h.Location).ToArray();

            var features = GetMapFeature(locations);

            return new MapPointData(features, Common.Data.Properties.Resources.HydraulicBoundaryConditions_DisplayName)
            {
                Style = new PointStyle(Color.DarkBlue, 6, PointSymbol.Circle)
            };
        }

        /// <summary>
        /// Create a <see cref="MapLineData"/> instance with a name, but without data.
        /// </summary>
        /// <param name="name">The name of the <see cref="MapLineData"/>.</param>
        /// <returns>An empty <see cref="MapLineData"/> object.</returns>
        public static MapLineData CreateEmptyLineData(string name)
        {
            return new MapLineData(Enumerable.Empty<MapFeature>(), name);
        }

        /// <summary>
        /// Create a <see cref="MapPointData"/> instance with a name, but without data.
        /// </summary>
        /// <param name="name">The name of the <see cref="MapPointData"/>.</param>
        /// <returns>An empty <see cref="MapPointData"/> object.</returns>
        public static MapPointData CreateEmptyPointData(string name)
        {
            return new MapPointData(Enumerable.Empty<MapFeature>(), name);
        }

        private static IEnumerable<MapFeature> GetMapFeature(IEnumerable<Point2D> points)
        {
            var features = new List<MapFeature>
            {
                new MapFeature(new List<MapGeometry>
                {
                    new MapGeometry(points)
                })
            };
            return features;
        }
    }
}