using NUnit.Framework;
using Wti.Controller;

namespace Wti.Forms.Test
{
    public class PipingContextMenuStripTest
    {
        [Test]
        public void GivenPipingContextMenu_WhenConstructedBasedOnAnything_ThenOneMenuItemIsAdded()
        {
            var actual = new PipingContextMenuStrip(null);
            CollectionAssert.AllItemsAreNotNull(actual.Items);
        }

        [Test]
        public void GivenPipingContextMenuWithoutHandlers_WhenClickingCalculateMenuItem_NoExceptions()
        {
            var pipingContextMenu = new PipingContextMenuStrip(null);
            pipingContextMenu.Items[0].PerformClick();
        }

        [Test]
        public void GivenPipingContextMenuWithCalculateHandler_WhenClickingCalculateMenuItem_ActionIsExecuted()
        {
            var executed = false;
            var pipingContextMenu = new PipingContextMenuStrip(null);
            pipingContextMenu.OnCalculationClick += p => executed = true;
            pipingContextMenu.Items[0].PerformClick();
            Assert.That(executed, Is.True);
        }
    }
}