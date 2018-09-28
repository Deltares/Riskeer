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

using System;
using System.Windows.Forms;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.Controls.DataGrid;
using NUnit.Framework;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.MacroStabilityInwards.Data.TestUtil;
using Ringtoets.MacroStabilityInwards.Forms.Views;

namespace Ringtoets.MacroStabilityInwards.Forms.Test.Views
{
    [TestFixture]
    public class MacroStabilityInwardsSlicesTableTest
    {
        private const int nameColumnIndex = 0;
        private const int xCenterColumnIndex = 1;
        private const int zCenterBottomColumnIndex = 2;
        private const int widthColumnIndex = 3;
        private const int arcLengthColumnIndex = 4;
        private const int topAngleColumnIndex = 5;
        private const int bottomAngleColumnIndex = 6;
        private const int frictionAngleColumnIndex = 7;
        private const int cohesionColumnIndex = 8;
        private const int effectiveStressColumnIndex = 9;
        private const int effectiveStressDailyColumnIndex = 10;
        private const int totalPorePressureColumnIndex = 11;
        private const int weightColumnIndex = 12;
        private const int piezometricPorePressureColumnIndex = 13;
        private const int porePressureColumnIndex = 14;
        private const int verticalPorePressureColumnIndex = 15;
        private const int horizontalPorePressureColumnIndex = 16;
        private const int overConsolidationRatioColumnIndex = 17;
        private const int popColumnIndex = 18;
        private const int normalStressColumnIndex = 19;
        private const int shearStressColumnIndex = 20;
        private const int loadStressColumnIndex = 21;

        [Test]
        public void Constructor_InitializesWithColumns()
        {
            // Call
            using (var table = new MacroStabilityInwardsSlicesTable())
            {
                // Assert
                Assert.IsInstanceOf<DataGridViewControl>(table);

                DataGridViewColumn nameColumn = table.GetColumnFromIndex(nameColumnIndex);
                Assert.AreEqual("Naam", nameColumn.HeaderText);
                DataGridViewColumn xCenterColumn = table.GetColumnFromIndex(xCenterColumnIndex);
                Assert.AreEqual("X centrum\r\n[m]", xCenterColumn.HeaderText);
                DataGridViewColumn zCenterBottomColumn = table.GetColumnFromIndex(zCenterBottomColumnIndex);
                Assert.AreEqual("Z centrum bodem\r\n[m+NAP]", zCenterBottomColumn.HeaderText);
                DataGridViewColumn widthColumn = table.GetColumnFromIndex(widthColumnIndex);
                Assert.AreEqual("Breedte\r\n[m]", widthColumn.HeaderText);
                DataGridViewColumn arcLengthColumn = table.GetColumnFromIndex(arcLengthColumnIndex);
                Assert.AreEqual("Booglengte\r\n[m]", arcLengthColumn.HeaderText);
                DataGridViewColumn topAngleColumn = table.GetColumnFromIndex(topAngleColumnIndex);
                Assert.AreEqual("Tophoek\r\n[°]", topAngleColumn.HeaderText);
                DataGridViewColumn bottomAngleColumn = table.GetColumnFromIndex(bottomAngleColumnIndex);
                Assert.AreEqual("Bodemhoek\r\n[°]", bottomAngleColumn.HeaderText);
                DataGridViewColumn frictionAngleColumn = table.GetColumnFromIndex(frictionAngleColumnIndex);
                Assert.AreEqual("Wrijvingshoek\r\n[°]", frictionAngleColumn.HeaderText);
                DataGridViewColumn cohesionColumn = table.GetColumnFromIndex(cohesionColumnIndex);
                Assert.AreEqual("Cohesie\r\n[kN/m²]", cohesionColumn.HeaderText);
                DataGridViewColumn effectiveStressColumn = table.GetColumnFromIndex(effectiveStressColumnIndex);
                Assert.AreEqual("Effectieve spanning\r\n[kN/m²]", effectiveStressColumn.HeaderText);
                DataGridViewColumn effectiveStressDailyColumn = table.GetColumnFromIndex(effectiveStressDailyColumnIndex);
                Assert.AreEqual("Effectieve spanning\r\n(dagelijks)\r\n[kN/m²]", effectiveStressDailyColumn.HeaderText);
                DataGridViewColumn totalPorePressureColumn = table.GetColumnFromIndex(totalPorePressureColumnIndex);
                Assert.AreEqual("Totale\r\nwaterspanning\r\n[kN/m²]", totalPorePressureColumn.HeaderText);
                DataGridViewColumn weightColumn = table.GetColumnFromIndex(weightColumnIndex);
                Assert.AreEqual("Gewicht\r\n[kN/m]", weightColumn.HeaderText);
                DataGridViewColumn piezometricPorePressureColumn = table.GetColumnFromIndex(piezometricPorePressureColumnIndex);
                Assert.AreEqual("Piezometrische\r\nwaterspanning\r\n[kN/m²]", piezometricPorePressureColumn.HeaderText);
                DataGridViewColumn porePressureColumn = table.GetColumnFromIndex(porePressureColumnIndex);
                Assert.AreEqual("Waterspanning\r\nop maaiveld\r\n[kN/m²]", porePressureColumn.HeaderText);
                DataGridViewColumn verticalPorePressureColumn = table.GetColumnFromIndex(verticalPorePressureColumnIndex);
                Assert.AreEqual("Verticale waterspanning\r\nop maaiveld\r\n[kN/m²]", verticalPorePressureColumn.HeaderText);
                DataGridViewColumn horizontalPorePressureColumn = table.GetColumnFromIndex(horizontalPorePressureColumnIndex);
                Assert.AreEqual("Horizontale waterspanning\r\nop maaiveld\r\n[kN/m²]", horizontalPorePressureColumn.HeaderText);
                DataGridViewColumn overConsolidationRatioColumn = table.GetColumnFromIndex(overConsolidationRatioColumnIndex);
                Assert.AreEqual("OCR\r\n[-]", overConsolidationRatioColumn.HeaderText);
                DataGridViewColumn popColumn = table.GetColumnFromIndex(popColumnIndex);
                Assert.AreEqual("POP\r\n[kN/m²]", popColumn.HeaderText);
                DataGridViewColumn normalStressColumn = table.GetColumnFromIndex(normalStressColumnIndex);
                Assert.AreEqual("Normaalspanning\r\n[kN/m²]", normalStressColumn.HeaderText);
                DataGridViewColumn shearStressColumn = table.GetColumnFromIndex(shearStressColumnIndex);
                Assert.AreEqual("Schuifspanning\r\n[kN/m²]", shearStressColumn.HeaderText);
                DataGridViewColumn loadStressColumn = table.GetColumnFromIndex(loadStressColumnIndex);
                Assert.AreEqual("Spanning belasting\r\n[kN/m²]", loadStressColumn.HeaderText);

                Assert.Throws<ArgumentOutOfRangeException>(() => table.GetColumnFromIndex(loadStressColumnIndex + 1));

                CollectionAssert.IsEmpty(table.Rows);
            }
        }

