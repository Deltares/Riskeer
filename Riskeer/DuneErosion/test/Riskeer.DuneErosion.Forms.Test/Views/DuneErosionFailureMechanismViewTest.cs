// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using Core.Components.Gis.Data;
using Core.Components.Gis.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.TestUtil;
using Ringtoets.Common.Forms.Views;
using Ringtoets.DuneErosion.Data;
using Ringtoets.DuneErosion.Data.TestUtil;
using Ringtoets.DuneErosion.Forms.TestUtil;
using Ringtoets.DuneErosion.Forms.Views;
using Riskeer.AssemblyTool.Data;
using Riskeer.AssemblyTool.KernelWrapper.Calculators;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil.Calculators;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil.Calculators.Assembly;

namespace Ringtoets.DuneErosion.Forms.Test.Views
{
    [TestFixture]
    public class DuneErosionFailureMechanismViewTest
    {
        private const int referenceLineIndex = 0;
        private const int sectionsCollectionIndex = 1;
        private const int assemblyResultsIndex = 2;
        private const int duneLocationsIndex = 3;

        private const int sectionsIndex = 0;
        private const int sectionsStartPointIndex = 1;
        private const int sectionsEndPointIndex = 2;

        private const int tailorMadeAssemblyIndex = 0;
        private const int detailedAssemblyIndex = 1;
        private const int simpleAssemblyIndex = 2;
        private const int combinedAssemblyIndex = 3;

        private const int duneLocationsObserverIndex = 1;
        private const int sectionsObserverIndex = 2;
        private const int sectionsStartPointObserverIndex = 3;
        private const int sectionsEndPointObserverIndex = 4;
        private const int simpleAssemblyObserverIndex = 5;
        private const int detailedAssemblyObserverIndex = 6;
        private const int tailorMadeAssemblyObserverIndex = 7;
        private const int combinedAssemblyObserverIndex = 8;

        private static IEnumerable<TestCaseData> GetCalculationFuncs
        {
            get
            {
                yield return new TestCaseData(new Func<DuneErosionFailureMechanism, DuneLocationCalculation>(
                                                  failureMechanism => failureMechanism.CalculationsForMechanismSpecificFactorizedSignalingNorm.First()))
                    .SetName("Mechanism specific factorized signaling norm");
                yield return new TestCaseData(new Func<DuneErosionFailureMechanism, DuneLocationCalculation>(
                                                  failureMechanism => failureMechanism.CalculationsForMechanismSpecificSignalingNorm.First()))
                    .SetName("Mechanism specific signaling norm");
                yield return new TestCaseData(new Func<DuneErosionFailureMechanism, DuneLocationCalculation>(
                                                  failureMechanism => failureMechanism.CalculationsForMechanismSpecificLowerLimitNorm.First()))
                    .SetName("Mechanism specific lower limit norm");
                yield return new TestCaseData(new Func<DuneErosionFailureMechanism, DuneLocationCalculation>(
                                                  failureMechanism => failureMechanism.CalculationsForLowerLimitNorm.First()))
                    .SetName("Lower limit norm");
                yield return new TestCaseData(new Func<DuneErosionFailureMechanism, DuneLocationCalculation>(
                                                  failureMechanism => failureMechanism.CalculationsForFactorizedLowerLimitNorm.First()))
                    .SetName("Factorized lower limit norm");
            }
        }

