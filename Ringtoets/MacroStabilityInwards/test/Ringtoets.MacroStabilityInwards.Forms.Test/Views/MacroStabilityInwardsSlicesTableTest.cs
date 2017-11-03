﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
        private const int totalPorePressureColumnIndex = 10;
        private const int weightColumnIndex = 11;
        private const int piezometricPorePressureColumnIndex = 12;
        private const int degreePorePressureSoilColumnIndex = 13;
        private const int degreePorePressureLoadColumnIndex = 14;
        private const int porePressureColumnIndex = 15;
        private const int verticalPorePressureColumnIndex = 16;
        private const int horizontalPorePressureColumnIndex = 17;
        private const int externalLoadColumnIndex = 18;
        private const int overConsolidationRatioColumnIndex = 19;
        private const int popColumnIndex = 20;
        private const int normalStressColumnIndex = 21;
        private const int shearStressColumnIndex = 22;
        private const int loadStressColumnIndex = 23;

        [Test]
        public void Constructor_InitializesWithColumns()
        {
            // Call
            using (var table = new MacroStabilityInwardsSlicesTable())
            {
                // Assert
                DataGridViewColumn nameColumn = table.GetColumnFromIndex(nameColumnIndex);
                Assert.AreEqual("Naam", nameColumn.HeaderText);
                DataGridViewColumn xCenterColumn = table.GetColumnFromIndex(xCenterColumnIndex);
                Assert.AreEqual("X centrum [m]", xCenterColumn.HeaderText);
                DataGridViewColumn zCenterBottomColumn = table.GetColumnFromIndex(zCenterBottomColumnIndex);
                Assert.AreEqual("Z centrum bodem [m+NAP]", zCenterBottomColumn.HeaderText);
                DataGridViewColumn widthColumn = table.GetColumnFromIndex(widthColumnIndex);
                Assert.AreEqual("Breedte [m]", widthColumn.HeaderText);
                DataGridViewColumn arcLengthColumn = table.GetColumnFromIndex(arcLengthColumnIndex);
                Assert.AreEqual("Booglengte [m]", arcLengthColumn.HeaderText);
                DataGridViewColumn topAngleColumn = table.GetColumnFromIndex(topAngleColumnIndex);
                Assert.AreEqual("Tophoek [°]", topAngleColumn.HeaderText);
                DataGridViewColumn bottomAngleColumn = table.GetColumnFromIndex(bottomAngleColumnIndex);
                Assert.AreEqual("Bodemhoek [°]", bottomAngleColumn.HeaderText);
                DataGridViewColumn frictionAngleColumn = table.GetColumnFromIndex(frictionAngleColumnIndex);
                Assert.AreEqual("Wrijvingshoek [°]", frictionAngleColumn.HeaderText);
                DataGridViewColumn cohesionColumn = table.GetColumnFromIndex(cohesionColumnIndex);
                Assert.AreEqual("Cohesie [kN/m²]", cohesionColumn.HeaderText);
                DataGridViewColumn effectiveStressColumn = table.GetColumnFromIndex(effectiveStressColumnIndex);
                Assert.AreEqual("Effectieve spanning [kN/m²]", effectiveStressColumn.HeaderText);
                DataGridViewColumn totalPorePressureColumn = table.GetColumnFromIndex(totalPorePressureColumnIndex);
                Assert.AreEqual("Totale waterspanning [kN/m²]", totalPorePressureColumn.HeaderText);
                DataGridViewColumn weightColumn = table.GetColumnFromIndex(weightColumnIndex);
                Assert.AreEqual("Gewicht [kN/m]", weightColumn.HeaderText);
                DataGridViewColumn piezometricPorePressureColumn = table.GetColumnFromIndex(piezometricPorePressureColumnIndex);
                Assert.AreEqual("Piezometrische waterspanning [kN/m²]", piezometricPorePressureColumn.HeaderText);
                DataGridViewColumn degreePorePressureSoilColumn = table.GetColumnFromIndex(degreePorePressureSoilColumnIndex);
                Assert.AreEqual("Waterspanning door consolidatiegraad grond [kN/m²]", degreePorePressureSoilColumn.HeaderText);
                DataGridViewColumn degreePorePressureLoadColumn = table.GetColumnFromIndex(degreePorePressureLoadColumnIndex);
                Assert.AreEqual("Waterspanning door consolidatiegraad belasting [kN/m²]", degreePorePressureLoadColumn.HeaderText);
                DataGridViewColumn porePressureColumn = table.GetColumnFromIndex(porePressureColumnIndex);
                Assert.AreEqual("Waterspanning op maaiveld [kN/m²]", porePressureColumn.HeaderText);
                DataGridViewColumn verticalPorePressureColumn = table.GetColumnFromIndex(verticalPorePressureColumnIndex);
                Assert.AreEqual("Verticale waterspanning op maaiveld [kN/m²]", verticalPorePressureColumn.HeaderText);
                DataGridViewColumn horizontalPorePressureColumn = table.GetColumnFromIndex(horizontalPorePressureColumnIndex);
                Assert.AreEqual("Horizontale waterspanning op maaiveld [kN/m²]", horizontalPorePressureColumn.HeaderText);
                DataGridViewColumn externalLoadColumn = table.GetColumnFromIndex(externalLoadColumnIndex);
                Assert.AreEqual("Externe belasting [kN/m²]", externalLoadColumn.HeaderText);
                DataGridViewColumn overConsolidationRatioColumn = table.GetColumnFromIndex(overConsolidationRatioColumnIndex);
                Assert.AreEqual("OCR [-]", overConsolidationRatioColumn.HeaderText);
                DataGridViewColumn popColumn = table.GetColumnFromIndex(popColumnIndex);
                Assert.AreEqual("POP [kN/m²]", popColumn.HeaderText);
                DataGridViewColumn normalStressColumn = table.GetColumnFromIndex(normalStressColumnIndex);
                Assert.AreEqual("Normaalspanning [kN/m²]", normalStressColumn.HeaderText);
                DataGridViewColumn shearStressColumn = table.GetColumnFromIndex(shearStressColumnIndex);
                Assert.AreEqual("Schuifspanning [kN/m²]", shearStressColumn.HeaderText);
                DataGridViewColumn loadStressColumn = table.GetColumnFromIndex(loadStressColumnIndex);
                Assert.AreEqual("Spanning belasting [kN/m²]", loadStressColumn.HeaderText);

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
                var slices = new[]
                {
                    MacroStabilityInwardsSliceTestFactory.CreateSlice(),
                    MacroStabilityInwardsSliceTestFactory.CreateSlice(),
                    MacroStabilityInwardsSliceTestFactory.CreateSlice()
                };

                // Call
                table.SetData(slices);

                // Assert
                Assert.AreEqual(3, table.Rows.Count);
            }
        }

        [Test]
        public void SetData_SetNullDataAfterDataAlreadySet_ClearsData()
        {
            // Setup
            using (var table = new MacroStabilityInwardsSlicesTable())
            {
                var slices = new[]
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
                var slices = new[]
                {
                    MacroStabilityInwardsSliceTestFactory.CreateSlice(),
                    MacroStabilityInwardsSliceTestFactory.CreateSlice(),
                    MacroStabilityInwardsSliceTestFactory.CreateSlice()
                };
                table.SetData(new[]
                {
                    MacroStabilityInwardsSliceTestFactory.CreateSlice()
                });

                // Call
                table.SetData(slices);

                // Assert
                Assert.AreEqual(3, table.Rows.Count);
            }
        }

        [Test]
        public void SetData_WithData_ExpectedValuesInTable()
        {
            // Setup
            using (var table = new MacroStabilityInwardsSlicesTable())
            {
                var slices = new[]
                {
                    MacroStabilityInwardsSliceTestFactory.CreateSlice(),
                    MacroStabilityInwardsSliceTestFactory.CreateSlice(),
                    MacroStabilityInwardsSliceTestFactory.CreateSlice()
                };

                // Call
                table.SetData(slices);

                // Assert
                Assert.AreEqual(3, table.Rows.Count);
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

                    var degreePorePressureSoil = (RoundedDouble) rowCells[degreePorePressureSoilColumnIndex].Value;
                    Assert.AreEqual(slice.DegreeOfConsolidationPorePressureSoil,
                                    degreePorePressureSoil,
                                    degreePorePressureSoil.GetAccuracy());

                    var degreePorePressureLoad = (RoundedDouble) rowCells[degreePorePressureLoadColumnIndex].Value;
                    Assert.AreEqual(slice.DegreeOfConsolidationPorePressureLoad,
                                    degreePorePressureLoad,
                                    degreePorePressureLoad.GetAccuracy());

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

                    var externalLoad = (RoundedDouble) rowCells[externalLoadColumnIndex].Value;
                    Assert.AreEqual(slice.ExternalLoad,
                                    externalLoad,
                                    externalLoad.GetAccuracy());

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