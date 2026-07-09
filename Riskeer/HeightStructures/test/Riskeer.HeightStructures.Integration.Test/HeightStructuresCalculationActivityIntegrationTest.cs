// Copyright (C) Stichting Deltares and State of the Netherlands 2026. All rights reserved.
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
using Core.Common.Base.Service;
using Core.Common.TestUtil;
using NSubstitute;
using NUnit.Framework;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.Structures;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Service;
using Riskeer.Common.Service.TestUtil;
using Riskeer.HeightStructures.Data;
using Riskeer.HeightStructures.Data.TestUtil;
using Riskeer.HeightStructures.Service;
using Riskeer.HydraRing.Calculation.Calculator.Factory;
using Riskeer.HydraRing.Calculation.Data.Input;
using Riskeer.HydraRing.Calculation.Data.Input.Structures;
using Riskeer.HydraRing.Calculation.TestUtil.Calculator;
using Riskeer.Integration.Data;
using Riskeer.Integration.TestUtil;

namespace Riskeer.HeightStructures.Integration.Test
{
    [TestFixture]
    public class HeightStructuresCalculationActivityIntegrationTest
    {
        private static readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Integration.Service, "HydraRingCalculation");
        private static readonly string validHlcdFilePath = Path.Combine(testDataPath, "hlcd.sqlite");
        private static readonly string validHrdFilePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");
        private static readonly string validHrdFileVersion = "Dutch coast South19-11-2015 12:0013";

        [Test]
        public void Run_CalculationInvalidInput_LogValidationStartAndEndWithError()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            var failureMechanism = new HeightStructuresFailureMechanism();
            var calculation = new StructuresCalculation<HeightStructuresInput>();

