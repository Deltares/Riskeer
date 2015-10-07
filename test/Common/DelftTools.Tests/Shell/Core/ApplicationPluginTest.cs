using System.Linq;

using DelftTools.Shell.Core;

using NUnit.Framework;

namespace DelftTools.Tests.Shell.Core
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
            Assert.IsFalse(applicationPlugin.IsActive);
            Assert.IsNull(applicationPlugin.Image);
            Assert.IsNull(applicationPlugin.Resources);
            Assert.IsNull(applicationPlugin.Application);
            CollectionAssert.IsEmpty(applicationPlugin.DependentPluginNames);
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

        [Test]
        public void GetPersistentAssemblies_ReturnEmptyEnummerable()
        {
            // setup
            var applicationPlugin = new SimpleApplicationPlugin();

            // call
            var importers = applicationPlugin.GetPersistentAssemblies().ToArray();

            // assert
            CollectionAssert.IsEmpty(importers);
        }

        [Test]
        public void Activate_PluginWasNotActivated_SetIsActiveTrue()
        {
            // setup
            var applicationPlugin = new SimpleApplicationPlugin();

            // call
            applicationPlugin.Activate();

            // assert
            Assert.IsTrue(applicationPlugin.IsActive);
        }

        [Test]
        public void Deactivate_PluginWasActivated_SetIsActiveFalse()
        {
            // setup
            var applicationPlugin = new SimpleApplicationPlugin();
            applicationPlugin.Activate();

            // call
            applicationPlugin.Deactivate();

            // assert
            Assert.IsFalse(applicationPlugin.IsActive);
        }
    }

    class SimpleApplicationPlugin : ApplicationPlugin
    {
        public override string Name
        {
            get
            {
                throw new System.NotImplementedException();
            }
        }

        public override string DisplayName
        {
            get
            {
                throw new System.NotImplementedException();
            }
        }

        public override string Description
        {
            get
            {
                throw new System.NotImplementedException();
            }
        }

        public override string Version
        {
            get
            {
                throw new System.NotImplementedException();
            }
        }
    }
}