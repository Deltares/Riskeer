/*
 * Created by Ranorex
 * User: rodriqu_dd
 * Date: 30/11/2020
 * Time: 16:33
 * 
 * To change this template use Tools > Options > Coding > Edit standard headers.
 */
using System;
using System.Globalization;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Threading;
using WinForms = System.Windows.Forms;

using Ranorex;
using Ranorex.Core;
using Ranorex.Core.Testing;

namespace AutomatedSystemTests.Modules.Validation.DocumentView
{
    /// <summary>
    /// Description of ValidateSectionCategoryBoundary.
    /// </summary>
    [TestModule("7DB319A1-061B-489D-A466-CC166AFEB2E0", ModuleType.UserCode, 1)]
    public class ValidateSectionCategoryBoundary : ITestModule
    {
        
        string _expectedCategoriesBoundaries = "";
        [TestVariable("2cf00c13-2878-4943-b748-690262879fa5")]
        public string expectedCategoriesBoundaries
        {
            get { return _expectedCategoriesBoundaries; }
            set { _expectedCategoriesBoundaries = value; }
        }
        
        
        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public ValidateSectionCategoryBoundary()
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
            string categoriesBoundariesDocumentView = GetCategoriesBoundariesFromDocumentView();
            Report.Log(ReportLevel.Info, "Validating Values in Document view");
            ValidateCategoriesBoundaries(expectedCategoriesBoundaries, categoriesBoundariesDocumentView);
            
            string categoriesBoundariesPropertiesPanel = GetCategoriesBoundariesFromPropertiesPanel();
            Report.Log(ReportLevel.Info, "Validating Values in Properties Panel");
            ValidateCategoriesBoundaries(expectedCategoriesBoundaries, categoriesBoundariesPropertiesPanel);
            
        }

        private string GetCategoriesBoundariesFromDocumentView()
        {
            var repo =global::AutomatedSystemTests.AutomatedSystemTestsRepository.Instance;
            var viewCategoryBoundaries = repo.RiskeerMainWindow.ContainerMultipleViews.DocumentViewContainerUncached.ViewCategoryBoundaries;

            Ranorex.Table tableTraject = viewCategoryBoundaries.CategoryBoundariesTraject.Self;
            Ranorex.Table tableVak = viewCategoryBoundaries.CategoryBoundariesSection.Self;
            
            Dictionary<int, Ranorex.Table> dictCategoryTypeTable = new Dictionary<int, Ranorex.Table>{
                {1, tableTraject},
                {2, tableVak}
            };
            
            Dictionary<int, string> dictCategoryType = new Dictionary<int, string>{
                {1, "t"},
                {2, "v"}
            };

            Dictionary<int, string> dictCategoryName = new Dictionary<int, string>{
                {1, "I"},
                {2, "II"},
                {3, "III"},
                {4, "IV"},
                {5, "V"},
                {6, "VI"}
            };
            
            Dictionary<int, string> dictBoundaryType = new Dictionary<int, string>{
                {3, "Onder"},
                {4, "Boven"}
            };
            
            
            string allResults = "";
            // CategoryType: {1, "t"}, {2, "v"}
            for (int idxCategoryType = 1; idxCategoryType < 3; idxCategoryType++) {
                // CategoryName: {1, "I"}, {2, "II"}, {3, "III"},
                //               {4, "IV"}, {5, "V"}, {6, "VI"}
                for (int idxCategoryName = 1; idxCategoryName < 7; idxCategoryName++) {
                    // BoundaryType: {3, "Onder"}, {4, "Boven"}
                    for (int idxBoundaryType = 3; idxBoundaryType < 5; idxBoundaryType++) {
                        var currentCell = dictCategoryTypeTable[idxCategoryType].Rows[idxCategoryName].Cells[idxBoundaryType];
                        currentCell.Select();
                        string valueCell = currentCell.Element.GetAttributeValueText("AccessibleValue");
                        allResults += valueCell + ";";
                    }
                }
            }
            allResults = allResults.TrimEnd(';');
            return allResults;
        }

        private string GetCategoriesBoundariesFromPropertiesPanel()
        {
            AutomatedSystemTestsRepository repo = global::AutomatedSystemTests.AutomatedSystemTestsRepository.Instance;
            Adapter propertiesPanelAdapter = repo.RiskeerMainWindow.ContainerMultipleViews.PropertiesPanelContainer.Table.Self;
            
            var allRows = propertiesPanelAdapter.As<Table>().Rows.ToList();
            string allResults = "";
            foreach (Ranorex.Row row in allRows) {
                string parameterName = row.Element.GetAttributeValueText("AccessibleName");
                if (parameterName.Contains("grens [1/jaar]")) {
                    allResults += row.Element.GetAttributeValueText("AccessibleValue") + ";";
                }
            }
            allResults = allResults.TrimEnd(';');
            return allResults;
        }

        private void ValidateCategoriesBoundaries(string expectedString, string actualString)
        {
            List<string> boundaryTypeCollection= new List<string> {
                "Ondergrens [1/jaar]",
                "Bovengrens [1/jaar]"
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
            
            List<string> expectedValues = expectedString.Split(';').ToList();
            List<string> actualValues = actualString.Split(';').ToList();

            int idx = 0;
            foreach (string categoryType in categoryTypeCollection) {
                foreach (string categoryName in categoryNameCollection) {
                    foreach (string boundaryType in boundaryTypeCollection) {
                        string messageCase = "Validating boundary " + categoryName + categoryType + " " + boundaryType + " : expected = " + expectedValues[idx] + "; actual = " + actualValues[idx]+ "  ";
                        Report.Log(ReportLevel.Info, messageCase);
                        ValidateSingleCategoryBoundary(expectedValues[idx], actualValues[idx]);
                        idx++;
                    }
                }
            }
            expectedCategoriesBoundaries = expectedCategoriesBoundaries.TrimEnd(';');
        }
        
        private void ValidateSingleCategoryBoundary(string expectedValue, string actualValue)
        {
            if (actualValue==expectedValue) {
            	Validate.AreEqual(actualValue, expectedValue);
            }
            else {
            	System.Globalization.CultureInfo currentCulture = CultureInfo.CurrentCulture;
            	Report.Log(ReportLevel.Info, "Validation", "Value found: " + actualValue + " is not equal to expected value: " + expectedValue + "\r\nEvaluating whether they are almost (within 0.01%) equal...");
            	var expectedDouble = 1.0/(Double.Parse(expectedValue.Substring(2,expectedValue.Length-2), currentCulture));
            	var actualDouble = 1.0/(Double.Parse(actualValue.Substring(2,actualValue.Length-2), currentCulture));
            	var deviation = Math.Abs(100.0*(expectedDouble - actualDouble) / expectedDouble);
            	Report.Log(ReportLevel.Info, "Validation", "Deviation = " + deviation + " %");
            	Validate.IsTrue(deviation<0.01);
            }

        }
    }
}
