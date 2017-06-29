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
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.IO.FileImporters;
using Ringtoets.Common.Service.TestUtil;
using Ringtoets.HeightStructures.Data;
using Ringtoets.HeightStructures.Data.TestUtil;
using Ringtoets.HeightStructures.Service;
using Ringtoets.HydraRing.Calculation.Calculator.Factory;
using Ringtoets.HydraRing.Calculation.TestUtil.Calculator;
using Ringtoets.Integration.Data;

namespace Ringtoets.HeightStructures.Integration.Test
{
    [TestFixture]
    public class HeightStructuresCalculationActivityIntegrationTest
    {
        private static readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Integration.Service, "HydraRingCalculation");
        private static readonly string validFilePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");

        [Test]
        public void Run_CalculationInvalidInput_LogValidationStartAndEndWithError()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike)
            {
                HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
                {
                    FilePath = Path.Combine(testDataPath, "notexisting.sqlite")
                }
            };

            var failureMechanism = new HeightStructuresFailureMechanism();
            failureMechanism.AddSection(new FailureMechanismSection("test section", new[]
            {
                new Point2D(0, 0),
                new Point2D(1, 1)
            }));

            var calculation = new StructuresCalculation<HeightStructuresInput>();

            var activity = new HeightStructuresCalculationActivity(calculation, "", failureMechanism, assessmentSection);

            // Call
            Action call = () => activity.Run();

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(4, msgs.Length);
                Assert.AreEqual($"Uitvoeren van berekening '{calculation.Name}' is gestart.", msgs[0]);
                CalculationServiceTestHelper.AssertValidationStartMessage(calculation.Name, msgs[1]);
                StringAssert.StartsWith("Validatie mislukt: Fout bij het lezen van bestand", msgs[2]);
                CalculationServiceTestHelper.AssertValidationEndMessage(calculation.Name, msgs[3]);
            });
            Assert.AreEqual(ActivityState.Failed, activity.State);
        }

        [Test]
        public void Run_ValidCalculation_PerformValidationAndCalculationAndLogStartAndEnd()
        {
            // Setup
            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateStructuresOvertoppingCalculator(testDataPath)).Return(new TestStructuresOvertoppingCalculator());
            mockRepository.ReplayAll();

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);

            using (var importer = new HydraulicBoundaryDatabaseImporter())
            {
                importer.Import(assessmentSection, validFilePath);
            }

            var failureMechanism = new HeightStructuresFailureMechanism();
            failureMechanism.AddSection(new FailureMechanismSection("test section", new[]
            {
                new Point2D(0, 0),
                new Point2D(1, 1)
            }));

            var calculation = new TestHeightStructuresCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001)
                }
            };

            var activity = new HeightStructuresCalculationActivity(calculation, validFilePath, failureMechanism, assessmentSection);
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
                    CalculationServiceTestHelper.AssertValidationStartMessage(calculation.Name, msgs[1]);
                    CalculationServiceTestHelper.AssertValidationEndMessage(calculation.Name, msgs[2]);
                    CalculationServiceTestHelper.AssertCalculationStartMessage(calculation.Name, msgs[3]);
                    StringAssert.StartsWith("Hoogte kunstwerk berekening is uitgevoerd op de tijdelijke locatie", msgs[4]);
                    CalculationServiceTestHelper.AssertCalculationEndMessage(calculation.Name, msgs[5]);
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
        public void Run_InvalidCalculationAndRan_PerformValidationAndCalculationAndActivityStateFailed(bool endInFailure, string lastErrorFileContent)
        {
            // Setup
            var calculator = new TestStructuresOvertoppingCalculator
            {
                EndInFailure = endInFailure,
                LastErrorFileContent = lastErrorFileContent
            };

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateStructuresOvertoppingCalculator(testDataPath)).Return(calculator);
            mockRepository.ReplayAll();

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);

            using (var importer = new HydraulicBoundaryDatabaseImporter())
            {
                importer.Import(assessmentSection, validFilePath);
            }

            var failureMechanism = new HeightStructuresFailureMechanism();
            failureMechanism.AddSection(new FailureMechanismSection("test section", new[]
            {
                new Point2D(0, 0),
                new Point2D(1, 1)
            }));

            var calculation = new TestHeightStructuresCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "test", 1, 1)
                }
            };

            var activity = new HeightStructuresCalculationActivity(calculation, validFilePath, failureMechanism, assessmentSection);

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
            var observerMock = mockRepository.StrictMock<IObserver>();
            observerMock.Expect(o => o.UpdateObserver());

            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateStructuresOvertoppingCalculator(testDataPath)).Return(new TestStructuresOvertoppingCalculator());
            mockRepository.ReplayAll();

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);

            using (var importer = new HydraulicBoundaryDatabaseImporter())
            {
                importer.Import(assessmentSection, validFilePath);
            }

            var failureMechanism = new HeightStructuresFailureMechanism();
            failureMechanism.AddSection(new FailureMechanismSection("test section", new[]
            {
                new Point2D(0, 0),
                new Point2D(1, 1)
            }));

            var calculation = new TestHeightStructuresCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001)
                }
            };

            calculation.Attach(observerMock);

            var activity = new HeightStructuresCalculationActivity(calculation, validFilePath, failureMechanism, assessmentSection);

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
        public void Finish_InvalidCalculationAndRan_DoesNotSetOutputAndNotifyObserversOfCalculation(bool endInFailure, string lastErrorFileContent)
        {
            // Setup
            var mockRepository = new MockRepository();
            var observerMock = mockRepository.StrictMock<IObserver>();
            observerMock.Expect(o => o.UpdateObserver());

            var calculator = new TestStructuresOvertoppingCalculator
            {
                EndInFailure = endInFailure,
                LastErrorFileContent = lastErrorFileContent
            };
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateStructuresOvertoppingCalculator(testDataPath)).Return(calculator);
            mockRepository.ReplayAll();

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);

            using (var importer = new HydraulicBoundaryDatabaseImporter())
            {
                importer.Import(assessmentSection, validFilePath);
            }

            var failureMechanism = new HeightStructuresFailureMechanism();
            failureMechanism.AddSection(new FailureMechanismSection("test section", new[]
            {
                new Point2D(0, 0),
                new Point2D(1, 1)
            }));

            var calculation = new StructuresCalculation<HeightStructuresInput>
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "test", 1, 1),
                    Structure = new TestHeightStructure()
                }
            };

            calculation.Attach(observerMock);

            var activity = new HeightStructuresCalculationActivity(calculation, validFilePath, failureMechanism, assessmentSection);

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
    }
}