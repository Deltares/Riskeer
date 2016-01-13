using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Core.Common.Utils.Extensions;

using NUnit.Framework;

namespace Core.Common.Utils.Test.Extensions
{
    [TestFixture]
    public class EnumerableExtensionsTest
    {
        [Test]
        public void ForEachElementDo_PerformTheActionForEachElement()
        {
            // Setup
            var items = new[]
            {
                1,
                2,
                3
            };

            var results = new List<int>();
            Action<int> action = results.Add;

            // Call
            items.ForEachElementDo(action);

            // Assert
            CollectionAssert.AreEqual(items, results, "elements should be equal");
        }

        [Test]
        public void Count_ForRandomRange_ReturnsElementCount([Random(0, 100, 1)] int expectedCount)
        {
            // Setup
            IEnumerable enumerable = Enumerable.Range(1, expectedCount);

            // Call
            var count = enumerable.Count();

            // Assert
            Assert.AreEqual(expectedCount, count);
        }

        [Test]
        public void Count_SequenceIsNull_ThrowArgumentNullException()
        {
            // Setup
            IEnumerable sequence = null;

            // Call
            TestDelegate call = () => sequence.Count();

            // Assert
            Assert.Throws<ArgumentNullException>(call);
        }
    }
}