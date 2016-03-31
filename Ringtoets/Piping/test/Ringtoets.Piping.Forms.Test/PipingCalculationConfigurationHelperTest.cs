using System;
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Data.TestUtil;
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.Forms.Test
{
    [TestFixture]
    public class PipingCalculationConfigurationHelperTest
    {
        #region GetPipingSoilProfilesForSurfaceLine

        [Test]
        public void GetPipingSoilProfilesForSurfaceLine_SurfaceLineIntersectingSoilModel_ReturnSoilProfilesOfSoilModel()
        {
            // Setup
            var soilProfile1 = new PipingSoilProfile("Profile 1", -10.0, new[]
            {
                new PipingSoilLayer(-5.0),
                new PipingSoilLayer(-2.0),
                new PipingSoilLayer(1.0)
            }, SoilProfileType.SoilProfile1D, 1);
            var soilProfile2 = new PipingSoilProfile("Profile 2", -8.0, new[]
            {
                new PipingSoilLayer(-4.0),
                new PipingSoilLayer(0.0),
                new PipingSoilLayer(4.0)
            }, SoilProfileType.SoilProfile1D, 2);

            var soilModel = new StochasticSoilModel(1, "A", "B");
            soilModel.Geometry.AddRange(new[]
            {
                new Point2D(1.0, 0.0),
                new Point2D(5.0, 0.0)
            });
            soilModel.StochasticSoilProfiles.AddRange(new[]
            {
                new StochasticSoilProfile(0.3, SoilProfileType.SoilProfile1D, 1)
                {
                    SoilProfile = soilProfile1
                },
                new StochasticSoilProfile(0.7, SoilProfileType.SoilProfile1D, 2)
                {
                    SoilProfile = soilProfile2
                }
            });
            var availableSoilModels = new[]
            {
                soilModel
            };

            var surfaceLine = new RingtoetsPipingSurfaceLine();
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(3.0, 5.0, 0.0),
                new Point3D(3.0, 0.0, 1.0),
                new Point3D(3.0, -5.0, 0.0)
            });

            // Call
            IEnumerable<PipingSoilProfile> result = PipingCalculationConfigurationHelper.GetPipingSoilProfilesForSurfaceLine(surfaceLine, availableSoilModels);

            // Assert
            PipingSoilProfile[] expected =
            {
                soilProfile1,
                soilProfile2
            };
            CollectionAssert.AreEquivalent(expected, result);
        }

        [Test]
        public void GetPipingSoilProfilesForSurfaceLine_NoSurfaceLine_ReturnEmpty()
        {
            // Setup
            var soilProfile1 = new PipingSoilProfile("Profile 1", -10.0, new[]
            {
                new PipingSoilLayer(-5.0),
                new PipingSoilLayer(-2.0),
                new PipingSoilLayer(1.0)
            }, SoilProfileType.SoilProfile1D, 1);
            var soilProfile2 = new PipingSoilProfile("Profile 2", -8.0, new[]
            {
                new PipingSoilLayer(-4.0),
                new PipingSoilLayer(0.0),
                new PipingSoilLayer(4.0)
            }, SoilProfileType.SoilProfile1D, 2);

            var soilModel = new StochasticSoilModel(1, "A", "B");
            soilModel.Geometry.AddRange(new[]
            {
                new Point2D(1.0, 0.0),
                new Point2D(5.0, 0.0)
            });
            soilModel.StochasticSoilProfiles.AddRange(new[]
            {
                new StochasticSoilProfile(0.3, SoilProfileType.SoilProfile1D, 1)
                {
                    SoilProfile = soilProfile1
                },
                new StochasticSoilProfile(0.7, SoilProfileType.SoilProfile1D, 2)
                {
                    SoilProfile = soilProfile2
                }
            });
            var availableSoilModels = new[]
            {
                soilModel
            };

            // Call
            IEnumerable<PipingSoilProfile> result = PipingCalculationConfigurationHelper.GetPipingSoilProfilesForSurfaceLine(null, availableSoilModels);

            // Assert
            CollectionAssert.IsEmpty(result);
        }

        [Test]
        public void GetPipingSoilProfilesForSurfaceLine_NoSoilModels_ReturnEmpty()
        {
            // Setup
            var surfaceLine = new RingtoetsPipingSurfaceLine();
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(3.0, 5.0, 0.0),
                new Point3D(3.0, 0.0, 1.0),
                new Point3D(3.0, -5.0, 0.0)
            });

            // Call
            IEnumerable<PipingSoilProfile> result = PipingCalculationConfigurationHelper.GetPipingSoilProfilesForSurfaceLine(surfaceLine, Enumerable.Empty<StochasticSoilModel>());

            // Assert
            CollectionAssert.IsEmpty(result);
        }

        [Test]
        public void GetPipingSoilProfilesForSurfaceLine_NoSoilProfiles_ReturnEmpty()
        {
            // Setup
            var soilModel = new StochasticSoilModel(1, "A", "B");
            soilModel.Geometry.AddRange(new[]
            {
                new Point2D(1.0, 0.0),
                new Point2D(5.0, 0.0)
            });

            var availableSoilModels = new[]
            {
                soilModel
            };

            var surfaceLine = new RingtoetsPipingSurfaceLine();
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(3.0, 5.0, 0.0),
                new Point3D(3.0, 0.0, 1.0),
                new Point3D(3.0, -5.0, 0.0)
            });

            // Call
            IEnumerable<PipingSoilProfile> result = PipingCalculationConfigurationHelper.GetPipingSoilProfilesForSurfaceLine(surfaceLine, availableSoilModels);

            // Assert
            CollectionAssert.IsEmpty(result);
        }

        [Test]
        public void GetPipingSoilProfilesForSurfaceLine_SoilModelGeometryNotIntersecting_ReturnEmpty()
        {
            // Setup
            var soilProfile1 = new PipingSoilProfile("Profile 1", -10.0, new[]
            {
                new PipingSoilLayer(-5.0),
                new PipingSoilLayer(-2.0),
                new PipingSoilLayer(1.0)
            }, SoilProfileType.SoilProfile1D, 1);
            var soilProfile2 = new PipingSoilProfile("Profile 2", -8.0, new[]
            {
                new PipingSoilLayer(-4.0),
                new PipingSoilLayer(0.0),
                new PipingSoilLayer(4.0)
            }, SoilProfileType.SoilProfile1D, 2);

            var soilModel = new StochasticSoilModel(1, "A", "B");
            soilModel.Geometry.AddRange(new[]
            {
                new Point2D(1.0, 0.0),
                new Point2D(5.0, 0.0)
            });
            soilModel.StochasticSoilProfiles.AddRange(new[]
            {
                new StochasticSoilProfile(0.3, SoilProfileType.SoilProfile1D, 1)
                {
                    SoilProfile = soilProfile1
                },
                new StochasticSoilProfile(0.7, SoilProfileType.SoilProfile1D, 2)
                {
                    SoilProfile = soilProfile2
                }
            });
            var availableSoilModels = new[]
            {
                soilModel
            };

            var surfaceLine = new RingtoetsPipingSurfaceLine();
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(0.0, 1.0, 0.0),
                new Point3D(2.5, 1.0, 1.0),
                new Point3D(5.0, 1.0, 0.0)
            });

            // Call
            IEnumerable<PipingSoilProfile> result = PipingCalculationConfigurationHelper.GetPipingSoilProfilesForSurfaceLine(surfaceLine, availableSoilModels);

            // Assert
            CollectionAssert.IsEmpty(result);
        }

        [Test]
        public void GetPipingSoilProfilesForSurfaceLine_SurfaceLineOverlappingSoilModel_ReturnSoilProfilesOfSoilModel()
        {
            // Setup
            var soilProfile1 = new PipingSoilProfile("Profile 1", -10.0, new[]
            {
                new PipingSoilLayer(-5.0),
                new PipingSoilLayer(-2.0),
                new PipingSoilLayer(1.0)
            }, SoilProfileType.SoilProfile1D, 1);
            var soilProfile2 = new PipingSoilProfile("Profile 2", -8.0, new[]
            {
                new PipingSoilLayer(-4.0),
                new PipingSoilLayer(0.0),
                new PipingSoilLayer(4.0)
            }, SoilProfileType.SoilProfile1D, 2);

            const double y = 1.1;
            var soilModel1 = new StochasticSoilModel(1, "A", "B");
            soilModel1.Geometry.AddRange(new[]
            {
                new Point2D(1.0, y),
                new Point2D(2.0, y)
            });
            soilModel1.StochasticSoilProfiles.AddRange(new[]
            {
                new StochasticSoilProfile(1.0, SoilProfileType.SoilProfile1D, 1)
                {
                    SoilProfile = soilProfile1
                }
            });

            var soilModel2 = new StochasticSoilModel(1, "A", "B");
            soilModel2.Geometry.AddRange(new[]
            {
                new Point2D(3.0, y),
                new Point2D(4.0, y)
            });
            soilModel2.StochasticSoilProfiles.AddRange(new[]
            {
                new StochasticSoilProfile(1.0, SoilProfileType.SoilProfile1D, 2)
                {
                    SoilProfile = soilProfile2
                }
            });
            var availableSoilModels = new[]
            {
                soilModel1,
                soilModel2
            };

            var surfaceLine = new RingtoetsPipingSurfaceLine();
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(5.0, y, 0.0),
                new Point3D(2.5, y, 1.0),
                new Point3D(0.0, y, 0.0)
            });

            // Call
            IEnumerable<PipingSoilProfile> result = PipingCalculationConfigurationHelper.GetPipingSoilProfilesForSurfaceLine(surfaceLine, availableSoilModels);

            // Assert
            PipingSoilProfile[] expected =
            {
                soilProfile1,
                soilProfile2
            };
            CollectionAssert.AreEquivalent(expected, result);
        }

        #endregion

        #region Generate

        [Test]
        public void GenerateCalculationsStructure_WithoutSurfaceLines_ReturnsEmptyCollection()
        {
            // Call
            var result = PipingCalculationConfigurationHelper.GenerateCalculationsStructure(null, null, null, null).ToList();

            // Assert
            Assert.AreEqual(0, result.Count);
        }

        [Test]
        public void GenerateCalculationsStructure_WithEmptySurfaceLines_ReturnsEmptyCollection()
        {
            // Call
            var result = PipingCalculationConfigurationHelper.GenerateCalculationsStructure(Enumerable.Empty<RingtoetsPipingSurfaceLine>(), null, null, null).ToList();

            // Assert
            Assert.AreEqual(0, result.Count);
        }

        [Test]
        public void GenerateCalculationsStructure_WithSurfaceLineAndSoilModelWithoutGeneralInput_ThrowsArgumentNullException()
        {
            // Setup
            var soilProfile1 = new PipingSoilProfile("Profile 1", -10.0, new[]
            {
                new PipingSoilLayer(-5.0),
                new PipingSoilLayer(-2.0),
                new PipingSoilLayer(1.0)
            }, SoilProfileType.SoilProfile1D, 1);

            var soilModel = new StochasticSoilModel(1, "A", "B");
            soilModel.Geometry.AddRange(new[]
            {
                new Point2D(1.0, 0.0),
                new Point2D(5.0, 0.0)
            });
            soilModel.StochasticSoilProfiles.Add(
                new StochasticSoilProfile(0.3, SoilProfileType.SoilProfile1D, 1)
                {
                    SoilProfile = soilProfile1
                }
            );
            var availableSoilModels = new[]
            {
                soilModel
            };

            var surfaceLine = new RingtoetsPipingSurfaceLine();
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(3.0, 5.0, 0.0),
                new Point3D(3.0, 0.0, 1.0),
                new Point3D(3.0, -5.0, 0.0)
            });

            var surfaceLines = new[]
            {
                surfaceLine
            };

            // Call
            TestDelegate test = () => PipingCalculationConfigurationHelper.GenerateCalculationsStructure(surfaceLines, availableSoilModels, null, new SemiProbabilisticPipingInput()).ToList();

            // Assert
            var parameter = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("generalInputParameters", parameter);
        }

        [Test]
        public void GenerateCalculationsStructure_WithSurfaceLineAndSoilModelWithoutSemiProbabilisticInput_ThrowsArgumentNullException()
        {
            // Setup
            var soilProfile1 = new PipingSoilProfile("Profile 1", -10.0, new[]
            {
                new PipingSoilLayer(-5.0),
                new PipingSoilLayer(-2.0),
                new PipingSoilLayer(1.0)
            }, SoilProfileType.SoilProfile1D, 1);

            var soilModel = new StochasticSoilModel(1, "A", "B");
            soilModel.Geometry.AddRange(new[]
            {
                new Point2D(1.0, 0.0),
                new Point2D(5.0, 0.0)
            });
            soilModel.StochasticSoilProfiles.Add(
                new StochasticSoilProfile(0.3, SoilProfileType.SoilProfile1D, 1)
                {
                    SoilProfile = soilProfile1
                }
            );
            var availableSoilModels = new[]
            {
                soilModel
            };

            var surfaceLine = new RingtoetsPipingSurfaceLine();
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(3.0, 5.0, 0.0),
                new Point3D(3.0, 0.0, 1.0),
                new Point3D(3.0, -5.0, 0.0)
            });

            var surfaceLines = new[]
            {
                surfaceLine
            };

            // Call
            TestDelegate test = () => PipingCalculationConfigurationHelper.GenerateCalculationsStructure(surfaceLines, availableSoilModels, new GeneralPipingInput(), null).ToList();

            // Assert
            var parameter = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("semiProbabilisticInputParameters", parameter);
        }

        [Test]
        public void GenerateCalculationsStructure_WithSurfaceLinesWithoutSoilModels_ReturnsFourEmptyGroups()
        {
            // Setup
            var testName1 = "group1";
            var testName2 = "group2";
            var testName3 = "group3";
            var testName4 = "group4";

            var ringtoetsPipingSurfaceLines = new List<RingtoetsPipingSurfaceLine>
            {
                new RingtoetsPipingSurfaceLine
                {
                    Name = testName1
                },
                new RingtoetsPipingSurfaceLine
                {
                    Name = testName2
                },
                new RingtoetsPipingSurfaceLine
                {
                    Name = testName3
                },
                new RingtoetsPipingSurfaceLine
                {
                    Name = testName4
                }
            };

            // Call
            var result = PipingCalculationConfigurationHelper.GenerateCalculationsStructure(ringtoetsPipingSurfaceLines, null, null, null).ToList();

            // Assert
            Assert.AreEqual(4, result.Count);
            CollectionAssert.AllItemsAreInstancesOfType(result, typeof(PipingCalculationGroup));
            Assert.AreEqual(new[]
            {
                testName1,
                testName2,
                testName3,
                testName4
            }, result.Select(g => g.Name));
            Assert.AreEqual(new[]
            {
                0,
                0,
                0,
                0
            }, result.Select(g => ((PipingCalculationGroup) g).Children.Count));
        }

        [Test]
        public void GenerateCalculationsStructure_WithSurfaceLinesWithEmptySoilModels_ReturnsFourEmptyGroups()
        {
            // Setup
            var testName1 = "group1";
            var testName2 = "group2";
            var testName3 = "group3";
            var testName4 = "group4";

            var ringtoetsPipingSurfaceLines = new List<RingtoetsPipingSurfaceLine>
            {
                new RingtoetsPipingSurfaceLine
                {
                    Name = testName1
                },
                new RingtoetsPipingSurfaceLine
                {
                    Name = testName2
                },
                new RingtoetsPipingSurfaceLine
                {
                    Name = testName3
                },
                new RingtoetsPipingSurfaceLine
                {
                    Name = testName4
                }
            };

            // Call
            var result = PipingCalculationConfigurationHelper.GenerateCalculationsStructure(ringtoetsPipingSurfaceLines, Enumerable.Empty<StochasticSoilModel>(), null, null).ToList();

            // Assert
            Assert.AreEqual(4, result.Count);
            CollectionAssert.AllItemsAreInstancesOfType(result, typeof(PipingCalculationGroup));
            Assert.AreEqual(new[]
            {
                testName1,
                testName2,
                testName3,
                testName4
            }, result.Select(g => g.Name));
            Assert.AreEqual(new[]
            {
                0,
                0,
                0,
                0
            }, result.Select(g => ((PipingCalculationGroup) g).Children.Count));
        }

        [Test]
        public void GenerateCalculationsStructure_SurfaceLineIntersectingSoilModel_ReturnOneGroupWithTwoCalculations()
        {
            // Setup
            var soilProfile1 = new PipingSoilProfile("Profile 1", -10.0, new[]
            {
                new PipingSoilLayer(-5.0),
                new PipingSoilLayer(-2.0),
                new PipingSoilLayer(1.0)
            }, SoilProfileType.SoilProfile1D, 1);
            var soilProfile2 = new PipingSoilProfile("Profile 2", -8.0, new[]
            {
                new PipingSoilLayer(-4.0),
                new PipingSoilLayer(0.0),
                new PipingSoilLayer(4.0)
            }, SoilProfileType.SoilProfile1D, 2);

            var soilModel = new StochasticSoilModel(1, "A", "B");
            soilModel.Geometry.AddRange(new[]
            {
                new Point2D(1.0, 0.0),
                new Point2D(5.0, 0.0)
            });
            soilModel.StochasticSoilProfiles.AddRange(new[]
            {
                new StochasticSoilProfile(0.3, SoilProfileType.SoilProfile1D, 1)
                {
                    SoilProfile = soilProfile1
                },
                new StochasticSoilProfile(0.7, SoilProfileType.SoilProfile1D, 2)
                {
                    SoilProfile = soilProfile2
                }
            });
            var availableSoilModels = new[]
            {
                soilModel
            };

            var surfaceLine = new RingtoetsPipingSurfaceLine();
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(3.0, 5.0, 0.0),
                new Point3D(3.0, 0.0, 1.0),
                new Point3D(3.0, -5.0, 0.0)
            });

            var surfaceLines = new[]
            {
                surfaceLine
            };

            // Call
            IEnumerable<IPipingCalculationItem> result = PipingCalculationConfigurationHelper.GenerateCalculationsStructure(surfaceLines, availableSoilModels, new GeneralPipingInput(), new SemiProbabilisticPipingInput()).ToList();

            // Assert
            Assert.AreEqual(1, result.Count());
            var calculationGroup = result.First() as PipingCalculationGroup;
            Assert.NotNull(calculationGroup);

            Assert.AreEqual(2, calculationGroup.Children.Count);
            CollectionAssert.AllItemsAreInstancesOfType(calculationGroup.Children, typeof(PipingCalculation));

            var calculationInput1 = ((PipingCalculation) calculationGroup.Children[0]).InputParameters;
            Assert.AreSame(soilProfile1, calculationInput1.SoilProfile);
            Assert.AreSame(surfaceLine, calculationInput1.SurfaceLine);

            var calculationInput2 = ((PipingCalculation) calculationGroup.Children[1]).InputParameters;
            Assert.AreSame(soilProfile2, calculationInput2.SoilProfile);
            Assert.AreSame(surfaceLine, calculationInput2.SurfaceLine);
        }

        [Test]
        public void GenerateCalculationsStructure_NoSoilProfiles_ReturnOneEmptyGroup()
        {
            // Setup
            var soilModel = new StochasticSoilModel(1, "A", "B");
            soilModel.Geometry.AddRange(new[]
            {
                new Point2D(1.0, 0.0),
                new Point2D(5.0, 0.0)
            });

            var availableSoilModels = new[]
            {
                soilModel
            };

            var surfaceLine = new RingtoetsPipingSurfaceLine();
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(3.0, 5.0, 0.0),
                new Point3D(3.0, 0.0, 1.0),
                new Point3D(3.0, -5.0, 0.0)
            });

            var surfaceLines = new[]
            {
                surfaceLine
            };

            // Call
            IEnumerable<IPipingCalculationItem> result = PipingCalculationConfigurationHelper.GenerateCalculationsStructure(surfaceLines, availableSoilModels, null, null).ToList();

            // Assert
            Assert.AreEqual(1, result.Count());
            var calculationGroup = result.First() as PipingCalculationGroup;
            Assert.NotNull(calculationGroup);

            Assert.IsEmpty(calculationGroup.Children);
        }

        [Test]
        public void GenerateCalculationsStructure_SoilModelGeometryNotIntersecting_ReturnOneEmptyGroup()
        {
            // Setup
            var soilProfile1 = new PipingSoilProfile("Profile 1", -10.0, new[]
            {
                new PipingSoilLayer(-5.0),
                new PipingSoilLayer(-2.0),
                new PipingSoilLayer(1.0)
            }, SoilProfileType.SoilProfile1D, 1);
            var soilProfile2 = new PipingSoilProfile("Profile 2", -8.0, new[]
            {
                new PipingSoilLayer(-4.0),
                new PipingSoilLayer(0.0),
                new PipingSoilLayer(4.0)
            }, SoilProfileType.SoilProfile1D, 2);

            var soilModel = new StochasticSoilModel(1, "A", "B");
            soilModel.Geometry.AddRange(new[]
            {
                new Point2D(1.0, 0.0),
                new Point2D(5.0, 0.0)
            });
            soilModel.StochasticSoilProfiles.AddRange(new[]
            {
                new StochasticSoilProfile(0.3, SoilProfileType.SoilProfile1D, 1)
                {
                    SoilProfile = soilProfile1
                },
                new StochasticSoilProfile(0.7, SoilProfileType.SoilProfile1D, 2)
                {
                    SoilProfile = soilProfile2
                }
            });
            var availableSoilModels = new[]
            {
                soilModel
            };

            var surfaceLine = new RingtoetsPipingSurfaceLine();
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(0.0, 1.0, 0.0),
                new Point3D(2.5, 1.0, 1.0),
                new Point3D(5.0, 1.0, 0.0)
            });

            var surfaceLines = new[]
            {
                surfaceLine
            };

            // Call
            IEnumerable<IPipingCalculationItem> result = PipingCalculationConfigurationHelper.GenerateCalculationsStructure(surfaceLines, availableSoilModels, null, null).ToList();

            // Assert
            Assert.AreEqual(1, result.Count());
            var calculationGroup = result.First() as PipingCalculationGroup;
            Assert.NotNull(calculationGroup);

            Assert.IsEmpty(calculationGroup.Children);
        }

        [Test]
        public void GenerateCalculationsStructure_SurfaceLineOverlappingSoilModel_ReturnOneGroupWithProfilesFromBothSoilModels()
        {
            // Setup
            var soilProfile1 = new PipingSoilProfile("Profile 1", -10.0, new[]
            {
                new PipingSoilLayer(-5.0),
                new PipingSoilLayer(-2.0),
                new PipingSoilLayer(1.0)
            }, SoilProfileType.SoilProfile1D, 1);
            var soilProfile2 = new PipingSoilProfile("Profile 2", -8.0, new[]
            {
                new PipingSoilLayer(-4.0),
                new PipingSoilLayer(0.0),
                new PipingSoilLayer(4.0)
            }, SoilProfileType.SoilProfile1D, 2);

            const double y = 1.1;
            var soilModel1 = new StochasticSoilModel(1, "A", "B");
            soilModel1.Geometry.AddRange(new[]
            {
                new Point2D(1.0, y),
                new Point2D(2.0, y)
            });
            soilModel1.StochasticSoilProfiles.AddRange(new[]
            {
                new StochasticSoilProfile(1.0, SoilProfileType.SoilProfile1D, 1)
                {
                    SoilProfile = soilProfile1
                }
            });

            var soilModel2 = new StochasticSoilModel(1, "A", "B");
            soilModel2.Geometry.AddRange(new[]
            {
                new Point2D(3.0, y),
                new Point2D(4.0, y)
            });
            soilModel2.StochasticSoilProfiles.AddRange(new[]
            {
                new StochasticSoilProfile(1.0, SoilProfileType.SoilProfile1D, 2)
                {
                    SoilProfile = soilProfile2
                }
            });
            var availableSoilModels = new[]
            {
                soilModel1,
                soilModel2
            };

            var surfaceLine = new RingtoetsPipingSurfaceLine();
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(5.0, y, 0.0),
                new Point3D(2.5, y, 1.0),
                new Point3D(0.0, y, 0.0)
            });

            var surfaceLines = new[]
            {
                surfaceLine
            };

            // Call
            IEnumerable<IPipingCalculationItem> result = PipingCalculationConfigurationHelper.GenerateCalculationsStructure(surfaceLines, availableSoilModels, new GeneralPipingInput(), new SemiProbabilisticPipingInput()).ToList();

            // Assert
            Assert.AreEqual(1, result.Count());
            var calculationGroup = result.First() as PipingCalculationGroup;
            Assert.NotNull(calculationGroup);

            Assert.AreEqual(2, calculationGroup.Children.Count);
            CollectionAssert.AllItemsAreInstancesOfType(calculationGroup.Children, typeof(PipingCalculation));

            var calculationInput1 = ((PipingCalculation) calculationGroup.Children[0]).InputParameters;
            Assert.AreSame(soilProfile1, calculationInput1.SoilProfile);
            Assert.AreSame(surfaceLine, calculationInput1.SurfaceLine);

            var calculationInput2 = ((PipingCalculation) calculationGroup.Children[1]).InputParameters;
            Assert.AreSame(soilProfile2, calculationInput2.SoilProfile);
            Assert.AreSame(surfaceLine, calculationInput2.SurfaceLine);
        }

        [Test]
        public void GenerateCalculationsStructure_SurfaceLinesEachIntersectingSoilModel_ReturnTwoGroupsWithProfilesFromIntersectingSoilModels()
        {
            // Setup
            var soilProfile1 = new PipingSoilProfile("Profile 1", -10.0, new[]
            {
                new PipingSoilLayer(-5.0),
                new PipingSoilLayer(-2.0),
                new PipingSoilLayer(1.0)
            }, SoilProfileType.SoilProfile1D, 1);
            var soilProfile2 = new PipingSoilProfile("Profile 2", -8.0, new[]
            {
                new PipingSoilLayer(-4.0),
                new PipingSoilLayer(0.0),
                new PipingSoilLayer(4.0)
            }, SoilProfileType.SoilProfile1D, 2);

            const double y = 1.1;
            var soilModel1 = new StochasticSoilModel(1, "A", "B");
            soilModel1.Geometry.AddRange(new[]
            {
                new Point2D(1.0, y),
                new Point2D(2.0, y)
            });
            soilModel1.StochasticSoilProfiles.AddRange(new[]
            {
                new StochasticSoilProfile(1.0, SoilProfileType.SoilProfile1D, 1)
                {
                    SoilProfile = soilProfile1
                },
                new StochasticSoilProfile(1.0, SoilProfileType.SoilProfile1D, 2)
                {
                    SoilProfile = soilProfile2
                }
            });

            var soilModel2 = new StochasticSoilModel(1, "A", "B");
            soilModel2.Geometry.AddRange(new[]
            {
                new Point2D(3.0, y),
                new Point2D(4.0, y)
            });
            soilModel2.StochasticSoilProfiles.AddRange(new[]
            {
                new StochasticSoilProfile(1.0, SoilProfileType.SoilProfile1D, 2)
                {
                    SoilProfile = soilProfile2
                }
            });
            var availableSoilModels = new[]
            {
                soilModel1,
                soilModel2
            };

            var surfaceLineName1 = "surface line 1";
            var surfaceLineName2 = "surface line 2";
            var surfaceLine1 = new RingtoetsPipingSurfaceLine
            {
                Name = surfaceLineName1
            };
            surfaceLine1.SetGeometry(new[]
            {
                new Point3D(2.5, y, 1.0),
                new Point3D(0.0, y, 0.0)
            });
            var surfaceLine2 = new RingtoetsPipingSurfaceLine
            {
                Name = surfaceLineName2
            };
            surfaceLine2.SetGeometry(new[]
            {
                new Point3D(2.5, y, 1.0),
                new Point3D(5.0, y, 0.0)
            });

            var surfaceLines = new[]
            {
                surfaceLine1,
                surfaceLine2
            };

            // Call
            IEnumerable<IPipingCalculationItem> result = PipingCalculationConfigurationHelper.GenerateCalculationsStructure(surfaceLines, availableSoilModels, new GeneralPipingInput(), new SemiProbabilisticPipingInput()).ToList();

            // Assert
            Assert.AreEqual(2, result.Count());
            var calculationGroup1 = result.First(g => g.Name == surfaceLineName1) as PipingCalculationGroup;
            Assert.NotNull(calculationGroup1);

            Assert.AreEqual(2, calculationGroup1.Children.Count);
            CollectionAssert.AllItemsAreInstancesOfType(calculationGroup1.Children, typeof(PipingCalculation));

            var calculationInput1 = ((PipingCalculation) calculationGroup1.Children[0]).InputParameters;
            Assert.AreSame(soilProfile1, calculationInput1.SoilProfile);
            Assert.AreSame(surfaceLine1, calculationInput1.SurfaceLine);

            var calculationInput2 = ((PipingCalculation) calculationGroup1.Children[1]).InputParameters;
            Assert.AreSame(soilProfile2, calculationInput2.SoilProfile);
            Assert.AreSame(surfaceLine1, calculationInput2.SurfaceLine);

            var calculationGroup2 = result.First(g => g.Name == surfaceLineName2) as PipingCalculationGroup;
            Assert.NotNull(calculationGroup2);

            Assert.AreEqual(1, calculationGroup2.Children.Count);
            CollectionAssert.AllItemsAreInstancesOfType(calculationGroup2.Children, typeof(PipingCalculation));

            var calculationInput3 = ((PipingCalculation) calculationGroup2.Children[0]).InputParameters;
            Assert.AreSame(soilProfile2, calculationInput3.SoilProfile);
            Assert.AreSame(surfaceLine2, calculationInput3.SurfaceLine);
        }

        [Test]
        public void GenerateCalculationsStructure_Always_CreateCalculationsWithSurfaceLineNameSoilProfileNameGeneralInputAndSemiProbabilisticInput()
        {
            // Setup
            var soilProfile1 = new PipingSoilProfile("Profile 1", -10.0, new[]
            {
                new PipingSoilLayer(-5.0),
                new PipingSoilLayer(-2.0),
                new PipingSoilLayer(1.0)
            }, SoilProfileType.SoilProfile1D, 1);
            var soilProfile2 = new PipingSoilProfile("Profile 2", -8.0, new[]
            {
                new PipingSoilLayer(-4.0),
                new PipingSoilLayer(0.0),
                new PipingSoilLayer(4.0)
            }, SoilProfileType.SoilProfile1D, 2);

            var soilModel = new StochasticSoilModel(1, "A", "B");
            soilModel.Geometry.AddRange(new[]
            {
                new Point2D(1.0, 0.0),
                new Point2D(5.0, 0.0)
            });
            soilModel.StochasticSoilProfiles.AddRange(new[]
            {
                new StochasticSoilProfile(0.3, SoilProfileType.SoilProfile1D, 1)
                {
                    SoilProfile = soilProfile1
                },
                new StochasticSoilProfile(0.7, SoilProfileType.SoilProfile1D, 2)
                {
                    SoilProfile = soilProfile2
                }
            });
            var availableSoilModels = new[]
            {
                soilModel
            };

            var surfaceLine = new RingtoetsPipingSurfaceLine
            {
                Name = "Surface Line"
            };
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(3.0, 5.0, 0.0),
                new Point3D(3.0, 0.0, 1.0),
                new Point3D(3.0, -5.0, 0.0)
            });

            var surfaceLines = new[]
            {
                surfaceLine
            };

            GeneralPipingInput generalInput = new GeneralPipingInput();
            SemiProbabilisticPipingInput semiProbabilisticInput = new SemiProbabilisticPipingInput();

            // Call
            IEnumerable<IPipingCalculationItem> result = PipingCalculationConfigurationHelper.GenerateCalculationsStructure(surfaceLines, availableSoilModels, generalInput, semiProbabilisticInput).ToList();

            // Assert
            var group = result.First(sl => sl.Name == surfaceLine.Name) as PipingCalculationGroup;
            Assert.NotNull(group);
            var calculationInput1 = (PipingCalculation) group.Children[0];
            var calculationInput2 = (PipingCalculation) group.Children[1];

            Assert.AreEqual(string.Format("{0} {1}", surfaceLine.Name, soilProfile1.Name), calculationInput1.Name);
            Assert.AreEqual(string.Format("{0} {1}", surfaceLine.Name, soilProfile2.Name), calculationInput2.Name);

            Assert.AreSame(semiProbabilisticInput, calculationInput1.SemiProbabilisticParameters);
            Assert.AreSame(semiProbabilisticInput, calculationInput2.SemiProbabilisticParameters);

            CompareGeneralInputToInput(generalInput, calculationInput1);
            CompareGeneralInputToInput(generalInput, calculationInput2);
        }

        [Test]
        public void GenerateCalculationsStructure_SoilProfileEqualNames_CalculationsGetUniqueName()
        {
            // Setup
            // Setup
            var soilProfile1 = new PipingSoilProfile("Profile 1", -10.0, new[]
            {
                new PipingSoilLayer(-5.0),
                new PipingSoilLayer(-2.0),
                new PipingSoilLayer(1.0)
            }, SoilProfileType.SoilProfile1D, 1);
            var soilProfile2 = new PipingSoilProfile("Profile 1", -8.0, new[]
            {
                new PipingSoilLayer(-4.0),
                new PipingSoilLayer(0.0),
                new PipingSoilLayer(4.0)
            }, SoilProfileType.SoilProfile1D, 2);
            var soilProfile3 = new PipingSoilProfile("Profile 1", -8.0, new[]
            {
                new PipingSoilLayer(-4.0),
                new PipingSoilLayer(0.0),
                new PipingSoilLayer(4.0)
            }, SoilProfileType.SoilProfile1D, 2);

            var soilModel = new StochasticSoilModel(1, "A", "B");
            soilModel.Geometry.AddRange(new[]
            {
                new Point2D(1.0, 0.0),
                new Point2D(5.0, 0.0)
            });
            soilModel.StochasticSoilProfiles.AddRange(new[]
            {
                new StochasticSoilProfile(0.3, SoilProfileType.SoilProfile1D, 1)
                {
                    SoilProfile = soilProfile1
                },
                new StochasticSoilProfile(0.2, SoilProfileType.SoilProfile1D, 2)
                {
                    SoilProfile = soilProfile2
                },
                new StochasticSoilProfile(0.5, SoilProfileType.SoilProfile1D, 3)
                {
                    SoilProfile = soilProfile3
                }
            });
            var availableSoilModels = new[]
            {
                soilModel
            };

            var surfaceLine = new RingtoetsPipingSurfaceLine
            {
                Name = "Surface Line"
            };
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(3.0, 5.0, 0.0),
                new Point3D(3.0, 0.0, 1.0),
                new Point3D(3.0, -5.0, 0.0)
            });

            var surfaceLines = new[]
            {
                surfaceLine
            };

            // Call
            IEnumerable<IPipingCalculationItem> result = PipingCalculationConfigurationHelper.GenerateCalculationsStructure(surfaceLines, availableSoilModels, new GeneralPipingInput(), new SemiProbabilisticPipingInput()).ToList();

            // Assert
            var group = result.First(sl => sl.Name == surfaceLine.Name) as PipingCalculationGroup;
            Assert.NotNull(group);
            var calculationInput1 = (PipingCalculation) group.Children[0];
            var calculationInput2 = (PipingCalculation) group.Children[1];
            var calculationInput3 = (PipingCalculation) group.Children[2];

            Assert.AreEqual(string.Format("{0} {1}", surfaceLine.Name, soilProfile1.Name), calculationInput1.Name);
            Assert.AreEqual(string.Format("{0} {1} (1)", surfaceLine.Name, soilProfile2.Name), calculationInput2.Name);
            Assert.AreEqual(string.Format("{0} {1} (2)", surfaceLine.Name, soilProfile3.Name), calculationInput3.Name);
        }

        #endregion

        private static void CompareGeneralInputToInput(GeneralPipingInput generalInput, PipingCalculation calculationInput)
        {
            Assert.AreEqual(generalInput.BeddingAngle, calculationInput.InputParameters.BeddingAngle);
            Assert.AreEqual(generalInput.CriticalHeaveGradient, calculationInput.InputParameters.CriticalHeaveGradient);
            Assert.AreEqual(generalInput.Gravity, calculationInput.InputParameters.Gravity);
            Assert.AreEqual(generalInput.MeanDiameter70, calculationInput.InputParameters.MeanDiameter70);
            Assert.AreEqual(generalInput.SandParticlesVolumicWeight, calculationInput.InputParameters.SandParticlesVolumicWeight);
            Assert.AreEqual(generalInput.SellmeijerModelFactor, calculationInput.InputParameters.SellmeijerModelFactor);
            Assert.AreEqual(generalInput.SellmeijerReductionFactor, calculationInput.InputParameters.SellmeijerReductionFactor);
            Assert.AreEqual(generalInput.UpliftModelFactor, calculationInput.InputParameters.UpliftModelFactor);
            Assert.AreEqual(generalInput.WaterKinematicViscosity, calculationInput.InputParameters.WaterKinematicViscosity);
            Assert.AreEqual(generalInput.WaterVolumetricWeight, calculationInput.InputParameters.WaterVolumetricWeight);
            Assert.AreEqual(generalInput.WhitesDragCoefficient, calculationInput.InputParameters.WhitesDragCoefficient);
        }
    }
}