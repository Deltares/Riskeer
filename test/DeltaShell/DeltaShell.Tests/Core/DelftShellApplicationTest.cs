using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using DelftTools.Shell.Core;
using DelftTools.TestUtils;
using DeltaShell.Core;
using DeltaShell.Gui;
using DeltaShell.Tests.TestObjects;
using NUnit.Framework;
using Rhino.Mocks;

namespace DeltaShell.Tests.Core
{
    [TestFixture]
    public class DeltaShellApplicationTest
    {
        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ApplicationShouldRunBeforeProjectCanBeOpened()
        {
            using (var app = new DeltaShellApplication())
            {
                app.OpenProject(null);
            }
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ApplicationShouldRunBeforeProjectCanBeCreated()
        {
            using (var app = new DeltaShellApplication())
            {
                app.OpenProject(null);
            }
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        [Category(TestCategory.Integration)]
        public void ApplicationRunCanBeCalledOnlyOnce()
        {
            using (var app = new DeltaShellApplication())
            {
                app.Run();
                app.Run();
            }
        }

        [Test]
        [Category(TestCategory.Integration)]
        public void ActivatePlugins()
        {
            var mocks = new MockRepository();

            var plugin = mocks.StrictMock<ApplicationPlugin>();

            Expect.Call(plugin.IsActive).Repeat.Any().Return(true);
            Expect.Call(plugin.Application = null).IgnoreArguments();
            Expect.Call(plugin.Resources = null).IgnoreArguments();
            Expect.Call(plugin.Name).Return("mock plugin").Repeat.Any();
            Expect.Call(plugin.Deactivate);
            Expect.Call(plugin.GetDataItemInfos()).Return(new List<DataItemInfo>()).Repeat.Any();
            Expect.Call(plugin.GetFileImporters()).Return(new List<IFileImporter>());
            Expect.Call(plugin.GetFileExporters()).Return(new List<IFileExporter>());

            plugin.Activate(); LastCall.Repeat.Once();

            mocks.ReplayAll();

            using (var gui = new DeltaShellGui())
            {
                var app = gui.Application;
                gui.Application = app;
                gui.Application.Plugins.Add(plugin);
                gui.Run();

                gui.Dispose();

                mocks.VerifyAll();
            }
        }

        [Test]
        [Category(TestCategory.Integration)]
        public void DisposingApplicationClosesProjectOnProjectService()
        {
            using (var delftShellApplication = new DeltaShellApplication())
            {
                var service = new TestProjectService();
                delftShellApplication.ProjectService = service;
                delftShellApplication.Project = new Project();

                //action!
                delftShellApplication.Dispose();

                //assert dispose got called
                Assert.AreEqual(1, service.CloseCallCount);
            }
        }

        [Test]
        public void CurrentCultureIsChangedWhenTurkishOrAzeri()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            using (var delftShellApplication = new DeltaShellApplication())
            {
                delftShellApplication.Run();
                Assert.AreEqual("en-US", Thread.CurrentThread.CurrentCulture.ToString());
            }

            Thread.CurrentThread.CurrentCulture = new CultureInfo("nl-NL");
            using (var delftShellApplication = new DeltaShellApplication())
            {
                delftShellApplication.Run();
                Assert.AreEqual("nl-NL", Thread.CurrentThread.CurrentCulture.ToString());
            }

            Thread.CurrentThread.CurrentCulture = new CultureInfo("tr-TR");
            using (var delftShellApplication = new DeltaShellApplication())
            {
                delftShellApplication.Run();
                Assert.AreEqual("en-US", Thread.CurrentThread.CurrentCulture.ToString());
            }

            Thread.CurrentThread.CurrentCulture = new CultureInfo("az");
            using (var delftShellApplication = new DeltaShellApplication())
            {
                delftShellApplication.Run();
                Assert.AreEqual("en-US", Thread.CurrentThread.CurrentCulture.ToString());
            }
        }
    }
}