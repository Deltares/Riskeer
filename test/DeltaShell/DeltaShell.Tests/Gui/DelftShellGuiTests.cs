using System.Windows.Forms;
using DelftTools.Controls.Swf;
using DelftTools.Shell.Core;
using DelftTools.Shell.Gui;
using DelftTools.TestUtils;
using DelftTools.Utils;
using DeltaShell.Gui;
using DeltaShell.Tests.TestObjects;
using log4net.Core;
using NUnit.Framework;
using Rhino.Mocks;
using SharpTestsEx;

namespace DeltaShell.Tests.Gui
{
    [TestFixture]
    public class DeltaShellGuiTests
    {
        private readonly MockRepository mocks = new MockRepository();
        private DeltaShellGui gui;

        [SetUp]
        public void SetUp()
        {
            LogHelper.SetLoggingLevel(Level.Error);
            gui = new DeltaShellGui();
        }

        [TearDown]
        public void TearDown()
        {
            gui.Dispose();
        }

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

        [Test]
        [Category(TestCategory.Integration)]
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
        [Category(TestCategory.Integration)]
        public void CheckViewPropertyEditorIsInitialized()
        {
            ViewPropertyEditor.Gui.Should().Not.Be.Null();
        }

        [Test]
        [Category(TestCategory.Integration)]
        public void FireEventAfterRun()
        {
            var callCount = 0;
            gui.AfterRun += () => callCount++;
            gui.Run();

            callCount.Should("AfterRun event is fired after gui starts").Be.EqualTo(1);
        }
    }
}