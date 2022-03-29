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
using Riskeer.AssemblyTool.KernelWrapper.Calculators;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil.Calculators;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil.Calculators.Assembly;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.TestUtil;
using Riskeer.Common.Forms.Views;
using Riskeer.WaveImpactAsphaltCover.Data;
using Riskeer.WaveImpactAsphaltCover.Forms.Views.RegistrationState;

namespace Riskeer.WaveImpactAsphaltCover.Forms.Test.Views.RegistrationState
{
    [TestFixture]
    public class WaveImpactAsphaltCoverFailureMechanismViewTest
    {
        private const int referenceLineIndex = 0;
        private const int sectionsCollectionIndex = 1;
        private const int assemblyResultsIndex = 2;
        private const int hydraulicBoundaryLocationsIndex = 3;
        private const int foreshoreProfilesIndex = 4;
        private const int calculationsIndex = 5;

        private const int sectionsIndex = 0;
        private const int sectionsStartPointIndex = 1;
        private const int sectionsEndPointIndex = 2;

        private const int sectionsObserverIndex = 0;
        private const int sectionsStartPointObserverIndex = 1;
        private const int sectionsEndPointObserverIndex = 2;

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
        public void Constructor_ExpectedValues()
        {
            // Setup
            var failureMechanism = new WaveImpactAsphaltCoverFailureMechanism();
            var assessmentSection = new AssessmentSectionStub();

            // Call
            WaveImpactAsphaltCoverFailureMechanismView view = CreateView(failureMechanism, assessmentSection);

            // Assert
            Assert.IsInstanceOf<Forms.Views.HydraulicLoadsState.WaveImpactAsphaltCoverFailureMechanismView>(view);
            Assert.IsNull(view.Data);
            Assert.AreSame(failureMechanism, view.FailureMechanism);
            Assert.AreSame(assessmentSection, view.AssessmentSection);
            AssertEmptyMapData(view.Map.Data);
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void Constructor_WithAllData_DataUpdatedToCollectionOfFilledMapData()
        {
            // Setup
            var calculationA = new WaveImpactAsphaltCoverWaveConditionsCalculation
            {
                InputParameters =
                {
                    ForeshoreProfile = new TestForeshoreProfile(new Point2D(1.3, 1.3)),
                    HydraulicBoundaryLocation = new TestHydraulicBoundaryLocation()
                }
            };
            var calculationB = new WaveImpactAsphaltCoverWaveConditionsCalculation
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

            var failureMechanism = new WaveImpactAsphaltCoverFailureMechanism();
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

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // Call
                WaveImpactAsphaltCoverFailureMechanismView view = CreateView(failureMechanism, assessmentSection);

                IMapControl map = ((RiskeerMapControl) view.Controls[0]).MapControl;

                // Assert
                MapDataCollection mapData = map.Data;
                Assert.IsInstanceOf<MapDataCollection>(mapData);

                List<MapData> mapDataList = mapData.Collection.ToList();
                Assert.AreEqual(6, mapDataList.Count);
                MapDataTestHelper.AssertReferenceLineMapData(assessmentSection.ReferenceLine, mapDataList[referenceLineIndex]);

                IEnumerable<MapData> sectionsCollection = ((MapDataCollection) mapDataList[sectionsCollectionIndex]).Collection;
                MapDataTestHelper.AssertFailureMechanismSectionsMapData(failureMechanism.Sections, sectionsCollection.ElementAt(sectionsIndex));
                MapDataTestHelper.AssertFailureMechanismSectionsStartPointMapData(failureMechanism.Sections, sectionsCollection.ElementAt(sectionsStartPointIndex));
                MapDataTestHelper.AssertFailureMechanismSectionsEndPointMapData(failureMechanism.Sections, sectionsCollection.ElementAt(sectionsEndPointIndex));

                MapDataTestHelper.AssertHydraulicBoundaryLocationsMapData(assessmentSection, mapDataList[hydraulicBoundaryLocationsIndex]);
                MapDataTestHelper.AssertForeshoreProfilesMapData(failureMechanism.ForeshoreProfiles, mapDataList[foreshoreProfilesIndex]);
                AssertCalculationsMapData(failureMechanism.Calculations.Cast<WaveImpactAsphaltCoverWaveConditionsCalculation>(), mapDataList[calculationsIndex]);

                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;
                MapDataTestHelper.AssertAssemblyMapData(failureMechanism, calculator.FailureMechanismSectionAssemblyResultOutput, mapDataList[assemblyResultsIndex]);
            }
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void GivenViewWithFailureMechanismSectionsData_WhenFailureMechanismUpdatedAndNotified_ThenMapDataUpdated()
        {
            // Given
            var failureMechanism = new WaveImpactAsphaltCoverFailureMechanism();

            WaveImpactAsphaltCoverFailureMechanismView view = CreateView(failureMechanism, new AssessmentSectionStub());

            IMapControl map = ((RiskeerMapControl) view.Controls[0]).MapControl;

            IEnumerable<MapData> sectionsCollection = ((MapDataCollection) map.Data.Collection.ElementAt(sectionsCollectionIndex)).Collection;
            var sectionMapData = (MapLineData) sectionsCollection.ElementAt(sectionsIndex);
            var sectionStartsMapData = (MapPointData) sectionsCollection.ElementAt(sectionsStartPointIndex);
            var sectionsEndsMapData = (MapPointData) sectionsCollection.ElementAt(sectionsEndPointIndex);

            var mocks = new MockRepository();
            IObserver[] observers = AttachMapDataObservers(mocks, map.Data.Collection);
            observers[sectionsObserverIndex].Expect(obs => obs.UpdateObserver());
            observers[sectionsStartPointObserverIndex].Expect(obs => obs.UpdateObserver());
            observers[sectionsEndPointObserverIndex].Expect(obs => obs.UpdateObserver());
            mocks.ReplayAll();

            // When
            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                new FailureMechanismSection(string.Empty, new[]
                {
                    new Point2D(1, 2),
                    new Point2D(1, 2)
                })
            });
            failureMechanism.NotifyObservers();

