﻿///////////////////////////////////////////////////////////////////////////////
//
// This file was automatically generated by RANOREX.
// Your custom recording code should go in this file.
// The designer will only add methods to this file, so your custom code won't be overwritten.
// http://www.ranorex.com
//
///////////////////////////////////////////////////////////////////////////////

using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Threading;
using WinForms = System.Windows.Forms;

using Ranorex;
using Ranorex.Core;
using Ranorex.Core.Repository;
using Ranorex.Core.Testing;

namespace AutomatedSystemTests.Modules.IO
{
    public partial class ExportConfigurationSelectedCalculationToXMLFile
    {
        /// <summary>
        /// This method gets called right after the recording has been started.
        /// It can be used to execute recording specific initialization code.
        /// </summary>
        private void Init()
        {
            // Your recording specific initialization code goes here.
        }

        public void ConfirmOverwrite(RepoItemInfo buttonInfo)
        {
            try {
                Report.Log(ReportLevel.Info, "Mouse", "(Optional Action)\r\nMouse Left Click item 'buttonInfo' at Center.", buttonInfo);
                buttonInfo.FindAdapter<Button>().Click();
            } catch (Exception) {
                Report.Log(ReportLevel.Info, "No confirmation dialog to overwrite confuguration file.");
            }
        }

        public void AddWorkingDirectoryToFileNameIfRelativeFileName()
        {
            if (fileNameToSave.Substring(1,2) != @":\") {
                // fileNameToSave has been declared using relative path 
                fileNameToSave = Directory.GetCurrentDirectory() + "\\" + fileNameToSave;
            }
        }
    }
}
