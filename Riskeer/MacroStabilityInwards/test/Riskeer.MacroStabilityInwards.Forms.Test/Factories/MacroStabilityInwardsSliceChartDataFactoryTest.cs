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

using System.Drawing;
using Core.Components.Chart.Data;
using Core.Components.Chart.Styles;
using NUnit.Framework;
using Riskeer.MacroStabilityInwards.Forms.Factories;

namespace Riskeer.MacroStabilityInwards.Forms.Test.Factories
{
    [TestFixture]
    public class MacroStabilityInwardsSliceChartDataFactoryTest
    {
        [Test]
        public void CreateSlicesChartData_ReturnsChartMultipleAreaData()
        {
            // Call
            ChartMultipleAreaData data = MacroStabilityInwardsSliceChartDataFactory.CreateSlicesChartData();

            // Assert
            CollectionAssert.IsEmpty(data.Areas);
            Assert.AreEqual("Lamellen", data.Name);
            AssertEqualStyle(data.Style, Color.Empty, Color.DarkGreen, 2, true);
        }

        [Test]
        public void CreateSliceParametersChartDataCollection_ReturnsEmptyChartDataCollection()
        {
            // Call
            ChartDataCollection data = MacroStabilityInwardsSliceChartDataFactory.CreateSliceParametersChartDataCollection();

            // Assert
            CollectionAssert.IsEmpty(data.Collection);
            Assert.AreEqual("Uitvoer per lamel", data.Name);
        }

        [Test]
        public void CreateCohesionChartData_ReturnsChartMultipleAreaData()
        {
            // Call
            ChartMultipleAreaData data = MacroStabilityInwardsSliceChartDataFactory.CreateCohesionChartData();

            // Assert
            CollectionAssert.IsEmpty(data.Areas);
            Assert.AreEqual("Cohesie", data.Name);
            Assert.IsFalse(data.IsVisible);
            AssertEqualStyle(data.Style, Color.FromArgb(150, 255, 0, 0), Color.Black, 1, true);
        }

        [Test]
        public void CreateEffectiveStressChartData_ReturnsChartMultipleAreaData()
        {
            // Call
            ChartMultipleAreaData data = MacroStabilityInwardsSliceChartDataFactory.CreateEffectiveStressChartData();

            // Assert
            CollectionAssert.IsEmpty(data.Areas);
            Assert.AreEqual("Effectieve spanning", data.Name);
            Assert.IsTrue(data.IsVisible);
            AssertEqualStyle(data.Style, Color.FromArgb(150, 255, 0, 0), Color.Black, 1, true);
        }

        [Test]
        public void CreateEffectiveStressDailyChartData_ReturnsChartMultipleAreaData()
        {
            // Call
            ChartMultipleAreaData data = MacroStabilityInwardsSliceChartDataFactory.CreateEffectiveStressDailyChartData();

            // Assert
            CollectionAssert.IsEmpty(data.Areas);
            Assert.AreEqual("Effectieve spanning (dagelijks)", data.Name);
            Assert.IsFalse(data.IsVisible);
            AssertEqualStyle(data.Style, Color.FromArgb(150, 255, 0, 0), Color.Black, 1, true);
        }

        [Test]
        public void CreateTotalPorePressureChartData_ReturnsChartMultipleAreaData()
        {
            // Call
            ChartMultipleAreaData data = MacroStabilityInwardsSliceChartDataFactory.CreateTotalPorePressureChartData();

            // Assert
            CollectionAssert.IsEmpty(data.Areas);
            Assert.AreEqual("Totale waterspanning", data.Name);
            Assert.IsFalse(data.IsVisible);
            AssertEqualStyle(data.Style, Color.FromArgb(150, 255, 0, 0), Color.Black, 1, true);
        }

        [Test]
        public void CreateWeightChartData_ReturnsChartMultipleAreaData()
        {
            // Call
            ChartMultipleAreaData data = MacroStabilityInwardsSliceChartDataFactory.CreateWeightChartData();

            // Assert
            CollectionAssert.IsEmpty(data.Areas);
            Assert.AreEqual("Gewicht", data.Name);
            Assert.IsFalse(data.IsVisible);
            AssertEqualStyle(data.Style, Color.FromArgb(150, 255, 0, 0), Color.Black, 1, true);
        }

        [Test]
        public void CreatePiezometricPorePressureChartData_ReturnsChartMultipleAreaData()
        {
            // Call
            ChartMultipleAreaData data = MacroStabilityInwardsSliceChartDataFactory.CreatePiezometricPorePressureChartData();

            // Assert
            CollectionAssert.IsEmpty(data.Areas);
            Assert.AreEqual("Piezometrische waterspanning", data.Name);
            Assert.IsFalse(data.IsVisible);
            AssertEqualStyle(data.Style, Color.FromArgb(150, 255, 0, 0), Color.Black, 1, true);
        }

