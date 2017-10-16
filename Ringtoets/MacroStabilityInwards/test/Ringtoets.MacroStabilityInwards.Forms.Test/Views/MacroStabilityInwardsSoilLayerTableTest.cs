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
using System.Drawing;
using System.Windows.Forms;
using Core.Common.Base.Data;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.MacroStabilityInwards.Data.SoilProfile;
using Ringtoets.MacroStabilityInwards.Forms.Views;
using Ringtoets.MacroStabilityInwards.Primitives;

namespace Ringtoets.MacroStabilityInwards.Forms.Test.Views
{
    [TestFixture]
    public class MacroStabilityInwardsSoilLayerTableTest
    {
        private const int nameColumnIndex = 0;
        private const int colorColumnIndex = 1;
        private const int isAquiferColumnIndex = 2;
        private const int abovePhreaticLevelColumnIndex = 3;
        private const int belowPhreaticLevelColumnIndex = 4;
        private const int shearStrengthModelColumnIndex = 5;
        private const int cohesionColumnIndex = 6;
        private const int frictionAngleColumnIndex = 7;
        private const int shrearStrengthRatioColumnIndex = 8;
        private const int strengthIncreaseExponentColumnIndex = 9;
        private const int usePopColumnIndex = 10;
        private const int popColumnIndex = 11;

        [Test]
        public void Constructor_InitializesWithColumns()
        {
            // Call
            using (var table = new MacroStabilityInwardsSoilLayerDataTable())
            {
                // Assert
                DataGridViewColumn nameColumn = table.GetColumnFromIndex(nameColumnIndex);
                Assert.AreEqual("Naam", nameColumn.HeaderText);
                DataGridViewColumn colorColumn = table.GetColumnFromIndex(colorColumnIndex);
                Assert.AreEqual("Kleur", colorColumn.HeaderText);
                DataGridViewColumn abovePhreaticLevelColumn = table.GetColumnFromIndex(abovePhreaticLevelColumnIndex);
                Assert.AreEqual("Onverzadigd gewicht [kN/m³]", abovePhreaticLevelColumn.HeaderText);
                DataGridViewColumn belowPhreaticLevelColumn = table.GetColumnFromIndex(belowPhreaticLevelColumnIndex);
                Assert.AreEqual("Verzadigd gewicht [kN/m³]", belowPhreaticLevelColumn.HeaderText);
                DataGridViewColumn shearStrengthModelColumn = table.GetColumnFromIndex(shearStrengthModelColumnIndex);
                Assert.AreEqual("Schuifsterkte model", shearStrengthModelColumn.HeaderText);
                DataGridViewColumn cohesionColumn = table.GetColumnFromIndex(cohesionColumnIndex);
                Assert.AreEqual("Cohesie [kN/m³]", cohesionColumn.HeaderText);
                DataGridViewColumn frictionAngleColumn = table.GetColumnFromIndex(frictionAngleColumnIndex);
                Assert.AreEqual("Wrijvingshoek [°]", frictionAngleColumn.HeaderText);
                DataGridViewColumn shrearStrengthRatioColumn = table.GetColumnFromIndex(shrearStrengthRatioColumnIndex);
                Assert.AreEqual("Schuifsterkte ratio S [-]", shrearStrengthRatioColumn.HeaderText);
                DataGridViewColumn strengthIncreaseExponentColumn = table.GetColumnFromIndex(strengthIncreaseExponentColumnIndex);
                Assert.AreEqual("Sterkte toename exp (m) [-]", strengthIncreaseExponentColumn.HeaderText);
                DataGridViewColumn usePopColumn = table.GetColumnFromIndex(usePopColumnIndex);
                Assert.AreEqual("Gebruik POP", usePopColumn.HeaderText);
                DataGridViewColumn popColumn = table.GetColumnFromIndex(popColumnIndex);
                Assert.AreEqual("POP [kN/m³]", popColumn.HeaderText);

                Assert.Throws<ArgumentOutOfRangeException>(() => table.GetColumnFromIndex(popColumnIndex + 1));

                CollectionAssert.IsEmpty(table.Rows);
            }
        }

