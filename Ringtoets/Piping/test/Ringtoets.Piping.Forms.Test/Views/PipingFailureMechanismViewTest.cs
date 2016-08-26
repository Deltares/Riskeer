﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Base.Geometry;
using Core.Components.DotSpatial.Forms;
using Core.Components.Gis.Data;
using Core.Components.Gis.Forms;
using NUnit.Framework;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Contribution;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.HydraRing.Data;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.PresentationObjects;
using Ringtoets.Piping.Forms.Views;
using Ringtoets.Piping.Primitives;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;
using RingtoetsCommonDataResources = Ringtoets.Common.Data.Properties.Resources;
using PipingDataResources = Ringtoets.Piping.Data.Properties.Resources;
using PipingFormsResources = Ringtoets.Piping.Forms.Properties.Resources;

namespace Ringtoets.Piping.Forms.Test.Views
{
    [TestFixture]
    public class PipingFailureMechanismViewTest
    {
        private const int referenceLineIndex = 0;
        private const int sectionsIndex = 1;
        private const int stochasticSoilModelsIndex = 2;
        private const int surfaceLinesIndex = 3;
        private const int sectionsStartPointIndex = 4;
        private const int sectionsEndPointIndex = 5;
        private const int hydraulicBoundaryDatabaseIndex = 6;

        [Test]
        public void DefaultConstructor_DefaultValues()
        {
            // Call
            using (var view = new PipingFailureMechanismView())
            {
                // Assert
                Assert.IsInstanceOf<UserControl>(view);
                Assert.IsInstanceOf<IMapView>(view);
                Assert.IsNotNull(view.Map);
                Assert.IsNull(view.Data);
            }
        }

        [Test]
        public void DefaultConstructor_Always_AddMapControlWithCollectionOfEmptyMapData()
        {
            // Call
            using (var view = new PipingFailureMechanismView())
            {
                // Assert
                Assert.AreEqual(1, view.Controls.Count);
                Assert.AreSame(view.Map, view.Controls[0]);
                Assert.AreEqual(DockStyle.Fill, ((Control) view.Map).Dock);
                Assert.AreEqual(PipingDataResources.PipingFailureMechanism_DisplayName, view.Map.Data.Name);
                AssertEmptyMapData(view.Map.Data);
            }
        }

        [Test]
        public void Data_PipingFailureMechanismContext_DataSet()
        {
            // Setup
            using (var view = new PipingFailureMechanismView())
            {
                var pipingFailureMechanismContext = new PipingFailureMechanismContext(new PipingFailureMechanism(), new TestAssessmentSection());

                // Call
                view.Data = pipingFailureMechanismContext;

                // Assert
                Assert.AreSame(pipingFailureMechanismContext, view.Data);
            }
        }

        [Test]
        public void Data_OtherThanPipingFailureMechanismContext_DataNull()
        {
            // Setup
            using (var view = new PipingFailureMechanismView())
            {
                var data = new object();

                // Call
                view.Data = data;

                // Assert
                Assert.IsNull(view.Data);
            }
        }

        [Test]
        public void Data_SetToNull_MapDataCleared()
        {
            // Setup
            using (var view = new PipingFailureMechanismView())
            {
                var pipingFailureMechanismContext = new PipingFailureMechanismContext(new PipingFailureMechanism(), new TestAssessmentSection());

                view.Data = pipingFailureMechanismContext;

                // Precondition
                Assert.AreEqual(7, view.Map.Data.Collection.Count());

                // Call
                view.Data = null;

                // Assert
                Assert.IsNull(view.Data);
                Assert.IsNull(view.Map.Data);
            }
        }

        [Test]
        public void Data_EmptyPipingFailureMechanismContext_NoMapDataSet()
        {
            // Setup
            using (var view = new PipingFailureMechanismView())
            {
                var pipingFailureMechanismContext = new PipingFailureMechanismContext(new PipingFailureMechanism(), new TestAssessmentSection());

                // Call
                view.Data = pipingFailureMechanismContext;

                // Assert
                Assert.AreSame(pipingFailureMechanismContext, view.Data);
                AssertEmptyMapData(view.Map.Data);
            }
        }

