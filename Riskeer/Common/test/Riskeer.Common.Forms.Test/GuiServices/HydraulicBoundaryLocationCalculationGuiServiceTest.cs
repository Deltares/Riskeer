﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
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
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.GuiServices;
using Riskeer.Common.Service.TestUtil;
using Riskeer.HydraRing.Calculation.Calculator.Factory;
using Riskeer.HydraRing.Calculation.Data.Input;
using Riskeer.HydraRing.Calculation.TestUtil.Calculator;

namespace Riskeer.Common.Forms.Test.GuiServices
{
    [TestFixture]
    public class HydraulicBoundaryLocationCalculationGuiServiceTest : NUnitFormTest
    {
        private static readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Integration.Service, "HydraRingCalculation");
        private static readonly string validHlcdFilePath = Path.Combine(testDataPath, "hlcd.sqlite");
        private static readonly string validHrdFilePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");
        private static readonly string validHrdFileVersion = "Dutch coast South19-11-2015 12:0013";

        [Test]
        public void Constructor_ViewParentNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new HydraulicBoundaryLocationCalculationGuiService(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("viewParent", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var viewParent = mocks.Stub<IViewParent>();
            mocks.ReplayAll();

            // Call
            var guiService = new HydraulicBoundaryLocationCalculationGuiService(viewParent);

            // Assert
            Assert.IsInstanceOf<IHydraulicBoundaryLocationCalculationGuiService>(guiService);
            mocks.VerifyAll();
        }

        [Test]
        public void CalculateDesignWaterLevels_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var viewParent = mocks.Stub<IViewParent>();
            mocks.ReplayAll();

            var guiService = new HydraulicBoundaryLocationCalculationGuiService(viewParent);

            // Call
            void Call() => guiService.CalculateDesignWaterLevels(Enumerable.Empty<HydraulicBoundaryLocationCalculation>(), null, 0.01, "1/100");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void CalculateDesignWaterLevels_CalculationsNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var viewParent = mocks.Stub<IViewParent>();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var guiService = new HydraulicBoundaryLocationCalculationGuiService(viewParent);

            // Call
            void Call() => guiService.CalculateDesignWaterLevels(null, assessmentSection, 0.01, "1/100");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("calculations", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void CalculateDesignWaterLevels_EmptyCalculationList_NoLog()
        {
            // Setup
            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(mocks);
            mocks.ReplayAll();

            DialogBoxHandler = (name, wnd) =>
            {
                // Expect an activity dialog which is automatically closed
            };

            using (var viewParent = new TestViewParentForm())
            {
                var guiService = new HydraulicBoundaryLocationCalculationGuiService(viewParent);

                // Call
                void Call() => guiService.CalculateDesignWaterLevels(Enumerable.Empty<HydraulicBoundaryLocationCalculation>(), assessmentSection, 0.01, "1/100");

                // Assert
                TestHelper.AssertLogMessagesCount(Call, 0);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void CalculateDesignWaterLevels_ValidHydraulicBoundaryDataOneCalculation_LogsMessages()
        {
            // Setup
            const string hydraulicLocationName = "name";
            const string calculationIdentifier = "1/100";

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(mocks);

            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation(hydraulicLocationName);

            var calculatorFactory = mocks.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateDesignWaterLevelCalculator(Arg<HydraRingCalculationSettings>.Is.NotNull))
                             .WhenCalled(invocation =>
                             {
                                 HydraRingCalculationSettingsTestHelper.AssertHydraRingCalculationSettings(
                                     HydraulicBoundaryCalculationSettingsFactory.CreateSettings(
                                         assessmentSection.HydraulicBoundaryData,
                                         hydraulicBoundaryLocation),
                                     (HydraRingCalculationSettings) invocation.Arguments[0]);
                             })
                             .Return(new TestDesignWaterLevelCalculator());
            mocks.ReplayAll();

            assessmentSection.HydraulicBoundaryData.HydraulicLocationConfigurationDatabase.FilePath = validHlcdFilePath;
            assessmentSection.HydraulicBoundaryData.HydraulicBoundaryDatabases.Add(new HydraulicBoundaryDatabase
            {
                FilePath = validHrdFilePath,
                Version = validHrdFileVersion,
                Locations =
                {
                    hydraulicBoundaryLocation
                }
            });

            DialogBoxHandler = (name, wnd) =>
            {
                // Expect an activity dialog which is automatically closed
            };

            using (var viewParent = new TestViewParentForm())
            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                var guiService = new HydraulicBoundaryLocationCalculationGuiService(viewParent);

                // Call
                void Call()
                {
                    guiService.CalculateDesignWaterLevels(new[]
                    {
                        new HydraulicBoundaryLocationCalculation(hydraulicBoundaryLocation)
                    }, assessmentSection, 0.01, calculationIdentifier);
                }

                // Assert
                TestHelper.AssertLogMessages(Call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(8, msgs.Length);
                    string activityDescription = GetDesignWaterLevelCalculationActivityDescription(hydraulicLocationName, calculationIdentifier);
                    Assert.AreEqual($"{activityDescription} is gestart.", msgs[0]);
                    CalculationServiceTestHelper.AssertValidationStartMessage(msgs[1]);
                    CalculationServiceTestHelper.AssertValidationEndMessage(msgs[2]);
                    CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[3]);
                    Assert.AreEqual($"Waterstand berekening voor locatie 'name' ({calculationIdentifier}) is niet geconvergeerd.", msgs[4]);
                    StringAssert.StartsWith("Waterstand berekening is uitgevoerd op de tijdelijke locatie", msgs[5]);
                    CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[6]);
                    Assert.AreEqual($"{activityDescription} is gelukt.", msgs[7]);
                });
            }

