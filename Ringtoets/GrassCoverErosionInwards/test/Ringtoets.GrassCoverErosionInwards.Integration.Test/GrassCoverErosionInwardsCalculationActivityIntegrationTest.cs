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
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.Probability;
using Ringtoets.Common.IO.FileImporters;
using Ringtoets.Common.Service;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionInwards.Service;
using Ringtoets.HydraRing.Calculation.Calculator.Factory;
using Ringtoets.HydraRing.Calculation.Data;
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
        private static readonly string validFile = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");

        [Test]
        public void Run_InvalidHydraulicBoundaryDatabase_LogValidationStartAndEndWithError()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike)
            {
                HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
                {
                    FilePath = Path.Combine(testDataPath, "notexisting.sqlite")
                }
            };
            AddSectionToAssessmentSection(assessmentSection);

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    DikeProfile = CreateDikeProfile()
                }
            };

            var activity = new GrassCoverErosionInwardsCalculationActivity(calculation, "", assessmentSection.GrassCoverErosionInwards, assessmentSection);

            // Call
            Action call = () => activity.Run();

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                StringAssert.StartsWith($"Validatie van '{calculation.Name}' gestart om: ", msgs[0]);
                StringAssert.StartsWith("Validatie mislukt: Fout bij het lezen van bestand", msgs[1]);
                StringAssert.StartsWith($"Validatie van '{calculation.Name}' beëindigd om: ", msgs[2]);
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
            calculatorFactory.Stub(cf => cf.CreateOvertoppingCalculator(testDataPath)).Return(new TestOvertoppingCalculator());
            calculatorFactory.Stub(cf => cf.CreateOvertoppingRateCalculator(testDataPath)).Return(new TestHydraulicLoadsCalculator());
            calculatorFactory.Stub(cf => cf.CreateDikeHeightCalculator(testDataPath)).Return(new TestHydraulicLoadsCalculator());
            mockRepository.ReplayAll();

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            ImportHydraulicBoundaryDatabase(assessmentSection);
            AddSectionToAssessmentSection(assessmentSection);

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

            var activity = new GrassCoverErosionInwardsCalculationActivity(calculation, validFile, assessmentSection.GrassCoverErosionInwards, assessmentSection);

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

        private static void AddSectionToAssessmentSection(AssessmentSection assessmentSection)
        {
            assessmentSection.GrassCoverErosionInwards.AddSection(new FailureMechanismSection("test section", new[]
            {
                new Point2D(0, 0),
                new Point2D(1, 1)
            }));
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
        public void Run_ValidOvertoppingCalculation_InputPropertiesCorrectlySendToService()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            ImportHydraulicBoundaryDatabase(assessmentSection);
            AddSectionToAssessmentSection(assessmentSection);

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001),
                    DikeProfile = CreateDikeProfile()
                }
            };

            var testOvertoppingCalculator = new TestOvertoppingCalculator();
            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.Stub<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(testDataPath)).Return(testOvertoppingCalculator);
            mockRepository.ReplayAll();

            var activity = new GrassCoverErosionInwardsCalculationActivity(calculation, validFile, assessmentSection.GrassCoverErosionInwards, assessmentSection);

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                activity.Run();

                // Assert
                OvertoppingCalculationInput[] overtoppingCalculationInputs = testOvertoppingCalculator.ReceivedInputs.ToArray();
                Assert.AreEqual(1, overtoppingCalculationInputs.Length);

                OvertoppingCalculationInput actualInput = overtoppingCalculationInputs[0];
                GeneralGrassCoverErosionInwardsInput generalInput = assessmentSection.GrassCoverErosionInwards.GeneralInput;

                GrassCoverErosionInwardsInput input = calculation.InputParameters;
                var expectedInput = new OvertoppingCalculationInput(calculation.InputParameters.HydraulicBoundaryLocation.Id,
                                                                    calculation.InputParameters.Orientation,
                                                                    calculation.InputParameters.DikeGeometry.Select(roughnessPoint => new HydraRingRoughnessProfilePoint(roughnessPoint.Point.X, roughnessPoint.Point.Y, roughnessPoint.Roughness)),
                                                                    input.ForeshoreGeometry.Select(c => new HydraRingForelandPoint(c.X, c.Y)),
                                                                    new HydraRingBreakWater((int) input.BreakWater.Type, input.BreakWater.Height),
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
        public void Run_InvalidOvertoppingCalculationWithExceptionAndLastErrorPresent_LogErrorAndThrowException()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            ImportHydraulicBoundaryDatabase(assessmentSection);
            AddSectionToAssessmentSection(assessmentSection);

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001),
                    DikeProfile = CreateDikeProfile()
                }
            };

            var testOvertoppingCalculator = new TestOvertoppingCalculator
            {
                LastErrorFileContent = "An error occurred",
                EndInFailure = true
            };
            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.Stub<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(testDataPath)).Return(testOvertoppingCalculator);
            mockRepository.ReplayAll();

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                var activity = new GrassCoverErosionInwardsCalculationActivity(calculation, validFile, assessmentSection.GrassCoverErosionInwards, assessmentSection);

                // Call
                Action call = () => activity.Run();

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(6, msgs.Length);
                    StringAssert.StartsWith($"Validatie van '{calculation.Name}' gestart om: ", msgs[0]);
                    StringAssert.StartsWith($"Validatie van '{calculation.Name}' beëindigd om: ", msgs[1]);
                    StringAssert.StartsWith($"Berekening van '{calculation.Name}' gestart om: ", msgs[2]);
                    StringAssert.StartsWith($"De overloop en overslag berekening voor grasbekleding erosie kruin en binnentalud '{calculation.Name}' is niet gelukt. Bekijk het foutrapport door op details te klikken.", msgs[3]);
                    StringAssert.StartsWith("De overloop en overslag berekening is uitgevoerd op de tijdelijke locatie", msgs[4]);
                    StringAssert.StartsWith($"Berekening van '{calculation.Name}' beëindigd om: ", msgs[5]);
                });
                Assert.AreEqual(ActivityState.Failed, activity.State);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        public void Run_InvalidOvertoppingCalculationWithExceptionAndNoLastErrorPresent_LogErrorAndThrowException()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            ImportHydraulicBoundaryDatabase(assessmentSection);
            AddSectionToAssessmentSection(assessmentSection);

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001),
                    DikeProfile = CreateDikeProfile()
                }
            };
            var testOvertoppingCalculator = new TestOvertoppingCalculator
            {
                EndInFailure = true
            };
            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.Stub<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(testDataPath)).Return(testOvertoppingCalculator);
            mockRepository.ReplayAll();

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                var activity = new GrassCoverErosionInwardsCalculationActivity(calculation, validFile, assessmentSection.GrassCoverErosionInwards, assessmentSection);

                // Call
                Action call = () => activity.Run();

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(6, msgs.Length);
                    StringAssert.StartsWith($"Validatie van '{calculation.Name}' gestart om: ", msgs[0]);
                    StringAssert.StartsWith($"Validatie van '{calculation.Name}' beëindigd om: ", msgs[1]);
                    StringAssert.StartsWith($"Berekening van '{calculation.Name}' gestart om: ", msgs[2]);
                    StringAssert.StartsWith($"De overloop en overslag berekening voor grasbekleding erosie kruin en binnentalud '{calculation.Name}' is niet gelukt. Er is geen foutrapport beschikbaar.", msgs[3]);
                    StringAssert.StartsWith("De overloop en overslag berekening is uitgevoerd op de tijdelijke locatie", msgs[4]);
                    StringAssert.StartsWith($"Berekening van '{calculation.Name}' beëindigd om: ", msgs[5]);
                });
                Assert.AreEqual(ActivityState.Failed, activity.State);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        public void Run_InvalidOvertoppingCalculationWithoutExceptionAndWithLastErrorPresent_LogErrorAndThrowException()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            ImportHydraulicBoundaryDatabase(assessmentSection);
            AddSectionToAssessmentSection(assessmentSection);

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001),
                    DikeProfile = CreateDikeProfile()
                }
            };
            var testOvertoppingCalculator = new TestOvertoppingCalculator
            {
                LastErrorFileContent = "An error occurred",
                EndInFailure = false
            };
            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.Stub<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(testDataPath)).Return(testOvertoppingCalculator);
            mockRepository.ReplayAll();

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                var activity = new GrassCoverErosionInwardsCalculationActivity(calculation, validFile, assessmentSection.GrassCoverErosionInwards, assessmentSection);

                // Call
                Action call = () => activity.Run();

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(6, msgs.Length);
                    StringAssert.StartsWith($"Validatie van '{calculation.Name}' gestart om: ", msgs[0]);
                    StringAssert.StartsWith($"Validatie van '{calculation.Name}' beëindigd om: ", msgs[1]);
                    StringAssert.StartsWith($"Berekening van '{calculation.Name}' gestart om: ", msgs[2]);
                    StringAssert.StartsWith($"De overloop en overslag berekening voor grasbekleding erosie kruin en binnentalud '{calculation.Name}' is niet gelukt. Bekijk het foutrapport door op details te klikken.", msgs[3]);
                    StringAssert.StartsWith("De overloop en overslag berekening is uitgevoerd op de tijdelijke locatie", msgs[4]);
                    StringAssert.StartsWith($"Berekening van '{calculation.Name}' beëindigd om: ", msgs[5]);
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
            AddSectionToAssessmentSection(assessmentSection);

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001),
                    DikeProfile = CreateDikeProfile()
                }
            };

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.Stub<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(testDataPath)).Return(new TestOvertoppingCalculator());
            mockRepository.ReplayAll();

            var activity = new GrassCoverErosionInwardsCalculationActivity(calculation, validFile, assessmentSection.GrassCoverErosionInwards, assessmentSection);

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                Action call = () => activity.Run();

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(5, msgs.Length);
                    StringAssert.StartsWith($"Validatie van '{calculation.Name}' gestart om: ", msgs[0]);
                    StringAssert.StartsWith($"Validatie van '{calculation.Name}' beëindigd om: ", msgs[1]);
                    StringAssert.StartsWith($"Berekening van '{calculation.Name}' gestart om: ", msgs[2]);
                    StringAssert.StartsWith("De overloop en overslag berekening is uitgevoerd op de tijdelijke locatie", msgs[3]);
                    StringAssert.StartsWith($"Berekening van '{calculation.Name}' beëindigd om: ", msgs[4]);
                });
                Assert.AreEqual(ActivityState.Executed, activity.State);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        public void Finish_InvalidOvertoppingCalculation_DoesNotSetOutputAndNotifyObservers()
        {
            // Setup
            var mockRepository = new MockRepository();
            var observerMock = mockRepository.StrictMock<IObserver>();
            observerMock.Expect(o => o.UpdateObserver());

            var testOvertoppingCalculator = new TestOvertoppingCalculator
            {
                EndInFailure = true
            };
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(testDataPath)).Return(testOvertoppingCalculator);
            mockRepository.ReplayAll();

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            ImportHydraulicBoundaryDatabase(assessmentSection);
            AddSectionToAssessmentSection(assessmentSection);

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "test", 1, 1),
                    DikeProfile = CreateDikeProfile()
                }
            };

            calculation.Attach(observerMock);

            var activity = new GrassCoverErosionInwardsCalculationActivity(calculation, validFile, assessmentSection.GrassCoverErosionInwards, assessmentSection);

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
            var observerMock = mockRepository.StrictMock<IObserver>();
            observerMock.Expect(o => o.UpdateObserver());

            var calculatorFactory = mockRepository.Stub<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(testDataPath)).Return(new TestOvertoppingCalculator());
            mockRepository.ReplayAll();

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            ImportHydraulicBoundaryDatabase(assessmentSection);
            AddSectionToAssessmentSection(assessmentSection);

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001),
                    DikeProfile = CreateDikeProfile()
                }
            };

            calculation.Attach(observerMock);

            var activity = new GrassCoverErosionInwardsCalculationActivity(calculation, validFile, assessmentSection.GrassCoverErosionInwards, assessmentSection);

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                activity.Run();
            }

            // Call
            activity.Finish();

            // Assert
            Assert.IsNotNull(calculation.Output);
            Assert.IsFalse(double.IsNaN(calculation.Output.ProbabilityAssessmentOutput.Reliability));
            Assert.IsNull(calculation.Output.DikeHeightOutput);
            Assert.IsNull(calculation.Output.OvertoppingRateOutput);
            mockRepository.VerifyAll();
        }

        #endregion

        #region Dike height calculations

        [Test]
        [TestCase(DikeHeightCalculationType.CalculateByAssessmentSectionNorm, TestName = "Run_ValidDikeHeight_InputPropertiesCorrectlySendToService(Norm)")]
        [TestCase(DikeHeightCalculationType.CalculateByProfileSpecificRequiredProbability, TestName = "Run_ValidDikeHeight_InputPropertiesCorrectlySendToService(RequiredProbability)")]
        public void Run_ValidDikeHeightCalculation_InputPropertiesCorrectlySendToService(DikeHeightCalculationType dikeHeightCalculationType)
        {
            // Setup
            var testDikeHeightCalculator = new TestHydraulicLoadsCalculator();

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.Stub<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(testDataPath)).Return(new TestOvertoppingCalculator());
            calculatorFactory.Expect(cf => cf.CreateDikeHeightCalculator(testDataPath)).Return(testDikeHeightCalculator);
            mockRepository.ReplayAll();

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            ImportHydraulicBoundaryDatabase(assessmentSection);
            AddSectionToAssessmentSection(assessmentSection);

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001),
                    DikeProfile = CreateDikeProfile(),
                    DikeHeightCalculationType = dikeHeightCalculationType
                }
            };

            var activity = new GrassCoverErosionInwardsCalculationActivity(calculation, validFile, assessmentSection.GrassCoverErosionInwards, assessmentSection);

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                activity.Run();

                // Assert
                HydraulicLoadsCalculationInput[] hydraulicLoadsCalculationInputs = testDikeHeightCalculator.ReceivedInputs.ToArray();
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
                                                                   new HydraRingBreakWater((int) input.BreakWater.Type, input.BreakWater.Height),
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
            var testDikeHeightCalculator = new TestHydraulicLoadsCalculator
            {
                LastErrorFileContent = "An error occurred",
                EndInFailure = true
            };

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.Stub<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(testDataPath)).Return(new TestOvertoppingCalculator());
            calculatorFactory.Expect(cf => cf.CreateDikeHeightCalculator(testDataPath)).Return(testDikeHeightCalculator);
            mockRepository.ReplayAll();

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            ImportHydraulicBoundaryDatabase(assessmentSection);
            AddSectionToAssessmentSection(assessmentSection);

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
                var activity = new GrassCoverErosionInwardsCalculationActivity(calculation, validFile, assessmentSection.GrassCoverErosionInwards, assessmentSection);

                // Call
                Action call = () => activity.Run();

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(7, msgs.Length);
                    StringAssert.StartsWith($"Validatie van '{calculation.Name}' gestart om: ", msgs[0]);
                    StringAssert.StartsWith($"Validatie van '{calculation.Name}' beëindigd om: ", msgs[1]);
                    StringAssert.StartsWith($"Berekening van '{calculation.Name}' gestart om: ", msgs[2]);
                    StringAssert.StartsWith("De overloop en overslag berekening is uitgevoerd op de tijdelijke locatie", msgs[3]);
                    StringAssert.StartsWith($"De HBN berekening voor grasbekleding erosie kruin en binnentalud '{calculation.Name}' is niet gelukt. Bekijk het foutrapport door op details te klikken.", msgs[4]);
                    StringAssert.StartsWith("De HBN berekening is uitgevoerd op de tijdelijke locatie", msgs[5]);
                    StringAssert.StartsWith($"Berekening van '{calculation.Name}' beëindigd om: ", msgs[6]);
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
            var testDikeHeightCalculator = new TestHydraulicLoadsCalculator
            {
                EndInFailure = true
            };

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.Stub<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(testDataPath)).Return(new TestOvertoppingCalculator());
            calculatorFactory.Expect(cf => cf.CreateDikeHeightCalculator(testDataPath)).Return(testDikeHeightCalculator);
            mockRepository.ReplayAll();

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            ImportHydraulicBoundaryDatabase(assessmentSection);
            AddSectionToAssessmentSection(assessmentSection);

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
                var activity = new GrassCoverErosionInwardsCalculationActivity(calculation, validFile, assessmentSection.GrassCoverErosionInwards, assessmentSection);

                // Call
                Action call = () => activity.Run();

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(7, msgs.Length);
                    StringAssert.StartsWith($"Validatie van '{calculation.Name}' gestart om: ", msgs[0]);
                    StringAssert.StartsWith($"Validatie van '{calculation.Name}' beëindigd om: ", msgs[1]);
                    StringAssert.StartsWith($"Berekening van '{calculation.Name}' gestart om: ", msgs[2]);
                    StringAssert.StartsWith("De overloop en overslag berekening is uitgevoerd op de tijdelijke locatie", msgs[3]);
                    StringAssert.StartsWith($"De HBN berekening voor grasbekleding erosie kruin en binnentalud '{calculation.Name}' is niet gelukt. Er is geen foutrapport beschikbaar.", msgs[4]);
                    StringAssert.StartsWith("De HBN berekening is uitgevoerd op de tijdelijke locatie", msgs[5]);
                    StringAssert.StartsWith($"Berekening van '{calculation.Name}' beëindigd om: ", msgs[6]);
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
            var testDikeHeightCalculator = new TestHydraulicLoadsCalculator
            {
                EndInFailure = false,
                LastErrorFileContent = "An error occurred"
            };

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.Stub<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(testDataPath)).Return(new TestOvertoppingCalculator());
            calculatorFactory.Expect(cf => cf.CreateDikeHeightCalculator(testDataPath)).Return(testDikeHeightCalculator);
            mockRepository.ReplayAll();

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            ImportHydraulicBoundaryDatabase(assessmentSection);
            AddSectionToAssessmentSection(assessmentSection);

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
                var activity = new GrassCoverErosionInwardsCalculationActivity(calculation, validFile, assessmentSection.GrassCoverErosionInwards, assessmentSection);

                // Call
                Action call = () => activity.Run();

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(7, msgs.Length);
                    StringAssert.StartsWith($"Validatie van '{calculation.Name}' gestart om: ", msgs[0]);
                    StringAssert.StartsWith($"Validatie van '{calculation.Name}' beëindigd om: ", msgs[1]);
                    StringAssert.StartsWith($"Berekening van '{calculation.Name}' gestart om: ", msgs[2]);
                    StringAssert.StartsWith("De overloop en overslag berekening is uitgevoerd op de tijdelijke locatie", msgs[3]);
                    StringAssert.StartsWith($"De HBN berekening voor grasbekleding erosie kruin en binnentalud '{calculation.Name}' is niet gelukt. Bekijk het foutrapport door op details te klikken.", msgs[4]);
                    StringAssert.StartsWith("De HBN berekening is uitgevoerd op de tijdelijke locatie", msgs[5]);
                    StringAssert.StartsWith($"Berekening van '{calculation.Name}' beëindigd om: ", msgs[6]);
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
            var testDikeHeightCalculator = new TestHydraulicLoadsCalculator
            {
                Value = 2,
                ReliabilityIndex = -1
            };

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.Stub<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(testDataPath)).Return(new TestOvertoppingCalculator());
            calculatorFactory.Expect(cf => cf.CreateDikeHeightCalculator(testDataPath)).Return(testDikeHeightCalculator);
            mockRepository.ReplayAll();

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike)
            {
                FailureMechanismContribution =
                {
                    Norm = 1
                }
            };
            ImportHydraulicBoundaryDatabase(assessmentSection);
            AddSectionToAssessmentSection(assessmentSection);

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001),
                    DikeProfile = CreateDikeProfile(),
                    DikeHeightCalculationType = dikeHeightCalculationType
                }
            };

            var activity = new GrassCoverErosionInwardsCalculationActivity(calculation, validFile, assessmentSection.GrassCoverErosionInwards, assessmentSection);

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                Action call = () => activity.Run();

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(7, msgs.Length);
                    StringAssert.StartsWith($"Validatie van '{calculation.Name}' gestart om: ", msgs[0]);
                    StringAssert.StartsWith($"Validatie van '{calculation.Name}' beëindigd om: ", msgs[1]);
                    StringAssert.StartsWith($"Berekening van '{calculation.Name}' gestart om: ", msgs[2]);
                    StringAssert.StartsWith("De overloop en overslag berekening is uitgevoerd op de tijdelijke locatie", msgs[3]);
                    StringAssert.StartsWith("De HBN berekening is uitgevoerd op de tijdelijke locatie", msgs[4]);
                    StringAssert.StartsWith($"De HBN berekening voor grasbekleding erosie kruin en binnentalud '{calculation.Name}' is niet geconvergeerd.", msgs[5]);
                    StringAssert.StartsWith($"Berekening van '{calculation.Name}' beëindigd om: ", msgs[6]);
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
            var observerMock = mockRepository.StrictMock<IObserver>();
            observerMock.Expect(o => o.UpdateObserver());

            var testDikeHeightCalculator = new TestHydraulicLoadsCalculator
            {
                Value = double.NaN,
                EndInFailure = true
            };
            var calculatorFactory = mockRepository.Stub<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(testDataPath)).Return(new TestOvertoppingCalculator());
            calculatorFactory.Expect(cf => cf.CreateDikeHeightCalculator(testDataPath)).Return(testDikeHeightCalculator);
            mockRepository.ReplayAll();

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            ImportHydraulicBoundaryDatabase(assessmentSection);
            AddSectionToAssessmentSection(assessmentSection);

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001),
                    DikeProfile = CreateDikeProfile(),
                    DikeHeightCalculationType = dikeHeightCalculationType
                }
            };

            calculation.Attach(observerMock);

            var activity = new GrassCoverErosionInwardsCalculationActivity(calculation, validFile, assessmentSection.GrassCoverErosionInwards, assessmentSection);

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
            var observerMock = mockRepository.StrictMock<IObserver>();
            observerMock.Expect(o => o.UpdateObserver());

            var calculatorFactory = mockRepository.Stub<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(testDataPath)).Return(new TestOvertoppingCalculator());
            calculatorFactory.Expect(cf => cf.CreateDikeHeightCalculator(testDataPath)).Return(new TestHydraulicLoadsCalculator());
            mockRepository.ReplayAll();

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            ImportHydraulicBoundaryDatabase(assessmentSection);
            AddSectionToAssessmentSection(assessmentSection);

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001),
                    DikeProfile = CreateDikeProfile(),
                    DikeHeightCalculationType = dikeHeightCalculationType
                }
            };

            calculation.Attach(observerMock);

            var activity = new GrassCoverErosionInwardsCalculationActivity(calculation, validFile, assessmentSection.GrassCoverErosionInwards, assessmentSection);

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                activity.Run();
            }

            // Call
            activity.Finish();

            // Assert
            Assert.IsNotNull(calculation.Output);
            ProbabilityAssessmentOutput probabilisticAssessmentOutput = calculation.Output.ProbabilityAssessmentOutput;
            Assert.IsFalse(double.IsNaN(probabilisticAssessmentOutput.Reliability));
            DikeHeightOutput dikeHeightOutput = calculation.Output.DikeHeightOutput;
            Assert.IsNotNull(dikeHeightOutput);
            Assert.IsFalse(double.IsNaN(dikeHeightOutput.DikeHeight));
            mockRepository.VerifyAll();
        }

        #endregion

        #region Overtopping rate calculations

        [Test]
        [TestCase(OvertoppingRateCalculationType.CalculateByAssessmentSectionNorm, TestName = "Run_ValidOvertoppingRate_InputPropertiesCorrectlySendToService(Norm)")]
        [TestCase(OvertoppingRateCalculationType.CalculateByProfileSpecificRequiredProbability, TestName = "Run_ValidOvertoppingRate_InputPropertiesCorrectlySendToService(RequiredProbability)")]
        public void Run_ValidOvertoppingRateCalculation_InputPropertiesCorrectlySendToService(OvertoppingRateCalculationType overtoppingRateCalculationType)
        {
            // Setup
            var overtoppingRateCalculator = new TestHydraulicLoadsCalculator();

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.Stub<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(testDataPath)).Return(new TestOvertoppingCalculator());
            calculatorFactory.Expect(cf => cf.CreateOvertoppingRateCalculator(testDataPath)).Return(overtoppingRateCalculator);
            mockRepository.ReplayAll();

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            ImportHydraulicBoundaryDatabase(assessmentSection);
            AddSectionToAssessmentSection(assessmentSection);

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001),
                    DikeProfile = CreateDikeProfile(),
                    OvertoppingRateCalculationType = overtoppingRateCalculationType
                }
            };

            var activity = new GrassCoverErosionInwardsCalculationActivity(calculation, validFile, assessmentSection.GrassCoverErosionInwards, assessmentSection);

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
                                                                        new HydraRingBreakWater((int) input.BreakWater.Type, input.BreakWater.Height),
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

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.Stub<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(testDataPath)).Return(new TestOvertoppingCalculator());
            calculatorFactory.Expect(cf => cf.CreateOvertoppingRateCalculator(testDataPath)).Return(overtoppingRateCalculator);
            mockRepository.ReplayAll();

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            ImportHydraulicBoundaryDatabase(assessmentSection);
            AddSectionToAssessmentSection(assessmentSection);

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
                var activity = new GrassCoverErosionInwardsCalculationActivity(calculation, validFile, assessmentSection.GrassCoverErosionInwards, assessmentSection);

                // Call
                Action call = () => activity.Run();

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(7, msgs.Length);
                    StringAssert.StartsWith($"Validatie van '{calculation.Name}' gestart om: ", msgs[0]);
                    StringAssert.StartsWith($"Validatie van '{calculation.Name}' beëindigd om: ", msgs[1]);
                    StringAssert.StartsWith($"Berekening van '{calculation.Name}' gestart om: ", msgs[2]);
                    StringAssert.StartsWith("De overloop en overslag berekening is uitgevoerd op de tijdelijke locatie", msgs[3]);
                    StringAssert.StartsWith($"De overslagdebiet berekening voor grasbekleding erosie kruin en binnentalud '{calculation.Name}' is niet gelukt. Bekijk het foutrapport door op details te klikken.", msgs[4]);
                    StringAssert.StartsWith("De overslagdebiet berekening is uitgevoerd op de tijdelijke locatie", msgs[5]);
                    StringAssert.StartsWith($"Berekening van '{calculation.Name}' beëindigd om: ", msgs[6]);
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

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.Stub<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(testDataPath)).Return(new TestOvertoppingCalculator());
            calculatorFactory.Expect(cf => cf.CreateOvertoppingRateCalculator(testDataPath)).Return(overtoppingRateCalculator);
            mockRepository.ReplayAll();

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            ImportHydraulicBoundaryDatabase(assessmentSection);
            AddSectionToAssessmentSection(assessmentSection);

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
                var activity = new GrassCoverErosionInwardsCalculationActivity(calculation, validFile, assessmentSection.GrassCoverErosionInwards, assessmentSection);

                // Call
                Action call = () => activity.Run();

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(7, msgs.Length);
                    StringAssert.StartsWith($"Validatie van '{calculation.Name}' gestart om: ", msgs[0]);
                    StringAssert.StartsWith($"Validatie van '{calculation.Name}' beëindigd om: ", msgs[1]);
                    StringAssert.StartsWith($"Berekening van '{calculation.Name}' gestart om: ", msgs[2]);
                    StringAssert.StartsWith("De overloop en overslag berekening is uitgevoerd op de tijdelijke locatie", msgs[3]);
                    StringAssert.StartsWith($"De overslagdebiet berekening voor grasbekleding erosie kruin en binnentalud '{calculation.Name}' is niet gelukt. Er is geen foutrapport beschikbaar.", msgs[4]);
                    StringAssert.StartsWith("De overslagdebiet berekening is uitgevoerd op de tijdelijke locatie", msgs[5]);
                    StringAssert.StartsWith($"Berekening van '{calculation.Name}' beëindigd om: ", msgs[6]);
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

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.Stub<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(testDataPath)).Return(new TestOvertoppingCalculator());
            calculatorFactory.Expect(cf => cf.CreateOvertoppingRateCalculator(testDataPath)).Return(overtoppingRateCalculator);
            mockRepository.ReplayAll();

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            ImportHydraulicBoundaryDatabase(assessmentSection);
            AddSectionToAssessmentSection(assessmentSection);

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
                var activity = new GrassCoverErosionInwardsCalculationActivity(calculation, validFile, assessmentSection.GrassCoverErosionInwards, assessmentSection);

                // Call
                Action call = () => activity.Run();

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(7, msgs.Length);
                    StringAssert.StartsWith($"Validatie van '{calculation.Name}' gestart om: ", msgs[0]);
                    StringAssert.StartsWith($"Validatie van '{calculation.Name}' beëindigd om: ", msgs[1]);
                    StringAssert.StartsWith($"Berekening van '{calculation.Name}' gestart om: ", msgs[2]);
                    StringAssert.StartsWith("De overloop en overslag berekening is uitgevoerd op de tijdelijke locatie", msgs[3]);
                    StringAssert.StartsWith($"De overslagdebiet berekening voor grasbekleding erosie kruin en binnentalud '{calculation.Name}' is niet gelukt. Bekijk het foutrapport door op details te klikken.", msgs[4]);
                    StringAssert.StartsWith("De overslagdebiet berekening is uitgevoerd op de tijdelijke locatie", msgs[5]);
                    StringAssert.StartsWith($"Berekening van '{calculation.Name}' beëindigd om: ", msgs[6]);
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

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.Stub<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(testDataPath)).Return(new TestOvertoppingCalculator());
            calculatorFactory.Expect(cf => cf.CreateOvertoppingRateCalculator(testDataPath)).Return(overtoppingRateCalculator);
            mockRepository.ReplayAll();

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike)
            {
                FailureMechanismContribution =
                {
                    Norm = 1
                }
            };
            ImportHydraulicBoundaryDatabase(assessmentSection);
            AddSectionToAssessmentSection(assessmentSection);

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001),
                    DikeProfile = CreateDikeProfile(),
                    OvertoppingRateCalculationType = overtoppingRateCalculationType
                }
            };

            var activity = new GrassCoverErosionInwardsCalculationActivity(calculation, validFile, assessmentSection.GrassCoverErosionInwards, assessmentSection);

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                Action call = () => activity.Run();

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(7, msgs.Length);
                    StringAssert.StartsWith($"Validatie van '{calculation.Name}' gestart om: ", msgs[0]);
                    StringAssert.StartsWith($"Validatie van '{calculation.Name}' beëindigd om: ", msgs[1]);
                    StringAssert.StartsWith($"Berekening van '{calculation.Name}' gestart om: ", msgs[2]);
                    StringAssert.StartsWith("De overloop en overslag berekening is uitgevoerd op de tijdelijke locatie", msgs[3]);
                    StringAssert.StartsWith("De overslagdebiet berekening is uitgevoerd op de tijdelijke locatie", msgs[4]);
                    StringAssert.StartsWith($"De overslagdebiet berekening voor grasbekleding erosie kruin en binnentalud '{calculation.Name}' is niet geconvergeerd.", msgs[5]);
                    StringAssert.StartsWith($"Berekening van '{calculation.Name}' beëindigd om: ", msgs[6]);
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
            var observerMock = mockRepository.StrictMock<IObserver>();
            observerMock.Expect(o => o.UpdateObserver());

            var overtoppingRateCalculator = new TestHydraulicLoadsCalculator
            {
                Value = double.NaN,
                EndInFailure = true
            };
            var calculatorFactory = mockRepository.Stub<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(testDataPath)).Return(new TestOvertoppingCalculator());
            calculatorFactory.Expect(cf => cf.CreateOvertoppingRateCalculator(testDataPath)).Return(overtoppingRateCalculator);
            mockRepository.ReplayAll();

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            ImportHydraulicBoundaryDatabase(assessmentSection);
            AddSectionToAssessmentSection(assessmentSection);

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001),
                    DikeProfile = CreateDikeProfile(),
                    OvertoppingRateCalculationType = overtoppingRateCalculationType
                }
            };

            calculation.Attach(observerMock);

            var activity = new GrassCoverErosionInwardsCalculationActivity(calculation, validFile, assessmentSection.GrassCoverErosionInwards, assessmentSection);

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
            var observerMock = mockRepository.StrictMock<IObserver>();
            observerMock.Expect(o => o.UpdateObserver());

            var calculatorFactory = mockRepository.Stub<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(testDataPath)).Return(new TestOvertoppingCalculator());
            calculatorFactory.Expect(cf => cf.CreateOvertoppingRateCalculator(testDataPath)).Return(new TestHydraulicLoadsCalculator());
            mockRepository.ReplayAll();

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            ImportHydraulicBoundaryDatabase(assessmentSection);
            AddSectionToAssessmentSection(assessmentSection);

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001),
                    DikeProfile = CreateDikeProfile(),
                    OvertoppingRateCalculationType = overtoppingRateCalculationType
                }
            };

            calculation.Attach(observerMock);

            var activity = new GrassCoverErosionInwardsCalculationActivity(calculation, validFile, assessmentSection.GrassCoverErosionInwards, assessmentSection);

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                activity.Run();
            }

            // Call
            activity.Finish();

            // Assert
            Assert.IsNotNull(calculation.Output);
            ProbabilityAssessmentOutput probabilisticAssessmentOutput = calculation.Output.ProbabilityAssessmentOutput;
            Assert.IsFalse(double.IsNaN(probabilisticAssessmentOutput.Reliability));
            OvertoppingRateOutput overtoppingRateOutput = calculation.Output.OvertoppingRateOutput;
            Assert.IsNotNull(overtoppingRateOutput);
            Assert.IsFalse(double.IsNaN(overtoppingRateOutput.OvertoppingRate));
            mockRepository.VerifyAll();
        }

        #endregion
    }
}