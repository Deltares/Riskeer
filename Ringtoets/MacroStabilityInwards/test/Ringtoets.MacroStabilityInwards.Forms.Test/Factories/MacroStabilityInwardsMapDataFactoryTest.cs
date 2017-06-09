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

using System.Drawing;
using Core.Components.Gis.Data;
using Core.Components.Gis.Style;
using NUnit.Framework;
using Ringtoets.MacroStabilityInwards.Forms.Factories;

namespace Ringtoets.MacroStabilityInwards.Forms.Test.Factories
{
    [TestFixture]
    public class MacroStabilityInwardsMapDataFactoryTest
    {
        [Test]
        public void CreateSurfaceLinesMapData_ReturnsEmptyChartLineDataWithDefaultStyling()
        {
            // Call
            MapLineData data = MacroStabilityInwardsMapDataFactory.CreateSurfaceLinesMapData();

            // Assert
            Assert.IsEmpty(data.Features);
            Assert.AreEqual("Profielschematisaties", data.Name);
            AssertEqualStyle(data.Style, Color.DarkSeaGreen, 2, LineDashStyle.Solid);
            Assert.AreEqual("Naam", data.SelectedMetaDataAttribute);
        }

        [Test]
        public void CreateStochasticSoilModelsMapData_ReturnsEmptyChartLineDataWithDefaultStyling()
        {
            // Call
            MapLineData data = MacroStabilityInwardsMapDataFactory.CreateStochasticSoilModelsMapData();

            // Assert
            Assert.IsEmpty(data.Features);
            Assert.AreEqual("Stochastische ondergrondmodellen", data.Name);
            AssertEqualStyle(data.Style, Color.FromArgb(70, Color.SaddleBrown), 5, LineDashStyle.Solid);
            Assert.AreEqual("Naam", data.SelectedMetaDataAttribute);
        }

        private static void AssertEqualStyle(LineStyle lineStyle, Color color, int width, LineDashStyle style)
        {
            Assert.AreEqual(color, lineStyle.Color);
            Assert.AreEqual(width, lineStyle.Width);
            Assert.AreEqual(style, lineStyle.DashStyle);
        }
    }
}