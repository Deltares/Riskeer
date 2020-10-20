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
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Threading;
using WinForms = System.Windows.Forms;

using Ranorex;
using Ranorex.Core;
using Ranorex.Core.Repository;
using Ranorex.Core.Testing;

namespace AutomatedSystemTests
{
    public partial class SetDetailedAssessmentValueForRow
    {
        /// <summary>
        /// This method gets called right after the recording has been started.
        /// It can be used to execute recording specific initialization code.
        /// </summary>
        private void Init()
        {
            // Your recording specific initialization code goes here.
        }

        public void Set_value_DetailedAsssessmentTypeRowNth(RepoItemInfo cellInfo)
        {
            System.Globalization.CultureInfo fixedDataSourceCulture = new CultureInfo("en-US");
        	fixedDataSourceCulture.NumberFormat.NumberDecimalSeparator = ".";
        	fixedDataSourceCulture.NumberFormat.NumberGroupSeparator = "";
        	System.Globalization.CultureInfo currentCulture = CultureInfo.CurrentCulture;
            assessmentValueToAssign = Double.Parse(assessmentValueToAssign, fixedDataSourceCulture).ToString(currentCulture);
			Report.Log(ReportLevel.Info, "Info", "Value to assign: " + assessmentValueToAssign);
			Report.Log(ReportLevel.Info, "Set value", "Setting attribute AccessibleValue to '$assessmentValueToAssign' on item 'cellInfo'.", cellInfo);
            cellInfo.FindAdapter<Cell>().Element.SetAttributeValue("AccessibleValue", assessmentValueToAssign);
        }

    }
}
