﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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

using System.Linq;
using Core.Common.Base;
using NUnit.Framework;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Revetment.Data;
using RingtoetsCommonDataResources = Ringtoets.Common.Data.Properties.Resources;

namespace Ringtoets.GrassCoverErosionOutwards.Data.Test
{
    [TestFixture]
    public class GrassCoverErosionOutwardsWaveConditionsCalculationTest
    {
        [Test]
        public void Constructor_DefaultValues()
        {
            // Call
            var calculation = new GrassCoverErosionOutwardsWaveConditionsCalculation();

            // Assert
            Assert.IsInstanceOf<ICalculation<WaveConditionsInput>>(calculation);
            Assert.IsInstanceOf<Observable>(calculation);

            Assert.AreEqual(RingtoetsCommonDataResources.Calculation_DefaultName, calculation.Name);
            Assert.IsNotNull(calculation.InputParameters);
            Assert.IsFalse(calculation.HasOutput);
            Assert.IsNull(calculation.Comments.Body);
            Assert.IsNull(calculation.Output);
            Assert.IsNull(calculation.InputParameters.ForeshoreProfile);
        }

        [Test]
        public void ClearOutput_Always_SetsOutputToNull()
        {
            // Setup
            var calculation = new GrassCoverErosionOutwardsWaveConditionsCalculation
            {
                Output = new GrassCoverErosionOutwardsWaveConditionsOutput(Enumerable.Empty<WaveConditionsOutput>())
            };

            // Precondition
            Assert.IsNotNull(calculation.Output);

            // Call
            calculation.ClearOutput();

            // Assert
            Assert.IsNull(calculation.Output);
        }

        [Test]
        public void HasOutput_OutputNull_ReturnsFalse()
        {
            // Setup
            var calculation = new GrassCoverErosionOutwardsWaveConditionsCalculation
            {
                Output = null
            };

            // Call
            bool hasOutput = calculation.HasOutput;

            // Assert
            Assert.IsFalse(hasOutput);
        }

        [Test]
        public void HasOutput_OutputSet_ReturnsTrue()
        {
            // Setup
            var calculation = new GrassCoverErosionOutwardsWaveConditionsCalculation
            {
                Output = new GrassCoverErosionOutwardsWaveConditionsOutput(Enumerable.Empty<WaveConditionsOutput>())
            };

            // Call
            bool hasOutput = calculation.HasOutput;

            // Assert
            Assert.IsTrue(hasOutput);
        }
    }
}