            CalculatableActivity activity = HeightStructuresCalculationActivityFactory.CreateCalculationActivity(calculation,
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
                Assert.AreEqual("Er is geen hydraulische belastingenlocatie geselecteerd.", msgs[2]);
                CalculationServiceTestHelper.AssertValidationEndMessage(msgs[3]);
            });
            Assert.AreEqual(ActivityState.Failed, activity.State);
        }

        [Test]
        public void Run_ValidCalculation_PerformValidationAndCalculationAndLogStartAndEnd()
        {
            // Setup
            var calculatorFactory = Substitute.For<IHydraRingCalculatorFactory>();
            calculatorFactory.CreateStructuresCalculator<StructuresOvertoppingCalculationInput>(Arg.Any<HydraRingCalculationSettings>())
                             .Returns(new TestStructuresCalculator<StructuresOvertoppingCalculationInput>());

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);

            DataImportHelper.ImportHydraulicBoundaryData(assessmentSection, validHlcdFilePath, validHrdFilePath);

            var failureMechanism = new HeightStructuresFailureMechanism();
            var calculation = new TestHeightStructuresCalculationScenario
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryData.GetLocations().First(hbl => hbl.Id == 1300001)
                }
            };

            CalculatableActivity activity = HeightStructuresCalculationActivityFactory.CreateCalculationActivity(calculation,
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
                    StringAssert.StartsWith("Hoogte kunstwerk berekening is uitgevoerd op de tijdelijke locatie", msgs[4]);
                    CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[5]);
                });
                Assert.AreEqual(ActivityState.Executed, activity.State);
            }

            calculatorFactory.Received(1).CreateStructuresCalculator<StructuresOvertoppingCalculationInput>(Arg.Any<HydraRingCalculationSettings>());
        }

        [Test]
        [TestCaseSource(typeof(HydraRingCalculatorTestCaseProvider), nameof(HydraRingCalculatorTestCaseProvider.GetCalculatorFailingConditions), new object[]
        {
            nameof(Run_InvalidCalculationAndRan_PerformValidationAndCalculationAndActivityStateFailed)
        })]
        public void Run_InvalidCalculationAndRan_PerformValidationAndCalculationAndActivityStateFailed(bool endInFailure, string lastErrorFileContent)
        {
            // Setup
            var calculator = new TestStructuresCalculator<StructuresOvertoppingCalculationInput>
            {
                EndInFailure = endInFailure,
                LastErrorFileContent = lastErrorFileContent
            };

            var calculatorFactory = Substitute.For<IHydraRingCalculatorFactory>();
            calculatorFactory.CreateStructuresCalculator<StructuresOvertoppingCalculationInput>(Arg.Any<HydraRingCalculationSettings>())
                             .Returns(calculator);

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);

            DataImportHelper.ImportHydraulicBoundaryData(assessmentSection, validHlcdFilePath, validHrdFilePath);

            var failureMechanism = new HeightStructuresFailureMechanism();
            var calculation = new TestHeightStructuresCalculationScenario
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryData.GetLocations().First()
                }
            };

            CalculatableActivity activity = HeightStructuresCalculationActivityFactory.CreateCalculationActivity(calculation,
                                                                                                                 failureMechanism,
                                                                                                                 assessmentSection);
            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                activity.Run();

                // Assert
                Assert.AreEqual(ActivityState.Failed, activity.State);
            }

            calculatorFactory.Received(1).CreateStructuresCalculator<StructuresOvertoppingCalculationInput>(Arg.Any<HydraRingCalculationSettings>());
        }

        [Test]
        public void Finish_ValidCalculationAndRan_SetsOutputAndNotifyObserversOfCalculation()
        {
            // Setup
            var observer = Substitute.For<IObserver>();
            var calculatorFactory = Substitute.For<IHydraRingCalculatorFactory>();
            calculatorFactory.CreateStructuresCalculator<StructuresOvertoppingCalculationInput>(Arg.Any<HydraRingCalculationSettings>())
                             .Returns(new TestStructuresCalculator<StructuresOvertoppingCalculationInput>());

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);

            DataImportHelper.ImportHydraulicBoundaryData(assessmentSection, validHlcdFilePath, validHrdFilePath);

            var failureMechanism = new HeightStructuresFailureMechanism();
            var calculation = new TestHeightStructuresCalculationScenario
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryData.GetLocations().First(hbl => hbl.Id == 1300001)
                }
            };

            calculation.Attach(observer);

            CalculatableActivity activity = HeightStructuresCalculationActivityFactory.CreateCalculationActivity(calculation,
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
            observer.Received(1).UpdateObserver();
            calculatorFactory.Received(1).CreateStructuresCalculator<StructuresOvertoppingCalculationInput>(Arg.Any<HydraRingCalculationSettings>());
        }

        [Test]
        [TestCaseSource(typeof(HydraRingCalculatorTestCaseProvider), nameof(HydraRingCalculatorTestCaseProvider.GetCalculatorFailingConditions), new object[]
        {
            nameof(Finish_InvalidCalculationAndRan_DoesNotSetOutputAndNotifyObserversOfCalculation)
        })]
        public void Finish_InvalidCalculationAndRan_DoesNotSetOutputAndNotifyObserversOfCalculation(bool endInFailure, string lastErrorFileContent)
        {
            // Setup
            var observer = Substitute.For<IObserver>();

            var calculator = new TestStructuresCalculator<StructuresOvertoppingCalculationInput>
            {
                EndInFailure = endInFailure,
                LastErrorFileContent = lastErrorFileContent
            };
            var calculatorFactory = Substitute.For<IHydraRingCalculatorFactory>();
            calculatorFactory.CreateStructuresCalculator<StructuresOvertoppingCalculationInput>(Arg.Any<HydraRingCalculationSettings>())
                             .Returns(calculator);

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);

            DataImportHelper.ImportHydraulicBoundaryData(assessmentSection, validHlcdFilePath, validHrdFilePath);

            var failureMechanism = new HeightStructuresFailureMechanism();
            var calculation = new StructuresCalculation<HeightStructuresInput>
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryData.GetLocations().First(),
                    Structure = new TestHeightStructure()
                }
            };

            calculation.Attach(observer);

            CalculatableActivity activity = HeightStructuresCalculationActivityFactory.CreateCalculationActivity(calculation,
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
            observer.Received(1).UpdateObserver();
            calculatorFactory.Received(1).CreateStructuresCalculator<StructuresOvertoppingCalculationInput>(Arg.Any<HydraRingCalculationSettings>());
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Run_HydraulicBoundaryDataSet_ExpectedSettingsSetToCalculator(bool usePreprocessorClosure)
        {
            // Setup
            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike)
            {
                HydraulicBoundaryData =
                {
                    HydraulicLocationConfigurationDatabase =
                    {
                        FilePath = validHlcdFilePath
                    },
                    HydraulicBoundaryDatabases =
                    {
                        new HydraulicBoundaryDatabase
                        {
                            FilePath = validHrdFilePath,
                            Version = validHrdFileVersion,
                            UsePreprocessorClosure = usePreprocessorClosure,
                            Locations =
                            {
                                hydraulicBoundaryLocation
                            }
                        }
                    }
                }
            };

            var calculatorFactory = Substitute.For<IHydraRingCalculatorFactory>();
            calculatorFactory.CreateStructuresCalculator<StructuresOvertoppingCalculationInput>(Arg.Any<HydraRingCalculationSettings>())
                             .Returns(callInfo =>
                             {
                                 HydraRingCalculationSettingsTestHelper.AssertHydraRingCalculationSettings(
                                     HydraulicBoundaryCalculationSettingsFactory.CreateSettings(
                                         assessmentSection.HydraulicBoundaryData,
                                         hydraulicBoundaryLocation),
                                     (HydraRingCalculationSettings) callInfo[0]);

                                 return new TestStructuresCalculator<StructuresOvertoppingCalculationInput>();
                             });

            var failureMechanism = new HeightStructuresFailureMechanism();
            var calculation = new TestHeightStructuresCalculationScenario
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                }
            };

            CalculatableActivity activity = HeightStructuresCalculationActivityFactory.CreateCalculationActivity(calculation,
                                                                                                                 failureMechanism,
                                                                                                                 assessmentSection);

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                activity.Run();
            }

            // Assert
            calculatorFactory.Received(1).CreateStructuresCalculator<StructuresOvertoppingCalculationInput>(Arg.Any<HydraRingCalculationSettings>());
        }
    }
}