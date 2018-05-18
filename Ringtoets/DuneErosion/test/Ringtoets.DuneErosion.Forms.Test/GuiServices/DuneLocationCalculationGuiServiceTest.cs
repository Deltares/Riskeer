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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Service.TestUtil;
using Ringtoets.DuneErosion.Data;
using Ringtoets.DuneErosion.Data.TestUtil;
using Ringtoets.DuneErosion.Forms.GuiServices;
using Ringtoets.HydraRing.Calculation.Calculator.Factory;
using Ringtoets.HydraRing.Calculation.TestUtil.Calculator;

namespace Ringtoets.DuneErosion.Forms.Test.GuiServices
{
    [TestFixture]
    public class DuneLocationCalculationGuiServiceTest
    {
        private MockRepository mockRepository;
        private static readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO, "HydraulicBoundaryDatabaseImporter");
        private static readonly string validFilePath = Path.Combine(testDataPath, "complete.sqlite");
        private static readonly string validPreprocessorDirectory = TestHelper.GetScratchPadPath();

        [SetUp]
        public void Setup()
        {
            mockRepository = new MockRepository();
        }

        [Test]
        public void Constructor_ViewParentNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => new DuneLocationCalculationGuiService(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("viewParent", exception.ParamName);
        }

        [Test]
        public void Calculate_CalculationsNull_ThrowArgumentNullException()
        {
            // Setup
            using (var viewParent = new Form())
            {
                var guiService = new DuneLocationCalculationGuiService(viewParent);

                // Call
                TestDelegate test = () => guiService.Calculate(null,
                                                               validFilePath,
                                                               validPreprocessorDirectory,
                                                               1.0 / 30000);

                // Assert
                var exception = Assert.Throws<ArgumentNullException>(test);
                Assert.AreEqual("calculations", exception.ParamName);
            }
        }

        [Test]
        public void Calculate_ValidData_ScheduleAllLocations()
        {
            // Setup
            var duneLocationCalculations = new[]
            {
                new DuneLocationCalculation(new DuneLocation(1300001, "A", new Point2D(0, 0), new DuneLocation.ConstructionProperties
                {
                    CoastalAreaId = 0,
                    Offset = 0,
                    Orientation = 0,
                    D50 = 0.000007
                })),
                new DuneLocationCalculation(new DuneLocation(1300002, "B", new Point2D(0, 0), new DuneLocation.ConstructionProperties
                {
                    CoastalAreaId = 0,
                    Offset = 0,
                    Orientation = 0,
                    D50 = 0.000007
                }))
            };

            int nrOfCalculators = duneLocationCalculations.Length;
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateDunesBoundaryConditionsCalculator(testDataPath, validPreprocessorDirectory))
                             .Return(new TestDunesBoundaryConditionsCalculator())
                             .Repeat
                             .Times(nrOfCalculators);
            mockRepository.ReplayAll();

            using (var viewParent = new Form())
            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                var guiService = new DuneLocationCalculationGuiService(viewParent);

