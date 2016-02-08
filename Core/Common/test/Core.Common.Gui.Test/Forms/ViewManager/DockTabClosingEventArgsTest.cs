using Core.Common.Controls.Views;
using Core.Common.Gui.Forms.ViewManager;

using NUnit.Framework;

using Rhino.Mocks;

namespace Core.Common.Gui.Test.Forms.ViewManager
{
    [TestFixture]
    public class DockTabClosingEventArgsTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Setup

            // Call
            var eventArgs = new DockTabClosingEventArgs();

            // Assert
            Assert.IsNull(eventArgs.View);
            Assert.IsFalse(eventArgs.Cancel);
        }

        [Test]
        public void SimpleProperties_SetNewValue_GetNewValue()
        {
            // Setup
            var mocks = new MockRepository();
            var view = mocks.Stub<IView>();
            mocks.ReplayAll();

            var eventArgs = new DockTabClosingEventArgs();

            // Call
            eventArgs.View = view;
            eventArgs.Cancel = true;

            // Assert
            Assert.AreEqual(view, eventArgs.View);
            Assert.IsTrue(eventArgs.Cancel);

            mocks.VerifyAll();
        }
    }
}