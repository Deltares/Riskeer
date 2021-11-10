﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.TestUtil;
using Riskeer.Common.Forms.Views;
using Riskeer.GrassCoverErosionOutwards.Data;
using Riskeer.GrassCoverErosionOutwards.Forms.Views;

namespace Riskeer.GrassCoverErosionOutwards.Forms.Test.Views
{
    [TestFixture]
    public class GrassCoverErosionOutwardsFailurePathViewTest
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

        private const int tailorMadeAssemblyIndex = 0;
        private const int detailedAssemblyIndex = 1;
        private const int simpleAssemblyIndex = 2;
        private const int combinedAssemblyIndex = 3;

        private const int sectionsObserverIndex = 0;
        private const int sectionsStartPointObserverIndex = 1;
        private const int sectionsEndPointObserverIndex = 2;
        private const int simpleAssemblyObserverIndex = 3;
        private const int detailedAssemblyObserverIndex = 4;
        private const int tailorMadeAssemblyObserverIndex = 5;
        private const int combinedAssemblyObserverIndex = 6;

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
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            var assessmentSection = new AssessmentSectionStub();

            // Call
            GrassCoverErosionOutwardsFailurePathView view = CreateView(failureMechanism, assessmentSection);

            // Assert
            Assert.IsInstanceOf<GrassCoverErosionOutwardsFailureMechanismView>(view);
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
            var random = new Random(39);

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

            var calculationA = new GrassCoverErosionOutwardsWaveConditionsCalculation
            {
                InputParameters =
                {
                    ForeshoreProfile = new TestForeshoreProfile(new Point2D(1.3, 1.3)),
                    HydraulicBoundaryLocation = new TestHydraulicBoundaryLocation()
                }
            };
            var calculationB = new GrassCoverErosionOutwardsWaveConditionsCalculation
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

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
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

            var expectedSimpleAssembly = new FailureMechanismSectionAssembly(random.NextDouble(), random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>());
            var expectedDetailedAssemblyCategory = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>();
            var expectedTailorMadeAssemblyCategory = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>();
            var expectedCombinedAssemblyCategory = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>();

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                calculator.SimpleAssessmentAssemblyOutput = expectedSimpleAssembly;
                calculator.DetailedAssessmentAssemblyGroupOutput = expectedDetailedAssemblyCategory;
                calculator.TailorMadeAssemblyCategoryOutput = expectedTailorMadeAssemblyCategory;
                calculator.CombinedAssemblyCategoryOutput = expectedCombinedAssemblyCategory;

                // Call
                GrassCoverErosionOutwardsFailurePathView view = CreateView(failureMechanism, assessmentSection);

                IMapControl map = ((RiskeerMapControl) view.Controls[0]).MapControl;

                // Assert
                MapDataCollection mapData = map.Data;
                Assert.IsInstanceOf<MapDataCollection>(mapData);

                List<MapData> mapDataList = mapData.Collection.ToList();
                Assert.AreEqual(6, mapDataList.Count);
                MapDataTestHelper.AssertReferenceLineMapData(assessmentSection.ReferenceLine, mapDataList[referenceLineIndex]);
                MapDataTestHelper.AssertHydraulicBoundaryLocationsMapData(assessmentSection, mapDataList[hydraulicBoundaryLocationsIndex]);

                IEnumerable<MapData> sectionsCollection = ((MapDataCollection) mapDataList[sectionsCollectionIndex]).Collection;
                MapDataTestHelper.AssertFailureMechanismSectionsMapData(failureMechanism.Sections, sectionsCollection.ElementAt(sectionsIndex));
                MapDataTestHelper.AssertFailureMechanismSectionsStartPointMapData(failureMechanism.Sections, sectionsCollection.ElementAt(sectionsStartPointIndex));
                MapDataTestHelper.AssertFailureMechanismSectionsEndPointMapData(failureMechanism.Sections, sectionsCollection.ElementAt(sectionsEndPointIndex));
                MapDataTestHelper.AssertForeshoreProfilesMapData(failureMechanism.ForeshoreProfiles, mapDataList[foreshoreProfilesIndex]);
                AssertCalculationsMapData(failureMechanism.Calculations.Cast<GrassCoverErosionOutwardsWaveConditionsCalculation>(), mapDataList[calculationsIndex]);

