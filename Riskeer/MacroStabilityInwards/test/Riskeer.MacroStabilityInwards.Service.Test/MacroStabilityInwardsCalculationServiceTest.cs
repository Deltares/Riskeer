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
using Core.Common.TestUtil;
using log4net.Core;
using NUnit.Framework;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Service.TestUtil;
using Ringtoets.MacroStabilityInwards.CalculatedInput.TestUtil;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.MacroStabilityInwards.Data.TestUtil;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.Input;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.UpliftVan;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.UpliftVan.Input;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.UpliftVan.Output;
using Ringtoets.MacroStabilityInwards.KernelWrapper.TestUtil.Calculators;

namespace Riskeer.MacroStabilityInwards.Service.Test
{
    [TestFixture]
    public class MacroStabilityInwardsCalculationServiceTest
    {
        private MacroStabilityInwardsCalculationScenario testCalculation;

        [SetUp]
        public void Setup()
        {
            testCalculation = MacroStabilityInwardsCalculationScenarioTestFactory.CreateMacroStabilityInwardsCalculationScenarioWithValidInput(new TestHydraulicBoundaryLocation());
        }

        [Test]
        public void Validate_CalculationNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => MacroStabilityInwardsCalculationService.Validate(null, RoundedDouble.NaN);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("calculation", exception.ParamName);
        }

