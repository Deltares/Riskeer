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
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.IO.Exceptions;
using Riskeer.Common.IO.SoilProfile;
using Riskeer.Common.IO.TestUtil;
using Riskeer.MacroStabilityInwards.Data.SoilProfile;
using Riskeer.MacroStabilityInwards.IO.SoilProfiles;
using Riskeer.MacroStabilityInwards.Primitives;

namespace Riskeer.MacroStabilityInwards.IO.Test.SoilProfiles
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

            const double abovePhreaticLevelMean = 0.3;
            const double abovePhreaticLevelCoefficientOfVariation = 0.2;
            const double abovePhreaticLevelShift = 0.1;
            const double belowPhreaticLevelMean = 0.4;
            const double belowPhreaticLevelCoefficientOfVariation = 0.3;
            const double belowPhreaticLevelShift = 0.2;
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

            MacroStabilityInwardsSoilLayerData data = soilLayer1D.Data;

            Assert.AreEqual(materialName, data.MaterialName);
            bool expectedIsAquifer = isAquifer.Equals(1.0);
            Assert.AreEqual(expectedIsAquifer, data.IsAquifer);
            Color expectedColor = Color.FromArgb(Convert.ToInt32(color));
            Assert.AreEqual(expectedColor, data.Color);

            Assert.AreEqual(abovePhreaticLevelMean, data.AbovePhreaticLevel.Mean,
                            data.AbovePhreaticLevel.GetAccuracy());
            Assert.AreEqual(abovePhreaticLevelCoefficientOfVariation, data.AbovePhreaticLevel.CoefficientOfVariation,
                            data.AbovePhreaticLevel.GetAccuracy());
            Assert.AreEqual(abovePhreaticLevelShift, data.AbovePhreaticLevel.Shift,
                            data.AbovePhreaticLevel.GetAccuracy());

            Assert.AreEqual(belowPhreaticLevelMean, data.BelowPhreaticLevel.Mean,
                            data.BelowPhreaticLevel.GetAccuracy());
            Assert.AreEqual(belowPhreaticLevelCoefficientOfVariation, data.BelowPhreaticLevel.CoefficientOfVariation,
                            data.BelowPhreaticLevel.GetAccuracy());
            Assert.AreEqual(belowPhreaticLevelShift, data.BelowPhreaticLevel.Shift,
                            data.BelowPhreaticLevel.GetAccuracy());

            Assert.AreEqual(cohesionMean, data.Cohesion.Mean,
                            data.Cohesion.GetAccuracy());
            Assert.AreEqual(cohesionCoefficientOfVariation, data.Cohesion.CoefficientOfVariation,
                            data.Cohesion.GetAccuracy());

            Assert.AreEqual(frictionAngleMean, data.FrictionAngle.Mean,
                            data.FrictionAngle.GetAccuracy());
            Assert.AreEqual(frictionAngleCoefficientOfVariation, data.FrictionAngle.CoefficientOfVariation,
                            data.FrictionAngle.GetAccuracy());

            Assert.AreEqual(shearStrengthRatioMean, data.ShearStrengthRatio.Mean,
                            data.ShearStrengthRatio.GetAccuracy());
            Assert.AreEqual(shearStrengthRatioCoefficientOfVariation, data.ShearStrengthRatio.CoefficientOfVariation,
                            data.ShearStrengthRatio.GetAccuracy());

            Assert.AreEqual(strengthIncreaseExponentMean, data.StrengthIncreaseExponent.Mean,
                            data.StrengthIncreaseExponent.GetAccuracy());
            Assert.AreEqual(strengthIncreaseExponentCoefficientOfVariation, data.StrengthIncreaseExponent.CoefficientOfVariation,
                            data.StrengthIncreaseExponent.GetAccuracy());

            Assert.AreEqual(popMean, data.Pop.Mean, data.Pop.GetAccuracy());
            Assert.AreEqual(popCoefficientOfVariation, data.Pop.CoefficientOfVariation,
                            data.Pop.GetAccuracy());
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
            Assert.AreEqual(transformedUsePopValue, soilLayer1D.Data.UsePop);
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
            string expectedMessage = CreateExpectedErrorMessage(layer.MaterialName,
                                                                "Ongeldige waarde voor parameter 'Gebruik POP'.");
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        [TestCase(null, MacroStabilityInwardsShearStrengthModel.CPhi)]
        [TestCase(9, MacroStabilityInwardsShearStrengthModel.CPhiOrSuCalculated)]
        [TestCase(6, MacroStabilityInwardsShearStrengthModel.SuCalculated)]
        public void SoilLayer1DTransform_ValidShearStrengthModelValue_ReturnMacroStabilityInwardSoilLayer1D(double? sheartStrengthModel,
                                                                                                            MacroStabilityInwardsShearStrengthModel transformedShearStrengthModel)
        {
            // Setup
            SoilLayer1D layer = SoilLayer1DTestFactory.CreateSoilLayer1DWithValidAquifer();
            layer.ShearStrengthModel = sheartStrengthModel;

            // Call
            MacroStabilityInwardsSoilLayer1D soilLayer1D = MacroStabilityInwardsSoilLayerTransformer.Transform(layer);

            // Assert
            Assert.AreEqual(transformedShearStrengthModel, soilLayer1D.Data.ShearStrengthModel);
        }

        [Test]
        public void SoilLayer1DTransform_ShearStrengthModelValueNone_ThrowsImportedDataTransformException()
        {
            // Setup
            SoilLayer1D layer = SoilLayer1DTestFactory.CreateSoilLayer1DWithValidAquifer();
            layer.ShearStrengthModel = 1;

            // Call
            TestDelegate call = () => MacroStabilityInwardsSoilLayerTransformer.Transform(layer);

            // Assert
            var exception = Assert.Throws<ImportedDataTransformException>(call);
            string expectedMessage = CreateExpectedErrorMessage(layer.MaterialName,
                                                                "Er is geen schuifsterkte model opgegeven.");
            Assert.AreEqual(expectedMessage, exception.Message);
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
            string expectedMessage = CreateExpectedErrorMessage(layer.MaterialName,
                                                                "Ongeldige waarde voor parameter 'Schuifsterkte model'.");
            Assert.AreEqual(expectedMessage, exception.Message);
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
            Assert.AreEqual(transformedIsAquifer, soilLayer1D.Data.IsAquifer);
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
            string expectedMessage = CreateExpectedErrorMessage(layer.MaterialName,
                                                                "Ongeldige waarde voor parameter 'Is aquifer'.");
            Assert.AreEqual(expectedMessage, exception.Message);
            Assert.IsInstanceOf<NotSupportedException>(exception.InnerException);
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
            Assert.AreEqual(transformedColor, soilLayer1D.Data.Color);
        }

        [Test]
        [TestCaseSource(nameof(IncorrectShiftedLogNormalDistributionsSoilLayer1D))]
        public void SoilLayer1DTransform_IncorrectShiftedLogNormalDistribution_ThrowsImportedDataTransformException(SoilLayer1D layer, string parameterName)
        {
            // Call
            TestDelegate test = () => MacroStabilityInwardsSoilLayerTransformer.Transform(layer);

            // Assert
            Exception exception = Assert.Throws<ImportedDataTransformException>(test);
            string expectedMessage = CreateExpectedErrorMessageForParameterVariable(layer.MaterialName, parameterName, "Parameter moet verschoven lognormaal verdeeld zijn.");
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        [TestCaseSource(nameof(IncorrectNonShiftedLogNormalDistributionsTypeSoilLayer1D))]
        public void SoilLayer1DTransform_IncorrectLogNormalDistributionType_ThrowImportedDataTransformException(SoilLayer1D layer, string parameter)
        {
            // Call
            TestDelegate test = () => MacroStabilityInwardsSoilLayerTransformer.Transform(layer);

            // Assert
            Exception exception = Assert.Throws<ImportedDataTransformException>(test);
            string expectedMessage = CreateExpectedErrorMessageForParameterVariable(layer.MaterialName, parameter, "Parameter moet lognormaal verdeeld zijn.");
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        [TestCaseSource(nameof(IncorrectNonShiftedLogNormalDistributionsShiftSoilLayer1D))]
        public void SoilLayer1DTransform_IncorrectLogNormalDistributionShift_ThrowImportedDataTransformException(SoilLayer1D layer, string parameterName)
        {
            // Call
            TestDelegate test = () => MacroStabilityInwardsSoilLayerTransformer.Transform(layer);

            // Assert
            Exception exception = Assert.Throws<ImportedDataTransformException>(test);
            string expectedMessage = CreateExpectedErrorMessageForParameterVariable(layer.MaterialName, parameterName, "Parameter moet lognormaal verdeeld zijn met een verschuiving gelijk aan 0.");
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        [TestCaseSource(nameof(InvalidStochasticDistributionValuesSoilLayer1D))]
        public void SoilLayer1DTransform_InvalidStochasticDistributionValues_ThrowImportedDataTransformException(SoilLayer1D layer, string parameterName)
        {
            // Call
            TestDelegate test = () => MacroStabilityInwardsSoilLayerTransformer.Transform(layer);

            // Assert
            Exception exception = Assert.Throws<ImportedDataTransformException>(test);

            Exception innerException = exception.InnerException;
            Assert.IsInstanceOf<ArgumentOutOfRangeException>(innerException);
            string expectedMessage = CreateExpectedErrorMessageForParameterVariable(layer.MaterialName, parameterName, innerException.Message);
            Assert.AreEqual(expectedMessage, exception.Message);
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
            var nestedLayer1 = new SoilLayer2D(CreateRandomLoop(21), Enumerable.Empty<SoilLayer2D>());
            var nestedLayer2 = new SoilLayer2D(CreateRandomLoop(22), Enumerable.Empty<SoilLayer2D>());
            var nestedLayer3 = new SoilLayer2D(CreateRandomLoop(22),
                                               new[]
                                               {
                                                   nestedLayer2
                                               });
            var layer = new SoilLayer2D(CreateRandomLoop(23),
                                        new[]
                                        {
                                            nestedLayer1,
                                            nestedLayer3
                                        });

            SetRandomSoilData(nestedLayer1, 21, "Nested sand");
            SetRandomSoilData(nestedLayer2, 22, "Nested gold");
            SetRandomSoilData(nestedLayer3, 23, "Nested clay");
            SetRandomSoilData(layer, 24, "Sand");

            // Call
            MacroStabilityInwardsSoilLayer2D transformedLayer = MacroStabilityInwardsSoilLayerTransformer.Transform(layer);

            // Assert
            AssertSoilLayer(layer, transformedLayer);
            Assert.AreEqual(2, transformedLayer.NestedLayers.Count());

            MacroStabilityInwardsSoilLayer2D transformedNestedLayer1 = transformedLayer.NestedLayers.ElementAt(0);
            AssertSoilLayer(nestedLayer1, transformedNestedLayer1);
            CollectionAssert.IsEmpty(transformedNestedLayer1.NestedLayers);

            MacroStabilityInwardsSoilLayer2D transformedNestedLayer3 = transformedLayer.NestedLayers.ElementAt(1);
            AssertSoilLayer(nestedLayer3, transformedNestedLayer3);
            Assert.AreEqual(1, transformedNestedLayer3.NestedLayers.Count());

            MacroStabilityInwardsSoilLayer2D transformedNestedLayer2 = transformedNestedLayer3.NestedLayers.ElementAt(0);
            AssertSoilLayer(nestedLayer2, transformedNestedLayer2);
            CollectionAssert.IsEmpty(transformedNestedLayer2.NestedLayers);
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
            Assert.AreEqual(transformedUsePopValue, soilLayer2D.Data.UsePop);
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
            string expectedMessage = CreateExpectedErrorMessage(layer.MaterialName,
                                                                "Ongeldige waarde voor parameter 'Gebruik POP'.");
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        [TestCase(null, MacroStabilityInwardsShearStrengthModel.CPhi)]
        [TestCase(9, MacroStabilityInwardsShearStrengthModel.CPhiOrSuCalculated)]
        [TestCase(6, MacroStabilityInwardsShearStrengthModel.SuCalculated)]
        public void SoilLayer2DTransform_ValidShearStrengthModelValue_ReturnMacroStabilityInwardSoilLayer2D(double? shearStrengthModel,
                                                                                                            MacroStabilityInwardsShearStrengthModel transformedShearStrengthModel)
        {
            // Setup
            SoilLayer2D layer = SoilLayer2DTestFactory.CreateSoilLayer2D();
            layer.ShearStrengthModel = shearStrengthModel;

            // Call
            MacroStabilityInwardsSoilLayer2D soilLayer2D = MacroStabilityInwardsSoilLayerTransformer.Transform(layer);

            // Assert
            Assert.AreEqual(transformedShearStrengthModel, soilLayer2D.Data.ShearStrengthModel);
        }

        [Test]
        public void SoilLayer2DTransform_ShearStrengthModelValueNone_ThrowsImportedDataTransformException()
        {
            // Setup
            SoilLayer2D layer = SoilLayer2DTestFactory.CreateSoilLayer2D();

            layer.ShearStrengthModel = 1;

            // Call
            TestDelegate call = () => MacroStabilityInwardsSoilLayerTransformer.Transform(layer);

            // Assert
            var exception = Assert.Throws<ImportedDataTransformException>(call);
            string expectedMessage = CreateExpectedErrorMessage(layer.MaterialName,
                                                                "Er is geen schuifsterkte model opgegeven.");
            Assert.AreEqual(expectedMessage, exception.Message);
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
            string expectedMessage = CreateExpectedErrorMessage(layer.MaterialName,
                                                                "Ongeldige waarde voor parameter 'Schuifsterkte model'.");
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        [TestCase(1.0, true)]
        [TestCase(0.0, false)]
        public void SoilLayer2DTransform_ValidIsAquifer_ReturnsMacroStabilityInwardsSoilLayer2D(double isAquifer, bool transformedIsAquifer)
        {
            // Setup
            SoilLayer2D layer = SoilLayer2DTestFactory.CreateSoilLayer2D();
            layer.IsAquifer = isAquifer;

            // Call
            MacroStabilityInwardsSoilLayer2D soilLayer2D = MacroStabilityInwardsSoilLayerTransformer.Transform(layer);

            // Assert
            Assert.AreEqual(transformedIsAquifer, soilLayer2D.Data.IsAquifer);
        }

        [Test]
        [TestCase(null)]
        [TestCase(1.01)]
        [TestCase(0.01)]
        public void SoilLayer2DTransform_InvalidIsAquifer_ThrowsImportedDataException(double? isAquifer)
        {
            // Setup
            SoilLayer2D layer = SoilLayer2DTestFactory.CreateSoilLayer2D();
            layer.IsAquifer = isAquifer;

            // Call
            TestDelegate call = () => MacroStabilityInwardsSoilLayerTransformer.Transform(layer);

            // Assert
            var exception = Assert.Throws<ImportedDataTransformException>(call);
            string expectedMessage = CreateExpectedErrorMessage(layer.MaterialName,
                                                                "Ongeldige waarde voor parameter 'Is aquifer'.");
            Assert.AreEqual(expectedMessage, exception.Message);
            Assert.IsInstanceOf<NotSupportedException>(exception.InnerException);
        }

        [Test]
        [TestCaseSource(nameof(GetColorCases))]
        public void SoilLayer2DTransform_ValidColors_ReturnsMacroStabilityInwardsSoilLayer2D(double? color, Color transformedColor)
        {
            // Setup
            SoilLayer2D layer = SoilLayer2DTestFactory.CreateSoilLayer2D();
            layer.Color = color;

            // Call
            MacroStabilityInwardsSoilLayer2D soilLayer2D = MacroStabilityInwardsSoilLayerTransformer.Transform(layer);

            // Assert
            Assert.AreEqual(transformedColor, soilLayer2D.Data.Color);
        }

        [Test]
        [TestCaseSource(nameof(IncorrectShiftedLogNormalDistributionsTypeSoilLayer2D))]
        public void SoilLayer2DTransform_IncorrectShiftedLogNormalDistribution_ThrowsImportedDataTransformException(
            SoilLayer2D layer, string parameterName)
        {
            // Call
            TestDelegate test = () => MacroStabilityInwardsSoilLayerTransformer.Transform(layer);

            // Assert
            Exception exception = Assert.Throws<ImportedDataTransformException>(test);
            string expectedMessage = CreateExpectedErrorMessageForParameterVariable(layer.MaterialName, parameterName, "Parameter moet verschoven lognormaal verdeeld zijn.");
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        [TestCaseSource(nameof(IncorrectNonShiftedLogNormalDistributionsTypeSoilLayer2D))]
        public void SoilLayer2DTransform_IncorrectLogNormalDistributionType_ThrowImportedDataTransformException(SoilLayer2D layer, string parameterName)
        {
            // Call
            TestDelegate test = () => MacroStabilityInwardsSoilLayerTransformer.Transform(layer);

            // Assert
            Exception exception = Assert.Throws<ImportedDataTransformException>(test);
            string expectedMessage = CreateExpectedErrorMessageForParameterVariable(layer.MaterialName, parameterName, "Parameter moet lognormaal verdeeld zijn.");
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        [TestCaseSource(nameof(IncorrectNonShiftedLogNormalDistributionsShiftSoilLayer2D))]
        public void SoilLayer2DTransform_IncorrectLogNormalDistributionShift_ThrowImportedDataTransformException(SoilLayer2D layer, string parameterName)
        {
            // Call
            TestDelegate test = () => MacroStabilityInwardsSoilLayerTransformer.Transform(layer);

            // Assert
            Exception exception = Assert.Throws<ImportedDataTransformException>(test);
            string expectedMessage = CreateExpectedErrorMessageForParameterVariable(layer.MaterialName, parameterName, "Parameter moet lognormaal verdeeld zijn met een verschuiving gelijk aan 0.");
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        [TestCaseSource(nameof(GetSoilLayerWithInvalidGeometry))]
        public void SoilLayer2DTransform_SoilLayer2DWithInvalidLoops_ThrowsImportedDataException(SoilLayer2D soilLayer)
        {
            // Call
            TestDelegate call = () => MacroStabilityInwardsSoilLayerTransformer.Transform(soilLayer);

            // Assert
            var exception = Assert.Throws<ImportedDataTransformException>(call);
            string expectedMessage = CreateExpectedErrorMessage(soilLayer.MaterialName,
                                                                "De laag bevat een ongeldige geometrie.");
            Assert.AreEqual(expectedMessage, exception.Message);
            Assert.IsInstanceOf<ArgumentException>(exception.InnerException);
        }

        [Test]
        [TestCaseSource(nameof(InvalidStochasticDistributionValuesSoilLayer2D))]
        public void SoilLayer2DTransform_InvalidStochasticDistributionValues_ThrowImportedDataTransformException(SoilLayer2D layer, string parameterName)
        {
            // Call
            TestDelegate test = () => MacroStabilityInwardsSoilLayerTransformer.Transform(layer);

            // Assert
            Exception exception = Assert.Throws<ImportedDataTransformException>(test);

            Exception innerException = exception.InnerException;
            Assert.IsInstanceOf<ArgumentOutOfRangeException>(innerException);
            string expectedMessage = CreateExpectedErrorMessageForParameterVariable(layer.MaterialName, parameterName, innerException.Message);
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        private static SoilLayer2DLoop CreateRandomLoop(int seed)
        {
            var random = new Random(seed);
            var pointA = new Point2D(random.NextDouble(), random.NextDouble());
            var pointB = new Point2D(random.NextDouble(), random.NextDouble());
            var pointC = new Point2D(random.NextDouble(), random.NextDouble());

            return new SoilLayer2DLoop(new[]
            {
                new Segment2D(pointA, pointB),
                new Segment2D(pointB, pointC),
                new Segment2D(pointC, pointA)
            });
        }

        private static void SetRandomSoilData(SoilLayer2D layer, int seed, string materialName)
        {
            var random = new Random(seed);

            layer.MaterialName = materialName;
            layer.IsAquifer = random.Next(0, 2);
            layer.Color = random.NextDouble();
            layer.AbovePhreaticLevelMean = random.NextDouble() + 1;
            layer.AbovePhreaticLevelCoefficientOfVariation = random.NextDouble();
            layer.AbovePhreaticLevelShift = random.NextDouble();
            layer.BelowPhreaticLevelMean = random.NextDouble() + 1;
            layer.BelowPhreaticLevelCoefficientOfVariation = random.NextDouble();
            layer.BelowPhreaticLevelShift = random.NextDouble();
            layer.CohesionMean = random.NextDouble();
            layer.CohesionCoefficientOfVariation = random.NextDouble();
            layer.FrictionAngleMean = random.NextDouble();
            layer.FrictionAngleCoefficientOfVariation = random.NextDouble();
            layer.ShearStrengthRatioMean = random.NextDouble();
            layer.ShearStrengthRatioCoefficientOfVariation = random.NextDouble();
            layer.StrengthIncreaseExponentMean = random.NextDouble();
            layer.StrengthIncreaseExponentCoefficientOfVariation = random.NextDouble();
            layer.PopMean = random.NextDouble();
            layer.PopCoefficientOfVariation = random.NextDouble();
        }

        private static string CreateExpectedErrorMessage(string materialName, string errorMessage)
        {
            return $"Er is een fout opgetreden bij het inlezen van grondlaag '{materialName}': {errorMessage}";
        }

        private static string CreateExpectedErrorMessageForParameterVariable(string materialName, string parameterName, string errorMessage)
        {
            return $"Er is een fout opgetreden bij het inlezen van grondlaag '{materialName}' voor parameter '{parameterName}': {errorMessage}";
        }

        private static void AssertSoilLayer(SoilLayer2D original, MacroStabilityInwardsSoilLayer2D actual)
        {
            AssertOuterRing(original, actual);
            AssertSoilData(original, actual);
        }

        private static void AssertOuterRing(SoilLayer2D original, MacroStabilityInwardsSoilLayer2D actual)
        {
            Assert.AreEqual(GetRingFromSegments(original.OuterLoop.Segments), actual.OuterRing);
        }

        private static Ring GetRingFromSegments(IEnumerable<Segment2D> loop)
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

        private static void AssertSoilData(SoilLayer2D original, MacroStabilityInwardsSoilLayer2D actual)
        {
            Assert.AreEqual(original.MaterialName, actual.Data.MaterialName);
            Assert.AreEqual(original.IsAquifer.Equals(1.0), actual.Data.IsAquifer);
            Assert.AreEqual(Color.FromArgb(Convert.ToInt32(original.Color)), actual.Data.Color);

            MacroStabilityInwardsSoilLayerData soilLayerData = actual.Data;
            Assert.AreEqual(original.AbovePhreaticLevelMean, soilLayerData.AbovePhreaticLevel.Mean,
                            soilLayerData.AbovePhreaticLevel.GetAccuracy());
            Assert.AreEqual(original.AbovePhreaticLevelCoefficientOfVariation, soilLayerData.AbovePhreaticLevel.CoefficientOfVariation,
                            soilLayerData.AbovePhreaticLevel.GetAccuracy());
            Assert.AreEqual(original.AbovePhreaticLevelShift, soilLayerData.AbovePhreaticLevel.Shift,
                            soilLayerData.AbovePhreaticLevel.GetAccuracy());

            Assert.AreEqual(original.BelowPhreaticLevelMean, soilLayerData.BelowPhreaticLevel.Mean,
                            soilLayerData.BelowPhreaticLevel.GetAccuracy());
            Assert.AreEqual(original.BelowPhreaticLevelCoefficientOfVariation, soilLayerData.BelowPhreaticLevel.CoefficientOfVariation,
                            soilLayerData.BelowPhreaticLevel.GetAccuracy());
            Assert.AreEqual(original.BelowPhreaticLevelShift, soilLayerData.BelowPhreaticLevel.Shift,
                            soilLayerData.BelowPhreaticLevel.GetAccuracy());

            Assert.AreEqual(original.CohesionMean, soilLayerData.Cohesion.Mean,
                            soilLayerData.Cohesion.GetAccuracy());
            Assert.AreEqual(original.CohesionCoefficientOfVariation, soilLayerData.Cohesion.CoefficientOfVariation,
                            soilLayerData.Cohesion.GetAccuracy());

            Assert.AreEqual(original.FrictionAngleMean, soilLayerData.FrictionAngle.Mean,
                            soilLayerData.FrictionAngle.GetAccuracy());
            Assert.AreEqual(original.FrictionAngleCoefficientOfVariation, soilLayerData.FrictionAngle.CoefficientOfVariation,
                            soilLayerData.FrictionAngle.GetAccuracy());

            Assert.AreEqual(original.ShearStrengthRatioMean, soilLayerData.ShearStrengthRatio.Mean,
                            soilLayerData.ShearStrengthRatio.GetAccuracy());
            Assert.AreEqual(original.ShearStrengthRatioCoefficientOfVariation, soilLayerData.ShearStrengthRatio.CoefficientOfVariation,
                            soilLayerData.ShearStrengthRatio.GetAccuracy());

            Assert.AreEqual(original.StrengthIncreaseExponentMean, soilLayerData.StrengthIncreaseExponent.Mean,
                            soilLayerData.StrengthIncreaseExponent.GetAccuracy());
            Assert.AreEqual(original.StrengthIncreaseExponentCoefficientOfVariation, soilLayerData.StrengthIncreaseExponent.CoefficientOfVariation,
                            soilLayerData.StrengthIncreaseExponent.GetAccuracy());

            Assert.AreEqual(original.PopMean, soilLayerData.Pop.Mean, soilLayerData.Pop.GetAccuracy());
            Assert.AreEqual(original.PopCoefficientOfVariation, soilLayerData.Pop.CoefficientOfVariation,
                            soilLayerData.Pop.GetAccuracy());
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

        #region Test Data: Invalid 2D SoilLayer geometries

        private static IEnumerable<TestCaseData> GetSoilLayerWithInvalidGeometry()
        {
            var pointA = new Point2D(0.0, 0.0);
            var pointB = new Point2D(1.0, 0.0);
            var segmentOne = new Segment2D(pointA, pointB);
            var segmentTwo = new Segment2D(pointB, pointA);

            Segment2D[] validGeometry =
            {
                segmentOne,
                segmentTwo
            };

            yield return new TestCaseData(SoilLayer2DTestFactory.CreateSoilLayer2D(new IEnumerable<Segment2D>[0],
                                                                                   Enumerable.Empty<Segment2D>()))
                .SetName("OuterLoop_ContainsNoSegments");

            yield return new TestCaseData(SoilLayer2DTestFactory.CreateSoilLayer2D(new[]
                                                                                   {
                                                                                       Enumerable.Empty<Segment2D>()
                                                                                   },
                                                                                   validGeometry))
                .SetName("Innerloop_ContainsCollectionWithElementWithNoSegments");
        }

        #endregion

        #region Distribution properties

        private static IEnumerable<TestCaseData> InvalidStochasticDistributionValuesSoilLayer1D()
        {
            return InvalidStochasticDistributionValues(() => SoilLayer1DTestFactory.CreateSoilLayer1DWithValidAquifer(), nameof(SoilLayer1D));
        }

        private static IEnumerable<TestCaseData> InvalidStochasticDistributionValuesSoilLayer2D()
        {
            return InvalidStochasticDistributionValues(SoilLayer2DTestFactory.CreateSoilLayer2D, nameof(SoilLayer2D));
        }

        private static IEnumerable<TestCaseData> InvalidStochasticDistributionValues(Func<SoilLayerBase> soilLayer, string typeName)
        {
            const string testNameFormat = "{0}Transform_InvalidStochasticDistributionValues{{1}}_ThrowsImportedDataTransformException";
            const double invalidMean = 0;

            SoilLayerBase invalidCohesion = soilLayer();
            invalidCohesion.CohesionMean = invalidMean;
            yield return new TestCaseData(invalidCohesion, "Cohesie"
            ).SetName(string.Format(testNameFormat, typeName));

            SoilLayerBase invalidFrictionAngle = soilLayer();
            invalidFrictionAngle.FrictionAngleMean = invalidMean;
            yield return new TestCaseData(invalidFrictionAngle, "Wrijvingshoek"
            ).SetName(string.Format(testNameFormat, typeName));

            SoilLayerBase invalidShearStrengthRatio = soilLayer();
            invalidShearStrengthRatio.ShearStrengthRatioMean = invalidMean;
            yield return new TestCaseData(invalidShearStrengthRatio, "Schuifsterkte ratio (S)"
            ).SetName(string.Format(testNameFormat, typeName));

            SoilLayerBase invalidStrengthIncreaseExponent = soilLayer();
            invalidStrengthIncreaseExponent.StrengthIncreaseExponentMean = invalidMean;
            yield return new TestCaseData(invalidStrengthIncreaseExponent, "Sterkte toename exp (m)"
            ).SetName(string.Format(testNameFormat, typeName));

            SoilLayerBase invalidPop = soilLayer();
            invalidPop.PopMean = invalidMean;
            yield return new TestCaseData(invalidPop, "POP"
            ).SetName(string.Format(testNameFormat, typeName));

            const double validMean = 1;
            const double invalidShift = 2;
            SoilLayerBase invalidBelowPhreaticLevel = soilLayer();
            invalidBelowPhreaticLevel.BelowPhreaticLevelMean = validMean;
            invalidBelowPhreaticLevel.BelowPhreaticLevelShift = invalidShift;
            yield return new TestCaseData(invalidBelowPhreaticLevel, "Verzadigd gewicht")
                .SetName(string.Format(testNameFormat, typeName));

            SoilLayerBase invalidAbovePhreaticLevel = soilLayer();
            invalidAbovePhreaticLevel.AbovePhreaticLevelMean = validMean;
            invalidAbovePhreaticLevel.AbovePhreaticLevelShift = invalidShift;
            yield return new TestCaseData(invalidAbovePhreaticLevel, "Onverzadigd gewicht")
                .SetName(string.Format(testNameFormat, typeName));
        }

        #endregion

        #region Test Data: Shifted Log Normal Distributions

        private static IEnumerable<TestCaseData> IncorrectShiftedLogNormalDistributionsSoilLayer1D()
        {
            return IncorrectShiftedLogNormalDistributions(() => new SoilLayer1D(0.0), nameof(SoilLayer1D));
        }

        private static IEnumerable<TestCaseData> IncorrectShiftedLogNormalDistributionsTypeSoilLayer2D()
        {
            return IncorrectShiftedLogNormalDistributions(SoilLayer2DTestFactory.CreateSoilLayer2D, nameof(SoilLayer2D));
        }

        private static IEnumerable<TestCaseData> IncorrectShiftedLogNormalDistributions(Func<SoilLayerBase> soilLayer, string typeName)
        {
            const string testNameFormat = "{0}Transform_IncorrectDistribution{{1}}_ThrowsImportedDataTransformException";

            SoilLayerBase invalidBelowPhreaticLevel = soilLayer();
            invalidBelowPhreaticLevel.BelowPhreaticLevelDistributionType = -1;
            yield return new TestCaseData(invalidBelowPhreaticLevel, "Verzadigd gewicht")
                .SetName(string.Format(testNameFormat, typeName));

            SoilLayerBase invalidAbovePhreaticLevel = soilLayer();
            invalidAbovePhreaticLevel.AbovePhreaticLevelDistributionType = -1;
            yield return new TestCaseData(invalidAbovePhreaticLevel, "Onverzadigd gewicht")
                .SetName(string.Format(testNameFormat, typeName));
        }

        #endregion

        #region Test Data: NonShifted Log Normal Distributions

        private static IEnumerable<TestCaseData> IncorrectNonShiftedLogNormalDistributionsTypeSoilLayer1D()
        {
            return IncorrectNonShiftedLogNormalDistributionsType(() => new SoilLayer1D(0.0), nameof(SoilLayer1D));
        }

        private static IEnumerable<TestCaseData> IncorrectNonShiftedLogNormalDistributionsShiftSoilLayer1D()
        {
            return IncorrectNonShiftedLogNormalDistributionsShift(() => new SoilLayer1D(0.0), nameof(SoilLayer1D));
        }

        private static IEnumerable<TestCaseData> IncorrectNonShiftedLogNormalDistributionsTypeSoilLayer2D()
        {
            return IncorrectNonShiftedLogNormalDistributionsType(SoilLayer2DTestFactory.CreateSoilLayer2D, nameof(SoilLayer2D));
        }

        private static IEnumerable<TestCaseData> IncorrectNonShiftedLogNormalDistributionsShiftSoilLayer2D()
        {
            return IncorrectNonShiftedLogNormalDistributionsShift(SoilLayer2DTestFactory.CreateSoilLayer2D, nameof(SoilLayer2D));
        }

        private static IEnumerable<TestCaseData> IncorrectNonShiftedLogNormalDistributionsType(Func<SoilLayerBase> soilLayer, string typeName)
        {
            const string testNameFormat = "{0}Transform_IncorrectDistribution{{1}}_ThrowsImportedDataTransformException";
            const double validShift = 0.0;

            SoilLayerBase invalidCohesionDistribution = soilLayer();
            invalidCohesionDistribution.CohesionDistributionType = -1;
            invalidCohesionDistribution.CohesionShift = validShift;
            yield return new TestCaseData(invalidCohesionDistribution, "Cohesie"
            ).SetName(string.Format(testNameFormat, typeName));

            SoilLayerBase invalidFrictionAngleDistribution = soilLayer();
            invalidFrictionAngleDistribution.FrictionAngleDistributionType = -1;
            invalidFrictionAngleDistribution.FrictionAngleShift = validShift;
            yield return new TestCaseData(invalidFrictionAngleDistribution, "Wrijvingshoek"
            ).SetName(string.Format(testNameFormat, typeName));

            SoilLayerBase invalidShearStrengthRatioDistribution = soilLayer();
            invalidShearStrengthRatioDistribution.ShearStrengthRatioDistributionType = -1;
            invalidShearStrengthRatioDistribution.ShearStrengthRatioShift = validShift;
            yield return new TestCaseData(invalidShearStrengthRatioDistribution, "Schuifsterkte ratio (S)"
            ).SetName(string.Format(testNameFormat, typeName));

            SoilLayerBase invalidStrengthIncreaseExponentDistribution = soilLayer();
            invalidStrengthIncreaseExponentDistribution.StrengthIncreaseExponentDistributionType = -1;
            invalidStrengthIncreaseExponentDistribution.StrengthIncreaseExponentShift = validShift;
            yield return new TestCaseData(invalidStrengthIncreaseExponentDistribution, "Sterkte toename exp (m)"
            ).SetName(string.Format(testNameFormat, typeName));

            SoilLayerBase invalidPopDistribution = soilLayer();
            invalidPopDistribution.PopDistributionType = -1;
            invalidPopDistribution.PopShift = validShift;
            yield return new TestCaseData(invalidPopDistribution, "POP"
            ).SetName(string.Format(testNameFormat, typeName));
        }

        private static IEnumerable<TestCaseData> IncorrectNonShiftedLogNormalDistributionsShift(Func<SoilLayerBase> soilLayer, string typeName)
        {
            const string testNameFormat = "{0}Transform_IncorrectShift{{1}}_ThrowsImportedDataTransformException";
            const long validDistributionType = SoilLayerConstants.LogNormalDistributionValue;

            SoilLayerBase invalidCohesionShift = soilLayer();
            invalidCohesionShift.CohesionDistributionType = validDistributionType;
            invalidCohesionShift.CohesionShift = -1;
            yield return new TestCaseData(invalidCohesionShift, "Cohesie"
            ).SetName(string.Format(testNameFormat, typeName));

            SoilLayerBase invalidFrictionAngleShift = soilLayer();
            invalidFrictionAngleShift.FrictionAngleDistributionType = validDistributionType;
            invalidFrictionAngleShift.FrictionAngleShift = -1;
            yield return new TestCaseData(invalidFrictionAngleShift, "Wrijvingshoek"
            ).SetName(string.Format(testNameFormat, typeName));

            SoilLayerBase invalidShearStrengthRatioShift = soilLayer();
            invalidShearStrengthRatioShift.ShearStrengthRatioDistributionType = validDistributionType;
            invalidShearStrengthRatioShift.ShearStrengthRatioShift = -1;
            yield return new TestCaseData(invalidShearStrengthRatioShift, "Schuifsterkte ratio (S)"
            ).SetName(string.Format(testNameFormat, typeName));

            SoilLayerBase invalidStrengthIncreaseExponentShift = soilLayer();
            invalidStrengthIncreaseExponentShift.StrengthIncreaseExponentDistributionType = validDistributionType;
            invalidStrengthIncreaseExponentShift.StrengthIncreaseExponentShift = -1;
            yield return new TestCaseData(invalidStrengthIncreaseExponentShift, "Sterkte toename exp (m)"
            ).SetName(string.Format(testNameFormat, typeName));

            SoilLayerBase invalidPopShift = soilLayer();
            invalidPopShift.PopDistributionType = validDistributionType;
            invalidPopShift.PopShift = -1;
            yield return new TestCaseData(invalidPopShift, "POP"
            ).SetName(string.Format(testNameFormat, typeName));
        }

        #endregion
    }
}