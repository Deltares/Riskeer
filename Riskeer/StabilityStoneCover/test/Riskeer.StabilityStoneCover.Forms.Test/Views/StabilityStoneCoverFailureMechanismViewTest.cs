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
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.TestUtil;
using Riskeer.Common.Forms.Views;
using Riskeer.StabilityStoneCover.Data;
using Riskeer.StabilityStoneCover.Forms.Views;

namespace Riskeer.StabilityStoneCover.Forms.Test.Views
{
    [TestFixture]
    public class StabilityStoneCoverFailureMechanismViewTest
    {
        private const int referenceLineIndex = 0;
        private const int hydraulicBoundaryLocationsIndex = 1;
        private const int foreshoreProfilesIndex = 2;
        private const int calculationsIndex = 3;

        private const int hydraulicBoundaryLocationsObserverIndex = 1;
        private const int foreshoreProfileObserverIndex = 2;
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
        [Apartment(ApartmentState.STA)]
        public void Constructor_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            void Call() => new StabilityStoneCoverFailureMechanismView(null, assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void Constructor_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new StabilityStoneCoverFailureMechanismView(new StabilityStoneCoverFailureMechanism(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var failureMechanism = new StabilityStoneCoverFailureMechanism();
            var assessmentSection = new AssessmentSectionStub();

            // Call
            StabilityStoneCoverFailureMechanismView view = CreateView(failureMechanism, assessmentSection);

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
            StabilityStoneCoverFailureMechanismView view = CreateView(new StabilityStoneCoverFailureMechanism(), assessmentSection);

            // Assert
            MapDataTestHelper.AssertImageBasedMapData(assessmentSection.BackgroundData, view.Map.BackgroundMapData);
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void Constructor_WithAllData_DataUpdatedToCollectionOfFilledMapData()
        {
            // Setup
            var calculationA = new StabilityStoneCoverWaveConditionsCalculation
            {
                InputParameters =
                {
                    ForeshoreProfile = new TestForeshoreProfile(new Point2D(1.3, 1.3)),
                    HydraulicBoundaryLocation = new TestHydraulicBoundaryLocation()
                }
            };
            var calculationB = new StabilityStoneCoverWaveConditionsCalculation
            {
                InputParameters =
                {
                    ForeshoreProfile = new TestForeshoreProfile(new Point2D(1.5, 1.5)),
                    HydraulicBoundaryLocation = new TestHydraulicBoundaryLocation()
                }
            };

            var geometryPoints = new[]
            {
                new Point2D(0.0, 0.0),
                new Point2D(2.0, 0.0),
                new Point2D(4.0, 4.0),
                new Point2D(6.0, 4.0)
            };

            var failureMechanism = new StabilityStoneCoverFailureMechanism();
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

            failureMechanism.WaveConditionsCalculationGroup.Children.Add(calculationA);
            failureMechanism.WaveConditionsCalculationGroup.Children.Add(calculationB);

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
            StabilityStoneCoverFailureMechanismView view = CreateView(failureMechanism, assessmentSection);

            IMapControl map = ((RiskeerMapControl) view.Controls[0]).MapControl;

            // Assert
            MapDataCollection mapData = map.Data;
            Assert.IsInstanceOf<MapDataCollection>(mapData);

            List<MapData> mapDataList = mapData.Collection.ToList();
            Assert.AreEqual(4, mapDataList.Count);
            MapDataTestHelper.AssertReferenceLineMapData(assessmentSection.ReferenceLine, mapDataList[referenceLineIndex]);

            MapDataTestHelper.AssertHydraulicBoundaryLocationsMapData(assessmentSection, mapDataList[hydraulicBoundaryLocationsIndex]);
            MapDataTestHelper.AssertForeshoreProfilesMapData(failureMechanism.ForeshoreProfiles, mapDataList[foreshoreProfilesIndex]);
            AssertCalculationsMapData(failureMechanism.Calculations.Cast<StabilityStoneCoverWaveConditionsCalculation>(), mapDataList[calculationsIndex]);
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void GivenViewWithHydraulicBoundaryLocationsData_WhenHydraulicBoundaryLocationsUpdatedAndNotified_ThenMapDataUpdated()
        {
            // Given
            var assessmentSection = new AssessmentSectionStub();
            assessmentSection.SetHydraulicBoundaryLocationCalculations(new[]
            {
                new HydraulicBoundaryLocation(1, "test1", 1.0, 2.0)
            });

            StabilityStoneCoverFailureMechanismView view = CreateView(new StabilityStoneCoverFailureMechanism(), assessmentSection);

            IMapControl map = ((RiskeerMapControl) view.Controls[0]).MapControl;

            var mocks = new MockRepository();
            IObserver[] observers = AttachMapDataObservers(mocks, map.Data.Collection);
            observers[hydraulicBoundaryLocationsObserverIndex].Expect(obs => obs.UpdateObserver());
            mocks.ReplayAll();

            MapData hydraulicBoundaryLocationsMapData = map.Data.Collection.ElementAt(hydraulicBoundaryLocationsIndex);

            // Precondition
            MapDataTestHelper.AssertHydraulicBoundaryLocationsMapData(assessmentSection, hydraulicBoundaryLocationsMapData);

            // When
            assessmentSection.SetHydraulicBoundaryLocationCalculations(new[]
            {
                new HydraulicBoundaryLocation(2, "test2", 3.0, 4.0)
            });
            assessmentSection.HydraulicBoundaryDatabase.Locations.NotifyObservers();

            // Then
            MapDataTestHelper.AssertHydraulicBoundaryLocationsMapData(assessmentSection, hydraulicBoundaryLocationsMapData);
            mocks.VerifyAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        [TestCaseSource(typeof(MapViewTestHelper), nameof(MapViewTestHelper.GetCalculationFuncs))]
        public void GivenViewWithHydraulicBoundaryLocationsData_WhenHydraulicBoundaryLocationCalculationUpdatedAndNotified_ThenMapDataUpdated(
            Func<IAssessmentSection, HydraulicBoundaryLocationCalculation> getCalculationFunc)
        {
            // Given
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "test1", 1.0, 2.0);
            var assessmentSection = new AssessmentSectionStub();
            assessmentSection.SetHydraulicBoundaryLocationCalculations(new[]
            {
                hydraulicBoundaryLocation
            });

            StabilityStoneCoverFailureMechanismView view = CreateView(new StabilityStoneCoverFailureMechanism(), assessmentSection);

            IMapControl map = ((RiskeerMapControl) view.Controls[0]).MapControl;

            var mocks = new MockRepository();
            IObserver[] observers = AttachMapDataObservers(mocks, map.Data.Collection);
            observers[hydraulicBoundaryLocationsObserverIndex].Expect(obs => obs.UpdateObserver());
            mocks.ReplayAll();

            MapData hydraulicBoundaryLocationsMapData = map.Data.Collection.ElementAt(hydraulicBoundaryLocationsIndex);

            // Precondition
            MapDataTestHelper.AssertHydraulicBoundaryLocationsMapData(assessmentSection, hydraulicBoundaryLocationsMapData);

            // When
            HydraulicBoundaryLocationCalculation calculation = getCalculationFunc(assessmentSection);
            calculation.Output = new TestHydraulicBoundaryLocationCalculationOutput(new Random(21).NextDouble());
            calculation.NotifyObservers();

            // Then
            MapDataTestHelper.AssertHydraulicBoundaryLocationsMapData(assessmentSection, hydraulicBoundaryLocationsMapData);
            mocks.ReplayAll();
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

            StabilityStoneCoverFailureMechanismView view = CreateView(new StabilityStoneCoverFailureMechanism(), assessmentSection);

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

            StabilityStoneCoverFailureMechanismView view = CreateView(new StabilityStoneCoverFailureMechanism(), assessmentSection);

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

            var failureMechanism = new StabilityStoneCoverFailureMechanism();
            failureMechanism.ForeshoreProfiles.AddRange(new[]
            {
                foreshoreProfile
            }, "path");

            StabilityStoneCoverFailureMechanismView view = CreateView(failureMechanism, new AssessmentSectionStub());

            IMapControl map = ((RiskeerMapControl) view.Controls[0]).MapControl;

            var mocks = new MockRepository();
            IObserver[] observers = AttachMapDataObservers(mocks, map.Data.Collection);
            observers[foreshoreProfileObserverIndex].Expect(obs => obs.UpdateObserver());
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
            var failureMechanism = new StabilityStoneCoverFailureMechanism();
            failureMechanism.ForeshoreProfiles.AddRange(new[]
            {
                new TestForeshoreProfile("originalProfile ID", new[]
                {
                    new Point2D(0, 0),
                    new Point2D(1, 1)
                })
            }, "path");

            StabilityStoneCoverFailureMechanismView view = CreateView(failureMechanism, new AssessmentSectionStub());

            IMapControl map = ((RiskeerMapControl) view.Controls[0]).MapControl;

            var mocks = new MockRepository();
            IObserver[] observers = AttachMapDataObservers(mocks, map.Data.Collection);
            observers[foreshoreProfileObserverIndex].Expect(obs => obs.UpdateObserver());
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
        public void GivenViewWithCalculationGroupData_WhenCalculationGroupUpdatedAndNotified_ThenMapDataUpdated()
        {
            // Given
            var calculationA = new StabilityStoneCoverWaveConditionsCalculation
            {
                InputParameters =
                {
                    ForeshoreProfile = new TestForeshoreProfile(new Point2D(1.3, 1.3)),
                    HydraulicBoundaryLocation = new TestHydraulicBoundaryLocation()
                }
            };
            var failureMechanism = new StabilityStoneCoverFailureMechanism();
            failureMechanism.WaveConditionsCalculationGroup.Children.Add(calculationA);

            StabilityStoneCoverFailureMechanismView view = CreateView(failureMechanism, new AssessmentSectionStub());

            IMapControl map = ((RiskeerMapControl) view.Controls[0]).MapControl;

            var mocks = new MockRepository();
            IObserver[] observers = AttachMapDataObservers(mocks, map.Data.Collection);
            observers[calculationObserverIndex].Expect(obs => obs.UpdateObserver());
            mocks.ReplayAll();

            var calculationMapData = (MapLineData) map.Data.Collection.ElementAt(calculationsIndex);

            // Precondition
            AssertCalculationsMapData(failureMechanism.Calculations.Cast<StabilityStoneCoverWaveConditionsCalculation>(),
                                      calculationMapData);

            // When
            var calculationB = new StabilityStoneCoverWaveConditionsCalculation
            {
                InputParameters =
                {
                    ForeshoreProfile = new TestForeshoreProfile(new Point2D(1.5, 1.5)),
                    HydraulicBoundaryLocation = new TestHydraulicBoundaryLocation()
                }
            };
            failureMechanism.WaveConditionsCalculationGroup.Children.Add(calculationB);
            failureMechanism.WaveConditionsCalculationGroup.NotifyObservers();

            // Then
            AssertCalculationsMapData(failureMechanism.Calculations.Cast<StabilityStoneCoverWaveConditionsCalculation>(), calculationMapData);
            mocks.VerifyAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void GivenViewWithCalculationInputData_WhenCalculationInputUpdatedAndNotified_ThenMapDataUpdated()
        {
            // Given
            var calculationA = new StabilityStoneCoverWaveConditionsCalculation
            {
                InputParameters =
                {
                    ForeshoreProfile = new TestForeshoreProfile(new Point2D(1.3, 1.3)),
                    HydraulicBoundaryLocation = new TestHydraulicBoundaryLocation()
                }
            };
            var failureMechanism = new StabilityStoneCoverFailureMechanism();
            failureMechanism.WaveConditionsCalculationGroup.Children.Add(calculationA);

            StabilityStoneCoverFailureMechanismView view = CreateView(failureMechanism, new AssessmentSectionStub());

            IMapControl map = ((RiskeerMapControl) view.Controls[0]).MapControl;

            var mocks = new MockRepository();
            IObserver[] observers = AttachMapDataObservers(mocks, map.Data.Collection);
            observers[calculationObserverIndex].Expect(obs => obs.UpdateObserver());
            mocks.ReplayAll();

            var calculationMapData = (MapLineData) map.Data.Collection.ElementAt(calculationsIndex);

            // Precondition
            AssertCalculationsMapData(failureMechanism.Calculations.Cast<StabilityStoneCoverWaveConditionsCalculation>(),
                                      calculationMapData);

            // When
            calculationA.InputParameters.ForeshoreProfile = new TestForeshoreProfile(new Point2D(1.5, 1.5));
            calculationA.InputParameters.NotifyObservers();

            // Then
            AssertCalculationsMapData(failureMechanism.Calculations.Cast<StabilityStoneCoverWaveConditionsCalculation>(), calculationMapData);
            mocks.VerifyAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void GivenViewWithCalculationData_WhenCalculationUpdatedAndNotified_ThenMapDataUpdated()
        {
            // Given
            var calculationA = new StabilityStoneCoverWaveConditionsCalculation
            {
                InputParameters =
                {
                    ForeshoreProfile = new TestForeshoreProfile(new Point2D(1.3, 1.3)),
                    HydraulicBoundaryLocation = new TestHydraulicBoundaryLocation()
                }
            };
            var failureMechanism = new StabilityStoneCoverFailureMechanism();
            failureMechanism.WaveConditionsCalculationGroup.Children.Add(calculationA);

            StabilityStoneCoverFailureMechanismView view = CreateView(failureMechanism, new AssessmentSectionStub());

            IMapControl map = ((RiskeerMapControl) view.Controls[0]).MapControl;

            var mocks = new MockRepository();
            IObserver[] observers = AttachMapDataObservers(mocks, map.Data.Collection);
            observers[calculationObserverIndex].Expect(obs => obs.UpdateObserver());
            mocks.ReplayAll();

            var calculationMapData = (MapLineData) map.Data.Collection.ElementAt(calculationsIndex);

            // Precondition
            AssertCalculationsMapData(failureMechanism.Calculations.Cast<StabilityStoneCoverWaveConditionsCalculation>(),
                                      calculationMapData);

            // When
            calculationA.Name = "new name";
            calculationA.NotifyObservers();

            // Then
            AssertCalculationsMapData(failureMechanism.Calculations.Cast<StabilityStoneCoverWaveConditionsCalculation>(), calculationMapData);
            mocks.VerifyAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void UpdateObserver_DataUpdated_MapLayersSameOrder()
        {
            // Setup
            const int updatedReferenceLineLayerIndex = referenceLineIndex + 3;
            const int updatedHydraulicLocationsLayerIndex = hydraulicBoundaryLocationsIndex - 1;
            const int updatedForeshoreProfilesLayerIndex = foreshoreProfilesIndex - 1;
            const int updatedCalculationsIndex = calculationsIndex - 1;

            var assessmentSection = new AssessmentSectionStub();
            var failureMechanism = new StabilityStoneCoverFailureMechanism();

            StabilityStoneCoverFailureMechanismView view = CreateView(failureMechanism, assessmentSection);

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

            var actualCalculationsData = (MapLineData) mapDataList[updatedCalculationsIndex];
            Assert.AreEqual("Berekeningen", actualCalculationsData.Name);
        }

        private StabilityStoneCoverFailureMechanismView CreateView(StabilityStoneCoverFailureMechanism failureMechanism, IAssessmentSection assessmentSection)
        {
            var view = new StabilityStoneCoverFailureMechanismView(failureMechanism, assessmentSection);

            testForm.Controls.Add(view);
            testForm.Show();

            return view;
        }

        private static void AssertCalculationsMapData(IEnumerable<StabilityStoneCoverWaveConditionsCalculation> calculations, MapData mapData)
        {
            Assert.IsInstanceOf<MapLineData>(mapData);
            var calculationsMapData = (MapLineData) mapData;
            StabilityStoneCoverWaveConditionsCalculation[] calculationsArray = calculations.ToArray();
            MapFeature[] calculationsFeatures = calculationsMapData.Features.ToArray();
            Assert.AreEqual(calculationsArray.Length, calculationsFeatures.Length);

            for (var index = 0; index < calculationsArray.Length; index++)
            {
                MapGeometry[] geometries = calculationsFeatures[index].MapGeometries.ToArray();
                Assert.AreEqual(1, geometries.Length);

                StabilityStoneCoverWaveConditionsCalculation calculation = calculationsArray[index];
                CollectionAssert.AreEquivalent(new[]
                {
                    calculation.InputParameters.ForeshoreProfile.WorldReferencePoint,
                    calculation.InputParameters.HydraulicBoundaryLocation.Location
                }, geometries[0].PointCollections.First());
            }

            Assert.AreEqual("Berekeningen", mapData.Name);
        }

        private static void AssertEmptyMapData(MapDataCollection mapDataCollection)
        {
            Assert.AreEqual("Dijken en dammen - Stabiliteit steenzetting", mapDataCollection.Name);

            List<MapData> mapDataList = mapDataCollection.Collection.ToList();

            Assert.AreEqual(4, mapDataList.Count);

            var referenceLineMapData = (MapLineData) mapDataList[referenceLineIndex];
            var foreshoreProfilesMapData = (MapLineData) mapDataList[foreshoreProfilesIndex];
            var hydraulicBoundaryLocationsMapData = (MapPointData) mapDataList[hydraulicBoundaryLocationsIndex];
            var calculationsMapData = (MapLineData) mapDataList[calculationsIndex];

            CollectionAssert.IsEmpty(referenceLineMapData.Features);
            CollectionAssert.IsEmpty(foreshoreProfilesMapData.Features);
            CollectionAssert.IsEmpty(hydraulicBoundaryLocationsMapData.Features);
            CollectionAssert.IsEmpty(calculationsMapData.Features);

            Assert.AreEqual("Referentielijn", referenceLineMapData.Name);
            Assert.AreEqual("Voorlandprofielen", foreshoreProfilesMapData.Name);
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

            var hydraulicBoundaryLocationsMapDataObserver = mocks.StrictMock<IObserver>();
            mapDataArray[hydraulicBoundaryLocationsIndex].Attach(hydraulicBoundaryLocationsMapDataObserver);

            var foreshoreProfilesMapDataObserver = mocks.StrictMock<IObserver>();
            mapDataArray[foreshoreProfilesIndex].Attach(foreshoreProfilesMapDataObserver);

            var calculationsMapDataObserver = mocks.StrictMock<IObserver>();
            mapDataArray[calculationsIndex].Attach(calculationsMapDataObserver);

            return new[]
            {
                referenceLineMapDataObserver,
                hydraulicBoundaryLocationsMapDataObserver,
                foreshoreProfilesMapDataObserver,
                calculationsMapDataObserver
            };
        }
    }
}