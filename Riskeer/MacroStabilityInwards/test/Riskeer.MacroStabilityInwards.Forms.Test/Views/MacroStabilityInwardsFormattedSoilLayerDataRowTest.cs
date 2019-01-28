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
using Core.Common.Base.Data;
using Core.Common.TestUtil;
using Core.Common.Util;
using NUnit.Framework;
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.MacroStabilityInwards.Forms.Views;
using Ringtoets.MacroStabilityInwards.Primitives;

namespace Ringtoets.MacroStabilityInwards.Forms.Test.Views
{
    [TestFixture]
    public class MacroStabilityInwardsFormattedSoilLayerDataRowTest
    {
        [Test]
        public void Constructor_SoilLayerDataNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new MacroStabilityInwardsFormattedSoilLayerDataRow(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("layerData", paramName);
        }

        [Test]
        [SetCulture("nl-NL")]
        public void Constructor_WithSoilLayerData_ExpectedValues()
        {
            // Setup
            var soilLayerData = new MacroStabilityInwardsSoilLayerData
            {
                MaterialName = "Sand",
                Color = Color.Black,
                IsAquifer = true,
                AbovePhreaticLevel = new VariationCoefficientLogNormalDistribution
                {
                    CoefficientOfVariation = (RoundedDouble) 1.0,
                    Mean = (RoundedDouble) 1.0,
                    Shift = (RoundedDouble) 0.5
                },
                BelowPhreaticLevel = new VariationCoefficientLogNormalDistribution
                {
                    CoefficientOfVariation = (RoundedDouble) 2.0,
                    Mean = (RoundedDouble) 2.0,
                    Shift = (RoundedDouble) 1.0
                },
                ShearStrengthModel = MacroStabilityInwardsShearStrengthModel.CPhi,
                ShearStrengthRatio = new VariationCoefficientLogNormalDistribution
                {
                    CoefficientOfVariation = (RoundedDouble) 3.0,
                    Mean = (RoundedDouble) 3.0
                },
                Cohesion = new VariationCoefficientLogNormalDistribution
                {
                    CoefficientOfVariation = (RoundedDouble) 4.0,
                    Mean = (RoundedDouble) 4.0
                },
                FrictionAngle = new VariationCoefficientLogNormalDistribution
                {
                    CoefficientOfVariation = (RoundedDouble) 5.0,
                    Mean = (RoundedDouble) 5.0
                },
                StrengthIncreaseExponent = new VariationCoefficientLogNormalDistribution
                {
                    CoefficientOfVariation = (RoundedDouble) 6.0,
                    Mean = (RoundedDouble) 6.0
                },
                UsePop = true,
                Pop = new VariationCoefficientLogNormalDistribution
                {
                    CoefficientOfVariation = (RoundedDouble) 7.0,
                    Mean = (RoundedDouble) 7.0
                }
            };

            // Call
            var formattedSoilLayerDataRow = new MacroStabilityInwardsFormattedSoilLayerDataRow(soilLayerData);

            // Assert
            TestHelper.AssertTypeConverter<MacroStabilityInwardsFormattedSoilLayerDataRow, EnumTypeConverter>(
                nameof(MacroStabilityInwardsFormattedSoilLayerDataRow.ShearStrengthModel));

            Assert.AreEqual(soilLayerData.MaterialName, formattedSoilLayerDataRow.MaterialName);
            Assert.AreEqual(soilLayerData.Color, formattedSoilLayerDataRow.Color);
            Assert.AreEqual(soilLayerData.IsAquifer, formattedSoilLayerDataRow.IsAquifer);
            Assert.AreEqual("0,85 (Verwachtingswaarde = 1,00, Variatiecoëfficiënt = 1,00, Verschuiving = 0,50)", formattedSoilLayerDataRow.AbovePhreaticLevel);
            Assert.AreEqual("1,45 (Verwachtingswaarde = 2,00, Variatiecoëfficiënt = 2,00, Verschuiving = 1,00)", formattedSoilLayerDataRow.BelowPhreaticLevel);
            Assert.AreEqual(soilLayerData.ShearStrengthModel, formattedSoilLayerDataRow.ShearStrengthModel);
            Assert.AreEqual("0,08 (Verwachtingswaarde = 3,00, Variatiecoëfficiënt = 3,00)", formattedSoilLayerDataRow.ShearStrengthRatio);
            Assert.AreEqual("0,06 (Verwachtingswaarde = 4,00, Variatiecoëfficiënt = 4,00)", formattedSoilLayerDataRow.Cohesion);
            Assert.AreEqual("0,05 (Verwachtingswaarde = 5,00, Variatiecoëfficiënt = 5,00)", formattedSoilLayerDataRow.FrictionAngle);
            Assert.AreEqual("0,04 (Verwachtingswaarde = 6,00, Variatiecoëfficiënt = 6,00)", formattedSoilLayerDataRow.StrengthIncreaseExponent);
            Assert.AreEqual(soilLayerData.UsePop, formattedSoilLayerDataRow.UsePop);
            Assert.AreEqual("0,04 (Verwachtingswaarde = 7,00, Variatiecoëfficiënt = 7,00)", formattedSoilLayerDataRow.Pop);
        }

