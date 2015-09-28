using System;
using System.Configuration;
using System.IO;
using System.Linq;
using DelftTools.Utils;

namespace DeltaShell.Core
{
    //special folders not in latests stable (1.2.10) version of log4net hence this for DS user dir.
    //Any arguments are ignored and just just return c:\user\muurman\appdata\deltares\ds1.0.0.0\
    //from http://logging.apache.org/log4net/release/release-notes.html
    /// <summary>
    /// PatternConverter for 'Special' Folder 
    /// </summary>
    public class DeltaShellUserDataFolderConverter : log4net.Util.PatternConverter // NOTE: Class might be marked as unused, but it's actually created in DeltaShell.Core/app.config!
    {
        override protected void Convert(System.IO.TextWriter writer, object state)
        {
            // makes sure that the application log file is saved in a correct folder, settings it in DeltaShellApplication is not enough
            var settings = ConfigurationManager.AppSettings;
            if (settings.AllKeys.Contains("applicationName"))
            {
                SettingsHelper.ApplicationName = settings["applicationName"];
                SettingsHelper.ApplicationVersion = settings["fullVersion"];
            }

            var settingsDirectory = SettingsHelper.GetApplicationLocalUserSettingsDirectory();

            DeleteOldLogFiles(settingsDirectory);

            writer.Write(settingsDirectory);
        }

        private void DeleteOldLogFiles(string settingsDirectory)
        {
            var daysToKeepLogFiles = 30;

            // HACK: don't keep log files for tests
            if(settingsDirectory.ToLower().Contains("tests"))
            {
                daysToKeepLogFiles = 0;
            }

            var logFiles = Directory.GetFiles(settingsDirectory, "*.log");
            foreach (var logFile in logFiles)
            {
                if((DateTime.Now - File.GetCreationTime(logFile)).TotalDays > daysToKeepLogFiles)
                {
                    File.Delete(logFile);
                }
            }
        }
    }

}
/* Example: 
 * <file type="log4net.Util.PatternString">
        <converter>
          <name value="dsuserdata" />
          <type value="DeltaShell.Core.DeltaShellUserDataFolderConverter,DeltaShell.Core" />
        </converter>
        <conversionPattern value="%dsuserdata\log-file.txt" />
      </file>
 */
