﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Core.Components.Gis.Data;
using Core.Components.Gis.Features;
using Core.Components.Gis.Forms;
using Core.Components.Gis.Geometries;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.ClosingStructures.Data;
using Ringtoets.ClosingStructures.Data.TestUtil;
using Ringtoets.ClosingStructures.Forms.Views;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.TestUtil;
using Ringtoets.Common.Forms.Views;

namespace Ringtoets.ClosingStructures.Forms.Test.Views
{
    [TestFixture]
    public class ClosingStructuresFailureMechanismViewTest
    {
        private const int referenceLineIndex = 0;
        private const int sectionsIndex = 1;
        private const int sectionsStartPointIndex = 2;
        private const int sectionsEndPointIndex = 3;
        private const int hydraulicBoundaryLocationsIndex = 4;
        private const int foreshoreProfilesIndex = 5;
        private const int structuresIndex = 6;
        private const int calculationsIndex = 7;

        [Test]
        public void Constructor_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => new ClosingStructuresFailureMechanismView(null, assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new ClosingStructuresFailureMechanismView(new ClosingStructuresFailureMechanism(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var failureMechanism = new ClosingStructuresFailureMechanism();
            var assessmentSection = new AssessmentSectionStub();

            // Call
            using (var view = new ClosingStructuresFailureMechanismView(failureMechanism, assessmentSection))
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
            using (var view = new ClosingStructuresFailureMechanismView(new ClosingStructuresFailureMechanism(), assessmentSection))
            {
                // Assert
                MapDataTestHelper.AssertImageBasedMapData(assessmentSection.BackgroundData, view.Map.BackgroundMapData);
            }
        }

        [Test]
        public void Constructor_WithAllData_DataUpdatedToCollectionOfFilledMapData()
        {
            // Setup
            var calculationA = new StructuresCalculation<ClosingStructuresInput>
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, string.Empty, 1.3, 2.3),
                    Structure = new TestClosingStructure(new Point2D(1.2, 2.3))
                }
            };

