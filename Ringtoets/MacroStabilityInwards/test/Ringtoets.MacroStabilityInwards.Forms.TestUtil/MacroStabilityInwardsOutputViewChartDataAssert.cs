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
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Components.Chart.Data;
using NUnit.Framework;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.MacroStabilityInwards.Primitives;

namespace Ringtoets.MacroStabilityInwards.Forms.TestUtil
{
    /// <summary>
    /// Class for asserting chart data in the macro stability
    /// inwards output view.
    /// </summary>
    public static class MacroStabilityInwardsOutputViewChartDataAssert
    {
        private const int soilProfileIndex = 0;
        private const int surfaceLineIndex = 1;
        private const int surfaceLevelInsideIndex = 2;
        private const int ditchPolderSideIndex = 3;
        private const int bottomDitchPolderSideIndex = 4;
        private const int bottomDitchDikeSideIndex = 5;
        private const int ditchDikeSideIndex = 6;
        private const int dikeToeAtPolderIndex = 7;
        private const int shoulderTopInsideIndex = 8;
        private const int shoulderBaseInsideIndex = 9;
        private const int dikeTopAtPolderIndex = 10;
        private const int dikeToeAtRiverIndex = 11;
        private const int dikeTopAtRiverIndex = 12;
        private const int surfaceLevelOutsideIndex = 13;
        private const int waternetZonesExtremeIndex = 14;
        private const int waternetZonesDailyIndex = 15;
        private const int tangentLinesIndex = 16;
        private const int leftGridIndex = 17;
        private const int rightGridIndex = 18;
        private const int slicesIndex = 19;
        private const int slipPlaneIndex = 20;
        private const int activeCircleRadiusIndex = 21;
        private const int passiveCircleRadiusIndex = 22;
        private const int nrOfChartData = 23;

        /// <summary>
        /// Asserts whether <paramref name="actual"/> corresponds to the input of <paramref name="calculationScenario"/>.
        /// </summary>
        /// <param name="calculationScenario">The original <see cref="MacroStabilityInwardsCalculationScenario"/>.</param>
        /// <param name="actual">The actual <see cref="ChartDataCollection"/>.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="actual"/>
        /// does not correspond to <paramref name="calculationScenario"/>.</exception>
        public static void AssertInputChartData(MacroStabilityInwardsCalculationScenario calculationScenario, ChartDataCollection actual)
        {
            Assert.AreEqual(nrOfChartData, actual.Collection.Count());
            MacroStabilityInwardsViewChartDataAssert.AssertSurfaceLineChartData(calculationScenario.InputParameters.SurfaceLine, actual.Collection.ElementAt(surfaceLineIndex));
            MacroStabilityInwardsViewChartDataAssert.AssertSoilProfileChartData(calculationScenario.InputParameters.SoilProfileUnderSurfaceLine,
                                                                                calculationScenario.InputParameters.StochasticSoilProfile.SoilProfile.Name,
                                                                                true,
                                                                                actual.Collection.ElementAt(soilProfileIndex));

            AssertWaternetChartData(calculationScenario.InputParameters.WaternetExtreme,
                                    (ChartDataCollection) actual.Collection.ElementAt(waternetZonesExtremeIndex));
            AssertWaternetChartData(calculationScenario.InputParameters.WaternetDaily,
                                    (ChartDataCollection) actual.Collection.ElementAt(waternetZonesDailyIndex));
        }

