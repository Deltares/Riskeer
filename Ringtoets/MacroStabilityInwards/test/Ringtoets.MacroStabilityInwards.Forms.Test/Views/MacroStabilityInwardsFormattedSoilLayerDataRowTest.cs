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
using Core.Common.Base.Data;
using Core.Common.TestUtil;
using Core.Common.Utils;
using NUnit.Framework;
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.MacroStabilityInwards.Data.SoilProfile;
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
            Assert.Throws<ArgumentNullException>(test);
        }

        [Test]
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
                    Mean = (RoundedDouble) 1.0
                },
                BelowPhreaticLevel = new VariationCoefficientLogNormalDistribution
                {
                    CoefficientOfVariation = (RoundedDouble) 2.0,
                    Mean = (RoundedDouble) 2.0
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
            Assert.AreEqual("0,71 (Verwachtingswaarde = 1,00, Variatiecoëfficiënt = 1,00)", formattedSoilLayerDataRow.AbovePhreaticLevel);
            Assert.AreEqual("0,89 (Verwachtingswaarde = 2,00, Variatiecoëfficiënt = 2,00)", formattedSoilLayerDataRow.BelowPhreaticLevel);
            Assert.AreEqual(soilLayerData.ShearStrengthModel, formattedSoilLayerDataRow.ShearStrengthModel);
            Assert.AreEqual("0,08 (Verwachtingswaarde = 3,00, Variatiecoëfficiënt = 3,00)", formattedSoilLayerDataRow.ShearStrengthRatio);
            Assert.AreEqual("0,06 (Verwachtingswaarde = 4,00, Variatiecoëfficiënt = 4,00)", formattedSoilLayerDataRow.Cohesion);
            Assert.AreEqual("0,05 (Verwachtingswaarde = 5,00, Variatiecoëfficiënt = 5,00)", formattedSoilLayerDataRow.FrictionAngle);
            Assert.AreEqual("0,04 (Verwachtingswaarde = 6,00, Variatiecoëfficiënt = 6,00)", formattedSoilLayerDataRow.StrengthIncreaseExponent);
            Assert.AreEqual(soilLayerData.UsePop, formattedSoilLayerDataRow.UsePop);
            Assert.AreEqual("0,04 (Verwachtingswaarde = 7,00, Variatiecoëfficiënt = 7,00)", formattedSoilLayerDataRow.Pop);
        }
    }
}