        [Test]
        public void Data_PipingFailureMechanismContext_DataUpdatedToCollectionOfFilledMapData()
        {
            // Setup
            using (var view = new PipingFailureMechanismView())
            {
                var map = (MapControl) view.Controls[0];

                var geometryPoints = new[]
                {
                    new Point2D(0.0, 0.0),
                    new Point2D(2.0, 0.0),
                    new Point2D(4.0, 4.0),
                    new Point2D(6.0, 4.0)
                };

                var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();
                hydraulicBoundaryDatabase.Locations.Add(new HydraulicBoundaryLocation(1, "test", 1.0, 2.0));

                var referenceLine = new ReferenceLine();
                referenceLine.SetGeometry(new[]
                {
                    new Point2D(1.0, 2.0),
                    new Point2D(2.0, 1.0)
                });

                var assessmentSection = new TestAssessmentSection
                {
                    HydraulicBoundaryDatabase = hydraulicBoundaryDatabase,
                    ReferenceLine = referenceLine
                };

                var stochasticSoilModel = new StochasticSoilModel(0, "", "");
                stochasticSoilModel.Geometry.AddRange(new[]
                {
                    new Point2D(1.0, 2.0),
                    new Point2D(1.1, 2.2)
                });

                var pipingFailureMechanism = new PipingFailureMechanism();
                pipingFailureMechanism.SurfaceLines.Add(new RingtoetsPipingSurfaceLine());
                pipingFailureMechanism.AddSection(new FailureMechanismSection("A", geometryPoints.Take(2)));
                pipingFailureMechanism.AddSection(new FailureMechanismSection("B", geometryPoints.Skip(1).Take(2)));
                pipingFailureMechanism.AddSection(new FailureMechanismSection("C", geometryPoints.Skip(2).Take(2)));
                pipingFailureMechanism.StochasticSoilModels.Add(stochasticSoilModel);

                var pipingContext = new PipingFailureMechanismContext(pipingFailureMechanism, assessmentSection);

                // Call
                view.Data = pipingContext;

                // Assert
                Assert.AreSame(pipingContext, view.Data);

                var mapData = map.Data;
                Assert.IsInstanceOf<MapDataCollection>(mapData);

                var mapDataList = mapData.Collection.ToList();
                Assert.AreEqual(7, mapDataList.Count);
                AssertReferenceLineMapData(assessmentSection.ReferenceLine, mapDataList[referenceLineIndex]);
                AssertSurfacelinesMapData(pipingFailureMechanism.SurfaceLines, mapDataList[surfaceLinesIndex]);
                AssertFailureMechanismSectionsMapData(pipingFailureMechanism.Sections, mapDataList[sectionsIndex]);
                AssertFailureMechanismSectionsStartPointMapData(pipingFailureMechanism.Sections, mapDataList[sectionsStartPointIndex]);
                AssertFailureMechanismSectionsEndPointMapData(pipingFailureMechanism.Sections, mapDataList[sectionsEndPointIndex]);
                AssertHydraulicBoundaryLocationsMapData(assessmentSection.HydraulicBoundaryDatabase, mapDataList[hydraulicBoundaryDatabaseIndex]);
                AssertStochasticSoilModelsMapData(pipingFailureMechanism.StochasticSoilModels, mapDataList[stochasticSoilModelsIndex]);
            }
        }

        [Test]
        public void UpdateObserver_HydraulicBoundaryDatabaseUpdated_MapDataUpdated()
        {
            // Setup
            using (var view = new PipingFailureMechanismView())
            {
                var map = (MapControl) view.Controls[0];

                var hydraulicBoundaryDatabase1 = new HydraulicBoundaryDatabase
                {
                    Locations =
                    {
                        new HydraulicBoundaryLocation(1, "test1", 1.0, 2.0)
                    }
                };
                var hydraulicBoundaryDatabase2 = new HydraulicBoundaryDatabase
                {
                    Locations =
                    {
                        new HydraulicBoundaryLocation(2, "test2", 2.0, 3.0)
                    }
                };

                var assessmentSection = new TestAssessmentSection
                {
                    HydraulicBoundaryDatabase = hydraulicBoundaryDatabase1
                };

                var pipingContext = new PipingFailureMechanismContext(new PipingFailureMechanism(), assessmentSection);

                view.Data = pipingContext;

                var hydraulicBoundaryDatabaseMapData = map.Data.Collection.ElementAt(hydraulicBoundaryDatabaseIndex);

                // Precondition
                AssertHydraulicBoundaryLocationsMapData(hydraulicBoundaryDatabase1, hydraulicBoundaryDatabaseMapData);

                // Call
                assessmentSection.HydraulicBoundaryDatabase = hydraulicBoundaryDatabase2;
                assessmentSection.NotifyObservers();

                // Assert
                AssertHydraulicBoundaryLocationsMapData(hydraulicBoundaryDatabase2, hydraulicBoundaryDatabaseMapData);
            }
        }

