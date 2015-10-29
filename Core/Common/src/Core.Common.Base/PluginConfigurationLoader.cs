using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using Core.Common.Base.Properties;
using Core.Common.Utils.Reflection;
using log4net;

namespace Core.Common.Base
{
    /// <summary>
    /// Used to load plugin configurations from a specified directory.
    /// </summary>
    public class PluginConfigurationLoader
    {
        private static readonly ILog log = LogManager.GetLogger("PluginLoader");

        private readonly string directory;

        private readonly IList<string> disabledPlugins = new List<string>();

        private readonly IDictionary<string, string> loadedAssemblyConfigFiles = new Dictionary<string, string>();
        private IEnumerable<Assembly> assemblies;

        public PluginConfigurationLoader(string directory)
        {
            this.directory = directory;
        }

        public void FillPluginConfigurationsFromPath<TPluginConfig, TPluginConfigurationSectionHandler>(IList<TPluginConfig> pluginConfigs, IList<Assembly> pluginAssemblies)
            where TPluginConfigurationSectionHandler : IConfigurationSectionHandler, new()
            where TPluginConfig : class
        {
            if (assemblies == null)
            {
                assemblies = LoadAssembliesWithConfigInDirectory(directory);
            }

            foreach (var assembly in assemblies)
            {
                var config = GetPluginConfig<TPluginConfig, TPluginConfigurationSectionHandler>(loadedAssemblyConfigFiles[Path.GetFileNameWithoutExtension(assembly.Location)]);

                if (config == null)
                {
                    continue;
                }

                pluginConfigs.Add(config);
                pluginAssemblies.Add(assembly);
            }
        }

        public static TPluginConfig GetPluginConfig<TPluginConfig, TPluginConfigurationSectionHandler>(string appConfigFilePath)
            where TPluginConfigurationSectionHandler : IConfigurationSectionHandler, new()
            where TPluginConfig : class
        {
            try
            {
                if (!File.Exists(appConfigFilePath))
                {
                    log.WarnFormat(Resources.PluginConfigurationLoader_GetPluginConfig_plugin_configuration_file___0___not_found__skipped_, appConfigFilePath);
                    return default(TPluginConfig);
                }

                // find XML element with a name equal to configuration class name (e.g. applicationPlugin, guiPlugin)
                var nodeName = typeof(TPluginConfig).Name;

                var doc = new XmlDocument();
                doc.Load(new XmlTextReader(appConfigFilePath));

                var nodes = doc.GetElementsByTagName(nodeName);

                foreach (XmlNode node in nodes)
                {
                    if (node.LocalName == nodeName)
                    {
                        var handler = new TPluginConfigurationSectionHandler();
                        return (TPluginConfig) handler.Create(null, null, node);
                    }
                }
            }
            catch (Exception exception)
            {
                log.ErrorFormat(Resources.PluginConfigurationLoader_GetPluginConfig_plugin_could_not_be_loaded____0_____1_, appConfigFilePath, exception.Message);
                return default(TPluginConfig);
            }

            return default(TPluginConfig);
        }

        private IEnumerable<Assembly> LoadAssembliesWithConfigInDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                throw new DirectoryNotFoundException(string.Format(Resources.PluginConfigurationLoader_LoadAssembliesWithConfigInDirectory_Can_t_find_directory_to_load_assemblies_from___0___fix_your_configuration_file, path));
            }

            foreach (var directoryPath in Directory.GetDirectories(path).Concat(new[]
            {
                path
            }))
            {
                foreach (string filename in Directory.GetFiles(directoryPath).Where(name => name.EndsWith(".dll.config")))
                {
                    loadedAssemblyConfigFiles[Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(filename))] = filename;
                }

                foreach (var assembly in AssemblyUtils.LoadAllAssembliesFromDirectory(Path.GetFullPath(directoryPath)))
                {
                    if (CanContainAPlugin(assembly))
                    {
                        yield return assembly;
                    }
                }
            }
        }

        /// <summary>
        /// Determines whether the assembly should be taken into account on the search for T implementations
        /// </summary>
        /// <param name="assembly">Assembly to inspect</param>
        /// <returns></returns>
        private bool CanContainAPlugin(Assembly assembly)
        {
            if (TypeUtils.IsDynamic(assembly))
            {
                return false;
            }

            var fileName = assembly.Location;
            if (string.IsNullOrEmpty(fileName))
            {
                return false; //no 'location' gac or something we don't want
            }

            string pluginDllName = fileName;

            if (disabledPlugins != null && disabledPlugins.Contains(pluginDllName))
            {
                log.DebugFormat(Resources.PluginConfigurationLoader_CanContainAPlugin_Skipping_the_loading_of__0_, pluginDllName);
                return false;
            }

            if (!loadedAssemblyConfigFiles.ContainsKey(Path.GetFileNameWithoutExtension(pluginDllName)))
            {
                return false;
            }

            return true;
        }
    }
}