        /// <summary>
        /// Asserts whether <paramref name="actual"/> corresponds to the output of <paramref name="calculationScenario"/>.
        /// </summary>
        /// <param name="calculationScenario">The original <see cref="MacroStabilityInwardsCalculationScenario"/>.</param>
        /// <param name="actual">The actual <see cref="ChartDataCollection"/>.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="actual"/>
        /// does not correspond to <paramref name="calculationScenario"/>.</exception>
        public static void AssertOutputChartData(MacroStabilityInwardsCalculationScenario calculationScenario, ChartDataCollection actual)
        {
            Assert.AreEqual(nrOfChartData, actual.Collection.Count());

            MacroStabilityInwardsViewChartDataAssert.AssertGridChartData(calculationScenario.Output.SlipPlane.LeftGrid,
                                                                         (ChartPointData) actual.Collection.ElementAt(leftGridIndex));
            MacroStabilityInwardsViewChartDataAssert.AssertGridChartData(calculationScenario.Output.SlipPlane.RightGrid,
                                                                         (ChartPointData) actual.Collection.ElementAt(rightGridIndex));

            AssertTangentLinesChartData(calculationScenario.Output.SlipPlane.TangentLines, calculationScenario.InputParameters.SurfaceLine,
                                        (ChartMultipleLineData) actual.Collection.ElementAt(tangentLinesIndex));

            AssertSlicesChartData(calculationScenario.Output.SlidingCurve.Slices,
                                  (ChartMultipleAreaData) actual.Collection.ElementAt(slicesIndex));

            AssertSlipPlaneChartData(calculationScenario.Output.SlidingCurve,
                                     (ChartLineData) actual.Collection.ElementAt(slipPlaneIndex));

            AssertCircleRadiusChartData(calculationScenario.Output.SlidingCurve.Slices.First().TopLeftPoint,
                                        calculationScenario.Output.SlidingCurve.LeftCircle,
                                        (ChartLineData) actual.Collection.ElementAt(activeCircleRadiusIndex));
            AssertCircleRadiusChartData(calculationScenario.Output.SlidingCurve.Slices.Last().TopRightPoint,
                                        calculationScenario.Output.SlidingCurve.RightCircle,
                                        (ChartLineData) actual.Collection.ElementAt(passiveCircleRadiusIndex));
        }

        /// <summary>
        /// Asserts whether <paramref name="chartDataCollection"/> contains no waternet chart data.
        /// </summary>
        /// <param name="waternet">The original <see cref="MacroStabilityInwardsWaternet"/>.</param>
        /// <param name="chartDataCollection">The actual <see cref="ChartData"/>.</param>
        /// <exception cref="AssertionException">Thrown when a waternet layer is present.</exception>
        public static void AssertWaternetChartData(MacroStabilityInwardsWaternet waternet, ChartDataCollection chartDataCollection)
        {
            MacroStabilityInwardsViewChartDataAssert.AssertWaternetChartData(waternet,
                                                                             false,
                                                                             chartDataCollection);
        }

        /// <summary>
        /// Asserts whether <paramref name="chartDataCollection"/> contains empty output data.
        /// </summary>
        /// <param name="chartDataCollection">The actual <see cref="ChartData"/>.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="chartDataCollection"/>
        /// is not empty.</exception>
        public static void AssertEmptyOutputChartData(ChartDataCollection chartDataCollection)
        {
            ChartData[] chartDataArray = chartDataCollection.Collection.ToArray();

            Assert.AreEqual(nrOfChartData, chartDataArray.Length);
            var tangentLinesData = (ChartMultipleLineData) chartDataArray[tangentLinesIndex];
            var leftGridData = (ChartPointData) chartDataArray[leftGridIndex];
            var rightGridData = (ChartPointData) chartDataArray[rightGridIndex];
            var slicesData = (ChartMultipleAreaData) chartDataArray[slicesIndex];
            var slipPlaneData = (ChartLineData) chartDataArray[slipPlaneIndex];
            var activeCircleRadiusData = (ChartLineData) chartDataArray[activeCircleRadiusIndex];
            var passiveCircleRadiusData = (ChartLineData) chartDataArray[passiveCircleRadiusIndex];

            CollectionAssert.IsEmpty(tangentLinesData.Lines);
            CollectionAssert.IsEmpty(leftGridData.Points);
            CollectionAssert.IsEmpty(rightGridData.Points);
            CollectionAssert.IsEmpty(slicesData.Areas);
            CollectionAssert.IsEmpty(slipPlaneData.Points);
            CollectionAssert.IsEmpty(activeCircleRadiusData.Points);
            CollectionAssert.IsEmpty(passiveCircleRadiusData.Points);

            Assert.AreEqual("Tangentlijnen", tangentLinesData.Name);
            Assert.AreEqual("Linker grid", leftGridData.Name);
            Assert.AreEqual("Rechter grid", rightGridData.Name);
            Assert.AreEqual("Lamellen", slicesData.Name);
            Assert.AreEqual("Glijvlak", slipPlaneData.Name);
            Assert.AreEqual("Radius actieve cirkel", activeCircleRadiusData.Name);
            Assert.AreEqual("Radius passieve cirkel", passiveCircleRadiusData.Name);
        }

