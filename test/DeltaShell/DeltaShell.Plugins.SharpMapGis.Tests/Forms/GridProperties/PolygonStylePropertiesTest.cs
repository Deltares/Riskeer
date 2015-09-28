﻿using DelftTools.TestUtils;
using DeltaShell.Plugins.SharpMapGis.Gui.Forms.GridProperties;
using NUnit.Framework;
using SharpMap.Styles;

namespace DeltaShell.Plugins.SharpMapGis.Tests.Forms.GridProperties
{
    [TestFixture]
    public class PolygonStylePropertiesTest
    {
        [Test, Category(TestCategory.WindowsForms)]
        public void ShowProperties()
        {
            WindowsFormsTestHelper.ShowPropertyGridForObject(new PolygonStyleProperties { Data = new VectorStyle() });
        }

        [Test]
        public void TestMaximumValueClipping()
        {
            const int maxValue = 999999;
            var polygonStyleProperties = new PolygonStyleProperties
                {
                    Data = new VectorStyle(),
                    OutlineWidth = maxValue
                };

            Assert.AreEqual(maxValue, polygonStyleProperties.OutlineWidth);

            polygonStyleProperties.OutlineWidth = maxValue + 1;

            Assert.AreEqual(maxValue, polygonStyleProperties.OutlineWidth);
        }

        [Test]
        public void TestMinimumValueClipping()
        {
            const int minimum = 0;
            var polygonStyleProperties = new PolygonStyleProperties
                {
                    Data = new VectorStyle(),
                    OutlineWidth = minimum
                };

            Assert.AreEqual(minimum, polygonStyleProperties.OutlineWidth);

            polygonStyleProperties.OutlineWidth = minimum - 1;

            Assert.AreEqual(minimum, polygonStyleProperties.OutlineWidth);
        }
    }
}