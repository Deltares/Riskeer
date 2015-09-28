using DeltaShell.Plugins.SharpMapGis.Gui.Forms.GridProperties;
using NUnit.Framework;
using SharpMap.Styles;

namespace DeltaShell.Plugins.SharpMapGis.Tests.Forms.GridProperties
{
    [TestFixture]
    public class PointStylePropertiesTest
    {
        [Test]
        public void TestMaximumValueClipping()
        {
            const int maxValue = 999999;
            var pointStyleProperties = new PointStyleProperties
                {
                    Data = new VectorStyle(),
                    OutlineWidth = maxValue
                };

            Assert.AreEqual(maxValue, pointStyleProperties.OutlineWidth);

            pointStyleProperties.OutlineWidth = maxValue + 1;

            Assert.AreEqual(maxValue, pointStyleProperties.OutlineWidth);
        }

        [Test]
        public void TestMinimumValueClipping()
        {
            const int minimum = 0;
            var pointStyleProperties = new PointStyleProperties
                {
                    Data = new VectorStyle(),
                    OutlineWidth = minimum
                };

            Assert.AreEqual(minimum, pointStyleProperties.OutlineWidth);

            pointStyleProperties.OutlineWidth = minimum - 1;

            Assert.AreEqual(minimum, pointStyleProperties.OutlineWidth);
        }
    }
}