/*
 * Created by Ranorex
 * User: rodriqu_dd
 * Date: 14/12/2020
 * Time: 10:21
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
using Newtonsoft.Json;

using Ranorex;
using Ranorex.Core;
using Ranorex.Core.Testing;

namespace AutomatedSystemTests.Modules.ActionsDocumentView
{
    /// <summary>
    /// Description of ValidateSectionResultAsCombinationContributionsInFMResultView.
    /// </summary>
    [TestModule("B8F1AECF-C5F5-4C4A-BDA3-27D9FFB17F53", ModuleType.UserCode, 1)]
    public class ValidateSectionResultAsCombinationContributionsInFMResultView : ITestModule
    {
        
        string _jsonDataScenariosView = "";
        [TestVariable("ec137b62-f031-4865-9b56-d28ed3a2ceb6")]
        public string jsonDataScenariosView
        {
            get { return _jsonDataScenariosView; }
            set { _jsonDataScenariosView = value; }
        }
        
        
        string _labelFM = "";
        [TestVariable("3052acf7-896a-4e60-8e9e-634849f4cc8d")]
        public string labelFM
        {
            get { return _labelFM; }
            set { _labelFM = value; }
        }
        
        
        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public ValidateSectionResultAsCombinationContributionsInFMResultView()
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
            
            var repo =global::AutomatedSystemTests.AutomatedSystemTestsRepository.Instance;
            
            var dataSectionScenariosView = BuildDataScenariosView(jsonDataScenariosView);
            
            var table = repo.RiskeerMainWindow.ContainerMultipleViews.DocumentViewContainerUncached.FM_ResultView.Table.Self;
            var rows = table.Rows;
            
            var headerRow =rows[0];
            int indexColumnSectionName = GetIndex(headerRow, "Vak");
            int indexColumnCombinedProbability = GetIndexCombinedProbability(headerRow);
            
            rows.RemoveAt(0);
            
            foreach (var dataSection in dataSectionScenariosView) {
                var rowSection = rows[dataSection.IndexSection];
                var cellProb = rowSection.Cells[indexColumnCombinedProbability];
                cellProb.Focus();
                cellProb.Select();
                string actualProb = GetAccessibleValue(cellProb);
                Report.Info("Validating section: " + rowSection.Cells[indexColumnSectionName]);
                
                ValidateSectionResult(dataSection, actualProb);
            }
        }
        
        private int GetIndexCombinedProbability(Ranorex.Row row)
        {
            string queryOldResultTable = "Gedetailleerde toets per vak\r\nfaalkans";
            string queryNewResultTable ="";
            var newResultTableType1 = new List<string>(){"STPH", "STBI"};
            var newResultTableType2 = new List<string>(){"HTKW", "BSKW", "STKWp", "GEKB"};
            if (newResultTableType1.Contains(labelFM)) {
                queryNewResultTable = "mechanisme per doorsnede";
                }
            else if(newResultTableType2.Contains(labelFM)) {
                queryNewResultTable = "mechanisme per vak";
                }
            int indexColumnCombinedProbability = GetIndex(row, queryOldResultTable);
            if (indexColumnCombinedProbability>0) {
                Report.Warn($"Old Result View Windows. Using column with header containing {queryOldResultTable}");
            } else {
                Report.Warn($"New Results View. Using column with header containing '{queryNewResultTable}'");
                indexColumnCombinedProbability = GetIndex(row, queryNewResultTable);
            }
            return indexColumnCombinedProbability;
        }
        private void ValidateSectionResult(DataSectionScenariosView data, string actualProb)
        {
            System.Globalization.CultureInfo currentCulture = CultureInfo.CurrentCulture;

            double expectedSumWeightedProbs = GetExpectedSumWeightedProbs(data, currentCulture);
            string expectedProbNoSeparators = "1/" + ((long) Math.Round(1/expectedSumWeightedProbs)).ToString();
            
            string actualProbNoSeparators = actualProb.Replace(currentCulture.NumberFormat.NumberGroupSeparator, String.Empty);
            
            if (expectedProbNoSeparators==actualProbNoSeparators) {
                Report.Info("Validating if expected probability (" + expectedProbNoSeparators + ") is exactly equal to actual one (" + actualProbNoSeparators + ").");
                Validate.AreEqual(actualProbNoSeparators, expectedProbNoSeparators);
            } else {
                Report.Info("Expected probability (" + expectedProbNoSeparators + "), calculated based on data from Scenarios view, is not exactly equal to actual one (" + actualProbNoSeparators + ").");
                Report.Info("Validating if they are almost equal (within 0.2 %).");
                double relativeDeviation = CalculateRelativeDeviation(actualProbNoSeparators, expectedProbNoSeparators);
                Report.Info("Relative deviation: " + (relativeDeviation * 100).ToString() + "%");
                Validate.IsTrue(relativeDeviation<0.002);
            }
        }
        
        private double CalculateRelativeDeviation(string actualProbFraction, string expectedProbFraction)
        {
            double actualValue = ProbabilityFractionToDouble(actualProbFraction);
            double expectedValue = ProbabilityFractionToDouble(expectedProbFraction);
            return Math.Abs(actualValue-expectedValue) / expectedValue;
        }
        
        private double ProbabilityFractionToDouble(string probabilityFraction)
        {
            return 1.0 / Int64.Parse(probabilityFraction.Substring(2, probabilityFraction.Length-2));
        }
        
        private double GetExpectedSumWeightedProbs(DataSectionScenariosView data, System.Globalization.CultureInfo currentCulture)
        {
            double expectedSumWeightedProbs = 0;
            for (int i = 0; i < data.DataSection.DataScenariosViewList.Count; i++) {
                var currentCalculation = data.DataSection.DataScenariosViewList[i];
                if (currentCalculation.InFinalRating) {
                    string expectedProbNoSeparators = currentCalculation.FailureProbability.Replace(currentCulture.NumberFormat.NumberGroupSeparator, String.Empty);
                    string expectedNumeratorNoSeparators = expectedProbNoSeparators.Substring(2, expectedProbNoSeparators.Length - 2);
                    long expectedNumeratorInt = Int64.Parse(expectedNumeratorNoSeparators);
                    double currentCalcProbability = 1.0 / expectedNumeratorInt;
                    expectedSumWeightedProbs += currentCalcProbability * currentCalculation.Contribution;
                }
            }
            return expectedSumWeightedProbs / 100;
        }
        
        private int GetIndex(Row row, string name)
        {
            return row.Cells.ToList().FindIndex(cl=>GetAccessibleValue(cl).Contains(name));
        }
        
        private string GetAccessibleValue(Cell cl)
        {
            return cl.Element.GetAttributeValueText("AccessibleValue");
        }
        
        private List<DataSectionScenariosView> BuildDataScenariosView(string jsonString)
        {
            List<DataSectionScenariosView> dataFMScenariosView;
            if (jsonString=="") {
                dataFMScenariosView = new List<DataSectionScenariosView>();
            } else {
                var error = false;
                dataFMScenariosView = JsonConvert.DeserializeObject<List<DataSectionScenariosView>>(jsonString, new JsonSerializerSettings
                {
                    Error = (s, e) =>
                    {
                        error = true;
                        e.ErrorContext.Handled = true;
                    }
                }
            );
                if (error==true) {
                    
                    Report.Log(ReportLevel.Error, "error unserializing json string for data scenarios view: " + jsonString);
                }
                
            }
            return dataFMScenariosView;
        }

    }
}
