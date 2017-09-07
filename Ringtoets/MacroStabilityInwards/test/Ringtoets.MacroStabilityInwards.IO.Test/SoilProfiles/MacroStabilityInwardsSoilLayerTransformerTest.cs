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

            double isAquifer = random.Next(0, 2);
            double top = random.NextDouble();
            const string materialName = "materialX";
            double color = random.NextDouble();

            double abovePhreaticLevelMean = random.NextDouble();
            double abovePhreaticLevelCoefficientOfVariation = random.NextDouble();
            double abovePhreaticLevelShift = random.NextDouble();
            double belowPhreaticLevelMean = random.NextDouble();
            double belowPhreaticLevelCoefficientOfVariation = random.NextDouble();
            double belowPhreaticLevelShift = random.NextDouble();
            double cohesionMean = random.NextDouble();
            double cohesionCoefficientOfVariation = random.NextDouble();
            double frictionAngleMean = random.NextDouble();
            double frictionAngleCoefficientOfVariation = random.NextDouble();
            double shearStrengthRatioMean = random.NextDouble();
            double shearStrengthRatioCoefficientOfVariation = random.NextDouble();
            double strengthIncreaseExponentMean = random.NextDouble();
            double strengthIncreaseExponentCoefficientOfVariation = random.NextDouble();
            double popMean = random.NextDouble();
            double popCoefficientOfVariation = random.NextDouble();

            var layer = new SoilLayer1D(top)
            {
                IsAquifer = isAquifer,
                MaterialName = materialName,
                Color = color,
                AbovePhreaticLevelMean = abovePhreaticLevelMean,
                AbovePhreaticLevelCoefficientOfVariation = abovePhreaticLevelCoefficientOfVariation,
                AbovePhreaticLevelShift = abovePhreaticLevelShift,
                BelowPhreaticLevelMean = belowPhreaticLevelMean,
                BelowPhreaticLevelCoefficientOfVariation = belowPhreaticLevelCoefficientOfVariation,
                BelowPhreaticLevelShift = belowPhreaticLevelShift,
                CohesionMean = cohesionMean,
                CohesionCoefficientOfVariation = cohesionCoefficientOfVariation,
                FrictionAngleMean = frictionAngleMean,
                FrictionAngleCoefficientOfVariation = frictionAngleCoefficientOfVariation,
                ShearStrengthRatioMean = shearStrengthRatioMean,
                ShearStrengthRatioCoefficientOfVariation = shearStrengthRatioCoefficientOfVariation,
                StrengthIncreaseExponentMean = strengthIncreaseExponentMean,
                StrengthIncreaseExponentCoefficientOfVariation = strengthIncreaseExponentCoefficientOfVariation,
                PopMean = popMean,
                PopCoefficientOfVariation = popCoefficientOfVariation
            };

            // Call
            MacroStabilityInwardsSoilLayer1D soilLayer1D = MacroStabilityInwardsSoilLayerTransformer.Transform(layer);

            // Assert
            Assert.AreEqual(top, soilLayer1D.Top);

            MacroStabilityInwardsSoilLayerProperties properties = soilLayer1D.Properties;

            Assert.AreEqual(materialName, properties.MaterialName);
            bool expectedIsAquifer = isAquifer.Equals(1.0);
            Assert.AreEqual(expectedIsAquifer, properties.IsAquifer);
            Color expectedColor = Color.FromArgb(Convert.ToInt32(color));
            Assert.AreEqual(expectedColor, properties.Color);

            Assert.AreEqual(abovePhreaticLevelMean, properties.AbovePhreaticLevelMean);
            Assert.AreEqual(abovePhreaticLevelCoefficientOfVariation, properties.AbovePhreaticLevelCoefficientOfVariation);
            Assert.AreEqual(abovePhreaticLevelShift, properties.AbovePhreaticLevelShift);
            Assert.AreEqual(belowPhreaticLevelMean, properties.BelowPhreaticLevelMean);
            Assert.AreEqual(belowPhreaticLevelCoefficientOfVariation, properties.BelowPhreaticLevelCoefficientOfVariation);
            Assert.AreEqual(belowPhreaticLevelShift, properties.BelowPhreaticLevelShift);
            Assert.AreEqual(cohesionMean, properties.CohesionMean);
            Assert.AreEqual(cohesionCoefficientOfVariation, properties.CohesionCoefficientOfVariation);
            Assert.AreEqual(frictionAngleMean, properties.FrictionAngleMean);
            Assert.AreEqual(frictionAngleCoefficientOfVariation, properties.FrictionAngleCoefficientOfVariation);
            Assert.AreEqual(shearStrengthRatioMean, properties.ShearStrengthRatioMean);
            Assert.AreEqual(shearStrengthRatioCoefficientOfVariation, properties.ShearStrengthRatioCoefficientOfVariation);
            Assert.AreEqual(strengthIncreaseExponentMean, properties.StrengthIncreaseExponentMean);
            Assert.AreEqual(strengthIncreaseExponentCoefficientOfVariation, properties.StrengthIncreaseExponentCoefficientOfVariation);
            Assert.AreEqual(popMean, properties.PopMean);
            Assert.AreEqual(popCoefficientOfVariation, properties.PopCoefficientOfVariation);
        }

        [Test]
        [TestCase(null, true)]
        [TestCase(0, false)]
        public void SoilLayer1DTransform_ValidUsePopValue_ReturnMacroStabilityInwardSoilLayer1D(double? usePop, bool transformedUsePopValue)
        {
            // Setup
            SoilLayer1D layer = SoilLayer1DTestFactory.CreateSoilLayer1DWithValidAquifer();
            layer.UsePop = usePop;

            // Call
            MacroStabilityInwardsSoilLayer1D soilLayer1D = MacroStabilityInwardsSoilLayerTransformer.Transform(layer);

            // Assert
            Assert.AreEqual(transformedUsePopValue, soilLayer1D.Properties.UsePop);
        }

        [Test]
        public void SoilLayer1DTransform_InvalidUsePopValue_ThrowsImportedDataTransformationException()
        {
            // Setup
            SoilLayer1D layer = SoilLayer1DTestFactory.CreateSoilLayer1DWithValidAquifer();
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
        public void SoilLayer1DTransform_ValidShearStrengthModelValue_ReturnMacroStabilityInwardSoilLayer1D(double? sheartStrengthModel,
                                                                                                            MacroStabilityInwardsShearStrengthModel transformedShearStrengthModel)
        {
            // Setup
            SoilLayer1D layer = SoilLayer1DTestFactory.CreateSoilLayer1DWithValidAquifer();
            layer.ShearStrengthModel = sheartStrengthModel;

            // Call
            MacroStabilityInwardsSoilLayer1D soilLayer1D = MacroStabilityInwardsSoilLayerTransformer.Transform(layer);

            // Assert
            Assert.AreEqual(transformedShearStrengthModel, soilLayer1D.Properties.ShearStrengthModel);
        }

        [Test]
        public void SoilLayer1DTransform_InvalidShearStrengthModelValue_ThrowsImportedDataTransformException()
        {
            // Setup
            SoilLayer1D layer = SoilLayer1DTestFactory.CreateSoilLayer1DWithValidAquifer();
            layer.ShearStrengthModel = 2;

            // Call
            TestDelegate test = () => MacroStabilityInwardsSoilLayerTransformer.Transform(layer);

            // Assert
            var exception = Assert.Throws<ImportedDataTransformException>(test);
            Assert.AreEqual("Ongeldige waarde voor parameter 'Schuifsterkte model'.", exception.Message);
        }

        [Test]
        [TestCase(1.0, true)]
        [TestCase(0.0, false)]
        public void SoilLayer1DTransform_ValidIsAquifer_ReturnsMacroStabilityInwardsSoilLayer1D(double isAquifer, bool transformedIsAquifer)
        {
            // Setup
            SoilLayer1D layer = SoilLayer1DTestFactory.CreateSoilLayer1DWithValidAquifer();
            layer.IsAquifer = isAquifer;

            // Call
            MacroStabilityInwardsSoilLayer1D soilLayer1D = MacroStabilityInwardsSoilLayerTransformer.Transform(layer);

            // Assert
            Assert.AreEqual(transformedIsAquifer, soilLayer1D.Properties.IsAquifer);
        }

        [Test]
        [TestCase(null)]
        [TestCase(1.01)]
        [TestCase(0.01)]
        public void SoilLayer1DTransform_InvalidIsAquifer_ThrowsImportedDataException(double? isAquifer)
        {
            // Setup
            SoilLayer1D layer = SoilLayer1DTestFactory.CreateSoilLayer1DWithValidAquifer();
            layer.IsAquifer = isAquifer;

            // Call
            TestDelegate call = () => MacroStabilityInwardsSoilLayerTransformer.Transform(layer);

            // Assert
            var exception = Assert.Throws<ImportedDataTransformException>(call);
            Assert.AreEqual("Ongeldige waarde voor parameter 'Is aquifer'.", exception.Message);
        }

        [Test]
        [TestCaseSource(nameof(GetColorCases))]
        public void SoilLayer1DTransform_ValidColors_ReturnsMacroStabilityInwardsSoilLayer1D(double? color, Color transformedColor)
        {
            // Setup
            SoilLayer1D layer = SoilLayer1DTestFactory.CreateSoilLayer1DWithValidAquifer();
            layer.Color = color;

            // Call
            MacroStabilityInwardsSoilLayer1D soilLayer1D = MacroStabilityInwardsSoilLayerTransformer.Transform(layer);

            // Assert
            Assert.AreEqual(transformedColor, soilLayer1D.Properties.Color);
        }

        [Test]
        [TestCaseSource(nameof(IncorrectShiftedLogNormalDistributionsSoilLayer1D))]
        public void SoilLayer1DTransform_IncorrectShiftedLogNormalDistribution_ThrowsImportedDataTransformException(SoilLayer1D layer, string parameter)
        {
            // Call
            TestDelegate test = () => MacroStabilityInwardsSoilLayerTransformer.Transform(layer);

            // Assert
            Exception exception = Assert.Throws<ImportedDataTransformException>(test);
            Assert.AreEqual($"Parameter '{parameter}' is niet verschoven lognormaal verdeeld.", exception.Message);
        }

        [Test]
        [TestCaseSource(nameof(IncorrectNonShiftedLogNormalDistributionsSoilLayer1D))]
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

            double isAquifer = random.Next(0, 2);
            const string materialName = "materialX";
            double color = random.NextDouble();

            double abovePhreaticLevelMean = random.NextDouble();
            double abovePhreaticLevelDeviation = random.NextDouble();
            double abovePhreaticLevelShift = random.NextDouble();
            double belowPhreaticLevelMean = random.NextDouble();
            double belowPhreaticLevelCoefficientOfVariation = random.NextDouble();
            double belowPhreaticLevelShift = random.NextDouble();
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
            layer.AbovePhreaticLevelShift = abovePhreaticLevelShift;
            layer.BelowPhreaticLevelMean = belowPhreaticLevelMean;
            layer.BelowPhreaticLevelCoefficientOfVariation = belowPhreaticLevelCoefficientOfVariation;
            layer.BelowPhreaticLevelShift = belowPhreaticLevelShift;
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

            Assert.AreEqual(materialName, properties.MaterialName);
            bool expectedIsAquifer = isAquifer.Equals(1.0);
            Assert.AreEqual(expectedIsAquifer, properties.IsAquifer);
            Color expectedColor = Color.FromArgb(Convert.ToInt32(color));
            Assert.AreEqual(expectedColor, properties.Color);

            Assert.AreEqual(abovePhreaticLevelMean, properties.AbovePhreaticLevelMean);
            Assert.AreEqual(abovePhreaticLevelDeviation, properties.AbovePhreaticLevelCoefficientOfVariation);
            Assert.AreEqual(abovePhreaticLevelShift, properties.AbovePhreaticLevelShift);
            Assert.AreEqual(belowPhreaticLevelMean, properties.BelowPhreaticLevelMean);
            Assert.AreEqual(belowPhreaticLevelCoefficientOfVariation, properties.BelowPhreaticLevelCoefficientOfVariation);
            Assert.AreEqual(belowPhreaticLevelShift, properties.BelowPhreaticLevelShift);
            Assert.AreEqual(cohesionMean, properties.CohesionMean);
            Assert.AreEqual(cohesionDeviation, properties.CohesionCoefficientOfVariation);
            Assert.AreEqual(frictionAngleMean, properties.FrictionAngleMean);
            Assert.AreEqual(frictionAngleDeviation, properties.FrictionAngleCoefficientOfVariation);
            Assert.AreEqual(shearStrengthRatioMean, properties.ShearStrengthRatioMean);
            Assert.AreEqual(shearStrengthRatioDeviation, properties.ShearStrengthRatioCoefficientOfVariation);
            Assert.AreEqual(strengthIncreaseExponentMean, properties.StrengthIncreaseExponentMean);
            Assert.AreEqual(strengthIncreaseExponentDeviation, properties.StrengthIncreaseExponentCoefficientOfVariation);
            Assert.AreEqual(popMean, properties.PopMean);
            Assert.AreEqual(popDeviation, properties.PopCoefficientOfVariation);

            AssertRings(layer, soilLayer2D);
        }

        [Test]
        [TestCase(null, true)]
        [TestCase(0, false)]
        public void SoilLayer2DTransform_ValidUsePopValue_ReturnMacroStabilityInwardSoilLayer2D(double? usePop, bool transformedUsePopValue)
        {
            // Setup
            SoilLayer2D layer = SoilLayer2DTestFactory.CreateSoilLayer2DWithValidAquifer();
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
            SoilLayer2D layer = SoilLayer2DTestFactory.CreateSoilLayer2DWithValidAquifer();
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
            SoilLayer2D layer = SoilLayer2DTestFactory.CreateSoilLayer2DWithValidAquifer();
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
        [TestCase(1.0, true)]
        [TestCase(0.0, false)]
        public void SoilLayer2DTransform_ValidIsAquifer_ReturnsMacroStabilityInwardsSoilLayer2D(double isAquifer, bool transformedIsAquifer)
        {
            // Setup
            SoilLayer2D layer = SoilLayer2DTestFactory.CreateSoilLayer2DWithValidAquifer();
            layer.IsAquifer = isAquifer;

            // Call
            MacroStabilityInwardsSoilLayer2D soilLayer2D = MacroStabilityInwardsSoilLayerTransformer.Transform(layer);

            // Assert
            Assert.AreEqual(transformedIsAquifer, soilLayer2D.Properties.IsAquifer);
        }

        [Test]
        [TestCase(null)]
        [TestCase(1.01)]
        [TestCase(0.01)]
        public void SoilLayer2DTransform_InvalidIsAquifer_ThrowsImportedDataException(double? isAquifer)
        {
            // Setup
            SoilLayer2D layer = SoilLayer2DTestFactory.CreateSoilLayer2DWithValidAquifer();
            layer.IsAquifer = isAquifer;

            // Call
            TestDelegate call = () => MacroStabilityInwardsSoilLayerTransformer.Transform(layer);

            // Assert
            var exception = Assert.Throws<ImportedDataTransformException>(call);
            Assert.AreEqual("Ongeldige waarde voor parameter 'Is aquifer'.", exception.Message);
        }

        [Test]
        [TestCaseSource(nameof(GetColorCases))]
        public void SoilLayer2DTransform_ValidColors_ReturnsMacroStabilityInwardsSoilLayer2D(double? color, Color transformedColor)
        {
            // Setup
            SoilLayer2D layer = SoilLayer2DTestFactory.CreateSoilLayer2DWithValidAquifer();
            layer.Color = color;

            // Call
            MacroStabilityInwardsSoilLayer2D soilLayer2D = MacroStabilityInwardsSoilLayerTransformer.Transform(layer);

            // Assert
            Assert.AreEqual(transformedColor, soilLayer2D.Properties.Color);
        }

        [Test]
        [TestCaseSource(nameof(IncorrectShiftedLogNormalDistributionsSoilLayer2D))]
        public void SoilLayer2DTransform_IncorrectShiftedLogNormalDistribution_ThrowsImportedDataTransformException(
            SoilLayer2D layer, string parameter)
        {
            // Call
            TestDelegate test = () => MacroStabilityInwardsSoilLayerTransformer.Transform(layer);

            // Assert
            Exception exception = Assert.Throws<ImportedDataTransformException>(test);
            Assert.AreEqual($"Parameter '{parameter}' is niet verschoven lognormaal verdeeld.", exception.Message);
        }

        [Test]
        [TestCaseSource(nameof(IncorrectNonShiftedLogNormalDistributionsSoilLayer2D))]
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

        #region Test Data: Color test cases

        private static IEnumerable<TestCaseData> GetColorCases()
        {
            yield return new TestCaseData(null, Color.Empty)
                .SetName("Color result Empty");
            yield return new TestCaseData((double) -12156236, Color.FromArgb(70, 130, 180))
                .SetName("Color result Purple");
            yield return new TestCaseData((double) -65281, Color.FromArgb(255, 0, 255))
                .SetName("Color result Pink");
        }

        #endregion

        #region Test Data: Shifted Log Normal Distributions

        private static IEnumerable<TestCaseData> IncorrectShiftedLogNormalDistributionsSoilLayer1D()
        {
            return IncorrectShiftedLogNormalDistributions(() => new SoilLayer1D(0.0), nameof(SoilLayer1D));
        }

        private static IEnumerable<TestCaseData> IncorrectShiftedLogNormalDistributionsSoilLayer2D()
        {
            return IncorrectShiftedLogNormalDistributions(() => new SoilLayer2D(), nameof(SoilLayer2D));
        }

        private static IEnumerable<TestCaseData> IncorrectShiftedLogNormalDistributions(Func<SoilLayerBase> soilLayer, string typeName)
        {
            const string testNameFormat = "{0}Transform_Incorrect{1}{{1}}_ThrowsImportedDataTransformException";

            SoilLayerBase invalidBelowPhreaticLevel = soilLayer();
            invalidBelowPhreaticLevel.BelowPhreaticLevelDistributionType = -1;
            yield return new TestCaseData(invalidBelowPhreaticLevel, "Verzadigd gewicht")
                .SetName(string.Format(testNameFormat, typeName, "Distribution"));

            SoilLayerBase invalidAbovePhreaticLevel = soilLayer();
            invalidAbovePhreaticLevel.AbovePhreaticLevelDistributionType = -1;
            yield return new TestCaseData(invalidAbovePhreaticLevel, "Onverzadigd gewicht")
                .SetName(string.Format(testNameFormat, typeName, "Distribution"));
        }

        #endregion

        #region Test Data:NonShifted Log Normal Distributions

        private static IEnumerable<TestCaseData> IncorrectNonShiftedLogNormalDistributionsSoilLayer1D()
        {
            return IncorrectNonShiftedLogNormalDistributions(() => new SoilLayer1D(0.0), nameof(SoilLayer1D));
        }

        private static IEnumerable<TestCaseData> IncorrectNonShiftedLogNormalDistributionsSoilLayer2D()
        {
            return IncorrectNonShiftedLogNormalDistributions(() => new SoilLayer2D(), nameof(SoilLayer2D));
        }

        private static IEnumerable<TestCaseData> IncorrectNonShiftedLogNormalDistributions(Func<SoilLayerBase> soilLayer, string typeName)
        {
            const string testNameFormat = "{0}Transform_Incorrect{1}{{1}}_ThrowsImportedDataTransformException";
            const long validDistributionType = SoilLayerConstants.LogNormalDistributionValue;
            const double validShift = 0.0;

            SoilLayerBase invalidCohesionShift = soilLayer();
            invalidCohesionShift.CohesionDistributionType = validDistributionType;
            invalidCohesionShift.CohesionShift = -1;
            yield return new TestCaseData(invalidCohesionShift, "Cohesie"
            ).SetName(string.Format(testNameFormat, typeName, "Shift"));

            SoilLayerBase invalidCohesionDistribution = soilLayer();
            invalidCohesionDistribution.CohesionDistributionType = -1;
            invalidCohesionDistribution.CohesionShift = validShift;
            yield return new TestCaseData(invalidCohesionDistribution, "Cohesie"
            ).SetName(string.Format(testNameFormat, typeName, "Distribution"));

            SoilLayerBase invalidFrictionAngleShift = soilLayer();
            invalidFrictionAngleShift.FrictionAngleDistributionType = validDistributionType;
            invalidFrictionAngleShift.FrictionAngleShift = -1;
            yield return new TestCaseData(invalidFrictionAngleShift, "Wrijvingshoek"
            ).SetName(string.Format(testNameFormat, typeName, "Shift"));

            SoilLayerBase invalidFrictionAngleDistribution = soilLayer();
            invalidFrictionAngleDistribution.FrictionAngleDistributionType = -1;
            invalidFrictionAngleDistribution.FrictionAngleShift = validShift;
            yield return new TestCaseData(invalidFrictionAngleDistribution, "Wrijvingshoek"
            ).SetName(string.Format(testNameFormat, typeName, "Distribution"));

            SoilLayerBase invalidShearStrengthRatioShift = soilLayer();
            invalidShearStrengthRatioShift.ShearStrengthRatioDistributionType = validDistributionType;
            invalidShearStrengthRatioShift.ShearStrengthRatioShift = -1;
            yield return new TestCaseData(invalidShearStrengthRatioShift, "Schuifsterkte ratio S"
            ).SetName(string.Format(testNameFormat, typeName, "Shift"));

            SoilLayerBase invalidShearStrengthRatioDistribution = soilLayer();
            invalidShearStrengthRatioDistribution.ShearStrengthRatioDistributionType = -1;
            invalidShearStrengthRatioDistribution.ShearStrengthRatioShift = validShift;
            yield return new TestCaseData(invalidShearStrengthRatioDistribution, "Schuifsterkte ratio S"
            ).SetName(string.Format(testNameFormat, typeName, "Distribution"));

            SoilLayerBase invalidStrengthIncreaseExponentShift = soilLayer();
            invalidStrengthIncreaseExponentShift.StrengthIncreaseExponentDistributionType = validDistributionType;
            invalidStrengthIncreaseExponentShift.StrengthIncreaseExponentShift = -1;
            yield return new TestCaseData(invalidStrengthIncreaseExponentShift, "Sterkte toename exponent"
            ).SetName(string.Format(testNameFormat, typeName, "Shift"));

            SoilLayerBase invalidStrengthIncreaseExponentDistribution = soilLayer();
            invalidStrengthIncreaseExponentDistribution.StrengthIncreaseExponentDistributionType = -1;
            invalidStrengthIncreaseExponentDistribution.StrengthIncreaseExponentShift = validShift;
            yield return new TestCaseData(invalidStrengthIncreaseExponentDistribution, "Sterkte toename exponent"
            ).SetName(string.Format(testNameFormat, typeName, "Distribution"));

            SoilLayerBase invalidPopShift = soilLayer();
            invalidPopShift.PopDistributionType = validDistributionType;
            invalidPopShift.PopShift = -1;
            yield return new TestCaseData(invalidPopShift, "POP"
            ).SetName(string.Format(testNameFormat, typeName, "Shift"));

            SoilLayerBase invalidPopDistribution = soilLayer();
            invalidPopDistribution.PopDistributionType = -1;
            invalidPopDistribution.PopShift = validShift;
            yield return new TestCaseData(invalidPopDistribution, "POP"
            ).SetName(string.Format(testNameFormat, typeName, "Distribution"));
        }

        #endregion
    }
}