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
    public partial class CalculateValueParameterN
    {
        /// <summary>
        /// This method gets called right after the recording has been started.
        /// It can be used to execute recording specific initialization code.
        /// </summary>
        private void Init()
        {
            // Your recording specific initialization code goes here.
        }

        public void CalculateValueNFromFMParameters()
        {
            System.Globalization.CultureInfo currentCulture = CultureInfo.CurrentCulture;
            System.Globalization.CultureInfo dataCulture = new CultureInfo( "en-US", false );
            dataCulture.NumberFormat.NumberDecimalSeparator = ".";
            dataCulture.NumberFormat.NumberGroupSeparator = "";
        	
            var listformulaAB = new List<string>() {"STPH", "STBI"};
            
            if (listformulaAB.IndexOf(fmLabel)!=-1) {
            	// Calculate N based on formula with a, b
            	nameOfParameterInPropertiesPanel = "a [-]";
            	var a  = repo.RiskeerMainWindow.PropertiesPanelContainer.Table.GenericParameterVisibleInProjectExplorer.Element.GetAttributeValueText("AccessibleValue");
            	Report.Log(ReportLevel.Info, "", "Value of a: " + a);
            	
            	nameOfParameterInPropertiesPanel = "b [m]";
            	var b  = repo.RiskeerMainWindow.PropertiesPanelContainer.Table.GenericParameterVisibleInProjectExplorer.Element.GetAttributeValueText("AccessibleValue");
            	Report.Log(ReportLevel.Info, "", "Value of b: " + b);
            	
            	nameOfParameterInPropertiesPanel = "Lengte* [m]";
            	var length  = repo.RiskeerMainWindow.PropertiesPanelContainer.Table.GenericParameterVisibleInProjectExplorer.Element.GetAttributeValueText("AccessibleValue");
            	Report.Log(ReportLevel.Info, "", "Value of Length: " + length);
            	
            	double aDouble = Double.Parse(a, currentCulture);
            	double bDouble = Double.Parse(b, currentCulture);
            	double lengthDouble = Double.Parse(length, currentCulture);
            	
            	double parameterN = 1 + aDouble * lengthDouble / bDouble;
            	
            	valueOfParameterInPropertiesPanel = parameterN.ToString(currentCulture);
            } else if (fmLabel == "BSKW") {
            	// Calculate N based on formula with 2NA, C
            	nameOfParameterInPropertiesPanel = "2NA [-]";
            	var twoNA  = repo.RiskeerMainWindow.PropertiesPanelContainer.Table.GenericParameterVisibleInProjectExplorer.Element.GetAttributeValueText("AccessibleValue");
            	Report.Log(ReportLevel.Info, "", "Value of 2NA: " + twoNA);
            	
            	nameOfParameterInPropertiesPanel = "C [-]";
            	var C  = repo.RiskeerMainWindow.PropertiesPanelContainer.Table.GenericParameterVisibleInProjectExplorer.Element.GetAttributeValueText("AccessibleValue");
            	Report.Log(ReportLevel.Info, "", "Value of C: " + C);
            	
            	double twoNADouble = Double.Parse(twoNA, currentCulture);
            	double CDouble = Double.Parse(C, currentCulture);
            	
            	double parameterN = Math.Max(twoNADouble * CDouble, 1.0);
            	
            	valueOfParameterInPropertiesPanel = parameterN.ToString(currentCulture);
            		
            } else if (fmLabel == "AGK") {
            	// Calculate N based on formula with DeltaL
            	nameOfParameterInPropertiesPanel = "ΔL [m]";
            	var deltaL  = repo.RiskeerMainWindow.PropertiesPanelContainer.Table.GenericParameterVisibleInProjectExplorer.Element.GetAttributeValueText("AccessibleValue");
            	Report.Log(ReportLevel.Info, "", "Value of ΔL: " + deltaL);
            	
            	nameOfParameterInPropertiesPanel = "Lengte* [m]";
            	var length  = repo.RiskeerMainWindow.PropertiesPanelContainer.Table.GenericParameterVisibleInProjectExplorer.Element.GetAttributeValueText("AccessibleValue");
            	Report.Log(ReportLevel.Info, "", "Value of Length: " + length);
            	
            	double deltaLDouble = Double.Parse(deltaL, currentCulture);
            	double lengthDouble = Double.Parse(length, currentCulture);
            	
            	double parameterN = lengthDouble/deltaLDouble;
            	valueOfParameterInPropertiesPanel = parameterN.ToString(currentCulture);
            } else {
            	nameOfParameterInPropertiesPanel = "N [-]";
            	valueOfParameterInPropertiesPanel = repo.RiskeerMainWindow.PropertiesPanelContainer.Table.GenericParameterVisibleInProjectExplorer.Element.GetAttributeValueText("AccessibleValue");
            }
            
            Report.Log(ReportLevel.Info, "", "Value of N: " + valueOfParameterInPropertiesPanel);
        }
    }
}
