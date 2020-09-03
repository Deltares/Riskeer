// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using System.Linq;
using Core.Common.Base.Geometry;
using Deltares.MacroStability.CSharpWrapper.Input;
using NUnit.Framework;
using Riskeer.MacroStabilityInwards.KernelWrapper.Creators.Input;
using Riskeer.MacroStabilityInwards.KernelWrapper.TestUtil.Kernels.UpliftVan.Input;
using CSharpWrapperPoint2D = Deltares.MacroStability.CSharpWrapper.Point2D;

namespace Riskeer.MacroStabilityInwards.KernelWrapper.Test.Creators.Input
{
    [TestFixture]
    public class SoilProfileCreatorTest
    {
        [Test]
        public void Create_LayersWithSoilNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => SoilProfileCreator.Create(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("layersWithSoil", exception.ParamName);
        }

        /// <remarks>
        /// The soil profile used in this tests contains two outer layers (outer rings) and two neighbouring holes (inner rings):
        ///                                    
        ///  11    # # # # # # # # # # #       
        ///  10    #                   #       
        ///   9    #   # # # # # # #   #       
        ///   8    #   #           #   #       
        ///   7    #   # # # # # # #   #       
        ///   6    #   #           #   #       
        ///   5    #   # # # # # # #   #       
        ///   4    #                   #       
        ///   3    # # # # # # # # # # #       
        ///   2    #                   #       
        ///   1    #                   #       
        ///   0    # # # # # # # # # # #       
        ///                                    
        ///        0 1 2 3 4 5 6 7 8 9 10      
        /// </remarks>
        [Test]
        public void Create_WithNeighbouringInnerLoops_ReturnSoilProfile2D()
        {
            // Setup
            var layer1Points = new[]
            {
                new Point2D(0, 0),
                new Point2D(0, 3),
                new Point2D(10, 3),
                new Point2D(10, 0)
            };

            var layer2Points = new[]
            {
                new Point2D(0, 3),
                new Point2D(0, 11),
                new Point2D(10, 11),
                new Point2D(10, 3)
            };

            var layer2Hole1Points = new[]
            {
                new Point2D(2, 5),
                new Point2D(2, 7),
                new Point2D(8, 7),
                new Point2D(8, 5)
            };

            var layer2Hole2Points = new[]
            {
                new Point2D(2, 7),
                new Point2D(2, 9),
                new Point2D(8, 9),
                new Point2D(8, 7)
            };

            var soil1 = new Soil
            {
                Name = "Clay"
            };

            var soil2 = new Soil
            {
                Name = "Sand"
            };

            var soil3 = new Soil
            {
                Name = "Nested clay"
            };

            var soil4 = new Soil
            {
                Name = "Nested sand"
            };

            var layerWithSoil1 = new LayerWithSoil(
                layer1Points,
                new Point2D[0][],
                soil1,
                false,
                WaterPressureInterpolationModel.Automatic);

            var layerWithSoil2 = new LayerWithSoil(
                layer2Points,
                new[]
                {
                    layer2Hole1Points,
                    layer2Hole2Points
                },
                soil2,
                true,
                WaterPressureInterpolationModel.Hydrostatic);

            var layerWithSoil3 = new LayerWithSoil(
                layer2Hole1Points,
                new Point2D[0][],
                soil3,
                false,
                WaterPressureInterpolationModel.Automatic);

            var layerWithSoil4 = new LayerWithSoil(
                layer2Hole2Points,
                new Point2D[0][],
                soil4,
                true,
                WaterPressureInterpolationModel.Hydrostatic);

            // Call
            SoilProfile profile = SoilProfileCreator.Create(new[]
            {
                layerWithSoil1,
                layerWithSoil2,
                layerWithSoil3,
                layerWithSoil4
            });

            // Assert

            #region Geometry

            var outerLoopPoint1 = new CSharpWrapperPoint2D(0, 0);
            var outerLoopPoint2 = new CSharpWrapperPoint2D(0, 3);
            var outerLoopPoint3 = new CSharpWrapperPoint2D(10, 3);
            var outerLoopPoint4 = new CSharpWrapperPoint2D(10, 0);
            var outerLoopPoint5 = new CSharpWrapperPoint2D(0, 11);
            var outerLoopPoint6 = new CSharpWrapperPoint2D(10, 11);
            Curve outerLoopCurve1 = CreateCurve(outerLoopPoint1, outerLoopPoint2);
            Curve outerLoopCurve2 = CreateCurve(outerLoopPoint2, outerLoopPoint3);
            Curve outerLoopCurve3 = CreateCurve(outerLoopPoint3, outerLoopPoint4);
            Curve outerLoopCurve4 = CreateCurve(outerLoopPoint4, outerLoopPoint1);
            Curve outerLoopCurve5 = CreateCurve(outerLoopPoint2, outerLoopPoint5);
            Curve outerLoopCurve6 = CreateCurve(outerLoopPoint5, outerLoopPoint6);
            Curve outerLoopCurve7 = CreateCurve(outerLoopPoint6, outerLoopPoint3);
            var outerLoop1 = new Loop
            {
                Curves =
                {
                    outerLoopCurve1,
                    outerLoopCurve2,
                    outerLoopCurve3,
                    outerLoopCurve4
                }
            };
            var outerLoop2 = new Loop
            {
                Curves =
                {
                    outerLoopCurve5,
                    outerLoopCurve6,
                    outerLoopCurve7,
                    outerLoopCurve2
                }
            };

            var innerLoopPoint1 = new CSharpWrapperPoint2D(2, 5);
            var innerLoopPoint2 = new CSharpWrapperPoint2D(2, 7);
            var innerLoopPoint3 = new CSharpWrapperPoint2D(8, 7);
            var innerLoopPoint4 = new CSharpWrapperPoint2D(8, 5);
            var innerLoopPoint5 = new CSharpWrapperPoint2D(2, 9);
            var innerLoopPoint6 = new CSharpWrapperPoint2D(8, 9);
            Curve innerLoopCurve1 = CreateCurve(innerLoopPoint1, innerLoopPoint2);
            Curve innerLoopCurve2 = CreateCurve(innerLoopPoint2, innerLoopPoint3);
            Curve innerLoopCurve3 = CreateCurve(innerLoopPoint3, innerLoopPoint4);
            Curve innerLoopCurve4 = CreateCurve(innerLoopPoint4, innerLoopPoint1);
            Curve innerLoopCurve5 = CreateCurve(innerLoopPoint2, innerLoopPoint5);
            Curve innerLoopCurve6 = CreateCurve(innerLoopPoint5, innerLoopPoint6);
            Curve innerLoopCurve7 = CreateCurve(innerLoopPoint6, innerLoopPoint3);
            var innerLoop1 = new Loop
            {
                Curves =
                {
                    innerLoopCurve1,
                    innerLoopCurve2,
                    innerLoopCurve3,
                    innerLoopCurve4
                }
            };
            var innerLoop2 = new Loop
            {
                Curves =
                {
                    innerLoopCurve5,
                    innerLoopCurve6,
                    innerLoopCurve7,
                    innerLoopCurve2
                }
            };

            CollectionAssert.AreEqual(new[]
            {
                outerLoopPoint1,
                outerLoopPoint2,
                outerLoopPoint3,
                outerLoopPoint4,
                outerLoopPoint5,
                outerLoopPoint6,
                innerLoopPoint1,
                innerLoopPoint2,
                innerLoopPoint3,
                innerLoopPoint4,
                innerLoopPoint5,
                innerLoopPoint6
            }, profile.Geometry.Points, new StabilityPointComparer());

            CollectionAssert.AreEqual(new[]
            {
                outerLoopCurve1,
                outerLoopCurve2,
                outerLoopCurve3,
                outerLoopCurve4,
                outerLoopCurve5,
                outerLoopCurve6,
                outerLoopCurve7,
                innerLoopCurve1,
                innerLoopCurve2,
                innerLoopCurve3,
                innerLoopCurve4,
                innerLoopCurve5,
                innerLoopCurve6,
                innerLoopCurve7
            }, profile.Geometry.Curves, new GeometryCurveComparer());

            CollectionAssert.AreEqual(new[]
            {
                outerLoop1,
                outerLoop2,
                innerLoop1,
                innerLoop2
            }, profile.Geometry.Loops, new GeometryLoopComparer());

            Assert.AreEqual(0, profile.Geometry.Left);
            Assert.AreEqual(0, profile.Geometry.Bottom);
            Assert.AreEqual(10, profile.Geometry.Right);

            Assert.AreEqual(4, profile.SoilSurfaces.Count);
            Assert.AreEqual(4, profile.Geometry.Surfaces.Count);
            CollectionAssert.AreEqual(profile.SoilSurfaces.ToList()
                                             .Select(s => s.Surface),
                                      profile.Geometry.Surfaces);

            #endregion

            #region Surfaces

            SoilProfileSurface surface1 = profile.SoilSurfaces.ElementAt(0);
            Assert.AreSame(soil1, surface1.Soil);
            Assert.AreEqual(soil1.Name, surface1.Name);
            Assert.AreEqual(layerWithSoil1.IsAquifer, surface1.IsAquifer);
            Assert.AreEqual(layerWithSoil1.WaterPressureInterpolationModel, surface1.WaterPressureInterpolationModel);
            Assert.AreSame(profile.Geometry.Loops.ElementAt(0), surface1.Surface.OuterLoop);
            CollectionAssert.IsEmpty(surface1.Surface.InnerLoops);

            SoilProfileSurface surface2 = profile.SoilSurfaces.ElementAt(1);
            Assert.AreSame(soil2, surface2.Soil);
            Assert.AreEqual(soil2.Name, surface2.Name);
            Assert.AreEqual(layerWithSoil2.IsAquifer, surface2.IsAquifer);
            Assert.AreEqual(layerWithSoil2.WaterPressureInterpolationModel, surface2.WaterPressureInterpolationModel);
            Assert.AreSame(profile.Geometry.Loops.ElementAt(1), surface2.Surface.OuterLoop);
            CollectionAssert.AreEqual(new[]
            {
                profile.Geometry.Loops.ElementAt(2),
                profile.Geometry.Loops.ElementAt(3)
            }, surface2.Surface.InnerLoops);

            SoilProfileSurface surface3 = profile.SoilSurfaces.ElementAt(2);
            Assert.AreSame(soil3, surface3.Soil);
            Assert.AreEqual(soil3.Name, surface3.Name);
            Assert.AreEqual(layerWithSoil3.IsAquifer, surface3.IsAquifer);
            Assert.AreEqual(layerWithSoil3.WaterPressureInterpolationModel, surface3.WaterPressureInterpolationModel);
            Assert.AreSame(profile.Geometry.Loops.ElementAt(2), surface3.Surface.OuterLoop);
            CollectionAssert.IsEmpty(surface3.Surface.InnerLoops);

            SoilProfileSurface surface4 = profile.SoilSurfaces.ElementAt(3);
            Assert.AreSame(soil4, surface4.Soil);
            Assert.AreEqual(soil4.Name, surface4.Name);
            Assert.AreEqual(layerWithSoil4.IsAquifer, surface4.IsAquifer);
            Assert.AreEqual(layerWithSoil4.WaterPressureInterpolationModel, surface4.WaterPressureInterpolationModel);
            Assert.AreSame(profile.Geometry.Loops.ElementAt(3), surface4.Surface.OuterLoop);
            CollectionAssert.IsEmpty(surface4.Surface.InnerLoops);

            #endregion
        }

