using System;
using System.Collections.Generic;

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
    }
}