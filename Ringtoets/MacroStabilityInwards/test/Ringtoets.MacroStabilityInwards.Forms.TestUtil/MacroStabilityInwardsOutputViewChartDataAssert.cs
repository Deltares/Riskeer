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
    /// Class for asserting chart data.
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
        private const int nrOfChartData = 14;

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
            AssertSurfaceLineChartData(surfaceLine, actual.Collection.ElementAt(surfaceLineIndex));
            AssertSoilProfileChartData(stochasticSoilProfile, actual.Collection.ElementAt(soilProfileIndex), true);
        }

        /// <summary>
        /// Asserts whether <paramref name="chartDataCollection"/> contains empty data.
        /// </summary>
        /// <param name="chartDataCollection">The actual <see cref="ChartData"/>.</param>
        /// <param name="soilProfileEmpty">Indicator whether the soil profile chart data
        /// should be empty.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="chartDataCollection"/>
        /// is not empty.</exception>
        public static void AssertEmptyChartData(ChartDataCollection chartDataCollection, bool soilProfileEmpty)
        {
            Assert.AreEqual("Resultaat", chartDataCollection.Name);

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

            if (soilProfileEmpty)
            {
                CollectionAssert.IsEmpty(soilProfileData.Collection);
            }
            else
            {
                Assert.IsFalse(soilProfileData.Collection.Any(c => c.HasData));
            }
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
        }

        /// <summary>
        /// Asserts whether <paramref name="actual"/> corresponds to <paramref name="original"/>.
        /// </summary>
        /// <param name="original">The original <see cref="MacroStabilityInwardsSurfaceLine"/>.</param>
        /// <param name="actual">The actual <see cref="ChartData"/>.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="actual"/>
        /// does not correspond to <paramref name="original"/>.</exception>
        private static void AssertSurfaceLineChartData(MacroStabilityInwardsSurfaceLine original, ChartData actual)
        {
            Assert.IsInstanceOf<ChartLineData>(actual);
            var surfaceLineChartData = (ChartLineData) actual;

            Assert.AreEqual(original.Points.Length, surfaceLineChartData.Points.Length);
            CollectionAssert.AreEqual(original.LocalGeometry, surfaceLineChartData.Points);
            Assert.AreEqual(original.Name, actual.Name);
        }

        /// <summary>
        /// Asserts whether <paramref name="actual"/> corresponds to <paramref name="original"/>.
        /// </summary>
        /// <param name="original">The original <see cref="MacroStabilityInwardsStochasticSoilProfile"/>.</param>
        /// <param name="actual">The actual <see cref="ChartData"/>.</param>
        /// <param name="mapDataShouldContainAreas">Indicator whether areas are present.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="actual"/>
        /// does not correspond to <paramref name="original"/>.</exception>
        private static void AssertSoilProfileChartData(MacroStabilityInwardsStochasticSoilProfile original, ChartData actual, bool mapDataShouldContainAreas)
        {
            Assert.IsInstanceOf<ChartDataCollection>(actual);
            var soilProfileChartData = (ChartDataCollection) actual;

            int expectedLayerCount = original.SoilProfile.Layers.Count();
            Assert.AreEqual(expectedLayerCount + 1, soilProfileChartData.Collection.Count());
            Assert.AreEqual(original.SoilProfile.Name, soilProfileChartData.Name);

            string[] soilLayers = original.SoilProfile.Layers.Select((l, i) => $"{i + 1} {l.Data.MaterialName}").Reverse().ToArray();

            for (var i = 0; i < expectedLayerCount; i++)
            {
                var chartMultipleAreaData = soilProfileChartData.Collection.ElementAt(i) as ChartMultipleAreaData;

                Assert.IsNotNull(chartMultipleAreaData);
                Assert.AreEqual(soilLayers[i], chartMultipleAreaData.Name);
                Assert.AreEqual(mapDataShouldContainAreas, chartMultipleAreaData.Areas.Any());
            }
        }
    }
}