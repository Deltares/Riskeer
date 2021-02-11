// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using Riskeer.Common.Data.Probabilistics;
using Riskeer.Common.Service;
using Riskeer.Common.Service.TestUtil;
using Riskeer.HydraRing.Calculation.Calculator.Factory;
using Riskeer.HydraRing.Calculation.Data.Input.Piping;
using Riskeer.HydraRing.Calculation.TestUtil;
using Riskeer.HydraRing.Calculation.TestUtil.Calculator;
using Riskeer.Integration.Data;
using Riskeer.Integration.TestUtil;
using Riskeer.Piping.Data;
using Riskeer.Piping.Data.Probabilistic;
using Riskeer.Piping.Data.TestUtil;
using Riskeer.Piping.Data.TestUtil.Probabilistic;
using Riskeer.Piping.Service;

namespace Riskeer.Piping.Integration.Test
{
    [TestFixture]
    public class ProbabilisticPipingCalculationActivityIntegrationTest
    {
        private static readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Integration.Service, "HydraRingCalculation");
        private static readonly string validFilePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");

        [Test]
        public void Run_CalculationInvalidInput_LogValidationStartAndEndWithError()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike)
            {
                HydraulicBoundaryDatabase =
                {
                    FilePath = Path.Combine(testDataPath, "notexisting.sqlite")
                }
            };

            var failureMechanism = new PipingFailureMechanism();
            var calculation = new TestProbabilisticPipingCalculation();

            CalculatableActivity activity = PipingCalculationActivityFactory.CreateProbabilisticPipingCalculationActivity(
                calculation, failureMechanism, assessmentSection);

            // Call
            void Call() => activity.Run();

            // Assert
            TestHelper.AssertLogMessages(Call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(4, msgs.Length);
                Assert.AreEqual($"Uitvoeren van berekening '{calculation.Name}' is gestart.", msgs[0]);
                CalculationServiceTestHelper.AssertValidationStartMessage(msgs[1]);
                StringAssert.StartsWith("Herstellen van de verbinding met de hydraulische belastingendatabase is mislukt. " +
                                        "Fout bij het lezen van bestand", msgs[2]);
                CalculationServiceTestHelper.AssertValidationEndMessage(msgs[3]);
            });
            Assert.AreEqual(ActivityState.Failed, activity.State);
        }

        [Test]
        public void Run_ValidCalculation_PerformValidationAndCalculationAndLogStartAndEnd()
        {
            // Setup
            var mocks = new MockRepository();
            var calculatorFactory = mocks.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreatePipingCalculator(null))
                             .IgnoreArguments()
                             .Return(new TestPipingCalculator())
                             .Repeat.Twice();
            mocks.ReplayAll();

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);

            DataImportHelper.ImportHydraulicBoundaryDatabase(assessmentSection, validFilePath);

            TestPipingFailureMechanism failureMechanism = TestPipingFailureMechanism.GetFailureMechanismWithSurfaceLinesAndStochasticSoilModels();
            var calculation = ProbabilisticPipingCalculationTestFactory.CreateCalculationWithValidInput<TestProbabilisticPipingCalculation>(
                assessmentSection.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001));

            CalculatableActivity activity = PipingCalculationActivityFactory.CreateProbabilisticPipingCalculationActivity(
                calculation, failureMechanism, assessmentSection);

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                void Call() => activity.Run();

                // Assert
                TestHelper.AssertLogMessages(Call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(7, msgs.Length);
                    Assert.AreEqual($"Uitvoeren van berekening '{calculation.Name}' is gestart.", msgs[0]);
                    CalculationServiceTestHelper.AssertValidationStartMessage(msgs[1]);
                    CalculationServiceTestHelper.AssertValidationEndMessage(msgs[2]);
                    CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[3]);
                    StringAssert.StartsWith("De piping sterkte berekening voor doorsnede is uitgevoerd op de tijdelijke locatie", msgs[4]);
                    StringAssert.StartsWith("De piping sterkte berekening voor vak is uitgevoerd op de tijdelijke locatie", msgs[5]);
                    CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[6]);
                });
                Assert.AreEqual(ActivityState.Executed, activity.State);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void GivenProbabilisticPipingCalculationActivity_WhenRunAndFinished_ThenOutputSetAndObserversNotified()
        {
            // Given
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            var calculatorFactory = mocks.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreatePipingCalculator(null))
                             .IgnoreArguments()
                             .Return(new TestPipingCalculator())
                             .Repeat.Twice();
            mocks.ReplayAll();

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);

            DataImportHelper.ImportHydraulicBoundaryDatabase(assessmentSection, validFilePath);

            TestPipingFailureMechanism failureMechanism = TestPipingFailureMechanism.GetFailureMechanismWithSurfaceLinesAndStochasticSoilModels();
            var calculation = ProbabilisticPipingCalculationTestFactory.CreateCalculationWithValidInput<TestProbabilisticPipingCalculation>(
                assessmentSection.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001));

            calculation.Attach(observer);

            CalculatableActivity activity = PipingCalculationActivityFactory.CreateProbabilisticPipingCalculationActivity(
                calculation, failureMechanism, assessmentSection);

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // When
                activity.Run();
                activity.Finish();

                // Then
                Assert.IsNotNull(calculation.Output);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void Run_ValidCalculation_ProgressTextSetAccordingly()
        {
            // Setup
            var mocks = new MockRepository();
            var calculatorFactory = mocks.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreatePipingCalculator(null))
                             .IgnoreArguments()
                             .Return(new TestPipingCalculator())
                             .Repeat.Twice();
            mocks.ReplayAll();

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);

            DataImportHelper.ImportHydraulicBoundaryDatabase(assessmentSection, validFilePath);

            TestPipingFailureMechanism failureMechanism = TestPipingFailureMechanism.GetFailureMechanismWithSurfaceLinesAndStochasticSoilModels();
            var calculation = ProbabilisticPipingCalculationTestFactory.CreateCalculationWithValidInput<TestProbabilisticPipingCalculation>(
                assessmentSection.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001));

            CalculatableActivity activity = PipingCalculationActivityFactory.CreateProbabilisticPipingCalculationActivity(
                calculation, failureMechanism, assessmentSection);

            var progressTexts = "";
            activity.ProgressChanged += (s, e) => progressTexts += activity.ProgressText + Environment.NewLine;

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                activity.Run();

                // Assert
                string expectedProgressTexts = "Stap 1 van 2 | Uitvoeren sterkte berekening voor doorsnede" + Environment.NewLine +
                                               "Stap 2 van 2 | Uitvoeren sterkte berekening voor vak" + Environment.NewLine;

                Assert.AreEqual(expectedProgressTexts, progressTexts);
            }

            mocks.VerifyAll();
        }

        [Test]
        [TestCaseSource(typeof(HydraRingCalculatorTestCaseProvider), nameof(HydraRingCalculatorTestCaseProvider.GetCalculatorFailingConditions), new object[]
        {
            nameof(Run_InvalidProfileSpecificCalculation_LogErrorAndActivityStateFailedAndOutputNotSet)
        })]
        public void Run_InvalidProfileSpecificCalculation_LogErrorAndActivityStateFailedAndOutputNotSet(bool endInFailure,
                                                                                                        string lastErrorFileContent)
        {
            // Setup
            var profileSpecificCalculator = new TestPipingCalculator
            {
                LastErrorFileContent = lastErrorFileContent,
                EndInFailure = true
            };

            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();

            var calculatorFactory = mocks.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreatePipingCalculator(null))
                             .IgnoreArguments()
                             .Return(profileSpecificCalculator)
                             .Repeat.Once();
            calculatorFactory.Expect(cf => cf.CreatePipingCalculator(null))
                             .IgnoreArguments()
                             .Return(new TestPipingCalculator())
                             .Repeat.Once();
            mocks.ReplayAll();

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);

            DataImportHelper.ImportHydraulicBoundaryDatabase(assessmentSection, validFilePath);

            TestPipingFailureMechanism failureMechanism = TestPipingFailureMechanism.GetFailureMechanismWithSurfaceLinesAndStochasticSoilModels();
            var calculation = ProbabilisticPipingCalculationTestFactory.CreateCalculationWithValidInput<TestProbabilisticPipingCalculation>(
                assessmentSection.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001));

            calculation.Attach(observer);

            CalculatableActivity activity = PipingCalculationActivityFactory.CreateProbabilisticPipingCalculationActivity(
                calculation, failureMechanism, assessmentSection);

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                void Call() => activity.Run();

                // Assert
                TestHelper.AssertLogMessages(Call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(7, msgs.Length);
                    Assert.AreEqual($"Uitvoeren van berekening '{calculation.Name}' is gestart.", msgs[0]);
                    CalculationServiceTestHelper.AssertValidationStartMessage(msgs[1]);
                    CalculationServiceTestHelper.AssertValidationEndMessage(msgs[2]);
                    CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[3]);
                    string errorReportText = lastErrorFileContent != null
                                                 ? $"Bekijk het foutrapport door op details te klikken.{Environment.NewLine}{lastErrorFileContent}"
                                                 : "Er is geen foutrapport beschikbaar.";
                    Assert.AreEqual($"De piping sterkte berekening voor doorsnede '{calculation.Name}' is mislukt. {errorReportText}", msgs[4]);
                    StringAssert.StartsWith("De piping sterkte berekening voor doorsnede is uitgevoerd op de tijdelijke locatie", msgs[5]);
                    CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[6]);
                });
                Assert.AreEqual(ActivityState.Failed, activity.State);
                Assert.IsNull(calculation.Output);
            }

            mocks.VerifyAll(); // No observers notified
        }

        [Test]
        [TestCaseSource(typeof(HydraRingCalculatorTestCaseProvider), nameof(HydraRingCalculatorTestCaseProvider.GetCalculatorFailingConditions), new object[]
        {
            nameof(Run_InvalidSectionSpecificCalculation_LogErrorAndActivityStateFailedAndOutputNotSet)
        })]
        public void Run_InvalidSectionSpecificCalculation_LogErrorAndActivityStateFailedAndOutputNotSet(bool endInFailure,
                                                                                                        string lastErrorFileContent)
        {
            // Setup
            var sectionSpecificCalculator = new TestPipingCalculator
            {
                LastErrorFileContent = lastErrorFileContent,
                EndInFailure = true
            };

            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();

            var calculatorFactory = mocks.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreatePipingCalculator(null))
                             .IgnoreArguments()
                             .Return(new TestPipingCalculator())
                             .Repeat.Once();
            calculatorFactory.Expect(cf => cf.CreatePipingCalculator(null))
                             .IgnoreArguments()
                             .Return(sectionSpecificCalculator)
                             .Repeat.Once();
            mocks.ReplayAll();

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);

            DataImportHelper.ImportHydraulicBoundaryDatabase(assessmentSection, validFilePath);

            TestPipingFailureMechanism failureMechanism = TestPipingFailureMechanism.GetFailureMechanismWithSurfaceLinesAndStochasticSoilModels();
            var calculation = ProbabilisticPipingCalculationTestFactory.CreateCalculationWithValidInput<TestProbabilisticPipingCalculation>(
                assessmentSection.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001));

            calculation.Attach(observer);

            CalculatableActivity activity = PipingCalculationActivityFactory.CreateProbabilisticPipingCalculationActivity(
                calculation, failureMechanism, assessmentSection);

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                void Call() => activity.Run();

                // Assert
                TestHelper.AssertLogMessages(Call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(8, msgs.Length);
                    Assert.AreEqual($"Uitvoeren van berekening '{calculation.Name}' is gestart.", msgs[0]);
                    CalculationServiceTestHelper.AssertValidationStartMessage(msgs[1]);
                    CalculationServiceTestHelper.AssertValidationEndMessage(msgs[2]);
                    CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[3]);
                    StringAssert.StartsWith("De piping sterkte berekening voor doorsnede is uitgevoerd op de tijdelijke locatie", msgs[4]);
                    string errorReportText = lastErrorFileContent != null
                                                 ? $"Bekijk het foutrapport door op details te klikken.{Environment.NewLine}{lastErrorFileContent}"
                                                 : "Er is geen foutrapport beschikbaar.";
                    Assert.AreEqual($"De piping sterkte berekening voor vak '{calculation.Name}' is mislukt. {errorReportText}", msgs[5]);
                    StringAssert.StartsWith("De piping sterkte berekening voor vak is uitgevoerd op de tijdelijke locatie", msgs[6]);
                    CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[7]);
                });
                Assert.AreEqual(ActivityState.Failed, activity.State);
                Assert.IsNull(calculation.Output);
            }

            mocks.VerifyAll(); // No observers notified
        }

        [Test]
        public void Run_ValidCalculation_InputPropertiesCorrectlySentToService()
        {
            // Setup
            var mocks = new MockRepository();
            var profileSpecificCalculator = new TestPipingCalculator();
            var sectionSpecificCalculator = new TestPipingCalculator();
            var calculatorFactory = mocks.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreatePipingCalculator(null))
                             .IgnoreArguments()
                             .Return(profileSpecificCalculator)
                             .Repeat.Once();
            calculatorFactory.Expect(cf => cf.CreatePipingCalculator(null))
                             .IgnoreArguments()
                             .Return(sectionSpecificCalculator)
                             .Repeat.Once();
            mocks.ReplayAll();

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);

            DataImportHelper.ImportHydraulicBoundaryDatabase(assessmentSection, validFilePath);

            TestPipingFailureMechanism failureMechanism = TestPipingFailureMechanism.GetFailureMechanismWithSurfaceLinesAndStochasticSoilModels();
            var calculation = ProbabilisticPipingCalculationTestFactory.CreateCalculationWithValidInput<TestProbabilisticPipingCalculation>(
                assessmentSection.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001));

            calculation.InputParameters.HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001);

            CalculatableActivity activity = PipingCalculationActivityFactory.CreateProbabilisticPipingCalculationActivity(
                calculation, failureMechanism, assessmentSection);

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                activity.Run();

                // Assert
                PipingCalculationInput[] profileSpecificInputs = profileSpecificCalculator.ReceivedInputs.ToArray();
                PipingCalculationInput[] sectionSpecificInputs = sectionSpecificCalculator.ReceivedInputs.ToArray();

                Assert.AreEqual(1, profileSpecificInputs.Length);
                Assert.AreEqual(1, sectionSpecificInputs.Length);

                double sectionLength = failureMechanism.Sections.Single(
                    s => calculation.IsSurfaceLineIntersectionWithReferenceLineInSection(
                        Math2D.ConvertPointsToLineSegments(s.Points))).Length;

                AssertCalculatorInput(failureMechanism.GeneralInput, calculation.InputParameters, 0, profileSpecificInputs[0]);
                AssertCalculatorInput(failureMechanism.GeneralInput, calculation.InputParameters, sectionLength, sectionSpecificInputs[0]);
            }

            mocks.VerifyAll();
        }

        private static void AssertCalculatorInput(GeneralPipingInput generalInput, ProbabilisticPipingInput input, double sectionLength, PipingCalculationInput actualInput)
        {
            LogNormalDistribution effectiveThicknessCoverageLayer = DerivedPipingInput.GetEffectiveThicknessCoverageLayer(input, generalInput);
            LogNormalDistribution saturatedVolumicWeightOfCoverageLayer = DerivedPipingInput.GetSaturatedVolumicWeightOfCoverageLayer(input);
            VariationCoefficientLogNormalDistribution seepageLength = DerivedPipingInput.GetSeepageLength(input);
            LogNormalDistribution thicknessAquiferLayer = DerivedPipingInput.GetThicknessAquiferLayer(input);
            VariationCoefficientLogNormalDistribution darcyPermeability = DerivedPipingInput.GetDarcyPermeability(input);
            VariationCoefficientLogNormalDistribution diameterD70 = DerivedPipingInput.GetDiameterD70(input);

            var expectedInput = new PipingCalculationInput(
                1300001,
                sectionLength,
                input.PhreaticLevelExit.Mean, input.PhreaticLevelExit.StandardDeviation,
                generalInput.WaterVolumetricWeight,
                effectiveThicknessCoverageLayer.Mean, effectiveThicknessCoverageLayer.StandardDeviation,
                saturatedVolumicWeightOfCoverageLayer.Mean, saturatedVolumicWeightOfCoverageLayer.StandardDeviation,
                saturatedVolumicWeightOfCoverageLayer.Shift,
                generalInput.UpliftModelFactor.Mean, generalInput.UpliftModelFactor.StandardDeviation,
                input.DampingFactorExit.Mean, input.DampingFactorExit.StandardDeviation,
                seepageLength.Mean, seepageLength.CoefficientOfVariation,
                thicknessAquiferLayer.Mean, thicknessAquiferLayer.StandardDeviation,
                generalInput.SandParticlesVolumicWeight,
                generalInput.SellmeijerModelFactor.Mean, generalInput.SellmeijerModelFactor.StandardDeviation,
                generalInput.BeddingAngle,
                generalInput.WhitesDragCoefficient,
                generalInput.WaterKinematicViscosity,
                darcyPermeability.Mean, darcyPermeability.CoefficientOfVariation,
                diameterD70.Mean, diameterD70.CoefficientOfVariation,
                generalInput.Gravity,
                generalInput.CriticalHeaveGradient.Mean, generalInput.CriticalHeaveGradient.StandardDeviation);

            HydraRingDataEqualityHelper.AreEqual(expectedInput, actualInput);
        }
    }
}