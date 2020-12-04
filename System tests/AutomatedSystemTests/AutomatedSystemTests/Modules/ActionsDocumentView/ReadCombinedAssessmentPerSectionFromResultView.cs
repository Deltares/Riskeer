/*
 * Created by Ranorex
 * User: rodriqu_dd
 * Date: 04/12/2020
 * Time: 18:23
 * 
 * To change this template use Tools > Options > Coding > Edit standard headers.
 */
using System;
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

namespace AutomatedSystemTests.Modules.ActionsDocumentView
{
    /// <summary>
    /// Description of ReadCombinedAssessmentPerSectionFromResultView.
    /// </summary>
    [TestModule("0D324E02-5DAE-41D7-8EE9-9352A1327E9C", ModuleType.UserCode, 1)]
    public class ReadCombinedAssessmentPerSectionFromResultView : ITestModule
    {
        
        
        
        string _CombinedAssessmentLabelPerSection = "";
        [TestVariable("db1d31cb-58d7-4813-8948-c7f6b242ce1f")]
        public string CombinedAssessmentLabelPerSection
        {
            get { return _CombinedAssessmentLabelPerSection; }
            set { _CombinedAssessmentLabelPerSection = value; }
        }
        
        
        string _CombinedAssessmentProbabilityPerSection = "";
        [TestVariable("d76e12ae-7382-44f9-9ef0-93bac94173a0")]
        public string CombinedAssessmentProbabilityPerSection
        {
            get { return _CombinedAssessmentProbabilityPerSection; }
            set { _CombinedAssessmentProbabilityPerSection = value; }
        }
        
        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public ReadCombinedAssessmentPerSectionFromResultView()
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
            
            var repo = global::AutomatedSystemTests.AutomatedSystemTestsRepository.Instance;
            var tableResults = repo.RiskeerMainWindow.DocumentViewContainerUncached.FM_ResultView.Table.Self.As<Table>();
            var rowsData = tableResults.Rows;
            
            int indexCombinedAssessmentLabel = -1;
            int indexCombinedAssessmentProbability = -1;
            var headerRow = rowsData[0];
            
            for (int i = 0; i < headerRow.Cells.Count; i++) {
                var currentCell = headerRow.Cells[i];
                string currentValue = currentCell.Element.GetAttributeValueText("AccessibleValue");
                if (currentValue=="Toetsoordeel\r\ngecombineerd") {
                    indexCombinedAssessmentLabel = i;
                    Report.Log(ReportLevel.Info, "Combined assessment label found at column " + i.ToString());
                }
                if (currentValue=="Toetsoordeel\r\ngecombineerde\r\nfaalkansschatting") {
                    indexCombinedAssessmentProbability = i;
                    Report.Log(ReportLevel.Info, "Combined assessment probability found at column " + i.ToString());
                }
            }
            
            
            rowsData.RemoveAt(0);
            
            string tableData = "";
            
            foreach (var row in rowsData) {
                var cellsDataInRow = row.Cells.ToList();
                tableData += "[";
                for (int i = 1; i < 4; i++) {
                    var cellCurrent = cellsDataInRow[i];
                    cellCurrent.Select();
                    tableData += cellCurrent.Element.GetAttributeValueText("AccessibleValue") + ";";
                }
                tableData = tableData.TrimEnd(';') + "]*";
            }
            CombinedAssessmentLabelPerSection += tableData.TrimEnd('*') + "|";
        }
    }
}
