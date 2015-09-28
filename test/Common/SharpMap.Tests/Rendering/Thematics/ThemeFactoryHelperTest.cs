using NUnit.Framework;
using SharpMap.Rendering.Thematics;

namespace SharpMap.Tests.Rendering.Thematics
{
    [TestFixture]
    public class ThemeFactoryHelperTest
    {
        [Test]
        public void GenerateEqualIntervals()
        {
            var values = new[] {0.0f,0.5f,1.0f, 10.0f};
            var interVals = ThemeFactoryHelper.GetIntervalsForNumberOfClasses(values,
                                                                              QuantityThemeIntervalType.EqualIntervals,
                                                                              2);
            Assert.AreEqual(2,interVals.Count);
            //0..5
            Assert.AreEqual(0.0f,interVals[0].Min);
            Assert.AreEqual(5.0f, interVals[0].Max);
            //5..10
            Assert.AreEqual(5.0f, interVals[1].Min);
            Assert.AreEqual(10.0f, interVals[1].Max);
        }

        [Test]
        public void GenerateNaturalBreakIntervals()
        {
            var values = new[] { 0.0f, 0.5f, 1.0f, 10.0f };
            var interVals = ThemeFactoryHelper.GetIntervalsForNumberOfClasses(values,
                                                                              QuantityThemeIntervalType.NaturalBreaks,
                                                                              2);
            Assert.AreEqual(2, interVals.Count);
            //0..5
            Assert.AreEqual(0.0f, interVals[0].Min);
            Assert.AreEqual(1.0f, interVals[0].Max);
            //5..10
            Assert.AreEqual(1.0f, interVals[1].Min);
            Assert.AreEqual(10.0f, interVals[1].Max);
        }
    }
}
