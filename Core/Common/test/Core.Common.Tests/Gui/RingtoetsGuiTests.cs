using Core.Common.Base;
using Core.Common.Gui;
using Core.Common.TestUtils;
using log4net.Core;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Common.Tests.Gui
{
    [TestFixture]
    public class RingtoetsGuiTests
    {
        private RingtoetsGui gui;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            LogHelper.ConfigureLogging();
        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            LogHelper.ResetLogging();
        }

        [SetUp]
        public void SetUp()
        {
            LogHelper.SetLoggingLevel(Level.Error);

            gui = new RingtoetsGui();
        }

        [TearDown]
        public void TearDown()
        {
            gui.Dispose();
        }

        [Test]
        public void DisposingGuiDisposesApplication()
        {
            // Setup
            var mocks = new MockRepository();
            var applicationCore = mocks.Stub<ApplicationCore>();

            applicationCore.Expect(ac => ac.Dispose()).Repeat.Once();

            mocks.ReplayAll();

            gui.ApplicationCore = applicationCore;

            // Call
            gui.Dispose();

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void CheckViewPropertyEditorIsInitialized()
        {
            Assert.NotNull(ViewPropertyEditor.Gui);
        }
    }
}