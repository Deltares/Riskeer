using System.Windows.Forms;
using DelftTools.Shell.Gui.Swf;
using NUnit.Framework;

namespace DeltaShell.Plugins.ProjectExplorer.Tests.NodePresenters
{
    [TestFixture]
    public class NodePresenterHelperTest
    {
        [Test]
        public void TrimSeparatorsGetContextMenu()
        {
            var contextMenu = new ContextMenuStrip();
            var separator1 = new ToolStripSeparator();
            var separator2 = new ToolStripSeparator();
            var separator3 = new ToolStripSeparator();
            var menuItem1 = new ToolStripMenuItem("menuItem1");
            var menuItem2 = new ToolStripMenuItem("menuItem2");
            var menuItem3 = new ToolStripMenuItem("menuItem3");

            separator1.Available = true;
            separator2.Available = true;
            separator3.Available = true;
            menuItem1.Available = false;
            menuItem2.Available = true;
            menuItem3.Available = true;

            contextMenu.Items.Add(menuItem1);
            contextMenu.Items.Add(separator1);
            contextMenu.Items.Add(menuItem2);
            contextMenu.Items.Add(separator2);
            contextMenu.Items.Add(menuItem3);
            contextMenu.Items.Add(separator3);

            NodePresenterHelper.TrimSeparatorsGetContextMenu(contextMenu);

            Assert.IsFalse(menuItem1.Available);
            Assert.IsFalse(separator1.Available);
            Assert.IsTrue(menuItem2.Available);
            Assert.IsTrue(separator2.Available);
            Assert.IsTrue(menuItem3.Available);
            Assert.IsFalse(separator3.Available);
        }

        [Test]
        public void RemoveDoubleSeparatorsGetContextMenu()
        {
            var contextMenu = new ContextMenuStrip();
            var separator1 = new ToolStripSeparator();
            var separator2 = new ToolStripSeparator();
            var separator3 = new ToolStripSeparator();
            var menuItem1 = new ToolStripMenuItem("menuItem1");
            var menuItem2 = new ToolStripMenuItem("menuItem2");
            var menuItem3 = new ToolStripMenuItem("menuItem3");

            separator1.Available = true;
            separator2.Available = true;
            separator3.Available = true;

            menuItem1.Available = true;
            menuItem2.Available = false;
            menuItem3.Available = true;

            contextMenu.Items.Add(menuItem1);
            contextMenu.Items.Add(separator1);
            contextMenu.Items.Add(separator2);
            contextMenu.Items.Add(menuItem2);
            contextMenu.Items.Add(separator3);
            contextMenu.Items.Add(menuItem3);

            NodePresenterHelper.RemoveDoubleSeparatorsGetContextMenu(contextMenu);

            Assert.IsFalse(separator1.Available);
            Assert.IsFalse(separator2.Available);
            Assert.IsTrue(separator3.Available);
        }
    }
}