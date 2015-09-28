﻿using System.Collections.Specialized;
using System.Configuration;
using DelftTools.Utils;

namespace DeltaShell.Gui.Properties {
    
    
    // This class allows you to handle specific events on the settings class:
    //  The SettingChanging event is raised before a setting's value is changed.
    //  The PropertyChanged event is raised after a setting's value is changed.
    //  The SettingsLoaded event is raised after the setting values are loaded.
    //  The SettingsSaving event is raised before the setting values are saved.
    
    [SettingsProvider(typeof(PortableSettingsProvider))]
    internal sealed partial class Settings {
        
        public Settings()
        {
            PortableSettingsProvider.SettingsFileName = System.IO.Path.Combine(SettingsHelper.GetApplicationLocalUserSettingsDirectory(), "user.config");
            //add default intances for collections
            if (mruList == null)
            {
                mruList = new StringCollection();
            }
            if (defaultViews == null)
            {
                defaultViews = new StringCollection();
            }
            if (disabledPlugins == null)
            {
                disabledPlugins = new StringCollection();
            }
            if (defaultViewDataTypes == null)
            {
                defaultViewDataTypes = new StringCollection();
            }

            // // To add event handlers for saving and changing settings, uncomment the lines below:
            //
            // this.SettingChanging += this.SettingChangingEventHandler;
            //
            // this.SettingsSaving += this.SettingsSavingEventHandler;
            //
        }
        
        private void SettingChangingEventHandler(object sender, System.Configuration.SettingChangingEventArgs e) {
            // Add code to handle the SettingChangingEvent event here.
        }
        
        private void SettingsSavingEventHandler(object sender, System.ComponentModel.CancelEventArgs e) {
            // Add code to handle the SettingsSaving event here.
        }
    }
}