            // Then
            MapDataTestHelper.AssertFailureMechanismSectionsMapData(failureMechanism.Sections, sectionMapData);
            MapDataTestHelper.AssertFailureMechanismSectionsStartPointMapData(failureMechanism.Sections, sectionStartsMapData);
            MapDataTestHelper.AssertFailureMechanismSectionsEndPointMapData(failureMechanism.Sections, sectionsEndsMapData);
            mocks.VerifyAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void UpdateObserver_DataUpdated_MapLayersSameOrder()
        {
            // Setup
            const int updatedReferenceLineLayerIndex = referenceLineIndex + 5;
            const int updatedSectionsCollectionLayerIndex = sectionsCollectionIndex - 1;
            const int updatedAssemblyResultsIndex = assemblyResultsIndex - 1;
            const int updatedHydraulicLocationsLayerIndex = hydraulicBoundaryLocationsIndex - 1;
            const int updatedForeshoreProfilesLayerIndex = foreshoreProfilesIndex - 1;
            const int updatedCalculationsIndex = calculationsIndex - 1;

            var assessmentSection = new AssessmentSectionStub();
            var failureMechanism = new WaveImpactAsphaltCoverFailureMechanism();

            WaveImpactAsphaltCoverFailureMechanismView view = CreateView(failureMechanism, assessmentSection);

            IMapControl map = ((RiskeerMapControl) view.Controls[0]).MapControl;

            MapDataCollection mapData = map.Data;

            var dataToMove = (MapLineData) map.Data.Collection.ElementAt(referenceLineIndex);
            mapData.Remove(dataToMove);
            mapData.Add(dataToMove);

            IEnumerable<MapData> mapDataCollection = mapData.Collection;

            // Precondition
            var referenceLineData = (MapLineData) mapDataCollection.ElementAt(updatedReferenceLineLayerIndex);
            Assert.AreEqual("Referentielijn", referenceLineData.Name);

            var sectionsData = (MapDataCollection) mapDataCollection.ElementAt(updatedSectionsCollectionLayerIndex);
            Assert.AreEqual("Vakindeling", sectionsData.Name);

            var assemblyResultsData = (MapLineData) mapDataCollection.ElementAt(updatedAssemblyResultsIndex);
            Assert.AreEqual("Duidingsklasse per vak", assemblyResultsData.Name);

            var hydraulicLocationsData = (MapPointData) mapDataCollection.ElementAt(updatedHydraulicLocationsLayerIndex);
            Assert.AreEqual("Hydraulische belastingen", hydraulicLocationsData.Name);

            var foreshoreProfilesData = (MapLineData) mapDataCollection.ElementAt(updatedForeshoreProfilesLayerIndex);
            Assert.AreEqual("Voorlandprofielen", foreshoreProfilesData.Name);

            var calculationsData = (MapLineData) mapDataCollection.ElementAt(updatedCalculationsIndex);
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
            var actualReferenceLineData = (MapLineData) mapDataCollection.ElementAt(updatedReferenceLineLayerIndex);
            Assert.AreEqual("Referentielijn", actualReferenceLineData.Name);

            var actualSectionsData = (MapDataCollection) mapDataCollection.ElementAt(updatedSectionsCollectionLayerIndex);
            Assert.AreEqual("Vakindeling", actualSectionsData.Name);

            var actualAssemblyResultsData = (MapLineData) mapDataCollection.ElementAt(updatedAssemblyResultsIndex);
            Assert.AreEqual("Duidingsklasse per vak", actualAssemblyResultsData.Name);

            var actualHydraulicLocationsData = (MapPointData) mapDataCollection.ElementAt(updatedHydraulicLocationsLayerIndex);
            Assert.AreEqual("Hydraulische belastingen", actualHydraulicLocationsData.Name);

            var actualForeshoreProfilesData = (MapLineData) mapDataCollection.ElementAt(updatedForeshoreProfilesLayerIndex);
            Assert.AreEqual("Voorlandprofielen", actualForeshoreProfilesData.Name);

            var actualCalculationsData = (MapLineData) mapDataCollection.ElementAt(updatedCalculationsIndex);
            Assert.AreEqual("Berekeningen", actualCalculationsData.Name);
        }

