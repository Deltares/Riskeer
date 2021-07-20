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
using Riskeer.Common.Data.DikeProfiles;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.TestUtil;
using Riskeer.Common.Forms.Views;
using Riskeer.GrassCoverErosionInwards.Data;
using Riskeer.GrassCoverErosionInwards.Forms.Views;

namespace Riskeer.GrassCoverErosionInwards.Forms.Test.Views
{
    [TestFixture]
    public class GrassCoverErosionInwardsFailureMechanismViewTest
    {
        private const int referenceLineIndex = 0;
        private const int hydraulicBoundaryLocationsIndex = 1;
        private const int dikeProfilesIndex = 2;
        private const int foreshoreProfilesIndex = 3;
        private const int calculationsIndex = 4;

        private const int hydraulicBoundaryLocationsObserverIndex = 1;
        private const int dikeProfilesObserverIndex = 2;
        private const int foreshoreProfileObserverIndex = 3;
        private const int calculationObserverIndex = 4;

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
            void Call() => new GrassCoverErosionInwardsFailureMechanismView(null, assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new GrassCoverErosionInwardsFailureMechanismView(new GrassCoverErosionInwardsFailureMechanism(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var assessmentSection = new AssessmentSectionStub();

            // Call
            GrassCoverErosionInwardsFailureMechanismView view = CreateView(failureMechanism, assessmentSection);

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
            GrassCoverErosionInwardsFailureMechanismView view = CreateView(new GrassCoverErosionInwardsFailureMechanism(), assessmentSection);

            // Assert
            MapDataTestHelper.AssertImageBasedMapData(assessmentSection.BackgroundData, view.Map.BackgroundMapData);
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void Constructor_WithAllData_DataUpdatedToCollectionOfFilledMapData()
        {
            // Setup
            var calculationA = new GrassCoverErosionInwardsCalculationScenario(double.NaN)
            {
                InputParameters =
                {
                    DikeProfile = DikeProfileTestFactory.CreateDikeProfile(new Point2D(1.3, 1.3)),
                    HydraulicBoundaryLocation = new TestHydraulicBoundaryLocation()
                }
            };
            var calculationB = new GrassCoverErosionInwardsCalculationScenario(double.NaN)
            {
                InputParameters =
                {
                    DikeProfile = DikeProfileTestFactory.CreateDikeProfile(new Point2D(1.5, 1.5)),
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

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                new FailureMechanismSection("A", geometryPoints.Take(2)),
                new FailureMechanismSection("B", geometryPoints.Skip(1).Take(2)),
                new FailureMechanismSection("C", geometryPoints.Skip(2).Take(2))
            });

            failureMechanism.DikeProfiles.AddRange(new[]
            {
                DikeProfileTestFactory.CreateDikeProfile(new[]
                {
                    new Point2D(0, 0),
                    new Point2D(1, 1)
                }, "id1"),
                DikeProfileTestFactory.CreateDikeProfile(new[]
                {
                    new Point2D(2, 2),
                    new Point2D(3, 3)
                }, "id2")
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
            GrassCoverErosionInwardsFailureMechanismView view = CreateView(failureMechanism, assessmentSection);

            IMapControl map = ((RiskeerMapControl) view.Controls[0]).MapControl;

            // Assert
            MapDataCollection mapData = map.Data;
            Assert.IsInstanceOf<MapDataCollection>(mapData);

            List<MapData> mapDataList = mapData.Collection.ToList();
            Assert.AreEqual(5, mapDataList.Count);
            MapDataTestHelper.AssertReferenceLineMapData(assessmentSection.ReferenceLine, mapDataList[referenceLineIndex]);

            MapDataTestHelper.AssertHydraulicBoundaryLocationsMapData(assessmentSection, mapDataList[hydraulicBoundaryLocationsIndex]);
            AssertDikeProfiles(failureMechanism.DikeProfiles, mapDataList[dikeProfilesIndex]);
            MapDataTestHelper.AssertForeshoreProfilesMapData(failureMechanism.DikeProfiles.Select(dp => dp.ForeshoreProfile), mapDataList[foreshoreProfilesIndex]);
            AssertCalculationsMapData(failureMechanism.Calculations.Cast<GrassCoverErosionInwardsCalculation>(), mapDataList[calculationsIndex]);
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

            GrassCoverErosionInwardsFailureMechanismView view = CreateView(new GrassCoverErosionInwardsFailureMechanism(), assessmentSection);

            IMapControl map = ((RiskeerMapControl) view.Controls[0]).MapControl;

            MapData hydraulicBoundaryLocationsMapData = map.Data.Collection.ElementAt(hydraulicBoundaryLocationsIndex);

            var mocks = new MockRepository();
            IObserver[] observers = AttachMapDataObservers(mocks, map.Data.Collection);
            observers[hydraulicBoundaryLocationsObserverIndex].Expect(obs => obs.UpdateObserver());
            mocks.ReplayAll();

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

            GrassCoverErosionInwardsFailureMechanismView view = CreateView(new GrassCoverErosionInwardsFailureMechanism(), assessmentSection);

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
            mocks.VerifyAll();
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

            GrassCoverErosionInwardsFailureMechanismView view = CreateView(new GrassCoverErosionInwardsFailureMechanism(), assessmentSection);

            IMapControl map = ((RiskeerMapControl) view.Controls[0]).MapControl;

            var referenceLineMapData = (MapLineData) map.Data.Collection.ElementAt(referenceLineIndex);

            var mocks = new MockRepository();
            IObserver[] observers = AttachMapDataObservers(mocks, map.Data.Collection);
            observers[referenceLineIndex].Expect(obs => obs.UpdateObserver());
            mocks.ReplayAll();

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

            GrassCoverErosionInwardsFailureMechanismView view = CreateView(new GrassCoverErosionInwardsFailureMechanism(), assessmentSection);

            IMapControl map = ((RiskeerMapControl) view.Controls[0]).MapControl;

            MapData referenceLineMapData = map.Data.Collection.ElementAt(referenceLineIndex);

            var mocks = new MockRepository();
            IObserver[] observers = AttachMapDataObservers(mocks, map.Data.Collection);
            observers[referenceLineIndex].Expect(obs => obs.UpdateObserver());
            mocks.ReplayAll();

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
        public void GivenViewWithDikeProfilesData_WhenDikeProfilesUpdatedAndNotified_ThenMapDataUpdated()
        {
            // Given
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            failureMechanism.DikeProfiles.AddRange(new[]
            {
                DikeProfileTestFactory.CreateDikeProfile(string.Empty, "id1")
            }, "path");

            GrassCoverErosionInwardsFailureMechanismView view = CreateView(failureMechanism, new AssessmentSectionStub());

            IMapControl map = ((RiskeerMapControl) view.Controls[0]).MapControl;

            MapData dikeProfileData = map.Data.Collection.ElementAt(dikeProfilesIndex);
            var mocks = new MockRepository();
            IObserver[] observers = AttachMapDataObservers(mocks, map.Data.Collection);
            observers[dikeProfilesObserverIndex].Expect(obs => obs.UpdateObserver());
            observers[foreshoreProfileObserverIndex].Expect(obs => obs.UpdateObserver());
            mocks.ReplayAll();

            // Precondition
            AssertDikeProfiles(failureMechanism.DikeProfiles, dikeProfileData);

            // When
            failureMechanism.DikeProfiles.AddRange(new[]
            {
                DikeProfileTestFactory.CreateDikeProfile(string.Empty, "id2")
            }, "path");
            failureMechanism.DikeProfiles.NotifyObservers();

            // Then
            AssertDikeProfiles(failureMechanism.DikeProfiles, dikeProfileData);
            mocks.VerifyAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void GivenViewWithDikeProfileData_WhenDikeProfileUpdatedAndNotified_ThenMapDataUpdated()
        {
            // Given
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            DikeProfile dikeProfile = DikeProfileTestFactory.CreateDikeProfile(string.Empty, "id1");
            failureMechanism.DikeProfiles.AddRange(new[]
            {
                dikeProfile
            }, "path");

            GrassCoverErosionInwardsFailureMechanismView view = CreateView(failureMechanism, new AssessmentSectionStub());

            IMapControl map = ((RiskeerMapControl) view.Controls[0]).MapControl;

            MapData dikeProfileData = map.Data.Collection.ElementAt(dikeProfilesIndex);

            var mocks = new MockRepository();
            IObserver[] observers = AttachMapDataObservers(mocks, map.Data.Collection);
            observers[dikeProfilesObserverIndex].Expect(obs => obs.UpdateObserver());
            observers[foreshoreProfileObserverIndex].Expect(obs => obs.UpdateObserver());
            mocks.ReplayAll();

            // Precondition
            AssertDikeProfiles(failureMechanism.DikeProfiles, dikeProfileData);

            // When
            DikeProfile dikeProfileToUpdateFrom = DikeProfileTestFactory.CreateDikeProfile("A new name", "id1");
            dikeProfile.CopyProperties(dikeProfileToUpdateFrom);
            dikeProfile.NotifyObservers();

            // Then
            AssertDikeProfiles(failureMechanism.DikeProfiles, dikeProfileData);
            mocks.VerifyAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void GivenViewWithForeshoreProfileData_WhenForeshoreProfileUpdatedAndNotified_ThenMapDataUpdated()
        {
            // Given
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            DikeProfile dikeProfile = DikeProfileTestFactory.CreateDikeProfile(new[]
            {
                new Point2D(0, 0),
                new Point2D(1, 1)
            }, "id1");
            failureMechanism.DikeProfiles.AddRange(new[]
            {
                dikeProfile
            }, "path");

            GrassCoverErosionInwardsFailureMechanismView view = CreateView(failureMechanism, new AssessmentSectionStub());

            IMapControl map = ((RiskeerMapControl) view.Controls[0]).MapControl;

            MapData dikeProfileData = map.Data.Collection.ElementAt(foreshoreProfilesIndex);

            var mocks = new MockRepository();
            IObserver[] observers = AttachMapDataObservers(mocks, map.Data.Collection);
            observers[dikeProfilesObserverIndex].Expect(obs => obs.UpdateObserver());
            observers[foreshoreProfileObserverIndex].Expect(obs => obs.UpdateObserver());
            mocks.ReplayAll();

            // Precondition
            MapDataTestHelper.AssertForeshoreProfilesMapData(failureMechanism.DikeProfiles.Select(dp => dp.ForeshoreProfile), dikeProfileData);

            // When
            DikeProfile dikeProfileToUpdateFrom = DikeProfileTestFactory.CreateDikeProfile(new[]
            {
                new Point2D(2, 2),
                new Point2D(3, 3)
            }, "id1");
            dikeProfile.CopyProperties(dikeProfileToUpdateFrom);
            failureMechanism.DikeProfiles.NotifyObservers();

            // Then
            MapDataTestHelper.AssertForeshoreProfilesMapData(failureMechanism.DikeProfiles.Select(dp => dp.ForeshoreProfile), dikeProfileData);
            mocks.VerifyAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void GivenViewWithCalculationGroupData_WhenCalculationGroupUpdatedAndNotified_ThenMapDataUpdated()
        {
            // Given
            var calculationA = new GrassCoverErosionInwardsCalculation(double.NaN)
            {
                InputParameters =
                {
                    DikeProfile = DikeProfileTestFactory.CreateDikeProfile(new Point2D(1.3, 1.3)),
                    HydraulicBoundaryLocation = new TestHydraulicBoundaryLocation()
                }
            };
            var calculationB = new GrassCoverErosionInwardsCalculation(double.NaN)
            {
                InputParameters =
                {
                    DikeProfile = DikeProfileTestFactory.CreateDikeProfile(new Point2D(1.5, 1.5)),
                    HydraulicBoundaryLocation = new TestHydraulicBoundaryLocation()
                }
            };
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(calculationA);

            GrassCoverErosionInwardsFailureMechanismView view = CreateView(failureMechanism, new AssessmentSectionStub());

            IMapControl map = ((RiskeerMapControl) view.Controls[0]).MapControl;

            var calculationMapData = (MapLineData) map.Data.Collection.ElementAt(calculationsIndex);
            var mocks = new MockRepository();
            IObserver[] observers = AttachMapDataObservers(mocks, map.Data.Collection);
            observers[calculationObserverIndex].Expect(obs => obs.UpdateObserver());
            mocks.ReplayAll();

            // When
            failureMechanism.CalculationsGroup.Children.Add(calculationB);
            failureMechanism.CalculationsGroup.NotifyObservers();

            // Then
            AssertCalculationsMapData(failureMechanism.Calculations.Cast<GrassCoverErosionInwardsCalculation>(), calculationMapData);
            mocks.VerifyAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void GivenViewWithCalculationInputData_WhenCalculationInputUpdatedAndNotified_ThenMapDataUpdated()
        {
            // Given
            var calculationA = new GrassCoverErosionInwardsCalculation(double.NaN)
            {
                InputParameters =
                {
                    DikeProfile = DikeProfileTestFactory.CreateDikeProfile(new Point2D(1.3, 1.3)),
                    HydraulicBoundaryLocation = new TestHydraulicBoundaryLocation()
                }
            };

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(calculationA);

            GrassCoverErosionInwardsFailureMechanismView view = CreateView(failureMechanism, new AssessmentSectionStub());

            IMapControl map = ((RiskeerMapControl) view.Controls[0]).MapControl;

            var calculationMapData = (MapLineData) map.Data.Collection.ElementAt(calculationsIndex);

            var mocks = new MockRepository();
            IObserver[] observers = AttachMapDataObservers(mocks, map.Data.Collection);
            observers[calculationObserverIndex].Expect(obs => obs.UpdateObserver());
            mocks.ReplayAll();

            // When
            calculationA.InputParameters.DikeProfile = DikeProfileTestFactory.CreateDikeProfile(new Point2D(1.5, 1.5));
            calculationA.InputParameters.NotifyObservers();

            // Then
            AssertCalculationsMapData(failureMechanism.Calculations.Cast<GrassCoverErosionInwardsCalculation>(), calculationMapData);
            mocks.VerifyAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void GivenViewWithCalculationData_WhenCalculationUpdatedAndNotified_ThenMapDataUpdated()
        {
            // Given
            var calculationA = new GrassCoverErosionInwardsCalculation(double.NaN)
            {
                InputParameters =
                {
                    DikeProfile = DikeProfileTestFactory.CreateDikeProfile(new Point2D(1.3, 1.3)),
                    HydraulicBoundaryLocation = new TestHydraulicBoundaryLocation()
                }
            };

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(calculationA);

            GrassCoverErosionInwardsFailureMechanismView view = CreateView(failureMechanism, new AssessmentSectionStub());

            IMapControl map = ((RiskeerMapControl) view.Controls[0]).MapControl;

            var calculationMapData = (MapLineData) map.Data.Collection.ElementAt(calculationsIndex);

            var mocks = new MockRepository();
            IObserver[] observers = AttachMapDataObservers(mocks, map.Data.Collection);
            observers[calculationObserverIndex].Expect(obs => obs.UpdateObserver());
            mocks.ReplayAll();

            // When
            calculationA.Name = "new name";
            calculationA.NotifyObservers();

            // Then 
            AssertCalculationsMapData(failureMechanism.Calculations.Cast<GrassCoverErosionInwardsCalculation>(), calculationMapData);
            mocks.VerifyAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void NotifyObservers_DataUpdated_MapLayersSameOrder()
        {
            // Setup
            const int updatedReferenceLineLayerIndex = referenceLineIndex + 4;
            const int updatedHydraulicLocationsLayerIndex = hydraulicBoundaryLocationsIndex - 1;
            const int updatedDikeProfilesLayerIndex = dikeProfilesIndex - 1;
            const int updatedForeshoreProfilesLayerIndex = foreshoreProfilesIndex - 1;
            const int updatedCalculationsIndex = calculationsIndex - 1;

            var assessmentSection = new AssessmentSectionStub();

            GrassCoverErosionInwardsFailureMechanismView view = CreateView(new GrassCoverErosionInwardsFailureMechanism(), assessmentSection);

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

            var dikeProfilesData = (MapLineData) mapDataList[updatedDikeProfilesLayerIndex];
            Assert.AreEqual("Dijkprofielen", dikeProfilesData.Name);

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

            var actualDikeProfilesData = (MapLineData) mapDataList[updatedDikeProfilesLayerIndex];
            Assert.AreEqual("Dijkprofielen", actualDikeProfilesData.Name);

            var actualForeshoreProfilesData = (MapLineData) mapDataList[updatedForeshoreProfilesLayerIndex];
            Assert.AreEqual("Voorlandprofielen", actualForeshoreProfilesData.Name);

            var actualCalculationsData = (MapLineData) mapDataList[updatedCalculationsIndex];
            Assert.AreEqual("Berekeningen", actualCalculationsData.Name);
        }

        private GrassCoverErosionInwardsFailureMechanismView CreateView(GrassCoverErosionInwardsFailureMechanism failureMechanism, IAssessmentSection assessmentSection)
        {
            var view = new GrassCoverErosionInwardsFailureMechanismView(failureMechanism, assessmentSection);

            testForm.Controls.Add(view);
            testForm.Show();

            return view;
        }

        private static void AssertDikeProfiles(IEnumerable<DikeProfile> dikeProfiles, MapData mapData)
        {
            Assert.NotNull(dikeProfiles, "dikeProfiles should never be null.");

            var dikeProfilesData = (MapLineData) mapData;
            DikeProfile[] dikeProfileArray = dikeProfiles.ToArray();

            Assert.IsInstanceOf<MapLineData>(mapData);
            int dikeProfileCount = dikeProfileArray.Length;
            Assert.AreEqual(dikeProfileCount, dikeProfilesData.Features.Count());

            for (var i = 0; i < dikeProfileCount; i++)
            {
                MapGeometry profileDataA = dikeProfilesData.Features.ElementAt(i).MapGeometries.First();
                CollectionAssert.AreEquivalent(dikeProfileArray[0].DikeGeometry, profileDataA.PointCollections.First());
            }

            Assert.AreEqual("Dijkprofielen", mapData.Name);
        }

        private static void AssertCalculationsMapData(IEnumerable<GrassCoverErosionInwardsCalculation> calculations, MapData mapData)
        {
            Assert.IsInstanceOf<MapLineData>(mapData);
            var calculationsMapData = (MapLineData) mapData;
            GrassCoverErosionInwardsCalculation[] calculationsArray = calculations.ToArray();
            MapFeature[] calculationsFeatures = calculationsMapData.Features.ToArray();
            int calculationsCount = calculationsArray.Length;
            Assert.AreEqual(calculationsCount, calculationsFeatures.Length);

            for (var index = 0; index < calculationsCount; index++)
            {
                MapGeometry[] geometries = calculationsFeatures[index].MapGeometries.ToArray();
                Assert.AreEqual(1, geometries.Length);

                GrassCoverErosionInwardsCalculation calculation = calculationsArray[index];
                CollectionAssert.AreEquivalent(new[]
                                               {
                                                   calculation.InputParameters.DikeProfile.WorldReferencePoint,
                                                   calculation.InputParameters.HydraulicBoundaryLocation.Location
                                               },
                                               geometries[0].PointCollections.First());
            }

            Assert.AreEqual("Berekeningen", mapData.Name);
        }

        private static void AssertEmptyMapData(MapDataCollection mapDataCollection)
        {
            Assert.AreEqual("Dijken en dammen - Grasbekleding erosie kruin en binnentalud", mapDataCollection.Name);

            List<MapData> mapDataList = mapDataCollection.Collection.ToList();

            Assert.AreEqual(5, mapDataList.Count);

            var referenceLineMapData = (MapLineData) mapDataList[referenceLineIndex];
            var dikeProfilesMapData = (MapLineData) mapDataList[dikeProfilesIndex];
            var foreshoreProfilesMapData = (MapLineData) mapDataList[foreshoreProfilesIndex];
            var hydraulicBoundaryLocationsMapData = (MapPointData) mapDataList[hydraulicBoundaryLocationsIndex];
            var calculationsMapData = (MapLineData) mapDataList[calculationsIndex];

            CollectionAssert.IsEmpty(referenceLineMapData.Features);
            CollectionAssert.IsEmpty(dikeProfilesMapData.Features);
            CollectionAssert.IsEmpty(foreshoreProfilesMapData.Features);
            CollectionAssert.IsEmpty(hydraulicBoundaryLocationsMapData.Features);
            CollectionAssert.IsEmpty(calculationsMapData.Features);

            Assert.AreEqual("Referentielijn", referenceLineMapData.Name);
            Assert.AreEqual("Dijkprofielen", dikeProfilesMapData.Name);
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

            var dikeProfilesObserver = mocks.StrictMock<IObserver>();
            mapDataArray[dikeProfilesIndex].Attach(dikeProfilesObserver);

            var foreshoreProfilesObserver = mocks.StrictMock<IObserver>();
            mapDataArray[foreshoreProfilesIndex].Attach(foreshoreProfilesObserver);

            var calculationsMapDataObserver = mocks.StrictMock<IObserver>();
            mapDataArray[calculationsIndex].Attach(calculationsMapDataObserver);

            return new[]
            {
                referenceLineMapDataObserver,
                hydraulicBoundaryLocationsMapDataObserver,
                dikeProfilesObserver,
                foreshoreProfilesObserver,
                calculationsMapDataObserver
            };
        }
    }
}