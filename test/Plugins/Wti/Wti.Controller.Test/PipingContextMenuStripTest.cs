using System;
using NUnit.Framework;
using Wti.Data;

namespace Wti.Controller.Test
{
    public class PipingContextMenuStripTest
    {
        [Test]
        public void GivenSomeData_WhenPipingContextMenuStripConstructedBasedOnThisData_ThenOneMenuItemIsAdded()
        {
            PipingData someData = null;
            var actual = new PipingContextMenuStrip(someData);
            Assert.That(actual.Items, Has.Some.Not.Null);
        }

        [Test]
        public void GivenPipingContextMenuStripConstructedWithNull_WhenClickingCalculateMenuItem_ThenNullReferenceExceptionIsThrown()
        {
            var pipingContextMenu = new PipingContextMenuStrip(null);
            TestDelegate actual = () => pipingContextMenu.Items[0].PerformClick();
            Assert.That(actual, Throws.InstanceOf<NullReferenceException>());
        }

        [Test]
        public void GivenPipingContextMenuStripConstructedWithPipingData_WhenClickingCalculateMenuItem_ThenNoNullReferenceExceptions()
        {
            var pipingContextMenu = new PipingContextMenuStrip(new PipingData());
            TestDelegate actual = () => pipingContextMenu.Items[0].PerformClick();
            Assert.That(actual, Throws.Exception.Not.InstanceOf<NullReferenceException>());
        }
    }
}