using System;
using System.Linq;
using Core.Components.Charting.Data;
using NUnit.Framework;

namespace Core.Components.Charting.Test.Data
{
    [TestFixture]
    public class ChartDataCollectionTest
    {
        [Test]
        public void Constructor_NullList_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new ChartDataCollection(null);

            // Assert
            Assert.Throws<ArgumentNullException>(test);
        }

        [Test]
        public void Constructor_ListSet_InstanceWithListSet()
        {
            // Setup
            var list = Enumerable.Empty<ChartData>().ToList();

            // Call
            var collection = new ChartDataCollection(list);

            // Assert
            Assert.IsInstanceOf<ChartData>(collection);
            Assert.AreSame(list, collection.List);
        }
    }
}