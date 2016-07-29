using System;
using System.Diagnostics;
using System.Linq;

using Application.Ringtoets.Storage.BinaryConverters;

using NUnit.Framework;

namespace Application.Ringtoets.Storage.Test.BinaryConverters
{
    [TestFixture]
    public class BinaryDataEqualityHelperTest
    {
        [Test]
        public void AreEqual_ArraysAreNotEqual_ReturnFalse()
        {
            // Setup
            var random = new Random(42);
            const int arraySize = 10;
            var array1 = new byte[arraySize];
            var array2 = new byte[arraySize];
            random.NextBytes(array1);
            random.NextBytes(array2);

            // Precondition
            CollectionAssert.AreNotEqual(array1, array2);

            // Call
            bool areCollectionEqual = BinaryDataEqualityHelper.AreEqual(array1, array2);

            // Assert
            Assert.IsFalse(areCollectionEqual);
        }

        [Test]
        public void AreEqual_ArraysAreEqual_ReturnTrue()
        {
            // Setup
            var random = new Random(42);
            const int arraySize = 10;
            var array1 = new byte[arraySize];
            random.NextBytes(array1);

            // Precondition
            CollectionAssert.AreEqual(array1, array1);

            // Call
            bool areCollectionEqual = BinaryDataEqualityHelper.AreEqual(array1, array1);

            // Assert
            Assert.IsTrue(areCollectionEqual);
        }

        [Test]
        public void GivenNotEqualData_WhenComparingPerformance_ThenPerformanceShouldBeBetterThenLinq()
        {
            // Given
            var random = new Random(42);
            const int arraySize = 100000000;
            var array1 = new byte[arraySize];
            var array2 = new byte[arraySize];
            random.NextBytes(array1);
            random.NextBytes(array2);

            // Precondition
            CollectionAssert.AreNotEqual(array1, array2);

            // When
            var stopwatch = new Stopwatch();
            long timeToBeat = 0, actualTime = 0;
            for (int i = 0; i < 100000; i++)
            {
                stopwatch.Start();
                Assert.IsFalse(SlowBaselineLinqEqualty(array1, array2));
                stopwatch.Stop();
                timeToBeat += stopwatch.ElapsedTicks;

                stopwatch.Reset();
                stopwatch.Start();
                Assert.IsFalse(BinaryDataEqualityHelper.AreEqual(array1, array2));
                stopwatch.Stop();
                actualTime += stopwatch.ElapsedTicks;
            }

            // Then
            Assert.Less(actualTime, timeToBeat); // If you want to see the 'timings', just change it to Assert.Greater
        }

        private static bool SlowBaselineLinqEqualty(byte[] array1, byte[] array2)
        {
            return array1.SequenceEqual(array2);
        }
    }
}