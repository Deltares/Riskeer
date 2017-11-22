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
using Ringtoets.Common.IO.Exceptions;
using Ringtoets.Common.IO.SoilProfile;
using Ringtoets.Piping.IO.Properties;
using Ringtoets.Piping.Primitives;
using RingtoetsCommonIOResources = Ringtoets.Common.IO.Properties.Resources;

namespace Ringtoets.Piping.IO.SoilProfiles
{
    /// <summary>
    /// Transforms generic <see cref="SoilLayerBase"/> into <see cref="PipingSoilLayer"/>.
    /// </summary>
    internal static class PipingSoilLayerTransformer
    {
        /// <summary>
        /// Transforms the generic <paramref name="soilLayer"/> into a <see cref="PipingSoilLayer"/>.
        /// </summary>
        /// <param name="soilLayer">The soil layer to use in the transformation.</param>
        /// <returns>A new <see cref="PipingSoilLayer"/> based on the given data.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="soilLayer"/> is <c>null</c>.</exception>
        /// <exception cref="ImportedDataTransformException">Thrown when transformation would not result
        /// in a valid transformed instance.</exception>
        public static PipingSoilLayer Transform(SoilLayer1D soilLayer)
        {
            if (soilLayer == null)
            {
                throw new ArgumentNullException(nameof(soilLayer));
            }

            ValidateStochasticParameters(soilLayer);

            var pipingSoilLayer = new PipingSoilLayer(soilLayer.Top)
            {
                IsAquifer = TransformIsAquifer(soilLayer.IsAquifer, soilLayer.MaterialName),
                MaterialName = soilLayer.MaterialName,
                Color = SoilLayerColorConverter.Convert(soilLayer.Color)
            };

            SetStochasticParameters(pipingSoilLayer, soilLayer);

            return pipingSoilLayer;
        }

        /// <summary>
        /// Transforms the generic <paramref name="soilLayer"/> into one or more <see cref="PipingSoilLayer"/>.
        /// </summary>
        /// <param name="soilLayer">The soil layer to use in the transformation.</param>
        /// <param name="atX">The 1D intersection of the profile.</param>
        /// <param name="bottom">The bottom of the soil layer.</param>
        /// <returns>A collection of <see cref="PipingSoilLayer"/> based on the given data.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="soilLayer"/> is <c>null</c>.</exception>
        /// <exception cref="ImportedDataTransformException">Thrown when transformation would not result
        /// in a valid transformed instance.</exception>
        public static IEnumerable<PipingSoilLayer> Transform(SoilLayer2D soilLayer, double atX, out double bottom)
        {
            bottom = double.MaxValue;
            var soilLayers = new Collection<PipingSoilLayer>();
            Transform(soilLayer, atX, soilLayers, ref bottom);
            return soilLayers;
        }

        /// <summary>
        /// Transforms the generic <paramref name="soilLayer"/> into one or more <see cref="PipingSoilLayer"/>.
        /// </summary>
        /// <param name="soilLayer">The soil layer to use in the transformation.</param>
        /// <param name="atX">The 1D intersection of the profile.</param>
        /// <param name="soilLayers">The collection of transformed piping soil layers to add the
        /// transformation to.</param>
        /// <param name="bottom">The bottom of the soil layer.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="soilLayer"/> is <c>null</c>.</exception>
        /// <exception cref="ImportedDataTransformException">Thrown when transformation would not result
        /// in a valid transformed instance.</exception>
        private static void Transform(SoilLayer2D soilLayer, double atX, ICollection<PipingSoilLayer> soilLayers, ref double bottom)
        {
            if (soilLayer == null)
            {
                throw new ArgumentNullException(nameof(soilLayer));
            }

            ValidateStochasticParameters(soilLayer);

            if (soilLayer.OuterLoop == null)
            {
                return;
            }

            string soilLayerName = soilLayer.MaterialName;
            double[] outerLoopIntersectionHeights = GetLoopIntersectionHeights(soilLayer.OuterLoop.Segments, atX, soilLayerName).ToArray();

            if (!outerLoopIntersectionHeights.Any())
            {
                return;
            }

            IEnumerable<IEnumerable<double>> innerLoopsIntersectionHeights = soilLayer.NestedLayers.Select(l => GetLoopIntersectionHeights(l.OuterLoop.Segments,
                                                                                                                                           atX,
                                                                                                                                           soilLayerName));
            IEnumerable<Tuple<double, double>> innerLoopIntersectionHeightPairs = GetOrderedStartAndEndPairsIn1D(innerLoopsIntersectionHeights).ToList();
            IEnumerable<Tuple<double, double>> outerLoopIntersectionHeightPairs = GetOrderedStartAndEndPairsIn1D(outerLoopIntersectionHeights).ToList();

            double currentBottom = outerLoopIntersectionHeightPairs.First().Item1;
            var heights = new List<double>();
            heights.AddRange(innerLoopIntersectionHeightPairs.Where(p => p.Item1 >= currentBottom).Select(p => p.Item1));
            heights.AddRange(outerLoopIntersectionHeightPairs.Select(p => p.Item2));

            foreach (double height in heights.Where(height => !innerLoopIntersectionHeightPairs.Any(tuple => HeightInInnerLoop(tuple, height))))
            {
                var pipingSoilLayer = new PipingSoilLayer(height)
                {
                    IsAquifer = TransformIsAquifer(soilLayer.IsAquifer, soilLayerName),
                    MaterialName = soilLayer.MaterialName,
                    Color = SoilLayerColorConverter.Convert(soilLayer.Color)
                };

                SetStochasticParameters(pipingSoilLayer, soilLayer);

                soilLayers.Add(pipingSoilLayer);
            }

            bottom = currentBottom < bottom ? currentBottom : bottom;

            foreach (SoilLayer2D nestedLayer in soilLayer.NestedLayers)
            {
                Transform(nestedLayer, atX, soilLayers, ref bottom);
            }
        }

