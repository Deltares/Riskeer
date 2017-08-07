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

using System;
using System.Drawing;
using System.Windows.Forms;
using Core.Common.Base.Data;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.MacroStabilityInwards.Forms.Views;
using Ringtoets.MacroStabilityInwards.Primitives;

namespace Ringtoets.MacroStabilityInwards.Forms.Test.Views
{
    [TestFixture]
    public class MacroStabilityInwardsSoilLayerTableTest
    {
        private const int nameColumnIndex = 0;
        private const int colorColumnIndex = 1;
        private const int topColumnIndex = 2;
        private const int isAquiferColumnIndex = 3;

        [Test]
        public void Constructor_InitializesWithColumns()
        {
            // Call
            using (var table = new MacroStabilityInwardsSoilLayerTable())
            {
                // Assert
                DataGridViewColumn nameColumn = table.GetColumnFromIndex(nameColumnIndex);
                Assert.AreEqual("Naam", nameColumn.HeaderText);
                DataGridViewColumn colorColumn = table.GetColumnFromIndex(colorColumnIndex);
                Assert.AreEqual("Kleur", colorColumn.HeaderText);
                DataGridViewColumn topColumn = table.GetColumnFromIndex(topColumnIndex);
                Assert.AreEqual("Topniveau [m+NAP]", topColumn.HeaderText);
                DataGridViewColumn isAquiferColumn = table.GetColumnFromIndex(isAquiferColumnIndex);
                Assert.AreEqual("Is aquifer", isAquiferColumn.HeaderText);

                Assert.Throws<ArgumentOutOfRangeException>(() => table.GetColumnFromIndex(isAquiferColumnIndex + 1));

                Assert.IsEmpty(table.Rows);
            }
        }

        [Test]
        public void SetData_NoDataAlreadySet_SetNewData()
        {
            // Setup
            using (var table = new MacroStabilityInwardsSoilLayerTable())
            {
                var layers = new[]
                {
                    new MacroStabilityInwardsSoilLayer1D(2.5),
                    new MacroStabilityInwardsSoilLayer1D(2.3),
                    new MacroStabilityInwardsSoilLayer1D(1.1)
                };

                // Call
                table.SetData(layers);

                // Assert
                Assert.AreEqual(3, table.Rows.Count);
            }
        }

        [Test]
        public void SetData_SetNullDataAfterDataAlreadySet_ClearsData()
        {
            // Setup
            using (var table = new MacroStabilityInwardsSoilLayerTable())
            {
                var layers = new[]
                {
                    new MacroStabilityInwardsSoilLayer1D(2.5),
                    new MacroStabilityInwardsSoilLayer1D(2.3),
                    new MacroStabilityInwardsSoilLayer1D(1.1)
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
            using (var table = new MacroStabilityInwardsSoilLayerTable())
            {
                var layers = new[]
                {
                    new MacroStabilityInwardsSoilLayer1D(2.5),
                    new MacroStabilityInwardsSoilLayer1D(2.3),
                    new MacroStabilityInwardsSoilLayer1D(1.1)
                };
                table.SetData(new[]
                {
                    new MacroStabilityInwardsSoilLayer1D(1.0)
                });

                // Call
                table.SetData(layers);

                // Assert
                Assert.AreEqual(3, table.Rows.Count);
            }
        }

        [Test]
        public void SetData_WithData_ExpectedValuesInTable()
        {
            // Setup
            using (var table = new MacroStabilityInwardsSoilLayerTable())
            {
                var layers = new[]
                {
                    CreateMacroStabilityInwardsSoilLayer(),
                    CreateMacroStabilityInwardsSoilLayer(),
                    CreateMacroStabilityInwardsSoilLayer()
                };
                table.SetData(new[]
                {
                    new MacroStabilityInwardsSoilLayer1D(1.0)
                });

                // Call
                table.SetData(layers);

                // Assert
                Assert.AreEqual(3, table.Rows.Count);
                for (var i = 0; i < table.Rows.Count; i++)
                {
                    MacroStabilityInwardsSoilLayer1D soilLayer = layers[i];
                    DataGridViewCellCollection rowCells = table.Rows[i].Cells;
                    AssertColumnValueEqual(soilLayer.Top, rowCells[topColumnIndex].Value);
                    AssertColumnValueEqual(soilLayer.Properties.MaterialName, rowCells[nameColumnIndex].Value);
                    AssertColumnValueEqual(soilLayer.Properties.Color, rowCells[colorColumnIndex].Value);
                    AssertColumnValueEqual(soilLayer.Properties.IsAquifer, rowCells[isAquiferColumnIndex].Value);
                }
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

        private static MacroStabilityInwardsSoilLayer1D CreateMacroStabilityInwardsSoilLayer()
        {
            var random = new Random();

            return new MacroStabilityInwardsSoilLayer1D(random.NextDouble())
            {
                Properties =
                {
                    MaterialName = $"{random.NextDouble()}",
                    Color = Color.FromKnownColor(random.NextEnumValue<KnownColor>()),
                    IsAquifer = random.NextBoolean()
                }
            };
        }
    }
}