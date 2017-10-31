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

using System.Linq;
using Core.Components.Chart.Data;
using NUnit.Framework;
using Ringtoets.MacroStabilityInwards.Data;

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
        private const int leftGridIndex = 16;
        private const int rightGridIndex = 17;
        private const int nrOfChartData = 18;

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

            MacroStabilityInwardsViewChartDataAssert.AssertWaternetChartData(calculationScenario.InputParameters.WaternetExtreme,
                                                                             (ChartDataCollection) actual.Collection.ElementAt(waternetZonesExtremeIndex));
            MacroStabilityInwardsViewChartDataAssert.AssertWaternetChartData(calculationScenario.InputParameters.WaternetDaily,
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

            MacroStabilityInwardsViewChartDataAssert.AssertGridChartData(calculationScenario.Output.SlipPlane.LeftGrid, (ChartPointData) actual.Collection.ElementAt(leftGridIndex));
            MacroStabilityInwardsViewChartDataAssert.AssertGridChartData(calculationScenario.Output.SlipPlane.RightGrid, (ChartPointData) actual.Collection.ElementAt(rightGridIndex));
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
            var leftGridData = (ChartPointData)chartDataArray[leftGridIndex];
            var rightGridData = (ChartPointData)chartDataArray[rightGridIndex];

            CollectionAssert.IsEmpty(leftGridData.Points);
            CollectionAssert.IsEmpty(rightGridData.Points);
            
            Assert.AreEqual("Linker grid", leftGridData.Name);
            Assert.AreEqual("Rechter grid", rightGridData.Name);
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
            var waternetExtremeData = (ChartDataCollection) chartDataCollection.Collection.ElementAt(waternetZonesExtremeIndex);
            var waternetDailyData = (ChartDataCollection) chartDataCollection.Collection.ElementAt(waternetZonesDailyIndex);

            CollectionAssert.IsEmpty(waternetExtremeData.Collection);
            CollectionAssert.IsEmpty(waternetDailyData.Collection);

            Assert.AreEqual("Zones extreem", waternetExtremeData.Name);
            Assert.AreEqual("Zones dagelijks", waternetDailyData.Name);
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