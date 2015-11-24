using System.Linq;
using NUnit.Framework;

namespace Core.Common.Base.Tests.Shell.Core
{
    [TestFixture]
    public class ApplicationPluginTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // call
            var applicationPlugin = new SimpleApplicationPlugin();

            // assert
            Assert.IsNull(applicationPlugin.ApplicationCore);
        }

        [Test]
        public void GetFileImporters_ReturnEmptyEnummerable()
        {
            // setup
            var applicationPlugin = new SimpleApplicationPlugin();

            // call
            var importers = applicationPlugin.GetFileImporters().ToArray();

            // assert
            CollectionAssert.IsEmpty(importers);
        }

        [Test]
        public void GetFileExporters_ReturnEmptyEnummerable()
        {
            // setup
            var applicationPlugin = new SimpleApplicationPlugin();

            // call
            var importers = applicationPlugin.GetFileExporters().ToArray();

            // assert
            CollectionAssert.IsEmpty(importers);
        }

        [Test]
        public void GetDataItemInfos_ReturnEmptyEnummerable()
        {
            // setup
            var applicationPlugin = new SimpleApplicationPlugin();

            // call
            var importers = applicationPlugin.GetDataItemInfos().ToArray();

            // assert
            CollectionAssert.IsEmpty(importers);
        }
    }

    internal class SimpleApplicationPlugin : ApplicationPlugin
    {
    }
}