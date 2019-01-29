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
using System.Drawing;
using System.Windows.Forms;
using Core.Common.Base.Data;
using Core.Common.Controls.DataGrid;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Piping.Forms.Views;
using Ringtoets.Piping.Primitives;

namespace Riskeer.Piping.Forms.Test.Views
{
    [TestFixture]
    public class PipingSoilLayerTableTest
    {
        private const int nameColumnIndex = 0;
        private const int colorColumnIndex = 1;
        private const int topColumnIndex = 2;
        private const int isAquiferColumnIndex = 3;
        private const int permeabilityMeanColumnIndex = 4;
        private const int permeabilityCoefficientOfVariationColumnIndex = 5;
        private const int d70MeanColumnIndex = 6;
        private const int d70CoefficientOfVariationColumnIndex = 7;
        private const int belowPhreaticLevelWeightMeanColumnIndex = 8;
        private const int belowPhreaticLevelWeightDeviationColumnIndex = 9;
        private const int belowPhreaticLevelWeightShiftColumnIndex = 10;

        [Test]
        public void Constructor_InitializesWithColumns()
        {
            // Call
            using (var table = new PipingSoilLayerTable())
            {
                // Assert
                Assert.IsInstanceOf<DataGridViewControl>(table);

                DataGridViewColumn nameColumn = table.GetColumnFromIndex(nameColumnIndex);
                Assert.AreEqual("Naam", nameColumn.HeaderText);
                DataGridViewColumn colorColumn = table.GetColumnFromIndex(colorColumnIndex);
                Assert.AreEqual("Kleur", colorColumn.HeaderText);
                DataGridViewColumn topColumn = table.GetColumnFromIndex(topColumnIndex);
                Assert.AreEqual("Topniveau\r\n[m+NAP]", topColumn.HeaderText);
                DataGridViewColumn isAquiferColumn = table.GetColumnFromIndex(isAquiferColumnIndex);
                Assert.AreEqual("Is aquifer", isAquiferColumn.HeaderText);
                DataGridViewColumn permeabilityMeanColumn = table.GetColumnFromIndex(permeabilityMeanColumnIndex);
                Assert.AreEqual("Doorlatendheid\r\n(verwachtingswaarde)\r\n[m/s]", permeabilityMeanColumn.HeaderText);
                DataGridViewColumn permeabilityCoefficientOfVariationColumn = table.GetColumnFromIndex(permeabilityCoefficientOfVariationColumnIndex);
                Assert.AreEqual("Doorlatendheid\r\n(variatiecoëfficiënt)\r\n[-]", permeabilityCoefficientOfVariationColumn.HeaderText);
                DataGridViewColumn d70MeanColumn = table.GetColumnFromIndex(d70MeanColumnIndex);
                Assert.AreEqual("d70\r\n(verwachtingswaarde)\r\n[m]", d70MeanColumn.HeaderText);
                DataGridViewColumn d70DeviationColumn = table.GetColumnFromIndex(d70CoefficientOfVariationColumnIndex);
                Assert.AreEqual("d70\r\n(variatiecoëfficiënt)\r\n[-]", d70DeviationColumn.HeaderText);
                DataGridViewColumn belowPhreaticLevelWeightMeanColumn = table.GetColumnFromIndex(belowPhreaticLevelWeightMeanColumnIndex);
                Assert.AreEqual("Verzadigd gewicht\r\n(verwachtingswaarde)\r\n[kN/m³]", belowPhreaticLevelWeightMeanColumn.HeaderText);
                DataGridViewColumn belowPhreaticLevelWeightDeviationColumn = table.GetColumnFromIndex(belowPhreaticLevelWeightDeviationColumnIndex);
                Assert.AreEqual("Verzadigd gewicht\r\n(standaardafwijking)\r\n[kN/m³]", belowPhreaticLevelWeightDeviationColumn.HeaderText);
                DataGridViewColumn belowPhreaticLevelWeightShiftColumn = table.GetColumnFromIndex(belowPhreaticLevelWeightShiftColumnIndex);
                Assert.AreEqual("Verzadigd gewicht\r\n(verschuiving)\r\n[kN/m³]", belowPhreaticLevelWeightShiftColumn.HeaderText);

                Assert.Throws<ArgumentOutOfRangeException>(() => table.GetColumnFromIndex(belowPhreaticLevelWeightShiftColumnIndex + 1));

                CollectionAssert.IsEmpty(table.Rows);
            }
        }

