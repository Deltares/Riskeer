using Core.Common.Base;
using Core.Common.Gui;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Common.Tests.Gui
{
    [TestFixture]
    public class RingtoetsGuiTests
    {
        [Test]
        public void DisposingGuiDisposesApplication()
        {
            // Setup
            var mocks = new MockRepository();
            var applicationCore = mocks.Stub<ApplicationCore>();

            applicationCore.Expect(ac => ac.Dispose()).Repeat.Once();

            mocks.ReplayAll();

            var gui = new RingtoetsGui(applicationCore);

            // Call
            gui.Dispose();

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void CheckViewPropertyEditorIsInitialized()
        {
            using (var gui = new RingtoetsGui())
            {
                Assert.NotNull(ViewPropertyEditor.Gui);
            }
        }
    }
}