        [Test]
        public void UpdateObserver_ReferenceLineUpdated_MapDataUpdated()
        {
            // Setup
            using (var view = new PipingFailureMechanismView())
            {
                var map = (MapControl) view.Controls[0];

                var points1 = new List<Point2D>
                {
                    new Point2D(1.0, 2.0),
                    new Point2D(2.0, 1.0)
                };

                var points2 = new List<Point2D>
                {
                    new Point2D(2.0, 5.0),
                    new Point2D(4.0, 3.0)
                };

                var assessmentSection = new TestAssessmentSection
                {
                    ReferenceLine = new ReferenceLine()
                };
                assessmentSection.ReferenceLine.SetGeometry(points1);

                var pipingContext = new PipingFailureMechanismContext(new PipingFailureMechanism(), assessmentSection);

                view.Data = pipingContext;

                var referenceLineMapData = map.Data.Collection.ElementAt(referenceLineIndex);

                // Precondition
                AssertReferenceLineMapData(assessmentSection.ReferenceLine, referenceLineMapData);

                // Call
                assessmentSection.ReferenceLine.SetGeometry(points2);
                assessmentSection.NotifyObservers();

                // Assert
                AssertReferenceLineMapData(assessmentSection.ReferenceLine, referenceLineMapData);
            }
        }

        [Test]
        public void UpdateObserver_SurfaceLinesUpdated_MapDataUpdated()
        {
            // Setup
            using (var view = new PipingFailureMechanismView())
            {
                var map = (MapControl) view.Controls[0];
                var pipingFailureMechanism = new PipingFailureMechanism();
                var pipingContext = new PipingFailureMechanismContext(pipingFailureMechanism, new TestAssessmentSection());
                var surfaceLine = new RingtoetsPipingSurfaceLine();

                var geometry1 = new Collection<Point3D>
                {
                    new Point3D(1, 2, 3),
                    new Point3D(4, 5, 6)
                };

                var geometry2 = new Collection<Point3D>
                {
                    new Point3D(11, 22, 33),
                    new Point3D(44, 55, 66)
                };

                surfaceLine.SetGeometry(geometry1);
                pipingFailureMechanism.SurfaceLines.Add(surfaceLine);

                view.Data = pipingContext;

                var surfaceLineMapData = (MapLineData) map.Data.Collection.ElementAt(surfaceLinesIndex);

                // Precondition
                AssertSurfacelinesMapData(pipingFailureMechanism.SurfaceLines, surfaceLineMapData);

                // Call
                surfaceLine.SetGeometry(geometry2);
                pipingFailureMechanism.NotifyObservers();

                // Assert
                AssertSurfacelinesMapData(pipingFailureMechanism.SurfaceLines, surfaceLineMapData);
            }
        }

