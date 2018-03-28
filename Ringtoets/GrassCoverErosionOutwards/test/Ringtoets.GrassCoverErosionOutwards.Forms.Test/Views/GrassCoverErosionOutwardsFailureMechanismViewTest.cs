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
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Components.Gis.Data;
using Core.Components.Gis.Features;
using Core.Components.Gis.Forms;
using Core.Components.Gis.Geometries;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.TestUtil;
using Ringtoets.Common.Forms.Views;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.GrassCoverErosionOutwards.Forms.PresentationObjects;
using Ringtoets.GrassCoverErosionOutwards.Forms.Views;

namespace Ringtoets.GrassCoverErosionOutwards.Forms.Test.Views
{
    [TestFixture]
    public class GrassCoverErosionOutwardsFailureMechanismViewTest
    {
        private const int referenceLineIndex = 0;
        private const int sectionsIndex = 1;
        private const int sectionsStartPointIndex = 2;
        private const int sectionsEndPointIndex = 3;
        private const int hydraulicBoundaryLocationsIndex = 4;
        private const int foreshoreProfilesIndex = 5;
        private const int calculationsIndex = 6;

        [Test]
        public void Constructor_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => new GrassCoverErosionOutwardsFailureMechanismView(null, assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new GrassCoverErosionOutwardsFailureMechanismView(new GrassCoverErosionOutwardsFailureMechanism(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            var assessmentSection = new ObservableTestAssessmentSectionStub();

            // Call
            using (var view = new GrassCoverErosionOutwardsFailureMechanismView(failureMechanism, assessmentSection))
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
                Assert.IsNull(view.Map.Data);
            }
        }

        [Test]
        public void Data_GrassCoverErosionOutwardsFailureMechanismContext_DataSet()
        {
            // Setup
            using (var view = new GrassCoverErosionOutwardsFailureMechanismView(new GrassCoverErosionOutwardsFailureMechanism(), new ObservableTestAssessmentSectionStub()))
            {
                var assessmentSection = new ObservableTestAssessmentSectionStub();

                var failureMechanismContext = new GrassCoverErosionOutwardsFailureMechanismContext(new GrassCoverErosionOutwardsFailureMechanism(), assessmentSection);

                // Call
                view.Data = failureMechanismContext;

                // Assert
                Assert.AreSame(failureMechanismContext, view.Data);
            }
        }

        [Test]
        public void Data_OtherThanGrassCoverErosionOutwardsFailureMechanismContext_DataNull()
        {
            // Setup
            using (var view = new GrassCoverErosionOutwardsFailureMechanismView(new GrassCoverErosionOutwardsFailureMechanism(), new ObservableTestAssessmentSectionStub()))
            {
                var data = new object();

                // Call
                view.Data = data;

                // Assert
                Assert.IsNull(view.Data);
            }
        }

        [Test]
        public void Data_AssessmentSectionWithBackgroundData_BackgroundDataSet()
        {
            // Setup
            IAssessmentSection assessmentSection = new ObservableTestAssessmentSectionStub();

            using (var view = new GrassCoverErosionOutwardsFailureMechanismView(new GrassCoverErosionOutwardsFailureMechanism(), assessmentSection))
            {
                var failureMechanismContext = new GrassCoverErosionOutwardsFailureMechanismContext(
                    new GrassCoverErosionOutwardsFailureMechanism(), assessmentSection);

                // Call
                view.Data = failureMechanismContext;

                // Assert
                MapDataTestHelper.AssertImageBasedMapData(assessmentSection.BackgroundData, view.Map.BackgroundMapData);
            }
        }

        [Test]
        public void Data_SetToNull_MapDataCleared()
        {
            // Setup
            var assessmentSection = new ObservableTestAssessmentSectionStub();

            using (var view = new GrassCoverErosionOutwardsFailureMechanismView(new GrassCoverErosionOutwardsFailureMechanism(), assessmentSection))
            {
                var failureMechanismContext = new GrassCoverErosionOutwardsFailureMechanismContext(
                    new GrassCoverErosionOutwardsFailureMechanism(), assessmentSection);

                view.Data = failureMechanismContext;

                // Precondition
                Assert.AreEqual(7, view.Map.Data.Collection.Count());
                MapDataTestHelper.AssertImageBasedMapData(assessmentSection.BackgroundData, view.Map.BackgroundMapData);

                // Call
                view.Data = null;

                // Assert
                Assert.IsNull(view.Data);
                Assert.IsNull(view.Map.Data);
                Assert.IsNull(view.Map.BackgroundMapData);
            }
        }

        [Test]
        public void Data_EmptyGrassCoverErosionOutwardsFailureMechanismContext_NoMapDataSet()
        {
            // Setup
            var assessmentSection = new ObservableTestAssessmentSectionStub();

            using (var view = new GrassCoverErosionOutwardsFailureMechanismView(new GrassCoverErosionOutwardsFailureMechanism(), assessmentSection))
            {
                var failureMechanismContext = new GrassCoverErosionOutwardsFailureMechanismContext(
                    new GrassCoverErosionOutwardsFailureMechanism(), assessmentSection);

                // Call
                view.Data = failureMechanismContext;

                // Assert
                Assert.AreSame(failureMechanismContext, view.Data);
                AssertEmptyMapData(view.Map.Data);
                MapDataTestHelper.AssertImageBasedMapData(assessmentSection.BackgroundData, view.Map.BackgroundMapData);
            }
        }

        [Test]
        public void Data_GrassCoverErosionOutwardsFailureMechanismContext_DataUpdatedToCollectionOfFilledMapData()
        {
            // Setup
            var referenceLine = new ReferenceLine();
            referenceLine.SetGeometry(new[]
            {
                new Point2D(1.0, 2.0),
                new Point2D(2.0, 1.0)
            });

            var assessmentSection = new ObservableTestAssessmentSectionStub
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

            failureMechanism.WaveConditionsCalculationGroup.Children.Add(calculationA);
            failureMechanism.WaveConditionsCalculationGroup.Children.Add(calculationB);
            failureMechanism.HydraulicBoundaryLocations.Add(new HydraulicBoundaryLocation(1, "test", 1.0, 2.0));

            using (var view = new GrassCoverErosionOutwardsFailureMechanismView(failureMechanism, assessmentSection))
            {
                IMapControl map = ((RingtoetsMapControl) view.Controls[0]).MapControl;

                var failureMechanismContext = new GrassCoverErosionOutwardsFailureMechanismContext(failureMechanism, assessmentSection);

                // Call
                view.Data = failureMechanismContext;

                // Assert
                Assert.AreSame(failureMechanismContext, view.Data);

                MapDataCollection mapData = map.Data;
                Assert.IsInstanceOf<MapDataCollection>(mapData);

                List<MapData> mapDataList = mapData.Collection.ToList();
                Assert.AreEqual(7, mapDataList.Count);
                MapDataTestHelper.AssertReferenceLineMapData(assessmentSection.ReferenceLine, mapDataList[referenceLineIndex]);
                MapDataTestHelper.AssertFailureMechanismSectionsMapData(failureMechanism.Sections, mapDataList[sectionsIndex]);
                MapDataTestHelper.AssertFailureMechanismSectionsStartPointMapData(failureMechanism.Sections, mapDataList[sectionsStartPointIndex]);
                MapDataTestHelper.AssertFailureMechanismSectionsEndPointMapData(failureMechanism.Sections, mapDataList[sectionsEndPointIndex]);
                MapDataTestHelper.AssertHydraulicBoundaryLocationsMapData(failureMechanism.HydraulicBoundaryLocations, mapDataList[hydraulicBoundaryLocationsIndex]);
                MapDataTestHelper.AssertForeshoreProfilesMapData(failureMechanism.ForeshoreProfiles, mapDataList[foreshoreProfilesIndex]);
                AssertCalculationsMapData(failureMechanism.Calculations.Cast<GrassCoverErosionOutwardsWaveConditionsCalculation>(),
                                          mapDataList[calculationsIndex]);
            }
        }

        [Test]
        public void GivenViewWithHydraulicBoundaryLocationsData_WhenHydraulicBoundaryDatabaseUpdatedAndNotified_ThenMapDataUpdated()
        {
            // Given
            var assessmentSection = new ObservableTestAssessmentSectionStub();
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism
            {
                HydraulicBoundaryLocations =
                {
                    new HydraulicBoundaryLocation(1, "test1", 1.0, 2.0)
                }
            };

            using (var view = new GrassCoverErosionOutwardsFailureMechanismView(failureMechanism, assessmentSection))
            {
                IMapControl map = ((RingtoetsMapControl) view.Controls[0]).MapControl;

                var failureMechanismContext = new GrassCoverErosionOutwardsFailureMechanismContext(failureMechanism, assessmentSection);
                view.Data = failureMechanismContext;

                MapData hydraulicBoundaryLocationsMapData = map.Data.Collection.ElementAt(hydraulicBoundaryLocationsIndex);

                // Precondition
                MapDataTestHelper.AssertHydraulicBoundaryLocationsMapData(failureMechanism.HydraulicBoundaryLocations, hydraulicBoundaryLocationsMapData);

                // When
                failureMechanism.HydraulicBoundaryLocations.Add(new HydraulicBoundaryLocation(2, "test2", 3.0, 4.0));
                failureMechanism.HydraulicBoundaryLocations.NotifyObservers();

                // Then
                MapDataTestHelper.AssertHydraulicBoundaryLocationsMapData(failureMechanism.HydraulicBoundaryLocations, hydraulicBoundaryLocationsMapData);
            }
        }

        [Test]
        public void GivenViewWithHydraulicBoundaryLocationsData_WhenLocationUpdatedAndNotified_ThenMapDataUpdated()
        {
            // Given
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "test1", 1.0, 2.0);
            var assessmentSection = new ObservableTestAssessmentSectionStub();
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism
            {
                HydraulicBoundaryLocations =
                {
                    hydraulicBoundaryLocation
                }
            };

            var random = new Random(21);
            using (var view = new GrassCoverErosionOutwardsFailureMechanismView(failureMechanism, assessmentSection))
            {
                IMapControl map = ((RingtoetsMapControl) view.Controls[0]).MapControl;

                var failureMechanismContext = new GrassCoverErosionOutwardsFailureMechanismContext(failureMechanism, assessmentSection);
                view.Data = failureMechanismContext;

                MapData hydraulicBoundaryLocationsMapData = map.Data.Collection.ElementAt(hydraulicBoundaryLocationsIndex);

                // Precondition
                AssertHydraulicBoundaryLocationOutputsMapData(failureMechanism.HydraulicBoundaryLocations, hydraulicBoundaryLocationsMapData);

                // When
                hydraulicBoundaryLocation.DesignWaterLevelCalculation1.Output = new TestHydraulicBoundaryLocationOutput(random.NextDouble());
                hydraulicBoundaryLocation.WaveHeightCalculation1.Output = new TestHydraulicBoundaryLocationOutput(random.NextDouble());
                hydraulicBoundaryLocation.NotifyObservers();

                // Then
                AssertHydraulicBoundaryLocationOutputsMapData(failureMechanism.HydraulicBoundaryLocations, hydraulicBoundaryLocationsMapData);
            }
        }

