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

using Core.Common.Util.Reflection;
using Deltares.MacroStability.Data;
using Deltares.MacroStability.Geometry;
using Deltares.MacroStability.WaternetCreator;
using NUnit.Framework;
using Riskeer.MacroStabilityInwards.KernelWrapper.Kernels.Waternet;

namespace Riskeer.MacroStabilityInwards.KernelWrapper.Test.Kernels.Waternet
{
    [TestFixture]
    public class WaternetDailyKernelWrapperTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var kernel = new WaternetDailyKernelWrapper();

            // Assert
            Assert.IsInstanceOf<WaternetKernelWrapper>(kernel);
        }

        [Test]
        public void Constructor_CompleteInput_InputCorrectlySetToWrappedKernel()
        {
            // Setup
            var stabilityLocation = new Location();
            var soilModel = new SoilModel();
            var soilProfile2D = new SoilProfile2D();
            var surfaceLine = new SurfaceLine2();

            // Call
            var kernel = new WaternetDailyKernelWrapper();
            kernel.SetLocation(stabilityLocation);
            kernel.SetSoilModel(soilModel);
            kernel.SetSoilProfile(soilProfile2D);
            kernel.SetSurfaceLine(surfaceLine);

            // Assert
            var stabilityModel = TypeUtils.GetProperty<StabilityModel>(kernel, "StabilityModel");

            Assert.AreSame(stabilityLocation, stabilityModel.LocationDaily);
            Assert.AreSame(surfaceLine, stabilityModel.SurfaceLine2);
            Assert.AreSame(soilModel, stabilityModel.SoilModel);
            Assert.AreSame(soilProfile2D, stabilityModel.SoilProfile);

            Assert.IsNotNull(stabilityModel.GeotechnicsData.CurrentWaternetDaily);
            Assert.AreEqual("WaternetDaily", stabilityModel.GeotechnicsData.CurrentWaternetDaily.Name);

            AssertAutomaticallySyncedValues(stabilityModel, soilProfile2D, surfaceLine);
        }

        [Test]
        public void Calculate_ValidationErrorInCalculation_ThrowsWaternetKernelWrapperException()
        {
            // Setup
            WaternetDailyKernelWrapper kernel = CreateInvalidKernel();

            // Call
            TestDelegate test = () => kernel.Calculate();

            // Assert
            var exception = Assert.Throws<WaternetKernelWrapperException>(test);
            Assert.AreEqual("Waternet-Beoordeling: De punten in de hoogtegeometrie zijn niet oplopend. (x-waarde)\r\n" +
                            "Waternet-Dagelijks: De punten in de hoogtegeometrie zijn niet oplopend. (x-waarde)", exception.Message);
        }

        private static WaternetDailyKernelWrapper CreateInvalidKernel()
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
            var soil = new Soil();
            var waternetDailyKernelWrapper = new WaternetDailyKernelWrapper();
            waternetDailyKernelWrapper.SetLocation(new Location());
            waternetDailyKernelWrapper.SetSoilModel(new SoilModel
            {
                Soils =
                {
                    soil
                }
            });
            waternetDailyKernelWrapper.SetSoilProfile(new SoilProfile2D
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
            waternetDailyKernelWrapper.SetSurfaceLine(new SurfaceLine2());

            return waternetDailyKernelWrapper;
        }

        private static void AssertAutomaticallySyncedValues(StabilityModel stabilityModel, SoilProfile2D soilProfile2D, SurfaceLine2 surfaceLine)
        {
            Assert.AreSame(stabilityModel, stabilityModel.LocationDaily.StabilityModel);
            Assert.IsTrue(stabilityModel.LocationDaily.Inwards);
            Assert.AreSame(soilProfile2D, stabilityModel.LocationDaily.SoilProfile2D);
            Assert.AreSame(surfaceLine, stabilityModel.LocationDaily.Surfaceline);
            Assert.IsTrue(stabilityModel.LocationDaily.Inwards);
        }
    }
}