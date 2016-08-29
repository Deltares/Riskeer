// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using System.Drawing.Drawing2D;
using Core.Components.Gis.Data;
using Core.Components.Gis.Style;
using NUnit.Framework;
using Ringtoets.Piping.Forms.Properties;
using Ringtoets.Piping.Forms.Views;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.Piping.Forms.Test.Views
{
    [TestFixture]
    public class PipingMapDataFactoryTest
    {
        [Test]
        public void CreateSurfaceLinesMapData_ReturnsEmptyChartLineDataWithDefaultStyling()
        {
            // Call
            MapLineData data = PipingMapDataFactory.CreateSurfaceLinesMapData();

            // Assert
            Assert.IsEmpty(data.Features);
            Assert.AreEqual(Resources.PipingSurfaceLinesCollection_DisplayName, data.Name);
            AssertEqualStyle(data.Style, Color.DarkSeaGreen, 2, DashStyle.Solid);
        }

        [Test]
        public void CreateStochasticSoilModelsMapData_ReturnsEmptyChartLineDataWithDefaultStyling()
        {
            // Call
            MapLineData data = PipingMapDataFactory.CreateStochasticSoilModelsMapData();

            // Assert
            Assert.IsEmpty(data.Features);
            Assert.AreEqual(Resources.StochasticSoilModelCollection_DisplayName, data.Name);
            AssertEqualStyle(data.Style, Color.FromArgb(70, Color.SaddleBrown), 5, DashStyle.Solid);
        }

        [Test]
        public void CreateFailureMechanismSectionsMapData_ReturnsEmptyChartLineDataWithDefaultStyling()
        {
            // Call
            MapLineData data = PipingMapDataFactory.CreateFailureMechanismSectionsMapData();

            // Assert
            Assert.IsEmpty(data.Features);
            Assert.AreEqual(RingtoetsCommonFormsResources.FailureMechanism_Sections_DisplayName, data.Name);
            AssertEqualStyle(data.Style, Color.Khaki, 3, DashStyle.Dot);
        }

        [Test]
        public void CreateFailureMechanismSectionsStartPointMapData_ReturnsEmptyChartPointDataWithDefaultStyling()
        {
            // Call
            MapPointData data = PipingMapDataFactory.CreateFailureMechanismSectionsStartPointMapData();

            // Assert
            Assert.IsEmpty(data.Features);
            Assert.AreEqual(GetSectionPointDisplayName(RingtoetsCommonFormsResources.FailureMechanismSections_StartPoints_DisplayName), data.Name);
            AssertEqualStyle(data.Style, Color.DarkKhaki, 15, PointSymbol.Triangle);
        }

        [Test]
        public void CreateFailureMechanismSectionsEndPointMapData_ReturnsEmptyChartPointDataWithDefaultStyling()
        {
            // Call
            MapPointData data = PipingMapDataFactory.CreateFailureMechanismSectionsEndPointMapData();

            // Assert
            Assert.IsEmpty(data.Features);
            Assert.AreEqual(GetSectionPointDisplayName(RingtoetsCommonFormsResources.FailureMechanismSections_EndPoints_DisplayName), data.Name);
            AssertEqualStyle(data.Style, Color.DarkKhaki, 15, PointSymbol.Triangle);
        }

        private void AssertEqualStyle(PointStyle pointStyle, Color color, int width, PointSymbol symbol)
        {
            Assert.AreEqual(color, pointStyle.Color);
            Assert.AreEqual(width, pointStyle.Size);
            Assert.AreEqual(symbol, pointStyle.Symbol);
        }

        private static void AssertEqualStyle(LineStyle lineStyle, Color color, int width, DashStyle style)
        {
            Assert.AreEqual(color, lineStyle.Color);
            Assert.AreEqual(width, lineStyle.Width);
            Assert.AreEqual(style, lineStyle.Style);
        }

        private static string GetSectionPointDisplayName(string name)
        {
            return string.Format("{0} ({1})",
                                 RingtoetsCommonFormsResources.FailureMechanism_Sections_DisplayName,
                                 name);
        }
    }
}