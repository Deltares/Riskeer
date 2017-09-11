// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Core.Common.TestUtil;
using Deltares.WTIStability.Data.Geo;
using NUnit.Framework;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Creators;
using Ringtoets.MacroStabilityInwards.KernelWrapper.TestUtil;
using Ringtoets.MacroStabilityInwards.Primitives.MacroStabilityInwardsSoilUnderSurfaceLine;
using Point2D = Core.Common.Base.Geometry.Point2D;
using WTIStabilityPoint2D = Deltares.WTIStability.Data.Geo.Point2D;

namespace Ringtoets.MacroStabilityInwards.KernelWrapper.Test.Creators
{
    [TestFixture]
    public class SoilProfileCreatorTest
    {
        [Test]
        public void Create_SoilProfileNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => SoilProfileCreator.Create(null, new Dictionary<MacroStabilityInwardsSoilLayerUnderSurfaceLine, Soil>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("soilProfile", exception.ParamName);
        }

        [Test]
        public void Create_DictionaryNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => SoilProfileCreator.Create(new MacroStabilityInwardsSoilProfileUnderSurfaceLine(
                                                                    Enumerable.Empty<MacroStabilityInwardsSoilLayerUnderSurfaceLine>()), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("layersWithSoils", exception.ParamName);
        }

        [Test]
        public void Create_WithAllData_ReturnSoilProfile2D()
        {
            // Setup
            var random = new Random(11);
            double preconsolidationStressXCoordinate = random.Next();
            double preconsolidationStressZCoordinate = random.Next();
            RoundedDouble preconsolidationStressDesignValue = random.NextRoundedDouble();

            var outerRing = new[]
            {
                new Point2D(0, 0),
                new Point2D(10, 10),
                new Point2D(9, 9)
            };
            var holes = new[]
            {
                new[]
                {
                    new Point2D(2, 2),
                    new Point2D(4, 4),
                    new Point2D(2.5, 2.5)
                },
                new[]
                {
                    new Point2D(3, 3),
                    new Point2D(5, 5),
                    new Point2D(3, 3)
                }
            };

            var layer = new MacroStabilityInwardsSoilLayerUnderSurfaceLine(
                outerRing, holes,
                new MacroStabilityInwardsSoilLayerPropertiesUnderSurfaceLine(
                    new MacroStabilityInwardsSoilLayerPropertiesUnderSurfaceLine.ConstructionProperties
                    {
                        IsAquifer = true
                    }));

            var soil = new Soil();

            var soilProfile = new MacroStabilityInwardsSoilProfileUnderSurfaceLine(new[]
            {
                layer
            })
            {
                PreconsolidationStresses =
                {
                    new MacroStabilityInwardsPreconsolidationStressPropertiesUnderSurfaceLine(
                        new MacroStabilityInwardsPreconsolidationStressPropertiesUnderSurfaceLine.ConstructionProperties
                        {
                            XCoordinate = preconsolidationStressXCoordinate,
                            ZCoordinate = preconsolidationStressZCoordinate
                        })
                    {
                        PreconsolidationStressDesignVariable = preconsolidationStressDesignValue
                    }
                }
            };

            // Call
            SoilProfile2D profile = SoilProfileCreator.Create(soilProfile, new Dictionary<MacroStabilityInwardsSoilLayerUnderSurfaceLine, Soil>
            {
                {
                    layer, soil
                }
            });

            // Assert
            Assert.AreEqual(1, profile.Surfaces.Count);

            Assert.AreEqual(1, profile.PreconsolidationStresses.Count);
            PreConsolidationStress preconsolidationStress = profile.PreconsolidationStresses.First();

            Assert.AreEqual(preconsolidationStressXCoordinate, preconsolidationStress.X);
            Assert.AreEqual(preconsolidationStressZCoordinate, preconsolidationStress.Z);
            Assert.AreEqual(preconsolidationStressDesignValue, preconsolidationStress.StressValue);

            SoilLayer2D surface = profile.Surfaces.First();
            Assert.AreSame(soil, surface.Soil);
            Assert.AreEqual(layer.Properties.IsAquifer, surface.IsAquifer);
            Assert.AreEqual(WaterpressureInterpolationModel.Automatic, surface.WaterpressureInterpolationModel);

            var point1 = new WTIStabilityPoint2D(0, 0);
            var point2 = new WTIStabilityPoint2D(10, 10);
            var point3 = new WTIStabilityPoint2D(9, 9);
            var point4 = new WTIStabilityPoint2D(2, 2);
            var point5 = new WTIStabilityPoint2D(4, 4);
            var point6 = new WTIStabilityPoint2D(2.5, 2.5);
            var point7 = new WTIStabilityPoint2D(3, 3);
            var point8 = new WTIStabilityPoint2D(5, 5);
            var point9 = new WTIStabilityPoint2D(3, 3);
            var outerLoopCurve1 = new GeometryCurve(point1, point2);
            var outerLoopCurve2 = new GeometryCurve(point2, point3);
            var outerLoopCurve3 = new GeometryCurve(point3, point1);
            var innerLoop1Curve1 = new GeometryCurve(point4, point5);
            var innerLoop1Curve2 = new GeometryCurve(point5, point6);
            var innerLoop1Curve3 = new GeometryCurve(point6, point4);
            var innerLoop2Curve1 = new GeometryCurve(point7, point8);
            var innerLoop2Curve2 = new GeometryCurve(point8, point9);
            var expectedOuterLoop = new GeometryLoop
            {
                CurveList =
                {
                    outerLoopCurve1,
                    outerLoopCurve2,
                    outerLoopCurve3
                }
            };
            var expectedInnerLoop1 = new GeometryLoop
            {
                CurveList =
                {
                    innerLoop1Curve1,
                    innerLoop1Curve2,
                    innerLoop1Curve3
                }
            };
            var expectedInnerLoop2 = new GeometryLoop
            {
                CurveList =
                {
                    innerLoop2Curve1,
                    innerLoop2Curve2
                }
            };

            CollectionAssert.AreEqual(new[]
            {
                point1,
                point2,
                point3,
                point4,
                point5,
                point6,
                point7,
                point8,
                point9
            }, profile.Geometry.Points, new WTIStabilityPoint2DComparer());
            CollectionAssert.AreEqual(new[]
            {
                outerLoopCurve1,
                outerLoopCurve2,
                outerLoopCurve3,
                innerLoop1Curve1,
                innerLoop1Curve2,
                innerLoop1Curve3,
                innerLoop2Curve1,
                innerLoop2Curve2
            }, profile.Geometry.Curves, new WTIStabilityGeometryCurveComparer());
            CollectionAssert.AreEqual(new[]
            {
                expectedOuterLoop,
                expectedInnerLoop1,
                expectedInnerLoop2
            }, profile.Geometry.Loops, new WTIStabilityGeometryLoopComparer());

            CollectionAssert.AreEqual(expectedOuterLoop.CurveList, surface.GeometrySurface.OuterLoop.CurveList, new WTIStabilityGeometryCurveComparer());
            CollectionAssert.AreEqual(new[]
            {
                expectedInnerLoop1,
                expectedInnerLoop2
            }, surface.GeometrySurface.InnerLoops, new WTIStabilityGeometryLoopComparer());

            Assert.AreEqual(0, profile.Geometry.Left);
            Assert.AreEqual(0, profile.Geometry.Bottom);
            Assert.AreEqual(10, profile.Geometry.Right);
        }
    }
}