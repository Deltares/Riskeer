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
using Core.Common.Base;
using Core.Common.TestUtil;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.HydraRing.Data;
using Ringtoets.Integration.Forms.GuiServices;

namespace Ringtoets.Integration.Forms.Test.GuiServices
{
    [TestFixture]
    public class HydraulicBoundaryLocationGuiServiceTest : NUnitFormTest
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
        public void CalculateDesignWaterLevels_NullHydraulicBoundaryDatabase_ThrowsArgumentNullException()
        {
            // Setup
            var locations = Enumerable.Empty<HydraulicBoundaryLocation>();
            using (var viewParent = new Form())
            {
                var guiService = new HydraulicBoundaryLocationCalculationGuiService(viewParent);

                // Call
                TestDelegate test = () => guiService.CalculateDesignWaterLevels(null, locations, "", 1);

                // Assert
                string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
                const string expectedParamName = "hydraulicBoundaryDatabase";
                Assert.AreEqual(expectedParamName, paramName);
            }
        }

        [Test]
        public void CalculateDesignWaterLevels_NullLocations_ThrowsArgumentNullException()
        {
            // Setup
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();

            using (var viewParent = new Form())
            {
                var guiService = new HydraulicBoundaryLocationCalculationGuiService(viewParent);

                // Call
                TestDelegate test = () => guiService.CalculateDesignWaterLevels(hydraulicBoundaryDatabase, null, "", 1);

                // Assert
                string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
                const string expectedParamName = "locations";
                Assert.AreEqual(expectedParamName, paramName);
            }
        }

        [Test]
        public void CalculateDesignWaterLevels_HydraulicDatabaseDoesNotExist_LogsErrorAndDoesNotNotifyObservers()
        {
            // Setup
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                FilePath = "Does not exist"
            };

            var observerMock = mockRepository.StrictMock<IObserver>();
            mockRepository.ReplayAll();

            hydraulicBoundaryDatabase.Attach(observerMock);

            var locations = Enumerable.Empty<HydraulicBoundaryLocation>();

            using (var viewParent = new Form())
            {
                var guiService = new HydraulicBoundaryLocationCalculationGuiService(viewParent);

                // Call
                Action call = () => guiService.CalculateDesignWaterLevels(hydraulicBoundaryDatabase, locations, "", 1);

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
        public void CalculateDesignWaterLevels_ValidPathEmptyList_NotifyObserversButNoLog()
        {
            // Setup
            string validFilePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                FilePath = validFilePath
            };

            var observerMock = mockRepository.StrictMock<IObserver>();
            observerMock.Expect(o => o.UpdateObserver());
            mockRepository.ReplayAll();

            hydraulicBoundaryDatabase.Attach(observerMock);

            DialogBoxHandler = (name, wnd) =>
            {
                // Expect an activity dialog which is automatically closed
            };

            var locations = Enumerable.Empty<HydraulicBoundaryLocation>();
            using (var viewParent = new Form())
            {
                var guiService = new HydraulicBoundaryLocationCalculationGuiService(viewParent);

                // Call
                Action call = () => guiService.CalculateDesignWaterLevels(hydraulicBoundaryDatabase, locations, "", 1);

                // Assert
                TestHelper.AssertLogMessagesCount(call, 0);
            }
            mockRepository.VerifyAll();
        }

        [Test]
        public void CalculateDesignWaterLevels_ValidPathOneLocationInTheList_NotifyObserversAndLogsMessages()
        {
            // Setup
            string validFilePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                FilePath = validFilePath
            };

            var observerMock = mockRepository.StrictMock<IObserver>();
            observerMock.Expect(o => o.UpdateObserver());
            mockRepository.ReplayAll();

            hydraulicBoundaryDatabase.Attach(observerMock);

            DialogBoxHandler = (name, wnd) =>
            {
                // Expect an activity dialog which is automatically closed
            };

