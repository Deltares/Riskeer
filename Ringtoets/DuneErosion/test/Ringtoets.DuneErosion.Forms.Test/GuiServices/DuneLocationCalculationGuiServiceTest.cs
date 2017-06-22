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
        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO, "HydraulicBoundaryDatabaseImporter");

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
        public void Calculate_LocationsNull_ThrowArgumentNullException()
        {
            // Setup
            using (var viewParent = new Form())
            {
                var guiService = new DuneLocationCalculationGuiService(viewParent);

                // Call
                TestDelegate test = () => guiService.Calculate(null,
                                                               "path",
                                                               1.0 / 30000);

                // Assert
                var exception = Assert.Throws<ArgumentNullException>(test);
                Assert.AreEqual("locations", exception.ParamName);
            }
        }

        [Test]
        public void Calculate_ValidData_ScheduleAllLocations()
        {
            // Setup
            string validFilePath = Path.Combine(testDataPath, "complete.sqlite");

            var duneLocations = new[]
            {
                new DuneLocation(1300001, "A", new Point2D(0, 0), new DuneLocation.ConstructionProperties
                {
                    CoastalAreaId = 0,
                    Offset = 0,
                    Orientation = 0,
                    D50 = 0.000007
                }),
                new DuneLocation(1300002, "B", new Point2D(0, 0), new DuneLocation.ConstructionProperties
                {
                    CoastalAreaId = 0,
                    Offset = 0,
                    Orientation = 0,
                    D50 = 0.000007
                })
            };

            int nrOfCalculators = duneLocations.Length;
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateDunesBoundaryConditionsCalculator(testDataPath))
                             .Return(new TestDunesBoundaryConditionsCalculator())
                             .Repeat
                             .Times(nrOfCalculators);
            mockRepository.ReplayAll();

            using (var viewParent = new Form())
            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                var guiService = new DuneLocationCalculationGuiService(viewParent);

                // Call
                TestHelper.AssertLogMessages(() => guiService.Calculate(duneLocations,
                                                                        validFilePath,
                                                                        1.0 / 200),
                                             messages =>
                                             {
                                                 List<string> messageList = messages.ToList();

                                                 // Assert
                                                 Assert.AreEqual(14, messageList.Count);

                                                 const string calculationNameA = "Hydraulische belasting berekenen voor locatie 'A'";
                                                 CalculationServiceTestHelper.AssertValidationStartMessage(calculationNameA, messageList[0]);
                                                 CalculationServiceTestHelper.AssertValidationEndMessage(calculationNameA, messageList[1]);
                                                 CalculationServiceTestHelper.AssertCalculationStartMessage(calculationNameA, messageList[2]);
                                                 Assert.AreEqual("Hydraulische randvoorwaarden berekening voor locatie 'A' is niet geconvergeerd.", messageList[3]);
                                                 StringAssert.StartsWith("Hydraulische randvoorwaarden berekening is uitgevoerd op de tijdelijke locatie", messageList[4]);
                                                 CalculationServiceTestHelper.AssertCalculationEndMessage(calculationNameA, messageList[5]);

                                                 const string calculationNameB = "Hydraulische belasting berekenen voor locatie 'B'";
                                                 CalculationServiceTestHelper.AssertValidationStartMessage(calculationNameB, messageList[6]);
                                                 CalculationServiceTestHelper.AssertValidationEndMessage(calculationNameB, messageList[7]);
                                                 CalculationServiceTestHelper.AssertCalculationStartMessage(calculationNameB, messageList[8]);
                                                 Assert.AreEqual("Hydraulische randvoorwaarden berekening voor locatie 'B' is niet geconvergeerd.", messageList[9]);
                                                 StringAssert.StartsWith("Hydraulische randvoorwaarden berekening is uitgevoerd op de tijdelijke locatie", messageList[10]);
                                                 CalculationServiceTestHelper.AssertCalculationEndMessage(calculationNameB, messageList[11]);

                                                 Assert.AreEqual("Hydraulische randvoorwaarden berekenen voor locatie 'A' is gelukt.", messageList[12]);
                                                 Assert.AreEqual("Hydraulische randvoorwaarden berekenen voor locatie 'B' is gelukt.", messageList[13]);
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
                Action call = () => guiService.Calculate(Enumerable.Empty<DuneLocation>(), databasePath, 1);

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
            string validFilePath = Path.Combine(testDataPath, "complete.sqlite");

            var viewParent = mockRepository.Stub<IWin32Window>();

            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateDunesBoundaryConditionsCalculator(testDataPath)).Return(new TestDunesBoundaryConditionsCalculator());
            mockRepository.ReplayAll();

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                var guiService = new DuneLocationCalculationGuiService(viewParent);

                // Call
                Action call = () => guiService.Calculate(
                    new List<DuneLocation>
                    {
                        new TestDuneLocation(hydraulicLocationName)
                    }, validFilePath,
                    1);

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(7, msgs.Length);

                    string calculationName = $"Hydraulische belasting berekenen voor locatie '{hydraulicLocationName}'";

                    CalculationServiceTestHelper.AssertValidationStartMessage(calculationName, msgs[0]);
                    CalculationServiceTestHelper.AssertValidationEndMessage(calculationName, msgs[1]);
                    CalculationServiceTestHelper.AssertCalculationStartMessage(calculationName, msgs[2]);
                    Assert.AreEqual($"Hydraulische randvoorwaarden berekening voor locatie '{hydraulicLocationName}' is niet geconvergeerd.", msgs[3]);
                    StringAssert.StartsWith("Hydraulische randvoorwaarden berekening is uitgevoerd op de tijdelijke locatie", msgs[4]);
                    CalculationServiceTestHelper.AssertCalculationEndMessage(calculationName, msgs[5]);
                    StringAssert.AreNotEqualIgnoringCase($"Uitvoeren van '{calculationName}' is gelukt.", msgs[6]);
                });
            }
            mockRepository.VerifyAll();
        }
    }
}