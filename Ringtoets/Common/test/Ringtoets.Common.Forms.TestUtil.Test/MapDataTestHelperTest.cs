// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

using System.Linq;
using Core.Common.Base.Geometry;
using Core.Components.Gis.Data;
using Core.Components.Gis.Features;
using Core.Components.Gis.Geometries;
using NUnit.Framework;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;

namespace Ringtoets.Common.Forms.TestUtil.Test
{
    [TestFixture]
    public class MapDataTestHelperTest
    {
        #region AssertFailureMechanismSectionsMapData

        [Test]
        public void AssertFailureMechanismSectionsMapData_MapDataNotMapLineData_ThrowAssertionException()
        {
            // Setup
            var mapData = new MapPointData("Vakindeling");

            // Call
            TestDelegate test = () => MapDataTestHelper.AssertFailureMechanismSectionsMapData(Enumerable.Empty<FailureMechanismSection>(), mapData);

            // Assert
            Assert.Throws<AssertionException>(test);
        }

        [Test]
        public void AssertFailureMechanismSectionsMapData_FeaturesLengthDifferentFromSectionsLength_ThrowAssertionException()
        {
            // Setup
            var sections = new[]
            {
                new FailureMechanismSection("section1", new[]
                {
                    new Point2D(0, 0)
                })
            };

            var mapData = new MapLineData("Vakindeling");

            // Precondition
            CollectionAssert.IsEmpty(mapData.Features);

            // Call
            TestDelegate test = () => MapDataTestHelper.AssertFailureMechanismSectionsMapData(sections, mapData);

            // Assert
            Assert.Throws<AssertionException>(test);
        }

        [Test]
        public void AssertFailureMechanismSectionsMapData_GeometryNotSame_ThrowAssertionException()
        {
            // Setup
            var sections = new[]
            {
                new FailureMechanismSection("section1", new[]
                {
                    new Point2D(0, 0),
                    new Point2D(1, 1),
                    new Point2D(2, 2)
                })
            };

            var mapData = new MapLineData("Vakindeling")
            {
                Features = new[]
                {
                    new MapFeature(new[]
                    {
                        new MapGeometry(new[]
                        {
                            new[]
                            {
                                new Point2D(0, 0),
                                new Point2D(1, 1)
                            }
                        })
                    })
                }
            };

            // Call
            TestDelegate test = () => MapDataTestHelper.AssertFailureMechanismSectionsMapData(sections, mapData);

            // Assert
            Assert.Throws<AssertionException>(test);
        }

        [Test]
        public void AssertFailureMechanismSectionsMapData_MapDataNameNotCorrect_ThrowAssertionException()
        {
            // Setup
            var geometryPoints = new[]
            {
                new Point2D(0, 0),
                new Point2D(1, 1),
                new Point2D(2, 2)
            };

            var sections = new[]
            {
                new FailureMechanismSection("section1", geometryPoints)
            };

            var mapData = new MapLineData("test")
            {
                Features = new[]
                {
                    new MapFeature(new[]
                    {
                        new MapGeometry(new[]
                        {
                            geometryPoints
                        })
                    })
                }
            };

            // Call
            TestDelegate test = () => MapDataTestHelper.AssertFailureMechanismSectionsMapData(sections, mapData);

            // Assert
            Assert.Throws<AssertionException>(test);
        }

        [Test]
        public void AssertFailureMechanismSectionsMapData_MapDataCorrectToSections_DoesNotThrow()
        {
            // Setup
            var geometryPoints = new[]
            {
                new Point2D(0, 0),
                new Point2D(1, 1),
                new Point2D(2, 2)
            };

            var sections = new[]
            {
                new FailureMechanismSection("section1", geometryPoints)
            };

            var mapData = new MapLineData("Vakindeling")
            {
                Features = new[]
                {
                    new MapFeature(new[]
                    {
                        new MapGeometry(new[]
                        {
                            geometryPoints
                        })
                    })
                }
            };

            // Call
            TestDelegate test = () => MapDataTestHelper.AssertFailureMechanismSectionsMapData(sections, mapData);

            // Assert
            Assert.DoesNotThrow(test);
        }

