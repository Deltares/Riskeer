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
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.GuiServices;
using Ringtoets.Common.Service.MessageProviders;
using Ringtoets.Common.Service.TestUtil;
using Ringtoets.HydraRing.Calculation.Calculator.Factory;
using Ringtoets.HydraRing.Calculation.TestUtil.Calculator;

namespace Ringtoets.Common.Forms.Test.GuiServices
{
    [TestFixture]
    public class HydraulicBoundaryLocationCalculationGuiServiceTest : NUnitFormTest
    {
        private static readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Integration.Service, "HydraRingCalculation");
        private static readonly string validFilePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");
        private static readonly string validPreprocessorDirectory = TestHelper.GetScratchPadPath();

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
        public void CalculateDesignWaterLevels_CalculationsNull_ThrowsArgumentNullException()
        {
            // Setup
            var mockRepository = new MockRepository();
            var calculationMessageProvider = mockRepository.StrictMock<ICalculationMessageProvider>();
            mockRepository.ReplayAll();

            using (var viewParent = new Form())
            {
                var guiService = new HydraulicBoundaryLocationCalculationGuiService(viewParent);

                // Call
                TestDelegate test = () => guiService.CalculateDesignWaterLevels(validFilePath,
                                                                                validPreprocessorDirectory,
                                                                                null,
                                                                                0.01,
                                                                                calculationMessageProvider);

                // Assert
                string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
                const string expectedParamName = "calculations";
                Assert.AreEqual(expectedParamName, paramName);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        public void CalculateDesignWaterLevels_MessageProviderNull_ThrowsArgumentNullException()
        {
            // Setup
            using (var viewParent = new Form())
            {
                var guiService = new HydraulicBoundaryLocationCalculationGuiService(viewParent);

                // Call
                TestDelegate test = () => guiService.CalculateDesignWaterLevels(validFilePath,
                                                                                validPreprocessorDirectory,
                                                                                Enumerable.Empty<HydraulicBoundaryLocationCalculation>(),
                                                                                0.01,
                                                                                null);

                // Assert
                string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
                const string expectedParamName = "messageProvider";
                Assert.AreEqual(expectedParamName, paramName);
            }
        }

        [Test]
        public void CalculateDesignWaterLevels_HydraulicDatabaseDoesNotExist_LogsError()
        {
            // Setup
            var mockRepository = new MockRepository();
            var calculationMessageProvider = mockRepository.StrictMock<ICalculationMessageProvider>();
            mockRepository.ReplayAll();

            using (var viewParent = new Form())
            {
                var guiService = new HydraulicBoundaryLocationCalculationGuiService(viewParent);

                // Call
                Action call = () => guiService.CalculateDesignWaterLevels("Does not exist",
                                                                          validPreprocessorDirectory,
                                                                          Enumerable.Empty<HydraulicBoundaryLocationCalculation>(),
                                                                          0.01,
                                                                          calculationMessageProvider);

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
        public void CalculateDesignWaterLevels_InvalidNorm_LogsError()
        {
            // Setup
            var mockRepository = new MockRepository();
            var calculationMessageProvider = mockRepository.StrictMock<ICalculationMessageProvider>();
            mockRepository.ReplayAll();

            using (var viewParent = new Form())
            {
                var guiService = new HydraulicBoundaryLocationCalculationGuiService(viewParent);

                // Call
                Action call = () => guiService.CalculateDesignWaterLevels(validFilePath,
                                                                          validPreprocessorDirectory,
                                                                          Enumerable.Empty<HydraulicBoundaryLocationCalculation>(),
                                                                          1.0,
                                                                          calculationMessageProvider);

                // Assert
                TestHelper.AssertLogMessages(call, messages => { Assert.AreEqual("Berekeningen konden niet worden gestart. Doelkans is te groot om een berekening uit te kunnen voeren.", messages.Single()); });
            }

            mockRepository.VerifyAll();
        }

        [Test]
        public void CalculateDesignWaterLevels_ValidPathEmptyList_NoLog()
        {
            // Setup
            var mockRepository = new MockRepository();
            var calculationMessageProvider = mockRepository.StrictMock<ICalculationMessageProvider>();
            mockRepository.ReplayAll();

            DialogBoxHandler = (name, wnd) =>
            {
                // Expect an activity dialog which is automatically closed
            };

            using (var viewParent = new Form())
            {
                var guiService = new HydraulicBoundaryLocationCalculationGuiService(viewParent);

                // Call
                Action call = () => guiService.CalculateDesignWaterLevels(validFilePath,
                                                                          validPreprocessorDirectory,
                                                                          Enumerable.Empty<HydraulicBoundaryLocationCalculation>(),
                                                                          0.01,
                                                                          calculationMessageProvider);

                // Assert
                TestHelper.AssertLogMessagesCount(call, 0);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        public void CalculateDesignWaterLevels_ValidPathOneCalculation_LogsMessages()
        {
            // Setup
            const string hydraulicLocationName = "name";
            const string calculationName = "calculationName";
            const string calculatedNotConvergedMessage = "calculatedNotConvergedMessage";

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateDesignWaterLevelCalculator(testDataPath, validPreprocessorDirectory)).Return(new TestDesignWaterLevelCalculator());
            var calculationMessageProvider = mockRepository.StrictMock<ICalculationMessageProvider>();
            calculationMessageProvider.Expect(calc => calc.GetActivityDescription(hydraulicLocationName)).Return(string.Empty);
            calculationMessageProvider.Expect(calc => calc.GetCalculatedNotConvergedMessage(hydraulicLocationName)).Return(calculatedNotConvergedMessage);
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
                Action call = () => guiService.CalculateDesignWaterLevels(validFilePath,
                                                                          validPreprocessorDirectory,
                                                                          new[]
                                                                          {
                                                                              new HydraulicBoundaryLocationCalculation(new TestHydraulicBoundaryLocation(hydraulicLocationName))
                                                                          },
                                                                          0.01,
                                                                          calculationMessageProvider);

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(8, msgs.Length);
                    StringAssert.AreNotEqualIgnoringCase($"Uitvoeren van '{calculationName}' is gestart.", msgs[0]);
                    CalculationServiceTestHelper.AssertValidationStartMessage(msgs[1]);
                    CalculationServiceTestHelper.AssertValidationEndMessage(msgs[2]);
                    CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[3]);
                    Assert.AreEqual(calculatedNotConvergedMessage, msgs[4]);
                    StringAssert.StartsWith("Waterstand berekening is uitgevoerd op de tijdelijke locatie", msgs[5]);
                    CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[6]);
                    StringAssert.AreNotEqualIgnoringCase($"Uitvoeren van '{calculationName}' is gelukt.", msgs[7]);
                });
            }

            mockRepository.VerifyAll();
        }

