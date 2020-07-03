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

using System.Collections.Generic;
using System.Linq;
using Core.Common.Util.Reflection;
using Deltares.MacroStability.Geometry;
using Deltares.MacroStability.Standard;
using Deltares.MacroStability.WaternetCreator;
using NUnit.Framework;
using Riskeer.MacroStabilityInwards.KernelWrapper.Kernels.Waternet;
using WtiStabilityWaternet = Deltares.MacroStability.Geometry.Waternet;

namespace Riskeer.MacroStabilityInwards.KernelWrapper.Test.Kernels.Waternet
{
    [TestFixture]
    public class WaternetKernelWrapperTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var location = new Location();

            // Call
            var kernel = new WaternetKernelWrapper(location, "Waternet");

            // Assert
            Assert.IsInstanceOf<IWaternetKernel>(kernel);
            Assert.IsNull(location.Surfaceline);
            Assert.IsNull(location.SoilProfile2D);
        }

        [Test]
        public void Constructor_CompleteInput_InputCorrectlySetToWrappedKernel()
        {
            // Setup
            var location = new Location();
            var soilProfile2D = new SoilProfile2D();
            var surfaceLine = new SurfaceLine2();

            // Call
            var kernel = new WaternetKernelWrapper(location, "Waternet");
            kernel.SetSoilProfile(soilProfile2D);
            kernel.SetSurfaceLine(surfaceLine);

            // Assert
            var waternetCreator = TypeUtils.GetField<WaternetCreator>(kernel, "waternetCreator");

            Assert.AreSame(surfaceLine, location.Surfaceline);
            Assert.AreSame(soilProfile2D, location.SoilProfile2D);
            Assert.AreEqual(9.81, kernel.Waternet.UnitWeight);

            AssertIrrelevantValues(kernel.Waternet, waternetCreator);
        }

        [Test]
        public void Calculate_ExceptionInWrappedKernel_ThrowsWaternetKernelWrapperException()
        {
            // Setup
            var kernel = new WaternetKernelWrapper(new Location(), "Waternet");

            // Call
            void Call() => kernel.Calculate();

            // Assert
            var exception = Assert.Throws<WaternetKernelWrapperException>(Call);
            Assert.IsNotNull(exception.InnerException);
            Assert.AreEqual(exception.InnerException.Message, exception.Message);
        }

        [Test]
        public void Validate_CompleteInput_NoValidationMessages()
        {
            // Setup
            WaternetKernelWrapper kernel = CreateValidKernel(new Soil
            {
                ShearStrengthModel = ShearStrengthModel.CPhi,
                AbovePhreaticLevel = 15.0,
                BelowPhreaticLevel = 15.0,
                Cohesion = 10.0,
                Dilatancy = 10.0,
                FrictionAngle = 10.0
            });

            // Call
            IEnumerable<IValidationResult> validationMessages = kernel.Validate();

            // Assert
            CollectionAssert.IsEmpty(validationMessages);
        }

        [Test]
        public void Validate_InputNotComplete_ReturnsValidationMessages()
        {
            // Setup
            var location = new Location
            {
                WaternetCreationMode = WaternetCreationMode.CreateWaternet
            };

            var kernel = new WaternetKernelWrapper(location, "Waternet");

            // Call
            IEnumerable<IValidationResult> validationMessages = kernel.Validate();

            // Assert
            CollectionAssert.IsNotEmpty(validationMessages);
        }

        private static WaternetKernelWrapper CreateValidKernel(Soil soil)
        {
            var location = new Location
            {
                WaternetCreationMode = WaternetCreationMode.CreateWaternet,
                Inwards = true,
                PenetrationLength = 2,
                HeadInPlLine2Outwards = 1,
                WaterLevelPolder = 1,
                PlLineCreationMethod = PlLineCreationMethod.RingtoetsWti2017,
                PlLineOffsetBelowDikeToeAtPolder = 1,
                PlLineOffsetBelowDikeTopAtPolder = 1,
                PlLineOffsetBelowShoulderBaseInside = 1,
                MinimumLevelPhreaticLineAtDikeTopRiver = 1,
                MinimumLevelPhreaticLineAtDikeTopPolder = 1,
                DikeSoilScenario = DikeSoilScenario.ClayDikeOnSand
            };

            var point1 = new Point2D(-50, -50);
            var point2 = new Point2D(100, -50);
            var point3 = new Point2D(100, 6);
            var point4 = new Point2D(50, 6);
            var point5 = new Point2D(0, 10);
            var point6 = new Point2D(-25, 10);
            var point7 = new Point2D(-35, 6);
            var point8 = new Point2D(-50, 6);
            var point9 = new Point2D(100, -100);
            var point10 = new Point2D(-50, -100);
            var curve1 = new GeometryCurve(point1, point2);
            var curve2 = new GeometryCurve(point2, point3);
            var curve3 = new GeometryCurve(point3, point4);
            var curve4 = new GeometryCurve(point4, point5);
            var curve5 = new GeometryCurve(point5, point6);
            var curve6 = new GeometryCurve(point6, point7);
            var curve7 = new GeometryCurve(point7, point8);
            var curve8 = new GeometryCurve(point8, point1);
            var curve9 = new GeometryCurve(point2, point9);
            var curve10 = new GeometryCurve(point9, point10);
            var curve11 = new GeometryCurve(point10, point1);
            var loop1 = new GeometryLoop
            {
                CurveList =
                {
                    curve1,
                    curve2,
                    curve3,
                    curve4,
                    curve5,
                    curve6,
                    curve7,
                    curve8
                }
            };
            var loop2 = new GeometryLoop
            {
                CurveList =
                {
                    curve1,
                    curve9,
                    curve10,
                    curve11
                }
            };
            var geometrySurface1 = new GeometrySurface
            {
                OuterLoop = loop1
            };
            var geometrySurface2 = new GeometrySurface
            {
                OuterLoop = loop2
            };
            var kernelWrapper = new WaternetKernelWrapper(location, "Waternet");
            kernelWrapper.SetSoilProfile(new SoilProfile2D
            {
                Geometry = new GeometryData
                {
                    Points =
                    {
                        point1,
                        point2,
                        point3,
                        point4,
                        point5,
                        point6,
                        point7,
                        point9,
                        point10
                    },
                    Curves =
                    {
                        curve1,
                        curve2,
                        curve3,
                        curve4,
                        curve5,
                        curve6,
                        curve7,
                        curve8,
                        curve9,
                        curve10,
                        curve11
                    },
                    Loops =
                    {
                        loop1,
                        loop2
                    },
                    Surfaces =
                    {
                        geometrySurface1,
                        geometrySurface2
                    },
                    Left = -50,
                    Right = 100,
                    Bottom = -50
                },
                Surfaces =
                {
                    new SoilLayer2D
                    {
                        GeometrySurface = geometrySurface1,
                        Soil = soil
                    },
                    new SoilLayer2D
                    {
                        GeometrySurface = geometrySurface2,
                        Soil = soil,
                        IsAquifer = true
                    }
                }
            });
            var surfaceLine = new SurfaceLine2
            {
                Geometry = new GeometryPointString
                {
                    CalcPoints =
                    {
                        point8,
                        point7,
                        point6,
                        point5,
                        point4,
                        point3
                    }
                }
            };
            surfaceLine.Geometry.SyncPoints();
            surfaceLine.CharacteristicPoints.Add(new CharacteristicPoint
            {
                CharacteristicPointType = CharacteristicPointType.DikeToeAtPolder,
                X = -10,
                Z = 0,
                GeometryPoint = surfaceLine.Geometry.Points[5]
            });
            surfaceLine.CharacteristicPoints.Add(new CharacteristicPoint
            {
                CharacteristicPointType = CharacteristicPointType.DikeTopAtPolder,
                X = 0,
                Z = 10,
                GeometryPoint = surfaceLine.Geometry.Points[5]
            });
            surfaceLine.CharacteristicPoints.Add(new CharacteristicPoint
            {
                CharacteristicPointType = CharacteristicPointType.DikeTopAtRiver,
                X = 10,
                Z = 10,
                GeometryPoint = surfaceLine.Geometry.Points[5]
            });
            surfaceLine.CharacteristicPoints.Add(new CharacteristicPoint
            {
                CharacteristicPointType = CharacteristicPointType.DikeToeAtRiver,
                X = 20,
                Z = 0,
                GeometryPoint = surfaceLine.Geometry.Points[5]
            });
            surfaceLine.CharacteristicPoints.Add(new CharacteristicPoint
            {
                CharacteristicPointType = CharacteristicPointType.SurfaceLevelOutside,
                X = 20,
                Z = 0,
                GeometryPoint = surfaceLine.Geometry.Points[5]
            });
            surfaceLine.CharacteristicPoints.Add(new CharacteristicPoint
            {
                CharacteristicPointType = CharacteristicPointType.SurfaceLevelInside,
                X = 20,
                Z = 0,
                GeometryPoint = surfaceLine.Geometry.Points[5]
            });

            kernelWrapper.SetSurfaceLine(surfaceLine);

            return kernelWrapper;
        }

        private static void AssertIrrelevantValues(WtiStabilityWaternet waternet, WaternetCreator waternetCreator)
        {
            Assert.AreEqual("Waternet", waternet.Name);
            Assert.IsFalse(waternet.IsGenerated);

            Assert.AreEqual(Enumerable.Empty<LogMessage>(), waternetCreator.LogMessages);
            Assert.AreEqual(LanguageType.Dutch, waternetCreator.Language);
        }
    }
}