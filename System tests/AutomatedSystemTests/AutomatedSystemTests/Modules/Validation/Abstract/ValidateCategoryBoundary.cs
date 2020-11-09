/*
 * Created by Ranorex
 * User: lubbers
 * Date: 9-11-2020
 * Time: 11:06
 * 
 * To change this template use Tools > Options > Coding > Edit standard headers.
 */
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

namespace AutomatedSystemTests.Modules.Validation.Abstract
{
    /// <summary>
    /// Creates a Ranorex user code collection. A collection is used to publish user code methods to the user code library.
    /// </summary>
    [UserCodeCollection]
    public class ValidateCategoryBoundary
    {
        // You can use the "Insert New User Code Method" functionality from the context menu,
        // to add a new method with the attribute [UserCodeMethod].
        
        [UserCodeMethod]
        public static void Validate_GenericCategoryBoundaryCellAlmostEqual(RepoItemInfo cellInfo, string expectedBoundary)
        {
            Report.Log(ReportLevel.Info, "Validation", "(Optional Action)\r\nValidating AttributeEqual (AccessibleValue=$expectedBoundary) on item 'cellInfo'.", cellInfo);
            var currentValue = cellInfo.CreateAdapter<Cell>(true).GetAttributeValue<string>("AccessibleValue");
            if (currentValue==expectedBoundary) {
            	Validate.AttributeEqual(cellInfo, "AccessibleValue", expectedBoundary);
            }
            else {
            	System.Globalization.CultureInfo currentCulture = CultureInfo.CurrentCulture;
            	Report.Log(ReportLevel.Info, "Validation", "Value found: " + currentValue + " is not equal to expected value: " + expectedBoundary + "\r\nEvaluating whether they are almost (within 0.01%) equal...");
            	var expectedDouble = 1.0/(Double.Parse(expectedBoundary.Substring(2,expectedBoundary.Length-2), currentCulture));
            	var currentDouble = 1.0/(Double.Parse(currentValue.Substring(2,currentValue.Length-2), currentCulture));
            	var deviation = Math.Abs(100.0*(expectedDouble - currentDouble) / expectedDouble);
            	Report.Log(ReportLevel.Info, "Validation", "Deviation = " + deviation + " %");
            	Validate.IsTrue(deviation<0.01);
            }
        }
    }
}
