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

namespace Ringtoets.Piping.KernelWrapper.Test.SubCalculator
{
    [TestFixture]
    public class PipingSubCalculatorFactoryTest
    {
        [Test]
        public void Instance_Always_ReturnsAnInstance()
        {
            // Call
            IPipingSubCalculatorFactory factory = PipingSubCalculatorFactory.Instance;

            // Assert
            Assert.IsInstanceOf<PipingSubCalculatorFactory>(factory);
        }

        [Test]
        public void Instance_WhenSetToNull_ReturnsNewInstance()
        {
            // Setup
            IPipingSubCalculatorFactory firstFactory = PipingSubCalculatorFactory.Instance;
            PipingSubCalculatorFactory.Instance = null;

            // Call
            IPipingSubCalculatorFactory secondFactory = PipingSubCalculatorFactory.Instance;

            // Assert
            Assert.AreNotSame(firstFactory, secondFactory);
        }

        [Test]
        public void Instance_WhenSetToInstance_ReturnsThatInstance()
        {
            // Setup
            var firstFactory = new TestPipingSubCalculatorFactory();
            PipingSubCalculatorFactory.Instance = firstFactory;

            // Call
            IPipingSubCalculatorFactory secondFactory = PipingSubCalculatorFactory.Instance;

            // Assert
            Assert.AreSame(firstFactory, secondFactory);
        }

        [Test]
        public void CreateHeaveCalculator_Always_ReturnsHeaveCalculator()
        {
            // Setup
            IPipingSubCalculatorFactory factory = PipingSubCalculatorFactory.Instance;

            // Call
            IHeaveCalculator calculator = factory.CreateHeaveCalculator();

            // Assert
            Assert.IsInstanceOf<HeaveCalculator>(calculator);
        }

        [Test]
        public void CreateUpliftCalculator_Always_ReturnsUpliftCalculator()
        {
            // Setup
            IPipingSubCalculatorFactory factory = PipingSubCalculatorFactory.Instance;

            // Call
            IUpliftCalculator calculator = factory.CreateUpliftCalculator();

            // Assert
            Assert.IsInstanceOf<UpliftCalculator>(calculator);
        }

        [Test]
        public void CreateSellmeijerCalculator_Always_ReturnsSellmeijerCalculator()
        {
            // Setup
            IPipingSubCalculatorFactory factory = PipingSubCalculatorFactory.Instance;

            // Call
            ISellmeijerCalculator calculator = factory.CreateSellmeijerCalculator();

            // Assert
            Assert.IsInstanceOf<SellmeijerCalculator>(calculator);
        }

        [Test]
        public void CreateEffectiveThicknessCalculator_Always_ReturnsEffectiveThicknessCalculator()
        {
            // Setup
            IPipingSubCalculatorFactory factory = PipingSubCalculatorFactory.Instance;

            // Call
            IEffectiveThicknessCalculator calculator = factory.CreateEffectiveThicknessCalculator();

            // Assert
            Assert.IsInstanceOf<EffectiveThicknessCalculator>(calculator);
        }

        [Test]
        public void CreatePiezometricHeadAtExitCalculator_Always_ReturnsPiezometricHeadAtExitCalculator()
        {
            // Setup
            IPipingSubCalculatorFactory factory = PipingSubCalculatorFactory.Instance;

            // Call
            IPiezoHeadCalculator calculator = factory.CreatePiezometricHeadAtExitCalculator();

            // Assert
            Assert.IsInstanceOf<PiezoHeadCalculator>(calculator);
        }
    }
}