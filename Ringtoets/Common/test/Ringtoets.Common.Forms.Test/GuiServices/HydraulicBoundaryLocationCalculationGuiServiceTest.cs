// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Core.Common.TestUtil;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Forms.GuiServices;
using Ringtoets.Common.Service.MessageProviders;
using Ringtoets.HydraRing.Calculation.TestUtil.Calculator;
using Ringtoets.HydraRing.Data;

namespace Ringtoets.Common.Forms.Test.GuiServices
{
    [TestFixture]
    public class HydraulicBoundaryLocationCalculationGuiServiceTest : NUnitFormTest
    {
        private MockRepository mockRepository;
        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Integration.Service, "HydraRingCalculation");

        [SetUp]
        public void SetUp()
        {
            mockRepository = new MockRepository();
        }

        [Test]
        public void Constructor_NullMainWindow_ThrowsArgumentNullException()
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
                Assert.IsInstanceOf<HydraulicBoundaryLocationCalculationGuiService>(guiService);
            }
        }

        [Test]
        public void CalculateDesignWaterLevels_NullCalculationServiceMessageProvider_ThrowsArgumentNullException()
        {
            // Setup
            using (var viewParent = new Form())
            using (new HydraRingCalculatorFactoryConfig())
            {
                var guiService = new HydraulicBoundaryLocationCalculationGuiService(viewParent);

                // Call
                TestDelegate test = () => guiService.CalculateDesignWaterLevels(null, Enumerable.Empty<HydraulicBoundaryLocation>(), "", 1, null);

                // Assert
                string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
                const string expectedParamName = "messageProvider";
                Assert.AreEqual(expectedParamName, paramName);
            }
        }

        [Test]
        public void CalculateDesignWaterLevels_NullLocations_ThrowsArgumentNullException()
        {
            // Setup
            var calculationMessageProviderMock = mockRepository.StrictMock<ICalculationMessageProvider>();
            mockRepository.ReplayAll();
            const string databasePath = "Does not exist";

            using (var viewParent = new Form())
            using (new HydraRingCalculatorFactoryConfig())
            {
                var guiService = new HydraulicBoundaryLocationCalculationGuiService(viewParent);

                // Call
                TestDelegate test = () => guiService.CalculateDesignWaterLevels(databasePath, null, "", 1, calculationMessageProviderMock);

                // Assert
                string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
                const string expectedParamName = "locations";
                Assert.AreEqual(expectedParamName, paramName);
            }
            mockRepository.VerifyAll();
        }

        [Test]
        public void CalculateDesignWaterLevels_HydraulicDatabaseDoesNotExist_LogsError()
        {
            // Setup
            var calculationMessageProviderMock = mockRepository.StrictMock<ICalculationMessageProvider>();
            mockRepository.ReplayAll();

            const string databasePath = "Does not exist";
            using (var viewParent = new Form())
            using (new HydraRingCalculatorFactoryConfig())
            {
                var guiService = new HydraulicBoundaryLocationCalculationGuiService(viewParent);

                // Call
                Action call = () => guiService.CalculateDesignWaterLevels(databasePath, Enumerable.Empty<HydraulicBoundaryLocation>(), "", 1, calculationMessageProviderMock);

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    var msgs = messages.ToArray();
                    Assert.AreEqual(1, msgs.Length);
                    StringAssert.StartsWith("Berekeningen konden niet worden gestart. ", msgs.First());
                });
            }
            mockRepository.VerifyAll();
        }

        [Test]
        public void CalculateDesignWaterLevels_HydraulicDatabaseDoesNotExist_SuccessfulCalculationFalse()
        {
            // Setup
            var calculationMessageProviderMock = mockRepository.StrictMock<ICalculationMessageProvider>();
            mockRepository.ReplayAll();

            const string databasePath = "Does not exist";
            using (var viewParent = new Form())
            using (new HydraRingCalculatorFactoryConfig())
            {
                var guiService = new HydraulicBoundaryLocationCalculationGuiService(viewParent);

                // Call
                bool successfulCalculation = guiService.CalculateDesignWaterLevels(databasePath, Enumerable.Empty<HydraulicBoundaryLocation>(), "", 1, calculationMessageProviderMock);

                // Assert
                Assert.IsFalse(successfulCalculation);
            }
            mockRepository.VerifyAll();
        }

        [Test]
        public void CalculateDesignWaterLevels_ValidPathEmptyList_NoLog()
        {
            // Setup
            string validDatabasePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");

            var calculationMessageProviderMock = mockRepository.StrictMock<ICalculationMessageProvider>();
            mockRepository.ReplayAll();

            DialogBoxHandler = (name, wnd) =>
            {
                // Expect an activity dialog which is automatically closed
            };

            using (var viewParent = new Form())
            {
                var guiService = new HydraulicBoundaryLocationCalculationGuiService(viewParent);

                // Call
                Action call = () => guiService.CalculateDesignWaterLevels(validDatabasePath, Enumerable.Empty<HydraulicBoundaryLocation>(), "", 1, calculationMessageProviderMock);

                // Assert
                TestHelper.AssertLogMessagesCount(call, 0);
            }
            mockRepository.VerifyAll();
        }

        [Test]
        public void CalculateDesignWaterLevels_ValidPathEmptyList_SuccessfulCalculationTrue()
        {
            // Setup
            string validDatabasePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");

            var calculationMessageProviderMock = mockRepository.StrictMock<ICalculationMessageProvider>();
            mockRepository.ReplayAll();

            DialogBoxHandler = (name, wnd) =>
            {
                // Expect an activity dialog which is automatically closed
            };

            using (var viewParent = new Form())
            using (new HydraRingCalculatorFactoryConfig())
            {
                var guiService = new HydraulicBoundaryLocationCalculationGuiService(viewParent);

                // Call
                bool successfulCalculation = guiService.CalculateDesignWaterLevels(validDatabasePath, Enumerable.Empty<HydraulicBoundaryLocation>(), "", 1, calculationMessageProviderMock);

                // Assert
                Assert.IsTrue(successfulCalculation);
            }
            mockRepository.VerifyAll();
        }

        [Test]
        public void CalculateDesignWaterLevels_ValidPathOneLocationInTheList_LogsMessages()
        {
            // Setup
            const string hydraulicLocationName = "name";
            const string calculationName = "calculationName";
            const string notConvergedMessage = "not converged";
            string validDatabasePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");

            var calculationMessageProviderMock = mockRepository.StrictMock<ICalculationMessageProvider>();
            calculationMessageProviderMock.Expect(calc => calc.GetActivityName(hydraulicLocationName)).Return("GetActivityName");
            calculationMessageProviderMock.Expect(calc => calc.GetCalculationName(hydraulicLocationName)).Return(calculationName).Repeat.AtLeastOnce();
            calculationMessageProviderMock.Expect(calc => calc.GetCalculatedNotConvergedMessage(hydraulicLocationName)).Return(notConvergedMessage);
            mockRepository.ReplayAll();

            DialogBoxHandler = (name, wnd) =>
            {
                // Expect an activity dialog which is automatically closed
            };

            using (var viewParent = new Form())
            using (new HydraRingCalculatorFactoryConfig())
            {
                var guiService = new HydraulicBoundaryLocationCalculationGuiService(viewParent);

                // Call
                Action call = () => guiService.CalculateDesignWaterLevels(validDatabasePath,
                                                                          new List<HydraulicBoundaryLocation>
                                                                          {
                                                                              new HydraulicBoundaryLocation(1, hydraulicLocationName, 2, 3)
                                                                          },
                                                                          "", 1, calculationMessageProviderMock);

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    var msgs = messages.ToArray();
                    Assert.AreEqual(7, msgs.Length);
                    StringAssert.StartsWith(string.Format("Validatie van '{0}' gestart om: ", calculationName), msgs[0]);
                    StringAssert.StartsWith(string.Format("Validatie van '{0}' beëindigd om: ", calculationName), msgs[1]);
                    StringAssert.StartsWith(string.Format("Berekening van '{0}' gestart om: ", calculationName), msgs[2]);
                    Assert.AreEqual(notConvergedMessage, msgs[3]);
                    StringAssert.StartsWith("Toetspeil berekening is uitgevoerd op de tijdelijke locatie", msgs[4]);
                    StringAssert.StartsWith(string.Format("Berekening van '{0}' beëindigd om: ", calculationName), msgs[5]);
                    StringAssert.AreNotEqualIgnoringCase(string.Format("Uitvoeren van '{0}' is gelukt.", calculationName), msgs[6]);
                });
            }
            mockRepository.VerifyAll();
        }

        [Test]
        public void CalculateDesignWaterLevels_ValidPathOneLocationInTheList_SuccessfulCalculationTrue()
        {
            // Setup
            const string hydraulicLocationName = "name";
            const string calculationName = "calculationName";
            string validDatabasePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");

            var calculationMessageProviderMock = mockRepository.StrictMock<ICalculationMessageProvider>();
            calculationMessageProviderMock.Expect(calc => calc.GetActivityName(hydraulicLocationName)).Return("GetActivityName");
            calculationMessageProviderMock.Expect(calc => calc.GetCalculationName(hydraulicLocationName)).Return(calculationName).Repeat.AtLeastOnce();
            calculationMessageProviderMock.Expect(calc => calc.GetCalculatedNotConvergedMessage(hydraulicLocationName)).Return("not converged");
            mockRepository.ReplayAll();

            DialogBoxHandler = (name, wnd) =>
            {
                // Expect an activity dialog which is automatically closed
            };

            using (var viewParent = new Form())
            using (new HydraRingCalculatorFactoryConfig())
            {
                var guiService = new HydraulicBoundaryLocationCalculationGuiService(viewParent);

                // Call
                bool successfulCalculation = guiService.CalculateDesignWaterLevels(validDatabasePath,
                                                                                   new List<HydraulicBoundaryLocation>
                                                                                   {
                                                                                       new HydraulicBoundaryLocation(1, hydraulicLocationName, 2, 3)
                                                                                   },
                                                                                   "", 1, calculationMessageProviderMock);

                // Assert
                Assert.IsTrue(successfulCalculation);
            }
            mockRepository.VerifyAll();
        }

        [Test]
        public void CalculateWaveHeights_NullCalculationServiceMessageProvider_ThrowsArgumentNullException()
        {
            // Setup
            using (var viewParent = new Form())
            using (new HydraRingCalculatorFactoryConfig())
            {
                var guiService = new HydraulicBoundaryLocationCalculationGuiService(viewParent);

                // Call
                TestDelegate test = () => guiService.CalculateWaveHeights(null, Enumerable.Empty<HydraulicBoundaryLocation>(), "", 1, null);

                // Assert
                string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
                const string expectedParamName = "messageProvider";
                Assert.AreEqual(expectedParamName, paramName);
            }
        }

        [Test]
        public void CalculateWaveHeights_NullLocations_ThrowsArgumentNullException()
        {
            // Setup
            var calculationMessageProviderMock = mockRepository.StrictMock<ICalculationMessageProvider>();
            mockRepository.ReplayAll();
            const string databasePath = "Does not exist";

            using (var viewParent = new Form())
            using (new HydraRingCalculatorFactoryConfig())
            {
                var guiService = new HydraulicBoundaryLocationCalculationGuiService(viewParent);

                // Call
                TestDelegate test = () => guiService.CalculateWaveHeights(databasePath, null, "", 1, calculationMessageProviderMock);

                // Assert
                string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
                const string expectedParamName = "locations";
                Assert.AreEqual(expectedParamName, paramName);
            }
            mockRepository.VerifyAll();
        }

        [Test]
        public void CalculateWaveHeights_HydraulicDatabaseDoesNotExist_LogsError()
        {
            // Setup
            var calculationMessageProviderMock = mockRepository.StrictMock<ICalculationMessageProvider>();
            mockRepository.ReplayAll();

            const string databasePath = "Does not exist";
            using (var viewParent = new Form())
            using (new HydraRingCalculatorFactoryConfig())
            {
                var guiService = new HydraulicBoundaryLocationCalculationGuiService(viewParent);

                // Call
                Action call = () => guiService.CalculateWaveHeights(databasePath, Enumerable.Empty<HydraulicBoundaryLocation>(), "", 1, calculationMessageProviderMock);

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    var msgs = messages.ToArray();
                    Assert.AreEqual(1, msgs.Length);
                    StringAssert.StartsWith("Berekeningen konden niet worden gestart. ", msgs.First());
                });
            }
            mockRepository.VerifyAll();
        }

        [Test]
        public void CalculateWaveHeights_HydraulicDatabaseDoesNotExist_SuccessfulCalculationFalse()
        {
            // Setup
            var calculationMessageProviderMock = mockRepository.StrictMock<ICalculationMessageProvider>();
            mockRepository.ReplayAll();

            const string databasePath = "Does not exist";
            using (var viewParent = new Form())
            using (new HydraRingCalculatorFactoryConfig())
            {
                var guiService = new HydraulicBoundaryLocationCalculationGuiService(viewParent);

                // Call
                bool successfulCalculation = guiService.CalculateWaveHeights(databasePath, Enumerable.Empty<HydraulicBoundaryLocation>(), "", 1, calculationMessageProviderMock);

                // Assert
                Assert.IsFalse(successfulCalculation);
            }
            mockRepository.VerifyAll();
        }

        [Test]
        public void CalculateWaveHeights_ValidPathEmptyList_NoLog()
        {
            // Setup
            string validDatabasePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");

            var calculationMessageProviderMock = mockRepository.StrictMock<ICalculationMessageProvider>();
            mockRepository.ReplayAll();

            DialogBoxHandler = (name, wnd) =>
            {
                // Expect an activity dialog which is automatically closed
            };

            using (var viewParent = new Form())
            using (new HydraRingCalculatorFactoryConfig())
            {
                var guiService = new HydraulicBoundaryLocationCalculationGuiService(viewParent);

                // Call
                Action call = () => guiService.CalculateWaveHeights(validDatabasePath, Enumerable.Empty<HydraulicBoundaryLocation>(), "", 1, calculationMessageProviderMock);

                // Assert
                TestHelper.AssertLogMessagesCount(call, 0);
            }
            mockRepository.VerifyAll();
        }

        [Test]
        public void CalculateWaveHeights_ValidPathEmptyList_SuccessfulCalculationTrue()
        {
            // Setup
            string validDatabasePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");

            var calculationMessageProviderMock = mockRepository.StrictMock<ICalculationMessageProvider>();
            mockRepository.ReplayAll();

            DialogBoxHandler = (name, wnd) =>
            {
                // Expect an activity dialog which is automatically closed
            };

            using (var viewParent = new Form())
            using (new HydraRingCalculatorFactoryConfig())
            {
                var guiService = new HydraulicBoundaryLocationCalculationGuiService(viewParent);

                // Call
                bool succesfulCalculation = guiService.CalculateWaveHeights(validDatabasePath, Enumerable.Empty<HydraulicBoundaryLocation>(), "", 1, calculationMessageProviderMock);

                // Assert
                Assert.IsTrue(succesfulCalculation);
            }
            mockRepository.VerifyAll();
        }

        [Test]
        public void CalculateWaveHeights_ValidPathOneLocationInTheList_LogsMessages()
        {
            // Setup
            const string hydraulicLocationName = "name";
            const string calculationName = "calculationName";
            const string notConvergedMessage = "not converged";
            string validDatabasePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");

            var calculationMessageProviderMock = mockRepository.StrictMock<ICalculationMessageProvider>();
            calculationMessageProviderMock.Expect(calc => calc.GetActivityName(hydraulicLocationName)).Return("GetActivityName");
            calculationMessageProviderMock.Expect(calc => calc.GetCalculationName(hydraulicLocationName)).Return(calculationName).Repeat.AtLeastOnce();
            calculationMessageProviderMock.Expect(calc => calc.GetCalculatedNotConvergedMessage(hydraulicLocationName)).Return(notConvergedMessage);
            mockRepository.ReplayAll();

            DialogBoxHandler = (name, wnd) =>
            {
                // Expect an activity dialog which is automatically closed
            };

            using (var viewParent = new Form())
            using (new HydraRingCalculatorFactoryConfig())
            {
                var guiService = new HydraulicBoundaryLocationCalculationGuiService(viewParent);

                // Call
                Action call = () => guiService.CalculateWaveHeights(validDatabasePath,
                                                                    new List<HydraulicBoundaryLocation>()
                                                                    {
                                                                        new HydraulicBoundaryLocation(1, hydraulicLocationName, 2, 3)
                                                                    },
                                                                    "", 1, calculationMessageProviderMock);

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    var msgs = messages.ToArray();
                    Assert.AreEqual(7, msgs.Length);
                    StringAssert.StartsWith(string.Format("Validatie van '{0}' gestart om: ", calculationName), msgs[0]);
                    StringAssert.StartsWith(string.Format("Validatie van '{0}' beëindigd om: ", calculationName), msgs[1]);
                    StringAssert.StartsWith(string.Format("Berekening van '{0}' gestart om: ", calculationName), msgs[2]);
                    Assert.AreEqual(notConvergedMessage, msgs[3]);
                    StringAssert.StartsWith("Golfhoogte berekening is uitgevoerd op de tijdelijke locatie", msgs[4]);
                    StringAssert.StartsWith(string.Format("Berekening van '{0}' beëindigd om: ", calculationName), msgs[5]);
                    StringAssert.AreNotEqualIgnoringCase(string.Format("Uitvoeren van '{0}' is gelukt.", calculationName), msgs[6]);
                });
            }
            mockRepository.VerifyAll();
        }

        [Test]
        public void CalculateWaveHeights_ValidPathOneLocationInTheList_SuccessfulCalculationTrue()
        {
            // Setup
            const string hydraulicLocationName = "name";
            const string calculationName = "calculationName";
            string validDatabasePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");

            var calculationMessageProviderMock = mockRepository.StrictMock<ICalculationMessageProvider>();
            calculationMessageProviderMock.Expect(calc => calc.GetActivityName(hydraulicLocationName)).Return("GetActivityName");
            calculationMessageProviderMock.Expect(calc => calc.GetCalculationName(hydraulicLocationName)).Return(calculationName).Repeat.AtLeastOnce();
            calculationMessageProviderMock.Expect(calc => calc.GetCalculatedNotConvergedMessage(hydraulicLocationName)).Return("not converged");
            mockRepository.ReplayAll();

            DialogBoxHandler = (name, wnd) =>
            {
                // Expect an activity dialog which is automatically closed
            };

            using (var viewParent = new Form())
            using (new HydraRingCalculatorFactoryConfig())
            {
                var guiService = new HydraulicBoundaryLocationCalculationGuiService(viewParent);

                // Call
                bool successfulCalculation = guiService.CalculateWaveHeights(validDatabasePath,
                                                                             new List<HydraulicBoundaryLocation>
                                                                             {
                                                                                 new HydraulicBoundaryLocation(1, hydraulicLocationName, 2, 3)
                                                                             },
                                                                             "", 1, calculationMessageProviderMock);

                // Assert
                Assert.IsTrue(successfulCalculation);
            }
            mockRepository.VerifyAll();
        }
    }
}