        [Test]
        public void UpdateObserver_FailureMechanismSectionsUpdated_MapDataUpdated()
        {
            // Setup
            using (var view = new PipingFailureMechanismView())
            {
                var map = (MapControl) view.Controls[0];

                var pipingFailureMechanism = new PipingFailureMechanism();
                var pipingContext = new PipingFailureMechanismContext(pipingFailureMechanism, new TestAssessmentSection());

                view.Data = pipingContext;

                var sectionMapData = (MapLineData) map.Data.Collection.ElementAt(sectionsIndex);
                var sectionStartsMapData = (MapPointData) map.Data.Collection.ElementAt(sectionsStartPointIndex);
                var sectionsEndsMapData = (MapPointData) map.Data.Collection.ElementAt(sectionsEndPointIndex);

                // Call
                pipingFailureMechanism.AddSection(new FailureMechanismSection(string.Empty, new[]
                {
                    new Point2D(1, 2),
                    new Point2D(1, 2)
                }));
                pipingFailureMechanism.NotifyObservers();

                // Assert
                AssertFailureMechanismSectionsMapData(pipingFailureMechanism.Sections, sectionMapData);
                AssertFailureMechanismSectionsStartPointMapData(pipingFailureMechanism.Sections, sectionStartsMapData);
                AssertFailureMechanismSectionsEndPointMapData(pipingFailureMechanism.Sections, sectionsEndsMapData);
            }
        }

        [Test]
        public void UpdateObserver_StochasticSoilModelsUpdated_MapDataUpdated()
        {
            // Setup
            using (var view = new PipingFailureMechanismView())
            {
                var map = (MapControl) view.Controls[0];

                var pipingFailureMechanism = new PipingFailureMechanism();
                var pipingContext = new PipingFailureMechanismContext(pipingFailureMechanism, new TestAssessmentSection());
                var stochasticSoilModel = new StochasticSoilModel(0, "", "");

                stochasticSoilModel.Geometry.AddRange(new[]
                {
                    new Point2D(1, 2),
                    new Point2D(1, 2)
                });

                view.Data = pipingContext;

                var stochasticSoilModelMapData = (MapLineData) map.Data.Collection.ElementAt(stochasticSoilModelsIndex);

                // Call
                pipingFailureMechanism.StochasticSoilModels.Add(stochasticSoilModel);
                pipingFailureMechanism.StochasticSoilModels.NotifyObservers();

                // Assert
                AssertStochasticSoilModelsMapData(pipingFailureMechanism.StochasticSoilModels, stochasticSoilModelMapData);
            }
        }

        [Test]
        public void UpdateObserver_DataUpdated_MapLayersSameOrder()
        {
            // Setup
            const int updatedRefenceLineLayerIndex = referenceLineIndex + 6;
            const int updatedSurfaceLineLayerIndex = surfaceLinesIndex - 1;
            const int updatedSectionsLayerIndex = sectionsIndex - 1;
            const int updateSectionStartLayerIndex = sectionsStartPointIndex - 1;
            const int updatedSectionEndLayerIndex = sectionsEndPointIndex - 1;
            const int updatedHydraulicLocationsLayerIndex = hydraulicBoundaryDatabaseIndex - 1;
            const int updatedStochasticSoilModelsLayerIndex = stochasticSoilModelsIndex - 1;

            using (var view = new PipingFailureMechanismView())
            {
                var map = (MapControl) view.Controls[0];

                var assessmentSection = new TestAssessmentSection();
                var pipingFailureMechanism = new PipingFailureMechanism();
                var pipingContext = new PipingFailureMechanismContext(pipingFailureMechanism, assessmentSection);

                view.Data = pipingContext;

                var mapData = map.Data;

                var dataToMove = (MapLineData) map.Data.Collection.ElementAt(referenceLineIndex);
                mapData.Remove(dataToMove);
                mapData.Add(dataToMove);

                var mapDataList = mapData.Collection.ToList();

                // Precondition
                var referenceLineData = (MapLineData) mapDataList[updatedRefenceLineLayerIndex];
                Assert.AreEqual("Referentielijn", referenceLineData.Name);

                var surfaceLineData = (MapLineData) mapDataList[updatedSurfaceLineLayerIndex];
                Assert.AreEqual("Profielschematisaties", surfaceLineData.Name);

                var sectionsData = (MapLineData) mapDataList[updatedSectionsLayerIndex];
                Assert.AreEqual("Vakindeling", sectionsData.Name);

                var sectionStartsData = (MapPointData) mapDataList[updateSectionStartLayerIndex];
                Assert.AreEqual("Vakindeling (startpunten)", sectionStartsData.Name);

                var sectionEndsData = (MapPointData) mapDataList[updatedSectionEndLayerIndex];
                Assert.AreEqual("Vakindeling (eindpunten)", sectionEndsData.Name);

                var hydraulicLocationsData = (MapPointData) mapDataList[updatedHydraulicLocationsLayerIndex];
                Assert.AreEqual("Hydraulische randvoorwaarden", hydraulicLocationsData.Name);

                var stochasticSoilModelsData = (MapLineData) mapDataList[updatedStochasticSoilModelsLayerIndex];
                Assert.AreEqual("Stochastische ondergrondmodellen", stochasticSoilModelsData.Name);

                var points = new List<Point2D>
                {
                    new Point2D(2.0, 5.0),
                    new Point2D(4.0, 3.0)
                };
                ReferenceLine referenceLine = new ReferenceLine();
                referenceLine.SetGeometry(points);
                assessmentSection.ReferenceLine = referenceLine;

                // Call
                assessmentSection.NotifyObservers();

                // Call
                var actualReferenceLineData = (MapLineData) mapDataList[updatedRefenceLineLayerIndex];
                Assert.AreEqual("Referentielijn", actualReferenceLineData.Name);

                var actualSurfaceLineData = (MapLineData) mapDataList[updatedSurfaceLineLayerIndex];
                Assert.AreEqual("Profielschematisaties", actualSurfaceLineData.Name);

                var actualSectionsData = (MapLineData) mapDataList[updatedSectionsLayerIndex];
                Assert.AreEqual("Vakindeling", actualSectionsData.Name);

                var actualSectionStartsData = (MapPointData) mapDataList[updateSectionStartLayerIndex];
                Assert.AreEqual("Vakindeling (startpunten)", actualSectionStartsData.Name);

                var actualSectionEndsData = (MapPointData) mapDataList[updatedSectionEndLayerIndex];
                Assert.AreEqual("Vakindeling (eindpunten)", actualSectionEndsData.Name);

                var actualHydraulicLocationsData = (MapPointData) mapDataList[updatedHydraulicLocationsLayerIndex];
                Assert.AreEqual("Hydraulische randvoorwaarden", actualHydraulicLocationsData.Name);

                var actualStochasticSoilModelsData = (MapLineData) mapDataList[updatedStochasticSoilModelsLayerIndex];
                Assert.AreEqual("Stochastische ondergrondmodellen", actualStochasticSoilModelsData.Name);
            }
        }