                // Call
                TestHelper.AssertLogMessages(() => guiService.Calculate(duneLocationCalculations,
                                                                        validFilePath,
                                                                        validPreprocessorDirectory,
                                                                        1.0 / 200),
                                             messages =>
                                             {
                                                 List<string> messageList = messages.ToList();

                                                 // Assert
                                                 Assert.AreEqual(16, messageList.Count);

                                                 Assert.AreEqual("Hydraulische randvoorwaarden berekenen voor locatie 'A' is gestart.", messageList[0]);
                                                 CalculationServiceTestHelper.AssertValidationStartMessage(messageList[1]);
                                                 CalculationServiceTestHelper.AssertValidationEndMessage(messageList[2]);
                                                 CalculationServiceTestHelper.AssertCalculationStartMessage(messageList[3]);
                                                 Assert.AreEqual("Hydraulische randvoorwaarden berekening voor locatie 'A' is niet geconvergeerd.", messageList[4]);
                                                 StringAssert.StartsWith("Hydraulische randvoorwaarden berekening is uitgevoerd op de tijdelijke locatie", messageList[5]);
                                                 CalculationServiceTestHelper.AssertCalculationEndMessage(messageList[6]);

                                                 Assert.AreEqual("Hydraulische randvoorwaarden berekenen voor locatie 'B' is gestart.", messageList[7]);
                                                 CalculationServiceTestHelper.AssertValidationStartMessage(messageList[8]);
                                                 CalculationServiceTestHelper.AssertValidationEndMessage(messageList[9]);
                                                 CalculationServiceTestHelper.AssertCalculationStartMessage(messageList[10]);
                                                 Assert.AreEqual("Hydraulische randvoorwaarden berekening voor locatie 'B' is niet geconvergeerd.", messageList[11]);
                                                 StringAssert.StartsWith("Hydraulische randvoorwaarden berekening is uitgevoerd op de tijdelijke locatie", messageList[12]);
                                                 CalculationServiceTestHelper.AssertCalculationEndMessage(messageList[13]);

                                                 Assert.AreEqual("Hydraulische randvoorwaarden berekenen voor locatie 'A' is gelukt.", messageList[14]);
                                                 Assert.AreEqual("Hydraulische randvoorwaarden berekenen voor locatie 'B' is gelukt.", messageList[15]);
                                             });
            }

            mockRepository.VerifyAll();
        }

        [Test]
        public void Calculate_HydraulicDatabaseDoesNotExist_LogsError()
        {
            // Setup
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            mockRepository.ReplayAll();

            const string databasePath = "Does not exist";
            using (var viewParent = new Form())
            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                var guiService = new DuneLocationCalculationGuiService(viewParent);

                // Call
                Action call = () => guiService.Calculate(Enumerable.Empty<DuneLocationCalculation>(),
                                                         databasePath,
                                                         validPreprocessorDirectory,
                                                         1.0 / 30000);

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(1, msgs.Length);
                    StringAssert.StartsWith("Berekeningen konden niet worden gestart. ", msgs.First());
                });
            }

            mockRepository.VerifyAll();
        }

        [Test]
        public void Calculate_ValidPathOneLocationInTheList_LogsMessages()
        {
            // Setup
            const string hydraulicLocationName = "name";

            var viewParent = mockRepository.Stub<IWin32Window>();

            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateDunesBoundaryConditionsCalculator(testDataPath, validPreprocessorDirectory)).Return(new TestDunesBoundaryConditionsCalculator());
            mockRepository.ReplayAll();

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                var guiService = new DuneLocationCalculationGuiService(viewParent);

                // Call
                Action call = () => guiService.Calculate(new []
                                                         {
                                                             new DuneLocationCalculation(new TestDuneLocation(hydraulicLocationName))
                                                         },
                                                         validFilePath,
                                                         validPreprocessorDirectory,
                                                         1.0 / 30000);

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(8, msgs.Length);

                    string calculationName = $"Hydraulische belasting berekenen voor locatie '{hydraulicLocationName}'";
                    Assert.AreEqual($"Hydraulische randvoorwaarden berekenen voor locatie '{hydraulicLocationName}' is gestart.", msgs[0]);
                    CalculationServiceTestHelper.AssertValidationStartMessage(msgs[1]);
                    CalculationServiceTestHelper.AssertValidationEndMessage(msgs[2]);
                    CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[3]);
                    Assert.AreEqual($"Hydraulische randvoorwaarden berekening voor locatie '{hydraulicLocationName}' is niet geconvergeerd.", msgs[4]);
                    StringAssert.StartsWith("Hydraulische randvoorwaarden berekening is uitgevoerd op de tijdelijke locatie", msgs[5]);
                    CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[6]);
                    StringAssert.AreNotEqualIgnoringCase($"Uitvoeren van '{calculationName}' is gelukt.", msgs[7]);
                });
            }

            mockRepository.VerifyAll();
        }
    }
}