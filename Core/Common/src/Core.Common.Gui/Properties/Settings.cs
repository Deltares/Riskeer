// Copyright (C) Stichting Deltares 2016. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of 
// Stichting Deltares and remain full property of Stichting Deltares at all times. 
// All rights reserved.

using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.IO;

using Core.Common.Gui.Settings;

namespace Core.Common.Gui.Properties
{
    // This class allows you to handle specific events on the settings class:
    //  The SettingChanging event is raised before a setting's value is changed.
    //  The PropertyChanged event is raised after a setting's value is changed.
    //  The SettingsLoaded event is raised after the setting values are loaded.
    //  The SettingsSaving event is raised before the setting values are saved.

    [SettingsProvider(typeof(PortableSettingsProvider))]
    public sealed partial class Settings
    {
        public Settings()
        {
            PortableSettingsProvider.SettingsFileName = Path.Combine(SettingsHelper.GetApplicationLocalUserSettingsDirectory(), "user.config");
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

        private void SettingChangingEventHandler(object sender, SettingChangingEventArgs e)
        {
            // Add code to handle the SettingChangingEvent event here.
        }

        private void SettingsSavingEventHandler(object sender, CancelEventArgs e)
        {
            // Add code to handle the SettingsSaving event here.
        }
    }
}