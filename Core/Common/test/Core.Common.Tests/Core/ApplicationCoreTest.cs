using System;
using System.Collections.Generic;
using Core.Common.Base;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Common.Tests.Core
{
    [TestFixture]
    public class ApplicationCoreTest
    {
        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ApplicationShouldRunBeforeProjectCanBeOpened()
        {
            using (var applicationCore = new ApplicationCore())
            {
                applicationCore.OpenProject(null);
            }
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ApplicationShouldRunBeforeProjectCanBeCreated()
        {
            using (var applicationCore = new ApplicationCore())
            {
                applicationCore.OpenProject(null);
            }
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ApplicationRunCanBeCalledOnlyOnce()
        {
            using (var applicationCore = new ApplicationCore())
            {
                applicationCore.Run();
                applicationCore.Run();
            }
        }

        [Test]
        public void ActivatePlugins()
        {
            var mocks = new MockRepository();

            var plugin = mocks.StrictMock<ApplicationPlugin>();

            Expect.Call(plugin.ApplicationCore = null).IgnoreArguments();
            Expect.Call(plugin.Deactivate);
            Expect.Call(plugin.GetDataItemInfos()).Return(new List<DataItemInfo>()).Repeat.Any();

            plugin.Activate();
            LastCall.Repeat.Once();

            mocks.ReplayAll();

            using (var applicationCore = new ApplicationCore())
            {
                applicationCore.Plugins.Add(plugin);
                applicationCore.Run();

                applicationCore.Dispose();

                mocks.VerifyAll();
            }
        }
    }
}