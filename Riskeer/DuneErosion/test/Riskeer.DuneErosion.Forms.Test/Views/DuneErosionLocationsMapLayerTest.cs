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
using Core.Components.Gis.Data;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Forms.Helpers;
using Riskeer.DuneErosion.Data;
using Riskeer.DuneErosion.Data.TestUtil;
using Riskeer.DuneErosion.Forms.TestUtil;
using Riskeer.DuneErosion.Forms.Views;

namespace Riskeer.DuneErosion.Forms.Test.Views
{
    [TestFixture]
    public class DuneErosionLocationsMapLayerTest
    {
        private const string waterLevelDisplayNameFormat = "Rekenwaarde h - {0}";
        private const string waveHeightDisplayNameFormat = "Rekenwaarde Hs - {0}";
        private const string wavePeriodDisplayNameFormat = "Rekenwaarde Tp - {0}";

        [Test]
        public void Constructor_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new DuneErosionLocationsMapLayer(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            var failureMechanism = new DuneErosionFailureMechanism();
            failureMechanism.SetDuneLocations(new[]
            {
                new TestDuneLocation("test")
            });

            // Call
            using (var mapLayer = new DuneErosionLocationsMapLayer(failureMechanism))
            {
                // Assert
                Assert.IsInstanceOf<IDisposable>(mapLayer);
                AssertDuneLocationsMapData(failureMechanism, mapLayer.MapData);
            }
        }

