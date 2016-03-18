using System;
using NUnit.Framework;
using Ringtoets.Piping.KernelWrapper.SubCalculator;

namespace Ringtoets.Piping.KernelWrapper.Test.SubCalculator
{
    [TestFixture]
    public class EffectiveThicknessCalculatorTest
    {
        [Test]
        public void Constructor_WithInput_PropertiesThrowNullReferenceException()
        {
            // Call
            var calculator = new EffectiveThicknessCalculator();

            // Assert
            Assert.IsInstanceOf<IEffectiveThicknessCalculator>(calculator);

            Assert.Throws<NullReferenceException>(() => { var x = calculator.EffectiveHeight; });
            Assert.Throws<NullReferenceException>(() => { var x = calculator.EffectiveStress; });
        } 
    }
}