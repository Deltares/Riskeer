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
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.IO.Exceptions;
using Ringtoets.Common.IO.SoilProfile;
using Ringtoets.MacroStabilityInwards.IO.SoilProfiles;
using Ringtoets.MacroStabilityInwards.Primitives;

namespace Ringtoets.MacroStabilityInwards.IO.Test.SoilProfiles
{
    [TestFixture]
    public class MacroStabilityInwardsSoilLayerTransformerTest
    {
        [Test]
        public void SoilLayer1DTransform_SoilLayer1DNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => MacroStabilityInwardsSoilLayerTransformer.Transform(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("soilLayer", exception.ParamName);
        }

        [Test]
        public void SoilLayer1DTransform_PropertiesSetAndValid_ReturnMacroStabilityInwardSoilLayer1D()
        {
            // Setup
            var random = new Random(22);

            bool isAquifer = random.NextBoolean();
            double top = random.NextDouble();
            const string materialName = "materialX";
            Color color = Color.AliceBlue;
            bool usePop = random.NextBoolean();
            var shearStrengthModel = random.NextEnumValue<ShearStrengthModel>();

            double abovePhreaticLevelMean = random.NextDouble();
            double abovePhreaticLevelDeviation = random.NextDouble();
            double belowPhreaticLevelMean = random.NextDouble();
            double belowPhreaticLevelDeviation = random.NextDouble();
            double cohesionMean = random.NextDouble();
            double cohesionDeviation = random.NextDouble();
            double cohesionShift = random.NextDouble();
            double frictionAngleMean = random.NextDouble();
            double frictionAngleDeviation = random.NextDouble();
            double frictionAngleShift = random.NextDouble();
            double shearStrengthRatioMean = random.NextDouble();
            double shearStrengthRatioDeviation = random.NextDouble();
            double shearStrengthRatioShift = random.NextDouble();
            double strengthIncreaseExponentMean = random.NextDouble();
            double strengthIncreaseExponentDeviation = random.NextDouble();
            double strengthIncreaseExponentShift = random.NextDouble();
            double popMean = random.NextDouble();
            double popDeviation = random.NextDouble();
            double popShift = random.NextDouble();

            var layer = new SoilLayer1D(top)
            {
                IsAquifer = isAquifer,
                MaterialName = materialName,
                Color = color,
                UsePop = usePop,
                ShearStrengthModel = shearStrengthModel,
                AbovePhreaticLevelMean = abovePhreaticLevelMean,
                AbovePhreaticLevelDeviation = abovePhreaticLevelDeviation,
                BelowPhreaticLevelMean = belowPhreaticLevelMean,
                BelowPhreaticLevelDeviation = belowPhreaticLevelDeviation,
                CohesionMean = cohesionMean,
                CohesionDeviation = cohesionDeviation,
                CohesionShift = cohesionShift,
                FrictionAngleMean = frictionAngleMean,
                FrictionAngleDeviation = frictionAngleDeviation,
                FrictionAngleShift = frictionAngleShift,
                ShearStrengthRatioMean = shearStrengthRatioMean,
                ShearStrengthRatioDeviation = shearStrengthRatioDeviation,
                ShearStrengthRatioShift = shearStrengthRatioShift,
                StrengthIncreaseExponentMean = strengthIncreaseExponentMean,
                StrengthIncreaseExponentDeviation = strengthIncreaseExponentDeviation,
                StrengthIncreaseExponentShift = strengthIncreaseExponentShift,
                PopMean = popMean,
                PopDeviation = popDeviation,
                PopShift = popShift
            };

            // Call
            MacroStabilityInwardsSoilLayer1D soilLayer1D = MacroStabilityInwardsSoilLayerTransformer.Transform(layer);

            // Assert
            Assert.AreEqual(top, soilLayer1D.Top);

            SoilLayerProperties properties = soilLayer1D.Properties;
            Assert.AreEqual(isAquifer, properties.IsAquifer);
            Assert.AreEqual(materialName, properties.MaterialName);
            Assert.AreEqual(color, properties.Color);
            Assert.AreEqual(usePop, properties.UsePop);
            Assert.AreEqual(GetMacroStabilityInwardsShearStrengthModel(shearStrengthModel), properties.ShearStrengthModel);
            Assert.AreEqual(abovePhreaticLevelMean, properties.AbovePhreaticLevelMean);
            Assert.AreEqual(abovePhreaticLevelDeviation, properties.AbovePhreaticLevelDeviation);
            Assert.AreEqual(belowPhreaticLevelMean, properties.BelowPhreaticLevelMean);
            Assert.AreEqual(belowPhreaticLevelDeviation, properties.BelowPhreaticLevelDeviation);
            Assert.AreEqual(cohesionMean, properties.CohesionMean);
            Assert.AreEqual(cohesionDeviation, properties.CohesionDeviation);
            Assert.AreEqual(cohesionShift, properties.CohesionShift);
            Assert.AreEqual(frictionAngleMean, properties.FrictionAngleMean);
            Assert.AreEqual(frictionAngleDeviation, properties.FrictionAngleDeviation);
            Assert.AreEqual(frictionAngleShift, properties.FrictionAngleShift);
            Assert.AreEqual(shearStrengthRatioMean, properties.ShearStrengthRatioMean);
            Assert.AreEqual(shearStrengthRatioDeviation, properties.ShearStrengthRatioDeviation);
            Assert.AreEqual(shearStrengthRatioShift, properties.ShearStrengthRatioShift);
            Assert.AreEqual(strengthIncreaseExponentMean, properties.StrengthIncreaseExponentMean);
            Assert.AreEqual(strengthIncreaseExponentDeviation, properties.StrengthIncreaseExponentDeviation);
            Assert.AreEqual(strengthIncreaseExponentShift, properties.StrengthIncreaseExponentShift);
            Assert.AreEqual(popMean, properties.PopMean);
            Assert.AreEqual(popDeviation, properties.PopDeviation);
            Assert.AreEqual(popShift, properties.PopShift);
        }

        [Test]
        public void SoilLayer1DTransform_InvalidShearStrengthModel_ThrowImportedDataTransformException()
        {
            // Setup
            var layer = new SoilLayer1D(1)
            {
                ShearStrengthModel = (ShearStrengthModel) 99
            };

            // Call
            TestDelegate test = () => MacroStabilityInwardsSoilLayerTransformer.Transform(layer);

            // Assert
            var exception = Assert.Throws<ImportedDataTransformException>(test);
            Assert.AreEqual("Er ging iets mis met transformeren.", exception.Message);
            Assert.IsInstanceOf<NotSupportedException>(exception.InnerException);
        }

        private MacroStabilityInwardsShearStrengthModel GetMacroStabilityInwardsShearStrengthModel(ShearStrengthModel shearStrengthModel)
        {
            switch (shearStrengthModel)
            {
                case ShearStrengthModel.None:
                    return MacroStabilityInwardsShearStrengthModel.None;
                case ShearStrengthModel.SuCalculated:
                    return MacroStabilityInwardsShearStrengthModel.SuCalculated;
                case ShearStrengthModel.CPhi:
                    return MacroStabilityInwardsShearStrengthModel.CPhi;
                case ShearStrengthModel.CPhiOrSuCalculated:
                    return MacroStabilityInwardsShearStrengthModel.CPhiOrSuCalculated;
                default:
                    throw new ArgumentOutOfRangeException(nameof(shearStrengthModel), shearStrengthModel, null);
            }
        }
    }
}