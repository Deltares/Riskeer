// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.Helpers;
using Riskeer.Common.Forms.TestUtil;
using Riskeer.Common.Forms.Views;

namespace Riskeer.Common.Forms.Test.Views
{
    [TestFixture]
    public class HydraulicBoundaryLocationsMapLayerTest
    {
        [Test]
        public void Constructor_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new HydraulicBoundaryLocationsMapLayer(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var assessmentSection = new AssessmentSectionStub();
            assessmentSection.SetHydraulicBoundaryLocationCalculations(new[]
            {
                new HydraulicBoundaryLocation(1, "test", 1.0, 2.0)
            });

            // Call
            var mapLayer = new HydraulicBoundaryLocationsMapLayer(assessmentSection);

            // Assert
            Assert.IsInstanceOf<IDisposable>(mapLayer);
            MapDataTestHelper.AssertHydraulicBoundaryLocationsMapData(assessmentSection, mapLayer.MapData);
        }

        [Test]
        public void GivenMapLayerWithHydraulicBoundaryLocations_WhenChangingHydraulicBoundaryLocationsDataAndObserversNotified_ThenMapDataUpdated()
        {
            // Given
            var assessmentSection = new AssessmentSectionStub();
            assessmentSection.SetHydraulicBoundaryLocationCalculations(new[]
            {
                new HydraulicBoundaryLocation(1, "test1", 1.0, 2.0)
            });

            var mapLayer = new HydraulicBoundaryLocationsMapLayer(assessmentSection);

            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            mapLayer.MapData.Attach(observer);

            // Precondition
            MapDataTestHelper.AssertHydraulicBoundaryLocationsMapData(assessmentSection, mapLayer.MapData);

            // When
            assessmentSection.SetHydraulicBoundaryLocationCalculations(new[]
            {
                new HydraulicBoundaryLocation(2, "test2", 2.0, 3.0)
            });
            assessmentSection.HydraulicBoundaryDatabase.Locations.NotifyObservers();

            // Then
            MapDataTestHelper.AssertHydraulicBoundaryLocationsMapData(assessmentSection, mapLayer.MapData);
            mocks.VerifyAll();
        }

        [Test]
        [TestCaseSource(nameof(GetCalculationFuncs))]
        public void GivenMapLayerWithHydraulicBoundaryLocationsData_WhenHydraulicBoundaryLocationCalculationUpdatedAndNotified_ThenMapDataUpdated(
            Func<IAssessmentSection, HydraulicBoundaryLocationCalculation> getCalculationFunc)
        {
            // Given
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "test1", 1.0, 2.0);
            var assessmentSection = new AssessmentSectionStub();
            assessmentSection.SetHydraulicBoundaryLocationCalculations(new[]
            {
                hydraulicBoundaryLocation
            });

            var mapLayer = new HydraulicBoundaryLocationsMapLayer(assessmentSection);

            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            mapLayer.MapData.Attach(observer);

            // Precondition
            MapDataTestHelper.AssertHydraulicBoundaryLocationsMapData(assessmentSection, mapLayer.MapData);

            // When
            HydraulicBoundaryLocationCalculation calculation = getCalculationFunc(assessmentSection);
            calculation.Output = new TestHydraulicBoundaryLocationCalculationOutput(new Random(21).NextDouble());
            calculation.NotifyObservers();

            // Then
            MapDataTestHelper.AssertHydraulicBoundaryLocationsMapData(assessmentSection, mapLayer.MapData);
            mocks.VerifyAll();
        }

        [Test]
        [TestCaseSource(nameof(GetTargetProbabilitiesFuncs))]
        public void GivenMapLayerWithHydraulicBoundaryLocationsData_WhenUserDefinedTargetProbabilitiesCollectionUpdatedAndNotified_ThenMapDataUpdated(
            Func<IAssessmentSection, ObservableList<HydraulicBoundaryLocationCalculationsForTargetProbability>> getTargetProbabilitiesFunc)
        {
            // Given
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "test1", 1.0, 2.0);
            var assessmentSection = new AssessmentSectionStub();
            assessmentSection.SetHydraulicBoundaryLocationCalculations(new[]
            {
                hydraulicBoundaryLocation
            });

            var mapLayer = new HydraulicBoundaryLocationsMapLayer(assessmentSection);

            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            mapLayer.MapData.Attach(observer);

            // Precondition
            MapDataTestHelper.AssertHydraulicBoundaryLocationsMapData(assessmentSection, mapLayer.MapData);

