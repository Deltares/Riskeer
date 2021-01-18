/*
 * Created by Ranorex
 * User: rodriqu_dd
 * Date: 01/12/2020
 * Time: 09:08
 * 
 * To change this template use Tools > Options > Coding > Edit standard headers.
 */
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
using Ranorex.Core.Testing;

namespace AutomatedSystemTests.Modules.Calculation
{
    /// <summary>
    /// Description of CalculateExpectedCategoriesBoundaries.
    /// </summary>
    [TestModule("CC703AD4-BBC5-4365-A154-3F7C69272854", ModuleType.UserCode, 1)]
    public class CalculateExpectedCategoriesBoundaries : ITestModule
    {
        
        string _expectedCategoriesBoundaries = "";
        [TestVariable("75c8278d-537e-4ed7-93e8-482b42700784")]
        public string expectedCategoriesBoundaries
        {
            get { return _expectedCategoriesBoundaries; }
            set { _expectedCategoriesBoundaries = value; }
        }
        
        string _signalingValueText = "";
        [TestVariable("9729ee64-46b0-4724-8fb5-0622c4848710")]
        public string signalingValueText
        {
            get { return _signalingValueText; }
            set { _signalingValueText = value; }
        }
        
        string _lowerLimitText = "";
        [TestVariable("e245e13a-797a-4e56-9da6-b6f4b788d7a6")]
        public string lowerLimitText
        {
            get { return _lowerLimitText; }
            set { _lowerLimitText = value; }
        }
        
        string _contributionText = "";
        [TestVariable("6a695e78-69fe-4c10-94a2-e36d73f3974a")]
        public string contributionText
        {
            get { return _contributionText; }
            set { _contributionText = value; }
        }
        
        string _parameterNText = "";
        [TestVariable("dcab1944-8539-41ad-b5f4-a46596d4879a")]
        public string parameterNText
        {
            get { return _parameterNText; }
            set { _parameterNText = value; }
        }
        
        
        
        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public CalculateExpectedCategoriesBoundaries()
        {
            // Do not delete - a parameterless constructor is required!
        }

        /// <summary>
        /// Performs the playback of actions in this module.
        /// </summary>
        /// <remarks>You should not call this method directly, instead pass the module
        /// instance to the <see cref="TestModuleRunner.Run(ITestModule)"/> method
        /// that will in turn invoke this method.</remarks>
        void ITestModule.Run()
        {
            Mouse.DefaultMoveTime = 0;
            Keyboard.DefaultKeyPressTime = 0;
            Delay.SpeedFactor = 0.0;
            
            List<string> boundaryTypeCollection= new List<string> {
                "Onder",
                "Boven"
            };
            
            List<string> categoryNameCollection= new List<string> {
                "I",
                "II",
                "III",
                "IV",
                "V",
                "VI"
            };
            
            List<string> categoryTypeCollection= new List<string> {
                "t",
                "v"
            };
            
            expectedCategoriesBoundaries ="";
            
            foreach (string categoryType in categoryTypeCollection) {
                foreach (string categoryName in categoryNameCollection) {
                    foreach (string boundaryType in boundaryTypeCollection) {
                        expectedCategoriesBoundaries += CalculateSingleCategoryBoundary(signalingValueText, lowerLimitText, contributionText, parameterNText, boundaryType, categoryName, categoryType) + ";";
                    }
                }
            }
            expectedCategoriesBoundaries = expectedCategoriesBoundaries.TrimEnd(';');
        }
        
        private string CalculateSingleCategoryBoundary(string signalingValueText, string lowerLimitText, string contributionText, string parameterNText, string boundaryType, string categoryName, string categorySuffix)
        {
        	System.Globalization.CultureInfo currentCulture = CultureInfo.CurrentCulture;
        	string lowerLimitDenominator = lowerLimitText.Substring(2, lowerLimitText.Length-2);
        	double lowerLimitD = 1/Double.Parse(lowerLimitDenominator, currentCulture);
        	string signalingValueDenominator = signalingValueText.Substring(2, signalingValueText.Length-2);
        	double signalingValueD = 1/Double.Parse(signalingValueDenominator, currentCulture);
        	double contributionD = Double.Parse(contributionText, currentCulture);
        	double parameterND = Double.Parse(parameterNText, currentCulture);
        	double boundaryValue;
        	var boundaryToEvaluate = categoryName + boundaryType;
        	
        	string calculatedCategoryBoundary = "";
        	switch (boundaryToEvaluate) {
        		case "IOnder":
        			calculatedCategoryBoundary = "1/Oneindig";
        			Report.Log(ReportLevel.Info, "", categoryName+categorySuffix+boundaryType+ " : " + calculatedCategoryBoundary);
        			return calculatedCategoryBoundary;
        		case "VIBoven":
        			calculatedCategoryBoundary = "1/1";
        			Report.Log(ReportLevel.Info, "", categoryName+categorySuffix+boundaryType+ " : " + calculatedCategoryBoundary);
        			return calculatedCategoryBoundary;
        		case "IIOnder":
        			boundaryValue = (1.0/30.0)*signalingValueD*contributionD/100.0;
        			if (categorySuffix=="v") {
        				boundaryValue = boundaryValue /parameterND;
        			}
        			break;
        		case "IBoven":
        			boundaryValue = (1/30.0)*signalingValueD*contributionD/100.0;
        			if (categorySuffix=="v") {
        				boundaryValue = boundaryValue /parameterND;
        			}
        			break;
        		case "IIIOnder":
        			boundaryValue = signalingValueD*contributionD/100.0;
        			if (categorySuffix=="v") {
        				boundaryValue = boundaryValue /parameterND;
        			}
        			break;
        		case "IIBoven":
        			boundaryValue = signalingValueD*contributionD/100.0;
        			if (categorySuffix=="v") {
        				boundaryValue = boundaryValue /parameterND;
        			}
        			break;
        		case "IVOnder":
        			boundaryValue = lowerLimitD*contributionD/100.0;
        			if (categorySuffix=="v") {
        				boundaryValue = boundaryValue /parameterND;
        			}
        			break;
        		case "IIIBoven":
        			boundaryValue = lowerLimitD*contributionD/100.0;
        			if (categorySuffix=="v") {
        				boundaryValue = boundaryValue /parameterND;
        			}
        			break;
        		case "VOnder":
        			boundaryValue = lowerLimitD;
        			break;
        		case "IVBoven":
        			boundaryValue = lowerLimitD;
        			break;
        		case "VIOnder":
        			boundaryValue = 30.0*lowerLimitD;
        			break;
        		case "VBoven":
        			boundaryValue = 30.0*lowerLimitD;
        			break;
        		default:
        			Report.Log(ReportLevel.Info,"", "case : " + boundaryToEvaluate + "error!");
        			throw new Exception();
        	}
        	var denominatorBoundary = Math.Round(1/boundaryValue);
        	calculatedCategoryBoundary = "1/" + denominatorBoundary.ToString("N0", currentCulture.NumberFormat);
        	Report.Log(ReportLevel.Info, "", categoryName+categorySuffix+boundaryType+ " : " + calculatedCategoryBoundary);
        	return calculatedCategoryBoundary;
        }
    }
}