                MapDataTestHelper.AssertAssemblyMapDataCollection(expectedSimpleAssembly.Group,
                                                                  expectedDetailedAssemblyCategory,
                                                                  expectedTailorMadeAssemblyCategory,
                                                                  expectedCombinedAssemblyCategory,
                                                                  (MapDataCollection) mapDataList[assemblyResultsIndex],
                                                                  failureMechanism);
            }
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void GivenViewWithFailureMechanismSectionsData_WhenFailureMechanismSectionsUpdatedAndNotified_ThenMapDataUpdated()
        {
            // Given
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();

            GrassCoverErosionOutwardsFailurePathView view = CreateView(failureMechanism, new AssessmentSectionStub());

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
            var calculationA = new GrassCoverErosionOutwardsWaveConditionsCalculation
            {
                InputParameters =
                {
                    ForeshoreProfile = new TestForeshoreProfile(new Point2D(1.3, 1.3)),
                    HydraulicBoundaryLocation = new TestHydraulicBoundaryLocation()
                }
            };
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            failureMechanism.WaveConditionsCalculationGroup.Children.Add(calculationA);

            GrassCoverErosionOutwardsFailurePathView view = CreateView(failureMechanism, new AssessmentSectionStub());

            IMapControl map = ((RiskeerMapControl) view.Controls[0]).MapControl;

            var mocks = new MockRepository();
            IObserver[] observers = AttachMapDataObservers(mocks, map.Data.Collection);
            observers[simpleAssemblyObserverIndex].Expect(obs => obs.UpdateObserver());
            observers[detailedAssemblyObserverIndex].Expect(obs => obs.UpdateObserver());
            observers[tailorMadeAssemblyObserverIndex].Expect(obs => obs.UpdateObserver());
            observers[combinedAssemblyObserverIndex].Expect(obs => obs.UpdateObserver());
            mocks.ReplayAll();

            var calculationMapData = (MapLineData) map.Data.Collection.ElementAt(calculationsIndex);

            // Precondition
            AssertCalculationsMapData(failureMechanism.Calculations.Cast<GrassCoverErosionOutwardsWaveConditionsCalculation>(),
                                      calculationMapData);

            // When
            var calculationB = new GrassCoverErosionOutwardsWaveConditionsCalculation
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
            AssertCalculationsMapData(failureMechanism.Calculations.Cast<GrassCoverErosionOutwardsWaveConditionsCalculation>(), calculationMapData);
            mocks.VerifyAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void GivenViewWithCalculationInputData_WhenCalculationInputUpdatedAndNotified_ThenMapDataUpdated()
        {
            // Given
            var calculationA = new GrassCoverErosionOutwardsWaveConditionsCalculation
            {
                InputParameters =
                {
                    ForeshoreProfile = new TestForeshoreProfile(new Point2D(1.3, 1.3)),
                    HydraulicBoundaryLocation = new TestHydraulicBoundaryLocation()
                }
            };
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            failureMechanism.WaveConditionsCalculationGroup.Children.Add(calculationA);

            GrassCoverErosionOutwardsFailurePathView view = CreateView(failureMechanism, new AssessmentSectionStub());

            IMapControl map = ((RiskeerMapControl) view.Controls[0]).MapControl;

            var mocks = new MockRepository();
            IObserver[] observers = AttachMapDataObservers(mocks, map.Data.Collection);
            observers[simpleAssemblyObserverIndex].Expect(obs => obs.UpdateObserver());
            observers[detailedAssemblyObserverIndex].Expect(obs => obs.UpdateObserver());
            observers[tailorMadeAssemblyObserverIndex].Expect(obs => obs.UpdateObserver());
            observers[combinedAssemblyObserverIndex].Expect(obs => obs.UpdateObserver());
            mocks.ReplayAll();

            var calculationMapData = (MapLineData) map.Data.Collection.ElementAt(calculationsIndex);

            // Precondition
            AssertCalculationsMapData(failureMechanism.Calculations.Cast<GrassCoverErosionOutwardsWaveConditionsCalculation>(),
                                      calculationMapData);

            // When
            calculationA.InputParameters.ForeshoreProfile = new TestForeshoreProfile(new Point2D(1.5, 1.5));
            calculationA.InputParameters.NotifyObservers();

            // Then
            AssertCalculationsMapData(failureMechanism.Calculations.Cast<GrassCoverErosionOutwardsWaveConditionsCalculation>(), calculationMapData);
            mocks.VerifyAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void GivenViewWithCalculationData_WhenCalculationUpdatedAndNotified_ThenMapDataUpdated()
        {
            // Given
            var calculationA = new GrassCoverErosionOutwardsWaveConditionsCalculation
            {
                InputParameters =
                {
                    ForeshoreProfile = new TestForeshoreProfile(new Point2D(1.3, 1.3)),
                    HydraulicBoundaryLocation = new TestHydraulicBoundaryLocation()
                }
            };
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            failureMechanism.WaveConditionsCalculationGroup.Children.Add(calculationA);

            GrassCoverErosionOutwardsFailurePathView view = CreateView(failureMechanism, new AssessmentSectionStub());

            IMapControl map = ((RiskeerMapControl) view.Controls[0]).MapControl;

            var mocks = new MockRepository();
            IObserver[] observers = AttachMapDataObservers(mocks, map.Data.Collection);
            observers[simpleAssemblyObserverIndex].Expect(obs => obs.UpdateObserver());
            observers[detailedAssemblyObserverIndex].Expect(obs => obs.UpdateObserver());
            observers[tailorMadeAssemblyObserverIndex].Expect(obs => obs.UpdateObserver());
            observers[combinedAssemblyObserverIndex].Expect(obs => obs.UpdateObserver());
            mocks.ReplayAll();

            var calculationMapData = (MapLineData) map.Data.Collection.ElementAt(calculationsIndex);

            // Precondition
            AssertCalculationsMapData(failureMechanism.Calculations.Cast<GrassCoverErosionOutwardsWaveConditionsCalculation>(),
                                      calculationMapData);

            // When
            calculationA.Name = "new name";
            calculationA.NotifyObservers();

            // Then
            AssertCalculationsMapData(failureMechanism.Calculations.Cast<GrassCoverErosionOutwardsWaveConditionsCalculation>(), calculationMapData);
            mocks.VerifyAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void GivenViewWithAssemblyData_WhenFailureMechanismNotified_ThenMapDataUpdated()
        {
            // Given
            var random = new Random(39);
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            FailureMechanismTestHelper.AddSections(failureMechanism, random.Next(1, 10));

            var originalSimpleAssembly = new FailureMechanismSectionAssembly(random.NextDouble(), random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>());
            var originalDetailedAssemblyCategory = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>();
            var originalTailorMadeAssemblyCategory = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>();
            var originalCombinedAssemblyCategory = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>();

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                calculator.SimpleAssessmentAssemblyOutput = originalSimpleAssembly;
                calculator.DetailedAssessmentAssemblyGroupOutput = originalDetailedAssemblyCategory;
                calculator.TailorMadeAssemblyCategoryOutput = originalTailorMadeAssemblyCategory;
                calculator.CombinedAssemblyCategoryOutput = originalCombinedAssemblyCategory;

                GrassCoverErosionOutwardsFailurePathView view = CreateView(failureMechanism, new AssessmentSectionStub());

                IMapControl map = ((RiskeerMapControl) view.Controls[0]).MapControl;

                var mocks = new MockRepository();
                IObserver[] observers = AttachMapDataObservers(mocks, map.Data.Collection);
                observers[sectionsObserverIndex].Expect(obs => obs.UpdateObserver());
                observers[sectionsStartPointObserverIndex].Expect(obs => obs.UpdateObserver());
                observers[sectionsEndPointObserverIndex].Expect(obs => obs.UpdateObserver());
                observers[simpleAssemblyObserverIndex].Expect(obs => obs.UpdateObserver());
                observers[detailedAssemblyObserverIndex].Expect(obs => obs.UpdateObserver());
                observers[tailorMadeAssemblyObserverIndex].Expect(obs => obs.UpdateObserver());
                observers[combinedAssemblyObserverIndex].Expect(obs => obs.UpdateObserver());
                mocks.ReplayAll();

                // Precondition
                var assemblyMapData = (MapDataCollection) map.Data.Collection.ElementAt(assemblyResultsIndex);
                MapDataTestHelper.AssertAssemblyMapDataCollection(originalSimpleAssembly.Group,
                                                                  originalDetailedAssemblyCategory,
                                                                  originalTailorMadeAssemblyCategory,
                                                                  originalCombinedAssemblyCategory,
                                                                  assemblyMapData,
                                                                  failureMechanism);

                // When
                var updatedSimpleAssembly = new FailureMechanismSectionAssembly(random.NextDouble(), random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>());
                var updatedDetailedAssemblyCategory = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>();
                var updatedTailorMadeAssemblyCategory = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>();
                var updatedCombinedAssemblyCategory = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>();
                calculator.SimpleAssessmentAssemblyOutput = updatedSimpleAssembly;
                calculator.DetailedAssessmentAssemblyGroupOutput = updatedDetailedAssemblyCategory;
                calculator.TailorMadeAssemblyCategoryOutput = updatedTailorMadeAssemblyCategory;
                calculator.CombinedAssemblyCategoryOutput = updatedCombinedAssemblyCategory;
                failureMechanism.NotifyObservers();

                // Then
                MapDataTestHelper.AssertAssemblyMapDataCollection(updatedSimpleAssembly.Group,
                                                                  updatedDetailedAssemblyCategory,
                                                                  updatedTailorMadeAssemblyCategory,
                                                                  updatedCombinedAssemblyCategory,
                                                                  assemblyMapData,
                                                                  failureMechanism);
                mocks.VerifyAll();
            }
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void GivenViewWithAssemblyData_WhenCalculationNotified_ThenMapDataUpdated()
        {
            // Given
            var random = new Random(39);
            var calculationA = new GrassCoverErosionOutwardsWaveConditionsCalculation();

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            failureMechanism.WaveConditionsCalculationGroup.Children.Add(calculationA);
            FailureMechanismTestHelper.AddSections(failureMechanism, random.Next(1, 10));

            var originalSimpleAssembly = new FailureMechanismSectionAssembly(random.NextDouble(), random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>());
            var originalDetailedAssemblyCategory = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>();
            var originalTailorMadeAssemblyCategory = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>();
            var originalCombinedAssemblyCategory = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>();

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                calculator.SimpleAssessmentAssemblyOutput = originalSimpleAssembly;
                calculator.DetailedAssessmentAssemblyGroupOutput = originalDetailedAssemblyCategory;
                calculator.TailorMadeAssemblyCategoryOutput = originalTailorMadeAssemblyCategory;
                calculator.CombinedAssemblyCategoryOutput = originalCombinedAssemblyCategory;

                GrassCoverErosionOutwardsFailurePathView view = CreateView(failureMechanism, new AssessmentSectionStub());

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
                MapDataTestHelper.AssertAssemblyMapDataCollection(originalSimpleAssembly.Group,
                                                                  originalDetailedAssemblyCategory,
                                                                  originalTailorMadeAssemblyCategory,
                                                                  originalCombinedAssemblyCategory,
                                                                  assemblyMapData,
                                                                  failureMechanism);

                // When
                var updatedSimpleAssembly = new FailureMechanismSectionAssembly(random.NextDouble(), random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>());
                var updatedDetailedAssemblyCategory = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>();
                var updatedTailorMadeAssemblyCategory = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>();
                var updatedCombinedAssemblyCategory = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>();
                calculator.SimpleAssessmentAssemblyOutput = updatedSimpleAssembly;
                calculator.DetailedAssessmentAssemblyGroupOutput = updatedDetailedAssemblyCategory;
                calculator.TailorMadeAssemblyCategoryOutput = updatedTailorMadeAssemblyCategory;
                calculator.CombinedAssemblyCategoryOutput = updatedCombinedAssemblyCategory;
                calculationA.NotifyObservers();

                // Then
                MapDataTestHelper.AssertAssemblyMapDataCollection(updatedSimpleAssembly.Group,
                                                                  updatedDetailedAssemblyCategory,
                                                                  updatedTailorMadeAssemblyCategory,
                                                                  updatedCombinedAssemblyCategory,
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
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            FailureMechanismTestHelper.AddSections(failureMechanism, random.Next(1, 10));

            var originalSimpleAssembly = new FailureMechanismSectionAssembly(random.NextDouble(), random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>());
            var originalDetailedAssemblyCategory = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>();
            var originalTailorMadeAssemblyCategory = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>();
            var originalCombinedAssemblyCategory = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>();

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                calculator.SimpleAssessmentAssemblyOutput = originalSimpleAssembly;
                calculator.DetailedAssessmentAssemblyGroupOutput = originalDetailedAssemblyCategory;
                calculator.TailorMadeAssemblyCategoryOutput = originalTailorMadeAssemblyCategory;
                calculator.CombinedAssemblyCategoryOutput = originalCombinedAssemblyCategory;

                GrassCoverErosionOutwardsFailurePathView view = CreateView(failureMechanism, new AssessmentSectionStub());

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
                MapDataTestHelper.AssertAssemblyMapDataCollection(originalSimpleAssembly.Group,
                                                                  originalDetailedAssemblyCategory,
                                                                  originalTailorMadeAssemblyCategory,
                                                                  originalCombinedAssemblyCategory,
                                                                  assemblyMapData,
                                                                  failureMechanism);

                // When
                var updatedSimpleAssembly = new FailureMechanismSectionAssembly(random.NextDouble(), random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>());
                var updatedDetailedAssemblyCategory = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>();
                var updatedTailorMadeAssemblyCategory = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>();
                var updatedCombinedAssemblyCategory = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>();
                calculator.SimpleAssessmentAssemblyOutput = updatedSimpleAssembly;
                calculator.DetailedAssessmentAssemblyGroupOutput = updatedDetailedAssemblyCategory;
                calculator.TailorMadeAssemblyCategoryOutput = updatedTailorMadeAssemblyCategory;
                calculator.CombinedAssemblyCategoryOutput = updatedCombinedAssemblyCategory;
                failureMechanism.SectionResults.First().NotifyObservers();

                // Then
                MapDataTestHelper.AssertAssemblyMapDataCollection(updatedSimpleAssembly.Group,
                                                                  updatedDetailedAssemblyCategory,
                                                                  updatedTailorMadeAssemblyCategory,
                                                                  updatedCombinedAssemblyCategory,
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
            const int updatedReferenceLineLayerIndex = referenceLineIndex + 5;
            const int updatedSectionsCollectionLayerIndex = sectionsCollectionIndex - 1;
            const int updatedAssemblyResultsCollectionIndex = assemblyResultsIndex - 1;
            const int updatedHydraulicLocationsLayerIndex = hydraulicBoundaryLocationsIndex - 1;
            const int updatedForeshoreProfilesLayerIndex = foreshoreProfilesIndex - 1;
            const int updatedCalculationsIndex = calculationsIndex - 1;

            var assessmentSection = new AssessmentSectionStub();
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();

            GrassCoverErosionOutwardsFailurePathView view = CreateView(failureMechanism, assessmentSection);

            IMapControl map = ((RiskeerMapControl) view.Controls[0]).MapControl;

            MapDataCollection mapData = map.Data;

            var dataToMove = (MapLineData) map.Data.Collection.ElementAt(referenceLineIndex);
            mapData.Remove(dataToMove);
            mapData.Add(dataToMove);

            IEnumerable<MapData> mapDataList = mapData.Collection;

            // Precondition
            var referenceLineData = (MapLineData) mapDataList.ElementAt(updatedReferenceLineLayerIndex);
            Assert.AreEqual("Referentielijn", referenceLineData.Name);

            var sectionsData = (MapDataCollection) mapDataList.ElementAt(updatedSectionsCollectionLayerIndex);
            Assert.AreEqual("Vakindeling", sectionsData.Name);

            var assemblyResultsData = (MapDataCollection) mapDataList.ElementAt(updatedAssemblyResultsCollectionIndex);
            Assert.AreEqual("Toetsoordeel", assemblyResultsData.Name);

            var hydraulicLocationsData = (MapPointData) mapDataList.ElementAt(updatedHydraulicLocationsLayerIndex);
            Assert.AreEqual("Hydraulische belastingen", hydraulicLocationsData.Name);

            var foreshoreProfilesData = (MapLineData) mapDataList.ElementAt(updatedForeshoreProfilesLayerIndex);
            Assert.AreEqual("Voorlandprofielen", foreshoreProfilesData.Name);

            var calculationsData = (MapLineData) mapDataList.ElementAt(updatedCalculationsIndex);
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
            var actualReferenceLineData = (MapLineData) mapDataList.ElementAt(updatedReferenceLineLayerIndex);
            Assert.AreEqual("Referentielijn", actualReferenceLineData.Name);

            var actualSectionsData = (MapDataCollection) mapDataList.ElementAt(updatedSectionsCollectionLayerIndex);
            Assert.AreEqual("Vakindeling", actualSectionsData.Name);

            var actualAssemblyResultsData = (MapDataCollection) mapDataList.ElementAt(updatedAssemblyResultsCollectionIndex);
            Assert.AreEqual("Toetsoordeel", actualAssemblyResultsData.Name);

            var actualHydraulicLocationsData = (MapPointData) mapDataList.ElementAt(updatedHydraulicLocationsLayerIndex);
            Assert.AreEqual("Hydraulische belastingen", actualHydraulicLocationsData.Name);

            var actualForeshoreProfilesData = (MapLineData) mapDataList.ElementAt(updatedForeshoreProfilesLayerIndex);
            Assert.AreEqual("Voorlandprofielen", actualForeshoreProfilesData.Name);

            var actualCalculationsData = (MapLineData) mapDataList.ElementAt(updatedCalculationsIndex);
            Assert.AreEqual("Berekeningen", actualCalculationsData.Name);
        }

        private GrassCoverErosionOutwardsFailurePathView CreateView(GrassCoverErosionOutwardsFailureMechanism failureMechanism, IAssessmentSection assessmentSection)
        {
            var view = new GrassCoverErosionOutwardsFailurePathView(failureMechanism, assessmentSection);

            testForm.Controls.Add(view);
            testForm.Show();

            return view;
        }

        private static void AssertCalculationsMapData(IEnumerable<GrassCoverErosionOutwardsWaveConditionsCalculation> calculations, MapData mapData)
        {
            Assert.IsInstanceOf<MapLineData>(mapData);
            var calculationsMapData = (MapLineData) mapData;
            GrassCoverErosionOutwardsWaveConditionsCalculation[] calculationsArray = calculations.ToArray();
            MapFeature[] calculationsFeatures = calculationsMapData.Features.ToArray();
            Assert.AreEqual(calculationsArray.Length, calculationsFeatures.Length);

            for (var index = 0; index < calculationsArray.Length; index++)
            {
                MapGeometry[] geometries = calculationsFeatures[index].MapGeometries.ToArray();
                Assert.AreEqual(1, geometries.Length);

                GrassCoverErosionOutwardsWaveConditionsCalculation calculation = calculationsArray[index];
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
            Assert.AreEqual("Grasbekleding erosie buitentalud", mapDataCollection.Name);

            List<MapData> mapDataList = mapDataCollection.Collection.ToList();

            Assert.AreEqual(6, mapDataList.Count);

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

            MapData[] sectionsCollection = ((MapDataCollection) mapDataArray[sectionsCollectionIndex]).Collection.ToArray();
            var sectionsMapDataObserver = mocks.StrictMock<IObserver>();
            sectionsCollection[sectionsIndex].Attach(sectionsMapDataObserver);

            var sectionsStartPointMapDataObserver = mocks.StrictMock<IObserver>();
            sectionsCollection[sectionsStartPointIndex].Attach(sectionsStartPointMapDataObserver);

            var sectionsEndPointMapDataObserver = mocks.StrictMock<IObserver>();
            sectionsCollection[sectionsEndPointIndex].Attach(sectionsEndPointMapDataObserver);

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
                sectionsMapDataObserver,
                sectionsStartPointMapDataObserver,
                sectionsEndPointMapDataObserver,
                simpleAssemblyMapDataObserver,
                detailedAssemblyMapDataObserver,
                tailorMadeAssemblyMapDataObserver,
                combinedAssemblyMapDataObserver
            };
        }
    }
}