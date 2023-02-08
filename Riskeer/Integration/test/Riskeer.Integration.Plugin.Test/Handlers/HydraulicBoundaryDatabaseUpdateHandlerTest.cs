// Copyright (C) Stichting Deltares 2022. All rights reserved.
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
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base;
using Core.Common.TestUtil;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.TestUtil;
using Riskeer.DuneErosion.Plugin.Handlers;
using Riskeer.HydraRing.IO.HydraulicBoundaryDatabase;
using Riskeer.HydraRing.IO.HydraulicLocationConfigurationDatabase;
using Riskeer.HydraRing.IO.TestUtil;
using Riskeer.Integration.Data;
using Riskeer.Integration.IO.Handlers;
using Riskeer.Integration.Plugin.Handlers;
using Riskeer.Integration.TestUtil;
using Riskeer.MacroStabilityInwards.Data;
using Riskeer.Piping.Data.SemiProbabilistic;
using Riskeer.Piping.Data.TestUtil;

namespace Riskeer.Integration.Plugin.Test.Handlers
{
    [TestFixture]
    public class HydraulicBoundaryDatabaseUpdateHandlerTest : NUnitFormTest
    {
        [Test]
        public void Constructor_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var duneLocationsReplacementHandler = mocks.Stub<IDuneLocationsReplacementHandler>();
            mocks.ReplayAll();

