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
using Core.Common.Base.Geometry;
using Core.Components.Chart.Data;
using NUnit.Framework;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.MacroStabilityInwards.Data.SoilProfile;
using Ringtoets.MacroStabilityInwards.Primitives;

namespace Ringtoets.MacroStabilityInwards.Forms.TestUtil
{
    /// <summary>
    /// Class for asserting chart data in the macro stability
    /// inwards views.
    /// </summary>
    public static class MacroStabilityInwardsViewChartDataAssert
    {
        /// <summary>
        /// Asserts whether <paramref name="actual"/> corresponds to <paramref name="original"/>.
        /// </summary>
        /// <param name="original">The original <see cref="MacroStabilityInwardsSurfaceLine"/>.</param>
        /// <param name="actual">The actual <see cref="ChartData"/>.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="actual"/>
        /// does not correspond to <paramref name="original"/>.</exception>
        public static void AssertSurfaceLineChartData(MacroStabilityInwardsSurfaceLine original, ChartData actual)
        {
            Assert.IsInstanceOf<ChartLineData>(actual);
            var surfaceLineChartData = (ChartLineData) actual;

            Assert.AreEqual(original.Points.Count(), surfaceLineChartData.Points.Count());
            Assert.AreEqual(original.Name, actual.Name);
            CollectionAssert.AreEqual(original.LocalGeometry, surfaceLineChartData.Points);
        }

        /// <summary>
        /// Asserts whether <paramref name="actual"/> corresponds to <paramref name="soilProfileUnderSurface"/>,
        /// <paramref name="expectedName"/> and <paramref name="mapDataShouldContainAreas"/>.
        /// </summary>
        /// <param name="soilProfileUnderSurface">The <see cref="IMacroStabilityInwardsSoilProfileUnderSurfaceLine"/>
        /// that should be represented as series in <paramref name="actual"/>.</param>
        /// <param name="expectedName">The expected name of <paramref name="actual"/>.</param>
        /// <param name="mapDataShouldContainAreas">Indicator whether areas should be present.</param>
        /// <param name="actual">The actual <see cref="ChartData"/>.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="actual"/> does not correspond
        /// to <paramref name="soilProfileUnderSurface"/>, <paramref name="expectedName"/> or
        /// <paramref name="mapDataShouldContainAreas"/>.</exception>
        public static void AssertSoilProfileChartData(IMacroStabilityInwardsSoilProfileUnderSurfaceLine soilProfileUnderSurface,
                                                      string expectedName,
                                                      bool mapDataShouldContainAreas,
                                                      ChartData actual)
        {
            Assert.IsInstanceOf<ChartDataCollection>(actual);
            var soilProfileChartData = (ChartDataCollection) actual;

            MacroStabilityInwardsSoilLayer2D[] layers = MacroStabilityInwardsSoilProfile2DLayersHelper.GetLayersRecursively(soilProfileUnderSurface?.Layers).ToArray();
            int expectedLayerCount = layers.Length;

            Assert.AreEqual(expectedLayerCount, soilProfileChartData.Collection.Count());
            Assert.AreEqual(expectedName, soilProfileChartData.Name);

            for (var i = 0; i < expectedLayerCount; i++)
            {
                var chartMultipleAreaData = soilProfileChartData.Collection.ElementAt(i) as ChartMultipleAreaData;

                Assert.IsNotNull(chartMultipleAreaData);
                Assert.AreEqual(layers.ElementAt(i).Data.MaterialName, chartMultipleAreaData.Name);
                Assert.AreEqual(mapDataShouldContainAreas, chartMultipleAreaData.Areas.Any());
            }
        }

