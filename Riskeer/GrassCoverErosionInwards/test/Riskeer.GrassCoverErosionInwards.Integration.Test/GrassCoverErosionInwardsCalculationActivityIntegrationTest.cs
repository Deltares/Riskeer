﻿// Copyright (C) Stichting Deltares 2022. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using System.IO;
using System.Linq;
using Core.Common.Base;
using Core.Common.Base.Geometry;
using Core.Common.Base.Service;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.DikeProfiles;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Service;
using Riskeer.Common.Service.TestUtil;
using Riskeer.GrassCoverErosionInwards.Data;
using Riskeer.GrassCoverErosionInwards.Service;
using Riskeer.GrassCoverErosionInwards.Service.TestUtil;
using Riskeer.HydraRing.Calculation.Calculator.Factory;
using Riskeer.HydraRing.Calculation.Data;
using Riskeer.HydraRing.Calculation.Data.Input;
using Riskeer.HydraRing.Calculation.Data.Input.Hydraulics;
using Riskeer.HydraRing.Calculation.Data.Input.Overtopping;
using Riskeer.HydraRing.Calculation.TestUtil;
using Riskeer.HydraRing.Calculation.TestUtil.Calculator;
using Riskeer.Integration.Data;
using Riskeer.Integration.TestUtil;

namespace Riskeer.GrassCoverErosionInwards.Integration.Test
{
    [TestFixture]
    public class GrassCoverErosionInwardsCalculationActivityIntegrationTest
    {
        private static readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Integration.Service, "HydraRingCalculation");

