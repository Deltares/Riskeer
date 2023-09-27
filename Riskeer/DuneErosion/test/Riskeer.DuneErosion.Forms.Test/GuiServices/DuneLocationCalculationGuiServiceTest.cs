﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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
using Core.Common.TestUtil;
using Core.Gui.Forms;
using Core.Gui.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Service.TestUtil;
using Riskeer.DuneErosion.Data;
using Riskeer.DuneErosion.Data.TestUtil;
using Riskeer.DuneErosion.Forms.GuiServices;
using Riskeer.HydraRing.Calculation.Calculator.Factory;
using Riskeer.HydraRing.Calculation.Data.Input;
using Riskeer.HydraRing.Calculation.TestUtil.Calculator;

namespace Riskeer.DuneErosion.Forms.Test.GuiServices
{
    [TestFixture]
    public class DuneLocationCalculationGuiServiceTest
    {
        private static readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Common.IO, nameof(HydraulicBoundaryData));
        private static readonly string validHlcdFilePath = Path.Combine(testDataPath, "hlcd.sqlite");
        private static readonly string validHrdFilePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");
        private static readonly string validHrdFileVersion = "Dutch coast South19-11-2015 12:0013";

        [Test]
        public void Constructor_ViewParentNull_ThrowArgumentNullException()
        {
            // Call
            void Call() => new DuneLocationCalculationGuiService(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("viewParent", exception.ParamName);
        }

        [Test]
        public void Calculate_CalculationsNull_ThrowArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var viewParent = mocks.Stub<IViewParent>();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var guiService = new DuneLocationCalculationGuiService(viewParent);

            // Call
            void Call() => guiService.Calculate(null, assessmentSection, 0.01, "1/100");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("calculations", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Calculate_AssessmentSectionNull_ThrowArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var viewParent = mocks.Stub<IViewParent>();
            mocks.ReplayAll();

            var guiService = new DuneLocationCalculationGuiService(viewParent);

            // Call
            void Call() => guiService.Calculate(Enumerable.Empty<DuneLocationCalculation>(), null, 0.01, "1/100");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Calculate_ValidPathEmptyCalculationList_NoLog()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            using (var viewParent = new TestViewParentForm())
            {
                var guiService = new DuneLocationCalculationGuiService(viewParent);

                // Call
                void Call() => guiService.Calculate(Enumerable.Empty<DuneLocationCalculation>(), assessmentSection, 0.01, "1/100");

                // Assert
                TestHelper.AssertLogMessagesCount(Call, 0);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void Calculate_ValidPathOneCalculation_LogsMessages()
        {
            // Setup
            const string calculationIdentifier = "1/100";
            const string duneLocationName = "duneLocationName";

            var duneLocation = new TestDuneLocation(duneLocationName);

            var hydraulicBoundaryData = new HydraulicBoundaryData
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
                        UsePreprocessorClosure = true,
                        Locations =
                        {
                            duneLocation.HydraulicBoundaryLocation
                        }
                    }
                }
            };

            var mocks = new MockRepository();
            var calculatorFactory = mocks.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateDunesBoundaryConditionsCalculator(Arg<HydraRingCalculationSettings>.Is.NotNull))
                             .WhenCalled(invocation =>
                             {
                                 HydraRingCalculationSettingsTestHelper.AssertHydraRingCalculationSettings(
                                     HydraulicBoundaryCalculationSettingsFactory.CreateSettings(
                                         hydraulicBoundaryData,
                                         duneLocation.HydraulicBoundaryLocation),
                                     (HydraRingCalculationSettings) invocation.Arguments[0]);
                             })
                             .Return(new TestDunesBoundaryConditionsCalculator());
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(a => a.HydraulicBoundaryData)
                             .Return(hydraulicBoundaryData);
            mocks.ReplayAll();

            using (var viewParent = new TestViewParentForm())
            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                var guiService = new DuneLocationCalculationGuiService(viewParent);

                // Call
                void Call()
                {
                    guiService.Calculate(new[]
                    {
                        new DuneLocationCalculation(duneLocation)
                    }, assessmentSection, 0.01, calculationIdentifier);
                }

                // Assert
                TestHelper.AssertLogMessages(Call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(8, msgs.Length);
                    Assert.AreEqual($"Hydraulische belastingen berekenen voor locatie '{duneLocationName}' ({calculationIdentifier}) is gestart.", msgs[0]);
                    CalculationServiceTestHelper.AssertValidationStartMessage(msgs[1]);
                    CalculationServiceTestHelper.AssertValidationEndMessage(msgs[2]);
                    CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[3]);
                    Assert.AreEqual($"Hydraulische belastingenberekening voor locatie '{duneLocationName}' ({calculationIdentifier}) is niet geconvergeerd.", msgs[4]);
                    StringAssert.StartsWith("Hydraulische belastingenberekening is uitgevoerd op de tijdelijke locatie", msgs[5]);
                    CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[6]);
                    Assert.AreEqual($"Hydraulische belastingen berekenen voor locatie '{duneLocationName}' ({calculationIdentifier}) is gelukt.", msgs[7]);
                });
            }

            mocks.VerifyAll();
        }
    }
}