        [Test]
        public void Constructor_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => new DuneErosionFailureMechanismView(null, assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new DuneErosionFailureMechanismView(new DuneErosionFailureMechanism(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var failureMechanism = new DuneErosionFailureMechanism();
            var assessmentSection = new AssessmentSectionStub();

            // Call
            using (var view = new DuneErosionFailureMechanismView(failureMechanism, assessmentSection))
            {
                // Assert
                Assert.IsInstanceOf<UserControl>(view);
                Assert.IsInstanceOf<IMapView>(view);
                Assert.IsNull(view.Data);
                Assert.AreSame(failureMechanism, view.FailureMechanism);
                Assert.AreSame(assessmentSection, view.AssessmentSection);

                Assert.AreEqual(1, view.Controls.Count);
                Assert.IsInstanceOf<RingtoetsMapControl>(view.Controls[0]);
                Assert.AreSame(view.Map, ((RingtoetsMapControl) view.Controls[0]).MapControl);
                Assert.AreEqual(DockStyle.Fill, ((Control) view.Map).Dock);
                AssertEmptyMapData(view.Map.Data);
            }
        }

        [Test]
        public void Constructor_AssessmentSectionWithBackgroundData_BackgroundDataSet()
        {
            // Setup
            IAssessmentSection assessmentSection = new AssessmentSectionStub();

            // Call
            using (var view = new DuneErosionFailureMechanismView(new DuneErosionFailureMechanism(), assessmentSection))
            {
                // Assert
                MapDataTestHelper.AssertImageBasedMapData(assessmentSection.BackgroundData, view.Map.BackgroundMapData);
            }
        }

        [Test]
        public void Constructor_WithAllData_DataUpdatedToCollectionOfFilledMapData()
        {
            // Setup
            var random = new Random(39);

            var assessmentSection = new AssessmentSectionStub
            {
                ReferenceLine = new ReferenceLine()
            };
            assessmentSection.ReferenceLine.SetGeometry(new[]
            {
                new Point2D(1.0, 2.0),
                new Point2D(2.0, 1.0)
            });

            var geometryPoints = new[]
            {
                new Point2D(0.0, 0.0),
                new Point2D(2.0, 0.0),
                new Point2D(4.0, 4.0),
                new Point2D(6.0, 4.0)
            };

            var failureMechanism = new DuneErosionFailureMechanism();
            failureMechanism.SetDuneLocations(new[]
            {
                new TestDuneLocation()
            });
            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                new FailureMechanismSection("A", geometryPoints.Take(2)),
                new FailureMechanismSection("B", geometryPoints.Skip(1).Take(2)),
                new FailureMechanismSection("C", geometryPoints.Skip(2).Take(2))
            });

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
                using (var view = new DuneErosionFailureMechanismView(failureMechanism, assessmentSection))
                {
                    IMapControl map = ((RingtoetsMapControl) view.Controls[0]).MapControl;

                    // Assert
                    MapDataCollection mapData = map.Data;
                    Assert.IsInstanceOf<MapDataCollection>(mapData);

                    List<MapData> mapDataList = mapData.Collection.ToList();
                    Assert.AreEqual(4, mapDataList.Count);
                    MapDataTestHelper.AssertReferenceLineMapData(assessmentSection.ReferenceLine, mapDataList[referenceLineIndex]);

                    IEnumerable<MapData> sectionsCollection = ((MapDataCollection) mapDataList[sectionsCollectionIndex]).Collection;
                    MapDataTestHelper.AssertFailureMechanismSectionsMapData(failureMechanism.Sections, sectionsCollection.ElementAt(sectionsIndex));
                    MapDataTestHelper.AssertFailureMechanismSectionsStartPointMapData(failureMechanism.Sections, sectionsCollection.ElementAt(sectionsStartPointIndex));
                    MapDataTestHelper.AssertFailureMechanismSectionsEndPointMapData(failureMechanism.Sections, sectionsCollection.ElementAt(sectionsEndPointIndex));

                    AssertDuneLocationsMapData(failureMechanism, mapDataList[duneLocationsIndex]);

                    MapDataTestHelper.AssertAssemblyMapDataCollection(expectedSimpleAssembly.Group,
                                                                      expectedDetailedAssemblyCategory,
                                                                      expectedTailorMadeAssemblyCategory,
                                                                      expectedCombinedAssemblyCategory,
                                                                      (MapDataCollection) mapDataList[assemblyResultsIndex],
                                                                      failureMechanism);
                }
            }
        }

        [Test]
        public void GivenViewWithDuneLocationsData_WhenHydraulicBoundaryDatabaseUpdatedAndNotified_ThenMapDataUpdated()
        {
            // Given
            var assessmentSection = new AssessmentSectionStub();
            var failureMechanism = new DuneErosionFailureMechanism();
            failureMechanism.SetDuneLocations(new[]
            {
                new TestDuneLocation()
            });

            using (var view = new DuneErosionFailureMechanismView(failureMechanism, assessmentSection))
            {
                IMapControl map = ((RingtoetsMapControl) view.Controls[0]).MapControl;

                var mocks = new MockRepository();
                IObserver[] observers = AttachMapDataObservers(mocks, map.Data.Collection);
                observers[duneLocationsObserverIndex].Expect(obs => obs.UpdateObserver());
                mocks.ReplayAll();

                MapData hydraulicBoundaryLocationsMapData = map.Data.Collection.ElementAt(duneLocationsIndex);

                // Precondition
                AssertDuneLocationsMapData(failureMechanism, hydraulicBoundaryLocationsMapData);

                // When
                failureMechanism.SetDuneLocations(new[]
                {
                    new TestDuneLocation()
                });
                failureMechanism.DuneLocations.NotifyObservers();

                // Then
                AssertDuneLocationsMapData(failureMechanism, hydraulicBoundaryLocationsMapData);
                mocks.VerifyAll();
            }
        }