        [Test]
        public void GivenMapLayerWithDuneLocations_WhenChangingDuneLocationsDataAndObserversNotified_ThenMapDataUpdated()
        {
            // Given
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var failureMechanism = new DuneErosionFailureMechanism();
            failureMechanism.SetDuneLocations(new[]
            {
                new TestDuneLocation("test1")
            });

            using (var mapLayer = new DuneErosionLocationsMapLayer(failureMechanism))
            {
                mapLayer.MapData.Attach(observer);

                // Precondition
                AssertDuneLocationsMapData(failureMechanism, mapLayer.MapData);

                // When
                failureMechanism.SetDuneLocations(new[]
                {
                    new TestDuneLocation("test2")
                });
                failureMechanism.DuneLocations.NotifyObservers();

                // Then
                AssertDuneLocationsMapData(failureMechanism, mapLayer.MapData);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void GivenMapLayerWithDuneLocations_WhenUserDefinedTargetProbabilitiesCollectionUpdatedAndNotified_ThenMapDataUpdated()
        {
            // Given
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var failureMechanism = new DuneErosionFailureMechanism();
            failureMechanism.SetDuneLocations(new[]
            {
                new TestDuneLocation("test1")
            });

            using (var mapLayer = new DuneErosionLocationsMapLayer(failureMechanism))
            {
                mapLayer.MapData.Attach(observer);

                // Precondition
                AssertDuneLocationsMapData(failureMechanism, mapLayer.MapData);

                // When
                var newTargetProbability = new DuneLocationCalculationsForTargetProbability(0.1);
                newTargetProbability.DuneLocationCalculations.AddRange(failureMechanism.DuneLocations
                                                                                       .Select(l => new DuneLocationCalculation(l)));
                failureMechanism.DuneLocationCalculationsForUserDefinedTargetProbabilities.Add(newTargetProbability);
                failureMechanism.DuneLocationCalculationsForUserDefinedTargetProbabilities.NotifyObservers();

                // Then
                AssertDuneLocationsMapData(failureMechanism, mapLayer.MapData);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void GivenMapLayerWithDuneLocations_WhenCalculationsForUserDefinedTargetProbabilityUpdatedAndNotified_ThenMapDataUpdated()
        {
            // Given
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var targetProbability = new DuneLocationCalculationsForTargetProbability(0.1);
            var failureMechanism = new DuneErosionFailureMechanism();
            failureMechanism.DuneLocationCalculationsForUserDefinedTargetProbabilities.Add(targetProbability);
            failureMechanism.SetDuneLocations(new[]
            {
                new TestDuneLocation("test1")
            });

            using (var mapLayer = new DuneErosionLocationsMapLayer(failureMechanism))
            {
                mapLayer.MapData.Attach(observer);

                // Precondition
                AssertDuneLocationsMapData(failureMechanism, mapLayer.MapData);

                // When
                DuneLocationCalculation duneLocationCalculation = targetProbability.DuneLocationCalculations.First();
                duneLocationCalculation.Output = new TestDuneLocationCalculationOutput();
                duneLocationCalculation.NotifyObservers();

                // Then
                AssertDuneLocationsMapData(failureMechanism, mapLayer.MapData);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void GivenMapLayerWithDuneLocations_WhenCalculationsForAddedUserDefinedTargetProbabilityUpdatedAndNotified_ThenMapDataUpdated()
        {
            // Given
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var failureMechanism = new DuneErosionFailureMechanism();
            failureMechanism.SetDuneLocations(new[]
            {
                new TestDuneLocation("test1")
            });

            using (var mapLayer = new DuneErosionLocationsMapLayer(failureMechanism))
            {
                var targetProbability = new DuneLocationCalculationsForTargetProbability(0.1);
                targetProbability.DuneLocationCalculations.AddRange(failureMechanism.DuneLocations.Select(l => new DuneLocationCalculation(l)));
                failureMechanism.DuneLocationCalculationsForUserDefinedTargetProbabilities.Add(targetProbability);
                failureMechanism.DuneLocationCalculationsForUserDefinedTargetProbabilities.NotifyObservers();

                mapLayer.MapData.Attach(observer);

                // Precondition
                AssertDuneLocationsMapData(failureMechanism, mapLayer.MapData);

                // When
                DuneLocationCalculation duneLocationCalculation = targetProbability.DuneLocationCalculations.First();
                duneLocationCalculation.Output = new TestDuneLocationCalculationOutput();
                duneLocationCalculation.NotifyObservers();

                // Then
                AssertDuneLocationsMapData(failureMechanism, mapLayer.MapData);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void GivenMapLayerWithDuneLocations_WhenCalculationsForRemovedUserDefinedTargetProbabilityUpdatedAndNotified_ThenMapDataUpdated()
        {
            // Given
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            mocks.ReplayAll();

            var targetProbability = new DuneLocationCalculationsForTargetProbability(0.1);
            var failureMechanism = new DuneErosionFailureMechanism();
            failureMechanism.DuneLocationCalculationsForUserDefinedTargetProbabilities.Add(targetProbability);
            failureMechanism.SetDuneLocations(new[]
            {
                new TestDuneLocation("test1")
            });

            using (var mapLayer = new DuneErosionLocationsMapLayer(failureMechanism))
            {
                failureMechanism.DuneLocationCalculationsForUserDefinedTargetProbabilities.Clear();
                failureMechanism.DuneLocationCalculationsForUserDefinedTargetProbabilities.NotifyObservers();

                mapLayer.MapData.Attach(observer);

                // Precondition
                AssertDuneLocationsMapData(failureMechanism, mapLayer.MapData);

                // When
                DuneLocationCalculation duneLocationCalculation = targetProbability.DuneLocationCalculations.First();
                duneLocationCalculation.Output = new TestDuneLocationCalculationOutput();
                duneLocationCalculation.NotifyObservers();

                // Then
                AssertDuneLocationsMapData(failureMechanism, mapLayer.MapData);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void GivenMapLayerWithDuneLocations_WhenUserDefinedTargetProbabilityUpdatedAndNotified_ThenMapDataUpdated()
        {
            // Given
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var failureMechanism = new DuneErosionFailureMechanism();
            failureMechanism.DuneLocationCalculationsForUserDefinedTargetProbabilities.Add(
                new DuneLocationCalculationsForTargetProbability(0.1));
            failureMechanism.SetDuneLocations(new[]
            {
                new TestDuneLocation("test1")
            });

            using (var mapLayer = new DuneErosionLocationsMapLayer(failureMechanism))
            {
                mapLayer.MapData.Attach(observer);

                // Precondition
                AssertDuneLocationsMapData(failureMechanism, mapLayer.MapData);

                // When
                DuneLocationCalculationsForTargetProbability targetProbability = failureMechanism.DuneLocationCalculationsForUserDefinedTargetProbabilities.First();
                targetProbability.TargetProbability = 0.003;
                targetProbability.NotifyObservers();

                // Then
                AssertDuneLocationsMapData(failureMechanism, mapLayer.MapData);
            }

            mocks.VerifyAll();
        }

        [Test]
        [TestCaseSource(nameof(GetDisplayNameFormatTestCases))]
        public void GivenMapLayerWithDuneLocations_WhenSelectedTargetProbabilityRemovedAndNotified_ThenMapDataUpdatedSelectedMetaDataAttributeResetToDefault(string displayName)
        {
            // Given
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var failureMechanism = new DuneErosionFailureMechanism();
            failureMechanism.DuneLocationCalculationsForUserDefinedTargetProbabilities.Add(
                new DuneLocationCalculationsForTargetProbability(0.1));
            failureMechanism.SetDuneLocations(new[]
            {
                new TestDuneLocation("test1")
            });

            using (var mapLayer = new DuneErosionLocationsMapLayer(failureMechanism))
            {
                DuneLocationCalculationsForTargetProbability calculationsForTargetProbability = failureMechanism.DuneLocationCalculationsForUserDefinedTargetProbabilities.First();
                mapLayer.MapData.SelectedMetaDataAttribute = string.Format(displayName, ProbabilityFormattingHelper.Format(calculationsForTargetProbability.TargetProbability));
                mapLayer.MapData.NotifyObservers();

                mapLayer.MapData.Attach(observer);

                // Precondition
                AssertDuneLocationsMapData(failureMechanism, mapLayer.MapData);

                // When
                failureMechanism.DuneLocationCalculationsForUserDefinedTargetProbabilities.Remove(calculationsForTargetProbability);
                failureMechanism.DuneLocationCalculationsForUserDefinedTargetProbabilities.NotifyObservers();

                // Then
                AssertDuneLocationsMapData(failureMechanism, mapLayer.MapData);
                Assert.AreEqual("Naam", mapLayer.MapData.SelectedMetaDataAttribute);
            }

            mocks.VerifyAll();
        }

        [Test]
        [TestCaseSource(nameof(GetDisplayNameFormatTestCases))]
        public void GivenMapLayerWithDuneLocations_WhenNotSelectedTargetProbabilityRemovedAndNotified_ThenMapDataUpdated(string displayName)
        {
            // Given
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var failureMechanism = new DuneErosionFailureMechanism();
            failureMechanism.DuneLocationCalculationsForUserDefinedTargetProbabilities.Add(
                new DuneLocationCalculationsForTargetProbability(0.1));
            failureMechanism.DuneLocationCalculationsForUserDefinedTargetProbabilities.Add(
                new DuneLocationCalculationsForTargetProbability(0.001));
            failureMechanism.SetDuneLocations(new[]
            {
                new TestDuneLocation("test1")
            });

            using (var mapLayer = new DuneErosionLocationsMapLayer(failureMechanism))
            {
                DuneLocationCalculationsForTargetProbability calculationsForTargetProbabilityToRemove = failureMechanism.DuneLocationCalculationsForUserDefinedTargetProbabilities.First();
                DuneLocationCalculationsForTargetProbability calculationsForTargetProbability = failureMechanism.DuneLocationCalculationsForUserDefinedTargetProbabilities.Last();
                mapLayer.MapData.SelectedMetaDataAttribute = string.Format(displayName, ProbabilityFormattingHelper.Format(calculationsForTargetProbability.TargetProbability));
                mapLayer.MapData.NotifyObservers();

                mapLayer.MapData.Attach(observer);

                // Precondition
                AssertDuneLocationsMapData(failureMechanism, mapLayer.MapData);

                // When
                failureMechanism.DuneLocationCalculationsForUserDefinedTargetProbabilities.Remove(calculationsForTargetProbabilityToRemove);
                failureMechanism.DuneLocationCalculationsForUserDefinedTargetProbabilities.NotifyObservers();

                // Then
                AssertDuneLocationsMapData(failureMechanism, mapLayer.MapData);
                Assert.AreEqual(string.Format(displayName, ProbabilityFormattingHelper.Format(calculationsForTargetProbability.TargetProbability)),
                                mapLayer.MapData.SelectedMetaDataAttribute);
            }

            mocks.VerifyAll();
        }

        [Test]
        [TestCaseSource(nameof(GetDisplayNameFormatTestCases))]
        public void GivenMapLayerWithDuneLocations_WhenSelectedTargetProbabilityChangedAndNotified_ThenMapDataAndSelectedMetaDataAttributeUpdated(string displayName)
        {
            // Given
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var failureMechanism = new DuneErosionFailureMechanism();
            failureMechanism.DuneLocationCalculationsForUserDefinedTargetProbabilities.Add(
                new DuneLocationCalculationsForTargetProbability(0.1));
            failureMechanism.SetDuneLocations(new[]
            {
                new TestDuneLocation("test1")
            });

            using (var mapLayer = new DuneErosionLocationsMapLayer(failureMechanism))
            {
                DuneLocationCalculationsForTargetProbability calculationsForTargetProbability = failureMechanism.DuneLocationCalculationsForUserDefinedTargetProbabilities.Last();
                mapLayer.MapData.SelectedMetaDataAttribute = string.Format(displayName, ProbabilityFormattingHelper.Format(calculationsForTargetProbability.TargetProbability));
                mapLayer.MapData.NotifyObservers();

                mapLayer.MapData.Attach(observer);

                // Precondition
                AssertDuneLocationsMapData(failureMechanism, mapLayer.MapData);

                // When
                calculationsForTargetProbability.TargetProbability = 0.005;
                calculationsForTargetProbability.NotifyObservers();

                // Then
                AssertDuneLocationsMapData(failureMechanism, mapLayer.MapData);
                Assert.AreEqual(string.Format(displayName, ProbabilityFormattingHelper.Format(calculationsForTargetProbability.TargetProbability)),
                                mapLayer.MapData.SelectedMetaDataAttribute);
            }

            mocks.VerifyAll();
        }

        [Test]
        [TestCaseSource(nameof(GetTargetProbabilityItemShiftActionTestCases))]
        public void GivenMapLayerWithDuneLocations_WhenSelectedTargetProbabilityIndexUpdatedAndCollectionNotified_ThenMapDataAndSelectedMetaDataAttributeUpdated(
            Action<ObservableList<DuneLocationCalculationsForTargetProbability>> shiftItemAction,
            string selectedMetaDataAttributeFormat, string expectedSelectedMetadataAttributeFormat)
        {
            // Given
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            const double targetProbability = 0.1;
            var failureMechanism = new DuneErosionFailureMechanism
            {
                DuneLocationCalculationsForUserDefinedTargetProbabilities =
                {
                    new DuneLocationCalculationsForTargetProbability(targetProbability),
                    new DuneLocationCalculationsForTargetProbability(targetProbability),
                    new DuneLocationCalculationsForTargetProbability(targetProbability)
                }
            };
            failureMechanism.SetDuneLocations(new[]
            {
                new TestDuneLocation("test1")
            });

            using (var mapLayer = new DuneErosionLocationsMapLayer(failureMechanism))
            {
                string targetProbabilityString = ProbabilityFormattingHelper.Format(targetProbability);
                string selectedProbabilityAttribute = string.Format(selectedMetaDataAttributeFormat, targetProbabilityString);
                mapLayer.MapData.SelectedMetaDataAttribute = selectedProbabilityAttribute;
                mapLayer.MapData.NotifyObservers();

                mapLayer.MapData.Attach(observer);

                // Precondition
                AssertDuneLocationsMapData(failureMechanism, mapLayer.MapData);

                // When
                ObservableList<DuneLocationCalculationsForTargetProbability> duneLocationCalculationsForUserDefinedTargetProbabilities =
                    failureMechanism.DuneLocationCalculationsForUserDefinedTargetProbabilities;
                shiftItemAction(duneLocationCalculationsForUserDefinedTargetProbabilities);
                duneLocationCalculationsForUserDefinedTargetProbabilities.NotifyObservers();

                // Then
                AssertDuneLocationsMapData(failureMechanism, mapLayer.MapData);
                Assert.AreEqual(string.Format(expectedSelectedMetadataAttributeFormat, targetProbabilityString),
                                mapLayer.MapData.SelectedMetaDataAttribute);
            }

            mocks.VerifyAll();
        }

        private static void AssertDuneLocationsMapData(DuneErosionFailureMechanism failureMechanism,
                                                       MapData mapData)
        {
            Assert.IsInstanceOf<MapPointData>(mapData);
            Assert.AreEqual("Hydraulische belastingen", mapData.Name);

            var duneLocationsMapData = (MapPointData) mapData;
            DuneErosionMapFeaturesTestHelper.AssertDuneLocationFeaturesData(failureMechanism, duneLocationsMapData.Features);
        }

        private static IEnumerable<TestCaseData> GetDisplayNameFormatTestCases()
        {
            return GetOutputDisplayNameFormats().Select(displayNameFormat => new TestCaseData(displayNameFormat));
        }

        private static IEnumerable<TestCaseData> GetTargetProbabilityItemShiftActionTestCases()
        {
            foreach (string displayNameFormat in GetOutputDisplayNameFormats())
            {
                yield return new TestCaseData(
                        new Action<ObservableList<DuneLocationCalculationsForTargetProbability>>(col =>
                        {
                            DuneLocationCalculationsForTargetProbability itemToMove = col.First();
                            col.Remove(itemToMove);
                            col.Add(itemToMove);
                        }),
                        $"{displayNameFormat}", $"{displayNameFormat} (2)")
                    .SetName(string.Format(displayNameFormat, "MoveItemDown"));

                yield return new TestCaseData(
                        new Action<ObservableList<DuneLocationCalculationsForTargetProbability>>(col =>
                        {
                            DuneLocationCalculationsForTargetProbability itemToMove = col.Last();
                            col.Remove(itemToMove);
                            col.Insert(0, itemToMove);
                        }),
                        $"{displayNameFormat} (2)", $"{displayNameFormat}")
                    .SetName(string.Format(displayNameFormat, "MoveItemUp"));
            }
        }

        private static IEnumerable<string> GetOutputDisplayNameFormats()
        {
            yield return waterLevelDisplayNameFormat;
            yield return waveHeightDisplayNameFormat;
            yield return wavePeriodDisplayNameFormat;
        }
    }
}