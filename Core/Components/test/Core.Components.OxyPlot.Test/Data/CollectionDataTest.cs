using System;
using System.Collections.ObjectModel;
using Core.Components.OxyPlot.Data;
using NUnit.Framework;

namespace Core.Components.OxyPlot.Test.Data
{
    [TestFixture]
    public class CollectionDataTest
    {
        [Test]
        public void DefaultConstructor_NewInstanceOfIChartData()
        {
            // Call
            var data = new CollectionData();

            // Assert
            Assert.IsInstanceOf<IChartData>(data);
        }
    }
}