        [Test]
        [SetCulture("nl-NL")]
        public void Constructor_WithSoilLayerDataEmptyAndNaN_ExpectedValues()
        {
            // Setup
            var soilLayerData = new MacroStabilityInwardsSoilLayerData
            {
                MaterialName = string.Empty,
                Color = Color.Empty,
                IsAquifer = true,
                AbovePhreaticLevel = new VariationCoefficientLogNormalDistribution
                {
                    CoefficientOfVariation = (RoundedDouble) double.NaN,
                    Mean = (RoundedDouble) double.NaN
                },
                BelowPhreaticLevel = new VariationCoefficientLogNormalDistribution
                {
                    CoefficientOfVariation = (RoundedDouble) double.NaN,
                    Mean = (RoundedDouble) double.NaN
                },
                ShearStrengthModel = MacroStabilityInwardsShearStrengthModel.CPhi,
                ShearStrengthRatio = new VariationCoefficientLogNormalDistribution
                {
                    CoefficientOfVariation = (RoundedDouble) double.NaN,
                    Mean = (RoundedDouble) double.NaN
                },
                Cohesion = new VariationCoefficientLogNormalDistribution
                {
                    CoefficientOfVariation = (RoundedDouble) double.NaN,
                    Mean = (RoundedDouble) double.NaN
                },
                FrictionAngle = new VariationCoefficientLogNormalDistribution
                {
                    CoefficientOfVariation = (RoundedDouble) double.NaN,
                    Mean = (RoundedDouble) double.NaN
                },
                StrengthIncreaseExponent = new VariationCoefficientLogNormalDistribution
                {
                    CoefficientOfVariation = (RoundedDouble) double.NaN,
                    Mean = (RoundedDouble) double.NaN
                },
                UsePop = true,
                Pop = new VariationCoefficientLogNormalDistribution
                {
                    CoefficientOfVariation = (RoundedDouble) double.NaN,
                    Mean = (RoundedDouble) double.NaN
                }
            };

            // Call
            var formattedSoilLayerDataRow = new MacroStabilityInwardsFormattedSoilLayerDataRow(soilLayerData);

            // Assert
            TestHelper.AssertTypeConverter<MacroStabilityInwardsFormattedSoilLayerDataRow, EnumTypeConverter>(
                nameof(MacroStabilityInwardsFormattedSoilLayerDataRow.ShearStrengthModel));

            Assert.AreEqual("Onbekend", formattedSoilLayerDataRow.MaterialName);
            Assert.AreEqual(Color.White, formattedSoilLayerDataRow.Color);
            Assert.AreEqual(soilLayerData.IsAquifer, formattedSoilLayerDataRow.IsAquifer);
            Assert.AreEqual("NaN", formattedSoilLayerDataRow.AbovePhreaticLevel);
            Assert.AreEqual("NaN", formattedSoilLayerDataRow.BelowPhreaticLevel);
            Assert.AreEqual(soilLayerData.ShearStrengthModel, formattedSoilLayerDataRow.ShearStrengthModel);
            Assert.AreEqual("NaN", formattedSoilLayerDataRow.ShearStrengthRatio);
            Assert.AreEqual("NaN", formattedSoilLayerDataRow.Cohesion);
            Assert.AreEqual("NaN", formattedSoilLayerDataRow.FrictionAngle);
            Assert.AreEqual("NaN", formattedSoilLayerDataRow.StrengthIncreaseExponent);
            Assert.AreEqual(soilLayerData.UsePop, formattedSoilLayerDataRow.UsePop);
            Assert.AreEqual("NaN", formattedSoilLayerDataRow.Pop);
        }
    }
}