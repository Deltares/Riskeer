using System;
using System.Drawing;
using System.IO;
using Core.Common.Gui.ContextMenu;
using Core.Common.Gui.Properties;
using Core.Common.TestUtils;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Common.Gui.Tests.ContextMenu
{
    [TestFixture]
    public class StrictContextMenuItemTest
    {
        public interface IClick
        {
            void Click();
        }

        [Test]
        public void Constructor_WithParameters_PropertiesSet()
        {
            // Setup
            var text = "text";
            var toolTip = "tooltip";
            var image = Resources.ImportIcon;
            var mock = MockRepository.GenerateStrictMock<IClick>();
            mock.Expect(m => m.Click());
            EventHandler handler = (s,e) => mock.Click();

            // Call
            var result = new StrictContextMenuItem(text,toolTip,image,handler);
            result.PerformClick();

            // Assert
            Assert.IsInstanceOf<StrictContextMenuItem>(result);
            Assert.AreEqual(text, result.Text);
            Assert.AreEqual(toolTip, result.ToolTipText);
            TestHelper.AssertImagesAreEqual(image, result.Image);
        }
    }
}