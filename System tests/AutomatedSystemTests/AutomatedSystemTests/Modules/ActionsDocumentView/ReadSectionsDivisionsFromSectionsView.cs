/*
 * Created by Ranorex
 * User: rodriqu_dd
 * Date: 04/12/2020
 * Time: 17:48
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
    /// Description of ReadSectionsDivisionsFromSectionsView.
    /// </summary>
    [TestModule("F63ED64A-DB92-435D-A38E-21A7DC8E6F54", ModuleType.UserCode, 1)]
    public class ReadSectionsDivisionsFromSectionsView : ITestModule
    {
        
        
        string _trajectAssessmentInformation = "";
        [TestVariable("b4271fc1-5724-4698-85e2-3434ee79f78f")]
        public string trajectAssessmentInformationString
        {
            get { return _trajectAssessmentInformation; }
            set { _trajectAssessmentInformation = value; }
        }
        
        
        string _labelFM = "";
        [TestVariable("f6ff4e12-a192-49bd-9f9e-85c8899a2f7b")]
        public string labelFM
        {
            get { return _labelFM; }
            set { _labelFM = value; }
        }
        
        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public ReadSectionsDivisionsFromSectionsView()
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
            Delay.SpeedFactor = 0;
            
            var  fmAssessmentInformation = new FailureMechanismResultInformation();
            fmAssessmentInformation.Label = labelFM;

            var repo = global::AutomatedSystemTests.AutomatedSystemTestsRepository.Instance;
            var rowsSectionsDivisions = repo.RiskeerMainWindow.ContainerMultipleViews.DocumentViewContainer.FMSectionsViewTable.Rows;
            var sectionIndeces = GetColumnIndecesSectionsView(rowsSectionsDivisions[0]);
            rowsSectionsDivisions.RemoveAt(0);
            foreach (var row in rowsSectionsDivisions) {
                fmAssessmentInformation.SectionList.Add(CreateSectionFromRow(row, sectionIndeces));
            }
            var trajectAssessmentInformation = TrajectResultInformation.BuildAssessmenTrajectInformation(trajectAssessmentInformationString);
            trajectAssessmentInformation.ListFMsResultInformation.Add(fmAssessmentInformation);
            trajectAssessmentInformationString = JsonConvert.SerializeObject(trajectAssessmentInformation, Formatting.Indented);
        }
        
        
        private int GetColumnIndex(Row headerRow, string textInHeader)
        {
            return headerRow.Cells.ToList().FindIndex(c => c.Element.GetAttributeValueText("AccessibleValue") == textInHeader);
        }
        
        private string GetAccValue(Cell cell)
        {
            cell.Select();
            return cell.Element.GetAttributeValueText("AccessibleValue");
        }
        
        private double GetDoubleAccValue(Cell cell)
        {
            return Double.Parse(GetAccValue(cell));
        }
        
        private Section CreateSectionFromRow(Row row, List<int> sectionIndeces)
        {
            var cellsInRow = row.Cells.ToList();
            Section section = new Section();
            section.Name = GetAccValue(cellsInRow[sectionIndeces[0]]);
            section.StartDistance = GetDoubleAccValue(cellsInRow[sectionIndeces[1]]);
            section.EndDistance = GetDoubleAccValue(cellsInRow[sectionIndeces[2]]);
            section.Length = GetDoubleAccValue(cellsInRow[sectionIndeces[3]]);
            if (sectionIndeces[4]!=-1) {
                section.Nvak = GetDoubleAccValue(cellsInRow[sectionIndeces[4]]);
            }
            return section;
        }
        
        private List<int> GetColumnIndecesSectionsView(Row headerRow)
        {
            int indexName = GetColumnIndex(headerRow, "Vaknaam");
            int indexStartDistance = GetColumnIndex(headerRow, "Metrering van* [m]");
            int indexEndDistance = GetColumnIndex(headerRow, "Metrering tot* [m]");
            int indexLength = GetColumnIndex(headerRow, "Lengte* [m]");
            int indexNVak = GetColumnIndex(headerRow, "Nvak* [-]");
            return new List<int>{indexName, indexStartDistance, indexEndDistance, indexLength, indexNVak};
        }
    }
}
