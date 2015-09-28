using System.Windows.Forms;
using DelftTools.Controls.Swf;
using NUnit.Framework;

namespace DelftTools.Tests.Controls.Swf
{
    /// <summary>
    /// These test are to show some 'problems' with contextmenus and combininbg them
    /// In order to support rich context menus we typeically want the following scenario:
    /// baseMenu = new Menu
    /// baseMenu.Extend(ContextMenu.Items)
    /// 
    /// The problem is that a menuItem has an owner and there can only be 1 owner
    /// This could be easily solved if menuItems implemented IClonable.
    /// Unfortunately this is not the case. DelftTools.Shell.Gui.Swf.Controls offers
    /// a ClonableToolStripMenuItem class inherited from ToolStripMenuItem that can be used 
    /// to clone objects.
    /// Other solutions using extension methods (no support protected members (event handlers))
    /// or reflection (too complex for event handlers) have been taken into account. 
    /// </summary>
    [TestFixture]
    public class MenuItemContextMenuStripAdapterTest
    {
        [Test]
        public void SimpleOwnerTest()
        {
            ToolStripMenuItem   toolStripMenuItem = new ToolStripMenuItem("Test");

            ContextMenuStrip contextMenuStripFirst = new ContextMenuStrip();
            contextMenuStripFirst.Items.Add(toolStripMenuItem);
            // contextMenuStripFirst has 1 item as expected
            Assert.AreEqual(1, contextMenuStripFirst.Items.Count);

            ContextMenuStrip contextMenuStripSecond = new ContextMenuStrip();
            contextMenuStripSecond.Items.Add(toolStripMenuItem);
            // Add same item to second menu; this will have 1 item as expected 
            Assert.AreEqual(1, contextMenuStripSecond.Items.Count);
            // The first menu will have 0 items
            Assert.AreEqual(0, contextMenuStripFirst.Items.Count);
            // NB Setting toolStripMenuItem.Owner to null would have had the same effect
        }

        [Test]
        public void CombineMenusWithoutAdapterMenuItem()
        {
            ContextMenuStrip contextMenuStripFirst = new ContextMenuStrip();
            contextMenuStripFirst.Items.Add("item1");
            contextMenuStripFirst.Items.Add("item2");
            contextMenuStripFirst.Items.Add("item3");

            ContextMenuStrip contextMenuStripSecond = new ContextMenuStrip();
            contextMenuStripSecond.Items.Add("item4");
            contextMenuStripSecond.Items.Add("item5");

            Assert.AreEqual(3, contextMenuStripFirst.Items.Count);
            Assert.AreEqual(2, contextMenuStripSecond.Items.Count);

            // AddRange doesn't work!
            // contextMenuStripFirst.Items.AddRange(contextMenuStripSecond.Items);

            ToolStripItem[] toolStripItems = new ToolStripItem[contextMenuStripSecond.Items.Count];
            contextMenuStripSecond.Items.CopyTo(toolStripItems, 0);
            contextMenuStripFirst.Items.AddRange(toolStripItems);

            Assert.AreEqual(5, contextMenuStripFirst.Items.Count);

            // NB 0 is not what I want; a menuitem can only be part of 1 menu
            Assert.AreEqual(0, contextMenuStripSecond.Items.Count);
        }

        [Test]
        public void CombineMenusWithoutAdapterClonableToolStripMenuItem()
        {
            ContextMenuStrip contextMenuStripFirst = new ContextMenuStrip();
            contextMenuStripFirst.Items.Add(new ClonableToolStripMenuItem { Text = "item1" });
            contextMenuStripFirst.Items.Add(new ClonableToolStripMenuItem { Text = "item2" });
            contextMenuStripFirst.Items.Add(new ClonableToolStripMenuItem { Text = "item3" });

            ContextMenuStrip contextMenuStripSecond = new ContextMenuStrip();
            contextMenuStripSecond.Items.Add(new ClonableToolStripMenuItem { Text = "item4" });
            contextMenuStripSecond.Items.Add(new ClonableToolStripMenuItem { Text = "item5" });

            Assert.AreEqual(3, contextMenuStripFirst.Items.Count);
            Assert.AreEqual(2, contextMenuStripSecond.Items.Count);

            for (int i = 0; i < contextMenuStripSecond.Items.Count; i++)
            {
                contextMenuStripFirst.Items.Add(((ClonableToolStripMenuItem) contextMenuStripSecond.Items[i]).Clone());
            }
            Assert.AreEqual(5, contextMenuStripFirst.Items.Count);
            Assert.AreEqual(2, contextMenuStripSecond.Items.Count);
        }

        [Test]
        public void CombineMenusWithAdapter()
        {
            ContextMenuStrip contextMenuStripFirst = new ContextMenuStrip();
            contextMenuStripFirst.Items.Add(new ClonableToolStripMenuItem { Text = "item1" });
            contextMenuStripFirst.Items.Add(new ClonableToolStripMenuItem { Text = "item2" });
            contextMenuStripFirst.Items.Add(new ClonableToolStripMenuItem { Text = "item3" });

            ContextMenuStrip contextMenuStripSecond = new ContextMenuStrip();
            contextMenuStripSecond.Items.Add(new ClonableToolStripMenuItem { Text = "item4" });
            contextMenuStripSecond.Items.Add(new ClonableToolStripMenuItem { Text = "item5" });

            Assert.AreEqual(3, contextMenuStripFirst.Items.Count);
            Assert.AreEqual(2, contextMenuStripSecond.Items.Count);

            MenuItemContextMenuStripAdapter menuItemContextMenuStripAdapter = new MenuItemContextMenuStripAdapter(contextMenuStripFirst);            

            menuItemContextMenuStripAdapter.Add(new MenuItemContextMenuStripAdapter(contextMenuStripSecond));

            Assert.AreEqual(5, contextMenuStripFirst.Items.Count);
            Assert.AreEqual(2, contextMenuStripSecond.Items.Count);
        }

