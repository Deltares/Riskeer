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
    public class HydraulicBoundaryDataUpdateHandlerTest : NUnitFormTest
    {
        [Test]
        public void Constructor_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var duneLocationsUpdateHandler = mocks.Stub<IDuneLocationsUpdateHandler>();
            mocks.ReplayAll();

            // Call
            void Call() => new HydraulicBoundaryDataUpdateHandler(null, duneLocationsUpdateHandler);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_DuneLocationsUpdateHandlerNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new HydraulicBoundaryDataUpdateHandler(CreateAssessmentSection(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("duneLocationsUpdateHandler", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var duneLocationsUpdateHandler = mocks.Stub<IDuneLocationsUpdateHandler>();
            mocks.ReplayAll();

            // Call
            var handler = new HydraulicBoundaryDataUpdateHandler(CreateAssessmentSection(), duneLocationsUpdateHandler);

            // Assert
            Assert.IsInstanceOf<IHydraulicBoundaryDataUpdateHandler>(handler);
            mocks.VerifyAll();
        }

        [Test]
        public void AddHydraulicBoundaryDatabase_HydraulicBoundaryDataNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var duneLocationsUpdateHandler = mocks.Stub<IDuneLocationsUpdateHandler>();
            mocks.ReplayAll();

            ReadHydraulicBoundaryDatabase readHydraulicBoundaryDatabase = ReadHydraulicBoundaryDatabaseTestFactory.Create();

            var handler = new HydraulicBoundaryDataUpdateHandler(CreateAssessmentSection(), duneLocationsUpdateHandler);

            // Call
            void Call()
            {
                handler.AddHydraulicBoundaryDatabase(null, readHydraulicBoundaryDatabase,
                                                     ReadHydraulicLocationConfigurationDatabaseTestFactory.Create(readHydraulicBoundaryDatabase.TrackId),
                                                     Enumerable.Empty<long>(), "");
            }

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("hydraulicBoundaryData", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void AddHydraulicBoundaryDatabase_ReadHydraulicBoundaryDatabaseNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var duneLocationsUpdateHandler = mocks.Stub<IDuneLocationsUpdateHandler>();
            mocks.ReplayAll();

            var handler = new HydraulicBoundaryDataUpdateHandler(CreateAssessmentSection(), duneLocationsUpdateHandler);

            // Call
            void Call() => handler.AddHydraulicBoundaryDatabase(new HydraulicBoundaryData(), null,
                                                                ReadHydraulicLocationConfigurationDatabaseTestFactory.Create(1),
                                                                Enumerable.Empty<long>(), "");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("readHydraulicBoundaryDatabase", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void AddHydraulicBoundaryDatabase_ReadHydraulicLocationConfigurationDatabaseNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var duneLocationsUpdateHandler = mocks.Stub<IDuneLocationsUpdateHandler>();
            mocks.ReplayAll();

            var handler = new HydraulicBoundaryDataUpdateHandler(CreateAssessmentSection(), duneLocationsUpdateHandler);

            // Call
            void Call() => handler.AddHydraulicBoundaryDatabase(new HydraulicBoundaryData(), ReadHydraulicBoundaryDatabaseTestFactory.Create(),
                                                                null, Enumerable.Empty<long>(), "");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("readHydraulicLocationConfigurationDatabase", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void AddHydraulicBoundaryDatabase_ExcludedLocationIdsNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var duneLocationsUpdateHandler = mocks.Stub<IDuneLocationsUpdateHandler>();
            mocks.ReplayAll();

            ReadHydraulicBoundaryDatabase readHydraulicBoundaryDatabase = ReadHydraulicBoundaryDatabaseTestFactory.Create();

            var handler = new HydraulicBoundaryDataUpdateHandler(CreateAssessmentSection(), duneLocationsUpdateHandler);

            // Call
            void Call()
            {
                handler.AddHydraulicBoundaryDatabase(new HydraulicBoundaryData(), readHydraulicBoundaryDatabase,
                                                     ReadHydraulicLocationConfigurationDatabaseTestFactory.Create(readHydraulicBoundaryDatabase.TrackId),
                                                     null, "");
            }

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("excludedLocationIds", exception.ParamName);
        }

        [Test]
        public void AddHydraulicBoundaryDatabase_HrdFilePathNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var duneLocationsUpdateHandler = mocks.Stub<IDuneLocationsUpdateHandler>();
            mocks.ReplayAll();

            ReadHydraulicBoundaryDatabase readHydraulicBoundaryDatabase = ReadHydraulicBoundaryDatabaseTestFactory.Create();

            var handler = new HydraulicBoundaryDataUpdateHandler(CreateAssessmentSection(), duneLocationsUpdateHandler);

            // Call
            void Call()
            {
                handler.AddHydraulicBoundaryDatabase(new HydraulicBoundaryData(), readHydraulicBoundaryDatabase,
                                                     ReadHydraulicLocationConfigurationDatabaseTestFactory.Create(readHydraulicBoundaryDatabase.TrackId),
                                                     Enumerable.Empty<long>(), null);
            }

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("hrdFilePath", exception.ParamName);
        }

        [Test]
        public void AddHydraulicBoundaryDatabase_ValidData_SetsAllData()
        {
            // Setup
            var hydraulicBoundaryData = new HydraulicBoundaryData();

            var mocks = new MockRepository();
            var duneLocationsUpdateHandler = mocks.StrictMock<IDuneLocationsUpdateHandler>();
            duneLocationsUpdateHandler.Expect(h => h.AddLocations(Arg<IEnumerable<HydraulicBoundaryLocation>>.Is.NotNull))
                                      .WhenCalled(invocation =>
                                      {
                                          Assert.AreSame(hydraulicBoundaryData.HydraulicBoundaryDatabases.First().Locations, invocation.Arguments[0]);
                                      });
            mocks.ReplayAll();

            const string hrdFilePath = "some/file/path";
            AssessmentSection assessmentSection = CreateAssessmentSection();
            var handler = new HydraulicBoundaryDataUpdateHandler(assessmentSection, duneLocationsUpdateHandler);

            ReadHydraulicBoundaryDatabase readHydraulicBoundaryDatabase = ReadHydraulicBoundaryDatabaseTestFactory.Create();
            ReadHydraulicLocationConfigurationDatabase readHydraulicLocationConfigurationDatabase =
                ReadHydraulicLocationConfigurationDatabaseTestFactory.Create(readHydraulicBoundaryDatabase.TrackId);

            // Precondition
            Assert.IsFalse(hydraulicBoundaryData.IsLinked());

            // Call
            handler.AddHydraulicBoundaryDatabase(hydraulicBoundaryData, readHydraulicBoundaryDatabase, readHydraulicLocationConfigurationDatabase,
                                                 Enumerable.Empty<long>(), hrdFilePath);

            // Assert
            Assert.AreEqual(1, hydraulicBoundaryData.HydraulicBoundaryDatabases.Count);

            HydraulicBoundaryDatabase hydraulicBoundaryDatabase = hydraulicBoundaryData.HydraulicBoundaryDatabases.First();

            Assert.AreEqual(hrdFilePath, hydraulicBoundaryDatabase.FilePath);
            Assert.AreEqual(readHydraulicBoundaryDatabase.Version, hydraulicBoundaryDatabase.Version);
            Assert.IsFalse(hydraulicBoundaryDatabase.UsePreprocessorClosure);

            AssertHydraulicBoundaryLocations(readHydraulicBoundaryDatabase.Locations, readHydraulicLocationConfigurationDatabase,
                                             hydraulicBoundaryDatabase.Locations, readHydraulicBoundaryDatabase.TrackId);
            AssertHydraulicBoundaryLocationsAndCalculations(hydraulicBoundaryDatabase.Locations, assessmentSection);
            mocks.VerifyAll();
        }

        [Test]
        public void AddHydraulicBoundaryDatabase_HrdLocationIdsNotInHlcdLocationIds_ThenLocationsNotAdded()
        {
            var hydraulicBoundaryData = new HydraulicBoundaryData();

            var mocks = new MockRepository();
            var duneLocationsUpdateHandler = mocks.Stub<IDuneLocationsUpdateHandler>();
            mocks.ReplayAll();

            const string hrdFilePath = "some/file/path";
            AssessmentSection assessmentSection = CreateAssessmentSection();
            var handler = new HydraulicBoundaryDataUpdateHandler(assessmentSection, duneLocationsUpdateHandler);

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
            ReadHydraulicLocationConfigurationDatabase readHydraulicLocationConfigurationDatabase =
                ReadHydraulicLocationConfigurationDatabaseTestFactory.Create(readHydraulicBoundaryDatabase.TrackId);

            // Precondition
            Assert.IsFalse(hydraulicBoundaryData.IsLinked());

            // Call
            handler.AddHydraulicBoundaryDatabase(hydraulicBoundaryData, readHydraulicBoundaryDatabase, readHydraulicLocationConfigurationDatabase,
                                                 Enumerable.Empty<long>(), hrdFilePath);

            // Assert
            AssertHydraulicBoundaryLocations(readHydraulicBoundaryLocationsToInclude, readHydraulicLocationConfigurationDatabase,
                                             hydraulicBoundaryData.Locations, readHydraulicBoundaryDatabase.TrackId);
            mocks.VerifyAll();
        }

        [Test]
        public void AddHydraulicBoundaryDatabase_HrdLocationIdsInExcludedLocationIds_LocationsNotAdded()
        {
            // Setup
            var mocks = new MockRepository();
            var duneLocationsUpdateHandler = mocks.Stub<IDuneLocationsUpdateHandler>();
            mocks.ReplayAll();

            const string hrdFilePath = "some/file/path";
            AssessmentSection assessmentSection = CreateAssessmentSection();
            var handler = new HydraulicBoundaryDataUpdateHandler(assessmentSection, duneLocationsUpdateHandler);

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
                readHydraulicBoundaryLocationsToInclude.Select(l => l.Id),
                readHydraulicBoundaryDatabase.TrackId);

            var hydraulicBoundaryData = new HydraulicBoundaryData();

            // Precondition
            Assert.IsFalse(hydraulicBoundaryData.IsLinked());

            // Call
            handler.AddHydraulicBoundaryDatabase(hydraulicBoundaryData, readHydraulicBoundaryDatabase, readHydraulicLocationConfigurationDatabase,
                                                 readHydraulicBoundaryLocationsToExclude.Select(l => l.Id), hrdFilePath);

            // Assert
            AssertHydraulicBoundaryLocations(readHydraulicBoundaryLocationsToInclude, readHydraulicLocationConfigurationDatabase,
                                             hydraulicBoundaryData.Locations, readHydraulicBoundaryDatabase.TrackId);
            mocks.VerifyAll();
        }

        [Test]
        public void GivenDatabase_WhenAddingNewHydraulicBoundaryDatabase_ThenChangedObjectsReturned()
        {
            // Given
            var mocks = new MockRepository();
            var duneLocationsUpdateHandler = mocks.Stub<IDuneLocationsUpdateHandler>();
            mocks.ReplayAll();

            const string hrdFilePath = "some/file/path";
            AssessmentSection assessmentSection = CreateAssessmentSection();
            var hydraulicBoundaryData = new HydraulicBoundaryData();

            ReadHydraulicBoundaryDatabase readHydraulicBoundaryDatabase = ReadHydraulicBoundaryDatabaseTestFactory.Create();

            var handler = new HydraulicBoundaryDataUpdateHandler(assessmentSection, duneLocationsUpdateHandler);

            // When
            IEnumerable<IObservable> changedObjects = handler.AddHydraulicBoundaryDatabase(
                hydraulicBoundaryData, readHydraulicBoundaryDatabase,
                ReadHydraulicLocationConfigurationDatabaseTestFactory.Create(readHydraulicBoundaryDatabase.TrackId),
                Enumerable.Empty<long>(), hrdFilePath);

            // Then
            var observables = new List<IObservable>
            {
                hydraulicBoundaryData.HydraulicBoundaryDatabases,
                hydraulicBoundaryData.Locations,
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
        public void RemoveHydraulicBoundaryDatabase_HydraulicBoundaryDatabaseNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var duneLocationsUpdateHandler = mocks.Stub<IDuneLocationsUpdateHandler>();
            mocks.ReplayAll();

            var random = new Random(21);
            var assessmentSection = new AssessmentSection(random.NextEnumValue<AssessmentSectionComposition>());

            var handler = new HydraulicBoundaryDataUpdateHandler(assessmentSection, duneLocationsUpdateHandler);

            // Call
            void Call() => handler.RemoveHydraulicBoundaryDatabase(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("hydraulicBoundaryDatabase", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void GivenAssessmentSectionWithDatabase_WhenRemoving_ThenDatabaseRemoved()
        {
            // Given
            var location1 = new TestHydraulicBoundaryLocation();
            var location2 = new TestHydraulicBoundaryLocation();

            var mocks = new MockRepository();
            var duneLocationsUpdateHandler = mocks.Stub<IDuneLocationsUpdateHandler>();
            duneLocationsUpdateHandler.Expect(dlrh => dlrh.RemoveLocations(new[]
            {
                location1,
                location2
            }));
            mocks.ReplayAll();

            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                Locations =
                {
                    location1,
                    location2
                }
            };
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike)
            {
                HydraulicBoundaryData =
                {
                    HydraulicBoundaryDatabases =
                    {
                        hydraulicBoundaryDatabase
                    }
                }
            };

            assessmentSection.SetHydraulicBoundaryLocationCalculations(new[]
            {
                location1,
                location2
            });

            var handler = new HydraulicBoundaryDataUpdateHandler(assessmentSection, duneLocationsUpdateHandler);

            // Precondition
            Assert.AreEqual(1, assessmentSection.HydraulicBoundaryData.HydraulicBoundaryDatabases.Count);
            Assert.AreEqual(2, assessmentSection.WaterLevelCalculationsForSignalFloodingProbability.Count());
            Assert.AreEqual(2, assessmentSection.WaterLevelCalculationsForMaximumAllowableFloodingProbability.Count());

            foreach (HydraulicBoundaryLocationCalculationsForTargetProbability element in assessmentSection.WaterLevelCalculationsForUserDefinedTargetProbabilities)
            {
                Assert.AreEqual(2, element.HydraulicBoundaryLocationCalculations);
            }

            foreach (HydraulicBoundaryLocationCalculationsForTargetProbability element in assessmentSection.WaveHeightCalculationsForUserDefinedTargetProbabilities)
            {
                Assert.AreEqual(2, element.HydraulicBoundaryLocationCalculations);
            }

            // When
            IEnumerable<IObservable> changedObjects = handler.RemoveHydraulicBoundaryDatabase(hydraulicBoundaryDatabase);

            // Then
            CollectionAssert.IsEmpty(assessmentSection.HydraulicBoundaryData.HydraulicBoundaryDatabases);
            CollectionAssert.IsEmpty(assessmentSection.WaterLevelCalculationsForSignalFloodingProbability);
            CollectionAssert.IsEmpty(assessmentSection.WaterLevelCalculationsForMaximumAllowableFloodingProbability);

            foreach (HydraulicBoundaryLocationCalculationsForTargetProbability element in assessmentSection.WaterLevelCalculationsForUserDefinedTargetProbabilities)
            {
                CollectionAssert.IsEmpty(element.HydraulicBoundaryLocationCalculations);
            }

            foreach (HydraulicBoundaryLocationCalculationsForTargetProbability element in assessmentSection.WaveHeightCalculationsForUserDefinedTargetProbabilities)
            {
                CollectionAssert.IsEmpty(element.HydraulicBoundaryLocationCalculations);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void GivenAssessmentSectionWithDatabase_WhenRemoving_ThenChangedObjectsReturned()
        {
            // Given
            var mocks = new MockRepository();
            var duneLocationsUpdateHandler = mocks.Stub<IDuneLocationsUpdateHandler>();
            mocks.ReplayAll();

            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike)
            {
                HydraulicBoundaryData =
                {
                    HydraulicBoundaryDatabases =
                    {
                        hydraulicBoundaryDatabase
                    }
                }
            };

            var handler = new HydraulicBoundaryDataUpdateHandler(assessmentSection, duneLocationsUpdateHandler);

            // When
            IEnumerable<IObservable> changedObjects = handler.RemoveHydraulicBoundaryDatabase(hydraulicBoundaryDatabase);

            // Then
            var observables = new List<IObservable>
            {
                assessmentSection.HydraulicBoundaryData,
                assessmentSection.HydraulicBoundaryData.HydraulicBoundaryDatabases,
                assessmentSection.HydraulicBoundaryData.Locations,
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
        public void GivenCalculationsWithLocation_WhenRemovingHydraulicBoundaryDatabase_ThenCalculationOutputClearedAndChangedObjectsReturned()
        {
            // Given
            var mocks = new MockRepository();
            var duneLocationsUpdateHandler = mocks.Stub<IDuneLocationsUpdateHandler>();
            mocks.ReplayAll();

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

            var handler = new HydraulicBoundaryDataUpdateHandler(assessmentSection, duneLocationsUpdateHandler);

            // When
            IEnumerable<IObservable> changedObjects = handler.RemoveHydraulicBoundaryDatabase(assessmentSection.HydraulicBoundaryData.HydraulicBoundaryDatabases.First());

            // Then
            Assert.IsTrue(calculationsWithOutput.All(c => !c.HasOutput));
            CollectionAssert.IsSubsetOf(calculationsWithOutput, changedObjects);
            mocks.VerifyAll();
        }

        [Test]
        public void DoPostUpdateActions_Always_Perform()
        {
            // Setup
            var mocks = new MockRepository();
            var duneLocationsUpdateHandler = mocks.StrictMock<IDuneLocationsUpdateHandler>();
            duneLocationsUpdateHandler.Expect(h => h.DoPostUpdateActions());
            mocks.ReplayAll();

            AssessmentSection assessmentSection = CreateAssessmentSection();

            var handler = new HydraulicBoundaryDataUpdateHandler(assessmentSection, duneLocationsUpdateHandler);

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
                                                                          .Single(l => l.HrdLocationId == readLocation.Id)
                                                                          .HlcdLocationId, actualLocation.Id);
                Assert.AreEqual(readLocation.Name, actualLocation.Name);
                Assert.AreEqual(readLocation.CoordinateX, actualLocation.Location.X);
                Assert.AreEqual(readLocation.CoordinateY, actualLocation.Location.Y);
            }
        }
    }
}