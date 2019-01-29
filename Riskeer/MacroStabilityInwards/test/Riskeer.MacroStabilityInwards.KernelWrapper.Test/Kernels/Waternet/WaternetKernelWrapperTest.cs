// Copyright (C) Stichting Deltares 2018. All rights reserved.
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

using Core.Common.Util.Reflection;
using Deltares.WTIStability;
using Deltares.WTIStability.Calculation.Wrapper;
using Deltares.WTIStability.Data.Geo;
using NUnit.Framework;
using Riskeer.MacroStabilityInwards.KernelWrapper.Kernels.Waternet;
using WtiStabilityWaternet = Deltares.WTIStability.Data.Geo.Waternet;

namespace Riskeer.MacroStabilityInwards.KernelWrapper.Test.Kernels.Waternet
{
    [TestFixture]
    public class WaternetKernelWrapperTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var kernel = new TestWaternetKernelWrapper();

            // Assert
            Assert.IsInstanceOf<IWaternetKernel>(kernel);
        }

        [Test]
        public void Constructor_CompleteInput_InputCorrectlySetToWrappedKernel()
        {
            // Setup
            var stabilityLocation = new StabilityLocation();
            var soilModel = new SoilModel();
            var soilProfile2D = new SoilProfile2D();
            var surfaceLine = new SurfaceLine2();

            // Call
            var kernel = new TestWaternetKernelWrapper
            {
                Location = stabilityLocation,
                SoilModel = soilModel,
                SoilProfile = soilProfile2D,
                SurfaceLine = surfaceLine
            };

            // Assert
            var stabilityModel = TypeUtils.GetProperty<StabilityModel>(kernel, "StabilityModel");

            Assert.AreSame(stabilityLocation, stabilityModel.Location);
            Assert.AreSame(surfaceLine, stabilityModel.SurfaceLine2);
            Assert.AreSame(soilModel, stabilityModel.SoilModel);
            Assert.AreSame(soilProfile2D, stabilityModel.SoilProfile);

            AssertIrrelevantValues(stabilityModel);
            AssertAutomaticallySyncedValues(stabilityModel, soilProfile2D, surfaceLine);
        }

        [Test]
        public void Calculate_ExceptionInWrappedKernel_ThrowsWaternetKernelWrapperExceptionAndWaternetNotSet()
        {
            // Setup
            var kernel = new TestWaternetKernelWrapper();

            // Call
            TestDelegate test = () => kernel.Calculate();

            // Assert
            var exception = Assert.Throws<WaternetKernelWrapperException>(test);
            Assert.IsNotNull(exception.InnerException);
            Assert.AreEqual(exception.InnerException.Message, exception.Message);
            Assert.IsNull(kernel.Waternet);
        }

        private static void AssertIrrelevantValues(StabilityModel stabilityModel)
        {
            Assert.IsNaN(stabilityModel.SlipPlaneConstraints.XEntryMin); // Not applicable for Waternet calculation
            Assert.IsNaN(stabilityModel.SlipPlaneConstraints.XEntryMax); // Not applicable for Waternet calculation
            Assert.IsEmpty(stabilityModel.MultiplicationFactorsCPhiForUpliftList); // Not applicable for Waternet calculation
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
            Assert.AreEqual(ModelOptions.Bishop, stabilityModel.ModelOption); // Not applicable for Waternet calculation
            Assert.AreEqual(ModelOptions.Bishop, stabilityModel.SoilModel.ModelOption); // Not applicable for Waternet calculation
            Assert.IsNotNull(stabilityModel.SlipPlaneUpliftVan); // Not applicable for Waternet calculation
            Assert.IsNotNull(stabilityModel.SlipCircle); // Not applicable for Waternet calculation
        }

        private static void AssertAutomaticallySyncedValues(StabilityModel stabilityModel, SoilProfile2D soilProfile2D, SurfaceLine2 surfaceLine)
        {
            Assert.AreSame(stabilityModel, stabilityModel.Location.StabilityModel);
            Assert.IsTrue(stabilityModel.Location.Inwards);
            Assert.AreSame(soilProfile2D, stabilityModel.Location.SoilProfile2D);
            Assert.AreSame(surfaceLine, stabilityModel.Location.Surfaceline);
            Assert.IsTrue(stabilityModel.Location.Inwards);
            Assert.AreSame(soilProfile2D.Geometry, stabilityModel.GeometryData);
            Assert.IsNotNull(stabilityModel.GeotechnicsData);
            Assert.AreSame(soilProfile2D.Geometry, stabilityModel.GeotechnicsData.Geometry);
        }

        private class TestWaternetKernelWrapper : WaternetKernelWrapper
        {
            public override StabilityLocation Location
            {
                set
                {
                    StabilityModel.Location = value;
                }
            }

            protected override string CreateWaternetXmlResult(WTIStabilityCalculation waternetCalculation)
            {
                return null;
            }

            protected override WtiStabilityWaternet ReadResult(string waternetXmlResult)
            {
                return null;
            }
        }
    }
}