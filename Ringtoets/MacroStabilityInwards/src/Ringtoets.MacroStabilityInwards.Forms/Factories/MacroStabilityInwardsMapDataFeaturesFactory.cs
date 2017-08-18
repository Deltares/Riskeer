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

using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.Geometry;
using Core.Components.Gis.Data;
using Core.Components.Gis.Features;
using Ringtoets.Common.Forms.Factories;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.MacroStabilityInwards.Data.SoilProfile;
using Ringtoets.MacroStabilityInwards.Primitives;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.MacroStabilityInwards.Forms.Factories
{
    /// <summary>
    /// Factory for creating arrays of <see cref="MapFeature"/> to use in <see cref="FeatureBasedMapData"/>
    /// (created via <see cref="MacroStabilityInwardsMapDataFactory"/>).
    /// </summary>
    internal static class MacroStabilityInwardsMapDataFeaturesFactory
    {
        /// <summary>
        /// Create surface line features based on the provided <paramref name="surfaceLines"/>.
        /// </summary>
        /// <param name="surfaceLines">The collection of <see cref="MacroStabilityInwardsSurfaceLine"/> to create the surface line features for.</param>
        /// <returns>An array of features or an empty array when <paramref name="surfaceLines"/> is <c>null</c> or empty.</returns>
        public static MapFeature[] CreateSurfaceLineFeatures(MacroStabilityInwardsSurfaceLine[] surfaceLines)
        {
            if (surfaceLines != null && surfaceLines.Any())
            {
                var features = new MapFeature[surfaceLines.Length];

                for (var i = 0; i < surfaceLines.Length; i++)
                {
                    MacroStabilityInwardsSurfaceLine surfaceLine = surfaceLines[i];

                    MapFeature feature = RingtoetsMapDataFeaturesFactory.CreateSingleLineMapFeature(GetWorldPoints(surfaceLine));
                    feature.MetaData[RingtoetsCommonFormsResources.MetaData_Name] = surfaceLine.Name;

                    features[i] = feature;
                }

                return features;
            }

            return new MapFeature[0];
        }

        /// <summary>
        /// Create stochastic soil model features based on the provided <paramref name="stochasticSoilModels"/>.
        /// </summary>
        /// <param name="stochasticSoilModels">The collection of <see cref="StochasticSoilModel"/> to create the stochastic soil model features for.</param>
        /// <returns>An array of features or an empty array when <paramref name="stochasticSoilModels"/> is <c>null</c> or empty.</returns>
        public static MapFeature[] CreateStochasticSoilModelFeatures(StochasticSoilModel[] stochasticSoilModels)
        {
            if (stochasticSoilModels != null && stochasticSoilModels.Any())
            {
                var features = new MapFeature[stochasticSoilModels.Length];

                for (var i = 0; i < stochasticSoilModels.Length; i++)
                {
                    StochasticSoilModel stochasticSoilModel = stochasticSoilModels[i];

                    MapFeature feature = RingtoetsMapDataFeaturesFactory.CreateSingleLineMapFeature(GetWorldPoints(stochasticSoilModel));
                    feature.MetaData[RingtoetsCommonFormsResources.MetaData_Name] = stochasticSoilModel.Name;

                    features[i] = feature;
                }

                return features;
            }

            return new MapFeature[0];
        }

        /// <summary>
        /// Create calculation features based on the provided <paramref name="calculations"/>.
        /// </summary>
        /// <param name="calculations">The collection of <see cref="MacroStabilityInwardsCalculationScenario"/> to create the calculation features for.</param>
        /// <returns>An array of features or an empty array when <paramref name="calculations"/> is <c>null</c> or empty.</returns>
        public static MapFeature[] CreateCalculationFeatures(IEnumerable<MacroStabilityInwardsCalculationScenario> calculations)
        {
            bool hasCalculations = calculations != null && calculations.Any();

            if (!hasCalculations)
            {
                return new MapFeature[0];
            }

            IEnumerable<MacroStabilityInwardsCalculationScenario> calculationsWithLocationAndHydraulicBoundaryLocation =
                calculations.Where(c =>
                                       c.InputParameters.SurfaceLine != null &&
                                       c.InputParameters.HydraulicBoundaryLocation != null);

            MapCalculationData[] calculationData =
                calculationsWithLocationAndHydraulicBoundaryLocation.Select(
                    calculation => new MapCalculationData(
                        calculation.Name,
                        calculation.InputParameters.SurfaceLine.ReferenceLineIntersectionWorldPoint,
                        calculation.InputParameters.HydraulicBoundaryLocation)).ToArray();

            return RingtoetsMapDataFeaturesFactory.CreateCalculationFeatures(calculationData);
        }

        private static IEnumerable<Point2D> GetWorldPoints(MacroStabilityInwardsSurfaceLine surfaceLine)
        {
            return surfaceLine.Points.Select(p => new Point2D(p.X, p.Y));
        }

        private static IEnumerable<Point2D> GetWorldPoints(StochasticSoilModel stochasticSoilModel)
        {
            return stochasticSoilModel.Geometry.Select(p => new Point2D(p));
        }
    }
}