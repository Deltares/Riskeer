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
using Core.Common.Base.Service;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.ClosingStructures.Data;
using Ringtoets.ClosingStructures.Data.TestUtil;
using Ringtoets.ClosingStructures.Service;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Service;
using Ringtoets.Common.Service.TestUtil;
using Ringtoets.HydraRing.Calculation.TestUtil.Calculator;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.TestUtil;
using Riskeer.HydraRing.Calculation.Calculator.Factory;
using Riskeer.HydraRing.Calculation.Data.Input;
using Riskeer.HydraRing.Calculation.Data.Input.Structures;

namespace Ringtoets.ClosingStructures.Integration.Test
{
    [TestFixture]
    public class ClosingStructuresCalculationActivityIntegrationTest
    {
        private static readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Integration.Service, "HydraRingCalculation");
        private static readonly string validFilePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");
        private static readonly string validPreprocessorDirectory = TestHelper.GetScratchPadPath();

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

            var failureMechanism = new ClosingStructuresFailureMechanism();
            var calculation = new StructuresCalculation<ClosingStructuresInput>();

            CalculatableActivity activity = ClosingStructuresCalculationActivityFactory.CreateCalculationActivity(calculation,
                                                                                                                  failureMechanism,
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
            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateStructuresCalculator<StructuresClosureCalculationInput>(null))
                             .IgnoreArguments()
                             .Return(new TestStructuresCalculator<StructuresClosureCalculationInput>());
            mockRepository.ReplayAll();

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);

            DataImportHelper.ImportHydraulicBoundaryDatabase(assessmentSection, validFilePath);

            var failureMechanism = new ClosingStructuresFailureMechanism();
            var calculation = new TestClosingStructuresCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001)
                }
            };

            CalculatableActivity activity = ClosingStructuresCalculationActivityFactory.CreateCalculationActivity(calculation,
                                                                                                                  failureMechanism,
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
                    StringAssert.StartsWith("Betrouwbaarheid sluiting kunstwerk berekening is uitgevoerd op de tijdelijke locatie", msgs[4]);
                    CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[5]);
                });
                Assert.AreEqual(ActivityState.Executed, activity.State);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        [TestCaseSource(typeof(HydraRingCalculatorTestCaseProvider), nameof(HydraRingCalculatorTestCaseProvider.GetCalculatorFailingConditions), new object[]
        {
            nameof(Run_InvalidCalculationAndRan_PerformValidationAndCalculationAndActivityStateFailed)
        })]
        public void Run_InvalidCalculationAndRan_PerformValidationAndCalculationAndActivityStateFailed(bool endInFailure,
                                                                                                       string lastErrorFileContent)
        {
            // Setup
            var calculator = new TestStructuresCalculator<StructuresClosureCalculationInput>
            {
                EndInFailure = endInFailure,
                LastErrorFileContent = lastErrorFileContent
            };

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateStructuresCalculator<StructuresClosureCalculationInput>(null))
                             .IgnoreArguments()
                             .Return(calculator);
            mockRepository.ReplayAll();

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);

            DataImportHelper.ImportHydraulicBoundaryDatabase(assessmentSection, validFilePath);

            var failureMechanism = new ClosingStructuresFailureMechanism();
            var calculation = new TestClosingStructuresCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "test", 1, 1)
                }
            };

            CalculatableActivity activity = ClosingStructuresCalculationActivityFactory.CreateCalculationActivity(calculation,
                                                                                                                  failureMechanism,
                                                                                                                  assessmentSection);

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                activity.Run();

                // Assert
                Assert.AreEqual(ActivityState.Failed, activity.State);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        public void Finish_ValidCalculationAndRan_SetsOutputAndNotifyObserversOfCalculation()
        {
            // Setup
            var mockRepository = new MockRepository();
            var observer = mockRepository.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());

            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateStructuresCalculator<StructuresClosureCalculationInput>(null))
                             .IgnoreArguments()
                             .Return(new TestStructuresCalculator<StructuresClosureCalculationInput>());
            mockRepository.ReplayAll();

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);

            DataImportHelper.ImportHydraulicBoundaryDatabase(assessmentSection, validFilePath);

            var failureMechanism = new ClosingStructuresFailureMechanism();
            var calculation = new TestClosingStructuresCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001)
                }
            };

            calculation.Attach(observer);

            CalculatableActivity activity = ClosingStructuresCalculationActivityFactory.CreateCalculationActivity(calculation,
                                                                                                                  failureMechanism,
                                                                                                                  assessmentSection);

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                activity.Run();
            }

            // Call
            activity.Finish();

            // Assert
            Assert.IsNotNull(calculation.Output);
            mockRepository.VerifyAll();
        }

        [Test]
        [TestCaseSource(typeof(HydraRingCalculatorTestCaseProvider), nameof(HydraRingCalculatorTestCaseProvider.GetCalculatorFailingConditions), new object[]
        {
            nameof(Finish_InvalidCalculationAndRan_DoesNotSetOutputAndNotifyObserversOfCalculation)
        })]
        public void Finish_InvalidCalculationAndRan_DoesNotSetOutputAndNotifyObserversOfCalculation(bool endInFailure,
                                                                                                    string lastErrorFileContent)
        {
            // Setup
            var mockRepository = new MockRepository();
            var observer = mockRepository.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());

            var calculator = new TestStructuresCalculator<StructuresClosureCalculationInput>
            {
                EndInFailure = endInFailure,
                LastErrorFileContent = lastErrorFileContent
            };
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateStructuresCalculator<StructuresClosureCalculationInput>(null))
                             .IgnoreArguments()
                             .Return(calculator);
            mockRepository.ReplayAll();

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);

            DataImportHelper.ImportHydraulicBoundaryDatabase(assessmentSection, validFilePath);

            var failureMechanism = new ClosingStructuresFailureMechanism();
            var calculation = new StructuresCalculation<ClosingStructuresInput>
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "test", 1, 1),
                    Structure = new TestClosingStructure()
                }
            };

            calculation.Attach(observer);

            CalculatableActivity activity = ClosingStructuresCalculationActivityFactory.CreateCalculationActivity(calculation,
                                                                                                                  failureMechanism,
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
        public void Run_HydraulicBoundaryDatabaseWithCanUsePreprocessorFalse_ExpectedPreprocessorDirectorySetToCalculator()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike)
            {
                HydraulicBoundaryDatabase =
                {
                    FilePath = validFilePath
                }
            };
            HydraulicBoundaryDatabaseTestHelper.SetHydraulicBoundaryLocationConfigurationSettings(assessmentSection.HydraulicBoundaryDatabase);


            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateStructuresCalculator<StructuresClosureCalculationInput>(
                                         Arg<HydraRingCalculationSettings>.Is.NotNull))
                             .WhenCalled(invocation =>
                             {
                                 HydraRingCalculationSettingsTestHelper.AssertHydraRingCalculationSettings(
                                     HydraulicBoundaryCalculationSettingsFactory.CreateSettings(assessmentSection.HydraulicBoundaryDatabase),
                                     (HydraRingCalculationSettings) invocation.Arguments[0]);
                             })
                             .Return(new TestStructuresCalculator<StructuresClosureCalculationInput>());
            mockRepository.ReplayAll();

            var failureMechanism = new ClosingStructuresFailureMechanism();
            var calculation = new TestClosingStructuresCalculation();

            CalculatableActivity activity = ClosingStructuresCalculationActivityFactory.CreateCalculationActivity(calculation,
                                                                                                                  failureMechanism,
                                                                                                                  assessmentSection);

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                activity.Run();
            }

            // Assert
            mockRepository.VerifyAll();
        }

        [Test]
        public void Run_HydraulicBoundaryDatabaseWithUsePreprocessorTrue_ExpectedPreprocessorDirectorySetToCalculator()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike)
            {
                HydraulicBoundaryDatabase =
                {
                    FilePath = validFilePath,
                    CanUsePreprocessor = true,
                    UsePreprocessor = true,
                    PreprocessorDirectory = validPreprocessorDirectory
                }
            };
            HydraulicBoundaryDatabaseTestHelper.SetHydraulicBoundaryLocationConfigurationSettings(assessmentSection.HydraulicBoundaryDatabase);

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateStructuresCalculator<StructuresClosureCalculationInput>(
                                         Arg<HydraRingCalculationSettings>.Is.NotNull))
                             .WhenCalled(invocation =>
                             {
                                 HydraRingCalculationSettingsTestHelper.AssertHydraRingCalculationSettings(
                                     HydraulicBoundaryCalculationSettingsFactory.CreateSettings(assessmentSection.HydraulicBoundaryDatabase),
                                     (HydraRingCalculationSettings) invocation.Arguments[0]);
                             })
                             .Return(new TestStructuresCalculator<StructuresClosureCalculationInput>());
            mockRepository.ReplayAll();

            var failureMechanism = new ClosingStructuresFailureMechanism();
            var calculation = new TestClosingStructuresCalculation();

            CalculatableActivity activity = ClosingStructuresCalculationActivityFactory.CreateCalculationActivity(calculation,
                                                                                                                  failureMechanism,
                                                                                                                  assessmentSection);

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                activity.Run();
            }

            // Assert
            mockRepository.VerifyAll();
        }

        [Test]
        public void Run_HydraulicBoundaryDatabaseWithUsePreprocessorFalse_ExpectedPreprocessorDirectorySetToCalculator()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike)
            {
                HydraulicBoundaryDatabase =
                {
                    FilePath = validFilePath,
                    CanUsePreprocessor = true,
                    UsePreprocessor = false,
                    PreprocessorDirectory = "NonExistingPreprocessorDirectory"
                }
            };
            HydraulicBoundaryDatabaseTestHelper.SetHydraulicBoundaryLocationConfigurationSettings(assessmentSection.HydraulicBoundaryDatabase);

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateStructuresCalculator<StructuresClosureCalculationInput>(
                                         Arg<HydraRingCalculationSettings>.Is.NotNull))
                             .WhenCalled(invocation =>
                             {
                                 HydraRingCalculationSettingsTestHelper.AssertHydraRingCalculationSettings(
                                     HydraulicBoundaryCalculationSettingsFactory.CreateSettings(assessmentSection.HydraulicBoundaryDatabase),
                                     (HydraRingCalculationSettings) invocation.Arguments[0]);
                             })
                             .Return(new TestStructuresCalculator<StructuresClosureCalculationInput>());
            mockRepository.ReplayAll();

            var failureMechanism = new ClosingStructuresFailureMechanism();
            var calculation = new TestClosingStructuresCalculation();

            CalculatableActivity activity = ClosingStructuresCalculationActivityFactory.CreateCalculationActivity(calculation,
                                                                                                                  failureMechanism,
                                                                                                                  assessmentSection);

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                activity.Run();
            }

            // Assert
            mockRepository.VerifyAll();
        }
    }
}