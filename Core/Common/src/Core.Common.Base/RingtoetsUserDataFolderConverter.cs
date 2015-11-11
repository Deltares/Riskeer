using System;
using System.IO;
using Core.Common.Utils;
using log4net.Util;

namespace Core.Common.Base
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

            // HACK: don't keep log files for tests
            if (settingsDirectory.ToLower().Contains("tests"))
            {
                daysToKeepLogFiles = 0;
            }

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

/* Example: 
 * <file type="log4net.Util.PatternString">
        <converter>
          <name value="dsuserdata" />
          <type value="Core.Common.Base.RingtoetsUserDataFolderConverter,Ringtoets.Core" />
        </converter>
        <conversionPattern value="%dsuserdata\log-file.txt" />
      </file>
 */