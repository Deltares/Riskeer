// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base.Geometry;
using Core.Components.Gis.Data;
using Core.Components.Gis.Forms;
using NUnit.Framework;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.TestUtil;
using Ringtoets.Common.Forms.Views;
using Ringtoets.Integration.Forms.Views;

namespace Ringtoets.Integration.Forms.Test.Views
{
    [TestFixture]
    public class AssessmentSectionViewTest
    {
        private const int referenceLineIndex = 0;
        private const int hydraulicBoundaryLocationsIndex = 1;

        [Test]
        public void DefaultConstructor_DefaultValues()
        {
            // Call
            using (var view = new AssessmentSectionView())
            {
                // Assert
                Assert.IsInstanceOf<UserControl>(view);
                Assert.IsInstanceOf<IMapView>(view);
                Assert.IsNotNull(view.Map);
                Assert.IsNull(view.Data);
            }
        }

        [Test]
        public void DefaultConstructor_Always_AddEmptyMapControl()
        {
            // Call
            using (var view = new AssessmentSectionView())
            {
                // Assert
                Assert.AreEqual(1, view.Controls.Count);
                Assert.IsInstanceOf<RingtoetsMapControl>(view.Controls[0]);
                Assert.AreSame(view.Map, ((RingtoetsMapControl) view.Controls[0]).MapControl);
                Assert.AreEqual(DockStyle.Fill, ((Control) view.Map).Dock);
                Assert.IsNull(view.Map.Data);
            }
        }

        [Test]
        public void Data_AssessmentSection_DataSet()
        {
            // Setup
            using (var view = new AssessmentSectionView())
            {
                var assessmentSection = new ObservableTestAssessmentSectionStub();

                // Call
                view.Data = assessmentSection;

                // Assert
                Assert.AreSame(assessmentSection, view.Data);
            }
        }

        [Test]
        public void Data_OtherThanAssessmentSection_DataNull()
        {
            // Setup
            using (var view = new AssessmentSectionView())
            {
                var data = new object();

                // Call
                view.Data = data;

                // Assert
                Assert.IsNull(view.Data);
            }
        }

        [Test]
        public void Data_AssessmentSectionWithBackgroundData_BackgroundDataSet()
        {
            // Setup
            IAssessmentSection assessmentSection = new ObservableTestAssessmentSectionStub();

            using (var view = new AssessmentSectionView())
            {
                // Call
                view.Data = assessmentSection;

                // Assert
                MapDataTestHelper.AssertImageBasedMapData(assessmentSection.BackgroundData, view.Map.BackgroundMapData);
            }
        }

        [Test]
        public void Data_SetToNull_MapDataCleared()
        {
            // Setup
            var assessmentSection = new ObservableTestAssessmentSectionStub();
            using (var view = new AssessmentSectionView
            {
                Data = assessmentSection
            })
            {
                // Precondition
                Assert.AreEqual(2, view.Map.Data.Collection.Count());
                MapDataTestHelper.AssertImageBasedMapData(assessmentSection.BackgroundData, view.Map.BackgroundMapData);

                // Call
                view.Data = null;

                // Assert
                Assert.IsNull(view.Data);
                Assert.IsNull(view.Map.Data);
                Assert.IsNull(view.Map.BackgroundMapData);
            }
        }

        [Test]
        public void Data_EmptyAssessmentSection_DataUpdatedToCollectionOfEmptyMapData()
        {
            // Setup
            using (var view = new AssessmentSectionView())
            {
                var assessmentSection = new ObservableTestAssessmentSectionStub();

                // Call
                view.Data = assessmentSection;

                // Assert
                Assert.AreSame(assessmentSection, view.Data);
                AssertEmptyMapData(view.Map.Data);
                MapDataTestHelper.AssertImageBasedMapData(assessmentSection.BackgroundData, view.Map.BackgroundMapData);
            }
        }

        [Test]
        public void Data_WithReferenceLineAndHydraulicBoundaryDatabase_DataUpdatedToCollectionOfFilledMapData()
        {
            // Setup
            using (var view = new AssessmentSectionView())
            {
                var referenceLine = new ReferenceLine();
                referenceLine.SetGeometry(new[]
                {
                    new Point2D(1.0, 2.0),
                    new Point2D(2.0, 1.0)
                });

                var assessmentSection = new ObservableTestAssessmentSectionStub
                {
                    HydraulicBoundaryDatabase =
                    {
                        Locations =
                        {
                            new HydraulicBoundaryLocation(1, "test", 1.0, 2.0)
                        }
                    },
                    ReferenceLine = referenceLine
                };

                // Call
                view.Data = assessmentSection;

                // Assert
                Assert.AreSame(assessmentSection, view.Data);
                Assert.IsInstanceOf<MapDataCollection>(view.Map.Data);
                MapDataCollection mapData = view.Map.Data;
                Assert.IsNotNull(mapData);

                MapData hydraulicBoundaryLocationsMapData = mapData.Collection.ElementAt(hydraulicBoundaryLocationsIndex);
                AssertHydraulicBoundarLocationsMapData(assessmentSection.HydraulicBoundaryDatabase.Locations,
                                                       hydraulicBoundaryLocationsMapData);

                MapData referenceLineMapData = mapData.Collection.ElementAt(referenceLineIndex);
                AssertReferenceLineMapData(referenceLine, referenceLineMapData);
            }
        }

