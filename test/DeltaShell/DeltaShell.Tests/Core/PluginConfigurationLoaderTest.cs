using System.IO;
using DeltaShell.Plugins.CommonTools;
using NUnit.Framework;

namespace DeltaShell.Tests.Core
{
    [TestFixture]
    public class PluginConfigurationLoaderTest
    {
        private readonly string path = Path.GetFullPath("../../../../../src/DeltaShell/DeltaShell.Loader/bin/plugins/DeltaShell.Plugins.CommonTools");

        [SetUp]
        public void SetUp()
        {
            var plugin = new CommonToolsApplicationPlugin(); // make sure that plugin assembly + dependencies are loaded, normally this is done by PluginLoader
        }

        /// <summary>
        /// Pre-condition: The prebuild TestPlugin1 should exist in the path to run this test successful
        /// </summary>
        
/*        [Test]        
        [Category(TestCategory.Integration)]
        public void LoadPlugins()
        {
            LogHelper.ConfigureLogging();
            LogHelper.SetLoggingLevel(Level.Debug);
            
            var configurationLoader = new PluginConfigurationLoader(path);
            var pluginConfigs = new List<applicationPlugin>();
            var pluginAssemblies = new List<Assembly>();
            configurationLoader.FillPluginConfigurationsFromPath<applicationPlugin, ApplicationPluginConfigurationSectionHandler>(pluginConfigs, pluginAssemblies);
            Assert.AreEqual(1, pluginConfigs.Count);
            Assert.AreEqual(1, pluginAssemblies.Count);
            Assert.IsTrue(pluginConfigs[0].type.Contains("CommonTools"));
            
            LogHelper.ResetLogging();
        }

        [Test]
        [ExpectedException(typeof(DirectoryNotFoundException))]
        public void IncorrectPath()
        {
            var configurationLoader = new PluginConfigurationLoader("non existing");
            var pluginConfigs = new List<applicationPlugin>();
            var pluginAssemblies = new List<Assembly>();
            configurationLoader.FillPluginConfigurationsFromPath<applicationPlugin, ApplicationPluginConfigurationSectionHandler>(pluginConfigs, pluginAssemblies);
        }*/
    }
}