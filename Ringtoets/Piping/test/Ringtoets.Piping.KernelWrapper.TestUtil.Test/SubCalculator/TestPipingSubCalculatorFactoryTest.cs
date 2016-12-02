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

using NUnit.Framework;
using Ringtoets.Piping.KernelWrapper.SubCalculator;
using Ringtoets.Piping.KernelWrapper.TestUtil.SubCalculator;

namespace Ringtoets.Piping.KernelWrapper.TestUtil.Test.SubCalculator
{
    [TestFixture]
    public class TestPipingSubCalculatorFactoryTest
    {
        [Test]
        public void DefaultConstructor_SetDefaultProperties()
        {
            // Call
            var factory = new TestPipingSubCalculatorFactory();

            // Assert
            Assert.IsInstanceOf<IPipingSubCalculatorFactory>(factory);
            Assert.IsNull(factory.LastCreatedEffectiveThicknessCalculator);
            Assert.IsNull(factory.LastCreatedUpliftCalculator);
            Assert.IsNull(factory.LastCreatedSellmeijerCalculator);
            Assert.IsNull(factory.LastCreatedHeaveCalculator);
        }

        [Test]
        public void CreateEffectiveThicknessCalculator_Always_LastCreatedEffectiveThicknessCalculatorSetToReturnValue()
        {
            // Setup
            var factory = new TestPipingSubCalculatorFactory();

            // Call
            var subCalculator = factory.CreateEffectiveThicknessCalculator();

            // Assert
            Assert.AreSame(factory.LastCreatedEffectiveThicknessCalculator, subCalculator);
        }

        [Test]
        public void CreateUpliftCalculator_Always_LastCreatedUpliftCalculatorSetToReturnValue()
        {
            // Setup
            var factory = new TestPipingSubCalculatorFactory();

            // Call
            var subCalculator = factory.CreateUpliftCalculator();

            // Assert
            Assert.AreSame(factory.LastCreatedUpliftCalculator, subCalculator);
        }

        [Test]
        public void CreateHeaveCalculator_Always_LastCreatedHeaveCalculatorSetToReturnValue()
        {
            // Setup
            var factory = new TestPipingSubCalculatorFactory();

            // Call
            var subCalculator = factory.CreateHeaveCalculator();

            // Assert
            Assert.AreSame(factory.LastCreatedHeaveCalculator, subCalculator);
        }

        [Test]
        public void CreateSellmeijerCalculator_Always_LastCreatedSellmeijerCalculatorSetToReturnValue()
        {
            // Setup
            var factory = new TestPipingSubCalculatorFactory();

            // Call
            var subCalculator = factory.CreateSellmeijerCalculator();

            // Assert
            Assert.AreSame(factory.LastCreatedSellmeijerCalculator, subCalculator);
        }

        [Test]
        public void CreatePiezometricHeadAtExitCalculator_Always_LastCreatedPiezometricHeadAtExitCalculatorSetToReturnValue()
        {
            // Setup
            var factory = new TestPipingSubCalculatorFactory();

            // Call
            var subCalculator = factory.CreatePiezometricHeadAtExitCalculator();

            // Assert
            Assert.AreSame(factory.LastCreatedPiezometricHeadAtExitCalculator, subCalculator);
        }

        [Test]
        public void CreatePipingProfilePropertyCalculator_Always_LastCreatedPipingProfilePropertyCalculatorSetToReturnValue()
        {
            // Setup
            var factory = new TestPipingSubCalculatorFactory();

            // Call
            var subCalculator = factory.CreatePipingProfilePropertyCalculator();

            // Assert
            Assert.AreSame(factory.LastCreatedPipingProfilePropertyCalculator, subCalculator);
        }
    }
}