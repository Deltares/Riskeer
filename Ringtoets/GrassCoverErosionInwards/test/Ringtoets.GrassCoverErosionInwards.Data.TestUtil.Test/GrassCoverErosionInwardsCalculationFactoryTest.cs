// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.Data;
using NUnit.Framework;
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.HydraRing.Data;

namespace Ringtoets.GrassCoverErosionInwards.Data.TestUtil.Test
{
    [TestFixture]
    public class GrassCoverErosionInwardsCalculationFactoryTest
    {
        [Test]
        public void CreateCalculationWithInvalidData_ReturnsExpectedValues()
        {
            // Call
            var calculation = GrassCoverErosionInwardsCalculationFactory.CreateCalculationWithInvalidData();

            // Assert
            var emptyCalculation = new GrassCoverErosionInwardsCalculation(new GeneralGrassCoverErosionInwardsInput(),
                                                                           new NormProbabilityGrassCoverErosionInwardsInput());
            AssertGrassCoverErosionInwardsCalculationAreEqual(emptyCalculation, calculation);
        }

        [Test]
        public void CreateCalculationWithValidData_ReturnsExpectedValues()
        {
            // Call
            var calculation = GrassCoverErosionInwardsCalculationFactory.CreateCalculationWithValidInput();

            // Assert
            HydraulicBoundaryLocation hydraulicBoundaryLocation = new HydraulicBoundaryLocation(0, string.Empty, 0.0, 0.0)
            {
                DesignWaterLevel = (RoundedDouble) 1.0
            };

            var validCalculation = new GrassCoverErosionInwardsCalculation(new GeneralGrassCoverErosionInwardsInput(),
                                                                           new NormProbabilityGrassCoverErosionInwardsInput())
            {
                InputParameters =
                {
                    Orientation = (RoundedDouble) 1.0,
                    CriticalFlowRate = new LognormalDistribution(1),
                    DikeHeight = (RoundedDouble) 1.0,
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                    UseBreakWater = false,
                    UseForeshore = false
                }
            };

            AssertGrassCoverErosionInwardsCalculationAreEqual(validCalculation, calculation);
        }

        private static void AssertGrassCoverErosionInwardsCalculationAreEqual(GrassCoverErosionInwardsCalculation a, GrassCoverErosionInwardsCalculation b)
        {
            Assert.AreEqual(a.Comments, b.Comments);
            Assert.AreEqual(a.HasOutput, b.HasOutput);
            AssertGrassCoverErosionInwardsInputAreEqual(a.InputParameters, b.InputParameters);
            Assert.AreEqual(a.Name, b.Name);
        }

        private static void AssertGrassCoverErosionInwardsInputAreEqual(GrassCoverErosionInwardsInput a, GrassCoverErosionInwardsInput b)
        {
            Assert.AreEqual(a.BreakWater.Height, b.BreakWater.Height);
            Assert.AreEqual(a.BreakWater.Type, b.BreakWater.Type);
            Assert.AreEqual(a.CriticalFlowRate.Mean, b.CriticalFlowRate.Mean);
            Assert.AreEqual(a.CriticalFlowRate.StandardDeviation, b.CriticalFlowRate.StandardDeviation);
            Assert.AreEqual(a.CriticalOvertoppingModelFactor, b.CriticalOvertoppingModelFactor);
            CollectionAssertAreEqual(a.DikeGeometry.ToList(), b.DikeGeometry.ToList());
            Assert.AreEqual(a.DikeHeight, b.DikeHeight);
            Assert.AreEqual(a.FbFactor.Mean, b.FbFactor.Mean);
            Assert.AreEqual(a.FbFactor.StandardDeviation, b.FbFactor.StandardDeviation);
            Assert.AreEqual(a.FnFactor.Mean, b.FnFactor.Mean);
            Assert.AreEqual(a.FnFactor.StandardDeviation, b.FnFactor.StandardDeviation);
            Assert.AreEqual(a.FrunupModelFactor.Mean, b.FrunupModelFactor.Mean);
            Assert.AreEqual(a.FrunupModelFactor.StandardDeviation, b.FrunupModelFactor.StandardDeviation);
            Assert.AreEqual(a.FshallowModelFactor.Mean, b.FshallowModelFactor.Mean);
            Assert.AreEqual(a.FshallowModelFactor.StandardDeviation, b.FshallowModelFactor.StandardDeviation);
            CollectionAssertAreEqual(a.ForeshoreGeometry.ToList(), b.ForeshoreGeometry.ToList());
            Assert.AreEqual(a.Orientation, b.Orientation);
            Assert.AreEqual(a.OvertoppingModelFactor, b.OvertoppingModelFactor);
            Assert.AreEqual(a.UseBreakWater, b.UseBreakWater);
            Assert.AreEqual(a.UseForeshore, b.UseForeshore);
        }

        private static void CollectionAssertAreEqual(IList<ProfileSection> a, IList<ProfileSection> b)
        {
            Assert.AreEqual(a.Count, b.Count);
            for (var i = 0; i < a.Count; i++)
            {
                Assert.AreEqual(a[i].EndingPoint, b[i].EndingPoint);
                Assert.AreEqual(a[i].StartingPoint, b[i].StartingPoint);
            }
        }

        private static void CollectionAssertAreEqual(IList<RoughnessProfileSection> a, IList<RoughnessProfileSection> b)
        {
            Assert.AreEqual(a.Count, b.Count);
            for (var i = 0; i < a.Count; i++)
            {
                Assert.AreEqual(a[i].EndingPoint, b[i].EndingPoint);
                Assert.AreEqual(a[i].StartingPoint, b[i].StartingPoint);
                Assert.AreEqual(a[i].Roughness, b[i].Roughness);
            }
        }
    }
}