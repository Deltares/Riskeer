// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Ringtoets.MacroStabilityInwards.Primitives;

namespace Ringtoets.MacroStabilityInwards.Forms.TestUtil
{
    /// <summary>
    /// Class for asserting chart data in the macro stability
    /// inwards input view.
    /// </summary>
    public static class MacroStabilityInwardsInputViewChartDataAssert
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
        private const int tangentLinesIndex = 14;
        private const int leftGridIndex = 15;
        private const int rightGridIndex = 16;
        private const int waternetZonesExtremeIndex = 17;
        private const int waternetZonesDailyIndex = 18;
        private const int nrOfChartData = 19;

        /// <summary>
        /// Asserts whether <paramref name="actual"/> corresponds to <paramref name="calculationScenario"/>.
        /// </summary>
        /// <param name="calculationScenario">The original <see cref="MacroStabilityInwardsCalculationScenario"/>.</param>
        /// <param name="actual">The actual <see cref="ChartDataCollection"/>.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="actual"/>
        /// does not correspond to <paramref name="calculationScenario"/>.</exception>
        public static void AssertChartData(MacroStabilityInwardsCalculationScenario calculationScenario, ChartDataCollection actual)
        {
            Assert.AreEqual(nrOfChartData, actual.Collection.Count());
            MacroStabilityInwardsViewChartDataAssert.AssertSurfaceLineChartData(calculationScenario.InputParameters.SurfaceLine,
                                                                                actual.Collection.ElementAt(surfaceLineIndex));
            MacroStabilityInwardsViewChartDataAssert.AssertSoilProfileChartData(calculationScenario.InputParameters.SoilProfileUnderSurfaceLine,
                                                                                calculationScenario.InputParameters.StochasticSoilProfile.SoilProfile.Name,
                                                                                true,
                                                                                actual.Collection.ElementAt(soilProfileIndex));
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
        /// Asserts whether <paramref name="chartDataCollection"/> contains no waternet chart data.
        /// </summary>
        /// <param name="waternet">The original <see cref="MacroStabilityInwardsWaternet"/>.</param>
        /// <param name="chartDataCollection">The actual <see cref="ChartData"/>.</param>
        /// <exception cref="AssertionException">Thrown when a waternet layer is present.</exception>
        public static void AssertWaternetChartData(MacroStabilityInwardsWaternet waternet, ChartDataCollection chartDataCollection)
        {
            MacroStabilityInwardsViewChartDataAssert.AssertWaternetChartData(waternet,
                                                                             true,
                                                                             chartDataCollection);
        }

        /// <summary>
        /// Asserts whether <paramref name="chartDataCollection"/> contains empty data.
        /// </summary>
        /// <param name="chartDataCollection">The actual <see cref="ChartData"/>.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="chartDataCollection"/>
        /// is not empty.</exception>
        public static void AssertEmptyChartData(ChartDataCollection chartDataCollection)
        {
            Assert.AreEqual("Invoer", chartDataCollection.Name);

            ChartData[] chartDataArray = chartDataCollection.Collection.ToArray();

            Assert.AreEqual(nrOfChartData, chartDataArray.Length);

            var surfaceLineData = (ChartLineData) chartDataArray[surfaceLineIndex];
            var soilProfileData = (ChartDataCollection) chartDataArray[soilProfileIndex];
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
            var tangentLinesData = (ChartMultipleLineData) chartDataArray[tangentLinesIndex];
            var leftGridOutsideData = (ChartPointData) chartDataArray[leftGridIndex];
            var rightGridOutsideData = (ChartPointData) chartDataArray[rightGridIndex];
            var waternetZonesExtremeData = (ChartDataCollection) chartDataArray[waternetZonesExtremeIndex];
            var waternetZonesDailyData = (ChartDataCollection) chartDataArray[waternetZonesDailyIndex];

            CollectionAssert.IsEmpty(surfaceLineData.Points);
            CollectionAssert.IsEmpty(soilProfileData.Collection);
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
            CollectionAssert.IsEmpty(tangentLinesData.Lines);
            CollectionAssert.IsEmpty(leftGridOutsideData.Points);
            CollectionAssert.IsEmpty(rightGridOutsideData.Points);
            CollectionAssert.IsEmpty(waternetZonesExtremeData.Collection);
            CollectionAssert.IsEmpty(waternetZonesDailyData.Collection);

            Assert.AreEqual("Profielschematisatie", surfaceLineData.Name);
            Assert.AreEqual("Ondergrondschematisatie", soilProfileData.Name);
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
            Assert.AreEqual("Tangentlijnen", tangentLinesData.Name);
            Assert.AreEqual("Linker grid", leftGridOutsideData.Name);
            Assert.AreEqual("Rechter grid", rightGridOutsideData.Name);
            Assert.AreEqual("Zones extreem", waternetZonesExtremeData.Name);
            Assert.AreEqual("Zones dagelijks", waternetZonesDailyData.Name);
        }
    }
}