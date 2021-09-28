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
using System.Threading;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Base.Geometry;
using Core.Components.Gis.Data;
using Core.Components.Gis.Features;
using Core.Components.Gis.Forms;
using Core.Components.Gis.Geometries;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.Structures;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.TestUtil;
using Riskeer.Common.Forms.Views;
using Riskeer.HeightStructures.Data;
using Riskeer.HeightStructures.Data.TestUtil;
using Riskeer.HeightStructures.Forms.Views;

namespace Riskeer.HeightStructures.Forms.Test.Views
{
    [TestFixture]
    public class HeightStructuresFailureMechanismViewTest
    {
        private const int referenceLineIndex = 0;
        private const int hydraulicBoundaryLocationsIndex = 1;
        private const int foreshoreProfilesIndex = 2;
        private const int structuresIndex = 3;
        private const int calculationsIndex = 4;

        private const int foreshoreProfilesObserverIndex = 1;
        private const int structuresObserverIndex = 2;
        private const int calculationObserverIndex = 3;

        private Form testForm;

        [SetUp]
        public void Setup()
        {
            testForm = new Form();
        }

        [TearDown]
        public void TearDown()
        {
            testForm.Dispose();
        }

        [Test]
        public void Constructor_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            void Call() => new HeightStructuresFailureMechanismView(null, assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new HeightStructuresFailureMechanismView(new HeightStructuresFailureMechanism(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var failureMechanism = new HeightStructuresFailureMechanism();
            var assessmentSection = new AssessmentSectionStub();

            // Call
            HeightStructuresFailureMechanismView view = CreateView(failureMechanism, assessmentSection);

            // Assert
            Assert.IsInstanceOf<UserControl>(view);
            Assert.IsInstanceOf<IMapView>(view);
            Assert.IsNull(view.Data);
            Assert.AreSame(failureMechanism, view.FailureMechanism);
            Assert.AreSame(assessmentSection, view.AssessmentSection);

            Assert.AreEqual(1, view.Controls.Count);
            Assert.IsInstanceOf<RiskeerMapControl>(view.Controls[0]);
            Assert.AreSame(view.Map, ((RiskeerMapControl) view.Controls[0]).MapControl);
            Assert.AreEqual(DockStyle.Fill, ((Control) view.Map).Dock);
            AssertEmptyMapData(view.Map.Data);
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void Constructor_AssessmentSectionWithBackgroundData_BackgroundDataSet()
        {
            // Setup
            IAssessmentSection assessmentSection = new AssessmentSectionStub();

            // Call
            HeightStructuresFailureMechanismView view = CreateView(new HeightStructuresFailureMechanism(), assessmentSection);

            // Assert
            MapDataTestHelper.AssertImageBasedMapData(assessmentSection.BackgroundData, view.Map.BackgroundMapData);
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void Constructor_WithAllData_DataUpdatedToCollectionOfFilledMapData()
        {
            // Setup
            var calculationA = new StructuresCalculationScenario<HeightStructuresInput>
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, string.Empty, 1.3, 2.3),
                    Structure = new TestHeightStructure(new Point2D(1.2, 2.3))
                }
            };

            var calculationB = new StructuresCalculationScenario<HeightStructuresInput>
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, string.Empty, 7.7, 12.6),
                    Structure = new TestHeightStructure(new Point2D(2.7, 2.0))
                }
            };

            var geometryPoints = new[]
            {
                new Point2D(0.0, 0.0),
                new Point2D(2.0, 0.0),
                new Point2D(4.0, 4.0),
                new Point2D(6.0, 4.0)
            };

