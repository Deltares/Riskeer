using System.Drawing;
using System.Drawing.Imaging;

using Core.Common.TestUtil;
using Core.Common.Utils.Drawing;
using Core.Common.Utils.Test.Properties;

using NUnit.Framework;

namespace Core.Common.Utils.Test.Drawing
{
    [TestFixture]
    public class GraphicsExtensionsTest
    {
        [Test]
        public void DrawImageTransparent_DrawWithOpacity1_TargetShouldBeIdenticalToSource()
        {
            // Setup
            var rect2x2 = new RectangleF(0f, 0f, 2f, 2f);
            var imageFormat = PixelFormat.Format32bppArgb;

            using (var image = Resources.TestImage2x2.Clone(rect2x2, imageFormat))
            using (var target = Resources.Black2x2.Clone(rect2x2, imageFormat))
            {
                // Call
                Graphics.FromImage(target).DrawImageTransparent(image, 0, 0, 1f);

                // Assert
                TestHelper.AssertImagesAreEqual(image, target);
            }
        }

        [Test]
        public void DrawImageTransparent_DrawWithOpacity0_TargetShouldBeUnaffected()
        {
            // Setup
            var rect2x2 = new RectangleF(0f, 0f, 2f, 2f);
            var imageFormat = PixelFormat.Format32bppArgb;

            using (var image = Resources.TestImage2x2.Clone(rect2x2, imageFormat))
            using (var target = Resources.Black2x2.Clone(rect2x2, imageFormat))
            using (var expectedtarget = Resources.Black2x2.Clone(rect2x2, imageFormat))
            {
                // Call
                Graphics.FromImage(target).DrawImageTransparent(image, 0, 0, 0f);

                // Assert
                TestHelper.AssertImagesAreEqual(expectedtarget, target);
            }
        }

        [Test]
        public void DrawImageTransparent_DrawWithOpacity50Percent_TargetShouldBeUpdated()
        {
            // Setup
            var rect2x2 = new RectangleF(0f, 0f, 2f, 2f);
            var imageFormat = PixelFormat.Format32bppArgb;

            using (var image = Resources.TestImage2x2.Clone(rect2x2, imageFormat))
            using (var target = Resources.Black2x2.Clone(rect2x2, imageFormat))
            using (var expectedtarget = Resources.TestImageHalfOpacityOnBlack2x2.Clone(rect2x2, imageFormat))
            {
                // Call
                Graphics.FromImage(target).DrawImageTransparent(image, 0, 0, 0.5f);

                // Assert
                TestHelper.AssertImagesAreEqual(expectedtarget, target);
            }
        }
    }
}