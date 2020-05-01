using System;
using System.Collections;
using System.Collections.Generic;
using Deltares.MacroStability.Geometry;
using NUnit.Framework;
using Riskeer.MacroStabilityInwards.KernelWrapper.TestUtil.Kernels.UpliftVan.Input;

namespace Riskeer.MacroStabilityInwards.KernelWrapper.TestUtil.Test.Kernels.UpliftVan.Input
{
    [TestFixture]
    public class FixedSoilStressComparerTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var comparer = new FixedSoilStressComparer();

            // Assert
            Assert.IsInstanceOf<IComparer>(comparer);
            Assert.IsInstanceOf<IComparer<FixedSoilStress>>(comparer);
        }

        [Test]
        public void Compare_FirstObjectOfIncorrectType_ThrowArgumentException()
        {
            // Setup
            var firstObject = new object();
            object secondObject = new FixedSoilStress(new Soil(), StressValueType.POP, 0);

            var comparer = new FixedSoilStressComparer();

            // Call
            void Call() => comparer.Compare(firstObject, secondObject);

            // Assert
            var exception = Assert.Throws<ArgumentException>(Call);
            Assert.AreEqual($"Cannot compare objects other than {typeof(FixedSoilStress)} with this comparer.", exception.Message);
        }

        [Test]
        public void Compare_SecondObjectOfIncorrectType_ThrowArgumentException()
        {
            // Setup
            object firstObject = new FixedSoilStress(new Soil(), StressValueType.POP, 0);
            var secondObject = new object();

            var comparer = new FixedSoilStressComparer();

            // Call
            void Call() => comparer.Compare(firstObject, secondObject);

            // Assert
            var exception = Assert.Throws<ArgumentException>(Call);
            Assert.AreEqual($"Cannot compare objects other than {typeof(FixedSoilStress)} with this comparer.", exception.Message);
        }

        [Test]
        public void Compare_SameInstance_ReturnZero()
        {
            // Setup
            var soilStress = new FixedSoilStress(new Soil(), StressValueType.POP, 0);

            // Call
            int result = new FixedSoilStressComparer().Compare(soilStress, soilStress);

            // Assert
            Assert.AreEqual(0, result);
        }

        [Test]
        public void Compare_EqualSoilNameAndPop_ReturnZero()
        {
            // Setup
            const double pop = 1.1;
            const string soilName = "soil name";
            var fixedSoilStress1 = new FixedSoilStress(new Soil(soilName), StressValueType.POP, pop);
            var fixedSoilStress2 = new FixedSoilStress(new Soil(soilName), StressValueType.POP, pop);

            // Call
            int result = new FixedSoilStressComparer().Compare(fixedSoilStress1, fixedSoilStress2);

            // Assert
            Assert.AreEqual(0, result);
        }

        [Test]
        public void Compare_DifferentSoilNames_ReturnOne()
        {
            // Setup
            const double pop = 1.1;
            var fixedSoilStress1 = new FixedSoilStress(new Soil("soil name 1"), StressValueType.POP, pop);
            var fixedSoilStress2 = new FixedSoilStress(new Soil("soil name 2"), StressValueType.POP, pop);

            // Call
            int result = new FixedSoilStressComparer().Compare(fixedSoilStress1, fixedSoilStress2);

            // Assert
            Assert.AreEqual(1, result);
        }

        [Test]
        public void Compare_DifferentPop_ReturnOne()
        {
            // Setup
            const string soilName = "soil name";
            var fixedSoilStress1 = new FixedSoilStress(new Soil(soilName), StressValueType.POP, 1.1);
            var fixedSoilStress2 = new FixedSoilStress(new Soil(soilName), StressValueType.POP, 2.2);

            // Call
            int result = new FixedSoilStressComparer().Compare(fixedSoilStress1, fixedSoilStress2);

            // Assert
            Assert.AreEqual(1, result);
        }

        [Test]
        public void Compare_DifferentStressValueType_ReturnOne()
        {
            // Setup
            const double pop = 1.1;
            const string soilName = "soil name";
            var fixedSoilStress1 = new FixedSoilStress(new Soil(soilName), StressValueType.POP, pop);
            var fixedSoilStress2 = new FixedSoilStress(new Soil(soilName), StressValueType.OCR, pop);

            // Call
            int result = new FixedSoilStressComparer().Compare(fixedSoilStress1, fixedSoilStress2);

            // Assert
            Assert.AreEqual(1, result);
        }
    }
}