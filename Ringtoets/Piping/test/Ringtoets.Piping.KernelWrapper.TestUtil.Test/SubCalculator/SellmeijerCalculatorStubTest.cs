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
            var stub = new SellmeijerCalculatorStub();

            // Assert
            Assert.AreEqual(0, stub.BeddingAngle);
            Assert.AreEqual(0, stub.D70);
            Assert.AreEqual(0, stub.D70Mean);
            Assert.AreEqual(0, stub.DAquifer);
            Assert.AreEqual(0, stub.DTotal);
            Assert.AreEqual(0, stub.DarcyPermeability);
            Assert.AreEqual(0, stub.GammaSubParticles);
            Assert.AreEqual(0, stub.Gravity);
            Assert.AreEqual(0, stub.HExit);
            Assert.AreEqual(0, stub.HRiver);
            Assert.AreEqual(0, stub.KinematicViscosityWater);
            Assert.AreEqual(0, stub.ModelFactorPiping);
            Assert.AreEqual(0, stub.Rc);
            Assert.AreEqual(0, stub.SeepageLength);
            Assert.AreEqual(0, stub.VolumetricWeightOfWater);
            Assert.AreEqual(0, stub.WhitesDragCoefficient);
            Assert.AreEqual(0, stub.BottomLevelAquitardAboveExitPointZ);

            Assert.AreEqual(0, stub.CreepCoefficient);
            Assert.AreEqual(0, stub.CriticalFall);
            Assert.AreEqual(0, stub.ReducedFall);
            Assert.AreEqual(0, stub.FoSp);
            Assert.AreEqual(0, stub.Zp);
        }

        [Test]
        public void Validate_Always_EmptyListValidatedTrue()
        {
            // Setup
            var stub = new SellmeijerCalculatorStub();

            // Call
            List<string> result = stub.Validate();

            // Assert
            CollectionAssert.IsEmpty(result);
            Assert.IsTrue(stub.Validated);
        }

        [Test]
        public void Calculate_Always_CalculatedTrue()
        {
            // Setup
            var stub = new SellmeijerCalculatorStub();

            // Call
            stub.Calculate();

            // Assert
            Assert.IsTrue(stub.Calculated);
        }
    }
}