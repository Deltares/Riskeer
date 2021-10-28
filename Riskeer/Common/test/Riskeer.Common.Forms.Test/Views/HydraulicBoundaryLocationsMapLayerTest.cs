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
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Contribution;
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
        private const string waterLevelDisplayNameFormat = "h - {0}";
        private const string waveHeightDisplayNameFormat = "Hs - {0}";

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
            using (var mapLayer = new HydraulicBoundaryLocationsMapLayer(assessmentSection))
            {
                // Assert
                Assert.IsInstanceOf<IDisposable>(mapLayer);
                MapDataTestHelper.AssertHydraulicBoundaryLocationsMapData(assessmentSection, mapLayer.MapData);
            }
        }

        [Test]
        public void GivenMapLayerWithHydraulicBoundaryLocations_WhenChangingHydraulicBoundaryLocationsDataAndObserversNotified_ThenMapDataUpdated()
        {
            // Given
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var assessmentSection = new AssessmentSectionStub();
            assessmentSection.SetHydraulicBoundaryLocationCalculations(new[]
            {
                new HydraulicBoundaryLocation(1, "test1", 1.0, 2.0)
            });

            using (var mapLayer = new HydraulicBoundaryLocationsMapLayer(assessmentSection))
            {
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
            }

            mocks.VerifyAll();
        }

        [Test]
        public void GivenMapLayerWithHydraulicBoundaryLocationsData_WhenFailureMechanismContributionUpdatedAndNotified_ThenMapDataUpdated()
        {
            // Given
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var assessmentSection = new AssessmentSectionStub();
            assessmentSection.SetHydraulicBoundaryLocationCalculations(new[]
            {
                new HydraulicBoundaryLocation(1, "test1", 1.0, 2.0)
            });

            using (var mapLayer = new HydraulicBoundaryLocationsMapLayer(assessmentSection))
            {
                mapLayer.MapData.Attach(observer);

                // Precondition
                MapDataTestHelper.AssertHydraulicBoundaryLocationsMapData(assessmentSection, mapLayer.MapData);

                // When
                var random = new Random(21);
                FailureMechanismContribution contribution = assessmentSection.FailureMechanismContribution;
                contribution.LowerLimitNorm = random.NextDouble(0.000001, 0.1);
                contribution.SignalingNorm = contribution.LowerLimitNorm - 0.000001;
                contribution.NotifyObservers();

                // Then
                MapDataTestHelper.AssertHydraulicBoundaryLocationsMapData(assessmentSection, mapLayer.MapData);
            }

            mocks.VerifyAll();
        }

        [Test]
        [TestCaseSource(nameof(GetCalculationFuncs))]
        public void GivenMapLayerWithHydraulicBoundaryLocationsData_WhenHydraulicBoundaryLocationCalculationUpdatedAndNotified_ThenMapDataUpdated(
            Func<IAssessmentSection, HydraulicBoundaryLocationCalculation> getCalculationFunc)
        {
            // Given
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "test1", 1.0, 2.0);
            var assessmentSection = new AssessmentSectionStub();
            assessmentSection.SetHydraulicBoundaryLocationCalculations(new[]
            {
                hydraulicBoundaryLocation
            });

            using (var mapLayer = new HydraulicBoundaryLocationsMapLayer(assessmentSection))
            {
                mapLayer.MapData.Attach(observer);

                // Precondition
                MapDataTestHelper.AssertHydraulicBoundaryLocationsMapData(assessmentSection, mapLayer.MapData);

                // When
                HydraulicBoundaryLocationCalculation calculation = getCalculationFunc(assessmentSection);
                calculation.Output = new TestHydraulicBoundaryLocationCalculationOutput(new Random(21).NextDouble());
                calculation.NotifyObservers();

                // Then
                MapDataTestHelper.AssertHydraulicBoundaryLocationsMapData(assessmentSection, mapLayer.MapData);
            }

            mocks.VerifyAll();
        }

        [Test]
        [TestCaseSource(nameof(GetTargetProbabilitiesFuncs))]
        public void GivenMapLayerWithHydraulicBoundaryLocationsData_WhenUserDefinedTargetProbabilitiesCollectionUpdatedAndNotified_ThenMapDataUpdated(
            Func<IAssessmentSection, ObservableList<HydraulicBoundaryLocationCalculationsForTargetProbability>> getTargetProbabilitiesFunc)
        {
            // Given
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "test1", 1.0, 2.0);
            var assessmentSection = new AssessmentSectionStub();
            assessmentSection.SetHydraulicBoundaryLocationCalculations(new[]
            {
                hydraulicBoundaryLocation
            });

            using (var mapLayer = new HydraulicBoundaryLocationsMapLayer(assessmentSection))
            {
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
            }

            mocks.VerifyAll();
        }

        [Test]
        [TestCaseSource(nameof(GetTargetProbabilitiesFuncs))]
        public void GivenMapLayerWithHydraulicBoundaryLocationsData_WhenTargetProbabilityUpdatedAndNotified_ThenMapDataUpdated(
            Func<IAssessmentSection, ObservableList<HydraulicBoundaryLocationCalculationsForTargetProbability>> getTargetProbabilitiesFunc)
        {
            // Given
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "test1", 1.0, 2.0);
            var assessmentSection = new AssessmentSectionStub();
            assessmentSection.SetHydraulicBoundaryLocationCalculations(new[]
            {
                hydraulicBoundaryLocation
            });

            using (var mapLayer = new HydraulicBoundaryLocationsMapLayer(assessmentSection))
            {
                mapLayer.MapData.Attach(observer);

                // Precondition
                MapDataTestHelper.AssertHydraulicBoundaryLocationsMapData(assessmentSection, mapLayer.MapData);

                // When
                ObservableList<HydraulicBoundaryLocationCalculationsForTargetProbability> targetProbabilities = getTargetProbabilitiesFunc(assessmentSection);
                targetProbabilities.First().TargetProbability = 0.01;
                targetProbabilities.First().NotifyObservers();

                // Then
                MapDataTestHelper.AssertHydraulicBoundaryLocationsMapData(assessmentSection, mapLayer.MapData);
            }

            mocks.VerifyAll();
        }

        [Test]
        [TestCaseSource(nameof(GetTargetProbabilitiesFuncs))]
        public void GivenMapLayerWithHydraulicBoundaryLocationsData_WhenCalculationsForUserDefinedTargetProbabilitiesUpdatedAndNotified_ThenMapDataUpdated(
            Func<IAssessmentSection, ObservableList<HydraulicBoundaryLocationCalculationsForTargetProbability>> getTargetProbabilitiesFunc)
        {
            // Given
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "test1", 1.0, 2.0);
            var assessmentSection = new AssessmentSectionStub();
            assessmentSection.SetHydraulicBoundaryLocationCalculations(new[]
            {
                hydraulicBoundaryLocation
            });

            using (var mapLayer = new HydraulicBoundaryLocationsMapLayer(assessmentSection))
            {
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
            }

            mocks.VerifyAll();
        }

        [Test]
        [TestCaseSource(nameof(GetTargetProbabilitiesFuncs))]
        public void GivenMapLayerWithHydraulicBoundaryLocationsData_WhenCalculationsForRemovedUserDefinedTargetProbabilitiesUpdatedAndNotified_ThenMapDataNotUpdated(
            Func<IAssessmentSection, ObservableList<HydraulicBoundaryLocationCalculationsForTargetProbability>> getTargetProbabilitiesFunc)
        {
            // Given
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            mocks.ReplayAll();

            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "test1", 1.0, 2.0);
            var assessmentSection = new AssessmentSectionStub();
            assessmentSection.SetHydraulicBoundaryLocationCalculations(new[]
            {
                hydraulicBoundaryLocation
            });

            using (var mapLayer = new HydraulicBoundaryLocationsMapLayer(assessmentSection))
            {
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
            }

            mocks.VerifyAll();
        }

        [Test]
        [TestCaseSource(nameof(GetTargetProbabilitiesFuncsWithDisplayNameFormat))]
        public void GivenMapLayerWithHydraulicBoundaryLocationsData_WhenSelectedTargetProbabilityRemovedAndNotified_ThenMapDataUpdatedSelectedMetaDataAttributeResetToDefault(
            Func<IAssessmentSection, ObservableList<HydraulicBoundaryLocationCalculationsForTargetProbability>> getTargetProbabilitiesFunc, string displayName)
        {
            // Given
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "test1", 1.0, 2.0);
            var assessmentSection = new AssessmentSectionStub();
            assessmentSection.SetHydraulicBoundaryLocationCalculations(new[]
            {
                hydraulicBoundaryLocation
            });

            using (var mapLayer = new HydraulicBoundaryLocationsMapLayer(assessmentSection))
            {
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
            }

            mocks.VerifyAll();
        }

        [Test]
        [TestCaseSource(nameof(GetTargetProbabilitiesFuncsWithDisplayNameFormat))]
        public void GivenMapLayerWithHydraulicBoundaryLocationsData_WhenNotSelectedTargetProbabilityRemovedAndNotified_ThenMapDataUpdated(
            Func<IAssessmentSection, ObservableList<HydraulicBoundaryLocationCalculationsForTargetProbability>> getTargetProbabilitiesFunc, string displayName)
        {
            // Given
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "test1", 1.0, 2.0);
            var assessmentSection = new AssessmentSectionStub();
            assessmentSection.SetHydraulicBoundaryLocationCalculations(new[]
            {
                hydraulicBoundaryLocation
            });

            using (var mapLayer = new HydraulicBoundaryLocationsMapLayer(assessmentSection))
            {
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
            }

            mocks.VerifyAll();
        }

        [Test]
        [TestCaseSource(nameof(GetTargetProbabilitiesFuncsWithDisplayNameFormat))]
        public void GivenMapLayerWithHydraulicBoundaryLocationsData_WhenSelectedTargetProbabilityChangedAndNotified_ThenMapDataAndSelectedMetaDataAttributeUpdated(
            Func<IAssessmentSection, ObservableList<HydraulicBoundaryLocationCalculationsForTargetProbability>> getTargetProbabilitiesFunc, string displayName)
        {
            // Given
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "test1", 1.0, 2.0);
            var assessmentSection = new AssessmentSectionStub();
            assessmentSection.SetHydraulicBoundaryLocationCalculations(new[]
            {
                hydraulicBoundaryLocation
            });

            using (var mapLayer = new HydraulicBoundaryLocationsMapLayer(assessmentSection))
            {
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
                Assert.AreEqual(string.Format(displayName, ProbabilityFormattingHelper.Format(calculationsForTargetProbability.TargetProbability)),
                                mapLayer.MapData.SelectedMetaDataAttribute);
            }

            mocks.VerifyAll();
        }

        [Test]
        [TestCaseSource(nameof(GetTargetProbabilityItemShiftActions))]
        public void GivenMapLayerWithHydraulicBoundaryLocationsData_WhenSelectedWaterLevelTargetProbabilityIndexUpdatedAndCollectionNotified_ThenMapDataAndSelectedMetaDataAttributeUpdated(
            Action<ObservableList<HydraulicBoundaryLocationCalculationsForTargetProbability>> shiftItemAction,
            string selectedTargetProbabilityFormat, string expectedSelectedTargetProbabilityFormat)
        {
            // Given
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            const double targetProbability = 0.001;
            var assessmentSection = new AssessmentSectionStub();
            assessmentSection.WaterLevelCalculationsForUserDefinedTargetProbabilities.Clear();
            assessmentSection.WaterLevelCalculationsForUserDefinedTargetProbabilities.AddRange(new[]
            {
                new HydraulicBoundaryLocationCalculationsForTargetProbability(targetProbability),
                new HydraulicBoundaryLocationCalculationsForTargetProbability(targetProbability),
                new HydraulicBoundaryLocationCalculationsForTargetProbability(targetProbability)
            });

            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "test1", 1.0, 2.0);
            assessmentSection.SetHydraulicBoundaryLocationCalculations(new[]
            {
                hydraulicBoundaryLocation
            });

            using (var mapLayer = new HydraulicBoundaryLocationsMapLayer(assessmentSection))
            {
                string targetProbabilityString = ProbabilityFormattingHelper.Format(targetProbability);
                string selectedProbabilityString = string.Format(selectedTargetProbabilityFormat, targetProbabilityString);
                mapLayer.MapData.SelectedMetaDataAttribute = string.Format(waterLevelDisplayNameFormat, selectedProbabilityString);
                mapLayer.MapData.NotifyObservers();

                mapLayer.MapData.Attach(observer);

                // Precondition
                MapDataTestHelper.AssertHydraulicBoundaryLocationsMapData(assessmentSection, mapLayer.MapData);

                // When
                ObservableList<HydraulicBoundaryLocationCalculationsForTargetProbability> waterLevelCalculationsForUserDefinedTargetProbabilities =
                    assessmentSection.WaterLevelCalculationsForUserDefinedTargetProbabilities;
                shiftItemAction(waterLevelCalculationsForUserDefinedTargetProbabilities);
                waterLevelCalculationsForUserDefinedTargetProbabilities.NotifyObservers();

                // Then
                MapDataTestHelper.AssertHydraulicBoundaryLocationsMapData(assessmentSection, mapLayer.MapData);

                string expectedSelectedProbabilityString = string.Format(expectedSelectedTargetProbabilityFormat, targetProbabilityString);
                Assert.AreEqual(string.Format(waterLevelDisplayNameFormat, expectedSelectedProbabilityString),
                                mapLayer.MapData.SelectedMetaDataAttribute);
            }

            mocks.VerifyAll();
        }

        [Test]
        [TestCaseSource(nameof(GetTargetProbabilityItemShiftActions))]
        public void GivenMapLayerWithHydraulicBoundaryLocationsData_WhenSelectedWaveHeightTargetProbabilityIndexUpdatedAndCollectionNotified_ThenMapDataAndSelectedMetaDataAttributeUpdated(
            Action<ObservableList<HydraulicBoundaryLocationCalculationsForTargetProbability>> shiftItemAction,
            string selectedTargetProbabilityFormat, string expectedSelectedTargetProbabilityFormat)
        {
            // Given
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            const double targetProbability = 0.001;
            var assessmentSection = new AssessmentSectionStub();
            assessmentSection.WaveHeightCalculationsForUserDefinedTargetProbabilities.Clear();
            assessmentSection.WaveHeightCalculationsForUserDefinedTargetProbabilities.AddRange(new[]
            {
                new HydraulicBoundaryLocationCalculationsForTargetProbability(targetProbability),
                new HydraulicBoundaryLocationCalculationsForTargetProbability(targetProbability),
                new HydraulicBoundaryLocationCalculationsForTargetProbability(targetProbability)
            });

            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "test1", 1.0, 2.0);
            assessmentSection.SetHydraulicBoundaryLocationCalculations(new[]
            {
                hydraulicBoundaryLocation
            });

            using (var mapLayer = new HydraulicBoundaryLocationsMapLayer(assessmentSection))
            {
                string targetProbabilityString = ProbabilityFormattingHelper.Format(targetProbability);
                string selectedProbabilityString = string.Format(selectedTargetProbabilityFormat, targetProbabilityString);
                mapLayer.MapData.SelectedMetaDataAttribute = string.Format(waveHeightDisplayNameFormat, selectedProbabilityString);
                mapLayer.MapData.NotifyObservers();

                mapLayer.MapData.Attach(observer);

                // Precondition
                MapDataTestHelper.AssertHydraulicBoundaryLocationsMapData(assessmentSection, mapLayer.MapData);

                // When
                ObservableList<HydraulicBoundaryLocationCalculationsForTargetProbability> waveHeightCalculationsForUserDefinedTargetProbabilities =
                    assessmentSection.WaveHeightCalculationsForUserDefinedTargetProbabilities;
                shiftItemAction(waveHeightCalculationsForUserDefinedTargetProbabilities);
                waveHeightCalculationsForUserDefinedTargetProbabilities.NotifyObservers();

                // Then
                MapDataTestHelper.AssertHydraulicBoundaryLocationsMapData(assessmentSection, mapLayer.MapData);

                string expectedSelectedProbabilityString = string.Format(expectedSelectedTargetProbabilityFormat, targetProbabilityString);
                Assert.AreEqual(string.Format(waveHeightDisplayNameFormat, expectedSelectedProbabilityString),
                                mapLayer.MapData.SelectedMetaDataAttribute);
            }

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

        private static IEnumerable<TestCaseData> GetTargetProbabilitiesFuncsWithDisplayNameFormat()
        {
            yield return new TestCaseData(new Func<IAssessmentSection, ObservableList<HydraulicBoundaryLocationCalculationsForTargetProbability>>(
                                              assessmentSection => assessmentSection.WaterLevelCalculationsForUserDefinedTargetProbabilities),
                                          waterLevelDisplayNameFormat);
            yield return new TestCaseData(new Func<IAssessmentSection, ObservableList<HydraulicBoundaryLocationCalculationsForTargetProbability>>(
                                              assessmentSection => assessmentSection.WaveHeightCalculationsForUserDefinedTargetProbabilities),
                                          waveHeightDisplayNameFormat);
        }

        private static IEnumerable<TestCaseData> GetTargetProbabilityItemShiftActions()
        {
            yield return new TestCaseData(
                    new Action<ObservableList<HydraulicBoundaryLocationCalculationsForTargetProbability>>(col =>
                    {
                        HydraulicBoundaryLocationCalculationsForTargetProbability itemToMove = col.First();
                        col.Remove(itemToMove);
                        col.Add(itemToMove);
                    }),
                    "{0}", "{0} (2)")
                .SetName("MoveItemDown");

            yield return new TestCaseData(
                    new Action<ObservableList<HydraulicBoundaryLocationCalculationsForTargetProbability>>(col =>
                    {
                        HydraulicBoundaryLocationCalculationsForTargetProbability itemToMove = col.Last();
                        col.Remove(itemToMove);
                        col.Insert(0, itemToMove);
                    }),
                    "{0} (2)", "{0}")
                .SetName("MoveItemUp");
        }
    }
}