        [Test]
        public void Run_InvalidHydraulicBoundaryDatabase_LogValidationStartAndEndWithError()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike)
            {
                HydraulicBoundaryData =
                {
                    FilePath = Path.Combine(testDataPath, "notexisting.sqlite")
                }
            };

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    DikeProfile = CreateDikeProfile()
                }
            };

            CalculatableActivity activity = GrassCoverErosionInwardsCalculationActivityFactory.CreateCalculationActivity(calculation,
                                                                                                                         assessmentSection.GrassCoverErosionInwards,
                                                                                                                         assessmentSection);

            // Call
            Action call = () => activity.Run();

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(4, msgs.Length);
                Assert.AreEqual($"Uitvoeren van berekening '{calculation.Name}' is gestart.", msgs[0]);
                CalculationServiceTestHelper.AssertValidationStartMessage(msgs[1]);
                StringAssert.StartsWith("Herstellen van de verbinding met de hydraulische belastingendatabase is mislukt. Fout bij het lezen van bestand", msgs[2]);
                CalculationServiceTestHelper.AssertValidationEndMessage(msgs[3]);
            });
            Assert.AreEqual(ActivityState.Failed, activity.State);
        }

        [Test]
        [Combinatorial]
        public void Run_CombinationOfCalculations_ProgressTextSetAccordingly([Values(true, false)] bool shouldDikeHeightBeCalculated,
                                                                             [Values(true, false)] bool shouldOvertoppingRateBeCalculated)
        {
            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Stub(cf => cf.CreateOvertoppingCalculator(null))
                             .IgnoreArguments()
                             .Return(new TestOvertoppingCalculator());
            calculatorFactory.Stub(cf => cf.CreateOvertoppingRateCalculator(null))
                             .IgnoreArguments()
                             .Return(new TestHydraulicLoadsCalculator());
            calculatorFactory.Stub(cf => cf.CreateDikeHeightCalculator(null))
                             .IgnoreArguments()
                             .Return(new TestHydraulicLoadsCalculator());
            mockRepository.ReplayAll();

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            ImportHydraulicBoundaryDatabase(assessmentSection);

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryData.Locations.First(hl => hl.Id == 1300001),
                    DikeProfile = CreateDikeProfile(),
                    ShouldDikeHeightBeCalculated = shouldDikeHeightBeCalculated,
                    ShouldOvertoppingRateBeCalculated = shouldOvertoppingRateBeCalculated
                }
            };

            CalculatableActivity activity = GrassCoverErosionInwardsCalculationActivityFactory.CreateCalculationActivity(calculation,
                                                                                                                         assessmentSection.GrassCoverErosionInwards,
                                                                                                                         assessmentSection);
            var progressTexts = "";

            activity.ProgressChanged += (s, e) => progressTexts += activity.ProgressText + Environment.NewLine;

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                activity.Run();
            }

            // Assert
            int totalNumberOfCalculations = shouldDikeHeightBeCalculated
                                                ? shouldOvertoppingRateBeCalculated
                                                      ? 3
                                                      : 2
                                                : shouldOvertoppingRateBeCalculated
                                                    ? 2
                                                    : 1;

            string expectedProgressTexts = $"Stap 1 van {totalNumberOfCalculations} | Uitvoeren overloop en overslag berekening" + Environment.NewLine;

            if (shouldDikeHeightBeCalculated)
            {
                expectedProgressTexts += $"Stap 2 van {totalNumberOfCalculations} | Uitvoeren HBN berekening" + Environment.NewLine;
            }

            if (shouldOvertoppingRateBeCalculated)
            {
                expectedProgressTexts += $"Stap {totalNumberOfCalculations} van {totalNumberOfCalculations} | Uitvoeren overslagdebiet berekening" + Environment.NewLine;
            }

            Assert.AreEqual(expectedProgressTexts, progressTexts);
            mockRepository.VerifyAll();
        }

        private static void ImportHydraulicBoundaryDatabase(AssessmentSection assessmentSection)
        {
            DataImportHelper.ImportHydraulicBoundaryData(assessmentSection,
                                                         Path.Combine(testDataPath, "hlcd.sqlite"),
                                                         Path.Combine(testDataPath, "HRD dutch coast south.sqlite"));
        }

        private static DikeProfile CreateDikeProfile()
        {
            return new DikeProfile(new Point2D(0, 0),
                                   new[]
                                   {
                                       new RoughnessPoint(new Point2D(1.1, 2.2), 0.6),
                                       new RoughnessPoint(new Point2D(3.3, 4.4), 0.7)
                                   }, new[]
                                   {
                                       new Point2D(3.3, 4.4),
                                       new Point2D(5.5, 6.6)
                                   },
                                   new BreakWater(BreakWaterType.Dam, 10.0),
                                   new DikeProfile.ConstructionProperties
                                   {
                                       Id = "id",
                                       Orientation = 5.5,
                                       DikeHeight = 10
                                   });
        }

        #region Overtopping calculations

        [Test]
        [TestCase(BreakWaterType.Caisson)]
        [TestCase(BreakWaterType.Wall)]
        [TestCase(BreakWaterType.Dam)]
        public void Run_ValidOvertoppingCalculation_InputPropertiesCorrectlySentToService(BreakWaterType breakWaterType)
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);

            ImportHydraulicBoundaryDatabase(assessmentSection);

            DikeProfile dikeProfile = CreateDikeProfile();
            dikeProfile.BreakWater.Type = breakWaterType;

            HydraulicBoundaryLocation hydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryData.Locations.First(hl => hl.Id == 1300001);

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                    DikeProfile = dikeProfile
                }
            };

            var calculator = new TestOvertoppingCalculator();
            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(Arg<HydraRingCalculationSettings>.Is.NotNull))
                             .WhenCalled(invocation =>
                             {
                                 HydraRingCalculationSettingsTestHelper.AssertHydraRingCalculationSettings(
                                     HydraulicBoundaryCalculationSettingsFactory.CreateSettings(
                                         assessmentSection.HydraulicBoundaryData,
                                         hydraulicBoundaryLocation),
                                     (HydraRingCalculationSettings) invocation.Arguments[0]);
                             })
                             .Return(calculator);
            mockRepository.ReplayAll();

            CalculatableActivity activity = GrassCoverErosionInwardsCalculationActivityFactory.CreateCalculationActivity(calculation,
                                                                                                                         assessmentSection.GrassCoverErosionInwards,
                                                                                                                         assessmentSection);
            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                activity.Run();

                // Assert
                OvertoppingCalculationInput[] overtoppingCalculationInputs = calculator.ReceivedInputs.ToArray();
                Assert.AreEqual(1, overtoppingCalculationInputs.Length);

                OvertoppingCalculationInput actualInput = overtoppingCalculationInputs[0];
                GeneralGrassCoverErosionInwardsInput generalInput = assessmentSection.GrassCoverErosionInwards.GeneralInput;

                GrassCoverErosionInwardsInput input = calculation.InputParameters;
                var expectedInput = new OvertoppingCalculationInput(input.HydraulicBoundaryLocation.Id,
                                                                    input.Orientation,
                                                                    input.DikeGeometry.Select(roughnessPoint => new HydraRingRoughnessProfilePoint(roughnessPoint.Point.X, roughnessPoint.Point.Y, roughnessPoint.Roughness)),
                                                                    input.ForeshoreGeometry.Select(c => new HydraRingForelandPoint(c.X, c.Y)),
                                                                    new HydraRingBreakWater(BreakWaterTypeHelper.GetHydraRingBreakWaterType(breakWaterType), input.BreakWater.Height),
                                                                    input.DikeHeight,
                                                                    generalInput.CriticalOvertoppingModelFactor,
                                                                    generalInput.FbFactor.Mean,
                                                                    generalInput.FbFactor.StandardDeviation,
                                                                    generalInput.FbFactor.LowerBoundary,
                                                                    generalInput.FbFactor.UpperBoundary,
                                                                    generalInput.FnFactor.Mean,
                                                                    generalInput.FnFactor.StandardDeviation,
                                                                    generalInput.FnFactor.LowerBoundary,
                                                                    generalInput.FnFactor.UpperBoundary,
                                                                    generalInput.OvertoppingModelFactor,
                                                                    input.CriticalFlowRate.Mean,
                                                                    input.CriticalFlowRate.StandardDeviation,
                                                                    generalInput.FrunupModelFactor.Mean,
                                                                    generalInput.FrunupModelFactor.StandardDeviation,
                                                                    generalInput.FrunupModelFactor.LowerBoundary,
                                                                    generalInput.FrunupModelFactor.UpperBoundary,
                                                                    generalInput.FshallowModelFactor.Mean,
                                                                    generalInput.FshallowModelFactor.StandardDeviation,
                                                                    generalInput.FshallowModelFactor.LowerBoundary,
                                                                    generalInput.FshallowModelFactor.UpperBoundary);

                HydraRingDataEqualityHelper.AreEqual(expectedInput, actualInput);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        [TestCaseSource(typeof(HydraRingCalculatorTestCaseProvider), nameof(HydraRingCalculatorTestCaseProvider.GetCalculatorFailingConditionsWithReportDetails), new object[]
        {
            nameof(Run_InvalidOvertoppingCalculationAndRan_LogErrorAndActivityStateFailed)
        })]
        public void Run_InvalidOvertoppingCalculationAndRan_LogErrorAndActivityStateFailed(bool endInFailure,
                                                                                           string lastErrorFileContent,
                                                                                           string detailedReport)
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            ImportHydraulicBoundaryDatabase(assessmentSection);

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryData.Locations.First(hl => hl.Id == 1300001),
                    DikeProfile = CreateDikeProfile()
                }
            };

            var calculator = new TestOvertoppingCalculator
            {
                LastErrorFileContent = lastErrorFileContent,
                EndInFailure = endInFailure
            };
            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(null))
                             .IgnoreArguments()
                             .Return(calculator);
            mockRepository.ReplayAll();

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                CalculatableActivity activity = GrassCoverErosionInwardsCalculationActivityFactory.CreateCalculationActivity(calculation,
                                                                                                                             assessmentSection.GrassCoverErosionInwards,
                                                                                                                             assessmentSection);
                // Call
                Action call = () => activity.Run();

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(7, msgs.Length);
                    Assert.AreEqual($"Uitvoeren van berekening '{calculation.Name}' is gestart.", msgs[0]);
                    CalculationServiceTestHelper.AssertValidationStartMessage(msgs[1]);
                    CalculationServiceTestHelper.AssertValidationEndMessage(msgs[2]);
                    CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[3]);
                    GrassCoverErosionInwardsCalculationServiceTestHelper.AssertCalculationFailedMessage(GrassCoverErosionInwardsCalculationServiceTestHelper.OvertoppingCalculationDescription, calculation.Name, calculator.LastErrorFileContent, msgs[4]);
                    GrassCoverErosionInwardsCalculationServiceTestHelper.AssertCalculationFinishedMessage(GrassCoverErosionInwardsCalculationServiceTestHelper.OvertoppingCalculationDescription, calculator.OutputDirectory, msgs[5]);
                    CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[6]);
                });
                Assert.AreEqual(ActivityState.Failed, activity.State);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        public void Run_ValidOvertoppingCalculation_PerformValidationAndCalculationAndLogStartAndEnd()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            ImportHydraulicBoundaryDatabase(assessmentSection);

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryData.Locations.First(hl => hl.Id == 1300001),
                    DikeProfile = CreateDikeProfile()
                }
            };
            var overtoppingCalculator = new TestOvertoppingCalculator();

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(null))
                             .IgnoreArguments()
                             .Return(overtoppingCalculator);
            mockRepository.ReplayAll();

            CalculatableActivity activity = GrassCoverErosionInwardsCalculationActivityFactory.CreateCalculationActivity(calculation,
                                                                                                                         assessmentSection.GrassCoverErosionInwards,
                                                                                                                         assessmentSection);
            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                Action call = () => activity.Run();

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(6, msgs.Length);
                    Assert.AreEqual($"Uitvoeren van berekening '{calculation.Name}' is gestart.", msgs[0]);
                    CalculationServiceTestHelper.AssertValidationStartMessage(msgs[1]);
                    CalculationServiceTestHelper.AssertValidationEndMessage(msgs[2]);
                    CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[3]);
                    GrassCoverErosionInwardsCalculationServiceTestHelper.AssertCalculationFinishedMessage(GrassCoverErosionInwardsCalculationServiceTestHelper.OvertoppingCalculationDescription, overtoppingCalculator.OutputDirectory, msgs[4]);
                    CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[5]);
                });
                Assert.AreEqual(ActivityState.Executed, activity.State);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        [TestCaseSource(typeof(HydraRingCalculatorTestCaseProvider), nameof(HydraRingCalculatorTestCaseProvider.GetCalculatorFailingConditions), new object[]
        {
            nameof(Finish_InvalidOvertoppingCalculation_DoesNotSetOutputAndNotifyObservers)
        })]
        public void Finish_InvalidOvertoppingCalculation_DoesNotSetOutputAndNotifyObservers(bool endInFailure,
                                                                                            string lastErrorFileContent)
        {
            // Setup
            var mockRepository = new MockRepository();
            var observer = mockRepository.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());

            var calculator = new TestOvertoppingCalculator
            {
                EndInFailure = endInFailure,
                LastErrorFileContent = lastErrorFileContent
            };
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(null))
                             .IgnoreArguments()
                             .Return(calculator);
            mockRepository.ReplayAll();

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            ImportHydraulicBoundaryDatabase(assessmentSection);

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryData.GetLocations().First(),
                    DikeProfile = CreateDikeProfile()
                }
            };

            calculation.Attach(observer);

            CalculatableActivity activity = GrassCoverErosionInwardsCalculationActivityFactory.CreateCalculationActivity(calculation,
                                                                                                                         assessmentSection.GrassCoverErosionInwards,
                                                                                                                         assessmentSection);
            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                activity.Run();
            }

            // Call
            activity.Finish();

            // Assert
            Assert.IsNull(calculation.Output);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Finish_ValidOvertoppingCalculation_SetsOutputAndNotifyObserversOfCalculation()
        {
            // Setup
            var mockRepository = new MockRepository();
            var observer = mockRepository.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());

            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(null))
                             .IgnoreArguments()
                             .Return(new TestOvertoppingCalculator());
            mockRepository.ReplayAll();

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            ImportHydraulicBoundaryDatabase(assessmentSection);

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryData.Locations.First(hl => hl.Id == 1300001),
                    DikeProfile = CreateDikeProfile()
                }
            };

            calculation.Attach(observer);

            CalculatableActivity activity = GrassCoverErosionInwardsCalculationActivityFactory.CreateCalculationActivity(calculation,
                                                                                                                         assessmentSection.GrassCoverErosionInwards,
                                                                                                                         assessmentSection);
            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                activity.Run();
            }

            // Call
            activity.Finish();

            // Assert
            Assert.IsNotNull(calculation.Output);
            Assert.IsFalse(double.IsNaN(calculation.Output.OvertoppingOutput.Reliability));
            Assert.IsNull(calculation.Output.DikeHeightOutput);
            Assert.IsNull(calculation.Output.OvertoppingRateOutput);
            mockRepository.VerifyAll();
        }

        #endregion

        #region Dike height calculations

        [Test]
        [TestCase(BreakWaterType.Caisson)]
        [TestCase(BreakWaterType.Dam)]
        [TestCase(BreakWaterType.Wall)]
        public void Run_ValidDikeHeightCalculation_InputPropertiesCorrectlySentToService(BreakWaterType breakWaterType)
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            ImportHydraulicBoundaryDatabase(assessmentSection);

            var dikeHeightCalculator = new TestHydraulicLoadsCalculator();

            HydraulicBoundaryLocation hydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryData.Locations.First(hl => hl.Id == 1300001);

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(Arg<HydraRingCalculationSettings>.Is.NotNull))
                             .WhenCalled(invocation =>
                             {
                                 HydraRingCalculationSettingsTestHelper.AssertHydraRingCalculationSettings(
                                     HydraulicBoundaryCalculationSettingsFactory.CreateSettings(
                                         assessmentSection.HydraulicBoundaryData,
                                         hydraulicBoundaryLocation),
                                     (HydraRingCalculationSettings) invocation.Arguments[0]);
                             })
                             .Return(new TestOvertoppingCalculator());
            calculatorFactory.Expect(cf => cf.CreateDikeHeightCalculator(Arg<HydraRingCalculationSettings>.Is.NotNull))
                             .WhenCalled(invocation =>
                             {
                                 HydraRingCalculationSettingsTestHelper.AssertHydraRingCalculationSettings(
                                     HydraulicBoundaryCalculationSettingsFactory.CreateSettings(
                                         assessmentSection.HydraulicBoundaryData,
                                         hydraulicBoundaryLocation),
                                     (HydraRingCalculationSettings) invocation.Arguments[0]);
                             })
                             .Return(dikeHeightCalculator);
            mockRepository.ReplayAll();

            DikeProfile dikeProfile = CreateDikeProfile();
            dikeProfile.BreakWater.Type = breakWaterType;

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                    DikeProfile = dikeProfile,
                    ShouldDikeHeightBeCalculated = true
                }
            };

            CalculatableActivity activity = GrassCoverErosionInwardsCalculationActivityFactory.CreateCalculationActivity(calculation,
                                                                                                                         assessmentSection.GrassCoverErosionInwards,
                                                                                                                         assessmentSection);
            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                activity.Run();

                // Assert
                HydraulicLoadsCalculationInput[] hydraulicLoadsCalculationInputs = dikeHeightCalculator.ReceivedInputs.ToArray();
                Assert.AreEqual(1, hydraulicLoadsCalculationInputs.Length);

                HydraulicLoadsCalculationInput actualInput = hydraulicLoadsCalculationInputs[0];
                GeneralGrassCoverErosionInwardsInput generalInput = assessmentSection.GrassCoverErosionInwards.GeneralInput;

                GrassCoverErosionInwardsInput input = calculation.InputParameters;

                var expectedInput = new DikeHeightCalculationInput(input.HydraulicBoundaryLocation.Id,
                                                                   input.DikeHeightTargetProbability,
                                                                   input.Orientation,
                                                                   input.DikeGeometry.Select(roughnessPoint => new HydraRingRoughnessProfilePoint(roughnessPoint.Point.X, roughnessPoint.Point.Y, roughnessPoint.Roughness)),
                                                                   input.ForeshoreGeometry.Select(c => new HydraRingForelandPoint(c.X, c.Y)),
                                                                   new HydraRingBreakWater(BreakWaterTypeHelper.GetHydraRingBreakWaterType(breakWaterType), input.BreakWater.Height),
                                                                   generalInput.CriticalOvertoppingModelFactor,
                                                                   generalInput.FbFactor.Mean,
                                                                   generalInput.FbFactor.StandardDeviation,
                                                                   generalInput.FbFactor.LowerBoundary,
                                                                   generalInput.FbFactor.UpperBoundary,
                                                                   generalInput.FnFactor.Mean,
                                                                   generalInput.FnFactor.StandardDeviation,
                                                                   generalInput.FnFactor.LowerBoundary,
                                                                   generalInput.FnFactor.UpperBoundary,
                                                                   generalInput.OvertoppingModelFactor,
                                                                   input.CriticalFlowRate.Mean,
                                                                   input.CriticalFlowRate.StandardDeviation,
                                                                   generalInput.FrunupModelFactor.Mean,
                                                                   generalInput.FrunupModelFactor.StandardDeviation,
                                                                   generalInput.FrunupModelFactor.LowerBoundary,
                                                                   generalInput.FrunupModelFactor.UpperBoundary,
                                                                   generalInput.FshallowModelFactor.Mean,
                                                                   generalInput.FshallowModelFactor.StandardDeviation,
                                                                   generalInput.FshallowModelFactor.LowerBoundary,
                                                                   generalInput.FshallowModelFactor.UpperBoundary);

                HydraRingDataEqualityHelper.AreEqual(expectedInput, actualInput);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        public void Run_InvalidDikeHeightCalculationWithExceptionAndLastErrorPresent_LogError()
        {
            // Setup
            var dikeHeightCalculator = new TestHydraulicLoadsCalculator
            {
                LastErrorFileContent = "An error occurred",
                EndInFailure = true
            };

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            var overtoppingCalculator = new TestOvertoppingCalculator();
            calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(null))
                             .IgnoreArguments()
                             .Return(overtoppingCalculator);
            calculatorFactory.Expect(cf => cf.CreateDikeHeightCalculator(null))
                             .IgnoreArguments()
                             .Return(dikeHeightCalculator);
            mockRepository.ReplayAll();

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            ImportHydraulicBoundaryDatabase(assessmentSection);

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryData.Locations.First(hl => hl.Id == 1300001),
                    DikeProfile = CreateDikeProfile(),
                    ShouldDikeHeightBeCalculated = true
                }
            };

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                CalculatableActivity activity = GrassCoverErosionInwardsCalculationActivityFactory.CreateCalculationActivity(calculation,
                                                                                                                             assessmentSection.GrassCoverErosionInwards,
                                                                                                                             assessmentSection);
                // Call
                Action call = () => activity.Run();

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(8, msgs.Length);
                    Assert.AreEqual($"Uitvoeren van berekening '{calculation.Name}' is gestart.", msgs[0]);
                    CalculationServiceTestHelper.AssertValidationStartMessage(msgs[1]);
                    CalculationServiceTestHelper.AssertValidationEndMessage(msgs[2]);
                    CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[3]);
                    GrassCoverErosionInwardsCalculationServiceTestHelper.AssertCalculationFinishedMessage(GrassCoverErosionInwardsCalculationServiceTestHelper.OvertoppingCalculationDescription, overtoppingCalculator.OutputDirectory, msgs[4]);
                    GrassCoverErosionInwardsCalculationServiceTestHelper.AssertCalculationFailedMessage(GrassCoverErosionInwardsCalculationServiceTestHelper.HbnCalculationDescription, calculation.Name, dikeHeightCalculator.LastErrorFileContent, msgs[5]);
                    GrassCoverErosionInwardsCalculationServiceTestHelper.AssertCalculationFinishedMessage(GrassCoverErosionInwardsCalculationServiceTestHelper.HbnCalculationDescription, dikeHeightCalculator.OutputDirectory, msgs[6]);
                    CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[7]);
                });
                Assert.AreEqual(ActivityState.Executed, activity.State);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        public void Run_InvalidDikeHeightCalculationWithExceptionAndNoLastErrorPresent_LogError()
        {
            // Setup
            var dikeHeightCalculator = new TestHydraulicLoadsCalculator
            {
                EndInFailure = true
            };
            var overtoppingCalculator = new TestOvertoppingCalculator();

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(null))
                             .IgnoreArguments()
                             .Return(overtoppingCalculator);
            calculatorFactory.Expect(cf => cf.CreateDikeHeightCalculator(null))
                             .IgnoreArguments()
                             .Return(dikeHeightCalculator);
            mockRepository.ReplayAll();

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            ImportHydraulicBoundaryDatabase(assessmentSection);

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryData.Locations.First(hl => hl.Id == 1300001),
                    DikeProfile = CreateDikeProfile(),
                    ShouldDikeHeightBeCalculated = true
                }
            };

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                CalculatableActivity activity = GrassCoverErosionInwardsCalculationActivityFactory.CreateCalculationActivity(calculation,
                                                                                                                             assessmentSection.GrassCoverErosionInwards,
                                                                                                                             assessmentSection);
                // Call
                Action call = () => activity.Run();

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(8, msgs.Length);
                    Assert.AreEqual($"Uitvoeren van berekening '{calculation.Name}' is gestart.", msgs[0]);
                    CalculationServiceTestHelper.AssertValidationStartMessage(msgs[1]);
                    CalculationServiceTestHelper.AssertValidationEndMessage(msgs[2]);
                    CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[3]);
                    GrassCoverErosionInwardsCalculationServiceTestHelper.AssertCalculationFinishedMessage(GrassCoverErosionInwardsCalculationServiceTestHelper.OvertoppingCalculationDescription, overtoppingCalculator.OutputDirectory, msgs[4]);
                    GrassCoverErosionInwardsCalculationServiceTestHelper.AssertCalculationFailedMessage(GrassCoverErosionInwardsCalculationServiceTestHelper.HbnCalculationDescription, calculation.Name, dikeHeightCalculator.LastErrorFileContent, msgs[5]);
                    GrassCoverErosionInwardsCalculationServiceTestHelper.AssertCalculationFinishedMessage(GrassCoverErosionInwardsCalculationServiceTestHelper.HbnCalculationDescription, dikeHeightCalculator.OutputDirectory, msgs[6]);
                    CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[7]);
                });
                Assert.AreEqual(ActivityState.Executed, activity.State);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        public void Run_InvalidDikeHeightCalculationWithoutExceptionAndWithLastErrorPresent_LogError()
        {
            // Setup
            var dikeHeightCalculator = new TestHydraulicLoadsCalculator
            {
                EndInFailure = false,
                LastErrorFileContent = "An error occurred"
            };
            var overtoppingCalculator = new TestOvertoppingCalculator();

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(null))
                             .IgnoreArguments()
                             .Return(overtoppingCalculator);
            calculatorFactory.Expect(cf => cf.CreateDikeHeightCalculator(null))
                             .IgnoreArguments()
                             .Return(dikeHeightCalculator);
            mockRepository.ReplayAll();

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            ImportHydraulicBoundaryDatabase(assessmentSection);

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryData.Locations.First(hl => hl.Id == 1300001),
                    DikeProfile = CreateDikeProfile(),
                    ShouldDikeHeightBeCalculated = true
                }
            };

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                CalculatableActivity activity = GrassCoverErosionInwardsCalculationActivityFactory.CreateCalculationActivity(calculation,
                                                                                                                             assessmentSection.GrassCoverErosionInwards,
                                                                                                                             assessmentSection);
                // Call
                Action call = () => activity.Run();

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(8, msgs.Length);
                    Assert.AreEqual($"Uitvoeren van berekening '{calculation.Name}' is gestart.", msgs[0]);
                    CalculationServiceTestHelper.AssertValidationStartMessage(msgs[1]);
                    CalculationServiceTestHelper.AssertValidationEndMessage(msgs[2]);
                    CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[3]);
                    GrassCoverErosionInwardsCalculationServiceTestHelper.AssertCalculationFinishedMessage(GrassCoverErosionInwardsCalculationServiceTestHelper.OvertoppingCalculationDescription, overtoppingCalculator.OutputDirectory, msgs[4]);
                    GrassCoverErosionInwardsCalculationServiceTestHelper.AssertCalculationFailedMessage(GrassCoverErosionInwardsCalculationServiceTestHelper.HbnCalculationDescription, calculation.Name, dikeHeightCalculator.LastErrorFileContent, msgs[5]);
                    GrassCoverErosionInwardsCalculationServiceTestHelper.AssertCalculationFinishedMessage(GrassCoverErosionInwardsCalculationServiceTestHelper.HbnCalculationDescription, dikeHeightCalculator.OutputDirectory, msgs[6]);
                    CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[7]);
                });
                Assert.AreEqual(ActivityState.Executed, activity.State);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        public void Run_ValidDikeHeightCalculation_PerformValidationAndCalculationAndLogStartAndEndError()
        {
            // Setup
            var dikeHeightCalculator = new TestHydraulicLoadsCalculator
            {
                Value = 2,
                ReliabilityIndex = -1
            };
            var overtoppingCalculator = new TestOvertoppingCalculator();

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(null))
                             .IgnoreArguments()
                             .Return(overtoppingCalculator);
            calculatorFactory.Expect(cf => cf.CreateDikeHeightCalculator(null))
                             .IgnoreArguments()
                             .Return(dikeHeightCalculator);
            mockRepository.ReplayAll();

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            ImportHydraulicBoundaryDatabase(assessmentSection);

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryData.Locations.First(hl => hl.Id == 1300001),
                    DikeProfile = CreateDikeProfile(),
                    ShouldDikeHeightBeCalculated = true
                }
            };

            CalculatableActivity activity = GrassCoverErosionInwardsCalculationActivityFactory.CreateCalculationActivity(calculation,
                                                                                                                         assessmentSection.GrassCoverErosionInwards,
                                                                                                                         assessmentSection);
            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                Action call = () => activity.Run();

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(8, msgs.Length);
                    Assert.AreEqual($"Uitvoeren van berekening '{calculation.Name}' is gestart.", msgs[0]);
                    CalculationServiceTestHelper.AssertValidationStartMessage(msgs[1]);
                    CalculationServiceTestHelper.AssertValidationEndMessage(msgs[2]);
                    CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[3]);
                    GrassCoverErosionInwardsCalculationServiceTestHelper.AssertCalculationFinishedMessage(GrassCoverErosionInwardsCalculationServiceTestHelper.OvertoppingCalculationDescription, overtoppingCalculator.OutputDirectory, msgs[4]);
                    GrassCoverErosionInwardsCalculationServiceTestHelper.AssertCalculationFinishedMessage(GrassCoverErosionInwardsCalculationServiceTestHelper.HbnCalculationDescription, dikeHeightCalculator.OutputDirectory, msgs[5]);
                    Assert.AreEqual($"De HBN berekening voor grasbekleding erosie kruin en binnentalud '{calculation.Name}' is niet geconvergeerd.", msgs[6]);
                    CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[7]);
                });
                Assert.AreEqual(ActivityState.Executed, activity.State);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        public void Finish_InvalidDikeHeightCalculation_OutputSetAndObserversNotified()
        {
            // Setup
            var mockRepository = new MockRepository();
            var observer = mockRepository.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());

            var dikeHeightCalculator = new TestHydraulicLoadsCalculator
            {
                Value = double.NaN,
                EndInFailure = true
            };
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(null))
                             .IgnoreArguments()
                             .Return(new TestOvertoppingCalculator());
            calculatorFactory.Expect(cf => cf.CreateDikeHeightCalculator(null))
                             .IgnoreArguments()
                             .Return(dikeHeightCalculator);
            mockRepository.ReplayAll();

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            ImportHydraulicBoundaryDatabase(assessmentSection);

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryData.Locations.First(hl => hl.Id == 1300001),
                    DikeProfile = CreateDikeProfile(),
                    ShouldDikeHeightBeCalculated = true
                }
            };

            calculation.Attach(observer);

            CalculatableActivity activity = GrassCoverErosionInwardsCalculationActivityFactory.CreateCalculationActivity(calculation,
                                                                                                                         assessmentSection.GrassCoverErosionInwards,
                                                                                                                         assessmentSection);
            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                activity.Run();
            }

            // Call
            activity.Finish();

            // Assert
            Assert.IsNotNull(calculation.Output);
            Assert.IsNull(calculation.Output.DikeHeightOutput);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Finish_ValidDikeHeightCalculation_OutputSetAndObserversNotified()
        {
            // Setup
            var mockRepository = new MockRepository();
            var observer = mockRepository.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());

            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(null))
                             .IgnoreArguments()
                             .Return(new TestOvertoppingCalculator());
            calculatorFactory.Expect(cf => cf.CreateDikeHeightCalculator(null))
                             .IgnoreArguments()
                             .Return(new TestHydraulicLoadsCalculator());
            mockRepository.ReplayAll();

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            ImportHydraulicBoundaryDatabase(assessmentSection);

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryData.Locations.First(hl => hl.Id == 1300001),
                    DikeProfile = CreateDikeProfile(),
                    ShouldDikeHeightBeCalculated = true
                }
            };

            calculation.Attach(observer);

            CalculatableActivity activity = GrassCoverErosionInwardsCalculationActivityFactory.CreateCalculationActivity(calculation,
                                                                                                                         assessmentSection.GrassCoverErosionInwards,
                                                                                                                         assessmentSection);
            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                activity.Run();
            }

            // Call
            activity.Finish();

            // Assert
            Assert.IsNotNull(calculation.Output);
            Assert.IsFalse(double.IsNaN(calculation.Output.OvertoppingOutput.Reliability));
            DikeHeightOutput dikeHeightOutput = calculation.Output.DikeHeightOutput;
            Assert.IsNotNull(dikeHeightOutput);
            Assert.IsFalse(double.IsNaN(dikeHeightOutput.DikeHeight));
            mockRepository.VerifyAll();
        }

        #endregion

        #region Overtopping rate calculations

        [Test]
        [TestCase(BreakWaterType.Caisson)]
        [TestCase(BreakWaterType.Dam)]
        [TestCase(BreakWaterType.Wall)]
        public void Run_ValidOvertoppingRateCalculation_InputPropertiesCorrectlySentToService(BreakWaterType breakWaterType)
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            ImportHydraulicBoundaryDatabase(assessmentSection);

            var overtoppingRateCalculator = new TestHydraulicLoadsCalculator();

            HydraulicBoundaryLocation hydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryData.Locations.First(hl => hl.Id == 1300001);

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(Arg<HydraRingCalculationSettings>.Is.NotNull))
                             .WhenCalled(invocation =>
                             {
                                 HydraRingCalculationSettingsTestHelper.AssertHydraRingCalculationSettings(
                                     HydraulicBoundaryCalculationSettingsFactory.CreateSettings(
                                         assessmentSection.HydraulicBoundaryData,
                                         hydraulicBoundaryLocation),
                                     (HydraRingCalculationSettings) invocation.Arguments[0]);
                             })
                             .Return(new TestOvertoppingCalculator());
            calculatorFactory.Expect(cf => cf.CreateOvertoppingRateCalculator(Arg<HydraRingCalculationSettings>.Is.NotNull))
                             .WhenCalled(invocation =>
                             {
                                 HydraRingCalculationSettingsTestHelper.AssertHydraRingCalculationSettings(
                                     HydraulicBoundaryCalculationSettingsFactory.CreateSettings(
                                         assessmentSection.HydraulicBoundaryData,
                                         hydraulicBoundaryLocation),
                                     (HydraRingCalculationSettings) invocation.Arguments[0]);
                             })
                             .Return(overtoppingRateCalculator);
            mockRepository.ReplayAll();

            DikeProfile dikeProfile = CreateDikeProfile();
            dikeProfile.BreakWater.Type = breakWaterType;

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                    DikeProfile = dikeProfile,
                    ShouldOvertoppingRateBeCalculated = true
                }
            };

            CalculatableActivity activity = GrassCoverErosionInwardsCalculationActivityFactory.CreateCalculationActivity(calculation,
                                                                                                                         assessmentSection.GrassCoverErosionInwards,
                                                                                                                         assessmentSection);
            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                activity.Run();

                // Assert
                HydraulicLoadsCalculationInput[] hydraulicLoadsCalculationInputs = overtoppingRateCalculator.ReceivedInputs.ToArray();
                Assert.AreEqual(1, hydraulicLoadsCalculationInputs.Length);

                HydraulicLoadsCalculationInput actualInput = hydraulicLoadsCalculationInputs[0];
                GeneralGrassCoverErosionInwardsInput generalInput = assessmentSection.GrassCoverErosionInwards.GeneralInput;

                GrassCoverErosionInwardsInput input = calculation.InputParameters;

                var expectedInput = new OvertoppingRateCalculationInput(input.HydraulicBoundaryLocation.Id,
                                                                        input.OvertoppingRateTargetProbability,
                                                                        input.Orientation,
                                                                        input.DikeGeometry.Select(roughnessPoint => new HydraRingRoughnessProfilePoint(roughnessPoint.Point.X, roughnessPoint.Point.Y, roughnessPoint.Roughness)),
                                                                        input.ForeshoreGeometry.Select(c => new HydraRingForelandPoint(c.X, c.Y)),
                                                                        new HydraRingBreakWater(BreakWaterTypeHelper.GetHydraRingBreakWaterType(breakWaterType), input.BreakWater.Height),
                                                                        input.DikeHeight,
                                                                        generalInput.CriticalOvertoppingModelFactor,
                                                                        generalInput.FbFactor.Mean,
                                                                        generalInput.FbFactor.StandardDeviation,
                                                                        generalInput.FbFactor.LowerBoundary,
                                                                        generalInput.FbFactor.UpperBoundary,
                                                                        generalInput.FnFactor.Mean,
                                                                        generalInput.FnFactor.StandardDeviation,
                                                                        generalInput.FnFactor.LowerBoundary,
                                                                        generalInput.FnFactor.UpperBoundary,
                                                                        generalInput.OvertoppingModelFactor,
                                                                        generalInput.FrunupModelFactor.Mean,
                                                                        generalInput.FrunupModelFactor.StandardDeviation,
                                                                        generalInput.FrunupModelFactor.LowerBoundary,
                                                                        generalInput.FrunupModelFactor.UpperBoundary,
                                                                        generalInput.FshallowModelFactor.Mean,
                                                                        generalInput.FshallowModelFactor.StandardDeviation,
                                                                        generalInput.FshallowModelFactor.LowerBoundary,
                                                                        generalInput.FshallowModelFactor.UpperBoundary);

                HydraRingDataEqualityHelper.AreEqual(expectedInput, actualInput);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        public void Run_InvalidOvertoppingRateCalculationWithExceptionAndLastErrorPresent_LogError()
        {
            // Setup
            var overtoppingRateCalculator = new TestHydraulicLoadsCalculator
            {
                LastErrorFileContent = "An error occurred",
                EndInFailure = true
            };
            var overtoppingCalculator = new TestOvertoppingCalculator();

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(null))
                             .IgnoreArguments()
                             .Return(overtoppingCalculator);
            calculatorFactory.Expect(cf => cf.CreateOvertoppingRateCalculator(null))
                             .IgnoreArguments()
                             .Return(overtoppingRateCalculator);
            mockRepository.ReplayAll();

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            ImportHydraulicBoundaryDatabase(assessmentSection);

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryData.Locations.First(hl => hl.Id == 1300001),
                    DikeProfile = CreateDikeProfile(),
                    ShouldOvertoppingRateBeCalculated = true
                }
            };

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                CalculatableActivity activity = GrassCoverErosionInwardsCalculationActivityFactory.CreateCalculationActivity(calculation,
                                                                                                                             assessmentSection.GrassCoverErosionInwards,
                                                                                                                             assessmentSection);
                // Call
                Action call = () => activity.Run();

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(8, msgs.Length);
                    Assert.AreEqual($"Uitvoeren van berekening '{calculation.Name}' is gestart.", msgs[0]);
                    CalculationServiceTestHelper.AssertValidationStartMessage(msgs[1]);
                    CalculationServiceTestHelper.AssertValidationEndMessage(msgs[2]);
                    CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[3]);
                    GrassCoverErosionInwardsCalculationServiceTestHelper.AssertCalculationFinishedMessage(GrassCoverErosionInwardsCalculationServiceTestHelper.OvertoppingCalculationDescription, overtoppingCalculator.OutputDirectory, msgs[4]);
                    GrassCoverErosionInwardsCalculationServiceTestHelper.AssertCalculationFailedMessage(GrassCoverErosionInwardsCalculationServiceTestHelper.OvertoppingRateCalculationDescription, calculation.Name, overtoppingRateCalculator.LastErrorFileContent, msgs[5]);
                    GrassCoverErosionInwardsCalculationServiceTestHelper.AssertCalculationFinishedMessage(GrassCoverErosionInwardsCalculationServiceTestHelper.OvertoppingRateCalculationDescription, overtoppingRateCalculator.OutputDirectory, msgs[6]);
                    CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[7]);
                });
                Assert.AreEqual(ActivityState.Executed, activity.State);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        public void Run_InvalidOvertoppingRateCalculationWithExceptionAndNoLastErrorPresent_LogError()
        {
            // Setup
            var overtoppingRateCalculator = new TestHydraulicLoadsCalculator
            {
                EndInFailure = true
            };
            var overtoppingCalculator = new TestOvertoppingCalculator();

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(null))
                             .IgnoreArguments()
                             .Return(overtoppingCalculator);
            calculatorFactory.Expect(cf => cf.CreateOvertoppingRateCalculator(null))
                             .IgnoreArguments()
                             .Return(overtoppingRateCalculator);
            mockRepository.ReplayAll();

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            ImportHydraulicBoundaryDatabase(assessmentSection);

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryData.Locations.First(hl => hl.Id == 1300001),
                    DikeProfile = CreateDikeProfile(),
                    ShouldOvertoppingRateBeCalculated = true
                }
            };

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                CalculatableActivity activity = GrassCoverErosionInwardsCalculationActivityFactory.CreateCalculationActivity(calculation,
                                                                                                                             assessmentSection.GrassCoverErosionInwards,
                                                                                                                             assessmentSection);
                // Call
                Action call = () => activity.Run();

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(8, msgs.Length);
                    Assert.AreEqual($"Uitvoeren van berekening '{calculation.Name}' is gestart.", msgs[0]);
                    CalculationServiceTestHelper.AssertValidationStartMessage(msgs[1]);
                    CalculationServiceTestHelper.AssertValidationEndMessage(msgs[2]);
                    CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[3]);
                    GrassCoverErosionInwardsCalculationServiceTestHelper.AssertCalculationFinishedMessage(GrassCoverErosionInwardsCalculationServiceTestHelper.OvertoppingCalculationDescription, overtoppingCalculator.OutputDirectory, msgs[4]);
                    GrassCoverErosionInwardsCalculationServiceTestHelper.AssertCalculationFailedMessage(GrassCoverErosionInwardsCalculationServiceTestHelper.OvertoppingRateCalculationDescription, calculation.Name, overtoppingRateCalculator.LastErrorFileContent, msgs[5]);
                    GrassCoverErosionInwardsCalculationServiceTestHelper.AssertCalculationFinishedMessage(GrassCoverErosionInwardsCalculationServiceTestHelper.OvertoppingRateCalculationDescription, overtoppingRateCalculator.OutputDirectory, msgs[6]);
                    CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[7]);
                });
                Assert.AreEqual(ActivityState.Executed, activity.State);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        public void Run_InvalidOvertoppingRateCalculationWithoutExceptionAndWithLastErrorPresent_LogError()
        {
            // Setup
            var overtoppingRateCalculator = new TestHydraulicLoadsCalculator
            {
                EndInFailure = false,
                LastErrorFileContent = "An error occurred"
            };
            var overtoppingCalculator = new TestOvertoppingCalculator();

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(null))
                             .IgnoreArguments()
                             .Return(overtoppingCalculator);
            calculatorFactory.Expect(cf => cf.CreateOvertoppingRateCalculator(null))
                             .IgnoreArguments()
                             .Return(overtoppingRateCalculator);
            mockRepository.ReplayAll();

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            ImportHydraulicBoundaryDatabase(assessmentSection);

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryData.Locations.First(hl => hl.Id == 1300001),
                    DikeProfile = CreateDikeProfile(),
                    ShouldOvertoppingRateBeCalculated = true
                }
            };

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                CalculatableActivity activity = GrassCoverErosionInwardsCalculationActivityFactory.CreateCalculationActivity(calculation,
                                                                                                                             assessmentSection.GrassCoverErosionInwards,
                                                                                                                             assessmentSection);
                // Call
                Action call = () => activity.Run();

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(8, msgs.Length);
                    Assert.AreEqual($"Uitvoeren van berekening '{calculation.Name}' is gestart.", msgs[0]);
                    CalculationServiceTestHelper.AssertValidationStartMessage(msgs[1]);
                    CalculationServiceTestHelper.AssertValidationEndMessage(msgs[2]);
                    CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[3]);
                    GrassCoverErosionInwardsCalculationServiceTestHelper.AssertCalculationFinishedMessage(GrassCoverErosionInwardsCalculationServiceTestHelper.OvertoppingCalculationDescription, overtoppingCalculator.OutputDirectory, msgs[4]);
                    GrassCoverErosionInwardsCalculationServiceTestHelper.AssertCalculationFailedMessage(GrassCoverErosionInwardsCalculationServiceTestHelper.OvertoppingRateCalculationDescription, calculation.Name, overtoppingRateCalculator.LastErrorFileContent, msgs[5]);
                    GrassCoverErosionInwardsCalculationServiceTestHelper.AssertCalculationFinishedMessage(GrassCoverErosionInwardsCalculationServiceTestHelper.OvertoppingRateCalculationDescription, overtoppingRateCalculator.OutputDirectory, msgs[6]);
                    CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[7]);
                });
                Assert.AreEqual(ActivityState.Executed, activity.State);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        public void Run_ValidOvertoppingRateCalculation_PerformValidationAndCalculationAndLogStartAndEndError()
        {
            // Setup
            var overtoppingRateCalculator = new TestHydraulicLoadsCalculator
            {
                Value = 2,
                ReliabilityIndex = -1
            };
            var overtoppingCalculator = new TestOvertoppingCalculator();

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(null))
                             .IgnoreArguments()
                             .Return(overtoppingCalculator);
            calculatorFactory.Expect(cf => cf.CreateOvertoppingRateCalculator(null))
                             .IgnoreArguments()
                             .Return(overtoppingRateCalculator);
            mockRepository.ReplayAll();

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            ImportHydraulicBoundaryDatabase(assessmentSection);

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryData.Locations.First(hl => hl.Id == 1300001),
                    DikeProfile = CreateDikeProfile(),
                    ShouldOvertoppingRateBeCalculated = true
                }
            };

            CalculatableActivity activity = GrassCoverErosionInwardsCalculationActivityFactory.CreateCalculationActivity(calculation,
                                                                                                                         assessmentSection.GrassCoverErosionInwards,
                                                                                                                         assessmentSection);
            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                Action call = () => activity.Run();

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(8, msgs.Length);
                    Assert.AreEqual($"Uitvoeren van berekening '{calculation.Name}' is gestart.", msgs[0]);
                    CalculationServiceTestHelper.AssertValidationStartMessage(msgs[1]);
                    CalculationServiceTestHelper.AssertValidationEndMessage(msgs[2]);
                    CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[3]);
                    GrassCoverErosionInwardsCalculationServiceTestHelper.AssertCalculationFinishedMessage(GrassCoverErosionInwardsCalculationServiceTestHelper.OvertoppingCalculationDescription, overtoppingCalculator.OutputDirectory, msgs[4]);
                    GrassCoverErosionInwardsCalculationServiceTestHelper.AssertCalculationFinishedMessage(GrassCoverErosionInwardsCalculationServiceTestHelper.OvertoppingRateCalculationDescription, overtoppingRateCalculator.OutputDirectory, msgs[5]);
                    Assert.AreEqual($"De overslagdebiet berekening voor grasbekleding erosie kruin en binnentalud '{calculation.Name}' is niet geconvergeerd.", msgs[6]);
                    CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[7]);
                });
                Assert.AreEqual(ActivityState.Executed, activity.State);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        public void Finish_InvalidOvertoppingRateCalculation_OutputSetAndObserversNotified()
        {
            // Setup
            var mockRepository = new MockRepository();
            var observer = mockRepository.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());

            var overtoppingRateCalculator = new TestHydraulicLoadsCalculator
            {
                Value = double.NaN,
                EndInFailure = true
            };
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(null))
                             .IgnoreArguments()
                             .Return(new TestOvertoppingCalculator());
            calculatorFactory.Expect(cf => cf.CreateOvertoppingRateCalculator(null))
                             .IgnoreArguments()
                             .Return(overtoppingRateCalculator);
            mockRepository.ReplayAll();

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            ImportHydraulicBoundaryDatabase(assessmentSection);

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryData.Locations.First(hl => hl.Id == 1300001),
                    DikeProfile = CreateDikeProfile(),
                    ShouldOvertoppingRateBeCalculated = true
                }
            };

            calculation.Attach(observer);

            CalculatableActivity activity = GrassCoverErosionInwardsCalculationActivityFactory.CreateCalculationActivity(calculation,
                                                                                                                         assessmentSection.GrassCoverErosionInwards,
                                                                                                                         assessmentSection);
            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                activity.Run();
            }

            // Call
            activity.Finish();

            // Assert
            Assert.IsNotNull(calculation.Output);
            Assert.IsNull(calculation.Output.OvertoppingRateOutput);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Finish_ValidOvertoppingRateCalculation_OutputSetAndObserversNotified()
        {
            // Setup
            var mockRepository = new MockRepository();
            var observer = mockRepository.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());

            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(null))
                             .IgnoreArguments()
                             .Return(new TestOvertoppingCalculator());
            calculatorFactory.Expect(cf => cf.CreateOvertoppingRateCalculator(null))
                             .IgnoreArguments()
                             .Return(new TestHydraulicLoadsCalculator());
            mockRepository.ReplayAll();

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            ImportHydraulicBoundaryDatabase(assessmentSection);

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryData.Locations.First(hl => hl.Id == 1300001),
                    DikeProfile = CreateDikeProfile(),
                    ShouldOvertoppingRateBeCalculated = true
                }
            };

            calculation.Attach(observer);

            CalculatableActivity activity = GrassCoverErosionInwardsCalculationActivityFactory.CreateCalculationActivity(calculation,
                                                                                                                         assessmentSection.GrassCoverErosionInwards,
                                                                                                                         assessmentSection);
            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                activity.Run();
            }

            // Call
            activity.Finish();

            // Assert
            Assert.IsNotNull(calculation.Output);
            Assert.IsFalse(double.IsNaN(calculation.Output.OvertoppingOutput.Reliability));
            OvertoppingRateOutput overtoppingRateOutput = calculation.Output.OvertoppingRateOutput;
            Assert.IsNotNull(overtoppingRateOutput);
            Assert.IsFalse(double.IsNaN(overtoppingRateOutput.OvertoppingRate));
            mockRepository.VerifyAll();
        }

        #endregion
    }
}