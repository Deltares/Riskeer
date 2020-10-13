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
using System.Globalization;
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
    public partial class ValidateValueParameterForItemInTraject
    {
        /// <summary>
        /// This method gets called right after the recording has been started.
        /// It can be used to execute recording specific initialization code.
        /// </summary>
        private void Init()
        {
            // Your recording specific initialization code goes here.
        }

        public void Validate_GenericParameterVisibleInProjectExplorer(RepoItemInfo rowInfo)
        {
            System.Globalization.CultureInfo currentCulture = CultureInfo.CurrentCulture;
        	Report.Log(ReportLevel.Info, "Validation", "Validating AttributeEqual (AccessibleValue=$expectedValueOfParameterForItemInTraject) on item 'rowInfo'.", rowInfo);
            Report.Log(ReportLevel.Info, "Validation", "Expected value = "+ expectedValueOfParameterForItemInTraject);
            string currentValueOfParameterForItemInTraject = rowInfo.CreateAdapter<Row>(true).GetAttributeValue<string>("AccessibleValue");
            Report.Log(ReportLevel.Info, "Validation", "Current value = "+ currentValueOfParameterForItemInTraject);
            double currentValueDouble = Double.Parse(currentValueOfParameterForItemInTraject, currentCulture);
            double expectedValueDouble = Double.Parse(expectedValueOfParameterForItemInTraject, currentCulture);
            double deviation = Math.Abs(currentValueDouble - expectedValueDouble);
            Validate.AreEqual( deviation <= 0.0000001, true);
            //Validate.AttributeEqual(rowInfo, "AccessibleValue", expectedValueOfParameterForItemInTraject);
        }

    }
}
