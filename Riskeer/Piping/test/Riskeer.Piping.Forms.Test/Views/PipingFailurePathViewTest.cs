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
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.TestUtil;
using Riskeer.Common.Forms.Views;
using Riskeer.Piping.Data;
using Riskeer.Piping.Data.Probabilistic;
using Riskeer.Piping.Data.SemiProbabilistic;
using Riskeer.Piping.Data.SoilProfile;
using Riskeer.Piping.Data.TestUtil;
using Riskeer.Piping.Data.TestUtil.Probabilistic;
using Riskeer.Piping.Data.TestUtil.SemiProbabilistic;
using Riskeer.Piping.Forms.Views;
using Riskeer.Piping.Primitives;

namespace Riskeer.Piping.Forms.Test.Views
{
    [TestFixture]
    public class PipingFailurePathViewTest
    {
        private const int referenceLineIndex = 0;
        private const int stochasticSoilModelsIndex = 1;
        private const int surfaceLinesIndex = 2;
        private const int sectionsCollectionIndex = 3;
        private const int assemblyResultsIndex = 4;
        private const int hydraulicBoundaryLocationsIndex = 5;
        private const int probabilisticCalculationsIndex = 6;
        private const int semiProbabilisticCalculationsIndex = 7;

        private const int sectionsIndex = 0;
        private const int sectionsStartPointIndex = 1;
        private const int sectionsEndPointIndex = 2;

        private const int tailorMadeAssemblyIndex = 0;
        private const int detailedAssemblyIndex = 1;
        private const int simpleAssemblyIndex = 2;
        private const int combinedAssemblyIndex = 3;

        private const int simpleAssemblyObserverIndex = 0;
        private const int detailedAssemblyObserverIndex = 1;
        private const int tailorMadeAssemblyObserverIndex = 2;
        private const int combinedAssemblyObserverIndex = 3;

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
            var failureMechanism = new PipingFailureMechanism();
            var assessmentSection = new AssessmentSectionStub();

            // Call
            var view = new PipingFailurePathView(failureMechanism, assessmentSection);

            testForm.Controls.Add(view);
            testForm.Show();

            // Assert
            Assert.IsInstanceOf<PipingFailureMechanismView>(view);
            Assert.IsNull(view.Data);
            Assert.AreSame(failureMechanism, view.FailureMechanism);
            Assert.AreSame(assessmentSection, view.AssessmentSection);
            AssertEmptyMapData(view.Map.Data);
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void Constructor_AssessmentSectionWithBackgroundData_BackgroundDataSet()
        {
            // Setup
            var assessmentSection = new AssessmentSectionStub();

            // Call
            var view = new PipingFailurePathView(new PipingFailureMechanism(), assessmentSection);

            testForm.Controls.Add(view);
            testForm.Show();

            // Assert
            MapDataTestHelper.AssertImageBasedMapData(assessmentSection.BackgroundData, view.Map.BackgroundMapData);
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void Constructor_WithAllData_DataUpdatedToCollectionOfFilledMapData()
        {
            // Setup
            var random = new Random(39);

            PipingStochasticSoilModel stochasticSoilModel1 = PipingStochasticSoilModelTestFactory.CreatePipingStochasticSoilModel("name1", new[]
            {
                new Point2D(1.0, 2.0),
                new Point2D(1.1, 2.2)
            });

            PipingStochasticSoilModel stochasticSoilModel2 = PipingStochasticSoilModelTestFactory.CreatePipingStochasticSoilModel("name2", new[]
            {
                new Point2D(3.0, 4.0),
                new Point2D(3.3, 4.4)
            });

            var surfaceLineA = new PipingSurfaceLine("Line A");
            surfaceLineA.SetGeometry(new[]
            {
                new Point3D(0.0, 0.0, 1.0),
                new Point3D(3.0, 0.0, 1.7)
            });

            var surfaceLineB = new PipingSurfaceLine("Name B");
            surfaceLineB.SetGeometry(new[]
            {
                new Point3D(0.0, 0.0, 1.5),
                new Point3D(3.0, 0.0, 1.8)
            });
            surfaceLineA.ReferenceLineIntersectionWorldPoint = new Point2D(1.3, 1.3);
            surfaceLineB.ReferenceLineIntersectionWorldPoint = new Point2D(1.5, 1.5);

            var failureMechanism = new PipingFailureMechanism();
            const string arbitraryFilePath = "path";
            var geometryPoints = new[]
            {
                new Point2D(0.0, 0.0),
                new Point2D(2.0, 0.0),
                new Point2D(4.0, 4.0),
                new Point2D(6.0, 4.0)
            };
            failureMechanism.SurfaceLines.AddRange(new[]
            {
                surfaceLineA,
                surfaceLineB
            }, arbitraryFilePath);
            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                new FailureMechanismSection("A", geometryPoints.Take(2)),
                new FailureMechanismSection("B", geometryPoints.Skip(1).Take(2)),
                new FailureMechanismSection("C", geometryPoints.Skip(2).Take(2))
            });
            failureMechanism.StochasticSoilModels.AddRange(new[]
            {
                stochasticSoilModel1,
                stochasticSoilModel2
            }, arbitraryFilePath);

            var hydraulicBoundaryLocation1 = new HydraulicBoundaryLocation(1, "test", 1.0, 2.0);
            var hydraulicBoundaryLocation2 = new HydraulicBoundaryLocation(2, "test", 3.0, 4.0);