        [Test]
        [TestCaseSource(nameof(GetCalculationFuncs))]
        public void GivenViewWithDuneLocationsData_WhenDuneLocationCalculationUpdatedAndNotified_ThenMapDataUpdated(
            Func<DuneErosionFailureMechanism, DuneLocationCalculation> getCalculationFunc)
        {
            // Given
            var assessmentSection = new AssessmentSectionStub();
            var failureMechanism = new DuneErosionFailureMechanism();
            failureMechanism.SetDuneLocations(new[]
            {
                new TestDuneLocation()
            });

            using (var view = new DuneErosionFailureMechanismView(failureMechanism, assessmentSection))
            {
                IMapControl map = ((RingtoetsMapControl) view.Controls[0]).MapControl;

                var mocks = new MockRepository();
                IObserver[] observers = AttachMapDataObservers(mocks, map.Data.Collection);
                observers[duneLocationsObserverIndex].Expect(obs => obs.UpdateObserver());
                mocks.ReplayAll();

                MapData hydraulicBoundaryLocationsMapData = map.Data.Collection.ElementAt(duneLocationsIndex);

                // Precondition
                AssertDuneLocationsMapData(failureMechanism, hydraulicBoundaryLocationsMapData);

                // When
                DuneLocationCalculation duneLocationCalculation = getCalculationFunc(failureMechanism);
                duneLocationCalculation.Output = new TestDuneLocationCalculationOutput();
                duneLocationCalculation.NotifyObservers();

                // Then
                AssertDuneLocationsMapData(failureMechanism, hydraulicBoundaryLocationsMapData);
                mocks.VerifyAll();
            }
        }

        [Test]
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

            using (var view = new DuneErosionFailureMechanismView(new DuneErosionFailureMechanism(), assessmentSection))
            {
                IMapControl map = ((RingtoetsMapControl) view.Controls[0]).MapControl;

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
        }

        [Test]
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

            using (var view = new DuneErosionFailureMechanismView(new DuneErosionFailureMechanism(), assessmentSection))
            {
                IMapControl map = ((RingtoetsMapControl) view.Controls[0]).MapControl;

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
        }

