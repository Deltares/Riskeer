using System;
using System.Collections;
using System.Collections.Generic;

using NUnit.Framework;

namespace Core.Common.TestUtil.Test
{
    [TestFixture]
    public class DoubleWithToleranceComparerTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup

            // Call
            var comparer = new DoubleWithToleranceComparer(1.1);

            // Assert
            Assert.IsInstanceOf<IComparer>(comparer);
            Assert.IsInstanceOf<IComparer<double>>(comparer);
        }

        [Test]
        public void Compare_FirstObjectOfIncorrectType_ThrowArgumentException()
        {
            // Setup
            object firstObject = new object();
            object secondObject = 1.1;

            var comparer = new DoubleWithToleranceComparer(2.2);

            // Call
            TestDelegate call = () => comparer.Compare(firstObject, secondObject);

            // Assert
            var message = Assert.Throws<ArgumentException>(call).Message;
            Assert.AreEqual("Cannot compare objects other than System.Double with this comparer.", message);
        }

        [Test]
        public void Compare_SecondObjectOfIncorrectType_ThrowArgumentException()
        {
            // Setup
            object firstObject = 2.2;
            object secondObject = new object();

            var comparer = new DoubleWithToleranceComparer(2.2);

            // Call
            TestDelegate call = () => comparer.Compare(firstObject, secondObject);

            // Assert
            var message = Assert.Throws<ArgumentException>(call).Message;
            Assert.AreEqual("Cannot compare objects other than System.Double with this comparer.", message);
        }

        [Test]
        [TestCase(1.1, 2.2, 1.1)]
        [TestCase(1.1, 1.1, 0.0)]
        [TestCase(-2.2, 0.0, 2.2)]
        [TestCase(0.0, -1.6, 2)]
        public void Compare_ValuesWithinTolerance_ReturnZero(double first, double second, double tolerance)
        {
            // Setup
            var comparer = new DoubleWithToleranceComparer(tolerance);

            // Call
            int result = comparer.Compare(first, second);

            // Assert
            Assert.AreEqual(0, result);
        }

        [Test]
        [Combinatorial]
        public void Compare_FirstLessThanSecond_ReturnLessThanZero(
            [Values(1.1)]double first, 
            [Values(2.2 + 1e-6, 6.8)]double second,
            [Values(true, false)]bool castToObject)
        {
            // Setup
            var comparer = new DoubleWithToleranceComparer(1.1);

            // Call
            var result = castToObject ?
                             comparer.Compare((object)first, second) :
                             comparer.Compare(first, second);

            // Assert
            Assert.Less(result, 0);
        }

        [Test]
        [Combinatorial]
        public void Compare_FirstGreaterThanSecond_ReturnGreaterThanZero(
            [Values(1.1)]double first, 
            [Values(0.6 - 1e-6, -9.65)]double second, 
            [Values(true, false)]bool castToObject)
        {
            // Setup
            var comparer = new DoubleWithToleranceComparer(0.5);

            // Call
            var result = castToObject ?
                             comparer.Compare((object)first, second) :
                             comparer.Compare(first, second);

            // Assert
            Assert.Greater(result, 0);
        }
    }
}