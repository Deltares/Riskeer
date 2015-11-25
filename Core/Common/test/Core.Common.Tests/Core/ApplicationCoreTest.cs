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
        public void ActivatePlugins()
        {
            var mocks = new MockRepository();

            var plugin = mocks.StrictMock<ApplicationPlugin>();

            using (var applicationCore = new ApplicationCore())
            {
                Expect.Call(plugin.Deactivate);
                Expect.Call(plugin.GetDataItemInfos()).Return(new List<DataItemInfo>()).Repeat.Any();

                plugin.Activate();

                LastCall.Repeat.Once();

                mocks.ReplayAll();

                applicationCore.AddPlugin(plugin);
                applicationCore.Dispose();

                mocks.VerifyAll();
            }
        }
    }
}