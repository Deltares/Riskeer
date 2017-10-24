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
using Core.Components.Chart.Data;
using NUnit.Framework;
using Ringtoets.MacroStabilityInwards.Data.SoilProfile;
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
        private const int leftGridIndex = 14;
        private const int rightGridIndex = 15;
        private const int waternetZonesExtremeIndex = 16;
        private const int waternetZonesDailyIndex = 17;
        private const int nrOfChartData = 18;

        /// <summary>
        /// Asserts whether <paramref name="actual"/> corresponds to <paramref name="surfaceLine"/>
        /// and <paramref name="stochasticSoilProfile"/>.
        /// </summary>
        /// <param name="surfaceLine">The original <see cref="MacroStabilityInwardsSurfaceLine"/>.</param>
        /// <param name="stochasticSoilProfile">The original <see cref="MacroStabilityInwardsStochasticSoilProfile"/>.</param>
        /// <param name="actual">The actual <see cref="ChartDataCollection"/>.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="actual"/>
        /// does not correspond to <paramref name="surfaceLine"/> or<paramref name="stochasticSoilProfile"/>.</exception>
        public static void AssertChartData(MacroStabilityInwardsSurfaceLine surfaceLine,
                                           MacroStabilityInwardsStochasticSoilProfile stochasticSoilProfile,
                                           ChartDataCollection actual)
        {
            Assert.AreEqual(nrOfChartData, actual.Collection.Count());
            MacroStabilityInwardsViewChartDataAssert.AssertSurfaceLineChartData(surfaceLine, actual.Collection.ElementAt(surfaceLineIndex));
            MacroStabilityInwardsViewChartDataAssert.AssertSoilProfileChartData(stochasticSoilProfile, actual.Collection.ElementAt(soilProfileIndex), true);
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

            List<ChartData> chartDatasList = chartDataCollection.Collection.ToList();

            Assert.AreEqual(nrOfChartData, chartDatasList.Count);

            var surfaceLineData = (ChartLineData) chartDatasList[surfaceLineIndex];
            var soilProfileData = (ChartDataCollection) chartDatasList[soilProfileIndex];
            var surfaceLevelInsideData = (ChartPointData) chartDatasList[surfaceLevelInsideIndex];
            var ditchPolderSideData = (ChartPointData) chartDatasList[ditchPolderSideIndex];
            var bottomDitchPolderSideData = (ChartPointData) chartDatasList[bottomDitchPolderSideIndex];
            var bottomDitchDikeSideData = (ChartPointData) chartDatasList[bottomDitchDikeSideIndex];
            var ditchDikeSideData = (ChartPointData) chartDatasList[ditchDikeSideIndex];
            var dikeToeAtPolderData = (ChartPointData) chartDatasList[dikeToeAtPolderIndex];
            var shoulderTopInsideData = (ChartPointData) chartDatasList[shoulderTopInsideIndex];
            var shoulderBaseInsideData = (ChartPointData) chartDatasList[shoulderBaseInsideIndex];
            var dikeTopAtPolderData = (ChartPointData) chartDatasList[dikeTopAtPolderIndex];
            var dikeToeAtRiverData = (ChartPointData) chartDatasList[dikeToeAtRiverIndex];
            var dikeTopAtRiverData = (ChartPointData) chartDatasList[dikeTopAtRiverIndex];
            var surfaceLevelOutsideData = (ChartPointData) chartDatasList[surfaceLevelOutsideIndex];
            var leftGridOutsideData = (ChartPointData) chartDatasList[leftGridIndex];
            var rightGridOutsideData = (ChartPointData) chartDatasList[rightGridIndex];
            var waternetZonesExtremeData = (ChartDataCollection) chartDatasList[waternetZonesExtremeIndex];
            var waternetZonesDailyData = (ChartDataCollection) chartDatasList[waternetZonesDailyIndex];

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
            Assert.AreEqual("Linker grid", leftGridOutsideData.Name);
            Assert.AreEqual("Rechter grid", rightGridOutsideData.Name);
            Assert.AreEqual("Zones extreem", waternetZonesExtremeData.Name);
            Assert.AreEqual("Zones dagelijks", waternetZonesDailyData.Name);
        }
    }
}