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
using System.Collections.ObjectModel;
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
using Riskeer.MacroStabilityInwards.Data;
using Riskeer.MacroStabilityInwards.Data.SoilProfile;
using Riskeer.MacroStabilityInwards.Data.TestUtil;
using Riskeer.MacroStabilityInwards.Forms.Views;
using Riskeer.MacroStabilityInwards.Primitives;

namespace Riskeer.MacroStabilityInwards.Forms.Test.Views
{
    [TestFixture]
    public class MacroStabilityInwardsFailureMechanismViewTest
    {
        private const int referenceLineIndex = 0;
        private const int stochasticSoilModelsIndex = 1;
        private const int surfaceLinesIndex = 2;
        private const int hydraulicBoundaryLocationsIndex = 3;
        private const int calculationsIndex = 4;

        private const int hydraulicBoundaryLocationsObserverIndex = 3;
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
        [Apartment(ApartmentState.STA)]
        public void Constructor_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            void Call() => new MacroStabilityInwardsFailureMechanismView(null, assessmentSection);

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
            void Call() => new MacroStabilityInwardsFailureMechanismView(new MacroStabilityInwardsFailureMechanism(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            var assessmentSection = new AssessmentSectionStub();

            // Call
            var view = new MacroStabilityInwardsFailureMechanismView(failureMechanism, assessmentSection);

            ShowView(view);

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
            var assessmentSection = new AssessmentSectionStub();

            // Call
            var view = new MacroStabilityInwardsFailureMechanismView(new MacroStabilityInwardsFailureMechanism(), assessmentSection);

            ShowView(view);

            // Assert
            MapDataTestHelper.AssertImageBasedMapData(assessmentSection.BackgroundData, view.Map.BackgroundMapData);
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void Constructor_WithAllData_DataUpdatedToCollectionOfFilledMapData()
        {
            // Setup
            var random = new Random(39);

            MacroStabilityInwardsStochasticSoilModel stochasticSoilModel1 =
                MacroStabilityInwardsStochasticSoilModelTestFactory.CreateValidStochasticSoilModel("name1", new[]
                {
                    new Point2D(1.0, 2.0),
                    new Point2D(1.1, 2.2)
                });

            MacroStabilityInwardsStochasticSoilModel stochasticSoilModel2 =
                MacroStabilityInwardsStochasticSoilModelTestFactory.CreateValidStochasticSoilModel("name2", new[]
                {
                    new Point2D(3.0, 4.0),
                    new Point2D(3.3, 4.4)
                });

            var surfaceLineA = new MacroStabilityInwardsSurfaceLine("Line A");
            surfaceLineA.SetGeometry(new[]
            {
                new Point3D(0.0, 0.0, 1.0),
                new Point3D(3.0, 0.0, 1.7)
            });

            var surfaceLineB = new MacroStabilityInwardsSurfaceLine("Name B");
            surfaceLineB.SetGeometry(new[]
            {
                new Point3D(0.0, 0.0, 1.5),
                new Point3D(3.0, 0.0, 1.8)
            });
            surfaceLineA.ReferenceLineIntersectionWorldPoint = new Point2D(1.3, 1.3);
            surfaceLineB.ReferenceLineIntersectionWorldPoint = new Point2D(1.5, 1.5);

            var geometryPoints = new[]
            {
                new Point2D(0.0, 0.0),
                new Point2D(2.0, 0.0),
                new Point2D(4.0, 4.0),
                new Point2D(6.0, 4.0)
            };

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            const string arbitraryFilePath = "path";
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

            MacroStabilityInwardsCalculationScenario calculationA = MacroStabilityInwardsCalculationScenarioTestFactory.CreateMacroStabilityInwardsCalculationScenarioWithValidInput(hydraulicBoundaryLocation1);
            calculationA.InputParameters.SurfaceLine = surfaceLineA;
            MacroStabilityInwardsCalculationScenario calculationB = MacroStabilityInwardsCalculationScenarioTestFactory.CreateMacroStabilityInwardsCalculationScenarioWithValidInput(hydraulicBoundaryLocation2);
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
                var view = new MacroStabilityInwardsFailureMechanismView(failureMechanism, assessmentSection);

                ShowView(view);

                IMapControl map = ((RiskeerMapControl) view.Controls[0]).MapControl;

                // Assert
                MapDataCollection mapData = map.Data;
                Assert.IsInstanceOf<MapDataCollection>(mapData);

                List<MapData> mapDataList = mapData.Collection.ToList();
                Assert.AreEqual(5, mapDataList.Count);
                MapDataTestHelper.AssertReferenceLineMapData(assessmentSection.ReferenceLine, mapDataList[referenceLineIndex]);
                AssertSurfaceLinesMapData(failureMechanism.SurfaceLines, mapDataList[surfaceLinesIndex]);

                MapDataTestHelper.AssertHydraulicBoundaryLocationsMapData(assessmentSection, mapDataList[hydraulicBoundaryLocationsIndex]);
                AssertStochasticSoilModelsMapData(failureMechanism.StochasticSoilModels, mapDataList[stochasticSoilModelsIndex]);
                AssertCalculationsMapData(failureMechanism.Calculations.Cast<MacroStabilityInwardsCalculationScenario>(), mapDataList[calculationsIndex]);
            }
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

            var view = new MacroStabilityInwardsFailureMechanismView(new MacroStabilityInwardsFailureMechanism(), assessmentSection);

            ShowView(view);

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
                new HydraulicBoundaryLocation(2, "test2", 2.0, 3.0)
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

            var view = new MacroStabilityInwardsFailureMechanismView(new MacroStabilityInwardsFailureMechanism(), assessmentSection);

            ShowView(view);
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

            var view = new MacroStabilityInwardsFailureMechanismView(new MacroStabilityInwardsFailureMechanism(), assessmentSection);

            ShowView(view);

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

            var view = new MacroStabilityInwardsFailureMechanismView(new MacroStabilityInwardsFailureMechanism(), assessmentSection);

            ShowView(view);

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
        public void GivenViewWithSurfaceLinesData_WhenSurfaceLinesUpdatedAndNotified_ThenMapDataUpdated()
        {
            // Given
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            var view = new MacroStabilityInwardsFailureMechanismView(failureMechanism, new AssessmentSectionStub());

            ShowView(view);

            IMapControl map = ((RiskeerMapControl) view.Controls[0]).MapControl;

            var surfaceLine = new MacroStabilityInwardsSurfaceLine(string.Empty);
            surfaceLine.SetGeometry(new Collection<Point3D>
            {
                new Point3D(1, 2, 3),
                new Point3D(4, 5, 6)
            });

            var surfaceLineMapData = (MapLineData) map.Data.Collection.ElementAt(surfaceLinesIndex);

            var mocks = new MockRepository();
            IObserver[] observers = AttachMapDataObservers(mocks, map.Data.Collection);
            observers[surfaceLinesIndex].Expect(obs => obs.UpdateObserver());
            mocks.ReplayAll();

            // When
            failureMechanism.SurfaceLines.AddRange(new[]
            {
                surfaceLine
            }, "path");
            failureMechanism.SurfaceLines.NotifyObservers();

            // Then
            AssertSurfaceLinesMapData(failureMechanism.SurfaceLines, surfaceLineMapData);
            mocks.VerifyAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void GivenViewWithSurfaceLineData_WhenSurfaceLineUpdatedAndNotified_ThenMapDataUpdated()
        {
            // Given
            var surfaceLine = new MacroStabilityInwardsSurfaceLine(string.Empty);
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            failureMechanism.SurfaceLines.AddRange(new[]
            {
                surfaceLine
            }, "path");

            var view = new MacroStabilityInwardsFailureMechanismView(failureMechanism, new AssessmentSectionStub());

            ShowView(view);

            IMapControl map = ((RiskeerMapControl) view.Controls[0]).MapControl;

            var mocks = new MockRepository();
            IObserver[] observers = AttachMapDataObservers(mocks, map.Data.Collection);
            observers[surfaceLinesIndex].Expect(obs => obs.UpdateObserver());
            mocks.ReplayAll();

            // When
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(7, 8, 9),
                new Point3D(10, 11, 12)
            });
            surfaceLine.NotifyObservers();

            // Then
            var surfaceLineMapData = (MapLineData) map.Data.Collection.ElementAt(surfaceLinesIndex);
            AssertSurfaceLinesMapData(failureMechanism.SurfaceLines, surfaceLineMapData);
            mocks.VerifyAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void GivenViewWithStochasticSoilModels_WhenStochasticSoilModelsUpdatedAndNotified_ThenMapDataUpdated()
        {
            // Given
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            var view = new MacroStabilityInwardsFailureMechanismView(failureMechanism, new AssessmentSectionStub());

            ShowView(view);

            IMapControl map = ((RiskeerMapControl) view.Controls[0]).MapControl;

            var stochasticSoilModelMapData = (MapLineData) map.Data.Collection.ElementAt(stochasticSoilModelsIndex);

            var mocks = new MockRepository();
            IObserver[] observers = AttachMapDataObservers(mocks, map.Data.Collection);
            observers[stochasticSoilModelsIndex].Expect(obs => obs.UpdateObserver());
            mocks.ReplayAll();

            // When
            failureMechanism.StochasticSoilModels.AddRange(new[]
            {
                MacroStabilityInwardsStochasticSoilModelTestFactory.CreateValidStochasticSoilModel("", new[]
                {
                    new Point2D(1, 2),
                    new Point2D(1, 2)
                })
            }, "path");
            failureMechanism.StochasticSoilModels.NotifyObservers();

            // Then
            AssertStochasticSoilModelsMapData(failureMechanism.StochasticSoilModels, stochasticSoilModelMapData);
            mocks.VerifyAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void GivenViewWithCalculationGroupData_WhenCalculationGroupUpdatedAndNotified_ThenMapDataUpdated()
        {
            // Given
            var surfaceLineA = new MacroStabilityInwardsSurfaceLine(string.Empty);
            surfaceLineA.SetGeometry(new[]
            {
                new Point3D(0.0, 0.0, 1.0),
                new Point3D(3.0, 0.0, 1.7)
            });
            surfaceLineA.ReferenceLineIntersectionWorldPoint = new Point2D(1.3, 1.3);

            MacroStabilityInwardsCalculationScenario calculationA = MacroStabilityInwardsCalculationScenarioTestFactory.CreateMacroStabilityInwardsCalculationScenarioWithValidInput(new TestHydraulicBoundaryLocation());
            calculationA.InputParameters.SurfaceLine = surfaceLineA;

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(calculationA);

            var view = new MacroStabilityInwardsFailureMechanismView(failureMechanism, new AssessmentSectionStub());

            ShowView(view);

            IMapControl map = ((RiskeerMapControl) view.Controls[0]).MapControl;

            var surfaceLineB = new MacroStabilityInwardsSurfaceLine(string.Empty);
            surfaceLineB.SetGeometry(new[]
            {
                new Point3D(0.0, 0.0, 1.5),
                new Point3D(3.0, 0.0, 1.8)
            });
            surfaceLineB.ReferenceLineIntersectionWorldPoint = new Point2D(1.5, 1.5);

            MacroStabilityInwardsCalculationScenario calculationB = MacroStabilityInwardsCalculationScenarioTestFactory.CreateMacroStabilityInwardsCalculationScenarioWithValidInput(new TestHydraulicBoundaryLocation());
            calculationB.InputParameters.SurfaceLine = surfaceLineB;

            var calculationMapData = (MapLineData) map.Data.Collection.ElementAt(calculationsIndex);

            var mocks = new MockRepository();
            IObserver[] observers = AttachMapDataObservers(mocks, map.Data.Collection);
            observers[calculationObserverIndex].Expect(obs => obs.UpdateObserver());
            mocks.ReplayAll();

            // When
            failureMechanism.CalculationsGroup.Children.Add(calculationB);
            failureMechanism.CalculationsGroup.NotifyObservers();

            // Then
            AssertCalculationsMapData(failureMechanism.Calculations.Cast<MacroStabilityInwardsCalculationScenario>(), calculationMapData);
            mocks.VerifyAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void GivenViewWithCalculationInputData_WhenCalculationInputUpdatedAndNotified_ThenMapDataUpdated()
        {
            // Given
            var surfaceLineA = new MacroStabilityInwardsSurfaceLine(string.Empty);
            surfaceLineA.SetGeometry(new[]
            {
                new Point3D(0.0, 0.0, 1.0),
                new Point3D(3.0, 0.0, 1.7)
            });
            surfaceLineA.ReferenceLineIntersectionWorldPoint = new Point2D(1.3, 1.3);

            MacroStabilityInwardsCalculationScenario calculationA = MacroStabilityInwardsCalculationScenarioTestFactory.CreateMacroStabilityInwardsCalculationScenarioWithValidInput(new TestHydraulicBoundaryLocation());
            calculationA.InputParameters.SurfaceLine = surfaceLineA;

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(calculationA);

            var view = new MacroStabilityInwardsFailureMechanismView(failureMechanism, new AssessmentSectionStub());

            ShowView(view);

            IMapControl map = ((RiskeerMapControl) view.Controls[0]).MapControl;

            var surfaceLineB = new MacroStabilityInwardsSurfaceLine(string.Empty);
            surfaceLineB.SetGeometry(new[]
            {
                new Point3D(0.0, 0.0, 1.5),
                new Point3D(3.0, 0.0, 1.8)
            });
            surfaceLineB.ReferenceLineIntersectionWorldPoint = new Point2D(1.5, 1.5);

            var calculationMapData = (MapLineData) map.Data.Collection.ElementAt(calculationsIndex);

            var mocks = new MockRepository();
            IObserver[] observers = AttachMapDataObservers(mocks, map.Data.Collection);
            observers[calculationObserverIndex].Expect(obs => obs.UpdateObserver());
            mocks.ReplayAll();

            // When
            calculationA.InputParameters.SurfaceLine = surfaceLineB;
            calculationA.InputParameters.NotifyObservers();

            // Then
            AssertCalculationsMapData(failureMechanism.Calculations.Cast<MacroStabilityInwardsCalculationScenario>(), calculationMapData);
            mocks.VerifyAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void GivenViewWithCalculationData_WhenCalculationUpdatedAndNotified_ThenMapDataUpdated()
        {
            // Given
            var surfaceLineA = new MacroStabilityInwardsSurfaceLine(string.Empty);
            surfaceLineA.SetGeometry(new[]
            {
                new Point3D(0.0, 0.0, 1.0),
                new Point3D(3.0, 0.0, 1.7)
            });
            surfaceLineA.ReferenceLineIntersectionWorldPoint = new Point2D(1.3, 1.3);

            MacroStabilityInwardsCalculationScenario calculationA = MacroStabilityInwardsCalculationScenarioTestFactory.CreateMacroStabilityInwardsCalculationScenarioWithValidInput(new TestHydraulicBoundaryLocation());
            calculationA.InputParameters.SurfaceLine = surfaceLineA;

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(calculationA);

            var view = new MacroStabilityInwardsFailureMechanismView(failureMechanism, new AssessmentSectionStub());

            ShowView(view);

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
            AssertCalculationsMapData(failureMechanism.Calculations.Cast<MacroStabilityInwardsCalculationScenario>(), calculationMapData);
            mocks.VerifyAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void UpdateObserver_DataUpdated_MapLayersSameOrder()
        {
            // Setup
            const int updatedReferenceLineLayerIndex = referenceLineIndex + 4;
            const int updatedSurfaceLineLayerIndex = surfaceLinesIndex - 1;
            const int updatedHydraulicLocationsLayerIndex = hydraulicBoundaryLocationsIndex - 1;
            const int updatedStochasticSoilModelsLayerIndex = stochasticSoilModelsIndex - 1;
            const int updatedCalculationsIndex = calculationsIndex - 1;

            var assessmentSection = new AssessmentSectionStub();

            var view = new MacroStabilityInwardsFailureMechanismView(new MacroStabilityInwardsFailureMechanism(), assessmentSection);

            ShowView(view);

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

            var hydraulicLocationsData = (MapPointData) mapDataList[updatedHydraulicLocationsLayerIndex];
            Assert.AreEqual("Hydraulische belastingen", hydraulicLocationsData.Name);

            var stochasticSoilModelsData = (MapLineData) mapDataList[updatedStochasticSoilModelsLayerIndex];
            Assert.AreEqual("Stochastische ondergrondmodellen", stochasticSoilModelsData.Name);

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

            var actualSurfaceLineData = (MapLineData) mapDataList[updatedSurfaceLineLayerIndex];
            Assert.AreEqual("Profielschematisaties", actualSurfaceLineData.Name);

            var actualHydraulicLocationsData = (MapPointData) mapDataList[updatedHydraulicLocationsLayerIndex];
            Assert.AreEqual("Hydraulische belastingen", actualHydraulicLocationsData.Name);

            var actualStochasticSoilModelsData = (MapLineData) mapDataList[updatedStochasticSoilModelsLayerIndex];
            Assert.AreEqual("Stochastische ondergrondmodellen", actualStochasticSoilModelsData.Name);

            var actualCalculationsData = (MapLineData) mapDataList[updatedCalculationsIndex];
            Assert.AreEqual("Berekeningen", actualCalculationsData.Name);
        }

        private void ShowView(Control view)
        {
            testForm.Controls.Add(view);
            testForm.Show();
        }

        private static void AssertSurfaceLinesMapData(IEnumerable<MacroStabilityInwardsSurfaceLine> surfaceLines, MapData mapData)
        {
            Assert.IsInstanceOf<MapLineData>(mapData);
            var surfaceLinesMapData = (MapLineData) mapData;
            MapFeature[] surfaceLineFeatures = surfaceLinesMapData.Features.ToArray();
            MacroStabilityInwardsSurfaceLine[] surfaceLinesArray = surfaceLines.ToArray();
            Assert.AreEqual(surfaceLinesArray.Length, surfaceLineFeatures.Length);

            for (var index = 0; index < surfaceLinesArray.Length; index++)
            {
                Assert.AreEqual(1, surfaceLineFeatures[index].MapGeometries.Count());
                MacroStabilityInwardsSurfaceLine surfaceLine = surfaceLinesArray[index];
                CollectionAssert.AreEquivalent(surfaceLine.Points.Select(p => new Point2D(p.X, p.Y)), surfaceLineFeatures[index].MapGeometries.First().PointCollections.First());
            }

            Assert.AreEqual("Profielschematisaties", mapData.Name);
        }

        private static void AssertStochasticSoilModelsMapData(IEnumerable<MacroStabilityInwardsStochasticSoilModel> soilModels, MapData mapData)
        {
            Assert.IsInstanceOf<MapLineData>(mapData);
            var soilModelsMapData = (MapLineData) mapData;
            MapFeature[] soilModelsFeatures = soilModelsMapData.Features.ToArray();
            MacroStabilityInwardsStochasticSoilModel[] stochasticSoilModelsArray = soilModels.ToArray();
            Assert.AreEqual(stochasticSoilModelsArray.Length, soilModelsFeatures.Length);

            for (var index = 0; index < stochasticSoilModelsArray.Length; index++)
            {
                Assert.AreEqual(1, soilModelsFeatures[index].MapGeometries.Count());
                MacroStabilityInwardsStochasticSoilModel stochasticSoilModel = stochasticSoilModelsArray[index];
                CollectionAssert.AreEquivalent(stochasticSoilModel.Geometry.Select(p => new Point2D(p)), soilModelsFeatures[index].MapGeometries.First().PointCollections.First());
            }

            Assert.AreEqual("Stochastische ondergrondmodellen", mapData.Name);
        }

        private static void AssertCalculationsMapData(IEnumerable<MacroStabilityInwardsCalculationScenario> calculations, MapData mapData)
        {
            Assert.IsInstanceOf<MapLineData>(mapData);
            var calculationsMapData = (MapLineData) mapData;
            MacroStabilityInwardsCalculationScenario[] calculationsArray = calculations.ToArray();
            MapFeature[] calculationsFeatures = calculationsMapData.Features.ToArray();
            Assert.AreEqual(calculationsArray.Length, calculationsFeatures.Length);

            for (var index = 0; index < calculationsArray.Length; index++)
            {
                MapGeometry[] geometries = calculationsFeatures[index].MapGeometries.ToArray();
                Assert.AreEqual(1, geometries.Length);

                MacroStabilityInwardsCalculationScenario calculation = calculationsArray[index];
                CollectionAssert.AreEquivalent(new[]
                                               {
                                                   calculation.InputParameters.SurfaceLine.ReferenceLineIntersectionWorldPoint,
                                                   calculation.InputParameters.HydraulicBoundaryLocation.Location
                                               },
                                               geometries[0].PointCollections.First());
            }

            Assert.AreEqual("Berekeningen", mapData.Name);
        }

        private static void AssertEmptyMapData(MapDataCollection mapDataCollection)
        {
            Assert.AreEqual("Dijken en dammen - Macrostabiliteit binnenwaarts", mapDataCollection.Name);

            List<MapData> mapDataList = mapDataCollection.Collection.ToList();

            Assert.AreEqual(5, mapDataList.Count);

            var referenceLineMapData = (MapLineData) mapDataList[referenceLineIndex];
            var stochasticSoilModelsMapData = (MapLineData) mapDataList[stochasticSoilModelsIndex];
            var surfaceLinesMapData = (MapLineData) mapDataList[surfaceLinesIndex];
            var hydraulicBoundaryLocationsMapData = (MapPointData) mapDataList[hydraulicBoundaryLocationsIndex];
            var calculationsMapData = (MapLineData) mapDataList[calculationsIndex];

            CollectionAssert.IsEmpty(referenceLineMapData.Features);
            CollectionAssert.IsEmpty(stochasticSoilModelsMapData.Features);
            CollectionAssert.IsEmpty(surfaceLinesMapData.Features);
            CollectionAssert.IsEmpty(hydraulicBoundaryLocationsMapData.Features);
            CollectionAssert.IsEmpty(calculationsMapData.Features);

            Assert.AreEqual("Referentielijn", referenceLineMapData.Name);
            Assert.AreEqual("Stochastische ondergrondmodellen", stochasticSoilModelsMapData.Name);
            Assert.AreEqual("Profielschematisaties", surfaceLinesMapData.Name);
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

            var stochasticSoilModelsMapDataObserver = mocks.StrictMock<IObserver>();
            mapDataArray[stochasticSoilModelsIndex].Attach(stochasticSoilModelsMapDataObserver);

            var surfaceLinesMapDataObserver = mocks.StrictMock<IObserver>();
            mapDataArray[surfaceLinesIndex].Attach(surfaceLinesMapDataObserver);

            var hydraulicBoundaryLocationsMapDataObserver = mocks.StrictMock<IObserver>();
            mapDataArray[hydraulicBoundaryLocationsIndex].Attach(hydraulicBoundaryLocationsMapDataObserver);

            var calculationsMapDataObserver = mocks.StrictMock<IObserver>();
            mapDataArray[calculationsIndex].Attach(calculationsMapDataObserver);

            return new[]
            {
                referenceLineMapDataObserver,
                stochasticSoilModelsMapDataObserver,
                surfaceLinesMapDataObserver,
                hydraulicBoundaryLocationsMapDataObserver,
                calculationsMapDataObserver
            };
        }
    }
}