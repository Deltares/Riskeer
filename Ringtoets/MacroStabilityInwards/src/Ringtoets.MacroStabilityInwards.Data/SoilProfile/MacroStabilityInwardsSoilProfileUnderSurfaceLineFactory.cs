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

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Core.Common.Base.Geometry;
using Core.Common.Geometry;
using Ringtoets.MacroStabilityInwards.Primitives;

namespace Ringtoets.MacroStabilityInwards.Data.SoilProfile
{
    /// <summary>
    /// Class for constructing soil profiles for which the geometry of the layers lay under a surface line.
    /// </summary>
    public static class MacroStabilityInwardsSoilProfileUnderSurfaceLineFactory
    {
        /// <summary>
        /// Creates a new <see cref="MacroStabilityInwardsSoilProfileUnderSurfaceLine"/>.
        /// </summary>
        /// <param name="soilProfile">The soil profile containing layers under the <paramref name="surfaceLine"/>.</param>
        /// <param name="surfaceLine">The surface line for which determines the top of the <paramref name="soilProfile"/>.</param>
        /// <returns>A new <see cref="MacroStabilityInwardsSoilProfileUnderSurfaceLine"/> containing geometries from the 
        /// <paramref name="soilProfile"/> under the <paramref name="surfaceLine"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        /// <exception cref="NotSupportedException">Thrown when the given <paramref name="soilProfile"/> type
        /// is not supported.</exception>
        public static MacroStabilityInwardsSoilProfileUnderSurfaceLine Create(IMacroStabilityInwardsSoilProfile soilProfile,
                                                                              MacroStabilityInwardsSurfaceLine surfaceLine)
        {
            if (soilProfile == null)
            {
                throw new ArgumentNullException(nameof(soilProfile));
            }
            if (surfaceLine == null)
            {
                throw new ArgumentNullException(nameof(surfaceLine));
            }

            var profile1D = soilProfile as MacroStabilityInwardsSoilProfile1D;
            if (profile1D != null)
            {
                return Create(profile1D, surfaceLine);
            }
            var profile2D = soilProfile as MacroStabilityInwardsSoilProfile2D;
            if (profile2D != null)
            {
                return Create(profile2D);
            }
            throw new NotSupportedException();
        }

        private static MacroStabilityInwardsSoilProfileUnderSurfaceLine Create(MacroStabilityInwardsSoilProfile1D soilProfile,
                                                                               MacroStabilityInwardsSurfaceLine surfaceLine)
        {
            Point2D[] localizedSurfaceLine = surfaceLine.LocalGeometry.ToArray();

            double geometryBottom = Math.Min(soilProfile.Bottom, localizedSurfaceLine.Min(p => p.Y)) - 1;
            IEnumerable<Point2D> surfaceLineGeometry = AdvancedMath2D.CompleteLineToPolygon(localizedSurfaceLine, geometryBottom);
            IEnumerable<TempSoilLayerGeometry> layerGeometries = soilProfile.Layers.Select(
                layer => As2DGeometry(
                    layer,
                    soilProfile,
                    localizedSurfaceLine.First().X,
                    localizedSurfaceLine.Last().X));

            return GeometriesToIntersections(layerGeometries, surfaceLineGeometry);
        }

        private static MacroStabilityInwardsSoilProfileUnderSurfaceLine Create(MacroStabilityInwardsSoilProfile2D soilProfile)
        {
            IEnumerable<MacroStabilityInwardsSoilLayerUnderSurfaceLine> layersUnderSurfaceLine = soilProfile.Layers.Select(
                layer => new MacroStabilityInwardsSoilLayerUnderSurfaceLine(
                    RingToPoints(layer.OuterRing),
                    layer.Holes.Select(RingToPoints),
                    ToUnderSurfaceLineProperties(layer.Properties)));

            return new MacroStabilityInwardsSoilProfileUnderSurfaceLine(layersUnderSurfaceLine);
        }

        private static MacroStabilityInwardsSoilLayerPropertiesUnderSurfaceLine ToUnderSurfaceLineProperties(
            MacroStabilityInwardsSoilLayerProperties properties)
        {
            var props = new MacroStabilityInwardsSoilLayerPropertiesUnderSurfaceLine(
                new MacroStabilityInwardsSoilLayerPropertiesUnderSurfaceLine.ConstructionProperties
                {
                    UsePop = properties.UsePop,
                    IsAquifer = properties.IsAquifer,
                    ShearStrengthModel = properties.ShearStrengthModel,
                    MaterialName = properties.MaterialName,
                    AbovePhreaticLevelMean = properties.AbovePhreaticLevelMean,
                    AbovePhreaticLevelCoefficientOfVariation = properties.AbovePhreaticLevelCoefficientOfVariation,
                    BelowPhreaticLevelMean = properties.BelowPhreaticLevelMean,
                    BelowPhreaticLevelCoefficientOfVariation = properties.BelowPhreaticLevelCoefficientOfVariation,
                    CohesionMean = properties.CohesionMean,
                    CohesionCoefficientOfVariation = properties.CohesionCoefficientOfVariation,
                    FrictionAngleMean = properties.FrictionAngleMean,
                    FrictionAngleCoefficientOfVariation = properties.FrictionAngleCoefficientOfVariation,
                    StrengthIncreaseExponentMean = properties.StrengthIncreaseExponentMean,
                    StrengthIncreaseExponentCoefficientOfVariation = properties.StrengthIncreaseExponentCoefficientOfVariation,
                    ShearStrengthRatioMean = properties.ShearStrengthRatioMean,
                    ShearStrengthRatioCoefficientOfVariation = properties.ShearStrengthRatioCoefficientOfVariation,
                    PopMean = properties.PopMean,
                    PopCoefficientOfVariation = properties.PopCoefficientOfVariation
                });

            SetDesignVariables(props);
            return props;
        }

