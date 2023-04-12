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
            var duneLocationsReplacementHandler = mocks.Stub<IDuneLocationsReplacementHandler>();
            mocks.ReplayAll();

            // Call
            void Call() => new HydraulicBoundaryDataUpdateHandler(null, duneLocationsReplacementHandler);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_DuneLocationsReplacementHandlerNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new HydraulicBoundaryDataUpdateHandler(CreateAssessmentSection(), null);

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
            var handler = new HydraulicBoundaryDataUpdateHandler(CreateAssessmentSection(), duneLocationsReplacementHandler);

            // Assert
            Assert.IsInstanceOf<IHydraulicBoundaryDataUpdateHandler>(handler);
            mocks.VerifyAll();
        }

        [Test]
        public void Update_HydraulicBoundaryDataNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var duneLocationsReplacementHandler = mocks.Stub<IDuneLocationsReplacementHandler>();
            mocks.ReplayAll();

            ReadHydraulicBoundaryDatabase readHydraulicBoundaryDatabase = ReadHydraulicBoundaryDatabaseTestFactory.Create();

            var handler = new HydraulicBoundaryDataUpdateHandler(CreateAssessmentSection(), duneLocationsReplacementHandler);

            // Call
            void Call()
            {
                handler.Update(null, readHydraulicBoundaryDatabase,
                               ReadHydraulicLocationConfigurationDatabaseTestFactory.Create(readHydraulicBoundaryDatabase.TrackId),
                               Enumerable.Empty<long>(), "");
            }

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("hydraulicBoundaryData", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Update_ReadHydraulicBoundaryDatabaseNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var duneLocationsReplacementHandler = mocks.Stub<IDuneLocationsReplacementHandler>();
            mocks.ReplayAll();

            var handler = new HydraulicBoundaryDataUpdateHandler(CreateAssessmentSection(), duneLocationsReplacementHandler);

            // Call
            void Call() => handler.Update(new HydraulicBoundaryData(), null,
                                          ReadHydraulicLocationConfigurationDatabaseTestFactory.Create(1),
                                          Enumerable.Empty<long>(), "");

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

            var handler = new HydraulicBoundaryDataUpdateHandler(CreateAssessmentSection(), duneLocationsReplacementHandler);

            // Call
            void Call() => handler.Update(new HydraulicBoundaryData(), ReadHydraulicBoundaryDatabaseTestFactory.Create(),
                                          null, Enumerable.Empty<long>(), "");

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

            ReadHydraulicBoundaryDatabase readHydraulicBoundaryDatabase = ReadHydraulicBoundaryDatabaseTestFactory.Create();

            var handler = new HydraulicBoundaryDataUpdateHandler(CreateAssessmentSection(), duneLocationsReplacementHandler);

            // Call
            void Call()
            {
                handler.Update(new HydraulicBoundaryData(), readHydraulicBoundaryDatabase,
                               ReadHydraulicLocationConfigurationDatabaseTestFactory.Create(readHydraulicBoundaryDatabase.TrackId),
                               null, "");
            }

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("excludedLocationIds", exception.ParamName);
        }

        [Test]
        public void Update_HrdFilePathNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var duneLocationsReplacementHandler = mocks.Stub<IDuneLocationsReplacementHandler>();
            mocks.ReplayAll();

            ReadHydraulicBoundaryDatabase readHydraulicBoundaryDatabase = ReadHydraulicBoundaryDatabaseTestFactory.Create();

            var handler = new HydraulicBoundaryDataUpdateHandler(CreateAssessmentSection(), duneLocationsReplacementHandler);

            // Call
            void Call()
            {
                handler.Update(new HydraulicBoundaryData(), readHydraulicBoundaryDatabase,
                               ReadHydraulicLocationConfigurationDatabaseTestFactory.Create(readHydraulicBoundaryDatabase.TrackId),
                               Enumerable.Empty<long>(), null);
            }

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("hrdFilePath", exception.ParamName);
        }

        [Test]
        public void Update_ValidData_SetsAllData()
        {
            // Setup
            var hydraulicBoundaryData = new HydraulicBoundaryData();

            var mocks = new MockRepository();
            var duneLocationsReplacementHandler = mocks.StrictMock<IDuneLocationsReplacementHandler>();
            duneLocationsReplacementHandler.Expect(h => h.Replace(Arg<IEnumerable<HydraulicBoundaryLocation>>.Is.NotNull))
                                           .WhenCalled(invocation =>
                                           {
                                               Assert.AreSame(hydraulicBoundaryData.Locations, invocation.Arguments[0]);
                                           });
            mocks.ReplayAll();

            const string hrdFilePath = "some/file/path";
            AssessmentSection assessmentSection = CreateAssessmentSection();
            var handler = new HydraulicBoundaryDataUpdateHandler(assessmentSection, duneLocationsReplacementHandler);

            ReadHydraulicBoundaryDatabase readHydraulicBoundaryDatabase = ReadHydraulicBoundaryDatabaseTestFactory.Create();
            ReadHydraulicLocationConfigurationDatabase readHydraulicLocationConfigurationDatabase =
                ReadHydraulicLocationConfigurationDatabaseTestFactory.Create(readHydraulicBoundaryDatabase.TrackId);

            // Precondition
            Assert.IsFalse(hydraulicBoundaryData.IsLinked());

            // Call
            handler.Update(hydraulicBoundaryData, readHydraulicBoundaryDatabase, readHydraulicLocationConfigurationDatabase,
                           Enumerable.Empty<long>(), hrdFilePath);

            // Assert
            Assert.IsTrue(hydraulicBoundaryData.IsLinked());
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
        public void Update_HrdLocationIdsNotInHlcdLocationIds_ThenLocationsNotAdded()
        {
            var hydraulicBoundaryData = new HydraulicBoundaryData();

            var mocks = new MockRepository();
            var duneLocationsReplacementHandler = mocks.Stub<IDuneLocationsReplacementHandler>();
            mocks.ReplayAll();

            const string hrdFilePath = "some/file/path";
            AssessmentSection assessmentSection = CreateAssessmentSection();
            var handler = new HydraulicBoundaryDataUpdateHandler(assessmentSection, duneLocationsReplacementHandler);

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
            handler.Update(hydraulicBoundaryData, readHydraulicBoundaryDatabase, readHydraulicLocationConfigurationDatabase,
                           Enumerable.Empty<long>(), hrdFilePath);

            // Assert
            AssertHydraulicBoundaryLocations(readHydraulicBoundaryLocationsToInclude, readHydraulicLocationConfigurationDatabase,
                                             hydraulicBoundaryData.Locations, readHydraulicBoundaryDatabase.TrackId);
            mocks.VerifyAll();
        }

        [Test]
        public void Update_HrdLocationIdsInExcludedLocationIds_LocationsNotAdded()
        {
            var hydraulicBoundaryData = new HydraulicBoundaryData();

            var mocks = new MockRepository();
            var duneLocationsReplacementHandler = mocks.Stub<IDuneLocationsReplacementHandler>();
            mocks.ReplayAll();

            const string hrdFilePath = "some/file/path";
            AssessmentSection assessmentSection = CreateAssessmentSection();
            var handler = new HydraulicBoundaryDataUpdateHandler(assessmentSection, duneLocationsReplacementHandler);

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

            // Precondition
            Assert.IsFalse(hydraulicBoundaryData.IsLinked());

            // Call
            handler.Update(hydraulicBoundaryData, readHydraulicBoundaryDatabase, readHydraulicLocationConfigurationDatabase,
                           readHydraulicBoundaryLocationsToExclude.Select(l => l.Id), hrdFilePath);

            // Assert
            AssertHydraulicBoundaryLocations(readHydraulicBoundaryLocationsToInclude, readHydraulicLocationConfigurationDatabase,
                                             hydraulicBoundaryData.Locations, readHydraulicBoundaryDatabase.TrackId);
            mocks.VerifyAll();
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

            const string hrdFilePath = "some/file/path";
            AssessmentSection assessmentSection = CreateAssessmentSection();
            var hydraulicBoundaryData = new HydraulicBoundaryData();
            if (isLinked)
            {
                hydraulicBoundaryData.FilePath = hrdFilePath;
                hydraulicBoundaryData.Version = "1";
                hydraulicBoundaryData.Locations.AddRange(new[]
                {
                    new TestHydraulicBoundaryLocation("old location 1"),
                    new TestHydraulicBoundaryLocation("old location 2"),
                    new TestHydraulicBoundaryLocation("old location 3")
                });
            }

            ReadHydraulicBoundaryDatabase readHydraulicBoundaryDatabase = ReadHydraulicBoundaryDatabaseTestFactory.Create();

            var handler = new HydraulicBoundaryDataUpdateHandler(assessmentSection, duneLocationsReplacementHandler);

            // Precondition
            Assert.AreEqual(isLinked, hydraulicBoundaryData.IsLinked());

            // When
            IEnumerable<IObservable> changedObjects = handler.Update(hydraulicBoundaryData, readHydraulicBoundaryDatabase,
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
        public void GivenCalculationsWithLocation_WhenUpdatingDatabaseWithNewLocations_ThenCalculationOutputClearedAndChangedObjectsReturned()
        {
            // Given
            var mocks = new MockRepository();
            var duneLocationsReplacementHandler = mocks.Stub<IDuneLocationsReplacementHandler>();
            mocks.ReplayAll();

            const string hrdFilePath = "some/file/path";
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

            ReadHydraulicBoundaryDatabase readHydraulicBoundaryDatabase = ReadHydraulicBoundaryDatabaseTestFactory.Create();

            var handler = new HydraulicBoundaryDataUpdateHandler(assessmentSection, duneLocationsReplacementHandler);

            // When
            IEnumerable<IObservable> changedObjects = handler.Update(assessmentSection.HydraulicBoundaryData, readHydraulicBoundaryDatabase,
                                                                     ReadHydraulicLocationConfigurationDatabaseTestFactory.Create(readHydraulicBoundaryDatabase.TrackId),
                                                                     Enumerable.Empty<long>(), hrdFilePath);

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
            var duneLocationsReplacementHandler = mocks.StrictMock<IDuneLocationsReplacementHandler>();
            duneLocationsReplacementHandler.Stub(h => h.Replace(null)).IgnoreArguments();
            duneLocationsReplacementHandler.Expect(h => h.DoPostReplacementUpdates());
            mocks.ReplayAll();

            const string hrdFilePath = "old/file/path";
            AssessmentSection assessmentSection = CreateAssessmentSection();
            var hydraulicBoundaryData = new HydraulicBoundaryData
            {
                FilePath = hrdFilePath,
                Version = "1",
                Locations =
                {
                    new TestHydraulicBoundaryLocation("old location 1"),
                    new TestHydraulicBoundaryLocation("old location 2"),
                    new TestHydraulicBoundaryLocation("old location 3")
                }
            };

            ReadHydraulicBoundaryDatabase readHydraulicBoundaryDatabase = ReadHydraulicBoundaryDatabaseTestFactory.Create();

            var handler = new HydraulicBoundaryDataUpdateHandler(assessmentSection, duneLocationsReplacementHandler);

            IEnumerable<IObservable> changedObjects = handler.Update(hydraulicBoundaryData, readHydraulicBoundaryDatabase,
                                                                     ReadHydraulicLocationConfigurationDatabaseTestFactory.Create(readHydraulicBoundaryDatabase.TrackId),
                                                                     Enumerable.Empty<long>(), hrdFilePath);

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
                                                                          .Single(l => l.HrdLocationId == readLocation.Id)
                                                                          .HlcdLocationId, actualLocation.Id);
                Assert.AreEqual(readLocation.Name, actualLocation.Name);
                Assert.AreEqual(readLocation.CoordinateX, actualLocation.Location.X);
                Assert.AreEqual(readLocation.CoordinateY, actualLocation.Location.Y);
            }
        }
    }
}