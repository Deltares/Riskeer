using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using Core.Common.Base;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Common.Tests.Core
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
        public void ApplicationRunCanBeCalledOnlyOnce()
        {
            using (var app = new DeltaShellApplication())
            {
                app.Run();
                app.Run();
            }
        }

        [Test]
        public void ActivatePlugins()
        {
            var mocks = new MockRepository();

            var plugin = mocks.StrictMock<ApplicationPlugin>();

            Expect.Call(plugin.Application = null).IgnoreArguments();
            Expect.Call(plugin.Deactivate);
            Expect.Call(plugin.GetDataItemInfos()).Return(new List<DataItemInfo>()).Repeat.Any();
            Expect.Call(plugin.GetFileImporters()).Return(new List<IFileImporter>());
            Expect.Call(plugin.GetFileExporters()).Return(new List<IFileExporter>());

            plugin.Activate();
            LastCall.Repeat.Once();

            mocks.ReplayAll();

            using (var deltaShellApplication = new DeltaShellApplication())
            {
                deltaShellApplication.Plugins.Add(plugin);
                deltaShellApplication.Run();

                deltaShellApplication.Dispose();

                mocks.VerifyAll();
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