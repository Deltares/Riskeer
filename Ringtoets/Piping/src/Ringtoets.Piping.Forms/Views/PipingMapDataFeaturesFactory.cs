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

using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.Geometry;
using Core.Components.Gis.Data;
using Core.Components.Gis.Features;
using Core.Components.Gis.Geometries;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.Forms.Views
{
    /// <summary>
    /// Factory for creating arrays of <see cref="MapFeature"/> to use in <see cref="MapData"/>
    /// (created via <see cref="PipingMapDataFactory"/>).
    /// </summary>
    public static class PipingMapDataFeaturesFactory
    {
        /// <summary>
        /// Create surface line features based on the provided <paramref name="surfaceLines"/>.
        /// </summary>
        /// <param name="surfaceLines">The collection of <see cref="RingtoetsPipingSurfaceLine"/> to create the surface line features for.</param>
        /// <returns>An array of features or an empty array when <paramref name="surfaceLines"/> is <c>null</c> or empty.</returns>
        public static MapFeature[] CreateSurfaceLineFeatures(IEnumerable<RingtoetsPipingSurfaceLine> surfaceLines)
        {
            return surfaceLines != null && surfaceLines.Any()
                       ? new[]
                       {
                           new MapFeature(surfaceLines.Select(surfaceLine => new MapGeometry(new[]
                           {
                               surfaceLine.Points.Select(p => new Point2D(p.X, p.Y))
                           })))
                       }
                       : new MapFeature[0];
        }

        /// <summary>
        /// Create stochastic soil model features based on the provided <paramref name="stochasticSoilModels"/>.
        /// </summary>
        /// <param name="stochasticSoilModels">The collection of <see cref="StochasticSoilModel"/> to create the stochastic soil model features for.</param>
        /// <returns>An array of features or an empty array when <paramref name="stochasticSoilModels"/> is <c>null</c> or empty.</returns>
        public static MapFeature[] CreateStochasticSoilModelFeatures(IEnumerable<StochasticSoilModel> stochasticSoilModels)
        {
            return stochasticSoilModels != null && stochasticSoilModels.Any()
                       ? new[]
                       {
                           new MapFeature(stochasticSoilModels.Select(stochasticSoilModel => new MapGeometry(new[]
                           {
                               stochasticSoilModel.Geometry.Select(p => new Point2D(p.X, p.Y))
                           })))
                       }
                       : new MapFeature[0];
        }
    }
}