﻿using Core.GIS.SharpMap.Layers;
using Core.Plugins.SharpMapGis.Gui.Forms.GridProperties;
using NUnit.Framework;

namespace Core.Plugins.SharpMapGis.Test.Forms.GridProperties
{
    [TestFixture]
    public class VectorLayerPropertiesTest
    {
        [Test]
        public void TestMaximumValueClipping()
        {
            const int maxValue = 999999;
            var vectorLayerProperties = new VectorLayerProperties
            {
                Data = new VectorLayer(),
                LineWidth = maxValue,
                OutlineWidth = maxValue
            };

            Assert.AreEqual(maxValue, vectorLayerProperties.LineWidth);
            Assert.AreEqual(maxValue, vectorLayerProperties.OutlineWidth);

            vectorLayerProperties.LineWidth = maxValue + 1;
            vectorLayerProperties.OutlineWidth = maxValue + 1;

            Assert.AreEqual(maxValue, vectorLayerProperties.LineWidth);
            Assert.AreEqual(maxValue, vectorLayerProperties.OutlineWidth);
        }

        [Test]
        public void TestMinimumValueClipping()
        {
            const int minimum = 0;
            var vectorLayerProperties = new VectorLayerProperties
            {
                Data = new VectorLayer(),
                LineWidth = minimum,
                OutlineWidth = minimum
            };

            Assert.AreEqual(minimum, vectorLayerProperties.LineWidth);
            Assert.AreEqual(minimum, vectorLayerProperties.OutlineWidth);

            vectorLayerProperties.LineWidth = minimum - 1;
            vectorLayerProperties.OutlineWidth = minimum - 1;

            Assert.AreEqual(minimum, vectorLayerProperties.LineWidth);
            Assert.AreEqual(minimum, vectorLayerProperties.OutlineWidth);
        }
    }
}