        [Test]
        public void GivenViewWithReferenceLineData_WhenReferenceLineUpdatedAndNotified_ThenMapDataUpdated()
        {
            // Given
            var assessmentSection = new ObservableTestAssessmentSectionStub
            {
                ReferenceLine = new ReferenceLine()
            };
            assessmentSection.ReferenceLine.SetGeometry(new List<Point2D>
            {
                new Point2D(1.0, 2.0),
                new Point2D(2.0, 1.0)
            });

            using (var view = new GrassCoverErosionOutwardsFailureMechanismView(new GrassCoverErosionOutwardsFailureMechanism(), assessmentSection))
            {
                IMapControl map = ((RingtoetsMapControl) view.Controls[0]).MapControl;

                var failureMechanismContext = new GrassCoverErosionOutwardsFailureMechanismContext(new GrassCoverErosionOutwardsFailureMechanism(), assessmentSection);
                view.Data = failureMechanismContext;

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
            }
        }

        [Test]
        public void GivenViewWithFailureMechanismSectionsData_WhenFailureMechanismSectionsUpdatedAndNotified_ThenMapDataUpdated()
        {
            // Given
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();

            using (var view = new GrassCoverErosionOutwardsFailureMechanismView(failureMechanism, new ObservableTestAssessmentSectionStub()))
            {
                IMapControl map = ((RingtoetsMapControl) view.Controls[0]).MapControl;

                var failureMechanismContext = new GrassCoverErosionOutwardsFailureMechanismContext(failureMechanism, new ObservableTestAssessmentSectionStub());
                view.Data = failureMechanismContext;

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

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            failureMechanism.ForeshoreProfiles.AddRange(new[]
            {
                foreshoreProfile
            }, "path");

            using (var view = new GrassCoverErosionOutwardsFailureMechanismView(failureMechanism, new ObservableTestAssessmentSectionStub()))
            {
                IMapControl map = ((RingtoetsMapControl) view.Controls[0]).MapControl;

                var failureMechanismContext = new GrassCoverErosionOutwardsFailureMechanismContext(failureMechanism, new ObservableTestAssessmentSectionStub());
                view.Data = failureMechanismContext;

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
            }
        }

        [Test]
        public void GivenViewWithForeshoreProfilesData_WhenForeshoreProfilesUpdatedAndNotified_ThenMapDataUpdated()
        {
            // Given
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            failureMechanism.ForeshoreProfiles.AddRange(new[]
            {
                new TestForeshoreProfile("originalProfile ID", new[]
                {
                    new Point2D(0, 0),
                    new Point2D(1, 1)
                })
            }, "path");

            using (var view = new GrassCoverErosionOutwardsFailureMechanismView(failureMechanism, new ObservableTestAssessmentSectionStub()))
            {
                IMapControl map = ((RingtoetsMapControl) view.Controls[0]).MapControl;

                var failureMechanismContext = new GrassCoverErosionOutwardsFailureMechanismContext(failureMechanism, new ObservableTestAssessmentSectionStub());
                view.Data = failureMechanismContext;

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
            }
        }

        [Test]
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

            using (var view = new GrassCoverErosionOutwardsFailureMechanismView(failureMechanism, new ObservableTestAssessmentSectionStub()))
            {
                IMapControl map = ((RingtoetsMapControl) view.Controls[0]).MapControl;

                var failureMechanismContext = new GrassCoverErosionOutwardsFailureMechanismContext(failureMechanism, new ObservableTestAssessmentSectionStub());
                view.Data = failureMechanismContext;

                var calculationMapData = (MapLineData) map.Data.Collection.ElementAt(calculationsIndex);

                // Precondition
                AssertCalculationsMapData(failureMechanism.Calculations.Cast<GrassCoverErosionOutwardsWaveConditionsCalculation>(), calculationMapData);

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
            }
        }

