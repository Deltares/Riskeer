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
using System.Linq;
using Core.Common.Base.Data;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Service.TestUtil;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.MacroStabilityInwards.Data.TestUtil;
using Ringtoets.MacroStabilityInwards.KernelWrapper;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Result;
using Ringtoets.MacroStabilityInwards.KernelWrapper.TestUtil;
using Ringtoets.MacroStabilityInwards.Primitives;

namespace Ringtoets.MacroStabilityInwards.Service.Test
{
    [TestFixture]
    public class MacroStabilityInwardsCalculationServiceTest
    {
        private MacroStabilityInwardsCalculationScenario testCalculation;

        [SetUp]
        public void Setup()
        {
            testCalculation = MacroStabilityInwardsCalculationScenarioFactory.CreateMacroStabilityInwardsCalculationScenarioWithValidInput();
        }

        [Test]
        public void Validate_CalculationNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => MacroStabilityInwardsCalculationService.Validate(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("calculation", exception.ParamName);
        }

        [Test]
        public void Validate_Always_LogStartAndEndOfValidatingInputs()
        {
            // Setup
            const string name = "<very nice name>";

            testCalculation.Name = name;

            // Call
            Action call = () => MacroStabilityInwardsCalculationService.Validate(testCalculation);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] msgs = messages.ToArray();
                CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                CalculationServiceTestHelper.AssertValidationEndMessage(msgs[1]);
            });
        }

        [Test]
        public void Validate_InvalidCalculationWithOutput_ReturnsFalseNoOutputChange()
        {
            // Setup
            var output = new TestMacroStabilityInwardsOutput();
            MacroStabilityInwardsCalculation invalidMacroStabilityInwardsCalculation = MacroStabilityInwardsCalculationScenarioFactory.CreateMacroStabilityInwardsCalculationScenarioWithInvalidInput();
            invalidMacroStabilityInwardsCalculation.Output = output;

            // Call
            bool isValid = MacroStabilityInwardsCalculationService.Validate(invalidMacroStabilityInwardsCalculation);

            // Assert
            Assert.IsFalse(isValid);
            Assert.AreSame(output, invalidMacroStabilityInwardsCalculation.Output);
        }

        [Test]
        public void Validate_InvalidCalculationInput_LogsErrorAndReturnsFalse()
        {
            // Setup
            const string name = "<very nice name>";

            var calculation = new MacroStabilityInwardsCalculation
            {
                Name = name
            };

            // Call
            var isValid = false;
            Action call = () => isValid = MacroStabilityInwardsCalculationService.Validate(calculation);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(5, msgs.Length);
                CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                Assert.AreEqual("Validatie mislukt: Er is geen hydraulische randvoorwaardenlocatie geselecteerd.", msgs[1]);
                Assert.AreEqual("Validatie mislukt: Er is geen profielschematisatie geselecteerd.", msgs[2]);
                Assert.AreEqual("Validatie mislukt: Er is geen ondergrondschematisatie geselecteerd.", msgs[3]);
                CalculationServiceTestHelper.AssertValidationEndMessage(msgs[4]);
            });
            Assert.IsFalse(isValid);
        }

        [Test]
        public void Validate_HydraulicBoundaryLocationNotCalculated_LogsErrorAndReturnsFalse()
        {
            // Setup
            const string name = "<very nice name>";

            testCalculation.Name = name;
            testCalculation.InputParameters.HydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();
            testCalculation.InputParameters.UseAssessmentLevelManualInput = false;

            // Call
            var isValid = false;
            Action call = () => isValid = MacroStabilityInwardsCalculationService.Validate(testCalculation);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                Assert.AreEqual("Validatie mislukt: Kan het toetspeil niet afleiden op basis van de invoer.", msgs[1]);
                CalculationServiceTestHelper.AssertValidationEndMessage(msgs[2]);
            });
            Assert.IsFalse(isValid);
        }

        [Test]
        [TestCase(double.NaN)]
        [TestCase(double.NegativeInfinity)]
        [TestCase(double.PositiveInfinity)]
        public void Validate_InvalidManualAssessmentLevel_LogsErrorAndReturnsFalse(double assessmentLevel)
        {
            // Setup
            const string name = "<very nice name>";

            testCalculation.InputParameters.UseAssessmentLevelManualInput = true;
            testCalculation.InputParameters.AssessmentLevel = (RoundedDouble) assessmentLevel;
            testCalculation.Name = name;

            // Call
            var isValid = false;
            Action call = () => isValid = MacroStabilityInwardsCalculationService.Validate(testCalculation);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                Assert.AreEqual("Validatie mislukt: De waarde voor 'toetspeil' moet een concreet getal zijn.", msgs[1]);
                CalculationServiceTestHelper.AssertValidationEndMessage(msgs[2]);
            });
            Assert.IsFalse(isValid);
        }

        [Test]
        public void Validate_WithoutSurfaceLine_LogsErrorAndReturnsFalse()
        {
            // Setup
            const string name = "<very nice name>";

            testCalculation.InputParameters.SurfaceLine = null;
            testCalculation.Name = name;

            // Call
            var isValid = false;
            Action call = () => isValid = MacroStabilityInwardsCalculationService.Validate(testCalculation);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                Assert.AreEqual("Validatie mislukt: Er is geen profielschematisatie geselecteerd.", msgs[1]);
                CalculationServiceTestHelper.AssertValidationEndMessage(msgs[2]);
            });
            Assert.IsFalse(isValid);
        }

        [Test]
        public void Validate_WithoutStochasticSoilProfile_LogsErrorAndReturnsFalse()
        {
            // Setup
            const string name = "<very nice name>";

            testCalculation.InputParameters.StochasticSoilProfile = null;
            testCalculation.Name = name;

            var isValid = false;

            // Call
            Action call = () => isValid = MacroStabilityInwardsCalculationService.Validate(testCalculation);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                Assert.AreEqual("Validatie mislukt: Er is geen ondergrondschematisatie geselecteerd.", msgs[1]);
                CalculationServiceTestHelper.AssertValidationEndMessage(msgs[2]);
            });
            Assert.IsFalse(isValid);
        }

        [Test]
        public void Calculate_CalculationNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => MacroStabilityInwardsCalculationService.Calculate(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("calculation", exception.ParamName);
        }

        [Test]
        public void Calculate_ValidCalculation_LogStartAndEndOfValidatingInputsAndCalculation()
        {
            // Setup
            const string name = "<very nice name>";

            testCalculation.Name = name;

            using (new MacroStabilityInwardsCalculatorFactoryConfig(new TestMacroStabilityInwardsCalculatorFactory()))
            {
                Action call = () =>
                {
                    // Precondition
                    Assert.IsTrue(MacroStabilityInwardsCalculationService.Validate(testCalculation));

                    // Call
                    MacroStabilityInwardsCalculationService.Calculate(testCalculation);
                };

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                    CalculationServiceTestHelper.AssertValidationEndMessage(msgs[1]);
                    CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[2]);
                    CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[3]);
                });
            }
        }

        [Test]
        public void Calculate_ValidCalculationNoOutput_ShouldSetOutput()
        {
            // Setup
            using (new MacroStabilityInwardsCalculatorFactoryConfig(new TestMacroStabilityInwardsCalculatorFactory()))
            {
                // Precondition
                Assert.IsNull(testCalculation.Output);
                Assert.IsTrue(MacroStabilityInwardsCalculationService.Validate(testCalculation));

                // Call
                MacroStabilityInwardsCalculationService.Calculate(testCalculation);

                // Assert
                Assert.IsNotNull(testCalculation.Output);
            }
        }

        [Test]
        public void Calculate_ValidCalculationWithOutput_ShouldChangeOutput()
        {
            // Setup
            var output = new TestMacroStabilityInwardsOutput();

            testCalculation.Output = output;

            using (new MacroStabilityInwardsCalculatorFactoryConfig(new TestMacroStabilityInwardsCalculatorFactory()))
            {
                // Precondition
                Assert.IsTrue(MacroStabilityInwardsCalculationService.Validate(testCalculation));

                // Call
                MacroStabilityInwardsCalculationService.Calculate(testCalculation);

                // Assert
                Assert.AreNotSame(output, testCalculation.Output);
            }
        }

        [Test]
        public void Calculate_CompleteInput_SetsInputOnCalculator()
        {
            // Setup
            using (new MacroStabilityInwardsCalculatorFactoryConfig(new TestMacroStabilityInwardsCalculatorFactory()))
            {
                // Call
                MacroStabilityInwardsCalculationService.Calculate(testCalculation);

                // Assert
                AssertInput(testCalculation.InputParameters, (TestMacroStabilityInwardsCalculatorFactory) MacroStabilityInwardsCalculatorFactory.Instance);
            }
        }

        [Test]
        public void Calculate_CalculationRan_SetOutput()
        {
            // Setup
            using (new MacroStabilityInwardsCalculatorFactoryConfig(new TestMacroStabilityInwardsCalculatorFactory()))
            {
                var calculatorFactory = (TestMacroStabilityInwardsCalculatorFactory) MacroStabilityInwardsCalculatorFactory.Instance;

                // Precondition
                Assert.IsTrue(MacroStabilityInwardsCalculationService.Validate(testCalculation));

                // Call
                MacroStabilityInwardsCalculationService.Calculate(testCalculation);

                // Assert
                AssertOutput(calculatorFactory.LastCreatedCalculator.Output, testCalculation.Output);
            }
        }

        private static void AssertInput(MacroStabilityInwardsInput originalInput, TestMacroStabilityInwardsCalculatorFactory factory)
        {
            MacroStabilityInwardsCalculatorInput actualInput = factory.LastCreatedCalculator.Input;
            Assert.AreSame(originalInput.SoilProfileUnderSurfaceLine, actualInput.SoilProfile);
            Assert.AreSame(originalInput.SurfaceLine, actualInput.SurfaceLine);
            Assert.AreEqual(originalInput.AssessmentLevel, actualInput.AssessmentLevel);
            Assert.AreEqual(originalInput.DikeSoilScenario, actualInput.DikeSoilScenario);
            Assert.AreEqual(originalInput.WaterLevelRiverAverage, actualInput.WaterLevelRiverAverage);
            Assert.AreEqual(originalInput.WaterLevelPolder, actualInput.WaterLevelPolder);
            Assert.AreEqual(originalInput.XCoordinateDrainageConstruction, actualInput.XCoordinateDrainageConstruction);
            Assert.AreEqual(originalInput.ZCoordinateDrainageConstruction, actualInput.ZCoordinateDrainageConstruction);
            Assert.AreEqual(originalInput.MinimumLevelPhreaticLineAtDikeTopRiver, actualInput.MinimumLevelPhreaticLineAtDikeTopRiver);
            Assert.AreEqual(originalInput.MinimumLevelPhreaticLineAtDikeTopPolder, actualInput.MinimumLevelPhreaticLineAtDikeTopPolder);
            Assert.AreEqual(originalInput.PhreaticLineOffsetBelowDikeTopAtRiver, actualInput.PhreaticLineOffsetBelowDikeTopAtRiver);
            Assert.AreEqual(originalInput.PhreaticLineOffsetBelowDikeTopAtPolder, actualInput.PhreaticLineOffsetBelowDikeTopAtPolder);
            Assert.AreEqual(originalInput.PhreaticLineOffsetBelowShoulderBaseInside, actualInput.PhreaticLineOffsetBelowShoulderBaseInside);
            Assert.AreEqual(originalInput.PhreaticLineOffsetBelowDikeToeAtPolder, actualInput.PhreaticLineOffsetBelowDikeToeAtPolder);
            Assert.AreEqual(originalInput.LeakageLengthOutwardsPhreaticLine3, actualInput.LeakageLengthOutwardsPhreaticLine3);
            Assert.AreEqual(originalInput.LeakageLengthInwardsPhreaticLine3, actualInput.LeakageLengthInwardsPhreaticLine3);
            Assert.AreEqual(originalInput.LeakageLengthOutwardsPhreaticLine4, actualInput.LeakageLengthOutwardsPhreaticLine4);
            Assert.AreEqual(originalInput.LeakageLengthInwardsPhreaticLine4, actualInput.LeakageLengthInwardsPhreaticLine4);
            Assert.AreEqual(originalInput.PiezometricHeadPhreaticLine2Outwards, actualInput.PiezometricHeadPhreaticLine2Outwards);
            Assert.AreEqual(originalInput.PiezometricHeadPhreaticLine2Inwards, actualInput.PiezometricHeadPhreaticLine2Inwards);
            Assert.AreEqual(originalInput.PenetrationLength, actualInput.PenetrationLength);
            Assert.AreEqual(originalInput.DrainageConstructionPresent, actualInput.DrainageConstructionPresent);
            Assert.AreEqual(originalInput.AdjustPhreaticLine3And4ForUplift, actualInput.AdjustPhreaticLine3And4ForUplift);
            Assert.AreEqual(originalInput.UseDefaultOffsets, actualInput.UseDefaultOffsets);
            Assert.AreEqual(originalInput.MoveGrid, actualInput.MoveGrid);
            Assert.AreEqual(originalInput.MaximumSliceWidth, actualInput.MaximumSliceWidth);
            Assert.AreEqual(originalInput.GridDeterminationType == MacroStabilityInwardsGridDeterminationType.Automatic, actualInput.GridAutomaticDetermined);
            Assert.AreEqual(originalInput.LeftGrid, actualInput.LeftGrid);
            Assert.AreEqual(originalInput.RightGrid, actualInput.RightGrid);
            Assert.AreEqual(originalInput.TangentLineDeterminationType == MacroStabilityInwardsTangentLineDeterminationType.LayerSeparated, actualInput.TangentLineAutomaticAtBoundaries);
            Assert.AreEqual(originalInput.TangentLineZTop, actualInput.TangentLineZTop);
            Assert.AreEqual(originalInput.TangentLineZBottom, actualInput.TangentLineZBottom);
            Assert.AreEqual(originalInput.CreateZones, actualInput.CreateZones);
            Assert.AreEqual(originalInput.ZoningBoundariesDeterminationType == MacroStabilityInwardsZoningBoundariesDeterminationType.Automatic, actualInput.AutomaticForbiddenZones);
            Assert.AreEqual(originalInput.SlipPlaneMinimumDepth, actualInput.SlipPlaneMinimumDepth);
            Assert.AreEqual(originalInput.SlipPlaneMinimumLength, actualInput.SlipPlaneMinimumLength);
        }

        private static void AssertOutput(UpliftVanCalculatorResult expectedOutput, MacroStabilityInwardsOutput actualOutput)
        {
            Assert.AreEqual(expectedOutput.FactorOfStability, actualOutput.FactorOfStability);
            Assert.AreEqual(expectedOutput.ZValue, actualOutput.ZValue);
            Assert.AreEqual(expectedOutput.ForbiddenZonesXEntryMin, actualOutput.ForbiddenZonesXEntryMin);
            Assert.AreEqual(expectedOutput.ForbiddenZonesXEntryMax, actualOutput.ForbiddenZonesXEntryMax);
            Assert.AreEqual(expectedOutput.GridAutomaticallyCalculated, actualOutput.GridAutomaticallyCalculated);
            Assert.AreEqual(expectedOutput.ForbiddenZonesAutomaticallyCalculated, actualOutput.ForbiddenZonesAutomaticallyCalculated);
            AssertSlidingCurve(expectedOutput.SlidingCurveResult, actualOutput.SlidingCurve);
            AssertSlipPlane(expectedOutput.CalculationGridResult, actualOutput.SlipPlane);
        }

        private static void AssertSlidingCurve(UpliftVanSlidingCurveResult expected, MacroStabilityInwardsSlidingCurve actual)
        {
            Assert.AreEqual(expected.IteratedHorizontalForce, actual.IteratedHorizontalForce);
            Assert.AreEqual(expected.NonIteratedHorizontalForce, actual.NonIteratedHorizontalForce);
            AssertCircle(expected.LeftCircle, actual.LeftCircle);
            AssertCircle(expected.RightCircle, actual.RightCircle);
            AssertSlices(expected.Slices, actual.Slices);
        }

        private static void AssertCircle(UpliftVanSlidingCircleResult circleResult, MacroStabilityInwardsSlidingCircle circleOutput)
        {
            Assert.AreEqual(circleResult.Center, circleOutput.Center);
            Assert.AreEqual(circleResult.IsActive, circleOutput.IsActive);
            Assert.AreEqual(circleResult.Radius, circleOutput.Radius);
            Assert.AreEqual(circleResult.DrivingMoment, circleOutput.DrivingMoment);
            Assert.AreEqual(circleResult.ResistingMoment, circleOutput.ResistingMoment);
            Assert.AreEqual(circleResult.IteratedForce, circleOutput.IteratedForce);
            Assert.AreEqual(circleResult.NonIteratedForce, circleOutput.NonIteratedForce);
        }

        private static void AssertSlices(IEnumerable<UpliftVanSliceResult> resultSlices, IEnumerable<MacroStabilityInwardsSlice> outputSlices)
        {
            UpliftVanSliceResult[] expectedSlices = resultSlices.ToArray();
            MacroStabilityInwardsSlice[] actualSlices = outputSlices.ToArray();

            Assert.AreEqual(expectedSlices.Length, actualSlices.Length);

            for (var i = 0; i < expectedSlices.Length; i++)
            {
                Assert.AreEqual(expectedSlices[i].Cohesion, actualSlices[i].Cohesion);
                Assert.AreEqual(expectedSlices[i].FrictionAngle, actualSlices[i].FrictionAngle);
                Assert.AreEqual(expectedSlices[i].CriticalPressure, actualSlices[i].CriticalPressure);
                Assert.AreEqual(expectedSlices[i].OverConsolidationRatio, actualSlices[i].OverConsolidationRatio);
                Assert.AreEqual(expectedSlices[i].Pop, actualSlices[i].Pop);
                Assert.AreEqual(expectedSlices[i].DegreeOfConsolidationPorePressureSoil, actualSlices[i].DegreeOfConsolidationPorePressureSoil);
                Assert.AreEqual(expectedSlices[i].DegreeOfConsolidationPorePressureLoad, actualSlices[i].DegreeOfConsolidationPorePressureLoad);
                Assert.AreEqual(expectedSlices[i].Dilatancy, actualSlices[i].Dilatancy);
                Assert.AreEqual(expectedSlices[i].ExternalLoad, actualSlices[i].ExternalLoad);
                Assert.AreEqual(expectedSlices[i].HydrostaticPorePressure, actualSlices[i].HydrostaticPorePressure);
                Assert.AreEqual(expectedSlices[i].LeftForce, actualSlices[i].LeftForce);
                Assert.AreEqual(expectedSlices[i].LeftForceAngle, actualSlices[i].LeftForceAngle);
                Assert.AreEqual(expectedSlices[i].LeftForceY, actualSlices[i].LeftForceY);
                Assert.AreEqual(expectedSlices[i].RightForce, actualSlices[i].RightForce);
                Assert.AreEqual(expectedSlices[i].RightForceAngle, actualSlices[i].RightForceAngle);
                Assert.AreEqual(expectedSlices[i].RightForceY, actualSlices[i].RightForceY);
                Assert.AreEqual(expectedSlices[i].LoadStress, actualSlices[i].LoadStress);
                Assert.AreEqual(expectedSlices[i].NormalStress, actualSlices[i].NormalStress);
                Assert.AreEqual(expectedSlices[i].PorePressure, actualSlices[i].PorePressure);
                Assert.AreEqual(expectedSlices[i].HorizontalPorePressure, actualSlices[i].HorizontalPorePressure);
                Assert.AreEqual(expectedSlices[i].VerticalPorePressure, actualSlices[i].VerticalPorePressure);
                Assert.AreEqual(expectedSlices[i].PiezometricPorePressure, actualSlices[i].PiezometricPorePressure);
                Assert.AreEqual(expectedSlices[i].EffectiveStress, actualSlices[i].EffectiveStress);
                Assert.AreEqual(expectedSlices[i].EffectiveStressDaily, actualSlices[i].EffectiveStressDaily);
                Assert.AreEqual(expectedSlices[i].ExcessPorePressure, actualSlices[i].ExcessPorePressure);
                Assert.AreEqual(expectedSlices[i].ShearStress, actualSlices[i].ShearStress);
                Assert.AreEqual(expectedSlices[i].SoilStress, actualSlices[i].SoilStress);
                Assert.AreEqual(expectedSlices[i].TotalPorePressure, actualSlices[i].TotalPorePressure);
                Assert.AreEqual(expectedSlices[i].TotalStress, actualSlices[i].TotalStress);
                Assert.AreEqual(expectedSlices[i].Weight, actualSlices[i].Weight);
            }
        }

        private static void AssertSlipPlane(UpliftVanCalculationGridResult expected, MacroStabilityInwardsSlipPlaneUpliftVan actual)
        {
            CollectionAssert.AreEqual(expected.TangentLines, actual.TangentLines);
            AssertGrid(expected.LeftGrid, actual.LeftGrid);
            AssertGrid(expected.RightGrid, actual.RightGrid);
        }

        private static void AssertGrid(UpliftVanGridResult expectedGrid, MacroStabilityInwardsGrid actualGrid)
        {
            Assert.AreEqual(expectedGrid.XLeft, actualGrid.XLeft);
            Assert.AreEqual(expectedGrid.XRight, actualGrid.XRight);
            Assert.AreEqual(expectedGrid.ZTop, actualGrid.ZTop);
            Assert.AreEqual(expectedGrid.ZBottom, actualGrid.ZBottom);
            Assert.AreEqual(expectedGrid.NumberOfHorizontalPoints, actualGrid.NumberOfHorizontalPoints);
            Assert.AreEqual(expectedGrid.NumberOfVerticalPoints, actualGrid.NumberOfVerticalPoints);
        }
    }
}