        /// <summary>
        /// Asserts whether <paramref name="chartDataCollection"/> contains empty data,
        /// empty soil layer chart data and empty waternet chart data.
        /// </summary>
        /// <param name="chartDataCollection">The actual <see cref="ChartData"/>.</param>
        /// <exception cref="AssertionException">Thrown when:
        /// <list type="bullet">
        /// <item><paramref name="chartDataCollection"/> is not empty;</item>
        /// <item>a soil layer chart data contains data;</item>
        /// <item>a waternet layer chart data contains data.</item>
        /// </list>
        /// </exception>
        public static void AssertEmptyChartDataWithEmptySoilLayerAndWithWaternetChartData(ChartDataCollection chartDataCollection)
        {
            var waternetExtremeData = (ChartDataCollection) chartDataCollection.Collection.ElementAt(waternetZonesExtremeIndex);
            var waternetDailyData = (ChartDataCollection) chartDataCollection.Collection.ElementAt(waternetZonesDailyIndex);

            CollectionAssert.IsNotEmpty(waternetExtremeData.Collection);
            CollectionAssert.IsNotEmpty(waternetDailyData.Collection);

            Assert.IsFalse(waternetExtremeData.Collection.Any(c => c.HasData));
            Assert.IsFalse(waternetDailyData.Collection.Any(c => c.HasData));

            AssertEmptyChartDataWithEmptySoilLayerChartData(chartDataCollection);
        }

        /// <summary>
        /// Asserts whether <paramref name="chartDataCollection"/> contains empty data and
        /// empty soil layer chart data and no waternet chart data.
        /// </summary>
        /// <param name="chartDataCollection">The actual <see cref="ChartData"/>.</param>
        /// <exception cref="AssertionException">Thrown when:
        /// <list type="bullet">
        /// <item><paramref name="chartDataCollection"/> is not empty;</item>
        /// <item>a soil layer chart data contains data;</item>
        /// <item>a waternet layer is present.</item>
        /// </list>
        /// </exception>
        public static void AssertEmptyChartDataWithEmptySoilLayerAndEmptyWaternetChartData(ChartDataCollection chartDataCollection)
        {
            AssertEmptyWaternetChartData(chartDataCollection);
            AssertEmptyChartDataWithEmptySoilLayerChartData(chartDataCollection);
        }

        /// <summary>
        /// Asserts whether <paramref name="chartDataCollection"/> contains no waternet chart data.
        /// </summary>
        /// <param name="chartDataCollection">The actual <see cref="ChartData"/>.</param>
        /// <exception cref="AssertionException">Thrown when a waternet layer is present.</exception>
        public static void AssertEmptyWaternetChartData(ChartDataCollection chartDataCollection)
        {
            MacroStabilityInwardsViewChartDataAssert.AssertEmptyWaternetChartData(chartDataCollection,
                                                                                  waternetZonesExtremeIndex,
                                                                                  waternetZonesDailyIndex);
        }

        /// <summary>
        /// Asserts whether <paramref name="actual"/> corresponds to <paramref name="tangentLines"/>
        /// and <paramref name="surfaceLine"/>.
        /// </summary>
        /// <param name="tangentLines">The original tangent line Y-coordinates.</param>
        /// <param name="surfaceLine">The original surface line.</param>
        /// <param name="actual">The actual <see cref="ChartMultipleLineData"/>.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="actual"/>
        /// does not correspond to <paramref name="tangentLines"/> and <paramref name="surfaceLine"/>.
        /// </exception>
        private static void AssertTangentLinesChartData(IEnumerable<RoundedDouble> tangentLines,
                                                        MacroStabilityInwardsSurfaceLine surfaceLine,
                                                        ChartMultipleLineData actual)
        {
            CollectionAssert.IsNotEmpty(actual.Lines);
            RoundedDouble[] tangentLinesArray = tangentLines.ToArray();
            for (var i = 0; i < tangentLinesArray.Length; i++)
            {
                var expectedPoints = new[]
                {
                    new Point2D(surfaceLine.LocalGeometry.First().X, tangentLinesArray[i]),
                    new Point2D(surfaceLine.LocalGeometry.Last().X, tangentLinesArray[i])
                };
                CollectionAssert.AreEqual(expectedPoints, actual.Lines.ElementAt(i));
            }
        }

