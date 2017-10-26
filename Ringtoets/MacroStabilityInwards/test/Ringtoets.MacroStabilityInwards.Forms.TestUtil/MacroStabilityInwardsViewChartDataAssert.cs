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

            Assert.AreEqual(original.Points.Length, surfaceLineChartData.Points.Length);
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
        public static void AssertSoilProfileChartData(IMacroStabilityInwardsSoilProfileUnderSurfaceLine soilProfileUnderSurface, string expectedName, bool mapDataShouldContainAreas, ChartData actual)
        {
            Assert.IsInstanceOf<ChartDataCollection>(actual);
            var soilProfileChartData = (ChartDataCollection) actual;

            IMacroStabilityInwardsSoilLayer2D[] layers = soilProfileUnderSurface?.Layers != null
                                                             ? MacroStabilityInwardsSoilProfile2DLayersHelper.GetLayersRecursively(soilProfileUnderSurface.Layers).ToArray()
                                                             : new IMacroStabilityInwardsSoilLayer2D[0];
            int expectedLayerCount = layers.Length;

            Assert.AreEqual(expectedLayerCount, soilProfileChartData.Collection.Count());
            Assert.AreEqual(expectedName, soilProfileChartData.Name);

            string[] expectedSoilLayerNames = layers.Select((l, i) => $"{i + 1} {l.Data.MaterialName}").Reverse().ToArray();

            for (var i = 0; i < expectedLayerCount; i++)
            {
                var chartMultipleAreaData = soilProfileChartData.Collection.ElementAt(i) as ChartMultipleAreaData;

                Assert.IsNotNull(chartMultipleAreaData);
                Assert.AreEqual(expectedSoilLayerNames[i], chartMultipleAreaData.Name);
                Assert.AreEqual(mapDataShouldContainAreas, chartMultipleAreaData.Areas.Any());
            }
        }

        /// <summary>
        /// Asserts whether <paramref name="actual"/> corresponds to <paramref name="original"/>.
        /// </summary>
        /// <param name="original">The original <see cref="MacroStabilityInwardsWaternet"/>.</param>
        /// <param name="actual">The actual <see cref="ChartDataCollection"/>.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="actual"/>
        /// does not correspond to <paramref name="original"/>.</exception>
        public static void AssertWaternetChartData(MacroStabilityInwardsWaternet original, ChartDataCollection actual)
        {
            ChartData[] waternetChartData = actual.Collection.ToArray();
            MacroStabilityInwardsWaternetLine[] waternetLines = original.WaternetLines.ToArray();
            MacroStabilityInwardsPhreaticLine[] phreaticLines = original.PhreaticLines.ToArray();

            Assert.AreEqual(waternetLines.Length + phreaticLines.Length, waternetChartData.Length);

            for (var i = 0; i < waternetChartData.Length; i++)
            {
                if (i < phreaticLines.Length)
                {
                    ChartLineData phreaticLineChartData = (ChartLineData) waternetChartData[i];
                    Assert.AreEqual(phreaticLines[i].Name, phreaticLineChartData.Name);
                    Assert.AreEqual(phreaticLines[i].Geometry, phreaticLineChartData.Points);
                }
                else
                {
                    ChartMultipleAreaData waternetLineChartData = (ChartMultipleAreaData) waternetChartData[i];
                    MacroStabilityInwardsWaternetLine waternetLine = waternetLines[i - waternetLines.Length];
                    Assert.AreEqual(waternetLine.Name, waternetLineChartData.Name);
                    Assert.IsTrue(waternetLineChartData.HasData);
                }
            }
        }
    }
}