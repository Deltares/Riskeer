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
using Core.Components.Gis.Features;
using Core.Components.Gis.Forms;
using Core.Components.Gis.Geometries;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Calculators;
using Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Calculators.Assembly;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.TestUtil;
using Ringtoets.Common.Forms.Views;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionInwards.Forms.Views;
using Riskeer.AssemblyTool.Data;
using Riskeer.AssemblyTool.KernelWrapper.Calculators;

namespace Ringtoets.GrassCoverErosionInwards.Forms.Test.Views
{
    [TestFixture]
    public class GrassCoverErosionInwardsFailureMechanismViewTest
    {
        private const int referenceLineIndex = 0;
        private const int sectionsCollectionIndex = 1;
        private const int assemblyResultsIndex = 2;
        private const int hydraulicBoundaryLocationsIndex = 3;
        private const int dikeProfilesIndex = 4;
        private const int foreshoreProfilesIndex = 5;
        private const int calculationsIndex = 6;

        private const int sectionsIndex = 0;
        private const int sectionsStartPointIndex = 1;
        private const int sectionsEndPointIndex = 2;

        private const int tailorMadeAssemblyIndex = 0;
        private const int detailedAssemblyIndex = 1;
        private const int simpleAssemblyIndex = 2;
        private const int combinedAssemblyIndex = 3;

        private const int hydraulicBoundaryLocationsObserverIndex = 1;
        private const int dikeProfilesObserverIndex = 2;
        private const int foreshoreProfileObserverIndex = 3;
        private const int calculationObserverIndex = 4;
        private const int sectionsObserverIndex = 5;
        private const int sectionsStartPointObserverIndex = 6;
        private const int sectionsEndPointObserverIndex = 7;
        private const int simpleAssemblyObserverIndex = 8;
        private const int detailedAssemblyObserverIndex = 9;
        private const int tailorMadeAssemblyObserverIndex = 10;
        private const int combinedAssemblyObserverIndex = 11;

        [Test]
        public void Constructor_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => new GrassCoverErosionInwardsFailureMechanismView(null, assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new GrassCoverErosionInwardsFailureMechanismView(new GrassCoverErosionInwardsFailureMechanism(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var assessmentSection = new AssessmentSectionStub();

            // Call
            using (var view = new GrassCoverErosionInwardsFailureMechanismView(failureMechanism, assessmentSection))
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
            using (var view = new GrassCoverErosionInwardsFailureMechanismView(new GrassCoverErosionInwardsFailureMechanism(), assessmentSection))
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

            var calculationA = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    DikeProfile = DikeProfileTestFactory.CreateDikeProfile(new Point2D(1.3, 1.3)),
                    HydraulicBoundaryLocation = new TestHydraulicBoundaryLocation()
                }
            };
            var calculationB = new GrassCoverErosionInwardsCalculation
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
                using (var view = new GrassCoverErosionInwardsFailureMechanismView(failureMechanism, assessmentSection))
                {
                    IMapControl map = ((RingtoetsMapControl) view.Controls[0]).MapControl;

                    // Assert
                    MapDataCollection mapData = map.Data;
                    Assert.IsInstanceOf<MapDataCollection>(mapData);

                    List<MapData> mapDataList = mapData.Collection.ToList();
                    Assert.AreEqual(7, mapDataList.Count);
                    MapDataTestHelper.AssertReferenceLineMapData(assessmentSection.ReferenceLine, mapDataList[referenceLineIndex]);

                    IEnumerable<MapData> sectionsCollection = ((MapDataCollection) mapDataList[sectionsCollectionIndex]).Collection;
                    MapDataTestHelper.AssertFailureMechanismSectionsMapData(failureMechanism.Sections, sectionsCollection.ElementAt(sectionsIndex));
                    MapDataTestHelper.AssertFailureMechanismSectionsStartPointMapData(failureMechanism.Sections, sectionsCollection.ElementAt(sectionsStartPointIndex));
                    MapDataTestHelper.AssertFailureMechanismSectionsEndPointMapData(failureMechanism.Sections, sectionsCollection.ElementAt(sectionsEndPointIndex));

                    MapDataTestHelper.AssertHydraulicBoundaryLocationsMapData(assessmentSection, mapDataList[hydraulicBoundaryLocationsIndex]);
                    AssertDikeProfiles(failureMechanism.DikeProfiles, mapDataList[dikeProfilesIndex]);
                    MapDataTestHelper.AssertForeshoreProfilesMapData(failureMechanism.DikeProfiles.Select(dp => dp.ForeshoreProfile), mapDataList[foreshoreProfilesIndex]);
                    AssertCalculationsMapData(failureMechanism.Calculations.Cast<GrassCoverErosionInwardsCalculation>(), mapDataList[calculationsIndex]);

                    MapDataTestHelper.AssertAssemblyMapDataCollection(expectedSimpleAssembly,
                                                                      expectedDetailedAssembly,
                                                                      expectedTailorMadeAssembly,
                                                                      expectedCombinedAssembly,
                                                                      (MapDataCollection) mapDataList[assemblyResultsIndex],
                                                                      failureMechanism);
                }
            }
        }