        [Test]
        public void GivenViewWithFailureMechanismSectionsData_WhenFailureMechanismSectionsUpdatedAndNotified_ThenMapDataUpdated()
        {
            // Given
            var failureMechanism = new DuneErosionFailureMechanism();

            using (var view = new DuneErosionFailureMechanismView(failureMechanism, new AssessmentSectionStub()))
            {
                IMapControl map = ((RingtoetsMapControl) view.Controls[0]).MapControl;

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
        }

        [Test]
        public void GivenViewWithAssemblyData_WhenFailureMechanismNotified_ThenMapDataUpdated()
        {
            // Given
            var random = new Random(39);
            var failureMechanism = new DuneErosionFailureMechanism();
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

                using (var view = new DuneErosionFailureMechanismView(failureMechanism, new AssessmentSectionStub()))
                {
                    IMapControl map = ((RingtoetsMapControl) view.Controls[0]).MapControl;

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
        }

        [Test]
        public void GivenViewWithAssemblyData_WhenFailureMechanismSectionResultNotified_ThenMapDataUpdated()
        {
            // Given
            var random = new Random(39);
            var failureMechanism = new DuneErosionFailureMechanism();
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

                using (var view = new DuneErosionFailureMechanismView(failureMechanism, new AssessmentSectionStub()))
                {
                    IMapControl map = ((RingtoetsMapControl) view.Controls[0]).MapControl;

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
        }

        [Test]
        public void UpdateObserver_DataUpdated_MapLayersSameOrder()
        {
            // Setup
            const int updatedReferenceLineLayerIndex = referenceLineIndex + 3;
            const int updatedSectionsCollectionLayerIndex = sectionsCollectionIndex - 1;
            const int updatedAssemblyResultsCollectionIndex = assemblyResultsIndex - 1;
            const int updatedDuneLocationsLayerIndex = duneLocationsIndex - 1;

            var assessmentSection = new AssessmentSectionStub();
            var failureMechanism = new DuneErosionFailureMechanism();

            using (var view = new DuneErosionFailureMechanismView(failureMechanism, assessmentSection))
            {
                IMapControl map = ((RingtoetsMapControl) view.Controls[0]).MapControl;

                MapDataCollection mapData = map.Data;

                var dataToMove = (MapLineData) map.Data.Collection.ElementAt(referenceLineIndex);
                mapData.Remove(dataToMove);
                mapData.Add(dataToMove);

                List<MapData> mapDataList = mapData.Collection.ToList();

                // Precondition
                var referenceLineData = (MapLineData) mapDataList[updatedReferenceLineLayerIndex];
                Assert.AreEqual("Referentielijn", referenceLineData.Name);

                var sectionsData = (MapDataCollection) mapDataList[updatedSectionsCollectionLayerIndex];
                Assert.AreEqual("Vakindeling", sectionsData.Name);

                var assemblyResultsData = (MapDataCollection) mapDataList[updatedAssemblyResultsCollectionIndex];
                Assert.AreEqual("Toetsoordeel", assemblyResultsData.Name);

                var duneLocationsData = (MapPointData) mapDataList[updatedDuneLocationsLayerIndex];
                Assert.AreEqual("Hydraulische belastingen", duneLocationsData.Name);

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

                var actualSectionsData = (MapDataCollection) mapDataList[updatedSectionsCollectionLayerIndex];
                Assert.AreEqual("Vakindeling", actualSectionsData.Name);

                var actualAssemblyResultsData = (MapDataCollection) mapDataList[updatedAssemblyResultsCollectionIndex];
                Assert.AreEqual("Toetsoordeel", actualAssemblyResultsData.Name);

                var actualDuneLocationsData = (MapPointData) mapDataList[updatedDuneLocationsLayerIndex];
                Assert.AreEqual("Hydraulische belastingen", actualDuneLocationsData.Name);
            }
        }

        private static void AssertEmptyMapData(MapDataCollection mapDataCollection)
        {
            Assert.AreEqual("Duinwaterkering - Duinafslag", mapDataCollection.Name);

            List<MapData> mapDataList = mapDataCollection.Collection.ToList();

            Assert.AreEqual(4, mapDataList.Count);

            var referenceLineMapData = (MapLineData) mapDataList[referenceLineIndex];
            var hydraulicBoundaryLocationsMapData = (MapPointData) mapDataList[duneLocationsIndex];

            CollectionAssert.IsEmpty(referenceLineMapData.Features);
            CollectionAssert.IsEmpty(hydraulicBoundaryLocationsMapData.Features);

            Assert.AreEqual("Referentielijn", referenceLineMapData.Name);
            Assert.AreEqual("Hydraulische belastingen", hydraulicBoundaryLocationsMapData.Name);

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

            var referenceLineMapDataObserver = mocks.StrictMock<IObserver>();
            mapDataArray[referenceLineIndex].Attach(referenceLineMapDataObserver);

            var duneLocationsMapDataObserver = mocks.StrictMock<IObserver>();
            mapDataArray[duneLocationsIndex].Attach(duneLocationsMapDataObserver);

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
                referenceLineMapDataObserver,
                duneLocationsMapDataObserver,
                sectionsMapDataObserver,
                sectionsStartPointMapDataObserver,
                sectionsEndPointMapDataObserver,
                simpleAssemblyMapDataObserver,
                detailedAssemblyMapDataObserver,
                tailorMadeAssemblyMapDataObserver,
                combinedAssemblyMapDataObserver
            };
        }

        private static void AssertDuneLocationsMapData(DuneErosionFailureMechanism failureMechanism,
                                                       MapData mapData)
        {
            Assert.IsInstanceOf<MapPointData>(mapData);
            Assert.AreEqual("Hydraulische belastingen", mapData.Name);

            var duneLocationsMapData = (MapPointData) mapData;
            DuneErosionMapFeaturesTestHelper.AssertDuneLocationFeaturesData(failureMechanism, duneLocationsMapData.Features);
        }
    }
}