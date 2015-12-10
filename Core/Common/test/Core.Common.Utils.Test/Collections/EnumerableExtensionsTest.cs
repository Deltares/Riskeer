using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.Common.Utils.Collections;
using NUnit.Framework;

namespace Core.Common.Utils.Test.Collections
{
    [TestFixture]
    public class EnumerableExtensionsTest
    {
        [Test]
        public void ForEach()
        {
            var items = new[]
            {
                1,
                2,
                3
            };

            var results = new List<int>();

            items.ForEachElementDo(results.Add);

            CollectionAssert.AreEqual(items, results, "elements should be equal");
        }

        [Test]
        public void Count_ForRandomRange_ReturnsRangeElementCount()
        {
            var expectedCount = new Random().Next(100);
            IEnumerable enumerable = Enumerable.Range(1, expectedCount);

            Assert.AreEqual(expectedCount, enumerable.Count());
        }
    }
}