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

using System.Collections.Generic;
using NUnit.Framework;
using Ringtoets.Piping.KernelWrapper.TestUtil.SubCalculator;

namespace Ringtoets.Piping.KernelWrapper.TestUtil.Test.SubCalculator
{
    [TestFixture]
    public class SellmeijerCalculatorStubTest
    {
        [Test]
        public void DefaultConstructor_PropertiesSet()
        {
            // Call
            var sellmeijerCalculator = new SellmeijerCalculatorStub();

            // Assert
            Assert.AreEqual(0, sellmeijerCalculator.BeddingAngle);
            Assert.AreEqual(0, sellmeijerCalculator.D70);
            Assert.AreEqual(0, sellmeijerCalculator.D70Mean);
            Assert.AreEqual(0, sellmeijerCalculator.DAquifer);
            Assert.AreEqual(0, sellmeijerCalculator.DTotal);
            Assert.AreEqual(0, sellmeijerCalculator.DarcyPermeability);
            Assert.AreEqual(0, sellmeijerCalculator.GammaSubParticles);
            Assert.AreEqual(0, sellmeijerCalculator.Gravity);
            Assert.AreEqual(0, sellmeijerCalculator.HExit);
            Assert.AreEqual(0, sellmeijerCalculator.HRiver);
            Assert.AreEqual(0, sellmeijerCalculator.KinematicViscosityWater);
            Assert.AreEqual(0, sellmeijerCalculator.ModelFactorPiping);
            Assert.AreEqual(0, sellmeijerCalculator.Rc);
            Assert.AreEqual(0, sellmeijerCalculator.SeepageLength);
            Assert.AreEqual(0, sellmeijerCalculator.VolumetricWeightOfWater);
            Assert.AreEqual(0, sellmeijerCalculator.WhitesDragCoefficient);
            Assert.AreEqual(0, sellmeijerCalculator.BottomLevelAquitardAboveExitPointZ);

            Assert.AreEqual(0, sellmeijerCalculator.CreepCoefficient);
            Assert.AreEqual(0, sellmeijerCalculator.CriticalFall);
            Assert.AreEqual(0, sellmeijerCalculator.ReducedFall);
            Assert.AreEqual(0, sellmeijerCalculator.FoSp);
            Assert.AreEqual(0, sellmeijerCalculator.Zp);
        }

        [Test]
        public void Validate_Always_EmptyListValidatedTrue()
        {
            // Setup
            var sellmeijerCalculator = new SellmeijerCalculatorStub();

            // Call
            List<string> result = sellmeijerCalculator.Validate();

            // Assert
            CollectionAssert.IsEmpty(result);
            Assert.IsTrue(sellmeijerCalculator.Validated);
        }

        [Test]
        public void Calculate_Always_CalculatedTrue()
        {
            // Setup
            var sellmeijerCalculator = new SellmeijerCalculatorStub();

            // Call
            sellmeijerCalculator.Calculate();

            // Assert
            Assert.IsTrue(sellmeijerCalculator.Calculated);
        }
    }
}