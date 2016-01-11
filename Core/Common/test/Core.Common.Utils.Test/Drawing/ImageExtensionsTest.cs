using System.Drawing;
using System.Drawing.Imaging;

using Core.Common.TestUtil;
using Core.Common.Utils.Drawing;
using Core.Common.Utils.Test.Properties;

using NUnit.Framework;

namespace Core.Common.Utils.Test.Drawing
{
    [TestFixture]
    public class ImageExtensionsTest
    {
        [Test]
        public void AddOverlayImage_SourceDrawnCompletelyOverTarget_TargetShouldBeTheSameAsSource()
        {
            // Setup
            var rect2x2 = new RectangleF(0f, 0f, 2f, 2f);
            var imageFormat = PixelFormat.Format32bppArgb;

            using (var image = Resources.TestImage2x2.Clone(rect2x2, imageFormat))
            using (var target = Resources.Black2x2.Clone(rect2x2, imageFormat))
            {
                // Call
                using (var result = target.AddOverlayImage(image, 0, 0, 2, 2))
                {
                    // Assert
                    TestHelper.AssertImagesAreEqual(image, result);
                }
            }
        }

        [Test]
        public void AddOverlayImage_SourceDrawnPartiallyOverTarget_TargetPartiallyChanged()
        {
            // Setup
            var rect2x2 = new RectangleF(0f, 0f, 2f, 2f);
            var imageFormat = PixelFormat.Format32bppArgb;

            using (var target = Resources.TestImage2x2.Clone(rect2x2, imageFormat))
            using (var image = Resources.Black2x2.Clone(rect2x2, imageFormat))
            using (var expectedImage = Resources.TestImageWithBlack2x2.Clone(rect2x2, imageFormat))
            {
                // Call
                using (var result = target.AddOverlayImage(image, 1, 1, 2, 2))
                {
                    // Assert
                    TestHelper.AssertImagesAreEqual(expectedImage, result);
                }

                // 'target' should be unchanged! A new image should have been created.
                TestHelper.AssertImagesAreEqual(Resources.TestImage2x2, target);
            }
        }
    }
}