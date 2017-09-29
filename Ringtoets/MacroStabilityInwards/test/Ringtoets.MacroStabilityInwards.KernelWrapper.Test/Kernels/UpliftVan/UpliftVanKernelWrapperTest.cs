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
using System.Xml.Schema;
using Core.Common.TestUtil;
using Core.Common.Utils.Reflection;
using Deltares.WTIStability;
using Deltares.WTIStability.Data.Geo;
using NUnit.Framework;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Kernels.UpliftVan;

namespace Ringtoets.MacroStabilityInwards.KernelWrapper.Test.Kernels.UpliftVan
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
        }

        [Test]
        public void Calculate_InputNotComplete_ThrowsUpliftVanKernelWrapperException()
        {
            // Setup
            var kernel = new UpliftVanKernelWrapper();

            // Call
            TestDelegate test = () => kernel.Calculate();

            // Assert
            var exception = Assert.Throws<UpliftVanKernelWrapperException>(test);
            Assert.IsInstanceOf<XmlSchemaValidationException>(exception.InnerException);
            Assert.AreEqual(exception.InnerException.Message, exception.Message);
        }

        [Test]
        public void Calculate_InvalidInput_ThrowsUpliftVanKernelWrapperException()
        {
            // Setup
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
            var kernel = new UpliftVanKernelWrapper
            {
                SurfaceLine = new SurfaceLine2(),
                Location = new StabilityLocation(),
                SoilProfile = new SoilProfile2D
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
                        }
                    },
                    Surfaces =
                    {
                        new SoilLayer2D
                        {
                            GeometrySurface = new GeometrySurface
                            {
                                OuterLoop = loop
                            }
                        }
                    }
                },
                SoilModel = new SoilModel
                {
                    Soils =
                    {
                        new Soil()
                    }
                },
                SlipPlaneUpliftVan = new SlipPlaneUpliftVan(),
                MoveGrid = true,
                AutomaticForbiddenZones = true,
                CreateZones = true,
                SlipPlaneMinimumDepth = 0,
                MaximumSliceWidth = 0,
                SlipPlaneMinimumLength = 0
            };

            // Call
            TestDelegate test = () => kernel.Calculate();

            // Assert
            var exception = Assert.Throws<UpliftVanKernelWrapperException>(test);
            Assert.IsInstanceOf<ArgumentNullException>(exception.InnerException);
            Assert.AreEqual(exception.InnerException.Message, exception.Message);
        }

        [Test]
        public void Calculate_ErrorInCalculation_ThrowsUpliftVanKernelWrapperException()
        {
            // Setup
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
            var kernel = new UpliftVanKernelWrapper
            {
                SurfaceLine = new SurfaceLine2(),
                Location = new StabilityLocation(),
                SoilProfile = new SoilProfile2D
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
                },
                SoilModel = new SoilModel
                {
                    Soils =
                    {
                        soil
                    }
                },
                SlipPlaneUpliftVan = new SlipPlaneUpliftVan(),
                MoveGrid = true,
                AutomaticForbiddenZones = true,
                CreateZones = true,
                SlipPlaneMinimumDepth = 0,
                MaximumSliceWidth = 0,
                SlipPlaneMinimumLength = 0
            };

            // Call
            TestDelegate test = () => kernel.Calculate();

            // Assert
            var exception = Assert.Throws<UpliftVanKernelWrapperException>(test);
            CollectionAssert.AreEqual($"Index was out of range. Must be non-negative and less than the size of the collection.{Environment.NewLine}" +
                                      $"Parameter name: index{Environment.NewLine}" +
                                      $"Fatale fout in Uplift-Van berekening", exception.Message);
        }

        [Test]
        public void Calculate_CompleteInput_InputCorrectlySetToWrappedKernel()
        {
            // Setup
            var random = new Random(21);
            var soilModel = new SoilModel();
            var soilProfile2D = new SoilProfile2D();
            var stabilityLocation = new StabilityLocation();
            var surfaceLine = new SurfaceLine2();
            double maximumSliceWidth = random.NextDouble();
            var slipPlaneUpliftVan = new SlipPlaneUpliftVan();
            bool moveGrid = random.NextBoolean();
            bool gridAutomaticDetermined = random.NextBoolean();
            bool createZones = random.NextBoolean();
            bool automaticForbiddenZones = random.NextBoolean();
            double slipPlaneMinimumDepth = random.NextDouble();
            double slipPlaneMinimumLength = random.NextDouble();

            // Call
            var kernel = new UpliftVanKernelWrapper
            {
                SoilModel = soilModel,
                SoilProfile = soilProfile2D,
                Location = stabilityLocation,
                SurfaceLine = surfaceLine,
                MaximumSliceWidth = maximumSliceWidth,
                SlipPlaneUpliftVan = slipPlaneUpliftVan,
                MoveGrid = moveGrid,
                GridAutomaticDetermined = gridAutomaticDetermined,
                CreateZones = createZones,
                AutomaticForbiddenZones = automaticForbiddenZones,
                SlipPlaneMinimumDepth = slipPlaneMinimumDepth,
                SlipPlaneMinimumLength = slipPlaneMinimumLength
            };

            // Assert
            var stabilityModel = TypeUtils.GetField<StabilityModel>(kernel, "stabilityModel");

            Assert.IsNull(stabilityModel.LocationDaily);
            Assert.IsNotNull(stabilityModel.SlipPlaneConstraints);
            Assert.AreEqual(GridOrientation.Inwards, stabilityModel.GridOrientation);
            Assert.IsNotNull(stabilityModel.SlipCircle);
            Assert.AreEqual(SearchAlgorithm.Grid, stabilityModel.SearchAlgorithm);
            Assert.AreEqual(ModelOptions.UpliftVan, stabilityModel.ModelOption);
            Assert.AreSame(surfaceLine, stabilityModel.SurfaceLine2);
            Assert.AreSame(stabilityLocation, stabilityModel.Location);
            Assert.AreSame(soilModel, stabilityModel.SoilModel);
            Assert.AreSame(soilProfile2D, stabilityModel.SoilProfile);
            Assert.AreEqual(maximumSliceWidth, stabilityModel.MaximumSliceWidth);
            Assert.AreSame(slipPlaneUpliftVan, stabilityModel.SlipPlaneUpliftVan);
            Assert.AreEqual(moveGrid, stabilityModel.MoveGrid);
            Assert.AreEqual(gridAutomaticDetermined, stabilityModel.SlipCircle.Auto);
            Assert.AreEqual(createZones, stabilityModel.SlipPlaneConstraints.CreateZones);
            Assert.AreEqual(automaticForbiddenZones, stabilityModel.SlipPlaneConstraints.AutomaticForbiddenZones);
            Assert.AreEqual(slipPlaneMinimumDepth, stabilityModel.SlipPlaneConstraints.SlipPlaneMinDepth);
            Assert.AreEqual(slipPlaneMinimumLength, stabilityModel.SlipPlaneConstraints.SlipPlaneMinLength);

            AssertIrrelevantValues(stabilityModel);
            AssertAutomaticallySyncedValues(stabilityModel, soilProfile2D, surfaceLine);
        }

        private static void AssertIrrelevantValues(StabilityModel stabilityModel)
        {
            Assert.IsNaN(stabilityModel.SlipPlaneConstraints.XEntryMin); // Set during calculation
            Assert.IsNaN(stabilityModel.SlipPlaneConstraints.XEntryMax); // Set during calculation
            Assert.IsEmpty(stabilityModel.MultiplicationFactorsCPhiForUpliftList); // No multiplication factors CPhi for WBI
            Assert.IsEmpty(stabilityModel.UniformLoads); // No traffic load for WBI
            Assert.AreEqual(0.0, stabilityModel.FileVersionAsRead); // Set by XML serialization
            Assert.IsNull(stabilityModel.MinimumSafetyCurve); // Output
            Assert.IsFalse(stabilityModel.OnlyMinimumSafetyCurve); // Only for Bishop
            Assert.IsFalse(stabilityModel.AutoGenerateGeneticSpencer); // Only for Spencer
            Assert.AreEqual(SlipPlanePosition.High, stabilityModel.SlipPlanePosition); // Only for Spencer
            Assert.AreEqual(0.8, stabilityModel.RequiredForcePointsInSlices); // Only for Spencer
            Assert.AreEqual(60.0, stabilityModel.MaxAllowedAngleBetweenSlices); // Only for Spencer
            Assert.IsNotNull(stabilityModel.GeneticAlgorithmOptions); // Only for genetic search algorithm
            Assert.IsNotNull(stabilityModel.LevenbergMarquardtOptions); // Only for Levenberg Marquardt search algorithm
            Assert.AreEqual(ShearStrengthModel.CPhi, stabilityModel.DefaultShearStrengthModel); // Unused property
            Assert.AreEqual(50.0, stabilityModel.NumberOfGridMoves); // Only for Bishop
            Assert.IsEmpty(stabilityModel.ConsolidationMatrix.ConsolidationValues); // No consolidation for WBI
            Assert.IsNotNull(stabilityModel.ConsolidationLoad); // No consolidation for WBI
        }

        private static void AssertAutomaticallySyncedValues(StabilityModel stabilityModel, SoilProfile2D soilProfile2D, SurfaceLine2 surfaceLine)
        {
            Assert.AreSame(stabilityModel, stabilityModel.Location.StabilityModel);
            Assert.AreSame(soilProfile2D, stabilityModel.Location.SoilProfile2D);
            Assert.AreSame(surfaceLine, stabilityModel.Location.Surfaceline);
            Assert.IsTrue(stabilityModel.Location.Inwards);
            Assert.AreEqual(ModelOptions.UpliftVan, stabilityModel.SoilModel.ModelOption);
            Assert.AreSame(stabilityModel, stabilityModel.SlipCircle.StabilityModel);
            Assert.AreSame(soilProfile2D.Geometry, stabilityModel.GeometryData);
            Assert.IsNotNull(stabilityModel.GeotechnicsData);
            Assert.AreSame(soilProfile2D.Geometry, stabilityModel.GeotechnicsData.Geometry);
        }
    }
}