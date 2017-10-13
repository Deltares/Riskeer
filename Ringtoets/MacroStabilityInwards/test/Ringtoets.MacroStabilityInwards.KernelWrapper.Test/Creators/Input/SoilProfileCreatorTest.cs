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
using System.ComponentModel;
using System.Linq;
using Core.Common.Base.Data;
using Core.Common.TestUtil;
using Deltares.WTIStability.Data.Geo;
using NUnit.Framework;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.Input;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Creators.Input;
using Ringtoets.MacroStabilityInwards.KernelWrapper.TestUtil.Kernels.UpliftVan.Input;
using Point2D = Core.Common.Base.Geometry.Point2D;
using SoilLayer = Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.Input.SoilLayer;
using SoilProfile = Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.Input.SoilProfile;
using WtiStabilityPoint2D = Deltares.WTIStability.Data.Geo.Point2D;

namespace Ringtoets.MacroStabilityInwards.KernelWrapper.Test.Creators.Input
{
    [TestFixture]
    public class SoilProfileCreatorTest
    {
        [Test]
        public void Create_SoilProfileNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => SoilProfileCreator.Create(null, new Dictionary<SoilLayer, Soil>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("soilProfile", exception.ParamName);
        }

        [Test]
        public void Create_SoilDictionaryNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => SoilProfileCreator.Create(new SoilProfile(Enumerable.Empty<SoilLayer>(),
                                                                                Enumerable.Empty<PreconsolidationStress>()),
                                                                null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("layersWithSoils", exception.ParamName);
        }

        [Test]
        public void Create_InvalidWaterPressureInterpolationModel_ThrowInvalidEnumArgumentException()
        {
            // Setup
            var layer = new SoilLayer(
                new[]
                {
                    new Point2D(0, 0),
                    new Point2D(1, 1)
                }, Enumerable.Empty<Point2D[]>(),
                new SoilLayer.ConstructionProperties
                {
                    WaterPressureInterpolationModel = (WaterPressureInterpolationModel) 99
                });

            var soilProfile = new SoilProfile(new[]
            {
                layer
            }, Enumerable.Empty<PreconsolidationStress>());

            // Call
            TestDelegate test = () => SoilProfileCreator.Create(soilProfile, new Dictionary<SoilLayer, Soil>
            {
                {
                    layer, new Soil()
                }
            });

            // Assert
            string message = $"The value of argument 'waterPressureInterpolationModel' ({99}) is invalid for Enum type '{typeof(WaterPressureInterpolationModel).Name}'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(test, message);
        }

