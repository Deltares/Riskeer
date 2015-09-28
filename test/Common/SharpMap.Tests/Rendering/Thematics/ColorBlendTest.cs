using NUnit.Framework;
using SharpMap.Rendering.Thematics;

namespace SharpMap.Tests.Rendering.Thematics
{
    [TestFixture ]
    public class ColorBlendTest
    {
        ///Equals is implemented to make databinding work..in ThemeEditor
        [Test]
        public void Equals()
        {
            
            var colorBlend = ColorBlend.BlueToRed;
            var otherBlend = ColorBlend.BlueToRed;
            //reference does not equal
            Assert.IsFalse(colorBlend == otherBlend);
            //values does equal.
            Assert.IsTrue(colorBlend.Equals(otherBlend));
        }
    }
}
