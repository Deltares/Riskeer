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
        public void GivenMapLayerWithHydraulicBoundaryLocationsData_WhenWaterLevelCalculationsForUserDefinedTargetProbabilitiesUpdatedAndNotified_ThenMapDataUpdated(
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
            targetProbabilities.Add(new HydraulicBoundaryLocationCalculationsForTargetProbability(0.1));
            targetProbabilities.NotifyObservers();

            // Then
            MapDataTestHelper.AssertHydraulicBoundaryLocationsMapData(assessmentSection, mapLayer.MapData);
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
    }
}