        [Test]
        public void NotifyObservers_DataUpdatedNotifyObserversOnOldData_NoUpdateInViewData()
        {
            // Setup
            IAssessmentSection oldAssessmentSection = new TestAssessmentSection();
            IAssessmentSection newAssessmentSection = new TestAssessmentSection();

            newAssessmentSection.ReferenceLine = new ReferenceLine();
            newAssessmentSection.ReferenceLine.SetGeometry(new[]
            {
                new Point2D(2, 4),
                new Point2D(3, 4)
            });

            var oldPipingFailureMechanismContext = new PipingFailureMechanismContext(new PipingFailureMechanism(), oldAssessmentSection);
            var newPipingFailureMechanismContext = new PipingFailureMechanismContext(new PipingFailureMechanism(), newAssessmentSection);
            using (var view = new PipingFailureMechanismView())
            {
                var map = (MapControl) view.Controls[0];

                view.Data = oldPipingFailureMechanismContext;
                view.Data = newPipingFailureMechanismContext;
                MapData dataBeforeUpdate = map.Data;

                newAssessmentSection.ReferenceLine.SetGeometry(Enumerable.Empty<Point2D>());

                // Call
                oldAssessmentSection.NotifyObservers();

                // Assert
                Assert.AreEqual(dataBeforeUpdate, map.Data);
            }
        }

        private static void AssertReferenceLineMapData(ReferenceLine referenceLine, MapData mapData)
        {
            Assert.IsInstanceOf<MapLineData>(mapData);
            var referenceLineData = (MapLineData) mapData;
            if (referenceLine == null)
            {
                CollectionAssert.IsEmpty(referenceLineData.Features.First().MapGeometries.First().PointCollections.First());
            }
            else
            {
                CollectionAssert.AreEqual(referenceLine.Points, referenceLineData.Features.First().MapGeometries.First().PointCollections.First());
            }
            Assert.AreEqual("Referentielijn", mapData.Name);
        }