            var calculationA =
                SemiProbabilisticPipingCalculationTestFactory.CreateCalculationWithValidInput<SemiProbabilisticPipingCalculationScenario>(
                    hydraulicBoundaryLocation1);
            var calculationB =
                ProbabilisticPipingCalculationTestFactory.CreateCalculationWithValidInput<ProbabilisticPipingCalculationScenario>(
                    hydraulicBoundaryLocation2);

            calculationA.InputParameters.SurfaceLine = surfaceLineA;
            calculationB.InputParameters.SurfaceLine = surfaceLineB;

            failureMechanism.CalculationsGroup.Children.Add(calculationA);
            failureMechanism.CalculationsGroup.Children.Add(calculationB);

            var referenceLine = new ReferenceLine();
            referenceLine.SetGeometry(new[]
            {
                new Point2D(0.0, 3.0),
                new Point2D(3.0, 0.0)
            });

            var assessmentSection = new AssessmentSectionStub
            {
                ReferenceLine = referenceLine
            };

            assessmentSection.SetHydraulicBoundaryLocationCalculations(new[]
            {
                hydraulicBoundaryLocation1,
                hydraulicBoundaryLocation2
            });

            var expectedSimpleAssembly = new FailureMechanismSectionAssembly(random.NextDouble(), random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>());
            var expectedDetailedAssembly = new FailureMechanismSectionAssembly(random.NextDouble(), random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>());
            var expectedTailorMadeAssembly = new FailureMechanismSectionAssembly(random.NextDouble(), random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>());
            var expectedCombinedAssembly = new FailureMechanismSectionAssembly(random.NextDouble(), random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>());

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                calculator.SimpleAssessmentAssemblyOutput = expectedSimpleAssembly;
                calculator.DetailedAssessmentAssemblyOutput = expectedDetailedAssembly;
                calculator.TailorMadeAssessmentAssemblyOutput = expectedTailorMadeAssembly;
                calculator.CombinedAssemblyOutput = expectedCombinedAssembly;

                // Call
                var view = new PipingFailurePathView(failureMechanism, assessmentSection);

                testForm.Controls.Add(view);
                testForm.Show();

                IMapControl map = ((RiskeerMapControl) view.Controls[0]).MapControl;

                // Assert
                MapDataCollection mapData = map.Data;
                Assert.IsInstanceOf<MapDataCollection>(mapData);

                List<MapData> mapDataList = mapData.Collection.ToList();
                Assert.AreEqual(8, mapDataList.Count);
                MapDataTestHelper.AssertReferenceLineMapData(assessmentSection.ReferenceLine, mapDataList[referenceLineIndex]);
                AssertSurfaceLinesMapData(failureMechanism.SurfaceLines, mapDataList[surfaceLinesIndex]);

                IEnumerable<MapData> sectionsCollection = ((MapDataCollection) mapDataList[sectionsCollectionIndex]).Collection;
                MapDataTestHelper.AssertFailureMechanismSectionsMapData(failureMechanism.Sections, sectionsCollection.ElementAt(sectionsIndex));
                MapDataTestHelper.AssertFailureMechanismSectionsStartPointMapData(failureMechanism.Sections, sectionsCollection.ElementAt(sectionsStartPointIndex));
                MapDataTestHelper.AssertFailureMechanismSectionsEndPointMapData(failureMechanism.Sections, sectionsCollection.ElementAt(sectionsEndPointIndex));

                MapDataTestHelper.AssertHydraulicBoundaryLocationsMapData(assessmentSection, mapDataList[hydraulicBoundaryLocationsIndex]);
                AssertStochasticSoilModelsMapData(failureMechanism.StochasticSoilModels, mapDataList[stochasticSoilModelsIndex]);
                AssertProbabilisticCalculationsMapData(failureMechanism.Calculations.OfType<ProbabilisticPipingCalculationScenario>(), mapDataList[probabilisticCalculationsIndex]);
                AssertSemiProbabilisticCalculationsMapData(failureMechanism.Calculations.OfType<SemiProbabilisticPipingCalculationScenario>(), mapDataList[semiProbabilisticCalculationsIndex]);

