using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Core.Common.Base.Properties;
using Core.Common.BaseDelftTools;
using Core.Common.Utils;
using Core.Common.Utils.Collections;
using Core.Common.Utils.IO;
using Core.Common.Utils.Reflection;
using Mono.Addins;

namespace Core.Common.Base
{
    /// <summary>
    /// This class wraps the MonoAddins AddinManager for the sole purpose of providing similar functionality under test. Bleh.
    /// </summary>
    public static class PluginManager
    {
        private static readonly List<IPlugin> AllPlugins = new List<IPlugin>();
        private static bool testMode;

        public static void RegisterAdditionalPlugins(IEnumerable<IPlugin> plugins)
        {
            AllPlugins.AddRange(plugins);
        }

        public static void Initialize(string pluginsDirectory)
        {
            if (pluginsDirectory != null)
            {
                pluginsDirectory = Path.GetFullPath(pluginsDirectory);

                // load all assemblies since they can be located in different directory
                var pluginDirectories = FileUtils.GetDirectoriesRelative(pluginsDirectory);

                foreach (var pluginDirectory in pluginDirectories)
                {
                    var path = Path.Combine(pluginsDirectory, pluginDirectory);
                    AssemblyUtils.LoadAllAssembliesFromDirectory(path).ToList();
                }

                var registryPath = SettingsHelper.GetApplicationLocalUserSettingsDirectory();

                // Initialize using the provided (DeltaShellApplication) assembly (OpenMi is started as dll and so the EntryAssembly and the CallingAssembly are incorrect). 
                var assembly = typeof(DeltaShellApplication).Assembly;
                var methodInitialize = AddinManager.AddinEngine.GetType().GetMethods(BindingFlags.NonPublic | BindingFlags.Instance).First(m => m.Name == "Initialize");

                // initialize AddinEngine (without console messages -> internally the Initialize call will run Registry.Update with a verbose console logger (only when there is no cache))
                WithoutConsoleMessage(() => methodInitialize.Invoke(AddinManager.AddinEngine, new object[]
                {
                    assembly,
                    registryPath,
                    pluginsDirectory,
                    null
                }));

                // Force local scanning (disable isolation)
                AddinManager.Registry.RegisterExtension(new ForceLocalAddinFileSystemExtension());
                AddinManager.Registry.Update(new MonoAddinsConsoleLogger(2)
                {
                    LogAssemblyScanErrors = false
                });

                // Reset addin roots
                AddinManager.Registry.ResetConfiguration();

                // Activate mono addin roots
                AddinManager.Registry.GetAddinRoots()
                            .Where(m => !AddinManager.IsAddinLoaded(m.Id))
                            .ForEach(m => AddinManager.LoadAddin(new ConsoleProgressStatus(0), m.Id));

                testMode = false;
            }
            else
            {
                testMode = true;
            }
        }

        public static IEnumerable<T> GetPlugins<T>() where T : IPlugin
        {
            var plugins = (testMode
                               ? AllPlugins.OfType<T>()
                               : AddinManager.GetExtensionObjects<IPlugin>().OfType<T>())
                .OrderBy(p => p.GetType().FullName) //order by name (arbitrary) for deterministic loading order
                .ToList();

            ThrowOnNonUniquePluginNames(plugins);

            return plugins;
        }

        public static void Reset()
        {
            AllPlugins.Clear();
        }

        [Conditional("DEBUG")]
        private static void ThrowOnNonUniquePluginNames<T>(IList<T> plugins) where T : IPlugin
        {
            var names = plugins.Select(p => p.Name).ToList();
            var distinctNames = names.Distinct().ToList();

            if (names.Count == distinctNames.Count)
            {
                return; //no problems
            }
            distinctNames.ForEach(name => names.Remove(name));

            var nonUniquePlugins = plugins.Where(p => names.Contains(p.Name));

            throw new InvalidOperationException(
                string.Format(Resources.PluginManager_ThrowOnNonUniquePluginNames_Multiple_plugins_with_the_same_name_s___0__1_,
                              Environment.NewLine,
                              string.Join(Environment.NewLine, nonUniquePlugins.Select(p => p.GetType().Name).ToArray())));
        }

        private static void WithoutConsoleMessage(Action action)
        {
            Console.SetOut(new StringWriter());

            try
            {
                action();
            }
            finally
            {
                Console.SetOut(new StreamWriter(Console.OpenStandardOutput())
                {
                    AutoFlush = true
                });
            }
        }
    }
}