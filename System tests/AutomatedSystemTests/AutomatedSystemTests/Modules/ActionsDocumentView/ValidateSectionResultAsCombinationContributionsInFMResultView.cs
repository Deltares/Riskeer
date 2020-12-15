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
            
            var table = repo.RiskeerMainWindow.DocumentViewContainerUncached.FM_ResultView.Table.Self;
            var rows = table.Rows;
            
            var headerRow =rows[0];
            int indexColumnSectionName = GetIndex(headerRow, "Vak");
            int indexColumnCombinedProbability = GetIndex(headerRow, "Gedetailleerde toets per vak\r\nfaalkans");
            rows.RemoveAt(0);
            
            foreach (var dataSection in dataSectionScenariosView) {
                var rowSection = rows[dataSection.IndexSection];
                var cellProb = rowSection.Cells[indexColumnCombinedProbability];
                cellProb.Focus();
                cellProb.Select();
                string actualProb = GetAV(cellProb);
                Report.Info("Validating section: " + rowSection.Cells[indexColumnSectionName]);
                
                ValidateSectionResult(dataSection, actualProb);
            }
        }
        
        private void ValidateSectionResult(DataSectionScenariosView data, string actualProb)
        {
            System.Globalization.CultureInfo currentCulture = CultureInfo.CurrentCulture;
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
            expectedSumWeightedProbs = expectedSumWeightedProbs / 100;
            long numeratorExpectedProb = (long) Math.Round(1/expectedSumWeightedProbs);
            string expectedExactProbFraction = "1/" + numeratorExpectedProb.ToString(currentCulture);
            if (expectedExactProbFraction==actualProb.Replace(currentCulture.NumberFormat.NumberGroupSeparator, String.Empty)) {
                Report.Info("Validating if expected probability (" + expectedExactProbFraction + ") is exactly equal to actual one (" + actualProb.Replace(currentCulture.NumberFormat.NumberGroupSeparator, String.Empty) + ").");
                Validate.AreEqual(actualProb.Replace(currentCulture.NumberFormat.NumberGroupSeparator, String.Empty), expectedExactProbFraction);
            } else {
                Report.Info("Expected probability (" + expectedExactProbFraction + ") is not exactly equal to actual one (" + actualProb + ").");
                string actualProbNoSeparators = actualProb.Replace(currentCulture.NumberFormat.NumberGroupSeparator, String.Empty);
                long actualNumerator = Int64.Parse(actualProbNoSeparators.Substring(2, actualProbNoSeparators.Length-2));
                double actualSumWeightedProbs = 1.0 / actualNumerator;
                double relativeDeviation = Math.Abs(actualSumWeightedProbs-expectedSumWeightedProbs) / expectedSumWeightedProbs;
                Report.Info("Validating if actual probability (" + actualSumWeightedProbs.ToString() + ") and expected probability (" + expectedSumWeightedProbs.ToString() + ") are almost equal (within 0.1 %).");
                Validate.IsTrue(relativeDeviation<0.001);
            }
        }
        
        private int GetIndex(Row row, string name)
        {
            return row.Cells.ToList().FindIndex(cl=>GetAV(cl).Contains(name));
        }
        
        private string GetAV(Cell cl)
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