        private WaveImpactAsphaltCoverFailureMechanismView CreateView(WaveImpactAsphaltCoverFailureMechanism failureMechanism, IAssessmentSection assessmentSection)
        {
            var view = new WaveImpactAsphaltCoverFailureMechanismView(failureMechanism, assessmentSection);

            testForm.Controls.Add(view);
            testForm.Show();

            return view;
        }

        private static void AssertCalculationsMapData(IEnumerable<WaveImpactAsphaltCoverWaveConditionsCalculation> calculations, MapData mapData)
        {
            Assert.IsInstanceOf<MapLineData>(mapData);
            var calculationsMapData = (MapLineData) mapData;
            WaveImpactAsphaltCoverWaveConditionsCalculation[] calculationsArray = calculations.ToArray();
            MapFeature[] calculationsFeatures = calculationsMapData.Features.ToArray();
            Assert.AreEqual(calculationsArray.Length, calculationsFeatures.Length);

            for (var index = 0; index < calculationsArray.Length; index++)
            {
                MapGeometry[] geometries = calculationsFeatures[index].MapGeometries.ToArray();
                Assert.AreEqual(1, geometries.Length);

                WaveImpactAsphaltCoverWaveConditionsCalculation calculation = calculationsArray[index];
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
            Assert.AreEqual("Golfklappen op asfaltbekleding", mapDataCollection.Name);

            List<MapData> mapDataList = mapDataCollection.Collection.ToList();

            Assert.AreEqual(6, mapDataList.Count);

            var referenceLineMapData = (MapLineData) mapDataList[referenceLineIndex];
            var assemblyResultsMapData = (MapLineData) mapDataList[assemblyResultsIndex];
            var foreshoreProfilesMapData = (MapLineData) mapDataList[foreshoreProfilesIndex];
            var hydraulicBoundaryLocationsMapData = (MapPointData) mapDataList[hydraulicBoundaryLocationsIndex];
            var calculationsMapData = (MapLineData) mapDataList[calculationsIndex];

            CollectionAssert.IsEmpty(referenceLineMapData.Features);
            CollectionAssert.IsEmpty(assemblyResultsMapData.Features);
            CollectionAssert.IsEmpty(foreshoreProfilesMapData.Features);
            CollectionAssert.IsEmpty(hydraulicBoundaryLocationsMapData.Features);
            CollectionAssert.IsEmpty(calculationsMapData.Features);

            Assert.AreEqual("Referentielijn", referenceLineMapData.Name);
            Assert.AreEqual("Duidingsklasse per vak", assemblyResultsMapData.Name);
            Assert.AreEqual("Voorlandprofielen", foreshoreProfilesMapData.Name);
            Assert.AreEqual("Hydraulische belastingen", hydraulicBoundaryLocationsMapData.Name);
            Assert.AreEqual("Berekeningen", calculationsMapData.Name);

            var sectionsMapDataCollection = (MapDataCollection) mapDataList[sectionsCollectionIndex];
            Assert.AreEqual("Vakindeling", sectionsMapDataCollection.Name);
            List<MapData> sectionsDataList = sectionsMapDataCollection.Collection.ToList();
            Assert.AreEqual(3, sectionsDataList.Count);

            var sectionsMapData = (MapLineData) sectionsDataList[sectionsIndex];
            var sectionsStartPointMapData = (MapPointData) sectionsDataList[sectionsStartPointIndex];
            var sectionsEndPointMapData = (MapPointData) sectionsDataList[sectionsEndPointIndex];

            CollectionAssert.IsEmpty(sectionsEndPointMapData.Features);
            CollectionAssert.IsEmpty(sectionsStartPointMapData.Features);
            CollectionAssert.IsEmpty(sectionsMapData.Features);

            Assert.AreEqual("Vakindeling (eindpunten)", sectionsEndPointMapData.Name);
            Assert.AreEqual("Vakindeling (startpunten)", sectionsStartPointMapData.Name);
            Assert.AreEqual("Vakindeling", sectionsMapData.Name);
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

            MapData[] sectionsCollection = ((MapDataCollection) mapDataArray[sectionsCollectionIndex]).Collection.ToArray();
            var sectionsMapDataObserver = mocks.StrictMock<IObserver>();
            sectionsCollection[sectionsIndex].Attach(sectionsMapDataObserver);

            var sectionsStartPointMapDataObserver = mocks.StrictMock<IObserver>();
            sectionsCollection[sectionsStartPointIndex].Attach(sectionsStartPointMapDataObserver);

            var sectionsEndPointMapDataObserver = mocks.StrictMock<IObserver>();
            sectionsCollection[sectionsEndPointIndex].Attach(sectionsEndPointMapDataObserver);

            return new[]
            {
                sectionsMapDataObserver,
                sectionsStartPointMapDataObserver,
                sectionsEndPointMapDataObserver
            };
        }
    }
}