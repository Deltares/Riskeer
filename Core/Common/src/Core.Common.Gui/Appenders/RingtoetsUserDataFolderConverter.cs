// Copyright (C) Stichting Deltares 2016. All rights preserved.
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
// All rights preserved.

using System;
using System.IO;
using Core.Common.Utils;
using log4net.Util;

namespace Core.Common.Gui.Appenders
{
    //special folders not in latests stable (1.2.10) version of log4net hence this for DS user dir.
    //Any arguments are ignored and just just return c:\user\muurman\appdata\deltares\ds1.0.0.0\
    //from http://logging.apache.org/log4net/release/release-notes.html
    /// <summary>
    /// PatternConverter for 'Special' Folder 
    /// </summary>
 
    public class RingtoetsUserDataFolderConverter : PatternConverter // NOTE: Class might be marked as unused, but it's actually created in Application.Ringtoets/app.config!
    {
        protected override void Convert(TextWriter writer, object state)
        {
            var settingsDirectory = SettingsHelper.GetApplicationLocalUserSettingsDirectory();

            DeleteOldLogFiles(settingsDirectory);

            writer.Write(settingsDirectory);
        }

        private void DeleteOldLogFiles(string settingsDirectory)
        {
            var daysToKeepLogFiles = 30;

            var logFiles = Directory.GetFiles(settingsDirectory, "*.log");
            foreach (var logFile in logFiles)
            {
                if ((DateTime.Now - File.GetCreationTime(logFile)).TotalDays > daysToKeepLogFiles)
                {
                    File.Delete(logFile);
                }
            }
        }
    }

}