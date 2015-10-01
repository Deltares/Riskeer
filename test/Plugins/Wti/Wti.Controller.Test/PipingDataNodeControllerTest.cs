using System;
using DelftTools.Controls;
using DelftTools.Controls.Swf;
using NUnit.Framework;
using Wti.Data;
using Wti.Forms.NodePresenters;

namespace Wti.Controller.Test
{
    public class PipingDataNodeControllerTest
    {
        [Test]
        public void GivenDefaultPipingDataNodeController_WhenObtainingTheNodePresenter_ThenANodePresenterInstanceIsReturned()
        {
            PipingDataNodePresenter actual = new PipingDataNodeController().NodePresenter;
            Assert.That(actual, Is.Not.Null);
            Assert.That(actual, Is.InstanceOf<PipingDataNodePresenter>());
        }

        [Test]
        public void GivenAnObject_WhenObtainingAContextMenuBasedOnObject_ThenACastExceptionIsThrown()
        {
            object someData = new object();
            var pipingDataNodeController = new PipingDataNodeController();
            TestDelegate actual = () => pipingDataNodeController.GetContextMenu(someData);
            Assert.That(actual, Throws.InstanceOf<InvalidCastException>());
        }

        [Test]
        public void GivenPipingData_WhenObtainingAContextMenuBasedOnThePipingData_ThenAContextMenuAdapterWithAPipingContextMenuIsReturned()
        {
            var pipingData = new PipingData();
            var pipingDataNodeController = new PipingDataNodeController();
            IMenuItem actual = pipingDataNodeController.GetContextMenu(pipingData);
            Assert.That(actual, Is.Not.Null);
            Assert.That(actual, Is.InstanceOf<MenuItemContextMenuStripAdapter>());
            Assert.That(((MenuItemContextMenuStripAdapter)actual).ContextMenuStrip, Is.InstanceOf<PipingContextMenuStrip>());
        }
    }
}