                MapDataTestHelper.AssertAssemblyMapDataCollection(expectedSimpleAssembly,
                                                                  expectedDetailedAssembly,
                                                                  expectedTailorMadeAssembly,
                                                                  expectedCombinedAssembly,
                                                                  (MapDataCollection) mapDataList[assemblyResultsIndex],
                                                                  failureMechanism);
            }
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void GivenViewWithFailureMechanismSectionsData_WhenFailureMechanismSectionsUpdatedAndNotified_ThenMapDataUpdated()
        {
            // Given
            var failureMechanism = new PipingFailureMechanism();

            var view = new PipingFailurePathView(failureMechanism, new AssessmentSectionStub());

            testForm.Controls.Add(view);
            testForm.Show();

            IMapControl map = ((RiskeerMapControl) view.Controls[0]).MapControl;

            IEnumerable<MapData> sectionsCollection = ((MapDataCollection) map.Data.Collection.ElementAt(sectionsCollectionIndex)).Collection;
            var sectionMapData = (MapLineData) sectionsCollection.ElementAt(sectionsIndex);
            var sectionStartsMapData = (MapPointData) sectionsCollection.ElementAt(sectionsStartPointIndex);
            var sectionsEndsMapData = (MapPointData) sectionsCollection.ElementAt(sectionsEndPointIndex);

            var mocks = new MockRepository();
            IObserver[] observers = AttachMapDataObservers(mocks, map.Data.Collection);
            observers[simpleAssemblyObserverIndex].Expect(obs => obs.UpdateObserver());
            observers[detailedAssemblyObserverIndex].Expect(obs => obs.UpdateObserver());
            observers[tailorMadeAssemblyObserverIndex].Expect(obs => obs.UpdateObserver());
            observers[combinedAssemblyObserverIndex].Expect(obs => obs.UpdateObserver());
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
        public void GivenViewWithCalculationGroupData_WhenCalculationGroupUpdatedAndNotified_ThenMapDataUpdated()
        {
            // Given
            var failureMechanism = new PipingFailureMechanism();
            var view = new PipingFailurePathView(failureMechanism, new AssessmentSectionStub());

            testForm.Controls.Add(view);
            testForm.Show();

            IMapControl map = ((RiskeerMapControl) view.Controls[0]).MapControl;

            var surfaceLineA = new PipingSurfaceLine(string.Empty);
            surfaceLineA.SetGeometry(new[]
            {
                new Point3D(0.0, 0.0, 1.0),
                new Point3D(3.0, 0.0, 1.7)
            });

            var surfaceLineB = new PipingSurfaceLine(string.Empty);
            surfaceLineB.SetGeometry(new[]
            {
                new Point3D(0.0, 0.0, 1.5),
                new Point3D(3.0, 0.0, 1.8)
            });
            surfaceLineA.ReferenceLineIntersectionWorldPoint = new Point2D(1.3, 1.3);
            surfaceLineB.ReferenceLineIntersectionWorldPoint = new Point2D(1.5, 1.5);

            var calculationA =
                SemiProbabilisticPipingCalculationTestFactory.CreateCalculationWithValidInput<SemiProbabilisticPipingCalculationScenario>(
                    new TestHydraulicBoundaryLocation());
            var calculationB =
                ProbabilisticPipingCalculationTestFactory.CreateCalculationWithValidInput<ProbabilisticPipingCalculationScenario>(
                    new TestHydraulicBoundaryLocation());

            calculationA.InputParameters.SurfaceLine = surfaceLineA;
            calculationB.InputParameters.SurfaceLine = surfaceLineB;

            failureMechanism.CalculationsGroup.Children.Add(calculationA);
            failureMechanism.CalculationsGroup.Children.Add(calculationB);

            var probabilisticCalculationMapData = (MapLineData) map.Data.Collection.ElementAt(probabilisticCalculationsIndex);
            var semiProbabilisticCalculationMapData = (MapLineData) map.Data.Collection.ElementAt(semiProbabilisticCalculationsIndex);

            var mocks = new MockRepository();
            IObserver[] observers = AttachMapDataObservers(mocks, map.Data.Collection);
            observers[simpleAssemblyObserverIndex].Expect(obs => obs.UpdateObserver());
            observers[detailedAssemblyObserverIndex].Expect(obs => obs.UpdateObserver());
            observers[tailorMadeAssemblyObserverIndex].Expect(obs => obs.UpdateObserver());
            observers[combinedAssemblyObserverIndex].Expect(obs => obs.UpdateObserver());
            mocks.ReplayAll();

            // When
            failureMechanism.CalculationsGroup.Children.Add(calculationB);
            failureMechanism.CalculationsGroup.NotifyObservers();

            // Then
            AssertSemiProbabilisticCalculationsMapData(failureMechanism.Calculations.OfType<SemiProbabilisticPipingCalculationScenario>(), semiProbabilisticCalculationMapData);
            AssertProbabilisticCalculationsMapData(failureMechanism.Calculations.OfType<ProbabilisticPipingCalculationScenario>(), probabilisticCalculationMapData);
            mocks.VerifyAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void GivenViewWithSemiProbabilisticCalculationInputData_WhenCalculationInputUpdatedAndNotified_ThenMapDataUpdated()
        {
            // Given
            var surfaceLineA = new PipingSurfaceLine(string.Empty);
            surfaceLineA.SetGeometry(new[]
            {
                new Point3D(0.0, 0.0, 1.0),
                new Point3D(3.0, 0.0, 1.7)
            });

            var surfaceLineB = new PipingSurfaceLine(string.Empty);
            surfaceLineB.SetGeometry(new[]
            {
                new Point3D(0.0, 0.0, 1.5),
                new Point3D(3.0, 0.0, 1.8)
            });
            surfaceLineA.ReferenceLineIntersectionWorldPoint = new Point2D(1.3, 1.3);
            surfaceLineB.ReferenceLineIntersectionWorldPoint = new Point2D(1.5, 1.5);

            var calculationScenario =
                SemiProbabilisticPipingCalculationTestFactory.CreateCalculationWithValidInput<SemiProbabilisticPipingCalculationScenario>(
                    new TestHydraulicBoundaryLocation());

            calculationScenario.InputParameters.SurfaceLine = surfaceLineA;

            var failureMechanism = new PipingFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(calculationScenario);

            var view = new PipingFailurePathView(failureMechanism, new AssessmentSectionStub());

            testForm.Controls.Add(view);
            testForm.Show();

            IMapControl map = ((RiskeerMapControl) view.Controls[0]).MapControl;

            var calculationMapData = (MapLineData) map.Data.Collection.ElementAt(semiProbabilisticCalculationsIndex);

            var mocks = new MockRepository();
            IObserver[] observers = AttachMapDataObservers(mocks, map.Data.Collection);
            observers[simpleAssemblyObserverIndex].Expect(obs => obs.UpdateObserver());
            observers[detailedAssemblyObserverIndex].Expect(obs => obs.UpdateObserver());
            observers[tailorMadeAssemblyObserverIndex].Expect(obs => obs.UpdateObserver());
            observers[combinedAssemblyObserverIndex].Expect(obs => obs.UpdateObserver());
            mocks.ReplayAll();

            // When
            calculationScenario.InputParameters.SurfaceLine = surfaceLineB;
            calculationScenario.InputParameters.NotifyObservers();

            // Then
            AssertSemiProbabilisticCalculationsMapData(failureMechanism.Calculations.Cast<SemiProbabilisticPipingCalculationScenario>(), calculationMapData);
            mocks.VerifyAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void GivenViewWithSemiProbabilisticCalculationData_WhenCalculationUpdatedAndNotified_ThenMapDataUpdated()
        {
            // Given
            var surfaceLineA = new PipingSurfaceLine(string.Empty);
            surfaceLineA.SetGeometry(new[]
            {
                new Point3D(0.0, 0.0, 1.0),
                new Point3D(3.0, 0.0, 1.7)
            });

            var surfaceLineB = new PipingSurfaceLine(string.Empty);
            surfaceLineB.SetGeometry(new[]
            {
                new Point3D(0.0, 0.0, 1.5),
                new Point3D(3.0, 0.0, 1.8)
            });
            surfaceLineA.ReferenceLineIntersectionWorldPoint = new Point2D(1.3, 1.3);
            surfaceLineB.ReferenceLineIntersectionWorldPoint = new Point2D(1.5, 1.5);

            var calculationScenario =
                SemiProbabilisticPipingCalculationTestFactory.CreateCalculationWithValidInput<SemiProbabilisticPipingCalculationScenario>(
                    new TestHydraulicBoundaryLocation());

            calculationScenario.InputParameters.SurfaceLine = surfaceLineA;

            var failureMechanism = new PipingFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(calculationScenario);

            var view = new PipingFailurePathView(failureMechanism, new AssessmentSectionStub());

            testForm.Controls.Add(view);
            testForm.Show();

            IMapControl map = ((RiskeerMapControl) view.Controls[0]).MapControl;

            var calculationMapData = (MapLineData) map.Data.Collection.ElementAt(semiProbabilisticCalculationsIndex);

            var mocks = new MockRepository();
            IObserver[] observers = AttachMapDataObservers(mocks, map.Data.Collection);
            observers[simpleAssemblyObserverIndex].Expect(obs => obs.UpdateObserver());
            observers[detailedAssemblyObserverIndex].Expect(obs => obs.UpdateObserver());
            observers[tailorMadeAssemblyObserverIndex].Expect(obs => obs.UpdateObserver());
            observers[combinedAssemblyObserverIndex].Expect(obs => obs.UpdateObserver());
            mocks.ReplayAll();

            // When
            calculationScenario.Name = "new name";
            calculationScenario.NotifyObservers();

            // Then
            AssertSemiProbabilisticCalculationsMapData(failureMechanism.Calculations.Cast<SemiProbabilisticPipingCalculationScenario>(), calculationMapData);
            mocks.VerifyAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void GivenViewWithAssemblyData_WhenFailureMechanismNotified_ThenMapDataUpdated()
        {
            // Given
            var random = new Random(39);
            var failureMechanism = new PipingFailureMechanism();
            FailureMechanismTestHelper.AddSections(failureMechanism, random.Next(1, 10));

            var originalSimpleAssembly = new FailureMechanismSectionAssembly(random.NextDouble(), random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>());
            var originalDetailedAssembly = new FailureMechanismSectionAssembly(random.NextDouble(), random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>());
            var originalTailorMadeAssembly = new FailureMechanismSectionAssembly(random.NextDouble(), random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>());
            var originalCombinedAssembly = new FailureMechanismSectionAssembly(random.NextDouble(), random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>());

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                calculator.SimpleAssessmentAssemblyOutput = originalSimpleAssembly;
                calculator.DetailedAssessmentAssemblyOutput = originalDetailedAssembly;
                calculator.TailorMadeAssessmentAssemblyOutput = originalTailorMadeAssembly;
                calculator.CombinedAssemblyOutput = originalCombinedAssembly;

                var view = new PipingFailurePathView(failureMechanism, new AssessmentSectionStub());

                testForm.Controls.Add(view);
                testForm.Show();

                IMapControl map = ((RiskeerMapControl) view.Controls[0]).MapControl;

                var mocks = new MockRepository();
                IObserver[] observers = AttachMapDataObservers(mocks, map.Data.Collection);
                observers[simpleAssemblyObserverIndex].Expect(obs => obs.UpdateObserver());
                observers[detailedAssemblyObserverIndex].Expect(obs => obs.UpdateObserver());
                observers[tailorMadeAssemblyObserverIndex].Expect(obs => obs.UpdateObserver());
                observers[combinedAssemblyObserverIndex].Expect(obs => obs.UpdateObserver());
                mocks.ReplayAll();

                // Precondition
                var assemblyMapData = (MapDataCollection) map.Data.Collection.ElementAt(assemblyResultsIndex);
                MapDataTestHelper.AssertAssemblyMapDataCollection(originalSimpleAssembly,
                                                                  originalDetailedAssembly,
                                                                  originalTailorMadeAssembly,
                                                                  originalCombinedAssembly,
                                                                  assemblyMapData,
                                                                  failureMechanism);

                // When
                var updatedSimpleAssembly = new FailureMechanismSectionAssembly(random.NextDouble(), random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>());
                var updatedDetailedAssembly = new FailureMechanismSectionAssembly(random.NextDouble(), random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>());
                var updatedTailorMadeAssembly = new FailureMechanismSectionAssembly(random.NextDouble(), random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>());
                var updatedCombinedAssembly = new FailureMechanismSectionAssembly(random.NextDouble(), random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>());
                calculator.SimpleAssessmentAssemblyOutput = updatedSimpleAssembly;
                calculator.DetailedAssessmentAssemblyOutput = updatedDetailedAssembly;
                calculator.TailorMadeAssessmentAssemblyOutput = updatedTailorMadeAssembly;
                calculator.CombinedAssemblyOutput = updatedCombinedAssembly;
                failureMechanism.NotifyObservers();

                // Then
                MapDataTestHelper.AssertAssemblyMapDataCollection(updatedSimpleAssembly,
                                                                  updatedDetailedAssembly,
                                                                  updatedTailorMadeAssembly,
                                                                  updatedCombinedAssembly,
                                                                  assemblyMapData,
                                                                  failureMechanism);
                mocks.VerifyAll();
            }
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void GivenViewWithAssemblyData_WhenSemiProbabilisticCalculationNotified_ThenMapDataUpdated()
        {
            // Given
            var random = new Random(39);
            var surfaceLineA = new PipingSurfaceLine(string.Empty);
            surfaceLineA.SetGeometry(new[]
            {
                new Point3D(0.0, 0.0, 1.0),
                new Point3D(3.0, 0.0, 1.7)
            });

            var surfaceLineB = new PipingSurfaceLine(string.Empty);
            surfaceLineB.SetGeometry(new[]
            {
                new Point3D(0.0, 0.0, 1.5),
                new Point3D(3.0, 0.0, 1.8)
            });
            surfaceLineA.ReferenceLineIntersectionWorldPoint = new Point2D(1.3, 1.3);
            surfaceLineB.ReferenceLineIntersectionWorldPoint = new Point2D(1.5, 1.5);

            var calculation =
                SemiProbabilisticPipingCalculationTestFactory.CreateCalculationWithValidInput<SemiProbabilisticPipingCalculationScenario>(
                    new TestHydraulicBoundaryLocation());

            calculation.InputParameters.SurfaceLine = surfaceLineA;

            var failureMechanism = new PipingFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(calculation);
            FailureMechanismTestHelper.AddSections(failureMechanism, random.Next(1, 10));

            var originalSimpleAssembly = new FailureMechanismSectionAssembly(random.NextDouble(), random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>());
            var originalDetailedAssembly = new FailureMechanismSectionAssembly(random.NextDouble(), random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>());
            var originalTailorMadeAssembly = new FailureMechanismSectionAssembly(random.NextDouble(), random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>());
            var originalCombinedAssembly = new FailureMechanismSectionAssembly(random.NextDouble(), random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>());

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                calculator.SimpleAssessmentAssemblyOutput = originalSimpleAssembly;
                calculator.DetailedAssessmentAssemblyOutput = originalDetailedAssembly;
                calculator.TailorMadeAssessmentAssemblyOutput = originalTailorMadeAssembly;
                calculator.CombinedAssemblyOutput = originalCombinedAssembly;

                var view = new PipingFailurePathView(failureMechanism, new AssessmentSectionStub());

                testForm.Controls.Add(view);
                testForm.Show();

                IMapControl map = ((RiskeerMapControl) view.Controls[0]).MapControl;

                var mocks = new MockRepository();
                IObserver[] observers = AttachMapDataObservers(mocks, map.Data.Collection);
                observers[simpleAssemblyObserverIndex].Expect(obs => obs.UpdateObserver());
                observers[detailedAssemblyObserverIndex].Expect(obs => obs.UpdateObserver());
                observers[tailorMadeAssemblyObserverIndex].Expect(obs => obs.UpdateObserver());
                observers[combinedAssemblyObserverIndex].Expect(obs => obs.UpdateObserver());
                mocks.ReplayAll();

                // Precondition
                var assemblyMapData = (MapDataCollection) map.Data.Collection.ElementAt(assemblyResultsIndex);
                MapDataTestHelper.AssertAssemblyMapDataCollection(originalSimpleAssembly,
                                                                  originalDetailedAssembly,
                                                                  originalTailorMadeAssembly,
                                                                  originalCombinedAssembly,
                                                                  assemblyMapData,
                                                                  failureMechanism);

                // When
                var updatedSimpleAssembly = new FailureMechanismSectionAssembly(random.NextDouble(), random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>());
                var updatedDetailedAssembly = new FailureMechanismSectionAssembly(random.NextDouble(), random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>());
                var updatedTailorMadeAssembly = new FailureMechanismSectionAssembly(random.NextDouble(), random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>());
                var updatedCombinedAssembly = new FailureMechanismSectionAssembly(random.NextDouble(), random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>());
                calculator.SimpleAssessmentAssemblyOutput = updatedSimpleAssembly;
                calculator.DetailedAssessmentAssemblyOutput = updatedDetailedAssembly;
                calculator.TailorMadeAssessmentAssemblyOutput = updatedTailorMadeAssembly;
                calculator.CombinedAssemblyOutput = updatedCombinedAssembly;
                calculation.NotifyObservers();

                // Then
                MapDataTestHelper.AssertAssemblyMapDataCollection(updatedSimpleAssembly,
                                                                  updatedDetailedAssembly,
                                                                  updatedTailorMadeAssembly,
                                                                  updatedCombinedAssembly,
                                                                  assemblyMapData,
                                                                  failureMechanism);
                mocks.VerifyAll();
            }
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void GivenViewWithAssemblyData_WhenFailureMechanismSectionResultNotified_ThenMapDataUpdated()
        {
            // Given
            var random = new Random(39);
            var failureMechanism = new PipingFailureMechanism();
            FailureMechanismTestHelper.AddSections(failureMechanism, random.Next(1, 10));

            var originalSimpleAssembly = new FailureMechanismSectionAssembly(random.NextDouble(), random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>());
            var originalDetailedAssembly = new FailureMechanismSectionAssembly(random.NextDouble(), random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>());
            var originalTailorMadeAssembly = new FailureMechanismSectionAssembly(random.NextDouble(), random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>());
            var originalCombinedAssembly = new FailureMechanismSectionAssembly(random.NextDouble(), random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>());

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                calculator.SimpleAssessmentAssemblyOutput = originalSimpleAssembly;
                calculator.DetailedAssessmentAssemblyOutput = originalDetailedAssembly;
                calculator.TailorMadeAssessmentAssemblyOutput = originalTailorMadeAssembly;
                calculator.CombinedAssemblyOutput = originalCombinedAssembly;

                var view = new PipingFailurePathView(failureMechanism, new AssessmentSectionStub());

                testForm.Controls.Add(view);
                testForm.Show();

                IMapControl map = ((RiskeerMapControl) view.Controls[0]).MapControl;

                var mocks = new MockRepository();
                IObserver[] observers = AttachMapDataObservers(mocks, map.Data.Collection);
                observers[simpleAssemblyObserverIndex].Expect(obs => obs.UpdateObserver());
                observers[detailedAssemblyObserverIndex].Expect(obs => obs.UpdateObserver());
                observers[tailorMadeAssemblyObserverIndex].Expect(obs => obs.UpdateObserver());
                observers[combinedAssemblyObserverIndex].Expect(obs => obs.UpdateObserver());
                mocks.ReplayAll();

                // Precondition
                var assemblyMapData = (MapDataCollection) map.Data.Collection.ElementAt(assemblyResultsIndex);
                MapDataTestHelper.AssertAssemblyMapDataCollection(originalSimpleAssembly,
                                                                  originalDetailedAssembly,
                                                                  originalTailorMadeAssembly,
                                                                  originalCombinedAssembly,
                                                                  assemblyMapData,
                                                                  failureMechanism);

                // When
                var updatedSimpleAssembly = new FailureMechanismSectionAssembly(random.NextDouble(), random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>());
                var updatedDetailedAssembly = new FailureMechanismSectionAssembly(random.NextDouble(), random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>());
                var updatedTailorMadeAssembly = new FailureMechanismSectionAssembly(random.NextDouble(), random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>());
                var updatedCombinedAssembly = new FailureMechanismSectionAssembly(random.NextDouble(), random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>());
                calculator.SimpleAssessmentAssemblyOutput = updatedSimpleAssembly;
                calculator.DetailedAssessmentAssemblyOutput = updatedDetailedAssembly;
                calculator.TailorMadeAssessmentAssemblyOutput = updatedTailorMadeAssembly;
                calculator.CombinedAssemblyOutput = updatedCombinedAssembly;
                failureMechanism.SectionResults.First().NotifyObservers();

                // Then
                MapDataTestHelper.AssertAssemblyMapDataCollection(updatedSimpleAssembly,
                                                                  updatedDetailedAssembly,
                                                                  updatedTailorMadeAssembly,
                                                                  updatedCombinedAssembly,
                                                                  assemblyMapData,
                                                                  failureMechanism);
                mocks.VerifyAll();
            }
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void UpdateObserver_DataUpdated_MapLayersSameOrder()
        {
            // Setup
            const int updatedReferenceLineLayerIndex = referenceLineIndex + 7;
            const int updatedSurfaceLineLayerIndex = surfaceLinesIndex - 1;
            const int updatedSectionCollectionIndex = sectionsCollectionIndex - 1;
            const int updatedAssemblyResultsCollectionIndex = assemblyResultsIndex - 1;
            const int updatedHydraulicLocationsLayerIndex = hydraulicBoundaryLocationsIndex - 1;
            const int updatedStochasticSoilModelsLayerIndex = stochasticSoilModelsIndex - 1;
            const int updatedProbabilisticCalculationsIndex = probabilisticCalculationsIndex - 1;
            const int updatedSemiProbabilisticCalculationsIndex = semiProbabilisticCalculationsIndex - 1;

            var assessmentSection = new AssessmentSectionStub();

            var view = new PipingFailurePathView(new PipingFailureMechanism(), assessmentSection);

            testForm.Controls.Add(view);
            testForm.Show();

            IMapControl map = ((RiskeerMapControl) view.Controls[0]).MapControl;

            MapDataCollection mapData = map.Data;

            var dataToMove = (MapLineData) map.Data.Collection.ElementAt(referenceLineIndex);
            mapData.Remove(dataToMove);
            mapData.Add(dataToMove);

            List<MapData> mapDataList = mapData.Collection.ToList();

            // Precondition
            var referenceLineData = (MapLineData) mapDataList[updatedReferenceLineLayerIndex];
            Assert.AreEqual("Referentielijn", referenceLineData.Name);

            var surfaceLineData = (MapLineData) mapDataList[updatedSurfaceLineLayerIndex];
            Assert.AreEqual("Profielschematisaties", surfaceLineData.Name);

            var sectionsData = (MapDataCollection) mapDataList[updatedSectionCollectionIndex];
            Assert.AreEqual("Vakindeling", sectionsData.Name);

            var assemblyResultsData = (MapDataCollection) mapDataList[updatedAssemblyResultsCollectionIndex];
            Assert.AreEqual("Toetsoordeel", assemblyResultsData.Name);

            var hydraulicLocationsData = (MapPointData) mapDataList[updatedHydraulicLocationsLayerIndex];
            Assert.AreEqual("Hydraulische belastingen", hydraulicLocationsData.Name);

            var stochasticSoilModelsData = (MapLineData) mapDataList[updatedStochasticSoilModelsLayerIndex];
            Assert.AreEqual("Stochastische ondergrondmodellen", stochasticSoilModelsData.Name);

            var probabilisticCalculationsData = (MapLineData) mapDataList[updatedProbabilisticCalculationsIndex];
            Assert.AreEqual("Probabilistische berekeningen", probabilisticCalculationsData.Name);

            var semiProbabilisticCalculationsData = (MapLineData) mapDataList[updatedSemiProbabilisticCalculationsIndex];
            Assert.AreEqual("Semi-probabilistische berekeningen", semiProbabilisticCalculationsData.Name);

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

            var actualSurfaceLineData = (MapLineData) mapDataList[updatedSurfaceLineLayerIndex];
            Assert.AreEqual("Profielschematisaties", actualSurfaceLineData.Name);

            var actualSectionsData = (MapDataCollection) mapDataList[updatedSectionCollectionIndex];
            Assert.AreEqual("Vakindeling", actualSectionsData.Name);

            var actualAssemblyResultsData = (MapDataCollection) mapDataList[updatedAssemblyResultsCollectionIndex];
            Assert.AreEqual("Toetsoordeel", actualAssemblyResultsData.Name);

            var actualHydraulicLocationsData = (MapPointData) mapDataList[updatedHydraulicLocationsLayerIndex];
            Assert.AreEqual("Hydraulische belastingen", actualHydraulicLocationsData.Name);

            var actualStochasticSoilModelsData = (MapLineData) mapDataList[updatedStochasticSoilModelsLayerIndex];
            Assert.AreEqual("Stochastische ondergrondmodellen", actualStochasticSoilModelsData.Name);

            var actualSemiProbabilisticCalculationsData = (MapLineData) mapDataList[updatedSemiProbabilisticCalculationsIndex];
            Assert.AreEqual("Semi-probabilistische berekeningen", actualSemiProbabilisticCalculationsData.Name);

            var actualProbabilisticCalculationsData = (MapLineData) mapDataList[updatedProbabilisticCalculationsIndex];
            Assert.AreEqual("Probabilistische berekeningen", actualProbabilisticCalculationsData.Name);
        }

        private static void AssertSurfaceLinesMapData(IEnumerable<PipingSurfaceLine> surfaceLines, MapData mapData)
        {
            Assert.IsInstanceOf<MapLineData>(mapData);
            var surfaceLinesMapData = (MapLineData) mapData;
            MapFeature[] surfaceLineFeatures = surfaceLinesMapData.Features.ToArray();
            PipingSurfaceLine[] surfaceLinesArray = surfaceLines.ToArray();
            Assert.AreEqual(surfaceLinesArray.Length, surfaceLineFeatures.Length);

            for (var index = 0; index < surfaceLinesArray.Length; index++)
            {
                Assert.AreEqual(1, surfaceLineFeatures[index].MapGeometries.Count());
                PipingSurfaceLine surfaceLine = surfaceLinesArray[index];
                CollectionAssert.AreEquivalent(surfaceLine.Points.Select(p => new Point2D(p.X, p.Y)), surfaceLineFeatures[index].MapGeometries.First().PointCollections.First());
            }

            Assert.AreEqual("Profielschematisaties", mapData.Name);
        }

        private static void AssertStochasticSoilModelsMapData(IEnumerable<PipingStochasticSoilModel> soilModels, MapData mapData)
        {
            Assert.IsInstanceOf<MapLineData>(mapData);
            var soilModelsMapData = (MapLineData) mapData;
            MapFeature[] soilModelsFeatures = soilModelsMapData.Features.ToArray();
            PipingStochasticSoilModel[] stochasticSoilModelsArray = soilModels.ToArray();
            Assert.AreEqual(stochasticSoilModelsArray.Length, soilModelsFeatures.Length);

            for (var index = 0; index < stochasticSoilModelsArray.Length; index++)
            {
                Assert.AreEqual(1, soilModelsFeatures[index].MapGeometries.Count());
                PipingStochasticSoilModel stochasticSoilModel = stochasticSoilModelsArray[index];
                CollectionAssert.AreEquivalent(stochasticSoilModel.Geometry.Select(p => new Point2D(p)), soilModelsFeatures[index].MapGeometries.First().PointCollections.First());
            }

            Assert.AreEqual("Stochastische ondergrondmodellen", mapData.Name);
        }

        private static void AssertSemiProbabilisticCalculationsMapData(IEnumerable<SemiProbabilisticPipingCalculationScenario> calculations, MapData mapData)
        {
            Assert.AreEqual("Semi-probabilistische berekeningen", mapData.Name);
            AssertCalculationsMapData(calculations, mapData);
        }

        private static void AssertProbabilisticCalculationsMapData(IEnumerable<ProbabilisticPipingCalculationScenario> calculations, MapData mapData)
        {
            Assert.AreEqual("Probabilistische berekeningen", mapData.Name);
            AssertCalculationsMapData(calculations, mapData);
        }

        private static void AssertCalculationsMapData<TCalculationScenario>(IEnumerable<TCalculationScenario> calculations, MapData mapData)
            where TCalculationScenario : IPipingCalculationScenario<PipingInput>
        {
            Assert.IsInstanceOf<MapLineData>(mapData);
            var calculationsMapData = (MapLineData) mapData;
            TCalculationScenario[] calculationsArray = calculations.ToArray();
            MapFeature[] calculationsFeatures = calculationsMapData.Features.ToArray();
            Assert.AreEqual(calculationsArray.Length, calculationsFeatures.Length);

            for (var index = 0; index < calculationsArray.Length; index++)
            {
                MapGeometry[] geometries = calculationsFeatures[index].MapGeometries.ToArray();
                Assert.AreEqual(1, geometries.Length);

                TCalculationScenario calculation = calculationsArray[index];
                CollectionAssert.AreEquivalent(new[]
                                               {
                                                   calculation.InputParameters.SurfaceLine.ReferenceLineIntersectionWorldPoint,
                                                   calculation.InputParameters.HydraulicBoundaryLocation.Location
                                               },
                                               geometries[0].PointCollections.First());
            }
        }

        private static void AssertEmptyMapData(MapDataCollection mapDataCollection)
        {
            Assert.AreEqual("Dijken en dammen - Piping", mapDataCollection.Name);

            List<MapData> mapDataList = mapDataCollection.Collection.ToList();

            Assert.AreEqual(8, mapDataList.Count);

            var referenceLineMapData = (MapLineData) mapDataList[referenceLineIndex];
            var stochasticSoilModelsMapData = (MapLineData) mapDataList[stochasticSoilModelsIndex];
            var surfaceLinesMapData = (MapLineData) mapDataList[surfaceLinesIndex];
            var hydraulicBoundaryLocationsMapData = (MapPointData) mapDataList[hydraulicBoundaryLocationsIndex];
            var probabilisticCalculationsMapData = (MapLineData) mapDataList[probabilisticCalculationsIndex];
            var semiProbabilisticCalculationsMapData = (MapLineData) mapDataList[semiProbabilisticCalculationsIndex];

            CollectionAssert.IsEmpty(referenceLineMapData.Features);
            CollectionAssert.IsEmpty(stochasticSoilModelsMapData.Features);
            CollectionAssert.IsEmpty(surfaceLinesMapData.Features);
            CollectionAssert.IsEmpty(hydraulicBoundaryLocationsMapData.Features);
            CollectionAssert.IsEmpty(probabilisticCalculationsMapData.Features);
            CollectionAssert.IsEmpty(semiProbabilisticCalculationsMapData.Features);

            Assert.AreEqual("Referentielijn", referenceLineMapData.Name);
            Assert.AreEqual("Stochastische ondergrondmodellen", stochasticSoilModelsMapData.Name);
            Assert.AreEqual("Profielschematisaties", surfaceLinesMapData.Name);
            Assert.AreEqual("Hydraulische belastingen", hydraulicBoundaryLocationsMapData.Name);
            Assert.AreEqual("Probabilistische berekeningen", probabilisticCalculationsMapData.Name);
            Assert.AreEqual("Semi-probabilistische berekeningen", semiProbabilisticCalculationsMapData.Name);

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

            var assemblyResultsMapDataCollection = (MapDataCollection) mapDataList[assemblyResultsIndex];
            Assert.AreEqual("Toetsoordeel", assemblyResultsMapDataCollection.Name);
            List<MapData> assemblyMapDataList = assemblyResultsMapDataCollection.Collection.ToList();
            Assert.AreEqual(4, assemblyMapDataList.Count);

            var combinedAssemblyMapData = (MapLineData) assemblyMapDataList[combinedAssemblyIndex];
            var simpleAssemblyMapData = (MapLineData) assemblyMapDataList[simpleAssemblyIndex];
            var detailedAssemblyMapData = (MapLineData) assemblyMapDataList[detailedAssemblyIndex];
            var tailorMadeAssemblyMapData = (MapLineData) assemblyMapDataList[tailorMadeAssemblyIndex];

            CollectionAssert.IsEmpty(combinedAssemblyMapData.Features);
            CollectionAssert.IsEmpty(simpleAssemblyMapData.Features);
            CollectionAssert.IsEmpty(detailedAssemblyMapData.Features);
            CollectionAssert.IsEmpty(tailorMadeAssemblyMapData.Features);

            Assert.AreEqual("Gecombineerd toetsoordeel", combinedAssemblyMapData.Name);
            Assert.AreEqual("Toetsoordeel eenvoudige toets", simpleAssemblyMapData.Name);
            Assert.AreEqual("Toetsoordeel gedetailleerde toets", detailedAssemblyMapData.Name);
            Assert.AreEqual("Toetsoordeel toets op maat", tailorMadeAssemblyMapData.Name);
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

            MapData[] assemblyResultsCollection = ((MapDataCollection) mapDataArray[assemblyResultsIndex]).Collection.ToArray();
            var simpleAssemblyMapDataObserver = mocks.StrictMock<IObserver>();
            assemblyResultsCollection[simpleAssemblyIndex].Attach(simpleAssemblyMapDataObserver);

            var detailedAssemblyMapDataObserver = mocks.StrictMock<IObserver>();
            assemblyResultsCollection[detailedAssemblyIndex].Attach(detailedAssemblyMapDataObserver);

            var tailorMadeAssemblyMapDataObserver = mocks.StrictMock<IObserver>();
            assemblyResultsCollection[tailorMadeAssemblyIndex].Attach(tailorMadeAssemblyMapDataObserver);

            var combinedAssemblyMapDataObserver = mocks.StrictMock<IObserver>();
            assemblyResultsCollection[combinedAssemblyIndex].Attach(combinedAssemblyMapDataObserver);

            return new[]
            {
                simpleAssemblyMapDataObserver,
                detailedAssemblyMapDataObserver,
                tailorMadeAssemblyMapDataObserver,
                combinedAssemblyMapDataObserver
            };
        }
    }
}