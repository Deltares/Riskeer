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
using System.Linq;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.IO.Exceptions;
using Ringtoets.Common.IO.SoilProfile;
using Ringtoets.Common.IO.TestUtil;
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
                AbovePhreaticLevelMean = abovePhreaticLevelMean,
                AbovePhreaticLevelCoefficientOfVariation = abovePhreaticLevelDeviation,
                BelowPhreaticLevelMean = belowPhreaticLevelMean,
                BelowPhreaticLevelDeviation = belowPhreaticLevelDeviation,
                CohesionMean = cohesionMean,
                CohesionCoefficientOfVariation = cohesionDeviation,
                FrictionAngleMean = frictionAngleMean,
                FrictionAngleCoefficientOfVariation = frictionAngleDeviation,
                ShearStrengthRatioMean = shearStrengthRatioMean,
                ShearStrengthRatioCoefficientOfVariation = shearStrengthRatioDeviation,
                StrengthIncreaseExponentMean = strengthIncreaseExponentMean,
                StrengthIncreaseExponentCoefficientOfVariation = strengthIncreaseExponentDeviation,
                PopMean = popMean,
                PopCoefficientOfVariation = popDeviation
            };

            // Call
            MacroStabilityInwardsSoilLayer1D soilLayer1D = MacroStabilityInwardsSoilLayerTransformer.Transform(layer);

            // Assert
            Assert.AreEqual(top, soilLayer1D.Top);

            MacroStabilityInwardsSoilLayerProperties properties = soilLayer1D.Properties;
            Assert.AreEqual(isAquifer, properties.IsAquifer);
            Assert.AreEqual(materialName, properties.MaterialName);
            Assert.AreEqual(color, properties.Color);
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
        [TestCase(null, true)]
        [TestCase(0, false)]
        public void SoilLayer1DTransform_ValidUsePopValue_ReturnMacroStabilityInwardSoilLayer1D(double? usePop, bool transformedUsePopValue)
        {
            // Setup
            var layer = new SoilLayer1D(0)
            {
                UsePop = usePop
            };

            // Call
            MacroStabilityInwardsSoilLayer1D soilLayer1D = MacroStabilityInwardsSoilLayerTransformer.Transform(layer);

            // Assert
            Assert.AreEqual(transformedUsePopValue, soilLayer1D.Properties.UsePop);
        }

        [Test]
        public void SoilLayer1DTransform_InvalidUsePopValue_ReturnMacroStabilityInwardSoilLayer1D()
        {
            // Setup
            var layer = new SoilLayer1D(0)
            {
                UsePop = 1
            };

            // Call
            TestDelegate test = () => MacroStabilityInwardsSoilLayerTransformer.Transform(layer);

            // Assert
            var exception = Assert.Throws<ImportedDataTransformException>(test);
            Assert.AreEqual("Ongeldige waarde voor parameter 'Gebruik POP'.", exception.Message);
        }

        [Test]
        [TestCase(null, MacroStabilityInwardsShearStrengthModel.CPhi)]
        [TestCase(9, MacroStabilityInwardsShearStrengthModel.CPhiOrSuCalculated)]
        [TestCase(6, MacroStabilityInwardsShearStrengthModel.SuCalculated)]
        [TestCase(1, MacroStabilityInwardsShearStrengthModel.None)]
        public void SoilLayer1DTransform_ValidShearStrengthModelValue_ReturnMacroStabilityInwardSoilLayer1D(double? sheartStrengthModel,
                                                                                                            MacroStabilityInwardsShearStrengthModel transformedShearStrengthModel)
        {
            // Setup
            var layer = new SoilLayer1D(0)
            {
                ShearStrengthModel = sheartStrengthModel
            };

            // Call
            MacroStabilityInwardsSoilLayer1D soilLayer1D = MacroStabilityInwardsSoilLayerTransformer.Transform(layer);

            // Assert
            Assert.AreEqual(transformedShearStrengthModel, soilLayer1D.Properties.ShearStrengthModel);
        }

        [Test]
        public void SoilLayer1DTransform_InvalidShearStrengthModelValue_ThrowsImportedDataTransformException()
        {
            // Setup
            var layer = new SoilLayer1D(0)
            {
                ShearStrengthModel = 2
            };

            // Call
            TestDelegate test = () => MacroStabilityInwardsSoilLayerTransformer.Transform(layer);

            // Assert
            var exception = Assert.Throws<ImportedDataTransformException>(test);
            Assert.AreEqual("Ongeldige waarde voor parameter 'Schuifsterkte model'.", exception.Message);
        }

        [Test]
        public void SoilLayer1DTransform_IncorrectShiftedLogNormalDistribution_ThrowsImportedDataTransformException()
        {
            // Setup
            var layer = new SoilLayer1D(0.0)
            {
                BelowPhreaticLevelDistribution = -1
            };

            // Call
            TestDelegate test = () => MacroStabilityInwardsSoilLayerTransformer.Transform(layer);

            // Assert
            Exception exception = Assert.Throws<ImportedDataTransformException>(test);
            Assert.AreEqual("Parameter 'Verzadigd gewicht' is niet verschoven lognormaal verdeeld.", exception.Message);
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

        [Test]
        public void SoilLayer2DTransform_SoilLayer2DNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => MacroStabilityInwardsSoilLayerTransformer.Transform((SoilLayer2D) null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("soilLayer", exception.ParamName);
        }

        [Test]
        public void SoilLayer2DTransform_PropertiesSetAndValid_ReturnMacroStabilityInwardSoilLayer2D()
        {
            // Setup
            var random = new Random(22);

            bool isAquifer = random.NextBoolean();
            const string materialName = "materialX";
            Color color = Color.AliceBlue;

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

            SoilLayer2D layer = SoilLayer2DTestFactory.CreateSoilLayer2D();
            layer.IsAquifer = isAquifer;
            layer.MaterialName = materialName;
            layer.Color = color;
            layer.AbovePhreaticLevelMean = abovePhreaticLevelMean;
            layer.AbovePhreaticLevelCoefficientOfVariation = abovePhreaticLevelDeviation;
            layer.BelowPhreaticLevelMean = belowPhreaticLevelMean;
            layer.BelowPhreaticLevelDeviation = belowPhreaticLevelDeviation;
            layer.CohesionMean = cohesionMean;
            layer.CohesionCoefficientOfVariation = cohesionDeviation;
            layer.FrictionAngleMean = frictionAngleMean;
            layer.FrictionAngleCoefficientOfVariation = frictionAngleDeviation;
            layer.ShearStrengthRatioMean = shearStrengthRatioMean;
            layer.ShearStrengthRatioCoefficientOfVariation = shearStrengthRatioDeviation;
            layer.StrengthIncreaseExponentMean = strengthIncreaseExponentMean;
            layer.StrengthIncreaseExponentCoefficientOfVariation = strengthIncreaseExponentDeviation;
            layer.PopMean = popMean;
            layer.PopCoefficientOfVariation = popDeviation;

            // Call
            MacroStabilityInwardsSoilLayer2D soilLayer2D = MacroStabilityInwardsSoilLayerTransformer.Transform(layer);

            // Assert
            MacroStabilityInwardsSoilLayerProperties properties = soilLayer2D.Properties;
            Assert.AreEqual(isAquifer, properties.IsAquifer);
            Assert.AreEqual(materialName, properties.MaterialName);
            Assert.AreEqual(color, properties.Color);
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

            AssertRings(layer, soilLayer2D);
        }

        [Test]
        [TestCase(null, true)]
        [TestCase(0, false)]
        public void SoilLayer2DTransform_ValidUsePopValue_ReturnMacroStabilityInwardSoilLayer2D(double? usePop, bool transformedUsePopValue)
        {
            // Setup
            SoilLayer2D layer = SoilLayer2DTestFactory.CreateSoilLayer2D();
            layer.UsePop = usePop;

            // Call
            MacroStabilityInwardsSoilLayer2D soilLayer2D = MacroStabilityInwardsSoilLayerTransformer.Transform(layer);

            // Assert
            Assert.AreEqual(transformedUsePopValue, soilLayer2D.Properties.UsePop);
        }

        [Test]
        public void SoilLayer2DTransform_InvalidUsePopValue_ReturnMacroStabilityInwardSoilLayer2D()
        {
            // Setup
            SoilLayer2D layer = SoilLayer2DTestFactory.CreateSoilLayer2D();
            layer.UsePop = 1;

            // Call
            TestDelegate test = () => MacroStabilityInwardsSoilLayerTransformer.Transform(layer);

            // Assert
            var exception = Assert.Throws<ImportedDataTransformException>(test);
            Assert.AreEqual("Ongeldige waarde voor parameter 'Gebruik POP'.", exception.Message);
        }

        [Test]
        [TestCase(null, MacroStabilityInwardsShearStrengthModel.CPhi)]
        [TestCase(9, MacroStabilityInwardsShearStrengthModel.CPhiOrSuCalculated)]
        [TestCase(6, MacroStabilityInwardsShearStrengthModel.SuCalculated)]
        [TestCase(1, MacroStabilityInwardsShearStrengthModel.None)]
        public void SoilLayer2DTransform_ValidShearStrengthModelValue_ReturnMacroStabilityInwardSoilLayer2D(double? sheartStrengthModel,
                                                                                                            MacroStabilityInwardsShearStrengthModel transformedShearStrengthModel)
        {
            // Setup
            SoilLayer2D layer = SoilLayer2DTestFactory.CreateSoilLayer2D();
            layer.ShearStrengthModel = sheartStrengthModel;

            // Call
            MacroStabilityInwardsSoilLayer2D soilLayer2D = MacroStabilityInwardsSoilLayerTransformer.Transform(layer);

            // Assert
            Assert.AreEqual(transformedShearStrengthModel, soilLayer2D.Properties.ShearStrengthModel);
        }

        [Test]
        public void SoilLayer2DTransform_InvalidShearStrengthModelValue_ThrowsImportedDataTransformException()
        {
            // Setup
            SoilLayer2D layer = SoilLayer2DTestFactory.CreateSoilLayer2D();
            layer.ShearStrengthModel = 2;

            // Call
            TestDelegate test = () => MacroStabilityInwardsSoilLayerTransformer.Transform(layer);

            // Assert
            var exception = Assert.Throws<ImportedDataTransformException>(test);
            Assert.AreEqual("Ongeldige waarde voor parameter 'Schuifsterkte model'.", exception.Message);
        }

        [Test]
        public void SoilLayer2DTransform_IncorrectShiftedLogNormalDistribution_ThrowsImportedDataTransformException()
        {
            // Setup
            var layer = new SoilLayer2D
            {
                BelowPhreaticLevelDistribution = -1
            };

            // Call
            TestDelegate test = () => MacroStabilityInwardsSoilLayerTransformer.Transform(layer);

            // Assert
            Exception exception = Assert.Throws<ImportedDataTransformException>(test);
            Assert.AreEqual("Parameter 'Verzadigd gewicht' is niet verschoven lognormaal verdeeld.", exception.Message);
        }

        [Test]
        [TestCaseSource(nameof(IncorrectLogNormalDistributionsSoilLayer2D))]
        public void SoilLayer2DTransform_IncorrectLogNormalDistribution_ThrowImportedDataTransformException(SoilLayer2D layer, string parameter)
        {
            // Call
            TestDelegate test = () => MacroStabilityInwardsSoilLayerTransformer.Transform(layer);

            // Assert
            Exception exception = Assert.Throws<ImportedDataTransformException>(test);
            Assert.AreEqual($"Parameter '{parameter}' is niet lognormaal verdeeld.", exception.Message);
        }

        [Test]
        public void SoilLayer2DTransform_OuterRingNull_ThrowImportedDataTransformException()
        {
            // Call
            TestDelegate test = () => MacroStabilityInwardsSoilLayerTransformer.Transform(new SoilLayer2D());

            // Assert
            Assert.Throws<ImportedDataTransformException>(test);
        }

        private static void AssertRings(SoilLayer2D soilLayer, MacroStabilityInwardsSoilLayer2D macroStabilityInwardsSoilLayer)
        {
            Assert.AreEqual(GetRingFromSegment(soilLayer.OuterLoop), macroStabilityInwardsSoilLayer.OuterRing);
            CollectionAssert.AreEqual(soilLayer.InnerLoops.Select(GetRingFromSegment), macroStabilityInwardsSoilLayer.Holes);
        }

        private static Ring GetRingFromSegment(IEnumerable<Segment2D> loop)
        {
            var points = new List<Point2D>();
            foreach (Segment2D segment in loop)
            {
                points.AddRange(new[]
                {
                    segment.FirstPoint,
                    segment.SecondPoint
                });
            }
            return new Ring(points.Distinct());
        }

        private static IEnumerable<TestCaseData> IncorrectLogNormalDistributionsSoilLayer1D()
        {
            return IncorrectLogNormalDistributions(() => new SoilLayer1D(0.0), nameof(SoilLayer1D));
        }

        private static IEnumerable<TestCaseData> IncorrectLogNormalDistributionsSoilLayer2D()
        {
            return IncorrectLogNormalDistributions(() => new SoilLayer2D(), nameof(SoilLayer1D));
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
    }
}