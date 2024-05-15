﻿/*
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
using Newtonsoft.Json;
using Ranorex_Automation_Helpers.UserCodeCollections;

using Ranorex;
using Ranorex.Core;
using Ranorex.Core.Testing;

namespace AutomatedSystemTests.Modules.ActionsDocumentView
{
    /// <summary>
    /// Description of ReadCombinedAssessmentPerSectionFromResultView.
    /// </summary>
    [TestModule("0D324E02-5DAE-41D7-8EE9-9352A1327E9C", ModuleType.UserCode, 1)]
    public class ReadResultsforFMFromResultView : ITestModule
    {
        
        string _trajectAssessmentInformationString = "";
        [TestVariable("9ac12269-a27c-4f0c-9b29-11857490bb77")]
        public string trajectAssessmentInformationString
        {
            get { return _trajectAssessmentInformationString; }
            set { _trajectAssessmentInformationString = value; }
        }
        
        
        string _labelFM = "";
        [TestVariable("37288c18-36a9-4f7e-840d-7627a311f6a8")]
        public string labelFM
        {
            get { return _labelFM; }
            set { _labelFM = value; }
        }
        
        
        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public ReadResultsforFMFromResultView()
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
            var trajectResultInformation = BuildAssessmenTrajectInformation(trajectAssessmentInformationString);

            var repo = global::AutomatedSystemTests.AutomatedSystemTestsRepository.Instance;
            var tableResults = repo.RiskeerMainWindow.ContainerMultipleViews.DocumentViewContainer.FM_ResultView.TableFMResultView.Self.As<Table>();
            var rowsData = tableResults.Rows;
            var rowHeader = rowsData[0];
            var sectionColumnIndeces = GetColumnIndecesResultView(rowsData[0]);

            var currentFMResultInformation = trajectResultInformation.ListFMsResultInformation.Where(fmItem=>fmItem.Label==labelFM).FirstOrDefault();

            int rowIndex = 0;
            rowsData.RemoveAt(0);
            foreach (var row in rowsData) {
                var cellsDataInRow = row.Cells.ToList();
                currentFMResultInformation.SectionList[rowIndex].CalculationFailureProbPerSection = GetAccValue(cellsDataInRow[sectionColumnIndeces[0]]);
                currentFMResultInformation.SectionList[rowIndex].AssemblyGroup = GetAccValue(cellsDataInRow[sectionColumnIndeces[1]]);
                rowIndex++;
            }
            
            var resultView = repo.RiskeerMainWindow.ContainerMultipleViews.DocumentViewContainer.FM_ResultView;
            currentFMResultInformation.FailureProbability = resultView.FailureProbabilityFM.TextValue;
            trajectAssessmentInformationString = JsonConvert.SerializeObject(trajectResultInformation, Formatting.Indented);
            Report.Log(ReportLevel.Info, trajectAssessmentInformationString);
        }

        private TrajectResultInformation BuildAssessmenTrajectInformation(string trajectAssessmentInformationString)
        {
            TrajectResultInformation trajectAssessmentInformation;
            if (trajectAssessmentInformationString=="") {
                trajectAssessmentInformation = new TrajectResultInformation();
            } else {
                var error = false;
                trajectAssessmentInformation = JsonConvert.DeserializeObject<TrajectResultInformation>(trajectAssessmentInformationString, new JsonSerializerSettings
                {
                    Error = (s, e) =>
                    {
                        error = true;
                        e.ErrorContext.Handled = true;
                    }
                }
            );
                if (error==true) {
                    
                    Report.Log(ReportLevel.Error, "error unserializing json string for trajectAssessmentInformationString: " + trajectAssessmentInformationString);
                }
                
            }
            return trajectAssessmentInformation;
        }

        private int GetColumnIndex(Row headerRow, string textInHeader)
        {
            return headerRow.Cells.ToList().FindIndex(c => c.Element.GetAttributeValueText("AccessibleValue") == textInHeader);
        }

        private List<int> GetColumnIndecesResultView(Row headerRow)
        {
            int indexCalculationFailureProbPerSection = GetColumnIndex(headerRow, "Rekenwaarde\r\nfaalkans per vak\r\n[1/jaar]");
            int indexAssemblyGroup = GetColumnIndex(headerRow, "Duidingsklasse");
            return new List<int>{indexCalculationFailureProbPerSection, indexAssemblyGroup};
        }

        private string GetAccValue(Cell cell)
        {
            cell.Select();
            return cell.Element.GetAttributeValueText("AccessibleValue");
        }
    }
}
