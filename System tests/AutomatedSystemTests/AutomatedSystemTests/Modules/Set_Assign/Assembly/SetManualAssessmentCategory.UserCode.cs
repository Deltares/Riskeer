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
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Threading;
using WinForms = System.Windows.Forms;

using Ranorex;
using Ranorex.Core;
using Ranorex.Core.Repository;
using Ranorex.Core.Testing;

namespace AutomatedSystemTests.Modules.Set_Assign.Assembly
{
    public partial class SetManualAssessmentCategory
    {
        /// <summary>
        /// This method gets called right after the recording has been started.
        /// It can be used to execute recording specific initialization code.
        /// </summary>
        private void Init()
        {
            // Your recording specific initialization code goes here.
        }

        public void ScrollRight()
        {
        	if(repo.RiskeerMainWindow.DocumentViewContainer.FailureMechanismResultView.TableDataGridView.ColumnRightInfo.Exists(1000)) {
	            Report.Log(ReportLevel.Info, "Mouse", "Mouse Right Click item 'RiskeerMainWindow.DocumentViewContainer.FailureMechanismResultView.TableDataGridView.ColumnRight' at 10;8.", repo.RiskeerMainWindow.DocumentViewContainer.FailureMechanismResultView.TableDataGridView.ColumnRightInfo, new RecordItemIndex(0));
	            repo.RiskeerMainWindow.DocumentViewContainer.FailureMechanismResultView.TableDataGridView.ColumnRight.Click(System.Windows.Forms.MouseButtons.Right, "10;8");
	            
	            Report.Log(ReportLevel.Info, "Mouse", "Mouse Left Click item 'ContextMenu.PageRight' at 49;7.", repo.ContextMenu.PageRightInfo, new RecordItemIndex(1));
	            repo.ContextMenu.PageRight.Click("49;7");
        	}
        }

        public void ScrollLeft()
        {
        	if(repo.RiskeerMainWindow.DocumentViewContainer.FailureMechanismResultView.TableDataGridView.ColumnLeftInfo.Exists(1000)) {
	            Report.Log(ReportLevel.Info, "Mouse", "Mouse Right Click item 'RiskeerMainWindow.DocumentViewContainer.FailureMechanismResultView.TableDataGridView.ColumnLeft' at 10;7.", repo.RiskeerMainWindow.DocumentViewContainer.FailureMechanismResultView.TableDataGridView.ColumnLeftInfo, new RecordItemIndex(4));
	            repo.RiskeerMainWindow.DocumentViewContainer.FailureMechanismResultView.TableDataGridView.ColumnLeft.Click(System.Windows.Forms.MouseButtons.Right, "10;7");
	            
	            Report.Log(ReportLevel.Info, "Mouse", "Mouse Left Click item 'ContextMenu.PageLeft' at 30;7.", repo.ContextMenu.PageLeftInfo, new RecordItemIndex(5));
	            repo.ContextMenu.PageLeft.Click("30;7");
        	}
        }

    }
}
