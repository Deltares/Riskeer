using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.Plugin.Test
{
    [TestFixture]
    public class PipingCalculationGeneratorTest
    {
        [Test]
        public void Generate_WithoutSurfaceLines_ReturnsEmptyCollection()
        {
            // Call
            var result = PipingCalculationGenerator.Generate(null, null).ToList();

            // Assert
            Assert.AreEqual(0, result.Count);
        }

        [Test]
        public void Generate_WithEmptySurfaceLines_ReturnsEmptyCollection()
        {
            // Call
            var result = PipingCalculationGenerator.Generate(Enumerable.Empty<RingtoetsPipingSurfaceLine>(), null).ToList();

            // Assert
            Assert.AreEqual(0, result.Count);
        }

        [Test]
        public void Generate_WithSurfaceLinesWithoutSoilModels_ReturnsFourEmptyGroups()
        {
            // Setup
            var testName1 = "group1";
            var testName2 = "group2";
            var testName3 = "group3";
            var testName4 = "group4";

            var ringtoetsPipingSurfaceLines = new List<RingtoetsPipingSurfaceLine>
            {
                new RingtoetsPipingSurfaceLine {Name = testName1},
                new RingtoetsPipingSurfaceLine {Name = testName2},
                new RingtoetsPipingSurfaceLine {Name = testName3},
                new RingtoetsPipingSurfaceLine {Name = testName4}
            };

            // Call
            var result = PipingCalculationGenerator.Generate(ringtoetsPipingSurfaceLines, null).ToList();

            // Assert
            Assert.AreEqual(4, result.Count);
            CollectionAssert.AllItemsAreInstancesOfType(result, typeof(PipingCalculationGroup));
            Assert.AreEqual(new[] { testName1, testName2, testName3, testName4 }, result.Select(g => g.Name));
            Assert.AreEqual(new [] {0,0,0,0}, result.Select(g => ((PipingCalculationGroup)g).Children.Count));
        }

        [Test]
        public void Generate_WithSurfaceLinesWithEmptySoilModels_ReturnsFourEmptyGroups()
        {
            // Setup
            var testName1 = "group1";
            var testName2 = "group2";
            var testName3 = "group3";
            var testName4 = "group4";

            var ringtoetsPipingSurfaceLines = new List<RingtoetsPipingSurfaceLine>
            {
                new RingtoetsPipingSurfaceLine {Name = testName1},
                new RingtoetsPipingSurfaceLine {Name = testName2},
                new RingtoetsPipingSurfaceLine {Name = testName3},
                new RingtoetsPipingSurfaceLine {Name = testName4}
            };

            // Call
            var result = PipingCalculationGenerator.Generate(ringtoetsPipingSurfaceLines, Enumerable.Empty<StochasticSoilModel>()).ToList();

            // Assert
            Assert.AreEqual(4, result.Count);
            CollectionAssert.AllItemsAreInstancesOfType(result, typeof(PipingCalculationGroup));
            Assert.AreEqual(new[] { testName1, testName2, testName3, testName4 }, result.Select(g => g.Name));
            Assert.AreEqual(new [] {0,0,0,0}, result.Select(g => ((PipingCalculationGroup)g).Children.Count));
        }

        [Test]
        public void Generate_SurfaceLineIntersectingSoilModel_ReturnOneGroupWithTwoCalculations()
        {
            // Setup
            var soilProfile1 = new PipingSoilProfile("Profile 1", -10.0, new[]
            {
                new PipingSoilLayer(-5.0), 
                new PipingSoilLayer(-2.0), 
                new PipingSoilLayer(1.0)
            }, 1);
            var soilProfile2 = new PipingSoilProfile("Profile 2", -8.0, new[]
            {
                new PipingSoilLayer(-4.0), 
                new PipingSoilLayer(0.0), 
                new PipingSoilLayer(4.0)
            }, 2);

            var soilModel = new StochasticSoilModel(1, "A", "B");
            soilModel.Geometry.AddRange(new[] { new Point2D(1.0, 0.0), new Point2D(5.0, 0.0) });
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

            var surfaceLines = new[] { surfaceLine };

            // Call
            IEnumerable<IPipingCalculationItem> result = PipingCalculationGenerator.Generate(surfaceLines, availableSoilModels).ToList();

            // Assert
            Assert.AreEqual(1, result.Count());
            var calculationGroup = result.First() as PipingCalculationGroup;
            Assert.NotNull(calculationGroup);

            Assert.AreEqual(2, calculationGroup.Children.Count);
            CollectionAssert.AllItemsAreInstancesOfType(calculationGroup.Children, typeof(PipingCalculation));

            var calculationInput1 = ((PipingCalculation)calculationGroup.Children[0]).InputParameters;
            Assert.AreSame(soilProfile1, calculationInput1.SoilProfile);
            Assert.AreSame(surfaceLine, calculationInput1.SurfaceLine);

            var calculationInput2 = ((PipingCalculation)calculationGroup.Children[1]).InputParameters;
            Assert.AreSame(soilProfile2, calculationInput2.SoilProfile);
            Assert.AreSame(surfaceLine, calculationInput2.SurfaceLine);
        }

        [Test]
        public void Generate_NoSoilProfiles_ReturnOneEmptyGroup()
        {
            // Setup
            var soilModel = new StochasticSoilModel(1, "A", "B");
            soilModel.Geometry.AddRange(new[] { new Point2D(1.0, 0.0), new Point2D(5.0, 0.0) });

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

            var surfaceLines = new[] { surfaceLine };

            // Call
            IEnumerable<IPipingCalculationItem> result = PipingCalculationGenerator.Generate(surfaceLines, availableSoilModels).ToList();

            // Assert
            Assert.AreEqual(1, result.Count());
            var calculationGroup = result.First() as PipingCalculationGroup;
            Assert.NotNull(calculationGroup);

            Assert.IsEmpty(calculationGroup.Children);
        }

        [Test]
        public void Generate_SoilModelGeometryNotIntersecting_ReturnOneEmptyGroup()
        {
            // Setup
            var soilProfile1 = new PipingSoilProfile("Profile 1", -10.0, new[]
            {
                new PipingSoilLayer(-5.0), 
                new PipingSoilLayer(-2.0), 
                new PipingSoilLayer(1.0)
            }, 1);
            var soilProfile2 = new PipingSoilProfile("Profile 2", -8.0, new[]
            {
                new PipingSoilLayer(-4.0), 
                new PipingSoilLayer(0.0), 
                new PipingSoilLayer(4.0)
            }, 2);

            var soilModel = new StochasticSoilModel(1, "A", "B");
            soilModel.Geometry.AddRange(new[] { new Point2D(1.0, 0.0), new Point2D(5.0, 0.0) });
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

            var surfaceLines = new[] { surfaceLine };

            // Call
            IEnumerable<IPipingCalculationItem> result = PipingCalculationGenerator.Generate(surfaceLines, availableSoilModels).ToList();

            // Assert
            Assert.AreEqual(1, result.Count());
            var calculationGroup = result.First() as PipingCalculationGroup;
            Assert.NotNull(calculationGroup);

            Assert.IsEmpty(calculationGroup.Children);
        }

        [Test]
        public void Generate_SurfaceLineOverlappingSoilModel_ReturnOneGroupWithProfilesFromBothSoilModels()
        {
            // Setup
            var soilProfile1 = new PipingSoilProfile("Profile 1", -10.0, new[]
            {
                new PipingSoilLayer(-5.0), 
                new PipingSoilLayer(-2.0), 
                new PipingSoilLayer(1.0)
            }, 1);
            var soilProfile2 = new PipingSoilProfile("Profile 2", -8.0, new[]
            {
                new PipingSoilLayer(-4.0), 
                new PipingSoilLayer(0.0), 
                new PipingSoilLayer(4.0)
            }, 2);

            const double y = 1.1;
            var soilModel1 = new StochasticSoilModel(1, "A", "B");
            soilModel1.Geometry.AddRange(new[] { new Point2D(1.0, y), new Point2D(2.0, y) });
            soilModel1.StochasticSoilProfiles.AddRange(new[]
            {
                new StochasticSoilProfile(1.0, SoilProfileType.SoilProfile1D, 1)
                {
                    SoilProfile = soilProfile1
                },
            });

            var soilModel2 = new StochasticSoilModel(1, "A", "B");
            soilModel2.Geometry.AddRange(new[] { new Point2D(3.0, y), new Point2D(4.0, y) });
            soilModel2.StochasticSoilProfiles.AddRange(new[]
            {
                new StochasticSoilProfile(1.0, SoilProfileType.SoilProfile1D, 2)
                {
                    SoilProfile = soilProfile2
                }
            });
            var availableSoilModels = new[]
            {
                soilModel1, soilModel2
            };

            var surfaceLine = new RingtoetsPipingSurfaceLine();
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(5.0, y, 0.0), 
                new Point3D(2.5, y, 1.0), 
                new Point3D(0.0, y, 0.0)
            });

            var surfaceLines = new[] { surfaceLine };

            // Call
            IEnumerable<IPipingCalculationItem> result = PipingCalculationGenerator.Generate(surfaceLines, availableSoilModels).ToList();

            // Assert
            Assert.AreEqual(1, result.Count());
            var calculationGroup = result.First() as PipingCalculationGroup;
            Assert.NotNull(calculationGroup);

            Assert.AreEqual(2, calculationGroup.Children.Count);
            CollectionAssert.AllItemsAreInstancesOfType(calculationGroup.Children, typeof(PipingCalculation));

            var calculationInput1 = ((PipingCalculation)calculationGroup.Children[0]).InputParameters;
            Assert.AreSame(soilProfile1, calculationInput1.SoilProfile);
            Assert.AreSame(surfaceLine, calculationInput1.SurfaceLine);

            var calculationInput2 = ((PipingCalculation)calculationGroup.Children[1]).InputParameters;
            Assert.AreSame(soilProfile2, calculationInput2.SoilProfile);
            Assert.AreSame(surfaceLine, calculationInput2.SurfaceLine);
        }

        [Test]
        public void Generate_SurfaceLinesEachIntersectingSoilModel_ReturnTwoGroupsWithProfilesFromIntersectingSoilModels()
        {
            // Setup
            var soilProfile1 = new PipingSoilProfile("Profile 1", -10.0, new[]
            {
                new PipingSoilLayer(-5.0), 
                new PipingSoilLayer(-2.0), 
                new PipingSoilLayer(1.0)
            }, 1);
            var soilProfile2 = new PipingSoilProfile("Profile 2", -8.0, new[]
            {
                new PipingSoilLayer(-4.0), 
                new PipingSoilLayer(0.0), 
                new PipingSoilLayer(4.0)
            }, 2);

            const double y = 1.1;
            var soilModel1 = new StochasticSoilModel(1, "A", "B");
            soilModel1.Geometry.AddRange(new[] { new Point2D(1.0, y), new Point2D(2.0, y) });
            soilModel1.StochasticSoilProfiles.AddRange(new[]
            {
                new StochasticSoilProfile(1.0, SoilProfileType.SoilProfile1D, 1)
                {
                    SoilProfile = soilProfile1
                },
                new StochasticSoilProfile(1.0, SoilProfileType.SoilProfile1D, 2)
                {
                    SoilProfile = soilProfile2
                },
            });

            var soilModel2 = new StochasticSoilModel(1, "A", "B");
            soilModel2.Geometry.AddRange(new[] { new Point2D(3.0, y), new Point2D(4.0, y) });
            soilModel2.StochasticSoilProfiles.AddRange(new[]
            {
                new StochasticSoilProfile(1.0, SoilProfileType.SoilProfile1D, 2)
                {
                    SoilProfile = soilProfile2
                }
            });
            var availableSoilModels = new[]
            {
                soilModel1, soilModel2
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

            var surfaceLines = new[] { surfaceLine1, surfaceLine2 };

            // Call
            IEnumerable<IPipingCalculationItem> result = PipingCalculationGenerator.Generate(surfaceLines, availableSoilModels).ToList();

            // Assert
            Assert.AreEqual(2, result.Count());
            var calculationGroup1 = result.First(g => g.Name == surfaceLineName1) as PipingCalculationGroup;
            Assert.NotNull(calculationGroup1);

            Assert.AreEqual(2, calculationGroup1.Children.Count);
            CollectionAssert.AllItemsAreInstancesOfType(calculationGroup1.Children, typeof(PipingCalculation));

            var calculationInput1 = ((PipingCalculation)calculationGroup1.Children[0]).InputParameters;
            Assert.AreSame(soilProfile1, calculationInput1.SoilProfile);
            Assert.AreSame(surfaceLine1, calculationInput1.SurfaceLine);

            var calculationInput2 = ((PipingCalculation)calculationGroup1.Children[1]).InputParameters;
            Assert.AreSame(soilProfile2, calculationInput2.SoilProfile);
            Assert.AreSame(surfaceLine1, calculationInput2.SurfaceLine);

            var calculationGroup2 = result.First(g => g.Name == surfaceLineName2) as PipingCalculationGroup;
            Assert.NotNull(calculationGroup2);

            Assert.AreEqual(1, calculationGroup2.Children.Count);
            CollectionAssert.AllItemsAreInstancesOfType(calculationGroup2.Children, typeof(PipingCalculation));

            var calculationInput3 = ((PipingCalculation)calculationGroup2.Children[0]).InputParameters;
            Assert.AreSame(soilProfile2, calculationInput3.SoilProfile);
            Assert.AreSame(surfaceLine2, calculationInput3.SurfaceLine);
        }
    }
}