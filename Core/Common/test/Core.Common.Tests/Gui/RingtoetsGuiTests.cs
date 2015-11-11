using Core.Common.Gui;
using Core.Common.Tests.TestObjects;
using Core.Common.TestUtils;
using log4net.Core;
using NUnit.Framework;
using SharpTestsEx;

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
            var testApplication = new TestApplication();
            gui.Application = testApplication;

            //action!
            gui.Dispose();

            //assert
            Assert.AreEqual(1, testApplication.DisposeCallCount);
        }

        [Test]
        public void CheckViewPropertyEditorIsInitialized()
        {
            ViewPropertyEditor.Gui.Should().Not.Be.Null();
        }
    }
}