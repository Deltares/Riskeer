﻿///////////////////////////////////////////////////////////////////////////////
//
// This file was automatically generated by RANOREX.
// Your custom recording code should go in this file.
// The designer will only add methods to this file, so your custom code won't be overwritten.
// http://www.ranorex.com
//
///////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

using WinForms = System.Windows.Forms;
using Ranorex;
using Ranorex.Core;
using Ranorex.Core.Repository;
using Ranorex.Core.Testing;

namespace AutomatedSystemTests.Modules.ActionsAUT
{
    public partial class StartAUT
    {	
        /// <summary>
        /// This method gets called right after the recording has been started.
        /// It can be used to execute recording specific initialization code.
        /// </summary>
        private void Init()
        {
            // Your recording specific initialization code goes here.
        }

        public void ResolveAppPath()
        {
        	AppPath = Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), AppPath);
        }

        public void StartAUT_Run_application()
        {
            Report.Log(ReportLevel.Info, "Application", "Run application with file name from variable $AppPath in normal mode. Return value bound to $StartAutProcessIDVar.");
            //bool succesfulStartUp = false;
            for (int i = 1; i < 11; i++) {
                Report.Info("Attempt #" + i.ToString() + " to start up the application.");
                StartAutProcessIDVar = ValueConverter.ToString(Host.Local.RunApplication(AppPath, "", "", false));
                repo.RiskeerMainWindow.SelfInfo.WaitForExists(120000);
                Delay.Duration(1000, false);
                try {
                    repo.RiskeerMainWindow.ProjectExplorer.ProjectRootNode.SelfInfo.WaitForExists(5000);
                    Report.Info("Application started up properly!");
                    i=20;
                } catch (Exception e) {
                    Report.Warn("Application not started up properly!");
                    Report.Warn("Reboot is required.");
                    Report.Info("Exception: " + e.ToString());
                    Delay.Duration(5000, false);
                    Host.Current.KillApplication(repo.RiskeerMainWindow.Self);
                    Delay.Duration(1000, false);
                    Report.Info("Rebooting...");
                }
            }
        }

    }
}
