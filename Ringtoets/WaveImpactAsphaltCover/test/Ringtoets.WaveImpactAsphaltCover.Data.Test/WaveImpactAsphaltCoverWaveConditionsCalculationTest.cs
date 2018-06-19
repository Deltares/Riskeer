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
using Core.Common.Data.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Revetment.Data;
using Ringtoets.WaveImpactAsphaltCover.Data.TestUtil;

namespace Ringtoets.WaveImpactAsphaltCover.Data.Test
{
    [TestFixture]
    public class WaveImpactAsphaltCoverWaveConditionsCalculationTest
    {
        [Test]
        public void Constructor_DefaultValues()
        {
            // Call
            var calculation = new WaveImpactAsphaltCoverWaveConditionsCalculation();

            // Assert
            Assert.IsInstanceOf<ICalculation<WaveConditionsInput>>(calculation);
            Assert.IsInstanceOf<CloneableObservable>(calculation);

            Assert.AreEqual("Nieuwe berekening", calculation.Name);
            Assert.IsNotNull(calculation.InputParameters);
            Assert.IsNull(calculation.Comments.Body);
            Assert.IsNull(calculation.Output);
            Assert.IsNull(calculation.InputParameters.ForeshoreProfile);
        }

        [Test]
        public void ClearOutput_Always_SetsOutputToNull()
        {
            // Setup
            var calculation = new WaveImpactAsphaltCoverWaveConditionsCalculation
            {
                Output = new WaveImpactAsphaltCoverWaveConditionsOutput(Enumerable.Empty<WaveConditionsOutput>())
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
            var calculation = new WaveImpactAsphaltCoverWaveConditionsCalculation
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
            var calculation = new WaveImpactAsphaltCoverWaveConditionsCalculation
            {
                Output = new WaveImpactAsphaltCoverWaveConditionsOutput(Enumerable.Empty<WaveConditionsOutput>())
            };

            // Call
            bool hasOutput = calculation.HasOutput;

            // Assert
            Assert.IsTrue(hasOutput);
        }

        [Test]
        public void ShouldCalculate_OutputNull_ReturnsTrue()
        {
            // Setup
            var calculation = new WaveImpactAsphaltCoverWaveConditionsCalculation
            {
                Output = null
            };

            // Call
            bool shouldCalculate = calculation.ShouldCalculate;

            // Assert
            Assert.IsTrue(shouldCalculate);
        }

        [Test]
        public void ShouldCalculate_OutputSet_ReturnsFalse()
        {
            // Setup
            var calculation = new WaveImpactAsphaltCoverWaveConditionsCalculation
            {
                Output = new WaveImpactAsphaltCoverWaveConditionsOutput(Enumerable.Empty<WaveConditionsOutput>())
            };

            // Call
            bool shouldCalculate = calculation.ShouldCalculate;

            // Assert
            Assert.IsFalse(shouldCalculate);
        }

        [Test]
        public void Clone_AllPropertiesSet_ReturnNewInstanceWithCopiedValues()
        {
            // Setup
            WaveImpactAsphaltCoverWaveConditionsCalculation original = WaveImpactAsphaltCoverTestDataGenerator.GetRandomWaveImpactAsphaltCoverWaveConditionsCalculation();

            original.Output = WaveImpactAsphaltCoverTestDataGenerator.GetRandomWaveImpactAsphaltCoverWaveConditionsOutput();

            // Call
            object clone = original.Clone();

            // Assert
            CoreCloneAssert.AreObjectClones(original, clone, WaveImpactAsphaltCoverCloneAssert.AreClones);
        }

        [Test]
        public void Clone_NotAllPropertiesSet_ReturnNewInstanceWithCopiedValues()
        {
            // Setup
            var original = new WaveImpactAsphaltCoverWaveConditionsCalculation();

            // Call
            object clone = original.Clone();

            // Assert
            CoreCloneAssert.AreObjectClones(original, clone, WaveImpactAsphaltCoverCloneAssert.AreClones);
        }
    }
}