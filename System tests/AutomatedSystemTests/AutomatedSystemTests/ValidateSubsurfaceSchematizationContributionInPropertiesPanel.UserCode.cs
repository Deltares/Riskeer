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
    public partial class ValidateSubsurfaceSchematizationContributionInPropertiesPanel
    {
        /// <summary>
        /// This method gets called right after the recording has been started.
        /// It can be used to execute recording specific initialization code.
        /// </summary>
        private void Init()
        {
            // Your recording specific initialization code goes here.
        }

        public void Validate_ContributionPercentage(RepoItemInfo rowInfo)
        {
            System.Globalization.CultureInfo currentCulture = CultureInfo.CurrentCulture;
            System.Globalization.CultureInfo dataCulture = new CultureInfo( "en-US", false );
            dataCulture.NumberFormat.NumberDecimalSeparator = ".";
            dataCulture.NumberFormat.NumberGroupSeparator = "";
        	Report.Log(ReportLevel.Info, "Validation", "Validating AttributeEqual (AccessibleValue=$contributionPercentage) on item 'rowInfo'.", rowInfo);
            
        	var accValue = rowInfo.CreateAdapter<Row>(true).GetAttributeValue<string>("AccessibleValue");
            double accValueNumber = Double.Parse(accValue, currentCulture);
            double contributionValueNumber = Double.Parse(contributionPercentage, dataCulture);
            Report.Log(ReportLevel.Info, "Actual contribution: " + accValue);
            Report.Log(ReportLevel.Info, "Expected contribution: " + contributionPercentage);
            Validate.AreEqual(accValueNumber, contributionValueNumber);
        }

    }
}