        [Test]
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

            using (var view = new GrassCoverErosionOutwardsFailureMechanismView(failureMechanism, new ObservableTestAssessmentSectionStub()))
            {
                IMapControl map = ((RingtoetsMapControl) view.Controls[0]).MapControl;

                var failureMechanismContext = new GrassCoverErosionOutwardsFailureMechanismContext(failureMechanism, new ObservableTestAssessmentSectionStub());
                view.Data = failureMechanismContext;

                var calculationMapData = (MapLineData) map.Data.Collection.ElementAt(calculationsIndex);

                // Precondition
                AssertCalculationsMapData(failureMechanism.Calculations.Cast<GrassCoverErosionOutwardsWaveConditionsCalculation>(), calculationMapData);

                // When
                calculationA.InputParameters.ForeshoreProfile = new TestForeshoreProfile(new Point2D(1.5, 1.5));
                calculationA.InputParameters.NotifyObservers();

                // Then
                AssertCalculationsMapData(failureMechanism.Calculations.Cast<GrassCoverErosionOutwardsWaveConditionsCalculation>(), calculationMapData);
            }
        }

        [Test]
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

            using (var view = new GrassCoverErosionOutwardsFailureMechanismView(failureMechanism, new ObservableTestAssessmentSectionStub()))
            {
                IMapControl map = ((RingtoetsMapControl) view.Controls[0]).MapControl;

                var failureMechanismContext = new GrassCoverErosionOutwardsFailureMechanismContext(failureMechanism, new ObservableTestAssessmentSectionStub());
                view.Data = failureMechanismContext;

                var calculationMapData = (MapLineData) map.Data.Collection.ElementAt(calculationsIndex);

                // Precondition
                AssertCalculationsMapData(failureMechanism.Calculations.Cast<GrassCoverErosionOutwardsWaveConditionsCalculation>(), calculationMapData);

                // When
                calculationA.Name = "new name";
                calculationA.NotifyObservers();

                // Then
                AssertCalculationsMapData(failureMechanism.Calculations.Cast<GrassCoverErosionOutwardsWaveConditionsCalculation>(), calculationMapData);
            }
        }

        [Test]
        public void UpdateObserver_DataUpdated_MapLayersSameOrder()
        {
            // Setup
            const int updatedRefenceLineLayerIndex = referenceLineIndex + 6;
            const int updatedSectionsLayerIndex = sectionsIndex - 1;
            const int updateSectionStartLayerIndex = sectionsStartPointIndex - 1;
            const int updatedSectionEndLayerIndex = sectionsEndPointIndex - 1;
            const int updatedHydraulicLocationsLayerIndex = hydraulicBoundaryLocationsIndex - 1;
            const int updatedForeshoreProfilesLayerIndex = foreshoreProfilesIndex - 1;
            const int updatedCalculationsIndex = calculationsIndex - 1;

            var assessmentSection = new ObservableTestAssessmentSectionStub();
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            using (var view = new GrassCoverErosionOutwardsFailureMechanismView(failureMechanism, assessmentSection))
            {
                IMapControl map = ((RingtoetsMapControl) view.Controls[0]).MapControl;

                var failureMechanismContext = new GrassCoverErosionOutwardsFailureMechanismContext(failureMechanism, assessmentSection);

                view.Data = failureMechanismContext;

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

                var actualCalculationsData = (MapLineData) mapDataList[updatedCalculationsIndex];
                Assert.AreEqual("Berekeningen", actualCalculationsData.Name);
            }
        }

        [Test]
        public void NotifyObservers_DataUpdatedNotifyObserversOnOldData_NoUpdateInViewData()
        {
            // Setup
            IAssessmentSection oldAssessmentSection = new ObservableTestAssessmentSectionStub();
            IAssessmentSection newAssessmentSection = new ObservableTestAssessmentSectionStub();

            newAssessmentSection.ReferenceLine = new ReferenceLine();
            newAssessmentSection.ReferenceLine.SetGeometry(new[]
            {
                new Point2D(2, 4),
                new Point2D(3, 4)
            });

            var oldGrassCoverErosionOutwardsFailureMechanismContext = new GrassCoverErosionOutwardsFailureMechanismContext(new GrassCoverErosionOutwardsFailureMechanism(), oldAssessmentSection);
            var newGrassCoverErosionOutwardsFailureMechanismContext = new GrassCoverErosionOutwardsFailureMechanismContext(new GrassCoverErosionOutwardsFailureMechanism(), newAssessmentSection);
            using (var view = new GrassCoverErosionOutwardsFailureMechanismView(new GrassCoverErosionOutwardsFailureMechanism(), oldAssessmentSection))
            {
                IMapControl map = ((RingtoetsMapControl) view.Controls[0]).MapControl;

                view.Data = oldGrassCoverErosionOutwardsFailureMechanismContext;
                view.Data = newGrassCoverErosionOutwardsFailureMechanismContext;
                MapData dataBeforeUpdate = map.Data;

                newAssessmentSection.ReferenceLine.SetGeometry(Enumerable.Empty<Point2D>());

                // Call
                oldAssessmentSection.NotifyObservers();

                // Assert
                Assert.AreEqual(dataBeforeUpdate, map.Data);
            }
        }

        private static void AssertHydraulicBoundaryLocationOutputsMapData(IEnumerable<HydraulicBoundaryLocation> hydraulicBoundaryLocations,
                                                                          MapData mapData)
        {
            var hydraulicLocationsMapData = (MapPointData) mapData;
            IEnumerable<HydraulicBoundaryLocation> hydraulicBoundaryLocationsArray = hydraulicBoundaryLocations.ToArray();
            CollectionAssert.AreEqual(hydraulicBoundaryLocationsArray.Select(hbl => hbl.DesignWaterLevelCalculation1.Output?.Result ?? RoundedDouble.NaN),
                                      hydraulicLocationsMapData.Features.Select(ft => (RoundedDouble) ft.MetaData["Waterstand bij doorsnede-eis"]));
            CollectionAssert.AreEqual(hydraulicBoundaryLocationsArray.Select(hbl => hbl.WaveHeightCalculation1.Output?.Result ?? RoundedDouble.NaN),
                                      hydraulicLocationsMapData.Features.Select(ft => (RoundedDouble) ft.MetaData["Golfhoogte bij doorsnede-eis"]));
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
                                               },
                                               geometries[0].PointCollections.First());
            }

            Assert.AreEqual("Berekeningen", mapData.Name);
        }

        private static void AssertEmptyMapData(MapDataCollection mapDataCollection)
        {
            Assert.AreEqual("Dijken en dammen - Grasbekleding erosie buitentalud", mapDataCollection.Name);

            List<MapData> mapDataList = mapDataCollection.Collection.ToList();

            Assert.AreEqual(7, mapDataList.Count);

            var referenceLineMapData = (MapLineData) mapDataList[referenceLineIndex];
            var sectionsMapData = (MapLineData) mapDataList[sectionsIndex];
            var foreshoreProfilesMapData = (MapLineData) mapDataList[foreshoreProfilesIndex];
            var sectionsStartPointMapData = (MapPointData) mapDataList[sectionsStartPointIndex];
            var sectionsEndPointMapData = (MapPointData) mapDataList[sectionsEndPointIndex];
            var hydraulicBoundaryLocationsMapData = (MapPointData) mapDataList[hydraulicBoundaryLocationsIndex];
            var calculationsMapData = (MapLineData) mapDataList[calculationsIndex];

            CollectionAssert.IsEmpty(referenceLineMapData.Features);
            CollectionAssert.IsEmpty(sectionsMapData.Features);
            CollectionAssert.IsEmpty(foreshoreProfilesMapData.Features);
            CollectionAssert.IsEmpty(sectionsStartPointMapData.Features);
            CollectionAssert.IsEmpty(sectionsEndPointMapData.Features);
            CollectionAssert.IsEmpty(hydraulicBoundaryLocationsMapData.Features);
            CollectionAssert.IsEmpty(calculationsMapData.Features);

            Assert.AreEqual("Referentielijn", referenceLineMapData.Name);
            Assert.AreEqual("Vakindeling", sectionsMapData.Name);
            Assert.AreEqual("Voorlandprofielen", foreshoreProfilesMapData.Name);
            Assert.AreEqual("Vakindeling (startpunten)", sectionsStartPointMapData.Name);
            Assert.AreEqual("Vakindeling (eindpunten)", sectionsEndPointMapData.Name);
            Assert.AreEqual("Hydraulische randvoorwaarden", hydraulicBoundaryLocationsMapData.Name);
            Assert.AreEqual("Berekeningen", calculationsMapData.Name);
        }
    }
}