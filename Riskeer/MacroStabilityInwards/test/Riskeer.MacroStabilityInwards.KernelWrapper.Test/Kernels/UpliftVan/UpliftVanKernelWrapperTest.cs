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
using System.Collections.Generic;
using System.Linq;
using Core.Common.TestUtil;
using Core.Common.Util.Reflection;
using Deltares.MacroStability.Data;
using Deltares.MacroStability.Geometry;
using Deltares.MacroStability.Standard;
using NUnit.Framework;
using Riskeer.MacroStabilityInwards.KernelWrapper.Kernels.UpliftVan;
using WtiStabilityWaternet = Deltares.MacroStability.Geometry.Waternet;

namespace Riskeer.MacroStabilityInwards.KernelWrapper.Test.Kernels.UpliftVan
{
    [TestFixture]
    public class UpliftVanKernelWrapperTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var kernel = new UpliftVanKernelWrapper();

            // Assert
            Assert.IsInstanceOf<IUpliftVanKernel>(kernel);
            Assert.IsNaN(kernel.FactorOfStability);
            Assert.IsNaN(kernel.ZValue);
            Assert.IsNaN(kernel.ForbiddenZonesXEntryMin);
            Assert.IsNaN(kernel.ForbiddenZonesXEntryMax);
            Assert.IsNull(kernel.SlidingCurveResult);
            Assert.IsNull(kernel.SlipPlaneResult);
            Assert.IsNull(kernel.CalculationMessages);
        }

        [Test]
        public void Constructor_CompleteInput_InputCorrectlySetToWrappedKernel()
        {
            // Setup
            var random = new Random(21);
            var soilProfile2D = new SoilProfile2D();
            double maximumSliceWidth = random.NextDouble();
            var slipPlaneUpliftVan = new SlipPlaneUpliftVan();
            bool moveGrid = random.NextBoolean();
            bool gridAutomaticDetermined = random.NextBoolean();
            var slipPlaneConstraints = new SlipPlaneConstraints();
            var waternetDaily = new WtiStabilityWaternet();
            var waternetExtreme = new WtiStabilityWaternet();

            // Call
            var kernel = new UpliftVanKernelWrapper();
            kernel.SetSoilProfile(soilProfile2D);
            kernel.SetMaximumSliceWidth(maximumSliceWidth);
            kernel.SetSlipPlaneUpliftVan(slipPlaneUpliftVan);
            kernel.SetMoveGrid(moveGrid);
            kernel.SetGridAutomaticDetermined(gridAutomaticDetermined);
            kernel.SetSlipPlaneConstraints(slipPlaneConstraints);
            kernel.SetWaternetDaily(waternetDaily);
            kernel.SetWaternetExtreme(waternetExtreme);

            // Assert
            var stabilityModel = TypeUtils.GetField<StabilityModel>(kernel, "stabilityModel");
            var actualDailyWaternet = TypeUtils.GetField<WtiStabilityWaternet>(kernel, "dailyWaternet");
            var actualExtremeWaternet = TypeUtils.GetField<WtiStabilityWaternet>(kernel, "extremeWaternet");

            Assert.IsNotNull(stabilityModel.SlipPlaneConstraints);
            Assert.AreEqual(GridOrientation.Inwards, stabilityModel.GridOrientation);
            Assert.IsNotNull(stabilityModel.SlipCircle);
            Assert.AreEqual(SearchAlgorithm.Grid, stabilityModel.SearchAlgorithm);
            Assert.AreEqual(ModelOptions.UpliftVan, stabilityModel.ModelOption);
            Assert.AreEqual(maximumSliceWidth, stabilityModel.MaximumSliceWidth);
            Assert.AreSame(slipPlaneUpliftVan, stabilityModel.SlipPlaneUpliftVan);
            Assert.AreSame(slipPlaneConstraints, stabilityModel.SlipPlaneConstraints);
            Assert.AreEqual(moveGrid, stabilityModel.MoveGrid);
            Assert.AreEqual(waternetDaily, actualDailyWaternet);
            Assert.AreEqual(waternetExtreme, actualExtremeWaternet);

            AssertIrrelevantValues(stabilityModel);
        }

        [Test]
        public void Calculate_ExceptionInWrappedKernel_ThrowsUpliftVanKernelWrapperException()
        {
            // Setup
            var kernel = new UpliftVanKernelWrapper();

            // Call
            TestDelegate test = () => kernel.Calculate();

            // Assert
            var exception = Assert.Throws<UpliftVanKernelWrapperException>(test);
            Assert.IsNotNull(exception.InnerException);
            Assert.AreEqual(exception.InnerException.Message, exception.Message);
        }

        [Test]
        public void Calculate_ExceptionDuringCalculation_OutputPropertiesNotSet()
        {
            // Setup
            var kernel = new UpliftVanKernelWrapper();

            // Call
            TestDelegate test = () => kernel.Calculate();

            // Assert
            Assert.Throws<UpliftVanKernelWrapperException>(test);
            Assert.IsNaN(kernel.FactorOfStability);
            Assert.IsNaN(kernel.ZValue);
            Assert.IsNaN(kernel.ForbiddenZonesXEntryMax);
            Assert.IsNaN(kernel.ForbiddenZonesXEntryMin);
            Assert.IsNull(kernel.SlidingCurveResult);
            Assert.IsNull(kernel.SlipPlaneResult);
        }

        [Test]
        public void Calculate_CompleteInput_ReturnsNoErrors()
        {
            // Setup
            UpliftVanKernelWrapper kernel = CreateValidKernel(new Soil
            {
                ShearStrengthModel = ShearStrengthModel.CPhi,
                AbovePhreaticLevel = 15.0,
                BelowPhreaticLevel = 15.0,
                Cohesion = 10.0,
                Dilatancy = 10.0,
                FrictionAngle = 10.0
            });

            // Call
            kernel.Calculate();

            // Assert
            LogMessage[] errorMessages = kernel.CalculationMessages.Where(m => m.MessageType == LogMessageType.Error ||
                                                                               m.MessageType == LogMessageType.FatalError).ToArray();
            Assert.AreEqual(0, errorMessages.Length);

            Assert.IsFalse(double.IsNaN(kernel.FactorOfStability));
            Assert.IsFalse(double.IsNaN(kernel.ZValue));
            Assert.IsFalse(double.IsNaN(kernel.ForbiddenZonesXEntryMin));
            Assert.IsFalse(double.IsNaN(kernel.ForbiddenZonesXEntryMax));
            Assert.IsNotNull(kernel.SlidingCurveResult);
            Assert.IsNotNull(kernel.SlipPlaneResult);
        }

        [Test]
        public void Validate_InputComplete_NoValidationMessages()
        {
            // Setup
            UpliftVanKernelWrapper kernel = CreateValidKernel(new Soil
            {
                ShearStrengthModel = ShearStrengthModel.CPhi,
                AbovePhreaticLevel = 15.0,
                BelowPhreaticLevel = 15.0,
                Cohesion = 10.0,
                Dilatancy = 10.0,
                FrictionAngle = 10.0
            });

            // Call
            var validationMessages = kernel.Validate();

            // Assert
            Assert.IsEmpty(validationMessages.Where(vm => vm.MessageType == ValidationResultType.Error));
        }

        [Test]
        public void Validate_InputNotComplete_ThrowsUpliftVanKernelWrapperException()
        {
            // Setup
            UpliftVanKernelWrapper kernel = CreateInvalidKernel(new Soil());

            // Call
            void Test() => kernel.Calculate();

            // Assert
            var exception = Assert.Throws<UpliftVanKernelWrapperException>(Test);
            Assert.IsInstanceOf<NullReferenceException>(exception.InnerException);
            Assert.AreEqual(exception.InnerException.Message, exception.Message);
        }

        [Test]
        public void Validate_InvalidInput_GeneratesValidationMessages()
        {
            // Setup
            UpliftVanKernelWrapper kernel = CreateInvalidKernel(null);

            // Call
            var validationMessages = kernel.Validate();

            // Assert
            Assert.AreEqual(14, validationMessages.Count());
        }

        private static UpliftVanKernelWrapper CreateValidKernel(Soil soil)
        {
            var point1 = new Point2D(-50, -50);
            var point2 = new Point2D(100, -50);
            var point3 = new Point2D(100, 6);
            var point4 = new Point2D(50, 6);
            var point5 = new Point2D(0, 10);
            var point6 = new Point2D(-25, 10);
            var point7 = new Point2D(-35, 6);
            var point8 = new Point2D(-50, 6);
            var curve1 = new GeometryCurve(point1, point2);
            var curve2 = new GeometryCurve(point2, point3);
            var curve3 = new GeometryCurve(point3, point4);
            var curve4 = new GeometryCurve(point4, point5);
            var curve5 = new GeometryCurve(point5, point6);
            var curve6 = new GeometryCurve(point6, point7);
            var curve7 = new GeometryCurve(point7, point8);
            var curve8 = new GeometryCurve(point8, point1);
            var loop = new GeometryLoop
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
            var geometrySurface = new GeometrySurface
            {
                OuterLoop = loop
            };
            var kernelWrapper = new UpliftVanKernelWrapper();
            kernelWrapper.SetSoilModel(new List<Soil>
            {
                soil
            });
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
                        point8
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
                        curve8
                    },
                    Loops =
                    {
                        loop
                    },
                    Surfaces =
                    {
                        geometrySurface
                    },
                    Left = -50,
                    Right = 100,
                    Bottom = -50
                },
                Surfaces =
                {
                    new SoilLayer2D
                    {
                        GeometrySurface = geometrySurface,
                        Soil = soil
                    }
                }
            });
            kernelWrapper.SetSlipPlaneUpliftVan(new SlipPlaneUpliftVan
            {
                SlipPlaneLeftGrid = new SlipCircleGrid
                {
                    GridXLeft = -10.0,
                    GridXRight = 20.0,
                    GridZBottom = 12.0,
                    GridZTop = 25.0,
                    GridXNumber = 5,
                    GridZNumber = 5
                },
                SlipPlaneRightGrid = new SlipCircleGrid
                {
                    GridXLeft = 30,
                    GridXRight = 60,
                    GridZBottom = 10.0,
                    GridZTop = 20.0,
                    GridXNumber = 5,
                    GridZNumber = 5
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
                CharacteristicPointType = CharacteristicPointType.DikeTopAtPolder,
                X = 0,
                Z = 10,
                GeometryPoint = surfaceLine.Geometry.Points[5]
            });

            kernelWrapper.SetSurfaceLine(surfaceLine);
            kernelWrapper.SetGridAutomaticDetermined(false);
            kernelWrapper.SetMoveGrid(true);
            kernelWrapper.SetMaximumSliceWidth(100);
            kernelWrapper.SetSlipPlaneConstraints(new SlipPlaneConstraints
            {
                XLeftMin = 0.0,
                XLeftMax = 100
            });

            return kernelWrapper;
        }

        private static UpliftVanKernelWrapper CreateInvalidKernel(Soil soil)
        {
            var point1 = new Point2D(0, 0);
            var point2 = new Point2D(1, 1);
            var point3 = new Point2D(2, 2);
            var point4 = new Point2D(3, 3);
            var curve1 = new GeometryCurve(point1, point2);
            var curve2 = new GeometryCurve(point2, point3);
            var curve3 = new GeometryCurve(point3, point4);
            var curve4 = new GeometryCurve(point4, point1);
            var loop = new GeometryLoop
            {
                CurveList =
                {
                    curve1,
                    curve2,
                    curve3,
                    curve4
                }
            };
            var geometrySurface = new GeometrySurface
            {
                OuterLoop = loop
            };
            var kernelWrapper = new UpliftVanKernelWrapper();
            kernelWrapper.SetSoilModel(new List<Soil>
            {
                soil
            });
            kernelWrapper.SetSoilProfile(new SoilProfile2D
            {
                Geometry = new GeometryData
                {
                    Points =
                    {
                        point1,
                        point2,
                        point3,
                        point4
                    },
                    Curves =
                    {
                        curve1,
                        curve2,
                        curve3,
                        curve4
                    },
                    Loops =
                    {
                        loop
                    },
                    Surfaces =
                    {
                        geometrySurface
                    }
                },
                Surfaces =
                {
                    new SoilLayer2D
                    {
                        GeometrySurface = geometrySurface,
                        Soil = soil
                    }
                }
            });
            kernelWrapper.SetSlipPlaneUpliftVan(new SlipPlaneUpliftVan());
            kernelWrapper.SetMoveGrid(true);
            kernelWrapper.SetMaximumSliceWidth(0);
            kernelWrapper.SetSlipPlaneConstraints(new SlipPlaneConstraints());

            return kernelWrapper;
        }

        private static void AssertIrrelevantValues(StabilityModel stabilityModel)
        {
            Assert.IsNull(stabilityModel.MinimumSafetyCurve); // Output
            Assert.AreEqual(0.8, stabilityModel.RequiredForcePointsInSlices); // Only for Spencer
            Assert.AreEqual(60.0, stabilityModel.MaxAllowedAngleBetweenSlices); // Only for Spencer
            Assert.IsNotNull(stabilityModel.GeneticAlgorithmOptions); // Only for genetic search algorithm
            Assert.IsNotNull(stabilityModel.LevenbergMarquardtOptions); // Only for Levenberg Marquardt search algorithm
        }
    }
}