            // Call
            void Call() => new HydraulicBoundaryDatabaseUpdateHandler(null, duneLocationsReplacementHandler);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_DuneLocationsReplacementHandlerNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new HydraulicBoundaryDatabaseUpdateHandler(CreateAssessmentSection(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("duneLocationsReplacementHandler", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var duneLocationsReplacementHandler = mocks.Stub<IDuneLocationsReplacementHandler>();
            mocks.ReplayAll();

            // Call
            var handler = new HydraulicBoundaryDatabaseUpdateHandler(CreateAssessmentSection(), duneLocationsReplacementHandler);

            // Assert
            Assert.IsInstanceOf<IHydraulicBoundaryDatabaseUpdateHandler>(handler);
            mocks.VerifyAll();
        }

        [Test]
        public void IsConfirmationRequired_HydraulicBoundaryDatabaseNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var duneLocationsReplacementHandler = mocks.Stub<IDuneLocationsReplacementHandler>();
            mocks.ReplayAll();

            var handler = new HydraulicBoundaryDatabaseUpdateHandler(CreateAssessmentSection(), duneLocationsReplacementHandler);

            // Call
            void Call() => handler.IsConfirmationRequired(null, ReadHydraulicBoundaryDatabaseTestFactory.Create());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("hydraulicBoundaryDatabase", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void IsConfirmationRequired_ReadHydraulicBoundaryDatabaseNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var duneLocationsReplacementHandler = mocks.Stub<IDuneLocationsReplacementHandler>();
            mocks.ReplayAll();

            var handler = new HydraulicBoundaryDatabaseUpdateHandler(CreateAssessmentSection(), duneLocationsReplacementHandler);

            // Call
            void Call() => handler.IsConfirmationRequired(new HydraulicBoundaryDatabase(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("readHydraulicBoundaryDatabase", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void IsConfirmationRequired_HydraulicBoundaryDatabaseNotLinked_ReturnsFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var duneLocationsReplacementHandler = mocks.Stub<IDuneLocationsReplacementHandler>();
            mocks.ReplayAll();

            var handler = new HydraulicBoundaryDatabaseUpdateHandler(CreateAssessmentSection(), duneLocationsReplacementHandler);

            // Call
            bool confirmationRequired = handler.IsConfirmationRequired(new HydraulicBoundaryDatabase(), ReadHydraulicBoundaryDatabaseTestFactory.Create());

            // Assert
            Assert.IsFalse(confirmationRequired);
            mocks.VerifyAll();
        }

        [Test]
        public void IsConfirmationRequired_HydraulicBoundaryDatabaseLinkedAndReadDatabaseSameVersion_ReturnsFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var duneLocationsReplacementHandler = mocks.Stub<IDuneLocationsReplacementHandler>();
            mocks.ReplayAll();

            ReadHydraulicBoundaryDatabase readHydraulicBoundaryDatabase = ReadHydraulicBoundaryDatabaseTestFactory.Create();

            var handler = new HydraulicBoundaryDatabaseUpdateHandler(CreateAssessmentSection(), duneLocationsReplacementHandler);
            var database = new HydraulicBoundaryDatabase
            {
                FilePath = "some/file/path",
                Version = readHydraulicBoundaryDatabase.Version
            };

            // Call
            bool confirmationRequired = handler.IsConfirmationRequired(database, readHydraulicBoundaryDatabase);

            // Assert
            Assert.IsFalse(confirmationRequired);
            mocks.VerifyAll();
        }

        [Test]
        public void IsConfirmationRequired_HydraulicBoundaryDatabaseLinkedAndReadDatabaseDifferentVersion_ReturnsTrue()
        {
            // Setup
            var mocks = new MockRepository();
            var duneLocationsReplacementHandler = mocks.Stub<IDuneLocationsReplacementHandler>();
            mocks.ReplayAll();

            var handler = new HydraulicBoundaryDatabaseUpdateHandler(CreateAssessmentSection(), duneLocationsReplacementHandler);
            var database = new HydraulicBoundaryDatabase
            {
                FilePath = "some/file/path",
                Version = "1"
            };

            // Call
            bool confirmationRequired = handler.IsConfirmationRequired(database, ReadHydraulicBoundaryDatabaseTestFactory.Create());

            // Assert
            Assert.IsTrue(confirmationRequired);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void InquireConfirmation_ClickDialog_ReturnsExpectedResult(bool clickOk)
        {
            // Setup
            var mocks = new MockRepository();
            var duneLocationsReplacementHandler = mocks.Stub<IDuneLocationsReplacementHandler>();
            mocks.ReplayAll();

            string dialogTitle = null, dialogMessage = null;
            DialogBoxHandler = (name, wnd) =>
            {
                var tester = new MessageBoxTester(wnd);
                dialogTitle = tester.Title;
                dialogMessage = tester.Text;
                if (clickOk)
                {
                    tester.ClickOk();
                }
                else
                {
                    tester.ClickCancel();
                }
            };

            var handler = new HydraulicBoundaryDatabaseUpdateHandler(CreateAssessmentSection(), duneLocationsReplacementHandler);

            // Call
            bool result = handler.InquireConfirmation();

            // Assert
            Assert.AreEqual(clickOk, result);

            Assert.AreEqual("Bevestigen", dialogTitle);
            Assert.AreEqual("U heeft een ander hydraulische belastingendatabase bestand geselecteerd. Als gevolg hiervan moet de uitvoer van alle ervan afhankelijke berekeningen verwijderd worden." +
                            Environment.NewLine +
                            Environment.NewLine +
                            "Wilt u doorgaan?",
                            dialogMessage);
            mocks.VerifyAll();
        }

        [Test]
        public void Update_HydraulicBoundaryDatabaseNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var duneLocationsReplacementHandler = mocks.Stub<IDuneLocationsReplacementHandler>();
            mocks.ReplayAll();

            var handler = new HydraulicBoundaryDatabaseUpdateHandler(CreateAssessmentSection(), duneLocationsReplacementHandler);

            ReadHydraulicBoundaryDatabase readHydraulicBoundaryDatabase = ReadHydraulicBoundaryDatabaseTestFactory.Create();

            // Call
            void Call()
            {
                handler.Update(null, readHydraulicBoundaryDatabase, ReadHydraulicLocationConfigurationDatabaseTestFactory.Create(readHydraulicBoundaryDatabase.TrackId),
                               Enumerable.Empty<long>(), "", "");
            }

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("hydraulicBoundaryDatabase", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Update_ReadHydraulicBoundaryDatabaseNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var duneLocationsReplacementHandler = mocks.Stub<IDuneLocationsReplacementHandler>();
            mocks.ReplayAll();

            var handler = new HydraulicBoundaryDatabaseUpdateHandler(CreateAssessmentSection(), duneLocationsReplacementHandler);

            // Call
            void Call() => handler.Update(new HydraulicBoundaryDatabase(), null,
                                          ReadHydraulicLocationConfigurationDatabaseTestFactory.Create(1000),
                                          Enumerable.Empty<long>(), "", "");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("readHydraulicBoundaryDatabase", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Update_ReadHydraulicLocationConfigurationDatabaseNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var duneLocationsReplacementHandler = mocks.Stub<IDuneLocationsReplacementHandler>();
            mocks.ReplayAll();

            var handler = new HydraulicBoundaryDatabaseUpdateHandler(CreateAssessmentSection(), duneLocationsReplacementHandler);

            // Call
            void Call() => handler.Update(new HydraulicBoundaryDatabase(), ReadHydraulicBoundaryDatabaseTestFactory.Create(),
                                          null, Enumerable.Empty<long>(), "", "");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("readHydraulicLocationConfigurationDatabase", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Update_ExcludedLocationIdsNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var duneLocationsReplacementHandler = mocks.Stub<IDuneLocationsReplacementHandler>();
            mocks.ReplayAll();

            var handler = new HydraulicBoundaryDatabaseUpdateHandler(CreateAssessmentSection(), duneLocationsReplacementHandler);

            ReadHydraulicBoundaryDatabase readHydraulicBoundaryDatabase = ReadHydraulicBoundaryDatabaseTestFactory.Create();

            // Call
            void Call()
            {
                handler.Update(new HydraulicBoundaryDatabase(), readHydraulicBoundaryDatabase,
                               ReadHydraulicLocationConfigurationDatabaseTestFactory.Create(readHydraulicBoundaryDatabase.TrackId),
                               null, "", "");
            }

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("excludedLocationIds", exception.ParamName);
        }

        [Test]
        public void Update_HydraulicBoundaryDatabaseFilePathNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var duneLocationsReplacementHandler = mocks.Stub<IDuneLocationsReplacementHandler>();
            mocks.ReplayAll();

            var handler = new HydraulicBoundaryDatabaseUpdateHandler(CreateAssessmentSection(), duneLocationsReplacementHandler);

            ReadHydraulicBoundaryDatabase readHydraulicBoundaryDatabase = ReadHydraulicBoundaryDatabaseTestFactory.Create();

            // Call
            void Call()
            {
                handler.Update(new HydraulicBoundaryDatabase(), readHydraulicBoundaryDatabase,
                               ReadHydraulicLocationConfigurationDatabaseTestFactory.Create(readHydraulicBoundaryDatabase.TrackId),
                               Enumerable.Empty<long>(), null, "");
            }

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("hydraulicBoundaryDatabaseFilePath", exception.ParamName);
        }

        [Test]
        public void Update_HlcdFilePathNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var duneLocationsReplacementHandler = mocks.Stub<IDuneLocationsReplacementHandler>();
            mocks.ReplayAll();

            var handler = new HydraulicBoundaryDatabaseUpdateHandler(CreateAssessmentSection(), duneLocationsReplacementHandler);

            ReadHydraulicBoundaryDatabase readHydraulicBoundaryDatabase = ReadHydraulicBoundaryDatabaseTestFactory.Create();

            // Call
            void Call()
            {
                handler.Update(new HydraulicBoundaryDatabase(), readHydraulicBoundaryDatabase,
                               ReadHydraulicLocationConfigurationDatabaseTestFactory.Create(readHydraulicBoundaryDatabase.TrackId),
                               Enumerable.Empty<long>(), "", null);
            }

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("hlcdFilePath", exception.ParamName);
        }

        [Test]
        [TestCaseSource(nameof(GetInvalidReadHydraulicBoundaryDatabaseConfigurations))]
        public void Update_ReadHydraulicLocationConfigurationDatabaseHasUnequalToOneSettings_ThrowsArgumentException(
            Func<long, ReadHydraulicLocationConfigurationDatabase> getConfigurationDatabase)
        {
            // Setup
            var mocks = new MockRepository();
            var duneLocationsReplacementHandler = mocks.Stub<IDuneLocationsReplacementHandler>();
            mocks.ReplayAll();

            var handler = new HydraulicBoundaryDatabaseUpdateHandler(CreateAssessmentSection(), duneLocationsReplacementHandler);

            ReadHydraulicBoundaryDatabase readHydraulicBoundaryDatabase = ReadHydraulicBoundaryDatabaseTestFactory.Create();

            // Call
            void Call()
            {
                handler.Update(new HydraulicBoundaryDatabase(), readHydraulicBoundaryDatabase,
                               getConfigurationDatabase(readHydraulicBoundaryDatabase.TrackId), Enumerable.Empty<long>(), "", "");
            }

            // Assert
            const string expectedMessage = "readHydraulicLocationConfigurationDatabase must be null or contain " +
                                           "exactly one item for the collection of hydraulic location configuration database settings.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(Call, expectedMessage);
        }

        [Test]
        public void Update_FilePathAndVersionSame_NothingUpdatesAndReturnsEmptyCollection()
        {
            // Setup
            var mocks = new MockRepository();
            var duneLocationsReplacementHandler = mocks.StrictMock<IDuneLocationsReplacementHandler>();
            mocks.ReplayAll();

            const string hydraulicBoundaryDatabaseFilePath = "some/file/path";
            const string hlcdFilePath = "some/hlcd/FilePath";
            AssessmentSection assessmentSection = CreateAssessmentSection();
            var handler = new HydraulicBoundaryDatabaseUpdateHandler(assessmentSection, duneLocationsReplacementHandler);
            ReadHydraulicBoundaryDatabase readHydraulicBoundaryDatabase = ReadHydraulicBoundaryDatabaseTestFactory.Create();
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                FilePath = hydraulicBoundaryDatabaseFilePath,
                Version = readHydraulicBoundaryDatabase.Version,
                Locations =
                {
                    new TestHydraulicBoundaryLocation("old location 1"),
                    new TestHydraulicBoundaryLocation("old location 2"),
                    new TestHydraulicBoundaryLocation("old location 3")
                }
            };
            assessmentSection.SetHydraulicBoundaryLocationCalculations(hydraulicBoundaryDatabase.Locations);

            HydraulicBoundaryLocation[] locations = hydraulicBoundaryDatabase.Locations.ToArray();

            // Call
            IEnumerable<IObservable> changedObjects = handler.Update(hydraulicBoundaryDatabase, readHydraulicBoundaryDatabase,
                                                                     ReadHydraulicLocationConfigurationDatabaseTestFactory.Create(readHydraulicBoundaryDatabase.TrackId),
                                                                     Enumerable.Empty<long>(), hydraulicBoundaryDatabaseFilePath, hlcdFilePath);

            // Assert
            CollectionAssert.IsEmpty(changedObjects);
            Assert.AreEqual(hydraulicBoundaryDatabaseFilePath, hydraulicBoundaryDatabase.FilePath);
            Assert.AreEqual("version", hydraulicBoundaryDatabase.Version);
            AssertHydraulicBoundaryLocationsAndCalculations(locations, assessmentSection);
            mocks.VerifyAll();
        }

        [Test]
        public void Update_VersionSameAndFilePathNotSame_UpdatesFilePathAndReturnsEmptyCollection()
        {
            // Setup
            var mocks = new MockRepository();
            var duneLocationsReplacementHandler = mocks.StrictMock<IDuneLocationsReplacementHandler>();
            mocks.ReplayAll();

            const string newHydraulicBoundaryDatabaseFilePath = "some/file/path";
            const string newHlcdFilePath = "some/hlcd/FilePath";
            AssessmentSection assessmentSection = CreateAssessmentSection();
            var handler = new HydraulicBoundaryDatabaseUpdateHandler(assessmentSection, duneLocationsReplacementHandler);
            ReadHydraulicBoundaryDatabase readHydraulicBoundaryDatabase = ReadHydraulicBoundaryDatabaseTestFactory.Create();
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                FilePath = "old/file/path",
                Version = readHydraulicBoundaryDatabase.Version,
                Locations =
                {
                    new TestHydraulicBoundaryLocation("old location 1"),
                    new TestHydraulicBoundaryLocation("old location 2"),
                    new TestHydraulicBoundaryLocation("old location 3")
                }
            };
            HydraulicBoundaryDatabaseTestHelper.SetHydraulicBoundaryLocationConfigurationSettings(hydraulicBoundaryDatabase);
            assessmentSection.SetHydraulicBoundaryLocationCalculations(hydraulicBoundaryDatabase.Locations);

            HydraulicBoundaryLocation[] locations = hydraulicBoundaryDatabase.Locations.ToArray();

            // Call
            IEnumerable<IObservable> changedObjects = handler.Update(hydraulicBoundaryDatabase, readHydraulicBoundaryDatabase,
                                                                     ReadHydraulicLocationConfigurationDatabaseTestFactory.Create(readHydraulicBoundaryDatabase.TrackId),
                                                                     Enumerable.Empty<long>(), newHydraulicBoundaryDatabaseFilePath, newHlcdFilePath);

            // Assert
            CollectionAssert.IsEmpty(changedObjects);
            Assert.AreEqual(newHydraulicBoundaryDatabaseFilePath, hydraulicBoundaryDatabase.FilePath);
            Assert.AreEqual(newHlcdFilePath, hydraulicBoundaryDatabase.HydraulicLocationConfigurationSettings.FilePath);
            Assert.AreEqual("version", hydraulicBoundaryDatabase.Version);
            AssertHydraulicBoundaryLocationsAndCalculations(locations, assessmentSection);
            mocks.VerifyAll();
        }

        [Test]
        public void Update_DatabaseLinkedAndVersionNotSame_RemovesOldLocationsAndCalculations()
        {
            // Setup
            const string hydraulicBoundaryDatabaseFilePath = "some/file/path";
            const string hlcdFilePath = "some/hlcd/FilePath";
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                FilePath = hydraulicBoundaryDatabaseFilePath,
                Version = "1",
                Locations =
                {
                    new TestHydraulicBoundaryLocation("old location 1"),
                    new TestHydraulicBoundaryLocation("old location 2"),
                    new TestHydraulicBoundaryLocation("old location 3")
                }
            };

            var mocks = new MockRepository();
            var duneLocationsReplacementHandler = mocks.StrictMock<IDuneLocationsReplacementHandler>();
            duneLocationsReplacementHandler.Expect(h => h.Replace(Arg<IEnumerable<HydraulicBoundaryLocation>>.Is.NotNull))
                                           .WhenCalled(invocation =>
                                           {
                                               Assert.AreSame(hydraulicBoundaryDatabase.Locations, invocation.Arguments[0]);
                                           });
            mocks.ReplayAll();

            AssessmentSection assessmentSection = CreateAssessmentSection();
            assessmentSection.SetHydraulicBoundaryLocationCalculations(hydraulicBoundaryDatabase.Locations);

            var handler = new HydraulicBoundaryDatabaseUpdateHandler(assessmentSection, duneLocationsReplacementHandler);

            HydraulicBoundaryLocation[] oldLocations = hydraulicBoundaryDatabase.Locations.ToArray();

            ReadHydraulicBoundaryDatabase readHydraulicBoundaryDatabase = ReadHydraulicBoundaryDatabaseTestFactory.Create();

            // Precondition
            Assert.IsTrue(hydraulicBoundaryDatabase.IsLinked());

            // Call
            handler.Update(hydraulicBoundaryDatabase, readHydraulicBoundaryDatabase,
                           ReadHydraulicLocationConfigurationDatabaseTestFactory.Create(readHydraulicBoundaryDatabase.TrackId),
                           Enumerable.Empty<long>(), hydraulicBoundaryDatabaseFilePath, hlcdFilePath);

            // Assert
            CollectionAssert.IsNotSubsetOf(oldLocations, hydraulicBoundaryDatabase.Locations);
            CollectionAssert.IsNotSubsetOf(oldLocations, assessmentSection.WaterLevelCalculationsForSignalFloodingProbability.Select(hblc => hblc.HydraulicBoundaryLocation));
            CollectionAssert.IsNotSubsetOf(oldLocations, assessmentSection.WaterLevelCalculationsForMaximumAllowableFloodingProbability.Select(hblc => hblc.HydraulicBoundaryLocation));

            foreach (HydraulicBoundaryLocationCalculationsForTargetProbability targetProbability in assessmentSection.WaterLevelCalculationsForUserDefinedTargetProbabilities)
            {
                CollectionAssert.IsNotSubsetOf(oldLocations, targetProbability.HydraulicBoundaryLocationCalculations.Select(hblc => hblc.HydraulicBoundaryLocation));
            }

            foreach (HydraulicBoundaryLocationCalculationsForTargetProbability targetProbability in assessmentSection.WaveHeightCalculationsForUserDefinedTargetProbabilities)
            {
                CollectionAssert.IsNotSubsetOf(oldLocations, targetProbability.HydraulicBoundaryLocationCalculations.Select(hblc => hblc.HydraulicBoundaryLocation));
            }

            mocks.VerifyAll();
        }

        [Test]
        public void Update_DatabaseNotLinked_SetsAllData()
        {
            // Setup
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();

            var mocks = new MockRepository();
            var duneLocationsReplacementHandler = mocks.StrictMock<IDuneLocationsReplacementHandler>();
            duneLocationsReplacementHandler.Expect(h => h.Replace(Arg<IEnumerable<HydraulicBoundaryLocation>>.Is.NotNull))
                                           .WhenCalled(invocation =>
                                           {
                                               Assert.AreSame(hydraulicBoundaryDatabase.Locations, invocation.Arguments[0]);
                                           });
            mocks.ReplayAll();

            const string hydraulicBoundaryDatabaseFilePath = "some/file/path";
            const string hlcdFilePath = "some/hlcd/FilePath";
            AssessmentSection assessmentSection = CreateAssessmentSection();
            var handler = new HydraulicBoundaryDatabaseUpdateHandler(assessmentSection, duneLocationsReplacementHandler);

            ReadHydraulicBoundaryDatabase readHydraulicBoundaryDatabase = ReadHydraulicBoundaryDatabaseTestFactory.Create();
            ReadHydraulicLocationConfigurationDatabase readHydraulicLocationConfigurationDatabase = ReadHydraulicLocationConfigurationDatabaseTestFactory.Create(readHydraulicBoundaryDatabase.TrackId);

            // Precondition
            Assert.IsFalse(hydraulicBoundaryDatabase.IsLinked());

            // Call
            handler.Update(hydraulicBoundaryDatabase, readHydraulicBoundaryDatabase,
                           readHydraulicLocationConfigurationDatabase,
                           Enumerable.Empty<long>(), hydraulicBoundaryDatabaseFilePath, hlcdFilePath);

            // Assert
            Assert.IsTrue(hydraulicBoundaryDatabase.IsLinked());
            Assert.AreEqual(hydraulicBoundaryDatabaseFilePath, hydraulicBoundaryDatabase.FilePath);
            Assert.AreEqual(readHydraulicBoundaryDatabase.Version, hydraulicBoundaryDatabase.Version);
            Assert.AreEqual(hydraulicBoundaryDatabase.HydraulicLocationConfigurationSettings.UsePreprocessorClosure,
                            readHydraulicLocationConfigurationDatabase.ReadTracks.First(rt => rt.TrackId == readHydraulicBoundaryDatabase.TrackId).UsePreprocessorClosure);

            AssertHydraulicBoundaryLocations(readHydraulicBoundaryDatabase.Locations, readHydraulicLocationConfigurationDatabase,
                                             hydraulicBoundaryDatabase.Locations, readHydraulicBoundaryDatabase.TrackId);
            AssertHydraulicBoundaryLocationsAndCalculations(hydraulicBoundaryDatabase.Locations, assessmentSection);
            mocks.VerifyAll();
        }

        [Test]
        public void Update_HrdLocationIdsNotInHlcdLocationIds_ThenLocationsNotAdded()
        {
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();

            var mocks = new MockRepository();
            var duneLocationsReplacementHandler = mocks.Stub<IDuneLocationsReplacementHandler>();
            mocks.ReplayAll();

            const string hydraulicBoundaryDatabaseFilePath = "some/file/path";
            const string hlcdFilePath = "some/hlcd/FilePath";
            AssessmentSection assessmentSection = CreateAssessmentSection();
            var handler = new HydraulicBoundaryDatabaseUpdateHandler(assessmentSection, duneLocationsReplacementHandler);

            var readHydraulicBoundaryLocationsToInclude = new[]
            {
                new ReadHydraulicBoundaryLocation(1, "location 1", 1, 1),
                new ReadHydraulicBoundaryLocation(2, "location 2", 2, 2)
            };
            var readHydraulicBoundaryLocationsToExclude = new[]
            {
                new ReadHydraulicBoundaryLocation(3, "location 3", 3, 3),
                new ReadHydraulicBoundaryLocation(4, "location 4", 4, 4)
            };
            ReadHydraulicBoundaryDatabase readHydraulicBoundaryDatabase = ReadHydraulicBoundaryDatabaseTestFactory.Create(
                readHydraulicBoundaryLocationsToInclude.Concat(readHydraulicBoundaryLocationsToExclude));
            ReadHydraulicLocationConfigurationDatabase readHydraulicLocationConfigurationDatabase = ReadHydraulicLocationConfigurationDatabaseTestFactory.Create(readHydraulicBoundaryDatabase.TrackId);

            // Precondition
            Assert.IsFalse(hydraulicBoundaryDatabase.IsLinked());

            // Call
            handler.Update(hydraulicBoundaryDatabase, readHydraulicBoundaryDatabase,
                           readHydraulicLocationConfigurationDatabase,
                           Enumerable.Empty<long>(), hydraulicBoundaryDatabaseFilePath, hlcdFilePath);

            // Assert
            AssertHydraulicBoundaryLocations(readHydraulicBoundaryLocationsToInclude, readHydraulicLocationConfigurationDatabase,
                                             hydraulicBoundaryDatabase.Locations, readHydraulicBoundaryDatabase.TrackId);
            mocks.VerifyAll();
        }

        [Test]
        public void Update_HrdLocationIdsInExcludedLocationIds_LocationsNotAdded()
        {
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();

            var mocks = new MockRepository();
            var duneLocationsReplacementHandler = mocks.Stub<IDuneLocationsReplacementHandler>();
            mocks.ReplayAll();

            const string hydraulicBoundaryDatabaseFilePath = "some/file/path";
            const string hlcdFilePath = "some/hlcd/FilePath";
            AssessmentSection assessmentSection = CreateAssessmentSection();
            var handler = new HydraulicBoundaryDatabaseUpdateHandler(assessmentSection, duneLocationsReplacementHandler);

            var readHydraulicBoundaryLocationsToExclude = new[]
            {
                new ReadHydraulicBoundaryLocation(1, "location 1", 1, 1),
                new ReadHydraulicBoundaryLocation(2, "location 2", 2, 2)
            };
            var readHydraulicBoundaryLocationsToInclude = new[]
            {
                new ReadHydraulicBoundaryLocation(3, "location 3", 3, 3),
                new ReadHydraulicBoundaryLocation(4, "location 4", 4, 4)
            };

            ReadHydraulicBoundaryDatabase readHydraulicBoundaryDatabase = ReadHydraulicBoundaryDatabaseTestFactory.Create(
                readHydraulicBoundaryLocationsToExclude.Concat(readHydraulicBoundaryLocationsToInclude));

            ReadHydraulicLocationConfigurationDatabase readHydraulicLocationConfigurationDatabase = ReadHydraulicLocationConfigurationDatabaseTestFactory.Create(
                readHydraulicBoundaryLocationsToInclude.Select(l => l.Id), readHydraulicBoundaryDatabase.TrackId);

            // Precondition
            Assert.IsFalse(hydraulicBoundaryDatabase.IsLinked());

            // Call
            handler.Update(hydraulicBoundaryDatabase, readHydraulicBoundaryDatabase,
                           readHydraulicLocationConfigurationDatabase,
                           readHydraulicBoundaryLocationsToExclude.Select(l => l.Id), hydraulicBoundaryDatabaseFilePath, hlcdFilePath);

            // Assert
            AssertHydraulicBoundaryLocations(readHydraulicBoundaryLocationsToInclude, readHydraulicLocationConfigurationDatabase,
                                             hydraulicBoundaryDatabase.Locations, readHydraulicBoundaryDatabase.TrackId);
            mocks.VerifyAll();
        }

        [Test]
        public void Update_ReadHydraulicLocationConfigurationDatabaseWithScenarioInformation_SetsHydraulicLocationConfigurationSettingsAndDoesNotLog()
        {
            // Setup
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();

            var mocks = new MockRepository();
            var duneLocationsReplacementHandler = mocks.Stub<IDuneLocationsReplacementHandler>();
            mocks.ReplayAll();

            const string hydraulicBoundaryDatabaseFilePath = "some/file/path";
            const string hlcdFilePath = "some/hlcd/FilePath";
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            var handler = new HydraulicBoundaryDatabaseUpdateHandler(assessmentSection, duneLocationsReplacementHandler);

            ReadHydraulicBoundaryDatabase readHydraulicBoundaryDatabase = ReadHydraulicBoundaryDatabaseTestFactory.Create();

            ReadHydraulicLocationConfigurationDatabase readHydraulicLocationConfigurationDatabase =
                ReadHydraulicLocationConfigurationDatabaseTestFactory.CreateWithConfigurationSettings(readHydraulicBoundaryDatabase.TrackId);

            // Precondition
            Assert.IsNotNull(readHydraulicLocationConfigurationDatabase.ReadHydraulicLocationConfigurationDatabaseSettings);
            Assert.AreEqual(1, readHydraulicLocationConfigurationDatabase.ReadHydraulicLocationConfigurationDatabaseSettings.Count());

            // Call
            void Call()
            {
                handler.Update(hydraulicBoundaryDatabase,
                               readHydraulicBoundaryDatabase,
                               readHydraulicLocationConfigurationDatabase,
                               Enumerable.Empty<long>(),
                               hydraulicBoundaryDatabaseFilePath,
                               hlcdFilePath);
            }

            // Assert
            TestHelper.AssertLogMessagesCount(Call, 0);

            ReadHydraulicLocationConfigurationDatabaseSettings expectedSettings = readHydraulicLocationConfigurationDatabase.ReadHydraulicLocationConfigurationDatabaseSettings
                                                                                                                            .Single();
            HydraulicLocationConfigurationSettings actualSettings = hydraulicBoundaryDatabase.HydraulicLocationConfigurationSettings;

            Assert.AreEqual(hlcdFilePath, actualSettings.FilePath);
            Assert.AreEqual(expectedSettings.ScenarioName, actualSettings.ScenarioName);
            Assert.AreEqual(expectedSettings.Year, actualSettings.Year);
            Assert.AreEqual(expectedSettings.Scope, actualSettings.Scope);
            Assert.AreEqual(readHydraulicLocationConfigurationDatabase.ReadTracks.First(rt => rt.TrackId == readHydraulicBoundaryDatabase.TrackId).UsePreprocessorClosure, actualSettings.UsePreprocessorClosure);
            Assert.AreEqual(expectedSettings.SeaLevel, actualSettings.SeaLevel);
            Assert.AreEqual(expectedSettings.RiverDischarge, actualSettings.RiverDischarge);
            Assert.AreEqual(expectedSettings.LakeLevel, actualSettings.LakeLevel);
            Assert.AreEqual(expectedSettings.WindDirection, actualSettings.WindDirection);
            Assert.AreEqual(expectedSettings.WindSpeed, actualSettings.WindSpeed);
            Assert.AreEqual(expectedSettings.Comment, actualSettings.Comment);
        }

        [Test]
        public void Update_ReadHydraulicLocationConfigurationDatabaseWithoutScenarioInformation_SetsDefaultHydraulicLocationConfigurationSettingsAndLogsWarning()
        {
            // Setup
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();

            var mocks = new MockRepository();
            var duneLocationsReplacementHandler = mocks.Stub<IDuneLocationsReplacementHandler>();
            mocks.ReplayAll();

            const string hydraulicBoundaryDatabaseFilePath = "some/file/path";
            const string hlcdFilePath = "some/hlcd/FilePath";
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            var handler = new HydraulicBoundaryDatabaseUpdateHandler(assessmentSection, duneLocationsReplacementHandler);

            ReadHydraulicBoundaryDatabase readHydraulicBoundaryDatabase = ReadHydraulicBoundaryDatabaseTestFactory.Create();

            ReadHydraulicLocationConfigurationDatabase readHydraulicLocationConfigurationDatabase =
                ReadHydraulicLocationConfigurationDatabaseTestFactory.Create(readHydraulicBoundaryDatabase.TrackId);

            // Precondition
            Assert.IsNull(readHydraulicLocationConfigurationDatabase.ReadHydraulicLocationConfigurationDatabaseSettings);

            // Call
            void Call()
            {
                handler.Update(hydraulicBoundaryDatabase,
                               readHydraulicBoundaryDatabase,
                               readHydraulicLocationConfigurationDatabase,
                               Enumerable.Empty<long>(),
                               hydraulicBoundaryDatabaseFilePath,
                               hlcdFilePath);
            }

            // Assert
            const string expectedMessage = "De tabel 'ScenarioInformation' in het HLCD bestand is niet aanwezig. Er worden standaardwaarden " +
                                           "conform WBI2017 gebruikt voor de HLCD bestandsinformatie.";
            TestHelper.AssertLogMessageWithLevelIsGenerated(Call, Tuple.Create(expectedMessage, LogLevelConstant.Warn), 1);

            HydraulicLocationConfigurationSettings actualSettings = hydraulicBoundaryDatabase.HydraulicLocationConfigurationSettings;
            Assert.AreEqual(hlcdFilePath, actualSettings.FilePath);
            Assert.AreEqual("WBI2017", actualSettings.ScenarioName);
            Assert.AreEqual(2023, actualSettings.Year);
            Assert.AreEqual("WBI2017", actualSettings.Scope);
            Assert.AreEqual(readHydraulicLocationConfigurationDatabase.ReadTracks.First(rt => rt.TrackId == readHydraulicBoundaryDatabase.TrackId).UsePreprocessorClosure, actualSettings.UsePreprocessorClosure);
            Assert.AreEqual("Conform WBI2017", actualSettings.SeaLevel);
            Assert.AreEqual("Conform WBI2017", actualSettings.RiverDischarge);
            Assert.AreEqual("Conform WBI2017", actualSettings.LakeLevel);
            Assert.AreEqual("Conform WBI2017", actualSettings.WindDirection);
            Assert.AreEqual("Conform WBI2017", actualSettings.WindSpeed);
            Assert.AreEqual("Gegenereerd door Riskeer (conform WBI2017)", actualSettings.Comment);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void GivenDatabase_WhenUpdatingDataWithNewLocations_ThenChangedObjectsReturned(bool isLinked)
        {
            // Given
            var mocks = new MockRepository();
            var duneLocationsReplacementHandler = mocks.Stub<IDuneLocationsReplacementHandler>();
            mocks.ReplayAll();

            const string hydraulicBoundaryDatabaseFilePath = "some/file/path";
            const string hlcdFilePath = "some/hlcd/FilePath";
            AssessmentSection assessmentSection = CreateAssessmentSection();
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();
            if (isLinked)
            {
                hydraulicBoundaryDatabase.FilePath = hydraulicBoundaryDatabaseFilePath;
                hydraulicBoundaryDatabase.Version = "1";
                hydraulicBoundaryDatabase.Locations.AddRange(new[]
                {
                    new TestHydraulicBoundaryLocation("old location 1"),
                    new TestHydraulicBoundaryLocation("old location 2"),
                    new TestHydraulicBoundaryLocation("old location 3")
                });
            }

            var handler = new HydraulicBoundaryDatabaseUpdateHandler(assessmentSection, duneLocationsReplacementHandler);

            ReadHydraulicBoundaryDatabase readHydraulicBoundaryDatabase = ReadHydraulicBoundaryDatabaseTestFactory.Create();

            // Precondition
            Assert.AreEqual(isLinked, hydraulicBoundaryDatabase.IsLinked());

            // When
            IEnumerable<IObservable> changedObjects = handler.Update(hydraulicBoundaryDatabase, readHydraulicBoundaryDatabase,
                                                                     ReadHydraulicLocationConfigurationDatabaseTestFactory.Create(readHydraulicBoundaryDatabase.TrackId),
                                                                     Enumerable.Empty<long>(), hydraulicBoundaryDatabaseFilePath, hlcdFilePath);

            // Then
            var observables = new List<IObservable>
            {
                hydraulicBoundaryDatabase.Locations,
                assessmentSection.WaterLevelCalculationsForSignalFloodingProbability,
                assessmentSection.WaterLevelCalculationsForMaximumAllowableFloodingProbability,
                assessmentSection.DuneErosion.DuneLocations,
                assessmentSection.DuneErosion.DuneLocationCalculationsForUserDefinedTargetProbabilities
            };

            observables.AddRange(assessmentSection.WaterLevelCalculationsForUserDefinedTargetProbabilities
                                                  .Select(element => element.HydraulicBoundaryLocationCalculations));
            observables.AddRange(assessmentSection.WaveHeightCalculationsForUserDefinedTargetProbabilities
                                                  .Select(element => element.HydraulicBoundaryLocationCalculations));
            observables.AddRange(assessmentSection.DuneErosion.DuneLocationCalculationsForUserDefinedTargetProbabilities
                                                  .Select(element => element.DuneLocationCalculations));

            CollectionAssert.AreEqual(observables, changedObjects);
            mocks.VerifyAll();
        }

        [Test]
        public void GivenCalculationsWithLocation_WhenUpdatingDatabaseWithNewLocations_ThenCalculationOutputClearedAndChangedObjectsReturned()
        {
            // Given
            var mocks = new MockRepository();
            var duneLocationsReplacementHandler = mocks.Stub<IDuneLocationsReplacementHandler>();
            mocks.ReplayAll();

            const string hydraulicBoundaryDatabaseFilePath = "some/file/path";
            const string hlcdFilePath = "some/hlcd/FilePath";
            AssessmentSection assessmentSection = TestDataGenerator.GetAssessmentSectionWithAllCalculationConfigurations();

            ICalculation[] calculationsWithOutput = assessmentSection.GetFailureMechanisms()
                                                                     .OfType<ICalculatableFailureMechanism>()
                                                                     .SelectMany(fm => fm.Calculations)
                                                                     .Where(c => c.HasOutput)
                                                                     .ToArray();

            calculationsWithOutput = calculationsWithOutput.Except(calculationsWithOutput.OfType<SemiProbabilisticPipingCalculationScenario>()
                                                                                         .Where(c => c.InputParameters.UseAssessmentLevelManualInput))
                                                           .Except(calculationsWithOutput.OfType<MacroStabilityInwardsCalculationScenario>()
                                                                                         .Where(c => c.InputParameters.UseAssessmentLevelManualInput))
                                                           .Except(calculationsWithOutput.OfType<TestPipingCalculationScenario>())
                                                           .ToArray();

            var handler = new HydraulicBoundaryDatabaseUpdateHandler(assessmentSection, duneLocationsReplacementHandler);

            ReadHydraulicBoundaryDatabase readHydraulicBoundaryDatabase = ReadHydraulicBoundaryDatabaseTestFactory.Create();

            // When
            IEnumerable<IObservable> changedObjects = handler.Update(assessmentSection.HydraulicBoundaryDatabase, readHydraulicBoundaryDatabase,
                                                                     ReadHydraulicLocationConfigurationDatabaseTestFactory.Create(readHydraulicBoundaryDatabase.TrackId),
                                                                     Enumerable.Empty<long>(), hydraulicBoundaryDatabaseFilePath, hlcdFilePath);

            // Then
            Assert.IsTrue(calculationsWithOutput.All(c => !c.HasOutput));
            CollectionAssert.IsSubsetOf(calculationsWithOutput, changedObjects);
            mocks.VerifyAll();
        }

        [Test]
        public void DoPostUpdateActions_NoUpdateCalled_DoNothing()
        {
            // Setup
            var mocks = new MockRepository();
            var duneLocationsReplacementHandler = mocks.StrictMock<IDuneLocationsReplacementHandler>();
            mocks.ReplayAll();

            AssessmentSection assessmentSection = CreateAssessmentSection();

            var handler = new HydraulicBoundaryDatabaseUpdateHandler(assessmentSection, duneLocationsReplacementHandler);

            // Call
            handler.DoPostUpdateActions();

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void DoPostUpdateActions_AfterUpdateCalledAndNoLocationsUpdated_DoNothing()
        {
            // Setup
            var mocks = new MockRepository();
            var duneLocationsReplacementHandler = mocks.StrictMock<IDuneLocationsReplacementHandler>();
            mocks.ReplayAll();

            const string hydraulicBoundaryDatabaseFilePath = "some/file/path";
            const string hlcdFilePath = "some/hlcd/FilePath";
            AssessmentSection assessmentSection = CreateAssessmentSection();
            ReadHydraulicBoundaryDatabase readHydraulicBoundaryDatabase = ReadHydraulicBoundaryDatabaseTestFactory.Create();
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                FilePath = hydraulicBoundaryDatabaseFilePath,
                Version = readHydraulicBoundaryDatabase.Version,
                Locations =
                {
                    new TestHydraulicBoundaryLocation("old location 1"),
                    new TestHydraulicBoundaryLocation("old location 2"),
                    new TestHydraulicBoundaryLocation("old location 3")
                }
            };

            var handler = new HydraulicBoundaryDatabaseUpdateHandler(assessmentSection, duneLocationsReplacementHandler);

            IEnumerable<IObservable> changedObjects = handler.Update(hydraulicBoundaryDatabase, readHydraulicBoundaryDatabase,
                                                                     ReadHydraulicLocationConfigurationDatabaseTestFactory.Create(readHydraulicBoundaryDatabase.TrackId),
                                                                     Enumerable.Empty<long>(), hydraulicBoundaryDatabaseFilePath, hlcdFilePath);

            // Precondition
            CollectionAssert.IsEmpty(changedObjects);

            // Call
            handler.DoPostUpdateActions();

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void DoPostUpdateActions_AfterUpdateCalledAndLocationsUpdated_Perform()
        {
            // Setup
            var mocks = new MockRepository();
            var duneLocationsReplacementHandler = mocks.StrictMock<IDuneLocationsReplacementHandler>();
            duneLocationsReplacementHandler.Stub(h => h.Replace(null)).IgnoreArguments();
            duneLocationsReplacementHandler.Expect(h => h.DoPostReplacementUpdates());
            mocks.ReplayAll();

            const string hydraulicBoundaryDatabaseFilePath = "old/file/path";
            const string hlcdFilePath = "some/hlcd/FilePath";
            AssessmentSection assessmentSection = CreateAssessmentSection();
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                FilePath = hydraulicBoundaryDatabaseFilePath,
                Version = "1",
                Locations =
                {
                    new TestHydraulicBoundaryLocation("old location 1"),
                    new TestHydraulicBoundaryLocation("old location 2"),
                    new TestHydraulicBoundaryLocation("old location 3")
                }
            };

            var handler = new HydraulicBoundaryDatabaseUpdateHandler(assessmentSection, duneLocationsReplacementHandler);

            ReadHydraulicBoundaryDatabase readHydraulicBoundaryDatabase = ReadHydraulicBoundaryDatabaseTestFactory.Create();

            IEnumerable<IObservable> changedObjects = handler.Update(hydraulicBoundaryDatabase, readHydraulicBoundaryDatabase,
                                                                     ReadHydraulicLocationConfigurationDatabaseTestFactory.Create(readHydraulicBoundaryDatabase.TrackId),
                                                                     Enumerable.Empty<long>(), hydraulicBoundaryDatabaseFilePath, hlcdFilePath);

            // Precondition
            CollectionAssert.IsNotEmpty(changedObjects);

            // Call
            handler.DoPostUpdateActions();

            // Assert
            mocks.VerifyAll();
        }

        private static AssessmentSection CreateAssessmentSection()
        {
            return new AssessmentSection(AssessmentSectionComposition.Dike);
        }

        private static void AssertHydraulicBoundaryLocationsAndCalculations(IEnumerable<HydraulicBoundaryLocation> locations, AssessmentSection assessmentSection)
        {
            CollectionAssert.AreEqual(locations, assessmentSection.WaterLevelCalculationsForSignalFloodingProbability.Select(hblc => hblc.HydraulicBoundaryLocation));
            CollectionAssert.AreEqual(locations, assessmentSection.WaterLevelCalculationsForMaximumAllowableFloodingProbability.Select(hblc => hblc.HydraulicBoundaryLocation));
            AssertHydraulicBoundaryLocationsOfUserDefinedTargetProbabilities(locations, assessmentSection.WaterLevelCalculationsForUserDefinedTargetProbabilities);
            AssertHydraulicBoundaryLocationsOfUserDefinedTargetProbabilities(locations, assessmentSection.WaveHeightCalculationsForUserDefinedTargetProbabilities);
        }

        private static void AssertHydraulicBoundaryLocationsOfUserDefinedTargetProbabilities(IEnumerable<HydraulicBoundaryLocation> locations,
                                                                                             IEnumerable<HydraulicBoundaryLocationCalculationsForTargetProbability> targetProbabilities)
        {
            foreach (HydraulicBoundaryLocationCalculationsForTargetProbability targetProbability in targetProbabilities)
            {
                CollectionAssert.AreEqual(locations, targetProbability.HydraulicBoundaryLocationCalculations.Select(calc => calc.HydraulicBoundaryLocation));
            }
        }

        private static void AssertHydraulicBoundaryLocations(IEnumerable<ReadHydraulicBoundaryLocation> readLocations,
                                                             ReadHydraulicLocationConfigurationDatabase readHydraulicLocationConfigurationDatabase,
                                                             IEnumerable<HydraulicBoundaryLocation> actualLocations,
                                                             long trackId)
        {
            Assert.AreEqual(readLocations.Count(), actualLocations.Count());

            for (var i = 0; i < actualLocations.Count(); i++)
            {
                ReadHydraulicBoundaryLocation readLocation = readLocations.ElementAt(i);
                HydraulicBoundaryLocation actualLocation = actualLocations.ElementAt(i);

                Assert.AreEqual(readHydraulicLocationConfigurationDatabase.ReadHydraulicLocations
                                                                          .Where(rhl => rhl.TrackId == trackId)
                                                                          .Single(l => l.HrdLocationId == readLocation.Id).HlcdLocationId,
                                actualLocation.Id);
                Assert.AreEqual(readLocation.Name, actualLocation.Name);
                Assert.AreEqual(readLocation.CoordinateX, actualLocation.Location.X);
                Assert.AreEqual(readLocation.CoordinateY, actualLocation.Location.Y);
            }
        }

        private static IEnumerable<TestCaseData> GetInvalidReadHydraulicBoundaryDatabaseConfigurations()
        {
            yield return new TestCaseData(new Func<long, ReadHydraulicLocationConfigurationDatabase>(
                                              trackId => ReadHydraulicLocationConfigurationDatabaseTestFactory.CreateWithConfigurationSettings(
                                                  Enumerable.Empty<ReadHydraulicLocationConfigurationDatabaseSettings>(), trackId)))
                .SetName("ReadHydraulicLocationConfigurationDatabaseSettingsEmpty");
            yield return new TestCaseData(new Func<long, ReadHydraulicLocationConfigurationDatabase>(
                                              trackId => ReadHydraulicLocationConfigurationDatabaseTestFactory.CreateWithConfigurationSettings(
                                                  new[]
                                                  {
                                                      ReadHydraulicLocationConfigurationDatabaseSettingsTestFactory.Create(),
                                                      ReadHydraulicLocationConfigurationDatabaseSettingsTestFactory.Create()
                                                  }, trackId)))
                .SetName("ReadHydraulicLocationConfigurationDatabaseSettingsMultipleItems");
        }
    }
}