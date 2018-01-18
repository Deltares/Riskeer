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
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using Core.Components.Gis.Data;
using Core.Components.Gis.Features;
using Core.Components.Gis.Geometries;
using Core.Components.Gis.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Util.TypeConverters;

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
        public void AssertHydraulicBoundaryLocationsMapData_FeaturesNotSameAsLocations_ThrowAssertionException()
        {
            // Setup
            IEnumerable<HydraulicBoundaryLocation> hydraulicBoundaryLocations = new[]
            {
                new HydraulicBoundaryLocation(1, "test1", 1, 0)
            };

            var mapData = new MapPointData("Hydraulische randvoorwaarden");

            // Call
            TestDelegate test = () => MapDataTestHelper.AssertHydraulicBoundaryLocationsMapData(hydraulicBoundaryLocations, mapData);

            // Assert
            Assert.Throws<AssertionException>(test);
        }

        [Test]
        public void AssertHydraulicBoundaryLocationsMapData_FeatureGeometryNotSameAsLocations_ThrowAssertionException()
        {
            // Setup
            IEnumerable<HydraulicBoundaryLocation> hydraulicBoundaryLocations = new[]
            {
                new HydraulicBoundaryLocation(1, "test1", 1, 0)
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
            TestDelegate test = () => MapDataTestHelper.AssertHydraulicBoundaryLocationsMapData(hydraulicBoundaryLocations, mapData);

            // Assert
            Assert.Throws<AssertionException>(test);
        }

        [Test]
        public void AssertHydraulicBoundaryLocationsMapData_WithHydraulicBoundaryLocationsMapDataCorrect_DoesNotThrow()
        {
            // Setup
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "test1", 1, 0);
            IEnumerable<HydraulicBoundaryLocation> hydraulicBoundaryLocations = new[]
            {
                hydraulicBoundaryLocation
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
            TestDelegate test = () => MapDataTestHelper.AssertHydraulicBoundaryLocationsMapData(hydraulicBoundaryLocations, mapData);

            // Assert
            Assert.DoesNotThrow(test);
        }

        [Test]
        [TestCaseSource(nameof(GetHydraulicBoundaryLocationUnequalOutputMapFeatures))]
        public void AssertHydraulicBoundaryLocationOutputsMapData_MapDataOutputFeaturesNotSameAsLocations_ThrowAssertionException(
            IEnumerable<MapFeature> mapFeatures)
        {
            // Setup
            IEnumerable<HydraulicBoundaryLocation> hydraulicBoundaryLocations = new[]
            {
                new HydraulicBoundaryLocation(1, "test1", 1, 0)
            };

            var mapData = new MapPointData("Hydraulische randvoorwaarden")
            {
                Features = mapFeatures
            };

            // Call
            TestDelegate test = () => MapDataTestHelper.AssertHydraulicBoundaryLocationOutputsMapData(hydraulicBoundaryLocations, mapData);

            // Assert
            Assert.Throws<AssertionException>(test);
        }

        [Test]
        [TestCaseSource(nameof(GetHydraulicBoundaryLocationEqualOutputMapFeatures))]
        public void AssertHydraulicBoundaryLocationOutputsMapData_MapDataOutputFeaturesSameAsLocations_DoesNotThrowAssertionException(
            HydraulicBoundaryLocation location,
            MapFeature mapFeature)
        {
            IEnumerable<HydraulicBoundaryLocation> hydraulicBoundaryLocations = new[]
            {
                location
            };

            var mapData = new MapPointData("Hydraulische randvoorwaarden")
            {
                Features = new[]
                {
                    mapFeature
                }
            };

            // Call
            TestDelegate test = () => MapDataTestHelper.AssertHydraulicBoundaryLocationOutputsMapData(hydraulicBoundaryLocations, mapData);

            // Assert
            Assert.DoesNotThrow(test);
        }

        private static IEnumerable<TestCaseData> GetHydraulicBoundaryLocationUnequalOutputMapFeatures()
        {
            var random = new Random(21);
            var mapGeometry = new MapGeometry(new[]
            {
                new[]
                {
                    new Point2D(1, 0)
                }
            });

            var differentWaveHeightMapFeature = new MapFeature(new[]
            {
                mapGeometry
            });
            differentWaveHeightMapFeature.MetaData["h(A+->A)"] = random.NextRoundedDouble();
            differentWaveHeightMapFeature.MetaData["h(A->B)"] = RoundedDouble.NaN;
            differentWaveHeightMapFeature.MetaData["h(B->C)"] = RoundedDouble.NaN;
            differentWaveHeightMapFeature.MetaData["h(C->D)"] = RoundedDouble.NaN;
            differentWaveHeightMapFeature.MetaData["Hs(A+->A)"] = RoundedDouble.NaN;
            differentWaveHeightMapFeature.MetaData["Hs(A->B)"] = RoundedDouble.NaN;
            differentWaveHeightMapFeature.MetaData["Hs(B->C)"] = RoundedDouble.NaN;
            differentWaveHeightMapFeature.MetaData["Hs(C->D)"] = RoundedDouble.NaN;
            var mapFeatures = new List<MapFeature>
            {
                differentWaveHeightMapFeature
            };
            yield return new TestCaseData(mapFeatures)
                .SetName("Different WaveHeight");

            var differentDesignWaterLevelMapFeature = new MapFeature(new[]
            {
                mapGeometry
            });
            differentDesignWaterLevelMapFeature.MetaData["h(A+->A)"] = RoundedDouble.NaN;
            differentDesignWaterLevelMapFeature.MetaData["h(A->B)"] = RoundedDouble.NaN;
            differentDesignWaterLevelMapFeature.MetaData["h(B->C)"] = RoundedDouble.NaN;
            differentDesignWaterLevelMapFeature.MetaData["h(C->D)"] = RoundedDouble.NaN;
            differentDesignWaterLevelMapFeature.MetaData["Hs(A+->A)"] = random.NextRoundedDouble();
            differentDesignWaterLevelMapFeature.MetaData["Hs(A->B)"] = RoundedDouble.NaN;
            differentDesignWaterLevelMapFeature.MetaData["Hs(B->C)"] = RoundedDouble.NaN;
            differentDesignWaterLevelMapFeature.MetaData["Hs(C->D)"] = RoundedDouble.NaN;
            mapFeatures = new List<MapFeature>
            {
                differentDesignWaterLevelMapFeature
            };
            yield return new TestCaseData(mapFeatures)
                .SetName("Different DesignWaterLevel");

            var mapFeatureOne = new MapFeature(new[]
            {
                mapGeometry
            });
            mapFeatureOne.MetaData["h(A+->A)"] = random.NextRoundedDouble();
            mapFeatureOne.MetaData["h(A->B)"] = RoundedDouble.NaN;
            mapFeatureOne.MetaData["h(B->C)"] = RoundedDouble.NaN;
            mapFeatureOne.MetaData["h(C->D)"] = RoundedDouble.NaN;
            mapFeatureOne.MetaData["Hs(A+->A)"] = RoundedDouble.NaN;
            mapFeatureOne.MetaData["Hs(A->B)"] = RoundedDouble.NaN;
            mapFeatureOne.MetaData["Hs(B->C)"] = RoundedDouble.NaN;
            mapFeatureOne.MetaData["Hs(C->D)"] = RoundedDouble.NaN;
            var mapFeatureTwo = new MapFeature(new[]
            {
                mapGeometry
            });
            mapFeatureTwo.MetaData["h(A+->A)"] = random.NextRoundedDouble();
            mapFeatureTwo.MetaData["h(A->B)"] = RoundedDouble.NaN;
            mapFeatureTwo.MetaData["h(B->C)"] = RoundedDouble.NaN;
            mapFeatureTwo.MetaData["h(C->D)"] = RoundedDouble.NaN;
            mapFeatureTwo.MetaData["Hs(A+->A)"] = RoundedDouble.NaN;
            mapFeatureTwo.MetaData["Hs(A->B)"] = RoundedDouble.NaN;
            mapFeatureTwo.MetaData["Hs(B->C)"] = RoundedDouble.NaN;
            mapFeatureTwo.MetaData["Hs(C->D)"] = RoundedDouble.NaN;
            mapFeatures = new List<MapFeature>
            {
                mapFeatureOne,
                mapFeatureTwo
            };
            yield return new TestCaseData(mapFeatures)
                .SetName("Different MapFeaturesCount");
        }

        private static IEnumerable<TestCaseData> GetHydraulicBoundaryLocationEqualOutputMapFeatures()
        {
            var location = new HydraulicBoundaryLocation(1, "test1", 1, 0);
            var mapGeometry = new MapGeometry(new[]
            {
                new[]
                {
                    new Point2D(1, 0)
                }
            });

            var mapFeatureNoOutput = new MapFeature(new[]
            {
                mapGeometry
            });
            mapFeatureNoOutput.MetaData["h(A+->A)"] = RoundedDouble.NaN;
            mapFeatureNoOutput.MetaData["h(A->B)"] = RoundedDouble.NaN;
            mapFeatureNoOutput.MetaData["h(B->C)"] = RoundedDouble.NaN;
            mapFeatureNoOutput.MetaData["h(C->D)"] = RoundedDouble.NaN;
            mapFeatureNoOutput.MetaData["Hs(A+->A)"] = RoundedDouble.NaN;
            mapFeatureNoOutput.MetaData["Hs(A->B)"] = RoundedDouble.NaN;
            mapFeatureNoOutput.MetaData["Hs(B->C)"] = RoundedDouble.NaN;
            mapFeatureNoOutput.MetaData["Hs(C->D)"] = RoundedDouble.NaN;
            yield return new TestCaseData(location, mapFeatureNoOutput)
                .SetName("LocationWithoutOutput");

            var random = new Random(21);
            location = new HydraulicBoundaryLocation(1, "test1", 1, 0);
            location.DesignWaterLevelCalculation1.Output = new TestHydraulicBoundaryLocationOutput(random.NextDouble());
            location.DesignWaterLevelCalculation2.Output = new TestHydraulicBoundaryLocationOutput(random.NextDouble());
            location.DesignWaterLevelCalculation3.Output = new TestHydraulicBoundaryLocationOutput(random.NextDouble());
            location.DesignWaterLevelCalculation4.Output = new TestHydraulicBoundaryLocationOutput(random.NextDouble());
            location.WaveHeightCalculation1.Output = new TestHydraulicBoundaryLocationOutput(random.NextDouble());
            location.WaveHeightCalculation2.Output = new TestHydraulicBoundaryLocationOutput(random.NextDouble());
            location.WaveHeightCalculation3.Output = new TestHydraulicBoundaryLocationOutput(random.NextDouble());
            location.WaveHeightCalculation4.Output = new TestHydraulicBoundaryLocationOutput(random.NextDouble());
            var mapFeatureWithOutput = new MapFeature(new[]
            {
                mapGeometry
            });
            mapFeatureWithOutput.MetaData["h(A+->A)"] = location.DesignWaterLevelCalculation1.Output.Result;
            mapFeatureWithOutput.MetaData["h(A->B)"] = location.DesignWaterLevelCalculation2.Output.Result;
            mapFeatureWithOutput.MetaData["h(B->C)"] = location.DesignWaterLevelCalculation3.Output.Result;
            mapFeatureWithOutput.MetaData["h(C->D)"] = location.DesignWaterLevelCalculation4.Output.Result;
            mapFeatureWithOutput.MetaData["Hs(A+->A)"] = location.WaveHeightCalculation1.Output.Result;
            mapFeatureWithOutput.MetaData["Hs(A->B)"] = location.WaveHeightCalculation2.Output.Result;
            mapFeatureWithOutput.MetaData["Hs(B->C)"] = location.WaveHeightCalculation3.Output.Result;
            mapFeatureWithOutput.MetaData["Hs(C->D)"] = location.WaveHeightCalculation4.Output.Result;
            yield return new TestCaseData(location, mapFeatureWithOutput)
                .SetName("LocationWithOutput");
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
                            new[]
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
                            new[]
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
                            new[]
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
                            new[]
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
                            new[]
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
                            new[]
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
                            new[]
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
                            new[]
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
        public void AssertForeshoreProfilesMapData_MapDataFeaturesLengthNotSameAsExpectedForeshoreProfilesLength_ThrowsAssertionException()
        {
            // Setup
            var mapData = new MapLineData("Voorlandprofielen")
            {
                Features = new[]
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
                        new MapGeometry(new[]
                        {
                            new[]
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
                new TestForeshoreProfile(new[]
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
                        new MapGeometry(new[]
                        {
                            new[]
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
                new TestForeshoreProfile(new[]
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
                        new MapGeometry(new[]
                        {
                            new[]
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
                new TestForeshoreProfile(new[]
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

        #region AssertImageBasedMapData

        [Test]
        public void AssertImageBasedMapData_ImageBasedMapDataNotSupported_ThrowAssertionException()
        {
            // Setup
            var imageBasedMapData = new SimpleImageBasedMapData();
            var backgroundData = new BackgroundData(new TestBackgroundDataConfiguration());

            // Call
            TestDelegate test = () => MapDataTestHelper.AssertImageBasedMapData(backgroundData, imageBasedMapData);

            // Assert
            Assert.Throws<AssertionException>(test, "Unsupported background configuration.");
        }

        [Test]
        [TestCaseSource(nameof(NotEqualToDefaultPdokMapData))]
        public void AssertImageBasedMapData_WmtsMapDataNotEqual_ThrowAssertionException(WmtsMapData wmtsMapData)
        {
            // Setup
            WmtsMapData mapData = WmtsMapDataTestHelper.CreateDefaultPdokMapData();
            BackgroundData backgroundData = BackgroundDataConverter.ConvertTo(wmtsMapData);

            // Call
            TestDelegate test = () => MapDataTestHelper.AssertImageBasedMapData(backgroundData, mapData);

            // Assert
            Assert.Throws<AssertionException>(test);
        }

        [Test]
        [TestCaseSource(nameof(NotEqualToBingAerial))]
        public void AssertImageBasedMapData_WellKnownTileSourceMapDataNotEqual_ThrowAssertionException(WellKnownTileSourceMapData wellKnownTileSourceMapData)
        {
            // Setup
            var mapData = new WellKnownTileSourceMapData(WellKnownTileSource.BingAerial);
            BackgroundData backgroundData = BackgroundDataConverter.ConvertTo(wellKnownTileSourceMapData);

            // Call
            TestDelegate test = () => MapDataTestHelper.AssertImageBasedMapData(backgroundData, mapData);

            // Assert
            Assert.Throws<AssertionException>(test);
        }

        [Test]
        public void AssertImageBasedMapData_WellKnownDataCorrect_DoesNotThrow()
        {
            // Setup
            var expectedMapData = new WellKnownTileSourceMapData(WellKnownTileSource.BingAerial);
            var actualMapData = new WellKnownTileSourceMapData(WellKnownTileSource.BingAerial);
            BackgroundData backgroundData = BackgroundDataConverter.ConvertTo(expectedMapData);

            // Call
            TestDelegate test = () => MapDataTestHelper.AssertImageBasedMapData(backgroundData, actualMapData);

            // Assert
            Assert.DoesNotThrow(test);
        }

        [Test]
        public void AssertImageBasedMapData_WmtsDataCorrect_DoesNotThrow()
        {
            // Setup
            WmtsMapData expectedMapData = WmtsMapDataTestHelper.CreateUnconnectedMapData();
            WmtsMapData actualMapData = WmtsMapDataTestHelper.CreateUnconnectedMapData();
            BackgroundData backgroundData = BackgroundDataConverter.ConvertTo(expectedMapData);

            // Call
            TestDelegate test = () => MapDataTestHelper.AssertImageBasedMapData(backgroundData, actualMapData);

            // Assert
            Assert.DoesNotThrow(test);
        }

        private class SimpleImageBasedMapData : ImageBasedMapData
        {
            public SimpleImageBasedMapData() : base("name") {}
        }

        private static IEnumerable<TestCaseData> NotEqualToDefaultPdokMapData()
        {
            WmtsMapData defaultMapData = WmtsMapDataTestHelper.CreateDefaultPdokMapData();

            var otherName = new WmtsMapData("otherName",
                                            defaultMapData.SourceCapabilitiesUrl,
                                            defaultMapData.SelectedCapabilityIdentifier,
                                            defaultMapData.PreferredFormat);
            yield return new TestCaseData(otherName).SetName("WmtsMapDataOtherName");

            var otherSourceCapabilitiesUrl = new WmtsMapData(defaultMapData.Name,
                                                             "otherSourceCapabilitiesUrl",
                                                             defaultMapData.SelectedCapabilityIdentifier,
                                                             defaultMapData.PreferredFormat);
            yield return new TestCaseData(otherSourceCapabilitiesUrl).SetName("WmtsMapDataOtherSourceCapabilitiesUrl");

            var otherSelectedCapabilityIdentifier = new WmtsMapData(defaultMapData.Name,
                                                                    defaultMapData.SourceCapabilitiesUrl,
                                                                    "otherSelectedCapabilityIdentifier",
                                                                    defaultMapData.PreferredFormat);
            yield return new TestCaseData(otherSelectedCapabilityIdentifier).SetName("WmtsMapDataOtherSelectedCapabilityIdentifier");

            var otherPreferredFormat = new WmtsMapData(defaultMapData.Name,
                                                       defaultMapData.SourceCapabilitiesUrl,
                                                       defaultMapData.SelectedCapabilityIdentifier,
                                                       "image/otherPreferredFormat");
            yield return new TestCaseData(otherPreferredFormat).SetName("WmtsMapDataOtherPreferredFormat");

            WmtsMapData otherVisibility = WmtsMapDataTestHelper.CreateDefaultPdokMapData();
            otherVisibility.IsVisible = !otherVisibility.IsVisible;
            yield return new TestCaseData(otherVisibility).SetName("WmtsMapDataOtherVisibility");

            WmtsMapData otherTransparency = WmtsMapDataTestHelper.CreateDefaultPdokMapData();
            otherTransparency.Transparency = (RoundedDouble) ((otherTransparency.Transparency + 0.5) % 1);
            yield return new TestCaseData(otherTransparency).SetName("WmtsMapDataOtherTransparency");
        }

        private static IEnumerable<TestCaseData> NotEqualToBingAerial()
        {
            var otherName = new WellKnownTileSourceMapData(WellKnownTileSource.BingAerial)
            {
                Name = "otherName"
            };
            yield return new TestCaseData(otherName).SetName("WellKnownTileSourceOtherName");

            var otherPreferredFormat = new WellKnownTileSourceMapData(WellKnownTileSource.EsriWorldShadedRelief)
            {
                Name = "Bing Maps - Satelliet"
            };
            yield return new TestCaseData(otherPreferredFormat).SetName("WellKnownTileSourceOtherPreferredFormat");

            var otherVisibility = new WellKnownTileSourceMapData(WellKnownTileSource.BingAerial);
            otherVisibility.IsVisible = !otherVisibility.IsVisible;
            yield return new TestCaseData(otherVisibility).SetName("WellKnownTileSourceOtherVisibility");

            var otherTransparency = new WellKnownTileSourceMapData(WellKnownTileSource.BingAerial);
            otherTransparency.Transparency = (RoundedDouble) ((otherTransparency.Transparency + 0.5) % 1);
            yield return new TestCaseData(otherTransparency).SetName("WellKnownTileSourceOtherTransparency");
        }

        #endregion

        #region AssertStructuresMapData

        [Test]
        public void AssertStructuresMapData_StructuresNull_ThrowAssertionException()
        {
            // Setup
            var mapData = new MapLineData("Kunstwerken");

            // Call
            TestDelegate test = () => MapDataTestHelper.AssertStructuresMapData(null, mapData);

            // Assert
            Assert.Throws<AssertionException>(test);
        }

        [Test]
        public void AssertStructuresMapData_MapDataNull_ThrowAssertionException()
        {
            // Call
            TestDelegate test = () => MapDataTestHelper.AssertStructuresMapData(Enumerable.Empty<StructureBase>(), null);

            // Assert
            Assert.Throws<AssertionException>(test);
        }

        [Test]
        public void AssertStructuresMapData_MapDataNotMapPointData_ThrowAssertionException()
        {
            // Setup
            var mapData = new MapLineData("Kunstwerken");

            // Call
            TestDelegate test = () => MapDataTestHelper.AssertStructuresMapData(Enumerable.Empty<StructureBase>(), mapData);

            // Assert
            Assert.Throws<AssertionException>(test);
        }

        [Test]
        public void AssertStructuresMapData_MapDataNameNotCorrect_ThrowAssertionException()
        {
            // Setup
            var mapData = new MapPointData("Other name")
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
            var structures = new[]
            {
                new TestStructure(new Point2D(0, 0))
            };

            // Call
            TestDelegate test = () => MapDataTestHelper.AssertStructuresMapData(structures, mapData);

            // Assert
            Assert.Throws<AssertionException>(test);
        }

        [Test]
        public void AssertStructuresMapData_MapDataFeaturesLengthNotSameAsExpectedStructuresLength_ThrowsAssertionException()
        {
            // Setup
            var mapData = new MapPointData("Kunstwerken")
            {
                Features = new[]
                {
                    new MapFeature(Enumerable.Empty<MapGeometry>())
                }
            };
            var structures = new[]
            {
                new TestStructure(),
                new TestStructure()
            };

            // Call
            TestDelegate test = () => MapDataTestHelper.AssertStructuresMapData(structures, mapData);

            // Assert
            Assert.Throws<AssertionException>(test);
        }

        [Test]
        public void AssertStructuresMapData_FeatureGeometryNotSameAsExpectedLocation_ThrowAssertionException()
        {
            // Setup
            var mapData = new MapPointData("Kunstwerken")
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
            var structures = new[]
            {
                new TestStructure(new Point2D(1, 1))
            };

            // Call
            TestDelegate test = () => MapDataTestHelper.AssertStructuresMapData(structures, mapData);

            // Assert
            Assert.Throws<AssertionException>(test);
        }

        [Test]
        public void AssertStructuresMapData_MultiplePointsInGeometry_ThrowAssertionException()
        {
            // Setup
            var mapData = new MapPointData("Kunstwerken")
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
                                new Point2D(0, -1)
                            }
                        })
                    })
                }
            };
            var structures = new[]
            {
                new TestStructure(new Point2D(0, 0))
            };

            // Call
            TestDelegate test = () => MapDataTestHelper.AssertStructuresMapData(structures, mapData);

            // Assert
            Assert.Throws<AssertionException>(test);
        }

        [Test]
        public void AssertStructuresMapData_DataCorrect_DoesNotThrow()
        {
            // Setup
            var mapData = new MapPointData("Kunstwerken")
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
            var structures = new[]
            {
                new TestStructure(new Point2D(0, 0))
            };

            // Call
            TestDelegate test = () => MapDataTestHelper.AssertStructuresMapData(structures, mapData);

            // Assert
            Assert.DoesNotThrow(test);
        }

        #endregion
    }
}