        [Test]
        public void SetData_NoDataAlreadySet_SetNewData()
        {
            // Setup
            using (var table = new MacroStabilityInwardsSlicesTable())
            {
                MacroStabilityInwardsSlice[] slices =
                {
                    MacroStabilityInwardsSliceTestFactory.CreateSlice(),
                    MacroStabilityInwardsSliceTestFactory.CreateSlice(),
                    MacroStabilityInwardsSliceTestFactory.CreateSlice()
                };

                // Call
                table.SetData(slices);

                // Assert
                Assert.AreEqual(slices.Length, table.Rows.Count);
            }
        }

        [Test]
        public void SetData_SetNullDataAfterDataAlreadySet_ClearsData()
        {
            // Setup
            using (var table = new MacroStabilityInwardsSlicesTable())
            {
                MacroStabilityInwardsSlice[] slices =
                {
                    MacroStabilityInwardsSliceTestFactory.CreateSlice(),
                    MacroStabilityInwardsSliceTestFactory.CreateSlice(),
                    MacroStabilityInwardsSliceTestFactory.CreateSlice()
                };
                table.SetData(slices);

                // Call
                table.SetData(null);

                // Assert
                Assert.AreEqual(0, table.Rows.Count);
            }
        }

        [Test]
        public void SetData_SetNewDataAfterDataAlreadySet_ClearDataAndAddNewData()
        {
            // Setup
            using (var table = new MacroStabilityInwardsSlicesTable())
            {
                table.SetData(new[]
                {
                    MacroStabilityInwardsSliceTestFactory.CreateSlice()
                });

                MacroStabilityInwardsSlice[] newSlices =
                {
                    MacroStabilityInwardsSliceTestFactory.CreateSlice(),
                    MacroStabilityInwardsSliceTestFactory.CreateSlice(),
                    MacroStabilityInwardsSliceTestFactory.CreateSlice()
                };

                // Call
                table.SetData(newSlices);

                // Assert
                Assert.AreEqual(newSlices.Length, table.Rows.Count);
            }
        }

