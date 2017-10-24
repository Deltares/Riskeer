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
        /// Asserts whether <paramref name="actual"/> corresponds to <paramref name="original"/>.
        /// </summary>
        /// <param name="original">The original <see cref="MacroStabilityInwardsStochasticSoilProfile"/>.</param>
        /// <param name="actual">The actual <see cref="ChartData"/>.</param>
        /// <param name="mapDataShouldContainAreas">Indicator whether areas are present.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="actual"/>
        /// does not correspond to <paramref name="original"/>.</exception>
        public static void AssertSoilProfileChartData(MacroStabilityInwardsStochasticSoilProfile original, ChartData actual, bool mapDataShouldContainAreas)
        {
            Assert.IsInstanceOf<ChartDataCollection>(actual);
            var soilProfileChartData = (ChartDataCollection) actual;

            int expectedLayerCount = original.SoilProfile.Layers.Count();
            Assert.AreEqual(expectedLayerCount + 1, soilProfileChartData.Collection.Count());
            Assert.AreEqual(original.SoilProfile.Name, soilProfileChartData.Name);

            string[] expectedSoilLayerNames = original.SoilProfile.Layers.Select((l, i) => $"{i + 1} {l.Data.MaterialName}").Reverse().ToArray();

            for (var i = 0; i < expectedLayerCount; i++)
            {
                var chartMultipleAreaData = soilProfileChartData.Collection.ElementAt(i) as ChartMultipleAreaData;

                Assert.IsNotNull(chartMultipleAreaData);
                Assert.AreEqual(expectedSoilLayerNames[i], chartMultipleAreaData.Name);
                Assert.AreEqual(mapDataShouldContainAreas, chartMultipleAreaData.Areas.Any());
            }

            var holesMultipleAreaData = soilProfileChartData.Collection.Last() as ChartMultipleAreaData;
            Assert.IsNotNull(holesMultipleAreaData);
            Assert.AreEqual("Binnenringen", holesMultipleAreaData.Name);
            Assert.IsFalse(holesMultipleAreaData.Areas.Any());
        }
    }
}