        /// <summary>
        /// Asserts whether <paramref name="actual"/> corresponds to <paramref name="slices"/>.
        /// </summary>
        /// <param name="slices">The original slices.</param>
        /// <param name="actual">The actual <see cref="ChartMultipleAreaData"/>.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="actual"/>
        /// does not correspond to <paramref name="slices"/>.</exception>
        private static void AssertSlicesChartData(IEnumerable<MacroStabilityInwardsSlice> slices, ChartMultipleAreaData actual)
        {
            MacroStabilityInwardsSlice[] macroStabilityInwardsSlices = slices.ToArray();
            CollectionAssert.IsNotEmpty(macroStabilityInwardsSlices);
            for (var i = 0; i < macroStabilityInwardsSlices.Length; i++)
            {
                MacroStabilityInwardsSlice slice = macroStabilityInwardsSlices[i];
                var expectedPoints = new[]
                {
                    slice.TopLeftPoint,
                    slice.TopRightPoint,
                    slice.BottomRightPoint,
                    slice.BottomLeftPoint
                };
                CollectionAssert.AreEqual(expectedPoints, actual.Areas.ElementAt(i));
            }
        }

        /// <summary>
        /// Asserts whether <paramref name="actual"/> corresponds to <paramref name="startingPoint"/>
        /// and <paramref name="slidingCircle"/>.
        /// </summary>
        /// <param name="startingPoint">The point to use for the start of the line</param>
        /// <param name="slidingCircle">The circle to use for the end of the line.</param>
        /// <param name="actual">The actual <see cref="ChartLineData"/>.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="actual"/>
        /// does not correspond to the specified start and end points.</exception>
        private static void AssertCircleRadiusChartData(Point2D startingPoint,
                                                        MacroStabilityInwardsSlidingCircle slidingCircle,
                                                        ChartLineData actual)
        {
            var points = new[]
            {
                slidingCircle.Center,
                startingPoint
            };

            CollectionAssert.AreEqual(points, actual.Points);
        }

        /// <summary>
        /// Asserts whether <paramref name="chartDataCollection"/> contains empty data and
        /// empty soil layer chart data.
        /// </summary>
        /// <param name="chartDataCollection">The actual <see cref="ChartData"/>.</param>
        /// <exception cref="AssertionException">Thrown when:
        /// <list type="bullet">
        /// <item><paramref name="chartDataCollection"/> is not empty;</item>
        /// <item>a soil layer chart data contains data.</item>
        /// </list>
        /// </exception>
        private static void AssertEmptyChartDataWithEmptySoilLayerChartData(ChartDataCollection chartDataCollection)
        {
            var soilProfileData = (ChartDataCollection) chartDataCollection.Collection.ElementAt(soilProfileIndex);
            CollectionAssert.IsNotEmpty(soilProfileData.Collection);
            Assert.IsFalse(soilProfileData.Collection.Any(c => c.HasData));
            AssertEmptyChartData(chartDataCollection);
        }

        /// <summary>
        /// Asserts whether <paramref name="actual"/> corresponds to <paramref name="original"/>.
        /// </summary>
        /// <param name="original">The original <see cref="MacroStabilityInwardsSlidingCurve"/>.</param>
        /// <param name="actual">The actual <see cref="ChartLineData"/>.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="actual"/>
        /// does not correspond to <paramref name="original"/>.</exception>
        private static void AssertSlipPlaneChartData(MacroStabilityInwardsSlidingCurve original, ChartLineData actual)
        {
            CollectionAssert.IsNotEmpty(original.Slices);
            List<Point2D> expectedPoints = original.Slices.Select(slice => slice.BottomLeftPoint).OrderBy(x => x.X).ToList();
            expectedPoints.Add(original.Slices.Last().BottomRightPoint);
            CollectionAssert.AreEqual(expectedPoints, actual.Points);
        }

