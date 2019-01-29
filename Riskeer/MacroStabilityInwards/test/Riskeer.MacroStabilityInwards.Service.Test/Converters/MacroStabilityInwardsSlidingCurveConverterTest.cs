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

using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.UpliftVan.Output;
using Ringtoets.MacroStabilityInwards.KernelWrapper.TestUtil.Calculators.UpliftVan.Output;
using Ringtoets.MacroStabilityInwards.Service.Converters;

namespace Riskeer.MacroStabilityInwards.Service.Test.Converters
{
    [TestFixture]
    public class MacroStabilityInwardsSlidingCurveConverterTest
    {
        [Test]
        public void Convert_ResultNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => MacroStabilityInwardsSlidingCurveConverter.Convert(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("result", exception.ParamName);
        }

        [Test]
        public void Convert_WithResult_ReturnConvertedSlidingCurve()
        {
            // Setup
            UpliftVanSlidingCurveResult result = UpliftVanSlidingCurveResultTestFactory.Create();

            // Call
            MacroStabilityInwardsSlidingCurve output = MacroStabilityInwardsSlidingCurveConverter.Convert(result);

            // Assert
            Assert.AreEqual(result.IteratedHorizontalForce, output.IteratedHorizontalForce);
            Assert.AreEqual(result.NonIteratedHorizontalForce, output.NonIteratedHorizontalForce);
            AssertCircle(result.LeftCircle, output.LeftCircle);
            AssertCircle(result.RightCircle, output.RightCircle);
            AssertSlices(result.Slices, output.Slices);
        }

        private static void AssertCircle(UpliftVanSlidingCircleResult circleResult, MacroStabilityInwardsSlidingCircle circleOutput)
        {
            Assert.AreEqual(circleResult.Center, circleOutput.Center);
            Assert.AreEqual(circleResult.IsActive, circleOutput.IsActive);
            Assert.AreEqual(circleResult.Radius, circleOutput.Radius);
            Assert.AreEqual(circleResult.DrivingMoment, circleOutput.DrivingMoment);
            Assert.AreEqual(circleResult.ResistingMoment, circleOutput.ResistingMoment);
            Assert.AreEqual(circleResult.IteratedForce, circleOutput.IteratedForce);
            Assert.AreEqual(circleResult.NonIteratedForce, circleOutput.NonIteratedForce);
        }

        private static void AssertSlices(IEnumerable<UpliftVanSliceResult> resultSlices, IEnumerable<MacroStabilityInwardsSlice> outputSlices)
        {
            UpliftVanSliceResult[] expectedSlices = resultSlices.ToArray();
            MacroStabilityInwardsSlice[] actualSlices = outputSlices.ToArray();

            Assert.AreEqual(expectedSlices.Length, actualSlices.Length);

            for (var i = 0; i < expectedSlices.Length; i++)
            {
                Assert.AreEqual(expectedSlices[i].Cohesion, actualSlices[i].Cohesion);
                Assert.AreEqual(expectedSlices[i].FrictionAngle, actualSlices[i].FrictionAngle);
                Assert.AreEqual(expectedSlices[i].CriticalPressure, actualSlices[i].CriticalPressure);
                Assert.AreEqual(expectedSlices[i].OverConsolidationRatio, actualSlices[i].OverConsolidationRatio);
                Assert.AreEqual(expectedSlices[i].Pop, actualSlices[i].Pop);
                Assert.AreEqual(expectedSlices[i].DegreeOfConsolidationPorePressureSoil, actualSlices[i].DegreeOfConsolidationPorePressureSoil);
                Assert.AreEqual(expectedSlices[i].DegreeOfConsolidationPorePressureLoad, actualSlices[i].DegreeOfConsolidationPorePressureLoad);
                Assert.AreEqual(expectedSlices[i].Dilatancy, actualSlices[i].Dilatancy);
                Assert.AreEqual(expectedSlices[i].ExternalLoad, actualSlices[i].ExternalLoad);
                Assert.AreEqual(expectedSlices[i].HydrostaticPorePressure, actualSlices[i].HydrostaticPorePressure);
                Assert.AreEqual(expectedSlices[i].LeftForce, actualSlices[i].LeftForce);
                Assert.AreEqual(expectedSlices[i].LeftForceAngle, actualSlices[i].LeftForceAngle);
                Assert.AreEqual(expectedSlices[i].LeftForceY, actualSlices[i].LeftForceY);
                Assert.AreEqual(expectedSlices[i].RightForce, actualSlices[i].RightForce);
                Assert.AreEqual(expectedSlices[i].RightForceAngle, actualSlices[i].RightForceAngle);
                Assert.AreEqual(expectedSlices[i].RightForceY, actualSlices[i].RightForceY);
                Assert.AreEqual(expectedSlices[i].LoadStress, actualSlices[i].LoadStress);
                Assert.AreEqual(expectedSlices[i].NormalStress, actualSlices[i].NormalStress);
                Assert.AreEqual(expectedSlices[i].PorePressure, actualSlices[i].PorePressure);
                Assert.AreEqual(expectedSlices[i].HorizontalPorePressure, actualSlices[i].HorizontalPorePressure);
                Assert.AreEqual(expectedSlices[i].VerticalPorePressure, actualSlices[i].VerticalPorePressure);
                Assert.AreEqual(expectedSlices[i].PiezometricPorePressure, actualSlices[i].PiezometricPorePressure);
                Assert.AreEqual(expectedSlices[i].EffectiveStress, actualSlices[i].EffectiveStress);
                Assert.AreEqual(expectedSlices[i].EffectiveStressDaily, actualSlices[i].EffectiveStressDaily);
                Assert.AreEqual(expectedSlices[i].ExcessPorePressure, actualSlices[i].ExcessPorePressure);
                Assert.AreEqual(expectedSlices[i].ShearStress, actualSlices[i].ShearStress);
                Assert.AreEqual(expectedSlices[i].SoilStress, actualSlices[i].SoilStress);
                Assert.AreEqual(expectedSlices[i].TotalPorePressure, actualSlices[i].TotalPorePressure);
                Assert.AreEqual(expectedSlices[i].TotalStress, actualSlices[i].TotalStress);
                Assert.AreEqual(expectedSlices[i].Weight, actualSlices[i].Weight);
            }
        }
    }
}