        /// <summary>
        /// Validates whether the values of the distribution and shift for the stochastic parameters 
        /// are correct for creating a <see cref="PipingSoilLayer"/>.
        /// </summary>
        /// <exception cref="ImportedDataTransformException">Thrown when any of the distributions of the
        /// stochastic parameters is not defined as lognormal or is shifted when it should not be.</exception>
        private static void ValidateStochasticParameters(SoilLayerBase soilLayer)
        {
            try
            {
                DistributionHelper.ValidateShiftedLogNormalDistribution(
                    soilLayer.BelowPhreaticLevelDistributionType,
                    Resources.SoilLayer_BelowPhreaticLevelDistribution_DisplayName);
                DistributionHelper.ValidateLogNormalDistribution(
                    soilLayer.DiameterD70DistributionType,
                    soilLayer.DiameterD70Shift,
                    Resources.SoilLayer_DiameterD70Distribution_DisplayName);
                DistributionHelper.ValidateLogNormalDistribution(
                    soilLayer.PermeabilityDistributionType,
                    soilLayer.PermeabilityShift,
                    Resources.SoilLayer_PermeabilityDistribution_DisplayName);
            }
            catch (ImportedDataTransformException e)
            {
                string errorMessage = string.Format(RingtoetsCommonIOResources.Transform_Error_occurred_when_transforming_SoilLayer_0_errorMessage_1,
                                                    soilLayer.MaterialName,
                                                    e.Message);
                throw new ImportedDataTransformException(errorMessage, e);
            }
        }

        /// <summary>
        /// Sets the values of the stochastic parameters for the given <see cref="PipingSoilLayer"/>.
        /// </summary>
        /// <param name="pipingSoilLayer">The <see cref="PipingSoilLayer"/> to set the property values for.</param>
        /// <param name="soilLayer">The <see cref="SoilLayerBase"/> to get the properties from.</param>
        /// <remarks>This method does not perform validation. Use <see cref="ValidateStochasticParameters"/> to 
        /// verify whether the distributions for the stochastic parameters are correctly defined.</remarks>
        private static void SetStochasticParameters(PipingSoilLayer pipingSoilLayer, SoilLayerBase soilLayer)
        {
            pipingSoilLayer.BelowPhreaticLevelMean = soilLayer.BelowPhreaticLevelMean;
            pipingSoilLayer.BelowPhreaticLevelDeviation = soilLayer.BelowPhreaticLevelDeviation;
            pipingSoilLayer.BelowPhreaticLevelShift = soilLayer.BelowPhreaticLevelShift;
            pipingSoilLayer.DiameterD70Mean = soilLayer.DiameterD70Mean;
            pipingSoilLayer.DiameterD70CoefficientOfVariation = soilLayer.DiameterD70CoefficientOfVariation;
            pipingSoilLayer.PermeabilityMean = soilLayer.PermeabilityMean;
            pipingSoilLayer.PermeabilityCoefficientOfVariation = soilLayer.PermeabilityCoefficientOfVariation;
        }

