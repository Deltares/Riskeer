using NUnit.Framework;
using Wti.Forms.NodePresenters;

namespace Wti.Forms.Test
{
    public class PipingContextMenuStripTest
    {
        [Test]
        public void Constructor_Always_ValidateAndCalculateItemsAreAdded()
        {
            var actual = new PipingContextMenuStrip(null);
            Assert.AreEqual(2, actual.Items.Count);
            CollectionAssert.AllItemsAreNotNull(actual.Items);
        }

        [Test]
        public void OnClick_CalculateItemAlways_NoExceptions()
        {
            var pipingContextMenu = new PipingContextMenuStrip(null);
            pipingContextMenu.Items[1].PerformClick();
        }

        [Test]
        public void OnClick_ValidateItemAlways_NoExceptions()
        {
            var pipingContextMenu = new PipingContextMenuStrip(null);
            pipingContextMenu.Items[0].PerformClick();
        }

        [Test]
        public void OnClick_CalculateItemOnCalculateHandlerActionDefined_ActionIsExecuted()
        {
            var executed = false;
            var pipingContextMenu = new PipingContextMenuStrip(null);
            pipingContextMenu.OnCalculationClick += p => executed = true;
            pipingContextMenu.Items[1].PerformClick();
            Assert.IsTrue(executed);
        }

        [Test]
        public void OnClick_ValidateItemOnValidateHandlerActionDefined_ActionIsExecuted()
        {
            var executed = false;
            var pipingContextMenu = new PipingContextMenuStrip(null);
            pipingContextMenu.OnValidationClick += p => executed = true;
            pipingContextMenu.Items[0].PerformClick();
            Assert.IsTrue(executed);
        }
    }
}