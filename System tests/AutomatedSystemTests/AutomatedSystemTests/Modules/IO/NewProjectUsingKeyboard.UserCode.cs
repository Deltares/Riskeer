﻿///////////////////////////////////////////////////////////////////////////////
//
// This file was automatically generated by RANOREX.
// Your custom recording code should go in this file.
// The designer will only add methods to this file, so your custom code won't be overwritten.
// http://www.ranorex.com
//
///////////////////////////////////////////////////////////////////////////////

using System;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Threading;
using WinForms = System.Windows.Forms;
using Ranorex;
using Ranorex.Core;
using Ranorex.Core.Repository;
using Ranorex.Core.Testing;
using Ranorex_Automation_Helpers.UserCodeCollections;

namespace AutomatedSystemTests.Modules.IO
{
    public partial class NewProjectUsingKeyboard
    {
        /// <summary>
        /// This method gets called right after the recording has been started.
        /// It can be used to execute recording specific initialization code.
        /// </summary>
        private void Init()
        {
            // Your recording specific initialization code goes here.
        }

        public void Mouse_Click_ButtonNoIfConformationDialogAppears(RepoItemInfo buttonInfo)
        {
            Report.Log(ReportLevel.Info, "Mouse", "Mouse Left Click item 'buttonInfo'.", buttonInfo);
            try {
                buttonInfo.WaitForExists(2000);
            	buttonInfo.FindAdapter<Button>().Click();
            } catch(Exception) { }
            
        }

        public void DoNotSaveIfAsked(RepoItemInfo buttonInfo)
        {
            if (buttonInfo.Exists(Duration.FromMilliseconds(1000))) {
                Report.Log(ReportLevel.Info, "Mouse", "(Optional Action)\r\nMouse Left Click item 'buttonInfo' at Center.", buttonInfo);
                buttonInfo.FindAdapter<Button>().Click();
                }
        }

        public void FocusAndSelectTrajectIdCell(RepoItemInfo trajectsTableInfo)
        {
            var rowToSelect = trajectsTableInfo.CreateAdapter<Table>(true).
                Rows.Where(rw=>rw.GetAttributeValue<string>("AccessibleValue").ToString().StartsWith(trajectID)).First();
            rowToSelect.Focus();
            rowToSelect.Select();
            signallingValue = rowToSelect.Cells[2].Text.ToNoGroupSeparator();
            lowLimitValue = rowToSelect.Cells[3].Text.ToNoGroupSeparator();
        }

    }
}
