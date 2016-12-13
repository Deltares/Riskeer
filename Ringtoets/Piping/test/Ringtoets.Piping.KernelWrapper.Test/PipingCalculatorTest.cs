// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Ringtoets.Piping.KernelWrapper.TestUtil;
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
            TestDelegate call = () => new PipingCalculator(new TestPipingInput().AsRealInput(), null);

            // Assert
            const string expectedMessage = "IPipingSubCalculatorFactory required for creating a PipingCalculator.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(call, expectedMessage);
        }

        [Test]
        public void Calculate_CompleteValidInput_ReturnsResultWithNoNaN()
        {
            // Setup
            PipingCalculatorInput input = new TestPipingInput().AsRealInput();
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
            PipingCalculatorInput input = new TestPipingInput().AsRealInput();
            var testPipingSubCalculatorFactory = new TestPipingSubCalculatorFactory();
            var bottomAquitardLayerAboveExitPointZ = new Random(21).NextDouble() * 10;
            testPipingSubCalculatorFactory.LastCreatedPipingProfilePropertyCalculator.BottomAquitardLayerAboveExitPointZ = bottomAquitardLayerAboveExitPointZ;

            // Call
            new PipingCalculator(input, testPipingSubCalculatorFactory).Calculate();

            // Assert
            Assert.AreEqual(bottomAquitardLayerAboveExitPointZ, testPipingSubCalculatorFactory.LastCreatedHeaveCalculator.BottomLevelAquitardAboveExitPointZ);
            Assert.AreEqual(bottomAquitardLayerAboveExitPointZ, testPipingSubCalculatorFactory.LastCreatedSellmeijerCalculator.BottomLevelAquitardAboveExitPointZ);
        }

        [Test]
        public void Validate_CompleteValidInput_ReturnsNoValidationMessages()
        {
            // Setup
            PipingCalculatorInput input = new TestPipingInput().AsRealInput();
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
            PipingCalculatorInput input = new TestPipingInput().AsRealInput();
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
            PipingCalculatorInput input = new TestPipingInput
            {
                SeepageLength = seepageLength
            }.AsRealInput();

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
            PipingCalculatorInput input = new TestPipingInput
            {
                ThicknessAquiferLayer = aquiferThickness
            }.AsRealInput();

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
            PipingCalculatorInput input = new TestPipingInput
            {
                BeddingAngle = beddingAngle
            }.AsRealInput();

            var calculation = new PipingCalculator(input, PipingSubCalculatorFactory.Instance);

            // Call
            List<string> validationMessages = calculation.Validate();

            // Assert
            Assert.AreEqual(1, validationMessages.Count);
            Assert.AreEqual("Rolweerstandshoek heeft een ongeldige waarde (0 of negatief).", validationMessages[0]);
        }

        [Test]
        [TestCase(-1e-6)]
        [TestCase(-100)]
        public void Validate_PiezometricHeadExitSameAsPhreaticLevelExit_ValidationMessageForPhiExitAndHExit(double level)
        {
            // Setup
            PipingCalculatorInput input = new TestPipingInput
            {
                PhreaticLevelExit = level,
                PiezometricHeadExit = (RoundedDouble) level
            }.AsRealInput();

            var calculation = new PipingCalculator(input, PipingSubCalculatorFactory.Instance);

            // Call
            List<string> validationMessages = calculation.Validate();

            // Assert
            Assert.AreEqual(1, validationMessages.Count);
            Assert.AreEqual("Het verschil tussen de parameters 'PhiExit' (Stijghoogte bij uittredepunt) en 'HExit' (Freatische waterstand bij uittredepunt) mag niet nul zijn.", validationMessages[0]);
        }

        [Test]
        public void Validate_DampingFactorExitZero_TwoValidationMessageForRExit()
        {
            // Setup
            PipingCalculatorInput input = new TestPipingInput
            {
                AssessmentLevel = (RoundedDouble) 0.1,
                DampingFactorExit = 0
            }.AsRealInput();

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
            PipingCalculatorInput input = new TestPipingInput
            {
                ThicknessAquiferLayer = 0
            }.AsRealInput();

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
            PipingCalculatorInput input = new TestPipingInput
            {
                WaterVolumetricWeight = 0
            }.AsRealInput();

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
            PipingCalculatorInput input = new TestPipingInput
            {
                AssessmentLevel = (RoundedDouble) assessmentLevel,
                PhreaticLevelExit = phreaticLevelExit,
                SellmeijerReductionFactor = sellmeijerReductionFactor,
                ThicknessCoverageLayer = thicknessCoverageLayer
            }.AsRealInput();

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
            PipingCalculatorInput input = new TestPipingInput
            {
                SurfaceLine = null,
            }.AsRealInput();

            var calculation = new PipingCalculator(input, PipingSubCalculatorFactory.Instance);

            // Call
            List<string> validationMessages = calculation.Validate();

            // Assert
            Assert.AreEqual(1, validationMessages.Count);
            Assert.AreEqual("Een profielschematisatie moet geselecteerd zijn om een Uplift berekening uit te kunnen voeren.", validationMessages[0]);
        }

        [Test]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        public void Validate_SurfaceLineMissingDitchPoint_ValidationMessageForIncompleteDitch(int missingType)
        {
            // Setup
            TestPipingInput tempInput = new TestPipingInput
            {
                SurfaceLine = new RingtoetsPipingSurfaceLine()
            };
            tempInput.SurfaceLine.SetGeometry(new[]
            {
                new Point3D(0, 0, 2),
                new Point3D(1, 0, -3),
                new Point3D(2, 0, -4),
                new Point3D(3, 0, 3)
            });
            if (missingType != 0)
            {
                tempInput.SurfaceLine.SetDitchDikeSideAt(tempInput.SurfaceLine.Points[0]);
            }
            if (missingType != 1)
            {
                tempInput.SurfaceLine.SetBottomDitchDikeSideAt(tempInput.SurfaceLine.Points[1]);
            }
            if (missingType != 2)
            {
                tempInput.SurfaceLine.SetBottomDitchPolderSideAt(tempInput.SurfaceLine.Points[2]);
            }
            if (missingType != 3)
            {
                tempInput.SurfaceLine.SetDitchPolderSideAt(tempInput.SurfaceLine.Points[3]);
            }
            PipingCalculatorInput input = tempInput.AsRealInput();

            var calculation = new PipingCalculator(input, PipingSubCalculatorFactory.Instance);

            // Call
            List<string> validationMessages = calculation.Validate();

            // Assert
            Assert.AreEqual(1, validationMessages.Count);
            Assert.AreEqual("The ditch in surface line  is incorrect. Not all 4 points are defined or the order is incorrect.", validationMessages.First());
        }

        [Test]
        [TestCase(0,2,1,3)]
        [TestCase(0,3,2,1)]
        [TestCase(3,2,1,0)]
        public void Validate_SurfaceLineInvalidDitchPointsOrder_ValidationMessageForInvalidDitchPointsOrder(
            int ditchDikeSidePosition,
            int bottomDitchDikeSidePosition,
            int bottomDitchPolderSidePosition,
            int ditchPolderSidePosition)
        {
            // Setup
            TestPipingInput tempInput = new TestPipingInput
            {
                SurfaceLine = new RingtoetsPipingSurfaceLine()
            };
            tempInput.SurfaceLine.SetGeometry(new[]
            {
                new Point3D(0, 0, 2),
                new Point3D(1, 0, -3),
                new Point3D(2, 0, -4),
                new Point3D(3, 0, 3)
            });
            tempInput.SurfaceLine.SetDitchDikeSideAt(tempInput.SurfaceLine.Points[ditchDikeSidePosition]);
            tempInput.SurfaceLine.SetBottomDitchDikeSideAt(tempInput.SurfaceLine.Points[bottomDitchDikeSidePosition]);
            tempInput.SurfaceLine.SetBottomDitchPolderSideAt(tempInput.SurfaceLine.Points[bottomDitchPolderSidePosition]);
            tempInput.SurfaceLine.SetDitchPolderSideAt(tempInput.SurfaceLine.Points[ditchPolderSidePosition]);

            PipingCalculatorInput input = tempInput.AsRealInput();

            var calculation = new PipingCalculator(input, PipingSubCalculatorFactory.Instance);

            // Call
            List<string> validationMessages = calculation.Validate();

            // Assert
            Assert.AreEqual(1, validationMessages.Count);
            Assert.AreEqual("The ditch in surface line  is incorrect. Not all 4 points are defined or the order is incorrect.", validationMessages.First());
        }

        [Test]
        public void Validate_NoSoilProfileSet_ValidationMessageForHavingNoSoilProfileSelected()
        {
            // Setup
            PipingCalculatorInput input = new TestPipingInput
            {
                SoilProfile = null,
            }.AsRealInput();

            var calculation = new PipingCalculator(input, PipingSubCalculatorFactory.Instance);

            // Call
            List<string> validationMessages = calculation.Validate();

            // Assert
            Assert.AreEqual(1, validationMessages.Count);
            Assert.AreEqual("Een ondergrondschematisatie moet geselecteerd zijn om een Uplift berekening uit te kunnen voeren.", validationMessages[0]);
        }

        [Test]
        [TestCase(-1e-6)]
        [TestCase(0)]
        public void Validate_SoilProfileBottomAtTopLevel_ValidationMessageForHavingTooHighBottom(double bottom)
        {
            // Setup
            var top = 0;
            PipingCalculatorInput input = new TestPipingInput
            {
                SoilProfile = new PipingSoilProfile(String.Empty, bottom, new[]
                {
                    new PipingSoilLayer(top)
                }, SoilProfileType.SoilProfile1D, 0)
            }.AsRealInput();

            var calculation = new PipingCalculator(input, PipingSubCalculatorFactory.Instance);

            // Call
            List<string> validationMessages = calculation.Validate();

            // Assert
            var message = string.Format("The bottomlevel ({0}) of the profile is not deep enough. It must be below at least {1} m below the toplevel of the deepest layer ({2}).", bottom, 0.001, top);
            Assert.AreEqual(1, validationMessages.Count);
            Assert.AreEqual(message, validationMessages[0]);
        }

        [Test]
        public void CalculateThicknessCoverageLayer_SoilProfileWithoutAquiferSet_ThrowsPipingCalculatorException()
        {
            // Setup
            PipingCalculatorInput input = new TestPipingInput
            {
                SoilProfile = new PipingSoilProfile(String.Empty, -1.0, new[]
                {
                    new PipingSoilLayer(0)
                }, SoilProfileType.SoilProfile1D, 0)
            }.AsRealInput();

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
            PipingCalculatorInput input = new TestPipingInput().AsRealInput();

            var calculation = new PipingCalculator(input, PipingSubCalculatorFactory.Instance);

            // Call
            var result = calculation.CalculateEffectiveThicknessCoverageLayer();

            // Assert
            Assert.AreEqual(1.0, result);
        }

        [Test]
        public void CalculateThicknessCoverageLayer_WithValidInput_UsedEffectiveThicknessCalculator()
        {
            // Setup
            PipingCalculatorInput input = new TestPipingInput().AsRealInput();

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
            PipingCalculatorInput input = new TestPipingInput().AsRealInput();
            input.SurfaceLine.SetGeometry(new[]
            {
                new Point3D(0, 0, 0.5),
                new Point3D(1, 0, 1.5),
                new Point3D(2, 0, -1)
            });

            var calculation = new PipingCalculator(input, PipingSubCalculatorFactory.Instance);

            // Call
            var result = calculation.CalculateEffectiveThicknessCoverageLayer();

            // Assert
            Assert.AreEqual(result, -3.0);
        }

        [Test]
        public void CalculateThicknessCoverageLayer_WithExitPointLBeyondSurfaceLineInput_ReturnsNaN()
        {
            // Setup
            PipingCalculatorInput input = new TestPipingInput
            {
                ExitPointXCoordinate = (RoundedDouble) 2.1
            }.AsRealInput();

            var calculation = new PipingCalculator(input, PipingSubCalculatorFactory.Instance);

            // Call
            var result = calculation.CalculateEffectiveThicknessCoverageLayer();

            // Assert
            Assert.IsNaN(result);
        }

        [Test]
        public void CalculatePiezometricHeadAtExit_WithValidInput_ReturnsSomeValue()
        {
            // Setup
            PipingCalculatorInput input = new TestPipingInput().AsRealInput();

            var calculation = new PipingCalculator(input, PipingSubCalculatorFactory.Instance);

            // Call
            var result = calculation.CalculatePiezometricHeadAtExit();

            // Assert
            Assert.IsFalse(double.IsNaN(result));
        }

        [Test]
        public void CalculateThicknessCoverageLayer_WithValidInput_UsedPiezometricHeadAtExitCalculator()
        {
            // Setup
            PipingCalculatorInput input = new TestPipingInput().AsRealInput();

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
    }
}