using System.Collections.Generic;
using System.Linq;

using Core.Common.Base.Geometry;

using NUnit.Framework;

using Ringtoets.Piping.Data;
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.Forms.Test
{
    [TestFixture]
    public class PipingCalculationConfigurationHelperTest
    {
        [Test]
        public void GetPipingSoilProfilesForSurfaceLine_SurfaceLineIntersectingSoilModel_ReturnSoilProfilesOfSoilModel()
        {
            // Setup
            var soilProfile1 = new PipingSoilProfile("Profile 1", -10.0, new []
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
            soilModel.Geometry.AddRange(new[]{ new Point2D(1.0, 0.0), new Point2D(5.0, 0.0) });
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
            PipingSoilProfile[] expected = { soilProfile1, soilProfile2 };
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

            // Call
            IEnumerable<PipingSoilProfile> result = PipingCalculationConfigurationHelper.GetPipingSoilProfilesForSurfaceLine(surfaceLine, availableSoilModels);

            // Assert
            PipingSoilProfile[] expected = { soilProfile1, soilProfile2 };
            CollectionAssert.AreEquivalent(expected, result);
        }
    }
}