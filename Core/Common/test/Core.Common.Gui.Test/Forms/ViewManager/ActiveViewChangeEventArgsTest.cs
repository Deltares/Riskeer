using System;

using Core.Common.Controls.Views;
using Core.Common.Gui.Forms.ViewManager;

using NUnit.Framework;

using Rhino.Mocks;

namespace Core.Common.Gui.Test.Forms.ViewManager
{
    [TestFixture]
    public class ActiveViewChangeEventArgsTest
    {
        [Test]
        public void ParameteredConstructor_OnlyActiveView_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var activeView = mocks.Stub<IView>();
            mocks.ReplayAll();

            // Call
            var eventArgs = new ActiveViewChangeEventArgs(activeView);

            // Assert
            Assert.IsInstanceOf<EventArgs>(eventArgs);
            Assert.AreSame(activeView, eventArgs.View);
            Assert.IsNull(eventArgs.OldView);

            mocks.VerifyAll();
        }

        [Test]
        public void ParameteredConstructor_SwitchedToOtherView_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var activeView = mocks.Stub<IView>();
            var previouslyActiveView = mocks.Stub<IView>();
            mocks.ReplayAll();

            // Call
            var eventArgs = new ActiveViewChangeEventArgs(activeView, previouslyActiveView);

            // Assert
            Assert.IsInstanceOf<EventArgs>(eventArgs);
            Assert.AreSame(activeView, eventArgs.View);
            Assert.AreSame(previouslyActiveView, eventArgs.OldView);

            mocks.VerifyAll();
        }
    }
}