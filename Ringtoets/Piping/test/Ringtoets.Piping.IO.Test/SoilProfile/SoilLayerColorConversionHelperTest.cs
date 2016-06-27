using System.Drawing;
using NUnit.Framework;
using Ringtoets.Piping.IO.Builders;

namespace Ringtoets.Piping.IO.Test.SoilProfile
{
    [TestFixture]
    public class SoilLayerColorConversionHelperTest
    {
        [Test]
        public void FromNullableDouble_Null_ReturnsEmptyColor()
        {
            // Call
            var color = SoilLayerColorConversionHelper.ColorFromNullableDouble(null);

            // Assert
            Assert.AreEqual(Color.Empty, color);
        }

        [Test]
        [TestCase(-12156236, 70, 130, 180)]
        [TestCase(-8372160, 128, 64, 64)]
        [TestCase(-65536, 255, 0, 0)]
        [TestCase(-14634326, 32, 178, 170)]
        [TestCase(-6632142, 154, 205, 50)]
        [TestCase(-12566528, 64, 64, 0)]
        [TestCase(-7278960, 144, 238, 144)]
        [TestCase(-8323200, 128, 255, 128)]
        [TestCase(-65281, 255, 0, 255)]
        [TestCase(-8372224, 128, 64, 0)]
        public void FromNullableDouble_DifferentDoubleValues_ReturnsExpectedColor(double colorValue, int r, int g, int b)
        {
            // Call
            var color = SoilLayerColorConversionHelper.ColorFromNullableDouble(colorValue);

            // Assert
            Assert.AreEqual(Color.FromArgb(r, g, b), color);
        }
    }
}