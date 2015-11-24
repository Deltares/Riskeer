using Core.Common.Base;
using Core.Common.Gui;
using Core.Common.TestUtils;
using log4net.Core;
using NUnit.Framework;

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
            var applicationCore = new ApplicationCore();

            gui.ApplicationCore = applicationCore;

            // Precondition
            Assert.IsNotNull(applicationCore.Plugins);

            // Call
            gui.Dispose();

            // Assert
            Assert.IsNull(applicationCore.Plugins);
        }

        [Test]
        public void CheckViewPropertyEditorIsInitialized()
        {
            Assert.NotNull(ViewPropertyEditor.Gui);
        }
    }
}