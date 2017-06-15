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
using Core.Common.Base.Geometry;
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

        [Test]
        public void Run_ValidCalculation_PerformCalculationAndLogStartAndEnd()
        {
            // Setup
            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateDunesBoundaryConditionsCalculator(testDataPath))
                             .Return(new TestDunesBoundaryConditionsCalculator());
            mockRepository.ReplayAll();

            var duneLocation = new DuneLocation(1300001, "test", new Point2D(0, 0),
                                                new DuneLocation.ConstructionProperties
                                                {
                                                    CoastalAreaId = 3,
                                                    Offset = 0,
                                                    Orientation = 0,
                                                    D50 = 0.000007
                                                });
            var activity = new DuneErosionBoundaryCalculationActivity(duneLocation,
                                                                      validFilePath,
                                                                      1.0 / 30000);

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                Action call = () => activity.Run();

                // Assert
                string calculationName = $"Hydraulische belasting berekenen voor locatie '{duneLocation.Name}'";
                TestHelper.AssertLogMessages(call,
                                             messages =>
                                             {
                                                 string[] msgs = messages.ToArray();
                                                 Assert.AreEqual(6, msgs.Length);

                                                 CalculationServiceTestHelper.AssertValidationStartMessage(calculationName, msgs[0]);
                                                 CalculationServiceTestHelper.AssertValidationEndMessage(calculationName, msgs[1]);
                                                 CalculationServiceTestHelper.AssertCalculationStartMessage(calculationName, msgs[2]);
                                                 Assert.AreEqual($"Hydraulische randvoorwaarden berekening voor locatie '{duneLocation.Name}' is niet geconvergeerd.", msgs[3]);
                                                 StringAssert.StartsWith("Hydraulische randvoorwaarden berekening is uitgevoerd op de tijdelijke locatie", msgs[4]);
                                                 CalculationServiceTestHelper.AssertCalculationEndMessage(calculationName, msgs[5]);
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
            calculatorFactory.Expect(cf => cf.CreateDunesBoundaryConditionsCalculator(testDataPath))
                             .Return(calculator);
            mockRepository.ReplayAll();

            var duneLocation = new DuneLocation(1300001, "test", new Point2D(0, 0),
                                                new DuneLocation.ConstructionProperties
                                                {
                                                    CoastalAreaId = 3,
                                                    Offset = 0,
                                                    Orientation = 0,
                                                    D50 = 0.000007
                                                });
            var activity = new DuneErosionBoundaryCalculationActivity(duneLocation,
                                                                      validFilePath,
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
            var duneLocation = new TestDuneLocation
            {
                Output = initialOutput
            };
            var activity = new DuneErosionBoundaryCalculationActivity(duneLocation,
                                                                      validFilePath,
                                                                      1.0 / 30000);

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                activity.Run();

                // Assert
                Assert.AreEqual(ActivityState.Skipped, activity.State);
                Assert.AreSame(initialOutput, duneLocation.Output);
            }

            mockRepository.VerifyAll();
        }
    }
}