            var calculationB = new StructuresCalculation<ClosingStructuresInput>
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, string.Empty, 7.7, 12.6),
                    Structure = new TestClosingStructure(new Point2D(2.7, 2.0))
                }
            };

            var geometryPoints = new[]
            {
                new Point2D(0.0, 0.0),
                new Point2D(2.0, 0.0),
                new Point2D(4.0, 4.0),
                new Point2D(6.0, 4.0)
            };

            var failureMechanism = new ClosingStructuresFailureMechanism();
            failureMechanism.AddSection(new FailureMechanismSection("A", geometryPoints.Take(2)));
            failureMechanism.AddSection(new FailureMechanismSection("B", geometryPoints.Skip(1).Take(2)));
            failureMechanism.AddSection(new FailureMechanismSection("C", geometryPoints.Skip(2).Take(2)));

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

            // Call
            using (var view = new ClosingStructuresFailureMechanismView(failureMechanism, assessmentSection))
            {
                IMapControl map = ((RingtoetsMapControl) view.Controls[0]).MapControl;

                // Assert
                MapDataCollection mapData = map.Data;
                Assert.IsInstanceOf<MapDataCollection>(mapData);

                List<MapData> mapDataList = mapData.Collection.ToList();
                Assert.AreEqual(8, mapDataList.Count);
                MapDataTestHelper.AssertReferenceLineMapData(assessmentSection.ReferenceLine, mapDataList[referenceLineIndex]);
                MapDataTestHelper.AssertFailureMechanismSectionsMapData(failureMechanism.Sections, mapDataList[sectionsIndex]);
                MapDataTestHelper.AssertFailureMechanismSectionsStartPointMapData(failureMechanism.Sections, mapDataList[sectionsStartPointIndex]);
                MapDataTestHelper.AssertFailureMechanismSectionsEndPointMapData(failureMechanism.Sections, mapDataList[sectionsEndPointIndex]);
                MapDataTestHelper.AssertHydraulicBoundaryLocationsMapData(assessmentSection, mapDataList[hydraulicBoundaryLocationsIndex]);
                MapDataTestHelper.AssertForeshoreProfilesMapData(failureMechanism.ForeshoreProfiles, mapDataList[foreshoreProfilesIndex]);

                AssertCalculationsMapData(
                    failureMechanism.Calculations.Cast<StructuresCalculation<ClosingStructuresInput>>(),
                    mapDataList[calculationsIndex]);
            }
        }

        [Test]
        public void GivenViewWithHydraulicBoundaryLocationsData_WhenHydraulicBoundaryLocationsUpdatedAndNotified_ThenMapDataUpdated()
        {
            // Given
            var assessmentSection = new AssessmentSectionStub();
            assessmentSection.SetHydraulicBoundaryLocationCalculations(new[]
            {
                new HydraulicBoundaryLocation(1, "test", 1.0, 2.0)
            });

            using (var view = new ClosingStructuresFailureMechanismView(new ClosingStructuresFailureMechanism(), assessmentSection))
            {
                IMapControl map = ((RingtoetsMapControl) view.Controls[0]).MapControl;

                var mocks = new MockRepository();
                IObserver[] observers = AttachMapDataObservers(mocks, map.Data.Collection);
                observers[hydraulicBoundaryLocationsIndex].Expect(obs => obs.UpdateObserver());
                mocks.ReplayAll();

                MapData hydraulicBoundaryLocationsMapData = map.Data.Collection.ElementAt(hydraulicBoundaryLocationsIndex);

                // Precondition
                MapDataTestHelper.AssertHydraulicBoundaryLocationsMapData(assessmentSection, hydraulicBoundaryLocationsMapData);

                // When
                assessmentSection.SetHydraulicBoundaryLocationCalculations(new[]
                {
                    new HydraulicBoundaryLocation(1, "test", 1.0, 2.0),
                    new HydraulicBoundaryLocation(2, "test2", 2.0, 3.0)
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

            using (var view = new ClosingStructuresFailureMechanismView(new ClosingStructuresFailureMechanism(), assessmentSection))
            {
                IMapControl map = ((RingtoetsMapControl) view.Controls[0]).MapControl;

                var mocks = new MockRepository();
                IObserver[] observers = AttachMapDataObservers(mocks, map.Data.Collection);
                observers[hydraulicBoundaryLocationsIndex].Expect(obs => obs.UpdateObserver());
                mocks.ReplayAll();

                MapData hydraulicBoundaryLocationsMapData = map.Data.Collection.ElementAt(hydraulicBoundaryLocationsIndex);

                // Precondition
                MapDataTestHelper.AssertHydraulicBoundaryLocationsMapData(assessmentSection, hydraulicBoundaryLocationsMapData);

                // When
                HydraulicBoundaryLocationCalculation calculation = getCalculationFunc(assessmentSection);
                calculation.Output = new TestHydraulicBoundaryLocationOutput(new Random(21).NextDouble());
                calculation.NotifyObservers();

                // Then
                MapDataTestHelper.AssertHydraulicBoundaryLocationsMapData(assessmentSection, hydraulicBoundaryLocationsMapData);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void GivenViewWithReferenceLineData_WhenReferenceLineUpdatedAndNotified_ThenMapDataUpdated()
        {
            // Given
            var assessmentSection = new AssessmentSectionStub
            {
                ReferenceLine = new ReferenceLine()
            };
            assessmentSection.ReferenceLine.SetGeometry(new List<Point2D>
            {
                new Point2D(1.0, 2.0),
                new Point2D(2.0, 1.0)
            });

            using (var view = new ClosingStructuresFailureMechanismView(new ClosingStructuresFailureMechanism(), assessmentSection))
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
                assessmentSection.ReferenceLine.SetGeometry(new List<Point2D>
                {
                    new Point2D(2.0, 5.0),
                    new Point2D(4.0, 3.0)
                });
                assessmentSection.NotifyObservers();

                // Then
                MapDataTestHelper.AssertReferenceLineMapData(assessmentSection.ReferenceLine, referenceLineMapData);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void GivenViewWithFailureMechanismSectionsData_WhenFailureMechanismSectionsUpdatedAndNotified_ThenMapDataUpdated()
        {
            // Given
            var failureMechanism = new ClosingStructuresFailureMechanism();

            using (var view = new ClosingStructuresFailureMechanismView(failureMechanism, new AssessmentSectionStub()))
            {
                IMapControl map = ((RingtoetsMapControl) view.Controls[0]).MapControl;

                var mocks = new MockRepository();
                IObserver[] observers = AttachMapDataObservers(mocks, map.Data.Collection);
                observers[sectionsIndex].Expect(obs => obs.UpdateObserver());
                observers[sectionsStartPointIndex].Expect(obs => obs.UpdateObserver());
                observers[sectionsEndPointIndex].Expect(obs => obs.UpdateObserver());
                mocks.ReplayAll();

                var sectionMapData = (MapLineData) map.Data.Collection.ElementAt(sectionsIndex);
                var sectionStartsMapData = (MapPointData) map.Data.Collection.ElementAt(sectionsStartPointIndex);
                var sectionsEndsMapData = (MapPointData) map.Data.Collection.ElementAt(sectionsEndPointIndex);

                // When
                failureMechanism.AddSection(new FailureMechanismSection(string.Empty, new[]
                {
                    new Point2D(1, 2),
                    new Point2D(1, 2)
                }));
                failureMechanism.NotifyObservers();

                // Then
                MapDataTestHelper.AssertFailureMechanismSectionsMapData(failureMechanism.Sections, sectionMapData);
                MapDataTestHelper.AssertFailureMechanismSectionsStartPointMapData(failureMechanism.Sections, sectionStartsMapData);
                MapDataTestHelper.AssertFailureMechanismSectionsEndPointMapData(failureMechanism.Sections, sectionsEndsMapData);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void GivenViewWithForeshoreProfileData_WhenForeshoreProfileUpdatedAndNotified_ThenMapDataUpdated()
        {
            // Given
            var foreshoreProfile = new TestForeshoreProfile("originalProfile ID", new[]
            {
                new Point2D(0, 0),
                new Point2D(1, 1)
            });
            var failureMechanism = new ClosingStructuresFailureMechanism();
            failureMechanism.ForeshoreProfiles.AddRange(new[]
            {
                foreshoreProfile
            }, "path");

            using (var view = new ClosingStructuresFailureMechanismView(failureMechanism, new AssessmentSectionStub()))
            {
                IMapControl map = ((RingtoetsMapControl) view.Controls[0]).MapControl;

                var mocks = new MockRepository();
                IObserver[] observers = AttachMapDataObservers(mocks, map.Data.Collection);
                observers[foreshoreProfilesIndex].Expect(obs => obs.UpdateObserver());
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
        }

        [Test]
        public void GivenViewWithForeshoreProfilesData_WhenForeshoreProfilesUpdatedAndNotified_ThenMapDataUpdated()
        {
            // Given
            var failureMechanism = new ClosingStructuresFailureMechanism();
            failureMechanism.ForeshoreProfiles.AddRange(new[]
            {
                new TestForeshoreProfile("originalProfile ID", new[]
                {
                    new Point2D(0, 0),
                    new Point2D(1, 1)
                })
            }, "path");

            using (var view = new ClosingStructuresFailureMechanismView(failureMechanism, new AssessmentSectionStub()))
            {
                IMapControl map = ((RingtoetsMapControl) view.Controls[0]).MapControl;

                var mocks = new MockRepository();
                IObserver[] observers = AttachMapDataObservers(mocks, map.Data.Collection);
                observers[foreshoreProfilesIndex].Expect(obs => obs.UpdateObserver());
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
        }

        [Test]
        public void GivenViewWithStructureData_WhenStructureUpdatedAndNotified_ThenMapDataUpdated()
        {
            // Given
            var structure = new TestClosingStructure(new Point2D(0, 0), "Id");
            var failureMechanism = new ClosingStructuresFailureMechanism();
            failureMechanism.ClosingStructures.AddRange(new[]
            {
                structure
            }, "path");

            using (var view = new ClosingStructuresFailureMechanismView(failureMechanism, new AssessmentSectionStub()))
            {
                IMapControl map = ((RingtoetsMapControl) view.Controls[0]).MapControl;

                var mocks = new MockRepository();
                IObserver[] observers = AttachMapDataObservers(mocks, map.Data.Collection);
                observers[structuresIndex].Expect(obs => obs.UpdateObserver());
                mocks.ReplayAll();

                MapData structuresData = map.Data.Collection.ElementAt(structuresIndex);

                // Precondition
                MapDataTestHelper.AssertStructuresMapData(failureMechanism.ClosingStructures,
                                                          structuresData);

                // When
                structure.CopyProperties(new TestClosingStructure(new Point2D(1, 1), "Id"));
                structure.NotifyObservers();

                // Then
                MapDataTestHelper.AssertStructuresMapData(failureMechanism.ClosingStructures,
                                                          structuresData);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void GivenViewWithStructuresData_WhenStructuresUpdatedAndNotified_ThenMapDataUpdated()
        {
            // Given
            var failureMechanism = new ClosingStructuresFailureMechanism();
            failureMechanism.ClosingStructures.AddRange(new[]
            {
                new TestClosingStructure(new Point2D(0, 0), "Id1")
            }, "path");

            using (var view = new ClosingStructuresFailureMechanismView(failureMechanism, new AssessmentSectionStub()))
            {
                IMapControl map = ((RingtoetsMapControl) view.Controls[0]).MapControl;

                var mocks = new MockRepository();
                IObserver[] observers = AttachMapDataObservers(mocks, map.Data.Collection);
                observers[structuresIndex].Expect(obs => obs.UpdateObserver());
                mocks.ReplayAll();

                MapData structuresData = map.Data.Collection.ElementAt(structuresIndex);

                // Precondition
                MapDataTestHelper.AssertStructuresMapData(failureMechanism.ClosingStructures,
                                                          structuresData);

                // When
                failureMechanism.ClosingStructures.AddRange(new[]
                {
                    new TestClosingStructure(new Point2D(1, 1), "Id2")
                }, "some path");
                failureMechanism.ClosingStructures.NotifyObservers();

                // Then
                MapDataTestHelper.AssertStructuresMapData(failureMechanism.ClosingStructures,
                                                          structuresData);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void GivenViewWithCalculationGroupData_WhenCalculationGroupUpdatedAndNotified_ThenMapDataUpdated()
        {
            // Given
            var calculationA = new StructuresCalculation<ClosingStructuresInput>
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, string.Empty, 1.3, 2.3),
                    Structure = new TestClosingStructure(new Point2D(1.2, 2.3))
                }
            };

            var failureMechanism = new ClosingStructuresFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(calculationA);

            using (var view = new ClosingStructuresFailureMechanismView(failureMechanism, new AssessmentSectionStub()))
            {
                IMapControl map = ((RingtoetsMapControl) view.Controls[0]).MapControl;

                var mocks = new MockRepository();
                IObserver[] observers = AttachMapDataObservers(mocks, map.Data.Collection);
                observers[calculationsIndex].Expect(obs => obs.UpdateObserver());
                mocks.ReplayAll();

                var calculationMapData = (MapLineData) map.Data.Collection.ElementAt(calculationsIndex);

                // Precondition
                AssertCalculationsMapData(failureMechanism.Calculations.Cast<StructuresCalculation<ClosingStructuresInput>>(),
                                          calculationMapData);

                // When
                var calculationB = new StructuresCalculation<ClosingStructuresInput>
                {
                    InputParameters =
                    {
                        HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, string.Empty, 7.7, 12.6),
                        Structure = new TestClosingStructure(new Point2D(2.7, 2.0))
                    }
                };

                failureMechanism.CalculationsGroup.Children.Add(calculationB);
                failureMechanism.CalculationsGroup.NotifyObservers();

                // Then
                AssertCalculationsMapData(failureMechanism.Calculations.Cast<StructuresCalculation<ClosingStructuresInput>>(), calculationMapData);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void GivenViewWithCalculationInputData_WhenCalculationInputUpdatedAndNotified_ThenMapDataUpdated()
        {
            // Given
            var calculationA = new StructuresCalculation<ClosingStructuresInput>
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, string.Empty, 1.3, 2.3),
                    Structure = new TestClosingStructure(new Point2D(1.2, 2.3))
                }
            };
            var failureMechanism = new ClosingStructuresFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(calculationA);

            using (var view = new ClosingStructuresFailureMechanismView(failureMechanism, new AssessmentSectionStub()))
            {
                IMapControl map = ((RingtoetsMapControl) view.Controls[0]).MapControl;

                var mocks = new MockRepository();
                IObserver[] observers = AttachMapDataObservers(mocks, map.Data.Collection);
                observers[calculationsIndex].Expect(obs => obs.UpdateObserver());
                mocks.ReplayAll();

                var calculationMapData = (MapLineData) map.Data.Collection.ElementAt(calculationsIndex);

                // Precondition
                AssertCalculationsMapData(failureMechanism.Calculations.Cast<StructuresCalculation<ClosingStructuresInput>>(),
                                          calculationMapData);

                // When
                calculationA.InputParameters.Structure = new TestClosingStructure(new Point2D(2.7, 2.0));
                calculationA.InputParameters.NotifyObservers();

                // Then
                AssertCalculationsMapData(failureMechanism.Calculations.Cast<StructuresCalculation<ClosingStructuresInput>>(), calculationMapData);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void GivenViewWithCalculationData_WhenCalculationUpdatedAndNotified_ThenMapDataUpdated()
        {
            // Given
            var calculationA = new StructuresCalculation<ClosingStructuresInput>
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, string.Empty, 1.3, 2.3),
                    Structure = new TestClosingStructure(new Point2D(1.2, 2.3))
                }
            };

            var failureMechanism = new ClosingStructuresFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(calculationA);

            using (var view = new ClosingStructuresFailureMechanismView(failureMechanism, new AssessmentSectionStub()))
            {
                IMapControl map = ((RingtoetsMapControl) view.Controls[0]).MapControl;

                var mocks = new MockRepository();
                IObserver[] observers = AttachMapDataObservers(mocks, map.Data.Collection);
                observers[calculationsIndex].Expect(obs => obs.UpdateObserver());
                mocks.ReplayAll();

                var calculationMapData = (MapLineData) map.Data.Collection.ElementAt(calculationsIndex);

                // Precondition
                AssertCalculationsMapData(failureMechanism.Calculations.Cast<StructuresCalculation<ClosingStructuresInput>>(),
                                          calculationMapData);

                // When
                calculationA.Name = "new name";
                calculationA.NotifyObservers();

                // Then
                AssertCalculationsMapData(failureMechanism.Calculations.Cast<StructuresCalculation<ClosingStructuresInput>>(), calculationMapData);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void UpdateObserver_DataUpdated_MapLayersSameOrder()
        {
            // Setup
            const int updatedRefenceLineLayerIndex = referenceLineIndex + 7;
            const int updatedSectionsLayerIndex = sectionsIndex - 1;
            const int updateSectionStartLayerIndex = sectionsStartPointIndex - 1;
            const int updatedSectionEndLayerIndex = sectionsEndPointIndex - 1;
            const int updatedHydraulicLocationsLayerIndex = hydraulicBoundaryLocationsIndex - 1;
            const int updatedForeshoreProfilesLayerIndex = foreshoreProfilesIndex - 1;
            const int updatedStructuresLayerIndex = structuresIndex - 1;
            const int updatedCalculationsIndex = calculationsIndex - 1;

            var assessmentSection = new AssessmentSectionStub();

            using (var view = new ClosingStructuresFailureMechanismView(new ClosingStructuresFailureMechanism(), assessmentSection))
            {
                IMapControl map = ((RingtoetsMapControl) view.Controls[0]).MapControl;

                MapDataCollection mapData = map.Data;

                var dataToMove = (MapLineData) map.Data.Collection.ElementAt(referenceLineIndex);
                mapData.Remove(dataToMove);
                mapData.Add(dataToMove);

                List<MapData> mapDataList = mapData.Collection.ToList();

                // Precondition
                var referenceLineData = (MapLineData) mapDataList[updatedRefenceLineLayerIndex];
                Assert.AreEqual("Referentielijn", referenceLineData.Name);

                var sectionsData = (MapLineData) mapDataList[updatedSectionsLayerIndex];
                Assert.AreEqual("Vakindeling", sectionsData.Name);

                var sectionStartsData = (MapPointData) mapDataList[updateSectionStartLayerIndex];
                Assert.AreEqual("Vakindeling (startpunten)", sectionStartsData.Name);

                var sectionEndsData = (MapPointData) mapDataList[updatedSectionEndLayerIndex];
                Assert.AreEqual("Vakindeling (eindpunten)", sectionEndsData.Name);

                var hydraulicLocationsData = (MapPointData) mapDataList[updatedHydraulicLocationsLayerIndex];
                Assert.AreEqual("Hydraulische randvoorwaarden", hydraulicLocationsData.Name);

                var foreshoreProfilesData = (MapLineData) mapDataList[updatedForeshoreProfilesLayerIndex];
                Assert.AreEqual("Voorlandprofielen", foreshoreProfilesData.Name);

                var structuresData = (MapPointData) mapDataList[updatedStructuresLayerIndex];
                Assert.AreEqual("Kunstwerken", structuresData.Name);

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
                var actualReferenceLineData = (MapLineData) mapDataList[updatedRefenceLineLayerIndex];
                Assert.AreEqual("Referentielijn", actualReferenceLineData.Name);

                var actualSectionsData = (MapLineData) mapDataList[updatedSectionsLayerIndex];
                Assert.AreEqual("Vakindeling", actualSectionsData.Name);

                var actualSectionStartsData = (MapPointData) mapDataList[updateSectionStartLayerIndex];
                Assert.AreEqual("Vakindeling (startpunten)", actualSectionStartsData.Name);

                var actualSectionEndsData = (MapPointData) mapDataList[updatedSectionEndLayerIndex];
                Assert.AreEqual("Vakindeling (eindpunten)", actualSectionEndsData.Name);

                var actualHydraulicLocationsData = (MapPointData) mapDataList[updatedHydraulicLocationsLayerIndex];
                Assert.AreEqual("Hydraulische randvoorwaarden", actualHydraulicLocationsData.Name);

                var actualForeshoreProfilesData = (MapLineData) mapDataList[updatedForeshoreProfilesLayerIndex];
                Assert.AreEqual("Voorlandprofielen", actualForeshoreProfilesData.Name);

                var actualStructuresData = (MapPointData) mapDataList[updatedStructuresLayerIndex];
                Assert.AreEqual("Kunstwerken", actualStructuresData.Name);

                var actualCalculationsData = (MapLineData) mapDataList[updatedCalculationsIndex];
                Assert.AreEqual("Berekeningen", actualCalculationsData.Name);
            }
        }

        private static void AssertCalculationsMapData(IEnumerable<StructuresCalculation<ClosingStructuresInput>> calculations, MapData mapData)
        {
            Assert.IsInstanceOf<MapLineData>(mapData);
            var calculationsMapData = (MapLineData) mapData;
            StructuresCalculation<ClosingStructuresInput>[] calculationsArray = calculations.ToArray();
            MapFeature[] calculationsFeatures = calculationsMapData.Features.ToArray();
            Assert.AreEqual(calculationsArray.Length, calculationsFeatures.Length);

            for (var index = 0; index < calculationsArray.Length; index++)
            {
                MapGeometry[] geometries = calculationsFeatures[index].MapGeometries.ToArray();
                Assert.AreEqual(1, geometries.Length);

                StructuresCalculation<ClosingStructuresInput> calculation = calculationsArray[index];
                CollectionAssert.AreEquivalent(new[]
                {
                    calculation.InputParameters.Structure.Location,
                    calculation.InputParameters.HydraulicBoundaryLocation.Location
                }, geometries[0].PointCollections.First());
            }

            Assert.AreEqual("Berekeningen", mapData.Name);
        }

        private static void AssertEmptyMapData(MapDataCollection mapDataCollection)
        {
            Assert.AreEqual("Kunstwerken - Betrouwbaarheid sluiting kunstwerk", mapDataCollection.Name);

            List<MapData> mapDataList = mapDataCollection.Collection.ToList();

            Assert.AreEqual(8, mapDataList.Count);

            var referenceLineMapData = (MapLineData) mapDataList[referenceLineIndex];
            var sectionsMapData = (MapLineData) mapDataList[sectionsIndex];
            var foreshoreProfilesMapData = (MapLineData) mapDataList[foreshoreProfilesIndex];
            var structuresMapData = (MapPointData) mapDataList[structuresIndex];
            var sectionsStartPointMapData = (MapPointData) mapDataList[sectionsStartPointIndex];
            var sectionsEndPointMapData = (MapPointData) mapDataList[sectionsEndPointIndex];
            var hydraulicBoundaryLocationsMapData = (MapPointData) mapDataList[hydraulicBoundaryLocationsIndex];
            var calculationsMapData = (MapLineData) mapDataList[calculationsIndex];

            CollectionAssert.IsEmpty(referenceLineMapData.Features);
            CollectionAssert.IsEmpty(sectionsMapData.Features);
            CollectionAssert.IsEmpty(foreshoreProfilesMapData.Features);
            CollectionAssert.IsEmpty(structuresMapData.Features);
            CollectionAssert.IsEmpty(sectionsStartPointMapData.Features);
            CollectionAssert.IsEmpty(sectionsEndPointMapData.Features);
            CollectionAssert.IsEmpty(hydraulicBoundaryLocationsMapData.Features);
            CollectionAssert.IsEmpty(calculationsMapData.Features);

            Assert.AreEqual("Referentielijn", referenceLineMapData.Name);
            Assert.AreEqual("Vakindeling", sectionsMapData.Name);
            Assert.AreEqual("Voorlandprofielen", foreshoreProfilesMapData.Name);
            Assert.AreEqual("Kunstwerken", structuresMapData.Name);
            Assert.AreEqual("Vakindeling (startpunten)", sectionsStartPointMapData.Name);
            Assert.AreEqual("Vakindeling (eindpunten)", sectionsEndPointMapData.Name);
            Assert.AreEqual("Hydraulische randvoorwaarden", hydraulicBoundaryLocationsMapData.Name);
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

            var sectionsMapDataObserver = mocks.StrictMock<IObserver>();
            mapDataArray[sectionsIndex].Attach(sectionsMapDataObserver);

            var sectionsStartPointMapDataObserver = mocks.StrictMock<IObserver>();
            mapDataArray[sectionsStartPointIndex].Attach(sectionsStartPointMapDataObserver);

            var sectionsEndPointMapDataObserver = mocks.StrictMock<IObserver>();
            mapDataArray[sectionsEndPointIndex].Attach(sectionsEndPointMapDataObserver);

            var hydraulicBoundaryLocationsMapDataObserver = mocks.StrictMock<IObserver>();
            mapDataArray[hydraulicBoundaryLocationsIndex].Attach(hydraulicBoundaryLocationsMapDataObserver);

            var foreshoreProfilesMapDataObserver = mocks.StrictMock<IObserver>();
            mapDataArray[foreshoreProfilesIndex].Attach(foreshoreProfilesMapDataObserver);

            var structuresMapDataObserver = mocks.StrictMock<IObserver>();
            mapDataArray[structuresIndex].Attach(structuresMapDataObserver);

            var calculationsMapDataObserver = mocks.StrictMock<IObserver>();
            mapDataArray[calculationsIndex].Attach(calculationsMapDataObserver);

            return new[]
            {
                referenceLineMapDataObserver,
                sectionsMapDataObserver,
                sectionsStartPointMapDataObserver,
                sectionsEndPointMapDataObserver,
                hydraulicBoundaryLocationsMapDataObserver,
                foreshoreProfilesMapDataObserver,
                structuresMapDataObserver,
                calculationsMapDataObserver
            };
        }
    }
}