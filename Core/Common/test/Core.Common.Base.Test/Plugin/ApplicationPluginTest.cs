using System.Linq;
using Core.Common.Base.Plugin;
using NUnit.Framework;

namespace Core.Common.Base.Test.Plugin
{
    [TestFixture]
    public class ApplicationPluginTest
    {
        [Test]
        public void GetFileImporters_ReturnEmptyEnumerable()
        {
            // Setup
            var applicationPlugin = new SimpleApplicationPlugin();

            // Call
            var importers = applicationPlugin.GetFileImporters().ToArray();

            // Assert
            CollectionAssert.IsEmpty(importers);
        }

        [Test]
        public void GetFileExporters_ReturnEmptyEnumerable()
        {
            // Setup
            var applicationPlugin = new SimpleApplicationPlugin();

            // Call
            var importers = applicationPlugin.GetFileExporters().ToArray();

            // Assert
            CollectionAssert.IsEmpty(importers);
        }

        [Test]
        public void GetDataItemInfos_ReturnEmptyEnumerable()
        {
            // Setup
            var applicationPlugin = new SimpleApplicationPlugin();

            // Call
            var importers = applicationPlugin.GetDataItemInfos().ToArray();

            // Assert
            CollectionAssert.IsEmpty(importers);
        }

        private class SimpleApplicationPlugin : ApplicationPlugin
        {

        }
    }
}