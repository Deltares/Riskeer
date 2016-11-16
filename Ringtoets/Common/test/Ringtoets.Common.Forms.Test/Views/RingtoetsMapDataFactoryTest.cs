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
using Ringtoets.Common.Forms.Views;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;
using RingtoetsCommonDataResources = Ringtoets.Common.Data.Properties.Resources;

namespace Ringtoets.Common.Forms.Test.Views
{
    [TestFixture]
    public class RingtoetsMapDataFactoryTest
    {
        [Test]
        public void CreateReferenceLineMapData_ReturnsEmptyMapLineDataWithDefaultStyling()
        {
            // Call
            MapLineData data = RingtoetsMapDataFactory.CreateReferenceLineMapData();

            // Assert
            Assert.IsEmpty(data.Features);
            Assert.AreEqual(RingtoetsCommonDataResources.ReferenceLine_DisplayName, data.Name);
            AssertEqualStyle(data.Style, Color.Red, 3, DashStyle.Solid);
        }

        [Test]
        public void CreateFailureMechanismSectionsMapData_ReturnsEmptyChartLineDataWithDefaultStyling()
        {
            // Call
            MapLineData data = RingtoetsMapDataFactory.CreateFailureMechanismSectionsMapData();

            // Assert
            Assert.IsEmpty(data.Features);
            Assert.AreEqual(RingtoetsCommonFormsResources.FailureMechanism_Sections_DisplayName, data.Name);
            AssertEqualStyle(data.Style, Color.Khaki, 3, DashStyle.Dot);
        }

        [Test]
        public void CreateFailureMechanismSectionsStartPointMapData_ReturnsEmptyChartPointDataWithDefaultStyling()
        {
            // Call
            MapPointData data = RingtoetsMapDataFactory.CreateFailureMechanismSectionsStartPointMapData();

            // Assert
            Assert.IsEmpty(data.Features);
            Assert.AreEqual(GetSectionPointDisplayName(RingtoetsCommonFormsResources.FailureMechanismSections_StartPoints_DisplayName), data.Name);
            AssertEqualStyle(data.Style, Color.DarkKhaki, 15, PointSymbol.Triangle);
        }

        [Test]
        public void CreateFailureMechanismSectionsEndPointMapData_ReturnsEmptyChartPointDataWithDefaultStyling()
        {
            // Call
            MapPointData data = RingtoetsMapDataFactory.CreateFailureMechanismSectionsEndPointMapData();

            // Assert
            Assert.IsEmpty(data.Features);
            Assert.AreEqual(GetSectionPointDisplayName(RingtoetsCommonFormsResources.FailureMechanismSections_EndPoints_DisplayName), data.Name);
            AssertEqualStyle(data.Style, Color.DarkKhaki, 15, PointSymbol.Triangle);
        }

        [Test]
        public void CreateHydraulicBoundaryDatabaseMapData_ReturnsEmptyMapPointDataWithDefaultStyling()
        {
            // Call
            MapPointData data = RingtoetsMapDataFactory.CreateHydraulicBoundaryDatabaseMapData();

            // Assert
            Assert.IsEmpty(data.Features);
            Assert.AreEqual(RingtoetsCommonDataResources.HydraulicBoundaryConditions_DisplayName, data.Name);
            Assert.IsTrue(data.ShowLabels);
            AssertEqualStyle(data.Style, Color.DarkBlue, 6, PointSymbol.Circle);
        }

        [Test]
        public void CreateDikeProfileMapData_ReturnsEmptyMapPointDataWithDefaultStyling()
        {
            // Call
            MapLineData data = RingtoetsMapDataFactory.CreateDikeProfileMapData();

            // Assert
            Assert.IsEmpty(data.Features);
            Assert.AreEqual(RingtoetsCommonFormsResources.DikeProfiles_DisplayName, data.Name);
            AssertEqualStyle(data.Style, Color.DarkSeaGreen, 2, DashStyle.Solid);
        }

        private static string GetSectionPointDisplayName(string name)
        {
            return string.Format("{0} ({1})",
                                 RingtoetsCommonFormsResources.FailureMechanism_Sections_DisplayName,
                                 name);
        }

        private static void AssertEqualStyle(LineStyle lineStyle, Color color, int width, DashStyle style)
        {
            Assert.AreEqual(color, lineStyle.Color);
            Assert.AreEqual(width, lineStyle.Width);
            Assert.AreEqual(style, lineStyle.Style);
        }

        private static void AssertEqualStyle(PointStyle pointStyle, Color color, int width, PointSymbol symbol)
        {
            Assert.AreEqual(color, pointStyle.Color);
            Assert.AreEqual(width, pointStyle.Size);
            Assert.AreEqual(symbol, pointStyle.Symbol);
        }
    }
}