        #endregion

        #region AssertHydraulicBoundaryLocationsMapData

        [Test]
        public void AssertHydraulicBoundaryLocationsMapData_MapDataNotPointData_ThrowAssertionException()
        {
            // Setup
            var mapData = new MapLineData("Hydraulische randvoorwaarden");

            // Call
            TestDelegate test = () => MapDataTestHelper.AssertHydraulicBoundaryLocationsMapData(Enumerable.Empty<HydraulicBoundaryLocation>(), mapData);

            // Assert
            Assert.Throws<AssertionException>(test);
        }

        [Test]
        public void AssertHydraulicBoundaryLocationsMapData_DatabaseNullMapDataHasFeatures_ThrowAssertionException()
        {
            // Setup
            var mapData = new MapPointData("Hydraulische randvoorwaarden")
            {
                Features = new[]
                {
                    new MapFeature(new[]
                    {
                        new MapGeometry(new[]
                        {
                            Enumerable.Empty<Point2D>()
                        })
                    }),
                    new MapFeature(new[]
                    {
                        new MapGeometry(new[]
                        {
                            Enumerable.Empty<Point2D>()
                        })
                    })
                }
            };

            // Call
            TestDelegate test = () => MapDataTestHelper.AssertHydraulicBoundaryLocationsMapData(null, mapData);

            // Assert
            Assert.Throws<AssertionException>(test);
        }

        [Test]
        public void AssertHydraulicBoundaryLocationsMapData_FeaturesNotSameAsLocations_ThrowAssertionException()
        {
            // Setup
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                Locations =
                {
                    new HydraulicBoundaryLocation(1, "test1", 0, 0)
                }
            };

            var mapData = new MapPointData("Hydraulische randvoorwaarden");

            // Call
            TestDelegate test = () => MapDataTestHelper.AssertHydraulicBoundaryLocationsMapData(hydraulicBoundaryDatabase.Locations, mapData);

            // Assert
            Assert.Throws<AssertionException>(test);
        }

        [Test]
        public void AssertHydraulicBoundaryLocationsMapData_FeatureGeometryNotSameAsLocations_ThrowAssertionException()
        {
            // Setup
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                Locations =
                {
                    new HydraulicBoundaryLocation(1, "test1", 1, 0)
                }
            };

            var mapData = new MapPointData("Hydraulische randvoorwaarden")
            {
                Features = new[]
                {
                    new MapFeature(new[]
                    {
                        new MapGeometry(new[]
                        {
                            new[]
                            {
                                new Point2D(0, 0)
                            }
                        })
                    })
                }
            };

            // Call
            TestDelegate test = () => MapDataTestHelper.AssertHydraulicBoundaryLocationsMapData(hydraulicBoundaryDatabase.Locations, mapData);