        [Test]
        public void Validate_Always_LogStartAndEndOfValidatingInputs()
        {
            // Setup
            using (new MacroStabilityInwardsCalculatorFactoryConfig())
            {
                // Call
                Action call = () => MacroStabilityInwardsCalculationService.Validate(testCalculation,
                                                                                     AssessmentSectionTestHelper.GetTestAssessmentLevel());

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                    CalculationServiceTestHelper.AssertValidationEndMessage(msgs[1]);
                });
            }
        }

        [Test]
        public void Validate_InvalidCalculationWithOutput_ReturnsFalseNoOutputChange()
        {
            // Setup
            MacroStabilityInwardsOutput output = MacroStabilityInwardsOutputTestFactory.CreateOutput();
            MacroStabilityInwardsCalculation invalidMacroStabilityInwardsCalculation = MacroStabilityInwardsCalculationScenarioTestFactory.CreateMacroStabilityInwardsCalculationScenarioWithInvalidInput();
            invalidMacroStabilityInwardsCalculation.Output = output;

            // Call
            bool isValid = MacroStabilityInwardsCalculationService.Validate(invalidMacroStabilityInwardsCalculation,
                                                                            AssessmentSectionTestHelper.GetTestAssessmentLevel());

            // Assert
            Assert.IsFalse(isValid);
            Assert.AreSame(output, invalidMacroStabilityInwardsCalculation.Output);
        }

        [Test]
        public void Validate_InvalidCalculationInput_LogsErrorAndReturnsFalse()
        {
            // Setup
            var calculation = new MacroStabilityInwardsCalculation();
            var isValid = true;

            // Call
            Action call = () => isValid = MacroStabilityInwardsCalculationService.Validate(calculation,
                                                                                           AssessmentSectionTestHelper.GetTestAssessmentLevel());

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(5, msgs.Length);
                CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                Assert.AreEqual("Er is geen hydraulische belastingenlocatie geselecteerd.", msgs[1]);
                Assert.AreEqual("Er is geen profielschematisatie geselecteerd.", msgs[2]);
                Assert.AreEqual("Er is geen ondergrondschematisatie geselecteerd.", msgs[3]);
                CalculationServiceTestHelper.AssertValidationEndMessage(msgs[4]);
            });
            Assert.IsFalse(isValid);
        }

        [Test]
        public void Validate_KernelReturnsValidationError_LogsErrorAndReturnsFalse()
        {
            // Setup
            var isValid = true;
            using (new MacroStabilityInwardsCalculatorFactoryConfig())
            {
                var calculator = (TestMacroStabilityInwardsCalculatorFactory) MacroStabilityInwardsCalculatorFactory.Instance;
                calculator.LastCreatedUpliftVanCalculator.ReturnValidationError = true;

                // Call
                Action call = () => isValid = MacroStabilityInwardsCalculationService.Validate(testCalculation,
                                                                                               AssessmentSectionTestHelper.GetTestAssessmentLevel());

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(3, msgs.Length);
                    CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                    Assert.AreEqual("Validation Error", msgs[1]);
                    CalculationServiceTestHelper.AssertValidationEndMessage(msgs[2]);
                });
                Assert.IsFalse(isValid);
            }
        }

        [Test]
        public void Validate_KernelReturnsValidationWarning_LogsWarningAndReturnsTrue()
        {
            // Setup
            var isValid = false;
            using (new MacroStabilityInwardsCalculatorFactoryConfig())
            {
                var calculator = (TestMacroStabilityInwardsCalculatorFactory) MacroStabilityInwardsCalculatorFactory.Instance;
                calculator.LastCreatedUpliftVanCalculator.ReturnValidationWarning = true;

                // Call
                Action call = () => isValid = MacroStabilityInwardsCalculationService.Validate(testCalculation,
                                                                                               AssessmentSectionTestHelper.GetTestAssessmentLevel());

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(3, msgs.Length);
                    CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                    Assert.AreEqual("Validation Warning", msgs[1]);
                    CalculationServiceTestHelper.AssertValidationEndMessage(msgs[2]);
                });
                Assert.IsTrue(isValid);
            }
        }

        [Test]
        public void Validate_KernelReturnsValidationErrorAndWarning_LogsErrorAndWarningAndReturnsFalse()
        {
            // Setup
            var isValid = true;
            using (new MacroStabilityInwardsCalculatorFactoryConfig())
            {
                var calculator = (TestMacroStabilityInwardsCalculatorFactory) MacroStabilityInwardsCalculatorFactory.Instance;
                calculator.LastCreatedUpliftVanCalculator.ReturnValidationWarning = true;
                calculator.LastCreatedUpliftVanCalculator.ReturnValidationError = true;

                // Call
                Action call = () => isValid = MacroStabilityInwardsCalculationService.Validate(testCalculation,
                                                                                               AssessmentSectionTestHelper.GetTestAssessmentLevel());

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(4, msgs.Length);
                    CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                    Assert.AreEqual("Validation Error", msgs[1]);
                    Assert.AreEqual("Validation Warning", msgs[2]);
                    CalculationServiceTestHelper.AssertValidationEndMessage(msgs[3]);
                });
                Assert.IsFalse(isValid);
            }
        }

        [Test]
        public void Validate_ErrorWhileValidating_LogErrorMessage()
        {
            // Setup
            using (new MacroStabilityInwardsCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestMacroStabilityInwardsCalculatorFactory) MacroStabilityInwardsCalculatorFactory.Instance;
                calculatorFactory.LastCreatedUpliftVanCalculator.ThrowExceptionOnValidate = true;

                // Call
                Action call = () => MacroStabilityInwardsCalculationService.Validate(testCalculation,
                                                                                     AssessmentSectionTestHelper.GetTestAssessmentLevel());

                // Assert
                TestHelper.AssertLogMessagesWithLevelAndLoggedExceptions(call, tuples =>
                {
                    Tuple<string, Level, Exception> tuple = tuples.ElementAt(1);
                    Assert.AreEqual("Macrostabiliteit binnenwaarts validatie mislukt.", tuple.Item1);
                    Assert.AreEqual(Level.Error, tuple.Item2);
                    Assert.IsInstanceOf<UpliftVanCalculatorException>(tuple.Item3);
                });
            }
        }

        [Test]
        public void Calculate_CalculationNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => MacroStabilityInwardsCalculationService.Calculate(null, RoundedDouble.NaN);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("calculation", exception.ParamName);
        }

        [Test]
        public void Calculate_ValidCalculation_LogStartAndEndOfValidatingInputsAndCalculation()
        {
            // Setup
            RoundedDouble normativeAssessmentLevel = AssessmentSectionTestHelper.GetTestAssessmentLevel();

            using (new MacroStabilityInwardsCalculatorFactoryConfig())
            {
                Action call = () =>
                {
                    // Precondition
                    Assert.IsTrue(MacroStabilityInwardsCalculationService.Validate(testCalculation, normativeAssessmentLevel));

                    // Call
                    MacroStabilityInwardsCalculationService.Calculate(testCalculation, normativeAssessmentLevel);
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
        public void Calculate_ValidCalculationWithOutput_ShouldChangeOutput()
        {
            // Setup
            RoundedDouble normativeAssessmentLevel = AssessmentSectionTestHelper.GetTestAssessmentLevel();
            MacroStabilityInwardsOutput output = MacroStabilityInwardsOutputTestFactory.CreateOutput();

            testCalculation.Output = output;

            using (new MacroStabilityInwardsCalculatorFactoryConfig())
            {
                // Precondition
                Assert.IsTrue(MacroStabilityInwardsCalculationService.Validate(testCalculation, normativeAssessmentLevel));

                // Call
                MacroStabilityInwardsCalculationService.Calculate(testCalculation, normativeAssessmentLevel);

                // Assert
                Assert.AreNotSame(output, testCalculation.Output);
            }
        }

        [Test]
        public void Calculate_DrainageConstructionPresentFalse_SetsInputOnCalculator()
        {
            // Setup
            var random = new Random(11);
            MacroStabilityInwardsInput inputParameters = testCalculation.InputParameters;
            inputParameters.DrainageConstructionPresent = false;
            inputParameters.XCoordinateDrainageConstruction = random.NextRoundedDouble();
            inputParameters.ZCoordinateDrainageConstruction = random.NextRoundedDouble();

            using (new MacroStabilityInwardsCalculatorFactoryConfig())
            {
                // Call
                MacroStabilityInwardsCalculationService.Calculate(testCalculation,
                                                                  AssessmentSectionTestHelper.GetTestAssessmentLevel());

                // Assert
                UpliftVanCalculatorInput actualInput = ((TestMacroStabilityInwardsCalculatorFactory) MacroStabilityInwardsCalculatorFactory.Instance)
                                                       .LastCreatedUpliftVanCalculator.Input;
                Assert.IsFalse(actualInput.DrainageConstruction.IsPresent);
                Assert.IsNaN(actualInput.DrainageConstruction.XCoordinate);
                Assert.IsNaN(actualInput.DrainageConstruction.ZCoordinate);
            }
        }

        [Test]
        public void Calculate_UseDefaultOffsetsTrue_SetsInputOnCalculator()
        {
            // Setup
            var random = new Random(11);
            MacroStabilityInwardsInput inputParameters = testCalculation.InputParameters;
            inputParameters.LocationInputExtreme.UseDefaultOffsets = true;
            inputParameters.LocationInputExtreme.PhreaticLineOffsetBelowDikeToeAtPolder = random.NextRoundedDouble();
            inputParameters.LocationInputExtreme.PhreaticLineOffsetBelowDikeTopAtPolder = random.NextRoundedDouble();
            inputParameters.LocationInputExtreme.PhreaticLineOffsetBelowDikeTopAtRiver = random.NextRoundedDouble();
            inputParameters.LocationInputExtreme.PhreaticLineOffsetBelowShoulderBaseInside = random.NextRoundedDouble();

            inputParameters.LocationInputDaily.UseDefaultOffsets = true;
            inputParameters.LocationInputDaily.PhreaticLineOffsetBelowDikeToeAtPolder = random.NextRoundedDouble();
            inputParameters.LocationInputDaily.PhreaticLineOffsetBelowDikeTopAtPolder = random.NextRoundedDouble();
            inputParameters.LocationInputDaily.PhreaticLineOffsetBelowDikeTopAtRiver = random.NextRoundedDouble();
            inputParameters.LocationInputDaily.PhreaticLineOffsetBelowShoulderBaseInside = random.NextRoundedDouble();

            using (new MacroStabilityInwardsCalculatorFactoryConfig())
            {
                // Call
                MacroStabilityInwardsCalculationService.Calculate(testCalculation,
                                                                  AssessmentSectionTestHelper.GetTestAssessmentLevel());

                // Assert
                UpliftVanCalculatorInput actualInput = ((TestMacroStabilityInwardsCalculatorFactory) MacroStabilityInwardsCalculatorFactory.Instance)
                                                       .LastCreatedUpliftVanCalculator.Input;
                Assert.IsTrue(actualInput.PhreaticLineOffsetsExtreme.UseDefaults);
                Assert.IsNaN(actualInput.PhreaticLineOffsetsExtreme.BelowDikeToeAtPolder);
                Assert.IsNaN(actualInput.PhreaticLineOffsetsExtreme.BelowDikeTopAtPolder);
                Assert.IsNaN(actualInput.PhreaticLineOffsetsExtreme.BelowDikeTopAtRiver);
                Assert.IsNaN(actualInput.PhreaticLineOffsetsExtreme.BelowShoulderBaseInside);

                Assert.IsTrue(actualInput.PhreaticLineOffsetsDaily.UseDefaults);
                Assert.IsNaN(actualInput.PhreaticLineOffsetsDaily.BelowDikeToeAtPolder);
                Assert.IsNaN(actualInput.PhreaticLineOffsetsDaily.BelowDikeTopAtPolder);
                Assert.IsNaN(actualInput.PhreaticLineOffsetsDaily.BelowDikeTopAtRiver);
                Assert.IsNaN(actualInput.PhreaticLineOffsetsDaily.BelowShoulderBaseInside);
            }
        }

        [Test]
        public void Calculate_GridDeterminationTypeAutomatic_SetsInputOnCalculator()
        {
            // Setup
            var random = new Random(11);
            MacroStabilityInwardsInput inputParameters = testCalculation.InputParameters;
            inputParameters.GridDeterminationType = MacroStabilityInwardsGridDeterminationType.Automatic;
            inputParameters.TangentLineZTop = random.NextRoundedDouble(2.0, 3.0);
            inputParameters.TangentLineZBottom = random.NextRoundedDouble(0.0, 1.0);
            inputParameters.TangentLineNumber = random.Next(1, 51);
            inputParameters.LeftGrid.XLeft = random.NextRoundedDouble(0.0, 0.1);
            inputParameters.LeftGrid.XRight = random.NextRoundedDouble(0.3, 0.4);
            inputParameters.LeftGrid.ZTop = random.NextRoundedDouble(2.0, 3.0);
            inputParameters.LeftGrid.ZBottom = random.NextRoundedDouble(0.0, 1.0);
            inputParameters.LeftGrid.NumberOfHorizontalPoints = random.Next(1, 100);
            inputParameters.LeftGrid.NumberOfVerticalPoints = random.Next(1, 100);
            inputParameters.RightGrid.XLeft = random.NextRoundedDouble(0.0, 0.1);
            inputParameters.RightGrid.XRight = random.NextRoundedDouble(0.3, 0.4);
            inputParameters.RightGrid.ZTop = random.NextRoundedDouble(2.0, 3.0);
            inputParameters.RightGrid.ZBottom = random.NextRoundedDouble(0.0, 1.0);
            inputParameters.RightGrid.NumberOfHorizontalPoints = random.Next(1, 100);
            inputParameters.RightGrid.NumberOfVerticalPoints = random.Next(1, 100);

            using (new MacroStabilityInwardsCalculatorFactoryConfig())
            {
                // Call
                MacroStabilityInwardsCalculationService.Calculate(testCalculation,
                                                                  AssessmentSectionTestHelper.GetTestAssessmentLevel());

                // Assert
                UpliftVanCalculatorInput actualInput = ((TestMacroStabilityInwardsCalculatorFactory) MacroStabilityInwardsCalculatorFactory.Instance)
                                                       .LastCreatedUpliftVanCalculator.Input;
                Assert.IsTrue(actualInput.SlipPlane.GridAutomaticDetermined);
                Assert.IsNull(actualInput.SlipPlane.LeftGrid);
                Assert.IsNull(actualInput.SlipPlane.RightGrid);
                Assert.IsTrue(actualInput.SlipPlane.TangentLinesAutomaticAtBoundaries);
                Assert.IsNaN(actualInput.SlipPlane.TangentZTop);
                Assert.IsNaN(actualInput.SlipPlane.TangentZBottom);
                Assert.AreEqual(0, actualInput.SlipPlane.TangentLineNumber);
            }
        }

        [Test]
        [TestCase(true, MacroStabilityInwardsZoningBoundariesDeterminationType.Automatic)]
        [TestCase(false, MacroStabilityInwardsZoningBoundariesDeterminationType.Automatic)]
        [TestCase(false, MacroStabilityInwardsZoningBoundariesDeterminationType.Manual)]
        public void Calculate_ZoningBoundariesDeterminationTypeAutomatic_SetsInputOnCalculator(bool createZones,
                                                                                               MacroStabilityInwardsZoningBoundariesDeterminationType zoningBoundariesDeterminationType)
        {
            // Setup
            var random = new Random(11);
            MacroStabilityInwardsInput inputParameters = testCalculation.InputParameters;
            inputParameters.CreateZones = createZones;
            inputParameters.ZoningBoundariesDeterminationType = zoningBoundariesDeterminationType;

            inputParameters.SlipPlaneMinimumDepth = random.NextRoundedDouble();
            inputParameters.SlipPlaneMinimumLength = random.NextRoundedDouble();
            inputParameters.ZoneBoundaryLeft = random.NextRoundedDouble();
            inputParameters.ZoneBoundaryRight = random.NextRoundedDouble();

            using (new MacroStabilityInwardsCalculatorFactoryConfig())
            {
                // Call
                MacroStabilityInwardsCalculationService.Calculate(testCalculation,
                                                                  AssessmentSectionTestHelper.GetTestAssessmentLevel());

                // Assert
                UpliftVanCalculatorInput actualInput = ((TestMacroStabilityInwardsCalculatorFactory) MacroStabilityInwardsCalculatorFactory.Instance)
                                                       .LastCreatedUpliftVanCalculator.Input;
                Assert.IsTrue(actualInput.SlipPlaneConstraints.AutomaticForbiddenZones);
                Assert.AreEqual(createZones, actualInput.SlipPlaneConstraints.CreateZones);
                Assert.IsNaN(actualInput.SlipPlaneConstraints.ZoneBoundaryLeft);
                Assert.IsNaN(actualInput.SlipPlaneConstraints.ZoneBoundaryRight);
                Assert.AreEqual(inputParameters.SlipPlaneMinimumDepth, actualInput.SlipPlaneConstraints.SlipPlaneMinimumDepth);
                Assert.AreEqual(inputParameters.SlipPlaneMinimumLength, actualInput.SlipPlaneConstraints.SlipPlaneMinimumLength);
            }
        }

        [Test]
        public void Calculate_CalculationRan_SetOutput()
        {
            // Setup
            RoundedDouble normativeAssessmentLevel = AssessmentSectionTestHelper.GetTestAssessmentLevel();

            using (new MacroStabilityInwardsCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestMacroStabilityInwardsCalculatorFactory) MacroStabilityInwardsCalculatorFactory.Instance;

                // Precondition
                Assert.IsTrue(MacroStabilityInwardsCalculationService.Validate(testCalculation, normativeAssessmentLevel));

                // Call
                MacroStabilityInwardsCalculationService.Calculate(testCalculation, normativeAssessmentLevel);

                // Assert
                AssertOutput(calculatorFactory.LastCreatedUpliftVanCalculator.Output, testCalculation.Output);
            }
        }

        [Test]
        public void Calculate_ErrorWhileCalculating_LogErrorMessageAndThrowException()
        {
            // Setup
            RoundedDouble normativeAssessmentLevel = AssessmentSectionTestHelper.GetTestAssessmentLevel();

            using (new MacroStabilityInwardsCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestMacroStabilityInwardsCalculatorFactory) MacroStabilityInwardsCalculatorFactory.Instance;
                calculatorFactory.LastCreatedUpliftVanCalculator.ThrowExceptionOnCalculate = true;

                // Precondition
                Assert.IsTrue(MacroStabilityInwardsCalculationService.Validate(testCalculation, normativeAssessmentLevel));

                var exceptionThrown = false;

                // Call
                Action call = () =>
                {
                    try
                    {
                        MacroStabilityInwardsCalculationService.Calculate(testCalculation, normativeAssessmentLevel);
                    }
                    catch (UpliftVanCalculatorException)
                    {
                        exceptionThrown = true;
                    }
                };

                // Assert
                TestHelper.AssertLogMessagesWithLevelAndLoggedExceptions(call, tuples =>
                {
                    Tuple<string, Level, Exception>[] messages = tuples as Tuple<string, Level, Exception>[] ?? tuples.ToArray();
                    Assert.AreEqual(3, messages.Length);

                    CalculationServiceTestHelper.AssertCalculationStartMessage(messages[0].Item1);

                    Tuple<string, Level, Exception> tuple1 = messages[1];
                    Assert.AreEqual("Er is een onverwachte fout opgetreden tijdens het uitvoeren van de berekening.", tuple1.Item1);
                    Assert.AreEqual(Level.Error, tuple1.Item2);
                    Assert.IsInstanceOf<UpliftVanCalculatorException>(tuple1.Item3);

                    CalculationServiceTestHelper.AssertCalculationEndMessage(messages[2].Item1);
                });

                Assert.IsTrue(exceptionThrown);
                Assert.IsNull(testCalculation.Output);
            }
        }

        [Test]
        public void Calculate_KernelReturnsCalculationErrors_LogAggregatedErrorMessageAndThrowException()
        {
            // Setup
            using (new MacroStabilityInwardsCalculatorFactoryConfig())
            {
                var calculator = (TestMacroStabilityInwardsCalculatorFactory) MacroStabilityInwardsCalculatorFactory.Instance;
                calculator.LastCreatedUpliftVanCalculator.ReturnCalculationError = true;

                var exceptionThrown = false;

                // Call
                Action call = () =>
                {
                    try
                    {
                        MacroStabilityInwardsCalculationService.Calculate(testCalculation,
                                                                          AssessmentSectionTestHelper.GetTestAssessmentLevel());
                    }
                    catch (UpliftVanCalculatorException)
                    {
                        exceptionThrown = true;
                    }
                };

                // Assert
                TestHelper.AssertLogMessagesWithLevelAndLoggedExceptions(call, messages =>
                {
                    Tuple<string, Level, Exception>[] tupleArray = messages.ToArray();
                    string[] msgs = tupleArray.Select(tuple => tuple.Item1).ToArray();

                    Assert.AreEqual(3, msgs.Length);

                    string expectedErrorMessage = "Er zijn een of meerdere fouten opgetreden. Klik op details voor meer informatie."
                                                  + $"{Environment.NewLine}"
                                                  + "* Calculation Error 1"
                                                  + $"{Environment.NewLine}"
                                                  + "* Calculation Error 2";

                    CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[0]);
                    Assert.AreEqual(expectedErrorMessage, msgs[1]);
                    CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[2]);

                    Assert.AreEqual(Level.Error, tupleArray[1].Item2);
                });

                Assert.IsTrue(exceptionThrown);
                Assert.IsNull(testCalculation.Output);
            }
        }

        [Test]
        public void Calculate_KernelReturnsCalculationWarnings_LogAggregatedWarningMessageAndSetOutput()
        {
            // Setup
            using (new MacroStabilityInwardsCalculatorFactoryConfig())
            {
                var calculator = (TestMacroStabilityInwardsCalculatorFactory) MacroStabilityInwardsCalculatorFactory.Instance;
                calculator.LastCreatedUpliftVanCalculator.ReturnCalculationWarning = true;

                // Call
                Action call = () => MacroStabilityInwardsCalculationService.Calculate(testCalculation,
                                                                                      AssessmentSectionTestHelper.GetTestAssessmentLevel());

                // Assert
                TestHelper.AssertLogMessagesWithLevelAndLoggedExceptions(call, messages =>
                {
                    Tuple<string, Level, Exception>[] tupleArray = messages.ToArray();
                    string[] msgs = tupleArray.Select(tuple => tuple.Item1).ToArray();

                    Assert.AreEqual(3, msgs.Length);

                    string expectedWarningMessage = "Er zijn een of meerdere waarschuwingsberichten. Klik op details voor meer informatie."
                                                    + $"{Environment.NewLine}"
                                                    + "* Calculation Warning 1"
                                                    + $"{Environment.NewLine}"
                                                    + "* Calculation Warning 2";

                    CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[0]);
                    Assert.AreEqual(expectedWarningMessage, msgs[1]);
                    CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[2]);

                    Assert.AreEqual(Level.Warn, tupleArray[1].Item2);
                });
            }

            Assert.IsNotNull(testCalculation.Output);
        }

        [Test]
        public void Calculate_KernelReturnsCalculationErrorsAndWarnings_LogAggregatedErrorMessageAndThrowException()
        {
            // Setup
            using (new MacroStabilityInwardsCalculatorFactoryConfig())
            {
                var calculator = (TestMacroStabilityInwardsCalculatorFactory) MacroStabilityInwardsCalculatorFactory.Instance;
                calculator.LastCreatedUpliftVanCalculator.ReturnCalculationWarning = true;
                calculator.LastCreatedUpliftVanCalculator.ReturnCalculationError = true;

                var exceptionThrown = false;

                // Call
                Action call = () =>
                {
                    try
                    {
                        MacroStabilityInwardsCalculationService.Calculate(testCalculation,
                                                                          AssessmentSectionTestHelper.GetTestAssessmentLevel());
                    }
                    catch (UpliftVanCalculatorException)
                    {
                        exceptionThrown = true;
                    }
                };

                // Assert
                TestHelper.AssertLogMessagesWithLevelAndLoggedExceptions(call, messages =>
                {
                    Tuple<string, Level, Exception>[] tupleArray = messages.ToArray();
                    string[] msgs = tupleArray.Select(tuple => tuple.Item1).ToArray();

                    Assert.AreEqual(3, msgs.Length);

                    string expectedErrorMessage = "Er zijn een of meerdere fouten opgetreden. Klik op details voor meer informatie."
                                                  + $"{Environment.NewLine}"
                                                  + "* Calculation Error 1"
                                                  + $"{Environment.NewLine}"
                                                  + "* Calculation Warning 1"
                                                  + $"{Environment.NewLine}"
                                                  + "* Calculation Error 2"
                                                  + $"{Environment.NewLine}"
                                                  + "* Calculation Warning 2";

                    CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[0]);
                    Assert.AreEqual(expectedErrorMessage, msgs[1]);
                    CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[2]);

                    Assert.AreEqual(Level.Error, tupleArray[1].Item2);
                });

                Assert.IsTrue(exceptionThrown);
                Assert.IsNull(testCalculation.Output);
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public void Calculate_CompleteInput_SetsInputOnCalculator(bool useAssessmentLevelManualInput)
        {
            // Setup
            RoundedDouble normativeAssessmentLevel = AssessmentSectionTestHelper.GetTestAssessmentLevel();
            MacroStabilityInwardsInput input = testCalculation.InputParameters;

            input.AssessmentLevel = (RoundedDouble) 2.2;

            input.UseAssessmentLevelManualInput = useAssessmentLevelManualInput;
            using (new MacroStabilityInwardsCalculatorFactoryConfig())
            {
                // Call
                MacroStabilityInwardsCalculationService.Calculate(testCalculation, normativeAssessmentLevel);

                // Assert
                RoundedDouble expectedAssessmentLevel = useAssessmentLevelManualInput
                                                            ? testCalculation.InputParameters.AssessmentLevel
                                                            : normativeAssessmentLevel;

                AssertInput(testCalculation.InputParameters, (TestMacroStabilityInwardsCalculatorFactory) MacroStabilityInwardsCalculatorFactory.Instance, expectedAssessmentLevel);
            }
        }

        private static void AssertInput(MacroStabilityInwardsInput originalInput, TestMacroStabilityInwardsCalculatorFactory factory, RoundedDouble expectedAssessmentLevel)
        {
            UpliftVanCalculatorInput actualInput = factory.LastCreatedUpliftVanCalculator.Input;
            CalculatorInputAssert.AssertSoilProfile(originalInput.SoilProfileUnderSurfaceLine, actualInput.SoilProfile);
            CalculatorInputAssert.AssertDrainageConstruction(originalInput, actualInput.DrainageConstruction);
            CalculatorInputAssert.AssertPhreaticLineOffsets(originalInput.LocationInputExtreme, actualInput.PhreaticLineOffsetsExtreme);
            CalculatorInputAssert.AssertPhreaticLineOffsets(originalInput.LocationInputDaily, actualInput.PhreaticLineOffsetsDaily);
            AssertSlipPlaneInput(originalInput, actualInput.SlipPlane);
            AssertSlipPlaneConstraints(originalInput, actualInput.SlipPlaneConstraints);
            Assert.AreEqual(WaternetCreationMode.CreateWaternet, actualInput.WaternetCreationMode);
            Assert.AreEqual(PlLineCreationMethod.RingtoetsWti2017, actualInput.PlLineCreationMethod);
            Assert.AreEqual(LandwardDirection.PositiveX, actualInput.LandwardDirection);
            Assert.AreSame(originalInput.SurfaceLine, actualInput.SurfaceLine);
            Assert.AreEqual(expectedAssessmentLevel, actualInput.AssessmentLevel);
            Assert.AreEqual(originalInput.DikeSoilScenario, actualInput.DikeSoilScenario);
            Assert.AreEqual(originalInput.WaterLevelRiverAverage, actualInput.WaterLevelRiverAverage);
            Assert.AreEqual(originalInput.LocationInputExtreme.WaterLevelPolder, actualInput.WaterLevelPolderExtreme);
            Assert.AreEqual(originalInput.LocationInputDaily.WaterLevelPolder, actualInput.WaterLevelPolderDaily);
            Assert.AreEqual(originalInput.DrainageConstructionPresent, actualInput.DrainageConstruction.IsPresent);
            Assert.AreEqual(originalInput.XCoordinateDrainageConstruction, actualInput.DrainageConstruction.XCoordinate);
            Assert.AreEqual(originalInput.ZCoordinateDrainageConstruction, actualInput.DrainageConstruction.ZCoordinate);
            Assert.AreEqual(originalInput.MinimumLevelPhreaticLineAtDikeTopRiver, actualInput.MinimumLevelPhreaticLineAtDikeTopRiver);
            Assert.AreEqual(originalInput.MinimumLevelPhreaticLineAtDikeTopPolder, actualInput.MinimumLevelPhreaticLineAtDikeTopPolder);
            Assert.AreEqual(originalInput.LeakageLengthOutwardsPhreaticLine3, actualInput.LeakageLengthOutwardsPhreaticLine3);
            Assert.AreEqual(originalInput.LeakageLengthInwardsPhreaticLine3, actualInput.LeakageLengthInwardsPhreaticLine3);
            Assert.AreEqual(originalInput.LeakageLengthOutwardsPhreaticLine4, actualInput.LeakageLengthOutwardsPhreaticLine4);
            Assert.AreEqual(originalInput.LeakageLengthInwardsPhreaticLine4, actualInput.LeakageLengthInwardsPhreaticLine4);
            Assert.AreEqual(originalInput.PiezometricHeadPhreaticLine2Outwards, actualInput.PiezometricHeadPhreaticLine2Outwards);
            Assert.AreEqual(originalInput.PiezometricHeadPhreaticLine2Inwards, actualInput.PiezometricHeadPhreaticLine2Inwards);
            Assert.AreEqual(originalInput.LocationInputExtreme.PenetrationLength, actualInput.PenetrationLengthExtreme);
            Assert.AreEqual(originalInput.LocationInputDaily.PenetrationLength, actualInput.PenetrationLengthDaily);
            Assert.AreEqual(originalInput.AdjustPhreaticLine3And4ForUplift, actualInput.AdjustPhreaticLine3And4ForUplift);
            Assert.AreEqual(originalInput.MoveGrid, actualInput.MoveGrid);
            Assert.AreEqual(originalInput.MaximumSliceWidth, actualInput.MaximumSliceWidth);
        }

        private static void AssertSlipPlaneConstraints(MacroStabilityInwardsInput originalInput, UpliftVanSlipPlaneConstraints actualConstraints)
        {
            Assert.AreEqual(originalInput.CreateZones, actualConstraints.CreateZones);
            Assert.AreEqual(originalInput.ZoningBoundariesDeterminationType == MacroStabilityInwardsZoningBoundariesDeterminationType.Automatic, actualConstraints.AutomaticForbiddenZones);
            Assert.AreEqual(originalInput.ZoneBoundaryLeft, actualConstraints.ZoneBoundaryLeft);
            Assert.AreEqual(originalInput.ZoneBoundaryRight, actualConstraints.ZoneBoundaryRight);
            Assert.AreEqual(originalInput.SlipPlaneMinimumDepth, actualConstraints.SlipPlaneMinimumDepth);
            Assert.AreEqual(originalInput.SlipPlaneMinimumLength, actualConstraints.SlipPlaneMinimumLength);
        }

        private static void AssertSlipPlaneInput(MacroStabilityInwardsInput originalInput, UpliftVanSlipPlane actualInput)
        {
            Assert.IsFalse(actualInput.GridAutomaticDetermined);
            Assert.AreEqual(originalInput.LeftGrid.XLeft, actualInput.LeftGrid.XLeft);
            Assert.AreEqual(originalInput.LeftGrid.XRight, actualInput.LeftGrid.XRight);
            Assert.AreEqual(originalInput.LeftGrid.ZTop, actualInput.LeftGrid.ZTop);
            Assert.AreEqual(originalInput.LeftGrid.ZBottom, actualInput.LeftGrid.ZBottom);
            Assert.AreEqual(originalInput.LeftGrid.NumberOfHorizontalPoints, actualInput.LeftGrid.NumberOfHorizontalPoints);
            Assert.AreEqual(originalInput.LeftGrid.NumberOfVerticalPoints, actualInput.LeftGrid.NumberOfVerticalPoints);
            Assert.AreEqual(originalInput.RightGrid.XLeft, actualInput.RightGrid.XLeft);
            Assert.AreEqual(originalInput.RightGrid.XRight, actualInput.RightGrid.XRight);
            Assert.AreEqual(originalInput.RightGrid.ZTop, actualInput.RightGrid.ZTop);
            Assert.AreEqual(originalInput.RightGrid.ZBottom, actualInput.RightGrid.ZBottom);
            Assert.AreEqual(originalInput.RightGrid.NumberOfHorizontalPoints, actualInput.RightGrid.NumberOfHorizontalPoints);
            Assert.AreEqual(originalInput.RightGrid.NumberOfVerticalPoints, actualInput.RightGrid.NumberOfVerticalPoints);
            Assert.IsFalse(actualInput.TangentLinesAutomaticAtBoundaries);
            Assert.AreEqual(originalInput.TangentLineZTop, actualInput.TangentZTop);
            Assert.AreEqual(originalInput.TangentLineZBottom, actualInput.TangentZBottom);
            Assert.AreEqual(originalInput.TangentLineNumber, actualInput.TangentLineNumber);
        }

        private static void AssertOutput(UpliftVanCalculatorResult expectedOutput, MacroStabilityInwardsOutput actualOutput)
        {
            Assert.AreEqual(expectedOutput.FactorOfStability, actualOutput.FactorOfStability);
            Assert.AreEqual(expectedOutput.ZValue, actualOutput.ZValue);
            Assert.AreEqual(expectedOutput.ForbiddenZonesXEntryMin, actualOutput.ForbiddenZonesXEntryMin);
            Assert.AreEqual(expectedOutput.ForbiddenZonesXEntryMax, actualOutput.ForbiddenZonesXEntryMax);
            AssertSlidingCurve(expectedOutput.SlidingCurveResult, actualOutput.SlidingCurve);
            AssertSlipPlaneOutput(expectedOutput.CalculationGridResult, actualOutput.SlipPlane);
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
                Assert.AreSame(expectedSlices[i].TopLeftPoint, actualSlices[i].TopLeftPoint);
                Assert.AreSame(expectedSlices[i].TopRightPoint, actualSlices[i].TopRightPoint);
                Assert.AreSame(expectedSlices[i].BottomLeftPoint, actualSlices[i].BottomLeftPoint);
                Assert.AreSame(expectedSlices[i].BottomRightPoint, actualSlices[i].BottomRightPoint);
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

        private static void AssertSlipPlaneOutput(UpliftVanCalculationGridResult expected, MacroStabilityInwardsSlipPlaneUpliftVan actual)
        {
            CollectionAssert.AreEqual(expected.TangentLines, actual.TangentLines.Select(tl => tl.Value), new DoubleWithToleranceComparer(1e-2));
            AssertGrid(expected.LeftGrid, actual.LeftGrid);
            AssertGrid(expected.RightGrid, actual.RightGrid);
        }

        private static void AssertGrid(UpliftVanGrid expectedGrid, MacroStabilityInwardsGrid actualGrid)
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