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
using System.IO;
using System.Linq;
using Core.Common.Base;
using Core.Common.Base.Geometry;
using Core.Common.Base.Service;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.IO.FileImporters;
using Ringtoets.Common.Service;
using Ringtoets.Common.Service.TestUtil;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionInwards.Service;
using Ringtoets.GrassCoverErosionInwards.Service.TestUtil;
using Ringtoets.HydraRing.Calculation.Calculator.Factory;
using Ringtoets.HydraRing.Calculation.Data;
using Ringtoets.HydraRing.Calculation.Data.Input;
using Ringtoets.HydraRing.Calculation.Data.Input.Hydraulics;
using Ringtoets.HydraRing.Calculation.Data.Input.Overtopping;
using Ringtoets.HydraRing.Calculation.TestUtil;
using Ringtoets.HydraRing.Calculation.TestUtil.Calculator;
using Ringtoets.Integration.Data;

namespace Ringtoets.GrassCoverErosionInwards.Integration.Test
{
    [TestFixture]
    public class GrassCoverErosionInwardsCalculationActivityIntegrationTest
    {
        private static readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Integration.Service, "HydraRingCalculation");

        [Test]
        public void Run_InvalidHydraulicBoundaryDatabase_LogValidationStartAndEndWithError()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike)
            {
                HydraulicBoundaryDatabase =
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
        [TestCase(DikeHeightCalculationType.NoCalculation, OvertoppingRateCalculationType.NoCalculation,
            TestName = "Run_OvertoppingOnly_ProgressTextSetAccordingly")]
        [TestCase(DikeHeightCalculationType.NoCalculation, OvertoppingRateCalculationType.CalculateByAssessmentSectionNorm,
            TestName = "Run_OvertoppingRateNorm_ProgressTextSetAccordingly")]
        [TestCase(DikeHeightCalculationType.NoCalculation, OvertoppingRateCalculationType.CalculateByProfileSpecificRequiredProbability,
            TestName = "Run_OvertoppingRateRequiredProbability_ProgressTextSetAccordingly")]
        [TestCase(DikeHeightCalculationType.CalculateByAssessmentSectionNorm, OvertoppingRateCalculationType.NoCalculation,
            TestName = "Run_DikeHeightNorm_ProgressTextSetAccordingly")]
        [TestCase(DikeHeightCalculationType.CalculateByAssessmentSectionNorm, OvertoppingRateCalculationType.CalculateByAssessmentSectionNorm,
            TestName = "Run_DikeHeightNormAndOvertoppingRateNorm_ProgressTextSetAccordingly")]
        [TestCase(DikeHeightCalculationType.CalculateByAssessmentSectionNorm, OvertoppingRateCalculationType.CalculateByProfileSpecificRequiredProbability,
            TestName = "Run_DikeHeightNormAndOvertoppingRequiredProbability_ProgressTextSetAccordingly")]
        [TestCase(DikeHeightCalculationType.CalculateByProfileSpecificRequiredProbability, OvertoppingRateCalculationType.NoCalculation,
            TestName = "Run_DikeHeightRequiredProbability_ProgressTextSetAccordingly")]
        [TestCase(DikeHeightCalculationType.CalculateByProfileSpecificRequiredProbability, OvertoppingRateCalculationType.CalculateByAssessmentSectionNorm,
            TestName = "Run_DikeHeightRequiredProbabilityAndOvertoppingNorm_ProgressTextSetAccordingly")]
        [TestCase(DikeHeightCalculationType.CalculateByProfileSpecificRequiredProbability, OvertoppingRateCalculationType.CalculateByProfileSpecificRequiredProbability,
            TestName = "Run_DikeHeightRequiredProbabilityAndOvertoppingRequiredProbability_ProgressTextSetAccordingly")]
        public void Run_CombinationOfCalculations_ProgressTextSetAccordingly(DikeHeightCalculationType dikeHeightCalculationType, OvertoppingRateCalculationType overtoppingRateCalculationType)
        {
            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Stub(cf => cf.CreateOvertoppingCalculator(testDataPath, string.Empty)).Return(new TestOvertoppingCalculator());
            calculatorFactory.Stub(cf => cf.CreateOvertoppingRateCalculator(Arg<HydraRingCalculationSettings>.Is.NotNull))
                             .WhenCalled(invocation =>
                             {
                                 var settings = (HydraRingCalculationSettings) invocation.Arguments[0];
                                 Assert.AreEqual(testDataPath, settings.HlcdFilePath);
                                 Assert.IsEmpty(settings.PreprocessorDirectory);
                             })
                             .Return(new TestHydraulicLoadsCalculator());
            calculatorFactory.Stub(cf => cf.CreateDikeHeightCalculator(Arg<HydraRingCalculationSettings>.Is.NotNull))
                             .WhenCalled(invocation =>
                             {
                                 var settings = (HydraRingCalculationSettings)invocation.Arguments[0];
                                 Assert.AreEqual(testDataPath, settings.HlcdFilePath);
                                 Assert.IsEmpty(settings.PreprocessorDirectory);
                             })
                             .Return(new TestHydraulicLoadsCalculator());
            mockRepository.ReplayAll();

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            ImportHydraulicBoundaryDatabase(assessmentSection);

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001),
                    DikeProfile = CreateDikeProfile(),
                    DikeHeightCalculationType = dikeHeightCalculationType,
                    OvertoppingRateCalculationType = overtoppingRateCalculationType
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
            int totalNumberOfCalculations = dikeHeightCalculationType != DikeHeightCalculationType.NoCalculation
                                                ? overtoppingRateCalculationType != OvertoppingRateCalculationType.NoCalculation
                                                      ? 3
                                                      : 2
                                                : overtoppingRateCalculationType != OvertoppingRateCalculationType.NoCalculation
                                                    ? 2
                                                    : 1;

            string expectedProgressTexts = $"Stap 1 van {totalNumberOfCalculations} | Uitvoeren overloop en overslag berekening" + Environment.NewLine;

            if (dikeHeightCalculationType != DikeHeightCalculationType.NoCalculation)
            {
                expectedProgressTexts += $"Stap 2 van {totalNumberOfCalculations} | Uitvoeren HBN berekening" + Environment.NewLine;
            }

            if (overtoppingRateCalculationType != OvertoppingRateCalculationType.NoCalculation)
            {
                expectedProgressTexts += $"Stap {totalNumberOfCalculations} van {totalNumberOfCalculations} | Uitvoeren overslagdebiet berekening" + Environment.NewLine;
            }

            Assert.AreEqual(expectedProgressTexts, progressTexts);
            mockRepository.VerifyAll();
        }

        private static void ImportHydraulicBoundaryDatabase(AssessmentSection assessmentSection)
        {
            string validFilePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");

            using (var importer = new HydraulicBoundaryDatabaseImporter())
            {
                importer.Import(assessmentSection, validFilePath);
            }
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
        public void Run_ValidOvertoppingCalculation_InputPropertiesCorrectlySendToService(BreakWaterType breakWaterType)
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            ImportHydraulicBoundaryDatabase(assessmentSection);

            DikeProfile dikeProfile = CreateDikeProfile();
            dikeProfile.BreakWater.Type = breakWaterType;

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001),
                    DikeProfile = dikeProfile
                }
            };

            var calculator = new TestOvertoppingCalculator();
            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(testDataPath, string.Empty)).Return(calculator);
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
                var expectedInput = new OvertoppingCalculationInput(calculation.InputParameters.HydraulicBoundaryLocation.Id,
                                                                    calculation.InputParameters.Orientation,
                                                                    calculation.InputParameters.DikeGeometry.Select(roughnessPoint => new HydraRingRoughnessProfilePoint(roughnessPoint.Point.X, roughnessPoint.Point.Y, roughnessPoint.Roughness)),
                                                                    input.ForeshoreGeometry.Select(c => new HydraRingForelandPoint(c.X, c.Y)),
                                                                    new HydraRingBreakWater(BreakWaterTypeHelper.GetHydraRingBreakWaterType(breakWaterType), input.BreakWater.Height),
                                                                    calculation.InputParameters.DikeHeight,
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
                                                                    calculation.InputParameters.CriticalFlowRate.Mean,
                                                                    calculation.InputParameters.CriticalFlowRate.StandardDeviation,
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
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001),
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
            calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(testDataPath, string.Empty)).Return(calculator);
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
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001),
                    DikeProfile = CreateDikeProfile()
                }
            };
            var overtoppingCalculator = new TestOvertoppingCalculator();

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(testDataPath, string.Empty)).Return(overtoppingCalculator);
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
            calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(testDataPath, string.Empty)).Return(calculator);
            mockRepository.ReplayAll();

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            ImportHydraulicBoundaryDatabase(assessmentSection);

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "test", 1, 1),
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
            calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(testDataPath, string.Empty)).Return(new TestOvertoppingCalculator());
            mockRepository.ReplayAll();

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            ImportHydraulicBoundaryDatabase(assessmentSection);

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001),
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
        public void Run_ValidDikeHeightCalculation_InputPropertiesCorrectlySendToService(
            [Values(BreakWaterType.Caisson,
                BreakWaterType.Wall,
                BreakWaterType.Dam)]
            BreakWaterType breakWaterType,
            [Values(DikeHeightCalculationType.CalculateByAssessmentSectionNorm,
                DikeHeightCalculationType.CalculateByProfileSpecificRequiredProbability)]
            DikeHeightCalculationType dikeHeightCalculationType)
        {
            // Setup
            var dikeHeightCalculator = new TestHydraulicLoadsCalculator();

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(testDataPath, string.Empty)).Return(new TestOvertoppingCalculator());
            calculatorFactory.Expect(cf => cf.CreateDikeHeightCalculator(Arg<HydraRingCalculationSettings>.Is.NotNull))
                             .WhenCalled(invocation =>
                             {
                                 var settings = (HydraRingCalculationSettings)invocation.Arguments[0];
                                 Assert.AreEqual(testDataPath, settings.HlcdFilePath);
                                 Assert.IsEmpty(settings.PreprocessorDirectory);
                             })
                             .Return(dikeHeightCalculator);
            mockRepository.ReplayAll();

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            ImportHydraulicBoundaryDatabase(assessmentSection);

            DikeProfile dikeProfile = CreateDikeProfile();
            dikeProfile.BreakWater.Type = breakWaterType;

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001),
                    DikeProfile = dikeProfile,
                    DikeHeightCalculationType = dikeHeightCalculationType
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

                double norm = dikeHeightCalculationType == DikeHeightCalculationType.CalculateByAssessmentSectionNorm
                                  ? assessmentSection.FailureMechanismContribution.Norm
                                  : RingtoetsCommonDataCalculationService.ProfileSpecificRequiredProbability(
                                      assessmentSection.FailureMechanismContribution.Norm,
                                      assessmentSection.GrassCoverErosionInwards.Contribution,
                                      generalInput.N);

                var expectedInput = new DikeHeightCalculationInput(calculation.InputParameters.HydraulicBoundaryLocation.Id,
                                                                   norm,
                                                                   calculation.InputParameters.Orientation,
                                                                   calculation.InputParameters.DikeGeometry.Select(roughnessPoint => new HydraRingRoughnessProfilePoint(roughnessPoint.Point.X, roughnessPoint.Point.Y, roughnessPoint.Roughness)),
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
                                                                   calculation.InputParameters.CriticalFlowRate.Mean,
                                                                   calculation.InputParameters.CriticalFlowRate.StandardDeviation,
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
        [TestCase(DikeHeightCalculationType.CalculateByAssessmentSectionNorm, TestName = "Run_InvalidDikeHeightWithExceptionAndLastErrorPresent_LogError(Norm)")]
        [TestCase(DikeHeightCalculationType.CalculateByProfileSpecificRequiredProbability, TestName = "Run_InvalidDikeHeightWithExceptionAndLastErrorPresent_LogError(RequiredProbability)")]
        public void Run_InvalidDikeHeightCalculationWithExceptionAndLastErrorPresent_LogError(DikeHeightCalculationType dikeHeightCalculationType)
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
            calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(testDataPath, string.Empty)).Return(overtoppingCalculator);
            calculatorFactory.Expect(cf => cf.CreateDikeHeightCalculator(Arg<HydraRingCalculationSettings>.Is.NotNull))
                             .WhenCalled(invocation =>
                             {
                                 var settings = (HydraRingCalculationSettings)invocation.Arguments[0];
                                 Assert.AreEqual(testDataPath, settings.HlcdFilePath);
                                 Assert.IsEmpty(settings.PreprocessorDirectory);
                             })
                             .Return(dikeHeightCalculator);
            mockRepository.ReplayAll();

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            ImportHydraulicBoundaryDatabase(assessmentSection);

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001),
                    DikeProfile = CreateDikeProfile(),
                    DikeHeightCalculationType = dikeHeightCalculationType
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
        [TestCase(DikeHeightCalculationType.CalculateByAssessmentSectionNorm, TestName = "Run_InvalidDikeHeightWithExceptionAndNoLastErrorPresent_LogError(Norm)")]
        [TestCase(DikeHeightCalculationType.CalculateByProfileSpecificRequiredProbability, TestName = "Run_InvalidDikeHeightWithExceptionAndNoLastErrorPresent_LogError(RequiredProbability)")]
        public void Run_InvalidDikeHeightCalculationWithExceptionAndNoLastErrorPresent_LogError(DikeHeightCalculationType dikeHeightCalculationType)
        {
            // Setup
            var dikeHeightCalculator = new TestHydraulicLoadsCalculator
            {
                EndInFailure = true
            };
            var overtoppingCalculator = new TestOvertoppingCalculator();

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(testDataPath, string.Empty)).Return(overtoppingCalculator);
            calculatorFactory.Expect(cf => cf.CreateDikeHeightCalculator(Arg<HydraRingCalculationSettings>.Is.NotNull))
                             .WhenCalled(invocation =>
                             {
                                 var settings = (HydraRingCalculationSettings)invocation.Arguments[0];
                                 Assert.AreEqual(testDataPath, settings.HlcdFilePath);
                                 Assert.IsEmpty(settings.PreprocessorDirectory);
                             })
                             .Return(dikeHeightCalculator);
            mockRepository.ReplayAll();

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            ImportHydraulicBoundaryDatabase(assessmentSection);

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001),
                    DikeProfile = CreateDikeProfile(),
                    DikeHeightCalculationType = dikeHeightCalculationType
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
        [TestCase(DikeHeightCalculationType.CalculateByAssessmentSectionNorm, TestName = "Run_InvalidDikeHeightWithoutExceptionAndLastErrorPresent_LogError(Norm)")]
        [TestCase(DikeHeightCalculationType.CalculateByProfileSpecificRequiredProbability, TestName = "Run_InvalidDikeHeightWithoutExceptionAndLastErrorPresent_LogError(RequiredProbability)")]
        public void Run_InvalidDikeHeightCalculationWithoutExceptionAndWithLastErrorPresent_LogError(DikeHeightCalculationType dikeHeightCalculationType)
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
            calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(testDataPath, string.Empty)).Return(overtoppingCalculator);
            calculatorFactory.Expect(cf => cf.CreateDikeHeightCalculator(Arg<HydraRingCalculationSettings>.Is.NotNull))
                             .WhenCalled(invocation =>
                             {
                                 var settings = (HydraRingCalculationSettings)invocation.Arguments[0];
                                 Assert.AreEqual(testDataPath, settings.HlcdFilePath);
                                 Assert.IsEmpty(settings.PreprocessorDirectory);
                             })
                             .Return(dikeHeightCalculator);
            mockRepository.ReplayAll();

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            ImportHydraulicBoundaryDatabase(assessmentSection);

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001),
                    DikeProfile = CreateDikeProfile(),
                    DikeHeightCalculationType = dikeHeightCalculationType
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
        [TestCase(DikeHeightCalculationType.CalculateByAssessmentSectionNorm, TestName = "Run_ValidDikeHeight_CalculateValidateAndLog(Norm)")]
        [TestCase(DikeHeightCalculationType.CalculateByProfileSpecificRequiredProbability, TestName = "Run_ValidDikeHeight_CalculateValidateAndLog(RequiredProbability)")]
        public void Run_ValidDikeHeightCalculation_PerformValidationAndCalculationAndLogStartAndEndError(DikeHeightCalculationType dikeHeightCalculationType)
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
            calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(testDataPath, string.Empty)).Return(overtoppingCalculator);
            calculatorFactory.Expect(cf => cf.CreateDikeHeightCalculator(Arg<HydraRingCalculationSettings>.Is.NotNull))
                             .WhenCalled(invocation =>
                             {
                                 var settings = (HydraRingCalculationSettings)invocation.Arguments[0];
                                 Assert.AreEqual(testDataPath, settings.HlcdFilePath);
                                 Assert.IsEmpty(settings.PreprocessorDirectory);
                             })
                             .Return(dikeHeightCalculator);
            mockRepository.ReplayAll();

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            ImportHydraulicBoundaryDatabase(assessmentSection);

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001),
                    DikeProfile = CreateDikeProfile(),
                    DikeHeightCalculationType = dikeHeightCalculationType
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
        [TestCase(DikeHeightCalculationType.CalculateByAssessmentSectionNorm, TestName = "Finish_InvalidDikeHeight_OutputSetAndObserversNotified(Norm)")]
        [TestCase(DikeHeightCalculationType.CalculateByProfileSpecificRequiredProbability, TestName = "Finish_InvalidDikeHeight_OutputSetAndObserversNotified(RequiredProbability)")]
        public void Finish_InvalidDikeHeightCalculation_OutputSetAndObserversNotified(DikeHeightCalculationType dikeHeightCalculationType)
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
            calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(testDataPath, string.Empty)).Return(new TestOvertoppingCalculator());
            calculatorFactory.Expect(cf => cf.CreateDikeHeightCalculator(Arg<HydraRingCalculationSettings>.Is.NotNull))
                             .WhenCalled(invocation =>
                             {
                                 var settings = (HydraRingCalculationSettings)invocation.Arguments[0];
                                 Assert.AreEqual(testDataPath, settings.HlcdFilePath);
                                 Assert.IsEmpty(settings.PreprocessorDirectory);
                             })
                             .Return(dikeHeightCalculator);
            mockRepository.ReplayAll();

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            ImportHydraulicBoundaryDatabase(assessmentSection);

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001),
                    DikeProfile = CreateDikeProfile(),
                    DikeHeightCalculationType = dikeHeightCalculationType
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
        [TestCase(DikeHeightCalculationType.CalculateByAssessmentSectionNorm, TestName = "Finish_ValidDikeHeight_OutputSetAndObserversNotified(Norm)")]
        [TestCase(DikeHeightCalculationType.CalculateByProfileSpecificRequiredProbability, TestName = "Finish_ValidDikeHeight_OutputSetAndObserversNotified(RequiredProbability)")]
        public void Finish_ValidDikeHeightCalculation_OutputSetAndObserversNotified(DikeHeightCalculationType dikeHeightCalculationType)
        {
            // Setup
            var mockRepository = new MockRepository();
            var observer = mockRepository.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());

            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(testDataPath, string.Empty)).Return(new TestOvertoppingCalculator());
            calculatorFactory.Expect(cf => cf.CreateDikeHeightCalculator(Arg<HydraRingCalculationSettings>.Is.NotNull))
                             .WhenCalled(invocation =>
                             {
                                 var settings = (HydraRingCalculationSettings)invocation.Arguments[0];
                                 Assert.AreEqual(testDataPath, settings.HlcdFilePath);
                                 Assert.IsEmpty(settings.PreprocessorDirectory);
                             })
                             .Return(new TestHydraulicLoadsCalculator());
            mockRepository.ReplayAll();

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            ImportHydraulicBoundaryDatabase(assessmentSection);

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001),
                    DikeProfile = CreateDikeProfile(),
                    DikeHeightCalculationType = dikeHeightCalculationType
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
        public void Run_ValidOvertoppingRateCalculation_InputPropertiesCorrectlySendToService(
            [Values(BreakWaterType.Caisson,
                BreakWaterType.Wall,
                BreakWaterType.Dam)]
            BreakWaterType breakWaterType,
            [Values(OvertoppingRateCalculationType.CalculateByAssessmentSectionNorm,
                OvertoppingRateCalculationType.CalculateByProfileSpecificRequiredProbability)]
            OvertoppingRateCalculationType overtoppingRateCalculationType)
        {
            // Setup
            var overtoppingRateCalculator = new TestHydraulicLoadsCalculator();

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(testDataPath, string.Empty)).Return(new TestOvertoppingCalculator());
            calculatorFactory.Expect(cf => cf.CreateOvertoppingRateCalculator(Arg<HydraRingCalculationSettings>.Is.NotNull))
                             .WhenCalled(invocation =>
                             {
                                 var settings = (HydraRingCalculationSettings) invocation.Arguments[0];
                                 Assert.AreEqual(testDataPath, settings.HlcdFilePath);
                                 Assert.IsEmpty(settings.PreprocessorDirectory);
                             })
                             .Return(overtoppingRateCalculator);
            mockRepository.ReplayAll();

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            ImportHydraulicBoundaryDatabase(assessmentSection);

            DikeProfile dikeProfile = CreateDikeProfile();
            dikeProfile.BreakWater.Type = breakWaterType;

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001),
                    DikeProfile = dikeProfile,
                    OvertoppingRateCalculationType = overtoppingRateCalculationType
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

                double norm = overtoppingRateCalculationType == OvertoppingRateCalculationType.CalculateByAssessmentSectionNorm
                                  ? assessmentSection.FailureMechanismContribution.Norm
                                  : RingtoetsCommonDataCalculationService.ProfileSpecificRequiredProbability(
                                      assessmentSection.FailureMechanismContribution.Norm,
                                      assessmentSection.GrassCoverErosionInwards.Contribution,
                                      generalInput.N);

                var expectedInput = new OvertoppingRateCalculationInput(calculation.InputParameters.HydraulicBoundaryLocation.Id,
                                                                        norm,
                                                                        calculation.InputParameters.Orientation,
                                                                        calculation.InputParameters.DikeGeometry.Select(roughnessPoint => new HydraRingRoughnessProfilePoint(roughnessPoint.Point.X, roughnessPoint.Point.Y, roughnessPoint.Roughness)),
                                                                        input.ForeshoreGeometry.Select(c => new HydraRingForelandPoint(c.X, c.Y)),
                                                                        new HydraRingBreakWater(BreakWaterTypeHelper.GetHydraRingBreakWaterType(breakWaterType), input.BreakWater.Height),
                                                                        calculation.InputParameters.DikeHeight,
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
        [TestCase(OvertoppingRateCalculationType.CalculateByAssessmentSectionNorm, TestName = "Run_InvalidOvertoppingRateWithExceptionAndLastErrorPresent_LogError(Norm)")]
        [TestCase(OvertoppingRateCalculationType.CalculateByProfileSpecificRequiredProbability, TestName = "Run_InvalidOvertoppingRateWithExceptionAndLastErrorPresent_LogError(RequiredProbability)")]
        public void Run_InvalidOvertoppingRateCalculationWithExceptionAndLastErrorPresent_LogError(OvertoppingRateCalculationType overtoppingRateCalculationType)
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
            calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(testDataPath, string.Empty)).Return(overtoppingCalculator);
            calculatorFactory.Expect(cf => cf.CreateOvertoppingRateCalculator(Arg<HydraRingCalculationSettings>.Is.NotNull))
                             .WhenCalled(invocation =>
                             {
                                 var settings = (HydraRingCalculationSettings) invocation.Arguments[0];
                                 Assert.AreEqual(testDataPath, settings.HlcdFilePath);
                                 Assert.IsEmpty(settings.PreprocessorDirectory);
                             })
                             .Return(overtoppingRateCalculator);
            mockRepository.ReplayAll();

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            ImportHydraulicBoundaryDatabase(assessmentSection);

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001),
                    DikeProfile = CreateDikeProfile(),
                    OvertoppingRateCalculationType = overtoppingRateCalculationType
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
        [TestCase(OvertoppingRateCalculationType.CalculateByAssessmentSectionNorm, TestName = "Run_InvalidOvertoppingRateWithExceptionAndNoLastErrorPresent_LogError(Norm)")]
        [TestCase(OvertoppingRateCalculationType.CalculateByProfileSpecificRequiredProbability, TestName = "Run_InvalidOvertoppingRateWithExceptionAndNoLastErrorPresent_LogError(RequiredProbability)")]
        public void Run_InvalidOvertoppingRateCalculationWithExceptionAndNoLastErrorPresent_LogError(OvertoppingRateCalculationType overtoppingRateCalculationType)
        {
            // Setup
            var overtoppingRateCalculator = new TestHydraulicLoadsCalculator
            {
                EndInFailure = true
            };
            var overtoppingCalculator = new TestOvertoppingCalculator();

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(testDataPath, string.Empty)).Return(overtoppingCalculator);
            calculatorFactory.Expect(cf => cf.CreateOvertoppingRateCalculator(Arg<HydraRingCalculationSettings>.Is.NotNull))
                             .WhenCalled(invocation =>
                             {
                                 var settings = (HydraRingCalculationSettings) invocation.Arguments[0];
                                 Assert.AreEqual(testDataPath, settings.HlcdFilePath);
                                 Assert.IsEmpty(settings.PreprocessorDirectory);
                             })
                             .Return(overtoppingRateCalculator);
            mockRepository.ReplayAll();

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            ImportHydraulicBoundaryDatabase(assessmentSection);

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001),
                    DikeProfile = CreateDikeProfile(),
                    OvertoppingRateCalculationType = overtoppingRateCalculationType
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
        [TestCase(OvertoppingRateCalculationType.CalculateByAssessmentSectionNorm, TestName = "Run_InvalidOvertoppingRateWithoutExceptionAndLastErrorPresent_LogError(Norm)")]
        [TestCase(OvertoppingRateCalculationType.CalculateByProfileSpecificRequiredProbability, TestName = "Run_InvalidOvertoppingRateWithoutExceptionAndLastErrorPresent_LogError(RequiredProbability)")]
        public void Run_InvalidOvertoppingRateCalculationWithoutExceptionAndWithLastErrorPresent_LogError(OvertoppingRateCalculationType overtoppingRateCalculationType)
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
            calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(testDataPath, string.Empty)).Return(overtoppingCalculator);
            calculatorFactory.Expect(cf => cf.CreateOvertoppingRateCalculator(Arg<HydraRingCalculationSettings>.Is.NotNull))
                             .WhenCalled(invocation =>
                             {
                                 var settings = (HydraRingCalculationSettings) invocation.Arguments[0];
                                 Assert.AreEqual(testDataPath, settings.HlcdFilePath);
                                 Assert.IsEmpty(settings.PreprocessorDirectory);
                             })
                             .Return(overtoppingRateCalculator);
            mockRepository.ReplayAll();

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            ImportHydraulicBoundaryDatabase(assessmentSection);

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001),
                    DikeProfile = CreateDikeProfile(),
                    OvertoppingRateCalculationType = overtoppingRateCalculationType
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
        [TestCase(OvertoppingRateCalculationType.CalculateByAssessmentSectionNorm, TestName = "Run_ValidOvertoppingRate_CalculateValidateAndLog(Norm)")]
        [TestCase(OvertoppingRateCalculationType.CalculateByProfileSpecificRequiredProbability, TestName = "Run_ValidOvertoppingRate_CalculateValidateAndLog(RequiredProbability)")]
        public void Run_ValidOvertoppingRateCalculation_PerformValidationAndCalculationAndLogStartAndEndError(OvertoppingRateCalculationType overtoppingRateCalculationType)
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
            calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(testDataPath, string.Empty)).Return(overtoppingCalculator);
            calculatorFactory.Expect(cf => cf.CreateOvertoppingRateCalculator(Arg<HydraRingCalculationSettings>.Is.NotNull))
                             .WhenCalled(invocation =>
                             {
                                 var settings = (HydraRingCalculationSettings) invocation.Arguments[0];
                                 Assert.AreEqual(testDataPath, settings.HlcdFilePath);
                                 Assert.IsEmpty(settings.PreprocessorDirectory);
                             })
                             .Return(overtoppingRateCalculator);
            mockRepository.ReplayAll();

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            ImportHydraulicBoundaryDatabase(assessmentSection);

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001),
                    DikeProfile = CreateDikeProfile(),
                    OvertoppingRateCalculationType = overtoppingRateCalculationType
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
        [TestCase(OvertoppingRateCalculationType.CalculateByAssessmentSectionNorm, TestName = "Finish_InvalidOvertoppingRate_OutputSetAndObserversNotified(Norm)")]
        [TestCase(OvertoppingRateCalculationType.CalculateByProfileSpecificRequiredProbability, TestName = "Finish_InvalidOvertoppingRate_OutputSetAndObserversNotified(RequiredProbability)")]
        public void Finish_InvalidOvertoppingRateCalculation_OutputSetAndObserversNotified(OvertoppingRateCalculationType overtoppingRateCalculationType)
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
            calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(testDataPath, string.Empty)).Return(new TestOvertoppingCalculator());
            calculatorFactory.Expect(cf => cf.CreateOvertoppingRateCalculator(Arg<HydraRingCalculationSettings>.Is.NotNull))
                             .WhenCalled(invocation =>
                             {
                                 var settings = (HydraRingCalculationSettings) invocation.Arguments[0];
                                 Assert.AreEqual(testDataPath, settings.HlcdFilePath);
                                 Assert.IsEmpty(settings.PreprocessorDirectory);
                             })
                             .Return(overtoppingRateCalculator);
            mockRepository.ReplayAll();

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            ImportHydraulicBoundaryDatabase(assessmentSection);

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001),
                    DikeProfile = CreateDikeProfile(),
                    OvertoppingRateCalculationType = overtoppingRateCalculationType
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
        [TestCase(OvertoppingRateCalculationType.CalculateByAssessmentSectionNorm, TestName = "Finish_ValidOvertoppingRate_OutputSetAndObserversNotified(Norm)")]
        [TestCase(OvertoppingRateCalculationType.CalculateByProfileSpecificRequiredProbability, TestName = "Finish_ValidOvertoppingRate_OutputSetAndObserversNotified(RequiredProbability)")]
        public void Finish_ValidOvertoppingRateCalculation_OutputSetAndObserversNotified(OvertoppingRateCalculationType overtoppingRateCalculationType)
        {
            // Setup
            var mockRepository = new MockRepository();
            var observer = mockRepository.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());

            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(testDataPath, string.Empty)).Return(new TestOvertoppingCalculator());
            calculatorFactory.Expect(cf => cf.CreateOvertoppingRateCalculator(Arg<HydraRingCalculationSettings>.Is.NotNull))
                             .WhenCalled(invocation =>
                             {
                                 var settings = (HydraRingCalculationSettings) invocation.Arguments[0];
                                 Assert.AreEqual(testDataPath, settings.HlcdFilePath);
                                 Assert.IsEmpty(settings.PreprocessorDirectory);
                             })
                             .Return(new TestHydraulicLoadsCalculator());
            mockRepository.ReplayAll();

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            ImportHydraulicBoundaryDatabase(assessmentSection);

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001),
                    DikeProfile = CreateDikeProfile(),
                    OvertoppingRateCalculationType = overtoppingRateCalculationType
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