using Core.Common.Base;
using Core.Common.Gui;
using Core.Common.TestUtils;
using Core.Common.Utils.Collections;
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
            var applicationPlugin = mocks.Stub<ApplicationPlugin>();

            mocks.ReplayAll();

            var applicationCore = new ApplicationCore();

            applicationCore.AddPlugin(applicationPlugin);

            gui.ApplicationCore = applicationCore;

            // Precondition
            Assert.AreEqual(1, applicationCore.Plugins.Count());

            // Call
            gui.Dispose();

            // Assert
            Assert.AreEqual(0, applicationCore.Plugins.Count());

            mocks.VerifyAll();
        }

        [Test]
        public void CheckViewPropertyEditorIsInitialized()
        {
            Assert.NotNull(ViewPropertyEditor.Gui);
        }
    }
}