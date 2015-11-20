using System.Drawing;
using System.Windows.Forms;
using NUnit.Framework;

using Ringtoets.Common.Forms.Extensions;
using FormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.Common.Forms.Test.Extensions
{
    [TestFixture]
    public class ContextMenuStripExtensionsTest
    {
        [Test]
        public void AddMenuItem_AllDataProvided_AddFullyInitializedMenuItem()
        {
            // Setup
            var menu = new ContextMenuStrip();
            int callCount = 0;

            const string menuItemsText = "<menu.Items[0].Text>";
            const string menuItemsTooltip = "<menu.Items[0].ToolTip>";

            // Call
            var newItem = menu.AddMenuItem(menuItemsText, menuItemsTooltip, FormsResources.GeneralFolderIcon, (sender, args) => callCount++);

            // Assert
            Assert.AreEqual(1, menu.Items.Count);
            var addedMenuItem = menu.Items[0];
            Assert.AreSame(newItem, addedMenuItem);
            Assert.AreEqual(menuItemsText, addedMenuItem.Text);
            Assert.AreEqual(menuItemsTooltip, addedMenuItem.ToolTipText);
            Assert.IsNotNull(addedMenuItem.Image);

            addedMenuItem.PerformClick();
            Assert.AreEqual(1, callCount);
        }
    }
}