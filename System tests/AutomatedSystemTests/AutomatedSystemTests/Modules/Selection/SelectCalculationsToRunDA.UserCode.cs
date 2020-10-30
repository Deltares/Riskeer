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

namespace AutomatedSystemTests.Modules.Selection
{
    public partial class SelectCalculationsToRunDA
    {
        /// <summary>
        /// This method gets called right after the recording has been started.
        /// It can be used to execute recording specific initialization code.
        /// </summary>
        private void Init()
        {
            // Your recording specific initialization code goes here.
        }

        public void SelectCalculationsToAddInHydraulicBC_DA(RepoItemInfo tableInfo)
        {
            Report.Log(ReportLevel.Info, "Mouse", "Mouse Left Click item 'cellInfo' at Center when required.");
            int index = Int32.Parse(rowIndex);
            var row = tableInfo.FindAdapter<Table>().Rows[index+1];
            var calculationCell = row.Children[1].As<Cell>();
            ClickOnCheckboxCellIfNeeded(calculationCell, calculationMustBeChecked);
        }
        
        public void ClickOnCheckboxCellIfNeeded(Cell cell, string expectedCheckedState)
        {
        	string currentState = cell.Element.GetAttributeValueText("AccessibleState");
        	var currentlyChecked = currentState.Contains("Checked").ToString().ToLower();
            if (currentlyChecked!=expectedCheckedState) {
        		cell.Click();
            }
        
        }

    }
}