        private static void AssertHydraulicBoundaryLocationsMapData(HydraulicBoundaryDatabase database, MapData mapData)
        {
            Assert.IsInstanceOf<MapPointData>(mapData);
            var hydraulicLocationsMapData = (MapPointData) mapData;
            if (database == null)
            {
                CollectionAssert.IsEmpty(hydraulicLocationsMapData.Features.First().MapGeometries.First().PointCollections.First());
            }
            else
            {
                CollectionAssert.AreEqual(database.Locations.Select(hrp => hrp.Location), hydraulicLocationsMapData.Features.First().MapGeometries.First().PointCollections.First());
            }
            Assert.AreEqual("Hydraulische randvoorwaarden", mapData.Name);
        }

        private static void AssertFailureMechanismSectionsMapData(IEnumerable<FailureMechanismSection> sections, MapData mapData)
        {
            Assert.IsInstanceOf<MapLineData>(mapData);
            var sectionsMapLinesData = (MapLineData) mapData;
            var sectionMapLinesFeatures = sectionsMapLinesData.Features.ToArray();
            Assert.AreEqual(1, sectionMapLinesFeatures.Length);

            var geometries = sectionMapLinesFeatures.First().MapGeometries.ToArray();
            var sectionsArray = sections.ToArray();
            Assert.AreEqual(sectionsArray.Length, geometries.Length);

            for (int index = 0; index < sectionsArray.Length; index++)
            {
                var failureMechanismSection = sectionsArray[index];
                CollectionAssert.AreEquivalent(failureMechanismSection.Points, geometries[index].PointCollections.First());
            }
            Assert.AreEqual("Vakindeling", mapData.Name);
        }

        private static void AssertFailureMechanismSectionsStartPointMapData(IEnumerable<FailureMechanismSection> sections, MapData mapData)
        {
            Assert.IsInstanceOf<MapPointData>(mapData);
            var sectionsStartPointData = (MapPointData) mapData;
            CollectionAssert.AreEqual(sections.Select(s => s.GetStart()), sectionsStartPointData.Features.First().MapGeometries.First().PointCollections.First());
            Assert.AreEqual("Vakindeling (startpunten)", mapData.Name);
        }

        private static void AssertFailureMechanismSectionsEndPointMapData(IEnumerable<FailureMechanismSection> sections, MapData mapData)
        {
            Assert.IsInstanceOf<MapPointData>(mapData);
            var sectionsStartPointData = (MapPointData) mapData;
            CollectionAssert.AreEqual(sections.Select(s => s.GetLast()), sectionsStartPointData.Features.First().MapGeometries.First().PointCollections.First());
            Assert.AreEqual("Vakindeling (eindpunten)", mapData.Name);
        }

        private static void AssertSurfacelinesMapData(IEnumerable<RingtoetsPipingSurfaceLine> surfaceLines, MapData mapData)
        {
            Assert.IsInstanceOf<MapLineData>(mapData);
            var surfacelinesMapData = (MapLineData) mapData;
            var surfacelineFeatures = surfacelinesMapData.Features.ToArray();
            Assert.AreEqual(1, surfacelineFeatures.Length);

            var geometries = surfacelineFeatures.First().MapGeometries.ToArray();
            var surfaceLinesArray = surfaceLines.ToArray();
            Assert.AreEqual(surfaceLinesArray.Length, geometries.Length);

            for (int index = 0; index < surfaceLinesArray.Length; index++)
            {
                var surfaceLine = surfaceLinesArray[index];
                CollectionAssert.AreEquivalent(surfaceLine.Points.Select(p => new Point2D(p.X, p.Y)), geometries[index].PointCollections.First());
            }
            Assert.AreEqual("Profielschematisaties", mapData.Name);
        }

