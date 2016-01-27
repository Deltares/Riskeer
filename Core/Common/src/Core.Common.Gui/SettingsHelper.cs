using System;
using System.IO;
using System.Reflection;

using Core.Common.Utils.Reflection;

namespace Core.Common.Gui
{
    public static class SettingsHelper
    {
        static SettingsHelper()
        {
            //set defaults based on executing assembly
            var info = AssemblyUtils.GetExecutingAssemblyInfo();
            ApplicationName = info.Product;
            ApplicationVersion = info.Version;
            ApplicationCompany = info.Company;
        }

        public static string ApplicationName { get; private set; }

        public static string ApplicationVersion { get; private set; }

        public static string ApplicationCompany { get; private set; }

        public static string GetApplicationLocalUserSettingsDirectory()
        {
            var localSettingsDirectoryPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var executingAssembly = Assembly.GetExecutingAssembly();
            var assemblyInfo = AssemblyUtils.GetAssemblyInfo(executingAssembly);
            var companySettingsDirectoryPath = Path.Combine(localSettingsDirectoryPath, assemblyInfo.Company);

            var appSettingsDirectoryPath = Path.Combine(companySettingsDirectoryPath, ApplicationName + " " + ApplicationVersion);

            if (!Directory.Exists(appSettingsDirectoryPath))
            {
                Directory.CreateDirectory(appSettingsDirectoryPath);
            }

            return appSettingsDirectoryPath;
        }
    }
}