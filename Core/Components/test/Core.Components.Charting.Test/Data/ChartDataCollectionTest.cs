using System;
using System.Linq;
using Core.Common.TestUtil;
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
            TestDelegate test = () => new ChartDataCollection(null, "test data");

            // Assert
            Assert.Throws<ArgumentNullException>(test);
        }

        [Test]
        [TestCase("")]
        [TestCase("    ")]
        [TestCase(null)]
        public void Constructor_InvalidName_ThrowsArgumentExcpetion(string invalidName)
        {
            // Setup
            var list = Enumerable.Empty<ChartData>().ToList();

            // Call
            TestDelegate test = () => new ChartDataCollection(list, invalidName);

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(test, "A name must be set to map data");
        }

        [Test]
        public void Constructor_ListSet_InstanceWithListSet()
        {
            // Setup
            var list = Enumerable.Empty<ChartData>().ToList();

            // Call
            var collection = new ChartDataCollection(list, "test data");

            // Assert
            Assert.IsInstanceOf<ChartData>(collection);
            Assert.AreSame(list, collection.List);
        }
    }
}