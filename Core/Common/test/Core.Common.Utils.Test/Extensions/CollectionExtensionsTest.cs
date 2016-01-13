using System.Collections.Generic;

using Core.Common.Utils.Extensions;

using NUnit.Framework;

namespace Core.Common.Utils.Test.Extensions
{
    [TestFixture]
    public class CollectionExtensionsTest
    {
        [Test]
        public void RemoveAllWhere_FilterReturningTrueForAllElements_CollectionIsCleared()
        {
            // Setup
            var collection = new List<object>();
            collection.AddRange(new[]
            {
                new object(),
                new object(),
                new object()
            });

            // Call
            collection.RemoveAllWhere(o => true);

            // Assert
            CollectionAssert.IsEmpty(collection);
        }

        [Test]
        public void RemoveAllWhere_FilterReturningFalseForAllElements_CollectionRemainsUnchanged()
        {
            // Setup
            var originalContents = new[]
            {
                new object(),
                new object(),
                new object()
            };

            var collection = new List<object>();
            collection.AddRange(originalContents);

            // Call
            collection.RemoveAllWhere(o => false);

            // Assert
            CollectionAssert.AreEqual(originalContents, collection);
        }

        [Test]
        public void RemoveAllWhere_FilterReturningAlternatesForAllElements_CollectionHasSomeElementsRemoved()
        {
            // Setup
            var expectedElementToKeep = 2;

            var collection = new List<int>();
            collection.AddRange(new[]
            {
                1,
                expectedElementToKeep,
                3
            });

            // Call
            bool alternatingFilterValue = false;
            collection.RemoveAllWhere(o =>
            {
                alternatingFilterValue = !alternatingFilterValue;
                return alternatingFilterValue;
            });

            // Assert
            CollectionAssert.AreEqual(new[] { expectedElementToKeep }, collection);
        }
    }
}