        [Test]
        public void CreatePorePressureChartData_ReturnsChartMultipleAreaData()
        {
            // Call
            ChartMultipleAreaData data = MacroStabilityInwardsSliceChartDataFactory.CreatePorePressureChartData();

            // Assert
            CollectionAssert.IsEmpty(data.Areas);
            Assert.AreEqual("Waterspanning op maaiveld", data.Name);
            Assert.IsFalse(data.IsVisible);
            AssertEqualStyle(data.Style, Color.FromArgb(150, 255, 0, 0), Color.Black, 1, true);
        }

        [Test]
        public void CreateVerticalPorePressureChartData_ReturnsChartMultipleAreaData()
        {
            // Call
            ChartMultipleAreaData data = MacroStabilityInwardsSliceChartDataFactory.CreateVerticalPorePressureChartData();

            // Assert
            CollectionAssert.IsEmpty(data.Areas);
            Assert.AreEqual("Verticale waterspanning op maaiveld", data.Name);
            Assert.IsFalse(data.IsVisible);
            AssertEqualStyle(data.Style, Color.FromArgb(150, 255, 0, 0), Color.Black, 1, true);
        }

        [Test]
        public void CreateHorizontalPorePressureChartData_ReturnsChartMultipleAreaData()
        {
            // Call
            ChartMultipleAreaData data = MacroStabilityInwardsSliceChartDataFactory.CreateHorizontalPorePressureChartData();

            // Assert
            CollectionAssert.IsEmpty(data.Areas);
            Assert.AreEqual("Horizontale waterspanning op maaiveld", data.Name);
            Assert.IsFalse(data.IsVisible);
            AssertEqualStyle(data.Style, Color.FromArgb(150, 255, 0, 0), Color.Black, 1, true);
        }

        [Test]
        public void CreateOverConsolidationRatioChartData_ReturnsChartMultipleAreaData()
        {
            // Call
            ChartMultipleAreaData data = MacroStabilityInwardsSliceChartDataFactory.CreateOverConsolidationRatioChartData();

            // Assert
            CollectionAssert.IsEmpty(data.Areas);
            Assert.AreEqual("OCR", data.Name);
            Assert.IsFalse(data.IsVisible);
            AssertEqualStyle(data.Style, Color.FromArgb(150, 255, 0, 0), Color.Black, 1, true);
        }

        [Test]
        public void CreatePopChartData_ReturnsChartMultipleAreaData()
        {
            // Call
            ChartMultipleAreaData data = MacroStabilityInwardsSliceChartDataFactory.CreatePopChartData();

            // Assert
            CollectionAssert.IsEmpty(data.Areas);
            Assert.AreEqual("POP", data.Name);
            Assert.IsFalse(data.IsVisible);
            AssertEqualStyle(data.Style, Color.FromArgb(150, 255, 0, 0), Color.Black, 1, true);
        }

        [Test]
        public void CreateNormalStressChartData_ReturnsChartMultipleAreaData()
        {
            // Call
            ChartMultipleAreaData data = MacroStabilityInwardsSliceChartDataFactory.CreateNormalStressChartData();

            // Assert
            CollectionAssert.IsEmpty(data.Areas);
            Assert.AreEqual("Normaalspanning", data.Name);
            Assert.IsFalse(data.IsVisible);
            AssertEqualStyle(data.Style, Color.FromArgb(150, 255, 0, 0), Color.Black, 1, true);
        }

        [Test]
        public void CreateShearStressChartData_ReturnsChartMultipleAreaData()
        {
            // Call
            ChartMultipleAreaData data = MacroStabilityInwardsSliceChartDataFactory.CreateShearStressChartData();

            // Assert
            CollectionAssert.IsEmpty(data.Areas);
            Assert.AreEqual("Schuifspanning", data.Name);
            Assert.IsFalse(data.IsVisible);
            AssertEqualStyle(data.Style, Color.FromArgb(150, 255, 0, 0), Color.Black, 1, true);
        }

        [Test]
        public void CreateLoadStressChartData_ReturnsChartMultipleAreaData()
        {
            // Call
            ChartMultipleAreaData data = MacroStabilityInwardsSliceChartDataFactory.CreateLoadStressChartData();

            // Assert
            CollectionAssert.IsEmpty(data.Areas);
            Assert.AreEqual("Spanning belasting", data.Name);
            Assert.IsFalse(data.IsVisible);
            AssertEqualStyle(data.Style, Color.FromArgb(150, 255, 0, 0), Color.Black, 1, true);
        }

        private static void AssertEqualStyle(ChartAreaStyle areaStyle, Color fillColor, Color strokeColor, int width, bool isEditable)
        {
            Assert.AreEqual(fillColor, areaStyle.FillColor);
            Assert.AreEqual(strokeColor, areaStyle.StrokeColor);
            Assert.AreEqual(width, areaStyle.StrokeThickness);
            Assert.AreEqual(isEditable, areaStyle.IsEditable);
        }
    }
}