        [Test]
        public void TestAdapterCloningForEventHandling()
        {
            ContextMenuStrip contextMenuStripFirst = new ContextMenuStrip();

            ContextMenuStrip contextMenuStripSecond = new ContextMenuStrip();
            ClonableToolStripMenuItem clonableToolStripMenuItem = new ClonableToolStripMenuItem { Text = "item1" };
            contextMenuStripSecond.Items.Add(clonableToolStripMenuItem);

            int counter = 0;
            clonableToolStripMenuItem.CheckedChanged += (o, e) => { counter++; };

            MenuItemContextMenuStripAdapter menuItemContextMenuStripAdapter = new MenuItemContextMenuStripAdapter(contextMenuStripFirst);
            // Add items from second menu to first menu
            menuItemContextMenuStripAdapter.Add(new MenuItemContextMenuStripAdapter(contextMenuStripSecond));

            Assert.AreEqual(1, contextMenuStripFirst.Items.Count);
            Assert.AreEqual(1, contextMenuStripSecond.Items.Count);

            // Both event should call the same eventhandler and thus increment the same counter var.
            ((ToolStripMenuItem)contextMenuStripFirst.Items[0]).Checked = !(((ToolStripMenuItem)contextMenuStripFirst.Items[0]).Checked);
            Assert.AreEqual(1, counter);
            ((ToolStripMenuItem)contextMenuStripSecond.Items[0]).Checked = !(((ToolStripMenuItem)contextMenuStripSecond.Items[0]).Checked);
            Assert.AreEqual(2, counter);
        }


        [Test]
        public void TestInsertAtOfMultiItems()
        {
            var contextMenuStripFirst = new ContextMenuStrip();
            var contextMenuStripSecond = new ContextMenuStrip();
            var clonableToolStripMenuItem1 = new ClonableToolStripMenuItem { Text = "item1" };
            var clonableToolStripMenuItem2 = new ClonableToolStripMenuItem { Text = "item2" };
            var clonableToolStripMenuItem3 = new ClonableToolStripMenuItem { Text = "item3" };
            
            contextMenuStripFirst.Items.Add(clonableToolStripMenuItem1);
            contextMenuStripFirst.Items.Add(clonableToolStripMenuItem2);

            contextMenuStripSecond.Items.Add(clonableToolStripMenuItem3);

            var menuItemContextMenuStripAdapter1 = new MenuItemContextMenuStripAdapter(contextMenuStripFirst);
            var menuItemContextMenuStripAdapter2 = new MenuItemContextMenuStripAdapter(contextMenuStripSecond);

            menuItemContextMenuStripAdapter2.Insert(0, menuItemContextMenuStripAdapter1);

            var contextMenuStripMerged = menuItemContextMenuStripAdapter2.ContextMenuStrip;

            Assert.AreEqual(3, contextMenuStripMerged.Items.Count);
            Assert.AreEqual("item1", contextMenuStripMerged.Items[0].Text);
            Assert.AreEqual("item2", contextMenuStripMerged.Items[1].Text);
            Assert.AreEqual("item3", contextMenuStripMerged.Items[2].Text);
        }

        [Test]
        public void TestIndexOf()
        {
            var contextMenuStrip = new ContextMenuStrip();
            var toolStripMenuItem = new ToolStripMenuItem() { Text = "item1", Name = "item1"};
            var clonableToolStripMenuItem1 = new ClonableToolStripMenuItem { Text = "item2" };
            var clonableToolStripMenuItem2 = new ClonableToolStripMenuItem { Text = "item3", Name = "item3" };

            contextMenuStrip.Items.Add(toolStripMenuItem);
            contextMenuStrip.Items.Add(clonableToolStripMenuItem1);
            contextMenuStrip.Items.Add(clonableToolStripMenuItem2);

            var menuItemContextMenuStripAdapter = new MenuItemContextMenuStripAdapter(contextMenuStrip);

            Assert.AreEqual(0, menuItemContextMenuStripAdapter.IndexOf("item1"));
            Assert.AreEqual(2, menuItemContextMenuStripAdapter.IndexOf("item3"));
            Assert.AreEqual(-1, menuItemContextMenuStripAdapter.IndexOf("haha"));

        }

        [Test]
        public void ContextMenuStripIndexByName()
        {
            var contextMenuStrip = new ContextMenuStrip();
            var toolStripMenuItemNotNamed = new ToolStripMenuItem() { Text = "NotNamed" };
            var toolStripMenuItemNamed = new ToolStripMenuItem() { Name = "Named" };
            contextMenuStrip.Items.Add(toolStripMenuItemNotNamed);
            contextMenuStrip.Items.Add(toolStripMenuItemNamed);

            Assert.IsNull(contextMenuStrip.Items["NotNamed"]);
            Assert.IsNotNull(contextMenuStrip.Items["Named"]);

        }
    }
}