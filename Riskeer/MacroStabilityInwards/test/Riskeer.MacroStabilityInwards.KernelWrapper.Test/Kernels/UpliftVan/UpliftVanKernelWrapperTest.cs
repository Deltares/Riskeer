﻿// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using System.Xml.Schema;
using Core.Common.TestUtil;
using Core.Common.Util.Reflection;
using Deltares.MacroStability.Data;
using Deltares.MacroStability.Geometry;
using Deltares.MacroStability.Standard;
using Deltares.MacroStability.WaternetCreator;
using NUnit.Framework;
using Riskeer.MacroStabilityInwards.KernelWrapper.Kernels.UpliftVan;

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
            var stabilityLocationExtreme = new Location();
            var stabilityLocationDaily = new Location();
            var surfaceLine = new SurfaceLine2();
            double maximumSliceWidth = random.NextDouble();
            var slipPlaneUpliftVan = new SlipPlaneUpliftVan();
            bool moveGrid = random.NextBoolean();
            bool gridAutomaticDetermined = random.NextBoolean();
            var slipPlaneConstraints = new SlipPlaneConstraints();

            // Call
            var kernel = new UpliftVanKernelWrapper();
            kernel.SetSoilProfile(soilProfile2D);
            kernel.SetMaximumSliceWidth(maximumSliceWidth);
            kernel.SetSlipPlaneUpliftVan(slipPlaneUpliftVan);
            kernel.SetMoveGrid(moveGrid);
            kernel.SetGridAutomaticDetermined(gridAutomaticDetermined);
            kernel.SetSlipPlaneConstraints(slipPlaneConstraints);

            // Assert
            var stabilityModel = TypeUtils.GetField<StabilityModel>(kernel, "stabilityModel");
            var location = TypeUtils.GetField<Location>(kernel, "location");

            Assert.IsNotNull(stabilityModel.SlipPlaneConstraints);
            Assert.AreEqual(GridOrientation.Inwards, stabilityModel.GridOrientation);
            Assert.IsNotNull(stabilityModel.SlipCircle);
            Assert.AreEqual(SearchAlgorithm.Grid, stabilityModel.SearchAlgorithm);
            Assert.AreEqual(ModelOptions.UpliftVan, stabilityModel.ModelOption);
            ConstructionStage firstConstructionStage = stabilityModel.ConstructionStages.First();
            Assert.IsNotNull(firstConstructionStage.GeotechnicsData.CurrentWaternet);
            Assert.AreEqual("WaternetDaily", firstConstructionStage.GeotechnicsData.CurrentWaternet.Name);
            Assert.AreSame(surfaceLine, location.Surfaceline);
            Assert.AreSame(stabilityLocationExtreme, location);
            Assert.AreSame(stabilityLocationDaily, location);
            Assert.AreSame(soilProfile2D, location.SoilProfile2D);
            Assert.AreEqual(maximumSliceWidth, stabilityModel.MaximumSliceWidth);
            Assert.AreSame(slipPlaneUpliftVan, stabilityModel.SlipPlaneUpliftVan);
            Assert.AreSame(slipPlaneConstraints, stabilityModel.SlipPlaneConstraints);
            Assert.AreEqual(moveGrid, stabilityModel.MoveGrid);

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
        public void Calculate_ErrorInValidation_SetsCalculationMessages()
        {
            // Setup
            UpliftVanKernelWrapper kernel = CreateInvalidKernel(new Soil());

            // Call
            kernel.Calculate();

            // Assert
            LogMessage[] messages = kernel.CalculationMessages.ToArray();
            Assert.AreEqual(2, messages.Length);

            LogMessage firstMessage = messages.ElementAt(0);
            Assert.AreEqual(LogMessageType.Error, firstMessage.MessageType);
            Assert.AreEqual($"Index was out of range. Must be non-negative and less than the size of the collection.{Environment.NewLine}" +
                            "Parameter name: index", firstMessage.Message);

            LogMessage secondMessage = messages.ElementAt(1);
            Assert.AreEqual(LogMessageType.Error, secondMessage.MessageType);
            Assert.AreEqual("Fatale fout in Uplift-Van berekening", secondMessage.Message);
        }

        [Test]
        public void Validate_InputNotComplete_ThrowsUpliftVanKernelWrapperException()
        {
            // Setup
            var kernel = new UpliftVanKernelWrapper();

            // Call
            TestDelegate test = () => kernel.Validate();

            // Assert
            var exception = Assert.Throws<UpliftVanKernelWrapperException>(test);
            Assert.IsInstanceOf<XmlSchemaValidationException>(exception.InnerException);
            Assert.AreEqual(exception.InnerException.Message, exception.Message);
        }

        [Test]
        public void Validate_InvalidInput_ThrowsUpliftVanKernelWrapperException()
        {
            // Setup
            UpliftVanKernelWrapper kernel = CreateInvalidKernel(null);

            // Call
            TestDelegate test = () => kernel.Validate();

            // Assert
            var exception = Assert.Throws<UpliftVanKernelWrapperException>(test);
            Assert.IsInstanceOf<ArgumentNullException>(exception.InnerException);
            Assert.AreEqual(exception.InnerException.Message, exception.Message);
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