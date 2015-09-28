using DelftTools.TestUtils;
using DeltaShell.Plugins.SharpMapGis.Gui.Forms.GridProperties;
using NUnit.Framework;
using SharpMap.Styles;

namespace DeltaShell.Plugins.SharpMapGis.Tests.Forms.GridProperties
{
    [TestFixture]
    public class LineStylePropertiesTest
    {
        [Test, Category(TestCategory.WindowsForms)]
        public void ShowProperties()
        {
            WindowsFormsTestHelper.ShowPropertyGridForObject(new LineStyleProperties { Data = new VectorStyle() });
        }

        [Test]
        public void TestMaximumValueClipping()
        {
            const int maxValue = 999999;
            var lineStyleProperties = new LineStyleProperties
                {
                    Data = new VectorStyle(),
                    LineWidth = maxValue,
                    OutlineWidth = maxValue
                };

            Assert.AreEqual(maxValue, lineStyleProperties.LineWidth);
            Assert.AreEqual(maxValue, lineStyleProperties.OutlineWidth);

            lineStyleProperties.LineWidth = maxValue + 1;
            lineStyleProperties.OutlineWidth = maxValue + 1;

            Assert.AreEqual(maxValue, lineStyleProperties.LineWidth);
            Assert.AreEqual(maxValue, lineStyleProperties.OutlineWidth);
        }

        [Test]
        public void TestMinimumValueClipping()
        {
            const int minimum = 0;
            var lineStyleProperties = new LineStyleProperties
                {
                    Data = new VectorStyle(),
                    LineWidth = minimum,
                    OutlineWidth = minimum
                };

            Assert.AreEqual(minimum, lineStyleProperties.LineWidth);
            Assert.AreEqual(minimum, lineStyleProperties.OutlineWidth);

            lineStyleProperties.LineWidth = minimum - 1;
            lineStyleProperties.OutlineWidth = minimum - 1;

            Assert.AreEqual(minimum, lineStyleProperties.LineWidth);
            Assert.AreEqual(minimum, lineStyleProperties.OutlineWidth);
        }
    }
}