            var failureMechanism = new HeightStructuresFailureMechanism();
            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                new FailureMechanismSection("A", geometryPoints.Take(2)),
                new FailureMechanismSection("B", geometryPoints.Skip(1).Take(2)),
                new FailureMechanismSection("C", geometryPoints.Skip(2).Take(2))
            });

            var profile1 = new TestForeshoreProfile("profile1 ID", new[]
            {
                new Point2D(0, 0),
                new Point2D(1, 1)
            });
            var profile2 = new TestForeshoreProfile("profile2 ID", new[]
            {
                new Point2D(2, 2),
                new Point2D(3, 3)
            });
            failureMechanism.ForeshoreProfiles.AddRange(new[]
            {
                profile1,
                profile2
            }, "path");
            failureMechanism.CalculationsGroup.Children.Add(calculationA);
            failureMechanism.CalculationsGroup.Children.Add(calculationB);

            var referenceLine = new ReferenceLine();
            referenceLine.SetGeometry(new[]
            {
                new Point2D(1.0, 2.0),
                new Point2D(2.0, 1.0)
            });

            var assessmentSection = new AssessmentSectionStub
            {
                ReferenceLine = referenceLine
            };
            assessmentSection.SetHydraulicBoundaryLocationCalculations(new[]
            {
                new HydraulicBoundaryLocation(1, "test", 1.0, 2.0)
            });

            // Call
            HeightStructuresFailureMechanismView view = CreateView(failureMechanism, assessmentSection);

            IMapControl map = ((RiskeerMapControl) view.Controls[0]).MapControl;

            // Assert
            MapDataCollection mapData = map.Data;
            Assert.IsInstanceOf<MapDataCollection>(mapData);

            List<MapData> mapDataList = mapData.Collection.ToList();
            Assert.AreEqual(5, mapDataList.Count);
            MapDataTestHelper.AssertReferenceLineMapData(assessmentSection.ReferenceLine, mapDataList[referenceLineIndex]);

            MapDataTestHelper.AssertHydraulicBoundaryLocationsMapData(assessmentSection, mapDataList[hydraulicBoundaryLocationsIndex]);
            MapDataTestHelper.AssertForeshoreProfilesMapData(failureMechanism.ForeshoreProfiles, mapDataList[foreshoreProfilesIndex]);
            AssertCalculationsMapData(
                failureMechanism.Calculations.Cast<StructuresCalculation<HeightStructuresInput>>(),
                mapDataList[calculationsIndex]);
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void GivenViewWithAssessmentSectionData_WhenAssessmentSectionUpdatedAndNotified_ThenMapDataUpdated()
        {
            // Given
            var referenceLine = new ReferenceLine();
            referenceLine.SetGeometry(new List<Point2D>
            {
                new Point2D(1.0, 2.0),
                new Point2D(2.0, 1.0)
            });
            var assessmentSection = new AssessmentSectionStub
            {
                ReferenceLine = referenceLine
            };

            HeightStructuresFailureMechanismView view = CreateView(new HeightStructuresFailureMechanism(), assessmentSection);

            IMapControl map = ((RiskeerMapControl) view.Controls[0]).MapControl;

            var mocks = new MockRepository();
            IObserver[] observers = AttachMapDataObservers(mocks, map.Data.Collection);
            observers[referenceLineIndex].Expect(obs => obs.UpdateObserver());
            mocks.ReplayAll();

            var referenceLineMapData = (MapLineData) map.Data.Collection.ElementAt(referenceLineIndex);

            // Precondition
            MapFeaturesTestHelper.AssertReferenceLineMetaData(assessmentSection.ReferenceLine, assessmentSection, referenceLineMapData.Features);

            // When
            assessmentSection.Name = "New name";
            assessmentSection.NotifyObservers();

            // Then
            MapFeaturesTestHelper.AssertReferenceLineMetaData(assessmentSection.ReferenceLine, assessmentSection, referenceLineMapData.Features);
            mocks.VerifyAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void GivenViewWithReferenceLineData_WhenReferenceLineUpdatedAndNotified_ThenMapDataUpdated()
        {
            // Given
            var referenceLine = new ReferenceLine();
            referenceLine.SetGeometry(new List<Point2D>
            {
                new Point2D(1.0, 2.0),
                new Point2D(2.0, 1.0)
            });
            var assessmentSection = new AssessmentSectionStub
            {
                ReferenceLine = referenceLine
            };

            HeightStructuresFailureMechanismView view = CreateView(new HeightStructuresFailureMechanism(), assessmentSection);

            IMapControl map = ((RiskeerMapControl) view.Controls[0]).MapControl;

            var mocks = new MockRepository();
            IObserver[] observers = AttachMapDataObservers(mocks, map.Data.Collection);
            observers[referenceLineIndex].Expect(obs => obs.UpdateObserver());
            mocks.ReplayAll();

            MapData referenceLineMapData = map.Data.Collection.ElementAt(referenceLineIndex);

            // Precondition
            MapDataTestHelper.AssertReferenceLineMapData(assessmentSection.ReferenceLine, referenceLineMapData);

            // When
            referenceLine.SetGeometry(new List<Point2D>
            {
                new Point2D(2.0, 5.0),
                new Point2D(4.0, 3.0)
            });
            referenceLine.NotifyObservers();

            // Then
            MapDataTestHelper.AssertReferenceLineMapData(assessmentSection.ReferenceLine, referenceLineMapData);
            mocks.VerifyAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void GivenViewWithForeshoreProfileData_WhenForeshoreProfileUpdatedAndNotified_ThenMapDataUpdated()
        {
            // Given
            var foreshoreProfile = new TestForeshoreProfile("originalProfile ID", new[]
            {
                new Point2D(0, 0),
                new Point2D(1, 1)
            });
            var failureMechanism = new HeightStructuresFailureMechanism();
            failureMechanism.ForeshoreProfiles.AddRange(new[]
            {
                foreshoreProfile
            }, "path");

            HeightStructuresFailureMechanismView view = CreateView(failureMechanism, new AssessmentSectionStub());

            IMapControl map = ((RiskeerMapControl) view.Controls[0]).MapControl;

            var mocks = new MockRepository();
            IObserver[] observers = AttachMapDataObservers(mocks, map.Data.Collection);
            observers[foreshoreProfilesObserverIndex].Expect(obs => obs.UpdateObserver());
            mocks.ReplayAll();

            MapData foreshoreProfileData = map.Data.Collection.ElementAt(foreshoreProfilesIndex);

            // Precondition
            MapDataTestHelper.AssertForeshoreProfilesMapData(failureMechanism.ForeshoreProfiles, foreshoreProfileData);

            // When
            var foreshoreProfileToUpdateFrom = new TestForeshoreProfile("originalProfile ID", new[]
            {
                new Point2D(2, 2),
                new Point2D(3, 3)
            });
            foreshoreProfile.CopyProperties(foreshoreProfileToUpdateFrom);
            foreshoreProfile.NotifyObservers();

            // Then
            MapDataTestHelper.AssertForeshoreProfilesMapData(failureMechanism.ForeshoreProfiles, foreshoreProfileData);
            mocks.VerifyAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void GivenViewWithForeshoreProfilesData_WhenForeshoreProfilesUpdatedAndNotified_ThenMapDataUpdated()
        {
            // Given
            var failureMechanism = new HeightStructuresFailureMechanism();
            failureMechanism.ForeshoreProfiles.AddRange(new[]
            {
                new TestForeshoreProfile("originalProfile ID", new[]
                {
                    new Point2D(0, 0),
                    new Point2D(1, 1)
                })
            }, "path");

            HeightStructuresFailureMechanismView view = CreateView(failureMechanism, new AssessmentSectionStub());

            IMapControl map = ((RiskeerMapControl) view.Controls[0]).MapControl;

            var mocks = new MockRepository();
            IObserver[] observers = AttachMapDataObservers(mocks, map.Data.Collection);
            observers[foreshoreProfilesObserverIndex].Expect(obs => obs.UpdateObserver());
            mocks.ReplayAll();

            MapData foreshoreProfileData = map.Data.Collection.ElementAt(foreshoreProfilesIndex);

            // Precondition
            MapDataTestHelper.AssertForeshoreProfilesMapData(failureMechanism.ForeshoreProfiles, foreshoreProfileData);

            // When
            failureMechanism.ForeshoreProfiles.AddRange(new[]
            {
                new TestForeshoreProfile("newProfile ID", new[]
                {
                    new Point2D(2, 2),
                    new Point2D(3, 3)
                })
            }, "path");
            failureMechanism.ForeshoreProfiles.NotifyObservers();

            // Then
            MapDataTestHelper.AssertForeshoreProfilesMapData(failureMechanism.ForeshoreProfiles, foreshoreProfileData);
            mocks.VerifyAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void GivenViewWithStructureData_WhenStructureUpdatedAndNotified_ThenMapDataUpdated()
        {
            // Given
            var structure = new TestHeightStructure(new Point2D(0, 0), "Id");
            var failureMechanism = new HeightStructuresFailureMechanism();
            failureMechanism.HeightStructures.AddRange(new[]
            {
                structure
            }, "path");

            HeightStructuresFailureMechanismView view = CreateView(failureMechanism, new AssessmentSectionStub());

            IMapControl map = ((RiskeerMapControl) view.Controls[0]).MapControl;

            var mocks = new MockRepository();
            IObserver[] observers = AttachMapDataObservers(mocks, map.Data.Collection);
            observers[structuresObserverIndex].Expect(obs => obs.UpdateObserver());
            mocks.ReplayAll();

            MapData structuresData = map.Data.Collection.ElementAt(structuresIndex);

            // Precondition
            MapDataTestHelper.AssertStructuresMapData(failureMechanism.HeightStructures,
                                                      structuresData);

            // When
            structure.CopyProperties(new TestHeightStructure(new Point2D(1, 1), "Id"));
            structure.NotifyObservers();

            // Then
            MapDataTestHelper.AssertStructuresMapData(failureMechanism.HeightStructures,
                                                      structuresData);
            mocks.VerifyAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void GivenViewWithStructuresData_WhenStructuresUpdatedAndNotified_ThenMapDataUpdated()
        {
            // Given
            var failureMechanism = new HeightStructuresFailureMechanism();
            failureMechanism.HeightStructures.AddRange(new[]
            {
                new TestHeightStructure(new Point2D(0, 0), "Id1")
            }, "path");

            HeightStructuresFailureMechanismView view = CreateView(failureMechanism, new AssessmentSectionStub());

            IMapControl map = ((RiskeerMapControl) view.Controls[0]).MapControl;

            var mocks = new MockRepository();
            IObserver[] observers = AttachMapDataObservers(mocks, map.Data.Collection);
            observers[structuresObserverIndex].Expect(obs => obs.UpdateObserver());
            mocks.ReplayAll();

            MapData structuresData = map.Data.Collection.ElementAt(structuresIndex);

            // Precondition
            MapDataTestHelper.AssertStructuresMapData(failureMechanism.HeightStructures,
                                                      structuresData);

            // When
            failureMechanism.HeightStructures.AddRange(new[]
            {
                new TestHeightStructure(new Point2D(1, 1), "Id2")
            }, "some path");
            failureMechanism.HeightStructures.NotifyObservers();

            // Then
            MapDataTestHelper.AssertStructuresMapData(failureMechanism.HeightStructures,
                                                      structuresData);
            mocks.VerifyAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void GivenViewWithCalculationGroupData_WhenCalculationGroupUpdatedAndNotified_ThenMapDataUpdated()
        {
            // Given
            var calculationA = new StructuresCalculation<HeightStructuresInput>
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, string.Empty, 1.3, 2.3),
                    Structure = new TestHeightStructure(new Point2D(1.2, 2.3))
                }
            };

            var failureMechanism = new HeightStructuresFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(calculationA);

            HeightStructuresFailureMechanismView view = CreateView(failureMechanism, new AssessmentSectionStub());

            IMapControl map = ((RiskeerMapControl) view.Controls[0]).MapControl;

            var mocks = new MockRepository();
            IObserver[] observers = AttachMapDataObservers(mocks, map.Data.Collection);
            observers[calculationObserverIndex].Expect(obs => obs.UpdateObserver());
            mocks.ReplayAll();

            var calculationMapData = (MapLineData) map.Data.Collection.ElementAt(calculationsIndex);

            // Precondition
            AssertCalculationsMapData(failureMechanism.Calculations.Cast<StructuresCalculation<HeightStructuresInput>>(),
                                      calculationMapData);

            // When
            var calculationB = new StructuresCalculation<HeightStructuresInput>
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, string.Empty, 7.7, 12.6),
                    Structure = new TestHeightStructure(new Point2D(2.7, 2.0))
                }
            };

            failureMechanism.CalculationsGroup.Children.Add(calculationB);
            failureMechanism.CalculationsGroup.NotifyObservers();

            // Then
            AssertCalculationsMapData(failureMechanism.Calculations.Cast<StructuresCalculation<HeightStructuresInput>>(), calculationMapData);
            mocks.VerifyAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void GivenViewWithCalculationInputData_WhenCalculationInputUpdatedAndNotified_ThenMapDataUpdated()
        {
            // Given
            var calculationA = new StructuresCalculation<HeightStructuresInput>
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, string.Empty, 1.3, 2.3),
                    Structure = new TestHeightStructure(new Point2D(1.2, 2.3))
                }
            };
            var failureMechanism = new HeightStructuresFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(calculationA);

            HeightStructuresFailureMechanismView view = CreateView(failureMechanism, new AssessmentSectionStub());

            IMapControl map = ((RiskeerMapControl) view.Controls[0]).MapControl;

            var mocks = new MockRepository();
            IObserver[] observers = AttachMapDataObservers(mocks, map.Data.Collection);
            observers[calculationObserverIndex].Expect(obs => obs.UpdateObserver());
            mocks.ReplayAll();

            var calculationMapData = (MapLineData) map.Data.Collection.ElementAt(calculationsIndex);

            // Precondition
            AssertCalculationsMapData(failureMechanism.Calculations.Cast<StructuresCalculation<HeightStructuresInput>>(),
                                      calculationMapData);

            // When
            calculationA.InputParameters.Structure = new TestHeightStructure(new Point2D(2.7, 2.0));
            calculationA.InputParameters.NotifyObservers();

            // Then
            AssertCalculationsMapData(failureMechanism.Calculations.Cast<StructuresCalculation<HeightStructuresInput>>(), calculationMapData);
            mocks.VerifyAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void NotifyObservers_DataUpdated_MapLayersSameOrder()
        {
            // Setup
            const int updatedReferenceLineLayerIndex = referenceLineIndex + 4;
            const int updatedHydraulicLocationsLayerIndex = hydraulicBoundaryLocationsIndex - 1;
            const int updatedForeshoreProfilesLayerIndex = foreshoreProfilesIndex - 1;
            const int updatedStructuresLayerIndex = structuresIndex - 1;
            const int updatedCalculationsIndex = calculationsIndex - 1;

            var assessmentSection = new AssessmentSectionStub();

            HeightStructuresFailureMechanismView view = CreateView(new HeightStructuresFailureMechanism(), assessmentSection);
            IMapControl map = ((RiskeerMapControl) view.Controls[0]).MapControl;

            MapDataCollection mapData = map.Data;

            var dataToMove = (MapLineData) map.Data.Collection.ElementAt(referenceLineIndex);
            mapData.Remove(dataToMove);
            mapData.Add(dataToMove);

            List<MapData> mapDataList = mapData.Collection.ToList();

            // Precondition
            var referenceLineData = (MapLineData) mapDataList[updatedReferenceLineLayerIndex];
            Assert.AreEqual("Referentielijn", referenceLineData.Name);

            var hydraulicLocationsData = (MapPointData) mapDataList[updatedHydraulicLocationsLayerIndex];
            Assert.AreEqual("Hydraulische belastingen", hydraulicLocationsData.Name);

            var foreshoreProfilesData = (MapLineData) mapDataList[updatedForeshoreProfilesLayerIndex];
            Assert.AreEqual("Voorlandprofielen", foreshoreProfilesData.Name);

            var structuresData = (MapPointData) mapDataList[updatedStructuresLayerIndex];
            Assert.AreEqual("Kunstwerken", structuresData.Name);

            var calculationsData = (MapLineData) mapDataList[updatedCalculationsIndex];
            Assert.AreEqual("Berekeningen", calculationsData.Name);

            var points = new List<Point2D>
            {
                new Point2D(2.0, 5.0),
                new Point2D(4.0, 3.0)
            };
            var referenceLine = new ReferenceLine();
            referenceLine.SetGeometry(points);
            assessmentSection.ReferenceLine = referenceLine;

            // Call
            assessmentSection.NotifyObservers();

            // Assert
            var actualReferenceLineData = (MapLineData) mapDataList[updatedReferenceLineLayerIndex];
            Assert.AreEqual("Referentielijn", actualReferenceLineData.Name);

            var actualHydraulicLocationsData = (MapPointData) mapDataList[updatedHydraulicLocationsLayerIndex];
            Assert.AreEqual("Hydraulische belastingen", actualHydraulicLocationsData.Name);

            var actualForeshoreProfilesData = (MapLineData) mapDataList[updatedForeshoreProfilesLayerIndex];
            Assert.AreEqual("Voorlandprofielen", actualForeshoreProfilesData.Name);

            var actualStructuresData = (MapPointData) mapDataList[updatedStructuresLayerIndex];
            Assert.AreEqual("Kunstwerken", actualStructuresData.Name);

            var actualCalculationsData = (MapLineData) mapDataList[updatedCalculationsIndex];
            Assert.AreEqual("Berekeningen", actualCalculationsData.Name);
        }

        private HeightStructuresFailureMechanismView CreateView(HeightStructuresFailureMechanism failureMechanism, IAssessmentSection assessmentSection)
        {
            var view = new HeightStructuresFailureMechanismView(failureMechanism, assessmentSection);

            testForm.Controls.Add(view);
            testForm.Show();

            return view;
        }

        private static void AssertCalculationsMapData(IEnumerable<StructuresCalculation<HeightStructuresInput>> calculations, MapData mapData)
        {
            Assert.IsInstanceOf<MapLineData>(mapData);
            var calculationsMapData = (MapLineData) mapData;
            StructuresCalculation<HeightStructuresInput>[] calculationsArray = calculations.ToArray();
            MapFeature[] calculationsFeatures = calculationsMapData.Features.ToArray();
            Assert.AreEqual(calculationsArray.Length, calculationsFeatures.Length);

            for (var index = 0; index < calculationsArray.Length; index++)
            {
                MapGeometry[] geometries = calculationsFeatures[index].MapGeometries.ToArray();
                Assert.AreEqual(1, geometries.Length);

                StructuresCalculation<HeightStructuresInput> calculation = calculationsArray[index];
                CollectionAssert.AreEquivalent(new[]
                {
                    calculation.InputParameters.Structure.Location,
                    calculation.InputParameters.HydraulicBoundaryLocation.Location
                }, geometries[0].PointCollections.First());
            }

            Assert.AreEqual("Berekeningen", mapData.Name);
        }

        private static void AssertEmptyMapData(MapDataCollection mapDataCollection)
        {
            Assert.AreEqual("Kunstwerken - Hoogte kunstwerk", mapDataCollection.Name);

            List<MapData> mapDataList = mapDataCollection.Collection.ToList();

            Assert.AreEqual(5, mapDataList.Count);

            var referenceLineMapData = (MapLineData) mapDataList[referenceLineIndex];
            var foreshoreProfilesMapData = (MapLineData) mapDataList[foreshoreProfilesIndex];
            var structuresMapData = (MapPointData) mapDataList[structuresIndex];
            var hydraulicBoundaryLocationsMapData = (MapPointData) mapDataList[hydraulicBoundaryLocationsIndex];
            var calculationsMapData = (MapLineData) mapDataList[calculationsIndex];

            CollectionAssert.IsEmpty(referenceLineMapData.Features);
            CollectionAssert.IsEmpty(foreshoreProfilesMapData.Features);
            CollectionAssert.IsEmpty(structuresMapData.Features);
            CollectionAssert.IsEmpty(hydraulicBoundaryLocationsMapData.Features);
            CollectionAssert.IsEmpty(calculationsMapData.Features);

            Assert.AreEqual("Referentielijn", referenceLineMapData.Name);
            Assert.AreEqual("Voorlandprofielen", foreshoreProfilesMapData.Name);
            Assert.AreEqual("Kunstwerken", structuresMapData.Name);
            Assert.AreEqual("Hydraulische belastingen", hydraulicBoundaryLocationsMapData.Name);
            Assert.AreEqual("Berekeningen", calculationsMapData.Name);
        }

        /// <summary>
        /// Attaches mocked observers to all <see cref="IObservable"/> map data components.
        /// </summary>
        /// <param name="mocks">The <see cref="MockRepository"/>.</param>
        /// <param name="mapData">The map data collection containing the <see cref="IObservable"/>
        /// elements.</param>
        /// <returns>An array of mocked observers attached to the data in <paramref name="mapData"/>.</returns>
        private static IObserver[] AttachMapDataObservers(MockRepository mocks, IEnumerable<MapData> mapData)
        {
            MapData[] mapDataArray = mapData.ToArray();

            var referenceLineMapDataObserver = mocks.StrictMock<IObserver>();
            mapDataArray[referenceLineIndex].Attach(referenceLineMapDataObserver);

            var foreshoreProfilesMapDataObserver = mocks.StrictMock<IObserver>();
            mapDataArray[foreshoreProfilesIndex].Attach(foreshoreProfilesMapDataObserver);

            var structuresMapDataObserver = mocks.StrictMock<IObserver>();
            mapDataArray[structuresIndex].Attach(structuresMapDataObserver);

            var calculationsMapDataObserver = mocks.StrictMock<IObserver>();
            mapDataArray[calculationsIndex].Attach(calculationsMapDataObserver);

            return new[]
            {
                referenceLineMapDataObserver,
                foreshoreProfilesMapDataObserver,
                structuresMapDataObserver,
                calculationsMapDataObserver
            };
        }
    }
}