        [Test]
        public void SetData_NoDataAlreadySet_SetNewData()
        {
            // Setup
            using (var table = new MacroStabilityInwardsSoilLayerDataTable())
            {
                var layers = new[]
                {
                    new MacroStabilityInwardsSoilLayerData(),
                    new MacroStabilityInwardsSoilLayerData(),
                    new MacroStabilityInwardsSoilLayerData()
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
            using (var table = new MacroStabilityInwardsSoilLayerDataTable())
            {
                var layers = new[]
                {
                    new MacroStabilityInwardsSoilLayerData(),
                    new MacroStabilityInwardsSoilLayerData(),
                    new MacroStabilityInwardsSoilLayerData()
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
            using (var table = new MacroStabilityInwardsSoilLayerDataTable())
            {
                var layers = new[]
                {
                    new MacroStabilityInwardsSoilLayerData(),
                    new MacroStabilityInwardsSoilLayerData(),
                    new MacroStabilityInwardsSoilLayerData()
                };
                table.SetData(new[]
                {
                    new MacroStabilityInwardsSoilLayerData()
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
            using (var table = new MacroStabilityInwardsSoilLayerDataTable())
            {
                var layers = new[]
                {
                    CreateMacroStabilityInwardsSoilLayerData(),
                    CreateMacroStabilityInwardsSoilLayerData(),
                    CreateMacroStabilityInwardsSoilLayerData()
                };
                table.SetData(new[]
                {
                    new MacroStabilityInwardsSoilLayerData()
                });

                // Call
                table.SetData(layers);

                // Assert
                Assert.AreEqual(3, table.Rows.Count);
                for (var i = 0; i < table.Rows.Count; i++)
                {
                    MacroStabilityInwardsSoilLayerData soilLayerData = layers[i];
                    DataGridViewCellCollection rowCells = table.Rows[i].Cells;
                    AssertColumnValueEqual(soilLayerData.MaterialName,
                                           rowCells[nameColumnIndex].Value);
                    AssertColumnValueEqual(soilLayerData.Color,
                                           rowCells[colorColumnIndex].Value);
                    AssertColumnValueEqual(soilLayerData.IsAquifer,
                                           rowCells[isAquiferColumnIndex].Value);
                    AssertColumnValueEqual(MacroStabilityInwardsSemiProbabilisticDesignVariableFactory.GetAbovePhreaticLevel(soilLayerData),
                                           rowCells[abovePhreaticLevelColumnIndex].Value);
                    AssertColumnValueEqual(MacroStabilityInwardsSemiProbabilisticDesignVariableFactory.GetBelowPhreaticLevel(soilLayerData),
                                           rowCells[belowPhreaticLevelColumnIndex].Value);
                    AssertColumnValueEqual(soilLayerData.ShearStrengthModel,
                                           rowCells[shearStrengthModelColumnIndex].Value);
                    AssertColumnValueEqual(MacroStabilityInwardsSemiProbabilisticDesignVariableFactory.GetCohesion(soilLayerData),
                                           rowCells[cohesionColumnIndex].Value);
                    AssertColumnValueEqual(MacroStabilityInwardsSemiProbabilisticDesignVariableFactory.GetFrictionAngle(soilLayerData),
                                           rowCells[frictionAngleColumnIndex].Value);
                    AssertColumnValueEqual(MacroStabilityInwardsSemiProbabilisticDesignVariableFactory.GetShearStrengthRatio(soilLayerData),
                                           rowCells[shrearStrengthRatioColumnIndex].Value);
                    AssertColumnValueEqual(MacroStabilityInwardsSemiProbabilisticDesignVariableFactory.GetStrengthIncreaseExponent(soilLayerData),
                                           rowCells[strengthIncreaseExponentColumnIndex].Value);
                    AssertColumnValueEqual(soilLayerData.UsePop,
                                           rowCells[usePopColumnIndex].Value);
                    AssertColumnValueEqual(MacroStabilityInwardsSemiProbabilisticDesignVariableFactory.GetPop(soilLayerData),
                                           rowCells[popColumnIndex].Value);
                }
            }
        }

        private static void AssertColumnValueEqual(object expectedValue, object actualValue)
        {
            if (expectedValue is RoundedDouble)
            {
                Assert.IsInstanceOf<RoundedDouble>(actualValue);
                var expectedRoundedDouble = (RoundedDouble) expectedValue;
                Assert.AreEqual(expectedRoundedDouble, (RoundedDouble) actualValue, expectedRoundedDouble.GetAccuracy());
            }
            else if (expectedValue is VariationCoefficientDesignVariable<VariationCoefficientLogNormalDistribution>)
            {
                var expectedDesignVariable = (VariationCoefficientDesignVariable<VariationCoefficientLogNormalDistribution>) expectedValue;
                string expectedFormattedDesignVariable = $"{expectedDesignVariable.GetDesignValue()} (Verwachtingswaarde = {expectedDesignVariable.Distribution.Mean}, " +
                                                         $"Standaardafwijking = {expectedDesignVariable.Distribution.CoefficientOfVariation})";
                Assert.AreEqual(expectedFormattedDesignVariable, actualValue);
            }
            else
            {
                Assert.AreEqual(expectedValue, actualValue);
            }
        }

        private static MacroStabilityInwardsSoilLayerData CreateMacroStabilityInwardsSoilLayerData()
        {
            var random = new Random(21);

            return new MacroStabilityInwardsSoilLayerData
            {
                MaterialName = $"{random.NextDouble()}",
                Color = Color.FromKnownColor(random.NextEnumValue<KnownColor>()),
                IsAquifer = random.NextBoolean(),
                AbovePhreaticLevel = new VariationCoefficientLogNormalDistribution
                {
                    CoefficientOfVariation = random.NextRoundedDouble(),
                    Mean = random.NextRoundedDouble()
                },
                BelowPhreaticLevel = new VariationCoefficientLogNormalDistribution
                {
                    CoefficientOfVariation = random.NextRoundedDouble(),
                    Mean = random.NextRoundedDouble()
                },
                ShearStrengthModel = random.NextEnumValue<MacroStabilityInwardsShearStrengthModel>(),
                ShearStrengthRatio = new VariationCoefficientLogNormalDistribution
                {
                    CoefficientOfVariation = random.NextRoundedDouble(),
                    Mean = random.NextRoundedDouble()
                },
                Cohesion = new VariationCoefficientLogNormalDistribution
                {
                    CoefficientOfVariation = random.NextRoundedDouble(),
                    Mean = random.NextRoundedDouble()
                },
                FrictionAngle = new VariationCoefficientLogNormalDistribution
                {
                    CoefficientOfVariation = random.NextRoundedDouble(),
                    Mean = random.NextRoundedDouble()
                },
                StrengthIncreaseExponent = new VariationCoefficientLogNormalDistribution
                {
                    CoefficientOfVariation = random.NextRoundedDouble(),
                    Mean = random.NextRoundedDouble()
                },
                UsePop = random.NextBoolean(),
                Pop = new VariationCoefficientLogNormalDistribution
                {
                    CoefficientOfVariation = random.NextRoundedDouble(),
                    Mean = random.NextRoundedDouble()
                }
            };
        }
    }
}