        [Test]
        public void CalculateWaveHeights_CalculationsNull_ThrowsArgumentNullException()
        {
            // Setup
            var mockRepository = new MockRepository();
            var calculationMessageProvider = mockRepository.StrictMock<ICalculationMessageProvider>();
            mockRepository.ReplayAll();

            using (var viewParent = new Form())
            {
                var guiService = new HydraulicBoundaryLocationCalculationGuiService(viewParent);

                // Call
                TestDelegate test = () => guiService.CalculateWaveHeights(validFilePath,
                                                                          validPreprocessorDirectory,
                                                                          null,
                                                                          0.01,
                                                                          calculationMessageProvider);

                // Assert
                string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
                const string expectedParamName = "calculations";
                Assert.AreEqual(expectedParamName, paramName);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        public void CalculateWaveHeights_CalculationServiceMessageProviderNull_ThrowsArgumentNullException()
        {
            // Setup
            using (var viewParent = new Form())
            {
                var guiService = new HydraulicBoundaryLocationCalculationGuiService(viewParent);

                // Call
                TestDelegate test = () => guiService.CalculateWaveHeights(validFilePath,
                                                                          validPreprocessorDirectory,
                                                                          Enumerable.Empty<HydraulicBoundaryLocationCalculation>(),
                                                                          0.01,
                                                                          null);

                // Assert
                string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
                const string expectedParamName = "messageProvider";
                Assert.AreEqual(expectedParamName, paramName);
            }
        }

        [Test]
        public void CalculateWaveHeights_HydraulicDatabaseDoesNotExist_LogsError()
        {
            // Setup
            var mockRepository = new MockRepository();
            var calculationMessageProvider = mockRepository.StrictMock<ICalculationMessageProvider>();
            mockRepository.ReplayAll();

            using (var viewParent = new Form())
            {
                var guiService = new HydraulicBoundaryLocationCalculationGuiService(viewParent);

                // Call
                Action call = () => guiService.CalculateWaveHeights("Does not exist",
                                                                    validPreprocessorDirectory,
                                                                    Enumerable.Empty<HydraulicBoundaryLocationCalculation>(),
                                                                    0.01,
                                                                    calculationMessageProvider);

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
        public void CalculateWaveHeights_InvalidNorm_LogsError()
        {
            // Setup
            var mockRepository = new MockRepository();
            var calculationMessageProvider = mockRepository.StrictMock<ICalculationMessageProvider>();
            mockRepository.ReplayAll();

            using (var viewParent = new Form())
            {
                var guiService = new HydraulicBoundaryLocationCalculationGuiService(viewParent);

                // Call
                Action call = () => guiService.CalculateWaveHeights(validFilePath,
                                                                    validPreprocessorDirectory,
                                                                    Enumerable.Empty<HydraulicBoundaryLocationCalculation>(),
                                                                    1.0,
                                                                    calculationMessageProvider);

                // Assert
                TestHelper.AssertLogMessages(call, messages => { Assert.AreEqual("Berekeningen konden niet worden gestart. Doelkans is te groot om een berekening uit te kunnen voeren.", messages.Single()); });
            }

            mockRepository.VerifyAll();
        }

        [Test]
        public void CalculateWaveHeights_ValidPathEmptyList_NoLog()
        {
            // Setup
            var mockRepository = new MockRepository();
            var calculationMessageProvider = mockRepository.StrictMock<ICalculationMessageProvider>();
            mockRepository.ReplayAll();

            DialogBoxHandler = (name, wnd) =>
            {
                // Expect an activity dialog which is automatically closed
            };

            using (var viewParent = new Form())
            {
                var guiService = new HydraulicBoundaryLocationCalculationGuiService(viewParent);

                // Call
                Action call = () => guiService.CalculateWaveHeights(validFilePath,
                                                                    validPreprocessorDirectory,
                                                                    Enumerable.Empty<HydraulicBoundaryLocationCalculation>(),
                                                                    0.01,
                                                                    calculationMessageProvider);

                // Assert
                TestHelper.AssertLogMessagesCount(call, 0);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        public void CalculateWaveHeights_ValidPathOneCalculation_LogsMessages()
        {
            // Setup
            const string hydraulicLocationName = "name";
            const string calculationName = "calculationName";
            const string calculatedNotConvergedMessage = "calculatedNotConvergedMessage";

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateWaveHeightCalculator(testDataPath, validPreprocessorDirectory)).Return(new TestWaveHeightCalculator());
            var calculationMessageProvider = mockRepository.StrictMock<ICalculationMessageProvider>();
            calculationMessageProvider.Expect(calc => calc.GetActivityDescription(hydraulicLocationName)).Return(string.Empty);
            calculationMessageProvider.Expect(calc => calc.GetCalculatedNotConvergedMessage(hydraulicLocationName)).Return(calculatedNotConvergedMessage);
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
                Action call = () => guiService.CalculateWaveHeights(validFilePath,
                                                                    validPreprocessorDirectory,
                                                                    new[]
                                                                    {
                                                                        new HydraulicBoundaryLocationCalculation(new TestHydraulicBoundaryLocation(hydraulicLocationName))
                                                                    },
                                                                    0.01,
                                                                    calculationMessageProvider);

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(8, msgs.Length);
                    StringAssert.AreNotEqualIgnoringCase($"Uitvoeren van '{calculationName}' is gestart.", msgs[0]);
                    CalculationServiceTestHelper.AssertValidationStartMessage(msgs[1]);
                    CalculationServiceTestHelper.AssertValidationEndMessage(msgs[2]);
                    CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[3]);
                    Assert.AreEqual(calculatedNotConvergedMessage, msgs[4]);
                    StringAssert.StartsWith("Golfhoogte berekening is uitgevoerd op de tijdelijke locatie", msgs[5]);
                    CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[6]);
                    StringAssert.AreNotEqualIgnoringCase($"Uitvoeren van '{calculationName}' is gelukt.", msgs[7]);
                });
            }

            mockRepository.VerifyAll();
        }
    }
}