        private static void AssertStochasticSoilModelsMapData(IEnumerable<StochasticSoilModel> soilModels, MapData mapData)
        {
            Assert.IsInstanceOf<MapLineData>(mapData);
            var soilModelsMapData = (MapLineData) mapData;
            var soilModelsFeatures = soilModelsMapData.Features.ToArray();
            Assert.AreEqual(1, soilModelsFeatures.Length);

            var geometries = soilModelsFeatures.First().MapGeometries.ToArray();
            var stochasticSoilModelsArray = soilModels.ToArray();
            Assert.AreEqual(stochasticSoilModelsArray.Length, geometries.Length);

            for (int index = 0; index < stochasticSoilModelsArray.Length; index++)
            {
                var stochasticSoilModel = stochasticSoilModelsArray[index];
                CollectionAssert.AreEquivalent(stochasticSoilModel.Geometry.Select(p => new Point2D(p.X, p.Y)), geometries[index].PointCollections.First());
            }
            Assert.AreEqual("Stochastische ondergrondmodellen", mapData.Name);
        }

        private static void AssertEmptyMapData(MapDataCollection mapData)
        {
            Assert.IsInstanceOf<MapDataCollection>(mapData);

            var mapDataList = mapData.Collection.ToList();

            Assert.AreEqual(7, mapDataList.Count);

            var referenceLineMapData = (MapLineData) mapDataList[referenceLineIndex];
            var sectionsMapData = (MapLineData) mapDataList[sectionsIndex];
            var stochasticSoilModelsMapData = (MapLineData) mapDataList[stochasticSoilModelsIndex];
            var surfaceLinesMapData = (MapLineData) mapDataList[surfaceLinesIndex];
            var sectionsStartPointMapData = (MapPointData) mapDataList[sectionsStartPointIndex];
            var sectionsEndPointMapData = (MapPointData) mapDataList[sectionsEndPointIndex];
            var hydraulicBoundaryDatabaseMapData = (MapPointData) mapDataList[hydraulicBoundaryDatabaseIndex];

            CollectionAssert.IsEmpty(referenceLineMapData.Features);
            CollectionAssert.IsEmpty(sectionsMapData.Features);
            CollectionAssert.IsEmpty(stochasticSoilModelsMapData.Features);
            CollectionAssert.IsEmpty(surfaceLinesMapData.Features);
            CollectionAssert.IsEmpty(sectionsStartPointMapData.Features);
            CollectionAssert.IsEmpty(sectionsEndPointMapData.Features);
            CollectionAssert.IsEmpty(hydraulicBoundaryDatabaseMapData.Features);

            Assert.AreEqual(RingtoetsCommonDataResources.ReferenceLine_DisplayName, referenceLineMapData.Name);
            Assert.AreEqual(RingtoetsCommonFormsResources.FailureMechanism_Sections_DisplayName, sectionsMapData.Name);
            Assert.AreEqual(PipingFormsResources.StochasticSoilModelCollection_DisplayName, stochasticSoilModelsMapData.Name);
            Assert.AreEqual(PipingFormsResources.PipingSurfaceLinesCollection_DisplayName, surfaceLinesMapData.Name);
            Assert.AreEqual(GetSectionPointDisplayName(RingtoetsCommonFormsResources.FailureMechanismSections_StartPoints_DisplayName), sectionsStartPointMapData.Name);
            Assert.AreEqual(GetSectionPointDisplayName(RingtoetsCommonFormsResources.FailureMechanismSections_EndPoints_DisplayName), sectionsEndPointMapData.Name);
            Assert.AreEqual(RingtoetsCommonDataResources.HydraulicBoundaryConditions_DisplayName, hydraulicBoundaryDatabaseMapData.Name);
        }

        private static string GetSectionPointDisplayName(string name)
        {
            return string.Format("{0} ({1})",
                                 RingtoetsCommonFormsResources.FailureMechanism_Sections_DisplayName,
                                 name);
        }

        private class TestAssessmentSection : Observable, IAssessmentSection
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string Comments { get; set; }
            public AssessmentSectionComposition Composition { get; private set; }
            public ReferenceLine ReferenceLine { get; set; }
            public FailureMechanismContribution FailureMechanismContribution { get; private set; }
            public HydraulicBoundaryDatabase HydraulicBoundaryDatabase { get; set; }

            public long StorageId { get; set; }

            public IEnumerable<IFailureMechanism> GetFailureMechanisms()
            {
                yield break;
            }

            public void ChangeComposition(AssessmentSectionComposition newComposition)
            {
                throw new NotImplementedException();
            }
        }
    }
}