        /// <summary>
        /// Asserts whether <paramref name="actual"/> corresponds to <paramref name="original"/>.
        /// </summary>
        /// <param name="original">The original <see cref="MacroStabilityInwardsWaternet"/>.</param>
        /// <param name="expectedVisibility">The expected visibility of the chart data.</param>
        /// <param name="actual">The actual <see cref="ChartDataCollection"/>.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="actual"/>
        /// does not correspond to <paramref name="original"/>.</exception>
        public static void AssertWaternetChartData(MacroStabilityInwardsWaternet original, bool expectedVisibility, ChartDataCollection actual)
        {
            ChartData[] waternetChartData = actual.Collection.ToArray();
            MacroStabilityInwardsWaternetLine[] waternetLines = original.WaternetLines.ToArray();
            MacroStabilityInwardsPhreaticLine[] phreaticLines = original.PhreaticLines.ToArray();

            CollectionAssert.IsNotEmpty(waternetLines);
            CollectionAssert.IsNotEmpty(phreaticLines);

            Assert.AreEqual(waternetLines.Length + phreaticLines.Length, waternetChartData.Length);

            for (var i = 0; i < waternetChartData.Length; i++)
            {
                if (i < phreaticLines.Length)
                {
                    var phreaticLineChartData = (ChartLineData) waternetChartData[i];
                    Assert.AreEqual(phreaticLines[i].Name, phreaticLineChartData.Name);
                    Assert.AreEqual(phreaticLines[i].Geometry, phreaticLineChartData.Points);
                    Assert.AreEqual(expectedVisibility, phreaticLineChartData.IsVisible);
                }
                else
                {
                    var waternetLineChartData = (ChartMultipleAreaData) waternetChartData[i];
                    MacroStabilityInwardsWaternetLine waternetLine = waternetLines[i - waternetLines.Length];
                    Assert.AreEqual(waternetLine.Name, waternetLineChartData.Name);
                    Assert.IsTrue(waternetLineChartData.HasData);
                    Assert.AreEqual(expectedVisibility, waternetLineChartData.IsVisible);
                }
            }
        }

        /// <summary>
        /// Asserts whether <paramref name="actual"/> corresponds to <paramref name="original"/>.
        /// </summary>
        /// <param name="original">The original <see cref="MacroStabilityInwardsGrid"/>.</param>
        /// <param name="actual">The actual <see cref="ChartPointData"/>.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="actual"/>
        /// does not correspond to <paramref name="original"/>.</exception>
        public static void AssertGridChartData(MacroStabilityInwardsGrid original, ChartPointData actual)
        {
            var expectedPoints = new[]
            {
                new Point2D(original.XLeft, original.ZBottom),
                new Point2D(original.XRight, original.ZBottom),
                new Point2D(original.XLeft, original.ZTop),
                new Point2D(original.XRight, original.ZTop)
            };

            CollectionAssert.AreEqual(expectedPoints, actual.Points);
        }

        /// <summary>
        /// Asserts whether <paramref name="chartDataCollection"/> contains no waternet chart data.
        /// </summary>
        /// <param name="chartDataCollection">The actual <see cref="ChartData"/>.</param>
        /// <param name="waternetZonesExtremeIndex">The index of the waternet zones chart data under extreme
        /// circumstances in the <paramref name="chartDataCollection"/>.</param>
        /// <param name="waternetZonesDailyIndex">The index of the waternet zones chart data under daily
        /// circumstances in the <paramref name="chartDataCollection"/>.</param>
        /// <exception cref="AssertionException">Thrown when a waternet layer is present.</exception>
        public static void AssertEmptyWaternetChartData(ChartDataCollection chartDataCollection,
                                                        int waternetZonesExtremeIndex,
                                                        int waternetZonesDailyIndex)
        {
            var waternetExtremeData = (ChartDataCollection) chartDataCollection.Collection.ElementAt(waternetZonesExtremeIndex);
            var waternetDailyData = (ChartDataCollection) chartDataCollection.Collection.ElementAt(waternetZonesDailyIndex);

            CollectionAssert.IsEmpty(waternetExtremeData.Collection);
            CollectionAssert.IsEmpty(waternetDailyData.Collection);

            Assert.AreEqual("Zones extreem", waternetExtremeData.Name);
            Assert.AreEqual("Zones dagelijks", waternetDailyData.Name);
        }
    }
}