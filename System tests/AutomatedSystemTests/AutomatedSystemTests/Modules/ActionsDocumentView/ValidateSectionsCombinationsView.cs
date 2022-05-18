/*
 * Created by Ranorex
 * User: rodriqu_dd
 * Date: 06/12/2020
 * Time: 18:44
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
using Ranorex_Automation_Helpers.UserCodeCollections;

using Ranorex;
using Ranorex.Core;
using Ranorex.Core.Testing;

namespace AutomatedSystemTests.Modules.ActionsDocumentView
{
    /// <summary>
    /// Description of ValidateSectionsCombinationsView.
    /// </summary>
    [TestModule("4AEF939A-35A6-4173-A921-C521453B760A", ModuleType.UserCode, 1)]
    public class ValidateSectionsCombinationsView : ITestModule
    {
        string _trajectAssessmentInformationString = "";
        [TestVariable("5dda9c28-3af1-49bc-b8d6-81c994d32f4d")]
        public string trajectAssessmentInformationString
        {
            get { return _trajectAssessmentInformationString; }
            set { _trajectAssessmentInformationString = value; }
        }
        
        private TrajectResultInformation trajectResultInformation {get; set;}
        
        private List<double> allSubsections {get; set;}
        
        private List<string> listAllLabelsFMs {get; set;}
        
        private Dictionary<string,int> dictCategoriesLabels = new Dictionary<string, int> {
                {"-III",   0},
                {"-II",    1},
                {"-I",     2},
                {"0",      3},
                {"+I",     4},
                {"+II",    5},
                {"+III",   6},
                {"-",      7},
                {"NR",     8}
            };
        
        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public ValidateSectionsCombinationsView()
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
            this.trajectResultInformation = TrajectResultInformation.BuildAssessmenTrajectInformation(trajectAssessmentInformationString);
            this.allSubsections = CreateSubsections(trajectResultInformation.ListFMsResultInformation);
            var rowsTable = AutomatedSystemTests.AutomatedSystemTestsRepository.Instance.RiskeerMainWindow.ContainerMultipleViews.DocumentViewContainer.AssemblySectionsView.Table.Self.Rows;
            this.listAllLabelsFMs = rowsTable[0].Cells.Select(cell=>cell.Text).Where(txt=>txt!="").Where(txt=>!txt.Contains("Metrering")).Where(txt=>!txt.Contains("Slechtste duidingsklasse")).ToList();
            rowsTable.RemoveAt(0);
            int indexRow = 0;
            foreach (var row in rowsTable) {
                ValidateRowSectionsCombinations(row, indexRow);
                indexRow++;
            }
        }
        
        private void ValidateRowSectionsCombinations(Row row, int rowIndex)
        {
            ValidateStartDistanceSubsection(row, rowIndex);
            ValidateEndDistanceSubsection(row, rowIndex);
            double distanceMiddlePoint = MiddlePoint(row.Cells[1].Text, row.Cells[2].Text);
            int cellIndex = 3;
            foreach (string labelFM in this.listAllLabelsFMs) {
                ValidateCategoryFMInSubsection(row, rowIndex, cellIndex, distanceMiddlePoint, labelFM);
                cellIndex++;
            }
            ValidateCombinedCategorySubsection(row, rowIndex);
        }
        
        private void ValidateStartDistanceSubsection(Row row, int rowIndex)
        {
            ValidateCell(row.Cells[1], string.Format("{0:0.00}",this.allSubsections[rowIndex]), "Validation subsection start distance measured along reference line " + " at row = " + (rowIndex+1).ToString() + ".");
        }
        
        private void ValidateEndDistanceSubsection(Row row, int rowIndex)
        {
            ValidateCell(row.Cells[2], string.Format("{0:0.00}",this.allSubsections[rowIndex + 1]), "Validation subsection end distance measured along reference line " + " at row = " + (rowIndex+1).ToString() + ".");
        }
        
        private void ValidateCategoryFMInSubsection(Row row, int rowIndex, int cellIndex, double distance, string labelFM)
        {
            var currentFMResultInfo = this.trajectResultInformation.ListFMsResultInformation.Where(fm=>fm.Label==labelFM).FirstOrDefault();
            var expectedCategory = currentFMResultInfo==null? "-" : GetCategoryAt(distance, currentFMResultInfo);
            ValidateCell(row.Cells[cellIndex], expectedCategory, "Validation categopry for FM = " + labelFM + " at row = " + (rowIndex+1).ToString() + ".");
        }
        
        private void ValidateCombinedCategorySubsection(Row row, int rowIndex)
        {
            ValidateCell(row.Cells.Last(), WorstCategoryClass(row), "Validation worst category class " + " at row = " + (rowIndex+1).ToString() + ".");
        }
        
        private string WorstCategoryClass(Row row)
        {
            var fmCells = row.Cells.Select(cl=>cl.Text).ToList().GetRange(3, row.Cells.Count-3);
            var worstCategoryValue = fmCells.Select(category=> this.dictCategoriesLabels[category]).Min();
            var worstCategoryClass = this.dictCategoriesLabels.FirstOrDefault(x=>x.Value==worstCategoryValue).Key;
            return worstCategoryClass;
        }
        
        private List<double> CreateSubsections(List<FailureMechanismResultInformation> listFMsResultInfo)
        {
            var listSubsections = new List<double>();
            listSubsections.Add(0);
            foreach (var fmResultInfo in listFMsResultInfo) {
                foreach (var section in fmResultInfo.SectionList) {
                    listSubsections.Add(section.EndDistance);
                }
            }
            listSubsections.Sort();
            return listSubsections.Distinct().ToList();
        }
        
        private void ValidateCell(Cell cell, string expectedValue, string message)
        {
            Report.Info(message);
            cell.Focus();
            cell.Select();
            string actualValue = cell.Element.GetAttributeValueText("AccessibleValue");
            Validate.AreEqual(actualValue, expectedValue);
        }
        
        
        private double MiddlePoint(string distance1, string distance2)
        {
            double d1 = Double.Parse(distance1);
            double d2 = Double.Parse(distance2);
            return (d1+d2)/2.0;
        }
        
        private string GetCategoryAt(double distance, FailureMechanismResultInformation fmResultInfo)
        {
            var endSections = fmResultInfo.SectionList.Select(it=>it.EndDistance).ToList();
            int index = endSections.FindIndex(it=> distance<it);
            var label = fmResultInfo.SectionList[index].AssemblyGroup;
            return label;
        }
    }
}