        /// <summary>
        /// Asserts whether <paramref name="chartDataCollection"/> contains empty data.
        /// </summary>
        /// <param name="chartDataCollection">The actual <see cref="ChartData"/>.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="chartDataCollection"/>
        /// is not empty.</exception>
        private static void AssertEmptyChartData(ChartDataCollection chartDataCollection)
        {
            Assert.AreEqual("Resultaat", chartDataCollection.Name);

            ChartData[] chartDataArray = chartDataCollection.Collection.ToArray();

            Assert.AreEqual(nrOfChartData, chartDataArray.Length);

            var surfaceLineData = (ChartLineData) chartDataArray[surfaceLineIndex];
            var surfaceLevelInsideData = (ChartPointData) chartDataArray[surfaceLevelInsideIndex];
            var ditchPolderSideData = (ChartPointData) chartDataArray[ditchPolderSideIndex];
            var bottomDitchPolderSideData = (ChartPointData) chartDataArray[bottomDitchPolderSideIndex];
            var bottomDitchDikeSideData = (ChartPointData) chartDataArray[bottomDitchDikeSideIndex];
            var ditchDikeSideData = (ChartPointData) chartDataArray[ditchDikeSideIndex];
            var dikeToeAtPolderData = (ChartPointData) chartDataArray[dikeToeAtPolderIndex];
            var shoulderTopInsideData = (ChartPointData) chartDataArray[shoulderTopInsideIndex];
            var shoulderBaseInsideData = (ChartPointData) chartDataArray[shoulderBaseInsideIndex];
            var dikeTopAtPolderData = (ChartPointData) chartDataArray[dikeTopAtPolderIndex];
            var dikeToeAtRiverData = (ChartPointData) chartDataArray[dikeToeAtRiverIndex];
            var dikeTopAtRiverData = (ChartPointData) chartDataArray[dikeTopAtRiverIndex];
            var surfaceLevelOutsideData = (ChartPointData) chartDataArray[surfaceLevelOutsideIndex];

            CollectionAssert.IsEmpty(surfaceLineData.Points);
            CollectionAssert.IsEmpty(surfaceLevelInsideData.Points);
            CollectionAssert.IsEmpty(ditchPolderSideData.Points);
            CollectionAssert.IsEmpty(bottomDitchPolderSideData.Points);
            CollectionAssert.IsEmpty(bottomDitchDikeSideData.Points);
            CollectionAssert.IsEmpty(ditchDikeSideData.Points);
            CollectionAssert.IsEmpty(dikeToeAtPolderData.Points);
            CollectionAssert.IsEmpty(shoulderTopInsideData.Points);
            CollectionAssert.IsEmpty(shoulderBaseInsideData.Points);
            CollectionAssert.IsEmpty(dikeTopAtPolderData.Points);
            CollectionAssert.IsEmpty(dikeToeAtRiverData.Points);
            CollectionAssert.IsEmpty(dikeTopAtRiverData.Points);
            CollectionAssert.IsEmpty(surfaceLevelOutsideData.Points);

            Assert.AreEqual("Profielschematisatie", surfaceLineData.Name);
            Assert.AreEqual("Maaiveld binnenwaarts", surfaceLevelInsideData.Name);
            Assert.AreEqual("Insteek sloot polderzijde", ditchPolderSideData.Name);
            Assert.AreEqual("Slootbodem polderzijde", bottomDitchPolderSideData.Name);
            Assert.AreEqual("Slootbodem dijkzijde", bottomDitchDikeSideData.Name);
            Assert.AreEqual("Insteek sloot dijkzijde", ditchDikeSideData.Name);
            Assert.AreEqual("Teen dijk binnenwaarts", dikeToeAtPolderData.Name);
            Assert.AreEqual("Kruin binnenberm", shoulderTopInsideData.Name);
            Assert.AreEqual("Insteek binnenberm", shoulderBaseInsideData.Name);
            Assert.AreEqual("Kruin binnentalud", dikeTopAtPolderData.Name);
            Assert.AreEqual("Teen dijk buitenwaarts", dikeToeAtRiverData.Name);
            Assert.AreEqual("Kruin buitentalud", dikeTopAtRiverData.Name);
            Assert.AreEqual("Maaiveld buitenwaarts", surfaceLevelOutsideData.Name);

            AssertEmptyOutputChartData(chartDataCollection);
        }
    }
}