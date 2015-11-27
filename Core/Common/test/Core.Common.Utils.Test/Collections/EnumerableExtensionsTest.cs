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
        public void HasExactOneValue()
        {
            IEnumerable one = Enumerable.Range(1, 1);
            Assert.IsTrue(one.HasExactlyOneValue());

            //has two
            IEnumerable two = Enumerable.Range(1, 2);

            Assert.IsFalse(two.HasExactlyOneValue());

            //has none
            Assert.IsFalse(Enumerable.Empty<double>().HasExactlyOneValue());
        }

        [Test]
        public void PlusShouldAddElements()
        {
            var items = new[]
            {
                1,
                2,
                3
            };
            var results = items.Plus(4);

            CollectionAssert.AreEqual(Enumerable.Range(1,4), results);

            results = items.Plus(4, 5);

            CollectionAssert.AreEqual(Enumerable.Range(1, 5), results);
        }

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

            items.ForEach(results.Add);

            CollectionAssert.AreEqual(items, results, "elements should be equal");
        }

        [Test]
        public void ForEachWithIndex()
        {
            var items = new[]
            {
                1,
                2,
                3
            };

            var resultIndices = new List<int>();
            var resultElements = new List<int>();

            items.ForEach((o, i) =>
            {
                resultElements.Add(o);
                resultIndices.Add(i);
            });

            CollectionAssert.AreEqual(items, resultElements);
            CollectionAssert.AreEqual(Enumerable.Range(0,3), resultIndices);
        }

        [Test]
        public void IsMonotonousAscending()
        {
            var items = new[]
            {
                1,
                2,
                3,
                3,
                3,
                5
            };
            Assert.IsTrue(items.IsMonotonousAscending());

            Assert.IsFalse(new[]
            {
                1,
                2,
                1
            }.IsMonotonousAscending());
        }

        [Test]
        public void SplitInGroupsAndVerify()
        {
            var items0 = new int[0];
            var items1 = new[]
            {
                1,
                2,
                3,
                3,
                3,
                5
            };

            Assert.AreEqual(0, items0.SplitInGroups(5).Count());
            Assert.AreEqual(2, items1.SplitInGroups(5).Count());
            Assert.AreEqual(2, items1.SplitInGroups(3).Count());
            Assert.AreEqual(3, items1.SplitInGroups(2).Count());

            Assert.AreEqual(1, items1.SplitInGroups(5).ToList()[0][0]);
            Assert.AreEqual(5, items1.SplitInGroups(5).ToList()[1][0]);
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