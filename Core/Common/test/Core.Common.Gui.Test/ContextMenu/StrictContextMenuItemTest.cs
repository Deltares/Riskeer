using System;
using Core.Common.Gui.ContextMenu;
using Core.Common.Gui.Properties;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Common.Gui.Test.ContextMenu
{
    [TestFixture]
    public class StrictContextMenuItemTest
    {
        [Test]
        public void Constructor_WithParameters_PropertiesSet()
        {
            // Setup
            var mockRepository = new MockRepository();

            var text = "text";
            var toolTip = "tooltip";
            var image = Resources.ImportIcon;
            var counter = 0;

            mockRepository.ReplayAll();

            EventHandler handler = (s, e) => counter++;

            // Call
            var result = new StrictContextMenuItem(text,toolTip,image,handler);
            result.PerformClick();

            // Assert
            Assert.IsInstanceOf<StrictContextMenuItem>(result);
            Assert.AreEqual(text, result.Text);
            Assert.AreEqual(toolTip, result.ToolTipText);
            Assert.AreEqual(1, counter);
            TestHelper.AssertImagesAreEqual(image, result.Image);

            mockRepository.VerifyAll();
        }
    }
}