            // When
            ObservableList<HydraulicBoundaryLocationCalculationsForTargetProbability> targetProbabilities = getTargetProbabilitiesFunc(assessmentSection);
            var newTargetProbability = new HydraulicBoundaryLocationCalculationsForTargetProbability(0.1);
            newTargetProbability.HydraulicBoundaryLocationCalculations.AddRange(assessmentSection.HydraulicBoundaryDatabase.Locations
                                                                                                 .Select(l => new HydraulicBoundaryLocationCalculation(l)));
            targetProbabilities.Add(newTargetProbability);
            targetProbabilities.NotifyObservers();

            // Then
            MapDataTestHelper.AssertHydraulicBoundaryLocationsMapData(assessmentSection, mapLayer.MapData);
            mocks.VerifyAll();
        }

        [Test]
        [TestCaseSource(nameof(GetTargetProbabilitiesFuncs))]
        public void GivenMapLayerWithHydraulicBoundaryLocationsData_WhenTargetProbabilityUpdatedAndNotified_ThenMapDataUpdated(
            Func<IAssessmentSection, ObservableList<HydraulicBoundaryLocationCalculationsForTargetProbability>> getTargetProbabilitiesFunc)
        {
            // Given
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "test1", 1.0, 2.0);
            var assessmentSection = new AssessmentSectionStub();
            assessmentSection.SetHydraulicBoundaryLocationCalculations(new[]
            {
                hydraulicBoundaryLocation
            });

            var mapLayer = new HydraulicBoundaryLocationsMapLayer(assessmentSection);

            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            mapLayer.MapData.Attach(observer);

            // Precondition
            MapDataTestHelper.AssertHydraulicBoundaryLocationsMapData(assessmentSection, mapLayer.MapData);

            // When
            ObservableList<HydraulicBoundaryLocationCalculationsForTargetProbability> targetProbabilities = getTargetProbabilitiesFunc(assessmentSection);
            targetProbabilities.First().TargetProbability = 0.01;
            targetProbabilities.First().NotifyObservers();

            // Then
            MapDataTestHelper.AssertHydraulicBoundaryLocationsMapData(assessmentSection, mapLayer.MapData);
            mocks.VerifyAll();
        }

        [Test]
        [TestCaseSource(nameof(GetTargetProbabilitiesFuncs))]
        public void GivenMapLayerWithHydraulicBoundaryLocationsData_WhenCalculationsForUserDefinedTargetProbabilitiesUpdatedAndNotified_ThenMapDataUpdated(
            Func<IAssessmentSection, ObservableList<HydraulicBoundaryLocationCalculationsForTargetProbability>> getTargetProbabilitiesFunc)
        {
            // Given
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "test1", 1.0, 2.0);
            var assessmentSection = new AssessmentSectionStub();
            assessmentSection.SetHydraulicBoundaryLocationCalculations(new[]
            {
                hydraulicBoundaryLocation
            });

            var mapLayer = new HydraulicBoundaryLocationsMapLayer(assessmentSection);

            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            ObservableList<HydraulicBoundaryLocationCalculationsForTargetProbability> targetProbabilities = getTargetProbabilitiesFunc(assessmentSection);
            var newTargetProbability = new HydraulicBoundaryLocationCalculationsForTargetProbability(0.1);
            newTargetProbability.HydraulicBoundaryLocationCalculations.AddRange(assessmentSection.HydraulicBoundaryDatabase.Locations
                                                                                                 .Select(l => new HydraulicBoundaryLocationCalculation(l))
                                                                                                 .ToArray());
            targetProbabilities.Add(newTargetProbability);
            targetProbabilities.NotifyObservers();

            mapLayer.MapData.Attach(observer);

            // Precondition
            MapDataTestHelper.AssertHydraulicBoundaryLocationsMapData(assessmentSection, mapLayer.MapData);

            // When
            HydraulicBoundaryLocationCalculation calculation = newTargetProbability.HydraulicBoundaryLocationCalculations.First();
            calculation.Output = new TestHydraulicBoundaryLocationCalculationOutput(new Random(21).NextDouble());
            calculation.NotifyObservers();

            // Then
            MapDataTestHelper.AssertHydraulicBoundaryLocationsMapData(assessmentSection, mapLayer.MapData);
            mocks.VerifyAll();
        }

        [Test]
        [TestCaseSource(nameof(GetTargetProbabilitiesFuncs))]
        public void GivenMapLayerWithHydraulicBoundaryLocationsData_WhenCalculationsForRemovedUserDefinedTargetProbabilitiesUpdatedAndNotified_ThenMapDataNotUpdated(
            Func<IAssessmentSection, ObservableList<HydraulicBoundaryLocationCalculationsForTargetProbability>> getTargetProbabilitiesFunc)
        {
            // Given
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "test1", 1.0, 2.0);
            var assessmentSection = new AssessmentSectionStub();
            assessmentSection.SetHydraulicBoundaryLocationCalculations(new[]
            {
                hydraulicBoundaryLocation
            });

            var mapLayer = new HydraulicBoundaryLocationsMapLayer(assessmentSection);

            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            mocks.ReplayAll();

            ObservableList<HydraulicBoundaryLocationCalculationsForTargetProbability> targetProbabilities = getTargetProbabilitiesFunc(assessmentSection);
            HydraulicBoundaryLocationCalculationsForTargetProbability targetProbability = targetProbabilities.First();
            targetProbabilities.Remove(targetProbability);
            targetProbabilities.NotifyObservers();

            mapLayer.MapData.Attach(observer);

            // Precondition
            MapDataTestHelper.AssertHydraulicBoundaryLocationsMapData(assessmentSection, mapLayer.MapData);

            // When
            HydraulicBoundaryLocationCalculation calculation = targetProbability.HydraulicBoundaryLocationCalculations.First();
            calculation.Output = new TestHydraulicBoundaryLocationCalculationOutput(new Random(21).NextDouble());
            calculation.NotifyObservers();

            // Then
            MapDataTestHelper.AssertHydraulicBoundaryLocationsMapData(assessmentSection, mapLayer.MapData);
            mocks.VerifyAll();
        }

        [Test]
        [TestCaseSource(nameof(GetTargetProbabilitiesFuncsWithDisplayNames))]
        public void GivenMapLayerWithHydraulicBoundaryLocationsData_WhenSelectedTargetProbabilityRemovedAndNotified_ThenMapDataUpdated(
            Func<IAssessmentSection, ObservableList<HydraulicBoundaryLocationCalculationsForTargetProbability>> getTargetProbabilitiesFunc, string displayName)
        {
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "test1", 1.0, 2.0);
            var assessmentSection = new AssessmentSectionStub();
            assessmentSection.SetHydraulicBoundaryLocationCalculations(new[]
            {
                hydraulicBoundaryLocation
            });

            var mapLayer = new HydraulicBoundaryLocationsMapLayer(assessmentSection);

            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            ObservableList<HydraulicBoundaryLocationCalculationsForTargetProbability> targetProbabilities = getTargetProbabilitiesFunc(assessmentSection);
            HydraulicBoundaryLocationCalculationsForTargetProbability calculationsForTargetProbability = targetProbabilities.First();
            mapLayer.MapData.SelectedMetaDataAttribute = string.Format(displayName, ProbabilityFormattingHelper.Format(calculationsForTargetProbability.TargetProbability));
            mapLayer.MapData.NotifyObservers();

            mapLayer.MapData.Attach(observer);

            // Precondition
            MapDataTestHelper.AssertHydraulicBoundaryLocationsMapData(assessmentSection, mapLayer.MapData);

            // When
            targetProbabilities.Remove(calculationsForTargetProbability);
            targetProbabilities.NotifyObservers();

            // Then
            MapDataTestHelper.AssertHydraulicBoundaryLocationsMapData(assessmentSection, mapLayer.MapData);
            Assert.AreEqual("Naam", mapLayer.MapData.SelectedMetaDataAttribute);
            mocks.VerifyAll();
        }

        [Test]
        [TestCaseSource(nameof(GetTargetProbabilitiesFuncsWithDisplayNames))]
        public void GivenMapLayerWithHydraulicBoundaryLocationsData_WhenNotSelectedTargetProbabilityRemovedAndNotified_ThenMapDataUpdated(
            Func<IAssessmentSection, ObservableList<HydraulicBoundaryLocationCalculationsForTargetProbability>> getTargetProbabilitiesFunc, string displayName)
        {
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "test1", 1.0, 2.0);
            var assessmentSection = new AssessmentSectionStub();
            assessmentSection.SetHydraulicBoundaryLocationCalculations(new[]
            {
                hydraulicBoundaryLocation
            });

            var mapLayer = new HydraulicBoundaryLocationsMapLayer(assessmentSection);

            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            ObservableList<HydraulicBoundaryLocationCalculationsForTargetProbability> targetProbabilities = getTargetProbabilitiesFunc(assessmentSection);
            HydraulicBoundaryLocationCalculationsForTargetProbability calculationsForTargetProbabilityToRemove = targetProbabilities.First();
            HydraulicBoundaryLocationCalculationsForTargetProbability calculationsForTargetProbability = targetProbabilities.Last();
            mapLayer.MapData.SelectedMetaDataAttribute = string.Format(displayName, ProbabilityFormattingHelper.Format(calculationsForTargetProbability.TargetProbability));
            mapLayer.MapData.NotifyObservers();

            mapLayer.MapData.Attach(observer);

            // Precondition
            MapDataTestHelper.AssertHydraulicBoundaryLocationsMapData(assessmentSection, mapLayer.MapData);

            // When
            targetProbabilities.Remove(calculationsForTargetProbabilityToRemove);
            targetProbabilities.NotifyObservers();

            // Then
            MapDataTestHelper.AssertHydraulicBoundaryLocationsMapData(assessmentSection, mapLayer.MapData);
            Assert.AreEqual(string.Format(displayName, ProbabilityFormattingHelper.Format(calculationsForTargetProbability.TargetProbability)),
                            mapLayer.MapData.SelectedMetaDataAttribute);
            mocks.VerifyAll();
        }

        [Test]
        [TestCaseSource(nameof(GetTargetProbabilitiesFuncsWithDisplayNames))]
        public void GivenMapLayerWithHydraulicBoundaryLocationsData_WhenSelectedTargetProbabilityChangedAndNotified_ThenMapDataUpdated(
            Func<IAssessmentSection, ObservableList<HydraulicBoundaryLocationCalculationsForTargetProbability>> getTargetProbabilitiesFunc, string displayName)
        {
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "test1", 1.0, 2.0);
            var assessmentSection = new AssessmentSectionStub();
            assessmentSection.SetHydraulicBoundaryLocationCalculations(new[]
            {
                hydraulicBoundaryLocation
            });

            var mapLayer = new HydraulicBoundaryLocationsMapLayer(assessmentSection);

            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            HydraulicBoundaryLocationCalculationsForTargetProbability calculationsForTargetProbability = getTargetProbabilitiesFunc(assessmentSection).First();
            mapLayer.MapData.SelectedMetaDataAttribute = string.Format(displayName, ProbabilityFormattingHelper.Format(calculationsForTargetProbability.TargetProbability));
            mapLayer.MapData.NotifyObservers();

            mapLayer.MapData.Attach(observer);

            // Precondition
            MapDataTestHelper.AssertHydraulicBoundaryLocationsMapData(assessmentSection, mapLayer.MapData);

            // When
            calculationsForTargetProbability.TargetProbability = 0.0004;
            calculationsForTargetProbability.NotifyObservers();

            // Then
            MapDataTestHelper.AssertHydraulicBoundaryLocationsMapData(assessmentSection, mapLayer.MapData);
            Assert.AreEqual(string.Format(displayName, ProbabilityFormattingHelper.Format(calculationsForTargetProbability.TargetProbability)), mapLayer.MapData.SelectedMetaDataAttribute);
            mocks.VerifyAll();
        }

        private static IEnumerable<TestCaseData> GetCalculationFuncs()
        {
            yield return new TestCaseData(new Func<IAssessmentSection, HydraulicBoundaryLocationCalculation>(
                                              assessmentSection => assessmentSection.WaterLevelCalculationsForSignalingNorm.First()));
            yield return new TestCaseData(new Func<IAssessmentSection, HydraulicBoundaryLocationCalculation>(
                                              assessmentSection => assessmentSection.WaterLevelCalculationsForLowerLimitNorm.First()));
            yield return new TestCaseData(new Func<IAssessmentSection, HydraulicBoundaryLocationCalculation>(
                                              assessmentSection => assessmentSection.WaterLevelCalculationsForUserDefinedTargetProbabilities.First()
                                                                                    .HydraulicBoundaryLocationCalculations.First()));
            yield return new TestCaseData(new Func<IAssessmentSection, HydraulicBoundaryLocationCalculation>(
                                              assessmentSection => assessmentSection.WaveHeightCalculationsForUserDefinedTargetProbabilities.First()
                                                                                    .HydraulicBoundaryLocationCalculations.First()));
        }

        private static IEnumerable<TestCaseData> GetTargetProbabilitiesFuncs()
        {
            yield return new TestCaseData(new Func<IAssessmentSection, ObservableList<HydraulicBoundaryLocationCalculationsForTargetProbability>>(
                                              assessmentSection => assessmentSection.WaterLevelCalculationsForUserDefinedTargetProbabilities));
            yield return new TestCaseData(new Func<IAssessmentSection, ObservableList<HydraulicBoundaryLocationCalculationsForTargetProbability>>(
                                              assessmentSection => assessmentSection.WaveHeightCalculationsForUserDefinedTargetProbabilities));
        }

        private static IEnumerable<TestCaseData> GetTargetProbabilitiesFuncsWithDisplayNames()
        {
            yield return new TestCaseData(new Func<IAssessmentSection, ObservableList<HydraulicBoundaryLocationCalculationsForTargetProbability>>(
                                              assessmentSection => assessmentSection.WaterLevelCalculationsForUserDefinedTargetProbabilities),
                                          "h - {0}");
            yield return new TestCaseData(new Func<IAssessmentSection, ObservableList<HydraulicBoundaryLocationCalculationsForTargetProbability>>(
                                              assessmentSection => assessmentSection.WaveHeightCalculationsForUserDefinedTargetProbabilities),
                                          "Hs - {0}");
        }
    }
}