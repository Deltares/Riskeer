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
using System.Linq;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Piping.KernelWrapper.SubCalculator;
using Ringtoets.Piping.KernelWrapper.TestUtil.SubCalculator;
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.KernelWrapper.Test
{
    [TestFixture]
    public class PipingCalculatorTest
    {
        [Test]
        public void Constructor_WithoutInput_ArgumentNullException()
        {
            // Call
            TestDelegate call = () => new PipingCalculator(null, null);

            // Assert
            const string expectedMessage = "PipingCalculatorInput required for creating a PipingCalculator.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(call, expectedMessage);
        }

        [Test]
        public void Constructor_FactoryNull_ArgumentNullException()
        {
            // Call
            var input = new PipingCalculatorInput(CreateSimpleConstructionProperties());
            TestDelegate call = () => new PipingCalculator(input, null);

            // Assert
            const string expectedMessage = "IPipingSubCalculatorFactory required for creating a PipingCalculator.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(call, expectedMessage);
        }

        [Test]
        public void Calculate_CompleteValidInput_ReturnsResultWithNoNaN()
        {
            // Setup
            var input = new PipingCalculatorInput(CreateSimpleConstructionProperties());
            var testPipingSubCalculatorFactory = new TestPipingSubCalculatorFactory();

            // Call
            PipingCalculatorResult actual = new PipingCalculator(input, testPipingSubCalculatorFactory).Calculate();

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsFalse(double.IsNaN(actual.UpliftEffectiveStress));
            Assert.IsFalse(double.IsNaN(actual.UpliftZValue));
            Assert.IsFalse(double.IsNaN(actual.UpliftFactorOfSafety));
            Assert.IsFalse(double.IsNaN(actual.HeaveZValue));
            Assert.IsFalse(double.IsNaN(actual.HeaveFactorOfSafety));
            Assert.IsFalse(double.IsNaN(actual.SellmeijerZValue));
            Assert.IsFalse(double.IsNaN(actual.SellmeijerFactorOfSafety));

            Assert.IsTrue(testPipingSubCalculatorFactory.LastCreatedHeaveCalculator.Calculated);
            Assert.IsTrue(testPipingSubCalculatorFactory.LastCreatedSellmeijerCalculator.Calculated);
            Assert.IsTrue(testPipingSubCalculatorFactory.LastCreatedUpliftCalculator.Calculated);
            Assert.IsTrue(testPipingSubCalculatorFactory.LastCreatedPipingProfilePropertyCalculator.Calculated);
            Assert.IsFalse(testPipingSubCalculatorFactory.LastCreatedPiezometricHeadAtExitCalculator.Calculated);
            Assert.IsFalse(testPipingSubCalculatorFactory.LastCreatedEffectiveThicknessCalculator.Calculated);
        }

        [Test]
        public void Calculate_CompleteValidInput_BottomLevelAquitardLayerAboveExitPointZUsedFromCalculator()
        {
            // Setup
            var input = new PipingCalculatorInput(CreateSimpleConstructionProperties());
            var testPipingSubCalculatorFactory = new TestPipingSubCalculatorFactory();
            double bottomAquitardLayerAboveExitPointZ = new Random(21).NextDouble(0, 10);
            testPipingSubCalculatorFactory.LastCreatedPipingProfilePropertyCalculator.BottomAquitardLayerAboveExitPointZ = bottomAquitardLayerAboveExitPointZ;

            // Call
            new PipingCalculator(input, testPipingSubCalculatorFactory).Calculate();

            // Assert
            Assert.AreEqual(bottomAquitardLayerAboveExitPointZ, testPipingSubCalculatorFactory.LastCreatedSellmeijerCalculator.BottomLevelAquitardAboveExitPointZ);
        }

        [Test]
        public void Validate_CompleteValidInput_ReturnsNoValidationMessages()
        {
            // Setup
            var input = new PipingCalculatorInput(CreateSimpleConstructionProperties());
            var calculation = new PipingCalculator(input, PipingSubCalculatorFactory.Instance);

            // Call
            List<string> validationMessages = calculation.Validate();

            // Assert
            Assert.AreEqual(0, validationMessages.Count);
        }

        [Test]
        public void Validate_CompleteValidInput_CalculatorsValidated()
        {
            // Setup
            var input = new PipingCalculatorInput(CreateSimpleConstructionProperties());
            var testPipingSubCalculatorFactory = new TestPipingSubCalculatorFactory();
            var calculation = new PipingCalculator(input, testPipingSubCalculatorFactory);

            // Call
            calculation.Validate();

            // Assert
            Assert.IsTrue(testPipingSubCalculatorFactory.LastCreatedHeaveCalculator.Validated);
            Assert.IsTrue(testPipingSubCalculatorFactory.LastCreatedSellmeijerCalculator.Validated);
            Assert.IsTrue(testPipingSubCalculatorFactory.LastCreatedUpliftCalculator.Validated);
            Assert.IsTrue(testPipingSubCalculatorFactory.LastCreatedPipingProfilePropertyCalculator.Validated);
        }

        [Test]
        [TestCase(0)]
        [TestCase(-1e-6)]
        [TestCase(-100)]
        public void Validate_ZeroOrNegativeSeepageLength_ValidationMessageForPipingLength(double seepageLength)
        {
            // Setup
            PipingCalculatorInput.ConstructionProperties properties = CreateSimpleConstructionProperties();
            properties.SeepageLength = seepageLength;

            var input = new PipingCalculatorInput(properties);

            var calculation = new PipingCalculator(input, PipingSubCalculatorFactory.Instance);

            // Call
            List<string> validationMessages = calculation.Validate();

            // Assert
            Assert.AreEqual(1, validationMessages.Count);
            Assert.AreEqual("Kwelweglengte heeft ongeldige waarde (0 of negatief).", validationMessages[0]);
        }

        [Test]
        [TestCase(0)]
        [TestCase(-1e-6)]
        [TestCase(-100)]
        public void Validate_ZeroOrNegativeAquiferThickness_ValidationMessageForDAquifer(double aquiferThickness)
        {
            // Setup
            PipingCalculatorInput.ConstructionProperties properties = CreateSimpleConstructionProperties();
            properties.ThicknessAquiferLayer = aquiferThickness;

            var input = new PipingCalculatorInput(properties);

            var calculation = new PipingCalculator(input, PipingSubCalculatorFactory.Instance);

            // Call
            List<string> validationMessages = calculation.Validate();

            // Assert
            Assert.AreEqual(1, validationMessages.Count);
            Assert.AreEqual("Parameter 'DAquifer' (dikte watervoerend pakket) heeft ongeldige waarde (0 of negatief).", validationMessages[0]);
        }

        [Test]
        [TestCase(-1e-6)]
        [TestCase(-100)]
        public void Validate_NegativeBeddingAngle_ValidationMessageForBeddingAngle(double beddingAngle)
        {
            // Setup
            PipingCalculatorInput.ConstructionProperties properties = CreateSimpleConstructionProperties();
            properties.BeddingAngle = beddingAngle;

            var input = new PipingCalculatorInput(properties);

            var calculation = new PipingCalculator(input, PipingSubCalculatorFactory.Instance);

            // Call
            List<string> validationMessages = calculation.Validate();

            // Assert
            Assert.AreEqual(1, validationMessages.Count);
            Assert.AreEqual("Rolweerstandshoek heeft een ongeldige waarde (0 of negatief).", validationMessages[0]);
        }

        [Test]
        public void Validate_DampingFactorExitZero_TwoValidationMessageForRExit()
        {
            // Setup
            PipingCalculatorInput.ConstructionProperties properties = CreateSimpleConstructionProperties();
            properties.AssessmentLevel = (RoundedDouble) 0.1;
            properties.DampingFactorExit = 0;

            var input = new PipingCalculatorInput(properties);

            var calculation = new PipingCalculator(input, PipingSubCalculatorFactory.Instance);

            // Call
            List<string> validationMessages = calculation.Validate();

            // Assert
            Assert.AreEqual(1, validationMessages.Count);
            Assert.AreEqual("Parameter 'RExit' (Dempingsfactor bij uittredepunt) mag niet nul zijn.", validationMessages[0]);
        }

        [Test]
        public void Validate_ThicknessAquiferLayerZero_ValidationMessageForDAquifer()
        {
            // Setup
            PipingCalculatorInput.ConstructionProperties properties = CreateSimpleConstructionProperties();
            properties.ThicknessAquiferLayer = 0;

            var input = new PipingCalculatorInput(properties);

            var calculation = new PipingCalculator(input, PipingSubCalculatorFactory.Instance);

            // Call
            List<string> validationMessages = calculation.Validate();

            // Assert
            Assert.AreEqual(1, validationMessages.Count);
            Assert.AreEqual("Parameter 'DAquifer' (dikte watervoerend pakket) heeft ongeldige waarde (0 of negatief).", validationMessages[0]);
        }

        [Test]
        public void Validate_VolumetricWeightWaterZero_ValidationMessageForVolumetricWeightWater()
        {
            // Setup
            PipingCalculatorInput.ConstructionProperties properties = CreateSimpleConstructionProperties();
            properties.WaterVolumetricWeight = 0;

            var input = new PipingCalculatorInput(properties);

            var calculation = new PipingCalculator(input, PipingSubCalculatorFactory.Instance);

            // Call
            List<string> validationMessages = calculation.Validate();

            // Assert
            Assert.AreEqual(1, validationMessages.Count);
            Assert.AreEqual("Volumiek gewicht water heeft ongeldige waarde (mag niet nul zijn).", validationMessages[0]);
        }

        [Test]
        [TestCase(2, 1, 2, 0.5, TestName = "AssessmntLvlPhreaticLevelExitEqualsSellmeijerReductionFactorTimesThicknessCoverageLayer_ValidateMsgHRiverHExitRcDTotal(2,1,2,0.5)")]
        [TestCase(2, 1.5, 0.5, 1, TestName = "AssessmntLvlPhreaticLevelExitEqualsSellmeijerReductionFactorTimesThicknessCoverageLayer_ValidateMsgHRiverHExitRcDTotal(2,1.5,0.5,1)")]
        [TestCase(8, 4, 2, 2, TestName = "AssessmntLvlPhreaticLevelExitEqualsSellmeijerReductionFactorTimesThicknessCoverageLayer_ValidateMsgHRiverHExitRcDTotal(8,4,2,2)")]
        public void Validate_DifferenceAssessmentLevelAndPhreaticLevelExitEqualToSellmeijerReductionFactorTimesThicknessCoverageLayer_ValidationMessageForHRiverHExitRcDTotal(
            double assessmentLevel, double phreaticLevelExit, double sellmeijerReductionFactor, double thicknessCoverageLayer)
        {
            // Setup
            PipingCalculatorInput.ConstructionProperties properties = CreateSimpleConstructionProperties();
            properties.AssessmentLevel = (RoundedDouble) assessmentLevel;
            properties.PhreaticLevelExit = phreaticLevelExit;
            properties.SellmeijerReductionFactor = sellmeijerReductionFactor;
            properties.ThicknessCoverageLayer = thicknessCoverageLayer;

            var input = new PipingCalculatorInput(properties);

            var calculation = new PipingCalculator(input, PipingSubCalculatorFactory.Instance);

            // Call
            List<string> validationMessages = calculation.Validate();

            // Assert
            Assert.AreEqual(1, validationMessages.Count);
            Assert.AreEqual("De term HRiver - HExit - (Rc*DTotal) mag niet nul zijn.", validationMessages[0]);
        }

        [Test]
        public void Validate_NoSurfaceLineSet_ValidationMessageForHavingNoSurfaceLineSelected()
        {
            // Setup
            PipingCalculatorInput.ConstructionProperties properties = CreateSimpleConstructionProperties();
            properties.SurfaceLine = null;

            var input = new PipingCalculatorInput(properties);

            var calculation = new PipingCalculator(input, PipingSubCalculatorFactory.Instance);

            // Call
            List<string> validationMessages = calculation.Validate();

            // Assert
            Assert.AreEqual(1, validationMessages.Count);
            Assert.AreEqual("De hoogtegeometrie is niet gedefinieerd.", validationMessages[0]);
        }

        [Test]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        public void Validate_SurfaceLineMissingDitchPoint_ValidationMessageForIncompleteDitch(int missingType)
        {
            // Setup
            PipingCalculatorInput.ConstructionProperties properties = CreateSimpleConstructionProperties();
            properties.SurfaceLine = new PipingSurfaceLine(string.Empty);

            var input = new PipingCalculatorInput(properties);
            input.SurfaceLine.SetGeometry(new[]
            {
                new Point3D(0, 0, 2),
                new Point3D(1, 0, -3),
                new Point3D(2, 0, -4),
                new Point3D(3, 0, 3)
            });
            if (missingType != 0)
            {
                input.SurfaceLine.SetDitchDikeSideAt(input.SurfaceLine.Points.ElementAt(0));
            }

            if (missingType != 1)
            {
                input.SurfaceLine.SetBottomDitchDikeSideAt(input.SurfaceLine.Points.ElementAt(1));
            }

            if (missingType != 2)
            {
                input.SurfaceLine.SetBottomDitchPolderSideAt(input.SurfaceLine.Points.ElementAt(2));
            }

            if (missingType != 3)
            {
                input.SurfaceLine.SetDitchPolderSideAt(input.SurfaceLine.Points.ElementAt(3));
            }

            var calculation = new PipingCalculator(input, PipingSubCalculatorFactory.Instance);

            // Call
            List<string> validationMessages = calculation.Validate();

            // Assert
            Assert.AreEqual(1, validationMessages.Count);
            Assert.AreEqual("De sloot in de hoogtegeometrie  is niet correct. Niet alle 4 punten zijn gedefinieerd of de volgorde is incorrect.", validationMessages.First());
        }

        [Test]
        [TestCase(0, 2, 1, 3)]
        [TestCase(0, 3, 2, 1)]
        [TestCase(3, 2, 1, 0)]
        public void Validate_SurfaceLineInvalidDitchPointsOrder_ValidationMessageForInvalidDitchPointsOrder(
            int ditchDikeSidePosition,
            int bottomDitchDikeSidePosition,
            int bottomDitchPolderSidePosition,
            int ditchPolderSidePosition)
        {
            // Setup
            PipingCalculatorInput.ConstructionProperties properties = CreateSimpleConstructionProperties();
            properties.SurfaceLine = new PipingSurfaceLine(string.Empty);

            var input = new PipingCalculatorInput(properties);
            input.SurfaceLine.SetGeometry(new[]
            {
                new Point3D(0, 0, 2),
                new Point3D(1, 0, -3),
                new Point3D(2, 0, -4),
                new Point3D(3, 0, 3)
            });
            input.SurfaceLine.SetDitchDikeSideAt(input.SurfaceLine.Points.ElementAt(ditchDikeSidePosition));
            input.SurfaceLine.SetBottomDitchDikeSideAt(input.SurfaceLine.Points.ElementAt(bottomDitchDikeSidePosition));
            input.SurfaceLine.SetBottomDitchPolderSideAt(input.SurfaceLine.Points.ElementAt(bottomDitchPolderSidePosition));
            input.SurfaceLine.SetDitchPolderSideAt(input.SurfaceLine.Points.ElementAt(ditchPolderSidePosition));

            var calculation = new PipingCalculator(input, PipingSubCalculatorFactory.Instance);

            // Call
            List<string> validationMessages = calculation.Validate();

            // Assert
            Assert.AreEqual(1, validationMessages.Count);
            Assert.AreEqual("De sloot in de hoogtegeometrie  is niet correct. Niet alle 4 punten zijn gedefinieerd of de volgorde is incorrect.", validationMessages.First());
        }

        [Test]
        public void Validate_NoSoilProfileSet_ValidationMessageForHavingNoSoilProfileSelected()
        {
            // Setup
            PipingCalculatorInput.ConstructionProperties properties = CreateSimpleConstructionProperties();
            properties.SoilProfile = null;

            var input = new PipingCalculatorInput(properties);

            var calculation = new PipingCalculator(input, PipingSubCalculatorFactory.Instance);

            // Call
            List<string> validationMessages = calculation.Validate();

            // Assert
            Assert.AreEqual(1, validationMessages.Count);
            Assert.AreEqual("Het ondergrondprofiel is niet gedefinieerd.", validationMessages[0]);
        }

        [Test]
        [TestCase(-1e-6)]
        [TestCase(0)]
        public void Validate_SoilProfileBottomAtTopLevel_ValidationMessageForHavingTooHighBottom(double bottom)
        {
            // Setup
            const int top = 0;
            PipingCalculatorInput.ConstructionProperties properties = CreateSimpleConstructionProperties();
            properties.SoilProfile = new PipingSoilProfile(string.Empty, bottom, new[]
            {
                new PipingSoilLayer(top)
                {
                    IsAquifer = true
                }
            }, SoilProfileType.SoilProfile1D);

            var input = new PipingCalculatorInput(properties);

            var calculation = new PipingCalculator(input, PipingSubCalculatorFactory.Instance);

            // Call
            List<string> validationMessages = calculation.Validate();

            // Assert
            string message = $"De onderkant({bottom}) van het ondergrondprofiel is niet laag genoeg. Het moet tenminste {0.001} m onder de bovenkant van de diepste laag ({top}) liggen.";
            Assert.AreEqual(1, validationMessages.Count);
            Assert.AreEqual(message, validationMessages[0]);
        }

        [Test]
        public void CalculateThicknessCoverageLayer_SoilProfileWithoutAquiferSet_ThrowsPipingCalculatorException()
        {
            // Setup
            PipingCalculatorInput.ConstructionProperties properties = CreateSimpleConstructionProperties();
            properties.SoilProfile = new PipingSoilProfile(string.Empty, -1.0, new[]
            {
                new PipingSoilLayer(0)
            }, SoilProfileType.SoilProfile1D);

            var input = new PipingCalculatorInput(properties);

            var calculation = new PipingCalculator(input, PipingSubCalculatorFactory.Instance);

            // Call
            TestDelegate call = () => calculation.CalculateEffectiveThicknessCoverageLayer();

            // Assert
            var exception = Assert.Throws<PipingCalculatorException>(call);
            Assert.IsInstanceOf<NullReferenceException>(exception.InnerException);
        }

        [Test]
        public void CalculateThicknessCoverageLayer_WithValidInput_ReturnsSomeThickness()
        {
            // Setup
            var input = new PipingCalculatorInput(CreateSimpleConstructionProperties());

            var calculation = new PipingCalculator(input, PipingSubCalculatorFactory.Instance);

            // Call
            double result = calculation.CalculateEffectiveThicknessCoverageLayer();

            // Assert
            Assert.AreEqual(1.0, result);
        }

        [Test]
        public void CalculateThicknessCoverageLayer_WithValidInput_UsedEffectiveThicknessCalculator()
        {
            // Setup
            var input = new PipingCalculatorInput(CreateSimpleConstructionProperties());

            var testPipingSubCalculatorFactory = new TestPipingSubCalculatorFactory();
            var calculation = new PipingCalculator(input, testPipingSubCalculatorFactory);

            // Call
            calculation.CalculateEffectiveThicknessCoverageLayer();

            // Assert
            Assert.IsFalse(testPipingSubCalculatorFactory.LastCreatedHeaveCalculator.Calculated);
            Assert.IsFalse(testPipingSubCalculatorFactory.LastCreatedSellmeijerCalculator.Calculated);
            Assert.IsFalse(testPipingSubCalculatorFactory.LastCreatedUpliftCalculator.Calculated);
            Assert.IsFalse(testPipingSubCalculatorFactory.LastCreatedPipingProfilePropertyCalculator.Calculated);
            Assert.IsFalse(testPipingSubCalculatorFactory.LastCreatedPiezometricHeadAtExitCalculator.Calculated);
            Assert.IsTrue(testPipingSubCalculatorFactory.LastCreatedEffectiveThicknessCalculator.Calculated);
        }

        [Test]
        public void CalculateThicknessCoverageLayer_WithValidInputWithAquiferAboveSurfaceLine_ReturnsNegativeThickness()
        {
            // Setup
            var input = new PipingCalculatorInput(CreateSimpleConstructionProperties());
            input.SurfaceLine.SetGeometry(new[]
            {
                new Point3D(0, 0, 0.5),
                new Point3D(1, 0, 1.5),
                new Point3D(2, 0, -1)
            });

            var calculation = new PipingCalculator(input, PipingSubCalculatorFactory.Instance);

            // Call
            double result = calculation.CalculateEffectiveThicknessCoverageLayer();

            // Assert
            Assert.AreEqual(result, -3.0);
        }

        [Test]
        public void CalculateThicknessCoverageLayer_WithExitPointLBeyondSurfaceLineInput_ReturnsNaN()
        {
            // Setup
            PipingCalculatorInput.ConstructionProperties properties = CreateSimpleConstructionProperties();
            properties.ExitPointXCoordinate = (RoundedDouble) 2.1;

            var input = new PipingCalculatorInput(properties);

            var calculation = new PipingCalculator(input, PipingSubCalculatorFactory.Instance);

            // Call
            double result = calculation.CalculateEffectiveThicknessCoverageLayer();

            // Assert
            Assert.IsNaN(result);
        }

        [Test]
        public void CalculatePiezometricHeadAtExit_WithValidInput_ReturnsSomeValue()
        {
            // Setup
            var input = new PipingCalculatorInput(CreateSimpleConstructionProperties());

            var calculation = new PipingCalculator(input, PipingSubCalculatorFactory.Instance);

            // Call
            double result = calculation.CalculatePiezometricHeadAtExit();

            // Assert
            Assert.IsFalse(double.IsNaN(result));
        }

        [Test]
        public void CalculateThicknessCoverageLayer_WithValidInput_UsedPiezometricHeadAtExitCalculator()
        {
            // Setup
            var input = new PipingCalculatorInput(CreateSimpleConstructionProperties());

            var testPipingSubCalculatorFactory = new TestPipingSubCalculatorFactory();
            var calculation = new PipingCalculator(input, testPipingSubCalculatorFactory);

            // Call
            calculation.CalculatePiezometricHeadAtExit();

            // Assert
            Assert.IsFalse(testPipingSubCalculatorFactory.LastCreatedHeaveCalculator.Calculated);
            Assert.IsFalse(testPipingSubCalculatorFactory.LastCreatedSellmeijerCalculator.Calculated);
            Assert.IsFalse(testPipingSubCalculatorFactory.LastCreatedUpliftCalculator.Calculated);
            Assert.IsFalse(testPipingSubCalculatorFactory.LastCreatedPipingProfilePropertyCalculator.Calculated);
            Assert.IsTrue(testPipingSubCalculatorFactory.LastCreatedPiezometricHeadAtExitCalculator.Calculated);
            Assert.IsFalse(testPipingSubCalculatorFactory.LastCreatedEffectiveThicknessCalculator.Calculated);
        }

        private static PipingCalculatorInput.ConstructionProperties CreateSimpleConstructionProperties()
        {
            var random = new Random(21);

            return new PipingCalculatorInput.ConstructionProperties
            {
                WaterVolumetricWeight = random.NextDouble(),
                SaturatedVolumicWeightOfCoverageLayer = random.NextDouble(),
                UpliftModelFactor = random.NextDouble(),
                AssessmentLevel = random.NextDouble(),
                PiezometricHeadExit = random.NextDouble(),
                PhreaticLevelExit = random.NextDouble(),
                DampingFactorExit = random.NextDouble(),
                CriticalHeaveGradient = random.NextDouble(),
                ThicknessCoverageLayer = random.NextDouble(),
                EffectiveThicknessCoverageLayer = random.NextDouble(),
                SellmeijerModelFactor = random.NextDouble(),
                SellmeijerReductionFactor = random.NextDouble(),
                SeepageLength = random.NextDouble(),
                SandParticlesVolumicWeight = random.NextDouble(),
                WhitesDragCoefficient = random.NextDouble(),
                Diameter70 = random.NextDouble(),
                DarcyPermeability = random.NextDouble(),
                WaterKinematicViscosity = random.NextDouble(),
                Gravity = random.NextDouble(),
                ExitPointXCoordinate = 0.5,
                BeddingAngle = random.NextDouble(),
                MeanDiameter70 = random.NextDouble(),
                ThicknessAquiferLayer = random.NextDouble(),
                SurfaceLine = CreateValidSurfaceLine(),
                SoilProfile = CreateValidSoilProfile()
            };
        }

        private static PipingSoilProfile CreateValidSoilProfile()
        {
            return new PipingSoilProfile(string.Empty, -2, new[]
            {
                new PipingSoilLayer(9),
                new PipingSoilLayer(4)
                {
                    IsAquifer = true
                },
                new PipingSoilLayer(2),
                new PipingSoilLayer(-1)
            }, SoilProfileType.SoilProfile1D);
        }

        private static PipingSurfaceLine CreateValidSurfaceLine()
        {
            var pipingSurfaceLine = new PipingSurfaceLine(string.Empty);
            pipingSurfaceLine.SetGeometry(new[]
            {
                new Point3D(0, 0, 2),
                new Point3D(1, 0, 8),
                new Point3D(2, 0, -1)
            });
            return pipingSurfaceLine;
        }
    }
}