        [Test]
        public void GivenViewWithHydraulicBoundaryLocationsData_WhenHydraulicBoundaryLocationsUpdatedAndNotified_ThenMapDataUpdated()
        {
            // Given
            var assessmentSection = new AssessmentSectionStub();
            assessmentSection.SetHydraulicBoundaryLocationCalculations(new[]
            {
                new HydraulicBoundaryLocation(1, "test1", 1.0, 2.0)
            });

            using (var view = new GrassCoverErosionInwardsFailureMechanismView(new GrassCoverErosionInwardsFailureMechanism(), assessmentSection))
            {
                IMapControl map = ((RingtoetsMapControl) view.Controls[0]).MapControl;

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
        }

        [Test]
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

            using (var view = new GrassCoverErosionInwardsFailureMechanismView(new GrassCoverErosionInwardsFailureMechanism(), assessmentSection))
            {
                IMapControl map = ((RingtoetsMapControl) view.Controls[0]).MapControl;

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

            using (var view = new GrassCoverErosionInwardsFailureMechanismView(new GrassCoverErosionInwardsFailureMechanism(), assessmentSection))
            {
                IMapControl map = ((RingtoetsMapControl) view.Controls[0]).MapControl;

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

            using (var view = new GrassCoverErosionInwardsFailureMechanismView(new GrassCoverErosionInwardsFailureMechanism(), assessmentSection))
            {
                IMapControl map = ((RingtoetsMapControl) view.Controls[0]).MapControl;

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
        }

        [Test]
        public void GivenViewWithFailureMechanismSectionsData_WhenFailureMechanismSectionsUpdatedAndNotified_ThenMapDataUpdated()
        {
            // Given
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            using (var view = new GrassCoverErosionInwardsFailureMechanismView(failureMechanism, new AssessmentSectionStub()))
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
        public void GivenViewWithDikeProfilesData_WhenDikeProfilesUpdatedAndNotified_ThenMapDataUpdated()
        {
            // Given
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            failureMechanism.DikeProfiles.AddRange(new[]
            {
                DikeProfileTestFactory.CreateDikeProfile(string.Empty, "id1")
            }, "path");

            using (var view = new GrassCoverErosionInwardsFailureMechanismView(failureMechanism, new AssessmentSectionStub()))
            {
                IMapControl map = ((RingtoetsMapControl) view.Controls[0]).MapControl;

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
        }

        [Test]
        public void GivenViewWithDikeProfileData_WhenDikeProfileUpdatedAndNotified_ThenMapDataUpdated()
        {
            // Given
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            DikeProfile dikeProfile = DikeProfileTestFactory.CreateDikeProfile(string.Empty, "id1");
            failureMechanism.DikeProfiles.AddRange(new[]
            {
                dikeProfile
            }, "path");

            using (var view = new GrassCoverErosionInwardsFailureMechanismView(failureMechanism, new AssessmentSectionStub()))
            {
                IMapControl map = ((RingtoetsMapControl) view.Controls[0]).MapControl;

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
        }

        [Test]
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

            using (var view = new GrassCoverErosionInwardsFailureMechanismView(failureMechanism, new AssessmentSectionStub()))
            {
                IMapControl map = ((RingtoetsMapControl) view.Controls[0]).MapControl;

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
        }

        [Test]
        public void GivenViewWithCalculationGroupData_WhenCalculationGroupUpdatedAndNotified_ThenMapDataUpdated()
        {
            // Given
            var calculationA = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    DikeProfile = DikeProfileTestFactory.CreateDikeProfile(new Point2D(1.3, 1.3)),
                    HydraulicBoundaryLocation = new TestHydraulicBoundaryLocation()
                }
            };
            var calculationB = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    DikeProfile = DikeProfileTestFactory.CreateDikeProfile(new Point2D(1.5, 1.5)),
                    HydraulicBoundaryLocation = new TestHydraulicBoundaryLocation()
                }
            };
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(calculationA);

            using (var view = new GrassCoverErosionInwardsFailureMechanismView(failureMechanism, new AssessmentSectionStub()))
            {
                IMapControl map = ((RingtoetsMapControl) view.Controls[0]).MapControl;

                var calculationMapData = (MapLineData) map.Data.Collection.ElementAt(calculationsIndex);
                var mocks = new MockRepository();
                IObserver[] observers = AttachMapDataObservers(mocks, map.Data.Collection);
                observers[calculationObserverIndex].Expect(obs => obs.UpdateObserver());
                observers[simpleAssemblyObserverIndex].Expect(obs => obs.UpdateObserver());
                observers[detailedAssemblyObserverIndex].Expect(obs => obs.UpdateObserver());
                observers[tailorMadeAssemblyObserverIndex].Expect(obs => obs.UpdateObserver());
                observers[combinedAssemblyObserverIndex].Expect(obs => obs.UpdateObserver());
                mocks.ReplayAll();

                // When
                failureMechanism.CalculationsGroup.Children.Add(calculationB);
                failureMechanism.CalculationsGroup.NotifyObservers();

                // Then
                AssertCalculationsMapData(failureMechanism.Calculations.Cast<GrassCoverErosionInwardsCalculation>(), calculationMapData);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void GivenViewWithCalculationInputData_WhenCalculationInputUpdatedAndNotified_ThenMapDataUpdated()
        {
            // Given
            var calculationA = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    DikeProfile = DikeProfileTestFactory.CreateDikeProfile(new Point2D(1.3, 1.3)),
                    HydraulicBoundaryLocation = new TestHydraulicBoundaryLocation()
                }
            };

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(calculationA);

            using (var view = new GrassCoverErosionInwardsFailureMechanismView(failureMechanism, new AssessmentSectionStub()))
            {
                IMapControl map = ((RingtoetsMapControl) view.Controls[0]).MapControl;

                var calculationMapData = (MapLineData) map.Data.Collection.ElementAt(calculationsIndex);

                var mocks = new MockRepository();
                IObserver[] observers = AttachMapDataObservers(mocks, map.Data.Collection);
                observers[calculationObserverIndex].Expect(obs => obs.UpdateObserver());
                observers[simpleAssemblyObserverIndex].Expect(obs => obs.UpdateObserver());
                observers[detailedAssemblyObserverIndex].Expect(obs => obs.UpdateObserver());
                observers[tailorMadeAssemblyObserverIndex].Expect(obs => obs.UpdateObserver());
                observers[combinedAssemblyObserverIndex].Expect(obs => obs.UpdateObserver());
                mocks.ReplayAll();

                // When
                calculationA.InputParameters.DikeProfile = DikeProfileTestFactory.CreateDikeProfile(new Point2D(1.5, 1.5));
                calculationA.InputParameters.NotifyObservers();

                // Then
                AssertCalculationsMapData(failureMechanism.Calculations.Cast<GrassCoverErosionInwardsCalculation>(), calculationMapData);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void GivenViewWithCalculationData_WhenCalculationUpdatedAndNotified_ThenMapDataUpdated()
        {
            // Given
            var calculationA = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    DikeProfile = DikeProfileTestFactory.CreateDikeProfile(new Point2D(1.3, 1.3)),
                    HydraulicBoundaryLocation = new TestHydraulicBoundaryLocation()
                }
            };

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(calculationA);

            using (var view = new GrassCoverErosionInwardsFailureMechanismView(failureMechanism, new AssessmentSectionStub()))
            {
                IMapControl map = ((RingtoetsMapControl) view.Controls[0]).MapControl;

                var calculationMapData = (MapLineData) map.Data.Collection.ElementAt(calculationsIndex);

                var mocks = new MockRepository();
                IObserver[] observers = AttachMapDataObservers(mocks, map.Data.Collection);
                observers[calculationObserverIndex].Expect(obs => obs.UpdateObserver());
                observers[simpleAssemblyObserverIndex].Expect(obs => obs.UpdateObserver());
                observers[detailedAssemblyObserverIndex].Expect(obs => obs.UpdateObserver());
                observers[tailorMadeAssemblyObserverIndex].Expect(obs => obs.UpdateObserver());
                observers[combinedAssemblyObserverIndex].Expect(obs => obs.UpdateObserver());
                mocks.ReplayAll();

                // When
                calculationA.Name = "new name";
                calculationA.NotifyObservers();

                // Then 
                AssertCalculationsMapData(failureMechanism.Calculations.Cast<GrassCoverErosionInwardsCalculation>(), calculationMapData);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void GivenViewWithAssemblyData_WhenFailureMechanismNotified_ThenMapDataUpdated()
        {
            // Given
            var random = new Random(39);
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
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

                using (var view = new GrassCoverErosionInwardsFailureMechanismView(failureMechanism, new AssessmentSectionStub()))
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
        }

        [Test]
        public void GivenViewWithAssemblyData_WhenCalculationNotified_ThenMapDataUpdated()
        {
            // Given
            var random = new Random(39);
            var calculationA = new GrassCoverErosionInwardsCalculation();

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(calculationA);
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

                using (var view = new GrassCoverErosionInwardsFailureMechanismView(failureMechanism, new AssessmentSectionStub()))
                {
                    IMapControl map = ((RingtoetsMapControl) view.Controls[0]).MapControl;

                    var mocks = new MockRepository();
                    IObserver[] observers = AttachMapDataObservers(mocks, map.Data.Collection);
                    observers[calculationObserverIndex].Expect(obs => obs.UpdateObserver());
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
                    calculationA.NotifyObservers();

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
        }

        [Test]
        public void GivenViewWithAssemblyData_WhenFailureMechanismSectionResultNotified_ThenMapDataUpdated()
        {
            // Given
            var random = new Random(39);
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
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

                using (var view = new GrassCoverErosionInwardsFailureMechanismView(failureMechanism, new AssessmentSectionStub()))
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
        }

        [Test]
        public void NotifyObservers_DataUpdated_MapLayersSameOrder()
        {
            // Setup
            const int updatedReferenceLineLayerIndex = referenceLineIndex + 6;
            const int updatedSectionCollectionIndex = sectionsCollectionIndex - 1;
            const int updatedAssemblyResultsCollectionIndex = assemblyResultsIndex - 1;
            const int updatedHydraulicLocationsLayerIndex = hydraulicBoundaryLocationsIndex - 1;
            const int updatedDikeProfilesLayerIndex = dikeProfilesIndex - 1;
            const int updatedForeshoreProfilesLayerIndex = foreshoreProfilesIndex - 1;
            const int updatedCalculationsIndex = calculationsIndex - 1;

            var assessmentSection = new AssessmentSectionStub();

            using (var view = new GrassCoverErosionInwardsFailureMechanismView(new GrassCoverErosionInwardsFailureMechanism(), assessmentSection))
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

                var sectionsData = (MapDataCollection) mapDataList[updatedSectionCollectionIndex];
                Assert.AreEqual("Vakindeling", sectionsData.Name);

                var assemblyResultsData = (MapDataCollection) mapDataList[updatedAssemblyResultsCollectionIndex];
                Assert.AreEqual("Toetsoordeel", assemblyResultsData.Name);

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

                var actualSectionsData = (MapDataCollection) mapDataList[updatedSectionCollectionIndex];
                Assert.AreEqual("Vakindeling", actualSectionsData.Name);

                var actualAssemblyResultsData = (MapDataCollection) mapDataList[updatedAssemblyResultsCollectionIndex];
                Assert.AreEqual("Toetsoordeel", actualAssemblyResultsData.Name);

                var actualHydraulicLocationsData = (MapPointData) mapDataList[updatedHydraulicLocationsLayerIndex];
                Assert.AreEqual("Hydraulische belastingen", actualHydraulicLocationsData.Name);

                var actualDikeProfilesData = (MapLineData) mapDataList[updatedDikeProfilesLayerIndex];
                Assert.AreEqual("Dijkprofielen", actualDikeProfilesData.Name);

                var actualForeshoreProfilesData = (MapLineData) mapDataList[updatedForeshoreProfilesLayerIndex];
                Assert.AreEqual("Voorlandprofielen", actualForeshoreProfilesData.Name);

                var actualCalculationsData = (MapLineData) mapDataList[updatedCalculationsIndex];
                Assert.AreEqual("Berekeningen", actualCalculationsData.Name);
            }
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

            Assert.AreEqual(7, mapDataList.Count);

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

            var hydraulicBoundaryLocationsMapDataObserver = mocks.StrictMock<IObserver>();
            mapDataArray[hydraulicBoundaryLocationsIndex].Attach(hydraulicBoundaryLocationsMapDataObserver);

            var dikeProfilesObserver = mocks.StrictMock<IObserver>();
            mapDataArray[dikeProfilesIndex].Attach(dikeProfilesObserver);

            var foreshoreProfilesObserver = mocks.StrictMock<IObserver>();
            mapDataArray[foreshoreProfilesIndex].Attach(foreshoreProfilesObserver);

            var calculationsMapDataObserver = mocks.StrictMock<IObserver>();
            mapDataArray[calculationsIndex].Attach(calculationsMapDataObserver);

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
                hydraulicBoundaryLocationsMapDataObserver,
                dikeProfilesObserver,
                foreshoreProfilesObserver,
                calculationsMapDataObserver,
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