        /// <remarks>
        /// The soil profile used in this tests contains two outer layers (outer rings) and two nested holes (inner rings):
        ///                                    
        ///  13    # # # # # # # # # # #       
        ///  12    #                   #       
        ///  11    #   # # # # # # #   #       
        ///  10    #   #           #   #       
        ///   9    #   #   # # #   #   #       
        ///   8    #   #   #   #   #   #       
        ///   7    #   #   # # #   #   #       
        ///   6    #   #           #   #       
        ///   5    #   # # # # # # #   #       
        ///   4    #                   #       
        ///   3    # # # # # # # # # # #       
        ///   2    #                   #       
        ///   1    #                   #       
        ///   0    # # # # # # # # # # #       
        ///                                    
        ///        0 1 2 3 4 5 6 7 8 9 10      
        /// </remarks>
        [Test]
        public void Create_WithNestedInnerLoops_ReturnSoilProfile2D()
        {
            // Setup
            var layer1Points = new[]
            {
                new Point2D(0, 0),
                new Point2D(0, 3),
                new Point2D(10, 3),
                new Point2D(10, 0)
            };

            var layer2Points = new[]
            {
                new Point2D(0, 3),
                new Point2D(0, 13),
                new Point2D(10, 13),
                new Point2D(10, 3)
            };

            var layer2Hole1Points = new[]
            {
                new Point2D(2, 5),
                new Point2D(2, 11),
                new Point2D(8, 11),
                new Point2D(8, 5)
            };

            var layer2Hole2Points = new[]
            {
                new Point2D(4, 7),
                new Point2D(4, 9),
                new Point2D(6, 9),
                new Point2D(6, 7)
            };

            var soil1 = new Soil
            {
                Name = "Clay"
            };

            var soil2 = new Soil
            {
                Name = "Sand"
            };

            var soil3 = new Soil
            {
                Name = "Nested clay"
            };

            var soil4 = new Soil
            {
                Name = "Nested sand"
            };

            var layerWithSoil1 = new LayerWithSoil(
                layer1Points,
                new Point2D[0][],
                soil1,
                false,
                WaterPressureInterpolationModel.Automatic);

            var layerWithSoil2 = new LayerWithSoil(
                layer2Points,
                new[]
                {
                    layer2Hole1Points
                },
                soil2,
                true,
                WaterPressureInterpolationModel.Hydrostatic);

            var layerWithSoil3 = new LayerWithSoil(
                layer2Hole1Points,
                new[]
                {
                    layer2Hole2Points
                },
                soil3,
                false,
                WaterPressureInterpolationModel.Automatic);

            var layerWithSoil4 = new LayerWithSoil(
                layer2Hole2Points,
                new Point2D[0][],
                soil4,
                true,
                WaterPressureInterpolationModel.Hydrostatic);

            // Call
            SoilProfile profile = SoilProfileCreator.Create(new[]
            {
                layerWithSoil1,
                layerWithSoil2,
                layerWithSoil3,
                layerWithSoil4
            });

            // Assert

            #region Geometry

            var outerLoopPoint1 = new CSharpWrapperPoint2D(0, 0);
            var outerLoopPoint2 = new CSharpWrapperPoint2D(0, 3);
            var outerLoopPoint3 = new CSharpWrapperPoint2D(10, 3);
            var outerLoopPoint4 = new CSharpWrapperPoint2D(10, 0);
            var outerLoopPoint5 = new CSharpWrapperPoint2D(0, 13);
            var outerLoopPoint6 = new CSharpWrapperPoint2D(10, 13);
            Curve outerLoopCurve1 = CreateCurve(outerLoopPoint1, outerLoopPoint2);
            Curve outerLoopCurve2 = CreateCurve(outerLoopPoint2, outerLoopPoint3);
            Curve outerLoopCurve3 = CreateCurve(outerLoopPoint3, outerLoopPoint4);
            Curve outerLoopCurve4 = CreateCurve(outerLoopPoint4, outerLoopPoint1);
            Curve outerLoopCurve5 = CreateCurve(outerLoopPoint2, outerLoopPoint5);
            Curve outerLoopCurve6 = CreateCurve(outerLoopPoint5, outerLoopPoint6);
            Curve outerLoopCurve7 = CreateCurve(outerLoopPoint6, outerLoopPoint3);
            var outerLoop1 = new Loop
            {
                Curves =
                {
                    outerLoopCurve1,
                    outerLoopCurve2,
                    outerLoopCurve3,
                    outerLoopCurve4
                }
            };
            var outerLoop2 = new Loop
            {
                Curves =
                {
                    outerLoopCurve5,
                    outerLoopCurve6,
                    outerLoopCurve7,
                    outerLoopCurve2
                }
            };

            var innerLoopPoint1 = new CSharpWrapperPoint2D(2, 5);
            var innerLoopPoint2 = new CSharpWrapperPoint2D(2, 11);
            var innerLoopPoint3 = new CSharpWrapperPoint2D(8, 11);
            var innerLoopPoint4 = new CSharpWrapperPoint2D(8, 5);
            var innerLoopPoint5 = new CSharpWrapperPoint2D(4, 7);
            var innerLoopPoint6 = new CSharpWrapperPoint2D(4, 9);
            var innerLoopPoint7 = new CSharpWrapperPoint2D(6, 9);
            var innerLoopPoint8 = new CSharpWrapperPoint2D(6, 7);
            Curve innerLoopCurve1 = CreateCurve(innerLoopPoint1, innerLoopPoint2);
            Curve innerLoopCurve2 = CreateCurve(innerLoopPoint2, innerLoopPoint3);
            Curve innerLoopCurve3 = CreateCurve(innerLoopPoint3, innerLoopPoint4);
            Curve innerLoopCurve4 = CreateCurve(innerLoopPoint4, innerLoopPoint1);
            Curve innerLoopCurve5 = CreateCurve(innerLoopPoint5, innerLoopPoint6);
            Curve innerLoopCurve6 = CreateCurve(innerLoopPoint6, innerLoopPoint7);
            Curve innerLoopCurve7 = CreateCurve(innerLoopPoint7, innerLoopPoint8);
            Curve innerLoopCurve8 = CreateCurve(innerLoopPoint8, innerLoopPoint5);

            var innerLoop1 = new Loop
            {
                Curves =
                {
                    innerLoopCurve1,
                    innerLoopCurve2,
                    innerLoopCurve3,
                    innerLoopCurve4
                }
            };
            var innerLoop2 = new Loop
            {
                Curves =
                {
                    innerLoopCurve5,
                    innerLoopCurve6,
                    innerLoopCurve7,
                    innerLoopCurve8
                }
            };

            CollectionAssert.AreEqual(new[]
            {
                outerLoopPoint1,
                outerLoopPoint2,
                outerLoopPoint3,
                outerLoopPoint4,
                outerLoopPoint5,
                outerLoopPoint6,
                innerLoopPoint1,
                innerLoopPoint2,
                innerLoopPoint3,
                innerLoopPoint4,
                innerLoopPoint5,
                innerLoopPoint6,
                innerLoopPoint7,
                innerLoopPoint8
            }, profile.Geometry.Points, new StabilityPointComparer());

            CollectionAssert.AreEqual(new[]
            {
                outerLoopCurve1,
                outerLoopCurve2,
                outerLoopCurve3,
                outerLoopCurve4,
                outerLoopCurve5,
                outerLoopCurve6,
                outerLoopCurve7,
                innerLoopCurve1,
                innerLoopCurve2,
                innerLoopCurve3,
                innerLoopCurve4,
                innerLoopCurve5,
                innerLoopCurve6,
                innerLoopCurve7,
                innerLoopCurve8
            }, profile.Geometry.Curves, new GeometryCurveComparer());

            CollectionAssert.AreEqual(new[]
            {
                outerLoop1,
                outerLoop2,
                innerLoop1,
                innerLoop2
            }, profile.Geometry.Loops, new GeometryLoopComparer());

            Assert.AreEqual(0, profile.Geometry.Left);
            Assert.AreEqual(0, profile.Geometry.Bottom);
            Assert.AreEqual(10, profile.Geometry.Right);

            Assert.AreEqual(4, profile.SoilSurfaces.Count);
            Assert.AreEqual(4, profile.Geometry.Surfaces.Count);
            CollectionAssert.AreEqual(profile.SoilSurfaces.ToList()
                                             .Select(s => s.Surface),
                                      profile.Geometry.Surfaces);

            #endregion

            #region Surfaces

            SoilProfileSurface surface1 = profile.SoilSurfaces.ElementAt(0);
            Assert.AreSame(soil1, surface1.Soil);
            Assert.AreEqual(soil1.Name, surface1.Name);
            Assert.AreEqual(layerWithSoil1.IsAquifer, surface1.IsAquifer);
            Assert.AreEqual(layerWithSoil1.WaterPressureInterpolationModel, surface1.WaterPressureInterpolationModel);
            Assert.AreSame(profile.Geometry.Loops.ElementAt(0), surface1.Surface.OuterLoop);
            CollectionAssert.IsEmpty(surface1.Surface.InnerLoops);

            SoilProfileSurface surface2 = profile.SoilSurfaces.ElementAt(1);
            Assert.AreSame(soil2, surface2.Soil);
            Assert.AreEqual(soil2.Name, surface2.Name);
            Assert.AreEqual(layerWithSoil2.IsAquifer, surface2.IsAquifer);
            Assert.AreEqual(layerWithSoil2.WaterPressureInterpolationModel, surface2.WaterPressureInterpolationModel);
            Assert.AreSame(profile.Geometry.Loops.ElementAt(1), surface2.Surface.OuterLoop);
            CollectionAssert.AreEqual(new[]
            {
                profile.Geometry.Loops.ElementAt(2)
            }, surface2.Surface.InnerLoops);

            SoilProfileSurface surface3 = profile.SoilSurfaces.ElementAt(2);
            Assert.AreSame(soil3, surface3.Soil);
            Assert.AreEqual(soil3.Name, surface3.Name);
            Assert.AreEqual(layerWithSoil3.IsAquifer, surface3.IsAquifer);
            Assert.AreEqual(layerWithSoil3.WaterPressureInterpolationModel, surface3.WaterPressureInterpolationModel);
            Assert.AreSame(profile.Geometry.Loops.ElementAt(2), surface3.Surface.OuterLoop);
            CollectionAssert.AreEqual(new[]
            {
                profile.Geometry.Loops.ElementAt(3)
            }, surface3.Surface.InnerLoops);

            SoilProfileSurface surface4 = profile.SoilSurfaces.ElementAt(3);
            Assert.AreSame(soil4, surface4.Soil);
            Assert.AreEqual(soil4.Name, surface4.Name);
            Assert.AreEqual(layerWithSoil4.IsAquifer, surface4.IsAquifer);
            Assert.AreEqual(layerWithSoil4.WaterPressureInterpolationModel, surface4.WaterPressureInterpolationModel);
            Assert.AreSame(profile.Geometry.Loops.ElementAt(3), surface4.Surface.OuterLoop);
            CollectionAssert.IsEmpty(surface4.Surface.InnerLoops);

            #endregion
        }

        private static Curve CreateCurve(CSharpWrapperPoint2D headPoint, CSharpWrapperPoint2D endPoint)
        {
            return new Curve
            {
                HeadPoint = headPoint,
                EndPoint = endPoint
            };
        }
    }
}