        [Test]
        public void SetData_WithData_ExpectedValuesInTable()
        {
            // Setup
            using (var table = new MacroStabilityInwardsSlicesTable())
            {
                MacroStabilityInwardsSlice[] slices =
                {
                    MacroStabilityInwardsSliceTestFactory.CreateSlice(),
                    MacroStabilityInwardsSliceTestFactory.CreateSlice(),
                    MacroStabilityInwardsSliceTestFactory.CreateSlice()
                };

                // Call
                table.SetData(slices);

                // Assert
                Assert.AreEqual(slices.Length, table.Rows.Count);
                for (var i = 0; i < table.Rows.Count; i++)
                {
                    MacroStabilityInwardsSlice slice = slices[i];
                    DataGridViewCellCollection rowCells = table.Rows[i].Cells;

                    Assert.AreEqual("Lamel " + (i + 1),
                                    rowCells[nameColumnIndex].Value);

                    var xCenter = (RoundedDouble) rowCells[xCenterColumnIndex].Value;
                    Assert.AreEqual((slice.TopLeftPoint.X + slice.TopRightPoint.X) / 2.0,
                                    xCenter,
                                    xCenter.GetAccuracy());

                    var zCenter = (RoundedDouble) rowCells[zCenterBottomColumnIndex].Value;
                    Assert.AreEqual(new Segment2D(slice.BottomLeftPoint, slice.BottomRightPoint).Interpolate(xCenter),
                                    zCenter,
                                    zCenter.GetAccuracy());

                    var width = (RoundedDouble) rowCells[widthColumnIndex].Value;
                    Assert.AreEqual(slice.TopRightPoint.X - slice.TopLeftPoint.X,
                                    width,
                                    width.GetAccuracy());

                    var arcLength = (RoundedDouble) rowCells[arcLengthColumnIndex].Value;
                    Assert.AreEqual(slice.BottomLeftPoint.GetEuclideanDistanceTo(slice.BottomRightPoint),
                                    arcLength,
                                    arcLength.GetAccuracy());

                    var bottomAngle = (RoundedDouble) rowCells[bottomAngleColumnIndex].Value;
                    Assert.AreEqual(Math2D.GetAngleBetween(slice.BottomLeftPoint, slice.BottomRightPoint),
                                    bottomAngle,
                                    bottomAngle.GetAccuracy());

                    var topAngle = (RoundedDouble) rowCells[topAngleColumnIndex].Value;
                    Assert.AreEqual(Math2D.GetAngleBetween(slice.TopLeftPoint, slice.TopRightPoint),
                                    topAngle,
                                    topAngle.GetAccuracy());

                    var frictionAngle = (RoundedDouble) rowCells[frictionAngleColumnIndex].Value;
                    Assert.AreEqual(slice.FrictionAngle,
                                    frictionAngle,
                                    frictionAngle.GetAccuracy());

                    var cohesion = (RoundedDouble) rowCells[cohesionColumnIndex].Value;
                    Assert.AreEqual(slice.Cohesion,
                                    cohesion,
                                    cohesion.GetAccuracy());

                    var effectiveStress = (RoundedDouble) rowCells[effectiveStressColumnIndex].Value;
                    Assert.AreEqual(slice.EffectiveStress,
                                    effectiveStress,
                                    effectiveStress.GetAccuracy());

                    var effectiveStressDaily = (RoundedDouble) rowCells[effectiveStressDailyColumnIndex].Value;
                    Assert.AreEqual(slice.EffectiveStressDaily,
                                    effectiveStressDaily,
                                    effectiveStressDaily.GetAccuracy());

                    var totalPorePressure = (RoundedDouble) rowCells[totalPorePressureColumnIndex].Value;
                    Assert.AreEqual(slice.TotalPorePressure,
                                    totalPorePressure,
                                    totalPorePressure.GetAccuracy());

                    var weight = (RoundedDouble) rowCells[weightColumnIndex].Value;
                    Assert.AreEqual(slice.Weight,
                                    weight,
                                    weight.GetAccuracy());

                    var piezometricPorePressure = (RoundedDouble) rowCells[piezometricPorePressureColumnIndex].Value;
                    Assert.AreEqual(slice.PiezometricPorePressure,
                                    piezometricPorePressure,
                                    piezometricPorePressure.GetAccuracy());

                    var porePressure = (RoundedDouble) rowCells[porePressureColumnIndex].Value;
                    Assert.AreEqual(slice.PorePressure,
                                    porePressure,
                                    porePressure.GetAccuracy());

                    var verticalPorePressure = (RoundedDouble) rowCells[verticalPorePressureColumnIndex].Value;
                    Assert.AreEqual(slice.VerticalPorePressure,
                                    verticalPorePressure,
                                    verticalPorePressure.GetAccuracy());

                    var horizontalPorePressure = (RoundedDouble) rowCells[horizontalPorePressureColumnIndex].Value;
                    Assert.AreEqual(slice.HorizontalPorePressure,
                                    horizontalPorePressure,
                                    horizontalPorePressure.GetAccuracy());

                    var ocr = (RoundedDouble) rowCells[overConsolidationRatioColumnIndex].Value;
                    Assert.AreEqual(slice.OverConsolidationRatio,
                                    ocr,
                                    ocr.GetAccuracy());

                    var pop = (RoundedDouble) rowCells[popColumnIndex].Value;
                    Assert.AreEqual(slice.Pop,
                                    pop,
                                    pop.GetAccuracy());

                    var normalStress = (RoundedDouble) rowCells[normalStressColumnIndex].Value;
                    Assert.AreEqual(slice.NormalStress,
                                    normalStress,
                                    normalStress.GetAccuracy());

                    var shearStress = (RoundedDouble) rowCells[shearStressColumnIndex].Value;
                    Assert.AreEqual(slice.ShearStress,
                                    shearStress,
                                    shearStress.GetAccuracy());

                    var loadStress = (RoundedDouble) rowCells[loadStressColumnIndex].Value;
                    Assert.AreEqual(slice.LoadStress,
                                    loadStress,
                                    loadStress.GetAccuracy());
                }
            }
        }
    }
}