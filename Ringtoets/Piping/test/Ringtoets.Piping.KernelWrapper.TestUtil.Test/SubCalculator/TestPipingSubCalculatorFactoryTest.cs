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
            Assert.NotNull(factory.LastCreatedEffectiveThicknessCalculator);
            Assert.NotNull(factory.LastCreatedUpliftCalculator);
            Assert.NotNull(factory.LastCreatedSellmeijerCalculator);
            Assert.NotNull(factory.LastCreatedHeaveCalculator);
            Assert.NotNull(factory.LastCreatedPiezometricHeadAtExitCalculator);
            Assert.NotNull(factory.LastCreatedPipingProfilePropertyCalculator);
        }

        [Test]
        public void CreateEffectiveThicknessCalculator_Always_ReturnLastCreatedEffectiveThicknessCalculator()
        {
            // Setup
            var factory = new TestPipingSubCalculatorFactory();

            // Call
            IEffectiveThicknessCalculator subCalculator = factory.CreateEffectiveThicknessCalculator();

            // Assert
            Assert.AreSame(factory.LastCreatedEffectiveThicknessCalculator, subCalculator);
        }

        [Test]
        public void CreateUpliftCalculator_Always_ReturnLastCreatedUpliftCalculator()
        {
            // Setup
            var factory = new TestPipingSubCalculatorFactory();

            // Call
            IUpliftCalculator subCalculator = factory.CreateUpliftCalculator();

            // Assert
            Assert.AreSame(factory.LastCreatedUpliftCalculator, subCalculator);
        }

        [Test]
        public void CreateHeaveCalculator_Always_ReturnLastCreatedHeaveCalculator()
        {
            // Setup
            var factory = new TestPipingSubCalculatorFactory();

            // Call
            IHeaveCalculator subCalculator = factory.CreateHeaveCalculator();

            // Assert
            Assert.AreSame(factory.LastCreatedHeaveCalculator, subCalculator);
        }

        [Test]
        public void CreateSellmeijerCalculator_Always_ReturnLastCreatedSellmeijerCalculator()
        {
            // Setup
            var factory = new TestPipingSubCalculatorFactory();

            // Call
            ISellmeijerCalculator subCalculator = factory.CreateSellmeijerCalculator();

            // Assert
            Assert.AreSame(factory.LastCreatedSellmeijerCalculator, subCalculator);
        }

        [Test]
        public void CreatePiezometricHeadAtExitCalculator_Always_ReturnLastCreatedPiezometricHeadAtExitCalculator()
        {
            // Setup
            var factory = new TestPipingSubCalculatorFactory();

            // Call
            IPiezoHeadCalculator subCalculator = factory.CreatePiezometricHeadAtExitCalculator();

            // Assert
            Assert.AreSame(factory.LastCreatedPiezometricHeadAtExitCalculator, subCalculator);
        }

        [Test]
        public void CreatePipingProfilePropertyCalculator_Always_ReturnLastCreatedPipingProfilePropertyCalculator()
        {
            // Setup
            var factory = new TestPipingSubCalculatorFactory();

            // Call
            IPipingProfilePropertyCalculator subCalculator = factory.CreatePipingProfilePropertyCalculator();

            // Assert
            Assert.AreSame(factory.LastCreatedPipingProfilePropertyCalculator, subCalculator);
        }
    }
}