        /// <remarks>
        /// The soil profile used in this tests contains two layers (outer rings) and two holes (inner rings):
        ///                                    
        ///  11    # # # # # # # # # # #       
        ///  10    #                   #       
        ///   9    #   # # # # # # #   #       
        ///   8    #   #           #   #       
        ///   7    #   # # # # # # #   #       
        ///   6    #   #           #   #       
        ///   5    #   # # # # # # #   #       
        ///   3    #                   #       
        ///   3    # # # # # # # # # # #       
        ///   2    #                   #       
        ///   1    #                   #       
        ///   0    # # # # # # # # # # #       
        ///                                    
        ///        0 1 2 3 4 5 6 7 8 9 10      
        /// </remarks>>
        [TestCase(WaterPressureInterpolationModel.Automatic, WaterpressureInterpolationModel.Automatic)]
        [TestCase(WaterPressureInterpolationModel.Hydrostatic, WaterpressureInterpolationModel.Hydrostatic)]
        public void Create_WithAllData_ReturnSoilProfile2D(WaterPressureInterpolationModel waterPressureInterpolationModel, WaterpressureInterpolationModel waterpressureInterpolationModel)
        {
            // Setup
            var random = new Random(11);
            double preconsolidationStressXCoordinate = random.Next();
            double preconsolidationStressZCoordinate = random.Next();
            RoundedDouble preconsolidationStressDesignValue = random.NextRoundedDouble();

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

            var layer1 = new SoilLayer(
                layer1Points, Enumerable.Empty<Point2D[]>(),
                new SoilLayer.ConstructionProperties
                {
                    MaterialName = "Clay",
                    IsAquifer = false,
                    WaterPressureInterpolationModel = waterPressureInterpolationModel
                });

            var layer2 = new SoilLayer(
                layer2Points, new[]
                {
                    layer2Hole1Points,
                    layer2Hole2Points
                },
                new SoilLayer.ConstructionProperties
                {
                    MaterialName = "Sand",
                    IsAquifer = true,
                    WaterPressureInterpolationModel = waterPressureInterpolationModel
                });

            var soil1 = new Soil
            {
                Name = layer1.MaterialName
            };

            var soil2 = new Soil
            {
                Name = layer2.MaterialName
            };

            var soilProfile = new SoilProfile(new[]
            {
                layer1,
                layer2
            }, new[]
            {
                new PreconsolidationStress(new Point2D(preconsolidationStressXCoordinate, preconsolidationStressZCoordinate), preconsolidationStressDesignValue)
            });

            // Call
            SoilProfile2D profile = SoilProfileCreator.Create(soilProfile, new Dictionary<SoilLayer, Soil>
            {
                {
                    layer1, soil1
                },
                {
                    layer2, soil2
                }
            });

            // Assert
            Assert.AreEqual(1, profile.PreconsolidationStresses.Count);
            PreConsolidationStress preconsolidationStress = profile.PreconsolidationStresses.First();

            Assert.IsTrue(string.IsNullOrEmpty(preconsolidationStress.Name)); // Unused property
            Assert.AreEqual(preconsolidationStressDesignValue, preconsolidationStress.StressValue);
            Assert.AreEqual(preconsolidationStressXCoordinate, preconsolidationStress.X);
            Assert.AreEqual(preconsolidationStressZCoordinate, preconsolidationStress.Z);

            Assert.AreEqual(2, profile.Surfaces.Count);
            SoilLayer2D surface1 = profile.Surfaces.ElementAt(0);
            Assert.AreSame(soil1, surface1.Soil);
            Assert.AreEqual(layer1.MaterialName, surface1.Name);
            Assert.AreEqual(layer1.IsAquifer, surface1.IsAquifer);
            Assert.AreEqual(waterpressureInterpolationModel, surface1.WaterpressureInterpolationModel);
            SoilLayer2D surface2 = profile.Surfaces.ElementAt(1);
            Assert.AreSame(soil2, surface2.Soil);
            Assert.AreEqual(layer2.MaterialName, surface2.Name);
            Assert.AreEqual(layer2.IsAquifer, surface2.IsAquifer);
            Assert.AreEqual(waterpressureInterpolationModel, surface2.WaterpressureInterpolationModel);

            var outerLoopPoint1 = new WtiStabilityPoint2D(0, 0);
            var outerLoopPoint2 = new WtiStabilityPoint2D(0, 3);
            var outerLoopPoint3 = new WtiStabilityPoint2D(10, 3);
            var outerLoopPoint4 = new WtiStabilityPoint2D(10, 0);
            var outerLoopPoint5 = new WtiStabilityPoint2D(0, 11);
            var outerLoopPoint6 = new WtiStabilityPoint2D(10, 11);
            var outerLoopCurve1 = new GeometryCurve(outerLoopPoint1, outerLoopPoint2);
            var outerLoopCurve2 = new GeometryCurve(outerLoopPoint2, outerLoopPoint3);
            var outerLoopCurve3 = new GeometryCurve(outerLoopPoint3, outerLoopPoint4);
            var outerLoopCurve4 = new GeometryCurve(outerLoopPoint4, outerLoopPoint1);
            var outerLoopCurve5 = new GeometryCurve(outerLoopPoint2, outerLoopPoint5);
            var outerLoopCurve6 = new GeometryCurve(outerLoopPoint5, outerLoopPoint6);
            var outerLoopCurve7 = new GeometryCurve(outerLoopPoint6, outerLoopPoint3);
            var outerLoop1 = new GeometryLoop
            {
                CurveList =
                {
                    outerLoopCurve1,
                    outerLoopCurve2,
                    outerLoopCurve3,
                    outerLoopCurve4
                }
            };
            var outerLoop2 = new GeometryLoop
            {
                CurveList =
                {
                    outerLoopCurve5,
                    outerLoopCurve6,
                    outerLoopCurve7,
                    outerLoopCurve2
                }
            };

            var innerLoopPoint1 = new WtiStabilityPoint2D(2, 5);
            var innerLoopPoint2 = new WtiStabilityPoint2D(2, 7);
            var innerLoopPoint3 = new WtiStabilityPoint2D(8, 7);
            var innerLoopPoint4 = new WtiStabilityPoint2D(8, 5);
            var innerLoopPoint5 = new WtiStabilityPoint2D(2, 9);
            var innerLoopPoint6 = new WtiStabilityPoint2D(8, 9);
            var innerLoopCurve1 = new GeometryCurve(innerLoopPoint1, innerLoopPoint2);
            var innerLoopCurve2 = new GeometryCurve(innerLoopPoint2, innerLoopPoint3);
            var innerLoopCurve3 = new GeometryCurve(innerLoopPoint3, innerLoopPoint4);
            var innerLoopCurve4 = new GeometryCurve(innerLoopPoint4, innerLoopPoint1);
            var innerLoopCurve5 = new GeometryCurve(innerLoopPoint2, innerLoopPoint5);
            var innerLoopCurve6 = new GeometryCurve(innerLoopPoint5, innerLoopPoint6);
            var innerLoopCurve7 = new GeometryCurve(innerLoopPoint6, innerLoopPoint3);
            var innerLoop1 = new GeometryLoop
            {
                CurveList =
                {
                    innerLoopCurve1,
                    innerLoopCurve2,
                    innerLoopCurve3,
                    innerLoopCurve4
                }
            };
            var innerLoop2 = new GeometryLoop
            {
                CurveList =
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
        }
    }
}