        [Test]
        public void GivenViewWithHydraulicBoundaryLocationsData_WhenLocationUpdatedAndNotified_ThenMapDataUpdated()
        {
            // Given
            var random = new Random(21);
            using (var view = new AssessmentSectionView())
            {
                IMapControl map = ((RingtoetsMapControl) view.Controls[0]).MapControl;

                var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "test1", 1.0, 2.0);
                var assessmentSection = new ObservableTestAssessmentSectionStub
                {
                    HydraulicBoundaryDatabase =
                    {
                        Locations =
                        {
                            hydraulicBoundaryLocation
                        }
                    }
                };

                view.Data = assessmentSection;

                MapData hydraulicBoundaryLocationsMapData = map.Data.Collection.ElementAt(hydraulicBoundaryLocationsIndex);

                // Precondition
                MapDataTestHelper.AssertHydraulicBoundaryLocationOutputsMapData(assessmentSection.HydraulicBoundaryDatabase.Locations,
                                                                                hydraulicBoundaryLocationsMapData);

                // When
                hydraulicBoundaryLocation.DesignWaterLevelCalculation1.Output = new TestHydraulicBoundaryLocationOutput(random.NextDouble());
                hydraulicBoundaryLocation.DesignWaterLevelCalculation2.Output = new TestHydraulicBoundaryLocationOutput(random.NextDouble());
                hydraulicBoundaryLocation.DesignWaterLevelCalculation3.Output = new TestHydraulicBoundaryLocationOutput(random.NextDouble());
                hydraulicBoundaryLocation.DesignWaterLevelCalculation4.Output = new TestHydraulicBoundaryLocationOutput(random.NextDouble());
                hydraulicBoundaryLocation.WaveHeightCalculation1.Output = new TestHydraulicBoundaryLocationOutput(random.NextDouble());
                hydraulicBoundaryLocation.WaveHeightCalculation2.Output = new TestHydraulicBoundaryLocationOutput(random.NextDouble());
                hydraulicBoundaryLocation.WaveHeightCalculation3.Output = new TestHydraulicBoundaryLocationOutput(random.NextDouble());
                hydraulicBoundaryLocation.WaveHeightCalculation4.Output = new TestHydraulicBoundaryLocationOutput(random.NextDouble());
                hydraulicBoundaryLocation.NotifyObservers();

                // Then
                MapDataTestHelper.AssertHydraulicBoundaryLocationOutputsMapData(assessmentSection.HydraulicBoundaryDatabase.Locations,
                                                                                hydraulicBoundaryLocationsMapData);
            }
        }

        [Test]
        public void GivenViewWithHydraulicBoundaryLocationsDatabase_WhenChangingHydraulicBoundaryLocationsDataAndObserversNotified_MapDataUpdated()
        {
            // Given
            using (var view = new AssessmentSectionView())
            {
                var assessmentSection = new ObservableTestAssessmentSectionStub
                {
                    HydraulicBoundaryDatabase =
                    {
                        Locations =
                        {
                            new HydraulicBoundaryLocation(1, "test", 1.0, 2.0)
                        }
                    }
                };

                view.Data = assessmentSection;
                MapDataCollection mapData = view.Map.Data;
                MapData hydraulicBoundaryLocationsMapData = mapData.Collection.ElementAt(hydraulicBoundaryLocationsIndex);

                // Precondition
                AssertHydraulicBoundarLocationsMapData(assessmentSection.HydraulicBoundaryDatabase.Locations,
                                                       hydraulicBoundaryLocationsMapData);

                // When
                assessmentSection.HydraulicBoundaryDatabase.Locations.Add(new HydraulicBoundaryLocation(2, "new 2", 2, 3));
                assessmentSection.HydraulicBoundaryDatabase.Locations.NotifyObservers();

                // Then
                AssertHydraulicBoundarLocationsMapData(assessmentSection.HydraulicBoundaryDatabase.Locations,
                                                       hydraulicBoundaryLocationsMapData);
            }
        }

        [Test]
        public void UpdateObserver_ReferenceLineUpdated_MapDataUpdated()
        {
            // Setup
            using (var view = new AssessmentSectionView())
            {
                var points = new List<Point2D>
                {
                    new Point2D(1.0, 2.0),
                    new Point2D(2.0, 1.0)
                };

                var pointsUpdate = new List<Point2D>
                {
                    new Point2D(2.0, 5.0),
                    new Point2D(4.0, 3.0)
                };

                var assessmentSection = new ObservableTestAssessmentSectionStub
                {
                    ReferenceLine = new ReferenceLine()
                };
                assessmentSection.ReferenceLine.SetGeometry(points);

                view.Data = assessmentSection;
                MapDataCollection mapData = view.Map.Data;

                MapData referenceLineMapData = mapData.Collection.ElementAt(referenceLineIndex);

                // Precondition
                AssertReferenceLineMapData(assessmentSection.ReferenceLine, referenceLineMapData);

                // Call
                assessmentSection.ReferenceLine.SetGeometry(pointsUpdate);
                assessmentSection.NotifyObservers();

                // Assert
                AssertReferenceLineMapData(assessmentSection.ReferenceLine, referenceLineMapData);
            }
        }