            mocks.VerifyAll();
        }

        [Test]
        public void CalculateWaveHeights_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var viewParent = mocks.Stub<IViewParent>();
            mocks.ReplayAll();

            var guiService = new HydraulicBoundaryLocationCalculationGuiService(viewParent);

            // Call
            void Call() => guiService.CalculateWaveHeights(Enumerable.Empty<HydraulicBoundaryLocationCalculation>(), null, 0.01, "1/100");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void CalculateWaveHeights_CalculationsNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var viewParent = mocks.Stub<IViewParent>();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var guiService = new HydraulicBoundaryLocationCalculationGuiService(viewParent);

            // Call
            void Call() => guiService.CalculateWaveHeights(null, assessmentSection, 0.01, "1/100");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("calculations", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void CalculateWaveHeights_EmptyCalculationList_NoLog()
        {
            // Setup
            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(mocks);
            mocks.ReplayAll();

            DialogBoxHandler = (name, wnd) =>
            {
                // Expect an activity dialog which is automatically closed
            };

            using (var viewParent = new TestViewParentForm())
            {
                var guiService = new HydraulicBoundaryLocationCalculationGuiService(viewParent);

                // Call
                void Call() => guiService.CalculateWaveHeights(Enumerable.Empty<HydraulicBoundaryLocationCalculation>(), assessmentSection, 0.01, "1/100");

                // Assert
                TestHelper.AssertLogMessagesCount(Call, 0);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void CalculateWaveHeights_ValidHydraulicBoundaryDataOneCalculation_LogsMessages()
        {
            // Setup
            const string hydraulicLocationName = "name";
            const string calculationIdentifier = "1/100";

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(mocks);

            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation(hydraulicLocationName);

            var calculatorFactory = mocks.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateWaveHeightCalculator(Arg<HydraRingCalculationSettings>.Is.NotNull))
                             .WhenCalled(invocation =>
                             {
                                 HydraRingCalculationSettingsTestHelper.AssertHydraRingCalculationSettings(
                                     HydraulicBoundaryCalculationSettingsFactory.CreateSettings(
                                         assessmentSection.HydraulicBoundaryData,
                                         hydraulicBoundaryLocation),
                                     (HydraRingCalculationSettings) invocation.Arguments[0]);
                             })
                             .Return(new TestWaveHeightCalculator());
            mocks.ReplayAll();

            assessmentSection.HydraulicBoundaryData.HydraulicLocationConfigurationDatabase.FilePath = validHlcdFilePath;
            assessmentSection.HydraulicBoundaryData.HydraulicBoundaryDatabases.Add(new HydraulicBoundaryDatabase
            {
                FilePath = validHrdFilePath,
                Version = validHrdFileVersion,
                Locations =
                {
                    hydraulicBoundaryLocation
                }
            });

            DialogBoxHandler = (name, wnd) =>
            {
                // Expect an activity dialog which is automatically closed
            };

            using (var viewParent = new TestViewParentForm())
            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                var guiService = new HydraulicBoundaryLocationCalculationGuiService(viewParent);

                // Call
                void Call()
                {
                    guiService.CalculateWaveHeights(new[]
                    {
                        new HydraulicBoundaryLocationCalculation(hydraulicBoundaryLocation)
                    }, assessmentSection, 0.01, calculationIdentifier);
                }

                // Assert
                TestHelper.AssertLogMessages(Call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(8, msgs.Length);
                    string activityDescription = GetWaveHeightCalculationActivityDescription(hydraulicLocationName, calculationIdentifier);
                    Assert.AreEqual($"{activityDescription} is gestart.", msgs[0]);
                    CalculationServiceTestHelper.AssertValidationStartMessage(msgs[1]);
                    CalculationServiceTestHelper.AssertValidationEndMessage(msgs[2]);
                    CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[3]);
                    Assert.AreEqual($"Golfhoogte berekening voor locatie 'name' ({calculationIdentifier}) is niet geconvergeerd.", msgs[4]);
                    StringAssert.StartsWith("Golfhoogte berekening is uitgevoerd op de tijdelijke locatie", msgs[5]);
                    CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[6]);
                    Assert.AreEqual($"{activityDescription} is gelukt.", msgs[7]);
                });
            }

            mocks.VerifyAll();
        }

        private static string GetWaveHeightCalculationActivityDescription(string locationName, string calculationIdentifier)
        {
            return $"Golfhoogte berekenen voor locatie '{locationName}' ({calculationIdentifier})";
        }

        private static string GetDesignWaterLevelCalculationActivityDescription(string locationName, string calculationIdentifier)
        {
            return $"Waterstand berekenen voor locatie '{locationName}' ({calculationIdentifier})";
        }
    }
}