using DeltaShell.Plugins.SharpMapGis.Gui.Forms.GridProperties;
using NUnit.Framework;
using SharpMap.Styles;

namespace DeltaShell.Plugins.SharpMapGis.Tests.Forms.GridProperties
{
    [TestFixture]
    public class VectorStylePropertiesTest
    {
        [Test]
        public void TestMaximumValueClipping()
        {
            const int maxValue = 999999;
            var vectorStyleProperties = new VectorStyleProperties
            {
                Data = new VectorStyle(),
                Width = maxValue,
                OutlineWidth = maxValue
            };

            Assert.AreEqual(maxValue, vectorStyleProperties.Width);
            Assert.AreEqual(maxValue, vectorStyleProperties.OutlineWidth);

            vectorStyleProperties.Width = maxValue + 1;
            vectorStyleProperties.OutlineWidth = maxValue + 1;

            Assert.AreEqual(maxValue, vectorStyleProperties.Width);
            Assert.AreEqual(maxValue, vectorStyleProperties.OutlineWidth);
        }

        [Test]
        public void TestMinimumValueClipping()
        {
            const int minimum = 0;
            var vectorStyleProperties = new VectorStyleProperties
            {
                Data = new VectorStyle(),
                Width = minimum,
                OutlineWidth = minimum
            };

            Assert.AreEqual(minimum, vectorStyleProperties.Width);
            Assert.AreEqual(minimum, vectorStyleProperties.OutlineWidth);

            vectorStyleProperties.Width = minimum - 1;
            vectorStyleProperties.OutlineWidth = minimum - 1;

            Assert.AreEqual(minimum, vectorStyleProperties.Width);
            Assert.AreEqual(minimum, vectorStyleProperties.OutlineWidth);
        }
    }
}