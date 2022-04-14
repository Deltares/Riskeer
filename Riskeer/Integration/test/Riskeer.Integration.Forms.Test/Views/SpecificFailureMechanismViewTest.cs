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
using Core.Components.Gis.Forms;
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
using Riskeer.Integration.Forms.Views;

namespace Riskeer.Integration.Forms.Test.Views
{
    [TestFixture]
    public class SpecificFailureMechanismViewTest
    {
        private const int referenceLineIndex = 0;
        private const int sectionsCollectionIndex = 1;
        private const int assemblyResultsIndex = 2;
        private const int hydraulicBoundaryLocationsIndex = 3;

        private const int sectionsIndex = 0;
        private const int sectionsStartPointIndex = 1;
        private const int sectionsEndPointIndex = 2;

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
        public void Constructor_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new SpecificFailureMechanismView(new SpecificFailureMechanism(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void Constructor_WithArguments_ExpectedValues()
        {
            // Setup
            var assessmentSection = new AssessmentSectionStub();
            var failureMechanism = new SpecificFailureMechanism();

            // Call
            SpecificFailureMechanismView view = CreateView(failureMechanism, assessmentSection);

            // Assert
            Assert.IsInstanceOf<CloseForFailureMechanismView>(view);
            Assert.IsInstanceOf<IMapView>(view);
            Assert.IsNull(view.Data);
            Assert.AreSame(failureMechanism, view.FailureMechanism);

            Assert.AreEqual(1, view.Controls.Count);
            Assert.IsInstanceOf<RiskeerMapControl>(view.Controls[0]);
            Assert.AreSame(view.Map, ((RiskeerMapControl) view.Controls[0]).MapControl);
            Assert.AreEqual(DockStyle.Fill, ((Control) view.Map).Dock);
            AssertEmptyMapData(failureMechanism, view.Map.Data);
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void Constructor_WithAllData_DataUpdatedToCollectionOfFilledMapData()
        {
            // Setup
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

            var geometryPoints = new[]
            {
                new Point2D(0.0, 0.0),
                new Point2D(2.0, 0.0),
                new Point2D(4.0, 4.0),
                new Point2D(6.0, 4.0)
            };
            var failureMechanism = new SpecificFailureMechanism();
            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                new FailureMechanismSection("A", geometryPoints.Take(2)),
                new FailureMechanismSection("B", geometryPoints.Skip(1).Take(2)),
                new FailureMechanismSection("C", geometryPoints.Skip(2).Take(2))
            });

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // Call
                SpecificFailureMechanismView view = CreateView(failureMechanism, assessmentSection);

                // Assert
                MapDataCollection mapData = view.Map.Data;
                List<MapData> mapDataList = mapData.Collection.ToList();
                Assert.AreEqual(4, mapDataList.Count);

                MapDataTestHelper.AssertReferenceLineMapData(assessmentSection.ReferenceLine, mapDataList[referenceLineIndex]);

                IEnumerable<MapData> sectionsCollection = ((MapDataCollection) mapDataList[sectionsCollectionIndex]).Collection;
                MapDataTestHelper.AssertFailureMechanismSectionsMapData(failureMechanism.Sections, sectionsCollection.ElementAt(sectionsIndex));
                MapDataTestHelper.AssertFailureMechanismSectionsStartPointMapData(failureMechanism.Sections, sectionsCollection.ElementAt(sectionsStartPointIndex));
                MapDataTestHelper.AssertFailureMechanismSectionsEndPointMapData(failureMechanism.Sections, sectionsCollection.ElementAt(sectionsEndPointIndex));

                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;
                MapDataTestHelper.AssertAssemblyMapData(failureMechanism, calculator.FailureMechanismSectionAssemblyResultOutput.AssemblyResult, mapDataList[assemblyResultsIndex]);

                MapDataTestHelper.AssertHydraulicBoundaryLocationsMapData(assessmentSection, mapDataList[hydraulicBoundaryLocationsIndex]);
            }
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void GivenViewWithSpecificFailureMechanismData_WhenFailureMechanismNameUpdatedAndNotified_ThenMapDataUpdatedAndObserversNotified()
        {
            // Given
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(obs => obs.UpdateObserver());
            mocks.ReplayAll();

            var assessmentSection = new AssessmentSectionStub();
            var failureMechanism = new SpecificFailureMechanism();

            SpecificFailureMechanismView view = CreateView(failureMechanism, assessmentSection);
            MapDataCollection mapData = view.Map.Data;
            mapData.Attach(observer);

            // Precondition
            Assert.AreEqual(failureMechanism.Name, mapData.Name);

            // When
            const string newFailureMechanismName = "New Failure Mechanism Name";
            failureMechanism.Name = newFailureMechanismName;
            failureMechanism.NotifyObservers();

            // Then
            Assert.AreEqual(newFailureMechanismName, mapData.Name);
            mocks.VerifyAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void GivenViewWithReferenceLineData_WhenReferenceLineUpdatedAndNotified_ThenMapDataUpdatedAndObserversNotified()
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

            var failureMechanism = new SpecificFailureMechanism();
            SpecificFailureMechanismView view = CreateView(failureMechanism, assessmentSection);

            IEnumerable<MapData> mapDataCollection = view.Map.Data.Collection;
            MapData referenceLineMapData = mapDataCollection.ElementAt(referenceLineIndex);

            var mocks = new MockRepository();
            IObserver[] observers = AttachMapDataObservers(mocks, mapDataCollection);
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
        public void GivenViewWithFailureMechanismSectionData_WhenFailureMechanismsUpdatedAndNotified_ThenMapDataUpdatedAndObserversNotified()
        {
            // Given
            var assessmentSection = new AssessmentSectionStub();
            var failureMechanism = new SpecificFailureMechanism();

            SpecificFailureMechanismView view = CreateView(failureMechanism, assessmentSection);

            IEnumerable<MapData> mapDataCollection = view.Map.Data.Collection;
            IEnumerable<MapData> sectionsCollection = ((MapDataCollection) mapDataCollection.ElementAt(sectionsCollectionIndex)).Collection;
            var sectionMapData = (MapLineData) sectionsCollection.ElementAt(sectionsIndex);
            var sectionStartsMapData = (MapPointData) sectionsCollection.ElementAt(sectionsStartPointIndex);
            var sectionsEndsMapData = (MapPointData) sectionsCollection.ElementAt(sectionsEndPointIndex);

            var mocks = new MockRepository();
            IObserver[] observers = AttachMapDataObservers(mocks, mapDataCollection);
            observers[sectionsCollectionIndex + sectionsIndex].Expect(obs => obs.UpdateObserver());
            observers[sectionsCollectionIndex + sectionsStartPointIndex].Expect(obs => obs.UpdateObserver());
            observers[sectionsCollectionIndex + sectionsEndPointIndex].Expect(obs => obs.UpdateObserver());
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
            // Given
            const int updatedReferenceLineLayerIndex = referenceLineIndex + hydraulicBoundaryLocationsIndex;
            const int updatedSectionsCollectionLayerIndex = sectionsCollectionIndex - 1;
            const int updatedHydraulicBoundaryLocationsLayerIndex = hydraulicBoundaryLocationsIndex - 1;
            const int updatedAssemblyResultsIndex = assemblyResultsIndex - 1;

            var assessmentSection = new AssessmentSectionStub();
            var failureMechanism = new SpecificFailureMechanism();

            SpecificFailureMechanismView view = CreateView(failureMechanism, assessmentSection);

            MapDataCollection mapData = view.Map.Data;
            IEnumerable<MapData> mapDataCollection = mapData.Collection;

            MapData dataToMove = mapDataCollection.ElementAt(referenceLineIndex);
            mapData.Remove(dataToMove);
            mapData.Add(dataToMove);

            // Precondition
            MapData referenceLineData = mapDataCollection.ElementAt(updatedReferenceLineLayerIndex);
            Assert.AreEqual("Referentielijn", referenceLineData.Name);

            MapData sectionsData = mapDataCollection.ElementAt(updatedSectionsCollectionLayerIndex);
            Assert.AreEqual("Vakindeling", sectionsData.Name);

            var assemblyResultsData = (MapLineData) mapDataCollection.ElementAt(updatedAssemblyResultsIndex);
            Assert.AreEqual("Duidingsklasse per vak", assemblyResultsData.Name);

            MapData hydraulicBoundaryLocationsData = mapDataCollection.ElementAt(updatedHydraulicBoundaryLocationsLayerIndex);
            Assert.AreEqual("Hydraulische belastingen", hydraulicBoundaryLocationsData.Name);

            // When
            var points = new List<Point2D>
            {
                new Point2D(2.0, 5.0),
                new Point2D(4.0, 3.0)
            };
            var referenceLine = new ReferenceLine();
            referenceLine.SetGeometry(points);
            assessmentSection.ReferenceLine = referenceLine;
            assessmentSection.NotifyObservers();

            // Then
            MapData actualReferenceLineData = mapDataCollection.ElementAt(updatedReferenceLineLayerIndex);
            Assert.AreEqual("Referentielijn", actualReferenceLineData.Name);

            MapData actualSectionsData = mapDataCollection.ElementAt(updatedSectionsCollectionLayerIndex);
            Assert.AreEqual("Vakindeling", actualSectionsData.Name);

            var actualAssemblyResultsData = (MapLineData) mapDataCollection.ElementAt(updatedAssemblyResultsIndex);
            Assert.AreEqual("Duidingsklasse per vak", actualAssemblyResultsData.Name);

            MapData actualHydraulicBoundaryLocationsData = mapDataCollection.ElementAt(updatedHydraulicBoundaryLocationsLayerIndex);
            Assert.AreEqual("Hydraulische belastingen", actualHydraulicBoundaryLocationsData.Name);
        }

        private SpecificFailureMechanismView CreateView(SpecificFailureMechanism failureMechanism, IAssessmentSection assessmentSection)
        {
            var view = new SpecificFailureMechanismView(failureMechanism, assessmentSection);

            testForm.Controls.Add(view);
            testForm.Show();

            return view;
        }

        private static void AssertEmptyMapData(SpecificFailureMechanism failureMechanism, MapDataCollection mapDataCollection)
        {
            Assert.AreEqual(failureMechanism.Name, mapDataCollection.Name);

            List<MapData> mapDataList = mapDataCollection.Collection.ToList();

            Assert.AreEqual(4, mapDataList.Count);

            var referenceLineMapData = (MapLineData) mapDataList[referenceLineIndex];
            Assert.AreEqual("Referentielijn", referenceLineMapData.Name);
            CollectionAssert.IsEmpty(referenceLineMapData.Features);

            var sectionsMapDataCollection = (MapDataCollection) mapDataList[sectionsCollectionIndex];
            Assert.AreEqual("Vakindeling", sectionsMapDataCollection.Name);
            List<MapData> sectionsDataList = sectionsMapDataCollection.Collection.ToList();
            Assert.AreEqual(3, sectionsDataList.Count);
            var sectionsMapData = (MapLineData) sectionsDataList[sectionsIndex];
            var sectionsStartPointMapData = (MapPointData) sectionsDataList[sectionsStartPointIndex];
            var sectionsEndPointMapData = (MapPointData) sectionsDataList[sectionsEndPointIndex];

            var assemblyResultsMapData = (MapLineData) mapDataList[assemblyResultsIndex];
            Assert.AreEqual("Duidingsklasse per vak", assemblyResultsMapData.Name);

            var hydraulicBoundaryLocationsMapData = (MapPointData) mapDataList[hydraulicBoundaryLocationsIndex];
            Assert.AreEqual("Hydraulische belastingen", hydraulicBoundaryLocationsMapData.Name);
            CollectionAssert.IsEmpty(hydraulicBoundaryLocationsMapData.Features);

            CollectionAssert.IsEmpty(sectionsMapData.Features);
            CollectionAssert.IsEmpty(sectionsStartPointMapData.Features);
            CollectionAssert.IsEmpty(sectionsEndPointMapData.Features);
            CollectionAssert.IsEmpty(assemblyResultsMapData.Features);
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

            MapData[] sectionsCollection = ((MapDataCollection) mapDataArray[sectionsCollectionIndex]).Collection.ToArray();
            var sectionsMapDataObserver = mocks.StrictMock<IObserver>();
            sectionsCollection[sectionsIndex].Attach(sectionsMapDataObserver);

            var sectionsStartPointMapDataObserver = mocks.StrictMock<IObserver>();
            sectionsCollection[sectionsStartPointIndex].Attach(sectionsStartPointMapDataObserver);

            var sectionsEndPointMapDataObserver = mocks.StrictMock<IObserver>();
            sectionsCollection[sectionsEndPointIndex].Attach(sectionsEndPointMapDataObserver);

            return new[]
            {
                referenceLineMapDataObserver,
                sectionsMapDataObserver,
                sectionsStartPointMapDataObserver,
                sectionsEndPointMapDataObserver
            };
        }
    }
}