        private static bool HeightInInnerLoop(Tuple<double, double> tuple, double height)
        {
            return height <= tuple.Item2 && height > tuple.Item1;
        }

        private static IEnumerable<Tuple<double, double>> GetOrderedStartAndEndPairsIn1D(IEnumerable<IEnumerable<double>> innerLoopsIntersectionPoints)
        {
            var result = new Collection<Tuple<double, double>>();
            foreach (IEnumerable<double> innerLoopIntersectionPoints in innerLoopsIntersectionPoints)
            {
                foreach (Tuple<double, double> tuple in GetOrderedStartAndEndPairsIn1D(innerLoopIntersectionPoints))
                {
                    result.Add(tuple);
                }
            }
            return result;
        }

        private static Collection<Tuple<double, double>> GetOrderedStartAndEndPairsIn1D(IEnumerable<double> innerLoopIntersectionPoints)
        {
            var result = new Collection<Tuple<double, double>>();
            List<double> orderedHeights = innerLoopIntersectionPoints.OrderBy(v => v).ToList();
            for (var i = 0; i < orderedHeights.Count; i = i + 2)
            {
                double first = orderedHeights[i];
                double second = orderedHeights[i + 1];
                result.Add(Tuple.Create(first, second));
            }
            return result;
        }

        /// <summary>
        /// Gets a <see cref="Collection{T}"/> of heights where the <paramref name="loop"/> intersects the 
        /// vertical line at <paramref name="atX"/>.
        /// </summary>
        /// <param name="loop">The sequence of <see cref="Segment2D"/> which together create a loop.</param>
        /// <param name="atX">The point on the x-axis where the vertical line is constructed do determine intersections with.</param>
        /// <param name="soilLayerName">The name of the soil layer.</param>
        /// <returns>A <see cref="Collection{T}"/> of <see cref="double"/>, representing the height at which the 
        /// <paramref name="loop"/> intersects the vertical line at <paramref name="atX"/>.</returns>
        /// <exception cref="ImportedDataTransformException">Thrown when a segment is vertical at <see cref="atX"/> and thus
        /// no deterministic intersection points can be determined.</exception>
        private static IEnumerable<double> GetLoopIntersectionHeights(IEnumerable<Segment2D> loop,
                                                                      double atX,
                                                                      string soilLayerName)
        {
            Segment2D[] segment2Ds = loop.ToArray();
            if (segment2Ds.Any(segment => IsVerticalAtX(segment, atX)))
            {
                string message = CreateExceptionMessage(soilLayerName,
                                                        string.Format(Resources.Error_Can_not_determine_1D_profile_with_vertical_segments_at_X_0_, atX));
                throw new ImportedDataTransformException(message);
            }
            return Math2D.SegmentsIntersectionWithVerticalLine(segment2Ds, atX).Select(p => p.Y);
        }

        private static string CreateExceptionMessage(string soilLayerName, string errorMessage)
        {
            return string.Format(RingtoetsCommonIOResources.Transform_Error_occurred_when_transforming_SoilLayer_0_errorMessage_1,
                                 soilLayerName,
                                 errorMessage);
        }

        private static bool IsVerticalAtX(Segment2D segment, double atX)
        {
            return segment.FirstPoint.X.Equals(atX) && segment.IsVertical();
        }

        /// <summary>
        /// Transforms a <see cref="double"/> to a <see cref="bool"/> for the 
        /// <see cref="PipingSoilLayer.IsAquifer"/>.
        /// </summary>
        /// <param name="isAquifer">The value to transform.</param>
        /// <param name="soilLayerName">The name of the soil layer.</param>
        /// <returns>A <see cref="bool"/> based on <paramref name="isAquifer"/>.</returns>
        /// <exception cref="ImportedDataTransformException">Thrown when
        /// <paramref name="isAquifer"/> could not be transformed.</exception>
        private static bool TransformIsAquifer(double? isAquifer, string soilLayerName)
        {
            try
            {
                return SoilLayerIsAquiferConverter.Convert(isAquifer);
            }
            catch (NotSupportedException e)
            {
                string message = CreateExceptionMessage(soilLayerName,
                                                        string.Format(RingtoetsCommonIOResources.Transform_Invalid_value_ParameterName_0,
                                                                      RingtoetsCommonIOResources.SoilLayerData_IsAquifer_DisplayName));
                throw new ImportedDataTransformException(message, e);
            }
        }
    }
}