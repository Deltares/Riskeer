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
using System.Collections.Generic;
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
            TestDelegate test = () => MacroStabilityInwardsSoilLayerTransformer.Transform((SoilLayer1D) null);

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
            double frictionAngleMean = random.NextDouble();
            double frictionAngleDeviation = random.NextDouble();
            double shearStrengthRatioMean = random.NextDouble();
            double shearStrengthRatioDeviation = random.NextDouble();
            double strengthIncreaseExponentMean = random.NextDouble();
            double strengthIncreaseExponentDeviation = random.NextDouble();
            double popMean = random.NextDouble();
            double popDeviation = random.NextDouble();

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
                FrictionAngleMean = frictionAngleMean,
                FrictionAngleDeviation = frictionAngleDeviation,
                ShearStrengthRatioMean = shearStrengthRatioMean,
                ShearStrengthRatioDeviation = shearStrengthRatioDeviation,
                StrengthIncreaseExponentMean = strengthIncreaseExponentMean,
                StrengthIncreaseExponentDeviation = strengthIncreaseExponentDeviation,
                PopMean = popMean,
                PopDeviation = popDeviation
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
            Assert.AreEqual(frictionAngleMean, properties.FrictionAngleMean);
            Assert.AreEqual(frictionAngleDeviation, properties.FrictionAngleDeviation);
            Assert.AreEqual(shearStrengthRatioMean, properties.ShearStrengthRatioMean);
            Assert.AreEqual(shearStrengthRatioDeviation, properties.ShearStrengthRatioDeviation);
            Assert.AreEqual(strengthIncreaseExponentMean, properties.StrengthIncreaseExponentMean);
            Assert.AreEqual(strengthIncreaseExponentDeviation, properties.StrengthIncreaseExponentDeviation);
            Assert.AreEqual(popMean, properties.PopMean);
            Assert.AreEqual(popDeviation, properties.PopDeviation);
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

        [Test]
        [TestCaseSource(nameof(IncorrectLogNormalDistributionsSoilLayer1D))]
        public void SoilLayer1DTransform_IncorrectLogNormalDistribution_ThrowImportedDataTransformException(SoilLayer1D layer, string parameter)
        {
            // Call
            TestDelegate test = () => MacroStabilityInwardsSoilLayerTransformer.Transform(layer);

            // Assert
            Exception exception = Assert.Throws<ImportedDataTransformException>(test);
            Assert.AreEqual($"Parameter '{parameter}' is niet lognormaal verdeeld.", exception.Message);
        }

        private static IEnumerable<TestCaseData> IncorrectLogNormalDistributionsSoilLayer1D()
        {
            return IncorrectLogNormalDistributions(() => new SoilLayer1D(0.0), nameof(SoilLayer1D));
        }

        private static IEnumerable<TestCaseData> IncorrectLogNormalDistributions(Func<SoilLayerBase> soilLayer, string typeName)
        {
            const string testNameFormat = "{0}Transform_Incorrect{1}{{1}}_ThrowsImportedDataTransformException";
            const long validDistribution = SoilLayerConstants.LogNormalDistributionValue;
            const double validShift = 0.0;

            SoilLayerBase invalidCohesionShift = soilLayer();
            invalidCohesionShift.CohesionDistribution = validDistribution;
            invalidCohesionShift.CohesionShift = -1;
            yield return new TestCaseData(invalidCohesionShift, "Cohesie"
            ).SetName(string.Format(testNameFormat, typeName, "Shift"));

            SoilLayerBase invalidCohesionDistribution = soilLayer();
            invalidCohesionDistribution.CohesionDistribution = -1;
            invalidCohesionDistribution.CohesionShift = validShift;
            yield return new TestCaseData(invalidCohesionDistribution, "Cohesie"
            ).SetName(string.Format(testNameFormat, typeName, "Distribution"));

            SoilLayerBase invalidFrictionAngleShift = soilLayer();
            invalidFrictionAngleShift.FrictionAngleDistribution = validDistribution;
            invalidFrictionAngleShift.FrictionAngleShift = -1;
            yield return new TestCaseData(invalidFrictionAngleShift, "Wrijvingshoek"
            ).SetName(string.Format(testNameFormat, typeName, "Shift"));

            SoilLayerBase invalidFrictionAngleDistribution = soilLayer();
            invalidFrictionAngleDistribution.FrictionAngleDistribution = -1;
            invalidFrictionAngleDistribution.FrictionAngleShift = validShift;
            yield return new TestCaseData(invalidFrictionAngleDistribution, "Wrijvingshoek"
            ).SetName(string.Format(testNameFormat, typeName, "Distribution"));

            SoilLayerBase invalidShearStrengthRatioShift = soilLayer();
            invalidShearStrengthRatioShift.ShearStrengthRatioDistribution = validDistribution;
            invalidShearStrengthRatioShift.ShearStrengthRatioShift = -1;
            yield return new TestCaseData(invalidShearStrengthRatioShift, "Schuifsterkte ratio S"
            ).SetName(string.Format(testNameFormat, typeName, "Shift"));

            SoilLayerBase invalidShearStrengthRatioDistribution = soilLayer();
            invalidShearStrengthRatioDistribution.ShearStrengthRatioDistribution = -1;
            invalidShearStrengthRatioDistribution.ShearStrengthRatioShift = validShift;
            yield return new TestCaseData(invalidShearStrengthRatioDistribution, "Schuifsterkte ratio S"
            ).SetName(string.Format(testNameFormat, typeName, "Distribution"));

            SoilLayerBase invalidStrengthIncreaseExponentShift = soilLayer();
            invalidStrengthIncreaseExponentShift.StrengthIncreaseExponentDistribution = validDistribution;
            invalidStrengthIncreaseExponentShift.StrengthIncreaseExponentShift = -1;
            yield return new TestCaseData(invalidStrengthIncreaseExponentShift, "Sterkte toename exponent"
            ).SetName(string.Format(testNameFormat, typeName, "Shift"));

            SoilLayerBase invalidStrengthIncreaseExponentDistribution = soilLayer();
            invalidStrengthIncreaseExponentDistribution.StrengthIncreaseExponentDistribution = -1;
            invalidStrengthIncreaseExponentDistribution.StrengthIncreaseExponentShift = validShift;
            yield return new TestCaseData(invalidStrengthIncreaseExponentDistribution, "Sterkte toename exponent"
            ).SetName(string.Format(testNameFormat, typeName, "Distribution"));

            SoilLayerBase invalidPopShift = soilLayer();
            invalidPopShift.PopDistribution = validDistribution;
            invalidPopShift.PopShift = -1;
            yield return new TestCaseData(invalidPopShift, "POP"
            ).SetName(string.Format(testNameFormat, typeName, "Shift"));

            SoilLayerBase invalidPopDistribution = soilLayer();
            invalidPopDistribution.PopDistribution = -1;
            invalidPopDistribution.PopShift = validShift;
            yield return new TestCaseData(invalidPopDistribution, "POP"
            ).SetName(string.Format(testNameFormat, typeName, "Distribution"));
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