        private static void SetDesignVariables(MacroStabilityInwardsSoilLayerPropertiesUnderSurfaceLine props)
        {
            props.AbovePhreaticLevelDesignVariable = MacroStabilityInwardsSemiProbabilisticDesignValueFactory.GetAbovePhreaticLevel(props).GetDesignValue();
            props.BelowPhreaticLevelDesignVariable = MacroStabilityInwardsSemiProbabilisticDesignValueFactory.GetBelowPhreaticLevel(props).GetDesignValue();
            props.CohesionDesignVariable = MacroStabilityInwardsSemiProbabilisticDesignValueFactory.GetCohesion(props).GetDesignValue();
            props.FrictionAngleDesignVariable = MacroStabilityInwardsSemiProbabilisticDesignValueFactory.GetFrictionAngle(props).GetDesignValue();
            props.StrengthIncreaseExponentDesignVariable = MacroStabilityInwardsSemiProbabilisticDesignValueFactory.GetStrengthIncreaseExponent(props).GetDesignValue();
            props.ShearStrengthRatioDesignVariable = MacroStabilityInwardsSemiProbabilisticDesignValueFactory.GetShearStrengthRatio(props).GetDesignValue();
            props.PopDesignVariable = MacroStabilityInwardsSemiProbabilisticDesignValueFactory.GetPop(props).GetDesignValue();
        }

        private static Point2D[] RingToPoints(Ring ring)
        {
            return ring.Points.ToArray();
        }

        private static MacroStabilityInwardsSoilProfileUnderSurfaceLine GeometriesToIntersections(IEnumerable<TempSoilLayerGeometry> layerGeometries, IEnumerable<Point2D> surfaceLineGeometry)
        {
            var collection = new Collection<MacroStabilityInwardsSoilLayerUnderSurfaceLine>();

            IEnumerable<Point2D> surfaceLineGeometryArray = surfaceLineGeometry.ToArray();

            foreach (TempSoilLayerGeometry layer in layerGeometries)
            {
                foreach (Point2D[] soilLayerArea in GetSoilLayerWithSurfaceLineIntersection(surfaceLineGeometryArray, layer.OuterLoop))
                {
                    collection.Add(new MacroStabilityInwardsSoilLayerUnderSurfaceLine(soilLayerArea, ToUnderSurfaceLineProperties(layer.Properties)));
                }
            }

            return new MacroStabilityInwardsSoilProfileUnderSurfaceLine(collection);
        }

        private static TempSoilLayerGeometry As2DGeometry(MacroStabilityInwardsSoilLayer1D layer, MacroStabilityInwardsSoilProfile1D soilProfile, double minX, double maxX)
        {
            double top = layer.Top;
            double bottom = top - soilProfile.GetLayerThickness(layer);

            return new TempSoilLayerGeometry(new[]
            {
                new Point2D(minX, top),
                new Point2D(maxX, top),
                new Point2D(maxX, bottom),
                new Point2D(minX, bottom)
            }, layer.Properties);
        }

        private static IEnumerable<Point2D[]> GetSoilLayerWithSurfaceLineIntersection(IEnumerable<Point2D> surfaceLineGeometry, IEnumerable<Point2D> soilLayerGeometry)
        {
            return AdvancedMath2D.PolygonIntersectionWithPolygon(surfaceLineGeometry, soilLayerGeometry).Where(arr => arr.Length > 2);
        }

        private class TempSoilLayerGeometry
        {
            public TempSoilLayerGeometry(Point2D[] outerLoop, MacroStabilityInwardsSoilLayerProperties properties)
            {
                OuterLoop = outerLoop;
                Properties = properties;
                InnerLoops = Enumerable.Empty<Point2D[]>();
            }

            public Point2D[] OuterLoop { get; }
            public MacroStabilityInwardsSoilLayerProperties Properties { get; }
            public IEnumerable<Point2D[]> InnerLoops { get; }
        }
    }
}