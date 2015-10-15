using NUnit.Framework;
using Wti.Forms.NodePresenters;

namespace Wti.Forms.Test
{
    public class PipingContextMenuStripTest
    {
        [Test]
        public void Constructor_Always_OneMenuItemIsAdded()
        {
            var actual = new PipingContextMenuStrip(null);
            Assert.AreEqual(1, actual.Items.Count);
            CollectionAssert.AllItemsAreNotNull(actual.Items);
        }

        [Test]
        public void OnClick_Always_NoExceptions()
        {
            var pipingContextMenu = new PipingContextMenuStrip(null);
            pipingContextMenu.Items[0].PerformClick();
        }

        [Test]
        public void OnClick_HandlerActionDefined_ActionIsExecuted()
        {
            var executed = false;
            var pipingContextMenu = new PipingContextMenuStrip(null);
            pipingContextMenu.OnCalculationClick += p => executed = true;
            pipingContextMenu.Items[0].PerformClick();
            Assert.IsTrue(executed);
        }
    }
}