        [Test]
        public void UpdateObserver_OtherAssessmentSectionUpdated_MapDataNotUpdated()
        {
            // Setup
            using (var view = new AssessmentSectionView())
            {
                var assessmentSection = new ObservableTestAssessmentSectionStub();

                view.Data = assessmentSection;

                var assessmentSection2 = new ObservableTestAssessmentSectionStub
                {
                    HydraulicBoundaryDatabase =
                    {
                        Locations =
                        {
                            new HydraulicBoundaryLocation(2, "test2", 2.0, 3.0)
                        }
                    },
                    ReferenceLine = new ReferenceLine()
                };

                assessmentSection2.ReferenceLine.SetGeometry(new List<Point2D>
                {
                    new Point2D(2.0, 1.0),
                    new Point2D(4.0, 3.0)
                });

                // Call
                assessmentSection2.NotifyObservers();

                // Assert
                Assert.AreEqual(assessmentSection, view.Data);
                AssertEmptyMapData(view.Map.Data);
            }
        }

        [Test]
        public void UpdateObserver_DataUpdated_MapLayersSameOrder()
        {
            // Setup
            using (var view = new AssessmentSectionView())
            {
                var referenceLine = new ReferenceLine();
                referenceLine.SetGeometry(new[]
                {
                    new Point2D(1.0, 2.0),
                    new Point2D(2.0, 1.0)
                });

                var assessmentSection = new ObservableTestAssessmentSectionStub
                {
                    HydraulicBoundaryDatabase =
                    {
                        Locations =
                        {
                            new HydraulicBoundaryLocation(1, "test", 1.0, 2.0)
                        }
                    },
                    ReferenceLine = referenceLine
                };

                view.Data = assessmentSection;

                MapDataCollection mapData = view.Map.Data;

                MapData dataToMove = mapData.Collection.ElementAt(0);
                mapData.Remove(dataToMove);
                mapData.Add(dataToMove);

                // Precondition
                var referenceLineMapData = (MapLineData) mapData.Collection.ElementAt(referenceLineIndex + 1);
                Assert.AreEqual("Referentielijn", referenceLineMapData.Name);

                var hrLocationsMapData = (MapPointData) mapData.Collection.ElementAt(hydraulicBoundaryLocationsIndex - 1);
                Assert.AreEqual("Hydraulische randvoorwaarden", hrLocationsMapData.Name);

                assessmentSection.HydraulicBoundaryDatabase.Locations.Clear();
                assessmentSection.HydraulicBoundaryDatabase.Locations.Add(new HydraulicBoundaryLocation(2, "test2", 2.0, 3.0));

                // Call
                assessmentSection.NotifyObservers();

                // Assert
                var actualReferenceLineMapData = (MapLineData) mapData.Collection.ElementAt(referenceLineIndex + 1);
                Assert.AreEqual("Referentielijn", actualReferenceLineMapData.Name);

                var actualHrLocationsMapData = (MapPointData) mapData.Collection.ElementAt(hydraulicBoundaryLocationsIndex - 1);
                Assert.AreEqual("Hydraulische randvoorwaarden", actualHrLocationsMapData.Name);
            }
        }

        private static void AssertReferenceLineMapData(ReferenceLine referenceLine, MapData referenceLineMapData)
        {
            MapDataTestHelper.AssertReferenceLineMapData(referenceLine, referenceLineMapData);
            Assert.IsTrue(referenceLineMapData.IsVisible);
        }

        private static void AssertHydraulicBoundarLocationsMapData(IEnumerable<HydraulicBoundaryLocation> locations, MapData hydraulicBoundaryLocationsMapData)
        {
            MapDataTestHelper.AssertHydraulicBoundaryLocationsMapData(locations, hydraulicBoundaryLocationsMapData);
            Assert.IsTrue(hydraulicBoundaryLocationsMapData.IsVisible);
        }

        private static void AssertEmptyMapData(MapDataCollection mapDataCollection)
        {
            Assert.AreEqual("Trajectkaart", mapDataCollection.Name);

            List<MapData> mapDataList = mapDataCollection.Collection.ToList();

            Assert.AreEqual(2, mapDataList.Count);

            var referenceLineMapData = (MapLineData) mapDataList[referenceLineIndex];
            var hydraulicBoundaryLocationsMapData = (MapPointData) mapDataList[hydraulicBoundaryLocationsIndex];

            CollectionAssert.IsEmpty(referenceLineMapData.Features);
            CollectionAssert.IsEmpty(hydraulicBoundaryLocationsMapData.Features);

            Assert.AreEqual("Referentielijn", referenceLineMapData.Name);
            Assert.AreEqual("Hydraulische randvoorwaarden", hydraulicBoundaryLocationsMapData.Name);
        }
    }
}