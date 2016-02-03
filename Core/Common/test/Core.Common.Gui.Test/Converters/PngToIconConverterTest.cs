using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;

using Core.Common.Gui.Converters;
using Core.Common.Gui.Test.Properties;

using NUnit.Framework;

namespace Core.Common.Gui.Test.Converters
{
    [TestFixture]
    public class PngToIconConverterTest
    {
        [Test]
        public void DefaultConcstructor_ExpectedValues()
        {
            // Call
            var converter = new PngToIconConverter();

            // Assert
            Assert.IsInstanceOf<IValueConverter>(converter);
        }

        [Test]
        public void Convert_WithPngImage_ConvertToBitmapImage()
        {
            // Setup
            var converter = new PngToIconConverter();

            // Call
            var bitmap = converter.Convert(Resources.abacus, typeof(BitmapImage), null, CultureInfo.InvariantCulture);

            // Assert
            Assert.IsInstanceOf<BitmapImage>(bitmap);
            var bitmapInstance = (BitmapImage)bitmap;
            Assert.AreEqual(16, bitmapInstance.Height);
            Assert.AreEqual(16, bitmapInstance.Width);
        }

        [Test]
        public void ConvertBack_ThrowNotImplementedException()
        {
            // Setup
            var converter = new PngToIconConverter();

            // Call
            TestDelegate call = () => converter.ConvertBack(new BitmapImage(), typeof(Bitmap), null, CultureInfo.InvariantCulture);

            // Assert
            Assert.Throws<NotImplementedException>(call);
        }
    }
}