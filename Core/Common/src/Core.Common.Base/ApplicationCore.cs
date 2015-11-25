using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using Core.Common.Base.Workflow;
using Core.Common.Utils;
using Core.Common.Utils.Globalization;

namespace Core.Common.Base
{
    public class ApplicationCore : IDisposable
    {
        private readonly ActivityRunner activityRunner;
        private readonly List<ApplicationPlugin> plugins;

        private bool userSettingsDirty;
        private ApplicationSettingsBase userSettings;

        public ApplicationCore()
        {
            plugins = new List<ApplicationPlugin>();
            activityRunner = new ActivityRunner();

            Settings = ConfigurationManager.AppSettings;
            UserSettings = Properties.Settings.Default;

            if (RunningActivityLogAppender.Instance != null)
            {
                RunningActivityLogAppender.Instance.ActivityRunner = ActivityRunner;
            }
        }

        public IEnumerable<ApplicationPlugin> Plugins
        {
            get
            {
                return plugins;
            }
        }

        public IActivityRunner ActivityRunner
        {
            get
            {
                return activityRunner;
            }
        }

        public ApplicationSettingsBase UserSettings
        {
            get
            {
                return userSettings;
            }
            set
            {
                if (userSettings != null)
                {
                    userSettings.PropertyChanged -= UserSettingsPropertyChanged;
                }

                userSettings = value;

                if (userSettings != null)
                {
                    userSettings.PropertyChanged += UserSettingsPropertyChanged;
                }

                userSettingsDirty = false;
            }
        }

        public NameValueCollection Settings { get; set; }

        public IEnumerable<IFileImporter> FileImporters
        {
            get
            {
                return Plugins.SelectMany(plugin => plugin.GetFileImporters());
            }
        }

        public IEnumerable<IFileExporter> FileExporters
        {
            get
            {
                return Plugins.SelectMany(plugin => plugin.GetFileExporters());
            }
        }

        public static void SetLanguageAndRegionalSettions(ApplicationSettingsBase tempUserSettings = null)
        {
            var settings = ConfigurationManager.AppSettings;

            var language = settings["language"];

            if (language != null)
            {
                RegionalSettingsManager.Language = language;
            }

            if (tempUserSettings != null && tempUserSettings.Properties.Count > 0)
            {
                var realNumberFormat = tempUserSettings["realNumberFormat"];
                if (realNumberFormat != null)
                {
                    RegionalSettingsManager.RealNumberFormat = (string) realNumberFormat;
                }
            }
        }

        public string GetUserSettingsDirectoryPath()
        {
            return SettingsHelper.GetApplicationLocalUserSettingsDirectory();
        }

        public void AddPlugin(ApplicationPlugin applicationPlugin)
        {
            plugins.Add(applicationPlugin);

            applicationPlugin.Activate();
        }

        public void RemovePlugin(ApplicationPlugin applicationPlugin)
        {
            plugins.Remove(applicationPlugin);

            applicationPlugin.Deactivate();
        }

        public void Exit()
        {
            if (userSettingsDirty)
            {
                UserSettings.Save();
            }
        }

        public void Dispose()
        {
            foreach (var plugin in Plugins.ToList())
            {
                RemovePlugin(plugin);
            }

            if (RunningActivityLogAppender.Instance != null)
            {
                RunningActivityLogAppender.Instance.ActivityRunner = null;
            }
        }

        private void UserSettingsPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            userSettingsDirty = true;
        }
    }
}