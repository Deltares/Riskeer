using System;
using System.IO;
using System.Reflection;
using DelftTools.Utils.Reflection;

namespace DelftTools.Utils
{
    public static class SettingsHelper
    {
        static SettingsHelper()
        {
            //set defaults based on executing assembly
            var info = AssemblyUtils.GetExecutingAssemblyInfo();
            ApplicationName = info.Product;
            ApplicationVersion = info.Version;
        }

        public static string ApplicationNameAndVersion
        {
            get { return ApplicationName + " " + ApplicationVersion; }
        }

        public static string ApplicationName { get; set; }
        public static string ApplicationVersion { get; set; }
        
        private static string GetApplicationLocalUserSettingsDirectoryName()
        {
            var executingAssembly = Assembly.GetExecutingAssembly();
            var applicationVersionDirectory = ApplicationName + "-" + ApplicationVersion;

            // For debug versions, and releases ran from the zip, the ApplicationName & Version are not unique 
            // for different versions. As a result Mono.Addins tries to mix dlls from different installations,
            // either corrupting DeltaShell or loading plugins not in the installation being ran. To prevent all
            // this, we add a hash for the current installation directory to make sure the local settings folder 
            // is still unique.

            var uniqueInstallationHash = Path.GetFullPath(executingAssembly.Location).GetHashCode();
            applicationVersionDirectory += "_#" + Math.Abs(uniqueInstallationHash);

            return applicationVersionDirectory;
        }

        public static string GetApplicationLocalUserSettingsDirectory()
        {
            var localSettingsDirectoryPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var executingAssembly = Assembly.GetExecutingAssembly();
            var assemblyInfo = AssemblyUtils.GetAssemblyInfo(executingAssembly);
            var companySettingsDirectoryPath = Path.Combine(localSettingsDirectoryPath, assemblyInfo.Company);

            var appSettingsDirectoryPath = Path.Combine(companySettingsDirectoryPath, GetApplicationLocalUserSettingsDirectoryName());

            if (!Directory.Exists(appSettingsDirectoryPath))
            {
                Directory.CreateDirectory(appSettingsDirectoryPath);
            }

            return appSettingsDirectoryPath;
        }
    }
}