            const string hydraulicLocationName = "name";
            var locations = new List<HydraulicBoundaryLocation>
            {
                new HydraulicBoundaryLocation(1, hydraulicLocationName, 2, 3)
            };
            using (var viewParent = new Form())
            {
                var guiService = new HydraulicBoundaryLocationCalculationGuiService(viewParent);

                // Call
                Action call = () => guiService.CalculateDesignWaterLevels(hydraulicBoundaryDatabase, locations, "", 1);

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    var msgs = messages.ToArray();
                    Assert.AreEqual(6, msgs.Length);
                    string expectedName = string.Format("Toetspeil voor locatie {0}", hydraulicLocationName);
                    StringAssert.StartsWith(string.Format("Validatie van '{0}' gestart om: ", expectedName), msgs.First());
                    StringAssert.StartsWith(string.Format("Validatie van '{0}' beëindigd om: ", expectedName), msgs.Skip(1).First());
                    StringAssert.StartsWith(string.Format("Berekening van '{0}' gestart om: ", expectedName), msgs.Skip(2).First());
                    StringAssert.StartsWith(string.Format("Berekening van '{0}' beëindigd om: ", expectedName), msgs.Skip(3).First());
                    StringAssert.AreNotEqualIgnoringCase(string.Format("Uitvoeren van '{0}' is mislukt.", expectedName), msgs.Last());
                });
            }
            mockRepository.VerifyAll();
        }

        [Test]
        public void CalculateWaveHeights_NullHydraulicBoundaryDatabase_ThrowsArgumentNullException()
        {
            // Setup
            var locations = Enumerable.Empty<HydraulicBoundaryLocation>();

            using (var viewParent = new Form())
            {
                var guiService = new HydraulicBoundaryLocationCalculationGuiService(viewParent);

                // Call
                TestDelegate test = () => guiService.CalculateWaveHeights(null, locations, "", 1);

                // Assert
                string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
                const string expectedParamName = "hydraulicBoundaryDatabase";
                Assert.AreEqual(expectedParamName, paramName);
            }
        }

        [Test]
        public void CalculateWaveHeights_NullLocations_ThrowsArgumentNullException()
        {
            // Setup
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();

            using (var viewParent = new Form())
            {
                var guiService = new HydraulicBoundaryLocationCalculationGuiService(viewParent);

                // Call
                TestDelegate test = () => guiService.CalculateWaveHeights(hydraulicBoundaryDatabase, null, "", 1);

                // Assert
                string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
                const string expectedParamName = "locations";
                Assert.AreEqual(expectedParamName, paramName);
            }
        }

        [Test]
        public void CalculateWaveHeights_HydraulicDatabaseDoesNotExist_LogsErrorAndDoesNotNotifyObservers()
        {
            // Setup
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                FilePath = "Does not exist"
            };

            var observerMock = mockRepository.StrictMock<IObserver>();
            mockRepository.ReplayAll();
            hydraulicBoundaryDatabase.Attach(observerMock);

            var locations = Enumerable.Empty<HydraulicBoundaryLocation>();

            using (var viewParent = new Form())
            {
                var guiService = new HydraulicBoundaryLocationCalculationGuiService(viewParent);

                // Call
                Action call = () => guiService.CalculateWaveHeights(hydraulicBoundaryDatabase, locations, "", 1);

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
        public void CalculateWaveHeights_ValidPathEmptyList_NotifyObserversButNoLog()
        {
            // Setup
            string validFilePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                FilePath = validFilePath
            };

            var observerMock = mockRepository.StrictMock<IObserver>();
            observerMock.Expect(o => o.UpdateObserver());
            mockRepository.ReplayAll();

            hydraulicBoundaryDatabase.Attach(observerMock);

            DialogBoxHandler = (name, wnd) =>
            {
                // Expect an activity dialog which is automatically closed
            };

            var locations = Enumerable.Empty<HydraulicBoundaryLocation>();

            using (var viewParent = new Form())
            {
                var guiService = new HydraulicBoundaryLocationCalculationGuiService(viewParent);

                // Call
                Action call = () => guiService.CalculateWaveHeights(hydraulicBoundaryDatabase, locations, "", 1);

                // Assert
                TestHelper.AssertLogMessagesCount(call, 0);
            }
            mockRepository.VerifyAll();
        }

        [Test]
        public void CalculateWaveHeights_ValidPathOneLocationInTheList_NotifyObserversAndLogsMessages()
        {
            // Setup
            string validFilePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                FilePath = validFilePath
            };

            var observerMock = mockRepository.StrictMock<IObserver>();
            observerMock.Expect(o => o.UpdateObserver());
            mockRepository.ReplayAll();

            hydraulicBoundaryDatabase.Attach(observerMock);

            DialogBoxHandler = (name, wnd) =>
            {
                // Expect an activity dialog which is automatically closed
            };

            const string hydraulicLocationName = "name";
            var locations = new List<HydraulicBoundaryLocation>
            {
                new HydraulicBoundaryLocation(1, hydraulicLocationName, 2, 3)
            };
            using (var viewParent = new Form())
            {
                var guiService = new HydraulicBoundaryLocationCalculationGuiService(viewParent);

                // Call
                Action call = () => guiService.CalculateWaveHeights(hydraulicBoundaryDatabase, locations, "", 1);

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    var msgs = messages.ToArray();
                    Assert.AreEqual(6, msgs.Length);
                    string expectedName = string.Format("Golfhoogte voor locatie {0}", hydraulicLocationName);
                    StringAssert.StartsWith(string.Format("Validatie van '{0}' gestart om: ", expectedName), msgs.First());
                    StringAssert.StartsWith(string.Format("Validatie van '{0}' beëindigd om: ", expectedName), msgs.Skip(1).First());
                    StringAssert.StartsWith(string.Format("Berekening van '{0}' gestart om: ", expectedName), msgs.Skip(2).First());
                    StringAssert.StartsWith(string.Format("Berekening van '{0}' beëindigd om: ", expectedName), msgs.Skip(3).First());
                    StringAssert.AreNotEqualIgnoringCase(string.Format("Uitvoeren van '{0}' is mislukt.", expectedName), msgs.Last());
                });
            }
            mockRepository.VerifyAll();
        }
    }
}