        [Test]
        public void SetData_NoDataAlreadySet_SetNewData()
        {
            // Setup
            using (var table = new PipingSoilLayerTable())
            {
                var layers = new[]
                {
                    new PipingSoilLayer(2.5),
                    new PipingSoilLayer(2.3),
                    new PipingSoilLayer(1.1)
                };

                // Call
                table.SetData(layers);

                // Assert
                Assert.AreEqual(layers.Length, table.Rows.Count);
            }
        }

        [Test]
        public void SetData_SetNullDataAfterDataAlreadySet_ClearsData()
        {
            // Setup
            using (var table = new PipingSoilLayerTable())
            {
                var layers = new[]
                {
                    new PipingSoilLayer(2.5),
                    new PipingSoilLayer(2.3),
                    new PipingSoilLayer(1.1)
                };
                table.SetData(layers);

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
            using (var table = new PipingSoilLayerTable())
            {
                table.SetData(new[]
                {
                    new PipingSoilLayer(1.0)
                });

                var newLayers = new[]
                {
                    new PipingSoilLayer(2.5),
                    new PipingSoilLayer(2.3),
                    new PipingSoilLayer(1.1)
                };

                // Call
                table.SetData(newLayers);

                // Assert
                Assert.AreEqual(newLayers.Length, table.Rows.Count);
            }
        }

        [Test]
        public void SetData_WithData_ExpectedValuesInTable()
        {
            // Setup
            using (var table = new PipingSoilLayerTable())
            {
                PipingSoilLayer[] layers =
                {
                    CreatePipingSoilLayer(),
                    CreatePipingSoilLayer(),
                    CreatePipingSoilLayer()
                };

                // Call
                table.SetData(layers);

                // Assert
                Assert.AreEqual(layers.Length, table.Rows.Count);
                for (var i = 0; i < table.Rows.Count; i++)
                {
                    PipingSoilLayer pipingSoilLayer = layers[i];
                    DataGridViewCellCollection rowCells = table.Rows[i].Cells;
                    AssertColumnValueEqual(pipingSoilLayer.MaterialName, rowCells[nameColumnIndex].Value);
                    AssertColumnValueEqual(pipingSoilLayer.Color, rowCells[colorColumnIndex].Value);
                    AssertColumnValueEqual(pipingSoilLayer.Top, rowCells[topColumnIndex].Value);
                    AssertColumnValueEqual(pipingSoilLayer.IsAquifer, rowCells[isAquiferColumnIndex].Value);
                    AssertColumnValueEqual(pipingSoilLayer.Permeability.Mean, rowCells[permeabilityMeanColumnIndex].Value);
                    AssertColumnValueEqual(pipingSoilLayer.Permeability.CoefficientOfVariation, rowCells[permeabilityCoefficientOfVariationColumnIndex].Value);
                    AssertColumnValueEqual(pipingSoilLayer.DiameterD70.Mean, rowCells[d70MeanColumnIndex].Value);
                    AssertColumnValueEqual(pipingSoilLayer.DiameterD70.CoefficientOfVariation, rowCells[d70CoefficientOfVariationColumnIndex].Value);
                    AssertColumnValueEqual(pipingSoilLayer.BelowPhreaticLevel.Mean, rowCells[belowPhreaticLevelWeightMeanColumnIndex].Value);
                    AssertColumnValueEqual(pipingSoilLayer.BelowPhreaticLevel.StandardDeviation, rowCells[belowPhreaticLevelWeightDeviationColumnIndex].Value);
                    AssertColumnValueEqual(pipingSoilLayer.BelowPhreaticLevel.Shift, rowCells[belowPhreaticLevelWeightShiftColumnIndex].Value);
                }
            }
        }

        [Test]
        public void SetData_WithEmptyNameAndColor_ExpectedValuesInTable()
        {
            // Setup
            using (var table = new PipingSoilLayerTable())
            {
                PipingSoilLayer soilLayer = CreatePipingSoilLayer();
                soilLayer.MaterialName = string.Empty;
                soilLayer.Color = Color.Empty;

                PipingSoilLayer[] layers =
                {
                    soilLayer
                };

                // Call
                table.SetData(layers);

                // Assert
                PipingSoilLayer pipingSoilLayer = layers[0];
                DataGridViewCellCollection rowCells = table.Rows[0].Cells;
                AssertColumnValueEqual("Onbekend", rowCells[nameColumnIndex].Value);
                AssertColumnValueEqual(Color.White, rowCells[colorColumnIndex].Value);
                AssertColumnValueEqual(pipingSoilLayer.Top, rowCells[topColumnIndex].Value);
                AssertColumnValueEqual(pipingSoilLayer.IsAquifer, rowCells[isAquiferColumnIndex].Value);
                AssertColumnValueEqual(pipingSoilLayer.Permeability.Mean, rowCells[permeabilityMeanColumnIndex].Value);
                AssertColumnValueEqual(pipingSoilLayer.Permeability.CoefficientOfVariation, rowCells[permeabilityCoefficientOfVariationColumnIndex].Value);
                AssertColumnValueEqual(pipingSoilLayer.DiameterD70.Mean, rowCells[d70MeanColumnIndex].Value);
                AssertColumnValueEqual(pipingSoilLayer.DiameterD70.CoefficientOfVariation, rowCells[d70CoefficientOfVariationColumnIndex].Value);
                AssertColumnValueEqual(pipingSoilLayer.BelowPhreaticLevel.Mean, rowCells[belowPhreaticLevelWeightMeanColumnIndex].Value);
                AssertColumnValueEqual(pipingSoilLayer.BelowPhreaticLevel.StandardDeviation, rowCells[belowPhreaticLevelWeightDeviationColumnIndex].Value);
                AssertColumnValueEqual(pipingSoilLayer.BelowPhreaticLevel.Shift, rowCells[belowPhreaticLevelWeightShiftColumnIndex].Value);
            }
        }

        private void AssertColumnValueEqual(object expectedValue, object actualValue)
        {
            if (expectedValue is string || expectedValue is Color)
            {
                Assert.AreEqual(expectedValue, actualValue);
            }

            if (expectedValue is RoundedDouble)
            {
                Assert.IsInstanceOf<RoundedDouble>(actualValue);
                var expectedRoundedDouble = (RoundedDouble) expectedValue;
                Assert.AreEqual(expectedRoundedDouble, (RoundedDouble) actualValue, expectedRoundedDouble.GetAccuracy());
            }
        }

        private static PipingSoilLayer CreatePipingSoilLayer()
        {
            var random = new Random();

            return new PipingSoilLayer(random.NextDouble())
            {
                MaterialName = $"{random.NextDouble()}",
                Color = Color.FromKnownColor(random.NextEnumValue<KnownColor>()),
                IsAquifer = random.NextBoolean(),
                BelowPhreaticLevel = new LogNormalDistribution
                {
                    Mean = random.NextRoundedDouble(1, double.MaxValue),
                    StandardDeviation = random.NextRoundedDouble(),
                    Shift = random.NextRoundedDouble()
                },
                DiameterD70 = new VariationCoefficientLogNormalDistribution
                {
                    Mean = random.NextRoundedDouble(1, double.MaxValue),
                    CoefficientOfVariation = random.NextRoundedDouble()
                },
                Permeability = new VariationCoefficientLogNormalDistribution
                {
                    Mean = random.NextRoundedDouble(1, double.MaxValue),
                    CoefficientOfVariation = random.NextRoundedDouble()
                }
            };
        }
    }
}