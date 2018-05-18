﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Core.Common.Base.Service;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Service.TestUtil;
using Ringtoets.DuneErosion.Data;
using Ringtoets.DuneErosion.Data.TestUtil;
using Ringtoets.DuneErosion.Service;
using Ringtoets.HydraRing.Calculation.Calculator.Factory;
using Ringtoets.HydraRing.Calculation.TestUtil.Calculator;

namespace Ringtoets.DuneErosion.Integration.Test
{
    [TestFixture]
    public class DuneErosionBoundaryCalculationActivityIntegrationTest
    {
        private static readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Integration.Service, "HydraRingCalculation");
        private static readonly string validFilePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");
        private static readonly string validPreprocessorDirectory = TestHelper.GetScratchPadPath();

        [Test]
        public void Run_ValidCalculation_PerformCalculationAndLogStartAndEnd()
        {
            // Setup
            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateDunesBoundaryConditionsCalculator(testDataPath, validPreprocessorDirectory))
                             .Return(new TestDunesBoundaryConditionsCalculator());
            mockRepository.ReplayAll();

            var duneLocation = new TestDuneLocation("A dune location name");
            var duneLocationCalculation = new DuneLocationCalculation(duneLocation);
            var activity = new DuneErosionBoundaryCalculationActivity(duneLocationCalculation,
                                                                      validFilePath,
                                                                      validPreprocessorDirectory,
                                                                      1.0 / 30000);

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                Action call = () => activity.Run();

                // Assert
                TestHelper.AssertLogMessages(call,
                                             messages =>
                                             {
                                                 string[] msgs = messages.ToArray();
                                                 Assert.AreEqual(7, msgs.Length);

                                                 Assert.AreEqual($"Hydraulische randvoorwaarden berekenen voor locatie '{duneLocation.Name}' is gestart.", msgs[0]);
                                                 CalculationServiceTestHelper.AssertValidationStartMessage(msgs[1]);
                                                 CalculationServiceTestHelper.AssertValidationEndMessage(msgs[2]);
                                                 CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[3]);
                                                 Assert.AreEqual($"Hydraulische randvoorwaarden berekening voor locatie '{duneLocation.Name}' is niet geconvergeerd.", msgs[4]);
                                                 StringAssert.StartsWith("Hydraulische randvoorwaarden berekening is uitgevoerd op de tijdelijke locatie", msgs[5]);
                                                 CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[6]);
                                             });
                Assert.AreEqual(ActivityState.Executed, activity.State);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        [TestCaseSource(typeof(HydraRingCalculatorTestCaseProvider), nameof(HydraRingCalculatorTestCaseProvider.GetCalculatorFailingConditions), new object[]
        {
            nameof(Run_InvalidCalculationAndRan_PerformCalculationAndActivityStateFailed)
        })]
        public void Run_InvalidCalculationAndRan_PerformCalculationAndActivityStateFailed(bool endInFailure,
                                                                                          string lastErrorFileContent)
        {
            // Setup
            var calculator = new TestDunesBoundaryConditionsCalculator
            {
                EndInFailure = endInFailure,
                LastErrorFileContent = lastErrorFileContent
            };

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateDunesBoundaryConditionsCalculator(testDataPath, validPreprocessorDirectory))
                             .Return(calculator);
            mockRepository.ReplayAll();

            var duneLocationCalculation = new DuneLocationCalculation(new TestDuneLocation());
            var activity = new DuneErosionBoundaryCalculationActivity(duneLocationCalculation,
                                                                      validFilePath,
                                                                      validPreprocessorDirectory,
                                                                      1.0 / 30000);

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
        public void Run_CalculationAlreadyPerformed_CalculationNotPerformedAndActivityStateSkipped()
        {
            // Setup
            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            mockRepository.ReplayAll();

            var initialOutput = new TestDuneLocationOutput();
            var duneLocationCalculation = new DuneLocationCalculation(new TestDuneLocation())
            {
                Output = initialOutput
            };
            var activity = new DuneErosionBoundaryCalculationActivity(duneLocationCalculation,
                                                                      validFilePath,
                                                                      validPreprocessorDirectory,
                                                                      1.0 / 30000);

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                activity.Run();

                // Assert
                Assert.AreEqual(ActivityState.Skipped, activity.State);
                Assert.AreSame(initialOutput, duneLocationCalculation.Output);
            }

            mockRepository.VerifyAll();
        }
    }
}