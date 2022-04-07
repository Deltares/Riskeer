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
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using Core.Components.Gis.Data;
using Core.Components.Gis.Features;
using Core.Components.Gis.Forms;
using Core.Components.Gis.Geometries;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.AssemblyTool.Data;
using Riskeer.AssemblyTool.KernelWrapper.Calculators;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil.Calculators;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil.Calculators.Assembly;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Forms.Helpers;
using Riskeer.Common.Forms.TestUtil;
using Riskeer.Common.Forms.Views;
using Riskeer.Integration.Data;
using Riskeer.Integration.Data.Assembly;
using Riskeer.Integration.Data.TestUtil;
using Riskeer.Integration.Forms.Views;
using Riskeer.Integration.Util;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.Integration.Forms.Test.Views
{
    [TestFixture]
    public class AssemblyResultPerSectionMapViewTest
    {
        private const int referenceLineIndex = 0;
        private const int hydraulicBoundaryLocationsIndex = 1;
        private const int assemblyResultsIndex = 2;

        private const int assemblyResultsObserverIndex = 1;

        [Test]
        public void Constructor_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new AssemblyResultPerSectionMapView(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void Constructor_WithAssessmentSectionWithoutData_ExpectedValues()
        {
            // Setup
            AssessmentSection assessmentSection = CreateAssessmentSection();

            // Call
            using (var view = new AssemblyResultPerSectionMapView(assessmentSection))
            {
                // Assert
                Assert.IsInstanceOf<UserControl>(view);
                Assert.IsInstanceOf<IMapView>(view);
                Assert.IsNull(view.Data);

                Assert.AreEqual(1, view.Controls.Count);

                IMapControl mapControl = GetMapControl(view);
                Assert.AreSame(view.Map, mapControl);
                Assert.AreEqual(DockStyle.Fill, ((Control) view.Map).Dock);

                Assert.AreSame(assessmentSection, view.AssessmentSection);

                MapDataCollection mapDataCollection = view.Map.Data;
                Assert.AreEqual("Assemblagekaart", mapDataCollection.Name);

                List<MapData> mapDataList = mapDataCollection.Collection.ToList();

                Assert.AreEqual(3, mapDataList.Count);

                var assemblyResultsLineMapData = (MapLineData) mapDataList[assemblyResultsIndex];
                var hydraulicBoundaryLocationsMapData = (MapPointData) mapDataList[hydraulicBoundaryLocationsIndex];
                var referenceLineMapData = (MapLineData) mapDataList[referenceLineIndex];

                CollectionAssert.IsEmpty(assemblyResultsLineMapData.Features);
                CollectionAssert.IsEmpty(hydraulicBoundaryLocationsMapData.Features);
                CollectionAssert.IsEmpty(referenceLineMapData.Features);

                Assert.AreEqual("Slechtste duidingsklasse per deelvak", assemblyResultsLineMapData.Name);
                Assert.AreEqual("Hydraulische belastingen", hydraulicBoundaryLocationsMapData.Name);
                Assert.AreEqual("Referentielijn", referenceLineMapData.Name);
            }
        }

        [Test]
        public void Constructor_AssessmentSectionWithBackgroundData_BackgroundDataSet()
        {
            // Setup
            AssessmentSection assessmentSection = CreateAssessmentSection();

            // Call
            using (var view = new AssemblyResultPerSectionMapView(assessmentSection))
            {
                // Assert
                MapDataTestHelper.AssertImageBasedMapData(assessmentSection.BackgroundData, view.Map.BackgroundMapData);
            }
        }

        [Test]
        public void Constructor_WithAllData_DataUpdatedToCollectionOfFilledMapData()
        {
            // Setup
            AssessmentSection assessmentSection = CreateAssessmentSectionWithReferenceLine();
            assessmentSection.SetHydraulicBoundaryLocationCalculations(new[]
            {
                new HydraulicBoundaryLocation(1, "test", 1.0, 2.0)
            });

            // Call
            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                AssessmentSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedAssessmentSectionAssemblyCalculator;
                CombinedFailureMechanismSectionAssembly[] failureMechanismSectionAssembly =
                {
                    CombinedFailureMechanismSectionAssemblyTestFactory.Create(assessmentSection, 21)
                };
                calculator.CombinedFailureMechanismSectionAssemblyOutput = failureMechanismSectionAssembly;

                using (var view = new AssemblyResultPerSectionMapView(assessmentSection))
                {
                    // Assert
                    Assert.IsInstanceOf<MapDataCollection>(view.Map.Data);
                    MapDataCollection mapData = view.Map.Data;
                    Assert.IsNotNull(mapData);

                    Assert.AreEqual(3, mapData.Collection.Count());

                    IEnumerable<CombinedFailureMechanismSectionAssemblyResult> expectedResults =
                        AssessmentSectionAssemblyFactory.AssembleCombinedPerFailureMechanismSection(assessmentSection);

                    AssertCombinedFailureMechanismSectionAssemblyResultMapData(expectedResults,
                                                                               assessmentSection.ReferenceLine,
                                                                               mapData.Collection.ElementAt(assemblyResultsIndex));

                    MapData hydraulicBoundaryLocationsMapData = mapData.Collection.ElementAt(hydraulicBoundaryLocationsIndex);
                    MapDataTestHelper.AssertHydraulicBoundaryLocationsMapData(assessmentSection, hydraulicBoundaryLocationsMapData);

                    MapData referenceLineMapData = mapData.Collection.ElementAt(referenceLineIndex);
                    AssertReferenceLineMapData(assessmentSection.ReferenceLine, referenceLineMapData);
                }
            }
        }

        [Test]
        public void UpdateObserver_AssessmentSectionUpdated_MapDataUpdated()
        {
            // Setup
            AssessmentSection assessmentSection = CreateAssessmentSectionWithReferenceLine();

            using (var view = new AssemblyResultPerSectionMapView(assessmentSection))
            {
                IMapControl map = GetMapControl(view);

                var mocks = new MockRepository();
                IObserver[] observers = AttachMapDataObservers(mocks, map.Data.Collection);
                observers[referenceLineIndex].Expect(obs => obs.UpdateObserver());
                observers[assemblyResultsObserverIndex].Expect(obs => obs.UpdateObserver());
                mocks.ReplayAll();

                var referenceLineMapData = (MapLineData) map.Data.Collection.ElementAt(referenceLineIndex);

                // Precondition
                MapFeaturesTestHelper.AssertReferenceLineMetaData(assessmentSection.ReferenceLine, assessmentSection, referenceLineMapData.Features);
                AssertReferenceLineMapData(assessmentSection.ReferenceLine, referenceLineMapData);

                // Call
                assessmentSection.Name = "New name";
                assessmentSection.NotifyObservers();

                // Assert
                MapFeaturesTestHelper.AssertReferenceLineMetaData(assessmentSection.ReferenceLine, assessmentSection, referenceLineMapData.Features);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void UpdateObserver_ReferenceLineUpdated_MapDataUpdated()
        {
            // Setup
            AssessmentSection assessmentSection = CreateAssessmentSectionWithReferenceLine();
            ReferenceLine referenceLine = assessmentSection.ReferenceLine;

            using (var view = new AssemblyResultPerSectionMapView(assessmentSection))
            {
                IMapControl map = GetMapControl(view);

                var mocks = new MockRepository();
                IObserver[] observers = AttachMapDataObservers(mocks, map.Data.Collection);
                observers[assemblyResultsObserverIndex].Expect(obs => obs.UpdateObserver());
                observers[referenceLineIndex].Expect(obs => obs.UpdateObserver());
                mocks.ReplayAll();

                MapData referenceLineMapData = map.Data.Collection.ElementAt(referenceLineIndex);

                // Precondition
                AssertReferenceLineMapData(assessmentSection.ReferenceLine, referenceLineMapData);

                // Call
                referenceLine.SetGeometry(new[]
                {
                    new Point2D(2.0, 5.0),
                    new Point2D(4.0, 3.0)
                });
                referenceLine.NotifyObservers();

                // Assert
                AssertReferenceLineMapData(assessmentSection.ReferenceLine, referenceLineMapData);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void GivenViewWithAssemblyResults_WhenFailureMechanismNotifiesObserver_ThenAssemblyResultsRefreshed()
        {
            // Given
            var random = new Random(21);
            AssessmentSection assessmentSection = CreateAssessmentSectionWithReferenceLine();

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                AssessmentSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedAssessmentSectionAssemblyCalculator;
                calculator.CombinedFailureMechanismSectionAssemblyOutput = new[]
                {
                    CombinedFailureMechanismSectionAssemblyTestFactory.Create(assessmentSection, 10)
                };

                using (var view = new AssemblyResultPerSectionMapView(assessmentSection))
                {
                    // Precondition
                    MapDataCollection mapData = view.Map.Data;

                    var mocks = new MockRepository();
                    IObserver[] observers = AttachMapDataObservers(mocks, mapData.Collection);
                    observers[referenceLineIndex].Expect(obs => obs.UpdateObserver());
                    observers[assemblyResultsObserverIndex].Expect(obs => obs.UpdateObserver());
                    mocks.ReplayAll();

                    IEnumerable<CombinedFailureMechanismSectionAssemblyResult> expectedResults =
                        AssessmentSectionAssemblyFactory.AssembleCombinedPerFailureMechanismSection(assessmentSection);

                    AssertCombinedFailureMechanismSectionAssemblyResultMapData(expectedResults,
                                                                               assessmentSection.ReferenceLine,
                                                                               mapData.Collection.ElementAt(assemblyResultsIndex));

                    // When 
                    calculator.CombinedFailureMechanismSectionAssemblyOutput = new[]
                    {
                        CombinedFailureMechanismSectionAssemblyTestFactory.Create(assessmentSection, 20)
                    };
                    IEnumerable<IFailureMechanism> failureMechanisms = assessmentSection.GetFailureMechanisms();
                    failureMechanisms.ElementAt(random.Next(failureMechanisms.Count())).NotifyObservers();

                    // Then
                    expectedResults = AssessmentSectionAssemblyFactory.AssembleCombinedPerFailureMechanismSection(assessmentSection);
                    AssertCombinedFailureMechanismSectionAssemblyResultMapData(expectedResults,
                                                                               assessmentSection.ReferenceLine,
                                                                               mapData.Collection.ElementAt(assemblyResultsIndex));
                    mocks.VerifyAll();
                }
            }
        }

        [Test]
        public void GivenViewWithAssemblyResults_WhenAssessmentSectionNotifiesObserver_ThenAssemblyResultsRefreshed()
        {
            // Given
            AssessmentSection assessmentSection = CreateAssessmentSectionWithReferenceLine();

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                AssessmentSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedAssessmentSectionAssemblyCalculator;
                calculator.CombinedFailureMechanismSectionAssemblyOutput = new[]
                {
                    CombinedFailureMechanismSectionAssemblyTestFactory.Create(assessmentSection, 10)
                };

                using (var view = new AssemblyResultPerSectionMapView(assessmentSection))
                {
                    // Precondition
                    MapDataCollection mapData = view.Map.Data;

                    var mocks = new MockRepository();
                    IObserver[] observers = AttachMapDataObservers(mocks, mapData.Collection);
                    observers[assemblyResultsObserverIndex].Expect(obs => obs.UpdateObserver());
                    observers[referenceLineIndex].Expect(obs => obs.UpdateObserver());
                    mocks.ReplayAll();

                    IEnumerable<CombinedFailureMechanismSectionAssemblyResult> expectedResults =
                        AssessmentSectionAssemblyFactory.AssembleCombinedPerFailureMechanismSection(assessmentSection);

                    AssertCombinedFailureMechanismSectionAssemblyResultMapData(expectedResults,
                                                                               assessmentSection.ReferenceLine,
                                                                               mapData.Collection.ElementAt(assemblyResultsIndex));

                    // When 
                    calculator.CombinedFailureMechanismSectionAssemblyOutput = new[]
                    {
                        CombinedFailureMechanismSectionAssemblyTestFactory.Create(assessmentSection, 20)
                    };
                    assessmentSection.NotifyObservers();

                    // Then
                    expectedResults = AssessmentSectionAssemblyFactory.AssembleCombinedPerFailureMechanismSection(assessmentSection);
                    AssertCombinedFailureMechanismSectionAssemblyResultMapData(expectedResults,
                                                                               assessmentSection.ReferenceLine,
                                                                               mapData.Collection.ElementAt(assemblyResultsIndex));
                    mocks.VerifyAll();
                }
            }
        }

        private static AssessmentSection CreateAssessmentSection()
        {
            var random = new Random(21);
            return new AssessmentSection(random.NextEnumValue<AssessmentSectionComposition>());
        }

        private static AssessmentSection CreateAssessmentSectionWithReferenceLine()
        {
            AssessmentSection assessmentSection = CreateAssessmentSection();
            assessmentSection.ReferenceLine.SetGeometry(new[]
            {
                new Point2D(1.0, 2.0),
                new Point2D(2.0, 1.0)
            });

            return assessmentSection;
        }

        private static void AssertReferenceLineMapData(ReferenceLine referenceLine, MapData referenceLineMapData)
        {
            MapDataTestHelper.AssertReferenceLineMapData(referenceLine, referenceLineMapData);
            Assert.IsTrue(referenceLineMapData.IsVisible);
        }

        private static void AssertCombinedFailureMechanismSectionAssemblyResultMapData(IEnumerable<CombinedFailureMechanismSectionAssemblyResult> expectedAssemblyResults,
                                                                                       ReferenceLine referenceLine,
                                                                                       MapData mapData)
        {
            Assert.IsInstanceOf<MapLineData>(mapData);
            var assemblyResultMapData = (MapLineData) mapData;

            int expectedNrOfResults = expectedAssemblyResults.Count();
            IEnumerable<MapFeature> mapFeatures = assemblyResultMapData.Features;
            Assert.AreEqual(expectedNrOfResults, mapFeatures.Count());

            for (var i = 0; i < expectedNrOfResults; i++)
            {
                CombinedFailureMechanismSectionAssemblyResult expectedAssemblyResult = expectedAssemblyResults.ElementAt(i);
                MapFeature actualFeature = mapFeatures.ElementAt(i);

                MapGeometry mapGeometry = actualFeature.MapGeometries.Single();
                AssertEqualPointCollections(referenceLine,
                                            expectedAssemblyResult,
                                            mapGeometry);

                Assert.AreEqual(1, actualFeature.MetaData.Keys.Count);
                Assert.AreEqual(EnumDisplayNameHelper.GetDisplayName(expectedAssemblyResult.TotalResult),
                                mapFeatures.ElementAt(i).MetaData["Duidingsklasse"]);
            }
        }

        private static IMapControl GetMapControl(AssemblyResultPerSectionMapView view)
        {
            var mapControl = (RiskeerMapControl) view.Controls[0];
            return mapControl.MapControl;
        }

        private static void AssertEqualPointCollections(ReferenceLine referenceLine,
                                                        CombinedFailureMechanismSectionAssemblyResult sectionAssemblyResult,
                                                        MapGeometry geometry)
        {
            IEnumerable<Point2D> expectedGeometry = FailureMechanismSectionHelper.GetFailureMechanismSectionGeometry(
                referenceLine,
                sectionAssemblyResult.SectionStart,
                sectionAssemblyResult.SectionEnd).ToArray();
            CollectionAssert.IsNotEmpty(expectedGeometry);

            CollectionAssert.AreEqual(expectedGeometry, geometry.PointCollections.Single());
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

            var assemblyResultsObserver = mocks.StrictMock<IObserver>();
            mapDataArray[assemblyResultsIndex].Attach(assemblyResultsObserver);

            return new[]
            {
                referenceLineMapDataObserver,
                assemblyResultsObserver
            };
        }
    }
}