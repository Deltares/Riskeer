using System;

using Core.Common.Utils.Extensions;

using NUnit.Framework;

namespace Core.Common.Utils.Test.Extensions
{
    [TestFixture]
    public class ComparableExtensionsTest
    {
        [Test]
        [TestCase(null, 1, false)] // Null is smaller then any "not-null"
        [TestCase(null, null, false)] // Null can be considered equal to Null
        [TestCase(1, null, true)] // Any "not-null" is greater then Null
        [TestCase(2, 1, true)]
        [TestCase(1, 1, false)]
        [TestCase(1, 2, false)]
        public void IsBigger_VariousUseCases_ReturnExpectedResult(
            IComparable first, IComparable second, bool expectedResult)
        {
            // Call
            var isFirstBiggerThenSecond = first.IsBigger(second);

            // Assert
            Assert.AreEqual(expectedResult, isFirstBiggerThenSecond);
        }

        [Test]
        public void IsBigger_FirstObjectNotSameTypeAsSecond_ThrowArgumentException()
        {
            // Setup
            int first = 1;
            string second = "one";

            // Call
            TestDelegate call = () => first.IsBigger(second);

            // Assert
            Assert.Throws<ArgumentException>(call);
        }

        [Test]
        [TestCase(null, 1, true)] // Null is smaller then any "not-null"
        [TestCase(null, null, false)] // Null can be considered equal to Null
        [TestCase(1, null, false)] // Any "not-null" is greater then Null
        [TestCase(2, 1, false)]
        [TestCase(1, 1, false)]
        [TestCase(1, 2, true)]
        public void IsSmaller_VariousUseCases_ReturnExpectedResult(
            IComparable first, IComparable second, bool expectedResult)
        {
            // Call
            var isFirstBiggerThenSecond = first.IsSmaller(second);

            // Assert
            Assert.AreEqual(expectedResult, isFirstBiggerThenSecond);
        }

        [Test]
        public void IsSmaller_FirstObjectNotSameTypeAsSecond_ThrowArgumentException()
        {
            // Setup
            int first = 1;
            string second = "one";

            // Call
            TestDelegate call = () => first.IsSmaller(second);

            // Assert
            Assert.Throws<ArgumentException>(call);
        }

        [Test]
        [TestCase(-5, 1, 3, false)]
        [TestCase(-5, 3, 1, false)]
        [TestCase(1 - 1e-6, 1.0, 3.0, false)]
        [TestCase(1 - 1e-6, 3.0, 1.0, false)]
        [TestCase(1, 1, 3, true)]
        [TestCase(1, 3, 1, true)]
        [TestCase(2.01, 1.0, 3.0, true)]
        [TestCase(2.01, 3.0, 1.0, true)]
        [TestCase(3, 1, 3, true)]
        [TestCase(3, 3, 1, true)]
        [TestCase(3 + 1e-6, 1.0, 3.0, false)]
        [TestCase(3 + 1e-6, 3.0, 1.0, false)]
        [TestCase(5, 1, 3, false)]
        [TestCase(5, 3, 1, false)]
        public void IsInRange_VariousUseCases_ReturnExpectedResult(
            IComparable sample, IComparable firstLimit, IComparable secondLimit, bool expectedResult)
        {
            // Call
            var isSampleInRange = sample.IsInRange(firstLimit, secondLimit);

            // Assert
            Assert.AreEqual(expectedResult, isSampleInRange);
        }

        [Test]
        public void IsInRange_SampleObjectTypeNotSameAsFirstLimit_ThrowArgumentException()
        {
            // Setup
            int sample = 1;
            string firstLimit = "one";
            int secondLimit = 2;

            // Call
            TestDelegate call = () => sample.IsInRange(firstLimit, secondLimit);

            // Assert
            Assert.Throws<ArgumentException>(call);
        }

        [Test]
        public void IsInRange_SampleObjectTypeNotSameAsSecondLimit_ThrowArgumentException()
        {
            // Setup
            int sample = 1;
            int firstLimit = 2;
            string secondLimit = "one";

            // Call
            TestDelegate call = () => sample.IsInRange(firstLimit, secondLimit);

            // Assert
            Assert.Throws<ArgumentException>(call);
        }
    }
}