            // Assert
            Assert.Throws<AssertionException>(test);
        }

        [Test]
        public void AssertHydraulicBoundaryLocationsMapData_MapDataNameNotCorrect_ThrowAssertionException()
        {
            // Setup
            var mapData = new MapPointData("test");

            // Call
            TestDelegate test = () => MapDataTestHelper.AssertHydraulicBoundaryLocationsMapData(null, mapData);

            // Assert
            Assert.Throws<AssertionException>(test);
        }

        [Test]
        public void AssertHydraulicBoundaryLocationsMapData_WithoutDatabaseMapDataCorrect_DoesNotThrow()
        {
            // Setup
            var mapData = new MapPointData("Hydraulische randvoorwaarden");

            // Call
            TestDelegate test = () => MapDataTestHelper.AssertHydraulicBoundaryLocationsMapData(null, mapData);

            // Assert
            Assert.DoesNotThrow(test);
        }

        [Test]
        public void AssertHydraulicBoundaryLocationsMapData_WithDatabaseMapDataCorrect_DoesNotThrow()
        {
            // Setup
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                Locations =
                {
                    new HydraulicBoundaryLocation(1, "test1", 1, 0)
                }
            };

            var mapData = new MapPointData("Hydraulische randvoorwaarden")
            {
                Features = new[]
                {
                    new MapFeature(new[]
                    {
                        new MapGeometry(new[]
                        {
                            new[]
                            {
                                new Point2D(1, 0)
                            }
                        })
                    })
                }
            };

            // Call
            TestDelegate test = () => MapDataTestHelper.AssertHydraulicBoundaryLocationsMapData(hydraulicBoundaryDatabase.Locations, mapData);

            // Assert
            Assert.DoesNotThrow(test);
        }

        #endregion

        #region AssertReferenceLineMapData

        [Test]
        public void AssertReferenceLineMapData_MapDataNotLineData_ThrowAssertionException()
        {
            // Setup
            var mapData = new MapPolygonData("Referentielijn");

            // Call
            TestDelegate test = () => MapDataTestHelper.AssertReferenceLineMapData(new ReferenceLine(), mapData);

            // Assert
            Assert.Throws<AssertionException>(test);
        }

        [Test]
        public void AssertReferenceLineMapData_ReferenceLineNullMapDataHasFeatures_ThrowAssertionException()
        {
            // Setup
            var mapData = new MapLineData("Referentielijn")
            {
                Features = new[]
                {
                    new MapFeature(new[]
                    {
                        new MapGeometry(new[]
                        {
                            Enumerable.Empty<Point2D>()
                        })
                    })
                }
            };

            // Call
            TestDelegate test = () => MapDataTestHelper.AssertReferenceLineMapData(null, mapData);

            // Assert
            Assert.Throws<AssertionException>(test);
        }

        [Test]
        public void AssertReferenceLineMapData_MapDataMoreThanOneFeature_ThrowAssertionException()
        {
            // Setup
            var mapData = new MapLineData("Referentielijn")
            {
                Features = new[]
                {
                    new MapFeature(new[]
                    {
                        new MapGeometry(new[]
                        {
                            Enumerable.Empty<Point2D>()
                        })
                    }),
                    new MapFeature(new[]
                    {
                        new MapGeometry(new[]
                        {
                            Enumerable.Empty<Point2D>()
                        })
                    })
                }
            };

            var referenceLine = new ReferenceLine();
            referenceLine.SetGeometry(new[]
                                      {
                                          new Point2D(0.0, 0.0),
                                          new Point2D(1.0, 1.0),
                                          new Point2D(2.0, 2.0)
                                      });

            // Call
            TestDelegate test = () => MapDataTestHelper.AssertReferenceLineMapData(referenceLine, mapData);

            // Assert
            Assert.Throws<AssertionException>(test);
        }

        [Test]
        public void AssertReferenceLineMapData_FeatureGeometryNotSameAsReferenceLinePoints_ThrowAssertionException()
        {
            // Setup
            var mapData = new MapLineData("Referentielijn")
            {
                Features = new[]
                {
                    new MapFeature(new[]
                    {
                        new MapGeometry(new[]
                        {
                            new []
                            {
                                new Point2D(0.0, 0.0),
                                new Point2D(2.0, 2.0),
                                new Point2D(1.0, 1.0)
                            }
                        })
                    })
                }
            };

            var referenceLine = new ReferenceLine();
            referenceLine.SetGeometry(new[]
                                      {
                                          new Point2D(0.0, 0.0),
                                          new Point2D(1.0, 1.0),
                                          new Point2D(2.0, 2.0)
                                      });

            // Call
            TestDelegate test = () => MapDataTestHelper.AssertReferenceLineMapData(referenceLine, mapData);

            // Assert
            Assert.Throws<AssertionException>(test);
        }

        [Test]
        public void AssertReferenceLineMapData_MapDataNameNotCorrect_ThrowAssertionException()
        {
            // Setup
            var mapData = new MapLineData("test");

            // Call
            TestDelegate test = () => MapDataTestHelper.AssertReferenceLineMapData(null, mapData);

            // Assert
            Assert.Throws<AssertionException>(test);
        }

        [Test]
        public void AssertReferenceLineMapData_ReferenceLineNullMapDataCorrect_DoesNotThrow()
        {
            // Setup
            var mapData = new MapLineData("Referentielijn");

            // Call
            TestDelegate test = () => MapDataTestHelper.AssertReferenceLineMapData(null, mapData);

            // Assert
            Assert.DoesNotThrow(test);
        }

        [Test]
        public void AssertReferenceLineMapData_WithReferenceLineMapDataCorrect_DoesNotThrow()
        {
            // Setup
            // Setup
            var mapData = new MapLineData("Referentielijn")
            {
                Features = new[]
                {
                    new MapFeature(new[]
                    {
                        new MapGeometry(new[]
                        {
                            new []
                            {
                                new Point2D(0.0, 0.0),
                                new Point2D(1.0, 1.0),
                                new Point2D(2.0, 2.0)
                            }
                        })
                    })
                }
            };

            var referenceLine = new ReferenceLine();
            referenceLine.SetGeometry(new[]
                                      {
                                          new Point2D(0.0, 0.0),
                                          new Point2D(1.0, 1.0),
                                          new Point2D(2.0, 2.0)
                                      });

            // Call
            TestDelegate test = () => MapDataTestHelper.AssertReferenceLineMapData(referenceLine, mapData);

            // Assert
            Assert.DoesNotThrow(test);
        }

        #endregion

        #region AssertFailureMechanismSectionsStartPointMapData

        [Test]
        public void AssertFailureMechanismSectionsStartPointMapData_MapDataNotMapPointData_ThrowAssertionException()
        {
            // Setup
            var mapData = new MapLineData("Vakindeling (startpunten)");

            // Call
            TestDelegate test = () => MapDataTestHelper.AssertFailureMechanismSectionsStartPointMapData(Enumerable.Empty<FailureMechanismSection>(), mapData);

            // Assert
            Assert.Throws<AssertionException>(test);
        }

        [Test]
        public void AssertFailureMechanismSectionsStartPointMapData_MapDataMoreThanOneFeature_ThrowAssertionException()
        {
            // Setup
            var mapData = new MapPointData("Vakindeling (startpunten)")
            {
                Features = new[]
                {
                    new MapFeature(new[]
                    {
                        new MapGeometry(new[]
                        {
                            Enumerable.Empty<Point2D>()
                        })
                    }),
                    new MapFeature(new[]
                    {
                        new MapGeometry(new[]
                        {
                            Enumerable.Empty<Point2D>()
                        })
                    })
                }
            };

            var sections = new[]
            {
                new FailureMechanismSection("section1", new[]
                {
                    new Point2D(0, 0),
                    new Point2D(1, 1),
                    new Point2D(2, 2)
                })
            };

            // Call
            TestDelegate test = () => MapDataTestHelper.AssertFailureMechanismSectionsStartPointMapData(sections, mapData);

            // Assert
            Assert.Throws<AssertionException>(test);
        }

        [Test]
        public void AssertFailureMechanismSectionsStartPointMapData_SectionsEmptyMapDataHasGeometry_ThrowAssertionException()
        {
            // Setup
            var mapData = new MapPointData("Vakindeling (startpunten)")
            {
                Features = new[]
                {
                    new MapFeature(new[]
                    {
                        new MapGeometry(new[]
                        {
                            new []
                            {
                                new Point2D(1, 1)
                            }
                        })
                    })
                }
            };

            // Call
            TestDelegate test = () => MapDataTestHelper.AssertFailureMechanismSectionsStartPointMapData(Enumerable.Empty<FailureMechanismSection>(), mapData);

            // Assert
            Assert.Throws<AssertionException>(test);
        }

        [Test]
        public void AssertFailureMechanismSectionsStartPointMapData_GeometryNotSameAsStartPoints_ThrowAssertionException()
        {
            // Setup
            var mapData = new MapPointData("Vakindeling (startpunten)")
            {
                Features = new[]
                {
                    new MapFeature(new[]
                    {
                        new MapGeometry(new[]
                        {
                            new []
                            {
                                new Point2D(1, 1)
                            }
                        })
                    })
                }
            };

            var sections = new[]
            {
                new FailureMechanismSection("section1", new[]
                {
                    new Point2D(0, 0),
                    new Point2D(1, 1),
                    new Point2D(2, 2)
                })
            };

            // Call
            TestDelegate test = () => MapDataTestHelper.AssertFailureMechanismSectionsStartPointMapData(sections, mapData);

            // Assert
            Assert.Throws<AssertionException>(test);
        }

        [Test]
        public void AssertFailureMechanismSectionsStartPointMapData_MapDataNameNotCorrect_ThrowAssertionException()
        {
            // Setup
            var geometryPoints = new[]
            {
                new Point2D(0, 0),
                new Point2D(1, 1),
                new Point2D(2, 2)
            };

            var sections = new[]
            {
                new FailureMechanismSection("section1", geometryPoints)
            };

            var mapData = new MapLineData("test")
            {
                Features = new[]
                {
                    new MapFeature(new[]
                    {
                        new MapGeometry(new[]
                        {
                            new[]
                            {
                                new Point2D(0, 0)
                            }
                        })
                    })
                }
            };

            // Call
            TestDelegate test = () => MapDataTestHelper.AssertFailureMechanismSectionsStartPointMapData(sections, mapData);

            // Assert
            Assert.Throws<AssertionException>(test);
        }

        [Test]
        public void AssertFailureMechanismSectionsStartPointMapData_MapDataCorrect_DoesNotThrow()
        {
            // Setup
            var mapData = new MapPointData("Vakindeling (startpunten)")
            {
                Features = new[]
                {
                    new MapFeature(new[]
                    {
                        new MapGeometry(new[]
                        {
                            new []
                            {
                                new Point2D(0, 0)
                            }
                        })
                    })
                }
            };

            var sections = new[]
            {
                new FailureMechanismSection("section1", new[]
                {
                    new Point2D(0, 0),
                    new Point2D(1, 1),
                    new Point2D(2, 2)
                })
            };

            // Call
            TestDelegate test = () => MapDataTestHelper.AssertFailureMechanismSectionsStartPointMapData(sections, mapData);

            // Assert
            Assert.DoesNotThrow(test);
        }

        #endregion

        #region AssertFailureMechanismSectionsEndPointMapData

        [Test]
        public void AssertFailureMechanismSectionsEndPointMapData_MapDataNotMapPointData_ThrowAssertionException()
        {
            // Setup
            var mapData = new MapLineData("Vakindeling (eindpunten)");

            // Call
            TestDelegate test = () => MapDataTestHelper.AssertFailureMechanismSectionsEndPointMapData(Enumerable.Empty<FailureMechanismSection>(), mapData);

            // Assert
            Assert.Throws<AssertionException>(test);
        }

        [Test]
        public void AssertFailureMechanismSectionsEndPointMapData_MapDataMoreThanOneFeature_ThrowAssertionException()
        {
            // Setup
            var mapData = new MapPointData("Vakindeling (eindpunten)")
            {
                Features = new[]
                {
                    new MapFeature(new[]
                    {
                        new MapGeometry(new[]
                        {
                            Enumerable.Empty<Point2D>()
                        })
                    }),
                    new MapFeature(new[]
                    {
                        new MapGeometry(new[]
                        {
                            Enumerable.Empty<Point2D>()
                        })
                    })
                }
            };

            var sections = new[]
            {
                new FailureMechanismSection("section1", new[]
                {
                    new Point2D(0, 0),
                    new Point2D(1, 1),
                    new Point2D(2, 2)
                })
            };

            // Call
            TestDelegate test = () => MapDataTestHelper.AssertFailureMechanismSectionsEndPointMapData(sections, mapData);

            // Assert
            Assert.Throws<AssertionException>(test);
        }

        [Test]
        public void AssertFailureMechanismSectionsEndPointMapData_SectionsEmptyMapDataHasGeometry_ThrowAssertionException()
        {
            // Setup
            var mapData = new MapPointData("Vakindeling (eindpunten)")
            {
                Features = new[]
                {
                    new MapFeature(new[]
                    {
                        new MapGeometry(new[]
                        {
                            new []
                            {
                                new Point2D(1, 1)
                            }
                        })
                    })
                }
            };

            // Call
            TestDelegate test = () => MapDataTestHelper.AssertFailureMechanismSectionsEndPointMapData(Enumerable.Empty<FailureMechanismSection>(), mapData);

            // Assert
            Assert.Throws<AssertionException>(test);
        }

        [Test]
        public void AssertFailureMechanismSectionsEndPointMapData_GeometryNotSameAsEndPoints_ThrowAssertionException()
        {
            // Setup
            var mapData = new MapPointData("Vakindeling (eindpunten)")
            {
                Features = new[]
                {
                    new MapFeature(new[]
                    {
                        new MapGeometry(new[]
                        {
                            new []
                            {
                                new Point2D(1, 1)
                            }
                        })
                    })
                }
            };

            var sections = new[]
            {
                new FailureMechanismSection("section1", new[]
                {
                    new Point2D(0, 0),
                    new Point2D(1, 1),
                    new Point2D(2, 2)
                })
            };

            // Call
            TestDelegate test = () => MapDataTestHelper.AssertFailureMechanismSectionsEndPointMapData(sections, mapData);

            // Assert
            Assert.Throws<AssertionException>(test);
        }

        [Test]
        public void AssertFailureMechanismSectionsEndPointMapData_MapDataNameNotCorrect_ThrowAssertionException()
        {
            // Setup
            var geometryPoints = new[]
            {
                new Point2D(0, 0),
                new Point2D(1, 1),
                new Point2D(2, 2)
            };

            var sections = new[]
            {
                new FailureMechanismSection("section1", geometryPoints)
            };

            var mapData = new MapLineData("test")
            {
                Features = new[]
                {
                    new MapFeature(new[]
                    {
                        new MapGeometry(new[]
                        {
                            new[]
                            {
                                new Point2D(0, 0)
                            }
                        })
                    })
                }
            };

            // Call
            TestDelegate test = () => MapDataTestHelper.AssertFailureMechanismSectionsEndPointMapData(sections, mapData);

            // Assert
            Assert.Throws<AssertionException>(test);
        }

        [Test]
        public void AssertFailureMechanismSectionsEndPointMapData_MapDataCorrect_DoesNotThrow()
        {
            // Setup
            var mapData = new MapPointData("Vakindeling (eindpunten)")
            {
                Features = new[]
                {
                    new MapFeature(new[]
                    {
                        new MapGeometry(new[]
                        {
                            new []
                            {
                                new Point2D(2, 2)
                            }
                        })
                    })
                }
            };

            var sections = new[]
            {
                new FailureMechanismSection("section1", new[]
                {
                    new Point2D(0, 0),
                    new Point2D(1, 1),
                    new Point2D(2, 2)
                })
            };

            // Call
            TestDelegate test = () => MapDataTestHelper.AssertFailureMechanismSectionsEndPointMapData(sections, mapData);

            // Assert
            Assert.DoesNotThrow(test);
        }

        #endregion

        #region AssertForeshoreProfilesMapData

        [Test]
        public void AssertForeshoreProfilesMapData_MapDataNotMapLineData_ThrowAssertionException()
        {
            // Setup
            var mapData = new MapPointData("Voorlandprofielen");

            // Call
            TestDelegate test = () => MapDataTestHelper.AssertForeshoreProfilesMapData(Enumerable.Empty<ForeshoreProfile>(), mapData);

            // Assert
            Assert.Throws<AssertionException>(test);
        }

        [Test]
        public void AssertForeshoreProfilesMapData_ForeshoreProfileLengthNotSameAsMapDataFeaturesLength_ThrowAssertionException()
        {
            // Setup
            var mapData = new MapLineData("Voorlandprofielen")
            {
                Features = new []
                {
                    new MapFeature(Enumerable.Empty<MapGeometry>())
                }
            };
            var foreshoreProfiles = new[]
            {
                new TestForeshoreProfile(),
                new TestForeshoreProfile()
            };

            // Call
            TestDelegate test = () => MapDataTestHelper.AssertForeshoreProfilesMapData(foreshoreProfiles, mapData);

            // Assert
            Assert.Throws<AssertionException>(test);
        }

        [Test]
        public void AssertForeshoreProfilesMapData_FeatureGeometryNotSameAsExpectedGeometry_ThrowAssertionException()
        {
            // Setup
            var mapData = new MapLineData("Voorlandprofielen")
            {
                Features = new[]
                {
                    new MapFeature(new[]
                                   {
                                       new MapGeometry(new []
                                                       {
                                                           new []
                                                           {
                                                               new Point2D(0, 1), 
                                                               new Point2D(0, -2)
                                                           }
                                                       })
                                   })
                }
            };
            var foreshoreProfiles = new[]
            {
                new TestForeshoreProfile(new []
                                         {
                                             new Point2D(0, 0), 
                                             new Point2D(1, 1)
                                         })
            };

            // Call
            TestDelegate test = () => MapDataTestHelper.AssertForeshoreProfilesMapData(foreshoreProfiles, mapData);

            // Assert
            Assert.Throws<AssertionException>(test);
        }

        [Test]
        public void AssertForeshoreProfilesMapData_MapDataNameNotCorrect_ThrowAssertionException()
        {
            // Setup
            var mapData = new MapLineData("test")
            {
                Features = new[]
                {
                    new MapFeature(new[]
                                   {
                                       new MapGeometry(new []
                                                       {
                                                           new []
                                                           {
                                                               new Point2D(0, 0),
                                                               new Point2D(0, -1)
                                                           }
                                                       })
                                   })
                }
            };
            var foreshoreProfiles = new[]
            {
                new TestForeshoreProfile(new []
                                         {
                                             new Point2D(0, 0),
                                             new Point2D(1, 1)
                                         })
            };

            // Call
            TestDelegate test = () => MapDataTestHelper.AssertForeshoreProfilesMapData(foreshoreProfiles, mapData);

            // Assert
            Assert.Throws<AssertionException>(test);
        }

        [Test]
        public void AssertForeshoreProfilesMapData_DataCorrect_DoesNotThrow()
        {
            // Setup
            var mapData = new MapLineData("Voorlandprofielen")
            {
                Features = new[]
                {
                    new MapFeature(new[]
                                   {
                                       new MapGeometry(new []
                                                       {
                                                           new []
                                                           {
                                                               new Point2D(0, 0), 
                                                               new Point2D(0, -1)
                                                           }
                                                       })
                                   })
                }
            };
            var foreshoreProfiles = new[]
            {
                new TestForeshoreProfile(new []
                                         {
                                             new Point2D(0, 0),
                                             new Point2D(1, 1)
                                         })
            };

            // Call
            TestDelegate test = () => MapDataTestHelper.AssertForeshoreProfilesMapData(foreshoreProfiles, mapData);

            // Assert
            Assert.DoesNotThrow(test);
        }

        #endregion
    }
}