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
using System.Windows.Forms;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Hydraulics;
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
        private static readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO, "HydraulicBoundaryDatabaseImporter");
        private static readonly string validFilePath = Path.Combine(testDataPath, "complete.sqlite");
        private static readonly string validPreprocessorDirectory = TestHelper.GetScratchPadPath();

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
            var mockRepository = new MockRepository();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            using (var viewParent = new Form())
            {
                var guiService = new DuneLocationCalculationGuiService(viewParent);

                // Call
                TestDelegate test = () => guiService.Calculate(null,
                                                               assessmentSection,
                                                               1.0 / 30000,
                                                               "A");

                // Assert
                var exception = Assert.Throws<ArgumentNullException>(test);
                Assert.AreEqual("calculations", exception.ParamName);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        public void Calculate_AssessmentSectionNull_ThrowArgumentNullException()
        {
            // Setup
            using (var viewParent = new Form())
            {
                var guiService = new DuneLocationCalculationGuiService(viewParent);

                // Call
                TestDelegate test = () => guiService.Calculate(Enumerable.Empty<DuneLocationCalculation>(),
                                                               null,
                                                               1.0 / 30000,
                                                               "A");

                // Assert
                var exception = Assert.Throws<ArgumentNullException>(test);
                Assert.AreEqual("assessmentSection", exception.ParamName);
            }
        }

        [Test]
        public void Calculate_HydraulicDatabaseDoesNotExist_LogsError()
        {
            // Setup
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                FilePath = "Does not exist"
            };

            var mockRepository = new MockRepository();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            assessmentSection.Stub(a => a.HydraulicBoundaryDatabase)
                             .Return(hydraulicBoundaryDatabase)
                             .Repeat.Any();
            mockRepository.ReplayAll();

            using (var viewParent = new Form())
            {
                var guiService = new DuneLocationCalculationGuiService(viewParent);

                // Call
                Action call = () => guiService.Calculate(Enumerable.Empty<DuneLocationCalculation>(),
                                                         assessmentSection,
                                                         1.0 / 30000,
                                                         "A");

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
        public void Calculate_InvalidNorm_LogsError()
        {
            // Setup
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                FilePath = validFilePath
            };

            var mockRepository = new MockRepository();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            assessmentSection.Stub(a => a.HydraulicBoundaryDatabase)
                             .Return(hydraulicBoundaryDatabase)
                             .Repeat.Any();
            mockRepository.ReplayAll();

            using (var viewParent = new Form())
            {
                var guiService = new DuneLocationCalculationGuiService(viewParent);

                // Call
                Action call = () => guiService.Calculate(Enumerable.Empty<DuneLocationCalculation>(),
                                                         assessmentSection,
                                                         1.0,
                                                         "A");

                // Assert
                TestHelper.AssertLogMessages(call, messages => { Assert.AreEqual("Berekeningen konden niet worden gestart. Doelkans is te groot om een berekening uit te kunnen voeren.", messages.Single()); });
            }

            mockRepository.VerifyAll();
        }

        [Test]
        public void Calculate_ValidPathEmptyCalculationList_NoLog()
        {
            // Setup
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                FilePath = validFilePath
            };

            var mockRepository = new MockRepository();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            assessmentSection.Stub(a => a.HydraulicBoundaryDatabase)
                             .Return(hydraulicBoundaryDatabase)
                             .Repeat.Any();
            mockRepository.ReplayAll();

            using (var viewParent = new Form())
            {
                var guiService = new DuneLocationCalculationGuiService(viewParent);

                // Call
                Action call = () => guiService.Calculate(Enumerable.Empty<DuneLocationCalculation>(),
                                                         assessmentSection,
                                                         0.01,
                                                         "A");

                // Assert
                TestHelper.AssertLogMessagesCount(call, 0);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        public void Calculate_ValidPathOneCalculation_LogsMessages()
        {
            // Setup
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                FilePath = validFilePath,
                CanUsePreprocessor = true,
                UsePreprocessor = true,
                PreprocessorDirectory = validPreprocessorDirectory
            };

            const string categoryBoundaryName = "A";
            const string duneLocationName = "duneLocationName";

            var mockRepository = new MockRepository();
            var viewParent = mockRepository.Stub<IWin32Window>();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateDunesBoundaryConditionsCalculator(testDataPath, validPreprocessorDirectory))
                             .Return(new TestDunesBoundaryConditionsCalculator());
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            assessmentSection.Stub(a => a.HydraulicBoundaryDatabase)
                             .Return(hydraulicBoundaryDatabase)
                             .Repeat.Any();
            mockRepository.ReplayAll();

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                var guiService = new DuneLocationCalculationGuiService(viewParent);

                // Call
                Action call = () => guiService.Calculate(new[]
                                                         {
                                                             new DuneLocationCalculation(new TestDuneLocation(duneLocationName))
                                                         },
                                                         assessmentSection,
                                                         1.0 / 30000,
                                                         categoryBoundaryName);

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(8, msgs.Length);
                    Assert.AreEqual($"Hydraulische randvoorwaarden berekenen voor locatie '{duneLocationName}' (Categorie {categoryBoundaryName}) is gestart.", msgs[0]);
                    CalculationServiceTestHelper.AssertValidationStartMessage(msgs[1]);
                    CalculationServiceTestHelper.AssertValidationEndMessage(msgs[2]);
                    CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[3]);
                    Assert.AreEqual($"Hydraulische randvoorwaarden berekening voor locatie '{duneLocationName}' (Categorie {categoryBoundaryName}) is niet geconvergeerd.", msgs[4]);
                    StringAssert.StartsWith("Hydraulische randvoorwaarden berekening is uitgevoerd op de tijdelijke locatie", msgs[5]);
                    CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[6]);
                    Assert.AreEqual($"Hydraulische randvoorwaarden berekenen voor locatie '{duneLocationName}' (Categorie {categoryBoundaryName}) is gelukt.", msgs[7]);
                });
            }

            mockRepository.VerifyAll();
        }
    }
}