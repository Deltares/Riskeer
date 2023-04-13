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
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.TestUtil;
using Riskeer.DuneErosion.Plugin.Handlers;
using Riskeer.Integration.Data;
using Riskeer.Integration.Plugin.Handlers;
using Riskeer.Integration.TestUtil;
using Riskeer.MacroStabilityInwards.Data;
using Riskeer.Piping.Data.SemiProbabilistic;
using Riskeer.Piping.Data.TestUtil;

namespace Riskeer.Integration.Plugin.Test.Handlers
{
    [TestFixture]
    public class HydraulicBoundaryDatabaseRemoveHandlerTest
    {
        [Test]
        public void Constructor_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var duneLocationsRemoveHandler = mocks.Stub<IDuneLocationsRemoveHandler>();
            mocks.ReplayAll();

            // Call
            void Call() => new HydraulicBoundaryDatabaseRemoveHandler(null, duneLocationsRemoveHandler);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_DuneLocationsRemoveHandlerNull_ThrowsArgumentNullException()
        {
            // Setup
            var random = new Random(21);
            var assessmentSection = new AssessmentSection(random.NextEnumValue<AssessmentSectionComposition>());

            // Call
            void Call() => new HydraulicBoundaryDatabaseRemoveHandler(assessmentSection, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("duneLocationsRemoveHandler", exception.ParamName);
        }

        [Test]
        public void RemoveHydraulicBoundaryDatabase_HydraulicBoundaryDatabaseNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var duneLocationsRemoveHandler = mocks.Stub<IDuneLocationsRemoveHandler>();
            mocks.ReplayAll();

            var random = new Random(21);
            var assessmentSection = new AssessmentSection(random.NextEnumValue<AssessmentSectionComposition>());

            var handler = new HydraulicBoundaryDatabaseRemoveHandler(assessmentSection, duneLocationsRemoveHandler);

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
            var duneLocationsRemoveHandler = mocks.StrictMock<IDuneLocationsRemoveHandler>();
            duneLocationsRemoveHandler.Expect(dlrh => dlrh.RemoveLocations(new[]
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

            var handler = new HydraulicBoundaryDatabaseRemoveHandler(assessmentSection, duneLocationsRemoveHandler);

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
            var duneLocationsRemoveHandler = mocks.Stub<IDuneLocationsRemoveHandler>();
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

            var handler = new HydraulicBoundaryDatabaseRemoveHandler(assessmentSection, duneLocationsRemoveHandler);

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
            var duneLocationsRemoveHandler = mocks.Stub<IDuneLocationsRemoveHandler>();
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

            var handler = new HydraulicBoundaryDatabaseRemoveHandler(assessmentSection, duneLocationsRemoveHandler);

            // When
            IEnumerable<IObservable> changedObjects = handler.RemoveHydraulicBoundaryDatabase(assessmentSection.HydraulicBoundaryData.HydraulicBoundaryDatabases.First());

            // Then
            Assert.IsTrue(calculationsWithOutput.All(c => !c.HasOutput));
            CollectionAssert.IsSubsetOf(calculationsWithOutput, changedObjects);
            mocks.VerifyAll();
        }

        [Test]
        public void DoPostRemoveActions_Always_PerformsPostRemoveActions()
        {
            // Setup
            var mocks = new MockRepository();
            var duneLocationsRemoveHandler = mocks.StrictMock<IDuneLocationsRemoveHandler>();
            duneLocationsRemoveHandler.Expect(dlrh => dlrh.DoPostRemoveActions());
            mocks.ReplayAll();

            var random = new Random(21);
            var assessmentSection = new AssessmentSection(random.NextEnumValue<AssessmentSectionComposition>());

            var handler = new HydraulicBoundaryDatabaseRemoveHandler(assessmentSection, duneLocationsRemoveHandler);

            // Call
            handler.DoPostRemoveActions();

            // Assert
            mocks.VerifyAll();
        }
    }
}