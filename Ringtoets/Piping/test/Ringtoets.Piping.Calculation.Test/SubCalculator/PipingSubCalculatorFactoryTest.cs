﻿using NUnit.Framework;
using Ringtoets.Piping.KernelWrapper.SubCalculator;

namespace Ringtoets.Piping.KernelWrapper.Test.SubCalculator
{
    [TestFixture]
    public class PipingSubCalculatorFactoryTest
    {
        [Test]
        public void Constructor_ReturnsNewInstance()
        {
            // Call
            var factory = new PipingSubCalculatorFactory();

            // Assert
            Assert.IsInstanceOf<IPipingSubCalculatorFactory>(factory);
        }

        [Test]
        public void CreateHeaveCalculator_Always_NewHeaveCalculator()
        {
            // Setup
            var factory = new PipingSubCalculatorFactory();

            // Call
            var calculator = factory.CreateHeaveCalculator();

            // Assert
            Assert.IsInstanceOf<IHeaveCalculator>(calculator);
        }

        [Test]
        public void CreateUpliftCalculator_Always_NewUpliftCalculator()
        {
            // Setup
            var factory = new PipingSubCalculatorFactory();

            // Call
            var calculator = factory.CreateUpliftCalculator();

            // Assert
            Assert.IsInstanceOf<IUpliftCalculator>(calculator);
        }

        [Test]
        public void CreateSellmeijerCalculator_Always_NewSellmeijerCalculator()
        {
            // Setup
            var factory = new PipingSubCalculatorFactory();

            // Call
            var calculator = factory.CreateSellmeijerCalculator();

            // Assert
            Assert.IsInstanceOf<ISellmeijerCalculator>(calculator);
        }

        [Test]
        public void CreateEffectiveThicknessCalculator_Always_NewSellmeijerCalculator()
        {
            // Setup
            var factory = new PipingSubCalculatorFactory();

            // Call
            var calculator = factory.CreateEffectiveThicknessCalculator();

            // Assert
            Assert.IsInstanceOf<IEffectiveThicknessCalculator>(calculator);
        }

        [Test]
        public void CreatPiezometricHeadAtExitCalculator_Always_NewPizometricHeadAtExitCalculator()
        {
            // Setup
            var factory = new PipingSubCalculatorFactory();

            // Call
            var calculator = factory.CreatePiezometricHeadAtExitCalculator();

            // Assert
            Assert.IsInstanceOf<IPiezoHeadCalculator>(calculator);
        }
    }
}