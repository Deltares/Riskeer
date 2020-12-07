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
            Mouse.DefaultMoveTime = 0;
            Keyboard.DefaultKeyPressTime = 0;
            Delay.SpeedFactor = 0.0;
            System.Globalization.CultureInfo currentCulture = CultureInfo.CurrentCulture;

            var trajectAssessmentInformation = BuildAssessmenTrajectInformation(trajectAssessmentInformationString);
            var repo = global::AutomatedSystemTests.AutomatedSystemTestsRepository.Instance;
            
            var table = repo.RiskeerMainWindow.DocumentViewContainerUncached.AssemblySectionsView.Table;
            List<double> allSubsections = new List<double>();
            allSubsections.Add(0);
            foreach (var fmTrjAssInfo in trajectAssessmentInformation.ListFMsAssessmentInformation) {
                foreach (var section in fmTrjAssInfo.SectionList) {
                    allSubsections.Add(section.EndDistance);
                }
            }
            allSubsections.Sort();
            var allSubsectionsUnique = allSubsections.Distinct().ToList();
            var rowsToIterate = table.Rows.ToList();
            var headerRow = rowsToIterate[0];
            
            List<string> fmsToValidate = trajectAssessmentInformation.ListFMsAssessmentInformation.Select(it=>it.Label).ToList();
            int indexSectionNumber       = GetIndex(headerRow, "Vaknummer");
            int indexDistanceStart       = GetIndex(headerRow, "van* [m]");
            int indexDistanceEnd         = GetIndex(headerRow, "tot* [m]");
            int indexCombinedAssessment  = GetIndex(headerRow, "Gecombineerd");
            List<int> indecesColumnsFMsToValidate = fmsToValidate.Select(fmLabel=>GetIndex(headerRow, fmLabel)).ToList();
            
            Dictionary<string,int> dicAssemblyLabels = new Dictionary<string, int> {
                {"-",    0},
                {"Iv",   1},
                {"IIv",  2},
                {"IIIv", 3},
                {"IVv",  4},
                {"Vv",   5},
                {"VIv",  6},
                {"VIIv", 7}
            };
            
            rowsToIterate.RemoveAt(0);
            int indexRow = 0;
            foreach (var row in rowsToIterate) {
                var cells = row.Cells.ToList();
                double expectedDistanceStart = allSubsectionsUnique[indexRow];
                double expectedDistanceEnd   = allSubsectionsUnique[indexRow + 1];
                double expectedDistanceMiddle = (expectedDistanceStart + expectedDistanceEnd)/2.0;
                string CombinedAssessmentSectionLabel = "";
                int worstFMLabel = 0;
                for (int i = 0; i < fmsToValidate.Count; i++) {
                    var currentFMAssInfo = trajectAssessmentInformation.ListFMsAssessmentInformation[i];
                    var expectedFMAssessmentLabel = GetAssessmentLabelForDistance(currentFMAssInfo, expectedDistanceMiddle);
                    if (dicAssemblyLabels[expectedFMAssessmentLabel]>worstFMLabel) {
                        worstFMLabel = dicAssemblyLabels[expectedFMAssessmentLabel];
                        CombinedAssessmentSectionLabel = expectedFMAssessmentLabel;
                    }
                }
                Report.Info("Validation for row " + (indexRow + 1).ToString() + ".");
                ValidateCell(cells[indexSectionNumber], (indexRow + 1).ToString(), "Validation section number.");
                ValidateCell(cells[indexDistanceStart], expectedDistanceStart.ToString("N2"), "Validation section start");
                ValidateCell(cells[indexDistanceEnd], expectedDistanceEnd.ToString("N2"), "Validation section end");
                ValidateCell(cells[indexCombinedAssessment], CombinedAssessmentSectionLabel, "Validation combined assessment label.");
                
                
                for (int i = 0; i < fmsToValidate.Count; i++) {
                    var currentFMAssInfo = trajectAssessmentInformation.ListFMsAssessmentInformation[i];
                    var expectedFMAssessmentLabel = GetAssessmentLabelForDistance(currentFMAssInfo, expectedDistanceMiddle);
                    
                    ValidateCell(cells[indecesColumnsFMsToValidate[i]], expectedFMAssessmentLabel, "Validation assessment label FM " + fmsToValidate[i]);
                }
                indexRow++;
            }
        }
        
        private void ValidateCell(Cell cell, string expectedValue, string message) 
        {
            Report.Info(message);
            cell.Select();
            string actualValue = GetAV(cell);
            Validate.AreEqual(actualValue, expectedValue);
        }
        
        private string GetAssessmentLabelForDistance(FailureMechanismAssessmentInformation fmAssInfo, double distance)
        {
            var endSections = fmAssInfo.SectionList.Select(it=>it.EndDistance).ToList();
            int index = endSections.FindIndex(it=> distance<it);
            var label = fmAssInfo.SectionList[index].CombinedAssessmentLabel;
            return label;
        }
        
        private TrajectAssessmentInformation BuildAssessmenTrajectInformation(string trajectAssessmentInformationString)
        {
            TrajectAssessmentInformation trajectAssessmentInformation;
            if (trajectAssessmentInformationString=="") {
                trajectAssessmentInformation = new TrajectAssessmentInformation();
            } else {
                var error = false;
                trajectAssessmentInformation = JsonConvert.DeserializeObject<TrajectAssessmentInformation>(trajectAssessmentInformationString, new JsonSerializerSettings
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
        
        private string GetAV(Cell cl)
        {
            return cl.Element.GetAttributeValueText("AccessibleValue");
        }
        
        private int GetIndex(Row row, string name)
        {
            return row.Cells.ToList().FindIndex(cl=>GetAV(cl).Contains(name));
        }

    }
}
