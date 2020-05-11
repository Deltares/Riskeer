// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using System.Windows.Forms;
using Core.Common.TestUtil;
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
        private static readonly string validFilePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");

        [Test]
        public void Constructor_MainWindowNull_ThrowsArgumentNullException()
        {
            // Setup & Call
            TestDelegate test = () => new HydraulicBoundaryLocationCalculationGuiService(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            const string expectedParamName = "viewParent";
            Assert.AreEqual(expectedParamName, paramName);
        }

        [Test]
        public void Constructor_DefaultValues()
        {
            // Setup
            using (var viewParent = new Form())
            {
                // Call
                var guiService = new HydraulicBoundaryLocationCalculationGuiService(viewParent);

                // Assert
                Assert.IsInstanceOf<IHydraulicBoundaryLocationCalculationGuiService>(guiService);
            }
        }

        [Test]
        public void CalculateDesignWaterLevels_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Setup
            using (var viewParent = new Form())
            {
                var guiService = new HydraulicBoundaryLocationCalculationGuiService(viewParent);

                // Call
                TestDelegate test = () => guiService.CalculateDesignWaterLevels(Enumerable.Empty<HydraulicBoundaryLocationCalculation>(),
                                                                                null,
                                                                                0.01,
                                                                                "A");

                // Assert
                string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
                Assert.AreEqual("assessmentSection", paramName);
            }
        }

        [Test]
        public void CalculateDesignWaterLevels_CalculationsNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            using (var viewParent = new Form())
            {
                var guiService = new HydraulicBoundaryLocationCalculationGuiService(viewParent);

                // Call
                TestDelegate test = () => guiService.CalculateDesignWaterLevels(null,
                                                                                assessmentSection,
                                                                                0.01,
                                                                                "A");

                // Assert
                string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
                Assert.AreEqual("calculations", paramName);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void CalculateDesignWaterLevels_HydraulicDatabaseDoesNotExist_LogsError()
        {
            // Setup
            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(mocks);
            mocks.ReplayAll();

            assessmentSection.HydraulicBoundaryDatabase.FilePath = "Does not exist";
            HydraulicBoundaryDatabaseTestHelper.SetHydraulicBoundaryLocationConfigurationSettings(assessmentSection.HydraulicBoundaryDatabase);

            using (var viewParent = new Form())
            {
                var guiService = new HydraulicBoundaryLocationCalculationGuiService(viewParent);

                // Call
                Action call = () => guiService.CalculateDesignWaterLevels(Enumerable.Empty<HydraulicBoundaryLocationCalculation>(),
                                                                          assessmentSection,
                                                                          0.01,
                                                                          "A");

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(1, msgs.Length);
                    StringAssert.StartsWith("Berekeningen konden niet worden gestart. ", msgs.First());
                });
            }

            mocks.VerifyAll();
        }

        [Test]
        public void CalculateDesignWaterLevels_InvalidNorm_LogsError()
        {
            // Setup
            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(mocks);
            mocks.ReplayAll();

            assessmentSection.HydraulicBoundaryDatabase.FilePath = validFilePath;
            HydraulicBoundaryDatabaseTestHelper.SetHydraulicBoundaryLocationConfigurationSettings(assessmentSection.HydraulicBoundaryDatabase);

            using (var viewParent = new Form())
            {
                var guiService = new HydraulicBoundaryLocationCalculationGuiService(viewParent);

                // Call
                Action call = () => guiService.CalculateDesignWaterLevels(Enumerable.Empty<HydraulicBoundaryLocationCalculation>(),
                                                                          assessmentSection,
                                                                          1.0,
                                                                          "A");

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    Assert.AreEqual("Berekeningen konden niet worden gestart. Doelkans is te groot om een berekening uit te kunnen voeren.", messages.Single());
                });
            }

            mocks.VerifyAll();
        }

        [Test]
        public void CalculateDesignWaterLevels_ValidPathEmptyCalculationList_NoLog()
        {
            // Setup
            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(mocks);
            mocks.ReplayAll();

            assessmentSection.HydraulicBoundaryDatabase.FilePath = validFilePath;
            HydraulicBoundaryDatabaseTestHelper.SetHydraulicBoundaryLocationConfigurationSettings(assessmentSection.HydraulicBoundaryDatabase);

            DialogBoxHandler = (name, wnd) =>
            {
                // Expect an activity dialog which is automatically closed
            };

            using (var viewParent = new Form())
            {
                var guiService = new HydraulicBoundaryLocationCalculationGuiService(viewParent);

                // Call
                Action call = () => guiService.CalculateDesignWaterLevels(Enumerable.Empty<HydraulicBoundaryLocationCalculation>(),
                                                                          assessmentSection,
                                                                          0.01,
                                                                          "A");

                // Assert
                TestHelper.AssertLogMessagesCount(call, 0);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void CalculateDesignWaterLevels_ValidPathOneCalculation_LogsMessages()
        {
            // Setup
            const string hydraulicLocationName = "name";
            const string categoryBoundaryName = "A";

            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation(hydraulicLocationName);

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(mockRepository);

            HydraulicBoundaryDatabase hydraulicBoundaryDatabase = assessmentSection.HydraulicBoundaryDatabases.First();
            hydraulicBoundaryDatabase.FilePath = validFilePath;
            hydraulicBoundaryDatabase.Locations.Add(hydraulicBoundaryLocation);
            HydraulicBoundaryDatabaseTestHelper.SetHydraulicBoundaryLocationConfigurationSettings(hydraulicBoundaryDatabase);

            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateDesignWaterLevelCalculator(Arg<HydraRingCalculationSettings>.Is.NotNull))
                             .WhenCalled(invocation =>
                             {
                                 HydraRingCalculationSettingsTestHelper.AssertHydraRingCalculationSettings(
                                     HydraulicBoundaryCalculationSettingsFactory.CreateSettings(assessmentSection, hydraulicBoundaryLocation),
                                     (HydraRingCalculationSettings) invocation.Arguments[0]);
                             })
                             .Return(new TestDesignWaterLevelCalculator());
            mockRepository.ReplayAll();


            DialogBoxHandler = (name, wnd) =>
            {
                // Expect an activity dialog which is automatically closed
            };

            using (var viewParent = new Form())
            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                var guiService = new HydraulicBoundaryLocationCalculationGuiService(viewParent);

                // Call
                Action call = () =>
                {
                    guiService.CalculateDesignWaterLevels(new[]
                                                          {
                                                              new HydraulicBoundaryLocationCalculation(hydraulicBoundaryLocation)
                                                          },
                                                          assessmentSection,
                                                          0.01,
                                                          categoryBoundaryName);
                };

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(8, msgs.Length);
                    string activityDescription = GetDesignWaterLevelCalculationActivityDescription(hydraulicLocationName, categoryBoundaryName);
                    Assert.AreEqual($"{activityDescription} is gestart.", msgs[0]);
                    CalculationServiceTestHelper.AssertValidationStartMessage(msgs[1]);
                    CalculationServiceTestHelper.AssertValidationEndMessage(msgs[2]);
                    CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[3]);
                    Assert.AreEqual($"Waterstand berekening voor locatie 'name' (Categoriegrens {categoryBoundaryName}) is niet geconvergeerd.", msgs[4]);
                    StringAssert.StartsWith("Waterstand berekening is uitgevoerd op de tijdelijke locatie", msgs[5]);
                    CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[6]);
                    Assert.AreEqual($"{activityDescription} is gelukt.", msgs[7]);
                });
            }

            mockRepository.VerifyAll();
        }

        [Test]
        public void CalculateWaveHeights_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Setup
            using (var viewParent = new Form())
            {
                var guiService = new HydraulicBoundaryLocationCalculationGuiService(viewParent);

                // Call
                TestDelegate test = () => guiService.CalculateWaveHeights(Enumerable.Empty<HydraulicBoundaryLocationCalculation>(),
                                                                          null,
                                                                          0.01,
                                                                          "A");

                // Assert
                string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
                Assert.AreEqual("assessmentSection", paramName);
            }
        }

        [Test]
        public void CalculateWaveHeights_CalculationsNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            using (var viewParent = new Form())
            {
                var guiService = new HydraulicBoundaryLocationCalculationGuiService(viewParent);

                // Call
                TestDelegate test = () => guiService.CalculateWaveHeights(null,
                                                                          assessmentSection,
                                                                          0.01,
                                                                          "A");

                // Assert
                string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
                Assert.AreEqual("calculations", paramName);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void CalculateWaveHeights_HydraulicDatabaseDoesNotExist_LogsError()
        {
            // Setup
            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(mocks);
            mocks.ReplayAll();

            assessmentSection.HydraulicBoundaryDatabase.FilePath = "Does not exist";
            HydraulicBoundaryDatabaseTestHelper.SetHydraulicBoundaryLocationConfigurationSettings(assessmentSection.HydraulicBoundaryDatabase);

            using (var viewParent = new Form())
            {
                var guiService = new HydraulicBoundaryLocationCalculationGuiService(viewParent);

                // Call
                Action call = () => guiService.CalculateWaveHeights(Enumerable.Empty<HydraulicBoundaryLocationCalculation>(),
                                                                    assessmentSection,
                                                                    0.01,
                                                                    "A");

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(1, msgs.Length);
                    StringAssert.StartsWith("Berekeningen konden niet worden gestart. ", msgs.First());
                });
            }

            mocks.VerifyAll();
        }

        [Test]
        public void CalculateWaveHeights_InvalidNorm_LogsError()
        {
            // Setup
            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(mocks);
            mocks.ReplayAll();

            assessmentSection.HydraulicBoundaryDatabase.FilePath = validFilePath;
            HydraulicBoundaryDatabaseTestHelper.SetHydraulicBoundaryLocationConfigurationSettings(assessmentSection.HydraulicBoundaryDatabase);

            using (var viewParent = new Form())
            {
                var guiService = new HydraulicBoundaryLocationCalculationGuiService(viewParent);

                // Call
                Action call = () => guiService.CalculateWaveHeights(Enumerable.Empty<HydraulicBoundaryLocationCalculation>(),
                                                                    assessmentSection,
                                                                    1.0,
                                                                    "A");

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    Assert.AreEqual("Berekeningen konden niet worden gestart. Doelkans is te groot om een berekening uit te kunnen voeren.", messages.Single());
                });
            }

            mocks.VerifyAll();
        }

        [Test]
        public void CalculateWaveHeights_ValidPathEmptyCalculationList_NoLog()
        {
            // Setup
            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(mocks);
            mocks.ReplayAll();

            assessmentSection.HydraulicBoundaryDatabase.FilePath = validFilePath;
            HydraulicBoundaryDatabaseTestHelper.SetHydraulicBoundaryLocationConfigurationSettings(assessmentSection.HydraulicBoundaryDatabase);

            DialogBoxHandler = (name, wnd) =>
            {
                // Expect an activity dialog which is automatically closed
            };

            using (var viewParent = new Form())
            {
                var guiService = new HydraulicBoundaryLocationCalculationGuiService(viewParent);

                // Call
                Action call = () => guiService.CalculateWaveHeights(Enumerable.Empty<HydraulicBoundaryLocationCalculation>(),
                                                                    assessmentSection,
                                                                    0.01,
                                                                    "A");

                // Assert
                TestHelper.AssertLogMessagesCount(call, 0);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void CalculateWaveHeights_ValidPathOneCalculation_LogsMessages()
        {
            // Setup
            const string hydraulicLocationName = "name";
            const string categoryBoundaryName = "A";

            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation(hydraulicLocationName);

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(mockRepository);

            HydraulicBoundaryDatabase hydraulicBoundaryDatabase = assessmentSection.HydraulicBoundaryDatabases.First();
            hydraulicBoundaryDatabase.FilePath = validFilePath;
            hydraulicBoundaryDatabase.Locations.Add(hydraulicBoundaryLocation);
            HydraulicBoundaryDatabaseTestHelper.SetHydraulicBoundaryLocationConfigurationSettings(hydraulicBoundaryDatabase);

            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateWaveHeightCalculator(Arg<HydraRingCalculationSettings>.Is.NotNull))
                             .WhenCalled(invocation =>
                             {
                                 HydraRingCalculationSettingsTestHelper.AssertHydraRingCalculationSettings(
                                     HydraulicBoundaryCalculationSettingsFactory.CreateSettings(assessmentSection, hydraulicBoundaryLocation),
                                     (HydraRingCalculationSettings) invocation.Arguments[0]);
                             })
                             .Return(new TestWaveHeightCalculator());
            mockRepository.ReplayAll();

            DialogBoxHandler = (name, wnd) =>
            {
                // Expect an activity dialog which is automatically closed
            };

            using (var viewParent = new Form())
            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                var guiService = new HydraulicBoundaryLocationCalculationGuiService(viewParent);

                // Call
                Action call = () => guiService.CalculateWaveHeights(new[]
                                                                    {
                                                                        new HydraulicBoundaryLocationCalculation(new TestHydraulicBoundaryLocation(hydraulicLocationName))
                                                                    },
                                                                    assessmentSection,
                                                                    0.01,
                                                                    categoryBoundaryName);

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(8, msgs.Length);
                    string activityDescription = GetWaveHeightCalculationActivityDescription(hydraulicLocationName, categoryBoundaryName);
                    Assert.AreEqual($"{activityDescription} is gestart.", msgs[0]);
                    CalculationServiceTestHelper.AssertValidationStartMessage(msgs[1]);
                    CalculationServiceTestHelper.AssertValidationEndMessage(msgs[2]);
                    CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[3]);
                    Assert.AreEqual($"Golfhoogte berekening voor locatie 'name' (Categoriegrens {categoryBoundaryName}) is niet geconvergeerd.", msgs[4]);
                    StringAssert.StartsWith("Golfhoogte berekening is uitgevoerd op de tijdelijke locatie", msgs[5]);
                    CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[6]);
                    Assert.AreEqual($"{activityDescription} is gelukt.", msgs[7]);
                });
            }

            mockRepository.VerifyAll();
        }

        private static string GetWaveHeightCalculationActivityDescription(string locationName, string categoryBoundaryName)
        {
            return $"Golfhoogte berekenen voor locatie '{locationName}' (Categoriegrens {categoryBoundaryName})";
        }

        private static string GetDesignWaterLevelCalculationActivityDescription(string locationName, string categoryBoundaryName)
        {
            return $"Waterstand berekenen voor locatie '{locationName}' (Categoriegrens {categoryBoundaryName})";
        }
    }
}